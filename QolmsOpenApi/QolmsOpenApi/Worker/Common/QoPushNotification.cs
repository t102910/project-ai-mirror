using MGF.QOLMS.QolmsOpenApi.Models;
using Microsoft.Azure.NotificationHubs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// プッシュ通知インターフェース
    /// </summary>
    public interface IQoPushNotification
    {
        /// <summary>
        /// NotificationHubを初期化する
        /// </summary>
        /// <param name="hubConnectionString"></param>
        /// <param name="hubName"></param>
        void Initialize(string hubConnectionString, string hubName);
        /// <summary>
        /// NotificationHubを初期化する
        /// </summary>
        /// <param name="settings"></param>
        void Initialize(NotificationHubsSettings settings);

        /// <summary>
        /// プッシュ通知のリクエストを行います
        /// </summary>
        /// <param name="notificationRequest"></param>
        /// <returns></returns>
        Task<string[]> RequestNotificationAsync(NotificationRequest notificationRequest);

        /// <summary>
        /// 指定されたInstallationIdの登録情報を削除します。
        /// </summary>
        /// <param name="installationId"></param>
        /// <returns></returns>
        Task<bool> DeleteInstallationAsync(string installationId);
    }

    /// <summary>
    /// プッシュ通知（Azure Notification Hubs利用）
    /// </summary>
    public class QoPushNotification: IQoPushNotification
    {

        private NotificationHubClient _hub { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public QoPushNotification()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hubConnectionString"></param>
        /// <param name="hubName"></param>
        public QoPushNotification(string hubConnectionString, string hubName)
        {
            Initialize(hubConnectionString, hubName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        public QoPushNotification(NotificationHubsSettings settings)
        {
            Initialize(settings);
        }

        /// <summary>
        /// NotificationHubを初期化する。
        /// コンストラクタで設定済みであれば不要
        /// </summary>
        /// <param name="hubConnectionString"></param>
        /// <param name="hubName"></param>
        public void Initialize(string hubConnectionString, string hubName)
        {
            _hub = NotificationHubClient.CreateClientFromConnectionString(hubConnectionString, hubName);
        }

        /// <summary>
        /// NotificationHubを初期化する。
        /// コンストラクタで設定済みであれば不要
        /// </summary>
        /// <param name="settings"></param>
        public void Initialize(NotificationHubsSettings settings)
        {
            _hub = NotificationHubClient.CreateClientFromConnectionString(settings.HubConnectionString, settings.HubName);
        }

        /// <summary>
        /// 登録されているデバイスを取得します。
        /// </summary>
        /// <returns></returns>
        public async Task<List<Installation>> GetAllRegisteredDevicesAsync()
        {

            var allRegistrations = await _hub.GetAllRegistrationsAsync(0);
            var continuationToken = allRegistrations.ContinuationToken;
            var registrationDescriptionsList = new List<RegistrationDescription>(allRegistrations);
            while (!string.IsNullOrWhiteSpace(continuationToken))
            {
                var otherRegistrations = await _hub.GetAllRegistrationsAsync(continuationToken, 0);
                registrationDescriptionsList.AddRange(otherRegistrations);
                continuationToken = otherRegistrations.ContinuationToken;
            }

            // Put into DeviceInstallation object
            var deviceInstallationList = new List<Installation>();
            int cnt = 0;
            foreach (var registration in registrationDescriptionsList)
            {
                var deviceInstallation = new Installation();
                cnt++;
                var tags = registration.Tags;
                foreach (var tag in tags)
                {
                    if (tag.Contains("InstallationId:"))
                    {
                        deviceInstallation.InstallationId = tag.Substring(tag.IndexOf(":") + 2).TrimEnd('}');
                    }
                    if (tag.Contains("UserId:"))
                    {
                        deviceInstallation.UserId = tag.Substring(tag.IndexOf(":") + 2).TrimEnd('}');
                    }
                }
                deviceInstallation.Platform = registration is AppleRegistrationDescription ? NotificationPlatform.Apns : NotificationPlatform.Fcm;
                deviceInstallation.PushChannel = registration.PnsHandle;
                deviceInstallation.Tags = new List<string>(registration.Tags);

                deviceInstallationList.Add(deviceInstallation);
            }
            return deviceInstallationList;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteInstallationAsync(string installationId)
        {
            try
            {
                await _hub.DeleteInstallationAsync(installationId);
                return true;
            }
            catch(Exception ex)
            {
                QoAccessLog.WriteAccessLog(QolmsApiCoreV1.QsApiSystemTypeEnum.QolmsOpenApi, Guid.Empty, DateTime.Now, QoAccessLog.AccessTypeEnum.Api, "QoPushNotification.DeleteInstallation",
                                ex.Message, null, null, null);
                return false;
            }
        }

        /// <summary>
        /// Push通知をリクエストします。
        /// </summary>
        /// <param name="notificationRequest"></param>
        /// <returns></returns>
        public async Task<string[]> RequestNotificationAsync(NotificationRequest notificationRequest)
        {
            //QoAccessLog.WriteInfoLog($"In RequestNotificationAsync {notificationRequest.Extra} {notificationRequest.TagExpression}");
            var androidPushTemplate = notificationRequest.Silent ? PushTemplates.Silent.AndroidFcmV1 : PushTemplates.Generic.AndroidFcmV1;
            var iOSPushTemplate = notificationRequest.Silent ? PushTemplates.Silent.iOS : PushTemplates.Generic.iOS;

            var androidPayload = PrepareAndroidNotificationPayload( androidPushTemplate,
                notificationRequest.Text, notificationRequest.Extra,
                notificationRequest.Url, notificationRequest.Badge, notificationRequest.Title);

            var iOSPayload = PrepareNotificationPayload( iOSPushTemplate,
                notificationRequest.Text, notificationRequest.Extra,
                notificationRequest.Url, notificationRequest.Badge, notificationRequest.Title);
            string[] result=new string[] { string.Empty, string.Empty };
            try
            {
                if (notificationRequest.ScheduleDate > DateTime.Now)
                {
                    ScheduledNotification[] res =await SendSchedulePlatformNotificationsAsync(androidPayload, iOSPayload, notificationRequest.ScheduleDate , notificationRequest.TagExpression, notificationRequest.Silent).ConfigureAwait(false);
                    result[0] =res[0]!=null? res[0].ScheduledNotificationId : string.Empty;
                    result[1] =res[0]!=null? res[1].ScheduledNotificationId : string.Empty;
                }
                else 
                {
                    NotificationOutcome[] res= await SendPlatformNotificationsAsync(androidPayload, iOSPayload, notificationRequest.TagExpression, notificationRequest.Silent).ConfigureAwait(false);
                    result[0] = res[0] != null ? res[0].NotificationId : string.Empty;
                    result[1] = res[1] != null ? res[1].NotificationId : string.Empty;

                    if (res[0] != null)
                    {
                        if (res[0].State != NotificationOutcomeState.Enqueued && res[0].State != NotificationOutcomeState.Processing && res[0].State != NotificationOutcomeState.Completed)
                        {
                            QoAccessLog.WriteAccessLog(QolmsApiCoreV1.QsApiSystemTypeEnum.QolmsOpenApi, Guid.Empty, DateTime.Now, QoAccessLog.AccessTypeEnum.Api, "AndroidSendPush_Error",
                                string.Format("Android即時通知失敗 State:{0} / NotificationId:{1} / TrackingId:{2} / TagExpression:{3}", res[0].State, res[0].NotificationId, res[0].TrackingId, notificationRequest.TagExpression), null, null, null);
                        }
                        //else
                        //{
                        //    QoAccessLog.WriteAccessLog(QolmsApiCoreV1.QsApiSystemTypeEnum.QolmsOpenApi, Guid.Empty, DateTime.Now, QoAccessLog.AccessTypeEnum.Api, "AndroidSendPush_Success",
                        //        string.Format("Android即時通知成功 State:{0} / NotificationId:{1} / TrackingId:{2} / TagExpression:{3}", res[0].State, res[0].NotificationId, res[0].TrackingId, notificationRequest.TagExpression), null, null, null);
                        //}
                    }
                    if (res[1] != null)
                    {
                        if (res[1].State != NotificationOutcomeState.Enqueued && res[1].State != NotificationOutcomeState.Processing && res[1].State != NotificationOutcomeState.Completed)
                        {
                            QoAccessLog.WriteAccessLog(QolmsApiCoreV1.QsApiSystemTypeEnum.QolmsOpenApi, Guid.Empty, DateTime.Now, QoAccessLog.AccessTypeEnum.Api, "iOSSendPush_Error",
                                string.Format("iOS即時通知失敗 State:{0} / NotificationId:{1} / TrackingId:{2} / TagExpression:{3}", res[1].State, res[1].NotificationId, res[1].TrackingId, notificationRequest.TagExpression), null, null, null);
                        }
                        //else
                        //{
                        //    QoAccessLog.WriteAccessLog(QolmsApiCoreV1.QsApiSystemTypeEnum.QolmsOpenApi, Guid.Empty, DateTime.Now, QoAccessLog.AccessTypeEnum.Api, "iOSSendPush_Success",
                        //        string.Format("iOS即時通知成功 State:{0} / NotificationId:{1} / TrackingId:{2} / TagExpression:{3}", res[1].State, res[1].NotificationId, res[1].TrackingId, notificationRequest.TagExpression), null, null, null);
                        //}
                    }
                }

                //QoAccessLog.WriteInfoLog($"In RequestNotificationAsync Result {result[0]} {result[1]}");
                return result;
            }
            catch (Exception ex)
            {
                //QoAccessLog.WriteInfoLog($"In RequestNotificationAsync Error {ex.Message}");
                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
                return result;
            }
        }

        /// <summary>
        /// iOSのみのPush通知をリクエストします。
        /// </summary>
        /// <param name="notificationRequest"></param>
        /// <returns></returns>
        public async Task<string> RequestiOsNotificationAsync(NotificationRequest notificationRequest)
        {
            var iOSPushTemplate = notificationRequest.Silent ? PushTemplates.Silent.iOS : PushTemplates.Generic.iOS;

            var iOSPayload = PrepareNotificationPayload(iOSPushTemplate,
                notificationRequest.Text, notificationRequest.Extra,
                notificationRequest.Url, notificationRequest.Badge, notificationRequest.Title);
            
            try
            {
                var schedule = notificationRequest.ScheduleDate;
                if (notificationRequest.ScheduleDate > DateTime.Now)
                {
                   ScheduledNotification sRes;
                   if (string.IsNullOrWhiteSpace(notificationRequest.TagExpression))
                        sRes = await _hub.ScheduleNotificationAsync(CreateAppleNotification(iOSPayload, notificationRequest.Silent), schedule.ToUniversalTime()).ConfigureAwait(false);
                    else
                        sRes = await _hub.ScheduleNotificationAsync(CreateAppleNotification(iOSPayload, notificationRequest.Silent), schedule.ToUniversalTime(), notificationRequest.TagExpression).ConfigureAwait(false);
 
                    return sRes!=null ?  sRes.ScheduledNotificationId : string.Empty;
                }
                else
                {
                    NotificationOutcome nRes;
                    if (string.IsNullOrWhiteSpace(notificationRequest.TagExpression))
                        nRes = await _hub.SendNotificationAsync(CreateAppleNotification(iOSPayload, notificationRequest.Silent)).ConfigureAwait(false);
                    else
                        nRes = await _hub.SendNotificationAsync(CreateAppleNotification(iOSPayload, notificationRequest.Silent), notificationRequest.TagExpression).ConfigureAwait(false);
             
                    return  nRes!=null ? nRes.NotificationId : string.Empty;
                }

            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
            }
            return string.Empty;
        }
        /// <summary>
        /// AndroidのみのPush通知をリクエストします。
        /// </summary>
        /// <param name="notificationRequest"></param>
        /// <returns></returns>
        public async Task<string> RequestAndroidNotificationAsync(NotificationRequest notificationRequest)
        {
            var androidPushTemplate = notificationRequest.Silent ? PushTemplates.Silent.AndroidFcmV1 : PushTemplates.Generic.AndroidFcmV1;
            var androidPayload = PrepareAndroidNotificationPayload(androidPushTemplate,
                notificationRequest.Text, notificationRequest.Extra,
                notificationRequest.Url, notificationRequest.Badge, notificationRequest.Title);
            try
            {
                if (notificationRequest.ScheduleDate > DateTime.Now)
                {
                    var schedule = notificationRequest.ScheduleDate;
                    ScheduledNotification sRes;
                    if (string.IsNullOrWhiteSpace(notificationRequest.TagExpression))
                        sRes = await _hub.ScheduleNotificationAsync(new FcmV1Notification(androidPayload), schedule.ToUniversalTime()).ConfigureAwait(false);
                    else
                        sRes = await _hub.ScheduleNotificationAsync(new FcmV1Notification(androidPayload), schedule.ToUniversalTime(), notificationRequest.TagExpression).ConfigureAwait(false);
                    return sRes != null ? sRes.ScheduledNotificationId : string.Empty;
                }
                else
                {
                    NotificationOutcome nRes;
                    if (string.IsNullOrWhiteSpace(notificationRequest.TagExpression))
                        nRes = await _hub.SendFcmV1NativeNotificationAsync(androidPayload).ConfigureAwait(false);
                    else
                        nRes = await _hub.SendFcmV1NativeNotificationAsync(androidPayload, notificationRequest.TagExpression).ConfigureAwait(false);
                    return nRes != null ? nRes.NotificationId : string.Empty;
                }
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
            }
            return string.Empty;
        }
        /// <summary>
        /// スケジュールプッシュをキャンセルする
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        public async Task RequestScheduleChancelAsync(string[] notificationId)
        {
            foreach (var item in notificationId)
            {
                await _hub.CancelNotificationAsync(item).ConfigureAwait(false);
            }
            return;
        }

        #region "Private Method"
        private string PrepareNotificationPayload(string template, string text, string extra, string url, int badge, string title)
        {
            // リクエスト事に一意のIDを設定する
            // 似た内容を連続送信した際に間引かれるのを防ぐ対策(効果の程は不明)
            var id = Guid.NewGuid().ToString("N");

            string payload;
            if (string.IsNullOrEmpty(extra))
                payload = template.Replace("$(alertMessage)", text.Replace("\r\n", "\\n")).Replace("$(extra)", "\"\"").Replace("$(url)", url).Replace("$(title)", title).Replace("$(id)", id);
            else
                payload = template.Replace("$(alertMessage)", text.Replace("\r\n", "\\n")).Replace("$(extra)", extra).Replace("$(url)", url).Replace("$(title)", title).Replace("$(id)", id);
            if (badge >= 0)
            {
                payload = payload.Replace("$(badge)", $", \"badge\" : {badge}");
            }
            else
            {
                payload = payload.Replace("$(badge)", "");
            }

            return payload;
        }

        private string PrepareAndroidNotificationPayload(string template, string text, string extra, string url, int badge, string title)
        {
            // リクエスト事に一意のIDを設定する
            // 似た内容を連続送信した際に間引かれるのを防ぐ対策(効果の程は不明)
            var id = Guid.NewGuid().ToString("N");

            string payload;
            if (string.IsNullOrEmpty(extra))
            {
                payload = template.Replace("$(alertMessage)", text.Replace("\r\n", "\\n")).Replace("$(extra)", "").Replace("$(url)", url).Replace("$(title)", title).Replace("$(id)", id);
            }
            else
            {
                // Fcm V1 から dataはネストできなくなったのでbase64でセットする
                var base64extra = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(extra));
                payload = template.Replace("$(alertMessage)", text.Replace("\r\n", "\\n")).Replace("$(extra)", base64extra).Replace("$(url)", url).Replace("$(title)", title).Replace("$(id)", id);
            }

            if (badge >= 0)
            {
                payload = payload.Replace("$(badge)", $", \"badge\" : {badge}");
            }
            else
            {
                payload = payload.Replace("$(badge)", "");
            }

            return payload;
        }


        private Task<NotificationOutcome[]> SendPlatformNotificationsAsync(string androidPayload, string iOSPayload, string tags = "", bool isSilent = false)
        {
            if (string.IsNullOrWhiteSpace(tags))
            {
                //QoAccessLog.WriteInfoLog($"In SendPlatformNotificationsAsync {androidPayload} {tags}");
                var sendTasks = new Task<NotificationOutcome>[]
                {
                _hub.SendFcmV1NativeNotificationAsync(androidPayload),
                _hub.SendNotificationAsync(CreateAppleNotification(iOSPayload, isSilent))
                };

                return Task.WhenAll(sendTasks);
            }
            else
            {
                //QoAccessLog.WriteInfoLog($"In SendPlatformNotificationsAsync HasTags {androidPayload}");
                var sendTasks = new Task<NotificationOutcome>[]
                {
                    _hub.SendFcmV1NativeNotificationAsync(androidPayload, tags),
                    _hub.SendNotificationAsync(CreateAppleNotification(iOSPayload, isSilent), tags)
                };
                return Task.WhenAll(sendTasks);
            }


        }
        private Task<ScheduledNotification[]> SendSchedulePlatformNotificationsAsync(string androidPayload, string iOSPayload, DateTime schedule, string tags = "", bool isSilent = false)
        {
            if (string.IsNullOrWhiteSpace(tags))
            {
                var sendTasks = new Task<ScheduledNotification>[]
                {
                    _hub.ScheduleNotificationAsync(new FcmV1Notification(androidPayload), schedule.ToUniversalTime() ),
                    _hub.ScheduleNotificationAsync(CreateAppleNotification(iOSPayload,isSilent), schedule.ToUniversalTime())
                };

                return Task.WhenAll(sendTasks);
            }
            else
            {
                var sendTasks = new Task<ScheduledNotification>[]
                {
                    _hub.ScheduleNotificationAsync(new FcmV1Notification(androidPayload), schedule.ToUniversalTime(),tags ),
                    _hub.ScheduleNotificationAsync(CreateAppleNotification(iOSPayload,isSilent), schedule.ToUniversalTime(),tags)
                };

                return Task.WhenAll(sendTasks);
            }
        }
        // iOS13からのHeader仕様対応
        private AppleNotification CreateAppleNotification(string iOSPayload, bool isSilent)
        {
            var appleHeaders = new Dictionary<string, string>
            {
                {"apns-push-type", isSilent ? "background" : "alert" },
                {"apns-priority", isSilent ? "5" : "10" }
            };

            return new AppleNotification(iOSPayload, appleHeaders);
        }

        #endregion
    }
}