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
        public void ForEth_Jahisレコードに編集対象が見つからない場合はエラー()
        {
            var args = GetValidEthArgs();
            SetUpValidEthMethods();

            args.Data.RpSetNo = 4; // Jahis内RpSet数3件で該当しない

            var results = _worker.Edit(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("4002"); // 専用エラーコード
            results.Result.Detail.Contains("編集対象の項目が存在しませんでした").IsTrue();
        }

        [TestMethod]
        public void ForEth_更新したJahisデータが検証NGの場合はエラー()
        {
            var args = GetValidEthArgs();
            args.Data.RpSetNo = 1;
            SetUpValidEthMethods();

            _noteRepo.Setup(m => m.ReadEntity(_accountKey, _targetDate, 1))
                .Returns(new QH_MEDICINE_DAT
                {
                    CONVERTEDMEDICINESET = CreateInvalidJahisData(),
                    DATATYPE = 1,
                    OWNERTYPE = 1
                });

            var results = _worker.Edit(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005"); 
            results.Result.Detail.Contains("JahisDataの検証でエラーが発生しました").IsTrue();
        }

        [TestMethod]
        public void ForEth_更新したJahisデータが検証で例外が発生した場合はエラー()
        {
            var args = GetValidEthArgs();
            args.Data.RpSetNo = 1;
            SetUpValidEthMethods();

            _noteRepo.Setup(m => m.ReadEntity(_accountKey, _targetDate, 1))
                .Returns(new QH_MEDICINE_DAT
                {
                    CONVERTEDMEDICINESET = CreateInvalidJahisData(true), // 例外起こすデータ
                    DATATYPE = 1,
                    OWNERTYPE = 1
                });

            var results = _worker.Edit(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            results.Result.Detail.Contains("JahisDataの検証で想定外のエラーが発生しました").IsTrue();
        }

        [TestMethod]
        public void ForEth_データ更新処理で例外が発生するとエラー()
        {
            var args = GetValidEthArgs();
            SetUpValidEthMethods();

            // 例外を起こす
            _noteRepo.Setup(m => m.UpdateEntity(_targetEntity)).Throws(new Exception());

            var results = _worker.Edit(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("お薬手帳情報の更新処理でエラーが発生しました").IsTrue();
        }

        [TestMethod]
        public void ForEth_SSMIXデータで正常終了する()
        {
            var args = GetValidEthArgs();
            args.Data.DataType = (byte)DataTypeEnum.Ssmix;
            args.Data.OwnerType = (byte)OwnerTypeEnum.Data;

            SetUpValidEthMethods(OwnerTypeEnum.Data, DataTypeEnum.Ssmix);

            var results = _worker.Edit(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // DBの更新処理が実行された
            _noteRepo.Verify(m => m.UpdateEntity(_targetEntity), Times.Once);

            // MedicineSetはメモのみ更新
            _callbackEthMedicineSet.PrescriptionDate.Is(_targetDate.ToApiDateString());
            _callbackEthMedicineSet.PharmacyId.Is("12345");
            _callbackEthMedicineSet.FacilityName.Is("QOLMS病院");
            _callbackEthMedicineSet.FacilityId.Is("5678");
            _callbackEthMedicineSet.PharmacyName.Is("QOLMS薬局");
            _callbackEthMedicineSet.PharmacistName.Is("薬剤師　花子");
            _callbackEthMedicineSet.LeftoverMedicine.Is("");
            _callbackEthMedicineSet.LeftoverMedicineAuthor.Is("");
            _callbackEthMedicineSet.SpecialNotes.Is("");
            _callbackEthMedicineSet.Memo.Is("メモ8メモ9");

            // 患者レコードは変化なし
            _callbackJahis.No001.No001_2.Is("山田　太郎");
            _callbackJahis.No001.No001_3.Is("1");
            _callbackJahis.No001.No001_4.Is("19990101");
            _callbackJahis.No001.No001_11.Is("ヤマダ　タロウ");

            // 処方レコード数に変化なし
            _callbackJahis.Prescription_List.Count.Is(2);

            var pre1 = _callbackJahis.Prescription_List[0];

            // メモは更新されている
            pre1.No601_List.Count.Is(2);
            pre1.No601_List[0].No601_2.Is("メモ8");
            pre1.No601_List[1].No601_2.Is("メモ9");

            // 以降メモ以外編集不可なので更新されない

            // 調剤日
            pre1.No005.No005_2.Is(_targetDate.ToString("yyyyMMdd"));

            // 病院・薬剤師
            pre1.No011.No011_2.Is("QOLMS薬局");
            pre1.No015.No015_2.Is("薬剤師　花子");
            pre1.No051.No051_2.Is("QOLMS病院");
            pre1.No051.No051_5.Is("5678");

            // 処方1 RpSet1件目            
            var rpSet1 = pre1.RpSet_List[0];
            rpSet1.No055.No055_2.Is("田中　一郎");
            rpSet1.No055.No055_3.Is("内科");
            rpSet1.Rp_List.Count.Is(2);
            rpSet1.Rp_List[0].No301.No301_3.Is("毎食後");
            rpSet1.Rp_List[1].No301.No301_3.Is("寝る前");

            // 処方1 RpSet2件目
            var rpSet2 = pre1.RpSet_List[1];
            rpSet2.No055.No055_2.Is("山田　一郎");
            rpSet2.No055.No055_3.Is("皮膚科");
            rpSet2.Rp_List.Count.Is(1);
            rpSet2.Rp_List[0].No301.No301_3.Is("痛い時");
            rpSet2.Rp_List[0].No301.No301_4.Is("10");
            rpSet2.Rp_List[0].No301.No301_5.Is("回分");
            rpSet2.Rp_List[0].No301.No301_6.Is("1");
            rpSet2.Rp_List[0].No311_List.Count.Is(1);
            rpSet2.Rp_List[0].No311_List[0].No311_3.Is("6時間空ける");
            var med1 = rpSet2.Rp_List[0].Medicine_List;
            med1.Count.Is(1);
            med1[0].No201.No201_3.Is("ロキソニン錠６０ｍｇ");
            med1[0].No201.No201_4.Is("3");
            med1[0].No201.No201_5.Is("錠");
            med1[0].No201.No201_7.Is("1149019F1560");           


            // 処方2件目
            var pre2 = _callbackJahis.Prescription_List[1];

            // メモ
            pre2.No601_List.Count.Is(1);
            pre2.No601_List[0].No601_2.Is("メモメモ");

            // 調剤日
            pre2.No005.No005_2.Is(_targetDate.ToString("yyyyMMdd"));

            // 病院・薬剤師
            pre2.No011.No011_2.Is("MGF薬局");
            pre2.No015.No015_2.Is("薬剤師　太郎");
            pre2.No051.No051_2.Is("MGF病院");
            pre2.No051.No051_5.Is("1234");

            pre2.RpSet_List.Count.Is(1);

            // 処方2 RpSet1件目            
            var rpSet3 = pre2.RpSet_List[0];

            rpSet3.No055.No055_2.Is("山田　二郎");
            rpSet3.No055.No055_3.Is("消化器科");
            rpSet3.Rp_List.Count.Is(1);
            rpSet3.Rp_List[0].No301.No301_3.Is("痛い時");
            rpSet3.Rp_List[0].No301.No301_4.Is("10");
            rpSet3.Rp_List[0].No301.No301_5.Is("回分");
            rpSet3.Rp_List[0].No301.No301_6.Is("1");
            rpSet3.Rp_List[0].No311_List[0].No311_3.Is("6時間空ける");
            var med3 = rpSet3.Rp_List[0].Medicine_List;
            med3.Count.Is(1);
            med3[0].No201.No201_3.Is("ロキソニン錠６０ｍｇ");
            med3[0].No201.No201_4.Is("3");
            med3[0].No201.No201_5.Is("錠");
            med3[0].No201.No201_7.Is("1149019F1560");
        }

        [TestMethod]
        public void ForEth_QR登録データで正常終了する()
        {
            var args = GetValidEthArgs();
            args.Data.DataType = (byte)DataTypeEnum.EthicalDrug;
            args.Data.OwnerType = (byte)OwnerTypeEnum.QrCode;

            SetUpValidEthMethods(OwnerTypeEnum.QrCode, DataTypeEnum.EthicalDrug);

            var results = _worker.Edit(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // DBの更新処理が実行された
            _noteRepo.Verify(m => m.UpdateEntity(_targetEntity), Times.Once);

            // MedicineSetはメモのみ更新
            _callbackEthMedicineSet.PrescriptionDate.Is(_targetDate.ToApiDateString());
            _callbackEthMedicineSet.PharmacyId.Is("12345");
            _callbackEthMedicineSet.FacilityName.Is("QOLMS病院");
            _callbackEthMedicineSet.FacilityId.Is("5678");
            _callbackEthMedicineSet.PharmacyName.Is("QOLMS薬局");
            _callbackEthMedicineSet.PharmacistName.Is("薬剤師　花子");
            _callbackEthMedicineSet.LeftoverMedicine.Is("");
            _callbackEthMedicineSet.LeftoverMedicineAuthor.Is("");
            _callbackEthMedicineSet.SpecialNotes.Is("");
            _callbackEthMedicineSet.Memo.Is("メモ8メモ9");

            // Jahisデータ

            // 患者レコードは変化なし
            _callbackJahis.No001.No001_2.Is("山田　太郎");
            _callbackJahis.No001.No001_3.Is("1");
            _callbackJahis.No001.No001_4.Is("19990101");
            _callbackJahis.No001.No001_11.Is("ヤマダ　タロウ");

            // 処方レコード数に変化なし
            _callbackJahis.Prescription_List.Count.Is(2);

            var pre1 = _callbackJahis.Prescription_List[0];

            // メモは更新されている
            pre1.No601_List.Count.Is(2);
            pre1.No601_List[0].No601_2.Is("メモ8");
            pre1.No601_List[1].No601_2.Is("メモ9");

            // 以降メモ以外編集不可なので更新されない

            // 調剤日
            pre1.No005.No005_2.Is(_targetDate.ToString("yyyyMMdd"));

            // 病院・薬剤師
            pre1.No011.No011_2.Is("QOLMS薬局");
            pre1.No015.No015_2.Is("薬剤師　花子");
            pre1.No051.No051_2.Is("QOLMS病院");
            pre1.No051.No051_5.Is("5678");

            // 処方1 RpSet1件目            
            var rpSet1 = pre1.RpSet_List[0];
            rpSet1.No055.No055_2.Is("田中　一郎");
            rpSet1.No055.No055_3.Is("内科");
            rpSet1.Rp_List.Count.Is(2);
            rpSet1.Rp_List[0].No301.No301_3.Is("毎食後");
            rpSet1.Rp_List[1].No301.No301_3.Is("寝る前");

            // 処方1 RpSet2件目
            var rpSet2 = pre1.RpSet_List[1];
            rpSet2.No055.No055_2.Is("山田　一郎");
            rpSet2.No055.No055_3.Is("皮膚科");
            rpSet2.Rp_List.Count.Is(1);
            rpSet2.Rp_List[0].No301.No301_3.Is("痛い時");
            rpSet2.Rp_List[0].No301.No301_4.Is("10");
            rpSet2.Rp_List[0].No301.No301_5.Is("回分");
            rpSet2.Rp_List[0].No301.No301_6.Is("1");
            rpSet2.Rp_List[0].No311_List.Count.Is(1);
            rpSet2.Rp_List[0].No311_List[0].No311_3.Is("6時間空ける");
            var med1 = rpSet2.Rp_List[0].Medicine_List;
            med1.Count.Is(1);
            med1[0].No201.No201_3.Is("ロキソニン錠６０ｍｇ");
            med1[0].No201.No201_4.Is("3");
            med1[0].No201.No201_5.Is("錠");
            med1[0].No201.No201_7.Is("1149019F1560");


            // 処方2件目
            var pre2 = _callbackJahis.Prescription_List[1];

            // メモ
            pre2.No601_List.Count.Is(1);
            pre2.No601_List[0].No601_2.Is("メモメモ");

            // 調剤日
            pre2.No005.No005_2.Is(_targetDate.ToString("yyyyMMdd"));

            // 病院・薬剤師
            pre2.No011.No011_2.Is("MGF薬局");
            pre2.No015.No015_2.Is("薬剤師　太郎");
            pre2.No051.No051_2.Is("MGF病院");
            pre2.No051.No051_5.Is("1234");

            pre2.RpSet_List.Count.Is(1);

            // 処方2 RpSet1件目            
            var rpSet3 = pre2.RpSet_List[0];

            rpSet3.No055.No055_2.Is("山田　二郎");
            rpSet3.No055.No055_3.Is("消化器科");
            rpSet3.Rp_List.Count.Is(1);
            rpSet3.Rp_List[0].No301.No301_3.Is("痛い時");
            rpSet3.Rp_List[0].No301.No301_4.Is("10");
            rpSet3.Rp_List[0].No301.No301_5.Is("回分");
            rpSet3.Rp_List[0].No301.No301_6.Is("1");
            rpSet3.Rp_List[0].No311_List[0].No311_3.Is("6時間空ける");
            var med3 = rpSet3.Rp_List[0].Medicine_List;
            med3.Count.Is(1);
            med3[0].No201.No201_3.Is("ロキソニン錠６０ｍｇ");
            med3[0].No201.No201_4.Is("3");
            med3[0].No201.No201_5.Is("錠");
            med3[0].No201.No201_7.Is("1149019F1560");
        }

        [TestMethod]
        public void ForEth_手入力調剤薬データで正常終了する()
        {
            var args = GetValidEthArgs();
            SetUpValidEthMethods();

            var results = _worker.Edit(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("0200");

            // DBの更新処理が実行された
            _noteRepo.Verify(m => m.UpdateEntity(_targetEntity), Times.Once);


            // MedicineSetが更新されている
            _callbackEthMedicineSet.PrescriptionDate.Is(_targetDate.ToApiDateString());
            _callbackEthMedicineSet.PharmacyId.Is("12345");
            _callbackEthMedicineSet.FacilityName.Is("Update病院"); // 更新対象
            _callbackEthMedicineSet.FacilityId.Is("5678");
            _callbackEthMedicineSet.PharmacyName.Is("Update薬局"); // 更新対象
            _callbackEthMedicineSet.PharmacistName.Is("薬剤師　愛子"); // 更新対象
            _callbackEthMedicineSet.LeftoverMedicine.Is("");
            _callbackEthMedicineSet.LeftoverMedicineAuthor.Is("");
            _callbackEthMedicineSet.SpecialNotes.Is("");
            _callbackEthMedicineSet.Memo.Is("メモ8メモ9"); // 更新対象

            // Jahisデータ

            // 患者レコードは変化なし
            _callbackJahis.No001.No001_2.Is("山田　太郎");
            _callbackJahis.No001.No001_3.Is("1");
            _callbackJahis.No001.No001_4.Is("19990101");
            _callbackJahis.No001.No001_11.Is("ヤマダ　タロウ");

            // 処方レコード数に変化なし
            _callbackJahis.Prescription_List.Count.Is(2);

            var pre1 = _callbackJahis.Prescription_List[0];

            // メモは更新されている
            pre1.No601_List.Count.Is(2);
            pre1.No601_List[0].No601_2.Is("メモ8");
            pre1.No601_List[1].No601_2.Is("メモ9");

            // 調剤日変化なし
            pre1.No005.No005_2.Is(_targetDate.ToString("yyyyMMdd"));

            // 病院・薬剤師は更新される
            pre1.No011.No011_2.Is("Update薬局");
            pre1.No015.No015_2.Is("薬剤師　愛子");
            pre1.No051.No051_2.Is("Update病院");
            pre1.No051.No051_5.Is("5678"); // 但し医療機関コードだけはそのまま

            // 処方1 RpSet1件目            
            var rpSet1 = pre1.RpSet_List[0];
            // RpSet1件目は更新対象外なので変化なし
            rpSet1.No055.No055_2.Is("田中　一郎");
            rpSet1.No055.No055_3.Is("内科");
            rpSet1.Rp_List.Count.Is(2);
            rpSet1.Rp_List[0].No301.No301_3.Is("毎食後");
            rpSet1.Rp_List[1].No301.No301_3.Is("寝る前");

            // 処方1 RpSet2件目
            // 更新対象なのでまるまる置き換わっている
            var rpSet2 = pre1.RpSet_List[1];
            rpSet2.No055.No055_2.Is("山田　三郎");
            rpSet2.No055.No055_3.Is("Update科");
            rpSet2.Rp_List.Count.Is(2);
            rpSet2.Rp_List[0].No301.No301_3.Is("食前");
            rpSet2.Rp_List[0].No301.No301_4.Is("3");
            rpSet2.Rp_List[0].No301.No301_5.Is("日分");
            rpSet2.Rp_List[0].No301.No301_6.Is("1");
            rpSet2.Rp_List[0].No311_List.Count.Is(1);
            rpSet2.Rp_List[0].No311_List[0].No311_3.Is("空腹時");
            var med1 = rpSet2.Rp_List[0].Medicine_List;
            med1.Count.Is(2);
            med1[0].No201.No201_3.Is("MGF錠");
            med1[0].No201.No201_4.Is("1");
            med1[0].No201.No201_5.Is("カプセル");
            med1[0].No201.No201_7.Is("1111");
            med1[1].No201.No201_3.Is("Qolms錠");
            med1[1].No201.No201_4.Is("2");
            med1[1].No201.No201_5.Is("カプセル");
            med1[1].No201.No201_7.Is("2222");
            rpSet2.Rp_List[1].No301.No301_3.Is("食後");
            rpSet2.Rp_List[1].No301.No301_4.Is("3");
            rpSet2.Rp_List[1].No301.No301_5.Is("日分");
            rpSet2.Rp_List[1].No301.No301_6.Is("1");
            rpSet2.Rp_List[1].No311_List.Count.Is(0);            
            var med2 = rpSet2.Rp_List[1].Medicine_List;
            med2.Count.Is(1);
            med2[0].No201.No201_3.Is("ロキソニン");
            med2[0].No201.No201_4.Is("1");
            med2[0].No201.No201_5.Is("錠");
            med2[0].No201.No201_7.Is("3333");


            // 処方2件目
            var pre2 = _callbackJahis.Prescription_List[1];
            
            // 以降更新なし

            // メモ
            pre2.No601_List.Count.Is(1);
            pre2.No601_List[0].No601_2.Is("メモメモ");

            // 調剤日
            pre2.No005.No005_2.Is(_targetDate.ToString("yyyyMMdd"));

            // 病院・薬剤師
            pre2.No011.No011_2.Is("MGF薬局");
            pre2.No015.No015_2.Is("薬剤師　太郎");
            pre2.No051.No051_2.Is("MGF病院");
            pre2.No051.No051_5.Is("1234");

            pre2.RpSet_List.Count.Is(1);

            // 処方2 RpSet1件目            
            var rpSet3 = pre2.RpSet_List[0];

            rpSet3.No055.No055_2.Is("山田　二郎");
            rpSet3.No055.No055_3.Is("消化器科");
            rpSet3.Rp_List.Count.Is(1);
            rpSet3.Rp_List[0].No301.No301_3.Is("痛い時");
            rpSet3.Rp_List[0].No301.No301_4.Is("10");
            rpSet3.Rp_List[0].No301.No301_5.Is("回分");
            rpSet3.Rp_List[0].No301.No301_6.Is("1");
            rpSet3.Rp_List[0].No311_List[0].No311_3.Is("6時間空ける");
            var med3 = rpSet3.Rp_List[0].Medicine_List;
            med3.Count.Is(1);
            med3[0].No201.No201_3.Is("ロキソニン錠６０ｍｇ");
            med3[0].No201.No201_4.Is("3");
            med3[0].No201.No201_5.Is("錠");
            med3[0].No201.No201_7.Is("1149019F1560");

        }

        [TestMethod]
        public void ForEth_部分的成功_戻り値用のデータ取得で例外()
        {
            var args = GetValidEthArgs();
            SetUpValidEthMethods();

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

        void SetUpValidEthMethods(OwnerTypeEnum o = OwnerTypeEnum.Oneself, DataTypeEnum d = DataTypeEnum.EthicalDrug)
        {
            _targetEntity = new QH_MEDICINE_DAT
            {
                ACCOUNTKEY = _accountKey,
                RECORDDATE = _targetDate,
                SEQUENCE = 1,
                CONVERTEDMEDICINESET = CreateValidJahisData(),
                MEDICINESET = "",
                PHARMACYNO = 12345,
                DATATYPE = (byte)d,
                OWNERTYPE = (int)o
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

            // 薬効分類セットアップ
            _noteRepo.Setup(m => m.ReadYakkoList(It.IsAny<List<string>>())).Returns(new List<DbEthicalDrugCategoryItem>());
        }       

        
    }    
}
