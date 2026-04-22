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
using System.Collections.Generic;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// Live2DWebアプリのSSO経由ログイン処理と初期データ取得処理
    /// </summary>
    public class AppLive2DInitWorker
    {
        IPasswordManagementRepository _passwordRepo;
        IAccountRepository _accountRepo;
        IIdentityApiRepository _identityApi;
        AppSettingsReadWorker _settingsReadWorker;
        QkRandomAdviceReadWorker _adviceWorker;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="passwordManagementRepository"></param>
        /// <param name="accountRepository"></param>
        /// <param name="appSettingsRepository"></param>
        /// <param name="qkRandomAdviceRepository"></param>
        /// <param name="identityApiRepository"></param>
        public AppLive2DInitWorker(
            IPasswordManagementRepository passwordManagementRepository,
            IAccountRepository accountRepository,
            IAppSettingsRepository appSettingsRepository,
            IQkRandomAdviceRepository qkRandomAdviceRepository,
            IIdentityApiRepository identityApiRepository)
        {
            _passwordRepo = passwordManagementRepository;
            _accountRepo = accountRepository;            
            _identityApi = identityApiRepository;

            _adviceWorker = new QkRandomAdviceReadWorker(qkRandomAdviceRepository);
            _settingsReadWorker = new AppSettingsReadWorker(appSettingsRepository);
        }


        /// <summary>
        /// ログインと初期データ取得
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoAppLive2DInitApiResults LoginAndLoadData(QoAppLive2DInitApiArgs args)
        {
            var results = new QoAppLive2DInitApiResults
            {
                IsSuccess = bool.FalseString
            };

            // アカウントキーを取得
            var accountKey = args.ActorKey.TryToValueType(Guid.Empty);
            if (accountKey == Guid.Empty)
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

            //  アカウント情報取得
            if (!TryGetAccountInfo(accountKey, out var accountInfo, results))
            {
                return results;
            }

            // ログイン実行
            if (!TryLogin(accountInfo.userId, accountInfo.password, results))
            {
                return results;
            }

            // ユーザー情報取得
            if (!TryReadUser(accountKey, results, out var userItem))
            {
                return results;
            }

            // アドバイス情報取得
            if(!TryGetAdviceList(args, results, out var adviceItems))
            {
                return results;
            }

            // アプリ設定の取得
            if (!TryReadSettings(args, results, out var json))
            {
                return results;
            }

            // 権限の設定(一旦AccessKeyとKagamino系のみ)
            var roles = QoApiFunctionTypeEnum.AccessKey | QoApiFunctionTypeEnum.Kagamino;

            // アクセスキーの生成(有効期限60分)
            var accessKey = new QsJwtTokenProvider().CreateOpenApiJwtAccessKey(encExecutor, accountKey, Guid.Empty,(int)roles, DateTime.Now.AddMinutes(60));

            results.AccessKey = accessKey;
            results.User = userItem;
            results.SettingsJson = json;
            results.AdviceList = adviceItems;

            // 利用可能なモデルと背景（機能は未実装）
            // 実装するには別途Live2DのRole管理が必要になる
            results.AvailableModels = new List<string>();
            results.AvailableBackgrounds = new List<string>();

            // 成功
            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);

            return results;
        }

        bool TryGetAccountInfo(Guid accountKey, out (string userId, string password) userInfo, QoApiResultsBase results)
        {
            userInfo = (string.Empty, string.Empty);

            try
            {
                var entity = _passwordRepo.ReadDecryptedEntity(accountKey);
                if (entity == null)
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, "ユーザーが存在しませんでした。");
                    return false;
                }

                userInfo = (entity.USERID, entity.USERPASSWORD);
                return true;
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "ユーザー情報の取得に失敗しました。");
                return false;
            }
        }

        bool TryLogin(string userId, string password, QoApiResultsBase results)
        {
            try
            {
                var loginResults = _identityApi.ExecuteLoginApi(string.Empty, userId, password, string.Empty, false, string.Empty);

                var resultType = loginResults.LoginResultType.TryToValueType(QsApiLoginResultTypeEnum.None);

                switch (resultType)
                {
                    case QsApiLoginResultTypeEnum.Success:
                        return true;                    
                    default:
                        break;
                }

                // Success以外は失敗とみなす
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, "ログインできませんでした。");

                return false;
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "ログイン処理でエラーが発生しました。");
                return false;
            }
        }

        bool TryReadUser(Guid accountKey, QoApiResultsBase results, out QoApiUserItem user)
        {
            user = null;
            try
            {
                var entity = _accountRepo.ReadAccountIndexDat(accountKey);
                if (entity == null)
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, "対象アカウントが存在しません。");
                    return false;
                }

                // 必要な情報のみ設定(LoginIdやMailやPhoneなどはセキュリティ上省く)
                user = new QoApiUserItem
                {
                    AccountKeyReference = accountKey.ToEncrypedReference(),
                    FamilyName = entity.FAMILYNAME,
                    GivenName = entity.GIVENNAME,
                    FamilyNameKana = entity.FAMILYKANANAME,
                    GivenNameKana = entity.GIVENKANANAME,
                    NickName = entity.NICKNAME,
                    Birthday = entity.BIRTHDAY.ToApiDateString(),
                    PersonPhotoReference = entity.PHOTOKEY.ToEncrypedReference(),
                    Sex = entity.SEXTYPE,
                };

                return true;
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "アカウント情報の取得に失敗しました。");
                return false;
            }
        }

        bool TryGetAdviceList(QoApiArgsBase args, QoApiResultsBase results, out List<QkAdviceItem> adviceList)
        {
            adviceList = null;
            try
            {
                var adviceArgs = new QoRandomAdviceListReadApiArgs
                {
                    Executor = args.Executor,
                    ExecutorName = args.ExecutorName,
                    ExecuteSystemType = args.ExecuteSystemType,
                    ActorKey = args.ActorKey,
                    ModelId = "0", // 今は共通固定
                    IsFilterCurrentSeason = true
                };

                var adviceResults = _adviceWorker.ReadList(adviceArgs);

                if(adviceResults.IsSuccess != bool.TrueString)
                {
                    throw new Exception(adviceResults.Result.Detail);
                }

                adviceList = adviceResults.AdviceItems;
                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "アドバイス情報の取得に失敗しました。");
                return false;
            }
        }

        bool TryReadSettings(QoApiArgsBase args, QoApiResultsBase results, out string json)
        {
            json = string.Empty;
            try
            {
                var settingsArgs = new QoAppSettingsReadApiArgs
                {
                    ActorKey = args.ActorKey,
                    ExecuteSystemType = args.ExecuteSystemType,
                    Executor = args.Executor,
                    ExecutorName = args.ExecutorName
                };

                var settingsResult = _settingsReadWorker.Read(settingsArgs);

                if (settingsResult.IsSuccess == bool.FalseString)
                {
                    results.Result = settingsResult.Result;
                    return false;
                }

                json = settingsResult.SettingsJson;
                return true;
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "アプリ設定の取得に失敗しました。");
                return false;
            }
        }
    }
}