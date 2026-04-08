using System.Web;
using System.Web.Mvc;

namespace MGF.QOLMS.QolmsJotoWebView
{
    public class QjAuthorizeAttribute : AuthorizeAttribute
    {
        #region "Public Property"

        /// <summary>
        /// ログイン ページ への リダイレクト を抑制するかを取得または設定します。
        /// </summary>
        public bool SuppressRedirect { get; set; } = false;

        #endregion

        #region "Constructor"

        /// <summary>
        /// <see cref="QhAuthorizeAttribute" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        public QjAuthorizeAttribute()
            : base()
            => this.SuppressRedirect = false;

        /// <summary>
        /// ログイン ページへ の リダイレクト を抑制するかを指定して、
        /// <see cref="QhAuthorizeAttribute" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        /// <param name="suppressRedirect">
        /// ログイン ページ への リダイレクト を抑制するなら true、
        /// 抑制しないなら false を指定。
        /// </param>
        public QjAuthorizeAttribute(bool suppressRedirect)
            : base()
            => this.SuppressRedirect = suppressRedirect;

        #endregion

        #region "Protected Method"

        /// <summary>
        /// カスタム の承認 チェック の エントリ ポイント を提供します。
        /// </summary>
        /// <param name="httpContext">HTTP コンテキスト。</param>
        /// <returns>
        /// ユーザーが承認された場合は true、
        /// それ以外の場合は false。
        /// </returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
            => QjLoginHelper.CheckLogin(httpContext.Session, httpContext.Request); // 認証、承認処理

        /// <summary>
        /// 承認されなかった HTTP 要求を処理します。
        /// </summary>
        /// <param name="filterContext">フィルター コンテキスト。</param>
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // ログイン ページ への リダイレクト を抑制するかを設定
            filterContext.HttpContext.Response.SuppressFormsAuthenticationRedirect = this.SuppressRedirect;

            base.HandleUnauthorizedRequest(filterContext);
        }

        #endregion
    }
}