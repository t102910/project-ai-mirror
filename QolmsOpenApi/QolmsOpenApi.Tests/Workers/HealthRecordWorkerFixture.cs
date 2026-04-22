using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class HealthRecordWorkerFixture
    {
        HealthRecordWorker _worker;
        Mock<IHealthRecordRepository> _repo;
        Mock<IHealthRecordValidator> _validator;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new Mock<IHealthRecordRepository>();
            _validator = new Mock<IHealthRecordValidator>();
            _worker = new HealthRecordWorker(_repo.Object, _validator.Object);
        }

        [TestMethod]
        public void 引数チェックにひっかかると例外が発生する()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                // 引数nullは例外
                _worker.Import(null);
            });

            var args = new QoHealthRecordImportApiArgs();

            Assert.ThrowsException<ArgumentException>(() =>
            {
                // ActorKeyがない場合は例外
                _worker.Import(args);
            });

            args.ActorKey = "hoge";


            Assert.ThrowsException<ArgumentException>(() =>
            {
                // バイタルリストがnullの場合は例外
                _worker.Import(args);
            });

            args.VitalValueN = new List<QhApiVitalValueItem>();
            Assert.ThrowsException<ArgumentException>(() =>
            {
                // バイタルリストが0件の場合は例外
                _worker.Import(args);
            });

            args.VitalValueN.Add(new QhApiVitalValueItem());

            args.Executor = null;
            Assert.ThrowsException<ArgumentException>(() =>
            {
                // Executor未設定は例外
                _worker.Import(args);
            });
        }

        [TestMethod]
        public void バイタル値チェックでNGであればNG()
        {
            var args = GetBaseArgs();

            // バリデーションでこける設定
            _validator.Setup(m => m.Validate(args.VitalValueN)).Returns((false, "error"));

            var result = _worker.Import(args);

            // 失敗
            result.IsSuccess.Is(bool.FalseString);
            result.Result.Detail.Contains("error");

            // 書き込み処理まで到達しなかった。
            _repo.Verify(m => m.WriteVitals(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<List<DbVitalValueItem>>()), Times.Never);
        }

        [TestMethod]
        public void バイタル値変換後にバイタルリストが0件になれば処理対象0で正常終了する()
        {
            var args = new QoHealthRecordImportApiArgs
            {
                ActorKey = "6ed1824d-aa9b-4277-9826-459cfb4d0b55",
                VitalValueN = new List<QhApiVitalValueItem>
                {
                    new QhApiVitalValueItem
                    {
                        RecordDate = new DateTime(2022,10,10,12,10,0).ToApiDateString(),
                        VitalType = $"{(int)QsDbVitalTypeEnum.None}",
                        Value1 = "90"
                    },
                }
            };

            // バリデーション通過設定
            _validator.Setup(m => m.Validate(args.VitalValueN)).Returns((true, null));

            var result = _worker.Import(args);

            // 処理0件 正常終了
            result.IsSuccess.Is(bool.TrueString);

            // 書き込み処理まで到達しなかった。
            _repo.Verify(m => m.WriteVitals(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<List<DbVitalValueItem>>()), Times.Never);
        }

        [TestMethod]
        public void DB登録処理で失敗するとエラーとなる()
        {
            var args = GetBaseArgs();

            // バリデーション通過設定
            _validator.Setup(m => m.Validate(args.VitalValueN)).Returns((true, null));

            _repo.Setup(m => m.WriteVitals(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<List<DbVitalValueItem>>())).Throws(new Exception());

            var result = _worker.Import(args);

            // 失敗
            result.IsSuccess.Is(bool.FalseString);
            result.Result.Detail.Contains("バイタル値のDB登録に失敗しました。");
        }

        [TestMethod]
        public void EXERCISEEVENT2へのDB登録処理で失敗するとエラーとなる()
        {
            var args = GetBaseArgs();

            // バリデーション通過設定
            _validator.Setup(m => m.Validate(args.VitalValueN)).Returns((true, null));

            _repo.Setup(m => m.WriteExercise(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<List<DbVitalValueItem>>())).Throws(new Exception());

            var result = _worker.Import(args);

            // 失敗
            result.IsSuccess.Is(bool.FalseString);
            result.Result.Detail.Contains("運動値のDB登録に失敗しました。");
        }

        [TestMethod]
        public void AzureStorage登録処理で失敗するとエラーとなる()
        {
            var args = GetBaseArgs();

            // バリデーション通過設定
            _validator.Setup(m => m.Validate(args.VitalValueN)).Returns((true, null));

            _repo.Setup(m => m.WriteStorage(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<List<DbVitalValueItem>>())).Throws(new Exception());

            var result = _worker.Import(args);

            // 失敗
            result.IsSuccess.Is(bool.FalseString);
            result.Result.Detail.Contains("AzureStorageへの登録に失敗しました。");
        }

        [TestMethod]
        public void バイタルリストが正常に変換されDB登録処理が正常に実行される()
        {
            var args = GetBaseArgs();

            // バリデーション通過設定
            _validator.Setup(m => m.Validate(args.VitalValueN)).Returns((true, null));

            _repo.Setup(m => m.WriteVitals(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<List<DbVitalValueItem>>())).Callback<Guid, Guid, List<DbVitalValueItem>>((accountKey, authorKey, list) =>
            {
                 accountKey.ToString("N").Is(args.ActorKey);
                 // AuthorKeyは読み取り専用プロパティのため、Executorから自動設定される想定
                 authorKey.ToString("N").Is(args.Executor);

                 // 日付順にソートされ適切にデータが変換されている
                 list[0].RecordDate.Is(new DateTime(2022, 10, 9, 10, 10, 0));
                 list[0].VitalType.Is(QsDbVitalTypeEnum.BodyWeight);
                 list[0].Value1.Is(60.52m);
                 list[0].Value2.Is(decimal.MinusOne);

                 // 体重は体重と身長に分割される
                 list[1].RecordDate.Is(new DateTime(2022, 10, 9, 10, 10, 0));
                 list[1].VitalType.Is(QsDbVitalTypeEnum.BodyHeight);
                 list[1].Value1.Is(169.3m);
                 list[1].Value2.Is(decimal.MinusOne);

                 list[2].RecordDate.Is(new DateTime(2022, 10, 10, 0, 0, 0));
                 list[2].VitalType.Is(QsDbVitalTypeEnum.Steps);
                 list[2].Value1.Is(3020m);

                 // BloodPressure (BloodOxygenはStorage行きのため除外)
                 list[3].RecordDate.Is(new DateTime(2022, 10, 10, 13, 10, 0));
                 list[3].VitalType.Is(QsDbVitalTypeEnum.BloodPressure);
                 list[3].Value1.Is(120m);
                 list[3].Value2.Is(70m);

                 list[4].RecordDate.Is(new DateTime(2022, 10, 11, 13, 10, 0));
                 list[4].VitalType.Is(QsDbVitalTypeEnum.SleepingTime);
                 list[4].Value1.Is(480m);

                 // 秒以下は切り捨てられ、また分単位での重複は後のデータが採用される
                 list[5].RecordDate.Is(new DateTime(2022, 10, 11, 15, 10, 0));
                 list[5].VitalType.Is(QsDbVitalTypeEnum.BloodSugar);
                 list[5].Value1.Is(50m);
                 list[5].ConditionType.Is((byte)QsDbVitalConditionTypeEnum.Fasting);

                 // 体重は体重と身長に分割されるが体重が0なので身長は-1で登録される
                 // （通常はチェックで弾かれるのでこのデータは登録されない）
                 list[6].RecordDate.Is(new DateTime(2022, 10, 14, 10, 10, 0));
                 list[6].VitalType.Is(QsDbVitalTypeEnum.BodyWeight);
                 list[6].Value1.Is(0m);

                 list[7].RecordDate.Is(new DateTime(2022, 10, 14, 10, 10, 0));
                 list[7].VitalType.Is(QsDbVitalTypeEnum.BodyHeight);
                 list[7].Value1.Is(decimal.MinusOne);

                 // Pulse, Mets, CalorieBurnはStorageまたはExercise行きのため、このリストには含まれない
            });

            // Exercise（運動データ）の検証
            _repo.Setup(m => m.WriteExercise(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<List<DbVitalValueItem>>())).Callback<Guid, Guid, List<DbVitalValueItem>>((accountKey, authorKey, list) =>
            {
                accountKey.ToString("N").Is(args.ActorKey);
                authorKey.ToString("N").Is(args.Executor);

                // CalorieBurnのみ
                list.Count.Is(1);
                list[0].RecordDate.Is(new DateTime(2022, 10, 16, 12, 10, 0));
                list[0].VitalType.Is(QsDbVitalTypeEnum.CalorieBurn);
                list[0].Value1.Is(10m);
            });

            // Storage（Azure Table Storage）の検証
            _repo.Setup(m => m.WriteStorage(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<List<DbVitalValueItem>>())).Callback<Guid, Guid, List<DbVitalValueItem>>((accountKey, authorKey, list) =>
            {
                accountKey.ToString("N").Is(args.ActorKey);
                authorKey.ToString("N").Is(args.Executor);

                // BloodOxygen, Pulse, Metsの3件（日付順）
                list.Count.Is(3);
                
                list[0].RecordDate.Is(new DateTime(2022, 10, 10, 12, 10, 0));
                list[0].VitalType.Is(QsDbVitalTypeEnum.BloodOxygen);
                list[0].Value1.Is(90m);

                list[1].RecordDate.Is(new DateTime(2022, 10, 15, 12, 10, 0));
                list[1].VitalType.Is(QsDbVitalTypeEnum.Pulse);
                list[1].Value1.Is(100m);

                list[2].RecordDate.Is(new DateTime(2022, 10, 15, 12, 10, 0));
                list[2].VitalType.Is(QsDbVitalTypeEnum.Mets);
                list[2].Value1.Is(3m);
            });

            var result = _worker.Import(args);

            // 正常終了
            result.IsSuccess.Is(bool.TrueString);
        }

        QoHealthRecordImportApiArgs GetBaseArgs()
        {
            return new QoHealthRecordImportApiArgs
            {
                ActorKey = "6ed1824daa9b42779826459cfb4d0b55",
                Executor = "6ed1824daa9b42779826459cfb4d0b55",
                VitalValueN = new List<QhApiVitalValueItem>
                {
                    new QhApiVitalValueItem
                    {
                        RecordDate = new DateTime(2022,10,10,0,0,0).ToApiDateString(),
                        VitalType = $"{(int)QsDbVitalTypeEnum.Steps}",
                        Value1 = "3020"
                    },
                    new QhApiVitalValueItem
                    {
                        RecordDate = new DateTime(2022,10,9,10,10,0).ToApiDateString(),
                        VitalType = $"{(int)QsDbVitalTypeEnum.BodyWeight}",
                        Value1 = "60.52",
                        Value2 = "169.3"
                    },
                    new QhApiVitalValueItem
                    {
                        RecordDate = new DateTime(2022,10,14,10,10,0).ToApiDateString(),
                        VitalType = $"{(int)QsDbVitalTypeEnum.BodyWeight}",
                        Value1 = "0",
                        Value2 = "169.3"
                    },
                    new QhApiVitalValueItem
                    {
                        RecordDate = new DateTime(2022,10,11,13,10,0).ToApiDateString(),
                        VitalType = $"{(int)QsDbVitalTypeEnum.SleepingTime}",
                        Value1 = "480"
                    },
                    new QhApiVitalValueItem
                    {
                        RecordDate = new DateTime(2022,10,10,13,10,0).ToApiDateString(),
                        VitalType = $"{(int)QsDbVitalTypeEnum.BloodPressure}",
                        Value1 = "120",
                        Value2 = "70"
                    },                    
                    new QhApiVitalValueItem
                    {
                        RecordDate = new DateTime(2022,10,10,12,10,0).ToApiDateString(),
                        VitalType = $"{(int)QsDbVitalTypeEnum.BloodOxygen}",
                        Value1 = "90"
                    },
                    new QhApiVitalValueItem
                    {
                        RecordDate = new DateTime(2022,10,10,12,10,0).ToApiDateString(),
                        VitalType = $"{(int)QsDbVitalTypeEnum.None}",
                        Value1 = "90"
                    },
                    new QhApiVitalValueItem
                    {
                        RecordDate = new DateTime(2022,10,11,15,10,15).ToApiDateString(),
                        VitalType = $"{(int)QsDbVitalTypeEnum.BloodSugar}",
                        Value1 = "40",
                        ConditionType = $"{(int)QsDbVitalConditionTypeEnum.Fasting}"
                    },
                    new QhApiVitalValueItem
                    {
                        RecordDate = new DateTime(2022,10,11,15,10,30).ToApiDateString(),
                        VitalType = $"{(int)QsDbVitalTypeEnum.BloodSugar}",
                        Value1 = "50",
                        ConditionType = $"{(int)QsDbVitalConditionTypeEnum.Fasting}"
                    },
                    new QhApiVitalValueItem
                    {
                        RecordDate = new DateTime(2022,10,15,12,10,0).ToApiDateString(),
                        VitalType = $"{(int)QsDbVitalTypeEnum.Pulse}",
                        Value1 = "100"
                    },
                    new QhApiVitalValueItem
                    {
                        RecordDate = new DateTime(2022,10,15,12,10,0).ToApiDateString(),
                        VitalType = $"{(int)QsDbVitalTypeEnum.Mets}",
                        Value1 = "3"
                    },
                    new QhApiVitalValueItem
                    {
                        RecordDate = new DateTime(2022,10,16,12,10,0).ToApiDateString(),
                        VitalType = $"{(int)QsDbVitalTypeEnum.CalorieBurn}",
                        Value1 = "10"
                    },
                }
            };
        }
    }
}
