using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using QolmsOpenApi.Tests.Workers;
using MGF.QOLMS.JAHISMedicineEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Worker;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsOpenApi.Sql;

namespace QolmsOpenApi.Tests.Sandbox
{
    [TestClass]
    public class NoteMedicineFixture
    {
        INoteMedicineRepository _repo;

        [TestInitialize]
        public void Initialize()
        {
            _repo = new NoteMedicineRepository();
        }

        [TestMethod]
        public void QH_MEDICINE_DATへのインサート()
        {
            var accountKey = Guid.Parse("3f250dc3-6d51-4c22-ad34-bc7125d1416b");

            var jahis = GetMedMessageData();
            var jahisData = new QsJsonSerializer().Serialize(jahis);

            var entity = new QH_MEDICINE_DAT
            {
                ACCOUNTKEY = accountKey,
                RECORDDATE = DateTime.Today,
                SEQUENCE = 0,
                COMMENTSET = "",
                CONVERTEDMEDICINESET = jahisData,
                DATATYPE = 1,
                OWNERTYPE = 1,
            };

            _repo.InsertEntity(entity);
        }

        [TestMethod]
        public void QH_MEDICINE_DATの更新()
        {
            var accountKey = Guid.Parse("3f250dc3-6d51-4c22-ad34-bc7125d1416b");
            var ret = _repo.ReadEntity(accountKey, DateTime.Today, 1);

            ret.PHARMACYNO = 9999;

            _repo.UpdateEntity(ret);
        }

        [TestMethod]
        public void QH_MEDICINE_DATの論理削除()
        {
            var accountKey = Guid.Parse("3f250dc3-6d51-4c22-ad34-bc7125d1416b");
            var ret = _repo.ReadEntity(accountKey, DateTime.Today, 1);

            _repo.DeleteEntity(ret);
        }

        [TestMethod]
        public void QH_MEDICINE_DATの物理削除()
        {
            var accountKey = Guid.Parse("3f250dc3-6d51-4c22-ad34-bc7125d1416b");            

            _repo.PhysicalDeleteEntity(accountKey, DateTime.Today, 1);
        }

        [TestMethod]
        public void お薬手帳の情報を主キーで取得()
        {
            var accountKey = Guid.Parse("3f250dc3-6d51-4c22-ad34-bc7125d1416b");

            var ret = _repo.ReadEntity(accountKey, new DateTime(2019, 8, 12), 1);
        }

        [TestMethod]
        public void お薬手帳の更新情報を取得できる()
        {
            var accountKey = Guid.Parse("3f250dc3-6d51-4c22-ad34-bc7125d1416b");

            var start = new DateTime(2025,4, 22, 9, 0, 33, 134);
            var end = new DateTime(2024, 11, 7, 10, 25, 01, 307);


            var ret = _repo.ReadNoteUpdate(accountKey, start, DateTime.Now);
        }

        [TestMethod]
        public void お薬手帳の履歴を取得できる()
        {
            var accountKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");

            var start = new DateTime(2022, 12, 5, 11, 11, 11);
            var end = new DateTime(2022, 9, 30, 11, 11, 11);

            var ret = _repo.ReadNoteHistory(accountKey, start, end);
        }

        [TestMethod]
        public void お薬手帳の履歴の次候補を取得できる()
        {
            var accountKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");

            var start = new DateTime(2022, 9, 30, 11, 11, 11);

            var ret = _repo.ReadNoteHistoryNext(accountKey, start);
        }

        [TestMethod]
        public void YJコードから薬効を取得()
        {
            var yjCodeList = new List<string>
            {
                "1119402A1022",
                "1119402A1073",
                "1119402A1103",
                "1119402A1111",
                "1119402A1120",
                "1119402A1138",
                "1119402A2029",
                "1119402A2100",
            };

            var ret = _repo.ReadYakkoList(new List<string>());
        }

