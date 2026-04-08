using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MGF.QOLMS.QolmsJotoWebView
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            BundleConfig.RegisterBundles(BundleTable.Bundles);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            //検証 アダプター を登録
            //QjAttributeAdapterConfig.RegisterAttributeAdapters();

            // レスポンス ヘッダ から X-AspNet-Mvc-Version を削除
            MvcHandler.DisableMvcResponseHeader = true;

            // __RequestVerificationTokenクッキーの名前を変更する
            System.Web.Helpers.AntiForgeryConfig.CookieName = "Mgf.Qolms.QolmsJotoWebViewCsrfToken";

        }
    }
}
