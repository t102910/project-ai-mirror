using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Sql;

namespace MGF.QOLMS.QolmsOpenApi.Repositories
{
    /// <summary>
    /// 順番待ちテーブル入出力インターフェース
    /// </summary>
    public interface IWaitingRepository
    {
        /// <summary>
        /// 指定条件の順番待ちリストを取得する
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="facilityKey"></param>
        /// <param name="targetDate"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        List<QH_WAITINGLIST_DAT> ReadLatestWaitingList(Guid accountKey, Guid facilityKey, DateTime targetDate, byte dataType);

        /// <summary>
        /// 指定条件の待ち人数を取得する
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="facilityKey"></param>
        /// <param name="targetDate"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        int GetWaitingCount(Guid accountKey, Guid facilityKey, DateTime targetDate, byte dataType);

        /// <summary>
        /// 順番待ちリストを書き込む
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        (bool isSuccess, List<QH_WAITINGLIST_DAT> oldList, List<string> errorMessages) WriteList(List<QH_WAITINGLIST_DAT> entities);

        /// <summary>
        /// 順番取得情報を取得する
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <param name="departmentCode"></param>
        /// <param name="doctorCode"></param>
        /// <param name="config"></param>
        /// <param name="reservationDate"></param>
        /// <param name="priorityType"></param>
        /// <param name="sameDaySequence"></param>
        /// <returns></returns>
        List<QH_WAITINGORDERLIST_DAT> GetWaitingOrderListEntity(int linkageSystemNo, string departmentCode, string doctorCode, QH_FACILITYMEDICALDEPARTMENTCONFIG_DAT config, DateTime reservationDate, QsDbWaitingPriorityTypeEnum priorityType, int sameDaySequence);

        /// <summary>
        /// 順番取得情報を取得する
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <param name="departmentCode"></param>
        /// <param name="doctorCode"></param>
        /// <param name="limit"></param>
        /// <param name="reserveFlag"></param>
        /// <param name="reservationDate"></param>
        /// <returns></returns>
        List<QH_WAITINGORDERLIST_DAT> GetWaitingOrderListEntity(int linkageSystemNo, string departmentCode, string doctorCode, int limit, bool reserveFlag, DateTime reservationDate);

        /// <summary>
        /// 対象者の順番取得情報を取得する
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <param name="departmentCode"></param>
        /// <param name="doctorCode"></param>
        /// <param name="config"></param>
        /// <param name="reservationDate"></param>
        /// <param name="linkageSystemId"></param>
        /// <param name="priorityType"></param>
        /// <param name="sameDaySequence"></param>
        /// <returns></returns>
        (int waitingCount, bool shouldChangeStatus) GetWaitingOrderNumber(int linkageSystemNo, string departmentCode, string doctorCode, QH_FACILITYMEDICALDEPARTMENTCONFIG_DAT config, DateTime reservationDate, string linkageSystemId, QsDbWaitingPriorityTypeEnum priorityType, int sameDaySequence);

        /// <summary>
        /// 順番取得情報を書き込む
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool UpsertWaitingOrderList(QH_WAITINGORDERLIST_DAT entity);

        /// <summary>
        /// 順番取得情報を削除する
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <param name="departmentCode"></param>
        /// <param name="doctorCode"></param>
        /// <param name="linkageSystemId"></param>
        /// <param name="sameDaySequence"></param>
        /// <returns></returns>
        bool DeleteWaitingOrderList(int linkageSystemNo, string departmentCode, string doctorCode, string linkageSystemId, int sameDaySequence);

        /// <summary>
        /// 施設診療科設定情報を取得する
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <param name="departmentCode"></param>
        /// <returns></returns>
        QH_FACILITYMEDICALDEPARTMENTCONFIG_DAT GetMedicalDepartmentConfig(int linkageSystemNo, string departmentCode);

        /// <summary>
        /// 施設診療科別JSON設定情報を取得する
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <param name="departmentCode"></param>
        /// <returns></returns>
        QH_FACILITYMEDICALDEPARTMENTAPPCONFIG_DAT GetMedicalDepartmentAppConfig(int linkageSystemNo, string departmentCode);

