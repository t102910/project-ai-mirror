using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsJotoWebView.Repositories;
using MGF.QOLMS.QolmsJotoWebView.Workers;

namespace MGF.QOLMS.QolmsJotoWebView
{
    public class HealthController : QjMvcControllerBase
    {

        #region 健康年齢

        [HttpGet]
        [QjAuthorize]
        [QyApiAuthorize]
        [QjLogging]
        public ActionResult Age(QjPageNoTypeEnum fromPageNoType = QjPageNoTypeEnum.NoteExamination)
        {
            var worker = new HealthAgeWorker(new HealthAgeRepository());

            var mainModel = this.GetQolmsJotoModel();
            var viewModel = worker.CreateViewModel(mainModel, fromPageNoType);

            return View(viewModel);
        }

        [HttpPost]
        [QjAjaxOnly]
        [QjAuthorize(true)]
        [QjActionMethodSelector("Report")]
        [ValidateAntiForgeryToken(Order = int.MaxValue)]
        [QyApiAuthorize]
        [QjLogging]
        public ActionResult AgeResult(QjHealthAgeReportTypeEnum healthAgeReportType)
        {
            var worker = new HealthAgeWorker(new HealthAgeRepository());

            var mainModel = this.GetQolmsJotoModel();
            var pageViewModel = mainModel.GetInputModelCache<HealthAgeViewModel>();
            var partialViewName = string.Empty;
            var partialViewModel = worker.CreateReportPartialViewModel(mainModel, pageViewModel, healthAgeReportType, ref partialViewName);

            return PartialView(partialViewName, partialViewModel);
        }

        #endregion

        #region 「健康年齢入力」画面

        [HttpGet]
        [QjAuthorize]
        [QyApiAuthorize]
        [QjLogging]
        public ActionResult AgeEdit(byte? fromPageNo)
        {
            var worker = new HealthAgeEditWorker(new HealthAgeRepository(),new JmdcHealthAgeApiRepository());
            QjPageNoTypeEnum fromPageNoType = QjPageNoTypeEnum.None;
            if (fromPageNo.HasValue)
            {
                fromPageNoType = fromPageNo.ToString().TryToValueType(QjPageNoTypeEnum.None);
            }

            QolmsJotoModel mainModel = this.GetQolmsJotoModel();

            // 編集対象のモデルを作成
            HealthAgeEditInputModel inputModel = worker.CreateInputModel(mainModel, fromPageNoType);

            // モデルをキャッシュへ格納（入力検証エラー時の再展開用）
            mainModel.SetInputModelCache(inputModel);

            // ビューを返却
            return View("AgeEdit", inputModel);
        }

