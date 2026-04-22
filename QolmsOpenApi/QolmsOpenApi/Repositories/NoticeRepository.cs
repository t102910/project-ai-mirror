using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Repositories
{
    /// <summary>
    /// お知らせ入出力インターフェース
    /// </summary>
    public interface INoticeRepository
    {
        /// <summary>
        /// お知らせリストを取得
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="executeSystemType"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        DbNoticeListReaderResults ReadList(
            Guid accountKey,
            QsApiSystemTypeEnum executeSystemType,
            DateTime from,
            DateTime to,
            int pageIndex,
            int pageSize);

        /// <summary>
        /// お知らせリストを取得
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="toSystemTypeList"></param>
        /// <param name="facilityKeyList"></param>
        /// <param name="categoryNoList"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        (List<QH_NOTICE_DAT> items, int pageIndex, int maxPageIndex) ReadList(Guid accountKey, List<byte> toSystemTypeList, List<Guid> facilityKeyList, List<byte> categoryNoList, DateTime startDate, DateTime endDate, int pageIndex, int pageSize);

        /// <summary>
        /// お知らせをIDを指定して1件取得
        /// </summary>
        /// <param name="noticeNo"></param>
        /// <returns></returns>
        QH_NOTICE_DAT ReadById(long noticeNo);
    }

    /// <summary>
    /// お知らせ入出力実装
    /// </summary>
    public class NoticeRepository: INoticeRepository
    {
        /// <summary>
        /// お知らせリストを取得
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="executeSystemType"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public DbNoticeListReaderResults ReadList(
            Guid accountKey, 
            QsApiSystemTypeEnum executeSystemType,
            DateTime from, 
            DateTime to,
            int pageIndex,
            int pageSize)
        {
            var reader = new DbNoticeListReader();
            var args = new DbNoticeListReaderArgs
            {
                AccountKey = accountKey,
                CreateStartDate = DateTime.MinValue,
                CreateEndDate = DateTime.MinValue,
                FacilityKeyN = new List<Guid>(),
                FromSystemType = byte.MinValue,
                ToSystemTypeList = new List<byte>
                {
                    (byte)executeSystemType
                },
                NoticeStartDate = from,
                NoticeEndDate = to,
                CategoryNoN = new List<byte> { byte.MinValue },
                PriorityNo = byte.MinValue,
                IsPushSend = false,
                FilterFlag = QolmsDbEntityV1.QsDbNoticeFilterTypeEnum.ToSystem | QolmsDbEntityV1.QsDbNoticeFilterTypeEnum.NoticeDate,
                SortKey = string.Empty,
                PageSize = pageSize,
                PageIndex = pageIndex,
            };

            var result = QsDbManager.Read(reader, args);

            // 結果null, 失敗, 結果Listがnullの場合はエラーとする
            if(result == null || !result.IsSuccess || result.NoticeListN == null)
            {
                throw new Exception();
            }

            return result;
        }

        /// <summary>
        /// お知らせリストを取得
        /// </summary>
        /// <param name="accountKey"></param>
        /// <param name="toSystemTypeList"></param>
        /// <param name="facilityKeyList"></param>
        /// <param name="categoryNoList"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public (List<QH_NOTICE_DAT> items, int pageIndex, int maxPageIndex) ReadList(Guid accountKey,List<byte> toSystemTypeList, List<Guid> facilityKeyList, List<byte> categoryNoList, DateTime startDate, DateTime endDate, int pageIndex, int pageSize)
        {
            var args = new DbNoticeListReaderArgs()
            {
                AccountKey = accountKey,
                ToSystemTypeList = toSystemTypeList,
                PageIndex = pageIndex,
                PageSize = pageSize,
                NoticeStartDate = startDate,
                NoticeEndDate = endDate,
                FacilityKeyN = facilityKeyList,
                FilterFlag = QsDbNoticeFilterTypeEnum.ToSystem | QsDbNoticeFilterTypeEnum.Facility | QsDbNoticeFilterTypeEnum.NoticeDate
            };

            if (categoryNoList.Any())
            {
                args.CategoryNoN = categoryNoList;
                args.FilterFlag |= QsDbNoticeFilterTypeEnum.CategoryNo;
            }

            var result = QsDbManager.Read(new DbNoticeListReader(), args);
            if (result.IsSuccess && result.NoticeListN != null)
            {
                return (result.NoticeListN, result.PageIndex, result.MaxPageIndex);
            };

            // 結果が取れなかった場合はエラーとする
            throw new InvalidOperationException();
        }

        /// <summary>
        /// お知らせをIDを指定して1件取得
        /// </summary>
        /// <param name="noticeNo"></param>
        /// <returns></returns>
        public QH_NOTICE_DAT ReadById(long noticeNo)
        {
            var args = new QhNoticeEntityReaderArgs
            {
                Data = new List<QH_NOTICE_DAT>
                {
                    new QH_NOTICE_DAT
                    {
                        NOTICENO = noticeNo
                    }
                }
            };

            var readerResults = QsDbManager.Read(new QhNoticeEntityReader(),args);
            if (readerResults != null && readerResults.IsSuccess && readerResults.Result.Count == 1)
            {
                return readerResults.Result.FirstOrDefault(x => !x.DELETEFLAG);
            }

            return null;
        }
    }
}