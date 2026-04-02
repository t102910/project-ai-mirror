using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsJwtAuthCore;
using System;
using System.Web;
using System.Web.Mvc;

namespace MGF.QOLMS.QolmsJotoWebView
{
    public class QjJwtAuthorizeAttribute: AuthorizeAttribute
    {
        #region "Constant"

        /// <summary>
        /// モジュール と ハンドラー 間で共有するための、
        /// 実行者 アカウント キー の キー 名を表します。
        /// </summary>
        public static readonly string JWT_EXECUTOR_ACCOUNT_KEY = "QolmsJotoJwtExecutorAccountKey";

        /// <summary>
        /// モジュール と ハンドラー 間で共有するための、
        /// 所有者 アカウント キー の キー 名を表します。
        /// </summary>
        public static readonly string JWT_AUTHOR_ACCOUNT_KEY = "QolmsJotoJwtAuthorAccountKey";

        /// <summary>
        /// モジュール と ハンドラー 間で共有するための、
        /// 対象者 アカウント キー の キー 名を表します。
        /// </summary>
        public static readonly string JWT_TARGET_ACCOUNT_KEY = "QolmsJotoJwtTargetAccountKey";

        /// <summary>
        /// モジュール と ハンドラー 間で共有するための、
        /// ページ 番号の キー 名を表します。
        /// </summary>
        public static readonly string JWT_PAGE_NO = "QolmsJotoJwtPageNo";

        #endregion

        #region "Constructor"

        /// <summary>
        /// <see cref="QjJwtAuthorizeAttribute" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        public QjJwtAuthorizeAttribute()
            : base() { }

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
        {
            // TODO: デバッグ
            //AccessLogWorker.WriteAccessLog(httpContext, "SSO Authorization Header", AccessLogWorker.AccessTypeEnum.None, httpContext.Request.Headers["Authorization"]);

            //TODO: QhJwtTokenValidationResults -> QjJwtTokenValidationResults
            // JWT の チェック
            QjJwtTokenValidationResults jwt = SsoHelper.ValidateJwt(SsoHelper.GetJwt(httpContext.Request.Headers["Authorization"]));

            // 実行者（呼び出し元 アプリ に割り当てられた アカウント キー）は復号化する必要がある
            Guid executor = Guid.Empty;

            if (!string.IsNullOrWhiteSpace(jwt.Executor))
            {
                using (var cryptor = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
                {
                    try
                    {
                        executor = cryptor.DecryptString(jwt.Executor).TryToValueType(Guid.Empty);
                    }
                    catch { }
                }
            }

            // 本人が本人の データ を見る場合：ParentKey は Guid.Empty
            // 本人が家族の データ を見る場合：ParentKey が 本人の アカウント キー
            httpContext.Items.Remove(QjJwtAuthorizeAttribute.JWT_EXECUTOR_ACCOUNT_KEY);
            httpContext.Items.Remove(QjJwtAuthorizeAttribute.JWT_AUTHOR_ACCOUNT_KEY);
            httpContext.Items.Remove(QjJwtAuthorizeAttribute.JWT_TARGET_ACCOUNT_KEY);
            httpContext.Items.Remove(QjJwtAuthorizeAttribute.JWT_PAGE_NO);

            if (jwt.IsSuccess && executor != Guid.Empty && jwt.AccountKey != Guid.Empty && jwt.PageNo > 0)
            {
                httpContext.Items.Add(QjJwtAuthorizeAttribute.JWT_EXECUTOR_ACCOUNT_KEY, executor);
                httpContext.Items.Add(QjJwtAuthorizeAttribute.JWT_AUTHOR_ACCOUNT_KEY, jwt.ParentKey != Guid.Empty ? jwt.ParentKey : jwt.AccountKey);
                httpContext.Items.Add(QjJwtAuthorizeAttribute.JWT_TARGET_ACCOUNT_KEY, jwt.AccountKey);
                httpContext.Items.Add(QjJwtAuthorizeAttribute.JWT_PAGE_NO, jwt.PageNo);

                // TODO: デバッグ
                AccessLogWorker.WriteAccessLog(httpContext, "SSO Authorization True", AccessLogWorker.AccessTypeEnum.None, string.Empty);

                return true;
            }
            else
            {
                // TODO: デバッグ
                AccessLogWorker.WriteAccessLog(httpContext, "SSO Authorization False", AccessLogWorker.AccessTypeEnum.None, string.Empty);

                return false;
            }
        }

        /// <summary>
        /// 承認されなかった HTTP 要求を処理します。
        /// </summary>
        /// <param name="filterContext">フィルター コンテキスト。</param>
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // ログイン ページ への リダイレクト を抑制するかを設定
            filterContext.HttpContext.Response.SuppressFormsAuthenticationRedirect = true;

            base.HandleUnauthorizedRequest(filterContext);
        }

        #endregion
    }
}