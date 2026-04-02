using MGF.QOLMS.QolmsApiCoreV1;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// APIコントローラの基底クラスです。
    /// </summary>
    public abstract class QjApiControllerBase : ApiController
    {
        #region "Constant"

        /// <summary>
        /// 不正な実行者 ID であることを表すエラーメッセージです。
        /// </summary>
        /// <remarks></remarks>
        protected const string EXECUTOR_ERROR_MESSAGE = "不正な実行者IDです。";

        #endregion

        #region "Public Property"
        /// <summary>
        /// アカウントキーを取得または設定します。
        /// </summary>
        public Guid AccountKey { get; set; } = Guid.Empty;

        /// <summary>
        /// 親アカウントキーを取得または設定します。
        /// </summary>
        public Guid ParentKey { get; set; } = Guid.Empty;

        /// <summary>
        /// 暗号化されたままのExecutorを取得または設定します。
        /// </summary>
        public string EncExecutor { get; set; } = string.Empty;

        #endregion

        /// <summary>
        ///  クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        protected QjApiControllerBase() : base()
        { }

        #region "Private Method"

        /// <summary>
        /// Web API の実行者をチェックします。
        /// </summary>
        /// <param name="args"></param>
        /// <param name="encExecutor">暗号化されたままのExecutor</param>
        /// <param name="refExecuteSystemType"> Web API の実行システムの種別が格納される変数。チェックに失敗の場合は不定。</param>
        /// <param name="refExecutor"> Web API の実行者が格納される変数。チェックに失敗の場合は不定。</param>
        /// <param name="refExecutorName">実行者名が格納される変数。チェックに失敗の場合は不定。</param>
        /// <returns>
        /// 成功ならTrue、失敗ならFalse。
        /// </returns>
        /// <remarks></remarks>
        private bool CheckExecutor(QjApiArgsBase args, string encExecutor, ref QsApiSystemTypeEnum refExecuteSystemType, ref Guid refExecutor, ref string refExecutorName)
        {
            refExecuteSystemType = args.ExecuteSystemType.TryToValueType(QsApiSystemTypeEnum.None);
            refExecutor = args.Executor.TryToValueType(Guid.Empty);
            refExecutorName = args.ExecutorName;

            if (args.ApiType.TryToValueType(QoApiTypeEnum.None).GetType().IsDefined(typeof(QsApiRequireAuthorizeAttribute), false))
            {    // 承認が必要な API

                // TODO: 実行システムと実行者とチェック
                return refExecuteSystemType != QsApiSystemTypeEnum.None && refExecutor != Guid.Empty && encExecutor == this.EncExecutor && !string.IsNullOrWhiteSpace(refExecutorName);
            }
            else
            {    // 承認が不必要なAPI

                // TODO: 実行システムのみチェック
                return refExecuteSystemType != QsApiSystemTypeEnum.None && !string.IsNullOrWhiteSpace(refExecutorName);
            }

        }

        /// <summary>
        /// Web API の実行に失敗したことをログに記録し、エラーをセットした戻り値クラスを返却します。
        /// </summary>
        /// <typeparam name="TResults">Web API 戻り値クラスの型。</typeparam>
        /// <param name="executeSystemType">実行システムの種別。</param>
        /// <param name="executor">実行者 ID。</param>
        /// <param name="ex">例外オブジェクト。</param>
        /// <returns>
        ///  Web API 戻り値クラスの新しいインスタンス。
        /// </returns>
        /// <remarks></remarks>
        private TResults ApiErrorResult<TResults>(QsApiSystemTypeEnum executeSystemType, Guid executor, Exception ex) where TResults : QjApiResultsBase, new()
        {
            var context = this.ControllerContext;
            // エラーログ
            Task.Run(() =>
            {
                //Todo：ログ
                //AccessLogWorker.WriteErrorLog(context, executeSystemType, executor, DateTime.Now, string.Empty, ex);
            });

            // エラーをセットした戻り値クラスを返却
            return new TResults()
            {
                IsSuccess = bool.FalseString,
                Result = new QjApiResultItem()
                {
                    Code = "0500",//Convert.ToInt32(QjApiResultCodeTypeEnum.InternalServerError).ToString("d4"),
                    Detail = "API実行中にエラーが発生しました。"
                }
            };
        }
        #endregion

        /// <summary>
        /// ワーカー クラス 内の処理 メソッド を実行します。
        /// </summary>
        /// <typeparam name="TArgs">Web API 引数 クラス の型。</typeparam>
        /// <typeparam name="TResults">Web API 戻り値 クラス の型。</typeparam>
        /// <param name="args">Web API 引数 クラス。</param>
        /// <param name="method">処理 メソッド。</param>
        /// <returns>
        /// Web API 戻り値 クラス。
        /// </returns>
        protected TResults ExecuteWorkerMethod<TArgs, TResults>(TArgs args, Func<TArgs, TResults> method)
        where TArgs : QjApiArgsBase
        where TResults : QjApiResultsBase, new()
        {
            QsApiSystemTypeEnum executeSystemType = QsApiSystemTypeEnum.None;
            Guid executor = Guid.Empty;
            string executorName = string.Empty;

            try
            {
                string encExecutor = args.Executor;//暗号化されたままのを確保。
                // 必要に応じて引数クラスのプロパティ値を復号化
                // Web API の実行者をチェック
                if (!this.CheckExecutor(args.DecryptMember<TArgs>(), encExecutor, ref executeSystemType, ref executor, ref executorName))
                    throw new ArgumentException(QjApiControllerBase.EXECUTOR_ERROR_MESSAGE);

                // JWT認証で取得したアカウントキーを対象者にセット
                if (this.AccountKey != Guid.Empty) //（APIKeyの場合はJWTから取得できないが、素直にActorKeyが入ってるときEmptyで上書きしないようにする）
                    args.ActorKey = this.AccountKey.ToString("N");

                if (Request != null)
                {
                    string userHostAddress = "";
                    string userHostName = "";
                    string userAgent = "";
                    string accessUri = "";
                    HttpContextBase httpContext = Request.Properties["MS_HttpContext"] as HttpContextBase;
                    if (httpContext != null)
                    {
                        userHostAddress = httpContext.Request.UserHostAddress;
                        userHostName = httpContext.Request.UserHostName;
                        userAgent = httpContext.Request.UserAgent;
                        accessUri = httpContext.Request.Url.AbsolutePath;
                    }

                    // ログ出力しとく
                    Task.Run(() =>
                    {
                        //todo :ログ内容確認
                        AccessLogWorker.WriteAccessLog(httpContext, accessUri, AccessLogWorker.AccessTypeEnum.Api,
                            "ExecuteWorkerMethod");
                    });
                }
                // 処理メソッドを実行し、
                // 必要に応じて戻り値クラスのプロパティ値を暗号化
                var result = method(args).EncryptMember<TResults>();
                // 成功時のコードは勝手に入るようにしとく
                if (result.Result == null && result.IsSuccess.TryToValueType(false))
                    result.Result = QjApiResult.Build(QjApiResultCodeTypeEnum.Success);
                return result;
            }
            catch (Exception ex)
            {
                AccessLogWorker.WriteErrorLog(null,string.Empty,ex);
                // エラー
                return this.ApiErrorResult<TResults>(executeSystemType, executor, ex);
            }
        }

        /// <summary>
        /// ワーカー クラス 内の処理 メソッド を非同期で実行します。
        /// </summary>
        /// <typeparam name="TArgs"></typeparam>
        /// <typeparam name="TResults"></typeparam>
        /// <param name="args"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        protected async Task<TResults> ExecuteWorkerMethodAsync<TArgs, TResults>(TArgs args, Func<TArgs, Task<TResults>> method)
        where TArgs : QjApiArgsBase
        where TResults : QjApiResultsBase, new()
        {
            QsApiSystemTypeEnum executeSystemType = QsApiSystemTypeEnum.None;
            Guid executor = Guid.Empty;
            string executorName = string.Empty;

            try
            {
                string encExecutor = args.Executor;//暗号化されたままのを確保。
                // 必要に応じて引数クラスのプロパティ値を復号化
                // Web API の実行者をチェック
                if (!this.CheckExecutor(args.DecryptMember<TArgs>(), encExecutor, ref executeSystemType, ref executor, ref executorName))
                    throw new ArgumentException(QjApiControllerBase.EXECUTOR_ERROR_MESSAGE);

                //// JWT認証で取得したアカウントキーを対象者にセット
                if (this.AccountKey != Guid.Empty) //（APIKeyの場合はJWTから取得できないが、素直にActorKeyが入ってるときEmptyで上書きしないようにする）
                    args.ActorKey = this.AccountKey.ToString("N");

                if (Request != null)
                {
                    string userHostAddress = "";
                    string userHostName = "";
                    string userAgent = "";
                    string accessUri = "";
                    HttpContextBase httpContext = Request.Properties["MS_HttpContext"] as HttpContextBase;
                    if (httpContext != null)
                    {
                        userHostAddress = httpContext.Request.UserHostAddress;
                        userHostName = httpContext.Request.UserHostName;
                        userAgent = httpContext.Request.UserAgent;
                        accessUri = httpContext.Request.Url.AbsolutePath;
                    }

                    // ログ出力しとく
                    _ = Task.Run(() =>
                    {
                        AccessLogWorker.WriteAccessLog(httpContext, accessUri, AccessLogWorker.AccessTypeEnum.Api,
                            "ExecuteWorkerMethod");
                    });
                }
                // 処理メソッドを実行し、
                // 必要に応じて戻り値クラスのプロパティ値を暗号化
                var result = (await method(args)).EncryptMember<TResults>();
                // 成功時のコードは勝手に入るようにしとく
                if (result.Result == null && result.IsSuccess.TryToValueType(false))
                    result.Result = QjApiResult.Build(QjApiResultCodeTypeEnum.Success);
                return result;
            }
            catch (Exception ex)
            {
                AccessLogWorker.WriteErrorLog(null,string.Empty,ex);
                // エラー
                return this.ApiErrorResult<TResults>(executeSystemType, executor, ex);
            }
        }

    }
}