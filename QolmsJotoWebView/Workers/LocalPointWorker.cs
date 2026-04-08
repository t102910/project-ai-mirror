using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsJotoWebView.Models;
using MGF.QOLMS.QolmsJotoWebView.Repositories;
using MGF.QOLMS.QolmsJotoWebView.Workers;
using MGF.QOLMS.QolmsJwtAuthCore;

namespace MGF.QOLMS.QolmsJotoWebView
{
    // 地域ポイント履歴/交換の取得・整形を担当するワーカー
    public class LocalPointWorker
    {
        // Wrapper API 呼び出し
        ILocalPointRepository _localPointRepo;
        // LinkageSystemId 解決 API 呼び出し
        ILinkageRepository _linkageRepo;
        // QOLMS ポイント API 呼び出し
        IPointRepository _pointRepo;

        #region Constant

        #endregion

        /// <summary>
        /// デフォルト コンストラクタは使用できません。
        /// </summary>
        public LocalPointWorker(ILocalPointRepository localPointRepo, ILinkageRepository linkageRepo, IPointRepository pointRepo)
        {
            _localPointRepo = localPointRepo;
            _linkageRepo = linkageRepo;
            _pointRepo = pointRepo;
        }

        #region Private Method

        private string CreateJotoApikey(string executor, Guid actorAccountKey)
        {
            string result = string.Empty;

            // Open API 用の JWT アクセスキーを生成
            int func = 1024;
            var encExecutor = string.Empty;
            using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
            {
                encExecutor = crypt.EncryptString(executor);
            }
            result = new QsJwtTokenProvider().CreateOpenApiJwtAccessKey(encExecutor, actorAccountKey, Guid.Empty, func);

            return result;
        }

        private static int ParsePointValue(string pointText)
        {
            // ポイント文字列は null/空/符号付き文字列が混在するため、常に安全に数値化する
            if (string.IsNullOrWhiteSpace(pointText))
            {
                return 0;
            }

            if (int.TryParse(pointText, out var value))
            {
                return value;
            }

            var sign = pointText.StartsWith("-") ? -1 : 1;
            var absValueText = pointText.TrimStart('+', '-');

            return int.TryParse(absValueText, out var absValue)
                ? sign * absValue
                : 0;
        }

