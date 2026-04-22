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
    public partial class NoteMedicineSyncDataEditWorkerFixture
    {
        [TestMethod]
        public void ForOtc_元データの処方レコードが存在しない場合はエラー()
        {
            var args = GetValidOtcArgs();
            SetUpValidOtcMethods();

            _noteRepo.Setup(m => m.ReadEntity(_accountKey, _targetDate, 1))
                .Returns(new QH_MEDICINE_DAT
                {
                    CONVERTEDMEDICINESET = CreateInValidOtcJahisData(),
                    DATATYPE = 2,
                    OWNERTYPE = 1
                });

            var results = _worker.Edit(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("4002");
            results.Result.Detail.Contains("編集対象の項目が存在しませんでした").IsTrue();
        }

        [TestMethod]
        public void ForOtc_更新したJahisデータが検証NGの場合はエラー()
        {
            var args = GetValidOtcArgs();
            SetUpValidOtcMethods();

            _noteRepo.Setup(m => m.ReadEntity(_accountKey, _targetDate, 1))
                .Returns(new QH_MEDICINE_DAT
                {
                    CONVERTEDMEDICINESET = CreateInValidOtcJahisData(1), // 検証NGデータ
                    DATATYPE = 2,
                    OWNERTYPE = 1
                });

            var results = _worker.Edit(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            results.Result.Detail.Contains("JahisDataの検証でエラーが発生しました").IsTrue();
        }

        [TestMethod]
        public void ForOtc_更新したJahisデータが検証で例外が発生した場合はエラー()
        {
            var args = GetValidOtcArgs();
            SetUpValidOtcMethods();

            _noteRepo.Setup(m => m.ReadEntity(_accountKey, _targetDate, 1))
                .Returns(new QH_MEDICINE_DAT
                {
                    CONVERTEDMEDICINESET = CreateInValidOtcJahisData(2), // 例外起こすデータ
                    DATATYPE = 2,
                    OWNERTYPE = 1
                });

            var results = _worker.Edit(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            results.Result.Detail.Contains("JahisDataの検証で想定外のエラーが発生しました").IsTrue();
        }

        [TestMethod]
        public void ForOtc_データ更新処理で例外が発生するとエラー()
        {
            var args = GetValidOtcArgs();
            SetUpValidOtcMethods();

            // 例外を起こす
            _noteRepo.Setup(m => m.UpdateEntity(_targetEntity)).Throws(new Exception());

            var results = _worker.Edit(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("お薬手帳情報の更新処理でエラーが発生しました").IsTrue();
        }

        [TestMethod]
        public void ForOtc_手入力市販薬データで正常終了する()
        {
            var args = GetValidOtcArgs();
            SetUpValidOtcMethods();

            var results = _worker.Edit(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // DBの更新処理が実行された
            _noteRepo.Verify(m => m.UpdateEntity(_targetEntity), Times.Once);


            // MedicineSetが更新されている
            _callbackOtcMedicineSet.PharmacyNo.Is(12345);
            _callbackOtcMedicineSet.PharmacyName.Is("Update薬局");
            _callbackOtcMedicineSet.Comment.Is("メモ8\nメモ9");

            var medicineN = _callbackOtcMedicineSet.MedicineN;
            medicineN[0].MedicineName.Is("ビオフェルミンS錠");
            medicineN[0].ItemCodeType.Is("J");
            medicineN[0].ItemCode.Is("1234");
            medicineN[1].MedicineName.Is("ビオスリー");
            medicineN[1].ItemCodeType.Is("T");
            medicineN[1].ItemCode.Is("5678");


            // Jahisデータ

            // 患者レコードは変化なし
            _callbackJahis.No001.No001_2.Is("山田　太郎");
            _callbackJahis.No001.No001_3.Is("1");
            _callbackJahis.No001.No001_4.Is("19990101");
            _callbackJahis.No001.No001_11.Is("ヤマダ　タロウ");

            // 手帳メモは更新されている
            _callbackJahis.No004_List.Count.Is(2);
            _callbackJahis.No004_List[0].No004_2.Is("メモ8");
            _callbackJahis.No004_List[1].No004_2.Is("メモ9");

            // 市販薬情報は更新されている
            _callbackJahis.No003_List.Count.Is(2);
            _callbackJahis.No003_List[0].No003_2.Is("ビオフェルミンS錠");
            _callbackJahis.No003_List[1].No003_2.Is("ビオスリー");

            // 処方レコード数に変化なし
            _callbackJahis.Prescription_List.Count.Is(1);

            var pre1 = _callbackJahis.Prescription_List[0];

            // 調剤日変化なし
            pre1.No005.No005_2.Is(_targetDate.ToString("yyyyMMdd"));

            // 薬局は更新される
            pre1.No011.No011_2.Is("Update薬局");           

        }

        [TestMethod]
        public void ForOtc_部分的成功_戻り値用のデータ取得で例外()
        {
            var args = GetValidOtcArgs();
            SetUpValidOtcMethods();

            // 2回目で例外を起こす (DBエラーの可能性）
            _noteRepo.SetupSequence(m => m.ReadEntity(_accountKey, _targetDate, 1))
                .Returns(_targetEntity)
                .Throws(new Exception());

            var results = _worker.Edit(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("4000");
            results.Result.Detail.Contains("正常に登録できましたが、戻り値用のデータは取得できませんでした").IsTrue();
            results.Result.Detail.Contains("お薬手帳データの取得に失敗しました。").IsTrue();
        }

        void SetUpValidOtcMethods()
        {
            _targetEntity = new QH_MEDICINE_DAT
            {
                ACCOUNTKEY = _accountKey,
                RECORDDATE = _targetDate,
                SEQUENCE = 1,
                CONVERTEDMEDICINESET = CreateValidOtcJahisData(),
                MEDICINESET = "",
                PHARMACYNO = 12345,
                DATATYPE = 2,
                OWNERTYPE = 1
            };

            _noteRepo.SetupSequence(m => m.ReadEntity(_accountKey, _targetDate, 1))
                .Returns(_targetEntity)
                .Returns(new QH_MEDICINE_DAT());

            _noteRepo.Setup(m => m.UpdateEntity(_targetEntity))
                .Callback((QH_MEDICINE_DAT entity) =>
                {
                    _callbackRecordDate = entity.RECORDDATE;

                    var qsJson = new QsJsonSerializer();
                    _callbackJahis = qsJson.Deserialize<JM_Message>(entity.CONVERTEDMEDICINESET);
                    if (entity.DATATYPE == 2)
                    {
                        _callbackOtcMedicineSet = qsJson.Deserialize<QhMedicineSetOtcDrugOfJson>(entity.MEDICINESET);
                    }
                    else
                    {
                        _callbackEthMedicineSet = qsJson.Deserialize<QhMedicineSetEthicalDrugOfJson>(entity.MEDICINESET);
                    }
                });
        }
    }
}
