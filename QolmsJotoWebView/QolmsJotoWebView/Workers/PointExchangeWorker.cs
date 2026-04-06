using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsJotoWebView.Models;
using MGF.QOLMS.QolmsJotoWebView.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MGF.QOLMS.QolmsJotoWebView.Workers
{
    internal sealed class PortalPointExchangeWorker
    {

        IPointRepository _pointRepo;
        IOcmCouponRepository _CouponRepo;

        #region Constructor

        /// <summary>
        /// デフォルト コンストラクタは使用できません。
        /// </summary>
        public PortalPointExchangeWorker(IOcmCouponRepository CouponRepo,IPointRepository pointRepo)
        {
            _pointRepo = pointRepo;
            _CouponRepo = CouponRepo;
        }

        #endregion

        #region Constant

        // メールを送る
        public static readonly string IsSendMail = ConfigurationManager.AppSettings["IsSendMailPointExchange"];

        #endregion

        #region Public Method

        public PointExchangeViewModel CreateViewModel(
            QolmsJotoModel mainModel)
        {

            var result = new PointExchangeViewModel();
            var apiResult = _CouponRepo.ExecutePointExchangeReadApi(mainModel);

            result.CouponN = apiResult.CouponN.ConvertAll(i => new CouponItem() {
                CouponType = i.CouponType.TryToValueType(byte.MinValue),
                DispName = i.DispName,
                Point = i.Point.TryToValueType(int.MinValue),
                RestCount = i.CouponType.TryToValueType(int.MinValue)
            });

            result.PointExchangeHistN = apiResult.PointExchangeHistN
                .Where(j => j.IssueDate.TryToValueType(DateTime.MinValue) > DateTime.Now.AddYears(-2))
                .ToList()
                .ConvertAll(i => new PointExchangeHistItem() {
                    CouponId = i.CouponId,
                    Point = i.Point.TryToValueType(int.MinValue),
                    DispName = i.DispName,
                    CouponType = i.CouponType.TryToValueType(byte.MinValue),
                    ExpirationDate = i.ExpirationDate.TryToValueType(DateTime.MinValue),
                    IssueDate = i.IssueDate.TryToValueType(DateTime.MinValue),
                });

            //result.FromPageNoType = (int)pageno;

            // 説明文を App_Data から読み込む
            string descPath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/PointExchangeDescription.txt");
            if (!string.IsNullOrWhiteSpace(descPath) && File.Exists(descPath))
            {
                result.Description = File.ReadAllText(descPath);
            }

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

        public bool Exchange(QolmsJotoModel mainModel, byte couponType)
        {
            var actionDate = DateTime.Now;
            var masterResult = _CouponRepo.ExecutePointExchangeMasterReadApi(mainModel, couponType);

            // ポイント減算
            var pointResult = _pointRepo.AddQolmsPoints(
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName,
                mainModel.SessionId,
                mainModel.ApiAuthorizeKey,
                mainModel.AuthorAccount.AccountKey,
                QjConfiguration.PointServiceno,
                new List<QolmsPointGrantItem>
                {
                new QolmsPointGrantItem
                {
                    ActionDate = actionDate,
                    SerialCode = Guid.NewGuid().ToApiGuidString(),
                    PointItemNo = (int)QjPointItemTypeEnum.PointExchange,
                    Point = int.Parse(masterResult.Coupon.Point) * -1,
                    PointTargetDate = actionDate,
                    PointExpirationDate = DateTime.MaxValue,
                    Reason = "「沖縄CLIPマルシェ」クーポンと交換"
                }
                }
            );

            if (pointResult.First().Value == 0)
            {
                // 成功したら交換
                try
                {
                    var result = _CouponRepo.ExecutePointExchangeWriteApi(mainModel, couponType, pointResult.First().Key);

                    if (int.Parse(result.Result) > 0)
                    {
                        // メール
                        if (int.Parse(result.Count) <= 100 &&
                            (!string.IsNullOrWhiteSpace(IsSendMail) && bool.Parse(IsSendMail)))
                        {
                            var bodyString = new StringBuilder();
                            bodyString.AppendLine("ポイント交換クーポン残数100枚以下。");
                            bodyString.AppendLine(string.Format("残り：{0}", result.Count));

                            var task = NoticeMailWorker.SendAsync(bodyString.ToString());
                        }

                        return true;
                    }
                    else
                    {
                        throw new InvalidOperationException("発行できるクーポンがありません。");
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
                        new List<QolmsPointGrantItem>
                        {
                            new QolmsPointGrantItem
                            {
                                ActionDate = actionDate,
                                SerialCode = Guid.NewGuid().ToApiGuidString(),
                                PointItemNo = (int)QjPointItemTypeEnum.RecoveryPoint,
                                Point = int.Parse(masterResult.Coupon.Point),
                                PointTargetDate = actionDate,
                                PointExpirationDate = pointLimitDate,
                                Reason = "「沖縄CLIPマルシェ」クーポンと交換失敗のためポイント復元"
                            }
                        }
                    );

                    if (removePointResult.First().Value != 0)
                    {
                        var bodyString1 = new StringBuilder();
                        bodyString1.AppendLine("クーポンポイント修正のエラーです。");
                        bodyString1.AppendLine(string.Format("error:{0}", removePointResult.First().Key));

                        var task1 = NoticeMailWorker.SendAsync(bodyString1.ToString());
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("ポイントの減算に失敗しました。");
            }

            return false;
        }

        #endregion
    }

}