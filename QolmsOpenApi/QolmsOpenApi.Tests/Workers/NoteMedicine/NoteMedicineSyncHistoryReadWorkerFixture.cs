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
    public class NoteMedicineSyncHistoryReadWorkerFixture
    {
        const int DefaultDays = 15;
        NoteMedicineSyncHistoryReadWorker _worker;
        Mock<INoteMedicineRepository> _noteRepo;
        Guid _accountKey = Guid.NewGuid();
        DateTime _startDate = new DateTime(2024, 10, 1, 10, 30, 15);

        [TestInitialize]
        public void Initialize()
        {
            _noteRepo = new Mock<INoteMedicineRepository>();
            _worker = new NoteMedicineSyncHistoryReadWorker(_noteRepo.Object);
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
            args.StartDate = "hoge";

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.StartDate)).IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();
        }

        [TestMethod]
        public void Daysが負の値の場合はエラーとなる()
        {
            var args = GetValidArgs();
            args.Days = -1;

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.Days)).IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();
        }

        [TestMethod]
        public void お薬手帳情報の取得で例外が発生するとエラー()
        {
            var args = GetValidArgs();
            var startDate = args.StartDate.TryToValueType(DateTime.MinValue);
            var endDate = startDate.Date.AddDays(-(DefaultDays-1));

            // 例外発生
            _noteRepo.Setup(m => m.ReadNoteHistory(_accountKey, startDate.Date, endDate.Date)).Throws(new Exception());

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
        public void お薬手帳情報次候補取得処理で例外が発生するとエラー()
        {
            var args = GetValidArgs();
            SetUpValidMethods(args);

            // 次候補取得処理で例外を発生させる
            var startDate = args.StartDate.TryToValueType(DateTime.MinValue).Date.AddDays(-args.Days);
            _noteRepo.Setup(m => m.ReadNoteHistoryNext(_accountKey, startDate)).Throws(new Exception());

            var results = _worker.Read(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("お薬手帳データの次候補取得に失敗しました").IsTrue();
        }

        [TestMethod]
        public void 日数が0の場合は規定値が設定される()
        {
            var args = GetValidArgs();
            args.Days = 0;
            SetUpValidMethods(args);

            var startDate = args.StartDate.TryToValueType(DateTime.MinValue);
            var endDate = startDate.Date.AddDays(-29);
            // 規定値(30)設定
            _noteRepo.Setup(m => m.ReadNoteHistory(_accountKey, startDate.Date, endDate.Date)).Returns(new List<QH_MEDICINE_DAT>());

            var results = _worker.Read(args);

            // 規定値で呼び出された
            _noteRepo.Verify(m => m.ReadNoteHistory(_accountKey, startDate.Date, endDate.Date), Times.Once);

            // これ以外の検証はここでは行わない
        }

        [TestMethod]
        public void 正常終了_結果0件の場合()
        {
            var args = GetValidArgs();
            SetUpValidMethods(args);

            var startDate = args.StartDate.TryToValueType(DateTime.MinValue);
            var endDate = startDate.Date.AddDays(-(args.Days - 1));

            // データ0件
            _noteRepo.Setup(m => m.ReadNoteHistory(_accountKey, startDate.Date, endDate.Date)).Returns(new List<QH_MEDICINE_DAT>());

            var results = _worker.Read(args);

            // データなしでも正常終了する
            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");
            results.HistoryList.Count.Is(0);

            // 次候補あり(初回指定範囲にはなくてもより古いデータがあることはある)
            results.HasNextData.IsTrue();           
        }

        [TestMethod]
        public void 正常に取得_次候補あり()
        {
            var args = GetValidArgs();
            SetUpValidMethods(args);

            var results = _worker.Read(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);

            // データ取得日時が設定されている(DateTime.Now値のため何らかの値が入っていたらよしとする)
            results.FetchedAt.TryToValueType(DateTime.MinValue).IsNot(DateTime.MinValue);
            // 次候補あり
            results.HasNextData.IsTrue();
            // 次候補開始日一致
            results.NextStartDate.Is(new DateTime(2024, 8, 15).ToApiDateString());

            // 結果3件
            results.HistoryList.Count.Is(3);

            // ------------------------------------------------------
            // 1件目 調剤薬
            var rec1 = results.HistoryList[0];
            rec1.RecordDate.Is(new DateTime(2024, 10, 1).ToApiDateString());
            rec1.ReceiptNo.Is("2024100101");
            rec1.Sequence.Is(1);
            rec1.LinkageSystemNo.Is(99999);
            rec1.DataIdReference.Is(rec1.ReceiptNo.ToEncrypedReference());
            rec1.DataType.Is((byte)1);
            rec1.OwnerType.Is((byte)2);
            rec1.DetailItems.Count.Is(3); // 詳細は3件(Prescription2件 / RPSet計3件)
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

            // 1件目 詳細3件目 (処方2件目)
            detail = rec1.DetailItems[2];
            detail.PrescriptionNo.Is(2);
            detail.RpSetNo.Is(1);
            detail.RecordDate.Is(new DateTime(2024, 10, 2).ToApiDateString());
            detail.DataType.Is((byte)1);
            detail.OwnerType.Is((byte)2);
            detail.PharmacyNo.Is(int.MinValue);
            detail.PharmacyName.Is("MGF薬局");
            detail.PharmacistName.Is("薬剤史朗");
            detail.HospitalName.Is("MGF病院");
            detail.DepartmentName.Is("皮膚科");
            detail.DoctorName.Is("医者太郎");
            detail.Memo.Is("メモ1");
            detail.MedicineItems.Count.Is(0);
            // 1件目 詳細3件目 RP情報1件
            detail.RpItems.Count.Is(1);
            // 1件目 詳細3件目 RP情報1件目
            detail.RpItems[0].UsageName.Is("用法");
            detail.RpItems[0].UsageSupplement.Is("用法補足1");
            detail.RpItems[0].DosageFormCode.Is("1");
            detail.RpItems[0].Quantity.Is(1);
            detail.RpItems[0].QuantityUnit.Is("日分");
            detail.RpItems[0].MedicineItems.Count.Is(1); // 薬1件
            // 1件目 詳細3件目 RP情報1件目 薬情報1件目
            detail.RpItems[0].MedicineItems[0].No.Is(1);
            detail.RpItems[0].MedicineItems[0].Name.Is("ロキソニン");
            detail.RpItems[0].MedicineItems[0].Quantity.Is("3");
            detail.RpItems[0].MedicineItems[0].QuantityUnit.Is("錠");
            detail.RpItems[0].MedicineItems[0].YjCode.Is("1149019F1560");
            detail.RpItems[0].MedicineItems[0].EffectCategoryCode.Is("1");
            detail.RpItems[0].MedicineItems[0].EffectCategoryName.Is("鎮痛剤");
            detail.RpItems[0].MedicineItems[0].ItemCode.Is(string.Empty);
            detail.RpItems[0].MedicineItems[0].ItemCodeType.Is(string.Empty);

            // ------------------------------------------------------
            // 2件目 市販薬
            var rec2 = results.HistoryList[1];
            rec2.RecordDate.Is(new DateTime(2024, 10, 1).ToApiDateString());
            rec2.ReceiptNo.Is("2024100102");
            rec2.Sequence.Is(2);
            rec2.LinkageSystemNo.Is(99999);
            rec2.DataIdReference.Is(rec2.ReceiptNo.ToEncrypedReference());
            rec2.DataType.Is((byte)2);
            rec2.OwnerType.Is((byte)1);
            rec2.DetailItems.Count.Is(1); // 詳細は1件 市販薬は必ず1件
            // 2件目 詳細1件目
            detail = rec2.DetailItems[0];
            detail.PrescriptionNo.Is(1); // 市販薬は1固定
            detail.RpSetNo.Is(1);        // 同上
            detail.RecordDate.Is(new DateTime(2024, 10, 1).ToApiDateString());
            detail.DataType.Is((byte)2);
            detail.OwnerType.Is((byte)1);
            detail.PharmacyNo.Is(1111);
            detail.PharmacyName.Is("市販薬局B"); // JahisでなくMEDICINESET優先(同じになるはずではある)
            detail.PharmacistName.Is(string.Empty);
            detail.HospitalName.Is(string.Empty);
            detail.DepartmentName.Is(string.Empty);
            detail.DoctorName.Is(string.Empty);
            detail.Memo.Is("Hoge"); // MEDICINESET優先
            detail.RpItems.Count.Is(0); // 調剤関連はなし
            detail.MedicineItems.Count.Is(2); // 市販薬情報2件
            // 2件目 詳細1件目 薬情報1件目
            detail.MedicineItems[0].No.Is(1);
            detail.MedicineItems[0].Name.Is("ロキソニンB");
            detail.MedicineItems[0].Quantity.Is(string.Empty); // 数量・単位は市販薬では対象外
            detail.MedicineItems[0].QuantityUnit.Is(string.Empty);
            detail.MedicineItems[0].YjCode.Is(string.Empty); // 対象外
            detail.MedicineItems[0].EffectCategoryCode.Is(string.Empty); // 対象外
            detail.MedicineItems[0].EffectCategoryName.Is(string.Empty); // 対象外
            detail.MedicineItems[0].ItemCode.Is("1000");
            detail.MedicineItems[0].ItemCodeType.Is("A");
            // 2件目 詳細1件目 薬情報2件目
            detail.MedicineItems[1].No.Is(2);
            detail.MedicineItems[1].Name.Is("アレグラ");
            detail.MedicineItems[1].Quantity.Is(string.Empty); // 数量・単位は市販薬では対象外
            detail.MedicineItems[1].QuantityUnit.Is(string.Empty);
            detail.MedicineItems[1].YjCode.Is(string.Empty); // 対象外
            detail.MedicineItems[1].EffectCategoryCode.Is(string.Empty); // 対象外
            detail.MedicineItems[1].EffectCategoryName.Is(string.Empty); // 対象外
            detail.MedicineItems[1].ItemCode.Is("2000");
            detail.MedicineItems[1].ItemCodeType.Is("B");

            // ------------------------------------------------------
            // 3件目 調剤薬
            var rec3 = results.HistoryList[2];
            rec3.RecordDate.Is(new DateTime(2024, 9, 30).ToApiDateString());
            rec3.ReceiptNo.Is("2024093001");
            rec3.Sequence.Is(1);
            rec3.LinkageSystemNo.Is(99999);
            rec3.DataIdReference.Is(rec3.ReceiptNo.ToEncrypedReference());
            rec3.DataType.Is((byte)100);
            rec3.OwnerType.Is((byte)101);
            rec3.DetailItems.Count.Is(1); // 詳細は1件
            // 3件目 詳細1件目
            detail = rec3.DetailItems[0];
            detail.PrescriptionNo.Is(1);
            detail.RpSetNo.Is(1);
            detail.RecordDate.Is(new DateTime(2024, 9, 30).ToApiDateString());
            detail.DataType.Is((byte)100);
            detail.OwnerType.Is((byte)101);
            detail.PharmacyNo.Is(int.MinValue);
            detail.PharmacyName.Is("MGF薬局");
            detail.PharmacistName.Is("薬剤花子");
            detail.HospitalName.Is("MGF病院");
            detail.DepartmentName.Is("耳鼻咽喉科");
            detail.DoctorName.Is("医者花子");
            detail.Memo.Is("メモ1");
            detail.MedicineItems.Count.Is(0);
            // 3件目 詳細1件目 RP情報1件
            detail.RpItems.Count.Is(1);
            // 3件目 詳細1件目 RP情報1件目
            detail.RpItems[0].UsageName.Is("用法D");
            detail.RpItems[0].UsageSupplement.Is("補足D1");
            detail.RpItems[0].DosageFormCode.Is("5");
            detail.RpItems[0].Quantity.Is(1);
            detail.RpItems[0].QuantityUnit.Is("日分");
            detail.RpItems[0].MedicineItems.Count.Is(1);
            // 3件目 詳細1件目 RP情報1件目 薬情報1件目
            detail.RpItems[0].MedicineItems[0].No.Is(1);
            detail.RpItems[0].MedicineItems[0].Name.Is("メリスロン錠１２ｍｇ");
            detail.RpItems[0].MedicineItems[0].Quantity.Is("2");
            detail.RpItems[0].MedicineItems[0].QuantityUnit.Is("錠");
            detail.RpItems[0].MedicineItems[0].YjCode.Is("1339005F2128");
            detail.RpItems[0].MedicineItems[0].EffectCategoryCode.Is("3");
            detail.RpItems[0].MedicineItems[0].EffectCategoryName.Is("抗めまい剤");
            detail.RpItems[0].MedicineItems[0].ItemCode.Is(string.Empty);
            detail.RpItems[0].MedicineItems[0].ItemCodeType.Is(string.Empty);
            
        }

        [TestMethod]
        public void 正常終了_次候補なし()
        {
            var args = GetValidArgs();
            SetUpValidMethods(args);

            var startDate = args.StartDate.TryToValueType(DateTime.MinValue);
            var endDate = startDate.Date.AddDays(-(args.Days - 1));
            // 次候補なし
            _noteRepo.Setup(m => m.ReadNoteHistoryNext(_accountKey, endDate.AddDays(-1))).Returns(default(QH_MEDICINE_DAT));

            var results = _worker.Read(args);

            // 成功
            results.IsSuccess.Is(bool.TrueString);
            // 次候補なし
            results.HasNextData.IsFalse();
            // 次候補開始日なし
            results.NextStartDate.Is(string.Empty);

            // 結果3件
            results.HistoryList.Count.Is(3);

            // 詳細は別テストで実施済みのため省略
        }

        void SetUpValidMethods(QoNoteMedicineSyncHistoryReadApiArgs args)
        {
            // 履歴セットアップ
            var noteMedicineEntities = CreateTestHistoryData();

            var startDate = args.StartDate.TryToValueType(DateTime.MinValue);
            var endDate = startDate.Date.AddDays(-(args.Days - 1));
            
            _noteRepo.Setup(m => m.ReadNoteHistory(_accountKey, startDate.Date, endDate.Date)).Returns(noteMedicineEntities);

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
            

            // 次候補セットアップ
            _noteRepo.Setup(m => m.ReadNoteHistoryNext(_accountKey, endDate.AddDays(-1))).Returns(new QH_MEDICINE_DAT
            {
                RECORDDATE = new DateTime(2024, 8, 15)
            });
                
        }

        QoNoteMedicineSyncHistoryReadApiArgs GetValidArgs()
        {
            return new QoNoteMedicineSyncHistoryReadApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),
                Executor = Guid.NewGuid().ToApiGuidString(),
                ExecutorName = "NoteMedicine",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.QolmsiOSApp}",
                StartDate = _startDate.ToApiDateString(),
                Days = DefaultDays
            };
        }

        List<QH_MEDICINE_DAT> CreateTestHistoryData()
        {
            // 履歴セットアップ
            var jahisList = new List<JahisMessageBuilder>
            {
                new JahisMessageBuilder(JMVersionTypeEnum.Latest)
                .AddPrescription(preCfg =>
                {
                    preCfg
                    .SetDate(new DateTime(2024, 10, 1))
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
                    });
                })
                .AddPrescription(preCfg =>
                {
                    preCfg
                    .SetDate(new DateTime(2024, 10, 2))
                    .SetHospital("MGF病院")
                    .SetPharmacy("MGF薬局", "薬剤史朗")
                    .SetPatientMemo("メモ1")
                    .AddRpSet(rpset =>
                    {
                        rpset
                        .SetDoctor("医者太郎")
                        .SetDepartment("皮膚科")
                        .AddRp(rp =>
                        {
                            rp.SetUsage("用法", "用法補足1")
                            .SetDosageFormCode("1")
                            .SetQuantity(1, "日分")
                            .AddMedicine("ロキソニン", "3", "錠", "1149019F1560");
                        });
                    });
                }),

                new JahisMessageBuilder(JMVersionTypeEnum.Latest)
                .SetNoteMemo("メモ1","メモ2")
                .AddOtcMedicine("ロキソニンA")
                .AddOtcMedicine("アレグラ")
                .AddPrescription(preCfg => {
                    preCfg
                    .SetDate(new DateTime(2024,10,1))
                    .SetPharmacy("市販薬局A","");                    
                }),

                new JahisMessageBuilder(JMVersionTypeEnum.Latest)
                .AddPrescription(preCfg =>
                {
                    preCfg
                    .SetDate(new DateTime(2024,9,30))
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
                new QH_MEDICINE_DAT
                {
                    RECORDDATE = new DateTime(2024,10,1),
                    SEQUENCE = 1,
                    RECEIPTNO = "2024100101",
                    PHARMACYNO = int.MinValue,
                    LINKAGESYSTEMNO = 99999,
                    DATATYPE = 1,
                    OWNERTYPE = 2,
                    CONVERTEDMEDICINESET = jahisList[0].BuildCrypted()
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
                    MEDICINESET = new QsJsonSerializer().Serialize(otcDrugList[0]).TryEncrypt()
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
                    CONVERTEDMEDICINESET = jahisList[2].BuildCrypted()
                },
            };
        }
    }
}
