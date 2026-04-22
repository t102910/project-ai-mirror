using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Attribute;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Worker;
using System.Web.Http;

namespace MGF.QOLMS.QolmsOpenApi.Controllers
{
    /// <summary>
    /// カレンダーに関する機能を提供する API コントローラです。
    /// </summary>
    public class CalendarController : QoApiControllerBase
    {

        /// <summary>
        /// カレンダー イベント情報 を取得します。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtQolmsApiKey | QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Calendar)]
        [ActionName("EventRead")]
        public QoCalendarEventReadApiResults PostEventRead(QoCalendarEventReadApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, CalendarWorker.EventRead);
        }

        /// <summary>
        /// カレンダー イベント情報 を登録・変更します。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtQolmsApiKey | QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Calendar)]
        [ActionName("EventWrite")]
        public QoCalendarEventWriteApiResults PostEventWrite(QoCalendarEventWriteApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, CalendarWorker.EventWrite);
        }

        /// <summary>
        /// カレンダー 画像情報を登録します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Calendar)]
        [ActionName("ImageWrite")]
        public QoCalendarImageWriteApiResults PostImageWrite(QoCalendarImageWriteApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, CalendarWorker.ImageWrite);
        }

        /// <summary>
        /// カレンダー 画像情報を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.Calendar)]
        [ActionName("ImageRead")]
        public QoCalendarImageReadApiResults PostImageRead(QoCalendarImageReadApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, CalendarWorker.ImageRead);
        }
    }


}
