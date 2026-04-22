using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Attribute;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MGF.QOLMS.QolmsOpenApi.Controllers
{
    /// <summary>
    /// 検査結果に関する機能を提供する API コントローラ です。
    /// </summary>
    public class ExaminationController : QoApiControllerBase
    {

        /// <summary>
        /// 検査結果のリストを返します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [ActionName("ExaminationDetailRead")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Examination)]
        public QoExaminationDetailReadApiResults PostExaminationDetailRead(QoExaminationDetailReadApiArgs args)
        {
            return base.ExecuteWorkerMethod(args, ExaminationWorker.ExaminationDetailRead);
        }
    }
}
