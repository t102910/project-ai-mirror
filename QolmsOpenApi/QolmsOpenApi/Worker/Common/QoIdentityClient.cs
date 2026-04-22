using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsCryptV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MGF.QOLMS.QolmsJwtAuthCore;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// Identity API 呼び出しの処理
    /// </summary>
    public sealed class QoIdentityClient
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

        #region "Public Method"
        /// <summary>
        /// QolmsLoginApiの呼び出しを実行します。
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <param name="passwordHash"></param>
        /// <param name="useTwoFactorAuthentication"></param>
        /// <param name="twoFactorAuthenticationToken"></param>
        /// <returns></returns>
        public static QiQolmsLoginApiResults ExecuteQolmsLoginApi(string sessionId, string userId, string password, string passwordHash, bool useTwoFactorAuthentication, string twoFactorAuthenticationToken
)
        {
            QiQolmsLoginApiArgs apiArgs = new QiQolmsLoginApiArgs(QiApiTypeEnum.QolmsLogin, QsApiSystemTypeEnum.QolmsOpenApi, Guid.Empty, string.Empty
        )
            {
                SessionId = string.IsNullOrWhiteSpace(sessionId)? DUMMY_SESSION_ID : sessionId,
                UserId = userId.Trim(),
                Password = password.Trim(),
                PasswordHash = passwordHash.Trim(),
                UseTwoFactorAuthentication = useTwoFactorAuthentication.ToString(),
                TwoFactorAuthenticationToken = twoFactorAuthenticationToken.Trim()
            };
            QiQolmsLoginApiResults apiResults = QsApiManager.ExecuteQolmsIdentityApi<QiQolmsLoginApiResults>(apiArgs, DUMMY_SESSION_ID, DUMMY_API_AUTHORIZE_KEY  );

             if (apiResults.IsSuccess.TryToValueType(false))
                    return apiResults;
                else
                    throw new InvalidOperationException(string.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsIdentityApiName(apiArgs)));
            
        }
    
        /// <summary>
        /// QolmsAccountSignUpReadApiの呼び出しを実行します。
        /// </summary>
        /// <param name="mailAddress"></param>
        /// <returns></returns>
        public static QiQolmsAccountSignUpReadApiResults ExecuteSignUpReadApi(string mailAddress)
        {
            QiQolmsAccountSignUpReadApiArgs apiArgs = new QiQolmsAccountSignUpReadApiArgs(mailAddress, Guid.Empty, string.Empty);
            QiQolmsAccountSignUpReadApiResults apiResults = QsApiManager.ExecuteQolmsIdentityApi<QiQolmsAccountSignUpReadApiResults>(apiArgs, DUMMY_SESSION_ID, DUMMY_API_AUTHORIZE_KEY );
                if (apiResults.IsSuccess.TryToValueType(false))
                    return apiResults;
                else
                    throw new InvalidOperationException(string.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsIdentityApiName(apiArgs)));
        }

        /// <summary>
        /// QolmsAccountSignUpWriteApiの呼び出しを実行します。
        /// </summary>
        /// <param name="mailAddress"></param>
        /// <returns></returns>
        public static QiQolmsAccountSignUpWriteApiResults ExecuteSignUpWriteApi(string mailAddress)
        {
            QiQolmsAccountSignUpWriteApiArgs apiArgs = new QiQolmsAccountSignUpWriteApiArgs(mailAddress, Guid.Empty, string.Empty);
            QiQolmsAccountSignUpWriteApiResults apiResults = QsApiManager.ExecuteQolmsIdentityApi<QiQolmsAccountSignUpWriteApiResults>(apiArgs, DUMMY_SESSION_ID, DUMMY_API_AUTHORIZE_KEY );
            if (apiResults.IsSuccess.TryToValueType(false))
                    return apiResults;
                else
                    throw new InvalidOperationException(string.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsIdentityApiName(apiArgs)));
        }

        /// <summary>
        /// QolmsAccountRegisterReadApiの呼び出しを実行します。
        /// </summary>
        /// <param name="accountkey"></param>
        /// <returns></returns>
        public static QiQolmsAccountRegisterReadApiResults ExecuteRegisterReadApi(Guid accountkey)
        {
            QiQolmsAccountRegisterReadApiArgs apiArgs = new QiQolmsAccountRegisterReadApiArgs(accountkey.ToString("N"), Guid.Empty, string.Empty);
            QiQolmsAccountRegisterReadApiResults apiResults = QsApiManager.ExecuteQolmsIdentityApi<QiQolmsAccountRegisterReadApiResults>(apiArgs, DUMMY_SESSION_ID, DUMMY_API_AUTHORIZE_KEY );

            if (apiResults.IsSuccess.TryToValueType(false))
                return apiResults;
            else
                throw new InvalidOperationException(string.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsIdentityApiName(apiArgs)));
        }
        /// <summary>
        /// QolmsAccountRegisterWriteApiの呼び出しを実行します。
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static QiQolmsAccountRegisterWriteApiResults ExecuteRegisterWriteApi(QoApiAccountRegisterItem model)
        {
            DateTime birthday = DateTime.Parse(model.Birthday);

            QiQolmsAccountRegisterWriteApiArgs apiArgs = new QiQolmsAccountRegisterWriteApiArgs(Guid.Empty, string.Empty)
            {
                Accountkey = model.AccountKeyReference.ToDecrypedReference<Guid>().ToString("N"),
                FamilyName = model.FamilyName,
                GivenName = model.GivenName,
                FamilyKanaName = model.FamilyNameKana,
                GivenKanaName = model.GivenNameKana,
                Sex = model.Sex.ToValueType<QsDbSexTypeEnum>().ToString(),
                BirthYear = birthday.Year.ToString(),
                BirthMonth = birthday.Month.ToString(),
                BirthDay = birthday.Day.ToString(),
                MailAddress = model.Mail,
                UserId = model.UserId,
                Password = model.Password,
                AccountType = "1",
                PrivateAccountFlag = bool.FalseString
            };
            QiQolmsAccountRegisterWriteApiResults apiResults = QsApiManager.ExecuteQolmsIdentityApi<QiQolmsAccountRegisterWriteApiResults>(apiArgs, DUMMY_SESSION_ID, DUMMY_API_AUTHORIZE_KEY );

            if (apiResults.IsSuccess.TryToValueType(false))
                return apiResults;
            else
                throw new InvalidOperationException(string.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsIdentityApiName(apiArgs)));
        }

        /// <summary>
        /// QiQolmsManagementLinkageUserRegisterApiの呼び出しを実行します。
        /// </summary>
        /// <param name="linkageSytemNo"></param>
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
        public static QiQolmsManagementLinkageUserRegisterApiResults ExecuteLinkageUserRegisterApi(
            string linkageSytemNo, string linkageSystemId, string userId, string userPass, 
            string familyName, string givenName, string familyKananame, string givenKanaName, 
            string sexType, string birthDay, string mailAddress)
        {
            var apiArgs = new QiQolmsManagementLinkageUserRegisterApiArgs(Guid.Empty, string.Empty)
            {
                FacilityKey =string.Empty ,
                LinkageSystemNo= linkageSytemNo,
                RegistUserData =new QmApiLinkageUserItem()
                {
                    Birthday=birthDay,
                    FamilyKanaName =familyKananame,
                    FamilyName=familyName,
                    GivenKanaName=givenKanaName,
                    GivenName=givenName,
                    LinkUserId = linkageSystemId,
                    LoginPass =userPass,
                    LoginUserId =userId,
                    MailAddress = mailAddress,
                    SexType = sexType,
                    StatusType="2"
                }
                
            };
            var apiResults = QsApiManager.ExecuteQolmsIdentityApi<QiQolmsManagementLinkageUserRegisterApiResults>(apiArgs, DUMMY_SESSION_ID, DUMMY_API_AUTHORIZE_KEY);

            if (apiResults!=null)
                return apiResults;
            else
                throw new InvalidOperationException(string.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsIdentityApiName(apiArgs)));
        }
        /// <summary>
        /// AccountConnectFamilyAccountEditWriteApi の呼び出しを実行します。 
        /// </summary>
        /// <param name="actorKey"></param>
        /// <param name="executorName"></param>
        /// <param name="account"></param>
        /// <param name="sex"></param>
        /// <param name="birthDay"></param>
        /// <returns></returns>
        public static QiQolmsAccountConnectFamilyAccountEditWriteApiResults ExecuteAccountConnectFamilyAccountEditWriteApi(Guid actorKey, string executorName, QoApiAccountFamilyInputItem account, string sex, string birthDay)
        {
            var apiArgs = new QiQolmsAccountConnectFamilyAccountEditWriteApiArgs(QiApiTypeEnum.QolmsAccountConnectFamilyAccountEditWrite, QsApiSystemTypeEnum.Qolms, actorKey,executorName)
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
                Birthday = birthDay ,
                PhotoKey = Guid.Empty.ToApiGuidString()
            };

            return QsApiManager.ExecuteQolmsIdentityApi<QiQolmsAccountConnectFamilyAccountEditWriteApiResults>(apiArgs, DUMMY_SESSION_ID, DUMMY_API_AUTHORIZE_KEY  );
            
        }

        /// <summary>
        /// ExecuteAccountConnectWriteApiの呼び出しを実行します。
        /// </summary>
        /// <param name="parentAccountKey"></param>
        /// <param name="childAccountKey"></param>
        /// <param name="executorName"></param>
        /// <returns></returns>
        public static QiQolmsAccountConnectWriteApiResults ExecuteAccountConnectWriteApi(Guid parentAccountKey,Guid childAccountKey, string executorName)
        {
            byte changeType = 2; // 完全削除

            QiQolmsAccountConnectWriteApiArgs apiArgs = new QiQolmsAccountConnectWriteApiArgs(QiApiTypeEnum.QolmsAccountConnectWrite, QsApiSystemTypeEnum.QolmsOpenApi, parentAccountKey, executorName )
            {
                ActorKey = childAccountKey.ToApiGuidString(),
                ChangeType = changeType.ToString()
            };

            QiQolmsAccountConnectWriteApiResults apiResults = QsApiManager.ExecuteQolmsIdentityApi<QiQolmsAccountConnectWriteApiResults>(apiArgs,  DUMMY_SESSION_ID, DUMMY_API_AUTHORIZE_KEY );

            if (apiResults.IsSuccess.TryToValueType(false))
                return apiResults;
            else
                throw new InvalidOperationException(string.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsIdentityApiName(apiArgs)));

        }
        #endregion

    }
}