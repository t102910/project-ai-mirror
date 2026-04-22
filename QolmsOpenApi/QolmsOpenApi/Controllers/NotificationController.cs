using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Attribute;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Providers;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace MGF.QOLMS.QolmsOpenApi.Controllers
{
    /// <summary>
    /// お知らせ通知サービスを提供。
    /// </summary>
    public class NotificationController : QoApiControllerBase
    {
        /// <summary>
        /// 個人へのお知らせを送ります。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.Basic | QoApiAuthorizeTypeEnum.JwtQolmsApiKey)]
        [ActionName("Send")]
        public QoNotificationSendApiResults PostSend(QoNotificationSendApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, NotificationWorker.Send);
        }
        /// <summary>
        /// 特定関係者へのお知らせを送ります。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.Basic | QoApiAuthorizeTypeEnum.JwtQolmsApiKey)]
        [ActionName("SendAll")]
        public  QoNotificationSendAllApiResults PostSendAll(QoNotificationSendAllApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, NotificationWorker.SendAll);
        }

        /// <summary>
        /// 個人（患者ID）へのお知らせを送ります。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.Basic | QoApiAuthorizeTypeEnum.JwtQolmsApiKey)]
        [ActionName("SendFromPatientId")]
        public QoNotificationSendFromPatientIdApiResults PostSendFromPatientId(QoNotificationSendFromPatientIdApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, NotificationWorker.SendFromPatientId);
        }

        /// <summary>
        /// お知らせリストを取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Notification)]
        [ActionName("Read")]
        public QoNotificationReadApiResults PostRead(QoNotificationReadApiArgs args)
        {
            var worker = new NotificationReadWorker(new NoticeRepository(), new LinkageRepository());
            return this.ExecuteWorkerMethod(args, worker.Read);
        }
        /// <summary>
        /// 個人あてメッセージリストを取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Notification)]
        [ActionName("PersonalRead")]
        public QoNotificationPersonalReadApiResults PostPersonalRead(QoNotificationPersonalReadApiArgs args)
        {
            var worker = new NotificationPersonalReadWorker(new NoticePersonalRepository(), new FamilyRepository());
            return this.ExecuteWorkerMethod(args, worker.Read);

        }

        /// <summary>
        /// 画像を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Notification)]
        [ActionName("ImageRead")]
        public QoNotificationImageReadApiResults PostImageRead(QoNotificationImageReadApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, NotificationWorker.ImageRead);

        }

        /// <summary>
        /// 既読の書き込みをします。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Notification)]
        [ActionName("PersonalReadReceiptWrite")]
        public QoNotificationPersonalReadReceiptWriteApiResults PostPersonalReadReceiptWrite(QoNotificationPersonalReadReceiptWriteApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, NotificationWorker.PersonalReadReceiptWrite);

        }

        /// <summary>
        /// メールを送ります。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey)]
        [ActionName("SendMail")]
        public QoNotificationSendMailApiResults PostNotificationSendMail(QoNotificationSendMailApiArgs args)
        {
            var worker = new NotificationSendMailApiWorker();

            return this.ExecuteWorkerMethod(args, worker.NotificationSendMail);
        }

        /// <summary>
        /// グループへお知らせを送ります。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.Basic | QoApiAuthorizeTypeEnum.JwtQolmsApiKey)]
        [ActionName("SendGroup")]
        public QoNotificationGroupApiResults PostSendGroup(QoNotificationGroupApiArgs args)
        {

            var worker = new NotificationGroupWorker(
                new NoticeGroupRepository(),
                new QoPushNotification(),
                new DateTimeProvider()
            );

            return this.ExecuteWorkerMethod(args, worker.SendGroup);
        }

    }
}
