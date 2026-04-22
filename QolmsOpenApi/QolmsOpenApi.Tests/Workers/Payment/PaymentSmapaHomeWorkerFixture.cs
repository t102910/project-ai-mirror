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
    public class PaymentSmapaHomeWorkerFixture
    {
        Mock<ISmapaApiRepository> _smapaApi;
        PaymentSmapaHomeWorker _worker;

        [TestInitialize]
        public void Initialize()
        {
            _smapaApi = new Mock<ISmapaApiRepository>();
            _worker = new PaymentSmapaHomeWorker(_smapaApi.Object);
        }

        [TestMethod]
        public async Task SmapaApiでエラーだった場合はエラーとする()
        {
            var args = GetValidArgs();

            // SmapaApiがエラーを返す
            _smapaApi.Setup(m => m.ExecuteSmapaApi(args.MedicalFacilityCode, args.LinkageSystemId, QoApiConfiguration.SmapaApiHomeUrl)).ReturnsAsync(new SmapaApiResults
            {
                Error = "1",
                ErrorFlag = "1001",
                ErrorDetail = "Hoge"
            });

            var results = await _worker.GetSmapaHome(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            results.Result.Detail.Contains("SmapaApi内部エラーです").IsTrue();
            results.Result.Detail.Contains("Hoge").IsTrue();
        }

        [TestMethod]
        public async Task SmapaApiでURLが取得できていない場合はエラーとする()
        {
            var args = GetValidArgs();

            // 正常終了だがURLが入っていない
            _smapaApi.Setup(m => m.ExecuteSmapaApi(args.MedicalFacilityCode, args.LinkageSystemId, QoApiConfiguration.SmapaApiHomeUrl)).ReturnsAsync(new SmapaApiResults
            {
                Error = "0",
                ErrorFlag = "",
                Url = ""
            });

            var results = await _worker.GetSmapaHome(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            results.Result.Detail.Contains("SmapaWebのURLが取得できませんでした").IsTrue();
        }

        [TestMethod]
        public async Task SmapaApi接続処理で例外が発生した場合はエラーとする()
        {
            var args = GetValidArgs();

            // SmapaAPI接続で例外発生
            _smapaApi.Setup(m => m.ExecuteSmapaApi(args.MedicalFacilityCode, args.LinkageSystemId, QoApiConfiguration.SmapaApiHomeUrl)).ThrowsAsync(new Exception());

            var results = await _worker.GetSmapaHome(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("Smapa API 接続処理でエラーが発生しました。").IsTrue();
        }

        [TestMethod]
        public async Task 正常にURLが取得できる()
        {
            var args = GetValidArgs();

            // 正常に取得
            _smapaApi.Setup(m => m.ExecuteSmapaApi(args.MedicalFacilityCode, args.LinkageSystemId, QoApiConfiguration.SmapaApiHomeUrl)).ReturnsAsync(new SmapaApiResults
            {
                Error = "0",
                ErrorFlag = "",
                Url = "https://abc.def.com"
            });

            var results = await _worker.GetSmapaHome(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
            results.Url.Is("https://abc.def.com");
        }

        QoPaymentSmapaHomeApiArgs GetValidArgs()
        {
            return new QoPaymentSmapaHomeApiArgs
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
