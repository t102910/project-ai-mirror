using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsJotoWebView.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;

namespace MGF.QOLMS.QolmsJotoWebView.Repositories
{
    public interface IPointRepository
    {
        /// <summary>
        /// 現在のポイントと直近の有功期限、
        /// その有効期限で失効するポイントを取得します。
        /// </summary>
        /// <param name="apiExecutor">Web API の実行者アカウント キー。</param>
        /// <param name="apiExecutorName">Web API の実行者名。</param>
        /// <param name="sessionId">セッション ID。</param>
        /// <param name="apiAuthorizeKey">API 認証キー。</param>
        /// <param name="targetAccountKey">対象者アカウント キー。</param>
        /// <param name="refClosestExprirationDate">直近の有効期限が格納される変数。</param>
        /// <param name="refClosestExprirationPoint">直近の有効期限で失効するポイントが格納される変数。</param>
        /// <returns>
        /// 現在のポイント。
        /// </returns>
        int GetQolmsPointWithClosestExpriration(Guid apiExecutor, string apiExecutorName, string sessionId, Guid apiAuthorizeKey, Guid targetAccountKey, int serviceNo, ref DateTime refClosestExprirationDate, ref int refClosestExprirationPoint);

        List<QoApiQolmsPointHistoryResultItem> GetTargetPointFromHistoryList(Guid apiExecutor, string apiExecutorName, string sessionId, Guid apiAuthorizeKey, Guid targetAccountKey, int serviceNo, QjPointItemTypeEnum targetPointItemType, DateTime fromDate, DateTime toDate);

        Dictionary<string, int> AddQolmsPoints(Guid apiExecutor, string apiExecutorName, string sessionId, Guid apiAuthorizeKey, Guid targetAccountKey, int serviceNo, List<QolmsPointGrantItem> pointList, int updateFlag = 0);

        QhYappliQolmsPointRetryWriteApiResults ExecuteQolmsPointRetryLogWriteApi(Guid apiExecutor, string apiExecutorName, string sessionId, Guid apiAuthorizeKey, Guid accountKey, DateTime actionDate, string callerSystemName, string statusCode, string message, int pointItemNo, string pointRequestString);

        int GetQolmsPoint(Guid apiExecutor, string apiExecutorName, string sessionId, Guid apiAuthorizeKey, Guid targetAccountKey);
    }

    public class PointRepository : IPointRepository
    {
        /// <summary>
        /// ロック オブジェクトを表します。
        /// </summary>
        private readonly object _lockObject = new object();
        private Dictionary<string, DateTime> _checkAccount = new Dictionary<string, DateTime>();

        //private const int SERVICENO = 47003;

        #region "Private Method"

        /// <summary>
        /// ポイント付与失敗のログを登録するAPIを実行します。
        /// </summary>
        public QhYappliQolmsPointRetryWriteApiResults ExecuteQolmsPointRetryLogWriteApi(
            Guid apiExecutor,
            string apiExecutorName,
            string sessionId,
            Guid apiAuthorizeKey,
            Guid accountKey,
            DateTime actionDate,
            string callerSystemName,
            string statusCode,
            string message,
            int pointItemNo,
            string pointRequestString)
        {
            var apiArgs = new QhYappliQolmsPointRetryWriteApiArgs(
                QhApiTypeEnum.YappliQolmsPointRetryWrite,
                QsApiSystemTypeEnum.Qolms,
                apiExecutor,
                apiExecutorName
            )
            {
                AccountKey = accountKey.ToApiGuidString(),
                ActionDate = actionDate.ToApiDateString(),
                CallerSystemName = "JotoSite",
                Message = message,
                PointItemNo = pointItemNo.ToString(),
                PointRequest = pointRequestString,
                StatusCode = statusCode
            };

            var apiResults = QsApiManager.ExecuteQolmsApi<QhYappliQolmsPointRetryWriteApiResults>(
                apiArgs,
                sessionId,
                apiAuthorizeKey
            );

            if (apiResults.IsSuccess.TryToValueType(false))
            {
                return apiResults;
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format(
                        "{0} APIの実行に失敗しました。",
                        QsApiManager.GetQolmsApiName(apiArgs)
                    )
                );
            }
        }

