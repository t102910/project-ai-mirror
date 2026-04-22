using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class MasterFacilityLanguageWorkerFixture
    {
        private MasterFacilityLanguageReadWorker _worker;
        private Mock<IFacilityRepository> _repo;

        /// <summary>
        /// DB に登録済の施設キー
        /// </summary>
        private readonly Guid _registeredFacilityKey = Guid.Parse("11DC3F56-5652-4D08-9147-1575C1723EDB");

        /// <summary>
        /// DB に登録されていない施設キー
        /// </summary>
        private readonly Guid _unregisteredFacilityKey = Guid.Parse("11111111-4c60-4f64-ba98-4d4cf7966ded");

        [TestInitialize]
        public void Initialize()
        {
            _repo = new Mock<IFacilityRepository>();
            _worker = new MasterFacilityLanguageReadWorker(_repo.Object);
        }

        [TestMethod]
        public void 登録済の施設キーで施設言語リソースを取得できる()
        {
            var args = new QoMasterFacilityLanguageReadApiArgs
            {
                FacilityCodeReference = _registeredFacilityKey.ToEncrypedReference()
            };

            var entities = new List<QH_FACILITYLANGUAGE_MST>
            {
                new QH_FACILITYLANGUAGE_MST
                {
                    FACILITYKEY = _registeredFacilityKey,
                    LANGUAGEKEY = "Accounting_Ready_Please",
                    VALUE = @"[{""Language"": 0, ""Value"": ""自動精算機にお越しください""},{""Language"": 1, ""Value"": ""Please go to the automatic payment machine.""}]",
                    DELETEFLAG = false,
                    CREATEDDATE = new DateTime(2024, 1, 1, 11, 11, 11),
                    UPDATEDDATE = new DateTime(2024, 1, 1, 11, 11, 11)
                },
                new QH_FACILITYLANGUAGE_MST
                {
                    FACILITYKEY = _registeredFacilityKey,
                    LANGUAGEKEY = "Medicine_Ready_Please",
                    VALUE = @"[{""Language"": 0, ""Value"": ""受け取りにお越しください""},{""Language"": 1, ""Value"": ""Please come to pick it up at the pharmacy.""}]",
                    DELETEFLAG = false,
                    CREATEDDATE = new DateTime(2024, 1, 1, 11, 11, 11),
                    UPDATEDDATE = new DateTime(2024, 1, 1, 11, 11, 11)
                }
            };

            _repo.Setup(x => x.ReadFacilityLanguage(_registeredFacilityKey)).Returns(entities);

            var results = _worker.Read(args);

            results.IsNotNull();
            results.IsSuccess.Is(bool.TrueString);
            results.Result.IsNotNull();
            results.Result.Code.Is("0200");
            results.LangResourceItems.IsNotNull();
            results.LangResourceItems.Count.Is(2);
            results.LangResourceItems[0].Key.Is("Accounting_Ready_Please");
            results.LangResourceItems[0].ResourceJson.Is(@"[{""Language"": 0, ""Value"": ""自動精算機にお越しください""},{""Language"": 1, ""Value"": ""Please go to the automatic payment machine.""}]");
            results.LangResourceItems[1].Key.Is("Medicine_Ready_Please");
            results.LangResourceItems[1].ResourceJson.Is(@"[{""Language"": 0, ""Value"": ""受け取りにお越しください""},{""Language"": 1, ""Value"": ""Please come to pick it up at the pharmacy.""}]");
        }

        [TestMethod]
        public void 登録されていない施設キーでデフォルトの施設言語リソースを取得()
        {
            var args = new QoMasterFacilityLanguageReadApiArgs
            {
                FacilityCodeReference = _unregisteredFacilityKey.ToEncrypedReference()
            };

            var entities = new List<QH_FACILITYLANGUAGE_MST>
            {
                new QH_FACILITYLANGUAGE_MST
                {
                    // 施設キーが空の場合はデフォルトの言語リソースとして扱う
                    FACILITYKEY = Guid.Empty,
                    LANGUAGEKEY = "Accounting_Ready_Please",
                    VALUE = @"[{""Language"": 0, ""Value"": ""自動精算機にお越しください""},{""Language"": 1, ""Value"": ""Please go to the automatic payment machine.""}]",
                    DELETEFLAG = false,
                    CREATEDDATE = new DateTime(2024, 1, 1, 11, 11, 11),
                    UPDATEDDATE = new DateTime(2024, 1, 1, 11, 11, 11)
                },
                new QH_FACILITYLANGUAGE_MST
                {
                    FACILITYKEY = Guid.Empty,
                    LANGUAGEKEY = "Medicine_Ready_Please",
                    VALUE = @"[{""Language"": 0, ""Value"": ""受け取りにお越しください""},{""Language"": 1, ""Value"": ""Please come to pick it up at the pharmacy.""}]",
                    DELETEFLAG = false,
                    CREATEDDATE = new DateTime(2024, 1, 1, 11, 11, 11),
                    UPDATEDDATE = new DateTime(2024, 1, 1, 11, 11, 11)
                }
            };

            _repo.Setup(x => x.ReadFacilityLanguage(_unregisteredFacilityKey)).Returns(entities);

            var results = _worker.Read(args);

            results.IsNotNull();
            results.IsSuccess.Is(bool.TrueString);
            results.Result.IsNotNull();
            results.Result.Code.Is("0200");
            results.LangResourceItems.IsNotNull();
            results.LangResourceItems.Count.Is(2);
            results.LangResourceItems[0].Key.Is("Accounting_Ready_Please");
            results.LangResourceItems[0].ResourceJson.Is(@"[{""Language"": 0, ""Value"": ""自動精算機にお越しください""},{""Language"": 1, ""Value"": ""Please go to the automatic payment machine.""}]");
            results.LangResourceItems[1].Key.Is("Medicine_Ready_Please");
            results.LangResourceItems[1].ResourceJson.Is(@"[{""Language"": 0, ""Value"": ""受け取りにお越しください""},{""Language"": 1, ""Value"": ""Please come to pick it up at the pharmacy.""}]");
        }

        [TestMethod]
        public void 施設言語リソース取得時にDB例外が発生した場合はエラー結果を返す()
        {
            var args = new QoMasterFacilityLanguageReadApiArgs
            {
                FacilityCodeReference = _registeredFacilityKey.ToEncrypedReference()
            };

            _repo.Setup(x => x.ReadFacilityLanguage(_registeredFacilityKey)).Throws(new Exception("DB からの施設言語リソース取得に失敗しました。"));

            var results = _worker.Read(args);

            results.IsNotNull();
            results.IsSuccess.Is(bool.FalseString);
            results.Result.IsNotNull();
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("DB からの施設言語リソース取得に失敗しました。").IsTrue();
        }
    }
}
