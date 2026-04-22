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
    /// 外部サービス連携情報に関する機能を提供する API コントローラ です。
    /// </summary>
    public class ExternalServiceController : QoApiControllerBase
    {
        /// <summary>
        /// 外部サービス連携情報を保存します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtQolmsApiKey, QoApiFunctionTypeEnum.Tis)]
        [ActionName("Write")]
        public async Task<QoExternalServiceWriteApiResults> Write(QoExternalServiceWriteApiArgs args)
        {
            var worker = new ExternalServiceWriteWorker(new LinkageRepository(), new ExternalServiceLinkageRepository());
            return this.ExecuteWorkerMethod(args, worker.Write);
        }

        /// <summary>
        /// 外部サービス SPHR連携 対象者情報 を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtQolmsApiKey, QoApiFunctionTypeEnum.Tis)]
        [ActionName("SphrLinkageRead")]
        public async Task<QoExternalServiceSphrLinkageReadApiResults> PostSphrLinkageRead(QoExternalServiceSphrLinkageReadApiArgs args)
        {
            var worker = new ExternalServiceSphrLinkageReadWorker(new ExternalServiceLinkageRepository());
            return this.ExecuteWorkerMethod(args, worker.LinkageRead);
        }

        /// <summary>
        /// 外部サービス システム連携情報 を登録します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey)]
        [ActionName("SphrLinkageWrite")]
        public QoExternalServiceSphrLinkageWriteApiResults PostSphrLinkageWrite(QoExternalServiceSphrLinkageWriteApiArgs args)
        {
            var worker = new ExternalServiceSphrLinkageReadWorker(new ExternalServiceLinkageRepository());
            return this.ExecuteWorkerMethod(args, worker.LinkageWrite);
        }

    }
}