        /// <summary>
        /// 施設診療科別JSON設定情報のJsonクラスを取得する
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        QhFacilityDepartmentAppConfigOfJson GetMedicalDepartmentAppValue(QH_FACILITYMEDICALDEPARTMENTAPPCONFIG_DAT entity);

        /// <summary>
        /// 待ち人数カウントの優先度情報を取得する
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        QsDbWaitingPriorityTypeEnum GetWaitingPriorityType(string json);
    }

    /// <summary>
    /// TIS 順番待ちテーブル入出力実装
    /// </summary>
    public class WaitingRepository:QsDbReaderBase,  IWaitingRepository
    {
        /// <summary>
        /// 指定条件の順番待ちリストを取得する
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="facilityKey"></param>
        /// <param name="targetDate"></param>
        /// <param name="dataType"></param>
        /// <returns>List<QH_WAITINGLIST_DAT></returns>
        public List<QH_WAITINGLIST_DAT> ReadLatestWaitingList(Guid accountKey, Guid facilityKey, DateTime targetDate, byte dataType)
        {
            var reader = new DbWaitingListReaderCore(accountKey, accountKey);

            return reader.ReadWaitingListEntities(facilityKey, string.Empty, targetDate, dataType);
        }

        /// <summary>
        /// 指定条件の待ち人数を取得する
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="facilityKey"></param>
        /// <param name="targetDate"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public int GetWaitingCount(Guid accountKey, Guid facilityKey, DateTime targetDate, byte dataType)
        {
            var reader = new DbWaitingListReaderCore(accountKey, accountKey);
            return reader.ReadWaitingCount(facilityKey, string.Empty, targetDate, dataType);
        }

        /// <summary>
        /// 順番待ちリストを書き込む
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public (bool isSuccess, List<QH_WAITINGLIST_DAT> oldList, List<string> errorMessages) WriteList(List<QH_WAITINGLIST_DAT> entities)
        {
            var oldList = new List<QH_WAITINGLIST_DAT>();
            var errorMessages = new List<string>();            
            var isSuccess = QolmsLibraryV1.WaitingListWorker.Write(entities, ref oldList, ref errorMessages);

            return (isSuccess, oldList, errorMessages);
        }

        /// <summary>
        /// 順番取得情報を取得する
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <param name="departmentCode"></param>
        /// <param name="doctorCode"></param>
        /// <param name="config"></param>
        /// <param name="reservationDate"></param>
        /// <param name="priorityType"></param>
        /// <param name="sameDaySequence"></param>
        /// <returns>List<QH_WAITINGORDERLIST_DAT></returns>        
        public List<QH_WAITINGORDERLIST_DAT> GetWaitingOrderListEntity(int linkageSystemNo, string departmentCode, string doctorCode, QH_FACILITYMEDICALDEPARTMENTCONFIG_DAT config, DateTime reservationDate, QsDbWaitingPriorityTypeEnum priorityType, int sameDaySequence)
        {
            var reader = new WaitingOrderListReader();
            //読込
            var readerArgs = new WaitingOrderListReaderArgs() { LinkageSystemNo = linkageSystemNo, DepartmentCode = departmentCode, DoctorCode = doctorCode, WaitingNumber = config.WAITNUMBER, ReserveFlag = config.RESERVEFLAG, OrderDate = reservationDate, priorityType = priorityType, SameDaySequence = sameDaySequence };
            var readerResults = QsDbManager.Read(reader, readerArgs);

            return readerResults.WaitingOrderListEntity;
        }