        private static string MakeCyptString(string str)
        {
            try
            {
                using (var crypt = new QOLMS.QolmsCryptV1.QsCrypt(QolmsCryptV1.QsCryptTypeEnum.QolmsSystem))
                {
                    return crypt.EncryptString(str);
                }
            }
            catch (Exception ex)
            {
                // ignore
            }
            return "";
        }

        private static string MakeCyptDataString(QoQolmsPointWriteApiArgs args)
        {
            try
            {
                using (var crypt = new QOLMS.QolmsCryptV1.QsCrypt(QolmsCryptV1.QsCryptTypeEnum.QolmsSystem))
                {
                    using (var ms = new MemoryStream())
                    {
                        var serializer = new DataContractJsonSerializer(args.GetType());
                        serializer.WriteObject(ms, args);
                        return crypt.EncryptString(Encoding.UTF8.GetString(ms.ToArray()));
                    }
                }
            }
            catch (Exception ex)
            {
                // ignore
            }
            return "";
        }

        //private static string MakeCyptDataString(QoQolmsPointWriteApiResults args)
        //{
        //    try
        //    {
        //        using (var crypt = new QOLMS.QolmsCryptV1.QsCrypt(QolmsCryptV1.QsCryptTypeEnum.QolmsSystem))
        //        {
        //            using (var ms = new MemoryStream())
        //            {
        //                var serializer = new DataContractJsonSerializer(args.GetType());
        //                serializer.WriteObject(ms, args);
        //                return crypt.EncryptString(Encoding.UTF8.GetString(ms.ToArray()));
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // ignore
        //    }
        //    return "";
        //}

        //#endregion

        //#region "Public Method"

