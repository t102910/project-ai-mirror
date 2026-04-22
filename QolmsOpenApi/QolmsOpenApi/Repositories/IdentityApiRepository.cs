using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Models;
using MGF.QOLMS.QolmsOpenApi.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Repositories
{
    /// <summary>
    /// QolmsIdentityApiとの入出力インターフェース
    /// QoIdentityClientは段階的にこちらに移植予定
    /// </summary>
    public interface IIdentityApiRepository
    {
        /// <summary>
        /// ログインAPIを実行する
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <param name="passwordHash"></param>
        /// <param name="useTwoFactorAuthentication"></param>
        /// <param name="twoFactorAuthenticationToken"></param>
        /// <returns></returns>
        QiQolmsLoginApiResults ExecuteLoginApi(string sessionId, string userId, string password, string passwordHash, bool useTwoFactorAuthentication, string twoFactorAuthenticationToken);

        /// <summary>
        /// 本登録する
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userPass"></param>
        /// <param name="familyName"></param>
        /// <param name="givenName"></param>
        /// <param name="familyKananame"></param>
        /// <param name="givenKanaName"></param>
        /// <param name="sexType"></param>
        /// <param name="birthDay"></param>
        /// <param name="mailAddress"></param>
        /// <returns></returns>
        IdentityRegisterApiResults ExecuteRegisterWriteApi(string userId, string userPass,
            string familyName, string givenName, string familyKananame, string givenKanaName,
            string sexType, string birthDay, string mailAddress);

        /// <summary>
        /// 連携ユーザーとして本登録する
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <param name="linkageSystemId"></param>
        /// <param name="userId"></param>
        /// <param name="userPass"></param>
        /// <param name="familyName"></param>
        /// <param name="givenName"></param>
        /// <param name="familyKananame"></param>
        /// <param name="givenKanaName"></param>
        /// <param name="sexType"></param>
        /// <param name="birthDay"></param>
        /// <param name="mailAddress"></param>
        /// <returns></returns>
        IdentityRegisterApiResults ExecuteLinkageUserRegisterApi(
            string linkageSystemNo, string linkageSystemId, string userId, string userPass,
            string familyName, string givenName, string familyKananame, string givenKanaName,
            string sexType, string birthDay, string mailAddress);

        /// <summary>
        /// 仮登録データを取得する
        /// </summary>
        /// <param name="accountkey">対象仮アカウントキー</param>
        /// <returns></returns>
        QiQolmsAccountRegisterReadApiResults ExecuteRegisterReadApi(Guid accountkey);

        /// <summary>
        /// 家族アカウントを追加する
        /// </summary>
        /// <param name="actorKey"></param>
        /// <param name="executorName"></param>
        /// <param name="account"></param>
        /// <param name="sex"></param>
        /// <param name="birthDay"></param>
        /// <returns></returns>
        QiQolmsAccountConnectFamilyAccountEditWriteApiResults ExecuteAccountConnectFamilyAccountEditWriteApi(Guid actorKey, string executorName, QoApiAccountFamilyInputItem account, string sex, string birthDay);
    }

    /// <summary>
    /// QolmsIdentityApiとの実装
    /// QoIdentityClientは段階的にこちらに移植予定
    /// </summary>
    public class IdentityApiRepository : IIdentityApiRepository
    {
        /// <summary>
        ///ダミーのセッションIDを現します。
        /// </summary>
        /// <remarks></remarks>
        private static string DUMMY_SESSION_ID = new string('Z', 100);

        /// <summary>
        /// ダミーのAPI認証キーを表します。
        /// </summary>
        /// <remarks></remarks>
        private static Guid DUMMY_API_AUTHORIZE_KEY = new Guid(new string('F', 32));

        public QiQolmsLoginApiResults ExecuteLoginApi(string sessionId, string userId, string password, string passwordHash, bool useTwoFactorAuthentication, string twoFactorAuthenticationToken)
        {
            var apiArgs = new QiQolmsLoginApiArgs(
                QiApiTypeEnum.QolmsLogin, 
                QsApiSystemTypeEnum.QolmsOpenApi, 
                Guid.Empty, 
                string.Empty)
            {
                SessionId = string.IsNullOrWhiteSpace(sessionId) ? DUMMY_SESSION_ID : sessionId,
                UserId = userId.Trim(),
                Password = password.Trim(),
                PasswordHash = passwordHash.Trim(),
                UseTwoFactorAuthentication = useTwoFactorAuthentication.ToString(),
                TwoFactorAuthenticationToken = twoFactorAuthenticationToken.Trim()
            };

            var apiResults = QsApiManager.ExecuteQolmsIdentityApi<QiQolmsLoginApiResults>(apiArgs, DUMMY_SESSION_ID, DUMMY_API_AUTHORIZE_KEY);

            if (apiResults.IsSuccess.TryToValueType(false))
            {
                return apiResults;
            }
            else
            {
                throw new InvalidOperationException(string.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsIdentityApiName(apiArgs)));
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IdentityRegisterApiResults ExecuteRegisterWriteApi(string userId, string userPass,
            string familyName, string givenName, string familyKananame, string givenKanaName,
            string sexType, string birthDay, string mailAddress)
        {
            var birthDate = birthDay.TryToValueType(DateTime.MinValue);
            if(birthDate == DateTime.MinValue)
            {
                return new IdentityRegisterApiResults
                {
                    IsSuccess = false
                };
            }

            var newAccountKey = Guid.NewGuid().ToString("N");

            var apiArgs = new QiQolmsAccountRegisterWriteApiArgs(Guid.Empty, string.Empty)
            {
                Accountkey = newAccountKey,
                FamilyName = familyName,
                GivenName = givenName,
                FamilyKanaName = familyKananame,
                GivenKanaName = givenKanaName,
                // このAPIは性別はMale / Femaleの文字列で指定する仕様。1 or 2ではない
                Sex = sexType.ToValueType<QsDbSexTypeEnum>().ToString(), 
                BirthYear = birthDate.Year.ToString(),
                BirthMonth = birthDate.Month.ToString(),
                BirthDay = birthDate.Day.ToString(),
                MailAddress = mailAddress,
                UserId = userId,
                Password = userPass,
                AccountType = "1",
                PrivateAccountFlag = bool.FalseString
            };

            var apiResults = QsApiManager.ExecuteQolmsIdentityApi<QiQolmsAccountRegisterWriteApiResults>(apiArgs, DUMMY_SESSION_ID, DUMMY_API_AUTHORIZE_KEY);


            QoAccessLog.WriteInfoLog($"AutoGenerate UserID: {userId}");
            QoAccessLog.WriteInfoLog($"AccountKey: {newAccountKey}");

            return new IdentityRegisterApiResults
            {
                IsSuccess = apiResults.IsSuccess == bool.TrueString,
                AccountKey = newAccountKey,
                ErrorList = apiResults.ErrorList
            };
        }

        /// <summary>
        /// 連携ユーザーとして本登録する
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <param name="linkageSystemId"></param>
        /// <param name="userId"></param>
        /// <param name="userPass"></param>
        /// <param name="familyName"></param>
        /// <param name="givenName"></param>
        /// <param name="familyKananame"></param>
        /// <param name="givenKanaName"></param>
        /// <param name="sexType"></param>
        /// <param name="birthDay"></param>
        /// <param name="mailAddress"></param>
        /// <returns></returns>
        public IdentityRegisterApiResults ExecuteLinkageUserRegisterApi(
            string linkageSystemNo, string linkageSystemId, string userId, string userPass,
            string familyName, string givenName, string familyKananame, string givenKanaName,
            string sexType, string birthDay, string mailAddress)
        {

            var apiArgs = new QiQolmsManagementLinkageUserRegisterApiArgs(Guid.Empty, string.Empty)
            {
                FacilityKey = string.Empty,
                LinkageSystemNo = linkageSystemNo,
                RegistUserData = new QmApiLinkageUserItem()
                {
                    Birthday = birthDay,
                    FamilyKanaName = familyKananame,
                    FamilyName = familyName,
                    GivenKanaName = givenKanaName,
                    GivenName = givenName,
                    LinkUserId = linkageSystemId,
                    LoginPass = userPass,
                    LoginUserId = userId,
                    MailAddress = mailAddress,
                    SexType = sexType, // こっちは値のままでOK
                    StatusType = "2"
                }

            };
            var apiResults = QsApiManager.ExecuteQolmsIdentityApi<QiQolmsManagementLinkageUserRegisterApiResults>(apiArgs, DUMMY_SESSION_ID, DUMMY_API_AUTHORIZE_KEY);

            QoAccessLog.WriteInfoLog($"AutoGenerate UserID: {userId}");
            QoAccessLog.WriteInfoLog($"AccountKey: {apiResults.AccountKey}");

            return new IdentityRegisterApiResults
            {
                IsSuccess = apiResults.IsSuccess == bool.TrueString,
                AccountKey = apiResults.AccountKey,
                ErrorList = apiResults.ErrorList
            };
        }

        /// <summary>
        /// 仮登録データを取得する
        /// </summary>
        /// <param name="accountkey">対象仮アカウントキー</param>
        /// <returns></returns>
        public QiQolmsAccountRegisterReadApiResults ExecuteRegisterReadApi(Guid accountkey)
        {
            QiQolmsAccountRegisterReadApiArgs apiArgs = new QiQolmsAccountRegisterReadApiArgs(accountkey.ToString("N"), Guid.Empty, string.Empty);
            QiQolmsAccountRegisterReadApiResults apiResults = QsApiManager.ExecuteQolmsIdentityApi<QiQolmsAccountRegisterReadApiResults>(apiArgs, DUMMY_SESSION_ID, DUMMY_API_AUTHORIZE_KEY);

            if (apiResults.IsSuccess.TryToValueType(false))
                return apiResults;
            else
                throw new InvalidOperationException(string.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsIdentityApiName(apiArgs)));
        }

        /// <summary>
        /// 家族アカウントを追加する
        /// </summary>
        /// <param name="actorKey"></param>
        /// <param name="executorName"></param>
        /// <param name="account"></param>
        /// <param name="sex"></param>
        /// <param name="birthDay"></param>
        /// <returns></returns>
        public QiQolmsAccountConnectFamilyAccountEditWriteApiResults ExecuteAccountConnectFamilyAccountEditWriteApi(Guid actorKey, string executorName, QoApiAccountFamilyInputItem account, string sex, string birthDay)
        {
            var apiArgs = new QiQolmsAccountConnectFamilyAccountEditWriteApiArgs(QiApiTypeEnum.QolmsAccountConnectFamilyAccountEditWrite, QsApiSystemTypeEnum.Qolms, actorKey, executorName)
            {
                FamilyName = account.FamilyName,
                FamilyKanaName = account.FamilyNameKana,
                FamilyRomanName = string.Empty,
                MiddleName = string.Empty,
                MiddleKanaName = string.Empty,
                MiddleRomanName = string.Empty,
                GivenName = account.GivenName,
                GivenKanaName = account.GivenNameKana,
                GivenRomanName = string.Empty,
                SexType = sex,
                Birthday = birthDay,
                PhotoKey = Guid.Empty.ToApiGuidString()
            };

            return QsApiManager.ExecuteQolmsIdentityApi<QiQolmsAccountConnectFamilyAccountEditWriteApiResults>(apiArgs, DUMMY_SESSION_ID, DUMMY_API_AUTHORIZE_KEY);
        }
    }
}