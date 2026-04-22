using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Models;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class PaymentSmapaTermsWriteWorkerFixture
    {
        Mock<IExternalServiceLinkageRepository> _externalRepo;
        PaymentSmapaTermsWriteWorker _worker;
        Guid _accountKey = Guid.NewGuid();

        [TestInitialize]
        public void Initialize()
        {
            _externalRepo = new Mock<IExternalServiceLinkageRepository>();
            _worker = new PaymentSmapaTermsWriteWorker(_externalRepo.Object);
        }

        [TestMethod]
        public void アカウントキーが不正でエラーとなる()
        {
            var args = GetValidArgs();
            args.ActorKey = "invalidGuid";

            var results = _worker.Write(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.ActorKey)).IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();
        }

        [TestMethod]
        public void 外部サービス連携情報の取得処理で例外が発生するとエラー()
        {
            var args = GetValidArgs();

            // 例外発生
            _externalRepo.Setup(m => m.ReadEntity(_accountKey, 1, args.TargetServiceType)).Throws(new Exception());

            var results = _worker.Write(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("外部サービス連携情報の取得に失敗しました").IsTrue();
        }

        [TestMethod]
        public void 外部サービス連携情報のVALUE値に異常値があればエラーとする()
        {
            var args = GetValidArgs();

            // VALUE値に異常値を含むEntityを返す
            _externalRepo.Setup(m => m.ReadEntity(_accountKey, 1, args.TargetServiceType)).Returns(new QH_EXTERNALSERVICELINKAGE_DAT
            {
                ACCOUNTKEY = _accountKey,
                EXTERNALSERVICETYPE = 1,
                TARGETSERVICETYPE = args.TargetServiceType,
                VALUE = "hogefuga" // 異常値
            });

            var results = _worker.Write(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            results.Result.Detail.Contains("外部サービス連携情報のVALUEの取得に失敗しました").IsTrue();
        }

        [TestMethod]
        public void 外部サービス連携情報の更新処理で例外が発生したらエラーとする()
        {
            var args = GetValidArgs();
            var entity = SetupValidMethods(args);

            // 更新処理で例外発生
            _externalRepo.Setup(m => m.UpsertEntity(entity)).Throws(new Exception());

            var results = _worker.Write(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("外部サービス連携情報の更新に失敗しました").IsTrue();
        }

        [TestMethod]
        public void 正常に更新できる()
        {
            var args = GetValidArgs();
            var entity = SetupValidMethods(args);

            _externalRepo.Setup(m => m.UpsertEntity(entity)).Callback((QH_EXTERNALSERVICELINKAGE_DAT e) =>
            {
                var json = new QsJsonSerializer().Deserialize<QhExternalServiceLinkageOfJson>(e.VALUE);

                // 同意フラグがTrueに変わっている
                json.IsTermsAccepted.IsTrue();
                // その他の情報は変化無し
                json.FacilityList.Count.Is(0);
            });

            var results = _worker.Write(args);
            // 成功
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
        }

        [TestMethod]
        public void 正常に更新できる_TargetServiceType自動設定の場合()
        {
            var args = GetValidArgs();
            args.TargetServiceType = 0; // 自動設定
            var entity = SetupValidMethods(args);

            _externalRepo.Setup(m => m.UpsertEntity(entity)).Callback((QH_EXTERNALSERVICELINKAGE_DAT e) =>
            {
                e.TARGETSERVICETYPE.Is(133); // 自動設定
                var json = new QsJsonSerializer().Deserialize<QhExternalServiceLinkageOfJson>(e.VALUE);

                // 同意フラグがTrueに変わっている
                json.IsTermsAccepted.IsTrue();
                // その他の情報は変化無し
                json.FacilityList.Count.Is(0);
            });

            var results = _worker.Write(args);
            // 成功
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
        }

        [TestMethod]
        public void 正常に更新できる_レコード登録が無かった場合()
        {
            var args = GetValidArgs();
            var entity = SetupValidMethods(args);

            // 該当データなし
            _externalRepo.Setup(m => m.ReadEntity(_accountKey, 1, args.TargetServiceType)).Returns(default(QH_EXTERNALSERVICELINKAGE_DAT));

            _externalRepo.Setup(m => m.UpsertEntity(It.IsAny<QH_EXTERNALSERVICELINKAGE_DAT>())).Callback((QH_EXTERNALSERVICELINKAGE_DAT e) =>
            {
                // 新規データとして正しく設定されている
                e.ACCOUNTKEY.Is(_accountKey);
                e.EXTERNALSERVICETYPE.Is(1);
                e.TARGETSERVICETYPE.Is(args.TargetServiceType);
                e.DELETEFLAG.IsFalse();
                var json = new QsJsonSerializer().Deserialize<QhExternalServiceLinkageOfJson>(e.VALUE);

                // 同意フラグがTrueに設定されている
                json.IsTermsAccepted.IsTrue();
                json.FacilityList.Count.Is(0);
            });

            var results = _worker.Write(args);
            // 成功
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
        }

        QH_EXTERNALSERVICELINKAGE_DAT SetupValidMethods(QoPaymentSmapaTermsWriteApiArgs args)
        {
            var json = new QhExternalServiceLinkageOfJson
            {
                IsTermsAccepted = false,
                FacilityList = new List<QhExternalServiceFacilityItem>()
            };

            var entity = new QH_EXTERNALSERVICELINKAGE_DAT
            {
                ACCOUNTKEY = _accountKey,
                EXTERNALSERVICETYPE = 1, // 1:アルメックス固定
                TARGETSERVICETYPE = args.TargetServiceType,
                VALUE = new QsJsonSerializer().Serialize(json)
            };

            _externalRepo.Setup(m => m.ReadEntity(_accountKey, 1, args.TargetServiceType)).Returns(entity);

            

            return entity;
        }

        QoPaymentSmapaTermsWriteApiArgs GetValidArgs()
        {
            return new QoPaymentSmapaTermsWriteApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "Hospa",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.TisiOSApp}",
                TargetServiceType = 1,
                IsTermsAccepted = true
            };
        }
    }
}
