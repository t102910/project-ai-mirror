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
    /// Line に関する機能を提供する API コントローラ です。
    /// </summary>
    public class LineController : QoApiControllerBase
    {

        /// <summary>
        /// TemplatePushメッセージをリクエストします。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [ActionName("TemplatePushSend")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.Basic | QoApiAuthorizeTypeEnum.JwtQolmsApiKey)]
        public QoLineTemplatePushResults PostTemplatePushSend(QoLineTemplatePushArgs args)
        {
            return base.ExecuteWorkerMethod(args, LineMessageWorker.TemplatePushSend);
        }

        /// <summary>
        /// CustomPushメッセージをリクエストします。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [ActionName("CustomPushSend")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.Basic | QoApiAuthorizeTypeEnum.JwtQolmsApiKey)]
        public QoLineCustomPushResults PostCustomPushSend(QoLineCustomPushArgs args)
        {
            return base.ExecuteWorkerMethod(args, LineMessageWorker.CustomPushSend);
        }

        /// <summary>
        /// MenuID更新API をリクエストします。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [ActionName("UpdateMenuId")]
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.Basic | QoApiAuthorizeTypeEnum.JwtQolmsApiKey)]
        public QoLineUpdateMenuIdResults PostUpdateMenuId(QoLineUpdateMenuIdArgs args)
        {
            return base.ExecuteWorkerMethod(args, LineMessageWorker.UpdateMenuId);
        }

    }
}
