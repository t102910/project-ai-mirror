using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsOpenApi.Worker.Mail;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// メール送信
    /// </summary>
    public class NotificationSendMailApiWorker
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoNotificationSendMailApiResults NotificationSendMail(QoNotificationSendMailApiArgs args)
        {
            var results = new QoNotificationSendMailApiResults
            {
                IsSuccess = bool.FalseString,
                ResultN = new List<QoSendMailResultItem>()
            };

            //送信対象情報がない場合
            if (args.SendMailN.Count == 0)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"送信対象情報が0件です。");

                return results;
            }

            // システム種別の取得
            var systemType = args.ExecuteSystemType.TryToValueType(QsApiSystemTypeEnum.None);
            string param = systemType.ToUrlParam();
            if (string.IsNullOrEmpty(param))
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"メール送信非対応です。");

                return results;
            }

            string footerPath = string.Empty;
            try
            {
                footerPath = HttpContext.Current.Server.MapPath(string.Format("~/App_Data/MailFooter_{0}.txt", param));
                if (string.IsNullOrEmpty(footerPath))
                    throw new ArgumentNullException("Footer", "フッターテンプレートがありません。");
            }
            catch (Exception e)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{e.Message}");

                return results;
            }

            string settingsName = string.Format("MailSettingsNamePersonalMessage_{0}", param);

            foreach (var item in args.SendMailN)
            { 
                var result = new QoSendMailResultItem() { MailAddress = item.Mail };

                try
                {
                    if (string.IsNullOrEmpty(item.Mail))
                    {
                        result.Result = "メールアドレスが空です";
                        result.IsSuccess = bool.FalseString;
                    }
                    else
                    {
                        var res = Task.Run(() =>
                        {
                            return SendPersonalMessageNoticeMail(settingsName, item.Title, item.Mail, item.Message, footerPath);
                        }).GetAwaiter().GetResult();

                        result.IsSuccess = res.ToString();
                        result.Result = res ? "メール送信成功" : "メール送信失敗";
                    }
                }
                catch(Exception e)
                {
                    result.IsSuccess = bool.FalseString;
                    result.Result = e.Message;
                }

                results.ResultN.Add(result);
            }

            // 全て成功
            if (results.ResultN.All(x => x.IsSuccess == bool.TrueString))
            {
                results.IsSuccess = bool.TrueString;
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success, $"メール送信にすべて成功しました。");
            }
            // すべて失敗
            else if (results.ResultN.All(x => x.IsSuccess == bool.FalseString))
            {
                results.IsSuccess = bool.FalseString;
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, $"メール送信にすべて失敗しました。");
            }
            // 一部成功
            else if (results.ResultN.Any(x => x.IsSuccess == bool.TrueString))
            {
                results.IsSuccess = bool.TrueString;
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success, $"メール送信しましたが、失敗したものがあります。");
            }

            return results;
        }

        /// <summary>
        /// メールを送信します
        /// </summary>
        /// <param name="settingsName"></param>
        /// <param name="titleString"></param>
        /// <param name="mailAddress"></param>
        /// <param name="bodyString"></param>
        /// <param name="footerPath"></param>
        /// <returns></returns>
        private static async Task<bool> SendPersonalMessageNoticeMail(string settingsName, string titleString, string mailAddress, string bodyString, string footerPath)
        {
            return await new NotificationSendMailClient(new NotificationSendMailClientArgs(settingsName, titleString, mailAddress, bodyString, footerPath)).SendAsync();
        }

    }
}