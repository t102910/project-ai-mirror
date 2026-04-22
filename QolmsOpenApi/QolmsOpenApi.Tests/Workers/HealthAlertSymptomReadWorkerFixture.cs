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
    public class HealthAlertSymptomReadWorkerFixture
    {
        HealthAlertSymptomReadWorker _worker;
        Mock<IHealthRecordAlertRepository> _healthAlertRepo;
        Guid _accountKey = Guid.NewGuid();

        [TestInitialize]
        public void Initialize()
        {
            _healthAlertRepo = new Mock<IHealthRecordAlertRepository>();
            _worker = new HealthAlertSymptomReadWorker(_healthAlertRepo.Object);
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
        public void 開始日が不正でエラーとなる()
        {
            var args = GetValidArgs();
            args.FromDate = "";

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.FromDate)).IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();
        }

        [TestMethod]
        public void データ取得で例外が発生したらエラーとなる()
        {
            var args = GetValidArgs();

            // 例外を発生させる
            _healthAlertRepo.Setup(m => m.ReadAlertSymptomView(_accountKey, 47016, DateTime.Today.AddDays(-30), DateTime.Today.AddDays(1))).Throws(new Exception());

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500"); 
            results.Result.Detail.Contains("アラートと症状のリストの取得処理に失敗しました").IsTrue();
        }

        [TestMethod]
        public void 正常にデータを取得できる()
        {
            var args = GetValidArgs();

            var entityList = new List<QH_HEALTH_ALERT_SYMPTOM_VIEW>
            {
                new QH_HEALTH_ALERT_SYMPTOM_VIEW
                {
                    ALERTNO = 1,
                    SYMPTOMID = Guid.Empty,
                    RECORDDATE = new DateTime(2024,1,9,10,30,0),
                    DATATYPE = (byte)QoApiHealthDataTypeEnum.Alert,
                    VITALTYPE = 8,
                    VALUE1 = 100,
                    VALUE2 = -1,
                    ABNORMALTYPE = (byte)QsDbVitalAbnormalTypeEnum.Low         
                },
                new QH_HEALTH_ALERT_SYMPTOM_VIEW
                {
                    ALERTNO = 0,
                    SYMPTOMID = Guid.NewGuid(),
                    RECORDDATE = new DateTime(2024,1,8,11,30,0),
                    DATATYPE = (byte)QoApiHealthDataTypeEnum.Symptom,
                    VITALTYPE = 0,
                    VALUE1 = 0,
                    VALUE2 = 0,
                    ABNORMALTYPE = 0
                },
            };

            // DBよりEntityList取得(ToDateは未指定なので明日が指定される)
            _healthAlertRepo.Setup(m => m.ReadAlertSymptomView(_accountKey, 47016, DateTime.Today.AddDays(-30), DateTime.Today.AddDays(1))).Returns(entityList);

            var results = _worker.Read(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // 2件取得できている
            results.ItemList.Count.Is(2);
            var rec1 = results.ItemList[0];

            // レコード1が正しく変換されている
            rec1.AlertNo.Is(entityList[0].ALERTNO);
            rec1.SymptomId.Is(Guid.Empty);
            rec1.RecordDate.Is(entityList[0].RECORDDATE.ToApiDateString());
            rec1.DataType.Is(QoApiHealthDataTypeEnum.Alert);
            rec1.VitalType.Is(entityList[0].VITALTYPE);
            rec1.Value1.Is(entityList[0].VALUE1);
            rec1.Value2.Is(entityList[0].VALUE2);
            rec1.AbnormalType.Is((byte)QsDbVitalAbnormalTypeEnum.Low);

            var rec2 = results.ItemList[1];

            // レコード2が正しく変換されている
            rec2.AlertNo.Is(0);
            rec2.SymptomId.Is(entityList[1].SYMPTOMID);
            rec2.RecordDate.Is(entityList[1].RECORDDATE.ToApiDateString());
            rec2.DataType.Is(QoApiHealthDataTypeEnum.Symptom);
            rec2.VitalType.Is((byte)0);
            rec2.Value1.Is(entityList[1].VALUE1);
            rec2.Value2.Is(entityList[1].VALUE2);
            rec2.AbnormalType.Is((byte)0);
        }

        [TestMethod]
        public void 正常に取得できる_終了日指定の場合()
        {
            var args = GetValidArgs();
            // 明示的に終了日を指定する
            args.ToDate = DateTime.Today.AddDays(-2).ToApiDateString();

            // DBよりEntityList取得
            _healthAlertRepo.Setup(m => m.ReadAlertSymptomView(_accountKey, 47016, DateTime.Today.AddDays(-30), DateTime.Today.AddDays(-2))).Returns(new List<QH_HEALTH_ALERT_SYMPTOM_VIEW>());

            var results = _worker.Read(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // 0件
            results.ItemList.Count.Is(0);

            // DB取得処理が正しく呼ばれた
            _healthAlertRepo.Verify(m => m.ReadAlertSymptomView(_accountKey, 47016, DateTime.Today.AddDays(-30), DateTime.Today.AddDays(-2)), Times.Once);
        }

        QoHealthAlertSymptomReadApiArgs GetValidArgs()
        {
            return new QoHealthAlertSymptomReadApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "HeartMonitor",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.HeartMonitoriOSApp}",
                FromDate = DateTime.Today.AddDays(-30).ToApiDateString(),
            };
        }
    }
}
