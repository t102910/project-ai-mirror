using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsJotoWebView;
using MGF.QOLMS.QolmsJotoWebView.Models;
using MGF.QOLMS.QolmsJotoWebView.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MGF.QOLMS.QolmsJotoWebView.Worker
{
    /// <summary>
    /// 「カロミル」画面に関する機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    public sealed class NoteCalomealWorker
    {

        #region "Repository"

        ICalomealRepository _calomealRepository;
        ICalomealWebViewApiRepository _calomealApiRepository;
        IVitalRepository _vitalRepository;

        #endregion

        #region "Constructor"

        /// <summary>
        /// <see cref="AccessTableEntityWriter" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        public NoteCalomealWorker(ICalomealRepository calomealRepository, ICalomealWebViewApiRepository calomealApiRepository, IVitalRepository vitalRepository)
        {
            _calomealRepository = calomealRepository;
            _calomealApiRepository = calomealApiRepository;
            _vitalRepository = vitalRepository;

        }

        #endregion

        #region Public Property

        public static string LogFilePath { get; set; } = string.Empty;

        #endregion

        #region Constant

        /// <summary>
        /// カロミル API URI
        /// </summary>
        public static readonly string REQUEST_URI = QjConfiguration.CalomealApiUri;

        /// <summary>
        /// 竹富町 JWT の設定
        /// </summary>
        public static readonly string TAKETOMI_JWT = QjConfiguration.CalomealWebViewTaketomiJwt;

        /// <summary>
        /// 伊平屋村 JWT の設定
        /// </summary>
        public static readonly string IHEYA_JWT = QjConfiguration.CalomealWebViewIheyaJwt;

        /// <summary>
        /// 沖縄セルラー JWT の設定
        /// </summary>
        public static readonly string OCT_JWT = QjConfiguration.CalomealWebViewOctJwt;


        /// <summary>
        /// カロミル 連携システム番号
        /// </summary>
        public static readonly int CALOMEAL_LINKAGESYSTEMNO = 47015;


        /// <summary>
        /// カロミル ログ のファイル名です
        /// </summary>
        private const string CALOMEAL_LOG_FILENAME = "Calomeal.Log";

        /// <summary>
        /// カロミル トークンAPI呼び出しログ のファイル名です
        /// </summary>
        private const string CALOMEAL_TOKENLOG_FILENAME = "CalomealToken.Log";

        /// <summary>
        /// カロミル 食事API呼び出しログ のファイル名です
        /// </summary>
        private const string CALOMEAL_MEALLOG_FILENAME = "CalomealMeal.Log";

        #endregion

        #region Constructor

        /// <summary>
        /// デフォルトコンストラクタは使用できません。
        /// </summary>
        private NoteCalomealWorker()
        {
        }

        #endregion

        #region Private Method

        //private static QhYappliPortalCalomealConnectionWriteApiResults ExecuteCalomealConnectionWriteApi(
        //    QolmsJotoModel mainModel,
        //    string linkgaSystemId,
        //    CalomealAccessTokenSet token,
        //    bool deleteFlag)
        //{
        //    var apiArgs = new QhYappliPortalCalomealConnectionWriteApiArgs(
        //        QhApiTypeEnum.YappliPortalCalomealConnectionWrite,
        //        QsApiSystemTypeEnum.Qolms,
        //        mainModel.ApiExecutor,
        //        mainModel.ApiExecutorName)
        //    {
        //        Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
        //        LinkageSystemNo = CALOMEAL_LINKAGESYSTEMNO.ToString(),
        //        LinkageSystemId = linkgaSystemId,
        //        DeleteFlag = deleteFlag.ToString(),
        //        TokenSet = new QhApiCalomealTokenSetItem()
        //        {
        //            Token = token.access_token,
        //            TokenExpires = token.TokenExpires.ToApiDateString(),
        //            RefreshToken = token.refresh_token,
        //            RefreshTokenExpires = token.RefreshTokenExpires.ToApiDateString()
        //        }
        //    };

        //    var apiResults = QsApiManager.ExecuteQolmsApi<QhYappliPortalCalomealConnectionWriteApiResults>(
        //        apiArgs,
        //        mainModel.SessionId,
        //        mainModel.ApiAuthorizeKey);

        //    if (apiResults.IsSuccess.TryToValueType(false))
        //    {
        //        return apiResults;
        //    }
        //    else
        //    {
        //        throw new InvalidOperationException(
        //            string.Format("{0} API の実行に失敗しました。",
        //                QsApiManager.GetQolmsApiName(apiArgs)));
        //    }
        //}

        //private static QhYappliPortalCalomealConnectionTokenReadApiResults ExecuteCalomealConnectionTokenReadApi(
        //    QolmsJotoModel mainModel)
        //{
        //    var apiArgs = new QhYappliPortalCalomealConnectionTokenReadApiArgs(
        //        QhApiTypeEnum.YappliPortalCalomealConnectionTokenRead,
        //        QsApiSystemTypeEnum.Qolms,
        //        mainModel.ApiExecutor,
        //        mainModel.ApiExecutorName)
        //    {
        //        Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
        //        LinkageSystemNo = CALOMEAL_LINKAGESYSTEMNO.ToString()
        //    };

        //    var apiResults = QsApiManager.ExecuteQolmsApi<QhYappliPortalCalomealConnectionTokenReadApiResults>(
        //        apiArgs,
        //        mainModel.SessionId,
        //        mainModel.ApiAuthorizeKey);

        //    if (apiResults.IsSuccess.TryToValueType(false))
        //    {
        //        return apiResults;
        //    }
        //    else
        //    {
        //        throw new InvalidOperationException(
        //            string.Format("{0} API の実行に失敗しました。",
        //                QsApiManager.GetQolmsApiName(apiArgs)));
        //    }
        //}

        //private static QhYappliNoteCalomealMealSyncReadApiResults ExecuteCalomealMealSyncRead(
        //    QolmsJotoModel mainModel)
        //{
        //    var apiArgs = new QhYappliNoteCalomealMealSyncReadApiArgs(
        //        QhApiTypeEnum.YappliNoteCalomealMealSyncRead,
        //        QsApiSystemTypeEnum.Qolms,
        //        mainModel.ApiExecutor,
        //        mainModel.ApiExecutorName)
        //    {
        //        ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
        //        LinkageSystemNo = CALOMEAL_LINKAGESYSTEMNO.ToString()
        //    };

        //    var apiResults = QsApiManager.ExecuteQolmsApi<QhYappliNoteCalomealMealSyncReadApiResults>(
        //        apiArgs,
        //        mainModel.SessionId,
        //        mainModel.ApiAuthorizeKey);

        //    if (apiResults.IsSuccess.TryToValueType(false))
        //    {
        //        return apiResults;
        //    }
        //    else
        //    {
        //        throw new InvalidOperationException(
        //            string.Format("{0} API の実行に失敗しました。",
        //                QsApiManager.GetQolmsApiName(apiArgs)));
        //    }
        //}

        //private static QhYappliNoteCalomealMealSyncWriteApiResults ExecuteCalomealMealSyncWrite(
        //    QolmsJotoModel mainModel,
        //    List<NoteMeal2InputModel> model)
        //{
        //    var apiArgs = new QhYappliNoteCalomealMealSyncWriteApiArgs(
        //        QhApiTypeEnum.YappliNoteCalomealMealSyncWrite,
        //        QsApiSystemTypeEnum.Qolms,
        //        mainModel.ApiExecutor,
        //        mainModel.ApiExecutorName)
        //    {
        //        ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
        //        MealItemN = model.ConvertAll(i => new QhApiCalomealMealSyncItem()
        //        {
        //            AnalysisSet = i.AnalysisSet,
        //            AnalysisType = i.AnalysisType.ToString(),
        //            Calorie = i.Calorie.ToString(),
        //            ChooseSet = i.ChooseSet,
        //            DeleteFlag = i.DeleteFlag.ToString(),
        //            HistoryId = i.HistoryId.ToString(),
        //            ItemName = i.ItemName.ToString(),
        //            MealType = i.MealType.ToString(),
        //            Rate = i.Rate.ToString(),
        //            RecordDate = i.RecordDate.ToApiDateString(),
        //            HasImage = i.HasImage.ToString()
        //        })
        //    };

        //    var apiResults = QsApiManager.ExecuteQolmsApi<QhYappliNoteCalomealMealSyncWriteApiResults>(
        //        apiArgs,
        //        mainModel.SessionId,
        //        mainModel.ApiAuthorizeKey);

        //    if (apiResults.IsSuccess.TryToValueType(false))
        //    {
        //        return apiResults;
        //    }
        //    else
        //    {
        //        throw new InvalidOperationException(
        //            string.Format("{0} API の実行に失敗しました。",
        //                QsApiManager.GetQolmsApiName(apiArgs)));
        //    }
        //}

        ///// <summary>
        ///// 日付入りのファイルパスをセットします
        ///// </summary>
        //private static void GetLogFilePath(string path, string name)
        //{
        //    // ログパスをセット
        //    NoteCalomealWorker.LogFilePath = System.IO.Path.Combine(path, $"{DateTime.Today:yyyyMMdd}_{name}");
        //    CalomealWebViewWorker.LogFilePath = NoteCalomealWorker.LogFilePath;
        //}

        #endregion

        #region Public Method

        public string GetToken(QolmsJotoModel mainModel, string code)
        {
            //// ログパスをセット
            //NoteCalomealWorker.GetLogFilePath(mainModel.DebugLogPath, CALOMEAL_TOKENLOG_FILENAME);

            // Codeから取得
            var tokenSet = _calomealApiRepository.GetNewToken(code);

            if (!string.IsNullOrWhiteSpace(tokenSet.access_token))
            {
                var str = tokenSet.access_token.Split(new char[] { '.' });
                var jwt = str.GetValue(1).ToString();

                // 文字列の長さが4の倍数じゃないとBase64エンコードで引っかかる
                if (jwt.Length % 4 != 0)
                {
                    int m = jwt.Length % 4;
                    // 足りない分を空白"="で埋める
                    for (int index = 1; index <= 4 - m; index++)
                    {
                        jwt += "=";
                    }
                }

                var enc = new UTF8Encoding();
                var json = enc.GetString(Convert.FromBase64String(jwt));

                var sr = new QsJsonSerializer();
                var c = sr.Deserialize<CalomealUser>(json);

                // DBに登録
                var apiresult = _calomealRepository.ExecuteCalomealConnectionWriteApi(mainModel,CALOMEAL_LINKAGESYSTEMNO, c.sub, tokenSet, false);

                var weight = decimal.Zero;
                // 食事ボタンが押せる時点で体重の登録はあるはず
                if (apiresult.Height.TryToValueType(decimal.MinValue) > 0 && apiresult.Weight.TryToValueType(decimal.MinValue) > 0)
                {
                    mainModel.AuthorAccount.Height = apiresult.Height.TryToValueType(decimal.MinValue);
                    weight = apiresult.Weight.TryToValueType(decimal.MinValue);
                }
                else
                {
                    throw new InvalidOperationException("基本情報の登録がありません。");
                }

                // 暫定プロフィールないと表示できないので更新しておく
                if (!_calomealApiRepository.SetProfile(tokenSet.access_token, mainModel.AuthorAccount.Birthday,
                    mainModel.AuthorAccount.Height, weight, mainModel.AuthorAccount.SexType))
                {
                    // error
                    AccessLogWorker.WriteErrorLog(null, "api_setprofile_error",
                        new InvalidOperationException(
                            string.Format("カロミルプロフィールの登録に失敗しました。accountkey={0}",
                                mainModel.AuthorAccount.AccountKey)));

                    throw new InvalidOperationException("カロミルプロフィールの登録に失敗しました。");
                }
            }
            else
            {
                // 取れなかった場合
                // error
                AccessLogWorker.WriteErrorLog(null, "api_accesstoken_error",
                    new InvalidOperationException(
                        string.Format("カロミルトークンの取得に失敗しました。accountkey={0}",
                            mainModel.AuthorAccount.AccountKey)));
            }

            return tokenSet.access_token;
        }

        public bool SetProfile(QolmsJotoModel mainModel)
        {
            decimal refHeight = decimal.MinValue;
            decimal refWeight = decimal.MinValue;

            _vitalRepository.GetLatestHeightAndWeight(mainModel.ApiExecutor, mainModel.ApiExecutorName,
                                                      mainModel.SessionId, mainModel.ApiAuthorizeKey2,
                                                      mainModel.AuthorAccount.AccountKey, ref refHeight, ref refWeight);

            if (!_calomealApiRepository.SetProfile(TokenRead(mainModel), mainModel.AuthorAccount.Birthday,
                refHeight, refWeight, mainModel.AuthorAccount.SexType))
            {
                // error
                AccessLogWorker.WriteErrorLog(null, "api_setprofile_error",
                    new InvalidOperationException(
                        string.Format("カロミルプロフィールの登録に失敗しました。accountkey={0}",
                            mainModel.AuthorAccount.AccountKey)));

                return false;
            }
                
            return true;
        }

        public string TokenRead(QolmsJotoModel mainModel)
        {
            //// ログパスをセット
            //NoteCalomealWorker.GetLogFilePath(mainModel.DebugLogPath, CALOMEAL_TOKENLOG_FILENAME);

            var result = _calomealRepository.ExecuteCalomealConnectionTokenReadApi(mainModel,CALOMEAL_LINKAGESYSTEMNO);

            if (string.IsNullOrWhiteSpace(result.TokenSet.Token))
            {
                return string.Empty;
            }
            else if (result.TokenSet.TokenExpires.TryToValueType(DateTime.MinValue) > DateTime.Now)
            {
                return result.TokenSet.Token;
            }
            else
            {
                var tokenSet = _calomealApiRepository.GetRefreshToken(result.TokenSet.RefreshToken);

                if (!string.IsNullOrWhiteSpace(tokenSet.access_token))
                {
                    var str = tokenSet.access_token.Split(new char[] { '.' });
                    var jwt = str.GetValue(1).ToString();

                    // 文字列の長さが4の倍数じゃないとBase64エンコードで引っかかる
                    if (jwt.Length % 4 != 0)
                    {
                        int m = jwt.Length % 4;
                        // 足りない分を空白"="で埋める
                        for (int index = 1; index <= 4 - m; index++)
                        {
                            jwt += "=";
                        }
                    }

                    var enc = new UTF8Encoding();
                    var json = enc.GetString(Convert.FromBase64String(jwt));

                    var sr = new QsJsonSerializer();
                    var c = sr.Deserialize<CalomealUser>(json);

                    var apiresult = _calomealRepository.ExecuteCalomealConnectionWriteApi(mainModel,CALOMEAL_LINKAGESYSTEMNO, c.sub, tokenSet, false);

                    if (apiresult.Height.TryToValueType(decimal.MinValue) > 0)
                    {
                        // 一応更新しておく
                        mainModel.AuthorAccount.Height = apiresult.Height.TryToValueType(decimal.MinValue);
                    }

                    var weight = decimal.Zero;
                    // 食事ボタンが押せる時点で体重の登録はあるはず
                    if (apiresult.Height.TryToValueType(decimal.MinValue) > 0 && apiresult.Weight.TryToValueType(decimal.MinValue) > 0)
                    {
                        mainModel.AuthorAccount.Height = apiresult.Height.TryToValueType(decimal.MinValue);
                        weight = apiresult.Weight.TryToValueType(decimal.MinValue);
                    }
                    else
                    {
                        throw new InvalidOperationException("基本情報の登録がありません。");
                    }

                    // 暫定プロフィールないと表示できないので更新しておく
                    if (!_calomealApiRepository.SetProfile(tokenSet.access_token, mainModel.AuthorAccount.Birthday,
                        mainModel.AuthorAccount.Height, weight, mainModel.AuthorAccount.SexType))
                    {
                        // error
                        AccessLogWorker.WriteErrorLog(null, "api_setprofile_error",
                            new InvalidOperationException(
                                string.Format("カロミルプロフィールの登録に失敗しました。accountkey={0}",
                                    mainModel.AuthorAccount.AccountKey)));

                        throw new InvalidOperationException("カロミルプロフィールの登録に失敗しました。");
                    }
                }
                else
                {
                    // 取れなかった場合の処理？
                    AccessLogWorker.WriteErrorLog(null, "api_accesstoken_error",
                        new InvalidOperationException(
                            string.Format("カロミルトークンの取得に失敗しました。accountkey={0}",
                                mainModel.AuthorAccount.AccountKey)));
                }

                return tokenSet.access_token;
            }
        }

        public string RefreshToken(QolmsJotoModel mainModel)
        {
            //// ログパスをセット
            //NoteCalomealWorker.GetLogFilePath(mainModel.DebugLogPath, CALOMEAL_TOKENLOG_FILENAME);

            // DebugLog("TokenRead");

            var result = _calomealRepository.ExecuteCalomealConnectionTokenReadApi(mainModel,CALOMEAL_LINKAGESYSTEMNO);

            // DebugLog(result.TokenSet.Token);

            if (!string.IsNullOrWhiteSpace(result.TokenSet.RefreshToken))
            {
                // DebugLog("GetRefreshToken前");

                var tokenSet = _calomealApiRepository.GetRefreshToken(result.TokenSet.RefreshToken);
                // DebugLog("GetRefreshToken後");

                if (!string.IsNullOrWhiteSpace(tokenSet.access_token))
                {
                    var str = tokenSet.access_token.Split(new char[] { '.' });
                    var jwt = str.GetValue(1).ToString();

                    // 文字列の長さが4の倍数じゃないとBase64エンコードで引っかかる
                    if (jwt.Length % 4 != 0)
                    {
                        int m = jwt.Length % 4;
                        // 足りない分を空白"="で埋める
                        for (int index = 1; index <= 4 - m; index++)
                        {
                            jwt += "=";
                        }
                    }

                    var enc = new UTF8Encoding();
                    var json = enc.GetString(Convert.FromBase64String(jwt));

                    var sr = new QsJsonSerializer();
                    var c = sr.Deserialize<CalomealUser>(json);

                    // DebugLog(c.sub);

                    // -------------------
                    var apiresult = _calomealRepository.ExecuteCalomealConnectionWriteApi(mainModel,CALOMEAL_LINKAGESYSTEMNO, c.sub, tokenSet, false);

                    var weight = decimal.Zero;
                    // 食事ボタンが押せる時点で体重の登録はあるはず
                    if (apiresult.Height.TryToValueType(decimal.MinValue) > 0 && apiresult.Weight.TryToValueType(decimal.MinValue) > 0)
                    {
                        mainModel.AuthorAccount.Height = apiresult.Height.TryToValueType(decimal.MinValue);
                        weight = apiresult.Weight.TryToValueType(decimal.MinValue);
                    }
                    else
                    {
                        throw new InvalidOperationException("基本情報の登録がありません。");
                    }

                    // 暫定プロフィールないと表示できないので更新しておく
                    if (!_calomealApiRepository.SetProfile(tokenSet.access_token, mainModel.AuthorAccount.Birthday,
                        mainModel.AuthorAccount.Height, weight, mainModel.AuthorAccount.SexType))
                    {
                        // error
                        AccessLogWorker.WriteErrorLog(null, "api_setprofile_error",
                            new InvalidOperationException(
                                string.Format("カロミルプロフィールの登録に失敗しました。accountkey={0}",
                                    mainModel.AuthorAccount.AccountKey)));

                        throw new InvalidOperationException("カロミルプロフィールの登録に失敗しました。");
                    }

                }
                else
                {
                    // 取れなかった場合の処理？
                    AccessLogWorker.WriteErrorLog(null, "api_accesstoken_error",
                        new InvalidOperationException(
                            string.Format("カロミルトークンの取得に失敗しました。accountkey={0}",
                                mainModel.AuthorAccount.AccountKey)));

                    throw new InvalidOperationException("カロミルトークンの取得に失敗しました。");
                }

                return tokenSet.access_token;
            }

            return string.Empty;
        }

        public static string GetWebViewAuthUrl(string token, DateTime selectDate, byte meal)
        {
            var newstate = Guid.Parse("B5519517-D92F-4F0A-A714-0ADE2333BCFB").ToString("N");
            var rootUri = REQUEST_URI;

            if (!rootUri.EndsWith("/"))
            {
                rootUri += "/";
            }

            var mealStr = string.Empty;
            switch (meal)
            {
                // breakfast:朝食, lunch:昼食, dinner:夕食, snack:間食
                case 1:
                    mealStr = "breakfast";
                    break;
                case 2:
                    mealStr = "lunch";
                    break;
                case 3:
                    mealStr = "dinner";
                    break;
                case 4:
                    mealStr = "snack";
                    break;
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                // Tokenなし
                return string.Format(rootUri + "auth/request?response_type=code&client_id={0}&state={1}&only_create_user=1",
                    QjConfiguration.CalomealApiClientID, newstate);
            }
            else
            {
                // Tokenあり
                if (string.IsNullOrWhiteSpace(mealStr))
                {
                    // 食事ボタンなのでカロミル食事画面TOPへ
                    return string.Format(rootUri + "web/?client_id={0}&access_token={1}&date={2}&meal_type={3}",
                        QjConfiguration.CalomealApiClientID, token, selectDate.ToString("yyyyMMdd"), mealStr);
                }
                else
                {
                    // 目標ボタンなので食事種別を指定して入力画面へ
                    return string.Format(rootUri + "web/meal?client_id={0}&access_token={1}&date={2}&meal_type={3}",
                        QjConfiguration.CalomealApiClientID, token, selectDate.ToString("yyyyMMdd"), mealStr);
                }
            }
        }

        //public bool History(QolmsJotoModel mainModel)
        //{
        //    var updatedDate = DateTime.MinValue;

        //    var token = TokenRead(mainModel);

        //    var result = _calomealRepository.ExecuteCalomealMealSyncRead(mainModel,CALOMEAL_LINKAGESYSTEMNO);
        //    updatedDate = result.LastUpdatedDate.TryToValueType(DateTime.MinValue);

        //    // ログパスをセット
        //    NoteCalomealWorker.GetLogFilePath(mainModel.DebugLogPath, CALOMEAL_MEALLOG_FILENAME);

        //    if (!string.IsNullOrWhiteSpace(token))
        //    {
        //        // トークンがなければ食事の同期はいらない
        //        var mealEvents = CalomealWebViewWorker.GetMeal(updatedDate, token)
        //            .ConvertAll(i => CalomealWebViewWorker.ToMealEvent2(i, mainModel.AuthorAccount.AccountKey));

        //        // 登録できないデータがないか確認する
        //        mealEvents = mealEvents.Where(i => i.HistoryId > 0).ToList();

        //        if (mealEvents.Any())
        //        {
        //            var ret = _calomealRepository.ExecuteCalomealMealSyncWrite(mainModel, mealEvents);

        //            // 登録する
        //            if (ret.IsSuccess.TryToValueType(false))
        //            {
        //                // Todo: 【Must】 ポイント処理
        //                //// 対象のポイントを付与
        //                //// ループせずに一括登録へ変更
        //                //var pointItemN = new List<QolmsPointGrantItem>();
        //                //var hsPointDate = new HashSet<DateTime>(); // 付与対象日が一意になるようにする

        //                //foreach (var item in ret.CanGivePointDateN)
        //                //{
        //                //    var recordDate = item.TryToValueType(DateTime.MinValue);
        //                //    if (recordDate > DateTime.MinValue && hsPointDate.Add(recordDate.Date))
        //                //    {
        //                //        recordDate = recordDate.Date;
        //                //        var limit = new DateTime(recordDate.Year, recordDate.Month, 1)
        //                //            .AddMonths(7).AddDays(-1); // ポイント有効期限は 6 ヶ月後の月末
        //                //        var pointMaxDay = DateTime.Now.Date; // ポイント付与範囲終了日（今日）
        //                //        var pointMinDay = pointMaxDay.AddDays(-6); // ポイント付与範囲開始日（過去 1 週間）
        //                //        var pointType = QyPointItemTypeEnum.None;

        //                //        if (recordDate >= pointMinDay && recordDate <= pointMaxDay)
        //                //        {
        //                //            // 2022/2/28から食事は種別に関わらず1日1ポイントに変更
        //                //            pointType = QyPointItemTypeEnum.Meal;
        //                //            pointItemN.Add(new QolmsPointGrantItem(
        //                //                mainModel.AuthorAccount.MembershipType,
        //                //                DateTime.Now,
        //                //                Guid.NewGuid().ToApiGuidString(),
        //                //                pointType,
        //                //                limit,
        //                //                recordDate));
        //                //        }
        //                //    }
        //                //}

        //                //// 非同期で付与
        //                //Task.Run(() =>
        //                //{
        //                //    QolmsPointWorker.AddQolmsPoints(
        //                //        mainModel.ApiExecutor,
        //                //        mainModel.ApiExecutorName,
        //                //        mainModel.SessionId,
        //                //        mainModel.ApiAuthorizeKey,
        //                //        mainModel.AuthorAccount.AccountKey,
        //                //        pointItemN);
        //                //});
        //            }
        //        }

        //        return true;
        //    }

        //    return false;
        //}

        ///// <summary>
        ///// カロミルWebViewに体重を登録します。
        ///// </summary>
        //public bool SetAnthropometric(QolmsJotoModel mainModel, DateTime targetDate, decimal weight)
        //{
        //    var token = TokenRead(mainModel);

        //    // ログパスをセット
        //    NoteCalomealWorker.GetLogFilePath(mainModel.DebugLogPath, CALOMEAL_MEALLOG_FILENAME);

        //    if (!string.IsNullOrWhiteSpace(token) &&
        //        CalomealWebViewWorker.SetAnthropometric(token, targetDate, weight, 1))
        //    {
        //        return true;
        //    }

        //    return false;
        //}

        ///// <summary>
        ///// カロミルWebViewに目標を登録します。
        ///// </summary>
        //public bool SetGoal(QolmsJotoModel mainModel, DateTime targetDate, decimal weight)
        //{
        //    var token = TokenRead(mainModel);

        //    // ログパスをセット
        //    NoteCalomealWorker.GetLogFilePath(mainModel.DebugLogPath, CALOMEAL_MEALLOG_FILENAME);

        //    if (!string.IsNullOrWhiteSpace(token) &&
        //        CalomealWebViewWorker.SetGoal(token, targetDate, weight))
        //    {
        //        return true;
        //    }

        //    return false;
        //}

        ///// <summary>
        ///// カロミルWebViewから食事と目標情報を取得します。
        ///// </summary>
        //public  List<CalomealGoalSet> GetMealWithBasis(QolmsJotoModel mainModel, DateTime startDate, DateTime endDate)
        //{
        //    var token = TokenRead(mainModel);

        //    // ログパスをセット
        //    NoteCalomealWorker.GetLogFilePath(mainModel.DebugLogPath, CALOMEAL_MEALLOG_FILENAME);

        //    if (!string.IsNullOrWhiteSpace(token))
        //    {
        //        var goalSet = CalomealWebViewWorker.GetMealWithBasis(token, startDate, endDate)
        //            .ConvertAll(i => CalomealWebViewWorker.ToCalomealGoalSet(i));
        //        return goalSet;
        //    }

        //    return new List<CalomealGoalSet>();
        //}

        /// <summary>
        /// カロミルアドバイス同意画面の呼び出しをするかどうか判定します。
        /// </summary>
        public bool IsCallDynamicLink(QolmsJotoModel mainModel, ref Guid? challengekey, ref int linkage)
        {
            // 初期値
            challengekey = null;
            linkage = int.MinValue;

            var token = TokenRead(mainModel);
            //var challenge = PortalChallengeWorker.GetChallengeEntryList(mainModel);
            //var list = PortalConnectionSettiongWorker.GetLinkageLists(mainModel);

            //// ログパスをセット
            //NoteCalomealWorker.GetLogFilePath(mainModel.DebugLogPath, CALOMEAL_LOG_FILENAME);

            //// 全部有効な場合は優先順位が高い方から採用されます。
            //if (list.ContainsKey(47100))
            //{
            //    // 1) 沖縄セルラー
            //    linkage = 47100;
            //}
            //else if (challenge.ContainsKey(Guid.Parse("6550b115-7411-4c0e-9ac3-9121ac7093b1")))
            //{
            //    // 2) 竹富町
            //    challengekey = Guid.Parse("6550b115-7411-4c0e-9ac3-9121ac7093b1");
            //}
            //else if (challenge.ContainsKey(Guid.Parse("a87c8371-e556-4d0f-8cae-c08b8f5a3d2c")))
            //{
            //    // 3) 伊平屋村
            //    challengekey = Guid.Parse("a87c8371-e556-4d0f-8cae-c08b8f5a3d2c");
            //}

            //if (challengekey != Guid.Empty || linkage > 0)
            //{
            //    // チャレンジ参加者だったら
            //    var info = CalomealWebViewWorker.ToCalomealInstructInfo(CalomealWebViewWorker.InstructInfoApi(token));
            //    if (string.IsNullOrWhiteSpace(info.UserName))
            //    {
            //        // まだ同意してなかったら表示
            //        return true;
            //    }
            //}

            return false;
        }

        public string DynamicLink(QolmsJotoModel mainModel, Guid? challengeKey = null, int linkage = int.MinValue)
        {
            var jwt = string.Empty;

            // チャレンジ
            if (challengeKey.HasValue)
            {
                switch (challengeKey.Value)
                {
                    case var val when val == Guid.Parse("6550b115-7411-4c0e-9ac3-9121ac7093b1"):
                        // 竹富町と連携するためのJWT
                        jwt = TAKETOMI_JWT;
                        break;

                    case var val when val == Guid.Parse("a87c8371-e556-4d0f-8cae-c08b8f5a3d2c"):
                        // 伊平屋村
                        jwt = IHEYA_JWT;
                        break;
                }
            }

            // 連携
            switch (linkage)
            {
                case 47100:
                    // 沖縄セルラーと連携するためのJWT
                    jwt = OCT_JWT;
                    break;
            }

            var token = TokenRead(mainModel);
            if (!string.IsNullOrWhiteSpace(token) && !string.IsNullOrWhiteSpace(jwt))
            {
                // 一応確認するけど、ない人は入らないように実装する
                // var state = Guid.Parse("2fe1980c-66a9-48cc-b8b0-36eadfb3ec3d").ToString("N");
                var rootUri = REQUEST_URI;

                if (!rootUri.EndsWith("/"))
                {
                    rootUri += "/";
                }

                var str = string.Format(rootUri + "web/dynamic_link?client_id={0}&access_token={1}&jwt={2}&name={3}",
                    QjConfiguration.CalomealApiClientID, token, jwt, mainModel.AuthorAccount.Name);
                return str;
            }

            return NoteCalomealWorker.GetWebViewAuthUrl(token, DateTime.Today, 0);
        }

        //public string DynamicLink(QolmsJotoModel mainModel, string jwt)
        //{
        //    var token = TokenRead(mainModel);

        //    NoteCalomealWorker.GetLogFilePath(mainModel.DebugLogPath, CALOMEAL_LOG_FILENAME);
        //    CalomealWebViewWorker.FileLog(jwt);

        //    if (!string.IsNullOrWhiteSpace(token) && !string.IsNullOrWhiteSpace(jwt))
        //    {
        //        // 一応確認するけど、ない人は入らないように実装する
        //        // var state = Guid.Parse("2fe1980c-66a9-48cc-b8b0-36eadfb3ec3d").ToString("N");
        //        var rootUri = REQUEST_URI;

        //        if (!rootUri.EndsWith("/"))
        //        {
        //            rootUri += "/";
        //        }

        //        var str = string.Format(rootUri + "web/dynamic_link?client_id={0}&access_token={1}&jwt={2}",
        //            CalomealWebViewWorker.CLIENT_ID, token, jwt);
        //        CalomealWebViewWorker.FileLog(str);

        //        return str;
        //    }

        //    return NoteCalomealWorker.GetWebViewAuthUrl(token, DateTime.Today, 0);
        //}

        internal string Hokenshido(QolmsJotoModel mainModel)
        {
            var token = TokenRead(mainModel);

            //NoteCalomealWorker.GetLogFilePath(mainModel.DebugLogPath, CALOMEAL_LOG_FILENAME);

            if (!string.IsNullOrWhiteSpace(token))
            {
                // 一応確認するけど、ない人は入らないように実装する
                // var state = Guid.Parse("2fe1980c-66a9-48cc-b8b0-36eadfb3ec3d").ToString("N");
                var rootUri = REQUEST_URI;

                if (!rootUri.EndsWith("/"))
                {
                    rootUri += "/";
                }

                var str = string.Format(rootUri + "web/hokenshido?client_id={0}&access_token={1}",
                    QjConfiguration.CalomealApiClientID, token);
                //CalomealWebViewWorker.FileLog(str);

                return str;
            }

            return NoteCalomealWorker.GetWebViewAuthUrl(token, DateTime.Today, 0);
        }

        /// <summary>
        /// カロミルWebViewを表示するURL文字列を作成します。
        /// </summary>
        /// <param name="mainModel"></param>
        /// <param name="meal"></param>
        /// <param name="selectdate"></param>
        /// <returns></returns>
        public string CreateWebViewUrl(QolmsJotoModel mainModel, string meal, string selectdate)
        {
            // 日付指定
            DateTime recordDate = DateTime.MinValue;
            DateTime tryRecordDate = DateTime.MinValue;
            var ci = CultureInfo.CurrentCulture;
            var dts = DateTimeStyles.None;

            if (!string.IsNullOrWhiteSpace(selectdate) &&
                selectdate.Length == 8 &&
                DateTime.TryParseExact(selectdate, "yyyyMMdd", ci, dts, out tryRecordDate) &&
                tryRecordDate < DateTime.Now.Date)
            {
                recordDate = tryRecordDate;
            }
            else
            {
                recordDate = DateTime.Now.Date;
            }

            var token = TokenRead(mainModel);

            return NoteCalomealWorker.GetWebViewAuthUrl(token, recordDate, meal.TryToValueType(byte.MinValue));
        }

        #endregion
    }
}