using MGF.QOLMS.QolmsKddiMessageCastApiCoreV1F472;
using MGF.QOLMS.QolmsKddiMessageCastApiCoreV1F472.API.messages;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// SMSクライアント インターフェース
    /// </summary>
    public interface IQoSmsClient
    {
        /// <summary>
        /// SMSを送信します。失敗時は例外を投げる。
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SendSms(string phoneNumber, string message);
    }

    /// <summary>
    ///  SMSクライアント 実装
    /// </summary>
    public class QoSmsClient: IQoSmsClient
    {
        // 認証コード使用文字列 0-9
        static readonly char[] CodeChars = "0123456789".ToCharArray();

        /// <summary>
        /// SMSを送信します。
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendSms(string phoneNumber, string message)
        {
            try
            {
                var botId = ConfigurationManager.AppSettings["BotId"];
                var BasicId = ConfigurationManager.AppSettings["BasicId"];
                var BasicPassword = ConfigurationManager.AppSettings["BasicPassword"];
                var SmsApiUri = ConfigurationManager.AppSettings["SmsApiUri"];
                var ApiHost = ConfigurationManager.AppSettings["ApiHost"];
                var AggId = ConfigurationManager.AppSettings["AggId"];
                var SmsNumber = ConfigurationManager.AppSettings["SmsNumber"];

                var configMessage = $"Config / BotId:{botId} BasicId:{BasicId} BasicPassword:{BasicPassword} SmsApiUri:{SmsApiUri} ApiHost:{ApiHost} AggId:{AggId} SmsNumber:{SmsNumber}";

                // 調査用 Config値がとれているか
                QoAccessLog.WriteAccessLog(QolmsApiCoreV1.QsApiSystemTypeEnum.QolmsOpenApi, Guid.Empty, DateTime.Now, QoAccessLog.AccessTypeEnum.Api, "SmsClient", configMessage, null, null, null);

                var args = new MessagesApiArgs(phoneNumber, message);               

                var results = await QsSmsApiManager.ExecuteAsync<MessagesApiArgs, MessagesApiResults>(args);

                if (results == null)
                {
                    QoAccessLog.WriteAccessLog(QolmsApiCoreV1.QsApiSystemTypeEnum.QolmsOpenApi, Guid.Empty, DateTime.Now, QoAccessLog.AccessTypeEnum.Api, "SmsClient", "結果がnullです。", null, null, null);
                    throw new Exception("結果を取得できません。");
                }

                if (!results.IsSuccess)
                {                   
                    var errorBuilder = new StringBuilder();

                    errorBuilder.AppendLine($"{results.title} {results.type}");
                    foreach(var msg in results.detail.messages)
                    {
                        errorBuilder.AppendLine($"code:{msg.message_code} message:{msg.message}");
                    }


                    var errorMessage = $"StatusCode:{results.StatusCode} Body:{results.ResponseString} エラー詳細:{errorBuilder}";

                    QoAccessLog.WriteAccessLog(QolmsApiCoreV1.QsApiSystemTypeEnum.QolmsOpenApi, Guid.Empty, DateTime.Now, QoAccessLog.AccessTypeEnum.Api, "SmsClient", errorMessage, null, null, null);

                    throw new Exception(errorMessage);                    
                }

                QoAccessLog.WriteAccessLog(QolmsApiCoreV1.QsApiSystemTypeEnum.None, Guid.Empty, DateTime.Now, QoAccessLog.AccessTypeEnum.Api, "SmsClient", "SMS送信成功", null, null, null);

            }
            catch(Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, $"SMSの送信に失敗しました。{ex.Message}", Guid.Empty);
                throw ex;
            }
        }

        /// <summary>
        /// SMS認証コードを生成する
        /// </summary>
        /// <param name="length">桁数</param>
        /// <returns></returns>
        public static string GenerateAuthCode(int length)
        {
            char[] buffer = new char[length];

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                for (int i = 0; i < length; i++)
                {
                    byte[] randomNumber = new byte[1];
                    rng.GetBytes(randomNumber);

                    // Get a random index into our array of valid characters
                    int index = randomNumber[0] % CodeChars.Length;
                    buffer[i] = CodeChars[index];
                }
            }

            return new string(buffer);
        }
    }
}