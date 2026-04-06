using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// Start 系画面への HTTP 要求に応答する機能を提供します。
    /// この クラス は継承できません。
    /// </summary>
    public sealed class StartController : QjMvcControllerBase
    {
        #region "Constructor"

        /// <summary>
        /// <see cref="StartController" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        public StartController() : base() { }

        #endregion

        #region 画面デモ用

        [HttpGet]
        public ActionResult Demo(string name)
        {
            var mainModel = this.GetQolmsJotoModel();

            var viewModel = DemoWorker.CreateViewModel(mainModel, name);

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult LoginByAuId()
        {
            var model = new LoginModel();

            return View(model);
        }
        #endregion

        #region "ページ ビュー アクション"

        #region "SSO による ログイン"
        /// <summary>
        /// SSO による画面遷移を処理します。（健診結果）
        /// </summary>
        /// <returns>
        /// アクション の結果。
        /// </returns>
        private ActionResult RedirectToNoteExamination()
        {
            var routeValuesDictionary = new RouteValueDictionary();
            this.Request.QueryString.AllKeys.ToList().ForEach(key => routeValuesDictionary.Add(key, Request.QueryString[key]));
            //Logger.WriteLog("RedirectToHealthCheckup");
            // URL パラメータ を引き継いて「健診結果」画面へ遷移
            return RedirectToAction("Examination", "Note", routeValuesDictionary);
        }

        /// <summary>
        /// SSO による画面遷移を処理します。（JOTOポイント）
        /// </summary>
        /// <returns>
        /// アクション の結果。
        /// </returns>
        private ActionResult RedirectToPointHistory()
        {
            var routeValuesDictionary = new RouteValueDictionary();
            this.Request.QueryString.AllKeys.ToList().ForEach(key => routeValuesDictionary.Add(key, Request.QueryString[key]));
            //Logger.WriteLog("RedirectToHealthCheckup");
            // URL パラメータ を引き継いて「JOTOポイント」画面へ遷移
            return RedirectToAction("History", "Point", routeValuesDictionary);
        }

        /// <summary>
        /// SSO による画面遷移を処理します。（地域ポイント）
        /// </summary>
        /// <returns>
        /// アクション の結果。
        /// </returns>
        private ActionResult RedirectToLocalHistory()
        {
            var routeValuesDictionary = new RouteValueDictionary();
            this.Request.QueryString.AllKeys.ToList().ForEach(key => routeValuesDictionary.Add(key, Request.QueryString[key]));
            //Logger.WriteLog("RedirectToHealthCheckup");
            // URL パラメータ を引き継いて「地域ポイント」画面へ遷移
            return RedirectToAction("LocalHistory", "Point", routeValuesDictionary);
        }

        /// <summary>
        /// SSO による画面遷移を処理します。（デモ画面）
        /// </summary>
        /// <returns>
        /// アクション の結果。
        /// </returns>
        private ActionResult RedirectToDemo(string name)
        {
            var routeValuesDictionary = new RouteValueDictionary();
            this.Request.QueryString.AllKeys.ToList().ForEach(key => routeValuesDictionary.Add(key, Request.QueryString[key]));

            routeValuesDictionary.Add("name", name);

            return RedirectToAction("Demo", "Start", routeValuesDictionary);
        }

        /// <summary>
        /// SSO による画面遷移を処理します。（カロミル食事画面）
        /// </summary>
        /// <returns>
        /// アクション の結果。
        /// </returns>
        private ActionResult RedirectToNoteCalomeal()
        {
            var routeValuesDictionary = new RouteValueDictionary();
            this.Request.QueryString.AllKeys.ToList().ForEach(key => routeValuesDictionary.Add(key, Request.QueryString[key]));

            return RedirectToAction("Calomeal", "Note", routeValuesDictionary);
        }

        /// <summary>
        /// SSO による画面遷移を処理します。（健康年齢画面）
        /// </summary>
        /// <returns>
        /// アクション の結果。
        /// </returns>
        private ActionResult RedirectToHealthAge()
        {
            var routeValuesDictionary = new RouteValueDictionary();
            this.Request.QueryString.AllKeys.ToList().ForEach(key => routeValuesDictionary.Add(key, Request.QueryString[key]));

            return RedirectToAction("Age", "Health", routeValuesDictionary);
        }

        /// <summary>
        /// SSO による画面遷移を処理します。（問診システム画面）
        /// </summary>
        /// <returns>
        /// アクション の結果。
        /// </returns>
        private ActionResult RedirectToMonshin()
        {
            var routeValuesDictionary = new RouteValueDictionary();
            this.Request.QueryString.AllKeys.ToList().ForEach(key => routeValuesDictionary.Add(key, Request.QueryString[key]));

            return RedirectToAction("Monshin", "Note", routeValuesDictionary);
        }

        /// <summary>
        /// SSO による画面遷移を処理します。（ぎのわんエントリー画面）
        /// </summary>
        /// <returns>
        /// アクション の結果。
        /// </returns>
        private ActionResult RedirectToPortalLocalIdVerification()
        {
            var routeValuesDictionary = new RouteValueDictionary();
            this.Request.QueryString.AllKeys.ToList().ForEach(key => routeValuesDictionary.Add(key, Request.QueryString[key]));

            return RedirectToAction("LocalIdVerification", "Portal", routeValuesDictionary);
        }

        /// <summary>
        /// SSO による画面遷移を処理します。（法人連携・病院連携画面）
        /// </summary>
        /// <returns>
        /// アクション の結果。
        /// </returns>
        private ActionResult RedirectToPortalConnectionSetting()
        {
            var routeValuesDictionary = new RouteValueDictionary();
            this.Request.QueryString.AllKeys.ToList().ForEach(key => routeValuesDictionary.Add(key, Request.QueryString[key]));

            return RedirectToAction("ConnectionSetting", "Portal", routeValuesDictionary);
        }


        /// <summary>
        /// SSO による画面遷移を処理します。（auログイン画面）
        /// </summary>
        /// <returns>
        /// アクション の結果。
        /// </returns>
        private ActionResult RedirectToLoginByAuId()
        {
            var routeValuesDictionary = new RouteValueDictionary();
            this.Request.QueryString.AllKeys.ToList().ForEach(key => routeValuesDictionary.Add(key, Request.QueryString[key]));

            return RedirectToAction("LoginByAuId", "Start", routeValuesDictionary);
        }

        /// <summary>
        /// SSO による画面遷移を処理します。（病院連携画面）
        /// </summary>
        /// <returns>
        /// アクション の結果。
        /// </returns>
        private ActionResult RedirectToHospitalConnection()
        {
            var routeValuesDictionary = new RouteValueDictionary();
            this.Request.QueryString.AllKeys.ToList().ForEach(key => routeValuesDictionary.Add(key, Request.QueryString[key]));

            return RedirectToAction("HospitalConnection", "Integration", routeValuesDictionary);
        }

        /// <summary>
        /// SSO による画面遷移を処理します。（法人連携画面）
        /// </summary>
        /// <returns>
        /// アクション の結果。
        /// </returns>
        private ActionResult RedirectToCompanyConnection()
        {
            var routeValuesDictionary = new RouteValueDictionary();
            this.Request.QueryString.AllKeys.ToList().ForEach(key => routeValuesDictionary.Add(key, Request.QueryString[key]));

            return RedirectToAction("CompanyConnection", "Integration", routeValuesDictionary);
        }

        /// <summary>
        /// SSO による画面遷移を処理します。（タニタ連携画面）
        /// </summary>
        /// <returns>
        /// アクション の結果。
        /// </returns>
        private ActionResult RedirectToTanitaConnection()
        {
            var routeValuesDictionary = new RouteValueDictionary();
            this.Request.QueryString.AllKeys.ToList().ForEach(key => routeValuesDictionary.Add(key, Request.QueryString[key]));

            return RedirectToAction("TanitaConnection", "Integration", routeValuesDictionary);
        }

        /// <summary>
        /// 戻り先のURLを取得し、Cookieに保存します。
        /// </summary>
        /// <param name="session"></param>
        private void SetRedirectURL(HttpSessionStateBase session)
        {

            var urlParam = System.Web.HttpUtility.UrlDecode(Request.QueryString["redirecturl"]) ?? string.Empty;
            var urlNameParam = System.Web.HttpUtility.UrlDecode(Request.QueryString["redirecturlname"]) ?? string.Empty;

            if (string.IsNullOrEmpty(urlNameParam))
            {
                urlNameParam = "呼出元システム";
            }

            if (urlParam.Length > 8)
            {
                if (!urlParam.Substring(0, 8).Contains("https://") && !urlParam.Substring(0, 8).Contains("http://"))
                {
                    urlParam = "https://" + urlParam;
                }
            }

            //Logger.WriteLog($"urlParam:{urlParam}");
            //Logger.WriteLog($"urlNameParam:{urlNameParam}");

            //todo:必要に応じてクッキー設定
            //QjSessionHelper.RemoveItem(session, HealthCheckupCookieEntity.HEALTHCHECKUP_SESSION_KEY);
            //QjSessionHelper.SetItem(session, HealthCheckupCookieEntity.HEALTHCHECKUP_SESSION_KEY,
            //    new HealthCheckupCookieEntity() { RedirectUrl = urlParam, RedirectUrlName = urlNameParam });

        }

        /// <summary>
        /// SSO処理
        /// </summary>
        /// <returns></returns>
        private ActionResult SsoInternal(Guid executorAccountKey, Guid authorAccountKey, Guid targetAccountKey, int pageNo, string jwt)
        {

            if (executorAccountKey == Guid.Empty) throw new ArgumentOutOfRangeException("executorAccountKey", "実行者 アカウント キー が不正です。");
            if (authorAccountKey == Guid.Empty) throw new ArgumentOutOfRangeException("authorAccountKey", "所有者 アカウント キー が不正です。");
            if (targetAccountKey == Guid.Empty) throw new ArgumentOutOfRangeException("targetAccountKey", "対象者 アカウント キー が不正です。");
            if (pageNo <= 0) throw new ArgumentOutOfRangeException("pageNo", "ページ 番号 が不正です。");

            bool requireLogin = false;

            if (QjLoginHelper.CheckLogin(this.Session, this.Request))
            {

               // Logger.WriteLog("CheckLogin True");

                // セッション は有効
                QolmsJotoModel mainModel = QjLoginHelper.GetQolmsJotoModel(this.Session);

                if (mainModel != null
                    && mainModel.AuthorAccount != null
                    && mainModel.AuthorAccount.AccountKey == authorAccountKey)
                {
                    // ログイン 済み
                    if (pageNo == (int)QjPageNoTypeEnum.NoteExamination)
                    {
                        // 「健診結果」画面へ遷移
                        return this.RedirectToNoteExamination();
                    }
                    else if (pageNo == (int)QjPageNoTypeEnum.PointHistory)
                    {
                        // 「JOTOポイント」画面へ遷移
                        return this.RedirectToPointHistory();
                    }
                    else if (pageNo == (int)QjPageNoTypeEnum.PointLocalHistory)
                    {
                        // 「地域ポイント」画面へ遷移
                        return this.RedirectToLocalHistory();
                    }
                    else if (pageNo == (int)QjPageNoTypeEnum.PortalLocalIdVerification)
                    {
                        // 「ぎのわんエントリー」画面へ遷移
                        return this.RedirectToPortalLocalIdVerification();
                    }
                    else if (pageNo == (int)QjPageNoTypeEnum.HealthAge)
                    {
                        // 「健康年齢」画面へ遷移
                        return this.RedirectToHealthAge();
                    }
                    else if (pageNo == (int)QjPageNoTypeEnum.PortalConnectionSetting)
                    {
                        // 「法人連携・病院連携」画面へ遷移
                        return this.RedirectToPortalConnectionSetting();
                    }
                    else if (pageNo == (int)QjPageNoTypeEnum.PortalMedicalPayment)
                    {
                        // 「医療費あと払い」画面へ遷移
                        return this.RedirectToDemo("医療費あと払い");
                    }
                    else if (pageNo == (int)QjPageNoTypeEnum.PremiumIndex)
                    {
                        // 「プレミアム会員」画面へ遷移
                        return this.RedirectToDemo("プレミアム会員");
                    }
                    else if (pageNo == (int)QjPageNoTypeEnum.NoteCalomeal)
                    {
                        // 「カロミル」画面へ遷移
                        return this.RedirectToNoteCalomeal();
                    }
                    else if (pageNo == (int)QjPageNoTypeEnum.NoteMonshin)
                    {
                        // 「問診システム」画面へ遷移
                        return this.RedirectToMonshin();
                    }
                    else if (pageNo == (int)QjPageNoTypeEnum.LoginByAuId)
                    {
                        // 「auログイン」画面へ遷移
                        return this.RedirectToLoginByAuId();
                    }
                    else if (pageNo == (int)QjPageNoTypeEnum.HospitalConnection)
                    {
                        // 「病院連携」画面へ遷移
                        return this.RedirectToHospitalConnection();
                    }
                    else if (pageNo == (int)QjPageNoTypeEnum.CompanyConnection)
                    {
                        // 「法人連携」画面へ遷移
                        return this.RedirectToCompanyConnection();
                    }
                    else if (pageNo == (int)QjPageNoTypeEnum.TanitaConnection)
                    {
                        // 「タニタ連携」画面へ遷移
                        return this.RedirectToTanitaConnection();
                    }
                }
                else
                {
                    // メイン モデル が無効
                    requireLogin = true;
                }
            }
            else
            {
                // セッション は無効
                requireLogin = true;
            }

            if (requireLogin)
            {

                //Logger.WriteLog("CheckLogin False");

                // ログイン
                var refAuthorAccount = new AuthorAccountItem();
                //var refTargetAccount = new PersonalItem();
                var refStandardValueN = new List<QhApiTargetValueItem>();
                var refTargetValueN = new List<QhApiTargetValueItem>();
                decimal refHeight = decimal.Zero;
                Guid refApiAuthorizeKey = Guid.Empty;
                DateTime refApiAuthorizeExpires = DateTime.MinValue;
                Guid refApiAuthorizeKey2 = Guid.Empty;
                DateTime refApiAuthorizeExpires2 = DateTime.MinValue;
                byte refLoginRetryCount = byte.MinValue;
                DateTime refLoginLockdownExpires = DateTime.MinValue;
                bool refIsSettingComplete = false;
                switch (
                    LoginWorker.Auth(
                        this.Session.SessionID,
                        executorAccountKey,
                        authorAccountKey,
                        targetAccountKey,
                        ref refAuthorAccount,
                        ref refStandardValueN,
                        ref refTargetValueN,
                        ref refHeight,
                        ref refApiAuthorizeKey,
                        ref refApiAuthorizeExpires,
                        ref refApiAuthorizeKey2,
                        ref refApiAuthorizeExpires2,
                        ref refLoginRetryCount,
                        ref refLoginLockdownExpires,
                        ref refIsSettingComplete
                    )
                )
                {
                    case QsApiLoginResultTypeEnum.Success:
                        if (QjLoginHelper.ToLogin(
                            this.Session,
                            this.Response,
                            refAuthorAccount,
                            refApiAuthorizeKey,
                            refApiAuthorizeExpires,
                            refApiAuthorizeKey2,
                            refApiAuthorizeExpires2
                        ))
                        {
                            // 成功
                            // ログイン直後に会員種別を取得してセッションへ反映する。
                            var mainModel = this.GetQolmsJotoModel();
                            mainModel.AuthorAccount.MembershipType = (QjMemberShipTypeEnum)PremiumWorker.GetMemberShipType(mainModel);

                            // ログイン 済み
                            if (pageNo == (int)QjPageNoTypeEnum.NoteExamination)
                            {
                                // 「健診結果」画面へ遷移
                                return this.RedirectToNoteExamination();
                            }
                            else if (pageNo == (int)QjPageNoTypeEnum.PointHistory)
                            {
                                // 「JOTOポイント」画面へ遷移
                                return this.RedirectToPointHistory();
                            }
                            else if (pageNo == (int)QjPageNoTypeEnum.PointLocalHistory)
                            {
                                // 「地域ポイント」画面へ遷移
                                return this.RedirectToLocalHistory();
                            }
                            else if (pageNo == (int)QjPageNoTypeEnum.PortalLocalIdVerification)
                            {
                                // 「ぎのわんエントリー」画面へ遷移
                                return this.RedirectToPortalLocalIdVerification();
                            }
                            else if (pageNo == (int)QjPageNoTypeEnum.HealthAge)
                            {
                                // 「健康年齢」画面へ遷移
                                return this.RedirectToHealthAge();
                            }
                            else if (pageNo == (int)QjPageNoTypeEnum.PortalConnectionSetting)
                            {
                                // 「法人連携・病院連携」画面へ遷移
                                return this.RedirectToPortalConnectionSetting();
                            }
                            else if (pageNo == (int)QjPageNoTypeEnum.PortalMedicalPayment)
                            {
                                // 「医療費あと払い」画面へ遷移
                                return this.RedirectToDemo("医療費あと払い");
                            }
                            else if (pageNo == (int)QjPageNoTypeEnum.PremiumIndex)
                            {
                                // 「プレミアム会員」画面へ遷移
                                return this.RedirectToDemo("プレミアム会員");
                            }
                            else if (pageNo == (int)QjPageNoTypeEnum.NoteCalomeal)
                            {
                                // 「カロミル」画面へ遷移
                                return this.RedirectToNoteCalomeal();
                            }
                            else if (pageNo == (int)QjPageNoTypeEnum.NoteMonshin)
                            {
                                // 「問診システム」画面へ遷移
                                return this.RedirectToMonshin();
                            }
                            else if (pageNo == (int)QjPageNoTypeEnum.LoginByAuId)
                            {
                                // 「auログイン」画面へ遷移
                                return this.RedirectToLoginByAuId();
                            }
                            else if (pageNo == (int)QjPageNoTypeEnum.HospitalConnection)
                            {
                                // 「病院連携」画面へ遷移
                                return this.RedirectToHospitalConnection();
                            }
                            else if (pageNo == (int)QjPageNoTypeEnum.CompanyConnection)
                            {
                                // 「法人連携」画面へ遷移
                                return this.RedirectToCompanyConnection();
                            }
                            else if (pageNo == (int)QjPageNoTypeEnum.TanitaConnection)
                            {
                                // 「タニタ連携」画面へ遷移
                                return this.RedirectToTanitaConnection();
                            }

                        }
                        break;
                }
            }

            throw new InvalidOperationException("SSO に失敗しました。");
        }


        /// <summary>
        /// SSO による ログイン 要求を処理します。
        /// </summary>
        /// <returns>
        /// アクション の結果。
        /// </returns>
        //
        /// [QjLogging()]
        [HttpGet()]
        [QjJwtAuthorize()]
        [OutputCache(CacheProfile = "DisableCacheProfile", NoStore = true)]
        public ActionResult Sso()
        {
            Guid executorAccountKey = this.HttpContext.Items[QjJwtAuthorizeAttribute.JWT_EXECUTOR_ACCOUNT_KEY].ToString().TryToValueType(Guid.Empty); // 呼び出し元 アカウント キー（アプリ 等に割り当てられた アカウント キー）
            Guid authorAccountKey = this.HttpContext.Items[QjJwtAuthorizeAttribute.JWT_AUTHOR_ACCOUNT_KEY].ToString().TryToValueType(Guid.Empty); // 本人の アカウント キー
            Guid targetAccountKey = this.HttpContext.Items[QjJwtAuthorizeAttribute.JWT_TARGET_ACCOUNT_KEY].ToString().TryToValueType(Guid.Empty); // 本人もしくは家族の アカウント キー
            int pageNo = this.HttpContext.Items[QjJwtAuthorizeAttribute.JWT_PAGE_NO].ToString().TryToValueType(int.MinValue); // 将来使用予定
            if (!string.IsNullOrEmpty(Request.QueryString["pageno"]))// HOSPAようにQueryパラメータでも取得できるように。
            {
                pageNo = Convert.ToInt32(Request.QueryString["pageno"]);
                //Logger.WriteLog($"Request pageno : {pageNo}");
            }

            //Logger.WriteLog($"pageno : {pageNo}");

            string jwt = SsoHelper.GetJwt(this.HttpContext.Request.Headers["Authorization"]);

            return SsoInternal(executorAccountKey, authorAccountKey, targetAccountKey, pageNo, jwt);
        }

        [HttpGet()]
        public ActionResult LoginbyId()
        {

            return View();
        }

        #endregion

        #region "アプリケーション ゲートウェイ の 正常性 プローブ 用応答"

        /// <summary>
        /// アプリケーション ゲートウェイ の 正常性 プローブ の要求を処理します。
        /// </summary>
        /// <returns>
        /// アクション の結果。
        /// </returns>
        [HttpGet()]
        [OutputCache(CacheProfile = "DisableCacheProfile")]
        public ActionResult Health()
        {
            // HTTP ステータス 200 を返すだけで良い
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }

        #endregion

        #endregion

    }
}