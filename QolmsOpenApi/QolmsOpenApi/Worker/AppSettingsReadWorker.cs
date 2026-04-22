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
using MGF.QOLMS.QolmsOpenApi.Enums;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// アプリ設定Read処理
    /// </summary>
    public class AppSettingsReadWorker
    {
        IAppSettingsRepository _appSettingsRepo;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appSettingsRepository"></param>
        public AppSettingsReadWorker(IAppSettingsRepository appSettingsRepository)
        {
            _appSettingsRepo = appSettingsRepository;
        }

        /// <summary>
        /// Read処理
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoAppSettingsReadApiResults Read( QoAppSettingsReadApiArgs args)
        {
            var results = new QoAppSettingsReadApiResults
            {
                IsSuccess = bool.FalseString
            };

            // アカウントキーチェック
            if(!args.ActorKey.CheckArgsConvert(nameof(args.ActorKey),Guid.Empty,results, out var accountKey))
            {
                return results;
            }
                        
            // アプリ種別の取得とチェック
            var appType = args.ExecuteSystemType.ToApplicationType();
            if(appType == QsDbApplicationTypeEnum.None)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "アプリ種別が不正です。");
                return results;
            }

            // アプリ設定の取得
            if(!TryReadSettings(accountKey, appType, results, out var json))
            {
                return results;
            }

            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            results.IsSuccess = bool.TrueString;

            results.SettingsJson = json;

            return results;
        }

        bool TryReadSettings(Guid accountKey, QsDbApplicationTypeEnum appType, QoApiResultsBase results, out string settingsValue)
        {
            settingsValue = GetDefaultSettings(appType);

            try
            {
                var entity = _appSettingsRepo.ReadEntity(accountKey, (int)appType);

                if (entity == null)
                {
                    return true;
                }

                settingsValue = entity.VALUE;
                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "アプリ設定取得処理でエラーが発生しました。");
                return false;
            }
        }

        string GetDefaultSettings(QsDbApplicationTypeEnum appType)
        {
            var serializer = new QsJsonSerializer();
            switch (appType)
            {
                case QsDbApplicationTypeEnum.HeartMonitorApp:
                   return  serializer.Serialize(new QhSettingsHeartMonitorOfJson());

                default:
                    return "{}";
            }
        }
    }
}