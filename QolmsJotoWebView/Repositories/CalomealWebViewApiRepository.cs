using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsCalomealWebViewApiCoreV1;
using MGF.QOLMS.QolmsJotoWebView.Models;
using MGF.QOLMS.QolmsJotoWebView.Repositories;
using System;
using System.Threading.Tasks;

namespace MGF.QOLMS.QolmsJotoWebView.Repositories
{
    /// <summary>
    /// カロミル情報入出力インターフェース
    /// </summary>
    public interface ICalomealWebViewApiRepository
    {
        /// <summary>
        /// ユーザー認証コードからアクセストークンを発行する
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        CalomealAccessTokenSet GetNewToken(string code);

        /// <summary>
        /// フレッシュトークンからアクセストークンを再発行するカロミルAPIを呼び出します。
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        CalomealAccessTokenSet GetRefreshToken(string refreshToken);

        /// <summary>
        /// カロミルアクセストークンを登録して、新規登録に必要な基本情報を取得します。
        /// </summary>
        /// <param name="mainModel"></param>
        /// <param name="linkgaSystemId"></param>
        /// <param name="token"></param>
        /// <param name="deleteFlag"></param>
        /// <returns></returns>
        bool SetProfile(string token, DateTime birthDay, decimal height, decimal weight, QjSexTypeEnum sex);
    }

    public class CalomealWebViewApiRepository : ICalomealWebViewApiRepository
    {

