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
    public class NoteMedicineSyncUpdteReadWorkerTest
    {
        NoteMedicineSyncUpdateReadWorker _worker;

        [TestInitialize]
        public void Initialize()
        {
            _worker = new NoteMedicineSyncUpdateReadWorker(new NoteMedicineRepository());
        }

        [TestMethod]
        public void 更新データが取得できない調査()
        {
            var args = new QoNoteMedicineSyncUpdateReadApiArgs
            {
                SyncStartDate = new DateTime(2025, 4, 22, 10, 0, 0).ToApiDateString(),
                ActorKey = Guid.Parse("3f250dc3-6d51-4c22-ad34-bc7125d1416b").ToApiGuidString()
            };

            var ret = _worker.Read(args);
        }
    }
}
