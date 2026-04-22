using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Attribute;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Worker;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;

namespace MGF.QOLMS.QolmsOpenApi.Controllers
{
    /// <summary>
    /// HaipAPIに関する機能を提供する API コントローラ です。
    /// </summary>
    public class HaipController : ApiController
    {

        /// <summary>
        /// 検査結果のリストを返します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [ActionName("PhrReceiveNotification")]
        public HaipPhrReceiveNotificationResults PostPhrReceiveNotification(HaipPhrReceiveNotificationArgs args)
        {
            //x-line-signatureの値を取得し、引数クラスに設定。取得できなければ空設定。
            try
            {
                args.XRequestId = Request.Headers.GetValues("X-Request-Id").First();
                args.XApiKey = Request.Headers.GetValues("X-Api-Key").First();
                
            }
            catch
            {
                args.XApiKey = string.Empty;
            }
            return HaipWorker.HaipPhrReceiveNotification(args);
        }

    }
}