        [HttpPost]
        [QjAuthorize]
        [QjActionMethodSelector("Edit")]
        [ValidateAntiForgeryToken(Order = int.MaxValue)]
        [ValidateInput(false)]
        [QyApiAuthorize]
        [QjLogging]
        public ActionResult AgeEditResult(HealthAgeEditInputModel model)
        {
            var worker = new HealthAgeEditWorker(new HealthAgeRepository(),new JmdcHealthAgeApiRepository());
            QolmsJotoModel mainModel = this.GetQolmsJotoModel();
            HealthAgeEditInputModel inputModel = mainModel.GetInputModelCache<HealthAgeEditInputModel>();

            // モデルへ入力値を反映
            inputModel.UpdateByInput(model);

            // モデルの検証状態を確認
            if (this.ModelState.IsValid)
            {
                // 検証成功
                StringBuilder errorMessage = new StringBuilder();

                // 健診受信日の時点での年齢が 18 歳以上かつ 74 歳以下かチェック
                errorMessage.Append(HealthAgeEditWorker.CheckRecordDate(inputModel.AuthorBirthday, inputModel.RecordDate));

                if (errorMessage.Length == 0)
                {
                    // 健康年齢 Web API を実行
                    List<QhApiHealthAgeResponseItem> responseN = worker.ExecuteJmdcHealthAgeApi(mainModel, inputModel);

                    responseN.ForEach(i =>
                    {
                        if (i.StatusCode.TryToValueType(500) != 200)
                        {
                            errorMessage.AppendFormat("{0} {1}{2}", i.StatusCode, i.Message, Environment.NewLine);
                        }
                    });

                    if (errorMessage.Length == 0)
                    {
                        // 登録
                        if (worker.Edit(mainModel, inputModel, responseN))
                        {
                            // モデルをキャッシュからクリア
                            mainModel.RemoveInputModelCache<HealthAgeEditInputModel>();
                        }

                        RouteValueDictionary dict = new RouteValueDictionary();
                        dict.Add("fromPageNo", Convert.ToByte(inputModel.FromPageNoType));

                        // 「健康年齢」画面へ遷移
                        return RedirectToAction("Age", "Health", dict);
                    }
                    else
                    {
                        // 健康年齢 Web API エラー
                        AccessLogWorker.WriteErrorLog(this.HttpContext, string.Empty, errorMessage.ToString());

                        // 独自にエラーメッセージを用意しビューに渡す
                        this.TempData["ErrorMessage"] = new Dictionary<string, string>
                {
                    { "HealthAgeApi", "健康年齢を測定出来ませんでした。健康年齢WEBAPIがメンテナンス中の可能性があります。" }
                };

                        // ビューを返却
                        return View("AgeEdit", inputModel);
                    }
                }
                else
                {
                    // 年齢チェックエラー

                    // 独自にエラーメッセージを用意しビューに渡す
                    this.TempData["ErrorMessage"] = new Dictionary<string, string>
            {
                { "model.RecordDate", errorMessage.ToString() }
            };

                    // ビューを返却
                    return View("AgeEdit", inputModel);
                }
            }
            else
            {
                // 検証失敗

                // 独自にエラーメッセージを用意しビューに渡す
                Dictionary<string, string> errorMessageDict = new Dictionary<string, string>();

                foreach (string key in this.ModelState.Keys)
                {
                    foreach (ModelError e in this.ModelState[key].Errors)
                    {
                        errorMessageDict.Add(key, e.ErrorMessage);
                    }
                }

                this.TempData["ErrorMessage"] = errorMessageDict;

                // ビューを返却
                return View("AgeEdit", inputModel);
            }

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
        public ActionResult HealthHeaderPartialView()
        {
            // パーシャル ビューを返却
            return PartialView("_HealthHeaderPartialView");
        }

        /// <summary>
        /// 「フッター」パーシャル ビューの表示要求を処理します。
        /// </summary>
        /// <returns>
        /// アクションの結果。
        /// </returns>
        [ChildActionOnly]
        [OutputCache(Duration = 600)]
        public ActionResult HealthFooterPartialView()
        {
            // パーシャル ビューを返却
            return PartialView("_HealthFooterPartialView");
        }

        #endregion

        #region 「健康年齢」画面用 パーシャル ビュー

        /// <summary>
        /// 「健康年齢」画面用
        /// 「健康年齢改善アドバイス」パーシャルビューの表示要求を処理します。
        /// </summary>
        /// <returns>
        /// アクションの結果。
        /// </returns>
        /// <remarks></remarks>
        [ChildActionOnly]
        public ActionResult HealthAgeAdviceAreaPartialView()
        {
            HealthAgeViewModel model = this.GetPageViewModel<HealthAgeViewModel>();

            // パーシャルビューを返却
            return PartialView("_HealthAgeAdviceAreaPartialView", new HealthAgeAdviceAreaPartialViewModel(model));
        }


        [ChildActionOnly]
        public ActionResult HealthAgeTransitionAreaPartialView()
        {
            HealthAgeTransitionAreaPartialViewModel model = null;

            try
            {
                model = new HealthAgeTransitionAreaPartialViewModel(this.GetPageViewModel<HealthAgeViewModel>());
            }
            catch
            {
            }

            if (model != null)
            {
                return PartialView("_HealthAgeTransitionAreaPartialView", model);
            }

            return new EmptyResult();
        }

        #endregion
    }
}