        [TestMethod]
        public void YJコードから調剤薬のファイルを取得()
        {
            var yjCodeList = new List<string>
            {
                "1119402A1022",
                "1119402A1073",
                "1119402A1103",
                "1119402A1111",
                "1119402A1120",
                "1119402A1138",
                "1119402A2029",
                "1119402A2100",
            };

            var ret = _repo.ReadEthicalDrugFileListWithYjCode(yjCodeList);
        }

        [TestMethod]
        public void ItemCodeとItemCodeTypeのリストから市販薬ファイルを取得()
        {
            var itemCodeList = new List<string>
            {
                "2910001000001B",
                "45010910J",
                "4518426281317J"
            };

            var ret = _repo.ReadOtcDrugFileListWithItemCode(itemCodeList);
        }

        [TestMethod]
        public void お薬手帳のリストを取得できる()
        {
            var accountKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");

            var args = new QoNoteMedicineReadApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                DataType = "1",
                LastAccessDate = DateTime.Now.ToString(),
                PageIndex = "0",
                DaysPerPage = "180"
            };

            var ret = _repo.ReadMedicineList(args, out var isModified, out var lastAccessDate, out var pageIndex, out var maxPageIndex);
        }

        [TestMethod]
        public void 最近のお薬情報を取得できる()
        {
            //var accountKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");

            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");

            var args = new QoNoteMedicineReadApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                ApiType = QoApiTypeEnum.NoteMedicineRead.ToString(),
                DataType = "1",
                DaysPerPage = "180",
                LastAccessDate = DateTime.Now.ToApiDateString(),
                PageIndex = "0"
            };

            var entities = _repo.ReadMedicineList(args, out var isModified, out var lastAccessDate, out var pageIndex, out var maxPageIndex);

