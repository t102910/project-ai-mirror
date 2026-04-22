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
using System.Threading.Tasks;

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class PaymentSmapaRevokeWorkerFixture
    {
        Mock<ISmapaApiRepository> _smapaApi;
        PaymentSmapaRevokeWorker _worker;

        [TestInitialize]
        public void Initialize()
        {
            _smapaApi = new Mock<ISmapaApiRepository>();
            _worker = new PaymentSmapaRevokeWorker(_smapaApi.Object);
        }

        [TestMethod]
        public async Task SmapaApiでエラーだった場合はエラーとする()
        {
            var args = GetValidArgs();

            // SmapaApiがエラーを返す
            _smapaApi.Setup(m => m.ExecuteSmapaApi(args.MedicalFacilityCode, args.LinkageSystemId, QoApiConfiguration.SmapaApiRevokeUrl)).ReturnsAsync(new SmapaApiResults
            {
                Error = "1",
                ErrorFlag = "1001",
                ErrorDetail = "hoge"
            });

            var results = await _worker.Revoke(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            results.Result.Detail.Contains("SmapaApi内部エラーです").IsTrue();

            results.IsSmapaSuccess.IsFalse();
            results.SmapaErrorCode.Is("1001");
            results.SmapaErrorDetail.Is("hoge");
        }

        [TestMethod]
        public async Task SmapaApi接続処理で例外が発生した場合はエラーとする()
        {
            var args = GetValidArgs();

            // SmapaAPI接続で例外発生
            _smapaApi.Setup(m => m.ExecuteSmapaApi(args.MedicalFacilityCode, args.LinkageSystemId, QoApiConfiguration.SmapaApiRevokeUrl)).ThrowsAsync(new Exception());

            var results = await _worker.Revoke(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("Smapa API 接続処理でエラーが発生しました。").IsTrue();

            results.IsSmapaSuccess.IsFalse();
            results.SmapaErrorCode.Is("");
            results.SmapaErrorDetail.Is("");
        }

        [TestMethod]
        public async Task 正常に退会する()
        {
            var args = GetValidArgs();

            // 正常に取得
            _smapaApi.Setup(m => m.ExecuteSmapaApi(args.MedicalFacilityCode, args.LinkageSystemId, QoApiConfiguration.SmapaApiRevokeUrl)).ReturnsAsync(new SmapaApiResults
            {
                Error = "0",
                ErrorFlag = "",
            });

            var results = await _worker.Revoke(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            results.IsSmapaSuccess.IsTrue();
            results.SmapaErrorCode.Is("");
            results.SmapaErrorDetail.Is("");
        }

        QoPaymentSmapaRevokeApiArgs GetValidArgs()
        {
            return new QoPaymentSmapaRevokeApiArgs
            {
                ActorKey = Guid.Empty.ToApiGuidString(),
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "Hospa",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.TisiOSApp}",
                MedicalFacilityCode = "9999910006",
                LinkageSystemId = "00001300"
            };
        }
    }
}
