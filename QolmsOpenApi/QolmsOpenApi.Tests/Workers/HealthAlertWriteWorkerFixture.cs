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
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class HealthAlertWriteWorkerFixture
    {
        HealthAlertWriteWorker _worker;
        Mock<IHealthRecordAlertRepository> _healthAlertRepo;
        Mock<IHealthRecordValidator> _validator;
        Guid _accountKey = Guid.NewGuid();

        [TestInitialize]
        public void Initilaize()
        {
            _healthAlertRepo = new Mock<IHealthRecordAlertRepository>();
            _validator = new Mock<IHealthRecordValidator>();
            _worker = new HealthAlertWriteWorker(_healthAlertRepo.Object, _validator.Object);
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
        public void RecordDateが不正でエラーとなる()
        {
            var args = GetValidArgs();
            args.HealthAlertItemN[0].RecordDate = "hoge";

            var results = _worker.Write(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(QoHealthAlertItem.RecordDate)).IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();
        }

        [TestMethod]
        public void バイタルタイプが不正でエラーとなる()
        {
            var args = GetValidArgs();
            args.HealthAlertItemN[0].VitalType = 100; // 範囲外

            var results = _worker.Write(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(QoHealthAlertItem.VitalType)).IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();
        }

        [TestMethod]
        public void アブノーマルタイプが不正でエラーとなる()
        {
            var args = GetValidArgs();
            args.HealthAlertItemN[0].AbnormalType = 100; // 範囲外

            var results = _worker.Write(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(QoHealthAlertItem.AbnormalType)).IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();
        }

        [TestMethod]
        public void バイタル値のバリデーションが通らないとエラー()
        {
            var args = GetValidArgs();            

            var value1 = args.HealthAlertItemN[0].Value1;
            var value2 = args.HealthAlertItemN[0].Value2;

            // バリデーションエラーを設定
            _validator.Setup(m => m.ValidateForAlert(QsDbVitalTypeEnum.Pulse, value1, value2)).Returns((false, "hoge"));

            var results = _worker.Write(args);
            
            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains("hoge").IsTrue();

            // バリデーションのテストはValidtorの方で行う
        }

        [TestMethod]
        public void 書き込み処理で例外が発生するとエラー()
        {
            var args = GetValidArgs();

            // バリデーション通過設定
            _validator.Setup(m => m.ValidateForAlert(It.IsAny<QsDbVitalTypeEnum>(),It.IsAny<decimal>(), It.IsAny<decimal>())).Returns((true, null));

            // 採番取得で例外を起こす
            _healthAlertRepo.Setup(m => m.GetNewAlertNo()).Throws(new Exception());

            var results = _worker.Write(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("アラート書き込み処理でエラーが発生しました").IsTrue();
        }

        [TestMethod]
        public void 正常に終了する()
        {
            var args = GetValidArgs();

            // バリデーション通過設定
            _validator.Setup(m => m.ValidateForAlert(It.IsAny<QsDbVitalTypeEnum>(), It.IsAny<decimal>(), It.IsAny<decimal>())).Returns((true, null));

            _healthAlertRepo.SetupSequence(x => x.GetNewAlertNo())
                .Returns(1)
                .Returns(2);

            var seq = new MockSequence();

            _healthAlertRepo.InSequence(seq).Setup(m => m.InsertEntity(It.IsAny<QH_HEALTHRECORDALERT_DAT>())).Callback((QH_HEALTHRECORDALERT_DAT entity) =>
            {
                // 秒以下は切り捨て。かつ順番は日付昇順で処理されている
                entity.RECORDDATE.Is(new DateTime(2023, 10, 1, 11, 5, 0));
                entity.ALERTNO.Is(1);
                entity.VITALTYPE.Is((byte)19);
                entity.VALUE1.Is(99);
                entity.VALUE2.Is(-1);
                entity.ABNORMALTYPE.Is((byte)QsDbVitalAbnormalTypeEnum.High);
                entity.ALERTTYPE.Is((byte)QsDbVitalAlertTypeEnum.JotoHeartRate);
                entity.EMERGENCYTYPE.Is((byte)QsDbVitalEmergencyTypeEnum.Warning);
                entity.MESSAGE.Is("fuga");
                entity.LINKAGESYSTEMNO.Is(47016);
            });

            _healthAlertRepo.InSequence(seq).Setup(m => m.InsertEntity(It.IsAny<QH_HEALTHRECORDALERT_DAT>())).Callback((QH_HEALTHRECORDALERT_DAT entity) =>
            {
                entity.RECORDDATE.Is(new DateTime(2023, 10, 1, 11, 10, 0));
                entity.ALERTNO.Is(2);
                entity.VITALTYPE.Is((byte)8);
                entity.VALUE1.Is(45);
                entity.VALUE2.Is(-1);
                entity.ABNORMALTYPE.Is((byte)QsDbVitalAbnormalTypeEnum.Low);
                entity.ALERTTYPE.Is((byte)QsDbVitalAlertTypeEnum.JotoHeartRate);
                entity.EMERGENCYTYPE.Is((byte)QsDbVitalEmergencyTypeEnum.Warning);
                entity.MESSAGE.Is("hoge");
                entity.LINKAGESYSTEMNO.Is(47016);
            });

            var results = _worker.Write(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // 採番処理が2回実行された
            _healthAlertRepo.Verify(m => m.GetNewAlertNo(), Times.Exactly(2));
        }

        QoHealthAlertWriteApiArgs GetValidArgs()
        {
            var items = new List<QoHealthAlertItem>
            {
                new QoHealthAlertItem
                {
                    RecordDate = new DateTime(2023,10,1,11,10,15).ToApiDateString(),
                    VitalType = (byte)QsDbVitalTypeEnum.Pulse,
                    Value1 = 45,
                    Value2 = -1,
                    AbnormalType = (byte)QsDbVitalAbnormalTypeEnum.Low,
                    Message = "hoge"
                },
                new QoHealthAlertItem
                {
                    RecordDate = new DateTime(2023,10,1,11,5,15).ToApiDateString(),
                    VitalType = (byte)QsDbVitalTypeEnum.BloodOxygen,
                    Value1 = 99,
                    Value2 = -1,
                    AbnormalType = (byte)QsDbVitalAbnormalTypeEnum.High,
                    Message = "fuga"
                },
            };

            return new QoHealthAlertWriteApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "HeartMonitorApp",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.HeartMonitoriOSApp}",
                HealthAlertItemN = items
            };
        }
    }
}