        /// <summary>
        /// ユーザー認証コードからアクセストークンを発行する
        /// </summary>
        public CalomealAccessTokenSet GetNewToken(string code)
        {
            // DebugLog("ExecuteAccessTokenApi");

            var apiArgs = new AccessTokenApiArgs()
            {
                grant_type = "authorization_code",
                client_id = QjConfiguration.CalomealApiClientID,
                client_secret = QjConfiguration.CalomealApiClientSecret,
                redirect_uri = QjConfiguration.CalomealApiRedirectUrl,
                code = code
            };

            //var request = MakeCyptDataString(apiArgs);
            //// DebugLog(request);
            //AccessLogWorker.WriteAccessLog(null, "/note/calomeal/api_accesstoken",
            //    AccessLogWorker.AccessTypeEnum.Api, request);

            Task<AccessTokenApiResults> apiResults;

            Func<Task<AccessTokenApiResults>, AccessTokenApiResults> callback =
                (antecedent) =>
                {
                    var gid = Guid.NewGuid();
                    //FileLog($" {gid}: ExecuteAccessTokenApi");

                    if (antecedent.Status == TaskStatus.RanToCompletion)
                    {
                        if (antecedent.Result.IsSuccess)
                        {
                            //FileLog($" {gid}: {antecedent.Result.IsSuccess}");
                        }
                        else
                        {
                            //FileLog($" {gid}: {antecedent.Result.IsSuccess}");
                            //FileLog($" {gid}: statusCode = {antecedent.Result.StatusCode} , error ={antecedent.Result.error}");
                            //FileLog($" {gid}: ResponseString = {antecedent.Result.ResponseString}");
                        }
                    }
                    else if (antecedent.Status == TaskStatus.Faulted)
                    {
                        //FileLog($" {gid}: {antecedent.Exception?.GetBaseException().Message}");
                        //FileLog($" {gid}: {antecedent.Exception?.GetBaseException().StackTrace}");
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

            var result = apiResults.Result;
            // DebugLog(result.ResponseString);
            AccessLogWorker.WriteAccessLog(null, "/note/calomeal/api_accesstoken",
                AccessLogWorker.AccessTypeEnum.Api, result.ResponseString);

            if (result.IsSuccess)
            {
                var tokenSet = new CalomealAccessTokenSet()
                {
                    access_token = result.access_token,
                    expires_in = result.expires_in,
                    RefreshTokenExpires = DateTime.MaxValue,
                    refresh_token = result.refresh_token,
                    TokenExpires = DateTime.Now.AddSeconds(int.Parse(result.expires_in)),
                    token_type = result.token_type
                };

                return tokenSet;
            }

            return new CalomealAccessTokenSet();
        }

        /// <summary>
        /// フレッシュトークンからアクセストークンを再発行する
        /// カロミルAPIを呼び出します。
        /// </summary>
        public CalomealAccessTokenSet GetRefreshToken(string refreshToken)
        {
            var apiArgs = new AccessTokenApiArgs()
            {
                grant_type = "refresh_token",
                client_id = QjConfiguration.CalomealApiClientID,
                client_secret = QjConfiguration.CalomealApiClientSecret,
                redirect_uri = QjConfiguration.CalomealApiRedirectUrl,
                refresh_token = refreshToken
            };

            //var request = MakeCyptDataString(apiArgs);
            //// DebugLog(request);
            //AccessLogWorker.WriteAccessLog(null, "/note/calomeal/api_accesstoken",
            //    AccessLogWorker.AccessTypeEnum.Api, request);

            Task<AccessTokenApiResults> apiResults;

            Func<Task<AccessTokenApiResults>, AccessTokenApiResults> callback =
                (antecedent) =>
                {
                    var gid = Guid.NewGuid();
                    //FileLog($" {gid}: ExecuteAccessTokenApi");

                    if (antecedent.Status == TaskStatus.RanToCompletion)
                    {
                        if (antecedent.Result.IsSuccess)
                        {
                            //FileLog($" {gid}: {antecedent.Result.IsSuccess}");
                        }
                        else
                        {
                            //FileLog($" {gid}: {antecedent.Result.IsSuccess}");
                            //FileLog($" {gid}: statusCode = {antecedent.Result.StatusCode} , error ={antecedent.Result.error}");
                            //FileLog($" {gid}: ResponseString = {antecedent.Result.ResponseString}");
                        }
                    }
                    else if (antecedent.Status == TaskStatus.Faulted)
                    {
                        //FileLog($" {gid}: {antecedent.Exception?.GetBaseException().Message}");
                        //FileLog($" {gid}: {antecedent.Exception?.GetBaseException().StackTrace}");
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

            var result = apiResults.Result;
            // DebugLog(result.ResponseString);
            AccessLogWorker.WriteAccessLog(null, "/note/calomeal/api_accesstoken",
                AccessLogWorker.AccessTypeEnum.Api, result.ResponseString);

            if (result.IsSuccess)
            {
                var tokenSet = new CalomealAccessTokenSet()
                {
                    access_token = result.access_token,
                    expires_in = result.expires_in,
                    RefreshTokenExpires = DateTime.MaxValue,
                    refresh_token = refreshToken,
                    TokenExpires = DateTime.Now.AddSeconds(int.Parse(result.expires_in)),
                    token_type = result.token_type
                };

                return tokenSet;
            }

            return new CalomealAccessTokenSet();
        }

        /// <summary>
        /// 指定されたアクセストークンに該当するユーザーのプロフィール情報を設定する
        /// カロミルAPIを呼び出します。
        /// </summary>
        public bool SetProfile(string token, DateTime birthDay, decimal height, decimal weight, QjSexTypeEnum sex)
        {
            try
            {
                var apiArgs = new SetProfileApiArgs()
                {
                    Token = token,
                    BirthDay = birthDay.ToString("yyyy/MM/dd"),
                    Sex = sex.ToString("d"),
                    Height = height,
                    Weight = weight
                };

                //var request = MakeCyptDataString(apiArgs);
                //// DebugLog(request);
                //AccessLogWorker.WriteAccessLog(null, "/note/calomeal/api_setprofile",
                //    AccessLogWorker.AccessTypeEnum.Api, request);

                Task<SetProfileApiResults> apiResults;

                Func<Task<SetProfileApiResults>, SetProfileApiResults> callback =
                    (antecedent) =>
                    {
                        var gid = Guid.NewGuid();
                        //FileLog($" {gid}: ExecuteSetProfileApi");

                        if (antecedent.Status == TaskStatus.RanToCompletion)
                        {
                            if (antecedent.Result.IsSuccess)
                            {
                                //FileLog($" {gid}: {antecedent.Result.IsSuccess}");
                            }
                            else
                            {
                                //FileLog($" {gid}: {antecedent.Result.IsSuccess}");
                                //FileLog($" {gid}: statusCode = {antecedent.Result.StatusCode} , error ={antecedent.Result.error}");
                                //FileLog($" {gid}: ResponseString = {antecedent.Result.ResponseString}");
                            }
                        }
                        else if (antecedent.Status == TaskStatus.Faulted)
                        {
                            //FileLog($" {gid}: {antecedent.Exception?.GetBaseException().Message}");
                            //FileLog($" {gid}: {antecedent.Exception?.GetBaseException().StackTrace}");
                            return new SetProfileApiResults() { error = antecedent.Exception?.GetBaseException().Message };
                        }

                        return antecedent.Result;
                    };

                apiResults = QsCalomealWebViewApiManager.ExecuteAsync<SetProfileApiArgs, SetProfileApiResults>(apiArgs, callback);

                var result = apiResults.Result;
                // DebugLog(result.ResponseString);
                AccessLogWorker.WriteAccessLog(null, "/note/calomeal/api_setprofile",
                    AccessLogWorker.AccessTypeEnum.Api, result.ResponseString);

                return result.IsSuccess && result.StatusCode == 200;
            }
            catch (Exception ex)
            {
                AccessLogWorker.WriteErrorLog(null, "/note/calomeal/api_setprofile_error", ex.Message);
            }
            return false;
        }
    }

}
