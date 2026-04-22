
using System.Web.Http;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsOpenApi.Attribute;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Worker;
using MGF.QOLMS.QolmsJwtAuthCore;
using System;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Providers;

namespace MGF.QOLMS.QolmsOpenApi.Controllers
{
    /// <summary>
    /// JOTO（宜野湾）関連の公開API
    /// </summary>
    public class PushController : QoApiControllerBase
    {
        
        /// <summary>
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtJotoApiKey,QoApiFunctionTypeEnum.JotoHdrAppUser)]
        [ActionName("Send")]
        public QoPushSendApiResults PostSend(QoPushSendApiArgs args)
        {
            var worker = new PushWorker(new LinkageRepository(),new ReproRepository(),new NoticeGroupRepository(), new QoPushNotification(), new DateTimeProvider());
            return this.ExecuteWorkerMethod(args, worker.Send);
        }

        /// <summary>
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtJotoApiKey, QoApiFunctionTypeEnum.JotoHdrAppUser)]
        [ActionName("SendAccount")]
        public QoPushSendAccountApiResults PostSendAccount(QoPushSendAccountApiArgs args)
        {
            var worker = new PushWorker(new LinkageRepository(), new ReproRepository(), new NoticeGroupRepository(), new QoPushNotification(), new DateTimeProvider());
            return this.ExecuteWorkerMethod(args, worker.SendAccount);
        }
    }


}
