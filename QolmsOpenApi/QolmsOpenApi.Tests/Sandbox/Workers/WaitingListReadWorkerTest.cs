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
    public class WaitingListReadWorkerTest
    {
        WaitingListReadWorker _worker;

        [TestInitialize]
        public void Initialize()
        {
            _worker = new WaitingListReadWorker(new WaitingRepository(), new LinkageRepository());
        }

        [TestMethod]
        public void 順番待ち取得()
        {
            var facility = Guid.Parse("480c61d2-36f9-4f30-846a-54cb1cdc3188").ToEncrypedReference();

            var args = new QoWaitingListReadApiArgs
            {
                ActorKey = Guid.Parse("9715cab8-8e80-4692-9348-a2635a923b36").ToApiGuidString(),
                Executor = Guid.Parse("9715cab8-8e80-4692-9348-a2635a923b36").ToApiGuidString(),
                ExecuteSystemType = "40",
                IsAll = bool.FalseString,
                FacilityKeyReference = facility,
                LinkageSystemId = "99900401",
                TargetDate = DateTime.Today.ToApiDateString(),
                DataType = "0"
            };

            var ret = _worker.Read(args);
        }
    }

}
