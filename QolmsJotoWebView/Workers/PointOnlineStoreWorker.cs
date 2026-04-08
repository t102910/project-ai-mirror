using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsJotoWebView.Models;
using MGF.QOLMS.QolmsJotoWebView.Repositories;
using MGF.QOLMS.QolmsJotoWebView.Workers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MGF.QOLMS.QolmsJotoWebView
{
    public sealed class PointOnlineStoreWorker
    {
        IPointRepository _pointRepo;
        IOnlineStoreCouponRepository _onlineStoreCouponRepo;

        #region Constructor

        /// <summary>
        /// デフォルト コンストラクタは使用できません。
        /// </summary>
        public PointOnlineStoreWorker(IPointRepository pointRepository, IOnlineStoreCouponRepository onlineStoreCouponRepository)
        {
            _pointRepo = pointRepository;
            _onlineStoreCouponRepo = onlineStoreCouponRepository;
        }

        #endregion

        #region Constant

        /// <summary>
        /// ポイント交換後にメールを送信するかどうかを設定します。
        /// </summary>
        public  readonly string IsSendMail = ConfigurationManager.AppSettings["IsSendMailPointExchange"];

        /// <summary>
        /// ポイント交換後にメールを送信する残りクーポン枚数を設定します。IsSendMail= Trueのときのみ有効です。
        /// </summary>
        public  readonly string IsSendMailRestCount = ConfigurationManager.AppSettings["IsSendMailPointExchangeRestCount"];

        #endregion

        #region Private Method

        #endregion

        #region Public Method

        public PointOnlineStoreViewModel CreateViewModel(QolmsJotoModel mainModel)
        {
            var result = new PointOnlineStoreViewModel(mainModel);

            string path = HttpContext.Current.Server.MapPath("~/App_Data/CouponForFitbitDescription.txt");
            string str = string.Empty;
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                str = File.ReadAllText(path);
            }
            result.Description = str;

            var apiResult = _onlineStoreCouponRepo.ExecuteCouponForFitbitReadApi(mainModel);
            result.CouponN = apiResult.CouponN.ConvertAll(i => 
            new CouponItem() {
                CouponType = i.CouponType.TryToValueType(byte.MinValue),
                RestCount = i.RestCount.TryToValueType(int.MinValue),
                DispName = i.DispName,
                Point = i.Point.TryToValueType(int.MinValue)
            });
            result.PointExchangeHistN = apiResult.PointExchangeHistN.ConvertAll(i => 
            new PointExchangeHistItem() { 
                CouponId = i.CouponId,
                CouponType = i.CouponType.TryToValueType(byte.MinValue),
                DispName = i.DispName,
                ExpirationDate = i.ExpirationDate.TryToValueType(DateTime.MinValue),
                IssueDate = i.IssueDate.TryToValueType(DateTime.MinValue),
                Point = i.Point.TryToValueType(int.MinValue)
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

        public  bool Exchange(QolmsJotoModel mainModel, byte couponType)
        {
            DateTime actionDate = DateTime.Now;
            var masterResult = _onlineStoreCouponRepo.ExecuteCouponForFitbitMasterReadApi(mainModel, couponType);

            // point減算
            DateTime limit = DateTime.MinValue;

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
                    PointItemNo = (int)QjPointItemTypeEnum.PointExchange,
                    Point = int.Parse(masterResult.Coupon.Point) * -1,
                    PointTargetDate = actionDate,
                    PointExpirationDate = DateTime.MaxValue,
                    Reason = "「Fitbit値引き」クーポンと交換"
                }
                }
            );

            if (pointResult.First().Value == 0)
            {
                // 成功したら交換
                try
                {
                    var result = _onlineStoreCouponRepo.ExecuteCouponForFitbitWriteApi(mainModel, couponType, pointResult.First().Key);

                    if (int.Parse(result.Result) > 0)
                    {
                        // メール
                        if (int.Parse(result.Count) <= 100 && (!string.IsNullOrWhiteSpace(IsSendMail) && bool.Parse(IsSendMail)))
                        {
                            var bodyString = new StringBuilder();
                            bodyString.AppendLine("「Fitbit値引き」クーポン残数100枚以下。");
                            bodyString.AppendLine(string.Format("残り：{0}", result.Count));

                            Task task = NoticeMailWorker.SendAsync(bodyString.ToString());
                        }

                        return true;
                    }
                    else
                    {
                        throw new InvalidOperationException(string.Format("発行できる「Fitbit値引き」クーポンがありません。"));
                    }
                }
                catch (Exception ex)
                {
                    // 交換失敗
                    DateTime pointLimitDate = new DateTime(
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
                        new QolmsPointGrantItem()
                        {
                            ActionDate = actionDate,
                            SerialCode = Guid.NewGuid().ToApiGuidString(),
                            PointItemNo = (int)QjPointItemTypeEnum.RecoveryPoint,
                            Point = int.Parse(masterResult.Coupon.Point),
                            PointTargetDate = actionDate,
                            PointExpirationDate = pointLimitDate,
                            Reason = "「Fitbit値引き」クーポンと交換失敗のためポイント復元"
                        }
                        }
                    );

                    if (removePointResult.First().Value != 0)
                    {
                        var bodyString1 = new StringBuilder();
                        bodyString1.AppendLine("「Fitbit値引き」クーポンポイント修正のエラーです。");
                        bodyString1.AppendLine(string.Format("error:{0}", removePointResult.First().Key));

                        Task<bool> task1 = NoticeMailWorker.SendAsync(bodyString1.ToString());
                    }
                }
            }
            else
            {
                throw new InvalidOperationException(string.Format("ポイントの減算に失敗しました。"));
            }

            return false;
        }

        #endregion
    }

}