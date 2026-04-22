using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsJwtAuthCore;
using MGF.QOLMS.QolmsOpenApi.Enums;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// SMSログイン2段階目処理
    /// </summary>
    public class AccountLoginForSms2Worker
    {
        IPasswordManagementRepository _passwordRepo;
        ISmsAuthCodeRepository _smsAuthCodeRepository;
        ILinkageRepository _linkageRepo;
        IIdentityApiRepository _identityApi;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="passwordManagementRepository"></param>
        /// <param name="smsAuthCodeRepository"></param>
        /// <param name="linkageRepository"></param>
        /// <param name="identityApiRepository"></param>
        public AccountLoginForSms2Worker(
            IPasswordManagementRepository passwordManagementRepository,
            ISmsAuthCodeRepository smsAuthCodeRepository,
            ILinkageRepository linkageRepository,
            IIdentityApiRepository identityApiRepository)
        {
            _passwordRepo = passwordManagementRepository;
            _smsAuthCodeRepository = smsAuthCodeRepository;
            _linkageRepo = linkageRepository;
            _identityApi = identityApiRepository;
        }

        /// <summary>
        /// SMSログイン処理2段階目実行
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoAccountLoginForSms2ApiResults LoginForSms2(QoAccountLoginForSms2ApiArgs args)
        {
            var results = new QoAccountLoginForSms2ApiResults
            {
                IsSuccess = bool.FalseString,
                LoginResultType = QsApiLoginResultTypeEnum.None.ToString()
            };

            // アカウントキーを取得
            var accountKey = args.ActorKey.TryToValueType(Guid.Empty);
            if(accountKey == Guid.Empty)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(args.ActorKey)}が不正です。");
                return results;
            }

            // Executorチェック
            if (!args.Executor.CheckArgsConvert(nameof(args.Executor), Guid.Empty, results, out var executor))
            {
                return results;
            }

            // Executorを暗号化
            var encExecutor = executor.ToString("N").TryEncrypt();            

            // 認証キーのデコード
            var authKey = args.AuthKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);
            if (authKey == Guid.Empty)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(args.AuthKeyReference)}が不正です。");
                return results;
            }

            // 認証コード照合
            if (!CheckAuthCode(authKey, args.AuthCode, results))
            {
                return results;
            }

            // ユーザー情報取得
            if(!TryGetUserInfo(accountKey,out var userInfo, results))
            {
                return results;
            }

            // ログイン実行
            if(!TryLogin(userInfo.userId, userInfo.password,out var loginResultType, results))
            {
                results.LoginResultType = loginResultType;
                return results;
            }

            // 通知用ID取得
            if(!TryGetNotificationId(accountKey,args.ExecuteSystemType,out var notificationId, results))
            {
                return results;
            }

            // API認証用のトークン設定
            results.Token = new QsJwtTokenProvider().CreateOpenApiJwtAuthenticateKey(encExecutor, accountKey);
            results.LinkageIdReference = notificationId;
            results.LoginResultType = loginResultType;      

            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);

            return results;
        }

        bool CheckAuthCode(Guid authKey, string authCode, QoApiResultsBase results)
        {
            try
            {
                var entity = _smsAuthCodeRepository.ReadEntity(authKey);
                var now = DateTime.Now;
                if (entity.EXPIRES < now)
                {
                    // 期限切れ
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.SmsAuthCodeExpired, "認証コードの期限が切れています。");
                    return false;
                }

                if (entity.FAILURECOUNT >= 2)
                {
                    // 試行回数オーバー
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.SmsAuthCodeCountOver, "認証コードが一定回数間違えたため無効となっています。");
                    return false;
                }

                if (entity.AUTHCODE != authCode)
                {
                    // 認証コード不一致
                    // 失敗回数カウントアップ＆更新
                    entity.FAILURECOUNT++;
                    _smsAuthCodeRepository.UpdateEntity(entity);
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.SmsAuthCodeInvalid, "認証コードが一致しませんでした。");
                    return false;
                }

                return true;

            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "認証コード照合処理に失敗しました。");
                return false;
            }
        }

        bool TryGetUserInfo(Guid accountKey,out (string userId, string password) userInfo, QoApiResultsBase results)
        {
            userInfo = (string.Empty, string.Empty);

            try
            {
                var entity = _passwordRepo.ReadDecryptedEntity(accountKey);
                if(entity == null)
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, "ユーザーが存在しませんでした。");
                    return false;
                }

                userInfo = (entity.USERID, entity.USERPASSWORD);
                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "ユーザー情報の取得に失敗しました。");
                return false;
            }
        }

        bool TryGetNotificationId(Guid accountKey, string systemType,out string id, QoApiResultsBase results)
        {
            id = string.Empty;

            // SystemTypeを文字列からEnumに変換
            var systemTypeEnum = systemType.TryToValueType(QsApiSystemTypeEnum.None);
            // 対応するLinkageSystemNoを取得
            var linkageSystemNo = systemTypeEnum.ToLinkageSystemNo();

            if (!linkageSystemNo.IsNaviApp())
            {
                // 医療ナビ系(HOSPA/医療ナビ)以外はアカウントキー参照をPush通知IDとする
                id = accountKey.ToEncrypedReference();
                return true;
            }           

            try
            {
                var entity = _linkageRepo.ReadEntity(accountKey, linkageSystemNo);
                if(entity == null)
                {
                    // 連携情報が存在しない場合は一応成功とみなす。要検討。
                    // アプリ等にこれが無かったとしても通知が届かないだけで動作はするはず。
                    return true;
                }

                id = entity.LINKAGESYSTEMID.ToEncrypedReference();
                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "通知用IDの取得に失敗しました。");
                return false;
            }
        }

        bool TryLogin(string userId, string password, out string loginResultType, QoApiResultsBase results)
        {
            try
            {
                var loginResults = _identityApi.ExecuteLoginApi(string.Empty, userId, password, string.Empty, false, string.Empty);

                loginResultType = loginResults.LoginResultType;
                var resultType = loginResults.LoginResultType.TryToValueType(QsApiLoginResultTypeEnum.None);

                switch (resultType)
                {
                    case QsApiLoginResultTypeEnum.Success:                        
                        return true;
                    case QsApiLoginResultTypeEnum.Lockdown:
                        //ロックダウン中はロックダウンという情報を返さないようにする
                        loginResultType = QsApiLoginResultTypeEnum.Retry.ToString();
                        break;
                    case QsApiLoginResultTypeEnum.Retry:                       
                    case QsApiLoginResultTypeEnum.TwoFactorRetry:
                        // リトライ中
                    case QsApiLoginResultTypeEnum.TwoFactorTimeout:
                        // 二段階認証タイムアウト
                    case QsApiLoginResultTypeEnum.TwoFactorRequire:                        
                        //二要素認証が必要                        
                        break;
                    default:
                        break;
                }

                // Success以外は失敗とみなす
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, "ログインできませんでした。");
                
                return false;
            }
            catch(Exception ex)
            {
                loginResultType = QsApiLoginResultTypeEnum.None.ToString();
                results.Result = QoApiResult.Build(ex, "ログイン処理でエラーが発生しました。");
                return false;
            }
        }
    }
}