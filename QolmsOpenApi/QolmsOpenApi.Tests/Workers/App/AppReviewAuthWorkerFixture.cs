using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class AppReviewAuthWorkerFixture
    {
        AppReviewAuthWorker _worker;

        [TestInitialize]
        public void Initialize()
        {
            _worker = new AppReviewAuthWorker();
        }


        [TestMethod]
        public void IDが不一致でエラー()
        {
            var args = GetValidArgs();
            args.Id = "hoge";

            var results = _worker.Auth(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
        }

        [TestMethod]
        public void Passが不一致でエラー()
        {
            var args = GetValidArgs();
            args.Password = "hoge";

            var results = _worker.Auth(args);

            // 失敗
            results.IsSuccess.Is(bool.FalseString);
        }

        [TestMethod]
        public void IDとPassが一致で成功()
        {
            var args = GetValidArgs();

            var results = _worker.Auth(args);

            results.IsSuccess.Is(bool.TrueString);
        }


        QoAppReviewAuthApiArgs GetValidArgs()
        {
            return new QoAppReviewAuthApiArgs
            {
                Id = "95521654",
                Password = "Z92DfeRtvf@d"
            };
        }
    }

    
}
