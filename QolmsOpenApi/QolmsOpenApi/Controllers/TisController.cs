using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Attribute;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Worker;
using System.Web.Http;
using MGF.QOLMS.QolmsApiCoreV1;
using System.Threading.Tasks;
using MGF.QOLMS.QolmsOpenApi.Extension;
using Microsoft.Azure.NotificationHubs;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using System;

namespace MGF.QOLMS.QolmsOpenApi.Controllers
{
    /// <summary>
    /// TIS用の、TISでしか確実に使わないサービスを提供。
    /// </summary>
    public class TisController : QoApiControllerBase
    {
        /// <summary>
        /// 暗号化された文字列を受け取って、内容を返します。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [Obsolete("このAPIは廃止予定です。代わりにLinkage/QrCodeReadを使用してください。")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Tis | QoApiFunctionTypeEnum.Guest, true)]
        [ActionName("QrCodeRead")]        
        public QoTisQrCodeReadApiResults PostQrCodeRead(QoTisQrCodeReadApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, TisWorker.QrCodeRead);
        }

        /// <summary>
        /// 順番待ち情報を保存します。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtQolmsApiKey, QoApiFunctionTypeEnum.Tis)]
        [ActionName("WaitingListWrite")]
        [Obsolete("このAPIは廃止予定です。代わりにQoWaitingListWriteApiを使用してください。")]
        public QoTisWaitingListWriteApiResults PostTisWaitingListWrite(QoTisWaitingListWriteApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, TisWorker.WaitingListWrite);
        }

        /// <summary>
        /// 順番待ち情報を取得します。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Tis)]
        [ActionName("WaitingListRead")]
        [Obsolete("廃止予定です。代わりにQoWaitingListReadApiを使用してください")]
        public QoTisWaitingListReadApiResults PostTisWaitingListRead(QoTisWaitingListReadApiArgs args)
        {
            var worker = new TisWaitingListReadWorker(new WaitingRepository());
            return ExecuteWorkerMethod(args, worker.ReadLatest);
        }

        /// <summary>
        /// 順番待ち情報を書き込みます。（デバッグ用）
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Tis)]
        [ActionName("WaitingListDebugWrite")]
        [Obsolete("廃止予定です。代わりにQoWaitingListDebugWriteApiを使用してください")]
        public QoTisWaitingListDebugWriteApiResults PostTisWaitingListDebugWrite(QoTisWaitingListDebugWriteApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, TisWorker.WaitingListDebugWrite);
        }
    }   
}
