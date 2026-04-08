using System;
using System.Web.Mvc;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// アクション メソッドが、
    /// AJAX 要求によってのみアクセス可能かを指定する属性を表します。
    /// このクラスは継承できません。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    internal sealed class QjAjaxOnlyAttribute: ActionFilterAttribute
    {
        #region "Constructor"

        public QjAjaxOnlyAttribute() : base() { }

        #endregion

        #region "Public Method"

        /// <summary>
        /// アクション メソッドの実行前に ASP.NET MVC フレームワークによって呼び出されます。
        /// </summary>
        /// <param name="filterContext">フィルター コンテキスト。</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                base.OnActionExecuting(filterContext);
            }
            else
            {
                throw new InvalidOperationException("Ajax リクエストによってのみアクセス可能です。");
            }
        }

        #endregion
    }
}