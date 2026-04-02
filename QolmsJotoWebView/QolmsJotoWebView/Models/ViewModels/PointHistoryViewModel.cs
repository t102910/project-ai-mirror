using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsJotoWebView.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 「JOTO ポイント履歴」画面ビュー モデルを表します。
    /// このクラスは継承できません。
    /// </summary>
    [Serializable()]
    public sealed class PointHistoryViewModel : QjPointPageViewModelBase
    {
        #region "Public Property"

        /// <summary>
        /// 現在のポイントを取得または設定します。
        /// </summary>
        public int FromPageNoType { get; set; } = int.MaxValue;

        /// <summary>
        /// 現在のポイントを取得または設定します。
        /// </summary>
        public int Point { get; set; } = int.MaxValue;

        /// <summary>
        /// 直近の有効期限を取得または設定します。
        /// </summary>
        public DateTime ClosestExprirationDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 直近の有効期限で失効するポイントを取得または設定します。
        /// </summary>
        public int ClosestExprirationPoint { get; set; } = int.MaxValue;

        /// <summary>
        /// 表示年を取得または設定します。
        /// </summary>
        public int Year { get; set; } = int.MaxValue;

        /// <summary>
        /// 表示月を取得または設定します。
        /// </summary>
        public int Month { get; set; } = int.MaxValue;

        /// <summary>
        /// au契約状態を取得または設定します。
        /// </summary>
        public bool IsMobileSubscriberOfAu { get; set; } = false;

        /// <summary>
        /// 日付毎の JOTO ポイント ログ情報のリストを取得または設定します。
        /// </summary>
        public List<JotoPointDailyLogItem> PointDailyLogN { get; set; } = new List<JotoPointDailyLogItem>();

        /// <summary>
        /// auIDかどうかを取得または設定します。
        /// </summary>
        public bool IsAuId { get; set; } = false;

        /// <summary>
        /// プレミアム会員かどうかを取得または設定します。
        /// </summary>
        public bool IsPremium { get; set; } = false;
        /// <summary>
        /// 法人連携かどうかを取得または設定します。
        /// </summary>
        public bool IsforBiz { get; set; } = false;
        /// <summary>
        /// 病院連携済みかどうかを取得または設定します。
        /// </summary>
        public bool IsConnectedHospital { get; set; } = false;

        /// <summary>
        /// 参加中のチャレンジのリストを取得または設定します。
        /// </summary>
        public Dictionary<Guid, string> ChallengeEntryList { get; set; } = new Dictionary<Guid, string>();

        #endregion

        #region "Constructor"

        //public PointHistoryViewModel(QolmsJotoModel mainModel) : base(mainModel, QjPageNoTypeEnum.PointHistory) { }

        ////検証用
        //public PointHistoryViewModel() : base() { }

        public PointHistoryViewModel(QolmsJotoModel mainModel, int point, DateTime closestExprirationDate, int closestExprirationPoint, int year, int month, List<QoApiQolmsPointHistoryResultItem> items, bool isMobileSubscriberOfAu, QjPageNoTypeEnum fromPageNoType, bool isAuId, bool isPremium, bool isforBiz, bool isConnectedHospital, Dictionary<Guid, string> challengeList) : base()
        {

            this.InitializeBy(point, closestExprirationDate, closestExprirationPoint, year, month, items, isMobileSubscriberOfAu, fromPageNoType, isAuId, isPremium, isforBiz, isConnectedHospital, challengeList);
        }

        #endregion

        #region Private Method

        ///// <summary>
        ///// 有効期限を取得します。
        ///// </summary>
        ///// <param name="itemNo">ポイント項目番号。</param>
        ///// <param name="actionDate">操作日時。</param>
        ///// <param name="targetDate">対象日時。</param>
        ///// <param name="point">ポイント。</param>
        ///// <returns>
        ///// 有効期限。
        ///// </returns>
        //private DateTime GetExpirationDate(int itemNo, DateTime actionDate, DateTime targetDate, int point)
        //{
        //    DateTime result = DateTime.MinValue;

        //    if (itemNo != (int)QjPointItemTypeEnum.None && point > 0)
        //    {
        //        // 加算ポイント
        //        if (itemNo != (int)QjPointItemTypeEnum.Examination)
        //        {
        //            // 健診以外
        //            result = new DateTime(targetDate.Year, targetDate.Month, 1).AddMonths(7).AddDays(-1);
        //            // ポイント有効期限は 6 ヶ月後の月末（起点は測定日）
        //        }
        //        else
        //        {
        //            // 健診
        //            result = new DateTime(actionDate.Year, actionDate.Month, 1).AddMonths(7).AddDays(-1);
        //            // ポイント有効期限は 6 ヶ月後の月末（起点は操作日時）
        //        }
        //    }

        //    return result;
        //}

        /// <summary>
        /// 値を指定して、
        /// <see cref="PortalHistoryViewModel" /> クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="point">現在のポイント。</param>
        /// <param name="closestExprirationDate">直近の有効期限。</param>
        /// <param name="closestExprirationPoint">直近の有効期限で失効するポイント。</param>
        /// <param name="year">表示年。</param>
        /// <param name="month">表示月。</param>
        /// <param name="items">QOLMS ポイント履歴情報のコレクション。</param>
        /// <param name="isMobileSubscriberOfAu">au携帯加入者かどうか。</param>
        /// <param name="fromPageNoType">ページ番号タイプ。</param>
        /// <param name="isAuId">au IDかどうか。</param>
        /// <param name="isPremium">プレミアムかどうか。</param>
        /// <param name="isforBiz">法人連携かどうか。</param>
        /// <param name="isConnectedHospital">病院連携があるかどうか。</param>
        /// <param name="challengeList">チャレンジリスト。</param>
        private void InitializeBy(
            int point,
            DateTime closestExprirationDate,
            int closestExprirationPoint,
            int year,
            int month,
            List<QoApiQolmsPointHistoryResultItem> items,
            bool isMobileSubscriberOfAu,
            QjPageNoTypeEnum fromPageNoType,
            bool isAuId,
            bool isPremium,
            bool isforBiz,
            bool isConnectedHospital,
            Dictionary<Guid, string> challengeList)
        {
            this.Point = point;
            this.ClosestExprirationDate = closestExprirationDate;
            this.ClosestExprirationPoint = closestExprirationPoint;
            this.Year = year;
            this.Month = month;
            this.IsMobileSubscriberOfAu = isMobileSubscriberOfAu;
            this.FromPageNoType = (int)fromPageNoType;
            this.IsAuId = isAuId;
            this.IsPremium = isPremium;
            this.IsforBiz = isforBiz;
            this.IsConnectedHospital = isConnectedHospital;
            this.ChallengeEntryList = challengeList;

            if (items != null && items.Any())
            {
                // 日付毎にまとめる
                var logN = new Dictionary<DateTime, List<QoApiQolmsPointHistoryResultItem>>();

                foreach (var item in items)
                {
                    DateTime key = item.ActionDate.TryToValueType(DateTime.MinValue).Date;

                    if (key.Year == year && key.Month == month)
                    {
                        if (!logN.ContainsKey(key))
                        {
                            logN.Add(key, new List<QoApiQolmsPointHistoryResultItem>());
                        }

                        logN[key].Add(item);
                    }
                }

                // 降順にソートされた日付毎の JOTO ポイント ログ情報のリストを作成する
                if (logN.Any())
                {
                    foreach (var item in logN.OrderByDescending(i => i.Key))
                    {
                        this.PointDailyLogN.Add(
                            new JotoPointDailyLogItem()
                            {
                                ActionDate = item.Key,
                                PointLogN = item.Value
                                    .ConvertAll(
                                        i => new JotoPointLogItem()
                                        {
                                            ActionDate = i.ActionDate.TryToValueType(DateTime.MinValue),
                                            TargetDate = i.PointTargetDate.TryToValueType(DateTime.MinValue),
                                            ItemNo = i.PointItemNo.TryToValueType(0),
                                            ItemName = i.PointItemName,
                                            Point = i.PointValue.TryToValueType(0),
                                            Reason = i.PointReason,
                                            ExpirationDate = i.ExprirationDate.TryToValueType(DateTime.MinValue)
                                        }
                                    )
                                    .OrderByDescending(i => i.ActionDate)
                                    .ThenByDescending(i => i.ItemNo)
                                    .ToList(),
                                Point = item.Value.Sum(i => i.PointValue.TryToValueType(0))
                            }
                        );
                    }
                }
            }
        }

        #endregion
    }
}
