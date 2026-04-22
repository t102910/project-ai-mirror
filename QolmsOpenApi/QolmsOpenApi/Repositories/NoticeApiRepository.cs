using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsOpenApi.Worker;
using MGF.QOLMS.QolmsOpenApi.Worker.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Repositories
{
    /// <summary>
    /// NoticeApiの連携インターフェース
    /// 現時点ではテスト容易性のためのWorker.Mailにあるクラスのラッパー
    /// </summary>
    public interface INoticeApiRepository
    {
        /// <summary>
        /// ユーザーID案内メールを送信する
        /// </summary>
        /// <param name="mailAddress"></param>
        /// <param name="userId"></param>
        /// <param name="systemType"></param>
        /// <returns></returns>
        Task<bool> SendIdNotificationMail(string mailAddress, string userId, QsApiSystemTypeEnum systemType);
    }

    /// <summary>
    /// NoticeApiの連携実装
    /// 現時点ではテスト容易性のためのWorker.Mailにあるクラスのラッパー
    /// </summary>
    public class NoticeApiRepository: INoticeApiRepository
    {
        /// <summary>
        /// ユーザーID案内メールを送信する
        /// </summary>
        /// <param name="mailAddress"></param>
        /// <param name="userId"></param>
        /// <param name="systemType"></param>
        /// <returns></returns>
        public Task<bool> SendIdNotificationMail(string mailAddress, string userId, QsApiSystemTypeEnum systemType)
        {
            var param = AppWorker.GetUrlParam(systemType);

            string bodyPath = string.Format("~/App_Data/MailBodyMailAddressSet_{0}.txt", param);
            string footerPath = string.Format("~/App_Data/MailFooter_{0}.txt", param);
            string settingsName = string.Format("MailSettingsNameMailAddressSet_{0}", param);
            string subject = string.Format("MailSubjectMailAddressSet_{0}", param);
            return new MailAddressSetNoticeClient(new MailAddressSetNoticeClientArgs(settingsName, subject, mailAddress, userId, bodyPath, footerPath)).SendAsync();
        }
    }
}