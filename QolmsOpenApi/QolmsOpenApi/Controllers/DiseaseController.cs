using System.Web.Http;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Attribute;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;

namespace MGF.QOLMS.QolmsOpenApi.Controllers
{
    /// <summary>
    /// MEDIS標準病名マスタを使った機能を提供するAPIコントローラー
    /// </summary>
    public class DiseaseController: QoApiControllerBase
    {
        /// <summary>
        /// キーワードから病名の候補を検索します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey)]
        [ActionName("Search")]
        public QoDiseaseSearchApiResults PostSearch(QoDiseaseSearchApiArgs args)
        {
            var worker = new DiseaseSearchWorker(new MedisRepository());
            return ExecuteWorkerMethod(args, worker.Search);
        }
    }
}