using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace QolmsOpenApi
{
    /// <summary>
    /// 
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config)
        {
            // Web API の設定およびサービス

            // Web API ルート
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "v1/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //ルートそのまま呼ばれたばあいでもOKを返すようにする（死活監視等の応答用アクションメソッド）
            config.Routes.MapHttpRoute(
                name: "RoutApi",
                routeTemplate: "",
                defaults: new { controller = "App", action = "Ping", id = RouteParameter.Optional }
            );
        }
    }
}
