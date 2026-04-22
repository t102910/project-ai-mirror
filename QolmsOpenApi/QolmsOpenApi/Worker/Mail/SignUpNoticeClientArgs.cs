using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker.Mail
{
    /// <summary>
    /// ''' アカウント登録メールを送信するための情報を格納する引数クラスを表します。
    /// ''' このクラスは継承できません。
    /// ''' </summary>
    /// ''' <remarks></remarks>
    internal sealed class SignUpNoticeClientArgs : QoNoticeClientArgsBase
    {


        /// <summary>
        /// デフォルト コンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        private SignUpNoticeClientArgs() : base()
        {
        }

        /// <summary>
        /// 値を指定して、
        /// <see cref="SignUpNoticeClientArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="mailAddress">アカウント登録通知先メール アドレス。</param>
        /// <param name="url">アカウント本登録用 URL。</param>
        /// <param name="bodyTemplatePath"></param>
        /// <param name="footerTemplatePath"></param>
        /// <remarks></remarks>
        public SignUpNoticeClientArgs(string mailAddress, string url, string bodyTemplatePath, string footerTemplatePath) : base(new List<string>() { mailAddress }, bodyTemplatePath,  footerTemplatePath)
        {
            if (!this.ToN.Any())
                throw new InvalidOperationException("受信者のリストに要素が含まれていません。");
            if (string.IsNullOrWhiteSpace(this.Subject))
                throw new ArgumentNullException("Subject", "件名が Null 参照もしくは空白です。");
            if (string.IsNullOrWhiteSpace(this.Body))
                throw new ArgumentNullException("Body", "本文が Null 参照もしくは空白です。");

            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException("url", "アカウント本登録用 URL が Null 参照もしくは空白です。");

            // 本文内のパラメータを置換
            this.Body = string.Format(this.Body, mailAddress, url);
        }

        /// <summary>
        /// 値を指定して、
        /// <see cref="SignUpNoticeClientArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="settingsName">指定SendGrid設定値。</param>
        /// <param name="subject">指定件名設定値。</param>
        /// <param name="mailAddress">アカウント登録通知先メール アドレス。</param>
        /// <param name="url">アカウント本登録用 URL。</param>
        /// <param name="bodyTemplatePath"></param>
        /// <param name="footerTemplatePath"></param>
        /// <remarks></remarks>
        public SignUpNoticeClientArgs(string settingsName, string subject, string mailAddress, string url, string bodyTemplatePath, string footerTemplatePath) : base(settingsName, subject, new List<string>() { mailAddress }, bodyTemplatePath, footerTemplatePath)
        {
            if (!this.ToN.Any())
                throw new InvalidOperationException("受信者のリストに要素が含まれていません。");
            if (string.IsNullOrWhiteSpace(this.Subject))
                throw new ArgumentNullException("Subject", "件名が Null 参照もしくは空白です。");
            if (string.IsNullOrWhiteSpace(this.Body))
                throw new ArgumentNullException("Body", "本文が Null 参照もしくは空白です。");

            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException("url", "アカウント本登録用 URL が Null 参照もしくは空白です。");

            // 本文内のパラメータを置換
            this.Body = string.Format(this.Body, mailAddress, url);
        }

        /// <summary>
        /// <see cref="SignUpNoticeClientArgs" /> クラスのインスタンスを初期化します。
        /// このメソッドは基本クラスの引数付きコンストラクタ内で呼び出されます。
        /// </summary>
        /// <remarks></remarks>
        protected  override void InitializeBase()
        {

            // 基本クラスのプロパティの初期化
            // 受信者のリストは初期化済みなので、その他のプロパティを初期化
            this.SetSettingsNameFromAppSettings("MailSettingsNameSignUp");
            this.SetSubjectFromAppSettings("MailSubjectSignUp");
            try
            {
                this.SetBodyFromTemplate(HttpContext.Current.Server.MapPath("~/App_Data/MailBodySignUp.txt"), HttpContext.Current.Server.MapPath("~/App_Data/MailFooter.txt"));
            }
            catch (Exception)
            {
                Body = "メールテンプレートが設定されていません。パラメータ:{0}/{1}";
            }
        }

        /// <summary>
        /// <see cref="SignUpNoticeClientArgs" /> クラスのインスタンスを初期化します。
        /// このメソッドは基本クラスの引数付きコンストラクタ内で呼び出されます。
        /// </summary>
        /// <remarks></remarks>
        protected override void InitializeBase(string bodyTemplatePath, string footerTemplatePath)
        {

            // 基本クラスのプロパティの初期化
            // 受信者のリストは初期化済みなので、その他のプロパティを初期化
            this.SetSettingsNameFromAppSettings("MailSettingsNameSignUp");
            this.SetSubjectFromAppSettings("MailSubjectSignUp");
            try
            {
                this.SetBodyFromTemplate(HttpContext.Current.Server.MapPath(bodyTemplatePath), HttpContext.Current.Server.MapPath(footerTemplatePath));
            }
            catch (Exception)
            {
                InitializeBase();
            }
        }

        /// <summary>
        /// <see cref="SignUpNoticeClientArgs" /> クラスのインスタンスを初期化します。
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
                InitializeBase();
            }
        }
    }

}