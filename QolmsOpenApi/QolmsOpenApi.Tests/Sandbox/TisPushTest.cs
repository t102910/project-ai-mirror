using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Models;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QolmsOpenApi.Tests.Sandbox
{
    [TestClass]
    public class TisPushTest
    {
        QoPushNotification _notification;

        [TestInitialize]
        public void Initialize()
        {
            //_notification = new QoPushNotification(QoApiConfiguration.QolmsNaviNotificationHubConnectionString, QoApiConfiguration.QolmsNaviNotificationHubName);
            _notification = new QoPushNotification(QoApiConfiguration.MEINaviNotificationHubConnectionString, QoApiConfiguration.MEINaviNotificationHubName);
        }

        [TestMethod]
        public async Task シンプルな通知()
        {
            var request = new NotificationRequest
            {               
                Text = "test",
                Url = "heart-plus://home",
                Title = "test",                     
                Silent = true
            };

            var userId = "db64dd62aca847e2b593a7e6cb40ee05"; // HOSPA
            //var userId = "697dbcdeb63a4747a158ad803a8fb3ff"; // 医療ナビ
            var tags = new List<string>
            {
                "Waiting", "$UserId:{" + userId.ToEncrypedReference() + "}" 
            };
            request.SetTagExpressionJoinAllAnd(tags.ToArray());

            var result = await _notification.RequestNotificationAsync(request);
       }

        [TestMethod]
        public async Task Androidへの通知()
        {
            var toSystemType = QsDbApplicationTypeEnum.TisAndroidApp;
            // 未来を指定するとスケジュールPushとなる。今より前だと即時Push
            var pushDate = DateTime.Now.AddMinutes(5);
            var facilityKey = Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb");

            var request = new NotificationRequest
            {
                Extra = "",
                Silent = false,
                Text = "android test",
                Url = string.Format("{0}info?no={1}", toSystemType.ToAppUrlScheme(), 1111),
                Badge = 1,
                ScheduleDate = pushDate,
                Title = "android test"
            };

            request.SetTagExpressionJoinAllAnd(new string[] { "Information", string.Format("Facility_{0}", facilityKey.ToEncrypedReference()) });

            var result = await _notification.RequestAndroidNotificationAsync(request);
            //var result = await _notification.RequestNotificationAsync(request);
        }

        [TestMethod]
        public async Task 待ち受け系の通知()
        {
            var payload = new WaitingNotificationPayload
            {
                AccountKeyReference = "b26c23768571b70c161bc6b228b03a8b652a12ce146e64258c7c1641533375289688748d7bf484084b7cc6d9ea2005e9",
                CreatedAt = DateTime.UtcNow,
                EventType = WaitingEventType.ExaminationReady,
                FacilityKeyReference = "24be2e12624c8e357a8228c21fe972cdcaf4b5d5f77c2290d4646138ab23991789576ba2a41cc6abb83d534a353f986d",
                ReceiptNumber = "0714",
                ReservationNo = "999"
            };

            var tags = new List<string>
            {
                "Waiting",
                CreateUserIdTag("7e70b6babfaf493cb1f349c7c0c2995b")
            };

            await SendPush(payload, tags, "dsfsdf", "", false);
        }

        async Task SendPush(WaitingNotificationPayload payload, List<string> tags, string message = "", string url = "", bool isSilent = false)
        {
            var payloadJson = CreatePayloadJson(payload);            

            var request = new NotificationRequest
            {
                Extra = payloadJson,
                Silent = isSilent,
                Text = message,
                Url = url,
                Badge = 1,
                Title = "HOGE",
            };
            request.SetTagExpressionJoinAllAnd(tags.ToArray());

            string[] result = await _notification.RequestNotificationAsync(request);
            if (string.IsNullOrEmpty(result[0]) && string.IsNullOrEmpty(result[1]))
                QoAccessLog.WriteErrorLog(string.Format("順番待ちPush送信失敗:{0}", payload), Guid.Empty);
        }

        static string CreateUserIdTag(string userId)
        {
            return "$UserId:{" + userId.ToEncrypedReference() + "}";
        }

        static string CreatePayloadJson(WaitingNotificationPayload payload)
        {
            var result = string.Empty;
            try
            {
                result = new QsJsonSerializer().Serialize(payload);
            }
            catch (Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, Guid.Empty);
            }
            return result;
        }

        [TestMethod]
        public async Task 待ち受け通知()
        {
            // 連携ユーザーID
            var userId = "47cb1c00856b4e1cb3c9614cfb99e9f8";
            var tags = new List<string>
            {
                "Waiting", "$UserId:{" + userId.ToEncrypedReference() + "}"
            };

            // 会計終了を表す通知ペイロード
            var payload = TisWorker.CreateWaitingNotificationPayload(
                MGF.QOLMS.QolmsApiEntityV1.TisWaitingEventType.AccountingEnd,
                new Guid("6ed1824d-aa9b-4277-9826-459cfb4d0b55"),
                new Guid("11dc3f56-5652-4d08-9147-1575c1723edb"),
                "300",
                "01"
                );

            //await TisWorker.SendPush(payload, tags, string.Empty, true);
        }

        [TestMethod]
        public void 患者IDからアカウントキーとユーザーIDの逆引き()
        {           
            (var accountKey, var userId) = TisWorker.ChangePatientIdToAccount(27004, "99990071");

            var encoded = userId.ToEncrypedReference();
        }
    }
}
