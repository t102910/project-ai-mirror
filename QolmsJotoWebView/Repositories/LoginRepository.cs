using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// アカウント情報への入出力インターフェース
    /// </summary>
    public interface ILoginRepository
    {
        /// <summary>
        /// QH_ACCOUNT_MSTから主キーでレコードを取得する
        /// </summary>
        /// <param name="accountKey"></param>
        /// <returns></returns>
        Guid SsoAccountExists(Guid executer, Guid accountKey);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="callerAccountKey"></param>
        /// <param name="authorAccountKey"></param>
        /// <param name="actorAccountKey"></param>
        /// <returns></returns>
        QiQolmsJotoSsoApiResults Sso(string sessionId, Guid callerAccountKey, Guid authorAccountKey, Guid actorAccountKey);

    }

    public class LoginRepository: ILoginRepository
    {
        /// <summary>
        /// ダミーのセッションIDを現します。
        /// </summary>
        readonly string DUMMY_SESSION_ID = new string('Z', 100);

        /// <summary>
        /// ダミーのAPI認証キーを表します
        /// </summary>
        readonly Guid DUMMY_API_AUTHORIZE_KEY = new Guid(new string('F', 32));

        /// <summary>
        /// QH_ACCOUNT_MSTから主キーでレコードを取得する
        /// </summary>
        /// <param name="accountKey"></param>
        /// <returns></returns>
        public Guid SsoAccountExists(Guid executer, Guid accountKey)
        {
            //Identityを呼び出してJWT用の存在チェック
            var apiArgs = new QiQolmsJotoSsoAccountExistsApiArgs(executer, string.Empty)
            {
                ActorKey = accountKey.ToApiGuidString()
            };

            var apiResults = QsApiManager.ExecuteQolmsIdentityApi<QiQolmsJotoSsoAccountExistsApiResults>(
                apiArgs,
                DUMMY_SESSION_ID,
                DUMMY_API_AUTHORIZE_KEY
                );

            if (apiResults.IsSuccess.TryToValueType(false))
            {
                return apiResults.Accountkey.TryToValueType(Guid.Empty);
            }

            return Guid.Empty;
        }


        /// <summary>
        /// SSO アカウント 確認 API を実行します。
        /// </summary>
        /// <param name="sessionId">セッション ID。</param>
        /// <param name="callerAccountKey">呼び出し元 アカウント キー（アプリ 等に割り当てられた アカウント キー）。</param>
        /// <param name="authorAccountKey">所有者 アカウント キー。</param>
        /// <param name="actorAccountKey">対象者 アカウント キー。</param>
        /// </param>
        /// <returns>
        /// Web API 戻り値 クラス。
        /// </returns>
        public QiQolmsJotoSsoApiResults Sso
        (
            string sessionId,
            Guid callerAccountKey,
            Guid authorAccountKey,
            Guid actorAccountKey
        )
        {

            //JOTO用のログインを実装する
            //リポジトリにする
            var apiArgs = new QiQolmsJotoSsoApiArgs(
                QiApiTypeEnum.QolmsSso,
                QsApiSystemTypeEnum.Qolms,
                authorAccountKey,
                string.Empty
            )
            {
                SessionId = sessionId,
                CallerKey = callerAccountKey.ToApiGuidString(),
                ActorKey = actorAccountKey.ToApiGuidString()
            };
            QiQolmsJotoSsoApiResults apiResults = QsApiManager.ExecuteQolmsIdentityApi<QiQolmsJotoSsoApiResults>(
                apiArgs,
                DUMMY_SESSION_ID,
                DUMMY_API_AUTHORIZE_KEY
            );

            if (apiResults.IsSuccess.TryToValueType(false))
                return apiResults;
            else
                throw new InvalidOperationException(string.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsIdentityApiName(apiArgs)));
        }

    }
}