using System;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;

namespace MGF.QOLMS.QolmsJotoWebView
{

    /// <summary>
    /// MVCコントローラの基底クラスです。
    /// </summary>
    public abstract class QjMvcControllerBase : Controller
    {

        #region "Constant"

        /// <summary>
        /// 一時データのディクショナリ内でのログイン済みフラグのキー名を表します。
        /// </summary>
        protected const string KEY_IS_LOGIN = "IsLogin";

        /// <summary>
        /// 一時データのディクショナリ内でのエラー メッセージのキー名を表します。
        /// </summary>
        protected const string KEY_ERROR_MESSAGE = "ErrorMessage";

        /// <summary>
        /// 標準のエラー メッセージを表します。
        /// </summary>
        protected const string DEFAULT_ERROR_MESSAGE = "ページの表示に失敗しました";

        #endregion

        #region "Variable"

        /// <summary>
        /// Error 画面に表示するエラー メッセージを保持します。
        /// </summary>
        /// <remarks></remarks>
        private string errorMessage = QjMvcControllerBase.DEFAULT_ERROR_MESSAGE;

        /// <summary>
        /// JavaScript が有効かを保持します。
        /// </summary>
        /// <remarks></remarks>
        private bool _enableJavaScript = false;

        /// <summary>
        /// クッキーが有効かを保持します。
        /// </summary>
        /// <remarks></remarks>
        private bool _enableCookies = false;

        /// <summary>
        /// デバッグ ビルドかを保持します。
        /// </summary>
        /// <remarks></remarks>
        private bool _isDebug = false;

        /// <summary>
        /// デバッグ ログのフォルダパスを保持します。
        /// </summary>
        /// <remarks></remarks>
        private string _debugLogPath = string.Empty;

        #endregion

        #region "Public Property"

        /// <summary>
        /// JavaScript が有効かを取得します。
        /// </summary>
        public bool EnableJavaScript { get => this._enableJavaScript; }

        /// <summary>
        /// クッキーが有効かを取得します。
        /// </summary>
        public bool EnableCookies { get => this._enableCookies; }

        /// <summary>
        /// クッキーが有効かを取得します。
        /// </summary>
        public bool IsDebug { get => this._isDebug; }

        /// <summary>
        /// デバッグ ログのフォルダパスを取得します。
        /// </summary>
        public string DebugLogPath { get => this._debugLogPath; }

        #endregion

        #region "Constructor"
        protected QjMvcControllerBase() : base()
        {

#if DEBUG
            this._isDebug = true;
#else
            this._isDebug = false;
#endif  
        }

        #endregion

        #region "Protected Method"

        /// <summary>
        /// アクション メソッド の呼び出し前に呼び出されます。
        /// </summary>
        /// <param name="filterContext">現在の要求および アクション に関する情報。</param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            // JavaScript が有効かを取得
            this._enableJavaScript = HtmlHelper.UnobtrusiveJavaScriptEnabled;

            // クッキー が有効かを取得
            this._enableCookies = !this.Session.IsCookieless;

            // デバッグ ログのパスを設定
            this._debugLogPath = this.HttpContext.Server.MapPath("~/App_Data/log");

            QolmsJotoModel mainModel = QjLoginHelper.CheckLogin(this.Session, this.Request) ? QjLoginHelper.GetQolmsJotoModel(this.Session) : null;

            if (mainModel != null)
            {
                // JavaScript が有効かを設定
                mainModel.SetEnableJavaScript(this._enableJavaScript);

                // クッキー が有効かを設定
                mainModel.SetEnableCookies(this._enableCookies);

                // デバッグ ビルド かを設定
                mainModel.SetIsDebug(this._isDebug);

                // デバッグ ログのパスを設定
                mainModel.SetDebugLogPath(this._debugLogPath);

                // セッション ID を設定
                mainModel.SetSessionId(this.Session.SessionID);

                // 画面の表示回数の カウント と不要な キャッシュ の破棄
                if (!filterContext.ActionDescriptor.IsDefined(typeof(ChildActionOnlyAttribute), false) || filterContext.ActionDescriptor.IsDefined(typeof(QjAjaxOnlyAttribute), false))
                {
                    // 画面の表示回数を カウント
                    if (filterContext.ActionDescriptor.IsDefined(typeof(QjViewCountAttribute), false))
                    {
                        mainModel.IncrementPageViewCount(((QjViewCountAttribute)filterContext.ActionDescriptor.GetCustomAttributes(typeof(QjViewCountAttribute), false).First()).PageNo);
                    }

                    // TODO: 不要な キャッシュ を破棄
                }
            }

