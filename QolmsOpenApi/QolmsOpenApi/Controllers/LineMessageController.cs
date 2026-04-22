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
    /// LINEMessageAPIに関する機能を提供する API コントローラ です。
    /// </summary>
    public class LineMessageController : ApiController
    {

        /// <summary>
        /// 検査結果のリストを返します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [ActionName("Webhook")]
        public LineWebhookEventResults PostWebhook(LineWebhookEventArgs args)
        {
            //x-line-signatureの値を取得し、引数クラスに設定。取得できなければ空設定。
            try
            {
                args.XLineSignature = Request.Headers.GetValues("x-line-signature").First();
                args.body = Request.Content.ReadAsStringAsync().Result;
                string url = Request.RequestUri.ToString();
                int index = url.IndexOf("?no=") + "?no=".Length;
                args.LinkageSystemNo = url.Substring(index, url.Length - index);
            }
            catch
            {
                args.XLineSignature = string.Empty;
            }
            return LineMessageWorker.Webhook(args);
        }

        /// <summary>
        /// 検査結果のリストを返します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [ActionName("LineImage")]
        public HttpResponseMessage GetLineImage(string name)
        {

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            switch (name)
            {
                case "LineThirdEventImage.jpg":
                case "LineFourthEventImage.jpg":
                    using (MemoryStream ms = new MemoryStream())
                    {
                        Bitmap bmp = new Bitmap(System.Web.HttpContext.Current.Server.MapPath($"~/App_Data/Line/{name}"));
                        bmp.Save(ms, ImageFormat.Jpeg);
                        result.Content = new ByteArrayContent(ms.ToArray());
                        result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
                    }
                    break;
                default:
                    break;
            }

            return result;
        }

    }
}
