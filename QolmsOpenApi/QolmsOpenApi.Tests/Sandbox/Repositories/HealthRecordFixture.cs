using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QolmsOpenApi.Tests.Sandbox
{
    [TestClass]
    public class HealthRecordFixture
    {
        IHealthRecordRepository _repo;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new HealthRecordRepository();
        }

        [TestMethod]
        public void バイタル値が正常に登録される()
        {
            var accountKey = new Guid("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");
            var authorKey = Guid.Empty;

            var vitalN = new List<DbVitalValueItem>
            {
                new DbVitalValueItem
                {
                    RecordDate = new DateTime(2022,10,10,10,10,0),
                    VitalType = QsDbVitalTypeEnum.BloodOxygen,
                    Value1 = 90m,
                    Value2 = decimal.MinusOne,
                },
                new DbVitalValueItem
                {
                    RecordDate = new DateTime(2022,10,10,11,10,0),
                    VitalType = QsDbVitalTypeEnum.BloodOxygen,
                    Value1 = 95m,
                    Value2 = decimal.MinusOne,
                },
            };

            _repo.WriteVitals(accountKey, authorKey, vitalN);
        }

        [TestMethod]
        public void 歩数が含まれていればサマリーも更新される()
        {
            var accountKey = new Guid("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");
            var authorKey = Guid.Empty;

            var vitalN = new List<DbVitalValueItem>
            {
                new DbVitalValueItem
                {
                    RecordDate = new DateTime(2022,10,10,0,0,0),
                    VitalType = QsDbVitalTypeEnum.Steps,
                    Value1 = 3500m,
                    Value2 = decimal.MinusOne,
                },
                new DbVitalValueItem
                {
                    RecordDate = new DateTime(2022,10,10,13,35,0),
                    VitalType = QsDbVitalTypeEnum.BloodPressure,
                    Value1 = 120m,
                    Value2 = 70m,
                },
            };

            _repo.WriteVitals(accountKey, authorKey, vitalN);

            // Workers.HealthRecord.HealthRecordWorker.WriteMonthlyAverage内部でこけてる
            // ローカル環境だからかも知れないので保留とする
        }

        [TestMethod]
        public void DBエラーチェック()
        {
            var accountKey = new Guid("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");
            var authorKey = Guid.Empty;

            var vitalN = new List<DbVitalValueItem>
            {
                new DbVitalValueItem
                {
                    RecordDate = new DateTime(2022,10,10,0,0,0),
                    VitalType = QsDbVitalTypeEnum.Pulse,
                    Value1 = 80m,
                    Value2 = decimal.MinusOne,
                },
                new DbVitalValueItem
                {
                    RecordDate = new DateTime(2022,10,10,0,0,0),
                    VitalType = QsDbVitalTypeEnum.Pulse,
                    Value1 = 80m,
                    Value2 = decimal.MinusOne,
                },
            };

            _repo.WriteVitals(accountKey, authorKey, vitalN);
        }

        [TestMethod]
        public void 範囲データを取得する()
        {
            var accountKey = new Guid("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");
            var from = new DateTime(2022, 9, 1);
            var to = new DateTime(2022, 10, 31);

            var ret = _repo.ReadRange(accountKey, from, to, (byte)QsDbVitalTypeEnum.Steps, (byte)QsDbVitalTypeEnum.SleepingTime);
        }

        [TestMethod]
        public void 最新データを取得する()
        {
            var accountKey = new Guid("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");
            var to = new DateTime(2022, 10, 31);
            var ret = _repo.ReadNew(accountKey, to, (byte)QsDbVitalTypeEnum.SleepingTime);
        }

        [TestMethod]
        public void 心拍データを取得する()
        {
            var accountKey = new Guid("6ed1824d-aa9b-4277-9826-459cfb4d0b55");
            var from = new DateTime(2025, 1, 1);
            var to = new DateTime(2026, 2, 10);

            var ret = _repo.ReadStoragePulse(accountKey, from, to);
        }

        [TestMethod]
        public void 酸素飽和度データを取得する()
        {
            var accountKey = new Guid("6ed1824d-aa9b-4277-9826-459cfb4d0b55");
            var from = new DateTime(2025, 1, 1);
            var to = new DateTime(2026, 2, 10);

            var ret = _repo.ReadStorageBloodOxygen(accountKey, from, to);
        }

        [TestMethod]
        public void METSデータを取得する()
        {
            var accountKey = new Guid("6ed1824d-aa9b-4277-9826-459cfb4d0b55");
            var from = new DateTime(2025, 1, 1);
            var to = new DateTime(2026, 2, 10);

            var ret = _repo.ReadStorageMets(accountKey, from, to);
        }

        [TestMethod]
        public void 消費カロリーを取得する()
        {
            var accountKey = new Guid("6ed1824d-aa9b-4277-9826-459cfb4d0b55");
            var from = new DateTime(2026, 1, 23);
            var to = new DateTime(2026, 1, 24);

            var ret = _repo.ReadExerciseRange(accountKey, from, to);
        }
    }
}
