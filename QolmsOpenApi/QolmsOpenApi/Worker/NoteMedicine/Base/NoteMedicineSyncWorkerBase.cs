
using MGF.QOLMS.JAHISMedicineEntityV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// お薬手帳 情報同期処理基本クラス
    /// </summary>
    public class NoteMedicineSyncWorkerBase
    {
        protected readonly INoteMedicineRepository _noteRepo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="noteMedicineRepository"></param>
        public NoteMedicineSyncWorkerBase(INoteMedicineRepository noteMedicineRepository)
        {
            _noteRepo = noteMedicineRepository;
        }

        /// <summary>
        /// 主キーからお薬手帳データを1件取得する
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="recordDate"></param>
        /// <param name="seq"></param>
        /// <param name="results"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected bool TryReadEntity(Guid accountKey, DateTime recordDate, int seq, QoApiResultsBase results, out QH_MEDICINE_DAT entity)
        {
            entity = null;
            try
            {
                entity = _noteRepo.ReadEntity(accountKey, recordDate, seq);
                if (entity == null)
                {
                    results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.NoteMedicineDataNotFound, "お薬手帳データが存在しませんでした。");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "お薬手帳データの取得に失敗しました。");
                return false;
            }
        }

        /// <summary>
        /// データ変換(リスト)
        /// </summary>
        /// <param name="entityList"></param>
        /// <returns></returns>
        public List<QoNoteMedicineItem> ConvertNoteMedicineItems(List<QH_MEDICINE_DAT> entityList)
        {
            var list = new List<QoNoteMedicineItem>();
            foreach (var entity in entityList)
            {
                try
                {
                    var item = ConvertNoteMedicineItem(entity);
                    switch (entity.DATATYPE)
                    {
                        case 1:
                        case 100:
                            item.DetailItems = ConvertNoteMedicineDetailForPrescription(entity);
                            break;
                        case 2:
                            item.DetailItems = ConvertNoteMedicineDetailForOtc(entity);
                            break;
                        case 4:
                            // 市販薬写真は既に廃止されているためMAUI版より対象外
                            continue;
                        default:
                            continue;
                    }

                    list.Add(item);
                }
                catch(Exception ex)
                {
                    // 問題があるレコードがあればエラーにはせずログだけ残しデータをスキップする
                    QoAccessLog.WriteErrorLog(ex, "お薬手帳データ変換失敗", Guid.Empty);
                }
            }

            return list;
        }

        /// <summary>
        /// データ変換(単体)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected QoNoteMedicineItem ConvertNoteMedicineEntity(QH_MEDICINE_DAT entity)
        {            
            var item = ConvertNoteMedicineItem(entity);
            switch (entity.DATATYPE)
            {
                case 1:
                case 100:
                    item.DetailItems = ConvertNoteMedicineDetailForPrescription(entity);
                    break;
                case 2:
                    item.DetailItems = ConvertNoteMedicineDetailForOtc(entity);
                    break;
                case 4:                    
                default:
                    break;
            }

            return item;
        }

        /// <summary>
        /// Jahisデータを調剤薬用のMedicineSetに変換する
        /// </summary>
        /// <param name="recordDate"></param>
        /// <param name="pharmacyNo"></param>
        /// <param name="jahis"></param>
        /// <returns></returns>
        protected string CreateEthMedicineSet(DateTime recordDate, int pharmacyNo, JM_Message jahis)
        {
            // 仕様上処方情報1件目しか格納できない
            var prescription = jahis.Prescription_List.First();

            var medicineSet = new QhMedicineSetEthicalDrugOfJson
            {
                PrescriptionDate = recordDate.ToApiDateString(),
                PharmacyId = pharmacyNo.ToString(),
                FacilityName = prescription.No051?.No051_2,
                FacilityId = prescription.No051?.No051_5,
                PharmacyName = prescription.No011?.No011_2,
                PharmacistName = prescription.No015?.No015_2,
                MedicineUsageN = new List<QhMedicineSetUsageItemOfJson>()
            };

            if (prescription.No421_List != null)
            {
                medicineSet.LeftoverMedicine = string.Join("", prescription.No421_List.Select(x => x.No421_2));
                medicineSet.LeftoverMedicineAuthor = prescription.No421_List.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.No421_3))?.No421_3 ?? string.Empty;
            }

            if (prescription.No501_List != null)
            {
                medicineSet.SpecialNotes = string.Join("", prescription.No501_List.Select(x => x.No501_2));
            }

            if (prescription.No601_List != null)
            {
                medicineSet.Memo = string.Join("", prescription.No601_List.Select(x => x.No601_2));
            }

            foreach (var rpSet in prescription.RpSet_List ?? new List<JM_RpSet>())
            {
                foreach (var rp in rpSet.Rp_List)
                {
                    var usage = new QhMedicineSetUsageItemOfJson
                    {
                        DoctorId = string.Empty,
                        DoctorName = rpSet.No055?.No055_2,
                        RepresentedOrganizationName = rpSet.No055?.No055_3,
                        DosageForm = rp.No301?.No301_6.TryToValueType(QsDbDosageFormTypeEnum.Other) ?? QsDbDosageFormTypeEnum.Other,
                        Usage = rp.No301?.No301_3,
                        Days = rp.No301?.No301_4,
                        Unit = rp.No301?.No301_5,
                        MedicineN = rp.Medicine_List?.Select(x => new QhMedicineSetEthicalDrugItemOfJson
                        {
                            CodeSystem = x.No201?.No201_6.TryToValueType<byte>(1) ?? 1,
                            Dose = x.No201?.No201_4,
                            MedicineCode = x.No201?.No201_7,
                            MedicineName = x.No201?.No201_3,
                            ReceiptNumber = x.No201?.No201_2.TryToValueType<byte>(1) ?? 1,
                            Unit = x.No201?.No201_5,
                        })?.ToList() ?? new List<QhMedicineSetEthicalDrugItemOfJson>()
                    };

                    medicineSet.MedicineUsageN.Add(usage);
                }
            }

            return new QsJsonSerializer().Serialize(medicineSet);
        }

        /// <summary>
        /// Jahisデータを市販薬用のMedicineSetに変換する
        /// </summary>
        /// <param name="pharmacyNo"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected string CreateOtcMedicineSet(int pharmacyNo, QoNoteMedicineDetail data)
        {
            var medicineSet = new QhMedicineSetOtcDrugOfJson
            {
                PharmacyNo = pharmacyNo,
                PharmacyName = data.PharmacyName,
                Comment = data.Memo,
                MedicineN = data.MedicineItems?.Select(x => new QhMedicineSetOtcDrugItemOfJson
                {
                    MedicineName = x.Name,
                    ItemCode = x.ItemCode,
                    ItemCodeType = x.ItemCodeType
                })?.ToList() ?? new List<QhMedicineSetOtcDrugItemOfJson>()
            };

            return new QsJsonSerializer().Serialize(medicineSet);
        }

        /// <summary>
        /// 改行コードを統一して改行コードで分割した文字列配列を返す
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        protected string[] SpliteBreakLine(string source)
        {
            // 改行コードを統一
            var fixSource = Regex.Replace(source, @"\r\n?|\n", "\n");
            // 改行コードで分割する
            return fixSource.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }
        
        /// <summary>
        /// お薬手帳情報に薬効分類情報を一括適用する
        /// </summary>
        /// <param name="noteMedicineItems"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        protected bool TryApplyYakkoData(List<QoNoteMedicineItem> noteMedicineItems, QoApiResultsBase results)
        {
            // 調剤薬のリストをまとめる
            var ethMedicineItems = noteMedicineItems
                .SelectMany(x => x.DetailItems
                    .SelectMany(y => y.RpItems
                        .SelectMany(z => z.MedicineItems)));

            // 取得したデータに含まれる全てのYJコードをリスト化する
            var yjCodeList = ethMedicineItems
                .Select(x => x.YjCode)
                .Where(x => !string.IsNullOrEmpty(x))
                .Distinct()
                .ToList();

            // 薬効分類一括取得
            if (!TryReadYakko(yjCodeList, results, out var yakkoDic))
            {
                return false;
            }

            // 調剤薬の薬効分類と画像をセット
            foreach (var item in ethMedicineItems)
            {
                if (yakkoDic.TryGetValue(item.YjCode, out var categoryItem))
                {
                    item.EffectCategoryCode = categoryItem.MinorClassCode;
                    item.EffectCategoryName = categoryItem.MinorClassName;
                }
            }

            return true;
        }

        /// <summary>
        /// 薬効分類一括取得
        /// </summary>
        /// <param name="yjCodeList"></param>
        /// <param name="results"></param>
        /// <param name="yakkoDic"></param>
        /// <returns></returns>
        public bool TryReadYakko(List<string> yjCodeList, QoApiResultsBase results, out Dictionary<string, DbEthicalDrugCategoryItem> yakkoDic)
        {
            yakkoDic = null;
            try
            {
                var ret = _noteRepo.ReadYakkoList(yjCodeList);
                yakkoDic = ret.ToDictionary(k => k.YjCode, v => v);
                return true;
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "薬効データの取得に失敗しました。");
                return false;
            }
        }

        QoNoteMedicineItem ConvertNoteMedicineItem(QH_MEDICINE_DAT entity)
        {
            return new QoNoteMedicineItem
            {
                RecordDate = entity.RECORDDATE.ToApiDateString(),
                Sequence = entity.SEQUENCE,
                ReceiptNo = entity.RECEIPTNO,
                DataIdReference = entity.RECEIPTNO.ToEncrypedReference(),
                LinkageSystemNo = entity.LINKAGESYSTEMNO,
                DataType = entity.DATATYPE,
                OwnerType = (byte)entity.OWNERTYPE,
                DeleteFlag = entity.DELETEFLAG
            };
        }

        List<QoNoteMedicineDetail> ConvertNoteMedicineDetailForOtc(QH_MEDICINE_DAT entity)
        {
            var qsJson = new QsJsonSerializer();

            var medicineSet = entity.MEDICINESET.TryDecrypt();
            var otcMedicineSet = qsJson.Deserialize<QhMedicineSetOtcDrugOfJson>(medicineSet);

            var medicineItems = otcMedicineSet.MedicineN.Select((med, i) => new QoMedicineItem
            {
                No = i + 1,
                Name = med.MedicineName,
                ItemCode = med.ItemCode,
                ItemCodeType = med.ItemCodeType,
            }).ToList();

            var detail = new QoNoteMedicineDetail
            {
                PrescriptionNo = 1, // 市販薬は分割されないので1固定
                RpSetNo = 1,        // 同上
                RecordDate = entity.RECORDDATE.ToApiDateString(),
                DataType = entity.DATATYPE,
                OwnerType = (byte)entity.OWNERTYPE,
                PharmacyNo = entity.PHARMACYNO,
                PharmacyName = otcMedicineSet.PharmacyName,
                Memo = otcMedicineSet.Comment,
                RpItems = new List<QoRpItem>(),
                MedicineItems = medicineItems
            };

            return new List<QoNoteMedicineDetail> { detail };
        }

        List<QoNoteMedicineDetail> ConvertNoteMedicineDetailForPrescription(QH_MEDICINE_DAT entity)
        {
            var qsJson = new QsJsonSerializer();
            var convertedMedicineSet = entity.CONVERTEDMEDICINESET.TryDecrypt();

            var jahis = qsJson.Deserialize<JM_Message>(convertedMedicineSet);


            var detailList = new List<QoNoteMedicineDetail>();


            var prescriptionNo = 1;
            foreach (var prescription in jahis.Prescription_List)
            {                
                var memo = string.Empty;
                if (prescription.No601_List != null && prescription.No601_List.Count > 0)
                {
                    memo = string.Join("\n", prescription.No601_List.ConvertAll(x => x.No601_2));
                }

                var rpSetSeq = 1;
                foreach (var rpSet in prescription.RpSet_List)
                {
                    var rpItems = new List<QoRpItem>();
                    var rpSeq = 1;
                    foreach (var rp in rpSet.Rp_List)
                    {
                        rpItems.Add(new QoRpItem
                        {
                            No = rpSeq++,
                            UsageName = rp.No301.No301_3,
                            DosageFormCode = rp.No301.No301_6,
                            Quantity = rp.No301.No301_4.TryToValueType(0),
                            QuantityUnit = rp.No301.No301_5,
                            UsageSupplement = string.Join("", rp.No311_List.ConvertAll(x => x.No311_3)),
                            MedicineItems = ConvertMedicineItemForRp(rp),
                        });
                    }

                    var detailItem = new QoNoteMedicineDetail
                    {
                        PrescriptionNo = prescriptionNo,
                        RpSetNo = rpSetSeq++,
                        RecordDate = DateTime.ParseExact(prescription.No005?.No005_2, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None).ToApiDateString(),
                        DataType = entity.DATATYPE,
                        OwnerType = (byte)entity.OWNERTYPE,
                        PharmacyNo = entity.PHARMACYNO,
                        PharmacyName = prescription.No011?.No011_2,
                        PharmacistName = prescription.No015?.No015_2,
                        HospitalName = prescription.No051?.No051_2,
                        DepartmentName = rpSet.No055?.No055_3,
                        DoctorName = rpSet.No055?.No055_2,
                        Memo = memo,
                        RpItems = rpItems,
                        MedicineItems = new List<QoMedicineItem>()
                    };

                    detailList.Add(detailItem);
                }

                prescriptionNo++;
            }

            return detailList;
        }

        List<QoMedicineItem> ConvertMedicineItemForRp(JM_Rp rp)
        {
            return rp.Medicine_List.Select((medicine, i) => new QoMedicineItem
            {
                No = i + 1,
                Name = medicine.No201.No201_3,
                Quantity = medicine.No201.No201_4,
                QuantityUnit = medicine.No201.No201_5,
                // 薬品コードがYJコードの場合のみ設定
                YjCode = medicine.No201.No201_6 == "4" ? medicine.No201.No201_7 : string.Empty,
            }).ToList();
        }
    }
}