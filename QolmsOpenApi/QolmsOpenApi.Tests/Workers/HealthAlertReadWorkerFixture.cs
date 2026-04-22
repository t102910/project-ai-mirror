using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class HealthAlertReadWorkerFixture
    {
        HealthAlertReadWorker _worker;
        Mock<IHealthRecordAlertRepository> _healthAlertRepo;
        Guid _accountKey = Guid.NewGuid();

        [TestInitialize]
        public void Initialize()
        {
            _healthAlertRepo = new Mock<IHealthRecordAlertRepository>();
            _worker = new HealthAlertReadWorker(_healthAlertRepo.Object);
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
        public void データ取得で例外が発生したらエラーとなる()
        {
            var args = GetValidArgs();

            // 例外を発生させる
            _healthAlertRepo.Setup(m => m.ReadEntities(_accountKey, DateTime.MinValue, DateTime.Today.AddDays(1), 8, 0, 10, 47016)).Throws(new Exception());

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500"); 
            results.Result.Detail.Contains("アラートのリストの取得処理に失敗しました").IsTrue();
        }

        [TestMethod]
        public void 正常にデータを取得できる()
        {
            var args = GetValidArgs();

            var entityList = new List<QH_HEALTHRECORDALERT_DAT>
            {
                new QH_HEALTHRECORDALERT_DAT
                {
                    ALERTNO = 1,
                    RECORDDATE = new DateTime(2024,5,30,10,30,0),
                    VITALTYPE = 8,
                    VALUE1 = 100,
                    VALUE2 = -1,
                    ABNORMALTYPE = (byte)QsDbVitalAbnormalTypeEnum.Low,
                    MESSAGE = "AvJFunNLENCqMAo/wSO5ItTdkUKzQ5Kk0u4gGzWC/mI="
                },
                new QH_HEALTHRECORDALERT_DAT
                {
                    ALERTNO = 0,
                    RECORDDATE = new DateTime(2024,5,30,11,30,0),
                    VITALTYPE = 0,
                    VALUE1 = 0,
                    VALUE2 = 0,
                    ABNORMALTYPE = 0,
                    MESSAGE = "AvJFunNLENCqMAo/wSO5IvdwPUrh8awdYVWAIAYiSHk="
                },
            };

            // DBよりEntityList取得
            _healthAlertRepo.Setup(m => m.ReadEntities(_accountKey, DateTime.MinValue, DateTime.Today.AddDays(1), 8, 0, 10, 47016)).Returns(entityList);

            var results = _worker.Read(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // 2件取得できている
            results.ItemList.Count.Is(2);
            var rec1 = results.ItemList[0];

            // レコード1が正しく変換されている
            rec1.RecordDate.Is(entityList[0].RECORDDATE.ToApiDateString());
            rec1.VitalType.Is(entityList[0].VITALTYPE);
            rec1.Value1.Is(entityList[0].VALUE1);
            rec1.Value2.Is(entityList[0].VALUE2);
            rec1.AbnormalType.Is((byte)QsDbVitalAbnormalTypeEnum.Low);
            rec1.Message.Is("メッセージ１");

            var rec2 = results.ItemList[1];

            // レコード2が正しく変換されている
            rec2.RecordDate.Is(entityList[1].RECORDDATE.ToApiDateString());
            rec2.VitalType.Is((byte)0);
            rec2.Value1.Is(entityList[1].VALUE1);
            rec2.Value2.Is(entityList[1].VALUE2);
            rec2.AbnormalType.Is((byte)0);
            rec2.Message.Is("メッセージ２");
        }

        [TestMethod]
        public void 正常に取得できる_開始日終了日指定の場合()
        {
            var args = GetValidArgs();
            // 明示的に開始日終了日を指定する
            var toDate = new DateTime(2024, 5, 28, 10, 30, 0);
            var fromDate = new DateTime(2024, 5, 27, 10, 30, 0);
            args.ToDate = toDate.ToApiDateString();
            args.FromDate = fromDate.ToApiDateString();

            // DBよりEntityList取得
            _healthAlertRepo.Setup(m => m.ReadEntities(_accountKey, fromDate, toDate, 8, 0, 10, 47016)).Returns(new List<QH_HEALTHRECORDALERT_DAT>());

            var results = _worker.Read(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // 0件
            results.ItemList.Count.Is(0);

            // DB取得処理が正しく呼ばれた
            _healthAlertRepo.Verify(m => m.ReadEntities(_accountKey, fromDate, toDate, 8, 0, 10, 47016), Times.Once);
        }

        QoHealthAlertReadApiArgs GetValidArgs()
        {
            return new QoHealthAlertReadApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "HeartMonitor",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.HeartMonitoriOSApp}",
                VitalType = (byte)QsDbVitalTypeEnum.Pulse,
                Offset = 0,
                Fetch = 10,
            };
        }
    }
}
