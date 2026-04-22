using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MGF.QOLMS.QolmsOpenApi.Controllers;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsApiCoreV1;

namespace QolmsOpenApi.Tests
{
    [TestClass]
    public class TestJotoControllers
    {
        private const string EXECUTOR = "6TtPhnRpWVDbFmjlDIApwgaKple5NvKbDe7SMvk+8IY4/e0bsFsRjlhxSuvzy31E"; //心拍見守りアプリ検証用
        private const string EXECUTORNAME = "YwTbRt4GOBdEYnqRzwkEGjBWlUhGr4R3GYbCOzQTxKQ=";

        [TestMethod]
        public void PostHeartRateWarningRead_Success()
        {
            var args = new QoJotoHdrHeartRateWarningReadApiArgs()
            {
                ApiType = QoApiTypeEnum.JotoHdrHeartRateWarningRead.ToString(),
                ExecuteSystemType = QsApiSystemTypeEnum.QolmsOpenApi.ToString(),
                AlertNo ="",
                CheckDate =DateTime.Now.AddDays(-3).ToApiDateString(),
                ExecutorName = EXECUTORNAME,
                Executor = EXECUTOR
            };

            var controller = new JotoHdrController();
            var result = controller.PostHeartRateWarningRead(args);
            Assert.AreEqual(result.IsSuccess.TryToValueType(false), true);
            Assert.AreNotEqual(result.ExecutedDate.TryToValueType<DateTime>(DateTime.MinValue),DateTime.MinValue );
            Assert.AreNotEqual(result.TargetList.Count ,0);
            Assert.AreNotEqual(string.IsNullOrWhiteSpace(result.TargetList[0].Token), true);


        }

        [TestMethod]
        public void TestGetGenerateToken_Success()
        {
            var controller = new JotoHdrController();
            var result = controller.GetGenerateToken(EXECUTOR);

            Assert.AreNotEqual(result.Length, 0);
        }
    
    }
}
