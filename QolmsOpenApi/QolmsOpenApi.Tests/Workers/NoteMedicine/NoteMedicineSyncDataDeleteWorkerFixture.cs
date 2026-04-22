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
using DataTypeEnum = MGF.QOLMS.QolmsDbEntityV1.QH_MEDICINE_DAT.DataTypeEnum;
using OwnerTypeEnum = MGF.QOLMS.QolmsDbEntityV1.QH_MEDICINE_DAT.OwnerTypeEnum;

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class NoteMedicineSyncDataDeleteWorkerFixture
    {
        NoteMedicineSyncDataDeleteWorker _worker;
        Mock<INoteMedicineRepository> _noteRepo;
        Guid _accountKey = Guid.NewGuid();
        DateTime _targetDate = new DateTime(2025, 2, 1);

        [TestInitialize]
        public void Initialize()
        {
            _noteRepo = new Mock<INoteMedicineRepository>();
            _worker = new NoteMedicineSyncDataDeleteWorker(_noteRepo.Object);
        }

        [TestMethod]
        public void アカウントキーが不正でエラーとなる()
        {
            var args = GetValidArgs();
            args.ActorKey = "invalidGuid";

            var results = _worker.Delete(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.ActorKey)).IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();
        }

        [TestMethod]
        public void RecordDateが日付に変換不可の場合はエラー()
        {
            var args = GetValidArgs();
            args.RecordDate = "hoge";

            var results = _worker.Delete(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.RecordDate)).IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();
        }

        [TestMethod]
        public void Sequenceが0以下でエラー()
        {
            var args = GetValidArgs();
            args.Sequence = 0;

            var results = _worker.Delete(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.Sequence)).IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();
        }

        [TestMethod]
        public void お薬手帳のデータ取得処理で対象データが存在しない場合はエラー()
        {
            var args = GetValidArgs();
            // データなし
            _noteRepo.Setup(m => m.ReadEntity(_accountKey, _targetDate, 1)).Returns(default(QH_MEDICINE_DAT));

            var results = _worker.Delete(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("4001");
            results.Result.Detail.Contains("お薬手帳データが存在しませんでした").IsTrue();
        }

        [TestMethod]
        public void お薬手帳のデータ取得処理で例外が発生したらエラー()
        {
            var args = GetValidArgs();
            // 例外発生
            _noteRepo.Setup(m => m.ReadEntity(_accountKey, _targetDate, 1)).Throws(new Exception());

            var results = _worker.Delete(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("お薬手帳データの取得に失敗しました").IsTrue();
        }

        [TestMethod]
        [DataRow(OwnerTypeEnum.Oneself, DataTypeEnum.Ssmix)]
        [DataRow(OwnerTypeEnum.Oneself, DataTypeEnum.OtcDrugPhoto)]
        [DataRow(OwnerTypeEnum.QrCode, DataTypeEnum.OtcDrug)]
        [DataRow(OwnerTypeEnum.QrCode, DataTypeEnum.Ssmix)]
        [DataRow(OwnerTypeEnum.Data, DataTypeEnum.EthicalDrug)]
        [DataRow(OwnerTypeEnum.Data, DataTypeEnum.OtcDrug)]
        [DataRow(OwnerTypeEnum.Data, DataTypeEnum.OtcDrugPhoto)]
        public void お薬手帳データのDataTypeとOwnerTypeの組み合わせで削除NGならエラー(OwnerTypeEnum ownerType, DataTypeEnum dataType)
        {
            var args = GetValidArgs();

            _noteRepo.Setup(m => m.ReadEntity(_accountKey, _targetDate, 1))
                .Returns(new QH_MEDICINE_DAT
                {                    
                    DATATYPE = (byte)dataType,
                    OWNERTYPE = (int)ownerType,
                });

            var results = _worker.Delete(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            results.Result.Detail.Contains("削除不可能なデータです").IsTrue();
        }

        [TestMethod]
        public void 削除処理に失敗するとエラー()
        {
            var args = GetValidArgs();
            SetUpValidMethods();

            // 削除処理で例外を投げる
            _noteRepo.Setup(m => m.DeleteEntity(It.IsAny<QH_MEDICINE_DAT>())).Throws(new Exception());

            var results = _worker.Delete(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("データの削除処理に失敗しました").IsTrue();
        }

        [TestMethod]
        public void 正常に削除が完了する()
        {
            var args = GetValidArgs();
            SetUpValidMethods();            

            var results = _worker.Delete(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
            
            // 削除処理が1回呼び出された
            _noteRepo.Verify(m => m.DeleteEntity(It.IsAny<QH_MEDICINE_DAT>()), Times.Once);
        }

        void SetUpValidMethods()
        {
            _noteRepo.Setup(m => m.ReadEntity(_accountKey, _targetDate, 1))
                .Returns(new QH_MEDICINE_DAT
                {
                    DATATYPE = 1,
                    OWNERTYPE = 1,
                });
        }

        QoNoteMedicineSyncDataDeleteApiArgs GetValidArgs()
        {
            return new QoNoteMedicineSyncDataDeleteApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),
                ExecutorName = "NoteMedicine",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.QolmsiOSApp}",
                RecordDate = _targetDate.ToApiDateString(),
                Sequence = 1                
            };
        }
    }
}
