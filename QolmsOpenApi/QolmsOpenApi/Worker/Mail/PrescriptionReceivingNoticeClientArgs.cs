using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker.Mail
{
    /// <summary>
    /// 薬局へ処方せん受信の通知メールを送信するための情報を格納する引数クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class PrescriptionReceivingNoticeClientArgs : QoNoticeClientArgsBase
    {


        /// <summary>
        /// デフォルト コンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        private PrescriptionReceivingNoticeClientArgs() : base()
        {
        }

        /// <summary>
        /// 値を指定して、
        /// <see cref="PrescriptionReceivingNoticeClientArgs" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="facilityName">薬局名。</param>
        /// <param name="mailAddress">処方せん受信通知先メールアドレス。</param>
        /// <remarks></remarks>
        public PrescriptionReceivingNoticeClientArgs(string facilityName, List<string> mailAddress) : base(mailAddress)
        {
            if (!this.ToN.Any())
                throw new InvalidOperationException("受信者のリストに要素が含まれていません。");
            if (string.IsNullOrWhiteSpace(this.Subject))
                throw new ArgumentNullException("Subject", "件名が Null 参照もしくは空白です。");
            if (string.IsNullOrWhiteSpace(this.Body))
                throw new ArgumentNullException("Body", "本文が Null 参照もしくは空白です。");

            // 件名、本文内のパラメータを置換
            this.Subject = string.Format(this.Subject, facilityName);
            this.Body = string.Format(this.Body, facilityName);
        }



        /// <summary>
        /// <see cref="PrescriptionReceivingNoticeClientArgs" /> クラスのインスタンスを初期化します。
        /// このメソッドは基本クラスの引数付きコンストラクタ内で呼び出されます。
        /// </summary>
        /// <remarks></remarks>
        protected  override void InitializeBase()
        {

            // 基本クラスのプロパティの初期化
            // 受信者のリストは初期化済みなので、その他のプロパティを初期化
            this.SetSettingsNameFromAppSettings("MailSettingsNamePrescriptionStatusChange");
            this.SetSubjectFromAppSettings("MailSubjectPrescriptionReceiving");
            this.SetBodyFromTemplate(HttpContext.Current.Server.MapPath("~/App_Data/MailBodyPrescriptionReceiving.txt"), string.Empty);
            
        }
    }


}