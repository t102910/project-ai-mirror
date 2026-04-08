using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsJotoWebView.Models;
using MGF.QOLMS.QolmsJotoWebView.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MGF.QOLMS.QolmsJotoWebView.Workers
{
    public class PointAmazonGiftCardWorker
    {
        IPointRepository _pointRepo;
        IAmazonGiftCardRepository _amazonGiftRepo;

        #region "Constructor"

        /// <summary>
        /// デフォルト コンストラクタは使用できません。
        /// </summary>
        public PointAmazonGiftCardWorker(IPointRepository pointRepository, IAmazonGiftCardRepository amazonGiftCardRepository)
        {
            _pointRepo = pointRepository;
            _amazonGiftRepo = amazonGiftCardRepository;
        }

        #endregion

        #region "Public Method"

        public PointAmazonGiftCardViewModel CreateViewModel(QolmsJotoModel mainModel)
        {
            var result = new PointAmazonGiftCardViewModel();
            var apiResult = _amazonGiftRepo.ExecuteAmazonGiftCardReadApi(mainModel);

            result.GiftCardN = apiResult.AmazonGiftCardN.ConvertAll(
                i => new AmazonGiftCardItem()
                {
                    DemandPoint = i.DemandPoint.TryToValueType(int.MinValue),
                    GiftCardName = i.GiftcardName,
                    GiftCardType = i.GiftcardType.TryToValueType(byte.MinValue)
                });
            result.GiftCardHistN = apiResult.AmazonGiftCardHistN.ConvertAll(
                i => new AmazonGiftCardHistItem()
                {
                    DemandPoint = i.DemandPoint.TryToValueType(int.MinValue),
                    ExpirationDate = i.ExpirationDate.TryToValueType(DateTime.MinValue),
                    GiftCardId = i.GiftcardId,
                    GiftCardName = i.GiftcardName,
                    GiftCardType = i.GiftcardType.TryToValueType(byte.MinValue),
                    IssueDate = i.IssueDate.TryToValueType(DateTime.MinValue)
                });

            // ポイント数の表示
            try
            {
                result.Point = _pointRepo.GetQolmsPoint(
                    mainModel.ApiExecutor,
                    mainModel.ApiExecutorName,
                    mainModel.SessionId,
                    mainModel.ApiAuthorizeKey,
                    mainModel.AuthorAccount.AccountKey
                );
            }
            catch (Exception ex)
            {
                result.Point = 0;
            }

            return result;
        }

        public  bool Exchange(QolmsJotoModel mainModel, byte itemId)
        {
            var actionDate = DateTime.Now;
            var masterResult = _amazonGiftRepo.ExecuteAmazonGiftCardMasterReadApi(mainModel, itemId);

            // point減算
            var pointResult = _pointRepo.AddQolmsPoints(
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName,
                mainModel.SessionId,
                mainModel.ApiAuthorizeKey,
                mainModel.AuthorAccount.AccountKey,
                QjConfiguration.PointServiceno,
                new List<QolmsPointGrantItem>()
                {
                new QolmsPointGrantItem()
                {
                    ActionDate = actionDate,
                    SerialCode = Guid.NewGuid().ToApiGuidString(),
                    PointItemNo = (int)QjPointItemTypeEnum.AmazonPoint,
                    Point = int.Parse(masterResult.GiftCard.DemandPoint) * -1,
                    PointTargetDate = actionDate,
                    PointExpirationDate = DateTime.MaxValue,
                    Reason = "Amazonギフト券と交換"
                }
                }
            );

            if (pointResult.First().Value == 0)
            {
                // 成功したら交換
                try
                {
                    var result = _amazonGiftRepo.ExecuteAmazonGiftCardWriteApi(mainModel, itemId, pointResult.First().Key);

                    if (int.Parse(result.Result) > 0)
                    {
                        // メール todo:枚数確認
                        if (int.Parse(result.Count) <= 500)
                        {
                            var bodyString = new StringBuilder();
                            bodyString.AppendLine("ポイント交換Amazonギフト券残数500枚以下。");
                            bodyString.AppendLine(
                                string.Format("{0} 残り：{1}", masterResult.GiftCard.GiftcardName, result.Count)
                            );

                            var task = NoticeMailWorker.SendAsync(bodyString.ToString());
                        }

                        return true;
                    }
                    else
                    {
                        var bodyString = new StringBuilder();
                        bodyString.AppendLine("ポイント交換Amazonギフト券の発行できるコードがありません。");
                        bodyString.AppendLine(masterResult.GiftCard.GiftcardName);

                        var task = NoticeMailWorker.SendAsync(bodyString.ToString());

                        throw new InvalidOperationException(
                            "ポイント交換Amazonギフト券の発行できるコードがありません。"
                        );
                    }
                }
                catch (Exception ex)
                {
                    // 交換失敗
                    var pointLimitDate = new DateTime(
                        actionDate.Year,
                        actionDate.Month,
                        1
                    ).AddMonths(7).AddDays(-1); // ポイント有効期限は 6 ヶ月後の月末（起点は操作日時）

                    var removePointResult = _pointRepo.AddQolmsPoints(
                        mainModel.ApiExecutor,
                        mainModel.ApiExecutorName,
                        mainModel.SessionId,
                        mainModel.ApiAuthorizeKey,
                        mainModel.AuthorAccount.AccountKey,
                        QjConfiguration.PointServiceno,
                        new List<QolmsPointGrantItem>()
                        {
                        new QolmsPointGrantItem()
                        {
                            ActionDate = actionDate,
                            SerialCode = Guid.NewGuid().ToApiGuidString(),
                            PointItemNo = (int)QjPointItemTypeEnum.RecoveryPoint,
                            Point = int.Parse(masterResult.GiftCard.DemandPoint),
                            PointTargetDate = actionDate,
                            PointExpirationDate = pointLimitDate,
                            Reason = "Amazonギフト券と交換失敗のためポイント復元"
                        }
                        }
                    );

                    if (removePointResult.First().Value != 0)
                    {
                        var bodyString1 = new StringBuilder();
                        bodyString1.AppendLine("Amazonギフト券ポイント修正のエラーです。");
                        bodyString1.AppendLine(string.Format("error:{0}", removePointResult.First().Key));

                        var task1 = NoticeMailWorker.SendAsync(bodyString1.ToString());
                    }
                }
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format(
                        "ポイントの減算に失敗しました。({0}):{1}",
                        pointResult.First().Value,
                        pointResult.First().Key
                    )
                );
            }

            return false;
        }

        #endregion
    }
}