            // アクセス ログ を出力
            if (filterContext.ActionDescriptor.IsDefined(typeof(QjLoggingAttribute), false))
            {
                string uri = string.Empty;
                try
                {
                    uri = $"/{filterContext.ActionDescriptor.ControllerDescriptor.ControllerName}/{filterContext.ActionDescriptor.ActionName}".ToLower(); // パス 部分は小文字で統一しておく
                    if (!string.IsNullOrWhiteSpace(filterContext.HttpContext.Request.Url.Query))
                    {
                        uri += filterContext.HttpContext.Request.Url.Query;
                    }
                }
                catch
                {
                }
                // アクセス ログ
                //AccessLogWorker.WriteAccessLog(filterContext.HttpContext, uri, AccessLogWorker.AccessTypeEnum.Show, AccessLogWorker.CommentTypeEnum.None);

            }

            // QolmsApi 用 API 認証 キー を取得
            if (filterContext.ActionDescriptor.IsDefined(typeof(QyApiAuthorizeAttribute), false) && mainModel != null)
            {
                // API 認証有効期限切れなら API 認証 キー を再取得
                if (DateTime.Now > mainModel.ApiAuthorizeExpires)
                {
                    Guid authorizeKey = Guid.Empty;
                    DateTime authorizeExpires = DateTime.MinValue;

                    // API 認証 キー および API 認証有効期限を取得
                    ApiAuthorizeWorker.NewKey(mainModel.AuthorAccount.AccountKey, mainModel.SessionId, ref authorizeKey, ref authorizeExpires);

                    if (authorizeKey == Guid.Empty || authorizeExpires == DateTime.MinValue)
                    {
                        throw new ArgumentOutOfRangeException("ApiAuthorizeKey、ApiAuthorizeExpires", "QolmsApi 用 API 認証 キー および API 認証有効期限の取得に失敗しました。");
                    }

                    // API 認証 キー および API 認証有効期限を設定
                    mainModel.SetApiAuthorizeKey(authorizeKey);
                    mainModel.SetApiAuthorizeExpires(authorizeExpires);
                }
            }

            // QolmsJotoApi 用 API 認証 キー を取得
            if (filterContext.ActionDescriptor.IsDefined(typeof(QjApiAuthorizeAttribute), false) && mainModel != null)
            {
                // API 認証有効期限切れなら API 認証 キー を再取得
                if (DateTime.Now > mainModel.ApiAuthorizeExpires2)
                {
                    Guid authorizeKey = Guid.Empty;
                    DateTime authorizeExpires = DateTime.MinValue;

                    // API 認証 キー および API 認証有効期限を取得
                    ApiAuthorizeWorker.NewKey2(mainModel.AuthorAccount.AccountKey, mainModel.SessionId, ref authorizeKey, ref authorizeExpires);

                    if (authorizeKey == Guid.Empty || authorizeExpires == DateTime.MinValue)
                    {
                        throw new ArgumentOutOfRangeException("ApiAuthorizeKey、ApiAuthorizeExpires", "QolmsJotoApi 用 API 認証 キー および API 認証有効期限の取得に失敗しました。");
                    }

                    // API 認証 キー および API 認証有効期限を設定
                    mainModel.SetApiAuthorizeKey2(authorizeKey);
                    mainModel.SetApiAuthorizeExpires2(authorizeExpires);
                }
            }

            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// アクション メソッドの呼び出し後に呼び出されます。
        /// </summary>
        /// <param name="filterContext">現在の要求およびアクションに関する情報。</param>
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // JSON レスポンスの XSS に対する予防的措置
            if (filterContext.Result.GetType() == typeof(JsonResult))
            {
                filterContext.HttpContext.Response.AddHeader("X-Content-Type-Options", "nosniff");
                filterContext.HttpContext.Response.AddHeader("Content-Disposition", $"attachment; filename={ DateTime.Now:yyyyMMddHHmmssfffffff}.json");
            }
            base.OnActionExecuted(filterContext);
        }

