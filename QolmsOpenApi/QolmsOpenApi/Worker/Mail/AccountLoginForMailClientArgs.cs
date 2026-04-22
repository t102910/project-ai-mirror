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
    internal sealed class AccountLoginForMailClientArgs : QoNoticeClientArgsBase
    {


        /// <summary>
        /// デフォルト コンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        private AccountLoginForMailClientArgs() : base()
        {
        }

        /// <summary>
        /// 値を指定して、
        /// <see cref="AccountLoginForMailClientArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="settingsName">NoticeApi用の設定名。</param>
        /// <param name="mailAddress">送信先メールアドレス（受信者）の設定名。</param>
        /// <param name="passCode">パスコード。</param>
        /// <param name="bodyTemplatePath">本文テンプレート ファイルのパス。</param>
        /// <param name="footerTemplatePath">フッター テンプレート ファイルのパス。</param>
        /// <param name="subjectName">件名の設定名。</param>
        /// <remarks></remarks>
        public AccountLoginForMailClientArgs(string settingsName, string subjectName, string mailAddress, string passCode, string bodyTemplatePath, string footerTemplatePath) : base(settingsName, subjectName, new List<string>() { mailAddress }, bodyTemplatePath, footerTemplatePath)
        {

            if (!this.ToN.Any())
                throw new InvalidOperationException("受信者のリストに要素が含まれていません。");
            if (string.IsNullOrWhiteSpace(this.Subject))
                throw new ArgumentNullException("Subject", "件名が Null 参照もしくは空白です。");
            if (string.IsNullOrWhiteSpace(this.Body))
                throw new ArgumentNullException("Body", "本文が Null 参照もしくは空白です。");

            if (string.IsNullOrWhiteSpace(passCode))
                throw new ArgumentNullException("AccountLoginPass", "passCode が Null 参照もしくは空白です。");

            // 本文内のパラメータを置換
            this.Body = string.Format(this.Body, passCode);

        }

        /// <summary>
        /// <see cref="AccountLoginForMailClientArgs" /> クラスのインスタンスを初期化します。
        /// このメソッドは基本クラスの引数付きコンストラクタ内で呼び出されます。
        /// </summary>
        /// <remarks></remarks>
        protected override void InitializeBase()
        {
            // 基本クラスのプロパティの初期化
            this.SetSettingsNameFromAppSettings("MailSettingsNameAccountLoginPass");
            this.SetSubjectFromAppSettings("MailSubjectAccountLoginPass");
            try
            {
                this.SetBodyFromTemplate(HttpContext.Current.Server.MapPath("~/App_Data/MailBodyAccountLoginPass.txt"), HttpContext.Current.Server.MapPath("~/App_Data/MailFooter.txt"));
            }
            catch (Exception)
            {
                Body = "メールテンプレートが設定されていません。パラメータ:{0}/{1}";
            }
        }

        /// <summary>
        /// <see cref="AccountLoginForMailClientArgs" /> クラスのインスタンスを初期化します。
        /// このメソッドは基本クラスの引数付きコンストラクタ内で呼び出されます。
        /// </summary>
        /// <param name="settingsName">NoticeApi用の設定名。</param>
        /// <param name="bodyTemplatePath">本文テンプレート ファイルのパス。</param>
        /// <param name="footerTemplatePath">フッター テンプレート ファイルのパス。</param>
        /// <param name="subjectName">件名の設定名。</param>
        protected override void InitializeBase(string settingsName, string subjectName, string bodyTemplatePath, string footerTemplatePath)
        {
            // 基本クラスのプロパティの初期化
            // 受信者のリストは初期化済みなので、その他のプロパティを初期化
            this.SetSettingsNameFromAppSettings(settingsName);
            this.SetSubjectFromAppSettings(subjectName);
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