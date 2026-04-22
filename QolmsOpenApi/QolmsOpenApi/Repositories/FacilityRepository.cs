using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.QolmsOpenApi.Worker;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Repositories
{
    /// <summary>
    /// 施設情報の入出力インターフェース
    /// TODO: 1000行近くなってきたら薬局と病院で分けることを検討する
    /// </summary>
    public interface IFacilityRepository
    {
        /// <summary>
        /// 位置情報から薬局検索
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="now"></param>
        /// <param name="isContractedFacilityOnly"></param>
        /// <param name="startDistance"></param>
        /// <param name="endDistance"></param>
        /// <returns></returns>
        List<DbFacilityItem> SearchPharmacyByLocation(Guid accountKey, decimal latitude, decimal longitude, DateTime now, bool isContractedFacilityOnly, int startDistance, int endDistance);

        /// <summary>
        /// Filter項目で薬局検索して結果を返す。
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="searchText"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="prefectureNo"></param>
        /// <param name="cityNo"></param>
        /// <param name="now"></param>
        /// <param name="isContractedFacilityOnly"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        (List<DbFacilityItem> facilityN, int pageIndex, int maxPageIndex, int totalCount) SearchPharmacyByFiltering(Guid accountKey, string searchText,
            decimal latitude, decimal longitude, byte prefectureNo, int cityNo, DateTime now, bool isContractedFacilityOnly,
            int pageIndex, int pagesize);

        /// <summary>
        /// 施設情報を取得
        /// </summary>
        /// <param name="facilityKey"></param>
        /// <param name="accountKey"></param>
        /// <returns></returns>
        DbFacilityItem ReadFacility(Guid facilityKey, Guid accountKey);

        /// <summary>
        /// 主キーで施設情報を取得
        /// </summary>
        /// <param name="facilityKey"></param>
        /// <returns></returns>
        QH_FACILITY_MST ReadFacility(Guid facilityKey);

        /// <summary>
        /// 施設画像のキーを取得
        /// </summary>
        /// <param name="facilityKey"></param>
        /// <returns></returns>
        List<QoApiFileKeyItem> GetFacilityImageKey(Guid facilityKey);

        /// <summary>
        /// 指定日が祝日か否かを祝日マスタを参照して返す
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        bool IsHoliday(DateTime date);

        /// <summary>
        /// 医療機関コードから対応する施設情報を返す
        /// </summary>
        /// <param name="medicalFacilityCode">医療機関コード</param>
        /// <returns></returns>
        QH_FACILITY_MST ReadFacilityFromMedicalFacilityCode(string medicalFacilityCode);

        /// <summary>
        /// 医療機関リストを連携システム番号の関連施設で抽出する。
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <param name="updatedDate"></param>
        /// <returns></returns>
        List<QH_FACILITY_ALL_VIEW> ReadMedicalFacilityListByLinkageSystemNo(int linkageSystemNo, DateTime updatedDate);

        /// <summary>
        /// 医療機関リストを施設キーで抽出する。
        /// </summary>
        /// <param name="updatedDate"></param>
        /// <param name="facilityKeys"></param>
        /// <returns></returns>
        List<QH_FACILITY_ALL_VIEW> ReadMedicalFacilityListByKey(DateTime updatedDate, params Guid[] facilityKeys);

        /// <summary>
        /// 施設の言語リソースを取得する。施設が登録されていない場合はデフォルトの言語リソースを取得する。
        /// </summary>
        /// <param name="facilityKey">施設キー</param>
        /// <returns>言語リソース</returns>
        List<QH_FACILITYLANGUAGE_MST> ReadFacilityLanguage(Guid facilityKey);

        /// <summary>
        /// 施設キーで施設設定を取得する。
        /// </summary>
        /// <param name="facilityKey">施設キー</param>
        /// <returns>施設設定エンティティ。未登録の場合は null。</returns>
        QH_FACILITYCONFIG_MST ReadFacilityConfig(Guid facilityKey);
    }

    /// <summary>
    /// 施設情報の入出力実装
    /// </summary>
    public class FacilityRepository: QsDbReaderBase, IFacilityRepository
    {
        internal const int PHARMACY_LINKAGE_SYSTEM_NO = 99000; //とりあえず、CCC用のLinkageSystemNoを入れておくが、MGの薬局システム使うようになったらこれを直すか、引数追加して外から与える

        /// <summary>
        /// 位置情報から薬局検索
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="now"></param>
        /// <param name="isContractedFacilityOnly"></param>
        /// <param name="startDistance"></param>
        /// <param name="endDistance"></param>
        /// <returns></returns>
        public List<DbFacilityItem> SearchPharmacyByLocation(Guid accountKey, decimal latitude, decimal longitude, DateTime now, bool isContractedFacilityOnly, int startDistance, int endDistance)
        {
            List<DbFacilityItem> result = new List<DbFacilityItem>();
            int dayOfWeek = (int)now.DayOfWeek;
            if (dayOfWeek == 0)
            {
                dayOfWeek = 7; // 日曜の値だけ異なる
            }
            DbFacilityLocationSearcher searcher = new DbFacilityLocationSearcher();
            DbFacilityLocationSearcherArgs searcherArgs = new DbFacilityLocationSearcherArgs()
            {
                FacilityType = 3,
                AccountKey = accountKey,
                CurrentLatitude = latitude,
                CurrentLongitude = longitude,
                StartDistance = startDistance,
                EndDistance = endDistance,
                NowDate = now,
                DayOfWeek = dayOfWeek,
                isHoriday = IsHoliday(now),
                LinkageSystemNo = isContractedFacilityOnly ? PHARMACY_LINKAGE_SYSTEM_NO : int.MinValue
            };
            DbFacilityLocationSearcherResults searcherResults = QsDbManager.Read(searcher, searcherArgs);

            if (searcherResults != null)
            {

                if (searcherResults.IsSuccess)
                {
                    if (searcherResults.FacilityN != null && searcherResults.FacilityN.Any())
                    {
                        result = searcherResults.FacilityN;
                    }
                }

            }

            return result;
        }

        /// <summary>
        /// Filter項目で薬局検索して結果を返す。
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="searchText"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="prefectureNo"></param>
        /// <param name="cityNo"></param>
        /// <param name="now"></param>
        /// <param name="isContractedFacilityOnly"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public (List<DbFacilityItem> facilityN, int pageIndex, int maxPageIndex, int totalCount) SearchPharmacyByFiltering(Guid accountKey, string searchText,
            decimal latitude, decimal longitude, byte prefectureNo, int cityNo, DateTime now, bool isContractedFacilityOnly,
            int pageIndex, int pagesize)
        {
            var facilityN = new List<DbFacilityItem>();
            int dayOfWeek = (int)now.DayOfWeek;
            if (dayOfWeek == 0)
            {
                dayOfWeek = 7; // 日曜の値だけ異なる
            }

            DbFacilityFilteringSearcher searcher = new DbFacilityFilteringSearcher();
            DbFacilityFilteringSearcherArgs searcherArgs = new DbFacilityFilteringSearcherArgs()
            {
                FacilityType = 3,
                AccountKey = accountKey,
                SearchText = searchText.Trim(),
                CurrentLatitude = latitude,
                CurrentLongitude = longitude,
                PrefectureNo = prefectureNo,
                CityNo = cityNo,
                NowDate = now,
                DayOfWeek = dayOfWeek,
                IsHoriday = IsHoliday(now),
                LinkageSystemNo = isContractedFacilityOnly ? PHARMACY_LINKAGE_SYSTEM_NO : int.MinValue,
                PageIndex = pageIndex,
                PageSize = pagesize
            };
            DbFacilityFilteringSearcherResults searcherResults = QsDbManager.Read(searcher, searcherArgs);

            if (searcherResults != null && searcherResults.IsSuccess)
            {
                if (searcherResults.FacilityN != null && searcherResults.FacilityN.Any())
                {
                    facilityN = searcherResults.FacilityN;
                }

                return (facilityN, searcherResults.PageIndex, searcherResults.MaxPageIndex, searcherResults.TotalCount);                
            }

            return (facilityN, 0,0,0);
        }

        /// <summary>
        /// 施設情報を取得
        /// </summary>
        /// <param name="facilityKey"></param>
        /// <param name="accountKey"></param>
        /// <returns></returns>
        public DbFacilityItem ReadFacility(Guid facilityKey, Guid accountKey)
        {
            var now = DateTime.Now;

            var reader = new DbFacilitySearcher();
            var args = new DbFacilitySearcherArgs
            {
                FacilityKey = facilityKey,
                AccountKey = accountKey,
                NowDate = now,
                DayOfWeek = (int)now.DayOfWeek,
                isHoriday = IsHoliday(now)
            };

            var result = QsDbManager.Read(reader, args);

            if(result != null && result.IsSuccess && 
                result.Facility != null && result.Facility.FacilityKey != Guid.Empty)
            {
                return result.Facility;
            }

            return new DbFacilityItem();
        }

        /// <summary>
        /// 施設キーでEntityを抽出
        /// </summary>
        /// <param name="facilityKey"></param>
        /// <returns></returns>
        public QH_FACILITY_MST ReadFacility(Guid facilityKey)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_FACILITY_MST>())
            {
                var parameters = new List<DbParameter>
                {
                    CreateParameter(con, "@p1", facilityKey),
                };
                
                var sql = $@"
                SELECT *
                FROM QH_FACILITY_MST
                WHERE FACILITYKEY = @p1
                AND DELETEFLAG = 0
                ";

                con.Open();

                var result = ExecuteReader<QH_FACILITY_MST>(con, null, sql, parameters);

                return result.FirstOrDefault();
            }
        }

        /// <summary>
        /// 施設画像のキーを取得
        /// </summary>
        /// <param name="facilityKey"></param>
        /// <returns></returns>
        public List<QoApiFileKeyItem> GetFacilityImageKey(Guid facilityKey)
        {
            // TODO サムネイル使用フラグが取れない？

            var result = new List<QoApiFileKeyItem>();

            var reader = new FacilityFileReader();
            var readerArgs = new FacilityFileReaderArgs
            {
                FacilityKey = facilityKey 
            };
            var readerResults = QsDbManager.Read(reader, readerArgs);

            if (readerResults.IsSuccess && readerResults.FileKeyN != null && readerResults.FileKeyN.Any())
            {
                for (int i = 0; i <= readerResults.FileKeyN.Count - 1; i++)
                {
                    result.Add(new QoApiFileKeyItem()
                    {
                        Sequence = (i + 1).ToString(),
                        FileKeyReference = readerResults.FileKeyN[i].ToEncrypedReference()
                    });
                }
            }

            return result;
        }

        /// <summary>
        /// 指定日が祝日か否かを祝日マスタを参照して返す
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool IsHoliday(DateTime date)
        {
            var reader = new QhHolidayEntityReader();
            var readerArgs = new QhHolidayEntityReaderArgs
            {
                Data = new List<QH_HOLIDAY_MST>
            {
                new QH_HOLIDAY_MST
                {
                    TARGETYEAR = date.Year,
                    TARGETMONTH = date.Month,
                    TARGETDAY = date.Day,
                    FACILITYKEY = Guid.Empty,
                    DELETEFLAG = false,
                }
            }
            };
            var readerResults = QsDbManager.Read(reader, readerArgs);
            bool result = false;
            if (readerResults.IsSuccess && readerResults.Result != null && readerResults.Result.Count > 0)
                result = true;
            return result;
        }

        /// <summary>
        /// 医療機関コードから対応する施設情報を返す
        /// </summary>
        /// <param name="medicalFacilityCode">医療機関コード</param>
        /// <returns></returns>
        public QH_FACILITY_MST ReadFacilityFromMedicalFacilityCode(string medicalFacilityCode)
        {
            var readerArgs = new FacilityMasterReaderArgs() { MedicalFacilityCode = medicalFacilityCode };
            var readerResult = QsDbManager.Read(new FacilityMasterReader(), readerArgs);
            if (readerResult != null && readerResult.IsSuccess && readerResult.Result != null && readerResult.Result.Count == 1)
            {
                return readerResult.Result.First();
            }

            var errorMsg = $"Facility情報の取得に失敗しました 医療機関番号:{medicalFacilityCode}";
            QoAccessLog.WriteErrorLog(errorMsg, Guid.Empty);

            throw new Exception(errorMsg);
        }

        /// <summary>
        /// 医療機関リストを連携システム番号の関連施設で抽出する。
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <param name="updatedDate"></param>
        /// <returns></returns>
        public List<QH_FACILITY_ALL_VIEW> ReadMedicalFacilityListByLinkageSystemNo(int linkageSystemNo, DateTime updatedDate)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_FACILITY_MST>())
            {
                var parameters = new List<DbParameter>
                {
                    CreateParameter(con, "@p1", linkageSystemNo),
                    CreateParameter(con, "@p2", updatedDate)
                };

                // 施設マスタと施設関連テーブルの情報を一括で取得する
                var sql = $@"
                {GetMedicalFacilityAllSelectSql()}
                FROM QH_FACILITY_MST t1
                INNER JOIN QH_LINKAGESYSTEM_MST t2
                    ON t2.FACILITYKEY = t1.PARENTKEY
                    AND t2.LINKAGESYSTEMNO = @p1
                    AND t2.DELETEFLAG = 0
                LEFT JOIN QH_FACILITYADDITION_MST t3
                    ON t3.FACILITYKEY = t1.FACILITYKEY
                    AND t1.DELETEFLAG = 0
                WHERE t1.FACILITYTYPE = 1
                AND t1.DELETEFLAG = 0
                AND t1.UPDATEDDATE >= @p2
                ORDER BY t1.DISPORDER;
                ";

                con.Open();

                return ExecuteReader<QH_FACILITY_ALL_VIEW>(con, null, sql, parameters);
            }
        }

        /// <summary>
        /// 医療機関リストを施設キーで抽出する。
        /// </summary>
        /// <param name="updatedDate"></param>
        /// <param name="facilityKeys"></param>
        /// <returns></returns>
        public List<QH_FACILITY_ALL_VIEW> ReadMedicalFacilityListByKey(DateTime updatedDate,params Guid[] facilityKeys)
        {
            var keyList = facilityKeys.Select(x => x.ToString("D")).ToList();

            using (var con = QsDbManager.CreateDbConnection<QH_FACILITY_MST>())
            {
                var parameters = new List<DbParameter>
                {
                    CreateParameter(con, "@p1", updatedDate)
                };

                // 施設マスタと施設関連テーブルの情報を一括で取得する                
                var sql = $@"
                {GetMedicalFacilityAllSelectSql()}
                FROM QH_FACILITY_MST t1
                LEFT JOIN QH_FACILITYADDITION_MST t3
                    ON t3.FACILITYKEY = t1.FACILITYKEY
                    AND t1.DELETEFLAG = 0
                WHERE t1.FACILITYTYPE = 1
                AND t1.FACILITYKEY IN ('{string.Join("','", keyList)}')
                AND t1.DELETEFLAG = 0
                AND t1.UPDATEDDATE >= @p1
                ORDER BY t1.DISPORDER;
                ";

                con.Open();

                return ExecuteReader<QH_FACILITY_ALL_VIEW>(con, null, sql, parameters);
            }
        }

        /// <inheritdoc />
        public List<QH_FACILITYLANGUAGE_MST> ReadFacilityLanguage(Guid facilityKey)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_FACILITYLANGUAGE_MST>())
            {
                var parameters = new List<DbParameter>
                {
                    CreateParameter(con, "@p1", facilityKey),
                    CreateParameter(con, "@p2", Guid.Empty) // デフォルトの言語リソースを取得するための施設キー
                };

                // 施設レコードない場合はデフォルトのレコードを使うように抽出する
                // デフォルトのキーは必ずあることが前提
                const string sql = @"
                SELECT 
                    COALESCE(b.FACILITYKEY, a.FACILITYKEY) as FACILITYKEY,
                    COALESCE(b.LANGUAGEKEY, a.LANGUAGEKEY) as LANGUAGEKEY,
                    COALESCE(b.VALUE, a.VALUE) as VALUE,
                    a.DELETEFLAG,
                    COALESCE(b.CREATEDDATE, a.CREATEDDATE) as CREATEDDATE,
                    COALESCE(b.UPDATEDDATE, a.UPDATEDDATE) as UPDATEDDATE
                FROM (
                    SELECT *
                    FROM QH_FACILITYLANGUAGE_MST
                    WHERE FACILITYKEY = @p2
                    AND DELETEFLAG = 0
                ) a
                LEFT JOIN (
                    SELECT *
                    FROM QH_FACILITYLANGUAGE_MST
                    WHERE FACILITYKEY = @p1
                    AND DELETEFLAG = 0    
                ) b
                ON a.LANGUAGEKEY = b.LANGUAGEKEY
                ";

                con.Open();

                return ExecuteReader<QH_FACILITYLANGUAGE_MST>(con, null, sql, parameters);
            }
        }

        /// <inheritdoc />
        public QH_FACILITYCONFIG_MST ReadFacilityConfig(Guid facilityKey)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_FACILITYCONFIG_MST>())
            {
                var parameters = new List<DbParameter>
                {
                    CreateParameter(con, "@p1", facilityKey),
                };

                var sql = $@"
                SELECT *
                FROM QH_FACILITYCONFIG_MST
                WHERE FACILITYKEY = @p1
                AND DELETEFLAG = 0
                ";

                con.Open();

                var result = ExecuteReader<QH_FACILITYCONFIG_MST>(con, null, sql, parameters);

                return result.FirstOrDefault();
            }
        }

        // 医療機関リスト取得用のSelect句を返す
        string GetMedicalFacilityAllSelectSql()
        {
            // 1:Nの関係はSELECT句のサブクエリをJSON化して取得
            return @"
            SELECT 
                t1.FACILITYKEY,t1.FACILITYNAME,t1.FACILITYKANANAME,t1.POSTALCODE,
                t1.ADDRESS1,t1.ADDRESS2,t1.PREFNO,t1.CITYNO,
                t1.OFFICIALNAME,t1.MEDICALFACILITYCODE,t1.DISPORDER,t1.UPDATEDDATE,
                t3.LATITUDE,t3.LONGITUDE,
                (
                    SELECT SEQUENCE, FILEKEY 
                    FROM QH_FACILITYFILE_MST t4
                    WHERE t4.FACILITYKEY = t1.FACILITYKEY
                    AND t4.DELETEFLAG = 0
                    FOR JSON PATH
                ) AS FILEJSON,
                (
                    SELECT 
                        t5.DEPARTMENTNO, t6.DEPARTMENTNAME, t5.LOCALCODE, t5.LOCALNAME,
                        t5.DISPORDER
                    FROM QH_FACILITYMEDICALDEPARTMENT_DAT t5
                    INNER JOIN QH_MEDICALDEPARTMENT_MST t6
                    ON t5.DEPARTMENTNO = t6.DEPARTMENTNO
                    AND t6.DELETEFLAG = 0
                    WHERE t5.FACILITYKEY = t1.FACILITYKEY
                    AND t5.DELETEFLAG = 0
                    AND t5.DISPFLAG = 1
                    FOR JSON PATH
                ) AS DEPARTMENTJSON,
                (
                    SELECT 
                        t7.CONTACTINFORMATIONNO,t7.CONTACTINFORMATIONTYPE,t7.TELAREACODE,t7.TELCITYCODE,
                        t7.TELSUBSCRIBERNUMBER,t7.TELFULL,t7.ACCEPTEDSTART,t7.ACCEPTEDEND,
                        t7.COMMENTSET,t7.DISPORDER
                    FROM QH_FACILITYCONTACTINFORMATION_DAT t7
                    WHERE t7.FACILITYKEY = t1.FACILITYKEY
                    AND DELETEFLAG = 0
                    FOR JSON PATH
                ) AS CONTACTJSON,
                (
                    SELECT 
                        t8.URLNO,t8.URLTYPE,t8.URL,t8.DISPORDER
                    FROM QH_FACILITYURL_DAT t8
                    WHERE t8.FACILITYKEY = t1.FACILITYKEY
                    AND t8.DELETEFLAG = 0
                    FOR JSON PATH
                ) AS URLJSON";
        }
    }
}