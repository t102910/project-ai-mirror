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
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// OTC医薬品詳細を取得する処理を行います。
    /// </summary>
    public class MasterDrugDetailReadWorker
    {
        IOtcDrugRepository _otcDrugRepo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="otcDrugRepository"></param>
        public MasterDrugDetailReadWorker(IOtcDrugRepository otcDrugRepository)
        {
            _otcDrugRepo = otcDrugRepository;
        }

        /// <summary>
        /// データ取得処理
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoMasterDrugDetailReadApiResults Read(QoMasterDrugDetailReadApiArgs args)
        {
            var results = new QoMasterDrugDetailReadApiResults
            {
                IsSuccess = bool.FalseString
            };

            // アカウントキーチェック
            if (!args.ActorKey.CheckArgsConvert(nameof(args.ActorKey), Guid.Empty, results, out var _))
            {
                return results;
            }

            // ItemCode 必須チェック
            if (!args.ItemCode.CheckArgsRequired(nameof(args.ItemCode), results))
            {
                return results;
            }

            // OTC医薬品情報取得
            if (!TryReadOtcDrug(args.ItemCode, args.ItemCodeType, results, out var dbItem))
            {
                return results;
            }

            // APIアイテムに変換
            if (!TryConvertDbItem(dbItem, args.ItemCodeType, results, out var otcItem))
            {
                return results;
            }

            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            results.OtcDrug = otcItem;

            return results;
        }

        private bool TryConvertDbItem(DbOtcDrugDetailItem dbItem, string itemCodeType, QoApiResultsBase results, out QoApiOtcDrugDetailItem otcItem)
        {
            otcItem = null;
            try
            {
                otcItem = new QoApiOtcDrugDetailItem
                {
                    ItemCode = dbItem.ItemCode,
                    ItemCodeType =  itemCodeType,
                    MakerName = dbItem.FullWidthMakerName,
                    ItemName = dbItem.MakerOfficialItemName,
                    ContentQuantity = dbItem.FullWidthContent.ToString("0.00"),
                    ContentUnit = dbItem.FullWidthContentUnit,
                    ItemUnit = dbItem.FullWidthQuantityUnit,
                    ItemFeatures = dbItem.ItemFeatures,
                    OtcDrugType = dbItem.OtcDrugType,
                    DosageFormType = dbItem.DosageFormType,
                    PackingStandardName = dbItem.PackingStandardName,
                    ChildrenType = dbItem.ChildrenType,
                    ItemType = dbItem.ItemType,
                    LastUpdate = dbItem.LastUpdate.ToApiDateString(),
                    RequiredReading = dbItem.RequiredReading,
                    Features = dbItem.Features,
                    PrefaceCaution = dbItem.PrefaceCaution,
                    ProhibitedMatters = dbItem.ProhibitedMatters,
                    Consult = dbItem.Consult,
                    OtherCaution = dbItem.OtherCaution,
                    Indications = dbItem.Indications,
                    Dosages = dbItem.Dosages,
                    Ingredient = dbItem.Ingredient,
                    PrecautionForHandling = dbItem.PrecautionForHandling,
                    OtherDescriptions = dbItem.OtherDescriptions,
                    FileKeyN = dbItem.FileEntityN.ConvertAll(x => new QoApiFileKeyItem
                    {
                        FileKeyReference = x.FILEKEY.ToEncrypedReference(),
                        Sequence = x.SEQUENCE.ToString()
                    })
                };
                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "OTC医薬品情報の変換に失敗しました。");
                return false;
            }
        }

        private bool TryReadOtcDrug(string itemCode, string itemCodeType, QoApiResultsBase results, out DbOtcDrugDetailItem dbItem)
        {
            dbItem = null;
            try
            {
                dbItem = _otcDrugRepo.ReadDrug(itemCode, itemCodeType);
                return true;
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "OTC医薬品情報の取得処理でエアラーが発生しました。");
                return false;
            }
        }
    }
}