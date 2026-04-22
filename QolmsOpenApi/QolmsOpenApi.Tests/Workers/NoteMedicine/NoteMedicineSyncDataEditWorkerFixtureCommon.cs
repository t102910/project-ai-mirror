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
    public partial class NoteMedicineSyncDataEditWorkerFixture
    {
        NoteMedicineSyncDataEditWorker _worker;
        Mock<INoteMedicineRepository> _noteRepo;
        Guid _accountKey = Guid.NewGuid();
        Guid _authorKey = Guid.NewGuid();
        DateTime _targetDate = new DateTime(2025, 2, 1);
        QH_MEDICINE_DAT _targetEntity;
        JM_Message _callbackJahis;
        QhMedicineSetOtcDrugOfJson _callbackOtcMedicineSet;
        QhMedicineSetEthicalDrugOfJson _callbackEthMedicineSet;
        int _callbackPharmacyNo;
        DateTime _callbackRecordDate;

        [TestInitialize]
        public void Initialize()
        {
            _noteRepo = new Mock<INoteMedicineRepository>();
            _worker = new NoteMedicineSyncDataEditWorker(_noteRepo.Object);
        }

        [TestMethod]
        public void アカウントキーが不正でエラーとなる()
        {
            var args = GetValidArgs();
            args.ActorKey = "invalidGuid";

            var results = _worker.Edit(args);

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

            var results = _worker.Edit(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.AuthorKey)).IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();
        }

        [TestMethod]
        public void Dataが未設定でエラーとなる()
        {
            var args = GetValidArgs();
            args.Data = null;

            var results = _worker.Edit(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.Data)).IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();
        }

        [TestMethod]
        [DataRow((OwnerTypeEnum)4)] // 未定義値
        [DataRow(OwnerTypeEnum.None)]
        public void オーナータイプが定義外またはNoneの場合はエラー(OwnerTypeEnum ownerType)
        {
            var args = GetValidArgs();
            args.Data.OwnerType = (byte)ownerType;

            var results = _worker.Edit(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.Data.OwnerType)).IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();
        }

        [TestMethod]
        [DataRow((DataTypeEnum)5)] // 未定義値
        [DataRow(DataTypeEnum.None)]
        public void データタイプが定義外またはNoneの場合はエラー(DataTypeEnum dataType)
        {
            var args = GetValidArgs();
            args.Data.DataType = (byte)dataType;

            var results = _worker.Edit(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.Data.DataType)).IsTrue();
            results.Result.Detail.Contains("不正").IsTrue();
        }

        [TestMethod]
        [DataRow(OwnerTypeEnum.TextReader)]
        public void オーナータイプが1_2_101以外の場合はエラー(OwnerTypeEnum ownerType)
        {
            var args = GetValidArgs();
            args.Data.OwnerType = (byte)ownerType; // NG値

            var results = _worker.Edit(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.Data.OwnerType)).IsTrue();
            results.Result.Detail.Contains("未対応").IsTrue();
        }

        [TestMethod]
        [DataRow(OwnerTypeEnum.Oneself, DataTypeEnum.Ssmix)]
        [DataRow(OwnerTypeEnum.Oneself, DataTypeEnum.OtcDrugPhoto)]
        [DataRow(OwnerTypeEnum.QrCode, DataTypeEnum.OtcDrug)]
        [DataRow(OwnerTypeEnum.QrCode, DataTypeEnum.Ssmix)]
        [DataRow(OwnerTypeEnum.Data, DataTypeEnum.EthicalDrug)]
        [DataRow(OwnerTypeEnum.Data, DataTypeEnum.OtcDrug)]
        [DataRow(OwnerTypeEnum.Data, DataTypeEnum.OtcDrugPhoto)]
        public void オーナータイプとデータタイプの組み合わせ不正でエラー(QH_MEDICINE_DAT.OwnerTypeEnum ownerType, QH_MEDICINE_DAT.DataTypeEnum dataType)
        {
            var args = GetValidArgs();
            args.Data.OwnerType = (byte)ownerType;
            args.Data.DataType = (byte)dataType;

            var results = _worker.Edit(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1002"); // 引数エラー
            results.Result.Detail.Contains(nameof(args.Data.OwnerType)).IsTrue();
            results.Result.Detail.Contains(nameof(args.Data.DataType)).IsTrue();
            results.Result.Detail.Contains("の組み合わせが不正です").IsTrue();
        }

        [TestMethod]
        public void RecordDateが日付に変換不可の場合はエラー()
        {
            var args = GetValidArgs();
            args.RecordDate = "hoge";

            var results = _worker.Edit(args);

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

            var results = _worker.Edit(args);

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

            var results = _worker.Edit(args);

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

            var results = _worker.Edit(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("0500");
            results.Result.Detail.Contains("お薬手帳データの取得に失敗しました").IsTrue();
        }


        [TestMethod]
        public void Jahisデータの復号に失敗するとエラー()
        {
            var args = GetValidArgs();

            // 暗号化されていないデータを返す
            _noteRepo.Setup(m => m.ReadEntity(_accountKey, _targetDate, 1))
                .Returns(new QH_MEDICINE_DAT
                {
                    CONVERTEDMEDICINESET = "hoge",
                    DATATYPE = 1,
                    OWNERTYPE = 1,
                });

            var results = _worker.Edit(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            results.Result.Detail.Contains("Jahisデータの復元に失敗しました").IsTrue();
        }

        [TestMethod]
        public void Jahisデータのデシリアライズに失敗するとエラー()
        {
            var args = GetValidArgs();

            // Jahisフォーマットではないデータを返す
            _noteRepo.Setup(m => m.ReadEntity(_accountKey, _targetDate, 1))
                .Returns(new QH_MEDICINE_DAT
                {
                    CONVERTEDMEDICINESET = "hoge".TryEncrypt(),
                    DATATYPE = 1,
                    OWNERTYPE = 1,
                });

            var results = _worker.Edit(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            results.Result.Detail.Contains("Jahisフォーマットの変換に失敗しました").IsTrue();
        }

        [TestMethod]
        [DataRow(OwnerTypeEnum.Oneself, DataTypeEnum.Ssmix)]
        [DataRow(OwnerTypeEnum.Oneself, DataTypeEnum.OtcDrugPhoto)]
        [DataRow(OwnerTypeEnum.QrCode, DataTypeEnum.OtcDrug)]
        [DataRow(OwnerTypeEnum.QrCode, DataTypeEnum.Ssmix)]
        [DataRow(OwnerTypeEnum.Data, DataTypeEnum.EthicalDrug)]
        [DataRow(OwnerTypeEnum.Data, DataTypeEnum.OtcDrug)]
        [DataRow(OwnerTypeEnum.Data, DataTypeEnum.OtcDrugPhoto)]
        public void 変更対象のお薬手帳データのDataTypeとOwnerTypeの組み合わせが編集NGならエラー(OwnerTypeEnum ownerType, DataTypeEnum dataType)
        {
            var args = GetValidArgs();
           
            _noteRepo.Setup(m => m.ReadEntity(_accountKey, _targetDate, 1))
                .Returns(new QH_MEDICINE_DAT
                {
                    CONVERTEDMEDICINESET = "hoge".TryEncrypt(),
                    DATATYPE = (byte)dataType,
                    OWNERTYPE = (int)ownerType,
                });

            var results = _worker.Edit(args);

            results.IsSuccess.Is(bool.FalseString);
            results.Result.Code.Is("1005");
            results.Result.Detail.Contains("編集不可能なデータです").IsTrue();
        }

        QoNoteMedicineSyncDataEditApiArgs GetValidArgs()
        {
            return new QoNoteMedicineSyncDataEditApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),
                Executor = _authorKey.ToApiGuidString(),
                ExecutorName = "NoteMedicine",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.QolmsiOSApp}",
                RecordDate = _targetDate.ToApiDateString(),
                Sequence = 1,
                Data = new QoNoteMedicineDetail
                {
                    OwnerType = (byte)OwnerTypeEnum.Oneself,
                    DataType = (byte)DataTypeEnum.EthicalDrug,
                    
                }
            };
        }

        QoNoteMedicineSyncDataEditApiArgs GetValidEthArgs()
        {
            return new QoNoteMedicineSyncDataEditApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),
                Executor = _authorKey.ToApiGuidString(),
                ExecutorName = "NoteMedicine",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.QolmsiOSApp}",
                RecordDate = _targetDate.ToApiDateString(),
                Sequence = 1,
                Data = new QoNoteMedicineDetail
                {
                    PrescriptionNo = 1,
                    RpSetNo = 2,
                    RecordDate = _targetDate.ToApiDateString(),
                    OwnerType = (byte)OwnerTypeEnum.Oneself,
                    DataType = (byte)DataTypeEnum.EthicalDrug,
                    PharmacyName = "Update薬局",
                    PharmacistName = "薬剤師　愛子",
                    HospitalName = "Update病院",
                    DepartmentName = "Update科",
                    DoctorName = "山田　三郎",
                    Memo = "メモ8\nメモ9",
                    RpItems = new List<QoRpItem>
                    {
                        new QoRpItem
                        {
                            No = 1,
                            UsageName = "食前",
                            UsageSupplement = "空腹時",
                            DosageFormCode = "1",
                            Quantity = 3,
                            QuantityUnit = "日分",
                            MedicineItems = new List<QoMedicineItem>
                            {
                                new QoMedicineItem
                                {
                                    No = 1,
                                    Name = "MGF錠",
                                    Quantity = "1",
                                    QuantityUnit = "カプセル",
                                    YjCode = "1111",
                                },
                                new QoMedicineItem
                                {
                                    No = 2,
                                    Name = "Qolms錠",
                                    Quantity = "2",
                                    QuantityUnit = "カプセル",
                                    YjCode = "2222",
                                },
                            }
                        },
                        new QoRpItem
                        {
                            No = 2,
                            UsageName = "食後",
                            DosageFormCode = "1",
                            Quantity = 3,
                            QuantityUnit = "日分",
                            MedicineItems = new List<QoMedicineItem>
                            {
                                new QoMedicineItem
                                {
                                    No = 1,
                                    Name = "ロキソニン",
                                    Quantity = "1",
                                    QuantityUnit = "錠",
                                    YjCode = "3333"
                                }
                            }
                        }
                    }
                }
            };
        }

        QoNoteMedicineSyncDataEditApiArgs GetValidOtcArgs()
        {
            return new QoNoteMedicineSyncDataEditApiArgs
            {
                ActorKey = _accountKey.ToApiGuidString(),
                Executor = _authorKey.ToApiGuidString(),
                ExecutorName = "NoteMedicine",
                ExecuteSystemType = $"{(int)QsApiSystemTypeEnum.QolmsiOSApp}",
                RecordDate = _targetDate.ToApiDateString(),
                Sequence = 1,
                Data = new QoNoteMedicineDetail
                {
                    PrescriptionNo = 1,
                    RpSetNo = 1,
                    RecordDate = _targetDate.ToApiDateString(),
                    OwnerType = (byte)OwnerTypeEnum.Oneself,
                    DataType = (byte)DataTypeEnum.OtcDrug,
                    PharmacyName = "Update薬局",                    
                    Memo = "メモ8\nメモ9",
                    MedicineItems = new List<QoMedicineItem>
                    {
                        new QoMedicineItem
                        {
                            No = 1,
                            Name = "ビオフェルミンS錠",
                            ItemCodeType = "J",
                            ItemCode = "1234"
                        },
                        new QoMedicineItem
                        {
                            No = 2,
                            Name = "ビオスリー",
                            ItemCodeType = "T",
                            ItemCode = "5678"
                        },
                    }                    
                }
            };
        }

        string CreateValidOtcJahisData()
        {
            var message = new JahisMessageBuilder(JMVersionTypeEnum.Latest)
            .SetPatientData("山田　太郎", "ヤマダ　タロウ", 1, new DateTime(1999, 1, 1))
            .SetNoteMemo("メモ1","メモ2")
            .AddOtcMedicine("アレグラ")
            .AddOtcMedicine("ロキソニンS")
            .AddPrescription(preCfg =>
            {
                preCfg
                .SetDate(_targetDate)
                .SetPharmacy("QOLMS薬局", "");
            })            
            .Build();

            return new QsCrypt(QsCryptTypeEnum.QolmsSystem).EncryptString(new QsJsonSerializer().Serialize(message));
        }

        string CreateInValidOtcJahisData(int mode = 0)
        {
            
            var message = new JahisMessageBuilder(JMVersionTypeEnum.Latest)
            .SetPatientData("山田　太郎", "ヤマダ　タロウ", 1, new DateTime(1999, 1, 1))
            .SetNoteMemo("メモ1", "メモ2")
            .AddOtcMedicine("アレグラ")
            .AddOtcMedicine("ロキソニンS")
            .AddPrescription(preCfg =>
            {
                preCfg
                .SetDate(_targetDate)
                .SetPharmacy("QOLMS薬局", "");
            })
            .Build();

            if(mode == 0)
            {
                // 処方レコードがないデータ
                message.Prescription_List = new List<JM_Prescription>();
            }           
            else if (mode == 1)
            {
                message.Header.Header_1 = "hoge"; // NGデータにする                
            }
            else
            {
                message.Header = null; // Headerを消して例外発生データにする
            }

            return new QsCrypt(QsCryptTypeEnum.QolmsSystem).EncryptString(new QsJsonSerializer().Serialize(message));
        }

        string CreateValidJahisData()
        {
            var message = new JahisMessageBuilder(JMVersionTypeEnum.Latest)
            .SetPatientData("山田　太郎", "ヤマダ　タロウ", 1, new DateTime(1999, 1, 1))
            .AddPrescription(preCfg =>
            {
                preCfg
                .SetDate(_targetDate)
                .SetHospital("QOLMS病院", "5678")
                .SetPharmacy("QOLMS薬局", "薬剤師　花子", 12345)
                .SetPatientMemo("メモ1","メモ2")
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
                })
                .AddRpSet(rpsetCfg =>
                {
                    rpsetCfg
                    .SetDoctor("山田　一郎")
                    .SetDepartment("皮膚科")
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
            .AddPrescription(preCfg =>
            {
                preCfg
                .SetDate(_targetDate)
                .SetHospital("MGF病院", "1234")
                .SetPharmacy("MGF薬局", "薬剤師　太郎", 12349)
                .SetPatientMemo("メモメモ")
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

            return new QsCrypt(QsCryptTypeEnum.QolmsSystem).EncryptString(new QsJsonSerializer().Serialize(message));
        }

        string CreateInvalidJahisData(bool isException = false)
        {
            var message = new JahisMessageBuilder(JMVersionTypeEnum.Latest)
            .SetPatientData("山田　太郎", "ヤマダ　タロウ", 1, new DateTime(1999, 1, 1))
            .AddPrescription(preCfg =>
            {
                preCfg
                .SetDate(_targetDate)
                .SetHospital("QOLMS病院", "5678")
                .SetPharmacy("QOLMS薬局", "薬剤師　花子", 12345)
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
                    });                    
                });
            })
            .Build();

            if (isException)
            {
                message.Header = null; // Headerを消して例外発生データにする
            }
            else
            {
                message.Header.Header_1 = "hoge"; // NGデータにする
            }

            return new QsCrypt(QsCryptTypeEnum.QolmsSystem).EncryptString(new QsJsonSerializer().Serialize(message));
        }
    }
}
