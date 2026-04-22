using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker.Mail
{
    /// <summary>
    /// お知らせメールを送信するための情報を格納する引数クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class PersonalMessageNoticeClientArgs : QoNoticeClientArgsBase
    {


        /// <summary>
        /// デフォルト コンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        private PersonalMessageNoticeClientArgs() : base()
        {
        }

        /// <summary>
        /// 値を指定して、
        /// <see cref="PersonalMessageNoticeClientArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="settingsName"></param>
        /// <param name="mailAddress"></param>
        /// <param name="url">リンク。</param>
        /// <param name="bodyTemplatePath"></param>
        /// <param name="footerTemplatePath"></param>
        /// <remarks></remarks>
        public PersonalMessageNoticeClientArgs(string settingsName, string mailAddress, string url, string bodyTemplatePath, string footerTemplatePath) : base(settingsName,new List<string>() { mailAddress }, bodyTemplatePath, footerTemplatePath)
        {
            if (!this.ToN.Any())
                throw new InvalidOperationException("受信者のリストに要素が含まれていません。");
            if (string.IsNullOrWhiteSpace(this.Subject))
                throw new ArgumentNullException("Subject", "件名が Null 参照もしくは空白です。");
            if (string.IsNullOrWhiteSpace(this.Body))
                throw new ArgumentNullException("Body", "本文が Null 参照もしくは空白です。");

            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException("url", "URLスキーム が Null 参照もしくは空白です。");



            // 本文内のパラメータを置換
            this.Body = string.Format(this.Body,  url);
        }


        /// <summary>
        /// <see cref="PersonalMessageNoticeClientArgs" /> クラスのインスタンスを初期化します。
        /// このメソッドは基本クラスの引数付きコンストラクタ内で呼び出されます。
        /// </summary>
        /// <remarks></remarks>
        protected override void InitializeBase()
        {

            // 基本クラスのプロパティの初期化
            // 受信者のリストは初期化済みなので、その他のプロパティを初期化
            this.SetSettingsNameFromAppSettings("MailSettingsNamePersonalMessage");
            this.SetSubjectFromAppSettings("MailSubjectPersonalMessage");
            try
            {
                this.SetBodyFromTemplate(HttpContext.Current.Server.MapPath("~/App_Data/MailBodyPersonalMessage.txt"), HttpContext.Current.Server.MapPath("~/App_Data/MailFooter.txt"));

            }
            catch (Exception)
            {
                InitializeBase();
            }
        }
        /// <summary>
        /// <see cref="PersonalMessageNoticeClientArgs" /> クラスのインスタンスを初期化します。
        /// このメソッドは基本クラスの引数付きコンストラクタ内で呼び出されます。
        /// </summary>
        /// <remarks></remarks>
        protected  override void InitializeBase(string settingsName, string bodyTemplatePath, string footerTemplatePath)
        {

            // 基本クラスのプロパティの初期化
            // 受信者のリストは初期化済みなので、その他のプロパティを初期化
            this.SetSettingsNameFromAppSettings(settingsName);
            this.SetSubjectFromAppSettings("MailSubjectPersonalMessage");
            try
            {
                this.SetBodyFromTemplate(bodyTemplatePath,footerTemplatePath );

            }
             catch (Exception)
            {
                InitializeBase();
            }
        }
    }

}