        /// <summary>
        /// アクションでハンドルされない例外が発生したときに呼び出されます。
        /// </summary>
        /// <param name="filterContext">現在の要求およびアクションに関する情報。</param>
        protected override void OnException(ExceptionContext filterContext)
        {
            var uri = string.Empty;

            try
            {
                uri = String.Format("/{0}/{1}", filterContext.RouteData.GetRequiredString("controller"), filterContext.RouteData.GetRequiredString("action"));
                if (!string.IsNullOrWhiteSpace(filterContext.HttpContext.Request.Url.Query))
                {
                    uri += filterContext.HttpContext.Request.Url.Query;
                }
            }
            catch
            {
            }

            // エラーログ
            AccessLogWorker.WriteErrorLog(filterContext.HttpContext, uri, filterContext.Exception);

            // エラー画面へ遷移
            if (filterContext.HttpContext.IsCustomErrorEnabled)
            {
                filterContext.ExceptionHandled = true;
                this.TempData.Clear();
                this.TempData.Add(QjMvcControllerBase.KEY_IS_LOGIN, QjLoginHelper.CheckLogin(this.Session, this.Request).ToString());
                this.TempData.Add(QjMvcControllerBase.KEY_ERROR_MESSAGE, this.errorMessage);

                //標準のエラーメッセージへ戻しておく
                this.errorMessage = QjMvcControllerBase.DEFAULT_ERROR_MESSAGE;
            }
            else
            {
                base.OnException(filterContext);
            }
        }

        /// <summary>
        /// Error 画面に表示するエラー メッセージを設定します。
        /// </summary>
        /// <param name="message"></param>
        protected void SetErrorMessage(string message)
        {
            if (!string.IsNullOrWhiteSpace(this.errorMessage))
            {
                this.errorMessage = message;
            }
        }

        /// <summary>
        /// メイン モデルを取得します。
        /// </summary>
        /// <returns>
        /// 成功ならメイン モデル、
        /// 失敗なら null。</returns>
        protected QolmsJotoModel GetQolmsJotoModel()
        {
            return QjLoginHelper.GetQolmsJotoModel(this.Session);
        }

        /// <summary>
        /// パーシャル ビュー モデルの親ビュー モデルとなる画面ビュー モデル取得します。
        /// </summary>
        /// <typeparam name="TModel">画面ビュー モデルの型。</typeparam>
        /// <returns>
        /// 成功なら画面ビュー モデル、
        /// 失敗なら null。
        /// </returns>
        protected TModel GetPageViewModel<TModel>()
        {
            TModel result = default;

            if (this.ControllerContext.IsChildAction)
            {
                try
                {
                    result = (TModel)this.ControllerContext.ParentActionViewContext.ViewData.Model;
                }
                catch 
                {
                }
            }

            return result;
        }

        #endregion

        #region "Public Method"

        /// <summary>
        /// セッションが有効かチェックします。
        /// </summary>
        /// <returns>アクションの結果。</returns>
        [HttpPost]
        [QjAjaxOnly]
        public JsonResult AjaxCheckSession()
        {
            return new CheckSessionJsonResult() { IsSuccess = QjLoginHelper.CheckLogin(this.Session, this.Request).ToString() }.ToJsonResult();
        }

        /// <summary>
        /// メイン ウィンドウからの要求に対し、
        /// セッションが有効かチェックします。
        /// </summary>
        /// <returns>
        /// セッションが有効なら "javascript:void(0);"、
        /// セッションが無効ならログイン画面へ遷移するスクリプトを返却。
        /// </returns>
        [HttpPost]
        [QjAjaxOnly]
        public JavaScriptResult AjaxCheckSessionByWindow()
        {
            string script = "javascript:void(0);";

            if (!QjLoginHelper.CheckLogin(this.Session, this.Request))
            {
                var url = ((AuthenticationSection)WebConfigurationManager.GetSection("system.web/authentication")).Forms.LoginUrl;

                if (url.StartsWith("/"))
                {
                    url += "..";
                }
                else if(url.StartsWith("~/"))
                {
                    url = ".." + url.Substring(1);
                }

                if (!string.IsNullOrWhiteSpace(url))
                {
                    script =$"$(location).attr('href', '{url}');";
                }
            }

            return JavaScript(script);
        }

        #endregion
    }
}