using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Models;
using System;
using System.Threading.Tasks;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// PushNotificationのインストール情報削除処理
    /// </summary>
    public class AppPushNotificationEntryDeleteWorker
    {
        IQoPushNotification _pushNotification;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="pushNotification">Push通知インターフェース</param>
        public AppPushNotificationEntryDeleteWorker(IQoPushNotification pushNotification)
        {
            _pushNotification = pushNotification;
        }

        /// <summary>
        /// PushNotificationのインストール情報を削除します。
        /// </summary>
        /// <param name="args">削除引数。</param>
        /// <returns>削除結果。</returns>
        public async Task<QoAppPushNotificationEntryDeleteApiResults> DeleteAsync(QoAppPushNotificationEntryDeleteApiArgs args)
        {
            var results = new QoAppPushNotificationEntryDeleteApiResults
            {
                IsSuccess = bool.FalseString
            };

            // アカウントキーチェック
            if (!args.ActorKey.CheckArgsConvert(nameof(args.ActorKey), Guid.Empty, results, out var accountKey))
            {
                return results;
            }

            // DeviceId 必須チェック
            if (string.IsNullOrWhiteSpace(args.DeviceId))
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(args.DeviceId)}が指定されていません。");
                return results;
            }

            // DeviceId 最大長チェック
            if (args.DeviceId.Length > 100)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(args.DeviceId)}が不正です。");
                return results;
            }

            // ExecuteSystemType から連携システム番号を取得
            var linkageSystemNo = args.ExecuteSystemType.ToLinkageSystemNo();

            // NotificationHub 設定を取得
            var settings = linkageSystemNo.ToNotificationHubSettings();
            if (settings == null)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, $"{nameof(args.ExecuteSystemType)}からNotificationHub設定を取得できませんでした。");
                return results;
            }

            // Push インストール情報を削除
            _pushNotification.Initialize(settings);
            var isDeleted = await _pushNotification.DeleteInstallationAsync(args.DeviceId).ConfigureAwait(false);

            if (!isDeleted)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, "デバイス情報の削除に失敗しました。");
                return results;
            }

            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            results.IsSuccess = bool.TrueString;

            return results;
        }
    }
}
