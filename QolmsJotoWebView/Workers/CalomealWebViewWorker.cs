using MGF.QOLMS.QolmsCalomealWebViewApiCoreV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsJwtAuthCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using MGF.QOLMS.QolmsJotoWebView;
using MGF.QOLMS.QolmsJotoWebView.Models;

namespace MGF.QOLMS.QolmsJotoWebView.Worker
{


    /// <summary>
    /// カロミルWebView関連の機能を提供します。
    /// </summary>
    internal sealed class CalomealWebViewWorker
    {
        #region Public Property

        public static string LogFilePath { get; set; } = string.Empty;

        #endregion

        #region Constant

        /// <summary>
        /// カロミル クライアント ID
        /// </summary>
        public static readonly string CLIENT_ID = new Lazy<string>(() => GetConfigSettings("CalomealApiClientID")).Value;

        /// <summary>
        /// カロミル クライアント シークレット
        /// </summary>
        public static readonly string CLIENT_SECRET = new Lazy<string>(() => GetConfigSettings("CalomealApiClientSecret")).Value;

        private static readonly string REDIRECT_URI = CalomealWebViewWorker.CreateReturnUrl();

        /// <summary>
        /// ReturnUrlに入れるパス（ドメイン以外）
        /// </summary>
        private const string RETURN_URL_PASS = "note/calomealresult";

        #endregion

        #region Constructor

        /// <summary>
        /// デフォルト コンストラクタは使用できません。
        /// </summary>
        private CalomealWebViewWorker()
        {
        }

        #endregion

        #region Private Method

        /// <summary>
        /// Config設定を取得します。
        /// </summary>
        /// <param name="settingsName">ConfigのKey名</param>
        /// <returns>Configのvalue値</returns>
        public static string GetConfigSettings(string settingsName)
        {
            var result = string.Empty;

            if (!string.IsNullOrWhiteSpace(settingsName))
            {
                try
                {
                    result = ConfigurationManager.AppSettings[settingsName];
                }
                catch
                {
                }
            }

            return result;
        }

        private static string CreateReturnUrl()
        {
            var root = ConfigurationManager.AppSettings["QolmsYappliSiteUri"];

            if (!root.EndsWith("/"))
            {
                root += "/";
            }

            var url = root + RETURN_URL_PASS;

            return url;
        }

