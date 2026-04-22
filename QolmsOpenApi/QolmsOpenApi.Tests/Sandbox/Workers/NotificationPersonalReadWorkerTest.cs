using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace QolmsOpenApi.Tests.Sandbox
{
    [TestClass]
    public class NotificationPersonalReadWorkerTest
    {
        NotificationPersonalReadWorker _worker;

        [TestInitialize]
        public void Initialize()
        {
            _worker = new NotificationPersonalReadWorker(new NoticePersonalRepository(), new FamilyRepository());
        }

        [TestMethod]
        public void 互換性テスト()
        {
            var args = new QoNotificationPersonalReadApiArgs
            {
                ExecuteSystemType = "40",
                PageIndex = "0",
                PageSize = "10",
                OnlyUnread = "False",
                CategoryNo = "0",
                FromDate = new DateTime(2025, 5, 26).ToApiDateString(),
                ToDate = new DateTime(2025, 5, 26).ToApiDateString(),
                ActorKey = "6ed1824d-aa9b-4277-9826-459cfb4d0b55"
            };

            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var oldApiResults = NotificationWorker.PersonalRead(args);            
            sw.Stop();
            System.Diagnostics.Debug.WriteLine(sw.Elapsed);

            sw.Reset();
            sw.Start();
            var newApiResults = _worker.Read(args);
            sw.Stop();
            System.Diagnostics.Debug.WriteLine(sw.Elapsed);

            newApiResults.NoticeN.Count.Is(oldApiResults.NoticeN.Count);
            newApiResults.PageIndex.Is(oldApiResults.PageIndex);
            newApiResults.MaxPageIndex.Is(oldApiResults.MaxPageIndex);
        }
    }
}
