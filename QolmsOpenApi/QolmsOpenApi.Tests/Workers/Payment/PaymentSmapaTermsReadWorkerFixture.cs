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
    public class PaymentSmapaTermsReadWorkerFixture
    {
        Mock<IExternalServiceLinkageRepository> _externalRepo;
        PaymentSmapaTermsReadWorker _worker;
        Guid _accountKey = Guid.NewGuid();

        [TestInitialize]
        public void Initialize()
        {
            _externalRepo = new Mock<IExternalServiceLinkageRepository>();
            _worker = new PaymentSmapaTermsReadWorker(_externalRepo.Object);
        }

        [TestMethod]
        public void アカウントキーが不正でエラーとなる()
        {
            var args = GetValidArgs();
            args.ActorKey = "invalidGuid";

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.ActorKey)).IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();
        }

        [TestMethod]
        public void レコードのVALUE値が異常値の場合はエラーとする()
        {
            var args = GetValidArgs();

            // VALUEが規定Jsonクラス外
            _externalRepo.Setup(m => m.ReadEntity(_accountKey, 1, args.TargetServiceType)).Returns(new QH_EXTERNALSERVICELINKAGE_DAT
            {
                VALUE = "hogefuga"
            });

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005"); 
            results.Result.Detail.Contains("外部サービス連携情報のVALUEの取得に失敗しました").IsTrue();
        }

        [TestMethod]
        public void 外部サービス連携情報取得処理で例外が発生するとエラーとなる()
        {
            var args = GetValidArgs();

            // レコード取得処理で例外発生
            _externalRepo.Setup(m => m.ReadEntity(_accountKey, 1, args.TargetServiceType)).Throws(new Exception());

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("外部サービス連携情報の取得に失敗しました").IsTrue();
        }

        [TestMethod]
        public void 正常に同意状況を取得できる()
        {
            var args = GetValidArgs();

            var json = new QhExternalServiceLinkageOfJson
            {
                IsTermsAccepted = true,
                FacilityList = new List<QhExternalServiceFacilityItem>()
            };

            _externalRepo.Setup(m => m.ReadEntity(_accountKey, 1, args.TargetServiceType)).Returns(new QH_EXTERNALSERVICELINKAGE_DAT
            {
                VALUE = new QsJsonSerializer().Serialize(json)
            });

            var results = _worker.Read(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
            // 正常にjsonのIsTermsAcceptedの値が設定されている
            results.IsTermsAccepted.IsTrue();
        }

        [TestMethod]
        public void 正常に同意状況を取得できる_レコードが存在しない場合()
        {
            var args = GetValidArgs();

            var json = new QhExternalServiceLinkageOfJson
            {
                IsTermsAccepted = true,
                FacilityList = new List<QhExternalServiceFacilityItem>()
            };

            // 該当レコードなし
            _externalRepo.Setup(m => m.ReadEntity(_accountKey, 1, args.TargetServiceType)).Returns(default(QH_EXTERNALSERVICELINKAGE_DAT));

            var results = _worker.Read(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
            // レコードが無かった場合は未同意となる
            results.IsTermsAccepted.IsFalse();
        }

        [TestMethod]
        public void 正常に同意状況を取得できる_VALUEが空の場合()
        {
            var args = GetValidArgs();

            var json = new QhExternalServiceLinkageOfJson
            {
                IsTermsAccepted = true,
                FacilityList = new List<QhExternalServiceFacilityItem>()
            };

            // VALUEが空
            _externalRepo.Setup(m => m.ReadEntity(_accountKey, 1, args.TargetServiceType)).Returns(new QH_EXTERNALSERVICELINKAGE_DAT
            {
                VALUE = string.Empty
            });

            var results = _worker.Read(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
            // VALUEが空の場合は未同意となる
            results.IsTermsAccepted.IsFalse();
        }

        [TestMethod]
        public void 正常に同意状況を取得できる_TargetServiceType自動設定の場合()
        {
            var args = GetValidArgs();
            args.TargetServiceType = 0; // 自動設定

            var json = new QhExternalServiceLinkageOfJson
            {
                IsTermsAccepted = true,
                FacilityList = new List<QhExternalServiceFacilityItem>()
            };

            // レコード値を返す設定(自動設定で133で呼び出される)
            _externalRepo.Setup(m => m.ReadEntity(_accountKey, 1, 133)).Returns(new QH_EXTERNALSERVICELINKAGE_DAT
            {
                VALUE = new QsJsonSerializer().Serialize(json)
            });

            var results = _worker.Read(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
            // 正常にjsonのIsTermsAcceptedの値が設定されている
            results.IsTermsAccepted.IsTrue();
        }

        QoPaymentSmapaTermsReadApiArgs GetValidArgs()
        {
            return new QoPaymentSmapaTermsReadApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "Hospa",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.TisiOSApp}",
                TargetServiceType = 1
            };
        }

    }
}
