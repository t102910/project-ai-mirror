using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System;
using System.Linq;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.QolmsCryptV1;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Threading;
using System.IO;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// EHR連携関連の機能を提供します。
    /// </summary>
    public class HealthRecordSphrVitalWorker
    {

        #region "Private Method"

        //QH_HEALTHRECORD_DATから対象データを取得する。
        private static List<QH_HEALTHRECORD_DAT> ReadHealthRecordDat(string linkageSystemNo, string linkageSystemId, string strWhere)
        {

            var reader = new SphrHealthRecordReader();

            try
            {
                var readerArgs = new SphrHealthRecordReaderArgs() { LinkageSystemNo = linkageSystemNo, LinkageSystemId = linkageSystemId, VitalTypes = strWhere };

                //読込
                var readerResults = QsDbManager.Read(reader, readerArgs);
                if (readerResults != null && readerResults.IsSuccess && readerResults.Result != null)
                {
                    //結果返却
                    return readerResults.Result;
                    
                }
                else
                {
                    QoAccessLog.WriteInfoLog(readerResults.Result.Count.ToString());
                    QoAccessLog.WriteErrorLog(string.Format($"[ReadHealthRecordDat]QH_HEALTHRECORD_DAT情報の取得に失敗しました。"), Guid.Empty);
                    return null;
                }
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
                return null;
            }


        }

        #endregion

        #region "Public Method"
        /// <summary>
        /// Webhook処理を行います。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoHealthRecordSphrVitalReadApiResults SphrVitalRead(QoHealthRecordSphrVitalReadApiArgs args)
        {
            var result = new QoHealthRecordSphrVitalReadApiResults() { IsSuccess = bool.FalseString };

            //取得条件設定かつ引数チェック
            var strWhere = string.Empty;
            foreach (string t in args.VitalTypeN)
            {
                if (Enum.TryParse<QsDbVitalTypeEnum>(t, out QsDbVitalTypeEnum type))
                {
                    strWhere = string.IsNullOrWhiteSpace(strWhere) ? strWhere + t.ToString() : strWhere + "," + t;
                }
                else
                {
                    //QsDbVitalTypeEnumに存在しないtypeなので例外
                    throw new ArgumentException();
                }

            }

            //データ取得
            var data = ReadHealthRecordDat(args.LinkageSystemNo, args.LinkageSystemId, strWhere);
            if(data != null && data.Count == 0)
            {
                QoAccessLog.WriteInfoLog("データ取得失敗");
                return result;
            }

            result.VitalValueN = new List<QhApiVitalValueItem>();
            foreach (var record in data)
            {
                result.VitalValueN.Add(
                    new QhApiVitalValueItem {
                        RecordDate = record.RECORDDATE.ToString(),
                        VitalType = record.VITALTYPE.ToString(),
                        Value1 = record.VALUE1.ToString(),
                        Value2 = record.VALUE2.ToString(),
                        Value3 = "0",
                        Value4 = "0",
                        ConditionType = record.CONDITIONTYPE.ToString()
                    });
            }
            result.IsSuccess = bool.TrueString;
            result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            return result;
        }

        #endregion
    }

}