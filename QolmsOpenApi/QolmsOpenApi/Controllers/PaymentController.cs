using System.Threading.Tasks;
using System.Web.Http;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Attribute;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;

namespace MGF.QOLMS.QolmsOpenApi.Controllers
{
    /// <summary>
    /// 支払いに関する機能を提供するAPIコントローラー
    /// </summary>
    public class PaymentController: QoApiControllerBase
    {
        /// <summary>
        /// アルメックス社 後払い会計 SmapaWeb のホームのURLを取します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey)]
        [ActionName("SmapaHome")]
        public async Task<QoPaymentSmapaHomeApiResults> PostSmapaHome(QoPaymentSmapaHomeApiArgs args)
        {
            var worker = new PaymentSmapaHomeWorker(new SmapaApiRepository());
            return await ExecuteWorkerMethodAsync(args, worker.GetSmapaHome).ConfigureAwait(false);
        }

        /// <summary>
        /// アルメックス社 後払い会計 SmapaWeb を退会します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtQolmsApiKey | QoApiAuthorizeTypeEnum.JwtAccessKey)]
        [ActionName("SmapaRevoke")]
        public async Task<QoPaymentSmapaRevokeApiResults> PostSmapaRevoke(QoPaymentSmapaRevokeApiArgs args)
        {
            var worker = new PaymentSmapaRevokeWorker(new SmapaApiRepository());
            return await ExecuteWorkerMethodAsync(args, worker.Revoke).ConfigureAwait(false);
        }

        /// <summary>
        /// アルメックス社 後払い会計 SmapaWeb の利用規約同意状況を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey)]
        [ActionName("SmapaTermsRead")]
        public QoPaymentSmapaTermsReadApiResults PostSmapaTermsRead(QoPaymentSmapaTermsReadApiArgs args)
        {
            var worker = new PaymentSmapaTermsReadWorker(new ExternalServiceLinkageRepository());
            return ExecuteWorkerMethod(args, worker.Read);
        }

        /// <summary>
        /// アルメックス社 後払い会計 SmapaWeb の利用規約同意状況を更新します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey)]
        [ActionName("SmapaTermsWrite")]
        public QoPaymentSmapaTermsWriteApiResults PostSmapaTermsWrite(QoPaymentSmapaTermsWriteApiArgs args)
        {
            var worker = new PaymentSmapaTermsWriteWorker(new ExternalServiceLinkageRepository());
            return ExecuteWorkerMethod(args, worker.Write);
        }
    }
}