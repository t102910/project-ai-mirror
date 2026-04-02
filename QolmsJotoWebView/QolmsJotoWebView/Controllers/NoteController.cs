using MGF.QOLMS.QolmsApiCoreV1;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using MGF.QOLMS.QolmsJotoWebView.Repositories;
using MGF.QOLMS.QolmsJotoWebView.Worker;
using MGF.QOLMS.QolmsCryptV1;
using System.Text;
using System.IO;

namespace MGF.QOLMS.QolmsJotoWebView
{
    public class NoteController : QjMvcControllerBase
    {

        #region Private Method

        /// <summary>
        /// 暗号化された JSON 形式のパラメータを復号化します。
        /// </summary>
        /// <typeparam name="T">復号化するパラメータクラスの型。</typeparam>
        /// <param name="reference">暗号化された JSON 形式のパラメータ。</param>
        /// <returns>
        /// 復号化されたパラメータクラスのインスタンス。
        /// </returns>
        /// <remarks></remarks>
        private T DecryptReference<T>(string reference) where T : QjJsonParameterBase
        {
            T result = null;

            try
            {
                string jsonString = string.Empty;

                using (QsCrypt crypt = new QsCrypt(QsCryptTypeEnum.QolmsWeb))
                {
                    jsonString = crypt.DecryptString(reference);
                }

                result = QjJsonParameterBase.FromJsonString<T>(jsonString);
            }
            catch
            {
            }

            return result;
        }

        #endregion


        #region "ページ ビュー アクション"

        #region 健診結果画面

        [HttpGet]
        [QjAuthorize]
        [QjApiAuthorize]
        [QjLogging]
        public ActionResult Examination()
        {
            var worker = new NoteExaminationWorker(new ExaminationRepository());
            var mainModel = this.GetQolmsJotoModel();

            var viewModel = worker.CreateViewModel(mainModel);
            mainModel.SetInputModelCache(viewModel);

            return View(viewModel);
        }


