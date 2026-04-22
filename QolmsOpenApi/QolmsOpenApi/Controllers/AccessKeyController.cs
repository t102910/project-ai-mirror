using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Attribute;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;
using System.Web.Http;


namespace MGF.QOLMS.QolmsOpenApi.Controllers
{
    /// <summary>
    /// アプリ用のAccessKeyを発行します。
    /// </summary>
    public class AccessKeyController : QoApiControllerBase
    {
        /// <summary>
        /// アクセスキーを生成します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtToken)]
        [ActionName("Generate")]
        public QoAccessKeyGenerateApiResults PostGenerate(QoAccessKeyGenerateApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, AccessKeyWorker.Generate);
        }

        /// <summary>
        /// アクセスキーを更新します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey)]
        [ActionName("Refresh")]
        public QoAccessKeyRefreshApiResults PostRefresh(QoAccessKeyRefreshApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, AccessKeyWorker.Refresh);
        }

        /// <summary>
        /// シングルサインオンキーを生成します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey)]
        [ActionName("Sso")]
        public QoAccessKeySsoApiResults PostSso(QoAccessKeySsoApiArgs args)
        {
            var worker = new AccessKeySsoWorker(new AccountRepository());
            return ExecuteWorkerMethod(args, worker.Generate);
        }
    }


}
