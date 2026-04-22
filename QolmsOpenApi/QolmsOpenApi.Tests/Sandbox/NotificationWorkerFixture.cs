using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Models;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MGF.QOLMS.QolmsOpenApi.Repositories;

namespace QolmsOpenApi.Tests.Sandbox
{
    [TestClass]
    public class NotificationWorkerFixture
    {
        [TestInitialize]
        public void Initialize()
        {

        }

        [TestMethod]
        public void お知らせ取得の統合テスト()
        {
            var accountKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");
            var now = DateTime.Now;
            var args = new QoNotificationReadApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.TisiOSApp}",
                FromDate = now.ToApiDateString(),
                ToDate = now.ToApiDateString(),
                FacilityKeyReference = "13d00930275c2ab0fd3d896b90eba8ea6faed98f5bb636ab977fbe6fcc2d6999cc84275f29f099dd466d3f554a4a8aec",
                PageIndex = "0",
                PageSize = "5"
            };

            var worker = new NotificationReadWorker(new NoticeRepository(), new LinkageRepository());

            var ret = worker.Read(args);
        }

        [TestMethod]
        public void 個人お知らせリスト取得()
        {
            var accountKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");
            var now = DateTime.Now;
            var args = new QoNotificationPersonalReadApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.TisiOSApp}",
                FromDate = now.ToApiDateString(),
                ToDate = now.ToApiDateString(),
                OnlyUnread = bool.FalseString,
                PageIndex = "0",
                PageSize = "10",                
            };

            var result = NotificationWorker.PersonalRead(args);
        }

        [TestMethod]
        public void 個人お知らせリスト取得_カテゴリ指定()
        {
            var accountKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");
            var now = DateTime.Now;
            var args = new QoNotificationPersonalReadApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.TisiOSApp}",
                FromDate = now.ToApiDateString(),
                ToDate = now.ToApiDateString(),
                OnlyUnread = bool.FalseString,
                PageIndex = "0",
                PageSize = "10",
                CategoryNo = "11"
            };

            var result = NotificationWorker.PersonalRead(args);
        }
    }
}
