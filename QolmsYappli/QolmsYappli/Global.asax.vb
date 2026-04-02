Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.Mvc
Imports System.Web.Optimization
Imports System.Web.Routing
Imports System.Web.Http

Public Class MvcApplication
    Inherits System.Web.HttpApplication

    Sub Application_Start()

        AreaRegistration.RegisterAllAreas()
        'WebApiConfig.Register(GlobalConfiguration.Configuration)
        FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters)
        RouteConfig.RegisterRoutes(RouteTable.Routes)
        BundleConfig.RegisterBundles(BundleTable.Bundles)

        ' __RequestVerificationTokenクッキーの名前を変更する
        System.Web.Helpers.AntiForgeryConfig.CookieName = "Mgf.Qolms.QolmsYappliCsrfToken"

        ' レスポンス ヘッダ から X-AspNet-Mvc-Version を削除
        MvcHandler.DisableMvcResponseHeader = True

    End Sub

    'Protected Sub Application_Start()
    '    AreaRegistration.RegisterAllAreas()
    '    FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters)
    '    RouteConfig.RegisterRoutes(RouteTable.Routes)
    '    BundleConfig.RegisterBundles(BundleTable.Bundles)
    'End Sub
End Class