            var ret = new NoteMedicineWorker(_repo).GetRecentMedicineItems(entities);
        }

        [TestMethod]
        public void 登録編集削除()
        {
            var ser = new QsJsonSerializer();
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");

            string refDataId = string.Empty;
            string refErrMsg = string.Empty;

            var worker = new NoteMedicineWorkerFixture();
            var args = worker.GetBaseArgs();
            args.ApiType = QoApiTypeEnum.NoteMedicineAdd.ToString();
            args.Data.DataType = "1";
            args.Data.ConvertedMedicineSet = new QsJsonSerializer().Serialize(GetMedMessageData());

            var otcItem = new QoApiMedicineSetOtcItem
            {
                PharmacyName = "テスト薬局",
                Memo = "メモメモ",
                OtcDrugN = new List<QoApiMedicineOtcItem>
                {
                    new QoApiMedicineOtcItem
                    {
                        MedicineName = "アレグラＦＸ２８錠",
                        ItemCode = "4987188166048",
                        ItemCodeType = "J",
                        FileKeyN = new List<QoApiFileKeyItem>(),
                    },
                    new QoApiMedicineOtcItem
                    {
                        MedicineName = "新ビオフェルミンＳ錠　１３０錠",
                        ItemCode = "4987123135177",
                        ItemCodeType = "J",
                        FileKeyN = new List<QoApiFileKeyItem>(),
                    }
                }
            };

            args.Data.OtcMedicineSet = otcItem;
            var jsonObj = new QhMedicineSetOtcDrugOfJson();
            foreach (QoApiMedicineOtcItem drug in args.Data.OtcMedicineSet.OtcDrugN)
            {
                jsonObj.MedicineN.Add(new QhMedicineSetOtcDrugItemOfJson()
                {
                    ItemCode = drug.ItemCode,
                    ItemCodeType = drug.ItemCodeType,
                    MedicineName = drug.MedicineName
                });
            }
            var medicineSet = ser.Serialize<QhMedicineSetOtcDrugOfJson>(jsonObj);
            var recorddate = new DateTime(2022, 11, 26);

            var (wret, ret, seq) = _repo.WriteMedicine(
                   args.ActorKey.TryToValueType<Guid>(Guid.Empty),
                   args.AuthorKey.TryToValueType<Guid>(Guid.Empty),
                   args.Data.ConvertedMedicineSet,
                   (byte)(QH_MEDICINE_DAT.DataTypeEnum)byte.Parse(args.Data.DataType),
                   recorddate,
                   args.LinkageSystemNo.TryToValueType<int>(int.MinValue),
                   medicineSet,
                   (int)(QH_MEDICINE_DAT.DataTypeEnum)byte.Parse(args.Data.OwnerType),
                   int.MinValue,
                   ref refDataId,
                   ref refErrMsg
                   );

            //if (wret)
            //{
            //    if(ret && args.Data.DataType == "1")
            //    {
            //        _repo.RegisterMedicineEvent(args, recorddate, seq);
            //    }

            //}

            // 作ったものを編集
            //EditMedicine(refDataId, recorddate, args.Data.ConvertedMedicineSet, "2", "1", otcItem);

            // 作ったものを削除
            //DeleteMedicine(accountKey, refDataId.ToEncrypedReference());
        }

        public void DeleteMedicine(Guid accountKey,string dataIdRef)
        {
            var result = false;

            var args = new QoNoteMedicineDeleteApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                ApiType = QoApiTypeEnum.NoteMedicineDelete.ToString(),
                LinkageSystemNo = "99999",
                DataIdReference = dataIdRef,
            };


            var ( ret, keys) = _repo.DeleteMedicine(accountKey, dataIdRef);

            //if (ret)
            //{
            //    if (keys != null && keys.Any())
            //    {
            //        result = true;

            //        //データ種別が調剤薬
            //        var query = keys.
            //            Where(x => x.DataType == (byte)QH_MEDICINE_DAT.DataTypeEnum.EthicalDrug).ToList();
            //        if (query != null && query.Any())
            //        {
            //            // イベントも削除
            //            result = _repo.DeleteMedicineEvent(args, keys);
            //        }
            //    }
            //    //result = _repo.DeleteMedicineEvent(args,keys);
            //}

            //result.Is(true);

            ret.Is(true);

            keys.Count.Is(1);
                      

        }

        public void EditMedicine(string dataIdRef, DateTime recorddate, string jahis, string dataType, string ownerType, QoApiMedicineSetOtcItem otcItem = null)
        {
            var ser = new QsJsonSerializer();
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");

            string refDataId = dataIdRef;
            string refErrMsg = string.Empty;

            if (otcItem != null)
            {
                otcItem.PharmacyName = "テスト編集後薬局";
                otcItem.Memo = "めもめもめも";
                otcItem.OtcDrugN[0].MedicineName = "アレグラＦＸＺ２８０錠";
            }

            var entity = JsonConvert.DeserializeObject<JM_Message>(jahis);
            entity.No003_List[0].No003_2 = "アレグラＦＸＺ２８０錠";
            entity.Prescription_List[0].No011.No011_2 = "テスト編集後薬局";
            entity.No004_List[0].No004_2 = "めもめもめも";                   

            var args = new QoNoteMedicineEditApiArgs
            {
                ActorKey = accountKey.ToApiGuidString(),
                ApiType = QoApiTypeEnum.NoteMedicineEdit.ToString(),
                LinkageSystemNo = "99999",
                DataIdReference = dataIdRef,
                Data = new QoApiNoteMedicineDetailItem
                {
                    ConvertedMedicineSet = JsonConvert.SerializeObject(entity),
                    DataIdReference = dataIdRef,
                    DataType = dataType,
                    OwnerType = ownerType,
                    OtcMedicineSet = otcItem
                }
            };

            var jsonObj = new QhMedicineSetOtcDrugOfJson();
            foreach (QoApiMedicineOtcItem drug in args.Data.OtcMedicineSet.OtcDrugN)
            {
                jsonObj.MedicineN.Add(new QhMedicineSetOtcDrugItemOfJson()
                {
                    ItemCode = drug.ItemCode,
                    ItemCodeType = drug.ItemCodeType,
                    MedicineName = drug.MedicineName
                });
            }
            var medicineSet = ser.Serialize<QhMedicineSetOtcDrugOfJson>(jsonObj);


            var (wret, ret, seq) = _repo.WriteMedicine(
                   args.ActorKey.TryToValueType<Guid>(Guid.Empty),
                   args.AuthorKey.TryToValueType<Guid>(Guid.Empty),
                   args.Data.ConvertedMedicineSet,
                   (byte)(QH_MEDICINE_DAT.DataTypeEnum)byte.Parse(args.Data.DataType),
                   recorddate,
                   args.LinkageSystemNo.TryToValueType<int>(int.MinValue),
                   medicineSet,
                   (int)(QH_MEDICINE_DAT.DataTypeEnum)byte.Parse(args.Data.OwnerType),
                   int.MinValue,
                   ref refDataId,
                   ref refErrMsg
                   );
        }

        [TestMethod]
        public void 指定日のお薬情報を連携システム番号を指定して取得()
        {
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");           

            var ret = _repo.ReadMedicineDayList(
                accountKey,
                new DateTime(2021, 9, 2),
                new List<byte> { 1, 100 },
                Guid.Empty,
                27004);
        }

        [TestMethod]
        public void 指定日のお薬情報を施設キーを指定して取得()
        {
            var accountKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");

            var ret = _repo.ReadMedicineDayList(
                accountKey,
                new DateTime(2022, 9, 30),
                new List<byte> { 1, 100 },
                Guid.Parse("11dc3f56-5652-4d08-9147-1575c1723edb"),
                0);
        }


        [TestMethod]
        public void 指定日のお薬情報を絞り込み無しで取得()
        {
            var accountKey = Guid.Parse("5ebdf528-5da2-495f-8a15-f2223dcc2dcf");

            var ret = _repo.ReadMedicineDayList(
                accountKey,
                new DateTime(2022, 6, 17),
                new List<byte> { 2, 4 },
                Guid.Empty,
                0);
        }

        [TestMethod]
        public void 指定日のお薬情報を施設キーを指定してWorkerから実行()
        {
            var worker = new NoteMedicineReadWorker(_repo);
            
            var accountKey = Guid.Parse("6ed1824d-aa9b-4277-9826-459cfb4d0b55");

            var args = new QoNoteMedicineDayReadApiArgs
            {
                ApiType = QoApiTypeEnum.NoteMedicineDayRead.ToString(),
                ActorKey = accountKey.ToApiGuidString(),
                DataType = "1",
                FacilityKeyReference = "13d00930275c2ab0fd3d896b90eba8ea6faed98f5bb636ab977fbe6fcc2d6999cc84275f29f099dd466d3f554a4a8aec",
                TargetDate = new DateTime(2022, 9, 1).ToApiDateString(),
            };

            var ret = worker.DayRead(args);
        }

        [TestMethod]
        public void テストデータ作成用()
        {
            var json = new QsJsonSerializer().Serialize(GetMedMessageData());
            var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem);
            var encoded = crypt.EncryptString(json);
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
                No003_List = new List<JM_No003>(),
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
                                     No201_3 = "ロキソニン",
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
                                     No201_3 = "テプレノン",
                                     No201_4 = ((double)2).ToString(),
                                     No201_5 = "錠",
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
                             No301_3 = "痛いとき",
                             No301_4 = "5",
                             No301_5 = "日分",
                             No301_6 = "1",
                             No301_7 = "1",
                             No301_8 = "1",
                             No301_9 = "1"
                         }
                     },
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
                                     No201_3 = "ボルタレンテープ",
                                     No201_4 = ((double)1).ToString(),
                                     No201_5 = "枚",
                                     No201_6 = "4",
                                     No201_7 = "1116700X1037",
                                     No201_8 = "1"
                                 }
                             },                             
                         },
                         No301 = new JM_No301
                         {
                             No301_1 = "1",
                             No301_2 = "1",
                             No301_3 = "痛いとき",
                             No301_4 = "5",
                             No301_5 = "日分",
                             No301_6 = "5",
                             No301_7 = "1",
                             No301_8 = "1",
                             No301_9 = "1"
                         }
                     }
                 }
            };
        }
    }
}
