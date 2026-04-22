using MGF.QOLMS.QolmsOpenApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// 心拍見守りアプリの初期ロードデータ取得処理
    /// </summary>
    public class AppHeartMonitorInitWorker
    {
        AppSettingsReadWorker _settingsReadWorker;
        IAccountRepository _accountRepo;
        IAppSettingsRepository _appSettingsRepo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="accountRepository"></param>
        /// <param name="appSettingsRepository"></param>
        public AppHeartMonitorInitWorker(
            IAccountRepository accountRepository,
            IAppSettingsRepository appSettingsRepository)
        {
            _accountRepo = accountRepository;
            _appSettingsRepo = appSettingsRepository;

            _settingsReadWorker = new AppSettingsReadWorker(_appSettingsRepo);
        }

        /// <summary>
        /// 初期ロードデータ取得
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoAppHeartMonitorInitApiResults GetInitData(QoAppHeartMonitorInitApiArgs args)
        {
            var results = new QoAppHeartMonitorInitApiResults
            {
                IsSuccess = bool.FalseString
            };

            // アカウントキーチェック
            if (!args.ActorKey.CheckArgsConvert(nameof(args.ActorKey), Guid.Empty, results, out var accountKey))
            {
                return results;
            }

            // アプリ設定の取得
            if(!TryReadSettings(args,results, out var json))
            {
                return results;
            }

            // アカウント情報の取得
            if(!TryReadAccount(accountKey, results, out var accountEntity))
            {
                return results;
            }

            results.SettingsJson = json;
            results.Birthday = accountEntity.BIRTHDAY.ToApiDateString();
            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);

            return results;
        }

        bool TryReadSettings(QoAppHeartMonitorInitApiArgs args,  QoApiResultsBase results, out string json)
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

                if(settingsResult.IsSuccess == bool.FalseString)
                {
                    results.Result = settingsResult.Result;
                    return false;
                }

                json = settingsResult.SettingsJson;
                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "アプリ設定の取得に失敗しました。");
                return false;
            }
        }

        bool TryReadAccount(Guid accountKey, QoApiResultsBase results, out QH_ACCOUNTINDEX_DAT entity)
        {
            entity = null;
            try
            {
                entity = _accountRepo.ReadAccountIndexDat(accountKey);
                if(entity == null)
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, "アカウントが存在しませんでした。");
                    return false;
                }

                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "アカウント情報の取得に失敗しました。");
                return false;
            }
        }
    }
}