using System;
using System.Web;

namespace MGF.QOLMS.QolmsJotoWebView
{
    internal sealed class QjLoginHelper
    {
        #region "Constant"

        /// <summary>
        /// セッション状態オブジェクト内でのログインモデルのキー名を表します。
        /// </summary>
        private static readonly string LOGIN_MODEL_SESSION_KEY = typeof(LoginModel).Name;

        /// <summary>LoginModel
        /// セッション状態オブジェクト内でのメインモデルのキー名を表します。
        /// </summary>
        private static readonly string MAIN_MODEL_SESSION_KEY = typeof(QolmsJotoModel).Name;

        /// <summary>
        /// 自動ログインの期限日数を表します。（本日から）
        /// </summary>
        private const int AUTOLOGIN_EXPIRES = 20;

        #endregion

        #region "Constructor"
        private QjLoginHelper() : base() { }

        #endregion

        #region "Private Method"

        private static bool CheckQolmsYappliModel(QolmsJotoModel mainModel)
        {
            return mainModel != null
                && mainModel.AuthorAccount != null
                && mainModel.AuthorAccount.AccountKey != Guid.Empty
                && ! string.IsNullOrWhiteSpace(mainModel.AuthorAccount.Name);
        }

        #endregion

        #region "Private Method"

        /// <summary>
        /// ログインモデルを取得します。
        /// </summary>
        /// <param name="session">セッション状態オブジェクト。</param>
        /// <returns>
        /// 成功ならログインモデルのインスタンス、
        /// 失敗なら Nothing。
        /// </returns>
        public static LoginModel GetLoginModel(HttpSessionStateBase session)
        {
            LoginModel result = null;

            return QjSessionHelper.GetItem(session, QjLoginHelper.LOGIN_MODEL_SESSION_KEY,ref result) ? result : null;
        }

        /// <summary>
        /// ログインモデルを削除します。
        /// </summary>
        /// <param name="session">セッション状態オブジェクト。</param>
        public static void RemoveLoginModel(HttpSessionStateBase session)
        {

            QjSessionHelper.RemoveItem(session, QjLoginHelper.LOGIN_MODEL_SESSION_KEY);
        }

        /// <summary>
        /// ログインモデルを追加します。
        /// </summary>
        /// <param name="session">セッション状態オブジェクト。</param>
        /// <param name="model">ログイン処理モデル。</param>
        public static void RemoveLoginModel(HttpSessionStateBase session, LoginModel model)
        {

            QjSessionHelper.RemoveItem(session, QjLoginHelper.LOGIN_MODEL_SESSION_KEY);
            QjSessionHelper.SetItem(session, QjLoginHelper.LOGIN_MODEL_SESSION_KEY, model);
        }

        /// <summary>
        /// メイン モデルを取得します。
        /// </summary>
        /// <param name="session">セッション状態オブジェクト。</param>
        /// <returns>
        /// 成功ならメイン モデルのインスタンス、
        /// 失敗なら null。
        /// </returns>
        public static QolmsJotoModel GetQolmsJotoModel(HttpSessionStateBase session)
        {
            QolmsJotoModel result = null;

            return QjSessionHelper.GetItem(session, QjLoginHelper.MAIN_MODEL_SESSION_KEY,ref result) && QjLoginHelper.CheckQolmsYappliModel(result) ? result : null;
        }

        /// <summary>
        /// ログイン済みかチェックします。
        /// </summary>
        /// <param name="session">セッション状態オブジェクト。</param>
        /// <param name="request">HTTP 要求。</param>
        /// <returns>
        /// ログイン済みなら True、
        /// 未ログインなら False。
        /// </returns>

