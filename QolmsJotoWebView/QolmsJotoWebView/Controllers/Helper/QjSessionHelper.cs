using System;
using System.Web;
using System.Web.Configuration;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// セッション状態オブジェクトに関する補助機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    internal sealed class QjSessionHelper
    {
        #region "Constant"

        /// <summary>
        /// セッション ID を保持する HTTP クッキー名を表します。
        /// </summary>
        private static readonly string COOKIE_NAME = ((SessionStateSection)WebConfigurationManager.GetSection("system.web/sessionState")).CookieName.Trim();

        #endregion

        #region "Constructor"
        public QjSessionHelper() : base() { }

        #endregion

        #region "Public Method"

        /// <summary>
        /// セッションをキャンセルし、
        /// 次回 HTTP 要求時に新規セッションを開始します。
        /// </summary>
        /// <param name="session">セッション状態オブジェクト。</param>
        /// <param name="response">HTTP 応答。</param>
        public static void NewSession(HttpSessionStateBase session, HttpResponseBase response)
        {
            session.Abandon();
            response.Cookies.Add(new HttpCookie(QjSessionHelper.COOKIE_NAME, String.Empty));
        }

        /// <summary>
        /// セッション状態オブジェクトから値を取得します。
        /// </summary>
        /// <typeparam name="T">取得するオブジェクトの型。</typeparam>
        /// <param name="session">セッション状態オブジェクト。</param>
        /// <param name="key">キー名。</param>
        /// <param name="refValue">取得したオブジェクトが格納される変数。</param>
        /// <returns>
        /// 成功なら True、
        /// 失敗なら False。
        /// </returns>
        public static bool GetItem<T>(HttpSessionStateBase session, string key, ref T refValue)
        {
            var result = false;

            if (session[key] != null)
            {
                try
                {
                    refValue = (T)session[key];
                    result = true;
                }
                catch
                {
                }
            }
            return result;
        }

        /// <summary>
        /// セッション状態オブジェクトへ値を追加します。
        /// </summary>
        /// <param name="session">セッション状態オブジェクト。</param>
        /// <param name="key">キー名。</param>
        /// <param name="value">追加するオブジェクト。</param>
        public static void SetItem(HttpSessionStateBase session, string key, object value)
        {
            session.Add(key, value);
        }


        /// <summary>
        /// セッション状態オブジェクトから値を削除します。
        /// </summary>
        /// <param name="session">セッション状態オブジェクト。</param>
        /// <param name="key">キー名。</param>
        public static void RemoveItem(HttpSessionStateBase session, string key)
        {
            session.Remove(key);
        }

        #endregion

    }
}