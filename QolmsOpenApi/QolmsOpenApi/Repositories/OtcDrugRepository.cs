using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsKddiMessageCastApiCoreV1F472.Extension;
using MGF.QOLMS.QolmsOpenApi.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Repositories
{
    /// <summary>
    /// OTC医薬品マスタ入出力インターフェース
    /// </summary>
    public interface IOtcDrugRepository
    {
        /// <summary>
        /// OTC医薬品を検索します。
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        (List<DbOtcDrugSearchItem> drugN, int pageIndex, int maxPageIndex) SearchDrug(string searchText, int pageIndex, int pageSize);

        /// <summary>
        /// OTC医薬品詳細情報を取得します。
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="itemCodeType"></param>
        /// <returns></returns>
        DbOtcDrugDetailItem ReadDrug(string itemCode, string itemCodeType);
    }

    /// <summary>
    /// OTC医薬品マスタ入出力実装
    /// </summary>
    public class OtcDrugRepository: IOtcDrugRepository
    {
        /// <summary>
        /// OTC医薬品を検索します。
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public (List<DbOtcDrugSearchItem> drugN, int pageIndex, int maxPageIndex) SearchDrug(string searchText, int pageIndex, int pageSize)
        {
            var searchar = new DbOtcDrugSearcher();
            var args = new DbOtcDrugSearcherArgs
            {
                SearchText = searchText,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            var result = QsDbManager.Read(searchar, args);

            if(result == null || !result.IsSuccess || result.OtcDrugN == null)
            {
                throw new Exception();
            }

            return (result.OtcDrugN, result.PageIndex, result.MaxPageIndex);
        }

        /// <inheritdoc/>
        public DbOtcDrugDetailItem ReadDrug(string itemCode, string itemCodeType)
        {
            var reader = new DbOtcDrugReader();
            var args = new DbOtcDrugReaderArgs
            {
                ItemCode = itemCode,
                ItemCodeType = itemCodeType
            };

            var result = QsDbManager.Read(reader, args);

            if (result == null || !result.IsSuccess || result.Entity == null)
            {
                throw new Exception();
            }

            return result.Entity;
            
        }
    }
}