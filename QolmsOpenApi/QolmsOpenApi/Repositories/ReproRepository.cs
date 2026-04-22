using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.QolmsOpenApi.Worker;
using MGF.QOLMS.QolmsReproApiCoreV1;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Repositories
{
    public interface IReproRepository
    {        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pushMessage"></param>
        /// <param name="accountkeys"></param>
        /// <param name="startDate"></param>
        /// <param name="deepLink"></param>
        /// <returns></returns>
        bool ReproPushApiCall(string pushMessage, List<Guid> accountkeys, DateTime startDate, string deepLink);

        bool InsertPushSend(List<Guid> accountKeys, DateTime now);
    }

    public class ReproRepository : IReproRepository
    {
        public bool ReproPushApiCall(string pushMessage, List<Guid> accountkeys, DateTime startDate, string deepLink)
        {
            //固定値でテストできるようにいったん（検証環境用）
            var pushKey = string.IsNullOrEmpty(ConfigurationManager.AppSettings["PushKey"]) ? "xv9kyj84" : ConfigurationManager.AppSettings["PushKey"];
            QoAccessLog.WriteInfoLog(pushKey);
            QoAccessLog.WriteInfoLog(ConfigurationManager.AppSettings["ReproPushApiIfUri"]);
            QoAccessLog.WriteInfoLog(ConfigurationManager.AppSettings["ReproApiToken"]);

            pushApiArgs args = new pushApiArgs(pushKey, accountkeys)
            {
                schedule = startDate.ToString("o"),
                notification = new Notification() { message = pushMessage, deeplink_url = deepLink }
            };

            pushApiResults res = QsReproApiManager.ExecuteAsync<pushApiArgs, pushApiResults>(args).Result;

            if (res.IsSuccess)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// プッシュ通知ログ を 登録します。
        /// </summary>
        /// <param name="accountKeys">アカウントキー</param>
        /// <returns>
        /// 成功ならtrue、
        /// 失敗ならfalseを返却。
        /// </returns>
        public bool InsertPushSend(List<Guid> accountKeys,DateTime now)
        {
            if (accountKeys.Count <= 0)
            {
                return false;
            }
            bool result;
            try
            {
                var entities = new List<QH_PUSHSEND_LOG>();

                foreach (var accountKey in accountKeys)
                {
                    QhEventForeignKeyOfJson foreignKey = new QhEventForeignKeyOfJson();
                    QhTableReferenceOfJson tableReference = new QhTableReferenceOfJson();
                    QhTableColumnReferenceOfJson keyColumn = new QhTableColumnReferenceOfJson();
                    QhTableColumnReferenceOfJson keyColumn2 = new QhTableColumnReferenceOfJson();
                    keyColumn.Name = "ACCOUNTKEY";
                    keyColumn.Value = accountKey.ToString();
                    keyColumn2.Name = "CREATEDDATE";
                    keyColumn2.Value = now.ToString();
                    tableReference.Name = string.Empty;
                    tableReference.KeyColumnN.Add(keyColumn);
                    tableReference.KeyColumnN.Add(keyColumn2);
                    foreignKey.TableReferenceN.Add(tableReference);
                    QH_PUSHSEND_LOG entity = new QH_PUSHSEND_LOG
                    {
                        ACCOUNTKEY = accountKey,
                        PUSHDATE = now,
                        PUSHTYPE = 1,
                        CALLERSYSTEMNAME = "QolmsOpenApi PushSend",
                        FOREIGNKEY = new QolmsDbEntityV1.QsJsonSerializer().Serialize<QhEventForeignKeyOfJson>(foreignKey)
                    };
                    entities.Add(entity);
                }

                var writer = new PushSendLogWriter();
                PushSendLogWriterArgs writerArgs = new PushSendLogWriterArgs() { Entities = entities };

                return QsDbManager.Write(writer, writerArgs).IsSuccess;
            }
            catch(Exception e)
            {
                var m = e.Message;
                result = false;
            }

            return result;
        }
    }
}