        [HttpPost]
        [QjAjaxOnly]
        [QjAuthorize(true)]
        [ValidateAntiForgeryToken(Order = int.MaxValue)]
        [QyApiAuthorize]
        [QjApiAuthorize]
        [QjActionMethodSelector("HealthAge")]
        [QjLogging]
        public ActionResult ExaminationResult(string reference)
        {
            var helthAgeWorker = new HealthAgeEditWorker(new HealthAgeRepository(), new JmdcHealthAgeApiRepository());
            var examinationWorker = new NoteExaminationWorker(new ExaminationRepository());

            var mainModel = this.GetQolmsJotoModel();
            var message = new Dictionary<string, string>();

            var result = false;

            if (!string.IsNullOrWhiteSpace(reference))
            {
                try
                {
                    var paramater = this.DecryptReference<NoteExaminationHelthAgeJsonParamater>(reference);

                    var healthAgeEditInputModelN = examinationWorker.CreateHealthAgeEditInputModel(mainModel, paramater);

                    foreach (var item in healthAgeEditInputModelN)
                    {
                        var valid = helthAgeWorker.ValidateByInputModelByExamination(item).ToList();

                        if (!valid.Any()) // モデルの検証成功
                        {
                            // 健康年齢の測定
                            var errorMessage = new StringBuilder();

                            // 健診受信日の時点での年齢が 18 歳以上かつ 74 歳以下かチェック
                            errorMessage.Append(HealthAgeEditWorker.CheckRecordDate(item.AuthorBirthday, item.RecordDate));

                            if (errorMessage.Length == 0)
                            {
                                // 健康年齢 Web API を実行
                                var responseN = helthAgeWorker.ExecuteJmdcHealthAgeApi(mainModel, item);

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
                                    if (helthAgeWorker.Edit(mainModel, item, responseN))
                                    {
                                        // 成功
                                        message.Add(item.RecordDate.ToString("yyyy/MM/dd"), "成功");
                                        // 一つでも成功したら成功を返却
                                        result = true;
                                    }
                                }
                                else
                                {
                                    // 健康年齢 Web APIエラー
                                    AccessLogWorker.WriteErrorLog(
                                        this.HttpContext,
                                        string.Empty,
                                        errorMessage.ToString()
                                    );

                                    message.Add(
                                        item.RecordDate.ToString("yyyy/MM/dd"),
                                        "健康年齢を測定出来ませんでした。健康年齢WEBAPIがメンテナンス中の可能性があります。"
                                    );
                                }
                            }
                            else
                            {
                                // 年齢チェックエラー
                                message.Add(item.RecordDate.ToString("yyyy/MM/dd"), errorMessage.ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 暫定ログ書き込み
                    message.Add("Exception", ex.Message);
                    AccessLogWorker.WriteErrorLog(
                        this.HttpContext,
                        string.Empty,
                        string.Format("{0}:{1}", "ExaminationResult", ex.Message)
                    );
                }
            }

            return new NoteExaminationJsonResult()
            {
                IsSuccess = result ? bool.TrueString : bool.FalseString,
                Messages = message
            }.ToJsonResult();
        }

        /// <summary>
        /// 「健診結果」画面の PDF ファイル の取得要求を処理します。
        /// </summary>
        /// <param name="reference">
        /// 取得対象の ファイル 情報を表す、
        /// 暗号化された JSON 文字列（<see cref="AssociatedFileStorageReferenceJsonParameter" /> クラス）。
        /// </param>
        /// <returns></returns>
        [HttpGet]
        [QyApiAuthorize]
        [QjApiAuthorize]
        [QjAuthorize]
        [OutputCache(CacheProfile = "DisableCacheProfile")]
        [QjLogging]
        public ActionResult ExaminationPdf(string reference)
        {
            var worker = new NoteExaminationWorker(new ExaminationRepository());

            // ファイル 情報を復号化
            var jsonObject = this.DecryptReference<AssociatedFileStorageReferenceJsonParameter>(reference);

            var mainModel = this.GetQolmsJotoModel();

            if (jsonObject != null)
            {
                var accountkey = jsonObject.Accountkey.TryToValueType(Guid.Empty);
                var loginAt = jsonObject.LoginAt.TryToValueType(DateTime.MinValue);
                var linkageSystemNo = jsonObject.LinkageSystemNo.TryToValueType(int.MinValue);
                var linkageSystemId = jsonObject.LinkageSystemId;
                var recordDate = jsonObject.RecordDate.TryToValueType(DateTime.MinValue);
                var facilityKey = jsonObject.FacilityKey.TryToValueType(Guid.Empty);
                var dataKey = jsonObject.DataKey.TryToValueType(Guid.Empty);

                if (accountkey == mainModel.AuthorAccount.AccountKey &&
                    loginAt.ToString("yyyyMMddhhmmss") == mainModel.AuthorAccount.LoginAt.ToString("yyyyMMddhhmmss") &&
                    linkageSystemNo != int.MinValue &&
                    !string.IsNullOrWhiteSpace(linkageSystemId) &&
                    recordDate != DateTime.MinValue &&
                    facilityKey != Guid.Empty &&
                    dataKey != Guid.Empty)
                {
                    try
                    {
                        var originalName = string.Empty;
                        var contentType = string.Empty;

                        AccessLogWorker.WriteAccessLog(
                            null,
                            "/Examination/ExaminationPdf",
                            AccessLogWorker.AccessTypeEnum.None,
                            "ファイル取得"
                        );

                        // ファイル を返却（ダウンロード として扱う）
                        return new FileContentResult(
                            worker.GetPdfFile(
                                mainModel,
                                dataKey,
                                linkageSystemNo,
                                linkageSystemId,
                                recordDate,
                                facilityKey,
                                ref originalName,
                                ref contentType
                            ),
                            contentType
                        )
                        {
                            FileDownloadName = string.Format(
                                "{0:yyyyMMddHHmmssfffffff}{1}",
                                DateTime.Now,
                                Path.GetExtension(originalName)
                            )
                        };
                    }
                    catch (Exception ex)
                    {
                        // ログまたはエラー処理
                    }
                }
            }

            return new MessageJsonResult()
            {
                IsSuccess = bool.FalseString,
                Message = "ファイルの取得に失敗しました。"
            }.ToJsonResult(true);
        }

        /// <summary>
        /// 「検査手帳」画面の検査結果表の表示要求を処理します。
        /// </summary>
        /// <param name="narrowInAbnormal">基準範囲外の結果のみへ絞り込むかのフラグ。</param>
        /// <returns>
        /// 成功ならパーシャルビュー、
        /// 失敗ならJSON形式のコンテンツ。
        /// </returns>
        [HttpPost]
        [QjAjaxOnly]
        [QjAuthorize]
        [ValidateAntiForgeryToken(Order = int.MaxValue)]
        [QjActionMethodSelector("Narrow")]
        [QjApiAuthorize]
        public ActionResult ExaminationResult(string[] narrowInFacility, string[] narrowInGroup, bool? narrowInAbnormal)
        {
            bool boolValue = narrowInAbnormal.HasValue ? narrowInAbnormal.Value : false;

            // 再展開用のモデルをセッションから取得
            QolmsJotoModel mainModel = this.GetQolmsJotoModel();
            NoteExaminationViewModel viewModel = null;

            string[] narrowInFacilityList = null;
            if (narrowInFacility != null)
            {
                narrowInFacilityList = narrowInFacility;
            }

            string[] narrowInGroupList = null;
            if (narrowInGroup != null)
            {
                narrowInGroupList = narrowInGroup;
            }

            // 編集対象のモデルをキャッシュから取得
            viewModel = mainModel.GetInputModelCache<NoteExaminationViewModel>();

            if (viewModel != null)
            {
                // 成功
                // ViewModelの中の、ExaminationSetNから、表示対象となる病院の検査結果をリストアップ
                var selectedExaminationList = new List<ExaminationSetItem>();

                if (narrowInFacilityList != null && narrowInFacilityList.Any())
                {
                    foreach (ExaminationSetItem eitem in viewModel.ExaminationSetN)
                    {
                        foreach (string fitem in narrowInFacilityList)
                        {
                            if (Guid.Parse(eitem.OrganizationKey) == Guid.Parse(fitem))
                            {
                                selectedExaminationList.Add(eitem);
                            }
                        }
                    }
                }
                else
                {
                    selectedExaminationList = viewModel.ExaminationSetN;
                }

                // ViewModelの中の、ExaminationGroupNから、表示対象となる検査グループをリストアップ
                var selectedGroupList = new List<ExaminationGroupItem>();

                if (narrowInGroupList != null && narrowInGroupList.Any())
                {
                    foreach (ExaminationGroupItem gitem in viewModel.ExaminationGroupN)
                    {
                        foreach (string gritem in narrowInGroupList)
                        {
                            if (gitem.GroupNo.ToString() == gritem)
                            {
                                selectedGroupList.Add(gitem);
                            }
                        }
                    }
                }
                else
                {
                    selectedGroupList = viewModel.ExaminationGroupN;
                }

                // 表（ExaminationMatrix）を再作成
                viewModel.MatrixN = NoteExaminationWorker.CreateExaminationMatrixFromItems(
                    mainModel,
                    selectedExaminationList,
                    selectedGroupList
                );

                viewModel.NarrowInAbnormal = boolValue;

                // キャッシュ内のモデルを更新
                mainModel.SetInputModelCache(viewModel);

                // パーシャルビューを返却
                return PartialView(
                    "_NoteExaminationResultPartialView",
                    new NoteExaminationResultPartialViewModel(viewModel)
                );
            }

            // 失敗ならJSONを返却
            return new NoteExaminationJsonResult
            {
                IsSuccess = bool.FalseString
            }.ToJsonResult();
        }

        #endregion

        #region 「カロミル」画面

        /// <summary>
        /// 「カロミル」画面の表示要求を処理します。
        /// </summary>
        /// <returns>
        /// アクション の結果。
        /// </returns>
        [HttpGet]
        [QjAuthorize]
        [QjApiAuthorize]
        [QyApiAuthorize]
        [QjLogging]
        public ActionResult Calomeal(string meal = "", string selectdate = "")
        {
            var worker = new NoteCalomealWorker(new CalomealRepository(), new CalomealWebViewApiRepository(), new VitalRepository());
            var mainModel = this.GetQolmsJotoModel();

            Guid? challengekey = Guid.Empty;
            int linkage = int.MinValue;

            if (worker.IsCallDynamicLink(mainModel, ref challengekey, ref linkage))
            {
                return RedirectToAction("DynamicLink", new { challengekey = challengekey, linkage = linkage });
            }
            else
            {

                var url = worker.CreateWebViewUrl(mainModel, meal, selectdate);
                return Redirect(url);
            }
        }

        /// <summary>
        /// 「カロミル」画面の表示要求を処理します。
        /// </summary>
        /// <returns>
        /// アクション の結果。
        /// </returns>
        [HttpGet]
        [QjAuthorize]
        [QjApiAuthorize]
        [QyApiAuthorize]
        [QjLogging]
        public ActionResult CalomealResult(string result, string state, string code, string error)
        {
            var worker = new NoteCalomealWorker(new CalomealRepository(), new CalomealWebViewApiRepository(), new VitalRepository());
            //旧JOTOのリダイレクトも受取る必要がある。
            //一旦Stateでわけとく
            var oldstate = Guid.Parse("2fe1980c-66a9-48cc-b8b0-36eadfb3ec3d").ToString("N");
            var newstate = Guid.Parse("B5519517-D92F-4F0A-A714-0ADE2333BCFB").ToString("N");

            if (state == newstate)
            {
                var token = worker.GetToken(this.GetQolmsJotoModel(), code);
                return RedirectToAction("Calomeal");
            }
            else
            {
                return Redirect($"https://devjoto.qolms.com/note/calomealresult?result={result}&state={state}&code={code}&error={error}");
            }
        }

        ///// <summary>
        ///// 「カロミル」画面の表示要求を処理します。
        ///// </summary>
        ///// <returns>
        ///// アクション の結果。
        ///// </returns>
        //[HttpGet]
        //[QjAuthorize]
        //[QjApiAuthorize]
        //[QjLogging]
        //public ActionResult CalomealError()
        //{
        //    return RedirectToAction("home", "portal");
        //}

        /// <summary>
        /// 「カロミル」画面の表示要求を処理します。
        /// </summary>
        /// <returns>
        /// アクション の結果。
        /// </returns>
        [HttpGet]
        [QjAuthorize]
        [QjApiAuthorize]
        [QyApiAuthorize]
        [QjLogging]
        public ActionResult CalomealErrorResult(string page_id, string status_code, string error, string retry)
        {
            var worker = new NoteCalomealWorker(new CalomealRepository(), new CalomealWebViewApiRepository(), new VitalRepository());

            var oldstate = Guid.Parse("2fe1980c-66a9-48cc-b8b0-36eadfb3ec3d").ToString("N");
            var newstate = Guid.Parse("B5519517-D92F-4F0A-A714-0ADE2333BCFB").ToString("N");
            if (newstate == status_code)
            {
                if (status_code == "200" && retry == "1")
                {
                    worker.RefreshToken(this.GetQolmsJotoModel());
                    return RedirectToAction("calomeal");
                }

                if (status_code == "400" && error == "Unset profile.")
                {
                    // page_id=meal,status_code=400,error=Unset profile.
                    worker.SetProfile(this.GetQolmsJotoModel());
                    return RedirectToAction("calomeal");
                }

                if (!string.IsNullOrWhiteSpace(error))
                {
                    this.SetErrorMessage(error);
                }

                throw new InvalidOperationException(
                    string.Format("カロミルエラーです。page_id={0},status_code={1},error={2}",
                        page_id, status_code, error));
            }
            else
            {
                return Redirect($"https://devjoto.qolms.com/note/calomealerrorresult?page_id={page_id}&status_code={status_code}&error={error}&retry={retry}");

            }

        }

        ///// <summary>
        ///// 「カロミル履歴の同期（検証）」
        ///// </summary>
        ///// <returns>
        ///// アクション の結果。
        ///// </returns>
        //[HttpGet]
        //[QjAuthorize]
        //[QjApiAuthorize]
        //[QjLogging]
        //public ActionResult CalomealHistory()
        //{
        //    NoteCalomealWorker.History(this.GetQolmsJotoModel());
        //    return View();
        //}

        //[HttpGet]
        //[QjAuthorize]
        //[QjApiAuthorize]
        //[QjLogging]
        //public ActionResult DynamicLink(Guid? challengekey, int linkage)
        //{
        //    return Redirect(NoteCalomealWorker.DynamicLink(this.GetQolmsJotoModel(), challengekey, linkage));
        //}

        //[HttpGet]
        //[QjAuthorize]
        //[QjApiAuthorize]
        //[QjLogging]
        //public ActionResult CalomealJwtDynamicLink(string jwt)
        //{
        //    // CalomealWebViewWorker.DebugLog(jwt);

        //    return Redirect(NoteCalomealWorker.DynamicLink(this.GetQolmsJotoModel(), jwt));
        //}

        //[HttpGet]
        //[QjAuthorize]
        //[QjApiAuthorize]
        //[QjLogging]
        //public ActionResult CalomealHokenshido()
        //{
        //    return Redirect(NoteCalomealWorker.Hokenshido(this.GetQolmsJotoModel()));
        //}

        #endregion

        #region 問診システム画面

        [HttpGet]
        [QjAuthorize]
        [QjApiAuthorize]
        [QjLogging]
        public RedirectResult Monshin()
        {
            var mainModel = this.GetQolmsJotoModel();
            var url = NoteMonshinWorker.CreateRedirectUrl(mainModel);
            return Redirect(url);
        }
        #endregion

        #region ガルフスポーツ動画画面

        /// <summary>
        /// 「ガルフスポーツ動画」TOP 画面の表示要求を処理します。
        /// </summary>
        /// <returns>
        /// アクションの結果。
        /// </returns>
        [HttpGet]
        [QjAuthorize]
        [QjApiAuthorize]
        [QjLogging]
        public ActionResult GulfSportsMovieIndex()
        {
            // ビューを返却
            return View();
        }

        /// <summary>
        /// 「ガルフスポーツ動画」詳細画面の表示要求を処理します。
        /// </summary>
        /// <param name="movieType">動画の種別（1〜3）。</param>
        /// <returns>
        /// アクションの結果。
        /// </returns>
        [HttpGet]
        [QjAuthorize]
        [QjApiAuthorize]
        [QjLogging]
        public ActionResult GulfSportsMovie(string movieType)
        {
            byte movieTypeValue;
            if (!byte.TryParse(movieType, out movieTypeValue) || movieTypeValue < 1 || movieTypeValue > 3)
            {
                return RedirectToAction("GulfSportsMovieIndex");
            }

            // ダミーデータ（モック用）
            var viewModel = new NoteGulfSportsMovieViewModel()
            {
                MovieType = movieTypeValue,
                MovieItemN = new System.Collections.Generic.List<GulfSportsMovieItem>()
                {
                    new GulfSportsMovieItem() { Id = "dQw4w9WgXcQ", ExerciseType = 1, Calorie = 150, Description = "ゴルフ スイング 基礎トレーニング", Time = "10:30" },
                    new GulfSportsMovieItem() { Id = "aBcDeFgHiJk", ExerciseType = 2, Calorie = 200, Description = "体幹強化ストレッチ", Time = "08:15" },
                    new GulfSportsMovieItem() { Id = "lMnOpQrStUv", ExerciseType = 3, Calorie = 120, Description = "肩・腕のウォームアップ", Time = "05:45" },
                }
            };

            // ビューを返却
            return View(viewModel);
        }

        #endregion

        #endregion

        #region 共通 パーツ

        /// <summary>
        /// 「ヘッダー」パーシャル ビュー の表示要求を処理します。
        /// </summary>
        /// <returns>
        /// アクション の結果。
        /// </returns>
        [ChildActionOnly]
        public ActionResult NoteHeaderPartialView()
        {
            // パーシャル ビュー を返却
            return PartialView("_NoteHeaderPartialView");
        }

        /// <summary>
        /// 「フッター」パーシャル ビュー の表示要求を処理します。
        /// </summary>
        /// <returns>
        /// アクション の結果。
        /// </returns>
        [ChildActionOnly]
        [OutputCache(Duration = 600)]
        public ActionResult NoteFooterPartialView()
        {
            // パーシャル ビュー を返却
            return PartialView("_NoteFooterPartialView");
        }

        #endregion

        #region 「健診結果」画面用パーシャルビュー

        /// <summary>
        /// 「健診結果」画面
        /// 健診結果表パーシャルビューの表示要求を処理します。
        /// </summary>
        /// <returns>
        /// アクションの結果。
        /// </returns>
        [ChildActionOnly]
        public ActionResult NoteExaminationResultPartialView()
        {
            NoteExaminationResultPartialViewModel model = null;

            try
            {
                model = new NoteExaminationResultPartialViewModel(this.GetPageViewModel<NoteExaminationViewModel>());
            }
            catch
            {
            }

            // パーシャルビューを返却
            if (model != null)
            {
                return PartialView("_NoteExaminationResultPartialView", model);
            }

            return new EmptyResult();
        }

        /// <summary>
        /// 「健診結果」画面
        /// 「絞り込み」ボックスパーシャルビューの表示要求を処理します。
        /// </summary>
        /// <returns>
        /// アクションの結果。
        /// </returns>
        [ChildActionOnly]
        public ActionResult NoteExaminationFilterPartialView()
        {
            NoteExaminationResultPartialViewModel model = null;

            try
            {
                model = new NoteExaminationResultPartialViewModel(this.GetPageViewModel<NoteExaminationViewModel>());
            }
            catch
            {
            }

            // パーシャルビューを返却
            if (model != null)
            {
                return PartialView("_NoteExaminationFilterPartialView", model);
            }

            return new EmptyResult();
        }

        #endregion
    }
}
