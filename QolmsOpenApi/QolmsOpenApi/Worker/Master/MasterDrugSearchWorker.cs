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
    /// OTC医薬品関連の処理を行います。
    /// </summary>
    public class MasterDrugSearchWorker
    {
        IOtcDrugRepository _repo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="otcDrugRepository"></param>
        public MasterDrugSearchWorker(IOtcDrugRepository otcDrugRepository)
        {
            _repo = otcDrugRepository;
        }

        /// <summary>
        /// OTC医薬品を検索します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoMasterDrugSearchApiResults Search(QoMasterDrugSearchApiArgs args)
        {
            var result = new QoMasterDrugSearchApiResults
            {
                IsSuccess = bool.FalseString
            };

            List<DbOtcDrugSearchItem> dbDrugN;
            int pageIndex;
            int maxPageIndex;
            try
            {
                 (dbDrugN, pageIndex, maxPageIndex) = _repo.SearchDrug(
                    args.SearchText,
                    args.PageIndex.TryToValueType(0),
                    args.PageSize.TryToValueType(50)
                );
            }
            catch(Exception ex)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.DatabaseError);
                return result;
            }

            try
            {
                result.OtcDrugN = dbDrugN.ConvertAll(x => new QoApiOtcDrugHeaderItem
                {
                    ItemCode = x.ItemCode,
                    ItemCodeType = x.ItemCodeType,
                    MakerName = x.MakerOfficialItemName,
                    ItemName = x.MakerOfficialItemName,
                    ContentQuantity = x.FullWidthContent.ToString(),
                    ContentUnit = x.FullWidthContentUnit.ToString(),
                    ItemUnit = x.FullWidthQuantityUnit
                });

                result.PageIndex = pageIndex.ToString();
                result.MaxPageIndex = maxPageIndex.ToString();

                result.IsSuccess = bool.TrueString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);

                return result;
            }
            catch(Exception ex)
            {
                result.Result = QoApiResult.Build(ex);
                return result;
            }
        }
    }
}