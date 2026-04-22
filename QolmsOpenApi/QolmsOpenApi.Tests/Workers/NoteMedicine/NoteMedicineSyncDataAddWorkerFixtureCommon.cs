using MGF.QOLMS.JAHISMedicineEntityV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Models;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public partial class NoteMedicineSyncDataAddWorkerFixture
    {
        NoteMedicineSyncDataAddWorker _worker;
        Mock<INoteMedicineRepository> _noteRepo;
        Mock<IAccountRepository> _accountRepo;
        Guid _accountKey = Guid.NewGuid();
        Guid _authorKey = Guid.NewGuid();
        DateTime _targetDate = new DateTime(2025, 2, 1);
        JM_Message _callbackJahis;
        QhMedicineSetOtcDrugOfJson _callbackOtcMedicineSet;
        QhMedicineSetEthicalDrugOfJson _callbackEthMedicineSet;
        int _callbackPharmacyNo;
        DateTime _callbackRecordDate;

        [TestInitialize]
        public void Initialize()
        {
            _noteRepo = new Mock<INoteMedicineRepository>();
            _accountRepo = new Mock<IAccountRepository>();

            _worker = new NoteMedicineSyncDataAddWorker(_noteRepo.Object, _accountRepo.Object);
        }

        [TestMethod]
        public void アカウントキーが不正でエラーとなる()
        {
            var args = GetValidArgs();
            args.ActorKey = "invalidGuid";

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.ActorKey)).IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();
        }

        [TestMethod]
        public void 所有者キーが不正でエラーとなる()
        {
            var args = GetValidArgs();
            args.Executor = "invalidGuid";

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.AuthorKey)).IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();
        }

        [TestMethod]
        [DataRow((QH_MEDICINE_DAT.OwnerTypeEnum)4)] // 未定義値
        [DataRow(QH_MEDICINE_DAT.OwnerTypeEnum.None)]
        public void オーナータイプが定義外またはNoneの場合はエラー(QH_MEDICINE_DAT.OwnerTypeEnum ownerType)
        {
            var args = GetValidArgs();
            args.OwnerType = (byte)ownerType;

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.OwnerType)).IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();
        }

        [TestMethod]
        [DataRow(QH_MEDICINE_DAT.OwnerTypeEnum.Data)]
        [DataRow(QH_MEDICINE_DAT.OwnerTypeEnum.TextReader)]
        public void オーナータイプが1または2以外の場合はエラー(QH_MEDICINE_DAT.OwnerTypeEnum ownerType)
        {
            var args = GetValidArgs();
            args.OwnerType = (byte)ownerType; // NG値

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.OwnerType)).IsTrue();
            results.Result.Detail.Contains("未対応").IsTrue();           
        }

        [TestMethod]
        [DataRow((QH_MEDICINE_DAT.DataTypeEnum)5)] // 未定義値
        [DataRow(QH_MEDICINE_DAT.DataTypeEnum.None)]
        public void データタイプが定義外またはNoneの場合はエラー(QH_MEDICINE_DAT.DataTypeEnum dataType)
        {
            var args = GetValidArgs();
            args.DataType = (byte)dataType;

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.DataType)).IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();            
        }

        [TestMethod]
        [DataRow(QH_MEDICINE_DAT.OwnerTypeEnum.Oneself, QH_MEDICINE_DAT.DataTypeEnum.Ssmix)]
        [DataRow(QH_MEDICINE_DAT.OwnerTypeEnum.Oneself, QH_MEDICINE_DAT.DataTypeEnum.OtcDrugPhoto)]
        [DataRow(QH_MEDICINE_DAT.OwnerTypeEnum.QrCode, QH_MEDICINE_DAT.DataTypeEnum.OtcDrug)]
        [DataRow(QH_MEDICINE_DAT.OwnerTypeEnum.QrCode, QH_MEDICINE_DAT.DataTypeEnum.Ssmix)]
        public void オーナータイプとデータタイプの組み合わせ不正でエラー(QH_MEDICINE_DAT.OwnerTypeEnum ownerType, QH_MEDICINE_DAT.DataTypeEnum dataType)
        {
            var args = GetValidArgs();
            args.OwnerType = (byte)ownerType;
            args.DataType = (byte)dataType;

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.OwnerType)).IsTrue();
            results.Result.Detail.Contains(nameof(args.DataType)).IsTrue();
            results.Result.Detail.Contains("の組み合わせが不正です").IsTrue();
        }

        QoNoteMedicineSyncDataAddApiArgs GetValidArgs()
        {
            return new QoNoteMedicineSyncDataAddApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),
                Executor = _authorKey.ToApiGuidString(),
                ExecutorName = "NoteMedicine",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.QolmsiOSApp}",
                LinkageSystemNo = 99999,
                DataType = 1,
                OwnerType = 1,
                JahisData = "hoge",
                Data = null
            };
        }
    }
}
