using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsOpenApi.Worker;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;

namespace MGF.QOLMS.QolmsOpenApi.Attribute
{
    /// <summary>
    /// Web API を実行するためにIP制限の承認が必要であることを指定します。
    /// </summary>
    /// <remarks></remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class QoIpFilterAttribute : AuthorizationFilterAttribute
    {
        private readonly string IP_ADDRESS_WHITE_LIST_KEY = "AllowIPAddress";

        private QoApiTypeEnum _apiType = QoApiTypeEnum.None;

        /// <summary>
        /// デフォルトコンストラクタは使用できません。
        /// </summary>
        /// <remarks></remarks>
        private QoIpFilterAttribute()
        {
        }

        /// <summary>
        /// 値を指定して、
        /// <see cref="QoApiAuthorizeAttribute" />クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="apiType">API種別。</param>
        /// <remarks></remarks>
        public QoIpFilterAttribute(QoApiTypeEnum apiType) : base()
        {
            this._apiType = apiType;
        }



        private List<string> GetWhiteList()
        {
            List<string> result = new List<string>();

            try
            {
                string value = string.Empty;
                value = ConfigurationManager.AppSettings[string.Format("{0}{1}", this.IP_ADDRESS_WHITE_LIST_KEY, Convert.ToInt32(this._apiType))];
                if (value!=null && value != "*")
                    result = value.Split('|').ToList();
            }
            catch
            {
            }

            return result;
        }

        private bool IsValidIpAddress(string ip)
        {
            // ホワイトリストを取得
            List<string> list = this.GetWhiteList();

            if (list != null && list.Any())
            {    // ホワイトリストのチェック
                return list.FindIndex(i => i.Trim().CompareTo(ip.Trim()) == 0) > -1; // 0
            }
            else
            {    // 制限なし
                return true;
            }
        }



        /// <summary>
        /// プロセスが承認を要求したときに呼び出します。
        /// </summary>
        /// <param name="actionContext">アクション コンテキスト。</param>
        /// <remarks></remarks>
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            var context = (HttpContextBase)actionContext.Request.Properties["MS_HttpContext"];
            bool result = this.IsValidIpAddress(context.Request.UserHostAddress);

            if (result)
                return;

            QoAccessLog.WriteAccessLog( QsApiSystemTypeEnum.QolmsOpenApi, Guid.Empty, DateTime.Now,  QoAccessLog.AccessTypeEnum.Api,
                 context.Request.Url.AbsolutePath, "IPCompareResult:" + result.ToString(),context.Request.UserHostAddress,context.Request.UserHostName,context.Request.UserAgent  );
            // 認証失敗
            var response = new System.Net.Http.HttpResponseMessage(HttpStatusCode.Forbidden);
            actionContext.Response = response;
        }
    }


}