        public static bool CheckLogin(HttpSessionStateBase session, HttpRequestBase request)
        {

            QolmsJotoModel model = null;

            return QjSessionHelper.GetItem(session, QjLoginHelper.MAIN_MODEL_SESSION_KEY, ref model)
                && QjLoginHelper.CheckQolmsYappliModel(model)
                && QjCookieHelper.CheckFormsAuthCookie(request, model.AuthorAccount.UserId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="response"></param>
        /// <param name="authorAccount"></param>
        /// <param name="apiAuthorizeKey"></param>
        /// <param name="apiAuthorizeExpires"></param>
        /// <param name="apiAuthorizeKey2"></param>
        /// <param name="apiAuthorizeExpires2"></param>
        /// <param name="rememberId"></param>
        /// <param name="rememberLogin"></param>
        /// <returns></returns>
        public static bool ToLogin(
            HttpSessionStateBase session,
            HttpResponseBase response,
            AuthorAccountItem authorAccount,
            Guid apiAuthorizeKey,
            DateTime apiAuthorizeExpires,
            Guid apiAuthorizeKey2,
            DateTime apiAuthorizeExpires2,
            bool rememberId,
            bool rememberLogin)
        {

            var result = false;

            try
            {
                QolmsJotoModel mainModel = new QolmsJotoModel(
                    authorAccount,
                    session.SessionID,
                    apiAuthorizeKey,
                    apiAuthorizeExpires,
                    apiAuthorizeKey2,
                    apiAuthorizeExpires2
                );

                if (mainModel != null)
                {
                    // 認証クッキーを設定
                    QjCookieHelper.SetFormsAuthCookie(response, authorAccount.UserId, session.Timeout);

                    QjCookieHelper.SetRememberIdCookie(response, rememberId, authorAccount.UserId, authorAccount.LoginAt);
                    //mainModel.SetIsAutoLogin(QjCookieHelper.SetRememberLoginCookie(response, rememberLogin, authorAccount.UserId, authorAccount.PasswordHash, authorAccount.LoginAt, DateTime.Now.AddDays(AUTOLOGIN_EXPIRES)));

                    QjSessionHelper.RemoveItem(session, QjLoginHelper.MAIN_MODEL_SESSION_KEY);
                    QjSessionHelper.SetItem(session, QjLoginHelper.MAIN_MODEL_SESSION_KEY, mainModel);

                    // 成功
                    result = true;
                }
            }
            catch
            {
                throw;
            }

            return result;
        }

        public static bool ToLogin(
            HttpSessionStateBase session,
            HttpResponseBase response,
            AuthorAccountItem authorAccount,
            Guid apiAuthorizeKey,
            DateTime apiAuthorizeExpires,
            Guid apiAuthorizeKey2,
            DateTime apiAuthorizeExpires2
            )
        {
            var result = false;
            try
            {
                QolmsJotoModel mainModel = new QolmsJotoModel(
                        authorAccount,
                        session.SessionID,
                        apiAuthorizeKey,
                        apiAuthorizeExpires,
                        apiAuthorizeKey2,
                        apiAuthorizeExpires2
                    );
                if (mainModel != null)
                {
                    // 認証クッキーを設定
                    QjCookieHelper.SetFormsAuthCookie(response, authorAccount.UserId, session.Timeout);
                    QjSessionHelper.RemoveItem(session, QjLoginHelper.MAIN_MODEL_SESSION_KEY);
                    QjSessionHelper.SetItem(session, QjLoginHelper.MAIN_MODEL_SESSION_KEY, mainModel);
                    // 成功
                    result = true;
                }
            }
            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// ログアウト状態へ移行します。
        /// </summary>
        /// <param name="session"></param>
        /// <param name="response"></param>
        /// <param name="clearRemember"></param>
        public static void ToLogout(HttpSessionStateBase session, HttpResponseBase response, bool clearRemember)
        {
            QjSessionHelper.RemoveItem(session, QjLoginHelper.MAIN_MODEL_SESSION_KEY);

            if (clearRemember)
            {
                //QjCookieHelper.DisableRememberLoginCookie(response);
                //QjCookieHelper.DisableRememberIdCookie(response);
                //auのautoログイン設定のcookieを削除
                //QjCookieHelper.DisableAutoAuIdLoginCookie(response);
            }
        }

        #endregion
    }
}