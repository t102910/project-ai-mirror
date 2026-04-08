using Autofac;
using System.Web.Http;

namespace MGF.QOLMS.QolmsJotoWebView
{
    public class WebApiConfig
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

            ////ルートそのまま呼ばれたばあいでもOKを返すようにする（死活監視等の応答用アクションメソッド）
            //config.Routes.MapHttpRoute(
            //    name: "RoutApi",
            //    routeTemplate: "v1/",
            //    defaults: new { controller = "App", action = "Ping", id = RouteParameter.Optional }
            //);

            // Autofac
            var builder = new ContainerBuilder();

            //// MVC コントローラを登録
            //builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // Provider
            //builder.Register(c => new HttpContextProvider(HttpContext.Current))
            // .As<IHttpContextProvider>().InstancePerRequest();

            //builder.RegisterType<ControllerModel>().As<IControllerModel>();
            //builder.RegisterType<WorkerModel>().As<IWorkerModel>();

            // Repository
            builder.RegisterType<LoginRepository>().As<ILoginRepository>();
            //builder.RegisterType<AccountRepository>().As<IAccountRepository>();
            //builder.RegisterType<AdviceRepository>().As<IAdviceRepository>();
            //builder.RegisterType<ModelRepository>().As<IModelRepository>();
            //builder.RegisterType<BackgroundRepository>().As<IBackgroundRepository>();
            //builder.RegisterType<StorageRepository>().As<IStorageRepository>();

            //// Worker
            //builder.RegisterType<AuthWorker>().As<IAuthWorker>();
            //builder.RegisterType<AdviceAddWorker>().As<IAdviceAddWorker>();
            //builder.RegisterType<AdviceEditWorker>().As<IAdviceEditWorker>();
            //builder.RegisterType<AdviceDeleteWorker>().As<IAdviceDeleteWorker>();
            //builder.RegisterType<AdviceReadWorker>().As<IAdviceReadWorker>();
            //builder.RegisterType<FileUploadWorker>().As<IFileUploadWorker>();
            //builder.RegisterType<FileDownloadWorker>().As<IFileDownloadWorker>();
            //builder.RegisterType<ModelAddWorker>().As<IModelAddWorker>();
            //builder.RegisterType<ModelEditWorker>().As<IModelEditWorker>();
            //builder.RegisterType<ModelDeleteWorker>().As<IModelDeleteWorker>();
            //builder.RegisterType<ModelReadWorker>().As<IModelReadWorker>();
            //builder.RegisterType<BackgroundAddWorker>().As<IBackgroundAddWorker>();
            //builder.RegisterType<BackgroundEditWorker>().As<IBackgroundEditWorker>();
            //builder.RegisterType<BackgroundDeleteWorker>().As<IBackgroundDeleteWorker>();
            //builder.RegisterType<BackgroundReadWorker>().As<IBackgroundReadWorker>();

            //builder.RegisterType<AuthWorker>().As<IAuthWorker>();


            /*** フィルターの登録  ***/

            //// ApiAuthorizeForJwtToken属性があるメソッドはTokenによる認証を行う
            //builder.Register(c =>
            //        new QjAuthorizationFilter(
            //            QjApiAuthorizeTypeEnum.JwtToken,
            //            c.Resolve<IAccessLogRepository>())
            //    ).AsWebApiAuthorizationFilterWhere(action => action.GetCustomAttributes<ApiAuthorizeForJwtTokenAttribute>().Any());

            // ApiAuthorizeForJwtAccessKey属性があるメソッドはAccessKeyによる認証を行う
            //builder.Register(c =>
            //        new QjAuthorizationFilter(
            //            QjApiAuthorizeTypeEnum.JwtAccessKey,
            //            c.Resolve<IAccessLogRepository>())
            //    ).AsWebApiAuthorizationFilterWhere(action => action.GetCustomAttributes<ApiAuthorizeForJwtAccessKeyAttribute>().Any());

            //var container = builder.Build();
            //config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

        }
    }
}