using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using MGF.QOLMS.QolmsOpenApi.Sql;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// 個人向けお知らせ取得処理
    /// </summary>
    public class NotificationPersonalReadWorker
    {
        readonly IFamilyRepository _familyRepository;
        readonly INoticePersonalRepository _noticePersonalRepository;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="noticePersonalRepository">PersonalRead 用リポジトリ</param>
        /// <param name="familyRepository">家族リポジトリ</param>
        public NotificationPersonalReadWorker(INoticePersonalRepository noticePersonalRepository, IFamilyRepository familyRepository)
        {
            _noticePersonalRepository = noticePersonalRepository;
            _familyRepository = familyRepository;
        }

        /// <summary>
        /// 個人向けお知らせを取得します。
        /// </summary>
        /// <param name="args">引数</param>
        /// <returns>結果</returns>
        public QoNotificationPersonalReadApiResults Read(QoNotificationPersonalReadApiArgs args)
        {
            var results = new QoNotificationPersonalReadApiResults
            {
                IsSuccess = bool.FalseString
            };

            var fromDate = args.FromDate.TryToValueType(DateTime.MinValue);
            var toDate = args.ToDate.TryToValueType(DateTime.MaxValue);
            if (fromDate > toDate)
            {
                results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.ArgumentError, "期間指定が不正です。");
                return results;
            }

            try
            {
                var noticeNo = args.NoticeNo.TryToValueType(long.MinValue);
                if (noticeNo < 0)
                {
                    // 一覧取得は Worker が件数取得とページ境界計算をオーケストレーションする。
                    var onlyUnread = args.OnlyUnread.TryToValueType(false);
                    var pageIndex = args.PageIndex.TryToValueType(int.MinValue);
                    var pageSize = args.PageSize.TryToValueType(int.MinValue);
                    if (pageSize < 1)
                    {
                        throw new ArgumentOutOfRangeException("pageSize", "ページサイズが不正です。");
                    }

                    var normalizedFromDate = fromDate;
                    var normalizedToDate = toDate;
                    DateTime normalizedStartDate;
                    DateTime normalizedEndDate;
                    if (TryNormalizeNoticeDateRange(fromDate, toDate, out normalizedStartDate, out normalizedEndDate))
                    {
                        // 日付条件が有効なときだけ日単位の境界へ丸めて旧 Reader 互換に寄せる。
                        normalizedFromDate = normalizedStartDate;
                        normalizedToDate = normalizedEndDate;
                    }

                    var accountKeyList = this.GetAccountKeyList(args.ActorKey.TryToValueType(Guid.Empty));
                    var systemTypeList = args.ExecuteSystemType.TryToValueType(QsApiSystemTypeEnum.None).ToApplicationTypeByteList();
                    var categoryNo = args.CategoryNo.TryToValueType(byte.MinValue);
                    var totalCount = _noticePersonalRepository.ReadCount(
                        accountKeyList,
                        systemTypeList,
                        normalizedFromDate,
                        normalizedToDate,
                        ShouldJoinAlreadyReadForCount(onlyUnread),
                        categoryNo);

                    var pageRange = CalculatePageRange(totalCount, pageIndex, pageSize);
                    var items = totalCount <= 0
                        ? new List<QH_NOTICEPERSONAL_READ_VIEW>()
                        : _noticePersonalRepository.ReadList(
                            accountKeyList,
                            systemTypeList,
                            normalizedFromDate,
                            normalizedToDate,
                            onlyUnread,
                            pageRange.rowStart,
                            pageRange.rowEnd,
                            categoryNo);

                    results.NoticeN = items.ConvertAll(this.BuildNoticeItem);
                    results.PageIndex = pageRange.clampedPageIndex.ToString();
                    results.MaxPageIndex = pageRange.maxPageIndex.ToString();
                }
                else
                {
                    // noticeNo 指定時はページングせず単件取得に切り替える。
                    var item = _noticePersonalRepository.ReadById(noticeNo);
                    results.NoticeN = item == null
                        ? new List<QoApiNoticeItem>()
                        : new List<QoApiNoticeItem> { this.BuildNoticeItem(item) };
                    results.PageIndex = "0";
                    results.MaxPageIndex = "0";
                }
            }
            catch (Exception ex)
            {
                results.Result = QoApiResult.Build(ex, "個人あてメッセージリストの取得に失敗しました。");
                return results;
            }

            results.IsSuccess = bool.TrueString;
            results.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            return results;
        }

        internal static (int clampedPageIndex, int maxPageIndex, int rowStart, int rowEnd) CalculatePageRange(
            int totalCount, int pageIndex, int pageSize)
        {
            if (totalCount <= 0 || pageSize <= 0)
            {
                return (0, 0, 1, 1);
            }

            // 旧 VB Reader と同じく 0 始まり pageIndex から ROW_NUMBER 用の 1 始まり範囲を作る。
            var maxPageIndex = (int)Math.Ceiling((double)totalCount / pageSize) - 1;
            var clampedPageIndex = pageIndex < 0 ? 0 : (pageIndex > maxPageIndex ? maxPageIndex : pageIndex);
            var rowStart = clampedPageIndex * pageSize + 1;
            var rowEnd = rowStart + pageSize - 1;
            return (clampedPageIndex, maxPageIndex, rowStart, rowEnd);
        }

        internal static bool TryNormalizeNoticeDateRange(DateTime startDate, DateTime endDate, out DateTime normalizedStartDate, out DateTime normalizedEndDate)
        {
            normalizedStartDate = DateTime.MinValue;
            normalizedEndDate = DateTime.MinValue;

            if (startDate <= DateTime.MinValue || endDate <= DateTime.MinValue)
            {
                return false;
            }

            // NoticeDate 条件は日単位比較なので、時刻は日初と日末へ寄せる。
            normalizedStartDate = DateTime.ParseExact(
                startDate.ToString("yyyyMMdd0000000000000"),
                "yyyyMMddHHmmssfffffff",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None);
            normalizedEndDate = DateTime.ParseExact(
                endDate.ToString("yyyyMMdd2359599999999"),
                "yyyyMMddHHmmssfffffff",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None);

            return true;
        }

        internal static bool ResolveSingleAlreadyReadFlag(int recordCount, bool alreadyReadFlag)
        {
            return recordCount == 1 && alreadyReadFlag;
        }

        internal static bool ShouldJoinAlreadyReadForCount(bool onlyUnread)
        {
            return onlyUnread;
        }

        QoApiNoticeItem BuildNoticeItem(QH_NOTICEPERSONAL_READ_VIEW item)
        {
            var dataset = this.TryDeserializeNoticeDataSet(item.NOTICEDATASET);
            var senderName = this.BuildSenderName(dataset, item);
            return new QoApiNoticeItem
            {
                CategoryNo = item.CATEGORYNO.ToString(),
                Contents = item.CONTENTS,
                EndDate = item.ENDDATE.ToApiDateString(),
                FacilityKeyReference = item.FACILITYKEY.ToEncrypedReference(),
                NoticeNo = item.NOTICENO.ToString(),
                PriorityNo = item.PRIORITYNO.ToString(),
                ReadFlag = item.ALREADYREADFLAG.ToString(),
                SenderFamilyName = senderName.familyName,
                SenderGivenName = senderName.givenName,
                StartDate = item.STARTDATE.ToApiDateString(),
                TargetAccountKeyReference = item.ACCOUNTKEY.ToEncrypedReference(),
                Title = item.TITLE
            };
        }

        (string familyName, string givenName) BuildSenderName(QhNoticeDataSetOfJson dataset, QH_NOTICEPERSONAL_READ_VIEW item)
        {
            // 送信者名はデータセット側の表示可否フラグを優先する。
            if (dataset == null || !dataset.SenderDispFlag.TryToValueType(false))
            {
                return (string.Empty, string.Empty);
            }

            if (string.IsNullOrWhiteSpace(item.SENDERFAMILYNAME) || string.IsNullOrWhiteSpace(item.SENDERGIVENNAME))
            {
                return (string.Empty, string.Empty);
            }

            return (item.SENDERFAMILYNAME, item.SENDERGIVENNAME);
        }

        List<Guid> GetAccountKeyList(Guid publicAccountKey)
        {
            var result = new List<Guid>();
            if (publicAccountKey != Guid.Empty)
            {
                result.Add(publicAccountKey);
            }

            // 家族ぶんも同時に取得対象へ含め、重複は最後にまとめて除去する。
            var familyList = _familyRepository.ReadFamilyList(publicAccountKey);
            if (familyList != null)
            {
                result.AddRange(familyList.Select(x => x.ACCOUNTKEY));
            }

            return result.Where(x => x != Guid.Empty).Distinct().ToList();
        }

        QhNoticeDataSetOfJson TryDeserializeNoticeDataSet(string noticeDataSet)
        {
            if (string.IsNullOrWhiteSpace(noticeDataSet))
            {
                return null;
            }

            try
            {
                return new QsJsonSerializer().Deserialize<QhNoticeDataSetOfJson>(noticeDataSet);
            }
            catch
            {
                // データセットが壊れていてもお知らせ本文の返却は継続する。
                return null;
            }
        }
    }
}