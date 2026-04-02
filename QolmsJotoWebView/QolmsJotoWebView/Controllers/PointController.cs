using MGF.QOLMS.QolmsJotoWebView.Repositories;
using MGF.QOLMS.QolmsJotoWebView.Workers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// ポイントに関する画面への HTTP 要求に応答する機能を提供します。
    /// この クラス は継承できません。
    /// </summary>
    public class PointController : QjMvcControllerBase
    {

        private string GetFirstModelErrorMessage()
        {
            return this.ModelState.Values
            .SelectMany(i => i.Errors)
            .Select(i => string.IsNullOrWhiteSpace(i.ErrorMessage) ? i.Exception?.Message : i.ErrorMessage)
            .FirstOrDefault(i => !string.IsNullOrWhiteSpace(i))
            ?? "入力内容が不正です。";
        }

        private string RenderPartialViewToString(string viewName, object model)
        {
            this.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, viewName);
                var viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);

                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(this.ControllerContext, viewResult.View);

                return sw.GetStringBuilder().ToString();
            }
        }

        #region 画面デモ用

        private QolmsJotoModel CreateDemoMainModel()
        {
            var author = new AuthorAccountItem
            {
                AccountKey = Guid.NewGuid(),
                UserId = "demo",
                LoginAt = DateTime.Now,
                FamilyName = "FamilyName",
                GivenName = "GivenName"
            };

            return new QolmsJotoModel(
                authorAccount: author,
                sessionId: Guid.NewGuid().ToString("N"),
                apiAuthorizeKey: Guid.NewGuid(),
                apiAuthorizeExpires: DateTime.Now.AddHours(1),
                apiAuthorizeKey2: Guid.NewGuid(),
                apiAuthorizeExpires2: DateTime.Now.AddHours(1)
            );
        }

        #endregion

        #region "ページ ビュー アクション"

        #region 地域ポイント画面

        [HttpGet]
        [QjAuthorize()]
        [QjLogging]
        public ActionResult LocalHistory()
        {
            var mainModel = this.GetQolmsJotoModel();

            // 初期表示は今日
            var targetDate = DateTime.Today.Date;

            var worker = new LocalPointWorker(new LocalPointRepository(), new LinkageRepository());
            var viewModel = worker.CreateViewModel(mainModel, targetDate);

            mainModel.SetInputModelCache(viewModel);

            return View(viewModel);
            //return View();
        }


        /// <summary>
        /// 「ローカルポイント」画面 年月絞り込み結果の表示要求を処理します。
        /// </summary>
        /// <param name="model">入力モデル。</param>
        /// <returns>成功ならパーシャルビュー、失敗ならJSON形式のコンテンツ。</returns>
        [HttpPost]
        [QjAjaxOnly]
        [QjActionMethodSelector("Send")]
        [QjAuthorize(true)]
        [ValidateAntiForgeryToken(Order = int.MaxValue)]
        [QjLogging]
        public ActionResult LocalHistoryResult(PointLocalHistoryInputModel model)
        {
            var mainModel = this.GetQolmsJotoModel();
            // 初期表示時に作成した最新の入力モデルをキャッシュから取得
            var cacheModel = mainModel.GetInputModelCache<PointLocalHistoryInputModel>();

            // 入力検証エラーはJSONで返却し、クライアント側で表示する
            if (!this.ModelState.IsValid)
            {
                return new MessageJsonResult()
                {
                    IsSuccess = bool.FalseString,
                    Message = this.GetFirstModelErrorMessage()
                }.ToJsonResult();
            }

            var worker = new LocalPointWorker(new LocalPointRepository(), new LinkageRepository());
            // キャッシュ未作成時は当月モデルを作成して処理継続
            var currentModel = cacheModel ?? worker.CreateViewModel(mainModel, DateTime.Now.Date);
            bool reSearch = currentModel.Year != model.Year || currentModel.Month != model.Month;

            if (reSearch)
            {
                // 年月が変わった場合のみAPIを再実行して履歴を再取得
                currentModel = worker.CreateViewModel(mainModel, new DateTime(model.Year, model.Month, 1));
            }
            else
            {
                // 同一年月なら入力値のみ反映
                currentModel.UpdateByInput(model);
            }

            // 次回の再描画に使えるよう最新状態をキャッシュへ保存
            mainModel.SetInputModelCache(currentModel);

            // 履歴領域のみ差し替えるためパーシャルビューを返却
            return PartialView("_PointLocalHistoryPartialView", new PointLocalHistoryPartialViewModel(currentModel));
        }

        /// <summary>
        /// 「ローカルポイント」画面 ポイント変換要求を処理します。
        /// </summary>
        /// <param name="model">入力モデル。</param>
        /// <returns>JSON形式のコンテンツ。</returns>
        [HttpPost]
        [QjAjaxOnly]
        [QjActionMethodSelector("Redeem")]
        [QjAuthorize(true)]
        [ValidateAntiForgeryToken(Order = int.MaxValue)]
        [QjLogging]
        public ActionResult LocalPointRedeemResult(PointLocalRedeemInputModel model)
        {
            var mainModel = this.GetQolmsJotoModel();
            var worker = new LocalPointWorker(new LocalPointRepository(), new LinkageRepository());
            var cacheModel = mainModel.GetInputModelCache<PointLocalHistoryInputModel>();
            var currentModel = cacheModel ?? worker.CreateViewModel(mainModel, DateTime.Now.Date);

            if (!this.ModelState.IsValid)
            {
                return Json(new
                {
                    IsSuccess = bool.FalseString,
                    Message = this.GetFirstModelErrorMessage()
                });
            }

            if (!int.TryParse(model.RedeemPoint, out var redeemPoint) || redeemPoint <= 0)
            {
                return Json(new
                {
                    IsSuccess = bool.FalseString,
                    Message = "変換ポイントは1以上の数字で入力してください。"
                });
            }

            if (redeemPoint > currentModel.Point)
            {
                return Json(new
                {
                    IsSuccess = bool.FalseString,
                    Message = "変換ポイントは現在の累計ポイント以下で入力してください。"
                });
            }

            try
            {
                currentModel = worker.RedeemLocalPoint(mainModel, currentModel, redeemPoint);
                mainModel.SetInputModelCache(currentModel);

                var historyHtml = this.RenderPartialViewToString(
                    "_PointLocalHistoryPartialView",
                    new PointLocalHistoryPartialViewModel(currentModel)
                );

                return Json(new
                {
                    IsSuccess = bool.TrueString,
                    Message = "ポイント変換が完了しました。",
                    Point = currentModel.Point,
                    HistoryHtml = historyHtml
                });
            }
            
            catch (Exception ex)
            {
                AccessLogWorker.WriteAccessLog(
                    this.HttpContext,
                    string.Empty,
                    AccessLogWorker.AccessTypeEnum.Error,
                    string.Format(ex.Message)
                );

                return Json(new
                {
                    IsSuccess = bool.FalseString,
                    Message = "ポイント変換に失敗しました。"
                });
            }
        }

        /// <summary>
        /// 「ポイント履歴」画面
        /// ポイント履歴パーシャルビューの表示要求を処理します。
        /// </summary>
        /// <returns>
        /// アクションの結果。
        /// </returns>
        [ChildActionOnly]
        public ActionResult PointLocalHistoryPartialView()
        {
            PointLocalHistoryPartialViewModel model = null;

            try
            {
                var pageViewModel = this.GetPageViewModel<PointLocalHistoryInputModel>();
                model = new PointLocalHistoryPartialViewModel(pageViewModel);
            }
            catch
            {
            }

            // パーシャルビューを返却
            if (model != null)
            {
                return PartialView("_PointLocalHistoryPartialView", model);
            }

            return new EmptyResult();
        }

        #endregion

        #region JOTOポイント履歴 画面

        [HttpGet()]
        [QjAuthorize()]
        [QjApiAuthorize()]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        [QjLogging()]
        public ActionResult History(int? year, int? month)
        {
            var mainmodel = this.GetQolmsJotoModel();

            var worker = new PointHistoryWorker(new PointRepository());
            return View(worker.CreateViewModel(mainmodel, year, month, QjPageNoTypeEnum.None));
        }

        #endregion

        #region Terms

        [HttpGet()]
        public ActionResult Terms()
        {
            return View();
        }
        #endregion

        #region 沖縄マルシェクーポン交換 画面
        [HttpGet()]
        [QjAuthorize()]
        [QyApiAuthorize()]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        [QjLogging()]
        public ActionResult Exchange()
        {
            var mainmodel = this.GetQolmsJotoModel();
            var worker = new PortalPointExchangeWorker(new OcmCouponRepository(),new PointRepository());

            return View(worker.CreateViewModel(mainmodel));
        }

        [HttpPost]
        [QjAjaxOnly]
        [QjAuthorize(true)]
        [QyApiAuthorize()]
        [ValidateAntiForgeryToken(Order = int.MaxValue)]
        [QjLogging]
        public ActionResult ExchangeResult(byte couponType)
        {
            var worker = new PortalPointExchangeWorker(new OcmCouponRepository(),new PointRepository());
            try
            {
                // データチャージ呼び出す
                if (worker.Exchange(this.GetQolmsJotoModel(), couponType))
                {
                    this.TempData["IsFinish"] = true;

                    return new MessageJsonResult()
                    {
                        Message = HttpUtility.HtmlEncode("ポイント交換が完了しました。"),
                        IsSuccess = bool.TrueString
                    }.ToJsonResult();
                }
                else
                {
                    return new MessageJsonResult()
                    {
                        Message = HttpUtility.HtmlEncode("ポイント交換が失敗しました。"),
                        IsSuccess = bool.FalseString
                    }.ToJsonResult();
                }
            }
            catch (Exception ex)
            {
                AccessLogWorker.WriteAccessLog(
                    this.HttpContext,
                    string.Empty,
                    AccessLogWorker.AccessTypeEnum.Error,
                    string.Format(ex.Message)
                );

                return new MessageJsonResult()
                {
                    Message = HttpUtility.HtmlEncode("ポイント交換が失敗しました。"),
                    IsSuccess = bool.FalseString
                }.ToJsonResult();
            }
        }

        #endregion

        #region Amazonポイント交換 画面

        [HttpGet()]
        [QjAuthorize()]
        [QyApiAuthorize()]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        [QjLogging()]
        public ActionResult AmazonGiftCard()
        {
            var worker = new PointAmazonGiftCardWorker( new PointRepository(),new AmazonGiftCardRepository());
            return View(worker.CreateViewModel(this.GetQolmsJotoModel()));
        }

        [HttpPost]
        [QjAjaxOnly]
        [QjAuthorize(true)]
        [ValidateAntiForgeryToken(Order = int.MaxValue)]
        [QyApiAuthorize]
        [QjLogging]
        public ActionResult AmazonGiftCardResult(byte itemid)
        {
            var worker = new PointAmazonGiftCardWorker( new PointRepository(),new AmazonGiftCardRepository());
            try
            {
                // データチャージ呼び出す
                if (worker.Exchange(this.GetQolmsJotoModel(), itemid))
                {
                    this.TempData["IsFinish"] = true;

                    return new MessageJsonResult()
                    {
                        Message = HttpUtility.HtmlEncode("Amazonギフト券の交換が完了しました。"),
                        IsSuccess = bool.TrueString
                    }.ToJsonResult();
                }
                else
                {
                    return new MessageJsonResult()
                    {
                        Message = HttpUtility.HtmlEncode("Amazonギフト券の交換が失敗しました。"),
                        IsSuccess = bool.FalseString
                    }.ToJsonResult();
                }
            }
            catch (Exception ex)
            {
                AccessLogWorker.WriteAccessLog(this.HttpContext, string.Empty, AccessLogWorker.AccessTypeEnum.Error, string.Format(ex.Message));

                return new MessageJsonResult()
                {
                    Message = HttpUtility.HtmlEncode("Amazonギフト券の交換が失敗しました。"),
                    IsSuccess = bool.FalseString
                }.ToJsonResult();
            }
        }

        #endregion

        #region "「Auポイントポイント交換」画面"
        [HttpGet()]
        [QjAuthorize()]
        [QjApiAuthorize()]
        [QjLogging()]
        public ActionResult AuPoint(byte? fromPageNo)
        {
            //QjPageNoTypeEnum fromPageNoType = QjPageNoTypeEnum.None;
            //if (fromPageNo.HasValue)
            //{
            //    fromPageNoType = fromPageNo.ToString().TryToValueType(QjPageNoTypeEnum.None);
            //}
            var worker = new PointPontaExchangeWorker(new PointRepository(), new PontaExchangeRepository());

            return View(worker.CreateViewModel(this.GetQolmsJotoModel()));
        }

        [HttpPost()]
        [QjAjaxOnly()]
        [QjAuthorize(true)]
        [ValidateAntiForgeryToken(Order = int.MaxValue)]
        [QjApiAuthorize()]
        [QjLogging()]
        public ActionResult AuPointResult(int itemid)
        {
            try
            {
                var worker = new PointPontaExchangeWorker(new PointRepository(), new PontaExchangeRepository());

                // データチャージ呼び出す
                if (worker.Exchange(this.GetQolmsJotoModel(), itemid))
                {
                    this.TempData["IsFinish"] = true;

                    return new MessageJsonResult()
                    {
                        Message = HttpUtility.HtmlEncode("Pontaポイント交換が成功しました。"),
                        IsSuccess = bool.TrueString
                    }.ToJsonResult();
                }
                else
                {
                    return new MessageJsonResult()
                    {
                        Message = HttpUtility.HtmlEncode("Pontaポイント交換が失敗しました。"),
                        IsSuccess = bool.FalseString
                    }.ToJsonResult();
                }
            }
            catch (Exception ex)
            {
                AccessLogWorker.WriteAccessLog(this.HttpContext, string.Empty, AccessLogWorker.AccessTypeEnum.Error, string.Format(ex.Message));
                return new MessageJsonResult()
                {
                    Message = HttpUtility.HtmlEncode("Pontaポイント交換が失敗しました。"),
                    IsSuccess = bool.FalseString
                }.ToJsonResult();
            }
        }


        #endregion

        #region オンラインストアポイント交換 画面

        [HttpGet]
        [QjAuthorize]
        [QjApiAuthorize]
        [QjLogging]
        public ActionResult OnlineStore(byte? fromPageNo)
        {
            //QyPageNoTypeEnum fromPageNoType = QyPageNoTypeEnum.None;
            //if (fromPageNo.HasValue)
            //{
            //    fromPageNoType = fromPageNo.ToString().TryToValueType(QyPageNoTypeEnum.None);
            //}
            var worker = new PointOnlineStoreWorker(new PointRepository(), new OnlineStoreCouponRepository());

            return View(worker.CreateViewModel(this.GetQolmsJotoModel()));
        }

        [HttpPost]
        [QjAjaxOnly]
        [QjAuthorize(true)]
        [ValidateAntiForgeryToken(Order = int.MaxValue)]
        [QyApiAuthorize]
        [QjLogging]
        public ActionResult OnlineStoreResult(byte couponType)
        {
            var worker = new PointOnlineStoreWorker(new PointRepository(), new OnlineStoreCouponRepository());
            try
            {
                if (worker.Exchange(this.GetQolmsJotoModel(), couponType))
                {
                    this.TempData["IsFinish"] = true;

                    return new MessageJsonResult()
                    {
                        Message = HttpUtility.HtmlEncode("ポイント交換が完了しました。"),
                        IsSuccess = bool.TrueString
                    }.ToJsonResult();
                }
                else
                {
                    return new MessageJsonResult()
                    {
                        Message = HttpUtility.HtmlEncode("ポイント交換が失敗しました。"),
                        IsSuccess = bool.FalseString
                    }.ToJsonResult();
                }
            }
            catch (Exception ex)
            {
                AccessLogWorker.WriteAccessLog(this.HttpContext, string.Empty, AccessLogWorker.AccessTypeEnum.Error, string.Format(ex.Message));

                return new MessageJsonResult()
                {
                    Message = HttpUtility.HtmlEncode("ポイント交換が失敗しました。"),
                    IsSuccess = bool.FalseString
                }.ToJsonResult();
            }
        }

        #endregion

        #region "データチャージ画面"

        [HttpGet()]
        //[QjAuthorize()]
        //[QyApiAuthorize()]
        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        //[QjLogging()]
        public ActionResult Datacharge()
        {
            return View();
        }

        #endregion

        #region 共通パーツ

        /// <summary>
        /// 「ヘッダー」パーシャル ビューの表示要求を処理します。
        /// </summary>
        /// <returns>
        /// アクションの結果。
        /// </returns>
        [ChildActionOnly]
        public ActionResult PointHeaderPartialView()
        {
            // パーシャル ビューを返却
            return PartialView("_PointHeaderPartialView");
        }

        /// <summary>
        /// 「ヘッダー」パーシャル ビューの表示要求を処理します。
        /// </summary>
        /// <returns>
        /// アクションの結果。
        /// </returns>
        [ChildActionOnly]
        [OutputCache(Duration = 600)]
        public ActionResult PointFooterPartialView()
        {
            // パーシャル ビューを返却
            return PartialView("_PointFooterPartialView");
        }

        #endregion

        #endregion

    }
}