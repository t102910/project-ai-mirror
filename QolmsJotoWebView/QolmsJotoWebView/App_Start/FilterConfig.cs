using System.Web.Mvc;

namespace MGF.QOLMS.QolmsJotoWebView
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
