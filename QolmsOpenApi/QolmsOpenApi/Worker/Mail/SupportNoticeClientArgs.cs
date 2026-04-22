using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker.Mail
{
    /// <summary>
    /// お問い合わせ登録メールを送信するための情報を格納する引数クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class SupportNoticeClientArgs : QoNoticeClientArgsBase
    {


        /// <summary>
        /// デフォルト コンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        private SupportNoticeClientArgs() : base()
        {
        }

        /// <summary>
        /// 値を指定して、
        /// <see cref="SupportNoticeClientArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="settingsName">NoticeApi用の設定名。</param>
        /// <param name="mailToSettingsName">送信先メールアドレス（受信者）の設定名。</param>
        /// <param name="inquiryNo">お問い合わせ番号。</param>
        /// <param name="bodyTemplatePath">本文テンプレート ファイルのパス。</param>
        /// <param name="footerTemplatePath">フッター テンプレート ファイルのパス。</param>
        /// <param name="subjectName">件名の設定名。</param>
        /// <remarks></remarks>
        public SupportNoticeClientArgs(string settingsName, string subjectName, string mailToSettingsName, string inquiryNo, string bodyTemplatePath, string footerTemplatePath) : base(settingsName, subjectName, mailToSettingsName, bodyTemplatePath, footerTemplatePath)
        {
            if (!this.ToN.Any())
                throw new InvalidOperationException("受信者のリストに要素が含まれていません。");
            if (string.IsNullOrWhiteSpace(this.Subject))
                throw new ArgumentNullException("Subject", "件名が Null 参照もしくは空白です。");
            if (string.IsNullOrWhiteSpace(this.Body))
                throw new ArgumentNullException("Body", "本文が Null 参照もしくは空白です。");

            // 本文内のパラメータを置換
            this.Body = string.Format(this.Body, inquiryNo);
        }

        /// <summary>
        /// <see cref="SupportNoticeClientArgs" /> クラスのインスタンスを初期化します。
        /// このメソッドは基本クラスの引数付きコンストラクタ内で呼び出されます。
        /// </summary>
        /// <remarks></remarks>
        protected override void InitializeBase()
        {
            // 基本クラスのプロパティの初期化
            this.SetSettingsNameFromAppSettings("MailSettingsNameSupport");
            this.SetSubjectFromAppSettings("MailSubjectSupport");
            this.SetSettingsNameToAppSettings("MailToSupport_qm");
            this.SetBodyFromTemplate(HttpContext.Current.Server.MapPath("~/App_Data/MailBodySupport.txt"), HttpContext.Current.Server.MapPath("~/App_Data/MailFooter.txt"));
        }

        /// <summary>
        /// <see cref="SupportNoticeClientArgs" /> クラスのインスタンスを初期化します。
        /// このメソッドは基本クラスの引数付きコンストラクタ内で呼び出されます。
        /// </summary>
        /// <param name="settingsName">NoticeApi用の設定名。</param>
        /// <param name="mailToSettingsName">送信先メールアドレス（受信者）の設定名。</param>
        /// <param name="bodyTemplatePath">本文テンプレート ファイルのパス。</param>
        /// <param name="footerTemplatePath">フッター テンプレート ファイルのパス。</param>
        /// <param name="subjectName">件名の設定名。</param>
        protected override void InitializeBase(string settingsName, string subjectName, string mailToSettingsName, string bodyTemplatePath, string footerTemplatePath)
        {
            // 基本クラスのプロパティの初期化
            this.SetSubjectFromAppSettings(subjectName);
            this.SetSettingsNameFromAppSettings(settingsName);
            this.SetSettingsNameToAppSettings(mailToSettingsName);
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