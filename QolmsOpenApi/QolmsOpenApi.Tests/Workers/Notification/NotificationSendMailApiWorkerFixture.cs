using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Worker;
using MGF.QOLMS.QolmsOpenApi.Worker.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class NotificationSendMailApiWorkerFixture
    {
        NotificationSendMailApiWorker _worker;

        [TestInitialize]
        public void Initialize()
        {
            _worker = new NotificationSendMailApiWorker();
        }

        [TestMethod]
        public void 送信対象情報が0件()
        {
            var args = new QoNotificationSendMailApiArgs();

            QoNotificationSendMailApiResults results = _worker.NotificationSendMail(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("送信対象情報が0件です").IsTrue();
        }

        [TestMethod]
        public void メール送信非対応()
        {
            var args = new QoNotificationSendMailApiArgs(QoApiTypeEnum.NotificationSendMail, QsApiSystemTypeEnum.None, Guid.NewGuid(), "executorName")
            {
                SendMailN = new List<QoSendMailItem>
                {
                    new QoSendMailItem()
                    {
                        Mail = "user1@mgfactory.co.jp",
                        Title = "タイトル",
                        Message = "メッセージ",
                    }
                },
            };

            QoNotificationSendMailApiResults results = _worker.NotificationSendMail(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("メール送信非対応です").IsTrue();
        }

        //[TestMethod]
        public void メール送信()
        {
            var args = new QoNotificationSendMailApiArgs(QoApiTypeEnum.NotificationSendMail, QsApiSystemTypeEnum.HeartMonitorApp, Guid.NewGuid(), "executorName")
            {
                SendMailN = new List<QoSendMailItem>
                {
                    new QoSendMailItem()
                    {
                        Mail = "user1@mgfactory.co.jp",
                        Title = "タイトル1",
                        Message = "メッセージ1",
                    },
                    new QoSendMailItem()
                    {
                        Mail = "user2@mgfactory.co.jp",
                        Title = "タイトル2",
                        Message = "メッセージ2",
                    },
                    new QoSendMailItem()
                    {
                        Mail = "user3@mgfactory.co.jp",
                        Title = "タイトル3",
                        Message = "メッセージ3",
                    },
                },
            };

            QoNotificationSendMailApiResults results = _worker.NotificationSendMail(args);

            results.IsSuccess.Is(bool.FalseString);
        }
    }
}
