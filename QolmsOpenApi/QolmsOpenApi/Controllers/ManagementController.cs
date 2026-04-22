using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Attribute;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Providers;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;
using System.Web.Http;

namespace MGF.QOLMS.QolmsOpenApi.Controllers
{
    /// <summary>
    /// マネジメント用の公開APIを取り扱うコントローラー
    /// </summary>
    public sealed class ManagementController : QoApiControllerBase
    {
        /// <summary>
        /// <see cref="ManagementController" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        public ManagementController() : base() { }

        /// <summary>
        /// ファイルアップロードの機能。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtJotoApiKey, QoApiFunctionTypeEnum.JotoHdrAppUser)]
        [ActionName("FileUpload")]
        public QoManagementFileUploadWriteApiResults PostFileUpload(QoManagementFileUploadWriteApiArgs args)
        {
            var worker = new ManagementWorker(new ManagementRepository(),new DateTimeProvider());
            return ExecuteWorkerMethod(args, worker.FileUpload);
        }
    }
}