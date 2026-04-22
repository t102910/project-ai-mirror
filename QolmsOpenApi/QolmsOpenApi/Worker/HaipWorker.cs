using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System;
using System.Linq;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsLineMessageApiCoreV1;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Threading;
using System.IO;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// HAIP関連の機能を提供します。
    /// </summary>
    public class HaipWorker
    {

        #region "Private Method"

        //QH_HAIPREQUESTMANAGEMENT_DATへrequestIdをもとにデータ収集完了情報を登録。
        private static bool WriteHaipRequestManagement(Guid requestId, string responseStatus)
        {

            try
            {
                var writer = new HaipRequestManagementWriter();

                //QH_HAIPREQUESTMANAGEMENT_DATに登録
                var writerArgs = new HaipRequestManagementWriterArgs() { RequestId = requestId, ResponseStatus = responseStatus };
                var writerResults = QsDbManager.Write(writer, writerArgs);

                //登録失敗時
                if (!(writerResults != null && writerResults.IsSuccess))
                {
                    QoAccessLog.WriteErrorLog(string.Format($"[WriteHaipRequestManagement]QH_HAIPREQUESTMANAGEMENT_DATへの登録に失敗しました。"), Guid.Empty);
                }
                return writerResults.IsSuccess;
            }
            catch (Exception ex)
            {

                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
                return false;
            }


        }

        #endregion

        #region "Public Method"
        /// <summary>
        /// Webhook処理を行います。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static HaipPhrReceiveNotificationResults HaipPhrReceiveNotification(HaipPhrReceiveNotificationArgs args)
        {
            QoAccessLog.WriteInfoLog("HAIP受信");
            var result = new HaipPhrReceiveNotificationResults() {};

            result.datetime = DateTime.Now.ToString("s");
            result.requestid = args.XRequestId;
            //引数チェック
            if (string.IsNullOrWhiteSpace(args.XApiKey))
            {
                QoAccessLog.WriteInfoLog("XApiKeyチェック");
                result.error = "Invalid Request";
                result.error_description = "Missing form parameter: X-Api-Key";
                return result;
            }
            //引数チェック
            if (!Guid.TryParse(args.XRequestId, out Guid RequestId))
            {
                QoAccessLog.WriteInfoLog("XRequestIdチェック");
                result.error = "Invalid Request";
                result.error_description = "Missing form parameter: X-Request-Id";
                return result;
            }

            //APIKeyチェック

            if (args.XApiKey != QoApiConfiguration.HaipApiKey)
            {
                QoAccessLog.WriteInfoLog("XApiKey認証チェック");
                result.error = "Unauthorized";
                result.error_description = "認証エラー";
                return result;
            }

            //DB更新
            if (!WriteHaipRequestManagement(RequestId, args.ResultStatus))
            {
                QoAccessLog.WriteInfoLog("DB書込失敗");
                result.error = "Unauthorized";
                result.error_description = "処理失敗";
                return result;
            }

            return result;
        }

        #endregion
    }

}