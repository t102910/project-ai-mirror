using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker.Mail
{
    /// <summary>
    /// ID問い合わせメールを送信するための情報を格納する引数クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class NotificationSendMailClientArgs : QoNoticeClientArgsBase
    {
        /// <summary>
        /// デフォルト コンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        private NotificationSendMailClientArgs() : base()
        {
        }

        /// <summary>
        /// 値を指定して、
        /// <see cref="NotificationSendMailClientArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="settingsName"></param>
        /// <param name="titleString"></param>
        /// <param name="mailAddress"></param>
        /// <param name="bodyString"></param>
        /// <param name="footerTemplatePath"></param>
        public NotificationSendMailClientArgs(string settingsName, string titleString, string mailAddress, string bodyString, string footerTemplatePath) : base(settingsName, titleString, mailAddress, bodyString, footerTemplatePath)
        {
            if (!this.ToN.Any())
                throw new InvalidOperationException("受信者のリストに要素が含まれていません。");
            if (string.IsNullOrWhiteSpace(this.Subject))
                throw new ArgumentNullException("Subject", "件名が Null 参照もしくは空白です。");
            if (string.IsNullOrWhiteSpace(this.Body))
                throw new ArgumentNullException("Body", "本文が Null 参照もしくは空白です。");
        }

        /// <summary>
        /// <see cref="NotificationSendMailClientArgs" /> クラスのインスタンスを初期化します。
        /// このメソッドは基本クラスの引数付きコンストラクタ内で呼び出されます。
        /// </summary>
        /// <remarks></remarks>
        protected override void InitializeBase()
        {

        }

        /// <summary>
        /// <see cref="MailAddressSetNoticeClientArgs" /> クラスのインスタンスを初期化します。
        /// このメソッドは基本クラスの引数付きコンストラクタ内で呼び出されます。
        /// </summary>
        /// <remarks></remarks>
        protected override void InitializeBase(string settingsName, string subject, string mailToSettingsName, string bodyTemplatePath, string footerTemplatePath)
        {
            // 基本クラスのプロパティの初期化
            // 受信者のリストは初期化済みなので、その他のプロパティを初期化
            this.ToN = new List<string> { mailToSettingsName };
            this.SetSettingsNameFromAppSettings(settingsName);
            this.Subject = subject;
            this.Body = bodyTemplatePath;
            this.SetBodyFromTemplate(null, footerTemplatePath);
        }
    }
}