using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker.Mail
{
    /// <summary>
    /// メールアドレス設定メール（UserIDお知らせ）を送信するための情報を格納する引数クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class MailAddressSetNoticeClientArgs : QoNoticeClientArgsBase
    {


        /// <summary>
        /// デフォルト コンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        private MailAddressSetNoticeClientArgs() : base()
        {
        }

        /// <summary>
        /// 値を指定して、
        /// <see cref="MailAddressSetNoticeClientArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="settingsName">NoticeApi用の設定名。</param>
        /// <param name="subject">件名の設定名。</param>
        /// <param name="mailAddress">アカウント登録通知先メール アドレス。</param>
        /// <param name="userId">アカウントuserId。</param>
        /// <param name="bodyTemplatePath"></param>
        /// <param name="footerTemplatePath"></param>
        /// <remarks></remarks>
        public MailAddressSetNoticeClientArgs(string settingsName, string subject, string mailAddress, string userId,string bodyTemplatePath, string footerTemplatePath) : base(settingsName, subject, new List<string>(){mailAddress},bodyTemplatePath ,footerTemplatePath )
        {
            if (!this.ToN.Any())
                throw new InvalidOperationException("受信者のリストに要素が含まれていません。");
            if (string.IsNullOrWhiteSpace(this.Subject))
                throw new ArgumentNullException("Subject", "件名が Null 参照もしくは空白です。");
            if (string.IsNullOrWhiteSpace(this.Body))
                throw new ArgumentNullException("Body", "本文が Null 参照もしくは空白です。");

            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException("userId", "userId が Null 参照もしくは空白です。");

            // 本文内のパラメータを置換
            this.Body = string.Format(this.Body, mailAddress, userId);
        }

        /// <summary>
        /// <see cref="MailAddressSetNoticeClientArgs" /> クラスのインスタンスを初期化します。
        /// このメソッドは基本クラスの引数付きコンストラクタ内で呼び出されます。
        /// </summary>
        /// <remarks></remarks>
        protected override void InitializeBase()
        {

            // 基本クラスのプロパティの初期化
            // 受信者のリストは初期化済みなので、その他のプロパティを初期化
            this.SetSettingsNameFromAppSettings("MailSettingsNameMailAddressSet");
            this.SetSubjectFromAppSettings("MailSubjectMailAddressSet");
            try
            {
                this.SetBodyFromTemplate(HttpContext.Current.Server.MapPath("~/App_Data/MailBodyMailAddressSet.txt"), HttpContext.Current.Server.MapPath("~/App_Data/MailFooter.txt"));
            }
            catch (Exception)
            {
                Body = "メールテンプレートが設定されていません。パラメータ:{0}/{1}";
            }
        }

        /// <summary>
        /// <see cref="MailAddressSetNoticeClientArgs" /> クラスのインスタンスを初期化します。
        /// このメソッドは基本クラスの引数付きコンストラクタ内で呼び出されます。
        /// </summary>
        /// <remarks></remarks>
        protected override void InitializeBase(string settingsName, string subject, string bodyTemplatePath, string footerTemplatePath)
        {

            // 基本クラスのプロパティの初期化
            // 受信者のリストは初期化済みなので、その他のプロパティを初期化
            this.SetSettingsNameFromAppSettings(settingsName);
            this.SetSubjectFromAppSettings(subject);
            try
            {
                this.SetBodyFromTemplate(HttpContext.Current.Server.MapPath(bodyTemplatePath), HttpContext.Current.Server.MapPath(footerTemplatePath));
            }
            catch (Exception)
            {
                // Body = "メールテンプレートが設定されていません。パラメータ:{0}/{1}";
                this.InitializeBase();
            }
        }
    }


}