using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker.Mail
{
    /// <summary>
    /// 処方箋のステータス変更メールを送信するための情報を格納する引数クラスを表します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class PrescriptionStatusChangeNoticeClientArgs : QoNoticeClientArgsBase
    {


        /// <summary>
        ///  デフォルト コンストラクタは使用できません。
        ///  </summary>
        ///  <remarks></remarks>
        private PrescriptionStatusChangeNoticeClientArgs() : base()
        {
        }

        /// <summary>
        ///  値を指定して、
        ///  <see cref="PrescriptionStatusChangeNoticeClientArgs" /> クラスの新しいインスタンスを初期化します。
        ///  </summary>
        ///  <param name="mailAddress">ステータス変更通知先メール アドレス。</param>
        ///  <param name="statusCode">処方せんステータスコード</param>
        ///  <param name="cancelReason">取消理由</param>
        ///  <remarks></remarks>
        public PrescriptionStatusChangeNoticeClientArgs(string mailAddress, string statusCode, string cancelReason) : base(new List<string>() { mailAddress })
        {
            this.CreateSubjectAndBody(statusCode);

            if (!this.ToN.Any())
                throw new InvalidOperationException("受信者のリストに要素が含まれていません。");
            if (string.IsNullOrWhiteSpace(this.Subject))
                throw new ArgumentNullException("Subject", "件名が Null 参照もしくは空白です。");
            if (string.IsNullOrWhiteSpace(this.Body))
                throw new ArgumentNullException("Body", "本文が Null 参照もしくは空白です。");

            if (string.IsNullOrWhiteSpace(statusCode))
                throw new ArgumentNullException("status", "処方箋ステータス が Null 参照もしくは空白です。");

            // 本文内のパラメータを置換
            this.Body = string.Format(this.Body, cancelReason);
        }



        /// <summary>
        ///  <see cref="PrescriptionStatusChangeNoticeClientArgs" /> クラスのインスタンスを初期化します。
        ///  このメソッドは基本クラスの引数付きコンストラクタ内で呼び出されます。
        ///  </summary>
        ///  <remarks></remarks>
        protected  override void InitializeBase()
        {

            // 基本クラスのプロパティの初期化
            // 受信者のリストは初期化済みなので、その他のプロパティを初期化
            this.SetSettingsNameFromAppSettings("MailSettingsNamePrescriptionStatusChange");
        }



        /// <summary>
        ///  処方せんステータスコードからメール件名とメール本文を生成します。
        ///  </summary>
        ///  <param name="statusCode">ステータスコード</param>
        ///  <remarks></remarks>
        private void CreateSubjectAndBody(string statusCode)
        {
            string subjectPath = string.Empty;
            string bodyPath = string.Empty;

            switch (statusCode)
            {
                case "1"    // 「処方せん受信」
               :
                    {
                        subjectPath = "MailSubjectPrescriptionSent";
                        bodyPath = "~/App_Data/MailBodyPrescriptionSent.txt";
                        break;
                    }

                case "2"    // 「確認・保留中」
         :
                    {
                        break;
                    }

                case "4"    // 「調剤中」
       :
                    {
                        break;
                    }

                case "6"    // 「調剤完了」
       :
                    {
                        subjectPath = "MailSubjectPrescriptionDispensed";
                        bodyPath = "~/App_Data/MailBodyPrescriptionDispensed.txt";
                        break;
                    }

                case "8"    // 「お渡し済み」
         :
                    {
                        break;
                    }

                case "9"    // 「受付不可」
       :
                    {
                        subjectPath = "MailSubjectPrescriptionCanceled";
                        bodyPath = "~/App_Data/MailBodyPrescriptionCanceled.txt";
                        break;
                    }

                default:
                    {
                        throw new ArgumentException("StatusCode", "ステータスコードが不正です。");
                    }
            }

            if (!string.IsNullOrWhiteSpace(subjectPath) && !string.IsNullOrWhiteSpace(bodyPath))
            {
                this.SetSubjectFromAppSettings(subjectPath);
                this.SetBodyFromTemplate(HttpContext.Current.Server.MapPath(bodyPath), HttpContext.Current.Server.MapPath("~/App_Data/MailFooter.txt"));
            }
        }
    }
    
}