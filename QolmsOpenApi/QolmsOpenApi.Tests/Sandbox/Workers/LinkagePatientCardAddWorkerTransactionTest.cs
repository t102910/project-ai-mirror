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
    public class LinkagePatientCardAddWorkerTransactionTest
    {
        LinkagePatientCardAddWorker _worker;
        Mock<ILinkageRepository> _linkageRepo;

        [TestInitialize]
        public void Initialize()
        {
            _linkageRepo = new Mock<ILinkageRepository>();

            // LinkageRepositoryをMockにすることでエラーを操作する
            _worker = new LinkagePatientCardAddWorker(_linkageRepo.Object, new AccountRepository(), new FamilyRepository(), new PatientCardRepository(), new StorageRepository());
        }

        [TestMethod]
        public void 家族作成後にエラーが発生した場合にロールバックされる()
        {
            var accountKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");

            var args = new QoLinkagePatientCardAddApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "Hospa",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.TisiOSApp}",
                LinkageSystemNo = QoLinkage.TIS_LINKAGE_SYSTEM_NO.ToString(),
                Birthday = new DateTime(2000, 1, 1).ToApiDateString(),
                FacilityKeyReference = "13d00930275c2ab0fd3d896b90eba8ea6faed98f5bb636ab977fbe6fcc2d6999cc84275f29f099dd466d3f554a4a8aec",
                FamilyName = "患者",
                GivenName = "テスト",
                FamilyKanaName = "カンジャ",
                GivenKanaName = "タロウ",
                NickName = "カン",
                SexType = "1",
                LinkUserId = "88880099",
                WithFamilyAccountRegistration = bool.TrueString
            };

            _linkageRepo.Setup(m => m.IsAvailableCard(It.IsAny<Guid>(), "88880099")).Returns(true);

            _linkageRepo.Setup(m => m.GetParentLinkageMst(It.IsAny<Guid>())).Throws(new Exception());

            var results = _worker.Add(args);
        }
    }
}
