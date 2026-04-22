using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class HealthRecordBatchWorkerFixture
    {
        HealthRecordBatchWorker _worker;
        Mock<IHealthRecordQueueRepository> _repo;
        Mock<IHealthRecordValidator> _validator;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new Mock<IHealthRecordQueueRepository>();
            _validator = new Mock<IHealthRecordValidator>();
            _worker = new HealthRecordBatchWorker(_repo.Object, _validator.Object);
        }

        [TestMethod]
        public void 引数チェックにひっかかると例外が発生する()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                // 引数nullは例外
                _worker.ImportBatch(null);
            });

            var args = new QoHealthRecordImportApiArgs();

            Assert.ThrowsException<ArgumentException>(() =>
            {
                // ActorKeyがない場合は例外
                _worker.ImportBatch(args);
            });


            args.ActorKey = "hoge";

            Assert.ThrowsException<ArgumentException>(() =>
            {
                // バイタルリストがnullの場合は例外
                _worker.ImportBatch(args);
            });

            args.VitalValueN = new List<QhApiVitalValueItem>();
            Assert.ThrowsException<ArgumentException>(() =>
            {
                // バイタルリストが0件の場合は例外
                _worker.ImportBatch(args);
            });

            args.VitalValueN.Add(new QhApiVitalValueItem());

            args.Executor = null;
            Assert.ThrowsException<ArgumentException>(() =>
            {
                // Executor未設定は例外
                _worker.ImportBatch(args);
            });            
        }       
        
        [TestMethod]
        public void バイタル値チェックでNGであればNG()
        {
            var args = GetValidVitalArgs();

            // バリデーションでこける設定
            _validator.Setup(m => m.Validate(args.VitalValueN)).Returns((false, "error"));

            var result = _worker.ImportBatch(args);

            // 失敗
            result.IsSuccess.Is(bool.FalseString);
            result.Result.Detail.Contains("error");

            // キュー登録まで到達しなかった。
            _repo.Verify(m => m.Enqueue(It.IsAny<Guid>(), It.IsAny<List<QhApiVitalValueItem>>()), Times.Never);
        }

        [TestMethod]
        public void QueueStorageの登録に失敗したらNG()
        {
            // 正常値引数
            var args = GetValidVitalArgs();

            // バリデーション通過設定
            _validator.Setup(m => m.Validate(args.VitalValueN)).Returns((true, null));

            // キューの登録に失敗するように設定
            _repo.Setup(m => m.Enqueue(It.IsAny<Guid>(), args.VitalValueN)).Returns(false);

            var result = _worker.ImportBatch(args);

            // 失敗
            result.IsSuccess.Is(bool.FalseString);
            result.Result.Detail.Contains("キューの登録に失敗しました").IsTrue();            
        }

        [TestMethod]
        public void データが正常でキューが登録されたら正常終了する()
        {
            // 正常値引数
            var args = GetValidVitalArgs();
            // バリデーション通過設定
            _validator.Setup(m => m.Validate(args.VitalValueN)).Returns((true, null));

            // キューの登録に成功する設定
            _repo.Setup(m => m.Enqueue(It.IsAny<Guid>(), args.VitalValueN)).Returns(true);

            var result = _worker.ImportBatch(args);

            // 成功
            result.IsSuccess.Is(bool.TrueString);
        }

        QoHealthRecordImportApiArgs GetBaseArgs()
        {
            return new QoHealthRecordImportApiArgs
            {
                ActorKey = "6ed1824d-aa9b-4277-9826-459cfb4d0b55",
            };
        }

        QoHealthRecordImportApiArgs GetValidVitalArgs()
        {
            var args = GetBaseArgs();
            args.VitalValueN = new List<QhApiVitalValueItem>
            {
                new QhApiVitalValueItem
                {
                    RecordDate = DateTime.Today.ToApiDateString(),
                    VitalType = $"{(int)QsDbVitalTypeEnum.Steps}",
                    Value1 = "3020"
                },
                new QhApiVitalValueItem
                {
                    RecordDate = new DateTime(2022,10,10,10,10,0).ToApiDateString(),
                    VitalType = $"{(int)QsDbVitalTypeEnum.SleepingTime}",
                    Value1 = "480"
                },
                new QhApiVitalValueItem
                {
                    RecordDate = new DateTime(2022,10,10,10,10,0).ToApiDateString(),
                    VitalType = $"{(int)QsDbVitalTypeEnum.BloodPressure}",
                    Value1 = "120",
                    Value2 = "70"
                },
                new QhApiVitalValueItem
                {
                    RecordDate = new DateTime(2022,10,10,10,10,0).ToApiDateString(),
                    VitalType = $"{(int)QsDbVitalTypeEnum.BodyAge}",
                    Value1 = "40"
                },
                new QhApiVitalValueItem
                {
                    RecordDate = new DateTime(2022,10,10,10,10,0).ToApiDateString(),
                    VitalType = $"{(int)QsDbVitalTypeEnum.Pulse}",
                    Value1 = "80"
                },
                new QhApiVitalValueItem
                {
                    RecordDate = new DateTime(2022,10,10,10,10,0).ToApiDateString(),
                    VitalType = $"{(int)QsDbVitalTypeEnum.BloodSugar}",
                    Value1 = "40",
                    ConditionType = $"{(int)QsDbVitalConditionTypeEnum.Fasting}"
                },
                new QhApiVitalValueItem
                {
                    RecordDate = new DateTime(2022,10,10,10,10,0).ToApiDateString(),
                    VitalType = $"{(int)QsDbVitalTypeEnum.Glycohemoglobin}",
                    Value1 = "40"
                },
                new QhApiVitalValueItem
                {
                    RecordDate = new DateTime(2022,10,10,10,10,0).ToApiDateString(),
                    VitalType = $"{(int)QsDbVitalTypeEnum.BodyFatPercentage}",
                    Value1 = "16"
                },
                new QhApiVitalValueItem
                {
                    RecordDate = new DateTime(2022,10,10,10,10,0).ToApiDateString(),
                    VitalType = $"{(int)QsDbVitalTypeEnum.TotalBodyWater}",
                    Value1 = "40"
                },
                new QhApiVitalValueItem
                {
                    RecordDate = new DateTime(2022,10,10,10,10,0).ToApiDateString(),
                    VitalType = $"{(int)QsDbVitalTypeEnum.BloodOxygen}",
                    Value1 = "90"
                },
                new QhApiVitalValueItem
                {
                    RecordDate = new DateTime(2022,10,10,10,10,0).ToApiDateString(),
                    VitalType = $"{(int)QsDbVitalTypeEnum.BodyHeight}",
                    Value1 = "62.5"
                },
            };

            return args;
        }
    }
}
