using System.Web.Http;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Attribute;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;

namespace MGF.QOLMS.QolmsOpenApi.Controllers
{
    /// <summary>
    /// 日記に関する機能を提供するAPIコントローラー
    /// </summary>
    public class DiaryController: QoApiControllerBase
    {
        /// <summary>
        /// 日記の状態(いいね数・気分など）を取得します
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey)]
        [ActionName("Status")]
        public QoDiaryStatusApiResults PostStatus(QoDiaryStatusApiArgs args)
        {
            var worker = new DiaryStatusWorker(new EventRepository());
            return ExecuteWorkerMethod(args, worker.Read);
        }
    }
}