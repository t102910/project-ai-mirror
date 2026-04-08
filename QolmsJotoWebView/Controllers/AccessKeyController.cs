using MGF.QOLMS.QolmsApiEntityV1;
using System.Web.Http;

namespace MGF.QOLMS.QolmsJotoWebView
{
    public class AccessKeyController : QjApiControllerBase
    {
        //IAccountRepository _accountRepository;

        //public AccessKeyController(
        //     IAccountRepository accountRepository) : base()
        //{
        //    _accountRepository = accountRepository;
        //}

        ///// <summary>
        ///// アクセスキーを生成します。
        ///// </summary>
        ///// <param name="args"></param>
        //[QjApiAuthorize(QjApiAuthorizeTypeEnum.JwtToken)]
        //[ActionName("Generate")]
        //public QoAccessKeyGenerateApiResults PostGenerate(QoAccessKeyGenerateApiArgs args)
        //{
        //    return this.ExecuteWorkerMethod(args, AccessKeyUseCase.Generate);
        //}

        ///// <summary>
        ///// アクセスキーを更新します。
        ///// </summary>
        ///// <param name="args"></param>
        //[QjApiAuthorize(QjApiAuthorizeTypeEnum.JwtAccessKey)]
        //[ActionName("Refresh")]
        //public QoAccessKeyRefreshApiResults PostRefresh(QoAccessKeyRefreshApiArgs args)
        //{
        //    return this.ExecuteWorkerMethod(args, AccessKeyUseCase.Refresh);
        //}

        /// <summary>
        /// シングルサインオンキーを生成します。
        /// </summary>
        /// <param name="args"></param>
        [QjApiAuthorize(QjApiAuthorizeTypeEnum.JwtAccessKey)]
        [ActionName("Sso")]
        public QjAccessKeySsoApiResults PostSso(QjAccessKeySsoApiArgs args)
        {
            var worker = new AccessKeySsoUseCase(new LoginRepository());
            return base.ExecuteWorkerMethod(args, worker.Generate);
        }
    }
}