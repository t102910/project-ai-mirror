using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.QolmsOpenApi.Sql.Core;
using MGF.QOLMS.QolmsOpenApi.Worker;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;
using MGF.QOLMS.QolmsCryptV1;
using System.Text;

namespace MGF.QOLMS.QolmsOpenApi.Repositories
{
    /// <summary>
    /// 診察券情報の入出力インターフェース
    /// </summary>
    public interface IPatientCardRepository
    {
        /// <summary>
        /// アカウントキーに紐づく施設情報も含む診察券(利用者カード情報)リストを取得する。 
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="statusType">0指定時全て、0以外は絞り込み</param>
        /// <param name="parentLinkageSystemNo">親連携システム番号</param>
        /// <returns></returns>
        List<QH_PATIENTCARD_FACILITY_VIEW> ReadPatientCardList(Guid accountKey, byte statusType = 0, int parentLinkageSystemNo = int.MinValue);

        QH_PATIENTCARD_FACILITY_VIEW ReadPatientCard(Guid accountKey, int cardCode, int sequence, byte statusType = 0);
    }

    /// <summary>
    /// 診察券情報の入出力実装
    /// </summary>
    public class PatientCardRepository : QsDbReaderBase, IPatientCardRepository
    {
        /// <summary>
        /// アカウントキーに紐づく施設情報も含む診察券(利用者カード情報)リストを取得する。 
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="statusType">0指定時全て、0以外は絞り込み</param>
        /// <param name="parentLinkageSystemNo"></param>
        /// <returns></returns>
        public List<QH_PATIENTCARD_FACILITY_VIEW> ReadPatientCardList(Guid accountKey, byte statusType = 0, int parentLinkageSystemNo = int.MinValue)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_PATIENTCARD_DAT>())
            {
                var parameters = new List<DbParameter>
                {
                    CreateParameter(con, "@p1", accountKey),
                    CreateParameter(con, "@p2", statusType),
                    CreateParameter(con, "@p3", parentLinkageSystemNo)
                };

                var builder = new StringBuilder();

                // QH_LINKAGESYSTEM_MSTは表示フラグの判定とカスタムバーコード書式のために結合
                // QH_LINKAGE_DATはStatusTypeの取得用
                // QH_FACILITY_MSTはカスタムバーコード生成の素材として結合
                // FormatはQH_LINKAGESYSTEM_MST.LINKAGESET内のJSONからら取得する
                // 連携マスタや連携データにないレコードは診察券対象外とする
                builder.AppendLine(@"
                    SELECT 
                        t1.ACCOUNTKEY,t1.CARDCODE,t1.SEQUENCE, t1.FACILITYKEY, 
                        t1.CARDNO,t1.CREATEDDATE,t3.STATUSTYPE,
                        t4.FACILITYNAME,t4.FACILITYKANANAME,t4.POSTALCODE,t4.ADDRESS1,
                        t4.ADDRESS2,t4.PREFNO,t4.CITYNO,t4.TEL,
                        t4.FAX,t4.OFFICIALNAME,t4.MEDICALFACILITYCODE,
                        COALESCE(
                            CASE WHEN ISJSON(t2.LINKAGESET) = 1 
                                THEN JSON_VALUE(t2.LINKAGESET,'$.PatientCardBarcodeFormat') 
                                ELSE NULL 
                            END,
                        '{MedicalCode:10}{CardNo}') As Format
                    From QH_PATIENTCARD_DAT t1
                    INNER JOIN QH_LINKAGESYSTEM_MST t2
                        On t1.CARDCODE = t2.LINKAGESYSTEMNO
                        And t2.DISPLAYFLAG = 1
                        And t2.DELETEFLAG = 0
                    INNER JOIN QH_LINKAGE_DAT t3
                        On t1.ACCOUNTKEY = t3.ACCOUNTKEY
                        And t1.CARDCODE = t3.LINKAGESYSTEMNO                        
                        And t3.DELETEFLAG = 0");
                if (statusType > 0)
                {
                    // StatusType0以外の場合のみ条件設定
                    builder.AppendLine("And t3.STATUSTYPE = @p2");
                }
                builder.AppendLine(@"
                    INNER JOIN QH_FACILITY_MST t4
                        On t1.FACILITYKEY = t4.FACILITYKEY
                        AND t4.FACILITYTYPE = 1
                        And t4.DELETEFLAG = 0");
                if (parentLinkageSystemNo != int.MinValue)
                {
                    // 親連携番号が指定されている場合は親連携番号で絞り込む
                    builder.AppendLine(@"
                        AND t4.PARENTKEY = (
                            SELECT FACILITYKEY FROM QH_LINKAGESYSTEM_MST
                            WHERE LINKAGESYSTEMNO = @p3
                        )");
                }
                builder.AppendLine(@"
                    WHERE t1.ACCOUNTKEY = @p1
                    AND t1.DELETEFLAG = 0
                    AND t1.CARDCODE > 0
                ");

                con.Open();

                return ExecuteReader<QH_PATIENTCARD_FACILITY_VIEW>(con, null, builder.ToString(), parameters);
            }
        }

        /// <summary>
        /// 施設情報も含む診察券（利用者カード情報）を取得する
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="cardCode"></param>
        /// <param name="sequence"></param>
        /// <param name="statusType"></param>
        /// <returns></returns>
        public QH_PATIENTCARD_FACILITY_VIEW ReadPatientCard(Guid accountKey,int cardCode,int sequence, byte statusType = 0)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_PATIENTCARD_DAT>())
            {
                var parameters = new List<DbParameter>
                {
                    CreateParameter(con, "@p1", accountKey),
                    CreateParameter(con, "@p2", statusType),
                    CreateParameter(con, "@p3", cardCode),
                    CreateParameter(con, "@p4", sequence)
                };

                var builder = new StringBuilder();

                // QH_LINKAGESYSTEM_MSTは表示フラグの判定のために結合
                // QH_LINKAGE_DATはStatusTypeの取得用
                // QH_FACILITY_MSTはカスタムバーコード生成の素材として結合
                // FormatはQH_LINKAGESYSTEM_MST.LINKAGESET内のJSONからら取得する
                // 連携マスタや連携データにないレコードは診察券対象外とする
                builder.AppendLine(@"
                    SELECT 
                        t1.ACCOUNTKEY,t1.CARDCODE,t1.SEQUENCE, t1.FACILITYKEY, 
                        t1.CARDNO,t1.CREATEDDATE,t3.STATUSTYPE,
                        t4.FACILITYNAME,t4.FACILITYKANANAME,t4.POSTALCODE,t4.ADDRESS1,
                        t4.ADDRESS2,t4.PREFNO,t4.CITYNO,t4.TEL,
                        t4.FAX,t4.OFFICIALNAME,t4.MEDICALFACILITYCODE,
                        COALESCE(
                            CASE WHEN ISJSON(t2.LINKAGESET) = 1 
                                THEN JSON_VALUE(t2.LINKAGESET,'$.PatientCardBarcodeFormat') 
                                ELSE NULL 
                            END,
                        '{MedicalCode:10}{CardNo}') As Format
                    From QH_PATIENTCARD_DAT t1
                    INNER JOIN QH_LINKAGESYSTEM_MST t2
                        On t1.CARDCODE = t2.LINKAGESYSTEMNO
                        And t2.DISPLAYFLAG = 1
                        And t2.DELETEFLAG = 0
                    INNER JOIN QH_LINKAGE_DAT t3
                        On t1.ACCOUNTKEY = t3.ACCOUNTKEY
                        And t1.CARDCODE = t3.LINKAGESYSTEMNO                        
                        And t3.DELETEFLAG = 0");
                if (statusType > 0)
                {
                    // StatusType0以外の場合のみ条件設定
                    builder.AppendLine("And t3.STATUSTYPE = @p2");
                }
                builder.AppendLine(@"
                    INNER JOIN QH_FACILITY_MST t4
                        On t1.FACILITYKEY = t4.FACILITYKEY
                        AND t4.FACILITYTYPE = 1
                        And t4.DELETEFLAG = 0
                    WHERE t1.ACCOUNTKEY = @p1
                    AND t1.CARDCODE = @p3
                    AND t1.SEQUENCE = @p4
                    AND t1.DELETEFLAG = 0
                ");

                con.Open();

                return ExecuteReader<QH_PATIENTCARD_FACILITY_VIEW>(con, null, builder.ToString(), parameters).FirstOrDefault();
            }
        }
    }
}