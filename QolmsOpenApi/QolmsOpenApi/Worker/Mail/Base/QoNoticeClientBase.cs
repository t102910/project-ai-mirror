using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsNoticeApiCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.Worker.Mail
{
    /// <summary>
    /// メールを送信するための基本クラスを表します。
    /// </summary>
    /// <typeparam name="TArgs">メールを送信するための情報を格納する引数クラスの型。</typeparam>
    /// <remarks></remarks>
    internal abstract class QoNoticeClientBase<TArgs> where TArgs : QoNoticeClientArgsBase
    {


        /// <summary>
        /// メールを送信するための情報を保持します。
        /// </summary>
        /// <remarks></remarks>
        private TArgs noticeArgs = null;



        /// <summary>
        /// クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        protected QoNoticeClientBase()
        {
        }

        /// <summary>
        /// メールを送信するための情報を指定して、
        /// クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="noticeArgs">メールを送信するための情報。</param>
        /// <remarks></remarks>
        public QoNoticeClientBase(TArgs noticeArgs)
        {
            this.noticeArgs = noticeArgs;
        }



        /// <summary>
        /// 「QOLMS 通知 API」を実行します。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>
        /// Web API 戻り値クラス。
        /// </returns>
        /// <remarks></remarks>
        private QnMailSendApiResults ExecuteQolmsNoticeApi(QnMailSendApiArgs args)
        {
            QnMailSendApiResults result = new QnMailSendApiResults() { IsSuccess = bool.FalseString };

            try
            {
                QoAccessLog.WriteAccessLog(QsApiSystemTypeEnum.QolmsOpenApi, args.Executor.TryToValueType(Guid.Empty),
                    DateTime.Now, QoAccessLog.AccessTypeEnum.Api, "QolmsNoticeMail/Send",
                    "SendMail:" + string.Join(",", args.CcN),string.Empty,string.Empty,string.Empty ) ;
                result = QsNoticeApiManager.ExecuteQolmsNoticeApi<QnMailSendApiResults>(args);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                QoAccessLog.WriteErrorLog(ex, args.Executor.TryToValueType(Guid.Empty));
            }

            return result;
        }



        /// <summary>
        /// 同期でメールを送信します。
        /// </summary>
        /// <returns>
        /// 成功なら True、
        /// 失敗なら False。
        /// </returns>
        /// <remarks></remarks>
        public bool Send()
        {
            bool result = false;

            if (this.noticeArgs != null && !string.IsNullOrWhiteSpace(this.noticeArgs.SettingsName) && !string.IsNullOrWhiteSpace(this.noticeArgs.Subject) && this.noticeArgs.ToN.Any() && !string.IsNullOrWhiteSpace(this.noticeArgs.Body))
            {
                QnMailSendApiArgs args = new QnMailSendApiArgs()
                {
                    ApiType = QnApiTypeEnum.MailSend.ToString(),
                    ExecuteSystemType = QsApiSystemTypeEnum.Qolms.ToString(),
                    Executor = Guid.Empty.ToApiGuidString(),
                    ExecutorName = "QOLMS",
                    ExecuteApplicationType = "None",
                    NoticeSettingsName = this.noticeArgs.SettingsName,
                    ToN = this.noticeArgs.ToN,
                    Subject = this.noticeArgs.Subject,
                    Body = this.noticeArgs.Body.Trim()
                };

                result = this.ExecuteQolmsNoticeApi(args).IsSuccess.TryToValueType(false);
            }

            return result;
        }

        /// <summary>
        /// 非同期でメールを送信します。
        /// </summary>
        /// <returns>
        /// タスク。
        /// </returns>
        /// <remarks></remarks>
        public async Task<bool> SendAsync()
        {
            return await Task.Run<bool>(() =>
            {
                return this.Send();
            }
    );
        }
    }

}