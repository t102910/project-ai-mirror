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
    public class NoteMedicineSyncUpdateReadWorkerFixture
    {
        NoteMedicineSyncUpdateReadWorker _worker;
        Mock<INoteMedicineRepository> _noteRepo;
        Guid _accountKey = Guid.NewGuid();
        DateTime _startDate = new DateTime(2024, 10, 1, 10, 30, 15);
        DateTime _endDate = new DateTime(2024,10,31, 10, 30, 15);

        [TestInitialize]
        public void Initialize()
        {
            _noteRepo = new Mock<INoteMedicineRepository>();
            _worker = new NoteMedicineSyncUpdateReadWorker(_noteRepo.Object);
        }

        [TestMethod]
        public void アカウントキーが不正でエラーとなる()
        {
            var args = GetValidArgs();
            args.ActorKey = "invalidGuid";

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.ActorKey)).IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();
        }

        [TestMethod]
        public void StartDateが不正の場合はエラーとなる()
        {
            var args = GetValidArgs();
            args.SyncStartDate = "hoge";

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.SyncStartDate)).IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();
        }

        [TestMethod]
        public void SyncEndDateが不正の場合はエラーとなる()
        {
            var args = GetValidArgs();
            args.SyncEndDate = "hoge";

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.SyncEndDate)).IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();
        }

        [TestMethod]
        public void SyncEndDateがSyncStartDateより過去の値の場合はエラーとなる()
        {
            var args = GetValidArgs();
            args.SyncEndDate = _startDate.AddDays(-1).ToApiDateString();

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains("期間指定").IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();
        }

        [TestMethod]
        public void お薬手帳情報の取得で例外が発生するとエラー()
        {
            var args = GetValidArgs();
            var startDate = args.SyncStartDate.TryToValueType(DateTime.MinValue);
            var endDate = args.SyncEndDate.TryToValueType(DateTime.MinValue);

            // 例外発生
            _noteRepo.Setup(m => m.ReadNoteUpdate(_accountKey, startDate, endDate)).Throws(new Exception());

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("お薬手帳データの取得に失敗しました").IsTrue();
        }

        [TestMethod]
        public void 薬効分類の取得で例外が発生するとエラー()
        {
            var args = GetValidArgs();
            SetUpValidMethods(args);

            // 薬効取得処理で例外を発生させる
            _noteRepo.Setup(m => m.ReadYakkoList(It.IsAny<List<string>>())).Throws(new Exception());

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("薬効データの取得に失敗しました").IsTrue();
        }      

        [TestMethod]
        public void 日数がEmptyの場合は規定値が設定される()
        {
            var args = GetValidArgs();
            args.SyncEndDate = string.Empty;
            SetUpValidMethods(args);

            var startDate = args.SyncStartDate.TryToValueType(DateTime.MinValue);
            var endDate = DateTime.Now;

            DateTime captureEndDate = DateTime.MinValue;

            // 規定値（現在時刻）設定
            _noteRepo.Setup(m => m.ReadNoteUpdate(_accountKey, startDate, It.IsAny<DateTime>())).Returns(new List<QH_MEDICINE_DAT>()).Callback((Guid a, DateTime s, DateTime e) =>
            {
                captureEndDate = e;
                
            });

            var results = _worker.Read(args);

            // Nowを一致させるのは不可能なので日付のみの一致でよしとする
            captureEndDate.Date.Is(endDate.Date);

            // 規定値で呼び出された
            _noteRepo.Verify(m => m.ReadNoteUpdate(_accountKey, startDate, It.IsAny<DateTime>()), Times.Once);

            // これ以外の検証はここでは行わない
        }

        [TestMethod]
        public void 正常終了_結果0件の場合()
        {
            var args = GetValidArgs();
            args.SyncStartDate = (new DateTime(2024, 8, 1)).ToApiDateString();
            args.SyncEndDate = (new DateTime(2024, 8, 15)).ToApiDateString();
            SetUpValidMethods(args);

            var startDate = args.SyncStartDate.TryToValueType(DateTime.MinValue);
            var endDate = args.SyncEndDate.TryToValueType(DateTime.MinValue);
            var nextStartDate = endDate.AddMilliseconds(1).ToApiDateString();

            // データ0件
            _noteRepo.Setup(m => m.ReadNoteUpdate(_accountKey, startDate.Date, endDate.Date)).Returns(new List<QH_MEDICINE_DAT>());

            var results = _worker.Read(args);

            // データなしでも正常終了する
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
            results.ItemList.Count.Is(0);
            results.NextSyncStartDate.Is(nextStartDate);

        }

        [TestMethod]
        public void 正常に取得_結果あり()
        {
            var args = GetValidArgs();
            SetUpValidMethods(args);
            var endDate = args.SyncEndDate.TryToValueType(DateTime.MinValue);
            var nextDate = endDate.AddMilliseconds(1).ToApiDateString();

            var results = _worker.Read(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);

            // 次候補開始日一致
            results.NextSyncStartDate.Is(nextDate);

            // 結果4件
            results.ItemList.Count.Is(6);

            // ------------------------------------------------------
            // 1件目 調剤薬
            var rec1 = results.ItemList[0];
            rec1.DeleteFlag.Is(false);
            rec1.RecordDate.Is(new DateTime(2024, 10, 1).ToApiDateString());
            rec1.ReceiptNo.Is("2024100101");
            rec1.Sequence.Is(1);
            rec1.LinkageSystemNo.Is(99999);
            rec1.DataIdReference.Is(rec1.ReceiptNo.ToEncrypedReference());
            rec1.DataType.Is((byte)1);
            rec1.OwnerType.Is((byte)2);
            rec1.DetailItems.Count.Is(2); // 詳細は2件
            // 1件目 詳細1件目
            var detail = rec1.DetailItems[0];
            detail.PrescriptionNo.Is(1);
            detail.RpSetNo.Is(1);
            detail.RecordDate.Is(new DateTime(2024, 10, 1).ToApiDateString());
            detail.DataType.Is((byte)1);
            detail.OwnerType.Is((byte)2);
            detail.PharmacyNo.Is(int.MinValue);
            detail.PharmacyName.Is("QOLMS薬局");
            detail.PharmacistName.Is("薬剤史朗");
            detail.HospitalName.Is("QOLMS病院");
            detail.DepartmentName.Is("内科");
            detail.DoctorName.Is("医者太郎");
            detail.Memo.Is("メモ1\nメモ2\nメモ3");
            detail.MedicineItems.Count.Is(0);
            // 1件目 詳細1件目 RP情報2件
            detail.RpItems.Count.Is(2);
            // 1件目 詳細1件目 RP情報1件目
            detail.RpItems[0].UsageName.Is("用法");
            detail.RpItems[0].UsageSupplement.Is("用法補足1用法補足2");
            detail.RpItems[0].DosageFormCode.Is("1");
            detail.RpItems[0].Quantity.Is(1);
            detail.RpItems[0].QuantityUnit.Is("日分");            
            detail.RpItems[0].MedicineItems.Count.Is(3);
            // 1件目 詳細1件目 RP情報1件目 薬情報1件目
            detail.RpItems[0].MedicineItems[0].No.Is(1);
            detail.RpItems[0].MedicineItems[0].Name.Is("ロキソニン");
            detail.RpItems[0].MedicineItems[0].Quantity.Is("3");
            detail.RpItems[0].MedicineItems[0].QuantityUnit.Is("錠");
            detail.RpItems[0].MedicineItems[0].YjCode.Is("1149019F1560");
            detail.RpItems[0].MedicineItems[0].EffectCategoryCode.Is("1");
            detail.RpItems[0].MedicineItems[0].EffectCategoryName.Is("鎮痛剤");
            detail.RpItems[0].MedicineItems[0].ItemCode.Is(string.Empty);
            detail.RpItems[0].MedicineItems[0].ItemCodeType.Is(string.Empty);
            // 1件目 詳細1件目 RP情報1件目 薬情報2件目
            detail.RpItems[0].MedicineItems[1].No.Is(2);
            detail.RpItems[0].MedicineItems[1].Name.Is("ルネスタ");
            detail.RpItems[0].MedicineItems[1].Quantity.Is("2");
            detail.RpItems[0].MedicineItems[1].QuantityUnit.Is("錠");
            detail.RpItems[0].MedicineItems[1].YjCode.Is("1129010F3020");
            detail.RpItems[0].MedicineItems[1].EffectCategoryCode.Is("2");
            detail.RpItems[0].MedicineItems[1].EffectCategoryName.Is("向精神薬");
            detail.RpItems[0].MedicineItems[1].ItemCode.Is(string.Empty);
            detail.RpItems[0].MedicineItems[1].ItemCodeType.Is(string.Empty);
            // 1件目 詳細1件目 RP情報1件目 薬情報3件目
            detail.RpItems[0].MedicineItems[2].No.Is(3);
            detail.RpItems[0].MedicineItems[2].Name.Is("タケキャブ");
            detail.RpItems[0].MedicineItems[2].Quantity.Is("1");
            detail.RpItems[0].MedicineItems[2].QuantityUnit.Is("錠");
            detail.RpItems[0].MedicineItems[2].YjCode.Is("2329030F1020");
            detail.RpItems[0].MedicineItems[2].EffectCategoryCode.Is("3");
            detail.RpItems[0].MedicineItems[2].EffectCategoryName.Is("胃腸薬");
            detail.RpItems[0].MedicineItems[2].ItemCode.Is(string.Empty);
            detail.RpItems[0].MedicineItems[2].ItemCodeType.Is(string.Empty);
            // 1件目 詳細1件目 RP情報2件目
            detail.RpItems[1].UsageName.Is("用法B");
            detail.RpItems[1].UsageSupplement.Is("補足B1補足B2");
            detail.RpItems[1].DosageFormCode.Is("2");
            detail.RpItems[1].Quantity.Is(2);
            detail.RpItems[1].QuantityUnit.Is("回分");
            detail.RpItems[1].MedicineItems.Count.Is(1); // 薬1件
            // 1件目 詳細1件目 RP情報2件目 薬情報1件目
            detail.RpItems[1].MedicineItems[0].No.Is(1);
            detail.RpItems[1].MedicineItems[0].Name.Is("ガチフロ点眼液");
            detail.RpItems[1].MedicineItems[0].Quantity.Is("2");
            detail.RpItems[1].MedicineItems[0].QuantityUnit.Is("滴");
            detail.RpItems[1].MedicineItems[0].YjCode.Is("1319749Q1030");
            detail.RpItems[1].MedicineItems[0].EffectCategoryCode.Is("4");
            detail.RpItems[1].MedicineItems[0].EffectCategoryName.Is("点眼");
            detail.RpItems[1].MedicineItems[0].ItemCode.Is(string.Empty);
            detail.RpItems[1].MedicineItems[0].ItemCodeType.Is(string.Empty);           

            // 1件目 詳細2件目
            detail = rec1.DetailItems[1];
            detail.PrescriptionNo.Is(1);
            detail.RpSetNo.Is(2);
            detail.RecordDate.Is(new DateTime(2024, 10, 1).ToApiDateString());
            detail.DataType.Is((byte)1);
            detail.OwnerType.Is((byte)2);
            detail.PharmacyNo.Is(int.MinValue);
            detail.PharmacyName.Is("QOLMS薬局");
            detail.PharmacistName.Is("薬剤史朗");
            detail.HospitalName.Is("QOLMS病院");
            detail.DepartmentName.Is("外科");
            detail.DoctorName.Is("医者二郎");
            detail.Memo.Is("メモ1\nメモ2\nメモ3");
            detail.MedicineItems.Count.Is(0);
            // 1件目 詳細2件目 RP情報1件
            detail.RpItems.Count.Is(1);
            // 1件目 詳細2件目 RP情報1件目
            detail.RpItems[0].UsageName.Is("用法C");
            detail.RpItems[0].UsageSupplement.Is("補足C1");
            detail.RpItems[0].DosageFormCode.Is("3");
            detail.RpItems[0].Quantity.Is(3);
            detail.RpItems[0].QuantityUnit.Is("調剤");
            detail.RpItems[0].MedicineItems.Count.Is(1); // 薬1件
            // 1件目 詳細2件目 RP情報1件目 薬情報1件目
            detail.RpItems[0].MedicineItems[0].No.Is(1);
            detail.RpItems[0].MedicineItems[0].Name.Is("ボルタレンテープ１５ｍｇ");
            detail.RpItems[0].MedicineItems[0].Quantity.Is("1");
            detail.RpItems[0].MedicineItems[0].QuantityUnit.Is("枚");
            detail.RpItems[0].MedicineItems[0].YjCode.Is("2649734S1074");
            detail.RpItems[0].MedicineItems[0].EffectCategoryCode.Is("5");
            detail.RpItems[0].MedicineItems[0].EffectCategoryName.Is("鎮痛剤");
            detail.RpItems[0].MedicineItems[0].ItemCode.Is(string.Empty);
            detail.RpItems[0].MedicineItems[0].ItemCodeType.Is(string.Empty);
            // ------------------------------------------------------
            // 2件目 市販薬
            var rec2 = results.ItemList[1];
            rec2.DeleteFlag.Is(false);
            rec2.RecordDate.Is(new DateTime(2024, 10, 1).ToApiDateString());
            rec2.ReceiptNo.Is("2024100102");
            rec2.Sequence.Is(2);
            rec2.LinkageSystemNo.Is(99999);
            rec2.DataIdReference.Is(rec2.ReceiptNo.ToEncrypedReference());
            rec2.DataType.Is((byte)2);
            rec2.OwnerType.Is((byte)1);
            rec2.DetailItems.Count.Is(1); // 詳細は1件 市販薬は必ず1件
            // 2件目 詳細1件目
            var detail2 = rec2.DetailItems[0];
            detail2.PrescriptionNo.Is(1);
            detail2.RpSetNo.Is(1);
            detail2.RecordDate.Is(new DateTime(2024, 10, 1).ToApiDateString());
            detail2.DataType.Is((byte)2);
            detail2.OwnerType.Is((byte)1);
            detail2.PharmacyNo.Is(1111);
            detail2.PharmacyName.Is("市販薬局B"); // JahisでなくMEDICINESET優先(同じになるはずではある)
            detail2.PharmacistName.Is(string.Empty);
            detail2.HospitalName.Is(string.Empty);
            detail2.DepartmentName.Is(string.Empty);
            detail2.DoctorName.Is(string.Empty);
            detail2.Memo.Is("Hoge"); // MEDICINESET優先
            detail2.RpItems.Count.Is(0); // 調剤関連はなし
            detail2.MedicineItems.Count.Is(2); // 市販薬情報2件
            // 2件目 詳細1件目 薬情報1件目
            detail2.MedicineItems[0].No.Is(1);
            detail2.MedicineItems[0].Name.Is("ロキソニンB");
            detail2.MedicineItems[0].Quantity.Is(string.Empty); // 数量・単位は市販薬では対象外
            detail2.MedicineItems[0].QuantityUnit.Is(string.Empty);
            detail2.MedicineItems[0].YjCode.Is(string.Empty); // 対象外
            detail2.MedicineItems[0].EffectCategoryCode.Is(string.Empty); // 対象外
            detail2.MedicineItems[0].EffectCategoryName.Is(string.Empty); // 対象外
            detail2.MedicineItems[0].ItemCode.Is("1000");
            detail2.MedicineItems[0].ItemCodeType.Is("A");
            // 2件目 詳細1件目 薬情報2件目
            detail2.MedicineItems[1].No.Is(2);
            detail2.MedicineItems[1].Name.Is("アレグラ");
            detail2.MedicineItems[1].Quantity.Is(string.Empty); // 数量・単位は市販薬では対象外
            detail2.MedicineItems[1].QuantityUnit.Is(string.Empty);
            detail2.MedicineItems[1].YjCode.Is(string.Empty); // 対象外
            detail2.MedicineItems[1].EffectCategoryCode.Is(string.Empty); // 対象外
            detail2.MedicineItems[1].EffectCategoryName.Is(string.Empty); // 対象外
            detail2.MedicineItems[1].ItemCode.Is("2000");
            detail2.MedicineItems[1].ItemCodeType.Is("B");

            // ------------------------------------------------------
            // 3件目 調剤薬 削除データ
            var rec3 = results.ItemList[2];
            rec3.DeleteFlag.Is(true);
            rec3.RecordDate.Is(new DateTime(2024, 10, 2).ToApiDateString());
            rec3.ReceiptNo.Is("2024100201");
            rec3.Sequence.Is(1);
            rec3.LinkageSystemNo.Is(99999);
            rec3.DataIdReference.Is(rec3.ReceiptNo.ToEncrypedReference());
            rec3.DataType.Is((byte)1);
            rec3.OwnerType.Is((byte)2);
            rec3.DetailItems.Count.Is(2); // 詳細は2件
            // 3件目 詳細1件目
            var detail3 = rec3.DetailItems[0];
            detail3.PrescriptionNo.Is(1);
            detail3.RpSetNo.Is(1);
            detail3.RecordDate.Is(new DateTime(2024, 10, 2).ToApiDateString());
            detail3.DataType.Is((byte)1);
            detail3.OwnerType.Is((byte)2);
            detail3.PharmacyNo.Is(int.MinValue);
            detail3.PharmacyName.Is("QOLMS薬局");
            detail3.PharmacistName.Is("薬剤史朗");
            detail3.HospitalName.Is("QOLMS病院");
            detail3.DepartmentName.Is("内科");
            detail3.DoctorName.Is("医者太郎");
            detail3.Memo.Is("メモ1\nメモ2\nメモ3");
            detail3.MedicineItems.Count.Is(0);
            // 3件目 詳細1件目 RP情報2件
            detail3.RpItems.Count.Is(2);
            // 3件目 詳細1件目 RP情報1件目
            detail3.RpItems[0].UsageName.Is("用法");
            detail3.RpItems[0].UsageSupplement.Is("用法補足1用法補足2");
            detail3.RpItems[0].DosageFormCode.Is("1");
            detail3.RpItems[0].Quantity.Is(1);
            detail3.RpItems[0].QuantityUnit.Is("日分");
            detail3.RpItems[0].MedicineItems.Count.Is(3);
            // 3件目 詳細1件目 RP情報1件目 薬情報1件目
            detail3.RpItems[0].MedicineItems[0].No.Is(1);
            detail3.RpItems[0].MedicineItems[0].Name.Is("ロキソニン");
            detail3.RpItems[0].MedicineItems[0].Quantity.Is("3");
            detail3.RpItems[0].MedicineItems[0].QuantityUnit.Is("錠");
            detail3.RpItems[0].MedicineItems[0].YjCode.Is("1149019F1560");
            detail3.RpItems[0].MedicineItems[0].EffectCategoryCode.Is("1");
            detail3.RpItems[0].MedicineItems[0].EffectCategoryName.Is("鎮痛剤");
            detail3.RpItems[0].MedicineItems[0].ItemCode.Is(string.Empty);
            detail3.RpItems[0].MedicineItems[0].ItemCodeType.Is(string.Empty);
            // 3件目 詳細1件目 RP情報1件目 薬情報2件目
            detail3.RpItems[0].MedicineItems[1].No.Is(2);
            detail3.RpItems[0].MedicineItems[1].Name.Is("ルネスタ");
            detail3.RpItems[0].MedicineItems[1].Quantity.Is("2");
            detail3.RpItems[0].MedicineItems[1].QuantityUnit.Is("錠");
            detail3.RpItems[0].MedicineItems[1].YjCode.Is("1129010F3020");
            detail3.RpItems[0].MedicineItems[1].EffectCategoryCode.Is("2");
            detail3.RpItems[0].MedicineItems[1].EffectCategoryName.Is("向精神薬");
            detail3.RpItems[0].MedicineItems[1].ItemCode.Is(string.Empty);
            detail3.RpItems[0].MedicineItems[1].ItemCodeType.Is(string.Empty);
            // 3件目 詳細1件目 RP情報1件目 薬情報3件目
            detail3.RpItems[0].MedicineItems[2].No.Is(3);
            detail3.RpItems[0].MedicineItems[2].Name.Is("タケキャブ");
            detail3.RpItems[0].MedicineItems[2].Quantity.Is("1");
            detail3.RpItems[0].MedicineItems[2].QuantityUnit.Is("錠");
            detail3.RpItems[0].MedicineItems[2].YjCode.Is("2329030F1020");
            detail3.RpItems[0].MedicineItems[2].EffectCategoryCode.Is("3");
            detail3.RpItems[0].MedicineItems[2].EffectCategoryName.Is("胃腸薬");
            detail3.RpItems[0].MedicineItems[2].ItemCode.Is(string.Empty);
            detail3.RpItems[0].MedicineItems[2].ItemCodeType.Is(string.Empty);
            // 3件目 詳細1件目 RP情報2件目
            detail3.RpItems[1].UsageName.Is("用法B");
            detail3.RpItems[1].UsageSupplement.Is("補足B1補足B2");
            detail3.RpItems[1].DosageFormCode.Is("2");
            detail3.RpItems[1].Quantity.Is(2);
            detail3.RpItems[1].QuantityUnit.Is("回分");
            detail3.RpItems[1].MedicineItems.Count.Is(1); // 薬1件
            // 3件目 詳細1件目 RP情報2件目 薬情報1件目
            detail3.RpItems[1].MedicineItems[0].No.Is(1);
            detail3.RpItems[1].MedicineItems[0].Name.Is("ガチフロ点眼液");
            detail3.RpItems[1].MedicineItems[0].Quantity.Is("2");
            detail3.RpItems[1].MedicineItems[0].QuantityUnit.Is("滴");
            detail3.RpItems[1].MedicineItems[0].YjCode.Is("1319749Q1030");
            detail3.RpItems[1].MedicineItems[0].EffectCategoryCode.Is("4");
            detail3.RpItems[1].MedicineItems[0].EffectCategoryName.Is("点眼");
            detail3.RpItems[1].MedicineItems[0].ItemCode.Is(string.Empty);
            detail3.RpItems[1].MedicineItems[0].ItemCodeType.Is(string.Empty);

            // 3件目 詳細2件目
            detail3 = rec3.DetailItems[1];
            detail3.PrescriptionNo.Is(1);
            detail3.RpSetNo.Is(2);
            detail3.RecordDate.Is(new DateTime(2024, 10, 2).ToApiDateString());
            detail3.DataType.Is((byte)1);
            detail3.OwnerType.Is((byte)2);
            detail3.PharmacyNo.Is(int.MinValue);
            detail3.PharmacyName.Is("QOLMS薬局");
            detail3.PharmacistName.Is("薬剤史朗");
            detail3.HospitalName.Is("QOLMS病院");
            detail3.DepartmentName.Is("外科");
            detail3.DoctorName.Is("医者二郎");
            detail3.Memo.Is("メモ1\nメモ2\nメモ3");
            detail3.MedicineItems.Count.Is(0);
            // 3件目 詳細2件目 RP情報1件
            detail3.RpItems.Count.Is(1);
            // 3件目 詳細2件目 RP情報1件目
            detail3.RpItems[0].UsageName.Is("用法C");
            detail3.RpItems[0].UsageSupplement.Is("補足C1");
            detail3.RpItems[0].DosageFormCode.Is("3");
            detail3.RpItems[0].Quantity.Is(3);
            detail3.RpItems[0].QuantityUnit.Is("調剤");
            detail3.RpItems[0].MedicineItems.Count.Is(1); // 薬1件
            // 3件目 詳細2件目 RP情報1件目 薬情報1件目
            detail3.RpItems[0].MedicineItems[0].No.Is(1);
            detail3.RpItems[0].MedicineItems[0].Name.Is("ボルタレンテープ１５ｍｇ");
            detail3.RpItems[0].MedicineItems[0].Quantity.Is("1");
            detail3.RpItems[0].MedicineItems[0].QuantityUnit.Is("枚");
            detail3.RpItems[0].MedicineItems[0].YjCode.Is("2649734S1074");
            detail3.RpItems[0].MedicineItems[0].EffectCategoryCode.Is("5");
            detail3.RpItems[0].MedicineItems[0].EffectCategoryName.Is("鎮痛剤");
            detail3.RpItems[0].MedicineItems[0].ItemCode.Is(string.Empty);
            detail3.RpItems[0].MedicineItems[0].ItemCodeType.Is(string.Empty);

            // ------------------------------------------------------
            // 4件目 調剤薬
            var rec4 = results.ItemList[3];
            rec4.DeleteFlag.Is(false);
            rec4.RecordDate.Is(new DateTime(2024, 9, 30).ToApiDateString());
            rec4.ReceiptNo.Is("2024093001");
            rec4.Sequence.Is(1);
            rec4.LinkageSystemNo.Is(99999);
            rec4.DataIdReference.Is(rec4.ReceiptNo.ToEncrypedReference());
            rec4.DataType.Is((byte)100);
            rec4.OwnerType.Is((byte)101);
            rec4.DetailItems.Count.Is(1); // 詳細は1件
            // 4件目 詳細1件目
            var detail4 = rec4.DetailItems[0];
            detail4.PrescriptionNo.Is(1);
            detail4.RpSetNo.Is(1);
            detail4.RecordDate.Is(new DateTime(2024, 9, 30).ToApiDateString());
            detail4.DataType.Is((byte)100);
            detail4.OwnerType.Is((byte)101);
            detail4.PharmacyNo.Is(int.MinValue);
            detail4.PharmacyName.Is("MGF薬局");
            detail4.PharmacistName.Is("薬剤花子");
            detail4.HospitalName.Is("MGF病院");
            detail4.DepartmentName.Is("耳鼻咽喉科");
            detail4.DoctorName.Is("医者花子");
            detail4.Memo.Is("メモ1");
            detail4.MedicineItems.Count.Is(0);
            // 4件目 詳細1件目 RP情報1件
            detail4.RpItems.Count.Is(1);
            // 4件目 詳細1件目 RP情報1件目
            detail4.RpItems[0].UsageName.Is("用法D");
            detail4.RpItems[0].UsageSupplement.Is("補足D1");
            detail4.RpItems[0].DosageFormCode.Is("5");
            detail4.RpItems[0].Quantity.Is(1);
            detail4.RpItems[0].QuantityUnit.Is("日分");
            detail4.RpItems[0].MedicineItems.Count.Is(1);
            // 4件目 詳細1件目 RP情報1件目 薬情報1件目
            detail4.RpItems[0].MedicineItems[0].No.Is(1);
            detail4.RpItems[0].MedicineItems[0].Name.Is("メリスロン錠１２ｍｇ");
            detail4.RpItems[0].MedicineItems[0].Quantity.Is("2");
            detail4.RpItems[0].MedicineItems[0].QuantityUnit.Is("錠");
            detail4.RpItems[0].MedicineItems[0].YjCode.Is("1339005F2128");
            detail4.RpItems[0].MedicineItems[0].EffectCategoryCode.Is("3");
            detail4.RpItems[0].MedicineItems[0].EffectCategoryName.Is("抗めまい剤");
            detail4.RpItems[0].MedicineItems[0].ItemCode.Is(string.Empty);
            detail4.RpItems[0].MedicineItems[0].ItemCodeType.Is(string.Empty);

            // ------------------------------------------------------
            // 5件目 調剤薬 削除データ
            var rec5 = results.ItemList[4];
            rec5.DeleteFlag.Is(true);
            rec5.RecordDate.Is(new DateTime(2024, 10, 15).ToApiDateString());
            rec5.ReceiptNo.Is("2024101501");
            rec5.Sequence.Is(1);
            rec5.LinkageSystemNo.Is(99999);
            rec5.DataIdReference.Is(rec5.ReceiptNo.ToEncrypedReference());
            rec5.DataType.Is((byte)100);
            rec5.OwnerType.Is((byte)101);
            rec5.DetailItems.Count.Is(2); // 詳細は2件
            // 5件目 詳細1件目
            var detail5 = rec5.DetailItems[0];
            detail5.PrescriptionNo.Is(1);
            detail5.RpSetNo.Is(1);
            detail5.RecordDate.Is(new DateTime(2024, 10, 15).ToApiDateString());
            detail5.DataType.Is((byte)100);
            detail5.OwnerType.Is((byte)101);
            detail5.PharmacyNo.Is(int.MinValue);
            detail5.PharmacyName.Is("QOLMS薬局");
            detail5.PharmacistName.Is("薬剤史朗");
            detail5.HospitalName.Is("QOLMS病院");
            detail5.DepartmentName.Is("循環器内科");
            detail5.DoctorName.Is("医者太郎");
            detail5.Memo.Is("メモ1\nメモ2\nメモ3");
            detail5.MedicineItems.Count.Is(0);
            // 5件目 詳細1件目 RP情報2件
            detail5.RpItems.Count.Is(2);
            // 5件目 詳細1件目 RP情報1件目
            detail5.RpItems[0].UsageName.Is("用法2");
            detail5.RpItems[0].UsageSupplement.Is("用法補足1用法補足2");
            detail5.RpItems[0].DosageFormCode.Is("5");
            detail5.RpItems[0].Quantity.Is(11);
            detail5.RpItems[0].QuantityUnit.Is("日分");
            detail5.RpItems[0].MedicineItems.Count.Is(3);
            // 5件目 詳細1件目 RP情報1件目 薬情報1件目
            detail5.RpItems[0].MedicineItems[0].No.Is(1);
            detail5.RpItems[0].MedicineItems[0].Name.Is("ロキソニン");
            detail5.RpItems[0].MedicineItems[0].Quantity.Is("13");
            detail5.RpItems[0].MedicineItems[0].QuantityUnit.Is("錠");
            detail5.RpItems[0].MedicineItems[0].YjCode.Is("1149019F1560");
            detail5.RpItems[0].MedicineItems[0].EffectCategoryCode.Is("1");
            detail5.RpItems[0].MedicineItems[0].EffectCategoryName.Is("鎮痛剤");
            detail5.RpItems[0].MedicineItems[0].ItemCode.Is(string.Empty);
            detail5.RpItems[0].MedicineItems[0].ItemCodeType.Is(string.Empty);
            // 5件目 詳細1件目 RP情報1件目 薬情報2件目
            detail5.RpItems[0].MedicineItems[1].No.Is(2);
            detail5.RpItems[0].MedicineItems[1].Name.Is("ルネスタ");
            detail5.RpItems[0].MedicineItems[1].Quantity.Is("12");
            detail5.RpItems[0].MedicineItems[1].QuantityUnit.Is("錠");
            detail5.RpItems[0].MedicineItems[1].YjCode.Is("1129010F3020");
            detail5.RpItems[0].MedicineItems[1].EffectCategoryCode.Is("2");
            detail5.RpItems[0].MedicineItems[1].EffectCategoryName.Is("向精神薬");
            detail5.RpItems[0].MedicineItems[1].ItemCode.Is(string.Empty);
            detail5.RpItems[0].MedicineItems[1].ItemCodeType.Is(string.Empty);
            // 5件目 詳細1件目 RP情報1件目 薬情報3件目
            detail5.RpItems[0].MedicineItems[2].No.Is(3);
            detail5.RpItems[0].MedicineItems[2].Name.Is("タケキャブ");
            detail5.RpItems[0].MedicineItems[2].Quantity.Is("11");
            detail5.RpItems[0].MedicineItems[2].QuantityUnit.Is("錠");
            detail5.RpItems[0].MedicineItems[2].YjCode.Is("2329030F1020");
            detail5.RpItems[0].MedicineItems[2].EffectCategoryCode.Is("3");
            detail5.RpItems[0].MedicineItems[2].EffectCategoryName.Is("胃腸薬");
            detail5.RpItems[0].MedicineItems[2].ItemCode.Is(string.Empty);
            detail5.RpItems[0].MedicineItems[2].ItemCodeType.Is(string.Empty);

            // 5件目 詳細1件目 RP情報2件目
            detail5.RpItems[1].UsageName.Is("用法2-B");
            detail5.RpItems[1].UsageSupplement.Is("補足B1補足B2");
            detail5.RpItems[1].DosageFormCode.Is("2");
            detail5.RpItems[1].Quantity.Is(12);
            detail5.RpItems[1].QuantityUnit.Is("回分");
            detail5.RpItems[1].MedicineItems.Count.Is(1); // 薬1件
            // 5件目 詳細1件目 RP情報2件目 薬情報1件目
            detail5.RpItems[1].MedicineItems[0].No.Is(1);
            detail5.RpItems[1].MedicineItems[0].Name.Is("ガチフロ点眼液");
            detail5.RpItems[1].MedicineItems[0].Quantity.Is("12");
            detail5.RpItems[1].MedicineItems[0].QuantityUnit.Is("滴");
            detail5.RpItems[1].MedicineItems[0].YjCode.Is("1319749Q1030");
            detail5.RpItems[1].MedicineItems[0].EffectCategoryCode.Is("4");
            detail5.RpItems[1].MedicineItems[0].EffectCategoryName.Is("点眼");
            detail5.RpItems[1].MedicineItems[0].ItemCode.Is(string.Empty);
            detail5.RpItems[1].MedicineItems[0].ItemCodeType.Is(string.Empty);

            // 5件目 詳細2件目
            detail5 = rec5.DetailItems[1];
            detail5.PrescriptionNo.Is(1);
            detail5.RpSetNo.Is(2);
            detail5.RecordDate.Is(new DateTime(2024, 10, 15).ToApiDateString());
            detail5.DataType.Is((byte)100);
            detail5.OwnerType.Is((byte)101);
            detail5.PharmacyNo.Is(int.MinValue);
            detail5.PharmacyName.Is("QOLMS薬局");
            detail5.PharmacistName.Is("薬剤史朗");
            detail5.HospitalName.Is("QOLMS病院");
            detail5.DepartmentName.Is("整形外科");
            detail5.DoctorName.Is("医者二郎");
            detail5.Memo.Is("メモ1\nメモ2\nメモ3");
            detail5.MedicineItems.Count.Is(0);
            // 5件目 詳細2件目 RP情報1件
            detail5.RpItems.Count.Is(1);
            // 5件目 詳細2件目 RP情報1件目
            detail5.RpItems[0].UsageName.Is("用法C");
            detail5.RpItems[0].UsageSupplement.Is("補足C1");
            detail5.RpItems[0].DosageFormCode.Is("3");
            detail5.RpItems[0].Quantity.Is(13);
            detail5.RpItems[0].QuantityUnit.Is("調剤");
            detail5.RpItems[0].MedicineItems.Count.Is(1); // 薬1件
            // 5件目 詳細2件目 RP情報1件目 薬情報1件目
            detail5.RpItems[0].MedicineItems[0].No.Is(1);
            detail5.RpItems[0].MedicineItems[0].Name.Is("ボルタレンテープ１５ｍｇ");
            detail5.RpItems[0].MedicineItems[0].Quantity.Is("11");
            detail5.RpItems[0].MedicineItems[0].QuantityUnit.Is("枚");
            detail5.RpItems[0].MedicineItems[0].YjCode.Is("2649734S1074");
            detail5.RpItems[0].MedicineItems[0].EffectCategoryCode.Is("5");
            detail5.RpItems[0].MedicineItems[0].EffectCategoryName.Is("鎮痛剤");
            detail5.RpItems[0].MedicineItems[0].ItemCode.Is(string.Empty);
            detail5.RpItems[0].MedicineItems[0].ItemCodeType.Is(string.Empty);

            // ------------------------------------------------------
            // 6件目 調剤薬
            var rec6 = results.ItemList[5];
            rec6.DeleteFlag.Is(false);
            rec6.RecordDate.Is(new DateTime(2024, 10, 31, 23, 59, 59).ToApiDateString());
            rec6.ReceiptNo.Is("2024103101");
            rec6.Sequence.Is(1);
            rec6.LinkageSystemNo.Is(99999);
            rec6.DataIdReference.Is(rec6.ReceiptNo.ToEncrypedReference());
            rec6.DataType.Is((byte)100);
            rec6.OwnerType.Is((byte)101);
            rec6.DetailItems.Count.Is(1); // 詳細は1件
            // 6件目 詳細1件目
            var detail6 = rec6.DetailItems[0];
            detail6.PrescriptionNo.Is(1);
            detail6.RpSetNo.Is(1);
            detail6.RecordDate.Is(new DateTime(2024,09, 30).ToApiDateString());
            detail6.DataType.Is((byte)100);
            detail6.OwnerType.Is((byte)101);
            detail6.PharmacyNo.Is(int.MinValue);
            detail6.PharmacyName.Is("MGF薬局");
            detail6.PharmacistName.Is("薬剤花子");
            detail6.HospitalName.Is("MGF病院");
            detail6.DepartmentName.Is("耳鼻咽喉科");
            detail6.DoctorName.Is("医者花子");
            detail6.Memo.Is("メモ1");
            detail6.MedicineItems.Count.Is(0);
            // 4件目 詳細1件目 RP情報1件
            detail6.RpItems.Count.Is(1);
            // 4件目 詳細1件目 RP情報1件目
            detail6.RpItems[0].UsageName.Is("用法D");
            detail6.RpItems[0].UsageSupplement.Is("補足D1");
            detail6.RpItems[0].DosageFormCode.Is("5");
            detail6.RpItems[0].Quantity.Is(1);
            detail6.RpItems[0].QuantityUnit.Is("日分");
            detail6.RpItems[0].MedicineItems.Count.Is(1);
            // 4件目 詳細1件目 RP情報1件目 薬情報1件目
            detail6.RpItems[0].MedicineItems[0].No.Is(1);
            detail6.RpItems[0].MedicineItems[0].Name.Is("メリスロン錠１２ｍｇ");
            detail6.RpItems[0].MedicineItems[0].Quantity.Is("2");
            detail6.RpItems[0].MedicineItems[0].QuantityUnit.Is("錠");
            detail6.RpItems[0].MedicineItems[0].YjCode.Is("1339005F2128");
            detail6.RpItems[0].MedicineItems[0].EffectCategoryCode.Is("3");
            detail6.RpItems[0].MedicineItems[0].EffectCategoryName.Is("抗めまい剤");
            detail6.RpItems[0].MedicineItems[0].ItemCode.Is(string.Empty);
            detail6.RpItems[0].MedicineItems[0].ItemCodeType.Is(string.Empty);

        }

        void SetUpValidMethods(QoNoteMedicineSyncUpdateReadApiArgs args)
        {
            // 更新情報セットアップ
            var noteMedicineEntities = CreateTestUpdateData();

            var startDate = args.SyncStartDate.TryToValueType(DateTime.MinValue);
            var endDate = args.SyncEndDate.TryToValueType(DateTime.MinValue);

            _noteRepo.Setup(m => m.ReadNoteUpdate(_accountKey, startDate, endDate)).Returns(noteMedicineEntities);

            // 薬効分類セットアップ
            _noteRepo.Setup(m => m.ReadYakkoList(It.IsAny<List<string>>())).Returns(new List<DbEthicalDrugCategoryItem>
            {
                new DbEthicalDrugCategoryItem
                {
                    YjCode = "1149019F1560",
                    MinorClassCode = "1",
                    MinorClassName = "鎮痛剤"
                },
                new DbEthicalDrugCategoryItem
                {
                    YjCode = "1129010F3020",
                    MinorClassCode = "2",
                    MinorClassName = "向精神薬"
                },
                new DbEthicalDrugCategoryItem
                {
                    YjCode = "2329030F1020",
                    MinorClassCode = "3",
                    MinorClassName = "胃腸薬"
                },
                new DbEthicalDrugCategoryItem
                {
                    YjCode = "1319749Q1030",
                    MinorClassCode = "4",
                    MinorClassName = "点眼"
                },
                new DbEthicalDrugCategoryItem
                {
                    YjCode = "2649734S1074",
                    MinorClassCode = "5",
                    MinorClassName = "鎮痛剤"
                },
                new DbEthicalDrugCategoryItem
                {
                    YjCode = "1339005F2128",
                    MinorClassCode = "3",
                    MinorClassName = "抗めまい剤"
                }

            }); 
        }

        QoNoteMedicineSyncUpdateReadApiArgs GetValidArgs()
        {
            return new QoNoteMedicineSyncUpdateReadApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "NoteMedicine",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.QolmsiOSApp}",
                SyncStartDate = _startDate.ToApiDateString(),
                SyncEndDate = _endDate.ToApiDateString()
            };
        }

        List<QH_MEDICINE_DAT> CreateTestUpdateData()
        {
            // 更新情報セットアップ
            var jahisList = new List<JahisMessageTestBuilder>
            {
                new JahisMessageTestBuilder(new DateTime(2024, 10, 1))
                .SetHospital("QOLMS病院")
                .SetPharmacy("QOLMS薬局", "薬剤史朗")
                .SetPatientMemo("メモ1", "メモ2", "メモ3")
                .AddRpSet(rpset =>
                {
                    rpset
                    .SetDoctor("医者太郎")
                    .SetDepartment("内科")
                    .AddRp(rp =>
                    {
                        rp.SetUsage("用法", "用法補足1", "用法補足2")
                        .SetDosageFormCode("1")
                        .SetQuantity(1, "日分")
                        .AddMedicine("ロキソニン", "3", "錠", "1149019F1560")
                        .AddMedicine("ルネスタ", "2", "錠", "1129010F3020")
                        .AddMedicine("タケキャブ", "1", "錠", "2329030F1020");
                    })
                    .AddRp(rp =>{
                        rp.SetUsage("用法B","補足B1","補足B2")
                        .SetDosageFormCode("2")
                        .SetQuantity(2, "回分")
                        .AddMedicine("ガチフロ点眼液","2","滴","1319749Q1030");
                    });
                })
                .AddRpSet(rpset =>
                {
                    rpset
                    .SetDoctor("医者二郎")
                    .SetDepartment("外科")
                    .AddRp(rp =>
                    {
                        rp.SetUsage("用法C","補足C1")
                        .SetDosageFormCode("3")
                        .SetQuantity(3, "調剤")
                        .AddMedicine("ボルタレンテープ１５ｍｇ","1","枚","2649734S1074");
                    });
                }),

                new JahisMessageTestBuilder(new DateTime(2024,10,1))
                .SetPharmacy("市販薬局A","")
                .SetNoteMemo("メモ1","メモ2")
                .AddOtcMedicine("ロキソニンA")
                .AddOtcMedicine("アレグラ"),

                new JahisMessageTestBuilder(new DateTime(2024, 10, 2))
                .SetHospital("QOLMS病院")
                .SetPharmacy("QOLMS薬局", "薬剤史朗")
                .SetPatientMemo("メモ1", "メモ2", "メモ3")
                .AddRpSet(rpset =>
                {
                    rpset
                    .SetDoctor("医者太郎")
                    .SetDepartment("内科")
                    .AddRp(rp =>
                    {
                        rp.SetUsage("用法", "用法補足1", "用法補足2")
                        .SetDosageFormCode("1")
                        .SetQuantity(1, "日分")
                        .AddMedicine("ロキソニン", "3", "錠", "1149019F1560")
                        .AddMedicine("ルネスタ", "2", "錠", "1129010F3020")
                        .AddMedicine("タケキャブ", "1", "錠", "2329030F1020");
                    })
                    .AddRp(rp =>{
                        rp.SetUsage("用法B","補足B1","補足B2")
                        .SetDosageFormCode("2")
                        .SetQuantity(2, "回分")
                        .AddMedicine("ガチフロ点眼液","2","滴","1319749Q1030");
                    });
                })
                .AddRpSet(rpset =>
                {
                    rpset
                    .SetDoctor("医者二郎")
                    .SetDepartment("外科")
                    .AddRp(rp =>
                    {
                        rp.SetUsage("用法C","補足C1")
                        .SetDosageFormCode("3")
                        .SetQuantity(3, "調剤")
                        .AddMedicine("ボルタレンテープ１５ｍｇ","1","枚","2649734S1074");
                    });
                }),

                new JahisMessageTestBuilder(new DateTime(2024,9,30))
                .SetHospital("MGF病院")
                .SetPharmacy("MGF薬局", "薬剤花子")
                .SetPatientMemo("メモ1")
                .AddRpSet(rpset =>
                {
                    rpset
                    .SetDoctor("医者花子")
                    .SetDepartment("耳鼻咽喉科")
                    .AddRp(rp =>
                    {
                        rp.SetUsage("用法D", "補足D1")
                        .SetDosageFormCode("5")
                        .SetQuantity(1, "日分")
                        .AddMedicine("メリスロン錠１２ｍｇ", "2", "錠", "1339005F2128");
                    });
                }),

                new JahisMessageTestBuilder(new DateTime(2024, 10, 15))
                .SetHospital("QOLMS病院")
                .SetPharmacy("QOLMS薬局", "薬剤史朗")
                .SetPatientMemo("メモ1", "メモ2", "メモ3")
                .AddRpSet(rpset =>
                {
                    rpset
                    .SetDoctor("医者太郎")
                    .SetDepartment("循環器内科")
                    .AddRp(rp =>
                    {
                        rp.SetUsage("用法2", "用法補足1", "用法補足2")
                        .SetDosageFormCode("5")
                        .SetQuantity(11, "日分")
                        .AddMedicine("ロキソニン", "13", "錠", "1149019F1560")
                        .AddMedicine("ルネスタ", "12", "錠", "1129010F3020")
                        .AddMedicine("タケキャブ", "11", "錠", "2329030F1020");
                    })
                    .AddRp(rp =>{
                        rp.SetUsage("用法2-B","補足B1","補足B2")
                        .SetDosageFormCode("2")
                        .SetQuantity(12, "回分")
                        .AddMedicine("ガチフロ点眼液","12","滴","1319749Q1030");
                    });
                })
                .AddRpSet(rpset =>
                {
                    rpset
                    .SetDoctor("医者二郎")
                    .SetDepartment("整形外科")
                    .AddRp(rp =>
                    {
                        rp.SetUsage("用法C","補足C1")
                        .SetDosageFormCode("3")
                        .SetQuantity(13, "調剤")
                        .AddMedicine("ボルタレンテープ１５ｍｇ","11","枚","2649734S1074");
                    });
                }),
            };

            var otcDrugList = new List<QhMedicineSetOtcDrugOfJson>
            {
                new QhMedicineSetOtcDrugOfJson
                {
                    PharmacyNo = 1111,
                    PharmacyName = "市販薬局B",
                    Comment = "Hoge",
                    MedicineN = new List<QhMedicineSetOtcDrugItemOfJson>
                    {
                        new QhMedicineSetOtcDrugItemOfJson
                        {
                            MedicineName = "ロキソニンB",
                            ItemCode = "1000",
                            ItemCodeType = "A",                            
                        },
                        new QhMedicineSetOtcDrugItemOfJson
                        {
                            MedicineName = "アレグラ",
                            ItemCode = "2000",
                            ItemCodeType = "B"
                        }
                    }
                }
            };

            return new List<QH_MEDICINE_DAT>
            {
                /// 3件目、５件目は削除データ
                new QH_MEDICINE_DAT
                {
                    RECORDDATE = new DateTime(2024,10,1),
                    SEQUENCE = 1,
                    RECEIPTNO = "2024100101",
                    PHARMACYNO = int.MinValue,
                    LINKAGESYSTEMNO = 99999,
                    DATATYPE = 1,
                    OWNERTYPE = 2,
                    CONVERTEDMEDICINESET = jahisList[0].BuildCrypted(),
                    DELETEFLAG = false
                }, 
                new QH_MEDICINE_DAT
                {
                    RECORDDATE = new DateTime(2024,10,1),
                    SEQUENCE = 2,
                    RECEIPTNO = "2024100102",
                    LINKAGESYSTEMNO = 99999,
                    PHARMACYNO = 1111,
                    DATATYPE = 2,
                    OWNERTYPE = 1,
                    CONVERTEDMEDICINESET = jahisList[1].BuildCrypted(),
                    MEDICINESET = new QsJsonSerializer().Serialize(otcDrugList[0]).TryEncrypt(),
                    DELETEFLAG = false
                },
                new QH_MEDICINE_DAT
                {
                    RECORDDATE = new DateTime(2024,10,2),
                    SEQUENCE = 1,
                    RECEIPTNO = "2024100201",
                    PHARMACYNO = int.MinValue,
                    LINKAGESYSTEMNO = 99999,
                    DATATYPE = 1,
                    OWNERTYPE = 2,
                    CONVERTEDMEDICINESET = jahisList[2].BuildCrypted(),
                    DELETEFLAG = true
                },
                new QH_MEDICINE_DAT
                {
                    RECORDDATE = new DateTime(2024,9,30),
                    SEQUENCE = 1,
                    RECEIPTNO = "2024093001",
                    PHARMACYNO = int.MinValue,
                    LINKAGESYSTEMNO = 99999,
                    DATATYPE = 100,
                    OWNERTYPE = 101,
                    CONVERTEDMEDICINESET = jahisList[3].BuildCrypted(),
                    DELETEFLAG = false
                },
                new QH_MEDICINE_DAT
                {
                    RECORDDATE = new DateTime(2024,10,15),
                    SEQUENCE = 1,
                    RECEIPTNO = "2024101501",
                    PHARMACYNO = int.MinValue,
                    LINKAGESYSTEMNO = 99999,
                    DATATYPE = 100,
                    OWNERTYPE = 101,
                    CONVERTEDMEDICINESET = jahisList[4].BuildCrypted(),
                    DELETEFLAG = true
                },
                new QH_MEDICINE_DAT
                {
                    RECORDDATE = new DateTime(2024,10,31,23,59,59),
                    SEQUENCE = 1,
                    RECEIPTNO = "2024103101",
                    PHARMACYNO = int.MinValue,
                    LINKAGESYSTEMNO = 99999,
                    DATATYPE = 100,
                    OWNERTYPE = 101,
                    CONVERTEDMEDICINESET = jahisList[3].BuildCrypted(),
                    DELETEFLAG = false
                },
            };
        }
    }
}