        /// <summary>
        /// 複数のポイントをリストで追加します。
        /// </summary>
        public Dictionary<string, int> AddQolmsPoints(
            Guid apiExecutor,
            string apiExecutorName,
            string sessionId,
            Guid apiAuthorizeKey,
            Guid targetAccountKey,
            int serviceNo,
            List<QolmsPointGrantItem> pointList,
            int updateFlag = 0)
        {

            var apiArgs = new QoQolmsPointWriteApiArgs(
                QoApiTypeEnum.QolmsPointWrite,
                QsApiSystemTypeEnum.Qolms,
                apiExecutor,
                apiExecutorName
            )
            {
                ActionServiceNo = serviceNo.ToString(),
                ActionDataList = new List<QoApiQolmsPointGrantActionItem>()
            };

            if (pointList != null)
            {
                foreach (var item in pointList)
                {
                    apiArgs.ActionDataList.Add(
                        new QoApiQolmsPointGrantActionItem()
                        {
                            AccountKey = targetAccountKey.ToApiGuidString(),
                            SerialNo = item.SerialCode,
                            ActionDate = item.ActionDate.ToApiDateString(),
                            ActionPoint = item.Point.ToString(),
                            PointTargetDate = item.PointTargetDate.ToApiDateString(),
                            ActionReason = item.Reason,
                            PointExpirationDate = string.Format("{0:yyyy/MM/dd}", item.PointExpirationDate),
                            PointItemNo = item.PointItemNo.ToString(),
                            UpdateFlag = updateFlag.ToString()
                        }
                    );
                }
            }

            var actionDate = pointList.First().ActionDate;
            var pointItemNo = pointList.First().PointItemNo;
            var errorMessage = string.Empty;
            var statusCode = string.Empty;
            var mailMessage = string.Empty;
            var isRequested = false;
            var checkKey = string.Empty;

            try
            {
                checkKey = string.Format("{0}{1}", apiArgs.ActionDataList.First().AccountKey, apiArgs.ActionDataList.First().PointItemNo);

                if (pointList.First().Point > 0)
                {
                    // 加算のみリクエストを重ねないようにする
                    lock (_lockObject)
                    {
                        if (_checkAccount.ContainsKey(checkKey))
                        {
                            if (_checkAccount[checkKey] > DateTime.Now.AddMinutes(-1))
                            {
                                isRequested = true;
                                throw new Exception("既にリクエスト中");
                            }
                            else
                            {
                                _checkAccount[checkKey] = DateTime.Now;
                            }
                        }
                        else
                        {
                            _checkAccount.Add(checkKey, DateTime.Now);
                        }
                    }
                }

                var apiResult = QsApiManager.ExecuteQolmsOpenApi<QoQolmsPointWriteApiResults>(
                    apiArgs,
                    sessionId,
                    apiAuthorizeKey
                );

                if (apiResult.IsSuccess.TryToValueType(false))
                {
                    var results = new Dictionary<string, int>();
                    var errorExists = false;

                    if (apiResult.IsSuccess.TryToValueType(false))
                    {
                        foreach (var result in apiResult.ResultList)
                        {
                            results.Add(result.SerialNo, result.ErrorCode.TryToValueType(0));

                            if (result.ErrorCode.TryToValueType(0) > 0 &&
                                result.ErrorCode.TryToValueType(QoApiPointGrantActionErrorTypeEnum.None) !=
                                QoApiPointGrantActionErrorTypeEnum.FrequencyLimit)
                            {
                                errorExists = true;
                            }
                        }

                        // 頻度チェック以外のエラー発生したらメール
                        if (errorExists)
                        {
                            //NoticeMailWorker.Send(
                            //    string.Format(
                            //        "{0:yyyy/MM/dd HH:mm}: AddQolmsPointsポイント付与に失敗しました。{1}result:{2}",
                            //        DateTime.Now,
                            //        Environment.NewLine,
                            //        MakeCyptDataString(apiResult)
                            //    )
                            //);
                        }

                        return results;
                    }
                }
                else
                {
                    var resultCode = (apiResult.Result == null ? "不明" : apiResult.Result.Code).ToString();
                    statusCode = resultCode;
                    mailMessage = string.Format(
                        "{0:yyyy/MM/dd HH:mm}: {1} APIの実行に失敗しました。{2}{3}{2} ResultCode={4}",
                        DateTime.Now,
                        QsApiManager.GetQolmsOpenApiName(apiArgs),
                        Environment.NewLine,
                        MakeCyptDataString(apiArgs),
                        resultCode
                    );
                }
            }
            catch (HttpException ex)
            {
                var code = ex.GetHttpCode();
                statusCode = code.ToString();
                errorMessage = ex.Message;
                mailMessage = string.Format(
                    "{0:yyyy/MM/dd HH:mm}: {1} APIの実行に失敗しました。{2}{3}{2} ex.Message={4}",
                    DateTime.Now,
                    QsApiManager.GetQolmsOpenApiName(apiArgs),
                    Environment.NewLine,
                    MakeCyptDataString(apiArgs),
                    ex.Message
                );
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                mailMessage = string.Format(
                    "{0:yyyy/MM/dd HH:mm}: {1} APIの実行に失敗しました。{2}{3}{2} ex.Message={4}",
                    DateTime.Now,
                    QsApiManager.GetQolmsOpenApiName(apiArgs),
                    Environment.NewLine,
                    MakeCyptDataString(apiArgs),
                    ex.Message
                );
            }
            finally
            {
                if (pointList.First().Point > 0 && isRequested == false)
                {
                    lock (_lockObject)
                    {
                        if (!string.IsNullOrEmpty(checkKey) && _checkAccount.ContainsKey(checkKey))
                        {
                            _checkAccount.Remove(checkKey);
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(statusCode) || !string.IsNullOrWhiteSpace(errorMessage))
                {
                    ExecuteQolmsPointRetryLogWriteApi(
                        apiExecutor,
                        apiExecutorName,
                        sessionId,
                        apiAuthorizeKey,
                        targetAccountKey,
                        actionDate,
                        "",
                        statusCode,
                        MakeCyptString(errorMessage),
                        pointItemNo,
                        MakeCyptDataString(apiArgs)
                    );

                    //NoticeMailWorker.Send(mailMessage);
                }
            }

            throw new InvalidOperationException(
                string.Format(
                    "{0} APIの実行に失敗しました。",
                    QsApiManager.GetQolmsOpenApiName(apiArgs)
                )
            );


            return null;
        }

        ///// <summary>
        ///// 複数のポイントをリストで削除します。
        ///// </summary>
        //public static Dictionary<string, int> RemoveQolmsPoints(
        //    Guid apiExecutor,
        //    string apiExecutorName,
        //    string sessionId,
        //    Guid apiAuthorizeKey,
        //    Guid targetAccountKey,
        //    List<QolmsPointGrantItem> pointList)
        //{
        //    var errorExists = false;
        //    var apiResult = ExecuteQolmsPointWriteApi(
        //        apiExecutor,
        //        apiExecutorName,
        //        sessionId,
        //        apiAuthorizeKey,
        //        targetAccountKey,
        //        SERVICENO,
        //        pointList,
        //        9
        //    );

        //    var results = new Dictionary<string, int>();

        //    if (apiResult.IsSuccess.TryToValueType(false))
        //    {
        //        foreach (var result in apiResult.ResultList)
        //        {
        //            results.Add(result.SerialNo, result.ErrorCode.TryToValueType(0));

        //            if (result.ErrorCode.TryToValueType(0) > 0)
        //            {
        //                errorExists = true;
        //            }
        //        }

        //        // エラー発生したらメール
        //        if (errorExists)
        //        {
        //            //NoticeMailWorker.Send(
        //            //    string.Format(
        //            //        "{0:yyyy/MM/dd HH:mm}: AddQolmsPointsポイント付与に失敗しました。{1}result:{2}",
        //            //        DateTime.Now,
        //            //        Environment.NewLine,
        //            //        MakeCyptDataString(apiResult)
        //            //    )
        //            //);
        //        }

        //        return results;
        //    }

        //    return null;
        //}

        /// <summary>
        /// 現在のポイントを返します。呼び出すAPIで同時に直近の有効期限と期限切れになるポイントを取得できるので必要なら同様のFunctionを追加してください。
        /// </summary>
        public int GetQolmsPoint(
            Guid apiExecutor,
            string apiExecutorName,
            string sessionId,
            Guid apiAuthorizeKey,
            Guid targetAccountKey)
        {
            var ref1 = DateTime.MinValue;
            var ref2 = int.MinValue;

            return GetQolmsPointWithClosestExpriration(
                apiExecutor,
                apiExecutorName,
                sessionId,
                apiAuthorizeKey,
                targetAccountKey,
                QjConfiguration.PointServiceno,
                ref ref1,
                ref ref2
            );
        }

        /// <summary>
        /// 現在のポイントと直近の有功期限、
        /// その有効期限で失効するポイントを取得します。
        /// </summary>
        /// <param name="apiExecutor">Web API の実行者アカウント キー。</param>
        /// <param name="apiExecutorName">Web API の実行者名。</param>
        /// <param name="sessionId">セッション ID。</param>
        /// <param name="apiAuthorizeKey">API 認証キー。</param>
        /// <param name="targetAccountKey">対象者アカウント キー。</param>
        /// <param name="refClosestExprirationDate">直近の有効期限が格納される変数。</param>
        /// <param name="refClosestExprirationPoint">直近の有効期限で失効するポイントが格納される変数。</param>
        /// <returns>
        /// 現在のポイント。
        /// </returns>
        public int GetQolmsPointWithClosestExpriration(Guid apiExecutor, string apiExecutorName, string sessionId, Guid apiAuthorizeKey, Guid targetAccountKey, int serviceNo, ref DateTime refClosestExprirationDate, ref int refClosestExprirationPoint)
        {
            var apiArgs = new QoQolmsPointReadApiArgs(
                QoApiTypeEnum.QolmsPointRead,
                QsApiSystemTypeEnum.Qolms,
                apiExecutor,
                apiExecutorName
            )
            {
                AccountKey = targetAccountKey.ToApiGuidString(),
                ActionServiceNo = serviceNo.ToString()
            };

            var apiResults = QsApiManager.ExecuteQolmsOpenApi<QoQolmsPointReadApiResults>(
                apiArgs,
                sessionId,
                apiAuthorizeKey
            );

            if (apiResults.IsSuccess.TryToValueType(false))
            {
                if (apiResults.IsSuccess.TryToValueType(false))
                {
                    // QO_QOLMSPOINT_DAT にデータ有り
                    refClosestExprirationDate = apiResults.ClosestExprirationDate.TryToValueType(DateTime.MinValue);
                    refClosestExprirationPoint = apiResults.ColsestExprirationPoint.TryToValueType(0); // 綴りミス

                    return apiResults.Point.TryToValueType(0);
                }
                else
                {
                    // QO_QOLMSPOINT_DAT にデータ無し
                    refClosestExprirationDate = DateTime.MinValue;
                    refClosestExprirationPoint = 0;

                    return 0;
                }
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format(
                        "{0} APIの実行に失敗しました。",
                        QsApiManager.GetQolmsOpenApiName(apiArgs)
                    )
                );
            }
        }

        ///// <summary>
        ///// 指定した日付のポイント履歴を取得します。
        ///// </summary>
        //public static int GetTargetPointFromHistory(
        //    Guid apiExecutor,
        //    string apiExecutorName,
        //    string sessionId,
        //    Guid apiAuthorizeKey,
        //    Guid targetAccountKey,
        //    QjPointItemTypeEnum targetPointItemType,
        //    DateTime targetDate)
        //{
        //    var apiResult = ExecuteQolmsPointHistoryReadApi(
        //        apiExecutor,
        //        apiExecutorName,
        //        sessionId,
        //        apiAuthorizeKey,
        //        targetAccountKey,
        //        SERVICENO,
        //        DateTime.MinValue,
        //        DateTime.Now
        //    );

        //    if (apiResult.IsSuccess.TryToValueType(false))
        //    {
        //        return apiResult.PointHistoryList
        //            .Where(m => m.PointTargetDate.TryToValueType(DateTime.MinValue).Date == targetDate.Date &&
        //                        (int)targetPointItemType == m.PointItemNo.TryToValueType(int.MinValue))
        //            .Sum(m => m.PointValue.TryToValueType(0));
        //    }

        //    return int.MinValue;
        //}

        /// <summary>
        /// 指定した期間のポイント履歴のリストを取得します。
        /// </summary>
        public List<QoApiQolmsPointHistoryResultItem> GetTargetPointFromHistoryList(
            Guid apiExecutor,
            string apiExecutorName,
            string sessionId,
            Guid apiAuthorizeKey,
            Guid targetAccountKey,
            int serviceNo,
            QjPointItemTypeEnum targetPointItemType,
            DateTime fromDate,
            DateTime toDate)
        {
            var apiArgs = new QoQolmsPointHistoryReadApiArgs(
                QoApiTypeEnum.QolmsPointHistoryRead,
                QsApiSystemTypeEnum.Qolms,
                apiExecutor,
                apiExecutorName
            )
            {
                ActionServiceNo = serviceNo.ToString(),
                Filter = new QoApiQolmsPointHistoryFilter()
                {
                    AccountKey = targetAccountKey.ToApiGuidString(),
                    ContainDeletedItem = bool.FalseString,
                    StartDate = fromDate.ToApiDateString(),
                    EndDate = toDate.ToApiDateString()
                }
            };

            var apiResults = QsApiManager.ExecuteQolmsOpenApi<QoQolmsPointHistoryReadApiResults>(
                apiArgs,
                sessionId,
                apiAuthorizeKey
            );

            if (apiResults.IsSuccess.TryToValueType(false))
            {
                return apiResults.PointHistoryList;
            }

            return null;
        }

        public Dictionary<string, int> AddQolmsPoints(Guid apiExecutor, string apiExecutorName, string sessionId, Guid apiAuthorizeKey, Guid targetAccountKey, int serviceNo, List<QolmsPointGrantItem> pointList)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
