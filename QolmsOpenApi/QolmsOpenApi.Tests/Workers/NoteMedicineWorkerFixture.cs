using MGF.QOLMS.JAHISMedicineEntityV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Sql;
using MGF.QOLMS.QolmsOpenApi.Worker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QolmsOpenApi.Tests.Workers
{
    [TestClass]
    public class NoteMedicineWorkerFixture
    {
        NoteMedicineWorker _worker;
        Mock<INoteMedicineRepository> _repo;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new Mock<INoteMedicineRepository>();
            _worker = new NoteMedicineWorker(_repo.Object);
        }

        //[TestMethod]
        public void AccessKeyGenerateでキーが生成される()
        {


        }
        //[TestMethod]
        public void AccessKeyRefreshでキーが生成される()
        {

        }

        [TestMethod]
        public void Readで正常にデータが取得できる()
        {
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");
            var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem);
            var isModified = false;
            var lastAccessDate = DateTime.MinValue;
            var pageIndex = int.MinValue;
            var maxPageIndex = int.MinValue;

            var args = new QoNoteMedicineReadApiArgs()
            {
                ActorKey = accountKey.ToApiGuidString(),
                DataType = "1",
                PageIndex = "0",
                DaysPerPage = "5",
            };

            var facilityKey = Guid.NewGuid();
            var facilityKey2 = Guid.NewGuid();

            var entities = new List<QH_MEDICINE_DAT>()
            {
                new QH_MEDICINE_DAT()
                {
                    
                    DATATYPE = 1,
                    OWNERTYPE = 3,
                    RECEIPTNO = "123",
                    CONVERTEDMEDICINESET= crypt.EncryptString("abc"),
                    MEDICINESET = crypt.EncryptString( "def"),
                },
                new QH_MEDICINE_DAT()
                {
                   
                    DATATYPE = 2,
                    OWNERTYPE = 4,
                    RECEIPTNO = "456",
                    CONVERTEDMEDICINESET=crypt.EncryptString("jkl"),
                }
            };

            _repo.Setup(m => m.ReadMedicineList(args, out isModified, out lastAccessDate, out pageIndex, out maxPageIndex)).Returns(entities);
            
            var ret = _worker.Read(args);

            // 成功
            ret.IsSuccess.Is(bool.TrueString);
            // 成功コードが返る
            ret.Result.Code.Is("0200");

            // 期待した結果
            ret.DataN.Count.Is(1);
            ret.DataN[0].MedicineSetN.Count.Is(2);
            ret.DataN[0].MedicineSetN[0].DataType.Is("1");
            ret.DataN[0].MedicineSetN[0].OwnerType.Is("3");
            ret.DataN[0].MedicineSetN[0].DataIdReference.Is("123".ToEncrypedReference());
            ret.DataN[0].MedicineSetN[0].ConvertedMedicineSet.Is("abc");
            ret.DataN[0].MedicineSetN[1].DataType.Is("2");
            ret.DataN[0].MedicineSetN[1].OwnerType.Is("4");
            ret.DataN[0].MedicineSetN[1].DataIdReference.Is("456".ToEncrypedReference());
            ret.DataN[0].MedicineSetN[1].ConvertedMedicineSet.Is("jkl");
        }

        [TestMethod]
        public void RecentMedicineReadで正常にデータが取得できる()
        {
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");
            var isModified = false;
            var lastAccessDate = DateTime.MinValue;
            var pageIndex = int.MinValue;
            var maxPageIndex = int.MinValue;

            var args = new QoNoteMedicineRecentMedicineReadApiArgs()
            {
                ActorKey = accountKey.ToApiGuidString(),
                ApiType = QoApiTypeEnum.NoteMedicineRecentMedicineRead.ToString(),
                FromDate = DateTime.Today.AddMonths(-1).ToApiDateString(),
                ToDate = DateTime.Today.ToApiDateString(),
            };

            var jahis = GetMedMessageData();
            var ser = new QsJsonSerializer();
            
            string medset = ser.Serialize(jahis);
            using(var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
            {
                medset = crypt.EncryptString(medset);
            }
            
            var entities = new List<QH_MEDICINE_DAT>
            {
                new QH_MEDICINE_DAT
                {

                    DATATYPE = 1,
                    OWNERTYPE = 3,
                    RECEIPTNO = "123",
                    CONVERTEDMEDICINESET= medset,
                },
                new QH_MEDICINE_DAT
                {

                    DATATYPE = 2,
                    OWNERTYPE = 4,
                    RECEIPTNO = "456",
                    CONVERTEDMEDICINESET= medset,
                }
            };

            var args2 = new QoNoteMedicineReadApiArgs()
            {
                ActorKey = args.ActorKey,
                ApiType = QoApiTypeEnum.NoteMedicineRead.ToString(),
                DataType = "1",
                DaysPerPage = "180",
                ExecuteSystemType = args.ExecuteSystemType,
                Executor = args.Executor,
                ExecutorName = args.ExecutorName,
                LastAccessDate = DateTime.Now.ToApiDateString(),
                PageIndex = "0"
            };

            List<Guid> fileKeys = new List<Guid>
            {
                Guid.NewGuid(),
                Guid.NewGuid()
            };

            List<DbEthicalDrugCategoryItem> yakkoList = new List<DbEthicalDrugCategoryItem>
            {
                new DbEthicalDrugCategoryItem
                {
                    YjCode = "12345678",
                    MinorClassCode = "111",
                    MinorClassName = "薬効小分類１",
                },
                new DbEthicalDrugCategoryItem
                {
                    YjCode = "987654321",
                    MinorClassCode = "112",
                    MinorClassName = "薬効小分類２",
                },
            };

            _repo.Setup(m => m.ReadMedicineList(It.IsAny<QoNoteMedicineReadApiArgs>(), out isModified, out lastAccessDate, out pageIndex, out maxPageIndex)).Returns(entities);
            _repo.Setup(m => m.ReadEthicalDrugFileKey(It.IsAny<string>())).Returns(fileKeys);
            _repo.Setup(m => m.ReadYakkoList(It.IsAny<List<string>>())).Returns(yakkoList);
            var dataN = _worker.GetRecentMedicineItems(entities);
            var ret = _worker.RecentMedicineRead(args);
           

            // 成功
            ret.IsSuccess.Is(bool.TrueString);
            // 成功コードが返る
            ret.Result.Code.Is("0200");

            // 期待した結果
            ret.DataN.Count.Is(2);
            ret.DataN[0].DepartmentName.Is(dataN[0].DepartmentName);
            ret.DataN[0].HospitalName.Is(dataN[0].HospitalName);
            ret.DataN[0].MedicineName.Is(dataN[0].MedicineName);
            ret.DataN[0].PharmacyName.Is(dataN[0].PharmacyName);
            ret.DataN[0].Quantity.Is(dataN[0].Quantity);
            ret.DataN[0].QuantityUnit.Is(dataN[0].QuantityUnit);
            ret.DataN[0].RecordDate.Is(dataN[0].RecordDate);
            ret.DataN[0].YjCode.Is(dataN[0].YjCode);
            ret.DataN[0].FileKeyN[0].FileKeyReference.Is(dataN[0].FileKeyN[0].FileKeyReference);

            ret.DataN[1].DepartmentName.Is(dataN[1].DepartmentName);
            ret.DataN[1].HospitalName.Is(dataN[1].HospitalName);
            ret.DataN[1].MedicineName.Is(dataN[1].MedicineName);
            ret.DataN[1].PharmacyName.Is(dataN[1].PharmacyName);
            ret.DataN[1].Quantity.Is(dataN[1].Quantity);
            ret.DataN[1].QuantityUnit.Is(dataN[1].QuantityUnit);
            ret.DataN[1].RecordDate.Is(dataN[1].RecordDate);
            ret.DataN[1].YjCode.Is(dataN[1].YjCode);
            ret.DataN[1].FileKeyN[0].FileKeyReference.Is(dataN[1].FileKeyN[0].FileKeyReference);
        }

        [TestMethod]
        public void Addで正常にデータが登録できる()
        {
            string refDataId = string.Empty;
            string refErrMsg = string.Empty;

            var args = GetBaseArgs();
            args.Data.ConvertedMedicineSet = new QsJsonSerializer().Serialize(GetMedMessageData());

            _repo.Setup(m => m.WriteMedicine(
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<byte>(),
                It.IsAny<DateTime>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                ref refDataId,
                ref refErrMsg
                )).Returns((true,true,1));

            _repo.Setup(m => m.RegisterMedicineEvent(args,It.IsAny<DateTime>(),It.IsAny<int>())).Returns(true);

            var ret = _worker.Add(args);

            // 成功
            ret.IsSuccess.Is(bool.TrueString);
            // 成功コードが返る
            ret.Result.Code.Is("0200");

            ret.DataIdReference.IsNotNull();
        }

        [TestMethod]
        public void Deleteで正常にデータが削除できる()
        {
            var args = new QoNoteMedicineDeleteApiArgs
            {
                ActorKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf").ToApiGuidString(),
                DataIdReference = "",
                LinkageSystemNo = "99999"
            };
            var items = new List<DbMedicineKeyItem>
            {
                new DbMedicineKeyItem
                {
                    AccountKey =  Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf"),
                    RecordDate = DateTime.Now,
                    Sequence = 1,
                    DataType = 2
                },
                new DbMedicineKeyItem
                {
                    AccountKey =  Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf"),
                    RecordDate = DateTime.Now,
                    Sequence = 2,
                    DataType = 2
                }
            };
            _repo.Setup(m => m.DeleteMedicine(It.IsAny<Guid>(),It.IsAny<string>())).Returns((true, items));
            _repo.Setup(m => m.DeleteMedicineEvent(args,items)).Returns(true);

            var ret = _worker.Delete(args);

            // 成功
            ret.IsSuccess.Is(bool.TrueString);
            // 成功コードが返る
            ret.Result.Code.Is("0200");
        }

        public JM_Message GetMedMessageData()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            return new JM_Message
            {
                Header = new JM_Header
                {
                    Header_1 = "JAHISTC06",
                    Header_2 = "2"
                },
                No001 = new JM_No001
                {
                    No001_1 = "1",
                    No001_2 = "薬局　花子",
                    No001_3 = "2",
                    No001_4 = "19900909",
                    No001_11 = "ヤッキョク　ハナコ"
                },
                No002_List = new List<JM_No002>(),
                No003_List = new List<JM_No003>
                {
                    new JM_No003
                    {
                        No003_1 = "3",
                        No003_2 = "アレグラＦＸ２８錠",
                        No003_5 = "2"
                    },
                    new JM_No003
                    {
                        No003_1 = "3",
                        No003_2 = "新ビオフェルミンＳ錠　１３０錠",
                        No003_5 = "2"
                    }
                },
                No004_List = new List<JM_No004>
                {
                    new JM_No004
                    {
                        No004_1 = "4",
                        No004_2 = "メモメモ",
                        No004_4 = "2"
                    }
                },
                Prescription_List = new List<JM_Prescription>
                {
                    new JM_Prescription
                    {
                        No005 = new JM_No005
                        {
                            No005_1 = "5",
                            No005_2 = "20221125",
                            No005_3 = "2"
                        },
                        No011 = new JM_No011
                        {
                            No011_1 = "11",
                            No011_2 = "テスト薬局",
                            No011_9 = "2"
                        },
                        No015 = new JM_No015(),
                        No051 = new JM_No051
                        {
                            No051_1 = "51",
                            No051_2 = "医療機関名称",
                            No051_3 = "10",
                            No051_4 = "4",
                            No051_5 = "510",
                            No051_6 = "1"
                        },
                        RpSet_List = new List<JM_RpSet>()
                        {
                            GetRpSetData()
                        },
                        No401_List = new List<JM_No401>(),
                        No411_List = new List<JM_No411>(),
                        No421_List = new List<JM_No421>(),
                        No501_List = new List<JM_No501>(),
                        No601_List = new List<JM_No601>(),
                    }
                },
                No701_List = new List<JM_No701>()
            };
        }

        JM_RpSet GetRpSetData()
        {
            return new JM_RpSet
            {
                 Rp_List = new List<JM_Rp>
                 {
                     new JM_Rp
                     {
                         Medicine_List = new List<JM_Medicine>
                         {
                             new JM_Medicine
                             {
                                 No201 = new JM_No201
                                 {
                                     No201_1 = "1",
                                     No201_2 = "1",
                                     No201_3 = "薬品名テスト",
                                     No201_4 = ((double)1).ToString(),
                                     No201_5 = "錠",
                                     No201_6 = "4",
                                     No201_7 = "1119401A1036",
                                     No201_8 = "1"
                                 }
                             },
                             new JM_Medicine
                             {
                                 No201 = new JM_No201
                                 {
                                     No201_1 = "2",
                                     No201_2 = "2",
                                     No201_3 = "薬品名テスト2番",
                                     No201_4 = ((double)2).ToString(),
                                     No201_5 = "枚",
                                     No201_6 = "4",
                                     No201_7 = "1115403D3043",
                                     No201_8 = "1"
                                 }
                             }
                         },
                         No301 = new JM_No301
                         {
                             No301_1 = "1",
                             No301_2 = "1",
                             No301_3 = "用法名",
                             No301_4 = "5",
                             No301_5 = "調剤単位",
                             No301_6 = "10",
                             No301_7 = "1",
                             No301_8 = "1",
                             No301_9 = "1"
                         }
                     }
                 }
            };
        }

        public QoNoteMedicineAddApiArgs GetBaseArgs()
        {
            return new QoNoteMedicineAddApiArgs
            {
                ApiType = QoApiTypeEnum.NoteMedicineAdd.ToString(),
                ActorKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf").ToApiGuidString(),
                LinkageSystemNo = "99999",
                Data = new QoApiNoteMedicineDetailItem
                    {
                        DataIdReference = "",
                        DataType = "1",  //  1:調剤薬、2:市販薬、4:市販薬写真。
                        OwnerType = "1", // 1:個人（手入力）、2:QRコード読み込み、101:システム連携。 ※市販薬のバーコード読み込みは1:個人（手入力）に該当。 ※3:テキスト読み込みは使用しないでください。
                        ConvertedMedicineSet = "",
                        OtcMedicineSet = null, //  DataTypeが2:市販薬のときのみ有効です。
                        OtcPhotoMedicineSet = null // DataTypeが4:市販薬写真のときのみ有効です。
                    }
            };
        }
    }
}
