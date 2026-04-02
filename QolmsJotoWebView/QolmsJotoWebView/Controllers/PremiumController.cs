﻿using System;
using System.Linq;
using System.Web.Mvc;

namespace MGF.QOLMS.QolmsJotoWebView
{
    public class PremiumController : QjMvcControllerBase
    {
        #region Private Method

        private int NormalizePaymentType(int? paymentType)
        {
            return paymentType == 2 ? 2 : 1;
        }

        #endregion

        #region 一覧ページ

        [HttpGet]
        public ContentResult MockPages()
        {
            string authority = Request?.Url?.GetLeftPart(UriPartial.Authority) ?? "https://localhost:44384";

            var mockPages = new[]
            {
                new { Label = "プレミアム会員紹介", Url = authority + Url.Action(nameof(LightMember), "Premium", null) },
                new { Label = "プレミアム会員", Url = authority + Url.Action(nameof(PremiumMember), "Premium", null) },
                new { Label = "プレミアム会員登録（お支払い方法 / au PAY）", Url = authority + Url.Action(nameof(PaymentMethod), "Premium", new { paymentType = 1 }) },
                new { Label = "プレミアム会員参加同意書", Url = authority + Url.Action(nameof(Agreement), "Premium", null) },
                new { Label = "プレミアム会員登録（カード情報）", Url = authority + Url.Action(nameof(CardInfo), "Premium", null) },
                new { Label = "プレミアム会員登録（カード情報 / 登録完了表示）", Url = authority + Url.Action(nameof(CardInfo), "Premium", new { registered = true }) }
            };

            string html = "<!DOCTYPE html><html><head><meta charset=\"utf-8\" /><title>Premium Mock Pages</title></head><body>"
                + "<h1>プレミアム画面モック確認用URL</h1><ul>"
                + string.Join(string.Empty, mockPages.Select(x => string.Format("<li>{0}<br /><a href=\"{1}\">{1}</a></li>", x.Label, x.Url)))
                + "</ul></body></html>";

            return Content(html, "text/html");
        }

        #endregion

        #region 「プレミアム会員紹介」画面

        [HttpGet]
        public ActionResult LightMember()
        {
            return View();
        }

        #endregion

        #region 「プレミアム会員」画面

        [HttpGet]
        public ActionResult PremiumMember()
        {
            return View();
        }

        #endregion

        #region 「プレミアム会員登録（お支払い方法）」画面

        [HttpGet]
        public ActionResult Regist(byte? fromPageNo, int? paymentType)
        {
            return RedirectToAction("PaymentMethod", new
            {
                fromPageNo = fromPageNo,
                paymentType = this.NormalizePaymentType(paymentType)
            });
        }

        [HttpGet]
        public ActionResult MethodChange(byte? fromPageNo, int? paymentType)
        {
            return RedirectToAction("PaymentMethod", new
            {
                fromPageNo = fromPageNo,
                paymentType = this.NormalizePaymentType(paymentType)
            });
        }

        [HttpGet]
        public ActionResult PaymentMethod(byte? fromPageNo, int? paymentType)
        {
            ViewBag.FromPageNo = fromPageNo;
            ViewBag.PaymentType = this.NormalizePaymentType(paymentType);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken(Order = int.MaxValue)]
        public ActionResult RegistResult(byte? fromPageNo, int? paymentType)
        {
            int normalizedPaymentType = this.NormalizePaymentType(paymentType);

            if (normalizedPaymentType == 2)
            {
                return RedirectToAction("Agreement", new
                {
                    fromPageNo = fromPageNo
                });
            }

            return RedirectToAction("PremiumMember", new
            {
                fromPageNo = fromPageNo
            });
        }

        #endregion

        #region 「プレミアム会員参加同意書」画面

        [HttpGet]
        public ActionResult Agreement(byte? fromPageNo)
        {
            ViewBag.FromPageNo = fromPageNo;
            return View();
        }

        #endregion

        #region 「プレミアム会員登録（カード情報）」画面

        [HttpGet]
        public ActionResult CardInfo(byte? fromPageNo, bool? registered)
        {
            ViewBag.FromPageNo = fromPageNo;
            ViewBag.IsRegistered = registered ?? false;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken(Order = int.MaxValue)]
        public ActionResult PayjpCardRegister(byte? fromPageNo)
        {
            return RedirectToAction("CardInfo", new
            {
                fromPageNo = fromPageNo,
                registered = true
            });
        }

        #endregion
    }
}
