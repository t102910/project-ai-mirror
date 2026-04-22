using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Attribute;
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
    /// 内部 用のAPIキー生成コントローラです。
    /// </summary>
    public class QolmsSystemKeyController : QoApiControllerBase
    {
        /// <summary>
        /// Qolms Api Key Generate
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoIpFilter(QolmsApiCoreV1.QoApiTypeEnum.QolmsSystemKeyGenerate)]
        [ActionName("Generate")]
        public QoQolmsSystemKeyGenerateApiResults PostGenerate(QoQolmsSystemKeyGenerateApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, QolmsSystemKeyWorker.Generate);
        }
    }
}
