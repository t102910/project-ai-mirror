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
    /// お知らせ関連の処理を行います。
    /// </summary>
    public class MasterNoticeWorker
    {
        INoticeRepository _repo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="noticeRepository"></param>
        public MasterNoticeWorker(INoticeRepository noticeRepository)
        {
            _repo = noticeRepository;
        }

        /// <summary>
        /// お知らせリストを取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoMasterNoticeReadApiResults Read(QoMasterNoticeReadApiArgs args)
        {
            var result = new QoMasterNoticeReadApiResults
            {
                IsSuccess = bool.FalseString
            };

            // 非ログイン時（ゲストアカウント）でもお知らせ取得可能なので、アカウントキーが無い場合はExecutorを代用します。
            var accountKey = args.ActorKey.TryToValueType(Guid.Empty);
            if(accountKey == Guid.Empty)
            {
                accountKey = args.Executor.TryToValueType(Guid.Empty);
            }

            DbNoticeListReaderResults dbListResults;
            try
            {
                // DBよりお知らせ取得
                dbListResults = _repo.ReadList(
                    accountKey,
                    args.ExecuteSystemType.TryToValueType(QsApiSystemTypeEnum.None),
                    args.FromDate.TryToValueType(DateTime.MinValue),
                    args.ToDate.TryToValueType(DateTime.MinValue),
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
                // DBクラスを変換
                result.NoticeN = dbListResults.NoticeListN.ConvertAll(x => new QoApiNoticeItem
                {
                    NoticeNo = x.NOTICENO.ToString(),
                    CategoryNo = x.CATEGORYNO.ToString(),
                    PriorityNo = x.PRIORITYNO.ToString(),
                    Title = x.TITLE,
                    Contents = x.CONTENTS,
                    FacilityKeyReference = "",
                    StartDate = x.STARTDATE.ToApiDateString(),
                    EndDate = x.ENDDATE.ToApiDateString(),
                });

                result.PageIndex = dbListResults.PageIndex.ToString();
                result.MaxPageIndex = dbListResults.MaxPageIndex.ToString();

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