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
    public partial class NoteMedicineSyncDataAddWorkerFixture
    {

        [TestMethod]
        public void FromData_調剤日が不正でエラー()
        {
            var args = GetValidEthArgs();
            args.Data.RecordDate = "abc"; // 日付が不正

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.Data.RecordDate)).IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();
        }

        [TestMethod]
        public void FromData_ユーザー情報取得で例外が発生するとエラー()
        {
            var args = GetValidEthArgs();

            // 例外を起こす
            _accountRepo.Setup(m => m.ReadAccountIndexDat(_accountKey)).Throws(new Exception());

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500"); 
            results.Result.Detail.Contains("ユーザー情報の取得に失敗しました。").IsTrue();
        }

        [TestMethod]
        public void FromData_ユーザー情報取得でデータがない場合はエラー()
        {
            var args = GetValidEthArgs();

            // nullを返す
            _accountRepo.Setup(m => m.ReadAccountIndexDat(_accountKey)).Returns(default(QH_ACCOUNTINDEX_DAT));

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            results.Result.Detail.Contains("ユーザーが存在しません").IsTrue();
        }

        [TestMethod]
        public void FromData_データ変換処理で例外が発生するとエラー()
        {
            var args = GetValidEthArgs();
            SetUpValidDataMethods();

            // RpItemsが未設定
            args.Data.RpItems = null;

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            results.Result.Detail.Contains("データの変換に失敗しました").IsTrue();
        }

        [TestMethod]
        public void FromData_Jahisバリデーションで例外が発生するとエラー()
        {
            var args = GetValidEthArgs();
            SetUpValidDataMethods();

            // 必須項目にnullをセット
            args.Data.RpItems[0].MedicineItems[0].Name = null;

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            results.Result.Detail.Contains("JahisDataの検証で想定外のエラーが発生しました").IsTrue();
        }

        [TestMethod]
        public void FromData_JahisバリデーションNGでエラー()
        {
            var args = GetValidEthArgs();
            SetUpValidDataMethods();

            // 薬剤師名が40バイトオーバー
            args.Data.PharmacistName = "01234567890123456789012345678901234567891";

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002");
            results.Result.Detail.Contains("JahisDataの検証でエラーが発生しました").IsTrue();
            results.Result.Detail.Contains("最大バイト数を超えています").IsTrue();
        }

        [TestMethod]
        public void FromData_データ登録処理で例外が発生するとエラー()
        {
            var args = GetValidEthArgs();
            SetUpValidDataMethods();

            string refDataId = string.Empty;
            string refErrMsg = string.Empty;

            // 例外を起こす
            _noteRepo.Setup(m => m.WriteMedicine(_accountKey, _authorKey, It.IsAny<string>(), 1, _targetDate, 99999, It.IsAny<string>(), 1, 12345, ref refDataId, ref refErrMsg)).Throws(new Exception());

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("お薬手帳情報の登録処理でエラーが発生しました").IsTrue();
        }

        [TestMethod]
        public void FromData_データ登録処理で失敗するとエラー()
        {
            var args = GetValidEthArgs();
            SetUpValidDataMethods();

            string refDataId = string.Empty;
            string refErrMsg = string.Empty;

            // 戻り値失敗
            _noteRepo.Setup(m => m.WriteMedicine(_accountKey, _authorKey, It.IsAny<string>(), 1, _targetDate, 99999, It.IsAny<string>(), 1, 12345, ref refDataId, ref refErrMsg)).Returns((false, false, 0));

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            results.Result.Detail.Contains("お薬手帳データの登録処理に失敗しました").IsTrue();
        }

        [TestMethod]
        public void FromData_部分的成功_戻り値用のデータ取得で例外()
        {
            var args = GetValidEthArgs();
            SetUpValidDataMethods();

            // 例外を起こす (DBエラーの可能性）
            _noteRepo.Setup(m => m.ReadEntity(_accountKey, _targetDate, 1)).Throws(new Exception());

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("4000");
            results.Result.Detail.Contains("正常に登録できましたが、戻り値用のデータは取得できませんでした").IsTrue();
            results.Result.Detail.Contains("お薬手帳データの取得に失敗しました。").IsTrue();
        }

        [TestMethod]
        public void FromData_部分的成功_戻り値用のデータ取得でデータなし()
        {
            var args = GetValidEthArgs();
            SetUpValidDataMethods();

            // データなし（同時実行で削除されたりする場合はありえる)
            _noteRepo.Setup(m => m.ReadEntity(_accountKey, _targetDate, 1)).Returns(default(QH_MEDICINE_DAT));

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("4000");
            results.Result.Detail.Contains("正常に登録できましたが、戻り値用のデータは取得できませんでした").IsTrue();
            results.Result.Detail.Contains("お薬手帳データが存在しませんでした。").IsTrue();
        }

        [TestMethod]
        public void FromData_調剤薬_成功()
        {
            var args = GetValidEthArgs();
            SetUpValidDataMethods();

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
            // 登録したデータがセットされている
            // Entity変換処理の検証はSyncHistoryReadで行うのでここでは省略
            results.Data.IsNotNull();

            // 登録情報取得処理が行われた
            _noteRepo.Verify(m => m.ReadEntity(_accountKey, _targetDate, 1), Times.Once);

            // 日付と薬局番号が正常に変換された
            _callbackRecordDate.Is(_targetDate);
            _callbackPharmacyNo.Is(12345);

            // MedicineSetに正しく変換された
            _callbackEthMedicineSet.PrescriptionDate.Is(_targetDate.ToApiDateString());
            _callbackEthMedicineSet.PharmacyId.Is("12345");
            _callbackEthMedicineSet.FacilityName.Is("QOLMS病院");
            _callbackEthMedicineSet.FacilityId.Is("");
            _callbackEthMedicineSet.PharmacyName.Is("QOLMS薬局");
            _callbackEthMedicineSet.PharmacistName.Is("薬剤師　太郎");
            _callbackEthMedicineSet.LeftoverMedicine.Is("");
            _callbackEthMedicineSet.LeftoverMedicineAuthor.Is("");
            _callbackEthMedicineSet.SpecialNotes.Is("");
            _callbackEthMedicineSet.Memo.Is("メモ1メモ2");

            var medicineUsageN = _callbackEthMedicineSet.MedicineUsageN;
            medicineUsageN.Count.Is(3);
            medicineUsageN[0].DoctorName.Is("医者　二郎");
            medicineUsageN[0].RepresentedOrganizationName.Is("内科");
            medicineUsageN[0].DosageForm.Is(QsDbDosageFormTypeEnum.Oral);
            medicineUsageN[0].Usage.Is("毎食後");
            medicineUsageN[0].Days.Is("30");
            medicineUsageN[0].Unit.Is("日分");
            medicineUsageN[0].MedicineN.Count.Is(2);
            medicineUsageN[0].MedicineN[0].MedicineName.Is("ロキソニン錠６０ｍｇ");
            medicineUsageN[0].MedicineN[0].CodeSystem.Is((byte)4);
            medicineUsageN[0].MedicineN[0].MedicineCode.Is("1149019F1560");
            medicineUsageN[0].MedicineN[0].ReceiptNumber.Is((byte)1);
            medicineUsageN[0].MedicineN[0].Dose.Is("3");
            medicineUsageN[0].MedicineN[0].Unit.Is("錠");
            medicineUsageN[0].MedicineN[1].MedicineName.Is("ボルタレン錠２５ｍｇ");
            medicineUsageN[0].MedicineN[1].CodeSystem.Is((byte)4);
            medicineUsageN[0].MedicineN[1].MedicineCode.Is("1147002F1560");
            medicineUsageN[0].MedicineN[1].ReceiptNumber.Is((byte)1);
            medicineUsageN[0].MedicineN[1].Dose.Is("6");
            medicineUsageN[0].MedicineN[1].Unit.Is("錠");
            medicineUsageN[1].DoctorName.Is("医者　二郎");
            medicineUsageN[1].RepresentedOrganizationName.Is("内科");
            medicineUsageN[1].DosageForm.Is(QsDbDosageFormTypeEnum.DoseOfMedicine);
            medicineUsageN[1].Usage.Is("痛い時");
            medicineUsageN[1].Days.Is("10");
            medicineUsageN[1].Unit.Is("回分");
            medicineUsageN[1].MedicineN[0].MedicineName.Is("ボルタレン錠２５ｍｇ");
            medicineUsageN[1].MedicineN[0].CodeSystem.Is((byte)4);
            medicineUsageN[1].MedicineN[0].MedicineCode.Is("1147002F1560");
            medicineUsageN[1].MedicineN[0].ReceiptNumber.Is((byte)2);
            medicineUsageN[1].MedicineN[0].Dose.Is("2");
            medicineUsageN[1].MedicineN[0].Unit.Is("錠");
            medicineUsageN[2].DoctorName.Is("医者　二郎");
            medicineUsageN[2].RepresentedOrganizationName.Is("内科");
            medicineUsageN[2].DosageForm.Is(QsDbDosageFormTypeEnum.External);
            medicineUsageN[2].Usage.Is("痛い時");
            medicineUsageN[2].Days.Is("1");
            medicineUsageN[2].Unit.Is("調剤");
            medicineUsageN[2].MedicineN[0].MedicineName.Is("ロキソニンテープ５０ｍｇ");
            medicineUsageN[2].MedicineN[0].CodeSystem.Is((byte)4);
            medicineUsageN[2].MedicineN[0].MedicineCode.Is("2649735S2024");
            medicineUsageN[2].MedicineN[0].ReceiptNumber.Is((byte)3);
            medicineUsageN[2].MedicineN[0].Dose.Is("7");
            medicineUsageN[2].MedicineN[0].Unit.Is("枚");

            // Jahisデータに正しく変換されている
            _callbackJahis.Header.Header_1.Is($"JAHISTC{(int)JMVersionTypeEnum.Latest:00}");
            _callbackJahis.Header.Header_2.Is("2");

            _callbackJahis.No001.No001_1.Is("1");
            _callbackJahis.No001.No001_2.Is("田中　一郎");
            _callbackJahis.No001.No001_3.Is("1");
            _callbackJahis.No001.No001_4.Is("20000101");
            _callbackJahis.No001.No001_11.Is("タナカ　イチロウ");

            var prescription = _callbackJahis.Prescription_List.First();
            prescription.No005.No005_1.Is("5");
            prescription.No005.No005_2.Is("20250201");
            prescription.No005.No005_3.Is("2");

            prescription.No011.No011_1.Is("11");
            prescription.No011.No011_2.Is("QOLMS薬局");
            prescription.No011.No011_5.Is("12345");
            prescription.No011.No011_9.Is("2");

            prescription.No015.No015_1.Is("15");
            prescription.No015.No015_2.Is("薬剤師　太郎");
            prescription.No015.No015_4.Is("2");

            prescription.No051.No051_1.Is("51");
            prescription.No051.No051_2.Is("QOLMS病院");
            prescription.No051.No051_6.Is("2");

            prescription.No601_List[0].No601_1.Is("601");
            prescription.No601_List[0].No601_2.Is("メモ1");
            prescription.No601_List[0].No601_3.Is(DateTime.Today.ToString("yyyyMMdd"));
            prescription.No601_List[1].No601_1.Is("601");
            prescription.No601_List[1].No601_2.Is("メモ2");
            prescription.No601_List[1].No601_3.Is(DateTime.Today.ToString("yyyyMMdd"));

            prescription.RpSet_List[0].No055.No055_1.Is("55");
            prescription.RpSet_List[0].No055.No055_2.Is("医者　二郎");
            prescription.RpSet_List[0].No055.No055_3.Is("内科");
            prescription.RpSet_List[0].No055.No055_4.Is("2");

            var rpItems = prescription.RpSet_List[0].Rp_List;
            rpItems[0].No301.No301_1.Is("301");
            rpItems[0].No301.No301_2.Is("1");
            rpItems[0].No301.No301_3.Is("毎食後");
            rpItems[0].No301.No301_4.Is("30");
            rpItems[0].No301.No301_5.Is("日分");
            rpItems[0].No301.No301_6.Is("1");
            rpItems[0].No301.No301_7.Is("1");
            rpItems[0].No301.No301_9.Is("2");
            rpItems[0].No311_List[0].No311_1.Is("311");
            rpItems[0].No311_List[0].No311_2.Is("1");
            rpItems[0].No311_List[0].No311_3.Is("補足1");
            rpItems[0].No311_List[0].No311_4.Is("2");
            rpItems[0].No311_List[1].No311_1.Is("311");
            rpItems[0].No311_List[1].No311_2.Is("1");
            rpItems[0].No311_List[1].No311_3.Is("補足2");
            rpItems[0].No311_List[1].No311_4.Is("2");

            var rp1Medicines = rpItems[0].Medicine_List;
            rp1Medicines[0].No201.No201_1.Is("201");
            rp1Medicines[0].No201.No201_2.Is("1");
            rp1Medicines[0].No201.No201_3.Is("ロキソニン錠６０ｍｇ");
            rp1Medicines[0].No201.No201_4.Is("3");
            rp1Medicines[0].No201.No201_5.Is("錠");
            rp1Medicines[0].No201.No201_6.Is("4");
            rp1Medicines[0].No201.No201_7.Is("1149019F1560");
            rp1Medicines[0].No201.No201_8.Is("2");
            rp1Medicines[1].No201.No201_1.Is("201");
            rp1Medicines[1].No201.No201_2.Is("1");
            rp1Medicines[1].No201.No201_3.Is("ボルタレン錠２５ｍｇ");
            rp1Medicines[1].No201.No201_4.Is("6");
            rp1Medicines[1].No201.No201_5.Is("錠");
            rp1Medicines[1].No201.No201_6.Is("4");
            rp1Medicines[1].No201.No201_7.Is("1147002F1560");
            rp1Medicines[1].No201.No201_8.Is("2");

            rpItems[1].No301.No301_1.Is("301");
            rpItems[1].No301.No301_2.Is("2");
            rpItems[1].No301.No301_3.Is("痛い時");
            rpItems[1].No301.No301_4.Is("10");
            rpItems[1].No301.No301_5.Is("回分");
            rpItems[1].No301.No301_6.Is("3");
            rpItems[1].No301.No301_7.Is("1");
            rpItems[1].No301.No301_9.Is("2");
            rpItems[1].No311_List[0].No311_2.Is("2");
            rpItems[1].No311_List[0].No311_3.Is("6時間あける");

            var rp2Medicines = rpItems[1].Medicine_List;
            rp2Medicines[0].No201.No201_1.Is("201");
            rp2Medicines[0].No201.No201_2.Is("2");
            rp2Medicines[0].No201.No201_3.Is("ボルタレン錠２５ｍｇ");
            rp2Medicines[0].No201.No201_4.Is("2");
            rp2Medicines[0].No201.No201_5.Is("錠");
            rp2Medicines[0].No201.No201_6.Is("4");
            rp2Medicines[0].No201.No201_7.Is("1147002F1560");
            rp2Medicines[0].No201.No201_8.Is("2");

            rpItems[2].No301.No301_1.Is("301");
            rpItems[2].No301.No301_2.Is("3");
            rpItems[2].No301.No301_3.Is("痛い時");
            rpItems[2].No301.No301_4.Is("1");
            rpItems[2].No301.No301_5.Is("調剤");
            rpItems[2].No301.No301_6.Is("5");
            rpItems[2].No301.No301_7.Is("1");
            rpItems[2].No301.No301_9.Is("2");

            var rp3Medicines = rpItems[2].Medicine_List;
            rp3Medicines[0].No201.No201_1.Is("201");
            rp3Medicines[0].No201.No201_2.Is("3");
            rp3Medicines[0].No201.No201_3.Is("ロキソニンテープ５０ｍｇ");
            rp3Medicines[0].No201.No201_4.Is("7");
            rp3Medicines[0].No201.No201_5.Is("枚");
            rp3Medicines[0].No201.No201_6.Is("4");
            rp3Medicines[0].No201.No201_7.Is("2649735S2024");
            rp3Medicines[0].No201.No201_8.Is("2");
        }

        [TestMethod]
        public void FromData_市販薬_成功()
        {
            var args = GetValidOtcArgs();
            SetUpValidDataMethods();

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
            // 登録したデータがセットされている
            // Entity変換処理の検証はSyncHistoryReadで行うのでここでは省略
            results.Data.IsNotNull();

            // 登録情報取得処理が行われた
            _noteRepo.Verify(m => m.ReadEntity(_accountKey, _targetDate, 1), Times.Once);

            // 日付と薬局番号が正常に変換された
            _callbackRecordDate.Is(_targetDate);
            _callbackPharmacyNo.Is(12345);

            // MedicineSetに正しく変換された
            _callbackOtcMedicineSet.PharmacyNo.Is(12345);
            _callbackOtcMedicineSet.PharmacyName.Is("QOLMS薬局");
            _callbackOtcMedicineSet.Comment.Is(args.Data.Memo);

            var medicineN = _callbackOtcMedicineSet.MedicineN;
            medicineN[0].MedicineName.Is("アレグラＦＸ１４錠");
            medicineN[0].ItemCodeType.Is("J");
            medicineN[0].ItemCode.Is("4987188166031");
            medicineN[1].MedicineName.Is("新ビオフェルミンＳ錠　４５錠");
            medicineN[1].ItemCodeType.Is("J");
            medicineN[1].ItemCode.Is("4987306054769");
            medicineN[2].MedicineName.Is("ナザール「スプレー」　１５ｍｌ");
            medicineN[2].ItemCodeType.Is("J");
            medicineN[2].ItemCode.Is("4987316000086");

            // Jahisデータに正しく変換されている
            _callbackJahis.Header.Header_1.Is($"JAHISTC{(int)JMVersionTypeEnum.Latest:00}");
            _callbackJahis.Header.Header_2.Is("2");

            _callbackJahis.No001.No001_1.Is("1");
            _callbackJahis.No001.No001_2.Is("田中　一郎");
            _callbackJahis.No001.No001_3.Is("1");
            _callbackJahis.No001.No001_4.Is("20000101");
            _callbackJahis.No001.No001_11.Is("タナカ　イチロウ");

            _callbackJahis.No004_List[0].No004_1.Is("4");
            _callbackJahis.No004_List[0].No004_2.Is("メモ1");
            _callbackJahis.No004_List[0].No004_3.Is(DateTime.Today.ToString("yyyyMMdd"));
            _callbackJahis.No004_List[0].No004_4.Is("2");
            _callbackJahis.No004_List[1].No004_1.Is("4");
            _callbackJahis.No004_List[1].No004_2.Is("メモ2");
            _callbackJahis.No004_List[1].No004_3.Is(DateTime.Today.ToString("yyyyMMdd"));
            _callbackJahis.No004_List[1].No004_4.Is("2");

            var otcMedicineN = _callbackJahis.No003_List;
            otcMedicineN[0].No003_1.Is("3");
            otcMedicineN[0].No003_2.Is("アレグラＦＸ１４錠");
            otcMedicineN[0].No003_5.Is("2");
            otcMedicineN[1].No003_1.Is("3");
            otcMedicineN[1].No003_2.Is("新ビオフェルミンＳ錠　４５錠");
            otcMedicineN[1].No003_5.Is("2");
            otcMedicineN[2].No003_1.Is("3");
            otcMedicineN[2].No003_2.Is("ナザール「スプレー」　１５ｍｌ");
            otcMedicineN[2].No003_5.Is("2");

            var prescription = _callbackJahis.Prescription_List[0];
            prescription.No005.No005_2.Is("20250201");
            prescription.No011.No011_2.Is("QOLMS薬局");
            prescription.No011.No011_5.Is("12345");            
        }

        void SetUpValidDataMethods()
        {
            // ユーザー情報を返す
            _accountRepo.Setup(m => m.ReadAccountIndexDat(_accountKey)).Returns(new QH_ACCOUNTINDEX_DAT
            {
                FAMILYNAME = "田中",
                GIVENNAME = "一郎",
                FAMILYKANANAME = "タナカ",
                GIVENKANANAME = "イチロウ",
                SEXTYPE = 1,
                BIRTHDAY = new DateTime(2000,1,1)
            });

            string refDataId = string.Empty;
            string refErrMsg = string.Empty;

            // 書き込み成功しSEQ1を返す
            _noteRepo.Setup(m => m.WriteMedicine(_accountKey, _authorKey, It.IsAny<string>(), It.IsAny<byte>(), _targetDate, 99999, It.IsAny<string>(), It.IsAny<int>(), 12345, ref refDataId, ref refErrMsg)).Returns((true, true, 1)).Callback(new WriteCallback((Guid a, Guid b, string jahis, byte dataType, DateTime recordDate, int f, string medicineSet, int ownerType, int pharmacyNo, ref string j, ref string k) =>
            {
                _callbackRecordDate = recordDate;
                _callbackPharmacyNo = pharmacyNo;

                var qsJson = new QsJsonSerializer();
                _callbackJahis = qsJson.Deserialize<JM_Message>(jahis);
                if (dataType == 2)
                {
                    _callbackOtcMedicineSet = qsJson.Deserialize<QhMedicineSetOtcDrugOfJson>(medicineSet);
                }
                else
                {
                    _callbackEthMedicineSet = qsJson.Deserialize<QhMedicineSetEthicalDrugOfJson>(medicineSet);
                }
            }));

            // 書き込んだデータを返す
            _noteRepo.Setup(m => m.ReadEntity(_accountKey, _targetDate, 1)).Returns(new QH_MEDICINE_DAT());

            // 薬効分類セットアップ
            _noteRepo.Setup(m => m.ReadYakkoList(It.IsAny<List<string>>())).Returns(new List<DbEthicalDrugCategoryItem>());
        }

        QoNoteMedicineSyncDataAddApiArgs GetValidEthArgs()
        {
            return new QoNoteMedicineSyncDataAddApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),
                Executor = _authorKey.ToApiGuidString(),
                ExecutorName = "NoteMedicine",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.QolmsiOSApp}",
                LinkageSystemNo = 99999,
                DataType = (byte)DataTypeEnum.EthicalDrug,
                OwnerType = (byte)OwnerTypeEnum.Oneself,
                Data = new QoNoteMedicineDetail
                {
                    RecordDate = _targetDate.ToApiDateString(),
                    DataType = (byte)DataTypeEnum.EthicalDrug,
                    OwnerType = (byte)OwnerTypeEnum.Oneself,
                    PharmacyNo = 12345,
                    PharmacyName = "QOLMS薬局",
                    PharmacistName ="薬剤師　太郎",
                    HospitalName = "QOLMS病院",
                    DepartmentName = "内科",
                    DoctorName = "医者　二郎",
                    Memo = "メモ1\nメモ2",
                    RpItems = new List<QoRpItem>
                    {
                        new QoRpItem
                        {
                            UsageName = "毎食後",
                            UsageSupplement = "補足1\n補足2",
                            Quantity = 30,
                            QuantityUnit = "日分",
                            DosageFormCode = "1",
                            MedicineItems = new List<QoMedicineItem>
                            {
                                new QoMedicineItem
                                {
                                    Name = "ロキソニン錠６０ｍｇ",
                                    Quantity = "3",
                                    QuantityUnit = "錠",
                                    YjCode = "1149019F1560",                                    
                                },
                                new QoMedicineItem
                                {
                                    Name = "ボルタレン錠２５ｍｇ",
                                    Quantity = "6",
                                    QuantityUnit = "錠",
                                    YjCode = "1147002F1560",
                                }
                            }
                        },
                        new QoRpItem
                        {
                            UsageName = "痛い時",
                            UsageSupplement = "6時間あける",
                            Quantity = 10,
                            QuantityUnit = "回分",
                            DosageFormCode = "3",
                            MedicineItems = new List<QoMedicineItem>
                            {
                                new QoMedicineItem
                                {
                                    Name = "ボルタレン錠２５ｍｇ",
                                    Quantity = "2",
                                    QuantityUnit = "錠",
                                    YjCode = "1147002F1560",
                                }
                            }
                        },
                        new QoRpItem
                        {
                            UsageName = "痛い時",
                            UsageSupplement = "",
                            Quantity = 1,
                            QuantityUnit = "調剤",
                            DosageFormCode = "5",
                            MedicineItems = new List<QoMedicineItem>
                            {
                                new QoMedicineItem
                                {
                                    Name = "ロキソニンテープ５０ｍｇ",
                                    Quantity = "7",
                                    QuantityUnit = "枚",
                                    YjCode = "2649735S2024",
                                }
                            }
                        }
                    }
                }
            };
        }

        QoNoteMedicineSyncDataAddApiArgs GetValidOtcArgs()
        {
            return new QoNoteMedicineSyncDataAddApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),
                Executor = _authorKey.ToApiGuidString(),
                ExecutorName = "NoteMedicine",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.QolmsiOSApp}",
                LinkageSystemNo = 99999,
                DataType = (byte)DataTypeEnum.OtcDrug,
                OwnerType = (byte)OwnerTypeEnum.Oneself,
                Data = new QoNoteMedicineDetail
                {
                    RecordDate = _targetDate.ToApiDateString(),
                    DataType = (byte)DataTypeEnum.EthicalDrug,
                    OwnerType = (byte)OwnerTypeEnum.Oneself,
                    PharmacyNo = 12345,
                    PharmacyName = "QOLMS薬局",
                    Memo = "メモ1\nメモ2",
                    MedicineItems = new List<QoMedicineItem>()
                    {
                        new QoMedicineItem
                        {
                            Name = "アレグラＦＸ１４錠",
                            ItemCodeType = "J",
                            ItemCode = "4987188166031"
                        },
                        new QoMedicineItem
                        {
                            Name = "新ビオフェルミンＳ錠　４５錠",
                            ItemCodeType = "J",
                            ItemCode = "4987306054769"
                        },
                        new QoMedicineItem
                        {
                            Name = "ナザール「スプレー」　１５ｍｌ",
                            ItemCodeType = "J",
                            ItemCode = "4987316000086"
                        }
                    }                 
                    
                }
            };
        }
    }
}