        private static string NormalizeItemName(string itemName)
        {
            // OCC API 側で SourceName が文字列 "null" として返るケースがあるため、空文字として扱う
            if (string.IsNullOrWhiteSpace(itemName))
            {
                return string.Empty;
            }

            var normalized = itemName.Trim();
            if (normalized.Equals("null", StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            return normalized;
        }

        private static List<JotoPointDailyLogItem> CreatePointDailyLogs(IEnumerable<QoApiLocalPointHistoryItem> history)
        {
            if (history == null)
            {
                return new List<JotoPointDailyLogItem>();
            }

            // API 履歴を「日単位ヘッダ + 明細行」に変換する
            return history
                // 日付キーでグルーピング（パース不可は MinValue 扱い）
                .GroupBy(h => DateTime.TryParse(h.TransactionDate, out var dt) ? dt.Date : DateTime.MinValue)
                // パース不能なレコード群は描画対象外
                .Where(g => g.Key != DateTime.MinValue)
                // 画面表示順は新しい日付を先頭
                .OrderByDescending(g => g.Key)
                .Select(g => new JotoPointDailyLogItem
                {
                    ActionDate = g.Key,
                    // 日内の各トランザクションを明細へマッピング
                    PointLogN = g.Select(item => new JotoPointLogItem
                    {
                        ActionDate = DateTime.TryParse(item.TransactionDate, out var ad) ? ad : DateTime.MinValue,
                        TargetDate = DateTime.TryParse(item.TransactionDate, out var td) ? td : DateTime.MinValue,
                        ItemNo = 0, // 必要に応じてマッピング
                        ItemName = NormalizeItemName(item.SourceName),
                        Point = ParsePointValue(item.Points),
                        Reason = item.TransactionType ?? string.Empty,
                        ExpirationDate = DateTime.TryParse(item.ValidUntil, out var ed) ? ed : DateTime.MinValue
                    }).ToList(),
                    // 日次ヘッダに表示するポイント合計
                    Point = g.Sum(item => ParsePointValue(item.Points))
                })
                .ToList();
        }

        #endregion

        #region Public Method

        /// <summary>
        /// 「ローカルポイント」画面 ビュー モデル を作成します。
        /// </summary>
        public PointLocalHistoryInputModel CreateViewModel(QolmsJotoModel mainModel, DateTime targetDate)
        {
            var apikey = this.CreateJotoApikey(mainModel.ApiExecutor.ToString(), mainModel.AuthorAccount.AccountKey);

            // アカウントキーから LinkageSystemId を取得する API の 呼び出し
            QoLinkageLinkageSystemIdReadApiResults linkageApiResults = _linkageRepo.ExecuteLinkageSystemIdReadApi(mainModel, apikey);
            var linkageSystemId = linkageApiResults.LinkageSystemId;

            // OCC 地域ポイント合計 API の Wrapper API 呼び出し
            QoJotoHdrLocalPointSummaryReadApiResults summaryApiResults = _localPointRepo.ExecuteLocalPointSummaryReadApi(mainModel, linkageSystemId, apikey);
            // OCC 地域ポイント履歴 API の Wrapper API 呼び出し
            QoJotoHdrLocalPointHistoryReadApiResults historyApiResults = _localPointRepo.ExecuteLocalPointHistoryReadApi(mainModel, targetDate, linkageSystemId, apikey);

            // 日付ごとにまとめてJotoPointDailyLogItemリストを作成
            var dailyLogs = CreatePointDailyLogs(historyApiResults.History);

            // 月末を取得
            DateTime today = DateTime.Today;
            DateTime endOfMonth = new DateTime(
                today.Year,
                today.Month,
                DateTime.DaysInMonth(today.Year, today.Month)
            );

            return new PointLocalHistoryInputModel(mainModel)
            {
                // 指定年月で履歴を表示
                Year = targetDate.Year,
                Month = targetDate.Month,
                // 現在利用可能ポイント
                Point = summaryApiResults.Data.TotalAvailablePoints.TryToValueType(int.MinValue),
                // 履歴表示データ
                PointDailyLogN = dailyLogs,
                // 失効情報（画面上部表示）
                ClosestExprirationDate = endOfMonth,
                ClosestExprirationPoint = summaryApiResults.Data.ExpireEndofMonth.TryToValueType(int.MinValue) // 直近失効予定ポイント(月末)
            };
        }

        /// <summary>
        /// 地域ポイントの変換を実行し、表示モデルを更新します。
        /// </summary>
        public PointLocalHistoryInputModel RedeemLocalPoint(QolmsJotoModel mainModel, PointLocalHistoryInputModel currentModel, int redeemPoint)
        {
            if (currentModel == null)
            {
                throw new ArgumentNullException(nameof(currentModel));
            }

            var targetDate = new DateTime(currentModel.Year, currentModel.Month, 1);
            var apikey = this.CreateJotoApikey(mainModel.ApiExecutor.ToString(), mainModel.AuthorAccount.AccountKey);
            var actionDate = DateTime.Now;
            var pointLimitDate = new DateTime(
                actionDate.Year,
                actionDate.Month,
                1
            ).AddMonths(7).AddDays(-1); // ポイント有効期限は 6 ヶ月後の月末（起点は操作日時）

            // アカウントキーから LinkageSystemId を取得する API の 呼び出し
            QoLinkageLinkageSystemIdReadApiResults linkageApiResults = _linkageRepo.ExecuteLinkageSystemIdReadApi(mainModel, apikey);
            var linkageSystemId = linkageApiResults.LinkageSystemId;

            // 先に QOLMS ポイントを加算し、変換失敗時は後続で巻き戻す
            var pointResult = _pointRepo.AddQolmsPoints(
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName,
                mainModel.SessionId,
                mainModel.ApiAuthorizeKey,
                mainModel.AuthorAccount.AccountKey,
                QjConfiguration.PointServiceno,
                new List<QolmsPointGrantItem>
                {
                    new QolmsPointGrantItem()
                    {
                        ActionDate = actionDate,
                        SerialCode = Guid.NewGuid().ToApiGuidString(),
                        PointItemNo = (int)QjPointItemTypeEnum.LocalPointRedeem,
                        Point = redeemPoint,
                        PointTargetDate = actionDate,
                        PointExpirationDate = pointLimitDate,
                        Reason = "ローカルポイント変換"
                    }
                }
            );

            if (pointResult.First().Value != 0)
            {
                throw new InvalidOperationException(string.Format("ポイントの加算に失敗しました。error:{0}", pointResult.First().Key));
            }

            try
            {
                // OCC 地域ポイント交換 API の Wrapper API 呼び出し
                var redeemApiResults = _localPointRepo.ExecuteLocalPointRedeemReadApi(mainModel, redeemPoint, linkageSystemId, apikey);
                var apiRedeemedPoint = redeemApiResults.RedeemedPoints.TryToValueType(int.MinValue);
                // API 本体の値が未設定のときは明細合計から実交換ポイントを補完
                if (apiRedeemedPoint == int.MinValue && redeemApiResults.RedemptionDetails != null)
                {
                    apiRedeemedPoint = redeemApiResults.RedemptionDetails
                        .Sum(detail => detail.PointsUsed.TryToValueType(0));
                }

                // 交換要求値と API 応答値の不整合を検出
                if (apiRedeemedPoint != int.MinValue && Math.Abs(apiRedeemedPoint) != redeemPoint)
                {
                    throw new InvalidOperationException("ポイント変換結果が不正です。");
                }

                var remainingPoint = redeemApiResults.RemainingBalance.TryToValueType(int.MinValue);
                // 残高が負値なら異常応答とみなす
                if (remainingPoint < 0)
                {
                    throw new InvalidOperationException("残高ポイントの取得に失敗しました。");
                }

                // 履歴を再取得して画面を再描画できる状態に更新
                var historyApiResults = _localPointRepo.ExecuteLocalPointHistoryReadApi(mainModel, targetDate, linkageSystemId, apikey);
                currentModel.PointDailyLogN = CreatePointDailyLogs(historyApiResults.History);
                currentModel.Point = remainingPoint;

                return currentModel;
            }
            catch (Exception)
            {
                // 変換失敗時は加算済みポイントを巻き戻す
                var rollbackResult = _pointRepo.AddQolmsPoints(
                    mainModel.ApiExecutor,
                    mainModel.ApiExecutorName,
                    mainModel.SessionId,
                    mainModel.ApiAuthorizeKey,
                    mainModel.AuthorAccount.AccountKey,
                    QjConfiguration.PointServiceno,
                    new List<QolmsPointGrantItem>
                    {
                        new QolmsPointGrantItem()
                        {
                            ActionDate = actionDate,
                            SerialCode = Guid.NewGuid().ToApiGuidString(),
                            PointItemNo = (int)QjPointItemTypeEnum.RecoveryPoint,
                            Point = redeemPoint * -1,
                            PointTargetDate = actionDate,
                            PointExpirationDate = DateTime.MaxValue,
                            Reason = "ローカルポイント変換失敗のためポイント巻き戻し"
                        }
                    }
                );

                if (rollbackResult.First().Value != 0)
                {
                    var bodyString = new StringBuilder();
                    bodyString.AppendLine("ローカルポイント変換のポイント巻き戻しエラーです。");
                    bodyString.AppendLine(string.Format("error:{0}", rollbackResult.First().Key));

                    var task = NoticeMailWorker.SendAsync(bodyString.ToString());
                }

                throw;
            }
        }

        #endregion

    }
}












