using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QolmsOpenApi.Tests.Sandbox.Repositories
{
    [TestClass]
    public class ExternalServiceFixture
    {
        IExternalServiceLinkageRepository _repo;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new ExternalServiceLinkageRepository();
        }

        [TestMethod]
        public void 外部サービス書き込みAPIを実行する()
        {

            Guid accountKey = Guid.Parse("e53cad72-0de6-48a9-93f4-250a5d8906b9");

            QH_EXTERNALSERVICELINKAGE_DAT target = new QH_EXTERNALSERVICELINKAGE_DAT()
            {
                ACCOUNTKEY = accountKey,
                EXTERNALSERVICETYPE = 1,
                TARGETSERVICETYPE = 1,
                DELETEFLAG = false,
                CREATEDDATE = DateTime.Now,
                UPDATEDDATE = DateTime.Now,
            };

            List<QH_EXTERNALSERVICELINKAGE_DAT> entity = new List<QH_EXTERNALSERVICELINKAGE_DAT>();
            entity.Add(target);

            var ret = _repo.WriteList(entity);
        }

        [TestMethod]
        public void 外部サービス情報を追加更新()
        {
            var accountKey = Guid.Parse("9cc4df23-8efc-4358-b348-815766f69939");
            var json = new QhExternalServiceLinkageOfJson
            {
                FacilityList = new List<QhExternalServiceFacilityItem>(),
                IsTermsAccepted = true
            };

            var serializeJson = new QsJsonSerializer().Serialize(json);

            var entity = new QH_EXTERNALSERVICELINKAGE_DAT
            {
                ACCOUNTKEY = accountKey,
                EXTERNALSERVICETYPE = 1,
                TARGETSERVICETYPE = (int)QsDbApplicationTypeEnum.QolmsTisApp,
                VALUE = serializeJson,
                DELETEFLAG = false,
            };

            _repo.UpsertEntity(entity);
        }

        [TestMethod]
        public void 外部サービス情報の読み取り()
        {
            var accountKey = Guid.Parse("9cc4df23-8efc-4358-b348-815766f69939");

            var entity = _repo.ReadEntity(accountKey, 1, 133);
        }
    }
}
