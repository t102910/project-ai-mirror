using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// Web API を実行するための認証 キー に関する機能を提供します。
    /// この クラス は継承できません。
    /// </summary>
    internal sealed class ApiAuthorizeWorker
    {
        #region "Constant"

        /// <summary>
        /// ダミー の セッション ID を表します。
        /// </summary>
        private static string DUMMY_SESSION_ID = new string('Z', 100);

        /// <summary>
        /// ダミー の API 認証 キー を表します。
        /// </summary>
        private static Guid DUMMY_API_AUTHORIZE_KEY = new Guid(new string('F', 32));

        #endregion

        #region "Constructor"
        public ApiAuthorizeWorker() : base() { }

        #endregion

        #region "Private Method"


        /// <summary>
        /// QolmsApi 用 API 認証 キー 取得 API を実行します。
        /// </summary>
        /// <param name="executor">実行者 アカウント キー。</param>
        /// <param name="sessionId">セッション ID。</param>
        /// <returns></returns>
        private static QiQolmsNewAuthorizeKeyApiResults ExecuteQolmsNewAuthorizeKeyApi(Guid executor, string sessionId)
        {
            var apiArgs = new QiQolmsJotoNewAuthorizeKeyApiArgs(
                QiApiTypeEnum.QolmsNewAuthorizeKey,
                QsApiSystemTypeEnum.Qolms,
                executor,
                string.Empty)
            {
                SessionId = sessionId
            };

            QiQolmsNewAuthorizeKeyApiResults apiResults = QsApiManager.ExecuteQolmsIdentityApi<QiQolmsNewAuthorizeKeyApiResults>(
                apiArgs,
                ApiAuthorizeWorker.DUMMY_SESSION_ID,
                ApiAuthorizeWorker.DUMMY_API_AUTHORIZE_KEY);

            if (apiResults.IsSuccess.TryToValueType(false))
            {
                return apiResults;
            }
            else
            {
                throw new InvalidOperationException(String.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsIdentityApiName(apiArgs)));
            }

        }


        /// <summary>
        /// QolmsJotoApi 用 API 認証 キー 取得 API を実行します。
        /// </summary>
        /// <param name="executor">実行者 アカウント キー。</param>
        /// <param name="sessionId">セッション ID。</param>
        /// <returns></returns>
        private static QiQolmsJotoNewAuthorizeKeyApiResults ExecuteQolmsJotoNewAuthorizeKeyApi(Guid executor, string sessionId)
        {
            var apiArgs = new QiQolmsJotoNewAuthorizeKeyApiArgs(
                QiApiTypeEnum.QolmsJotoNewAuthorizeKey,
                QsApiSystemTypeEnum.QolmsJoto,
                executor,
                string.Empty)
            {
                SessionId = sessionId
            };

            QiQolmsJotoNewAuthorizeKeyApiResults apiResults = QsApiManager.ExecuteQolmsIdentityApi<QiQolmsJotoNewAuthorizeKeyApiResults>(
                apiArgs,
                ApiAuthorizeWorker.DUMMY_SESSION_ID,
                ApiAuthorizeWorker.DUMMY_API_AUTHORIZE_KEY);

            if (apiResults.IsSuccess.TryToValueType(false))
            {
                return apiResults;
            }
            else
            {
                throw new InvalidOperationException(String.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsIdentityApiName(apiArgs)));
            }

        }

        #endregion

        #region "Public Method"

        /// <summary>
        /// 新しい QolmsJotoApi 用の API 認証 キー と API 有効期限を取得します。
        /// </summary>
        /// <param name="executor">実行者 アカウント キー。</param>
        /// <param name="sessionId">セッション ID。</param>
        /// <param name="refAuthorizeKey">取得した API 認証 キー が格納される変数。</param>
        /// <param name="refAuthorizeExpires">取得した API 有効期限が格納される変数。</param>
        public static void NewKey(Guid executor, string sessionId, ref Guid refAuthorizeKey, ref DateTime refAuthorizeExpires)
        {
            var apiResult = ApiAuthorizeWorker.ExecuteQolmsNewAuthorizeKeyApi(executor, sessionId);
            refAuthorizeKey = apiResult.AuthorizeKey.TryToValueType(Guid.Empty);
            refAuthorizeExpires = apiResult.AuthorizeExpires.TryToValueType(DateTime.MinValue);
        }

        /// <summary>
        /// 新しい QolmsJotoApi 用の API 認証 キー と API 有効期限を取得します。
        /// </summary>
        /// <param name="executor">実行者 アカウント キー。</param>
        /// <param name="sessionId">セッション ID。</param>
        /// <param name="refAuthorizeKey">取得した API 認証 キー が格納される変数。</param>
        /// <param name="refAuthorizeExpires">取得した API 有効期限が格納される変数。</param>
        public static void NewKey2(Guid executor, string sessionId, ref Guid refAuthorizeKey, ref DateTime refAuthorizeExpires)
        {
            var apiResult =  ApiAuthorizeWorker.ExecuteQolmsJotoNewAuthorizeKeyApi(executor, sessionId);
            refAuthorizeKey = apiResult.AuthorizeKey.TryToValueType(Guid.Empty);
            refAuthorizeExpires = apiResult.AuthorizeExpires.TryToValueType(DateTime.MinValue);
        }

        #endregion

    }
}