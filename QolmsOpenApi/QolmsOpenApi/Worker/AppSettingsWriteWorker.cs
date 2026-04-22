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
using Newtonsoft.Json;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// アプリ設定Write処理
    /// </summary>
    public class AppSettingsWriteWorker
    {
        IAppSettingsRepository _appSettingsRepo;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appSettingsRepository"></param>
        public AppSettingsWriteWorker(IAppSettingsRepository appSettingsRepository)
        {
            _appSettingsRepo = appSettingsRepository;
        }

        /// <summary>
        /// Write処理
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoAppSettingsWriteApiResults Write(QoAppSettingsWriteApiArgs args)
        {
            var results = new QoAppSettingsWriteApiResults
            {
                IsSuccess = bool.FalseString
            };

            // アカウントキーチェック
            if (!args.ActorKey.CheckArgsConvert(nameof(args.ActorKey), Guid.Empty, results, out var accountKey))
            {
                return results;
            }

            // アプリ種別の取得とチェック
            var appType = args.ExecuteSystemType.ToApplicationType();
            if (appType == QsDbApplicationTypeEnum.None)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "アプリ種別が不正です。");
                return results;
            }

            // 設定値の妥当性チェック
            if(!CheckAppSettings(args.SettingsJson, appType, results))
            {
                return results;
            }

            // 設定値の書き込みを実行
            if(!TryWriteSettings(accountKey, appType, args.SettingsJson, results))
            {
                return results;
            }

            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            results.IsSuccess = bool.TrueString;

            return results;
        }

        // 規定の設定クラスに復元可能かをチェックする
        bool CheckAppSettings(string json, QsDbApplicationTypeEnum appType, QoApiResultsBase results)
        {
            bool isJsonValid;
            switch (appType)
            {
                // 心拍見守りアプリ
                case QsDbApplicationTypeEnum.HeartMonitorApp:
                    isJsonValid = IsJsonValid<QhSettingsHeartMonitorOfJson>(json);
                    break;
                // Live2DWebアプリ
                case QsDbApplicationTypeEnum.QolmsLive2DWeb:
                    isJsonValid = IsJsonValid<QhSettingsLive2DOfJson>(json);
                    break;

                // 設定対応アプリが増えたら以下に追加していく
                default:
                    isJsonValid = false;
                    break;
            }

            if (!isJsonValid)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "設定値が不正です。");
                return false;
            }

            return true;
        }

        bool IsJsonValid<T>(string json) where T: class
        {
            try
            {
                var ret = new QsJsonSerializer().Deserialize<T>(json);
                return ret != null;
            }
            catch
            {
                return false;
            }
        }

        bool TryWriteSettings(Guid accoutKey, QsDbApplicationTypeEnum appType, string json, QoApiResultsBase results)
        {
            try
            {
                var entity = _appSettingsRepo.ReadEntity(accoutKey, (int)appType);

                if(entity == null)
                {
                    entity = new QH_APPSETTINGS_DAT
                    {
                        ACCOUNTKEY = accoutKey,
                        APPTYPE = (int)appType,
                        VALUE = json
                    };

                    _appSettingsRepo.InsertEntity(entity);
                }
                else
                {
                    entity.VALUE = json;
                    _appSettingsRepo.UpdateEntity(entity);
                }                               

                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "アプリ設定書き込み処理でエラーが発生しました。");
                return false;
            }
        }
    }
}