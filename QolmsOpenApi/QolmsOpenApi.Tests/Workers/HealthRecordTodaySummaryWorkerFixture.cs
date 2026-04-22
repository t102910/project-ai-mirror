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
    public class HealthRecordTodaySummaryWorkerFixture
    {
        HealthRecordTodaySummaryWorker _worker;
        Mock<IHealthRecordRepository> _healthRecordRepo;
        Guid _accountKey = Guid.NewGuid();

        [TestInitialize]
        public void Initialize()
        {
            _healthRecordRepo = new Mock<IHealthRecordRepository>();
            _worker = new HealthRecordTodaySummaryWorker(_healthRecordRepo.Object);
        }

        [TestMethod]
        public void アカウントキーが不正でエラーとなる()
        {
            var args = GetValidArgs();
            args.ActorKey = "invalidGuid";

            var results = _worker.ReadSummary(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.ActorKey)).IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();
        }

        [TestMethod]
        public void 対象バイタル以外は対象外エラーとなる()
        {
            var args = GetValidArgs();
            args.VitalTypes = new List<byte> { (byte)QsDbVitalTypeEnum.Steps, (byte)QsDbVitalTypeEnum.Pulse } ;

            var results = _worker.ReadSummary(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains("VitalTypeが対応外です").IsTrue();
        }

        [TestMethod]
        public void 対象バイタルが未指定の場合はエラーとなる()
        {
            var args = GetValidArgs();
            args.VitalTypes = new List<byte>();

            var results = _worker.ReadSummary(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains("VitalTypeが指定されていません").IsTrue();
        }

        [TestMethod]
        public void バイタル取得処理で例外が発生したらエラー()
        {
            var args = GetValidArgs();

            // 例外を起こす
            _healthRecordRepo.Setup(m => m.ReadRange(_accountKey, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<byte[]>())).Throws(new Exception());

            var results = _worker.ReadSummary(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500"); 
            results.Result.Detail.Contains("バイタル取得処理でエラーが発生しました").IsTrue();
        }

        [TestMethod]
        public void 正常に終了する()
        {
            var args = GetValidArgs();

            var records = new List<QH_HEALTHRECORD_DAT>
            {
                CreateSteps(DateTime.Now, 340),
                CreateSteps(DateTime.Now, 250),
                CreateSteps(DateTime.Now.AddDays(-1),550),
                CreateSteps(DateTime.Now.AddDays(-1),221),
                CreateSteps(DateTime.Now.AddDays(-2),833),
                CreateSteps(DateTime.Now.AddDays(-3),555),
                CreateSteps(DateTime.Now.AddDays(-4),2600),
                CreateSleeps(DateTime.Now, 395),
                CreateSleeps(DateTime.Now, 334),
                CreateSleeps(DateTime.Now.AddDays(-1),374),
                CreateSleeps(DateTime.Now.AddDays(-1),391),
                CreateSleeps(DateTime.Now.AddDays(-2),429),
                CreateSleeps(DateTime.Now.AddDays(-3),325),
                CreateSleeps(DateTime.Now.AddDays(-4),193),
            };

            var toDate = DateTime.Today.AddDays(1).AddMilliseconds(-1);
            var fromDate = DateTime.Today.AddDays(-6);

            _healthRecordRepo.Setup(m => m.ReadRange(_accountKey, fromDate, toDate, 1, 18)).Returns(records);

            var results = _worker.ReadSummary(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // 歩数・睡眠で2件
            results.SummaryList.Count.Is(2);
            
            // 歩数サマリー
            var steps = results.SummaryList.First(x => x.VitalType == 1);
            steps.VitalType.Is((byte)1);
            steps.TodayValue.Is(590);
            steps.DayBeforeValue.Is(771);
            steps.Average.Is(1069.8d); // 平均 = 合計値 / データのある日数

            // 睡眠サマリー
            var sleeps = results.SummaryList.First(x => x.VitalType == 18);
            sleeps.VitalType.Is((byte)18);
            sleeps.TodayValue.Is(729);
            sleeps.DayBeforeValue.Is(765);
            sleeps.Average.Is(488.2d); // 平均 = 合計値 / データのある日数
        }

        [TestMethod]
        public void 正常に終了する_データがなければ空の情報が返る()
        {
            var args = GetValidArgs();

            var records = new List<QH_HEALTHRECORD_DAT>
            {                
                CreateSleeps(DateTime.Now, 395),
                CreateSleeps(DateTime.Now, 334),
                CreateSleeps(DateTime.Now.AddDays(-1),374),
                CreateSleeps(DateTime.Now.AddDays(-1),391),
                CreateSleeps(DateTime.Now.AddDays(-2),429),
                CreateSleeps(DateTime.Now.AddDays(-3),325),
                CreateSleeps(DateTime.Now.AddDays(-4),193),
            };

            var toDate = DateTime.Today.AddDays(1).AddMilliseconds(-1);
            var fromDate = DateTime.Today.AddDays(-6);

            _healthRecordRepo.Setup(m => m.ReadRange(_accountKey, fromDate, toDate, 1, 18)).Returns(records);

            var results = _worker.ReadSummary(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // 歩数・睡眠で2件
            results.SummaryList.Count.Is(2);

            // 歩数サマリー データがないけど0データが設定されている
            var steps = results.SummaryList.First(x => x.VitalType == 1);
            steps.VitalType.Is((byte)1);
            steps.TodayValue.Is(0);
            steps.DayBeforeValue.Is(0);
            steps.Average.Is(0d); 

            // 睡眠サマリー
            var sleeps = results.SummaryList.First(x => x.VitalType == 18);
            sleeps.VitalType.Is((byte)18);
            sleeps.TodayValue.Is(729);
            sleeps.DayBeforeValue.Is(765);
            sleeps.Average.Is(488.2d); // 平均 = 合計値 / データのある日数
        }

        QH_HEALTHRECORD_DAT CreateSteps(DateTime dateTime, decimal value)
        {
            return new QH_HEALTHRECORD_DAT
            {
                RECORDDATE = dateTime,
                VITALTYPE = 1,
                VALUE1 = value
            };
        }

        QH_HEALTHRECORD_DAT CreateSleeps(DateTime dateTime, decimal value)
        {
            return new QH_HEALTHRECORD_DAT
            {
                RECORDDATE = dateTime,
                VITALTYPE = 18,
                VALUE1 = value
            };
        }

        QoHealthRecordTodaySummaryApiArgs GetValidArgs()
        {
            return new QoHealthRecordTodaySummaryApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "HealthDiary",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.HealthDiaryiOSApp}",
                AverageDays = 7,
                VitalTypes = new List<byte> { 1,18 },                
            };
        }

    }
}
