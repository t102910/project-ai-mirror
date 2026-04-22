using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker.Mail
{
    /// <summary>
    /// ユーザへの お問い合わせ受理メール を送信するための情報を格納する引数クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class SupportAcceptanceNoticeClientArgs : QoNoticeClientArgsBase
    {
        /// <summary>
        /// デフォルト コンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        private SupportAcceptanceNoticeClientArgs() : base() {}

        /// <summary>
        /// 値を指定して、
        /// <see cref="SupportAcceptanceNoticeClientArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="settingsName">NoticeApi用の設定名。</param>
        /// <param name="mailAddress">メールアドレス。</param>
        /// <param name="inquiryNo">お問い合わせ番号。</param>
        /// <param name="contents">お問い合わせ内容。</param>
        /// <param name="bodyTemplatePath">メール本文のパス</param>
        /// <param name="footerTemplatePath">メールフッターのパス</param>
        /// <remarks></remarks>
        public SupportAcceptanceNoticeClientArgs(string settingsName, string mailAddress, string inquiryNo, string contents, string bodyTemplatePath, string footerTemplatePath) : base(settingsName, new List<string>() {mailAddress}, bodyTemplatePath, footerTemplatePath)
        {
            if (!this.ToN.Any())
                throw new InvalidOperationException("受信者のリストに要素が含まれていません。");
            if (string.IsNullOrWhiteSpace(this.Subject))
                throw new ArgumentNullException("Subject", "件名が Null 参照もしくは空白です。");
            if (string.IsNullOrWhiteSpace(this.Body))
                throw new ArgumentNullException("Body", "本文が Null 参照もしくは空白です。");
            if (string.IsNullOrWhiteSpace(inquiryNo))
                throw new ArgumentNullException("inquiryNo", "お知らせ番号が Null 参照もしくは空白です。");

            // 本文内のパラメータを置換
            this.Body = string.Format(this.Body, inquiryNo, contents);
        }

        /// <summary>
        /// <see cref="SupportAcceptanceNoticeClientArgs" /> クラスのインスタンスを初期化します。
        /// このメソッドは基本クラスの引数付きコンストラクタ内で呼び出されます。
        /// </summary>
        /// <remarks></remarks>
        protected override void InitializeBase()
        {
            // 基本クラスのプロパティの初期化
            // 受信者のリストは初期化済みなので、その他のプロパティを初期化
            this.SetSettingsNameFromAppSettings("MailSettingsNamePersonalMessage");
            this.SetSubjectFromAppSettings("MailSubjectSupportAcceptance");
            this.SetBodyFromTemplate(HttpContext.Current.Server.MapPath("~/App_Data/MailBodySupportAcceptance.txt"), HttpContext.Current.Server.MapPath("~/App_Data/MailFooter.txt"));
        }

        /// <summary>
        /// <see cref="SupportAcceptanceNoticeClientArgs" /> クラスのインスタンスを初期化します。
        /// このメソッドは基本クラスの引数付きコンストラクタ内で呼び出されます。
        /// </summary>
        /// <remarks></remarks>
        protected override void InitializeBase(string settingsName, string bodyTemplatePath, string footerTemplatePath)
        {
            // 基本クラスのプロパティの初期化
            // 受信者のリストは初期化済みなので、その他のプロパティを初期化
            this.SetSettingsNameFromAppSettings(settingsName);
            this.SetSubjectFromAppSettings("MailSubjectSupportAcceptance");
            try
            {
                this.SetBodyFromTemplate(bodyTemplatePath, footerTemplatePath);
            }
            catch
            {
                // Body = "メールテンプレートが設定されていません。パラメータ:{0}/{1}";
                InitializeBase();
            }
        }
    }
}