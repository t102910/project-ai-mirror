using MGF.QOLMS.QolmsJotoWebView.Repositories;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace MGF.QOLMS.QolmsJotoWebView {
    public class PortalController : QjMvcControllerBase
    {

        #region Private Property

        private bool IsJoto => this.Request.UserAgent.ToLower().Contains("joto");

        #endregion

        #region Callenge

        // ぎのわんスマート健康増進プロジェクト​
        [HttpGet()]
        [QjAuthorize]
        [QjApiAuthorize]
        [QjLogging]
        public ActionResult LocalIdVerification()
        {
            var worker = new PortalLocalIdVerificationWorker(new LinkageRepository());

            if (worker.IsEntered(this.GetQolmsJotoModel()))
            {
                return RedirectToAction("LocalIdVerificationDetail");
            }
            return View();
        }

        [HttpGet()]
        [QjAuthorize]
        [QjApiAuthorize]
        [QjLogging]
        public ActionResult LocalIdVerificationAgreement()
        {
            var worker = new PortalLocalIdVerificationAgreementWorker(new TermsRepository()); 
            return View(worker.CreateViewModel(this.GetQolmsJotoModel()));
        }

        /// <summary>
        /// 市民認証入力画面呼び出し
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        [QjAuthorize]
        [QjApiAuthorize]
        [QjLogging]

        public ActionResult LocalIdVerificationRegister()
        {
            var worker = new PortalLocalIdVerificationRegisterWorker(new LocalIdVerificationRepository());
            return View(worker.CreateViewModel(this.GetQolmsJotoModel()));
        }

        /// <summary>
        /// 市民認証入力
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [QjAuthorize]
        [QjApiAuthorize]
        [QjLogging]
        public ActionResult LocalIdVerificationRegisterResult(PortalLocalIdVerificationRegisterInputModel inputModel)
        {
            var errorMessage = new Dictionary<string, string>();
            string message = string.Empty;
            var worker = new PortalLocalIdVerificationRegisterWorker(new LocalIdVerificationRepository());

            // 入力項目の更新
            // IDを発行して連携を登録
            if (this.ModelState.IsValid)
            {
                if (worker.Register(GetQolmsJotoModel(), inputModel, ref message))
                {
                    return new MessageListJsonResult()
                    {
                        IsSuccess = bool.TrueString
                    }.ToJsonResult();
                }
                else
                {
                    errorMessage.Add("summary", string.IsNullOrWhiteSpace(message) ? "登録に失敗しました。" : message);
                }
            }

            // 検証失敗
            foreach (string key in this.ModelState.Keys)
            {
                foreach (ModelError e in this.ModelState[key].Errors)
                {
                    if (errorMessage.ContainsKey(key))
                    {
                        errorMessage[key] = errorMessage[key] + HttpUtility.HtmlEncode(e.ErrorMessage);
                    }
                    else
                    {
                        errorMessage.Add(key, HttpUtility.HtmlEncode(e.ErrorMessage));
                    }
                }
            }

            return new MessageListJsonResult()
            {
                IsSuccess = bool.FalseString,
                Messages = errorMessage
            }.ToJsonResult();
        }

        /// <summary>
        /// 市民認証入力画面呼び出し
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [QjAuthorize]
        [QjApiAuthorize]
        [QjLogging]
        public ActionResult LocalIdVerificationRegisterCancelResult()
        {
            var errorMessage = new Dictionary<string, string>();
            string message = string.Empty;

            var worker = new PortalLocalIdVerificationRegisterWorker(new LocalIdVerificationRepository());

            // 入力項目の更新
            // IDを発行して連携を登録
            if (this.ModelState.IsValid)
            {
                if (worker.Cancel(this.GetQolmsJotoModel(), ref message))
                {
                    return new MessageListJsonResult()
                    {
                        IsSuccess = bool.TrueString
                    }.ToJsonResult();
                }
                else
                {
                    errorMessage.Add("summary", string.IsNullOrWhiteSpace(message) ? "登録に失敗しました。" : message);
                }
            }

            // 検証失敗
            foreach (string key in this.ModelState.Keys)
            {
                foreach (ModelError e in this.ModelState[key].Errors)
                {
                    if (errorMessage.ContainsKey(key))
                    {
                        errorMessage[key] = errorMessage[key] + HttpUtility.HtmlEncode(e.ErrorMessage);
                    }
                    else
                    {
                        errorMessage.Add(key, HttpUtility.HtmlEncode(e.ErrorMessage));
                    }
                }
            }

            return new MessageListJsonResult()
            {
                IsSuccess = bool.FalseString,
                Messages = errorMessage
            }.ToJsonResult();
        }

        /// <summary>
        /// 市民認証申請画面呼び出し
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [QjAuthorize]
        [QjApiAuthorize]
        [QjLogging]
        public ActionResult LocalIdVerificationRequest()
        {
            var worker = new PortalLocalIdVerificationRequestWorker(new LinkageRepository());
            var viewModel = worker.CreateViewModel(this.GetQolmsJotoModel());

            if (viewModel.LinkageSystemNo > 0)
            {
                return View(viewModel);
            }
            else
            {
                return RedirectToAction("LocalIdVerification");
            }
        }

        /// <summary>
        /// 市民認証確認画面呼び出し
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [QjAuthorize]
        [QjApiAuthorize]
        [QjLogging]
        public ActionResult LocalIdVerificationDetail()
        {
            var worker = new PortalLocalIdVerificationDetailWorker(new LinkageRepository());
            var viewModel = worker.CreateViewModel(this.GetQolmsJotoModel());

            if (viewModel.LinkageSystemNo > 0)
            {
                return View(viewModel);
            }
            else
            {
                return RedirectToAction("LocalIdVerification");
            }
        }


        [HttpGet()]
        [QjLogging]
        public ActionResult ConnectionSetting()
        {
            return View();
        }

        #endregion

        #region パーシャル ビュー アクション

        #region 共通パーツ

        /// <summary>
        /// 「ヘッダー」パーシャル ビューの表示要求を処理します。
        /// </summary>
        /// <returns>
        /// アクションの結果。
        /// </returns>
        /// <remarks></remarks>
        [ChildActionOnly]
        public ActionResult PortalHeaderPartialView()
        {
            // パーシャル ビューを返却
            if (this.IsJoto)
            {
                return new EmptyResult();
            }
            else
            {
                return PartialView("_PortalHeaderPartialView");
            }
        }

        /// <summary>
        /// 「フッター」パーシャル ビューの表示要求を処理します。
        /// </summary>
        /// <returns>
        /// アクションの結果。
        /// </returns>
        /// <remarks></remarks>
        [ChildActionOnly]
        [OutputCache(Duration = 600)]
        public ActionResult PortalFooterPartialView()
        {
            // パーシャル ビューを返却
            return PartialView("_PortalFooterPartialView");
        }

        #endregion

        #endregion
    }
}