using System.Linq;
using System.Web.Http;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Attribute;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Worker;

namespace MGF.QOLMS.QolmsOpenApi.Controllers
{
    
    /// <summary>
    /// サポート関連の機能を提供します。
    /// </summary>
    public class SupportController : QoApiControllerBase
    {

        /// <summary>
        /// お問い合わせを登録します。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        [QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.All | QoApiFunctionTypeEnum.Guest, true)]
        [ActionName("Register")]
        public QoSupportRegisterApiResults PostRegister(QoSupportRegisterApiArgs args)
        {
            return this.ExecuteWorkerMethod(args, SupportWorker.Register);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public bool GetIsDebug( bool? debug=null)
        {
            if(debug.HasValue)
                QoAccessLog.IsDebug = (bool)debug;
            QoAccessLog.WriteAccessLog(this.ControllerContext, QolmsApiCoreV1.QsApiSystemTypeEnum.QolmsOpenApi, 
                System.Guid.Empty, System.DateTime.Now, QoAccessLog.AccessTypeEnum.Api, string.Empty, "DebugMode:" + QoAccessLog.IsDebug.ToString());
            return QoAccessLog.IsDebug;
        }
    }


}