        /// <summary>
        /// 順番取得情報を取得する
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <param name="departmentCode"></param>
        /// <param name="doctorCode"></param>
        /// <param name="limit"></param>
        /// <param name="reserveFlag"></param>
        /// <param name="reservationDate"></param>
        /// <returns></returns>
        public List<QH_WAITINGORDERLIST_DAT> GetWaitingOrderListEntity(int linkageSystemNo, string departmentCode, string doctorCode, int limit, bool reserveFlag , DateTime reservationDate)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_WAITINGORDERLIST_DAT>())
            {
                var parameters = new List<DbParameter>
                {
                    CreateParameter(con,"@p1", limit),
                    CreateParameter(con,"@p2", linkageSystemNo),
                    CreateParameter(con,"@p3", departmentCode),
                    CreateParameter(con,"@p4", doctorCode),
                    CreateParameter(con,"@p5", reservationDate.ToString("yyyy-MM-dd 00:00:00")),
                    CreateParameter(con,"@p6", reservationDate.ToString("yyyy-MM-dd 23:59:58")),
                    CreateParameter(con,"@p7",reservationDate.ToString("yyyy-MM-dd 23:59:59")),
                };

                var sql = $@"
                    Select Top(@p1) * 
                    From {nameof(QH_WAITINGORDERLIST_DAT)}
                    Where {nameof(QH_WAITINGORDERLIST_DAT.LINKAGESYSTEMNO)} = @p2
                    And {nameof(QH_WAITINGORDERLIST_DAT.DEPARTMENTCODE)} = @p3
                    And {nameof(QH_WAITINGORDERLIST_DAT.DOCTORCODE)} = @p4
                    And {nameof(QH_WAITINGORDERLIST_DAT.DELETEFLAG)} = 0
                ";

                if (reserveFlag)
                {
                    // 予約ありだけを抽出
                    sql += $"And {nameof(QH_WAITINGORDERLIST_DAT.RESERVATIONDATE)} Between @p5 and @p6";
                }
                else
                {
                    // 予約なしだけを抽出
                    sql += $"And {nameof(QH_WAITINGORDERLIST_DAT.RESERVATIONDATE)} = @p7";
                }

                con.Open();

                var result = ExecuteReader<QH_WAITINGORDERLIST_DAT>(con, null, sql, parameters);

                return result;
            }
        }

        /// <summary>
        /// 対象者の順番取得情報を取得する
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <param name="departmentCode"></param>
        /// <param name="doctorCode"></param>
        /// <param name="config"></param>
        /// <param name="reservationDate"></param>
        /// <param name="linkageSystemId"></param>
        /// <param name="priorityType"></param>
        /// <param name="sameDaySequence"></param>
        /// <returns>waitingCount: 待ち人数 / shouldChangeStatus: ステータス変更が必要な場合はtrue</returns>        
        public (int waitingCount, bool shouldChangeStatus) GetWaitingOrderNumber(int linkageSystemNo, string departmentCode, string doctorCode, QH_FACILITYMEDICALDEPARTMENTCONFIG_DAT config, DateTime reservationDate, string linkageSystemId, QsDbWaitingPriorityTypeEnum priorityType, int sameDaySequence)
        {
            // レコード取得判定
            var IsAllRecord = true;
            // 対象予約が予約なし、かつ診療科設定の予約フラグがTRUE(予約ありなし区別)の場合は予約なしだけで待ち人数を取得する
            // 診療科設定の予約フラグ（予約ありなし区別なし)の場合は
            if (reservationDate.Hour == 0 && reservationDate.Minute == 0 && reservationDate.Second == 0 && config.RESERVEFLAG)
            {
                IsAllRecord = false;
            }

            var reader = new WaitingOrderListReader();
            //読込
            var readerArgs = new WaitingOrderListReaderArgs() { LinkageSystemNo = linkageSystemNo, DepartmentCode = departmentCode, DoctorCode = doctorCode, WaitingNumber = int.MaxValue, ReserveFlag = false, OrderDate = reservationDate, priorityType = priorityType, SameDaySequence = sameDaySequence };
            var readerResults = QsDbManager.Read(reader, readerArgs);
            int number = 0;
            if (readerResults.WaitingOrderListEntity != null && readerResults.WaitingOrderListEntity.Any())
            {
                if (IsAllRecord)
                {
                    // 予約ありなしも含めた待ち人数を取得
                    number = readerResults.WaitingOrderListEntity.FindIndex(x => x.LINKAGESYSTEMID.Contains(linkageSystemId));
                }
                else
                {
                    //予約なしだけの待ち人数を取得
                    var noReserveList = new List<QH_WAITINGORDERLIST_DAT>();
                    //予約なしのレコードのみ抽出
                    foreach (var entity in readerResults.WaitingOrderListEntity)
                    {
                        if (entity.RESERVATIONDATE == DateTime.Parse(entity.RESERVATIONDATE.ToString("yyyy-MM-dd 23:59:59")))
                        {
                            noReserveList.Add(entity);
                        }
                    }
                    number = noReserveList.FindIndex(x => x.LINKAGESYSTEMID.Contains(linkageSystemId));
                }
            }

            // 待ち人数が0以上で設定の待ち人数枠以内でかつ予約ありなし区別なしの場合はStatusの変更が必要
            // (要20→21への変更)
            return (number, number >= 0 && number < config.WAITNUMBER && IsAllRecord);
        }

        /// <summary>
        /// 順番取得情報を書き込む
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool UpsertWaitingOrderList(QH_WAITINGORDERLIST_DAT entity)
        {
            var ret = false;

            var writer = new WaitingOrderListWriter();
            //Upsert
            var writerArgs = new WaitingOrderListWriterArgs() { Entity = entity };
            var writerResults = QsDbManager.Write(writer, writerArgs);

            if (writerResults.IsSuccess && writerResults.Result == 1)
            {
                ret = true;
            }

            return ret;
        }

        /// <summary>
        /// 順番取得情報を削除する
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <param name="departmentCode"></param>
        /// <param name="doctorCode"></param>
        /// <param name="linkageSystemId"></param>
        /// <param name="sameDaySequence"></param>
        /// <returns></returns>
        public bool DeleteWaitingOrderList(int linkageSystemNo, string departmentCode, string doctorCode, string linkageSystemId, int sameDaySequence)
        {
            var ret = false;

            var writer = new WaitingOrderListWriter();
            //削除
            var writerArgs = new WaitingOrderListWriterArgs() { LinkageSystemNo = linkageSystemNo, DepartmentCode = departmentCode, DoctorCode = doctorCode, LinkageSystemId = linkageSystemId, DjKbn = sameDaySequence };
            var writerResults = QsDbManager.Write(writer, writerArgs);

            if (writerResults.IsSuccess && writerResults.Result == 1)
            {
                ret = true;
            }

            return ret;
        }

        /// <summary>
        /// 施設診療科設定情報を取得する
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <param name="departmentCode"></param>
        /// <returns></returns>
        public QH_FACILITYMEDICALDEPARTMENTCONFIG_DAT GetMedicalDepartmentConfig(int linkageSystemNo, string departmentCode)
        {
            var reader = new FacilityMedicalDepartmentConfigReader();
            //読込
            var readerArgs = new FacilityMedicalDepartmentConfigReaderArgs() { LinkageSystemNo = linkageSystemNo, DepartmentCode = departmentCode };
            var readerResults = QsDbManager.Read(reader, readerArgs);

            return readerResults.MedicalDepartMentConfigEntity;
        }

        /// <summary>
        /// 施設診療科JSON設定情報を取得する
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <param name="departmentCode"></param>
        /// <returns></returns>
        public QH_FACILITYMEDICALDEPARTMENTAPPCONFIG_DAT GetMedicalDepartmentAppConfig(int linkageSystemNo, string departmentCode)
        {
            var reader = new FacilityMedicalDepartmentAppConfigReader();
            // 読込
            var readerArgs = new FacilityMedicalDepartmentAppConfigReaderArgs() { LinkageSystemNo = linkageSystemNo, DepartmentCode = departmentCode };
            var readerResults = QsDbManager.Read(reader, readerArgs);          

            return readerResults.MedicalDepartMentAppConfigEntity;   
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public QhFacilityDepartmentAppConfigOfJson GetMedicalDepartmentAppValue(QH_FACILITYMEDICALDEPARTMENTAPPCONFIG_DAT entity)
        {
            QsJsonSerializer serializer = new QsJsonSerializer();
            QhFacilityDepartmentAppConfigOfJson jsonValue = serializer.Deserialize<QhFacilityDepartmentAppConfigOfJson>(entity.VALUE);

            return jsonValue;
        }

        /// <summary>
        /// 待ち人数カウントパターンを取得する
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public QsDbWaitingPriorityTypeEnum GetWaitingPriorityType(string json)
        {
            QsJsonSerializer serializer = new QsJsonSerializer();
            QhFacilityDepartmentAppConfigOfJson jsonValue = serializer.Deserialize<QhFacilityDepartmentAppConfigOfJson>(json);

            return jsonValue.WaitingPriority;
        }

    }
}