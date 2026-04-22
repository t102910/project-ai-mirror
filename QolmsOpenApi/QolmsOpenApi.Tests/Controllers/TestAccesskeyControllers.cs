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
    public class TestAccessKeyContller
    {
        private const string EXECUTOR = "eizk0BvAjplRIljBpKm57z5e2bqQ2/HkJXUI7YRzMUYlefeWoG0dLl9K96DRQ/dr";
        private const string EXECUTORNAME = "xbcXXzuwm9GMF/bu8K9ujA==";

        [TestMethod]
        public void PostGenerate_Success()
        {
            //ログイン成功したらTokenが返ってくるので本来はそのトークンをつけて認証通す（するとActorKeyがはいる）が
            //認証属性までテストするにはサーバが必要なので省略。
            var args = new QoAccessKeyGenerateApiArgs()
            {
                ApiType = QoApiTypeEnum.AccessKeyGenerate.ToString(),
                ExecuteSystemType = QsApiSystemTypeEnum.QolmsOpenApi.ToString(),
                ExecutorName = EXECUTORNAME,
                Executor = EXECUTOR,
                ActorKey = "gzwk8kqky2LTuAsqeFQnow1GwpcMWkVuKvMYydUwd8AWHIpKwmJ2cD3stDAXQ0xn"                    //テスト用（本来はトークンからいれられる）
            };
            
            var controller = new AccessKeyController();
            var result = controller.PostGenerate(args);
            Assert.AreEqual(result.IsSuccess.TryToValueType(false), true);
            Assert.AreEqual(string.IsNullOrWhiteSpace(result.AccessKey), false);
        }

        [TestMethod]
        public void PostRefresh_Success()
        {
            //ログイン成功したらTokenが返ってくるので本来はそのトークンをつけて認証通す（するとActorKeyがはいる）が
            //認証属性までテストするにはサーバが必要なので省略。
            var args = new QoAccessKeyRefreshApiArgs()
            {
                ApiType = QoApiTypeEnum.AccessKeyRefresh.ToString(),
                ExecuteSystemType = QsApiSystemTypeEnum.QolmsOpenApi.ToString(),
                ExecutorName = EXECUTORNAME,
                Executor = EXECUTOR,
                ActorKey = "gzwk8kqky2LTuAsqeFQnow1GwpcMWkVuKvMYydUwd8AWHIpKwmJ2cD3stDAXQ0xn"                    //テスト用（本来はトークンからいれられる）
            };

            var controller = new AccessKeyController();
            var result = controller.PostRefresh(args);
            Assert.AreEqual(result.IsSuccess.TryToValueType(false), true);
            Assert.AreEqual(string.IsNullOrWhiteSpace(result.AccessKey), false);
        }

    }
}
