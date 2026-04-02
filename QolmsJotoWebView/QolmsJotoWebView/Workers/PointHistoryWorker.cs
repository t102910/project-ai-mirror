using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsJotoWebView.Models;
using MGF.QOLMS.QolmsJotoWebView.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace MGF.QOLMS.QolmsJotoWebView.Workers
{

    /// <summary>
    /// 「JOTO ポイント履歴」画面に関する機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    public class PointHistoryWorker
    {
        /// <summary>
        /// auidの正規表現チェック
        /// 他のIDで似たような形式があったら通ってしまうので移植時はopenidtypeを取得してチェックしてください。
        /// </summary>
        private static readonly Regex REGEX_AU_ID = new Regex(@"^https://.+/*$", RegexOptions.IgnoreCase);

        IPointRepository _pointRepo;

        /// <summary>
        /// デフォルト コンストラクタは使用できません。
        /// </summary>
        public PointHistoryWorker(IPointRepository pointRepo) 
        {
            _pointRepo = pointRepo;

        }

        /// <summary>
        /// 「JOTO ポイント履歴」画面ビュー モデル作成します。
        /// </summary>
        /// <param name="mainModel">メイン モデル。</param>
        /// <param name="year">表示年。</param>
        /// <param name="month">表示月。</param>
        /// <param name="FromPageNoType">ページ種別。</param>
        /// <returns>
        /// 「JOTO ポイント履歴」画面ビュー モデル。
        /// </returns>
        public PointHistoryViewModel CreateViewModel(QolmsJotoModel mainModel, int? year, int? month, QjPageNoTypeEnum FromPageNoType)
        {

            int point = int.MinValue;
            DateTime closestExprirationDate = DateTime.MinValue;
            int closestExprirationPoint = int.MinValue;
            int targetYear = year.HasValue? year.Value: DateTime.Now.Year;
            int targetMonth =  month.HasValue? month.Value: DateTime.Now.Month;
            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.MinValue;
            var items = new List<QoApiQolmsPointHistoryResultItem>();
            var serviceNo = 47003;
            // 保有ポイント、直近の失効ポイントを取得
            try
            {
                point = _pointRepo.GetQolmsPointWithClosestExpriration(
                    mainModel.ApiExecutor,
                    mainModel.ApiExecutorName,
                    mainModel.SessionId,
                    mainModel.ApiAuthorizeKey,
                    mainModel.AuthorAccount.AccountKey,
                    serviceNo,
                    ref closestExprirationDate,
                    ref closestExprirationPoint
                );
            }
            catch
            {
                point = 0;
                closestExprirationDate = DateTime.MinValue;
                closestExprirationPoint = 0;
            }

            // ポイント履歴を取得
            if (targetYear < 1 || targetYear > 9999 || targetMonth < 1 || targetMonth > 12)
            {
                var now = DateTime.Now;
                targetYear = targetYear;
                targetMonth = now.Month;
            }

            fromDate = new DateTime(targetYear, targetMonth, 1);
            toDate = fromDate.AddMonths(1).AddDays(-1);

            try
            {
                items = _pointRepo.GetTargetPointFromHistoryList(
                    mainModel.ApiExecutor,
                    mainModel.ApiExecutorName,
                    mainModel.SessionId,
                    mainModel.ApiAuthorizeKey,
                    mainModel.AuthorAccount.AccountKey,
                    serviceNo,
                    QjPointItemTypeEnum.None,
                    fromDate,
                    toDate
                );

                // 減算ポイント
                int exprirat = items.Where(i => i.PointItemNo == "0").ToList().Sum(j => int.Parse(j.PointValue));
                if (exprirat < 0)
                {
                    var expriratItem = items.Where(i => i.PointItemNo == "0").ToList().First();
                    expriratItem.PointValue = exprirat.ToString();
                    var newItems = new List<QoApiQolmsPointHistoryResultItem>();
                    newItems.Add(expriratItem);
                    newItems.AddRange(items.Where(i => i.PointItemNo != "0").ToList());

                    items = newItems;
                }
            }
            catch
            {
                items = new List<QoApiQolmsPointHistoryResultItem>();
            }

            bool isAuId = false;
            if (!string.IsNullOrWhiteSpace(mainModel.AuthorAccount.OpenId) &&
                REGEX_AU_ID.IsMatch(mainModel.AuthorAccount.OpenId))
            {
                isAuId = true;
            }

            //// au契約があるかどうか
            bool isMobileSubscriberOfAu = false;
            //try
            //{
            //    isMobileSubscriberOfAu = AuOwlAccessWorker.IsMobileSubscriberOfAu(mainModel.AuthorAccount.OpenId);
            //}
            //catch (Exception ex)
            //{
            //    string message = ex.Message;
            //}

            isMobileSubscriberOfAu = false;

            // 最新の会員ステータスを取得
            //mainModel.AuthorAccount.MembershipType = (QjMemberShipTypeEnum)Enum.ToObject(
            //    typeof(QjMemberShipTypeEnum),
            //    PremiumWorker.GetMemberShipType(mainModel)
            //);

            // プレミアムかどうか
            bool isPremium =
                mainModel.AuthorAccount.MembershipType == QjMemberShipTypeEnum.LimitedTime ||
                mainModel.AuthorAccount.MembershipType == QjMemberShipTypeEnum.Premium;

            // 法人連携かどうか
            bool isforBiz =
                mainModel.AuthorAccount.MembershipType == QjMemberShipTypeEnum.Business ||
                mainModel.AuthorAccount.MembershipType == QjMemberShipTypeEnum.BusinessFree;

            // 病院連携があるかどうか
            bool IsConnectedHospital = false;
                
                //PortalHomeWorker.GetMedicalLinkageList(mainModel)
                //.Where(i => i.StatusType == "2")
                //.Count() > 0;

            var challengeList = new Dictionary<Guid,string>();
                //PortalChallengeWorker.GetChallengeEntryList(mainModel);

            // ビュー モデルを返却
            return new PointHistoryViewModel(
                mainModel,
                point,
                closestExprirationDate,
                closestExprirationPoint,
                targetYear,
                targetMonth,
                items,
                isMobileSubscriberOfAu,
                FromPageNoType,
                isAuId,
                isPremium,
                isforBiz,
                IsConnectedHospital,
                challengeList
            );
        }
    }

}