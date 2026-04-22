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
    public class NoteMedicineSyncDataEditWorkerTest
    {
        NoteMedicineSyncDataEditWorker _worker;

        [TestInitialize]
        public void Initialize()
        {
            _worker = new NoteMedicineSyncDataEditWorker(new NoteMedicineRepository());

        }

        [TestMethod]
        public void 更新チェック()
        {
            var args = new QoNoteMedicineSyncDataEditApiArgs
            {
                RecordDate = "2025-05-15T00:00:00.0000000+09:00",
                Sequence = 1,
                ActorKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55").ToApiGuidString(),
                Executor = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55").ToApiGuidString(),
                Data = new QoNoteMedicineDetail
                {
                    DataType = 2,
                    Memo = "test2",
                    OwnerType = 1,
                    PharmacyName = "qolmsdrug2",
                    PrescriptionNo = 1,
                    PharmacyNo = 0,
                    RecordDate = "2025-05-15T00:00:00.0000000+09:00",
                    RpItems = new List<QoRpItem>(),
                    RpSetNo = 1,
                    MedicineItems = new List<QoMedicineItem>
                    {
                        new QoMedicineItem
                        {
                            ItemCode = "4987306055056",
                            ItemCodeType = "J",
                            Name = "ビオフェルミンＶＣ　２個",
                            No = 1,
                        }
                    }
                }
            };

            var ret = _worker.Edit(args);
        }
    }
}
