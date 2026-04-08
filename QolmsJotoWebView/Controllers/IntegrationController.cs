using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsJotoWebView.Repositories;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 外部連携に関するコントローラーです。
    /// 企業連携・病院連携・デバイス連携の画面処理を担います。
    /// </summary>
    public class IntegrationController : QjMvcControllerBase
    {
        private const string DefaultCompanyLinkageSystemName = "企業";
        private const string DefaultHospitalName = "城東区医師会病院";

        [HttpGet]
        [QjAuthorize]
        //[QjLogging]
        public ActionResult FitbitConnection(byte? fromPageNo, bool? mockConnected)
        {
            var viewModel = new IntegrationFitbitConnectionViewModel()
            {
                FromPageNoType = ResolveFromPageNoType(fromPageNo),
                FitbitConnectedFlag = mockConnected ?? false
            };

            return View(viewModel);
        }


        [HttpGet]
        [QjAuthorize]
        //[QjLogging]
        public ActionResult OmronConnection(byte? fromPageNo, bool? mockConnected)
        {
            var viewModel = new IntegrationOmronConnectionViewModel()
            {
                FromPageNoType = ResolveFromPageNoType(fromPageNo),
                OmronConnectedFlag = mockConnected ?? false
            };

            return View(viewModel);
        }

        #region 「タニタ連携」画面

        [HttpGet]
        [QjAuthorize]
        [QyApiAuthorize]
        [QjLogging]
        public ActionResult TanitaConnection()
        {
            var worker = new IntegrationTanitaConnectionWorker(new KaradaKaruteApiRepository(), new TanitaConnectionRepository());
            return View(worker.CreateViewModel(this.GetQolmsJotoModel()));
        }

        [HttpPost]
        [QjAuthorize]
        [QjAjaxOnly]
        [QjActionMethodSelector("Register")]
        [ValidateAntiForgeryToken(Order = int.MaxValue)]
        [QyApiAuthorize]
        [QjLogging]
        public ActionResult TanitaConnectionResult(IntegrationTanitaConnectionViewModel model, bool alkooCancelFlag)
        {
            var worker = new IntegrationTanitaConnectionWorker(new KaradaKaruteApiRepository(), new TanitaConnectionRepository());
            // 更新のフローと画面反映処理とかエラー処理とか
            if (this.ModelState.IsValid)
            {
                //ALKOOから歩数もらわないので気にしない
                //string messageAlkoo = string.Empty;
                //if (alkooCancelFlag)
                //{
                //    if (!worker.Cancel(this.GetQolmsJotoModel(), ref messageAlkoo))
                //    {
                //        return new MessageJsonResult()
                //        {
                //            IsSuccess = bool.FalseString,
                //            Message = HttpUtility.HtmlEncode("Alkoo連携の解除に失敗しました。")
                //        }.ToJsonResult();
                //    }
                //}

                var errorMessage = new Dictionary<string, string>();

                if (this.ModelState.IsValid)
                {
                    string message = string.Empty;
                    if (worker.ConnectionRegister(this.GetQolmsJotoModel(), model, ref message))
                    {
                        return new MessageJsonResult()
                        {
                            IsSuccess = bool.TrueString,
                            Message = string.Empty
                        }.ToJsonResult();
                    }
                    else
                    {
                        return new MessageJsonResult()
                        {
                            IsSuccess = bool.FalseString,
                            Message = HttpUtility.HtmlEncode("登録に失敗しました。")
                        }.ToJsonResult();
                    }
                }

                return new MessageJsonResult()
                {
                    IsSuccess = bool.FalseString,
                    Message = HttpUtility.HtmlEncode("IDまたはパスワードが不正です。")
                }.ToJsonResult();
            }
            else
            {
                string errorMessage = string.Empty;

                foreach (var key in this.ModelState.Keys)
                {
                    foreach (var e in this.ModelState[key].Errors)
                    {
                        errorMessage += e.ErrorMessage;
                    }
                }

                return new MessageJsonResult()
                {
                    IsSuccess = bool.FalseString,
                    Message = HttpUtility.HtmlEncode(errorMessage)
                }.ToJsonResult();
            }
        }

        [HttpPost]
        [QjAjaxOnly]
        [QjAuthorize(true)]
        [QjActionMethodSelector("Update")]
        [ValidateAntiForgeryToken(Order = int.MaxValue)]
        [QyApiAuthorize]
        [QjLogging]
        public JsonResult TanitaConnectionResult(string data, bool @checked, bool alkooCancelFlag)
        {
            var worker = new IntegrationTanitaConnectionWorker(new KaradaKaruteApiRepository(), new TanitaConnectionRepository());
            //ALKOOから歩数もらわないので気にしない
            // ALKOO
            //string messageAlkoo = string.Empty;
            //if (alkooCancelFlag)
            //{
            //    if (!PortalAlkooConnectionWorker.Cancel(this.GetQolmsJotoModel(), ref messageAlkoo))
            //    {
            //        return new MessageJsonResult()
            //        {
            //            IsSuccess = bool.FalseString,
            //            Message = HttpUtility.HtmlEncode("Alkoo連携の解除に失敗しました。")
            //        }.ToJsonResult();
            //    }
            //}

            try
            {
                var devices = worker.DeviceRegister(this.GetQolmsJotoModel(), data, @checked);
                return new IntegrationTanitaConnectionJsonResult()
                {
                    IsSuccess = bool.TrueString,
                    Devises = devices,
                    Message = string.Empty
                }.ToJsonResult();
            }
            catch (Exception ex)
            {
                // アクセスログ
                AccessLogWorker.WriteAccessLog(this.HttpContext, string.Empty, AccessLogWorker.AccessTypeEnum.Error, string.Format(ex.Message));

                return new MessageJsonResult()
                {
                    IsSuccess = bool.FalseString,
                    Message = HttpUtility.HtmlEncode("更新に失敗しました")
                }.ToJsonResult();
            }
        }

        [HttpPost]
        [QjAjaxOnly]
        [QjAuthorize(true)]
        [QjActionMethodSelector("Cancel")]
        [ValidateAntiForgeryToken(Order = int.MaxValue)]
        [QyApiAuthorize]
        [QjLogging]
        public ActionResult TanitaConnectionResult()
        {
            var worker = new IntegrationTanitaConnectionWorker(new KaradaKaruteApiRepository(), new TanitaConnectionRepository());
            string message = string.Empty;
            if (worker.Cancel(this.GetQolmsJotoModel(), ref message))
            {
                return new MessageJsonResult()
                {
                    IsSuccess = bool.TrueString,
                    Message = string.Empty
                }.ToJsonResult();
            }
            else
            {
                return new MessageJsonResult()
                {
                    IsSuccess = bool.FalseString,
                    Message = HttpUtility.HtmlEncode("連携解除に失敗しました。")
                }.ToJsonResult();
            }
        }

        #endregion

        /// <summary>
        /// 企業連携ホーム画面の表示要求を処理します。
        /// 企業会員以外の場合は連携設定へリダイレクトします。
        /// </summary>
        [HttpGet]
        [QjAuthorize]
        //[QjLogging]
        public ActionResult CompanyConnectionHome(byte? fromPageNo)
        {
            // 企業会員以外は企業連携ホームを表示せず、連携設定へ戻す。
            var mainModel = this.GetQolmsJotoModel();
            if (mainModel.AuthorAccount.MembershipType != QjMemberShipTypeEnum.Business
                && mainModel.AuthorAccount.MembershipType != QjMemberShipTypeEnum.BusinessFree)
            {
                return RedirectToAction("ConnectionSetting", "Portal", new { fromPageNo, tabNo = 2 });
            }

            var viewModel = new IntegrationCompanyConnectionHomeViewModel()
            {
                FromPageNoType = ResolveFromPageNoType(fromPageNo),
                LinkageSystemName = DefaultCompanyLinkageSystemName
            };

            return View(viewModel);
        }

        /// <summary>
        /// 企業連携申請画面の表示要求を処理します。
        /// ログイン中アカウント情報を初期値として設定し、入力確認用キャッシュを保存します。
        /// </summary>
        [HttpGet]
        [QjAuthorize]
        //[QjLogging]
        public ActionResult CompanyConnectionRequest(byte? fromPageNo, string linkageSystemName)
        {
            // ログイン中アカウント情報から申請画面の初期値を構築する。
            var worker = new IntegrationCompanyConnectionRequestWorker();
            var viewModel = worker.CreateViewModel(this.GetQolmsJotoModel(), ResolveFromPageNoType(fromPageNo));
            viewModel.LinkageSystemName = string.IsNullOrWhiteSpace(linkageSystemName)
                ? DefaultCompanyLinkageSystemName
                : linkageSystemName;
            viewModel.SexLabel = ConvertSexToLabel(viewModel.SexType);
            viewModel.BirthDateLabel = string.Format("{0}年 {1}月 {2}日", viewModel.BirthYear, viewModel.BirthMonth, viewModel.BirthDay);

            // 入力確認に利用できるよう、初期状態をキャッシュしておく。
            this.GetQolmsJotoModel().SetInputModelCache(new IntegrationCompanyConnectionRequestInputModel
            {
                FromPageNoType = viewModel.FromPageNoType,
                FacilityId = viewModel.CompanyCode,
                EmployeeNo = viewModel.EmployeeNo,
                FamilyName = viewModel.FamilyName,
                GivenName = viewModel.GivenName,
                FamilyKanaName = viewModel.FamilyKanaName,
                GivenKanaName = viewModel.GivenKanaName,
                SexType = viewModel.SexType,
                BirthYear = viewModel.BirthYear,
                BirthMonth = viewModel.BirthMonth,
                BirthDay = viewModel.BirthDay,
                RelationContentFlags = BuildRelationContentFlags(
                    viewModel.ShareBasicInfo,
                    viewModel.ShareContactNotebook,
                    viewModel.ShareVitalNotebook,
                    viewModel.ShareMedicineNotebook,
                    viewModel.ShareExaminationNotebook)
            });

            return View(viewModel);
        }

        /// <summary>
        /// 企業連携詳細画面の表示要求を処理します。
        /// 無効な連携番号の場合は連携設定へリダイレクトします。
        /// </summary>
        [HttpGet]
        [QjAuthorize]
        [QyApiAuthorize]
        //[QjLogging]
        public ActionResult CompanyConnection(byte? fromPageNo, int? linkageSystemNo, string linkageSystemName, bool? mockConnected)
        {
            // 連携番号が不正な場合は企業タブ付き連携設定へ戻す。
            if (!linkageSystemNo.HasValue || linkageSystemNo.Value <= 0)
            {
                return RedirectToAction("ConnectionSetting", "Portal", new { fromPageNo, tabNo = 2 });
            }

            var worker = new IntegrationCompanyConnectionWorker();
            var viewModel = worker.CreateViewModel(this.GetQolmsJotoModel(), linkageSystemNo.Value, ResolveFromPageNoType(fromPageNo));
            if (viewModel == null)
            {
                return RedirectToAction("ConnectionSetting", "Portal", new { fromPageNo, tabNo = 2 });
            }

            if (!string.IsNullOrWhiteSpace(linkageSystemName))
            {
                viewModel.LinkageSystemName = linkageSystemName;
            }

            return View(viewModel);
        }

        /// <summary>
        /// 企業連携編集画面の表示要求を処理します。
        /// 無効な連携番号の場合は連携設定へリダイレクトします。
        /// </summary>
        [HttpGet]
        [QjAuthorize]
        [QyApiAuthorize]
        //[QjLogging]
        public ActionResult CompanyConnectionEdit(byte? fromPageNo, int? linkageSystemNo, string linkageSystemName)
        {
            // 編集対象が不正な場合は連携設定へ戻し、直接遷移を防ぐ。
            if (!linkageSystemNo.HasValue || linkageSystemNo.Value <= 0)
            {
                return RedirectToAction("ConnectionSetting", "Portal", new { fromPageNo, tabNo = 2 });
            }

            var worker = new IntegrationCompanyConnectionEditWorker();
            var viewModel = worker.CreateViewModel(this.GetQolmsJotoModel(), linkageSystemNo.Value, ResolveFromPageNoType(fromPageNo));
            if (viewModel == null || viewModel.LinkageSystemNo <= 0)
            {
                return RedirectToAction("ConnectionSetting", "Portal", new { fromPageNo, tabNo = 2 });
            }

            if (!string.IsNullOrWhiteSpace(linkageSystemName))
            {
                viewModel.LinkageSystemName = linkageSystemName;
            }

            return View(viewModel);
        }

        /// <summary>
        /// 企業連携解除の要求を処理し、結果を JSON で返します。
        /// </summary>
        [HttpPost]
        [QjAuthorize]
        [QyApiAuthorize]
        [QjApiAuthorize]
        //[QjLogging]
        [QjActionMethodSelector("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult CompanyConnectionResult(int linkageSystemNo)
        {
            // 解除APIの実行結果をJSONで返す。
            var worker = new IntegrationCompanyConnectionWorker();
            string message = string.Empty;

            if (linkageSystemNo > 0 && worker.Delete(this.GetQolmsJotoModel(), linkageSystemNo, ref message))
            {
                return new IntegrationCompanyConnectionJsonResult
                {
                    IsSuccess = bool.TrueString
                }.ToJsonResult();
            }

            return new IntegrationCompanyConnectionJsonResult
            {
                IsSuccess = bool.FalseString,
                Messages = new Dictionary<string, string>
                {
                    { "summary", string.IsNullOrWhiteSpace(message) ? "連携解除に失敗しました。" : HttpUtility.HtmlEncode(message) }
                }
            }.ToJsonResult();
        }

        /// <summary>
        /// 企業連携申請前の本人確認検証を行い、結果を JSON で返します。
        /// </summary>
        [HttpPost]
        [QjAuthorize]
        [QjApiAuthorize]
        //[QjLogging]
        [QjActionMethodSelector("IsIdentityChecked")]
        [ValidateAntiForgeryToken]
        public ActionResult CompanyConnectionRequestResult(IntegrationCompanyConnectionRequestInputModel model, string dummy)
        {
            var errors = new Dictionary<string, string>();
            var mainModel = this.GetQolmsJotoModel();
            var account = mainModel.AuthorAccount;

            if (!this.ModelState.IsValid)
            {
                AddModelStateErrors(errors);
                errors["summary"] = "入力内容を確認してください。";

                return new IntegrationCompanyConnectionJsonResult
                {
                    IsSuccess = bool.FalseString,
                    Messages = errors
                }.ToJsonResult();
            }

            DateTime birthday = DateTime.MinValue;
            try
            {
                birthday = new DateTime(int.Parse(model.BirthYear), int.Parse(model.BirthMonth), int.Parse(model.BirthDay));
            }
            catch
            {
            }

            var isNameMatched = account.FamilyName == model.FamilyName
                && account.GivenName == model.GivenName
                && account.FamilyKanaName == model.FamilyKanaName
                && account.GivenKanaName == model.GivenKanaName;
            var isBirthdaySexMatched = account.Birthday == birthday
                && account.SexType == model.SexType;

            if (isNameMatched && isBirthdaySexMatched)
            {
                return new IntegrationCompanyConnectionJsonResult
                {
                    IsSuccess = bool.TrueString,
                    Messages = errors
                }.ToJsonResult();
            }

            if (isBirthdaySexMatched)
            {
                errors["summary"] = "入力された個人情報が登録と一致しませんでした。個人情報更新しますか？";

                return new IntegrationCompanyConnectionJsonResult
                {
                    IsSuccess = bool.TrueString,
                    Messages = errors
                }.ToJsonResult();
            }

            errors["summary"] = "性別、生年月日の変更はできません。";

            return new IntegrationCompanyConnectionJsonResult
            {
                IsSuccess = bool.FalseString,
                Messages = errors
            }.ToJsonResult();
        }
        /// <summary>
        /// 企業連携申請の実行要求を処理し、結果を JSON で返します。
        /// </summary>        [HttpPost]
        [QjAuthorize]
        [QyApiAuthorize]
        [QjApiAuthorize]
        //[QjLogging]
        [QjActionMethodSelector("Request")]
        [ValidateAntiForgeryToken]
        public ActionResult CompanyConnectionRequestResult(IntegrationCompanyConnectionRequestInputModel model)
        {
            // 申請APIの結果をフロント用JSONとして返す。
            string message = string.Empty;
            int linkageSystemNo = int.MinValue;
            var worker = new IntegrationCompanyConnectionRequestWorker();
            var errors = new Dictionary<string, string>();
            var mainModel = this.GetQolmsJotoModel();

            if (this.ModelState.IsValid)
            {
                // Yappli 準拠: 先にプレミアム解約処理を実行し、成功時のみ申請を実行する。
                if (!PremiumWorker.CancelBusinessPremium(mainModel))
                {
                    errors["summary"] = "課金登録解除に失敗しました。サポート窓口へご連絡ください。";

                    return new IntegrationCompanyConnectionJsonResult
                    {
                        IsSuccess = bool.FalseString,
                        Messages = errors
                    }.ToJsonResult();
                }

                if (worker.Request(mainModel, model, ref linkageSystemNo, ref message))
                {
                    return new IntegrationCompanyConnectionJsonResult
                    {
                        IsSuccess = bool.TrueString,
                        LinkageSystemNo = linkageSystemNo.ToString()
                    }.ToJsonResult();
                }
            }

            AddModelStateErrors(errors);
            if (!string.IsNullOrWhiteSpace(message))
            {
                errors["summary"] = HttpUtility.HtmlEncode(message);
            }
            else
            {
                errors["summary"] = "入力内容を確認してください。";
            }

            return new IntegrationCompanyConnectionJsonResult
            {
                IsSuccess = bool.FalseString,
                Messages = errors
            }.ToJsonResult();
        }

        /// <summary>
        /// 企業連携情報の編集要求を処理し、結果を JSON で返します。
        /// </summary>
        [HttpPost]
        [QjAuthorize]
        [QyApiAuthorize]
        [QjApiAuthorize]
        //[QjLogging]
        [ValidateAntiForgeryToken]
        public ActionResult CompanyConnectionEditResult(IntegrationCompanyConnectionEditInputModel model)
        {
            // 編集APIの結果をフロント用JSONとして返す。
            string message = string.Empty;
            int linkageSystemNo = int.MinValue;
            var worker = new IntegrationCompanyConnectionEditWorker();
            var errors = new Dictionary<string, string>();

            if (this.ModelState.IsValid)
            {
                if (worker.Edit(this.GetQolmsJotoModel(), model, ref linkageSystemNo, ref message))
                {
                    return new IntegrationCompanyConnectionJsonResult
                    {
                        IsSuccess = bool.TrueString,
                        LinkageSystemNo = linkageSystemNo.ToString()
                    }.ToJsonResult();
                }
            }

            AddModelStateErrors(errors);
            if (!string.IsNullOrWhiteSpace(message))
            {
                errors["summary"] = HttpUtility.HtmlEncode(message);
            }
            else
            {
                string firstError = errors.Values.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));
                errors["summary"] = string.IsNullOrWhiteSpace(firstError)
                    ? "入力内容を確認してください。"
                    : firstError;
            }

            return new IntegrationCompanyConnectionJsonResult
            {
                IsSuccess = bool.FalseString,
                Messages = errors
            }.ToJsonResult();
        }

        /// <summary>
        /// 病院連携詳細画面の表示要求を処理します。
        /// 無効な連携番号の場合は連携設定へリダイレクトします。
        /// </summary>
        [HttpGet]
        [QjAuthorize]
        [QyApiAuthorize]
        //[QjLogging]
        public ActionResult HospitalConnection(byte? fromPageNo, int? linkageSystemNo, string hospitalName)
        {
            if (!linkageSystemNo.HasValue || linkageSystemNo.Value <= 0)
            {
                return RedirectToAction("ConnectionSetting", "Portal", new { fromPageNo, tabNo = 3 });
            }

            var worker = new IntegrationHospitalConnectionWorker();
            var viewModel = worker.CreateViewModel(this.GetQolmsJotoModel(), linkageSystemNo.Value, ResolveFromPageNoType(fromPageNo));
            if (viewModel == null)
            {
                return RedirectToAction("ConnectionSetting", "Portal", new { fromPageNo, tabNo = 3 });
            }

            if (!string.IsNullOrWhiteSpace(hospitalName))
            {
                viewModel.HospitalName = hospitalName;
            }

            return View(viewModel);
        }

        /// <summary>
        /// 病院連携申請画面の表示要求を処理します。
        /// ログイン中アカウント情報を初期値として設定し、メールアドレス変更判定用のキャッシュを保存します。
        /// </summary>
        [HttpGet]
        [QjAuthorize]
        [QyApiAuthorize]
        //[QjLogging]
        public ActionResult HospitalConnectionRequest(byte? fromPageNo, int? linkageSystemNo, string hospitalName)
        {
            var mainModel = this.GetQolmsJotoModel();
            var worker = new IntegrationHospitalConnectionRequestWorker();
            var viewModel = worker.CreateViewModel(
                mainModel,
                linkageSystemNo.HasValue ? linkageSystemNo.Value : int.MinValue,
                ResolveFromPageNoType(fromPageNo));
            if (viewModel == null)
            {
                return RedirectToAction("ConnectionSetting", "Portal", new { fromPageNo, tabNo = 3 });
            }

            viewModel.HospitalName = string.IsNullOrWhiteSpace(hospitalName) ? (string.IsNullOrWhiteSpace(viewModel.HospitalName) ? DefaultHospitalName : viewModel.HospitalName) : hospitalName;

            // inputModel のキャッシュ（メールアドレスの変更判定のため）
            var cacheModel = new IntegrationHospitalConnectionRequestInputModel
            {
                LinkageSystemNo = viewModel.LinkageSystemNo,
                LinkageSystemId = viewModel.PatientNo,
                FamilyName = viewModel.FamilyName,
                GivenName = viewModel.GivenName,
                FamilyKanaName = viewModel.FamilyKanaName,
                GivenKanaName = viewModel.GivenKanaName,
                SexType = viewModel.SexType,
                BirthYear = viewModel.BirthYear,
                BirthMonth = viewModel.BirthMonth,
                BirthDay = viewModel.BirthDay,
                MailAddress = viewModel.MailAddress,
                IdentityUpdateFlag = viewModel.IdentityUpdateFlag,
                RelationContentFlags = viewModel.RelationContentFlags
            };
            mainModel.SetInputModelCache(cacheModel);

            return View(viewModel);
        }

        /// <summary>
        /// 病院連携解除の要求を処理し、結果を JSON で返します。
        /// </summary>
        [HttpPost]
        [QjAuthorize]
        [QyApiAuthorize]
        [QjApiAuthorize]
        [QjActionMethodSelector("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult HospitalConnectionResult(int linkageSystemNo)
        {
            var worker = new IntegrationHospitalConnectionWorker();
            string message = string.Empty;

            if (linkageSystemNo > 0 && worker.Delete(this.GetQolmsJotoModel(), linkageSystemNo, ref message))
            {
                return new IntegrationCompanyConnectionJsonResult
                {
                    IsSuccess = bool.TrueString
                }.ToJsonResult();
            }

            return new IntegrationCompanyConnectionJsonResult
            {
                IsSuccess = bool.FalseString,
                Messages = new Dictionary<string, string>
                {
                    { "summary", string.IsNullOrWhiteSpace(message) ? "病院連携の解除に失敗しました。" : HttpUtility.HtmlEncode(message) }
                }
            }.ToJsonResult();
        }

        /// <summary>
        /// 病院連携申請前の本人確認検証を行い、結果を JSON で返します。
        /// キャッシュしたメールアドレスとの一致判定も含みます。
        /// </summary>
        [HttpPost]
        [QjAuthorize]
        [QjApiAuthorize]
        [QjActionMethodSelector("IsIdentityChecked")]
        [ValidateAntiForgeryToken]
        public ActionResult HospitalConnectionRequestResult(IntegrationHospitalConnectionRequestInputModel model, string dummy)
        {
            var errors = new Dictionary<string, string>();
            var mainModel = this.GetQolmsJotoModel();
            var account = mainModel.AuthorAccount;

            if (!this.ModelState.IsValid)
            {
                var firstError = this.ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .FirstOrDefault(e => !string.IsNullOrWhiteSpace(e));
                errors["summary"] = string.IsNullOrWhiteSpace(firstError)
                    ? "入力内容を確認してください。"
                    : HttpUtility.HtmlEncode(firstError);

                return new IntegrationCompanyConnectionJsonResult
                {
                    IsSuccess = bool.FalseString,
                    Messages = errors
                }.ToJsonResult();
            }

            DateTime birthday;
            try
            {
                birthday = new DateTime(int.Parse(model.BirthYear), int.Parse(model.BirthMonth), int.Parse(model.BirthDay));
            }
            catch
            {
                errors["summary"] = "生年月日が不正です。";

                return new IntegrationCompanyConnectionJsonResult
                {
                    IsSuccess = bool.FalseString,
                    Messages = errors
                }.ToJsonResult();
            }

            var cachedModel = mainModel.GetInputModelCache<IntegrationHospitalConnectionRequestInputModel>();
            string cachedMailAddress = cachedModel != null ? cachedModel.MailAddress : string.Empty;

            bool isIdentityMatched = account.FamilyName == model.FamilyName
                && account.GivenName == model.GivenName
                && account.FamilyKanaName == model.FamilyKanaName
                && account.GivenKanaName == model.GivenKanaName
                && account.Birthday == birthday
                && account.SexType == model.SexType
                && cachedMailAddress == model.MailAddress;

            if (isIdentityMatched)
            {
                return new IntegrationCompanyConnectionJsonResult
                {
                    IsSuccess = bool.TrueString,
                    Messages = errors
                }.ToJsonResult();
            }

            if (account.Birthday == birthday && account.SexType == model.SexType)
            {
                errors["summary"] = "入力された個人情報が登録と一致しませんでした。個人情報更新しますか？";

                return new IntegrationCompanyConnectionJsonResult
                {
                    IsSuccess = bool.TrueString,
                    Messages = errors
                }.ToJsonResult();
            }

            errors["summary"] = "性別、生年月日の変更はできません。";

            return new IntegrationCompanyConnectionJsonResult
            {
                IsSuccess = bool.FalseString,
                Messages = errors
            }.ToJsonResult();
        }

        /// <summary>
        /// 病院連携申請の実行要求を処理し、結果を JSON で返します。
        /// </summary>
        [HttpPost]
        [QjAuthorize]
        [QyApiAuthorize]
        [QjApiAuthorize]
        [QjActionMethodSelector("Request")]
        [ValidateAntiForgeryToken]
        public ActionResult HospitalConnectionRequestResult(IntegrationHospitalConnectionRequestInputModel model)
        {
            string message = string.Empty;
            var worker = new IntegrationHospitalConnectionRequestWorker();
            var errors = new Dictionary<string, string>();

            if (this.ModelState.IsValid)
            {
                if (worker.Request(this.GetQolmsJotoModel(), model, ref message))
                {
                    return new IntegrationCompanyConnectionJsonResult
                    {
                        IsSuccess = bool.TrueString,
                        LinkageSystemNo = model.LinkageSystemNo.ToString()
                    }.ToJsonResult();
                }
            }

            var firstValidationError = this.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .FirstOrDefault(e => !string.IsNullOrWhiteSpace(e));

            errors["summary"] = !string.IsNullOrWhiteSpace(message)
                ? HttpUtility.HtmlEncode(message)
                : (!string.IsNullOrWhiteSpace(firstValidationError)
                    ? HttpUtility.HtmlEncode(firstValidationError)
                    : "入力内容を確認してください。");

            return new IntegrationCompanyConnectionJsonResult
            {
                IsSuccess = bool.FalseString,
                Messages = errors
            }.ToJsonResult();
        }

        /// <summary>
        /// 画面確認用です
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [QjAuthorize]
        //[QjLogging]
        public ContentResult MockPages()
        {
            string authority = Request?.Url?.GetLeftPart(UriPartial.Authority) ?? "https://localhost:44384";

            var mockPages = new[]
            {
                new { Label = "タニタ連携（未連携）", Url = authority + Url.Action(nameof(TanitaConnection), "Integration", new { mockConnected = false }) },
                new { Label = "タニタ連携（連携済み）", Url = authority + Url.Action(nameof(TanitaConnection), "Integration", new { mockConnected = true }) },
                new { Label = "企業連携 ホーム", Url = authority + Url.Action(nameof(CompanyConnectionHome), "Integration", null) },
                new { Label = "企業連携 申請", Url = authority + Url.Action(nameof(CompanyConnectionRequest), "Integration", new { linkageSystemName = DefaultCompanyLinkageSystemName }) },
                new { Label = "病院連携 申請", Url = authority + Url.Action(nameof(HospitalConnectionRequest), "Integration", new { linkageSystemNo = 47010, hospitalName = DefaultHospitalName }) }
            };

            string html = "<!DOCTYPE html><html><head><meta charset=\"utf-8\" /><title>Integration Mock Pages</title></head><body>"
                + "<h1>連携画面モック確認用URL</h1><ul>"
                + string.Join(string.Empty, mockPages.Select(x => string.Format("<li>{0}<br /><a href=\"{1}\">{1}</a></li>", x.Label, x.Url)))
                + "</ul></body></html>";

            return Content(html, "text/html");
        }

        private static QjPageNoTypeEnum ResolveFromPageNoType(byte? fromPageNo)
        {
            QjPageNoTypeEnum fromPageNoType = QjPageNoTypeEnum.PortalConnectionSetting;

            if (fromPageNo.HasValue && Enum.IsDefined(typeof(QjPageNoTypeEnum), fromPageNo.Value))
            {
                fromPageNoType = (QjPageNoTypeEnum)fromPageNo.Value;
            }

            return fromPageNoType;
        }

        private static QjRelationContentTypeEnum BuildRelationContentFlags(
            bool shareBasicInfo,
            bool shareContactNotebook,
            bool shareVitalNotebook,
            bool shareMedicineNotebook,
            bool shareExaminationNotebook)
        {
            // 画面上のチェック状態をAPI連携用のFlags列挙体へ集約する。
            QjRelationContentTypeEnum flags = QjRelationContentTypeEnum.None;

            if (shareBasicInfo)
            {
                flags |= QjRelationContentTypeEnum.Information;
            }

            if (shareContactNotebook)
            {
                flags |= QjRelationContentTypeEnum.Contact;
            }

            if (shareVitalNotebook)
            {
                flags |= QjRelationContentTypeEnum.Vital;
            }

            if (shareMedicineNotebook)
            {
                flags |= QjRelationContentTypeEnum.Medicine;
            }

            if (shareExaminationNotebook)
            {
                flags |= QjRelationContentTypeEnum.Examination;
            }

            return flags;
        }

        private static string ConvertSexToLabel(QjSexTypeEnum sexType)
        {
            switch (sexType)
            {
                case QjSexTypeEnum.Male:
                    return "男性";
                case QjSexTypeEnum.Female:
                    return "女性";
                default:
                    return "その他";
            }
        }

        private void AddModelStateErrors(Dictionary<string, string> errors)
        {
            // ModelStateのエラーをフロント表示しやすい辞書形式へ変換する。
            foreach (string key in this.ModelState.Keys)
            {
                foreach (ModelError error in this.ModelState[key].Errors)
                {
                    if (errors.ContainsKey(key))
                    {
                        errors[key] += "," + HttpUtility.HtmlEncode(error.ErrorMessage);
                    }
                    else
                    {
                        errors.Add(key, HttpUtility.HtmlEncode(error.ErrorMessage));
                    }
                }
            }
        }

        #region 共通パーツ

        /// <summary>
        /// 「ヘッダー」パーシャル ビューの表示要求を処理します。
        /// </summary>
        /// <returns>
        /// アクションの結果。
        /// </returns>
        [ChildActionOnly]
        public ActionResult IntegrationHeaderPartialView()
        {
            // パーシャル ビューを返却
            return PartialView("_IntegrationHeaderPartialView");
        }

        /// <summary>
        /// 「ヘッダー」パーシャル ビューの表示要求を処理します。
        /// </summary>
        /// <returns>
        /// アクションの結果。
        /// </returns>
        [ChildActionOnly]
        [OutputCache(Duration = 600)]
        public ActionResult IntegrationFooterPartialView()
        {
            // パーシャル ビューを返却
            return PartialView("_IntegrationFooterPartialView");
        }

        #endregion
    }

}
