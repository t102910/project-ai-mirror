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
        public void FromJahis_市販薬ではエラー()
        {
            var args = GetValidJhahisArgs();
            args.OwnerType = (byte)OwnerTypeEnum.Oneself;
            args.DataType = (byte)DataTypeEnum.OtcDrug;

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains("JahisDataは市販薬には対応していません").IsTrue();
        }

        [TestMethod]
        public void FromsJahis_Jahisに変換失敗でエラー()
        {
            var args = GetValidJhahisArgs();
            args.JahisData = "hoge"; // 無効なフォーマット

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains("Jahisフォーマットの変換に失敗しました").IsTrue();
        }

        [TestMethod]
        public void FromsJahis_JahisバリデーションNGでエラー()
        {
            var args = GetValidJhahisArgs();

            // バージョン値が不正
            args.JahisData = new QsJsonSerializer().Serialize(
                new JahisMessageBuilder((JMVersionTypeEnum)99)
                .AddPrescription(preCfg =>
                {
                    preCfg
                    .SetDate(DateTime.Today)
                    .AddRpSet(rpSet => {
                        rpSet.SetDoctor("医者太郎")
                        .SetDepartment("内科")
                        .AddRp(rp =>
                        {                            
                        });
                    });
                })                
                .Build()
            );

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains("JahisDataの検証でエラーが発生しました").IsTrue();
            results.Result.Detail.Contains("バージョン情報が不正です").IsTrue();
        }

        [TestMethod]
        public void FromsJahis_処方情報無しでエラー()
        {
            var args = GetValidJhahisArgs();

            var jahis = new JahisMessageBuilder(JMVersionTypeEnum.Latest)
                .AddPrescription(preCfg =>
                {
                    preCfg.AddRpSet(rpSet => {
                        rpSet.SetDoctor("医者太郎")
                        .SetDepartment("内科")
                        .AddRp(rp =>
                        {
                        });
                    });
                })
                .Build();

            // 処方情報を消す
            jahis.Prescription_List.Clear();
            
            args.JahisData = new QsJsonSerializer().Serialize(jahis);               

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains("処方情報が存在しません").IsTrue();
        }

        [TestMethod]
        public void FromJahis_調剤日が不正でエラー()
        {
            var args = GetValidJhahisArgs();
            var jahis = new JahisMessageBuilder(JMVersionTypeEnum.Latest)
                .AddPrescription(preCfg =>
                {
                    preCfg.AddRpSet(rpSet => {
                        rpSet.SetDoctor("医者太郎")
                        .SetDepartment("内科")
                        .AddRp(rp =>
                        {
                        });
                    });
                })
                .Build();
            // 調剤日不正
            jahis.Prescription_List[0].No005 = new JM_No005
            {
                No005_1 = "5",
                No005_2 = "abcdef99",
                No005_3 = "2"
            };

            args.JahisData = new QsJsonSerializer().Serialize(jahis);

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains("調剤日が不正です").IsTrue();
        }

        [TestMethod]
        public void FromJahis_データ登録処理で例外が発生するとエラー()
        {
            var args = GetValidJhahisArgs();

            string refDataId = string.Empty;
            string refErrMsg = string.Empty;

            // 例外を起こす
            _noteRepo.Setup(m => m.WriteMedicine(_accountKey, _authorKey, It.IsAny<string>(), 1, _targetDate, 99999, It.IsAny<string>(), 2, 12345, ref refDataId, ref refErrMsg)).Throws(new Exception());

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("お薬手帳情報の登録処理でエラーが発生しました").IsTrue();
        }

        [TestMethod]
        public void FromJahis_データ登録処理で失敗するとエラー()
        {
            var args = GetValidJhahisArgs();

            string refDataId = string.Empty;
            string refErrMsg = string.Empty;

            // 戻り値失敗
            _noteRepo.Setup(m => m.WriteMedicine(_accountKey, _authorKey, It.IsAny<string>(), 1, _targetDate, 99999, It.IsAny<string>(), 2, 12345, ref refDataId, ref refErrMsg)).Returns((false, false, 0));

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            results.Result.Detail.Contains("お薬手帳データの登録処理に失敗しました").IsTrue();
        }

        [TestMethod]
        public void FromJahis_部分的成功_戻り値用のデータ取得で例外()
        {
            var args = GetValidJhahisArgs();
            SetUpValideJahisMethods();

            // 例外を起こす (DBエラーの可能性）
            _noteRepo.Setup(m => m.ReadEntity(_accountKey, _targetDate, 1)).Throws(new Exception());

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("4000");
            results.Result.Detail.Contains("正常に登録できましたが、戻り値用のデータは取得できませんでした").IsTrue();
            results.Result.Detail.Contains("お薬手帳データの取得に失敗しました。").IsTrue();
        }

        [TestMethod]
        public void FromJahis_部分的成功_戻り値用のデータ取得でデータなし()
        {
            var args = GetValidJhahisArgs();
            SetUpValideJahisMethods();

            // データなし（同時実行で削除されたりする場合はありえる)
            _noteRepo.Setup(m => m.ReadEntity(_accountKey, _targetDate, 1)).Returns(default(QH_MEDICINE_DAT));

            var results = _worker.Add(args);

            results.IsSuccess.Is(bool.TrueString);
            results.Result.Code.Is("4000");
            results.Result.Detail.Contains("正常に登録できましたが、戻り値用のデータは取得できませんでした").IsTrue();
            results.Result.Detail.Contains("お薬手帳データが存在しませんでした。").IsTrue();
        }

        [TestMethod]
        public void FromJahis_成功()
        {
            var args = GetValidJhahisArgs();
            SetUpValideJahisMethods();

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
            _callbackEthMedicineSet.FacilityId.Is("5678");
            _callbackEthMedicineSet.PharmacyName.Is("QOLMS薬局");
            _callbackEthMedicineSet.PharmacistName.Is("薬剤師　花子");
            _callbackEthMedicineSet.LeftoverMedicine.Is("残薬1残薬2");
            _callbackEthMedicineSet.LeftoverMedicineAuthor.Is("2");
            _callbackEthMedicineSet.SpecialNotes.Is("備考1備考2");
            _callbackEthMedicineSet.Memo.Is("メモ1メモ2");

            var medicineUsageN = _callbackEthMedicineSet.MedicineUsageN;
            medicineUsageN.Count.Is(2);
            medicineUsageN[0].DoctorName.Is("田中　一郎");
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
            medicineUsageN[1].MedicineN[0].MedicineName.Is("マイスリー錠５ｍｇ");
            medicineUsageN[1].MedicineN[0].CodeSystem.Is((byte)4);
            medicineUsageN[1].MedicineN[0].MedicineCode.Is("1129009F1025");
            medicineUsageN[1].MedicineN[0].ReceiptNumber.Is((byte)2);
            medicineUsageN[1].MedicineN[0].Dose.Is("1");
            medicineUsageN[1].MedicineN[0].Unit.Is("錠");
            // MedicineSetにはPrescriptionの1番目のみ採用する

            var prescription = _callbackJahis.Prescription_List;
            // 患者等記入情報が追記されている(改行で分解)
            prescription[0].No601_List[0].No601_2.Is("メモ1");
            prescription[0].No601_List[1].No601_2.Is("メモ2");
            prescription[1].No601_List[0].No601_2.Is("メモ3");
            prescription[1].No601_List[1].No601_2.Is("メモ4");
        }

        // CallbackでRefを扱うためのデリゲート
        delegate void WriteCallback(Guid accountKey, Guid authorKey, string jahis, byte dataType, DateTime recordDate, int linkageSystemNo, string medicineSet, int ownerType, int pharmacyNo, ref string dataId, ref string error);

        void SetUpValideJahisMethods()
        {
            string refDataId = string.Empty;
            string refErrMsg = string.Empty;

            // 書き込み成功しSEQ1を返す
            _noteRepo.Setup(m => m.WriteMedicine(_accountKey, _authorKey, It.IsAny<string>(), 1, _targetDate, 99999, It.IsAny<string>(), 2, 12345, ref refDataId, ref refErrMsg)).Returns((true, true, 1)).Callback(new WriteCallback((Guid a, Guid b, string jahis,byte dataType, DateTime recordDate,int f, string medicineSet, int ownerType, int pharmacyNo, ref string j ,ref string k) =>
            {
                _callbackRecordDate = recordDate;
                _callbackPharmacyNo = pharmacyNo;

                var qsJson = new QsJsonSerializer();
                _callbackJahis = qsJson.Deserialize<JM_Message>(jahis);
                if(dataType == 2)
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

        QoNoteMedicineSyncDataAddApiArgs GetValidJhahisArgs()
        {
            return new QoNoteMedicineSyncDataAddApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),
                Executor = _authorKey.ToApiGuidString(),
                ExecutorName = "NoteMedicine",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.QolmsiOSApp}",
                LinkageSystemNo = 99999,
                DataType = (byte)DataTypeEnum.EthicalDrug,
                OwnerType = (byte)OwnerTypeEnum.QrCode,
                JahisData = CreateValidJahisData(),
                JahisMemoList = new List<string> { "メモ1\nメモ2", "メモ3\r\nメモ4" },
                Data = null
            };
        }

        

        string CreateValidJahisData()
        {
            var message = new JahisMessageBuilder(JMVersionTypeEnum.Latest)
            .SetPatientData("山田　太郎", "ヤマダ　タロウ", 1, new DateTime(1999, 1, 1))
            .AddPrescription(preCfg =>
            {
                preCfg
                .SetDate(_targetDate)
                .SetHospital("QOLMS病院","5678")
                .SetPharmacy("QOLMS薬局", "薬剤師　花子",12345)
                .SetBikou("備考1","備考2")
                .SetZanyaku("残薬1","残薬2")
                .AddRpSet(rpsetCfg =>
                {
                    rpsetCfg
                    .SetDoctor("田中　一郎")
                    .SetDepartment("内科")
                    .AddRp(rpCfg =>
                    {
                        rpCfg
                        .SetUsage("毎食後", "6時間空ける")
                        .SetQuantity(30, "日分")
                        .SetDosageFormCode("1")
                        .AddMedicine("ロキソニン錠６０ｍｇ", "3", "錠", "1149019F1560")
                        .AddMedicine("ボルタレン錠２５ｍｇ", "6", "錠", "1147002F1560");
                    })
                    .AddRp(rpCfg =>
                    {
                        rpCfg
                        .SetUsage("寝る前", "")
                        .SetQuantity(10, "日分")
                        .SetDosageFormCode("1")
                        .AddMedicine("マイスリー錠５ｍｇ", "1", "錠", "1129009F1025");
                    });
                });
            })
            .AddPrescription(preCfg =>
            {
                preCfg
                .SetDate(_targetDate)
                .SetHospital("MGF病院", "1234")
                .SetPharmacy("MGF薬局", "薬剤師　太郎", 12349)
                .AddRpSet(rpsetCfg =>
                {
                    rpsetCfg
                    .SetDoctor("山田　二郎")
                    .SetDepartment("消化器科")
                    .AddRp(rpCfg =>
                    {
                        rpCfg
                        .SetUsage("痛い時", "6時間空ける")
                        .SetQuantity(10, "回分")
                        .SetDosageFormCode("1")
                        .AddMedicine("ロキソニン錠６０ｍｇ", "3", "錠", "1149019F1560");
                    });
                });
            })
            .Build();

            return new QsJsonSerializer().Serialize(message);
        }
    }
}
