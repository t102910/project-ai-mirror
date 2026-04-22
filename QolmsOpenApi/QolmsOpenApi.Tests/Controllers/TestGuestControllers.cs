using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MGF.QOLMS.QolmsOpenApi.Controllers;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsApiCoreV1;

namespace QolmsOpenApi.Tests
{
    [TestClass]
    public class TestGuestControllers
    {
        private const string EXECUTOR = "eizk0BvAjplRIljBpKm57z5e2bqQ2/HkJXUI7YRzMUYlefeWoG0dLl9K96DRQ/dr";
        private const string EXECUTORNAME = "xbcXXzuwm9GMF/bu8K9ujA==";

        [TestMethod]
        public void PostAccessKeyGenerate_Success()
        {
            var args = new QoGuestAccessKeyGenerateApiArgs()
            {
                ApiType = QoApiTypeEnum.GuestAccessKeyGenerate.ToString(),
                ExecuteSystemType = QsApiSystemTypeEnum.QolmsOpenApi.ToString(),
                ExecutorName = EXECUTORNAME,
                Executor = EXECUTOR
            };
            
            var controller = new GuestController();
            var result = controller.PostAccessKeyGenerate(args);
            Assert.AreEqual(result.IsSuccess.TryToValueType(false), true);
            Assert.AreEqual(string.IsNullOrWhiteSpace(result.AccessKey), false);
        }

        [TestMethod]
        public void PostAccessKeyRefresh_Success()
        {
            var args = new QoGuestAccessKeyRefreshApiArgs()
            {
                ApiType = QoApiTypeEnum.GuestAccessKeyRefresh.ToString(),
                ExecuteSystemType = QsApiSystemTypeEnum.QolmsOpenApi.ToString(),
                ExecutorName = EXECUTORNAME,
                Executor = EXECUTOR,
            };

            var controller = new GuestController();
            var result = controller.PostAccessKeyRefresh(args);
            Assert.AreEqual(result.IsSuccess.TryToValueType(false), true);
            Assert.AreEqual(string.IsNullOrWhiteSpace(result.AccessKey), false);
        }

    }
}
