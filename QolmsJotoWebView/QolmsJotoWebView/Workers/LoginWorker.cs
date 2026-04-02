using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using System;
using System.Collections.Generic;

namespace MGF.QOLMS.QolmsJotoWebView
{
    public class LoginWorker
    {
        /// <summary>
        /// ダミーのセッションIDを現します。
        /// </summary>
        readonly static string DUMMY_SESSION_ID = new string('Z', 100);

        /// <summary>
        /// ダミーのAPI認証キーを表します
        /// </summary>
        readonly static Guid DUMMY_API_AUTHORIZE_KEY = new Guid(new string('F', 32));

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
        private static QiQolmsJotoSsoApiResults ExecuteQolmsSsoApi
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
                LoginWorker.DUMMY_SESSION_ID,
                LoginWorker.DUMMY_API_AUTHORIZE_KEY
            );

            if (apiResults.IsSuccess.TryToValueType(false))
                return apiResults;
            else
                throw new InvalidOperationException(string.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsIdentityApiName(apiArgs)));
        }


        internal static QsApiLoginResultTypeEnum Auth(string sessionId,
            Guid executorAccountKey,
            Guid authorAccountKey,
            Guid targetAccountKey,
            ref AuthorAccountItem refAuthorAccount,
            ref List<QhApiTargetValueItem> refStandardValueN,
            ref List<QhApiTargetValueItem> refTargetValueN,
            ref decimal refHeight,
            ref Guid refApiAuthorizeKey,
            ref DateTime refApiAuthorizeExpires,
            ref Guid refApiAuthorizeKey2,
            ref DateTime refApiAuthorizeExpires2,
            ref byte refLoginRetryCount,
            ref DateTime refLoginLockdownExpires,
            ref bool refIsSettingComplete)
        {
            refAuthorAccount = new AuthorAccountItem()
            {
                UserId = string.Empty,
                LoginAt = DateTime.MinValue,
                AccountKey = Guid.Empty,
                FamilyName = string.Empty,
                MiddleName = string.Empty,
                GivenName = string.Empty,
                FamilyKanaName = string.Empty,
                MiddleKanaName = string.Empty,
                GivenKanaName = string.Empty,
                FamilyRomanName = string.Empty,
                MiddleRomanName = string.Empty,
                GivenRomanName = string.Empty,
                SexType = QjSexTypeEnum.None,
                Birthday = DateTime.MinValue,
                EncryptedAccountKey = string.Empty
            };

            refApiAuthorizeKey = Guid.Empty;
            refApiAuthorizeExpires = DateTime.MinValue;

            // QolmsJotoApi 用
            refApiAuthorizeKey2 = Guid.Empty;
            refApiAuthorizeExpires2 = DateTime.MinValue;

            QiQolmsJotoSsoApiResults apiResults = LoginWorker.ExecuteQolmsSsoApi(sessionId, executorAccountKey, authorAccountKey, targetAccountKey);
            QsApiLoginResultTypeEnum result = apiResults.LoginResultType.TryToValueType(QsApiLoginResultTypeEnum.None);

            if (result == QsApiLoginResultTypeEnum.Success)
            {
                //todo: ダミーを一旦返しておくログイン実装
                //' ログイン状態へ移行するために必要な情報を返却
                refAuthorAccount.AccountKey = apiResults.AccountKey.TryToValueType(Guid.Empty);
                //refAuthorAccount.AccountKeyHash = apiResults.AccountKeyHash;
                refAuthorAccount.FamilyName = apiResults.FamilyName;
                refAuthorAccount.MiddleName = apiResults.MiddleName;
                refAuthorAccount.GivenName = apiResults.GivenName;
                refAuthorAccount.FamilyKanaName = apiResults.FamilyKanaName;
                refAuthorAccount.MiddleKanaName = apiResults.MiddleKanaName;
                refAuthorAccount.GivenKanaName = apiResults.GivenKanaName;
                refAuthorAccount.FamilyRomanName = apiResults.FamilyRomanName;
                refAuthorAccount.MiddleRomanName = apiResults.MiddleRomanName;
                refAuthorAccount.GivenRomanName = apiResults.GivenRomanName;
                refAuthorAccount.SexType = apiResults.SexType.TryToValueType(QjSexTypeEnum.None);
                refAuthorAccount.Birthday = apiResults.Birthday.TryToValueType(DateTime.MinValue);
                //refAuthorAccount.AcceptFlag = apiResults.AcceptFlag.TryToValueType(false);
                refAuthorAccount.EncryptedAccountKey = QjAccountItemBase.EncryptAccountKey(apiResults.AccountKey.TryToValueType(Guid.Empty));
                refAuthorAccount.UserId = apiResults.UserId;
                refAuthorAccount.PasswordHash = apiResults.PasswordHash;
                refAuthorAccount.LoginCount = apiResults.LoginCount.TryToValueType(int.MinValue);
                refAuthorAccount.LoginAt = apiResults.LoginAt.TryToValueType(DateTime.MinValue);
                refIsSettingComplete = true;

                //' QolmsApi 用;
                refApiAuthorizeKey = apiResults.AuthorizeKey.TryToValueType(Guid.Empty); ;
                refApiAuthorizeExpires = apiResults.AuthorizeExpires.TryToValueType(DateTime.MinValue);

                //' QolmsJotoApi 用
                refApiAuthorizeKey2 = apiResults.AuthorizeKey2.TryToValueType(Guid.Empty); ;
                refApiAuthorizeExpires2 = apiResults.AuthorizeExpires2.TryToValueType(DateTime.MinValue);
            }
            return result;
        }
    }
}