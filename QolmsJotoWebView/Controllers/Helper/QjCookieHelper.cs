using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsCryptV1;
using System;
using System.Web;
using System.Web.Security;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// クッキーに関する補助機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    internal sealed class QjCookieHelper
    {

        #region "Constant"

        /// <summary>
        ///  認証チケット名を表します。
        /// </summary>
        private const string AUTH_TICKET_NAME = "QolmsYappliAuthTicket";
        private const string REMEMBER_ID_COOKIE_NAME = "Mgf.Qolms.QolmsYappliRememberId";
        private const string REMEMBER_LOGIN_COOKIE_NAME = "Mgf.Qolms.QolmsYappliRememberLogin";
        private const string AGREEMENT_VERSION_COOKIE_NAME = "Mgf.Qolms.QolmsYappliAgreementVersion";
        private const string AUTO_AUIDLOGIN_COOKIE_NAME = "Mgf.Qolms.QolmsYappliAutoAuIdLogin";
        private const string ENTRY_APPLEIDLOGIN_COOKIE_NAME = "Mgf.Qolms.QolmsYappliEntryAppleIdLogin";

        #endregion

        #region "Constructor"
        public QjCookieHelper() : base() { }

        #endregion

        #region "Public Method"

        /// <summary>
        /// 認証クッキーを設定します。
        /// </summary>
        /// <param name="response">HTTP 応答。</param>
        /// <param name="userId">ユーザー ID。</param>
        /// <param name="timeout">セッション タイムアウトまでの時間（分）。</param>
        public static void SetFormsAuthCookie(HttpResponseBase response, string userId, int timeout)
        {
            // TODO: 引数のチェック

            // TODO: 要検討、認証チケットを作成
            var ticket = new FormsAuthenticationTicket(
                1,
                QjCookieHelper.AUTH_TICKET_NAME,
                DateTime.Now,
                DateTime.Now.AddMinutes(timeout),
                false,
                new AuthTicketJsonParameter() { UserId = userId }.ToJsonString()
            );
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));

            // 有効期限
            cookie.Expires = DateTime.Now.AddMinutes(timeout);

            // パス
            cookie.Path = FormsAuthentication.FormsCookiePath;

            // クライアントスクリプトからのアクセスを許可しない
            cookie.HttpOnly = true;

            // クッキーに追加
            response.Cookies.Add(cookie);
        }

        /// <summary>
        /// 認証クッキーの有効性をチェックします。
        /// </summary>
        /// <param name="request">HTTP 要求。</param>
        /// <param name="userId">ユーザー ID。</param>
        /// <returns></returns>
        public static bool CheckFormsAuthCookie(HttpRequestBase request, string userId)
        {
            // TODO: 引数のチェック

            var result = false;
            var cookie = request.Cookies[FormsAuthentication.FormsCookieName];

            if (cookie!= null)
            {
                try
                {
                    var ticket = FormsAuthentication.Decrypt(cookie.Value);
                    result = String.Compare(ticket.Name, QjCookieHelper.AUTH_TICKET_NAME, true) == 0
                        && String.Compare(QjJsonParameterBase.FromJsonString< AuthTicketJsonParameter>(ticket.UserData).UserId, userId, true) == 0;
                }
                catch 
                {
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        /// <param name="remember"></param>
        /// <param name="userId"></param>
        /// <param name="loginAt"></param>
        /// <returns></returns>
        public static bool SetRememberIdCookie(  HttpResponseBase response,   bool remember,   string userId,   DateTime loginAt)
        {
            var value = string.Empty;

            if (remember && ! string.IsNullOrWhiteSpace(userId) && loginAt != DateTime.MinValue)
            {
                try
                {
                    using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsWeb))
                    {
                        value = crypt.EncryptString(
                            new RememberIdCookieJsonParameter(){
                                UserId = userId,
                                LoginAt = loginAt.ToApiDateString()
                            }.ToJsonString()
                        );
                    }
                }
                catch 
                {
                }
            }

            var cookie = new HttpCookie(QjCookieHelper.REMEMBER_ID_COOKIE_NAME, value);

            // 有効期限
            cookie.Expires = loginAt.AddYears(1).AddDays(-1);

            // パス
            if (FormsAuthentication.FormsCookiePath.EndsWith("/"))
            {
                cookie.Path = String.Format("{0}{1}", FormsAuthentication.FormsCookiePath, "Start/LoginById");
            }
            else
            {
                cookie.Path = String.Format("{0}{1}", FormsAuthentication.FormsCookiePath, "/Start/LoginById");
            }
            // クライアントスクリプトからのアクセスを許可しない
            cookie.HttpOnly = true;

            // クッキーに追加
            response.Cookies.Add(cookie);

            return !string.IsNullOrWhiteSpace(value);
        }

        #endregion
    }
}