using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MGF.QOLMS.QolmsDbEntityV1;
namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// お知らせ取得処理
    /// </summary>
    public class NotificationReadWorker
    {
        ILinkageRepository _linkageRepository;
        INoticeRepository _noticeRepository;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="noticeRepository"></param>
        /// <param name="linkageRepository"></param>
        public NotificationReadWorker(INoticeRepository noticeRepository, ILinkageRepository linkageRepository)
        {
            _noticeRepository = noticeRepository;
            _linkageRepository = linkageRepository;
        }

        /// <summary>
        /// お知らせを取得する
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public QoNotificationReadApiResults Read(QoNotificationReadApiArgs args)
        {
            var results = new QoNotificationReadApiResults
            {
                IsSuccess = bool.FalseString
            };

            var fromDate = args.FromDate.TryToValueType(DateTime.MinValue);
            var toDate = args.ToDate.TryToValueType(DateTime.MaxValue);

            // 日付範囲チェック
            if (!CheckRange(fromDate, toDate, results))
            {
                return results;
            }

            var categoryNoList = new List<byte>();
            if (args.CategoryNo != null && args.CategoryNo.Any())
            {
                args.CategoryNo.ForEach(category => {
                    categoryNoList.Add(category.TryToValueType(byte.MinValue));
                });
            }

            // 親施設フラグ 互換性のため規定値True
            var includeParent = args.IncludeParentFacility.TryToValueType(true);
            // 医療ナビ特殊 アプリお知らせフラグ 規定値False
            var includeApp = args.IncludeAppNotification.TryToValueType(false);

            // 施設フィルター用のリストを生成
            if(!TryGetFacilityList(args.FacilityKeyReference,includeApp,includeParent,results,out var facilityKeyList))
            {
                return results;
            }


            var noticeId = args.NoticeNo.TryToValueType(long.MinValue);
            var accountKey = args.ActorKey.TryToValueType(Guid.Empty);

            var pageIndex = args.PageIndex.TryToValueType(int.MinValue);
            var pageSize = args.PageSize.TryToValueType(int.MinValue);

            // お知らせ対象SystemTypeのリストを生成(iOS固有 + アプリ全体となるようにする)
            var systemTypeList = args.ExecuteSystemType.TryToValueType(QsApiSystemTypeEnum.None)
                .ToApplicationTypeByteList();

            var entityList = new List<QH_NOTICE_DAT>();
            try
            {
                if (noticeId < 0)
                {
                    // Id指定なし
                    var readResult = _noticeRepository.ReadList(accountKey, systemTypeList , facilityKeyList, categoryNoList, fromDate, toDate, pageIndex, pageSize);

                    results.PageIndex = readResult.pageIndex.ToString();
                    results.MaxPageIndex = readResult.maxPageIndex.ToString();
                    entityList = readResult.items;
                }
                else
                {
                    // Id指定
                    entityList.Add(_noticeRepository.ReadById(noticeId));
                    results.PageIndex = "0";
                    results.MaxPageIndex = "0";
                }
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "お知らせリストの取得に失敗しました。");
                return results;
            }

            try
            {
                // EntityからApiItemに変換
                results.NoticeN = entityList.ConvertAll(x => x.ToApiNoticeItem());
            }
            catch(Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "お知らせリストの変換に失敗しました。");
                return results;
            }

            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            return results;
        }

        bool CheckRange(DateTime from, DateTime to, QoApiResultsBase results)
        {
            if (from > to)
            {            
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "期間指定が不正です。");
                return false;
            }

            return true;
        }

        bool TryGetFacilityList(string facilityKeyReference,bool includeApp, bool includeParent, QoApiResultsBase results, out List<Guid> facilityList)
        {
            facilityList = new List<Guid>();
            if(string.IsNullOrEmpty(facilityKeyReference))
            {
                return true;
            }

            var targetFacilityKey = facilityKeyReference.ToDecrypedReference().TryToValueType(Guid.Empty);
            if (targetFacilityKey != Guid.Empty)
            {
                facilityList.Add(targetFacilityKey);
            }

            if (includeApp)
            {
                // 医療ナビではアプリからのお知らせは施設キーにGuid.Empty設定されるのでそれを含めるようにする
                facilityList.Add(Guid.Empty);
            }

            if (includeParent)
            {                
                try
                {
                    // 親施設を取得
                    var parent = _linkageRepository.GetParentLinkageMst(targetFacilityKey);
                    if(parent != null && parent.FACILITYKEY != Guid.Empty)
                    {
                        // 存在すれば追加
                        facilityList.Add(parent.FACILITYKEY);
                    }
                }
                catch(Exception ex)
                {
                    results.Result = QoApiResult.Build(ex, "親施設情報の取得に失敗しました。");
                    return false;
                }
            }

            return true;
        }        
    }    
}