using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Attribute;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace MGF.QOLMS.QolmsOpenApi.Controllers
{
    /// <summary>
    /// 順番待ち受け情報に関する機能を提供する API コントローラ です。
    /// </summary>
    public class WaitingController: QoApiControllerBase
    {
        /// <summary>
        /// 順番待ち情報を取得します。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Waiting)]
        [ActionName("ListRead")]
        public QoWaitingListReadApiResults PostListRead(QoWaitingListReadApiArgs args)
        {
            var worker = new WaitingListReadWorker(new WaitingRepository(),new LinkageRepository());
            return ExecuteWorkerMethod(args, worker.Read);
        }

        /// <summary>
        /// 順番待ちの情報を保存します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtQolmsApiKey, QoApiFunctionTypeEnum.Waiting)]
        [ActionName("ListWrite")]
        public async Task<QoWaitingListWriteApiResults> PostListWrite(QoWaitingListWriteApiArgs args)
        {
            var worker = new WaitingListWriteWorker(new WaitingRepository(), new LinkageRepository(), new QoPushNotification(), new FacilityRepository());
            return await this.ExecuteWorkerMethodAsync(args, worker.ListWrite).ConfigureAwait(false);
        }

        /// <summary>
        /// 順番待ち情報を書き込みます。（デバッグ用）
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Waiting)]
        [ActionName("ListDebugWrite")]
        public async Task<QoWaitingListDebugWriteApiResults> PostListDebugWrite(QoWaitingListDebugWriteApiArgs args)
        {
            var worker = new WaitingListDebugWriteWorker(new WaitingRepository(), new LinkageRepository(), new QoPushNotification(), new FacilityRepository());
            return await this.ExecuteWorkerMethodAsync(args, worker.ListDegugWrite).ConfigureAwait(false);
        }

        /// <summary>
        /// 順番待ちの待ち行列リストを取得します。（デバッグ用）
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Waiting)]
        [ActionName("OrderDebugRead")]
        public QoWaitingOrderDebugReadApiResults PostOrderDebugRead(QoWaitingOrderDebugReadApiArgs args)
        {
            var worker = new WaitingOrderDebugReadWorker(new WaitingRepository(), new LinkageRepository());
            return this.ExecuteWorkerMethod(args, worker.DebugRead);
        }

        /// <summary>
        /// プッシュ通知を送信します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtQolmsApiKey, QoApiFunctionTypeEnum.Waiting)]
        [ActionName("SendPush")]
        public async Task<QoWaitingListPushSendApiResults> PostSendPush(QoWaitingListPushSendApiArgs args)
        {
            var worker = new WaitingListWriteWorker(new WaitingRepository(), new LinkageRepository(), new QoPushNotification(), new FacilityRepository());
            return await this.ExecuteWorkerMethodAsync(args, worker.SendPush).ConfigureAwait(false);
        }
    }
}