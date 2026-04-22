using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsOpenApi.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    internal sealed class CalomealMealSyncClient
    {
        private const int DefaultDeadlineMinutes = 60;
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly TimeZoneInfo JstTimeZone = GetJstTimeZoneCore();

        private readonly CalomealWebViewApiSettings _settings;

        public CalomealMealSyncClient()
        {
            _settings = CalomealWebViewApiSettings.Load();
        }

        internal static DateTime GetCurrentJst()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, JstTimeZone);
        }

        public CalomealMealSyncExecutionResult SyncMealHistories(
            CalomealMealSyncRepository repository,
            Guid accountKey,
            DateTime targetDateTime,
            double timeSpanInHours)
        {
            if (repository == null) throw new ArgumentNullException(nameof(repository));
            if (accountKey == Guid.Empty) throw new ArgumentOutOfRangeException(nameof(accountKey));

            var execution = new CalomealMealSyncExecutionResult()
            {
                TargetDateTime = targetDateTime,
                TimeSpanInHours = timeSpanInHours
            };

            var tokenData = repository.ReadTokenData(accountKey, CalomealMealSyncRepository.CalomealLinkageSystemNo);
            if (tokenData == null)
            {
                throw new InvalidOperationException("カロミル連携トークンが存在しません。");
            }

            var token = tokenData.Token;
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new InvalidOperationException("カロミル連携トークンが未登録です。");
            }

            if (ShouldRefreshToken(tokenData.TokenExpires))
            {
                if (string.IsNullOrWhiteSpace(tokenData.RefreshToken))
                {
                    throw new InvalidOperationException("カロミル連携のリフレッシュトークンが未登録です。");
                }

                var refreshResponse = RefreshToken(tokenData.RefreshToken);
                if (!refreshResponse.IsSuccess || string.IsNullOrWhiteSpace(refreshResponse.AccessToken))
                {
                    var message = string.IsNullOrWhiteSpace(refreshResponse.Error)
                        ? "カロミルのトークン更新に失敗しました。"
                        : $"カロミルのトークン更新に失敗しました。{refreshResponse.Error}";
                    throw new InvalidOperationException(message);
                }

                token = refreshResponse.AccessToken;
                repository.UpdateToken(accountKey, CalomealMealSyncRepository.CalomealLinkageSystemNo, token, GetCurrentJst().AddDays(1));
                execution.TokenRefreshed = true;
            }

            var updatedAfter = targetDateTime.AddHours(-timeSpanInHours);
            var mealResponse = GetMeal(updatedAfter, token);
            if (!mealResponse.IsSuccess)
            {
                var detail = string.IsNullOrWhiteSpace(mealResponse.Error)
                    ? "カロミルの食事履歴取得に失敗しました。"
                    : $"カロミルの食事履歴取得に失敗しました。{mealResponse.Error}";
                throw new InvalidOperationException($"StatusCode:{mealResponse.StatusCode} {detail}");
            }

            if (mealResponse.MealHistories == null || mealResponse.MealHistories.Count == 0)
            {
                execution.Message = "同期対象の食事履歴は 0 件でした。";
                return execution;
            }

            var actionDate = GetCurrentJst();
            var actionKey = Guid.NewGuid();

            foreach (var history in mealResponse.MealHistories)
            {
                execution.ProcessedCount++;

                var item = ToMealSyncItem(history);
                if (item == null || item.HistoryId <= 0)
                {
                    execution.ErrorCount++;
                    execution.ErrorMessages.Add("カロミル食事データの変換に失敗しました。");
                    continue;
                }

                var actionType = repository.ApplyMealHistory(accountKey, item, actionDate, actionKey);
                if (actionType == CalomealMealSyncActionType.None)
                {
                    execution.ErrorCount++;
                    execution.ErrorMessages.Add($"同期先が存在しないため処理できませんでした。historyId:{item.HistoryId}");
                    continue;
                }

                execution.SuccessCount++;
                switch (actionType)
                {
                    case CalomealMealSyncActionType.Added:
                        execution.AddedCount++;
                        break;
                    case CalomealMealSyncActionType.Modified:
                        execution.ModifiedCount++;
                        break;
                    case CalomealMealSyncActionType.Deleted:
                        execution.DeletedCount++;
                        break;
                }
            }

            execution.Message = $"Processed:{execution.ProcessedCount} Success:{execution.SuccessCount} Error:{execution.ErrorCount}";
            return execution;
        }

        private bool ShouldRefreshToken(DateTime tokenExpires)
        {
            return GetCurrentJst() >= tokenExpires.AddMinutes(-DefaultDeadlineMinutes);
        }

        private CalomealAccessTokenResponse RefreshToken(string refreshToken)
        {
            using (var client = CreateHttpClient())
            using (var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("client_id", _settings.ClientId),
                new KeyValuePair<string, string>("client_secret", _settings.SecretKey),
                new KeyValuePair<string, string>("redirect_uri", _settings.RedirectUri),
                new KeyValuePair<string, string>("refresh_token", refreshToken),
            }))
            using (var response = client.PostAsync(new Uri(new Uri(_settings.ApiUri), "auth/accesstoken"), content).GetAwaiter().GetResult())
            {
                return ReadResponse<CalomealAccessTokenResponse>(response);
            }
        }

        private CalomealMealResponse GetMeal(DateTime updatedAfter, string token)
        {
            using (var client = CreateHttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                using (var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("updated_at", ToUnixTime(updatedAfter).ToString(CultureInfo.InvariantCulture))
                }))
                using (var response = client.PostAsync(new Uri(new Uri(_settings.ApiUri), "api/meal"), content).GetAwaiter().GetResult())
                {
                    return ReadResponse<CalomealMealResponse>(response);
                }
            }
        }

        private HttpClient CreateHttpClient()
        {
            var client = new HttpClient(new HttpClientHandler() { UseCookies = false });
            client.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds);
            if (!string.IsNullOrWhiteSpace(_settings.UserAgent))
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd(_settings.UserAgent);
            }
            client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue() { NoCache = true };
            return client;
        }

        private static TResponse ReadResponse<TResponse>(HttpResponseMessage response)
            where TResponse : CalomealResponseBase, new()
        {
            var body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var result = string.IsNullOrWhiteSpace(body)
                ? new TResponse()
                : JsonConvert.DeserializeObject<TResponse>(body) ?? new TResponse();

            result.StatusCode = (int)response.StatusCode;
            result.IsSuccess = response.StatusCode == HttpStatusCode.OK;
            result.ResponseString = body;

            if (!result.IsSuccess && string.IsNullOrWhiteSpace(result.Error))
            {
                result.Error = response.ReasonPhrase;
            }

            return result;
        }

        private static CalomealMealSyncItem ToMealSyncItem(CalomealMealHistory history)
        {
            if (history == null)
            {
                return null;
            }

            if (!DateTime.TryParseExact(history.MealDate ?? string.Empty, "yyyy/MM/dd", CultureInfo.CurrentCulture, DateTimeStyles.None, out var recordDate))
            {
                return null;
            }

            if (int.TryParse(history.Hour, out var hour) && hour >= 0)
            {
                recordDate = recordDate.AddHours(hour);
            }

            short calorie;
            if (string.IsNullOrWhiteSpace(history.Calorie))
            {
                calorie = short.MinValue;
            }
            else if (!short.TryParse(history.Calorie, out calorie))
            {
                return null;
            }

            return new CalomealMealSyncItem()
            {
                AnalysisSet = JsonConvert.SerializeObject(history),
                AnalysisType = 5,
                Calorie = calorie,
                ChooseSet = JsonConvert.SerializeObject(history),
                DeleteFlag = ToBool(history.IsDeleted),
                HasImage = ToBool(history.HasImage),
                HistoryId = int.TryParse(history.MealHistoryId, out var historyId) ? historyId : int.MinValue,
                ItemName = history.MenuName ?? string.Empty,
                MealType = ToMealType(history.MealType),
                Rate = 1m,
                RecordDate = recordDate
            };
        }

        private static byte ToMealType(string mealType)
        {
            switch (mealType)
            {
                case "朝食":
                    return 1;
                case "昼食":
                    return 2;
                case "夕食":
                    return 3;
                case "間食":
                    return 4;
                default:
                    return byte.MinValue;
            }
        }

        private static bool ToBool(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            if (bool.TryParse(value, out var boolValue))
            {
                return boolValue;
            }

            return value == "1";
        }

        private static int ToUnixTime(DateTime targetTime)
        {
            var unspecified = targetTime.Kind == DateTimeKind.Unspecified
                ? targetTime
                : DateTime.SpecifyKind(targetTime, DateTimeKind.Unspecified);
            var utc = TimeZoneInfo.ConvertTimeToUtc(unspecified, JstTimeZone);
            return Convert.ToInt32((utc - UnixEpoch).TotalSeconds);
        }

        private static TimeZoneInfo GetJstTimeZoneCore()
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
            }
            catch
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Asia/Tokyo");
            }
        }
    }

    internal sealed class CalomealMealSyncExecutionResult
    {
        public DateTime TargetDateTime { get; set; }
        public double TimeSpanInHours { get; set; }
        public int ProcessedCount { get; set; }
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public int AddedCount { get; set; }
        public int ModifiedCount { get; set; }
        public int DeletedCount { get; set; }
        public bool TokenRefreshed { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> ErrorMessages { get; } = new List<string>();
    }

    internal sealed class CalomealMealSyncItem
    {
        public DateTime RecordDate { get; set; } = DateTime.MinValue;
        public byte MealType { get; set; } = byte.MinValue;
        public string ItemName { get; set; } = string.Empty;
        public short Calorie { get; set; } = short.MinValue;
        public byte AnalysisType { get; set; } = byte.MinValue;
        public string AnalysisSet { get; set; } = string.Empty;
        public string ChooseSet { get; set; } = string.Empty;
        public decimal Rate { get; set; } = decimal.MinValue;
        public bool DeleteFlag { get; set; }
        public int HistoryId { get; set; } = int.MinValue;
        public bool HasImage { get; set; }
    }

    internal abstract class CalomealResponseBase
    {
        [JsonProperty("error")]
        public string Error { get; set; } = string.Empty;

        [JsonIgnore]
        public bool IsSuccess { get; set; }

        [JsonIgnore]
        public int StatusCode { get; set; }

        [JsonIgnore]
        public string ResponseString { get; set; } = string.Empty;
    }

    internal sealed class CalomealAccessTokenResponse : CalomealResponseBase
    {
        [JsonProperty("token_type")]
        public string TokenType { get; set; } = string.Empty;

        [JsonProperty("expires_in")]
        public string ExpiresIn { get; set; } = string.Empty;

        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; } = string.Empty;
    }

    internal sealed class CalomealMealResponse : CalomealResponseBase
    {
        [JsonProperty("meal_histories")]
        public List<CalomealMealHistory> MealHistories { get; set; } = new List<CalomealMealHistory>();
    }

    internal sealed class CalomealMealHistory
    {
        [JsonProperty("meal_history_id")]
        public string MealHistoryId { get; set; } = string.Empty;

        [JsonProperty("is_deleted")]
        public string IsDeleted { get; set; } = string.Empty;

        [JsonProperty("meal_date")]
        public string MealDate { get; set; } = string.Empty;

        [JsonProperty("meal_type")]
        public string MealType { get; set; } = string.Empty;

        [JsonProperty("hour")]
        public string Hour { get; set; } = string.Empty;

        [JsonProperty("menu_name")]
        public string MenuName { get; set; } = string.Empty;

        [JsonProperty("calorie")]
        public string Calorie { get; set; } = string.Empty;

        [JsonProperty("has_image")]
        public string HasImage { get; set; } = string.Empty;
    }

    internal sealed class CalomealWebViewApiSettings
    {
        private const string ApiUriKey = "CalomealWebViewApiUri";
        private const string ClientIdKey = "CalomealWebViewApiCliantId";
        private const string SecretKeyKey = "CalomealWebViewApiSecretKey";
        private const string EncryptedKey = "UseEncryptedCalomealWebViewApiSettings";
        private const string UserAgentKey = "CalomealWebViewApiUserAgent";
        private const string TimeoutKey = "CalomealWebViewApiTimeOut";
        private const string RedirectUriKey = "RedirectUrl";

        public string ApiUri { get; private set; }
        public string ClientId { get; private set; }
        public string SecretKey { get; private set; }
        public string RedirectUri { get; private set; }
        public string UserAgent { get; private set; }
        public int TimeoutSeconds { get; private set; }

        public static CalomealWebViewApiSettings Load()
        {
            var useEncrypted = TryGetBool(EncryptedKey, true);
            var apiUri = GetRequired(ApiUriKey);
            if (!apiUri.EndsWith("/"))
            {
                apiUri += "/";
            }

            if (!Uri.TryCreate(apiUri, UriKind.Absolute, out _))
            {
                throw new ConfigurationErrorsException("CalomealWebViewApiUri の設定が不正です。");
            }

            return new CalomealWebViewApiSettings()
            {
                ApiUri = apiUri,
                ClientId = GetSecret(ClientIdKey, useEncrypted),
                SecretKey = GetSecret(SecretKeyKey, useEncrypted),
                RedirectUri = GetRequired(RedirectUriKey),
                UserAgent = TryGet(UserAgentKey, string.Empty),
                TimeoutSeconds = TryGetInt(TimeoutKey, 60)
            };
        }

        private static string GetRequired(string key)
        {
            var value = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ConfigurationErrorsException($"{key} の設定が見つかりません。");
            }
            return value;
        }

        private static string TryGet(string key, string defaultValue)
        {
            var value = ConfigurationManager.AppSettings[key];
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }

        private static int TryGetInt(string key, int defaultValue)
        {
            return int.TryParse(ConfigurationManager.AppSettings[key], out var value) ? value : defaultValue;
        }

        private static bool TryGetBool(string key, bool defaultValue)
        {
            return bool.TryParse(ConfigurationManager.AppSettings[key], out var value) ? value : defaultValue;
        }

        private static string GetSecret(string key, bool encrypted)
        {
            var value = GetRequired(key);
            if (!encrypted)
            {
                return value;
            }

            using (var cryptor = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
            {
                return cryptor.DecryptString(value);
            }
        }
    }
}