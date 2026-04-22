using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Models;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class ExternalServiceLinkageWorkerFixture
    {
        Mock<ILinkageRepository> _linkageRepo;
        Mock<IExternalServiceLinkageRepository> _externalRepo;
        
        ExternalServiceWriteWorker _worker;

        [TestInitialize]
        public void Initialize()
        {
            _linkageRepo = new Mock<ILinkageRepository>();
            _externalRepo = new Mock<IExternalServiceLinkageRepository>();
            _worker = new ExternalServiceWriteWorker(_linkageRepo.Object, _externalRepo.Object);
        }

        [TestMethod]
        public void 実行者情報不正時は失敗()
        {
            var args = new QoExternalServiceWriteApiArgs
            {
                ActorKey = Guid.Empty.ToApiGuidString()
            };

            var ret = _worker.Write(args);

            // 失敗
            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Code.Is("1002");
            ret.Result.Detail.Contains("実行者が不正です。").IsTrue();
        }

        [TestMethod]
        public void 施設キー不正時は失敗()
        {
            var actorKey = "E53CAD72-0DE6-48A9-93F4-250A5D8906B9";

            var args = new QoExternalServiceWriteApiArgs
            {
                ActorKey = actorKey,
                FacilityKey = Guid.Empty.ToApiGuidString()
            };

            var ret = _worker.Write(args);

            // 失敗
            ret.IsSuccess.Is(bool.FalseString);
            ret.Result.Code.Is("1002");
            ret.Result.Detail.Contains("対象施設が指定されていません。").IsTrue();
        }

        [TestMethod]
        public void 連携情報リストがNull時は失敗()
        {
            var actorKey = "E53CAD72-0DE6-48A9-93F4-250A5D8906B9";
            var facilityKey = "28157A1F-C54B-48E6-95B4-0F170A0B9662";

            var args = new QoExternalServiceWriteApiArgs
            {
                ActorKey = actorKey,
                FacilityKey = facilityKey,
                ExternalLinkageN = null
            };

            var ret = _worker.Write(args);

            // 失敗
            ret.IsSuccess.Is(bool.FalseString);
        }

        [TestMethod]
        public void 連携情報リストが空の時はデータ無しとして成功()
        {
            var actorKey = "E53CAD72-0DE6-48A9-93F4-250A5D8906B9";
            var facilityKey = "28157A1F-C54B-48E6-95B4-0F170A0B9662";
            var externalLinkageN = new List<QoApiExternalServiceListItem>();

            var args = new QoExternalServiceWriteApiArgs
            {
                ActorKey = actorKey,
                FacilityKey = facilityKey,
                ExternalLinkageN = externalLinkageN
            };

            var entities = new List<QH_EXTERNALSERVICELINKAGE_DAT>();

            _externalRepo.Setup(m => m.WriteList(entities)).Returns((true, null));

            var ret = _worker.Write(args);

            // 成功
            ret.IsSuccess.Is(bool.TrueString);
        }

        [TestMethod]
        public void 書き込みに失敗したらエラーメッセージ付きで返却()
        {
            var actorKey = "E53CAD72-0DE6-48A9-93F4-250A5D8906B9";
            var facilityKey = "28157a1f-c54b-48e6-95b4-0f170a0b9662";
            var facilityGuid = Guid.Parse(facilityKey);
            var externalLinkageN = new List<QoApiExternalServiceListItem>();
            var apiItem = new QoApiExternalServiceListItem
            {
                ExternalServiceType = 1,
                TargetServiceType = 1,
                LinkageSystemId = "99999001",
                valueJson = "{\"FacilityList\":[{\"FacilityKey\":\"28157a1f-c54b-48e6-95b4-0f170a0b9662\",\"LinkageStatus\":1,\"MedicalFacilityCode\":\"9999910006\"}],\"IsTermsAccepted\":true}"
            };
            externalLinkageN.Add(apiItem);

            var args = new QoExternalServiceWriteApiArgs
            {
                ActorKey = actorKey,
                FacilityKey = facilityKey,
                ExternalLinkageN = externalLinkageN
            };

            var retAccountKey = Guid.Parse("e53cad72-0de6-48a9-93f4-250a5d8906b9");
            var errorList = new List<string>();
            errorList.Add("hogehoge");
            var retObj = (false, errorList);

            _linkageRepo.Setup(m => m.GetAccountKey(apiItem.LinkageSystemId, facilityGuid)).Returns(retAccountKey);
            _externalRepo.Setup(m => m.WriteList(It.IsAny<List<QH_EXTERNALSERVICELINKAGE_DAT>>())).Returns(retObj);

            var ret = _worker.Write(args);

            ret.IsSuccess.Is(bool.FalseString);
            ret.ErrorMessageN.IsStructuralEqual(errorList);
        }

        [TestMethod]
        public void 正常終了()
        {
            var actorKey = "E53CAD72-0DE6-48A9-93F4-250A5D8906B9";
            var facilityKey = "28157a1f-c54b-48e6-95b4-0f170a0b9662";
            var facilityGuid = Guid.Parse(facilityKey);
            var externalLinkageN = new List<QoApiExternalServiceListItem>();
            var apiItem = new QoApiExternalServiceListItem
            {
                ExternalServiceType = 1,
                TargetServiceType = 1,
                LinkageSystemId = "99999001",
                valueJson = "{\"FacilityList\":[{\"FacilityKey\":\"28157a1f-c54b-48e6-95b4-0f170a0b9662\",\"LinkageStatus\":1,\"MedicalFacilityCode\":\"9999910006\"}],\"IsTermsAccepted\":true}"
            };
            externalLinkageN.Add(apiItem);

            var args = new QoExternalServiceWriteApiArgs
            {
                ActorKey = actorKey,
                FacilityKey = facilityKey,
                ExternalLinkageN = externalLinkageN
            };

            var retAccountKey = Guid.Parse("e53cad72-0de6-48a9-93f4-250a5d8906b9");

            _linkageRepo.Setup(m => m.GetAccountKey(apiItem.LinkageSystemId, facilityGuid)).Returns(retAccountKey);
            _externalRepo.Setup(m => m.WriteList(It.IsAny<List<QH_EXTERNALSERVICELINKAGE_DAT>>())).Returns((true, null));

            var ret = _worker.Write(args);

            ret.IsSuccess.Is(bool.TrueString);
        }

    }
}