        // UNIXエポックを表すDateTimeオブジェクトを取得
        private static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        private static int GetUnixTime(DateTime targetTime)
        {
            if (targetTime > DateTime.MinValue)
            {
                // UTC時間に変換
                targetTime = targetTime.ToUniversalTime();

                // UNIXエポックからの経過時間を取得
                var elapsedTime = targetTime - UNIX_EPOCH;
                // 経過秒数に変換
                return Convert.ToInt32(elapsedTime.TotalSeconds);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// アクセストークンを取得します。
        /// </summary>
        private static async Task<AccessTokenApiResults> ExecuteAccessTokenApi(string code)
        {
            // DebugLog("ExecuteAccessTokenApi");

            var apiArgs = new AccessTokenApiArgs()
            {
                grant_type = "authorization_code",
                client_id = CLIENT_ID,
                client_secret = CLIENT_SECRET,
                redirect_uri = REDIRECT_URI,
                code = code
            };

            var request = MakeCyptDataString(apiArgs);
            // DebugLog(request);
            AccessLogWorker.WriteAccessLog(null, "/note/calomeal/api_accesstoken",
                AccessLogWorker.AccessTypeEnum.Api, request);

            Task<AccessTokenApiResults> apiResults;

            Func<Task<AccessTokenApiResults>, AccessTokenApiResults> callback =
                (antecedent) =>
                {
                    var gid = Guid.NewGuid();
                    FileLog($" {gid}: ExecuteAccessTokenApi");

                    if (antecedent.Status == TaskStatus.RanToCompletion)
                    {
                        if (antecedent.Result.IsSuccess)
                        {
                            FileLog($" {gid}: {antecedent.Result.IsSuccess}");
                        }
                        else
                        {
                            FileLog($" {gid}: {antecedent.Result.IsSuccess}");
                            FileLog($" {gid}: statusCode = {antecedent.Result.StatusCode} , error ={antecedent.Result.error}");
                            FileLog($" {gid}: ResponseString = {antecedent.Result.ResponseString}");
                        }
                    }
                    else if (antecedent.Status == TaskStatus.Faulted)
                    {
                        FileLog($" {gid}: {antecedent.Exception?.GetBaseException().Message}");
                        FileLog($" {gid}: {antecedent.Exception?.GetBaseException().StackTrace}");
                        return new AccessTokenApiResults() { error = antecedent.Exception?.GetBaseException().Message };
                    }

                    return antecedent.Result;
                };

            try
            {
                apiResults = QsCalomealWebViewApiManager.ExecuteAsync<AccessTokenApiArgs, AccessTokenApiResults>(apiArgs, callback);
            }
            catch (Exception ex)
            {
                AccessLogWorker.WriteAccessLog(null, "/note/calomeal/api_accesstoken_error",
                    AccessLogWorker.AccessTypeEnum.Error, ex.Message);
                throw;
            }

            var result = await apiResults;
            // DebugLog(result.ResponseString);
            AccessLogWorker.WriteAccessLog(null, "/note/calomeal/api_accesstoken",
                AccessLogWorker.AccessTypeEnum.Api, result.ResponseString);

            if (result.IsSuccess)
            {
                return result;
            }
            else
            {
                try
                {
                    AccessLogWorker.WriteAccessLog(null, "/note/calomeal/api_accesstoken_error",
                        AccessLogWorker.AccessTypeEnum.Error,
                        string.Format("statuscod={0},.error={1}", result.StatusCode, result.error));
                }
                catch (Exception ex)
                {
                    AccessLogWorker.WriteAccessLog(null, "/note/calomeal/api_accesstoken_error",
                        AccessLogWorker.AccessTypeEnum.Error, ex.Message);
                    AccessLogWorker.WriteAccessLog(null, "/note/calomeal/api_accesstoken_error",
                        AccessLogWorker.AccessTypeEnum.Error, ex.InnerException?.Message);
                }

                throw new InvalidOperationException(string.Format("{0} API の実行に失敗しました。",
                    "QolmsCalomealWebViewApiCoreV1.AccessTokenApi"));
            }
        }

        ///// <summary>
        ///// IDトークン更新
        ///// </summary>
        //private static async Task<AccessTokenApiResults> ExecuteRefreshTokenApi(string refreshToken)
        //{
        //    // DebugLog("ExecuteRefreshTokenApi");

        //    var apiArgs = new AccessTokenApiArgs()
        //    {
        //        grant_type = "refresh_token",
        //        client_id = CLIENT_ID,
        //        client_secret = CLIENT_SECRET,
        //        redirect_uri = REDIRECT_URI,
        //        refresh_token = refreshToken
        //    };

        //    var request = MakeCyptDataString(apiArgs);
        //    // DebugLog(request);
        //    AccessLogWorker.WriteAccessLog(null, "/note/calomeal/api_accesstoken",
        //        AccessLogWorker.AccessTypeEnum.Api, request);

        //    Task<AccessTokenApiResults> apiResults;

        //    Func<Task<AccessTokenApiResults>, AccessTokenApiResults> callback =
        //        (antecedent) =>
        //        {
        //            var gid = Guid.NewGuid();
        //            FileLog($" {gid}: ExecuteAccessTokenApi");

        //            if (antecedent.Status == TaskStatus.RanToCompletion)
        //            {
        //                if (antecedent.Result.IsSuccess)
        //                {
        //                    FileLog($" {gid}: {antecedent.Result.IsSuccess}");
        //                }
        //                else
        //                {
        //                    FileLog($" {gid}: {antecedent.Result.IsSuccess}");
        //                    FileLog($" {gid}: statusCode = {antecedent.Result.StatusCode} , error ={antecedent.Result.error}");
        //                    FileLog($" {gid}: ResponseString = {antecedent.Result.ResponseString}");
        //                }
        //            }
        //            else if (antecedent.Status == TaskStatus.Faulted)
        //            {
        //                FileLog($" {gid}: {antecedent.Exception?.GetBaseException().Message}");
        //                FileLog($" {gid}: {antecedent.Exception?.GetBaseException().StackTrace}");
        //                return new AccessTokenApiResults() { error = antecedent.Exception?.GetBaseException().Message };
        //            }

        //            return antecedent.Result;
        //        };

        //    try
        //    {
        //        apiResults = QsCalomealWebViewApiManager.ExecuteAsync<AccessTokenApiArgs, AccessTokenApiResults>(apiArgs, callback);
        //    }
        //    catch (Exception ex)
        //    {
        //        AccessLogWorker.WriteAccessLog(null, "/note/calomeal/api_accesstoken_error",
        //            AccessLogWorker.AccessTypeEnum.Error, ex.Message);
        //        throw;
        //    }

        //    var result = await apiResults;
        //    // DebugLog(result.ResponseString);
        //    AccessLogWorker.WriteAccessLog(null, "/note/calomeal/api_accesstoken",
        //        AccessLogWorker.AccessTypeEnum.Api, result.ResponseString);

        //    if (result.IsSuccess)
        //    {
        //        return result;
        //    }
        //    else
        //    {
        //        try
        //        {
        //            AccessLogWorker.WriteAccessLog(null, "/note/calomeal/api_accesstoken_error",
        //                AccessLogWorker.AccessTypeEnum.Error,
        //                string.Format("statuscod={0},.error={1}", result.StatusCode, result.error));
        //        }
        //        catch (Exception ex)
        //        {
        //            AccessLogWorker.WriteAccessLog(null, "/note/calomeal/api_accesstoken_error",
        //                AccessLogWorker.AccessTypeEnum.Error, ex.Message);
        //            AccessLogWorker.WriteAccessLog(null, "/note/calomeal/api_accesstoken_error",
        //                AccessLogWorker.AccessTypeEnum.Error, ex.InnerException?.Message);
        //        }

        //        throw new InvalidOperationException(string.Format("{0} API の実行に失敗しました。",
        //            "QolmsCalomealWebViewApiCoreV1.AccessTokenApi"));
        //    }
        //}

        /// <summary>
        /// 食事履歴の取得
        /// </summary>
        private static async Task<MealApiResults> ExecuteMealApi(int unixTime, string token)
        {
            // DebugLog("ExecuteMealApi");

            var apiArgs = new MealApiArgs()
            {
                updated_at = unixTime,
                Token = token
            };

            var request = MakeCyptDataString(apiArgs);
            // DebugLog(request);
            AccessLogWorker.WriteAccessLog(null, "/note/calomeal/api_meal",
                AccessLogWorker.AccessTypeEnum.Api, request);

            Task<MealApiResults> apiResults;

            Func<Task<MealApiResults>, MealApiResults> callback =
                (antecedent) =>
                {
                    var gid = Guid.NewGuid();
                    FileLog($" {gid}: ExecuteMealApi");

                    if (antecedent.Status == TaskStatus.RanToCompletion)
                    {
                        if (antecedent.Result.IsSuccess)
                        {
                            FileLog($" {gid}: {antecedent.Result.IsSuccess}");
                        }
                        else
                        {
                            FileLog($" {gid}: {antecedent.Result.IsSuccess}");
                            FileLog($" {gid}: statusCode = {antecedent.Result.StatusCode} , error ={antecedent.Result.error}");
                            FileLog($" {gid}: ResponseString = {antecedent.Result.ResponseString}");
                        }
                    }
                    else if (antecedent.Status == TaskStatus.Faulted)
                    {
                        FileLog($" {gid}: {antecedent.Exception?.GetBaseException().Message}");
                        FileLog($" {gid}: {antecedent.Exception?.GetBaseException().StackTrace}");
                        return new MealApiResults() { error = antecedent.Exception?.GetBaseException().Message };
                    }

                    return antecedent.Result;
                };

            apiResults = QsCalomealWebViewApiManager.ExecuteAsync<MealApiArgs, MealApiResults>(apiArgs, callback);

            var result = await apiResults;
            // DebugLog(result.ResponseString);
            AccessLogWorker.WriteAccessLog(null, "/note/calomeal/api_meal",
                AccessLogWorker.AccessTypeEnum.Api, result.ResponseString);

            if (result.IsSuccess)
            {
                return result;
            }
            else
            {
                throw new InvalidOperationException(string.Format("{0} API の実行に失敗しました。",
                    "QolmsCalomealWebViewApiCoreV1.MealApi"));
            }
        }

        ///// <summary>
        ///// プロフィールを登録します。
        ///// </summary>
        //private static async Task<SetProfileApiResults> ExecuteSetProfileApi(string token, DateTime birthDay,
        //    decimal height, decimal weight, QsCalomealWebViewApiSexTypeEnum sex)
        //{
        //    var apiArgs = new SetProfileApiArgs()
        //    {
        //        Token = token,
        //        BirthDay = birthDay.ToString("yyyy/MM/dd"),
        //        Sex = QsCalomealWebViewApiSexTypeEnum.Men.ToString("d"),
        //        Height = height,
        //        Weight = weight
        //    };

        //    var request = MakeCyptDataString(apiArgs);
        //    // DebugLog(request);
        //    AccessLogWorker.WriteAccessLog(null, "/note/calomeal/api_setprofile",
        //        AccessLogWorker.AccessTypeEnum.Api, request);

        //    Task<SetProfileApiResults> apiResults;

        //    Func<Task<SetProfileApiResults>, SetProfileApiResults> callback =
        //        (antecedent) =>
        //        {
        //            var gid = Guid.NewGuid();
        //            FileLog($" {gid}: ExecuteSetProfileApi");

        //            if (antecedent.Status == TaskStatus.RanToCompletion)
        //            {
        //                if (antecedent.Result.IsSuccess)
        //                {
        //                    FileLog($" {gid}: {antecedent.Result.IsSuccess}");
        //                }
        //                else
        //                {
        //                    FileLog($" {gid}: {antecedent.Result.IsSuccess}");
        //                    FileLog($" {gid}: statusCode = {antecedent.Result.StatusCode} , error ={antecedent.Result.error}");
        //                    FileLog($" {gid}: ResponseString = {antecedent.Result.ResponseString}");
        //                }
        //            }
        //            else if (antecedent.Status == TaskStatus.Faulted)
        //            {
        //                FileLog($" {gid}: {antecedent.Exception?.GetBaseException().Message}");
        //                FileLog($" {gid}: {antecedent.Exception?.GetBaseException().StackTrace}");
        //                return new SetProfileApiResults() { error = antecedent.Exception?.GetBaseException().Message };
        //            }

        //            return antecedent.Result;
        //        };

        //    apiResults = QsCalomealWebViewApiManager.ExecuteAsync<SetProfileApiArgs, SetProfileApiResults>(apiArgs, callback);

        //    var result = await apiResults;
        //    // DebugLog(result.ResponseString);
        //    AccessLogWorker.WriteAccessLog(null, "/note/calomeal/api_setprofile",
        //        AccessLogWorker.AccessTypeEnum.Api, result.ResponseString);

        //    if (result.IsSuccess)
        //    {
        //        return result;
        //    }
        //    else
        //    {
        //        throw new InvalidOperationException(string.Format("{0} API の実行に失敗しました。",
        //            "QolmsCalomealWebViewApiCoreV1.SetProfile"));
        //    }
        //}

        /// <summary>
        /// ユーザー目標設定を登録します。
        /// </summary>
        private static async Task<SetGoalApiResults> ExecuteSetGoalApi(string token, DateTime term, decimal weight)
        {
            var apiArgs = new SetGoalApiArgs()
            {
                Token = token,
                Term = term.ToString("yyyyMMdd"),
                Weight = weight,
                Calculationtype = "recommend"
            };

            var request = MakeCyptDataString(apiArgs);
            // DebugLog(request);
            AccessLogWorker.WriteAccessLog(null, "/note/calomeal/api_setgoal",
                AccessLogWorker.AccessTypeEnum.Api, request);

            Task<SetGoalApiResults> apiResults;

            Func<Task<SetGoalApiResults>, SetGoalApiResults> callback =
                (antecedent) =>
                {
                    var gid = Guid.NewGuid();
                    FileLog($" {gid}: ExecuteSetGoalApi");

                    if (antecedent.Status == TaskStatus.RanToCompletion)
                    {
                        if (antecedent.Result.IsSuccess)
                        {
                            FileLog($" {gid}: {antecedent.Result.IsSuccess}");
                        }
                        else
                        {
                            FileLog($" {gid}: {antecedent.Result.IsSuccess}");
                            FileLog($" {gid}: statusCode = {antecedent.Result.StatusCode} , error ={antecedent.Result.error}");
                            FileLog($" {gid}: ResponseString = {antecedent.Result.ResponseString}");
                        }
                    }
                    else if (antecedent.Status == TaskStatus.Faulted)
                    {
                        FileLog($" {gid}: {antecedent.Exception?.GetBaseException().Message}");
                        FileLog($" {gid}: {antecedent.Exception?.GetBaseException().StackTrace}");
                        return new SetGoalApiResults() { error = antecedent.Exception?.GetBaseException().Message };
                    }

                    return antecedent.Result;
                };

            apiResults = QsCalomealWebViewApiManager.ExecuteAsync<SetGoalApiArgs, SetGoalApiResults>(apiArgs, callback);

            var result = await apiResults;
            // DebugLog(result.ResponseString);
            AccessLogWorker.WriteAccessLog(null, "/note/calomeal/api_setgoal",
                AccessLogWorker.AccessTypeEnum.Api, result.ResponseString);

            if (result.IsSuccess)
            {
                return result;
            }
            else
            {
                throw new InvalidOperationException(string.Format("{0} API の実行に失敗しました。",
                    "QolmsCalomealWebViewApiCoreV1.SetGoal"));
            }
        }

        /// <summary>
        /// 食事履歴取得
        /// </summary>
        private static async Task<MealWithBasisResults> ExecuteMealWithBasisApi(string token,
            DateTime startDate, DateTime endDate)
        {
            var apiArgs = new MealWithBasisApiArgs()
            {
                Token = token,
                StartDate = startDate.ToString("yyyy/MM/dd"),
                EndDate = endDate.ToString("yyyy/MM/dd")
            };

            var request = MakeCyptDataString(apiArgs);
            // DebugLog(request);
            AccessLogWorker.WriteAccessLog(null, "/note/calomeal/api_mealwithbasis",
                AccessLogWorker.AccessTypeEnum.Api, request);

            Task<MealWithBasisResults> apiResults;

            Func<Task<MealWithBasisResults>, MealWithBasisResults> callback =
                (antecedent) =>
                {
                    var gid = Guid.NewGuid();
                    FileLog($" {gid}: ExecuteMealWithBasisApi");

                    if (antecedent.Status == TaskStatus.RanToCompletion)
                    {
                        if (antecedent.Result.IsSuccess)
                        {
                            FileLog($" {gid}: {antecedent.Result.IsSuccess}");
                        }
                        else
                        {
                            FileLog($" {gid}: {antecedent.Result.IsSuccess}");
                            FileLog($" {gid}: statusCode = {antecedent.Result.StatusCode} , error ={antecedent.Result.error}");
                            FileLog($" {gid}: ResponseString = {antecedent.Result.ResponseString}");
                        }
                    }
                    else if (antecedent.Status == TaskStatus.Faulted)
                    {
                        FileLog($" {gid}: {antecedent.Exception?.GetBaseException().Message}");
                        FileLog($" {gid}: {antecedent.Exception?.GetBaseException().StackTrace}");
                        return new MealWithBasisResults() { error = antecedent.Exception?.GetBaseException().Message };
                    }

                    return antecedent.Result;
                };

            apiResults = QsCalomealWebViewApiManager.ExecuteAsync<MealWithBasisApiArgs, MealWithBasisResults>(apiArgs, callback);

            var result = await apiResults;
            // DebugLog(result.ResponseString);
            AccessLogWorker.WriteAccessLog(null, "/note/calomeal/api_mealwithbasis",
                AccessLogWorker.AccessTypeEnum.Api, result.ResponseString);

            if (result.IsSuccess)
            {
                return result;
            }
            else
            {
                throw new InvalidOperationException(string.Format("{0} API の実行に失敗しました。",
                    "QolmsCalomealWebViewApiCoreV1.MealWithBasis"));
            }
        }

        /// <summary>
        /// 体重、体脂肪率設定を登録します。
        /// </summary>
        private static async Task<SetAnthropometricApiResults> ExecuteSetAnthropometricApi(string token,
            DateTime targetDate, decimal weight, int section)
        {
            var apiArgs = new SetAnthropometricApiArgs()
            {
                Token = token,
                list = new List<SetAnthropometricOfJson>
            {
                new SetAnthropometricOfJson()
                {
                    date = targetDate.ToString("yyyy/MM/dd"),
                    weight = weight,
                    section = section
                }
            }
            };

            var request = MakeCyptDataString(apiArgs);
            // DebugLog(request);
            AccessLogWorker.WriteAccessLog(null, "/note/calomeal/api_setanthropometric",
                AccessLogWorker.AccessTypeEnum.Api, request);

            Task<SetAnthropometricApiResults> apiResults;

            Func<Task<SetAnthropometricApiResults>, SetAnthropometricApiResults> callback =
                (antecedent) =>
                {
                    var gid = Guid.NewGuid();
                    FileLog($" {gid}: ExecuteSetAnthropometricApi");

                    if (antecedent.Status == TaskStatus.RanToCompletion)
                    {
                        if (antecedent.Result.IsSuccess)
                        {
                            FileLog($" {gid}: {antecedent.Result.IsSuccess}");
                        }
                        else
                        {
                            FileLog($" {gid}: {antecedent.Result.IsSuccess}");
                            FileLog($" {gid}: statusCode = {antecedent.Result.StatusCode} , error ={antecedent.Result.error}");
                            FileLog($" {gid}: ResponseString = {antecedent.Result.ResponseString}");
                        }
                    }
                    else if (antecedent.Status == TaskStatus.Faulted)
                    {
                        FileLog($" {gid}: {antecedent.Exception?.GetBaseException().Message}");
                        FileLog($" {gid}: {antecedent.Exception?.GetBaseException().StackTrace}");
                        return new SetAnthropometricApiResults() { error = antecedent.Exception?.GetBaseException().Message };
                    }

                    return antecedent.Result;
                };

            apiResults = QsCalomealWebViewApiManager.ExecuteAsync<SetAnthropometricApiArgs, SetAnthropometricApiResults>(apiArgs, callback);

            var result = await apiResults;
            // DebugLog(result.ResponseString);
            AccessLogWorker.WriteAccessLog(null, "/note/calomeal/api_setanthropometric",
                AccessLogWorker.AccessTypeEnum.Api, result.ResponseString);

            if (result.IsSuccess)
            {
                return result;
            }
            else
            {
                throw new InvalidOperationException(string.Format("{0} API の実行に失敗しました。",
                    "QolmsCalomealWebViewApiCoreV1.SetAnthropometric"));
            }
        }

        /// <summary>
        /// カロミルアドバイスの同意を取得します。
        /// </summary>
        private static async Task<InstructInfoApiResults> ExecuteInstructInfoApi(string token)
        {
            var apiArgs = new InstructInfoApiArgs()
            {
                Token = token
            };

            var request = MakeCyptDataString(apiArgs);
            // DebugLog(request);
            AccessLogWorker.WriteAccessLog(null, "/note/calomeal/api_instructinfo",
                AccessLogWorker.AccessTypeEnum.Api, request);

            Task<InstructInfoApiResults> apiResults;

            Func<Task<InstructInfoApiResults>, InstructInfoApiResults> callback =
                (antecedent) =>
                {
                    var gid = Guid.NewGuid();
                    FileLog($" {gid}: ExecuteInstructInfoApi");

                    if (antecedent.Status == TaskStatus.RanToCompletion)
                    {
                        if (antecedent.Result.IsSuccess)
                        {
                            FileLog($" {gid}: {antecedent.Result.IsSuccess}");
                        }
                        else
                        {
                            FileLog($" {gid}: {antecedent.Result.IsSuccess}");
                            FileLog($" {gid}: statusCode = {antecedent.Result.StatusCode} , error ={antecedent.Result.error}");
                            FileLog($" {gid}: ResponseString = {antecedent.Result.ResponseString}");
                        }
                    }
                    else if (antecedent.Status == TaskStatus.Faulted)
                    {
                        FileLog($" {gid}: {antecedent.Exception?.GetBaseException().Message}");
                        FileLog($" {gid}: {antecedent.Exception?.GetBaseException().StackTrace}");
                        return new InstructInfoApiResults() { error = antecedent.Exception?.GetBaseException().Message };
                    }

                    return antecedent.Result;
                };

            apiResults = QsCalomealWebViewApiManager.ExecuteAsync<InstructInfoApiArgs, InstructInfoApiResults>(apiArgs, callback);

            var result = await apiResults;
            // DebugLog(result.ResponseString);
            AccessLogWorker.WriteAccessLog(null, "/note/calomeal/api_instructinfo",
                AccessLogWorker.AccessTypeEnum.Api, result.ResponseString);

            if (result.IsSuccess)
            {
                return result;
            }
            else
            {
                throw new InvalidOperationException(string.Format("{0} API の実行に失敗しました。",
                    "QolmsCalomealWebViewApiCoreV1.InstructInfo"));
            }
        }

        /// <summary>
        /// リクエスト引数を暗号化した文字列を作成します
        /// </summary>
        private static string TestMakeCyptDataString(object args)
        {
            try
            {
                using (var ms = new System.IO.MemoryStream())
                {
                    var serializer = new DataContractJsonSerializer(args.GetType());
                    serializer.WriteObject(ms, args);
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
            catch (Exception ex)
            {
            }
            return "";
        }

        /// <summary>
        /// リクエスト引数を暗号化した文字列を作成します
        /// </summary>
        private static string MakeCyptDataString(QsCalomealWebViewApiArgsBase args)
        {
            try
            {
                using (var crypt = new QOLMS.QolmsCryptV1.QsCrypt(QOLMS.QolmsCryptV1.QsCryptTypeEnum.QolmsSystem))
                {
                    using (var ms = new System.IO.MemoryStream())
                    {
                        var serializer = new DataContractJsonSerializer(args.GetType());
                        serializer.WriteObject(ms, args);
                        return crypt.EncryptString(Encoding.UTF8.GetString(ms.ToArray()));
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return "";
        }

        // テスト用の手抜きログ吐き
        // [Conditional("DEBUG")]
        // public static void DebugLog(string message)
        // {
        //     try
        //     {
        //         string log = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/log"), "Calomeal.Log");
        //         System.IO.File.AppendAllText(log, string.Format("{0}:{1}{2}", DateTime.Now, message, Environment.NewLine));
        //     }
        //     catch (Exception ex)
        //     {
        //     }
        // }

        /// <summary>
        /// 本番出力用ログ
        /// </summary>
        public static void FileLog(string message)
        {
            try
            {
                var log = CalomealWebViewWorker.LogFilePath;
                System.IO.File.AppendAllText(log, string.Format("{0}:{1}{2}", DateTime.Now, message, Environment.NewLine));
            }
            catch (Exception ex)
            {
            }
        }

        #endregion

        #region Public Method

        ///// <summary>
        ///// ユーザー認証コードからアクセストークンを発行する
        ///// </summary>
        //public static CalomealAccessTokenSet GetNewToken(string code)
        //{
        //    var result = ExecuteAccessTokenApi(code).Result;

        //    if (result.IsSuccess)
        //    {
        //        var tokenSet = new CalomealAccessTokenSet()
        //        {
        //            access_token = result.access_token,
        //            expires_in = result.expires_in,
        //            RefreshTokenExpires = DateTime.MaxValue,
        //            refresh_token = result.refresh_token,
        //            TokenExpires = DateTime.Now.AddSeconds(int.Parse(result.expires_in)),
        //            token_type = result.token_type
        //        };

        //        return tokenSet;
        //    }

        //    return new CalomealAccessTokenSet();
        //}

        ///// <summary>
        ///// フレッシュトークンからアクセストークンを再発行する
        ///// カロミルAPIを呼び出します。
        ///// </summary>
        //public static CalomealAccessTokenSet GetRefreshToken(string refreshToken)
        //{
        //    var result = ExecuteRefreshTokenApi(refreshToken).Result;

        //    if (result.IsSuccess)
        //    {
        //        var tokenSet = new CalomealAccessTokenSet()
        //        {
        //            access_token = result.access_token,
        //            expires_in = result.expires_in,
        //            RefreshTokenExpires = DateTime.MaxValue,
        //            refresh_token = refreshToken,
        //            TokenExpires = DateTime.Now.AddSeconds(int.Parse(result.expires_in)),
        //            token_type = result.token_type
        //        };

        //        return tokenSet;
        //    }

        //    return new CalomealAccessTokenSet();
        //}

        /// <summary>
        /// 指定されたアクセストークンに該当するユーザーの食事履歴を取得する
        /// カロミルAPIを呼び出します。
        /// </summary>
        public static List<MealHistoriesOfJson> GetMeal(DateTime updatedDate, string token)
        {
            var unixTime = GetUnixTime(updatedDate);

            var result = ExecuteMealApi(unixTime, token).Result;
            if (result.IsSuccess)
            {
                return result.meal_histories;
            }

            return new List<MealHistoriesOfJson>();
        }

        ///// <summary>
        ///// 指定されたアクセストークンに該当するユーザーのプロフィール情報を設定する
        ///// カロミルAPIを呼び出します。
        ///// </summary>
        //public static bool SetProfile(string token, DateTime birthDay, decimal height, decimal weight, QjSexTypeEnum sex)
        //{
        //    try
        //    {
        //        var result = ExecuteSetProfileApi(token, birthDay, height, weight,
        //            sex.ToString("d").TryToValueType<QsCalomealWebViewApiSexTypeEnum>(0)).Result;

        //        return result.IsSuccess && result.StatusCode == 200;
        //    }
        //    catch (Exception ex)
        //    {
        //        AccessLogWorker.WriteErrorLog(null, "/note/calomeal/api_setprofile_error", ex.Message);
        //    }
        //    return false;
        //}

        /// <summary>
        /// 指定されたアクセストークンに該当するユーザーの体重、体脂肪率の記録、目標から算出した摂取基準、および食事履歴を日単位に取得する
        /// カロミルAPIを呼び出します。
        /// </summary>
        public static List<MealWithBasisOfJson> GetMealWithBasis(string token, DateTime startDate, DateTime endDate)
        {
            try
            {
                var result = ExecuteMealWithBasisApi(token, startDate, endDate).Result;
                if (result.IsSuccess && result.StatusCode == 200)
                {
                    return result.meal_with_basis;
                }
            }
            catch (Exception ex)
            {
                AccessLogWorker.WriteErrorLog(null, "/note/calomeal/api_mealwithbasis_error", ex.Message);
            }
            return new List<MealWithBasisOfJson>();
        }

        /// <summary>
        /// 指定されたアクセストークンに該当するユーザーの体重、体脂肪率を追加する
        /// カロミルAPIを呼び出します。
        /// </summary>
        public static bool SetAnthropometric(string token, DateTime targetDate, decimal weight, int section)
        {
            try
            {
                var result = ExecuteSetAnthropometricApi(token, targetDate, weight, section).Result;
                return result.IsSuccess && result.StatusCode == 200;
            }
            catch (Exception ex)
            {
                AccessLogWorker.WriteErrorLog(null, "/note/calomeal/api_setanthropometric_error", ex.Message);
            }

            return false;
        }

        /// <summary>
        /// 指定されたアクセストークンに該当するユーザーの目標を設定する
        /// カロミルAPIを呼び出します。
        /// </summary>
        public static bool SetGoal(string token, DateTime targetDate, decimal weight)
        {
            try
            {
                var result = ExecuteSetGoalApi(token, targetDate, weight).Result;
                return result.IsSuccess && result.StatusCode == 200;
            }
            catch (Exception ex)
            {
                AccessLogWorker.WriteErrorLog(null, "/note/calomeal/api_setgoal_error", ex.Message);
            }

            return false;
        }

        /// <summary>
        /// アドバイス同意情報を取得する
        /// カロミルAPIを呼び出します。
        /// </summary>
        public static InstructInfoOfJson InstructInfoApi(string token)
        {
            try
            {
                var result = ExecuteInstructInfoApi(token).Result;
                return result.info;
            }
            catch (Exception ex)
            {
                AccessLogWorker.WriteErrorLog(null, "/note/calomeal/api_instructinfo_error", ex.Message);
            }

            return new InstructInfoOfJson();
        }

        /// <summary>
        /// 食事区分の文字列からEnumへ変換します。
        /// </summary>
        private static QjMealTypeEnum ToMealTypeEnum(string target)
        {
            switch (target)
            {
                case "朝食":
                    return QjMealTypeEnum.Breakfast;
                case "昼食":
                    return QjMealTypeEnum.Lunch;
                case "夕食":
                    return QjMealTypeEnum.Dinner;
                case "間食":
                    return QjMealTypeEnum.Snacking;
                default:
                    return QjMealTypeEnum.None;
            }
        }

        /// <summary>
        /// カロミルの食事JsonをQOLMSの食事登録モデルに変換します。
        /// </summary>
        public static NoteMeal2InputModel ToMealEvent2(MealHistoriesOfJson target, Guid accountkey)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target), "変換元クラスがNull参照です。");

            var ci = CultureInfo.CurrentCulture;
            var dts = DateTimeStyles.None;
            var sr = new QsJsonSerializer();

            var recordDate = DateTime.MinValue;
            DateTime.TryParseExact(target.meal_date, "yyyy/MM/dd", ci, dts, out recordDate);
            if (recordDate > DateTime.MinValue)
            {
                recordDate = recordDate.AddHours(target.hour.TryToValueType(int.MinValue));
            }

            short cal = short.MinValue;
            if (target.calorie == null || short.TryParse(target.calorie, out cal))
            {
                return new NoteMeal2InputModel()
                {
                    AnalysisSet = sr.Serialize(target).ToString(),
                    AnalysisType = 5,
                    Calorie = target.calorie.TryToValueType(short.MinValue),
                    ChooseSet = sr.Serialize(target).ToString(),
                    DeleteFlag = target.is_deleted.TryToValueType(false),
                    HistoryId = target.meal_history_id.TryToValueType(int.MinValue),
                    ItemName = target.menu_name ?? string.Empty,
                    MealType = Convert.ToByte(target.meal_type == null ? 0 : ToMealTypeEnum(target.meal_type)).ToString().TryToValueType(byte.MinValue),
                    Rate = 1,
                    RecordDate = recordDate,
                    HasImage = target.has_image.TryToValueType(false)
                };
            }
            else
            {
                AccessLogWorker.WriteErrorLog(null, "/note/calomeal/api_meal_error",
                    new InvalidOperationException(string.Format(
                        "カロミル食事の変換に失敗しました。accountkey={0},historyid={1},dataset={2}",
                        accountkey, target.meal_history_id, sr.Serialize(target).ToString())));
                return new NoteMeal2InputModel();
            }
        }

        /// <summary>
        /// カロミルの食事JsonをQOLMSの目標情報モデルに変換します。
        /// </summary>
        public static CalomealGoalSet ToCalomealGoalSet(MealWithBasisOfJson target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target), "変換元クラスがNull参照です。");

            return new CalomealGoalSet()
            {
                TargetDate = target.date.TryToValueType(DateTime.MinValue),
                BasisAllCalorie = target.basis.all.calorie.TryToValueType(int.MinValue)
            };
        }

        /// <summary>
        /// カロミルのアドバイス情報をQOLMS形式に変換します。
        /// </summary>
        public static CalomealInstructInfo ToCalomealInstructInfo(InstructInfoOfJson target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target), "変換元クラスがNull参照です。");

            return new CalomealInstructInfo()
            {
                StoreName = target.store_name,
                UserName = target.user_name
            };
        }

        #endregion

    }
}