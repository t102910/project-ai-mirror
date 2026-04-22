using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class HealthRecordReadWorkerFixture
    {
        HealthRecordReadWorker _worker;
        Mock<IHealthRecordRepository> _healthRecordRepo;
        Guid _accountKey = Guid.NewGuid();

        [TestInitialize]
        public void Initialize()
        {
            _healthRecordRepo = new Mock<IHealthRecordRepository>();
            _worker = new HealthRecordReadWorker(_healthRecordRepo.Object);
        }

        [TestMethod]
        public void アカウントキーが不正でエラーとなる()
        {
            var args = GetValidArgs();
            args.ActorKey = "invalidGuid";

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // ArgumentError
            results.Result.Detail.Contains("ActorKey").IsTrue();
        }

        [TestMethod]
        public void FromDateがToDateより後の場合エラーとなる()
        {
            var args = GetValidArgs();
            args.FromDate = "2026/02/05 00:00:00";
            args.ToDate = "2026/02/01 00:00:00";

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // ArgumentError
            results.Result.Detail.Contains("FromDateはToDate以前の日時を指定してください").IsTrue();
        }

        [TestMethod]
        public void 期間が7日間を超える場合エラーとなる()
        {
            var args = GetValidArgs();
            args.FromDate = "2026/01/29 00:00:00";
            args.ToDate = "2026/02/05 23:59:59";

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // ArgumentError
            results.Result.Detail.Contains("取得期間は最大7日間です").IsTrue();
        }

        [TestMethod]
        public void DB単独のデータを正常に取得できる()
        {
            var args = GetValidArgs();
            args.VitalTypes = new List<byte> { 2, 5, 6 }; // BodyWeight, BloodPressure, BloodSugar

            var dbRecords = new List<QH_HEALTHRECORD_DAT>
            {
                CreateDbRecord(new DateTime(2026, 2, 1, 8, 0, 0), 2, 65.5m, 170.0m),
                CreateDbRecord(new DateTime(2026, 2, 2, 9, 0, 0), 5, 120m, 70m),
                CreateDbRecord(new DateTime(2026, 2, 3, 10, 0, 0), 6, 100m, 0m),
            };

            var fromDate = new DateTime(2026, 2, 1, 0, 0, 0);
            var toDate = new DateTime(2026, 2, 3, 23, 59, 59);

            _healthRecordRepo.Setup(m => m.ReadRange(
                It.Is<Guid>(g => g == _accountKey),
                It.Is<DateTime>(d => d == fromDate),
                It.Is<DateTime>(d => d == toDate),
                It.Is<byte[]>(b => b.Length == 3 && b[0] == 2 && b[1] == 5 && b[2] == 6)))
                .Returns(dbRecords);

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200"); // Success
            results.VitalDataN.Count.Is(3);

            // 日付昇順でソートされていることを確認
            results.VitalDataN[0].RecordDate.Is("2026/02/01 08:00:00");
            results.VitalDataN[0].VitalType.Is("2");
            results.VitalDataN[0].Value1.Is("65.5");
            results.VitalDataN[0].Value2.Is("170");

            results.VitalDataN[1].RecordDate.Is("2026/02/02 09:00:00");
            results.VitalDataN[1].VitalType.Is("5");
            results.VitalDataN[1].Value1.Is("120");
            results.VitalDataN[1].Value2.Is("70");

            results.VitalDataN[2].RecordDate.Is("2026/02/03 10:00:00");
            results.VitalDataN[2].VitalType.Is("6");
            results.VitalDataN[2].Value1.Is("100");
            results.VitalDataN[2].Value2.Is(string.Empty);
        }

        [TestMethod]
        public void Storage単独のデータを正常に取得できる()
        {
            var args = GetValidArgs();
            args.VitalTypes = new List<byte> { 8, 19, 20 }; // Pulse, BloodOxygen, Mets

            var pulseData = new List<DbVitalValueItem>
            {
                CreateVitalItem(new DateTime(2026, 2, 1, 8, 0, 0), QsDbVitalTypeEnum.Pulse, 72m),
            };

            var bloodOxygenData = new List<DbVitalValueItem>
            {
                CreateVitalItem(new DateTime(2026, 2, 2, 9, 0, 0), QsDbVitalTypeEnum.BloodOxygen, 98m),
            };

            var metsData = new List<DbVitalValueItem>
            {
                CreateVitalItem(new DateTime(2026, 2, 3, 10, 0, 0), QsDbVitalTypeEnum.Mets, 3.5m),
            };

            var fromDate = new DateTime(2026, 2, 1, 0, 0, 0);
            var toDate = new DateTime(2026, 2, 3, 23, 59, 59);

            _healthRecordRepo.Setup(m => m.ReadRange(
                It.IsAny<Guid>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<byte[]>()))
                .Returns(new List<QH_HEALTHRECORD_DAT>());
            _healthRecordRepo.Setup(m => m.ReadStoragePulse(_accountKey, fromDate, toDate))
                .Returns(pulseData);
            _healthRecordRepo.Setup(m => m.ReadStorageBloodOxygen(_accountKey, fromDate, toDate))
                .Returns(bloodOxygenData);
            _healthRecordRepo.Setup(m => m.ReadStorageMets(_accountKey, fromDate, toDate))
                .Returns(metsData);

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200"); // Success
            results.VitalDataN.Count.Is(3);

            // 日付昇順でソートされていることを確認
            results.VitalDataN[0].RecordDate.Is("2026/02/01 08:00:00");
            results.VitalDataN[0].VitalType.Is("8");
            results.VitalDataN[0].Value1.Is("72");

            results.VitalDataN[1].RecordDate.Is("2026/02/02 09:00:00");
            results.VitalDataN[1].VitalType.Is("19");
            results.VitalDataN[1].Value1.Is("98");

            results.VitalDataN[2].RecordDate.Is("2026/02/03 10:00:00");
            results.VitalDataN[2].VitalType.Is("20");
            results.VitalDataN[2].Value1.Is("3.5");
        }

        [TestMethod]
        public void DBとStorageのデータを混在で正常に取得できる()
        {
            var args = GetValidArgs();
            args.VitalTypes = new List<byte> { 2, 8, 19 }; // BodyWeight, Pulse, BloodOxygen

            var dbRecords = new List<QH_HEALTHRECORD_DAT>
            {
                CreateDbRecord(new DateTime(2026, 2, 1, 8, 0, 0), 2, 65.5m, 170.0m),
                CreateDbRecord(new DateTime(2026, 2, 3, 10, 0, 0), 2, 65.8m, 170.0m),
            };

            var pulseData = new List<DbVitalValueItem>
            {
                CreateVitalItem(new DateTime(2026, 2, 1, 9, 0, 0), QsDbVitalTypeEnum.Pulse, 72m),
                CreateVitalItem(new DateTime(2026, 2, 2, 8, 30, 0), QsDbVitalTypeEnum.Pulse, 75m),
            };

            var bloodOxygenData = new List<DbVitalValueItem>
            {
                CreateVitalItem(new DateTime(2026, 2, 2, 9, 0, 0), QsDbVitalTypeEnum.BloodOxygen, 98m),
            };

            var fromDate = new DateTime(2026, 2, 1, 0, 0, 0);
            var toDate = new DateTime(2026, 2, 3, 23, 59, 59);

            _healthRecordRepo.Setup(m => m.ReadRange(
                It.Is<Guid>(g => g == _accountKey),
                It.Is<DateTime>(d => d == fromDate),
                It.Is<DateTime>(d => d == toDate),
                It.Is<byte[]>(b => b.Length == 1 && b[0] == 2)))
                .Returns(dbRecords);
            _healthRecordRepo.Setup(m => m.ReadStoragePulse(_accountKey, fromDate, toDate))
                .Returns(pulseData);
            _healthRecordRepo.Setup(m => m.ReadStorageBloodOxygen(_accountKey, fromDate, toDate))
                .Returns(bloodOxygenData);

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200"); // Success
            results.VitalDataN.Count.Is(5);

            // 日付昇順でソートされていることを確認
            results.VitalDataN[0].RecordDate.Is("2026/02/01 08:00:00");
            results.VitalDataN[0].VitalType.Is("2");

            results.VitalDataN[1].RecordDate.Is("2026/02/01 09:00:00");
            results.VitalDataN[1].VitalType.Is("8");

            results.VitalDataN[2].RecordDate.Is("2026/02/02 08:30:00");
            results.VitalDataN[2].VitalType.Is("8");

            results.VitalDataN[3].RecordDate.Is("2026/02/02 09:00:00");
            results.VitalDataN[3].VitalType.Is("19");

            results.VitalDataN[4].RecordDate.Is("2026/02/03 10:00:00");
            results.VitalDataN[4].VitalType.Is("2");
        }

        [TestMethod]
        public void 最大期間7日間で正常に取得できる()
        {
            var args = GetValidArgs();
            args.FromDate = "2026/01/30 00:00:00";
            args.ToDate = "2026/02/05 23:59:59";
            args.VitalTypes = new List<byte> { 2 }; // BodyWeight

            var dbRecords = new List<QH_HEALTHRECORD_DAT>
            {
                CreateDbRecord(new DateTime(2026, 1, 30, 8, 0, 0), 2, 65.5m, 170.0m),
                CreateDbRecord(new DateTime(2026, 2, 5, 8, 0, 0), 2, 66.0m, 170.0m),
            };

            var fromDate = new DateTime(2026, 1, 30, 0, 0, 0);
            var toDate = new DateTime(2026, 2, 5, 23, 59, 59);

            _healthRecordRepo.Setup(m => m.ReadRange(
                It.Is<Guid>(g => g == _accountKey),
                It.Is<DateTime>(d => d == fromDate),
                It.Is<DateTime>(d => d == toDate),
                It.Is<byte[]>(b => b.Length == 1 && b[0] == 2)))
                .Returns(dbRecords);

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200"); // Success
            results.VitalDataN.Count.Is(2);
        }

        [TestMethod]
        public void Storage接続エラーでPulseデータ取得失敗()
        {
            var args = GetValidArgs();
            args.VitalTypes = new List<byte> { 8 }; // Pulse

            var fromDate = new DateTime(2026, 2, 1, 0, 0, 0);
            var toDate = new DateTime(2026, 2, 3, 23, 59, 59);

            _healthRecordRepo.Setup(m => m.ReadRange(
                It.IsAny<Guid>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<byte[]>()))
                .Returns(new List<QH_HEALTHRECORD_DAT>());
            _healthRecordRepo.Setup(m => m.ReadStoragePulse(_accountKey, fromDate, toDate))
                .Throws(new Exception("Storage connection error"));

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500"); // DatabaseError
            results.Result.Detail.Contains("Pulseデータの取得に失敗しました").IsTrue();
        }

        [TestMethod]
        public void Storage接続エラーでMetsデータ取得失敗()
        {
            var args = GetValidArgs();
            args.VitalTypes = new List<byte> { 20 }; // Mets

            var fromDate = new DateTime(2026, 2, 1, 0, 0, 0);
            var toDate = new DateTime(2026, 2, 3, 23, 59, 59);

            _healthRecordRepo.Setup(m => m.ReadRange(
                It.IsAny<Guid>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<byte[]>()))
                .Returns(new List<QH_HEALTHRECORD_DAT>());
            _healthRecordRepo.Setup(m => m.ReadStorageMets(_accountKey, fromDate, toDate))
                .Throws(new Exception("Storage connection error"));

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500"); // DatabaseError
            results.Result.Detail.Contains("Metsデータの取得に失敗しました").IsTrue();
        }

        [TestMethod]
        public void Storage接続エラーでBloodOxygenデータ取得失敗()
        {
            var args = GetValidArgs();
            args.VitalTypes = new List<byte> { 19 }; // BloodOxygen

            var fromDate = new DateTime(2026, 2, 1, 0, 0, 0);
            var toDate = new DateTime(2026, 2, 3, 23, 59, 59);

            _healthRecordRepo.Setup(m => m.ReadRange(
                It.IsAny<Guid>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<byte[]>()))
                .Returns(new List<QH_HEALTHRECORD_DAT>());
            _healthRecordRepo.Setup(m => m.ReadStorageBloodOxygen(_accountKey, fromDate, toDate))
                .Throws(new Exception("Storage connection error"));

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500"); // DatabaseError
            results.Result.Detail.Contains("BloodOxygenデータの取得に失敗しました").IsTrue();
        }

        [TestMethod]
        public void データが0件の場合は空のリストが返る()
        {
            var args = GetValidArgs();
            args.VitalTypes = new List<byte> { 2 }; // BodyWeight

            var fromDate = new DateTime(2026, 2, 1, 0, 0, 0);
            var toDate = new DateTime(2026, 2, 3, 23, 59, 59);

            _healthRecordRepo.Setup(m => m.ReadRange(
                It.Is<Guid>(g => g == _accountKey),
                It.Is<DateTime>(d => d == fromDate),
                It.Is<DateTime>(d => d == toDate),
                It.Is<byte[]>(b => b.Length == 1 && b[0] == 2)))
                .Returns(new List<QH_HEALTHRECORD_DAT>());

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200"); // Success
            results.VitalDataN.Count.Is(0);
        }

        [TestMethod]
        public void VitalTypesが未設定の場合は全タイプが動的に取得される()
        {
            var args = GetValidArgs();
            args.VitalTypes = null;
            args.FromDate = "2026/02/01 00:00:00";
            args.ToDate = "2026/02/01 23:59:59";

            var dbRecords = new List<QH_HEALTHRECORD_DAT>
            {
                CreateDbRecord(new DateTime(2026, 2, 1, 8, 0, 0), 2, 65.5m, 170.0m),
            };

            var pulseData = new List<DbVitalValueItem>
            {
                CreateVitalItem(new DateTime(2026, 2, 1, 9, 0, 0), QsDbVitalTypeEnum.Pulse, 72m),
            };

            var metsData = new List<DbVitalValueItem>
            {
                CreateVitalItem(new DateTime(2026, 2, 1, 10, 0, 0), QsDbVitalTypeEnum.Mets, 3.5m),
            };

            var bloodOxygenData = new List<DbVitalValueItem>
            {
                CreateVitalItem(new DateTime(2026, 2, 1, 11, 0, 0), QsDbVitalTypeEnum.BloodOxygen, 98m),
            };

            var fromDate = new DateTime(2026, 2, 1, 0, 0, 0);
            var toDate = new DateTime(2026, 2, 1, 23, 59, 59);

            // DB系バイタルの取得呼び出しを期待（Pulse=8, Mets=20, BloodOxygen=19以外の全て）
            _healthRecordRepo.Setup(m => m.ReadRange(
                It.Is<Guid>(g => g == _accountKey),
                It.Is<DateTime>(d => d == fromDate),
                It.Is<DateTime>(d => d == toDate),
                It.Is<byte[]>(b => b.Length > 0)))
                .Returns(dbRecords);

            // Storage系バイタルの取得呼び出しを期待
            _healthRecordRepo.Setup(m => m.ReadStoragePulse(_accountKey, fromDate, toDate))
                .Returns(pulseData);
            _healthRecordRepo.Setup(m => m.ReadStorageMets(_accountKey, fromDate, toDate))
                .Returns(metsData);
            _healthRecordRepo.Setup(m => m.ReadStorageBloodOxygen(_accountKey, fromDate, toDate))
                .Returns(bloodOxygenData);
            _healthRecordRepo.Setup(m => m.ReadExerciseRange(_accountKey, fromDate, toDate))
                .Returns(new List<DbVitalValueItem>
                {
                    CreateExerciseRecord(new DateTime(2026, 2, 1, 12, 0, 0), 300m),
                });

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200"); // Success
            results.VitalDataN.Count.Is(5);

            // Storage系3種類とDB系が取得されていることを確認
            _healthRecordRepo.Verify(m => m.ReadRange(
                It.IsAny<Guid>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<byte[]>()), Times.Once);
            _healthRecordRepo.Verify(m => m.ReadStoragePulse(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
            _healthRecordRepo.Verify(m => m.ReadStorageMets(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
            _healthRecordRepo.Verify(m => m.ReadStorageBloodOxygen(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
            _healthRecordRepo.Verify(m => m.ReadExerciseRange(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
        }

        [TestMethod]
        public void ToDateが未設定の場合は現在時刻が使用される()
        {
            var args = GetValidArgs();
            args.FromDate = DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd HH:mm:ss");
            args.ToDate = null;
            args.VitalTypes = new List<byte> { 2 };

            var dbRecords = new List<QH_HEALTHRECORD_DAT>
            {
                CreateDbRecord(DateTime.Now.AddHours(-1), 2, 65.5m, 170.0m),
            };

            _healthRecordRepo.Setup(m => m.ReadRange(
                It.IsAny<Guid>(),
                It.IsAny<DateTime>(),
                It.Is<DateTime>(d => (DateTime.Now - d).TotalSeconds < 5), // 現在時刻から5秒以内
                It.IsAny<byte[]>()))
                .Returns(dbRecords);

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200"); // Success
        }

        [TestMethod]
        public void ToDateが空文字の場合も現在時刻が使用される()
        {
            var args = GetValidArgs();
            args.FromDate = DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd HH:mm:ss");
            args.ToDate = "";
            args.VitalTypes = new List<byte> { 2 };

            var dbRecords = new List<QH_HEALTHRECORD_DAT>
            {
                CreateDbRecord(DateTime.Now.AddHours(-1), 2, 65.5m, 170.0m),
            };

            _healthRecordRepo.Setup(m => m.ReadRange(
                It.IsAny<Guid>(),
                It.IsAny<DateTime>(),
                It.Is<DateTime>(d => (DateTime.Now - d).TotalSeconds < 5), // 現在時刻から5秒以内
                It.IsAny<byte[]>()))
                .Returns(dbRecords);

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200"); // Success
        }

        [TestMethod]
        public void VitalTypesとToDate両方未設定でも正常動作する()
        {
            var args = GetValidArgs();
            args.FromDate = DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd HH:mm:ss");
            args.ToDate = null;
            args.VitalTypes = null;

            var dbRecords = new List<QH_HEALTHRECORD_DAT>
            {
                CreateDbRecord(DateTime.Now.AddHours(-1), 2, 65.5m, 170.0m),
            };

            var pulseData = new List<DbVitalValueItem>
            {
                CreateVitalItem(DateTime.Now.AddHours(-2), QsDbVitalTypeEnum.Pulse, 72m),
            };

            var metsData = new List<DbVitalValueItem>();
            var bloodOxygenData = new List<DbVitalValueItem>();

            _healthRecordRepo.Setup(m => m.ReadRange(
                It.IsAny<Guid>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<byte[]>()))
                .Returns(dbRecords);

            _healthRecordRepo.Setup(m => m.ReadStoragePulse(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(pulseData);
            _healthRecordRepo.Setup(m => m.ReadStorageMets(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(metsData);
            _healthRecordRepo.Setup(m => m.ReadStorageBloodOxygen(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(bloodOxygenData);
            _healthRecordRepo.Setup(m => m.ReadExerciseRange(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(new List<DbVitalValueItem>());

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200"); // Success

            // 全てのデータソースが呼び出されたことを確認
            _healthRecordRepo.Verify(m => m.ReadRange(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<byte[]>()), Times.Once);
            _healthRecordRepo.Verify(m => m.ReadStoragePulse(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
            _healthRecordRepo.Verify(m => m.ReadStorageMets(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
            _healthRecordRepo.Verify(m => m.ReadStorageBloodOxygen(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
            _healthRecordRepo.Verify(m => m.ReadExerciseRange(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
        }

        [TestMethod]
        public void CalorieBurnデータを正常に取得できる()
        {
            var args = GetValidArgs();
            args.VitalTypes = new List<byte> { 21 }; // CalorieBurn

            var calorieBurnData = new List<DbVitalValueItem>
            {
                CreateExerciseRecord(new DateTime(2026, 2, 1, 8, 0, 0), 250m),
                CreateExerciseRecord(new DateTime(2026, 2, 2, 9, 0, 0), 300m),
            };

            var fromDate = new DateTime(2026, 2, 1, 0, 0, 0);
            var toDate = new DateTime(2026, 2, 3, 23, 59, 59);

            _healthRecordRepo.Setup(m => m.ReadExerciseRange(_accountKey, fromDate, toDate))
                .Returns(calorieBurnData);

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200"); // Success
            results.VitalDataN.Count.Is(2);

            // 日付昇順でソートされていることを確認
            results.VitalDataN[0].RecordDate.Is("2026/02/01 08:00:00");
            results.VitalDataN[0].VitalType.Is("21");
            results.VitalDataN[0].Value1.Is("250");

            results.VitalDataN[1].RecordDate.Is("2026/02/02 09:00:00");
            results.VitalDataN[1].VitalType.Is("21");
            results.VitalDataN[1].Value1.Is("300");

            _healthRecordRepo.Verify(m => m.ReadExerciseRange(_accountKey, fromDate, toDate), Times.Once);
        }

        [TestMethod]
        public void CalorieBurnとDBデータを混在で正常に取得できる()
        {
            var args = GetValidArgs();
            args.VitalTypes = new List<byte> { 2, 21 }; // BodyWeight, CalorieBurn

            var dbRecords = new List<QH_HEALTHRECORD_DAT>
            {
                CreateDbRecord(new DateTime(2026, 2, 2, 10, 0, 0), 2, 65.5m, 170.0m),
            };

            var calorieBurnData = new List<DbVitalValueItem>
            {
                CreateExerciseRecord(new DateTime(2026, 2, 1, 8, 0, 0), 200m),
                CreateExerciseRecord(new DateTime(2026, 2, 3, 9, 0, 0), 350m),
            };

            var fromDate = new DateTime(2026, 2, 1, 0, 0, 0);
            var toDate = new DateTime(2026, 2, 3, 23, 59, 59);

            _healthRecordRepo.Setup(m => m.ReadRange(
                It.Is<Guid>(g => g == _accountKey),
                It.Is<DateTime>(d => d == fromDate),
                It.Is<DateTime>(d => d == toDate),
                It.Is<byte[]>(b => b.Length == 1 && b[0] == 2)))
                .Returns(dbRecords);
            _healthRecordRepo.Setup(m => m.ReadExerciseRange(_accountKey, fromDate, toDate))
                .Returns(calorieBurnData);

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200"); // Success
            results.VitalDataN.Count.Is(3);

            // 日付昇順でソートされていることを確認
            results.VitalDataN[0].RecordDate.Is("2026/02/01 08:00:00");
            results.VitalDataN[0].VitalType.Is("21");

            results.VitalDataN[1].RecordDate.Is("2026/02/02 10:00:00");
            results.VitalDataN[1].VitalType.Is("2");

            results.VitalDataN[2].RecordDate.Is("2026/02/03 09:00:00");
            results.VitalDataN[2].VitalType.Is("21");
        }

        [TestMethod]
        public void Storage接続エラーでCalorieBurnデータ取得失敗()
        {
            var args = GetValidArgs();
            args.VitalTypes = new List<byte> { 21 }; // CalorieBurn

            var fromDate = new DateTime(2026, 2, 1, 0, 0, 0);
            var toDate = new DateTime(2026, 2, 3, 23, 59, 59);

            _healthRecordRepo.Setup(m => m.ReadExerciseRange(_accountKey, fromDate, toDate))
                .Throws(new Exception("DB connection error"));

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500"); // DatabaseError
            results.Result.Detail.Contains("CalorieBurnデータの取得に失敗しました").IsTrue();
        }

        // ヘルパーメソッド

        QoHealthRecordReadApiArgs GetValidArgs()
        {
            return new QoHealthRecordReadApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "HealthDiary",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.HealthDiaryiOSApp}",
                FromDate = "2026/02/01 00:00:00",
                ToDate = "2026/02/03 23:59:59",
                VitalTypes = new List<byte> { 2, 5 }, // BodyWeight, BloodPressure
            };
        }

        QH_HEALTHRECORD_DAT CreateDbRecord(DateTime recordDate, byte vitalType, decimal value1, decimal value2)
        {
            return new QH_HEALTHRECORD_DAT
            {
                RECORDDATE = recordDate,
                VITALTYPE = vitalType,
                VALUE1 = value1,
                VALUE2 = value2,
                CONDITIONTYPE = 0
            };
        }

        DbVitalValueItem CreateVitalItem(DateTime recordDate, QsDbVitalTypeEnum vitalType, decimal value1)
        {
            return new DbVitalValueItem
            {
                RecordDate = recordDate,
                VitalType = vitalType,
                Value1 = value1,
                Value2 = decimal.MinusOne,
                Value3 = decimal.MinusOne,
                Value4 = decimal.MinusOne,
                ConditionType = 0
            };
        }

        DbVitalValueItem CreateExerciseRecord(DateTime recordDate, decimal calorie)
        {
            return new DbVitalValueItem
            {
                RecordDate = recordDate,
                VitalType = QsDbVitalTypeEnum.CalorieBurn,
                Value1 = calorie,
                Value2 = decimal.MinusOne,
                Value3 = decimal.MinusOne,
                Value4 = decimal.MinusOne,
                ConditionType = 0
            };
        }
    }
}
