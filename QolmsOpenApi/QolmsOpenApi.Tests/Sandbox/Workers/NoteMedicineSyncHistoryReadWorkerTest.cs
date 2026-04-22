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
    public class NoteMedicineSyncHistoryReadWorkerTest
    {
        NoteMedicineSyncHistoryReadWorker _worker;

        [TestInitialize]
        public void Initialize()
        {
            _worker = new NoteMedicineSyncHistoryReadWorker(new NoteMedicineRepository());
        }

        [TestMethod]
        public void 特定のレコードでクラッシュする調査()
        {
            var args = new QoNoteMedicineSyncHistoryReadApiArgs
            {
                Days = 180,
                StartDate = new DateTime(2025, 5, 15).ToApiDateString(),
                ActorKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55").ToApiGuidString()
            };

            var ret = _worker.Read(args);
        }
    }
}
