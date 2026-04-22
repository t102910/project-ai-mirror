using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Attribute;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Worker;
using System.Web.Http;

namespace MGF.QOLMS.QolmsOpenApi.Controllers
{
    /// <summary>
    /// アプリゲスト用のアクセスキーを発行します。
    /// </summary>
    public class GuestController : QoApiControllerBase
    {

        /// <summary>
        /// ゲスト用のアクセスキーを生成します。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [ActionName("AccessKeyGenerate")]
        public QoGuestAccessKeyGenerateApiResults PostAccessKeyGenerate(QoGuestAccessKeyGenerateApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, GuestWorker.Generate);
        }

        /// <summary>
        /// ゲスト用のアクセスキーを再生成します。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Guest)]
        [ActionName("AccessKeyRefresh")]
        public QoGuestAccessKeyRefreshApiResults PostAccessKeyRefresh(QoGuestAccessKeyRefreshApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, GuestWorker.Refresh);
        }
    }

    
}
