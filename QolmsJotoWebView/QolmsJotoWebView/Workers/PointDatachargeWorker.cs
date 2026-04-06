using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsJotoWebView.Models;
using MGF.QOLMS.QolmsJotoWebView.Repositories;
using MGF.QOLMS.QolmsJotoWebView.Workers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MGF.QOLMS.QolmsJotoWebView
{
    internal sealed class PointDatachargeWorker
    {

        IPointRepository _pointRepo;
        IDatachargeRepository _datachargeRepo;

        #region Enum

        /// <summary>
        /// データチャージログの種別を表します。
        /// </summary>
        public enum ActionTypeEnum : byte
        {
            /// <summary>
            /// 未指定です。
            /// </summary>
            None = QsApiDatachargeActionTypeEnum.None,

            /// <summary>
            /// リクエストです。
            /// </summary>
            Request = QsApiDatachargeActionTypeEnum.Request,

            /// <summary>
            /// レスポンスです。
            /// </summary>
            Response = QsApiDatachargeActionTypeEnum.Response,

            /// <summary>
            /// エラーです。
            /// </summary>
            Error = QsApiDatachargeActionTypeEnum.Error
        }

        #endregion

        #region Constant

        //// TODO: 検証サーバーで試す、リクエストIDの発行
        ///// <summary>加盟店ID</summary>
        //public  readonly string KAMEITENID = ConfigurationManager.AppSettings["AuPaymentKameitenId"];

        ///// <summary>要求URI</summary>
        //public  readonly string REQUESTURI = ConfigurationManager.AppSettings["AuDatachargeUri"];

        ///// <summary>テストサーバーの仮想日</summary>
        //public  readonly string TESTSERVER_VIRTUALDATE = ConfigurationManager.AppSettings["AuPaymentTestServerVirtualDate"];

        ///// <summary>エラーでメールを送る</summary>
        //public  readonly string IsSendMail = ConfigurationManager.AppSettings["IsSendMailDatacharge"];

        #endregion

        #region Constructor

        /// <summary>
        /// デフォルト コンストラクタは使用できません。
        /// </summary>
        public PointDatachargeWorker(IPointRepository pointRepository, IDatachargeRepository datachargeRepository)
        {

            _pointRepo = pointRepository;
            _datachargeRepo = datachargeRepository;

        }

        #endregion

        #region Private Method

        private  bool PostDataChargeRequest(QolmsJotoModel mainModel, string eventId, string serialCode)
        {
            DateTime now = DateTime.Now;
            // Log
            QhYappliPortalDatachageLogWriteApiResults result = _datachargeRepo.ExecuteDatachargeLogWriteApi(
                mainModel, (int)ActionTypeEnum.Request, "", eventId, now.ToString("yyyyMMddhhmmss"), "", 0, "", serialCode, "");
            string requestId = result.RequestId;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            try
            {
                // リクエストデータ
                NameValueCollection data = HttpUtility.ParseQueryString(string.Empty);

                if (QjConfiguration.AuDatachargeUri.Contains("test."))
                {
                    // テストは未来日に
                    // 検証サーバの日付ずれてるため
                    if (!string.IsNullOrEmpty(QjConfiguration.AuPaymentTestServerVirtualDate))
                    {
                        DateTime virtualdate = now.Date;
                        DateTime.TryParseExact(QjConfiguration.AuPaymentTestServerVirtualDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out virtualdate);
                        data["RequestDate"] = virtualdate.ToString("yyyyMMddHHmmss");
                    }
                }
                else
                {
                    data["RequestDate"] = now.ToString("yyyyMMddHHmmss");
                }

                data["RequestId"] = requestId; // 16桁
                data["Command"] = "DTC00201";
                data["AuId"] = this.GetAuSystemID(mainModel.AuthorAccount.OpenId);
                data["MemberId"] = QjConfiguration.AuPaymentKameitenId;
                data["EventId"] = HttpUtility.UrlEncode(eventId, System.Text.Encoding.UTF8);
                byte[] byteArray = Encoding.UTF8.GetBytes(data.ToString());

                // デバッグログ
                DebugLog("Postパラメータ");
                for (int i = 0; i < data.Count; i++)
                {
                    DebugLog(string.Format("{0}:{1}", data.Keys[i], data.Get(i)));
                }

                // リクエスト
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(QjConfiguration.AuDatachargeUri);
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = byteArray.Length;

                // レスポンス
                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(byteArray, 0, byteArray.Length);
                    using (HttpWebResponse res = (HttpWebResponse)req.GetResponse())
                    {
                        HttpStatusCode status = res.StatusCode;
                        // Response Header
                        DebugLog("status =" + status);
                        DebugLog("Response ヘッダ");
                        WebHeaderCollection headerCol = res.Headers;
                        for (int i = 0; i < headerCol.Count; i++)
                        {
                            DebugLog(string.Format("{0}={1} ", headerCol.Keys[i], headerCol.Get(i)));
                        }

                        // Response body
                        Dictionary<string, string> responseKeyValue = new Dictionary<string, string>();
                        using (Stream resStream = res.GetResponseStream())
                        {
                            using (StreamReader reader = new StreamReader(resStream))
                            {
                                DebugLog("Response Body");
                                responseKeyValue = GetKeyValue(reader.ReadToEnd());
                                for (int i = 0; i < responseKeyValue.Count; i++)
                                {
                                    var key = responseKeyValue.Keys.ElementAt(i);
                                    DebugLog(string.Format("{0}={1} ", key, responseKeyValue[key]));
                                }
                            }
                        }

                        // Log
                        if (responseKeyValue.ContainsKey("ResponseDate"))
                        {
                            _datachargeRepo.ExecuteDatachargeLogWriteApi(
                                mainModel, (int)ActionTypeEnum.Response, data["RequestId"], data["EventId"],
                                data["RequestDate"], responseKeyValue["ResponseDate"], (int)status,
                                responseKeyValue["Result"], serialCode, string.Empty);
                        }
                        else if (responseKeyValue.ContainsKey("err") && responseKeyValue["err"] == "6")
                        {
                            _datachargeRepo.ExecuteDatachargeLogWriteApi(
                                mainModel, (int)ActionTypeEnum.Response, data["RequestId"], data["EventId"],
                                string.Empty, string.Empty, (int)status, string.Empty, serialCode,
                                "KDDIシステムメンテナンス中です。");
                        }
                        else
                        {
                            _datachargeRepo.ExecuteDatachargeLogWriteApi(
                                mainModel, (int)ActionTypeEnum.Response, data["RequestId"], data["EventId"],
                                string.Empty, string.Empty, (int)status, string.Empty, serialCode, string.Empty);
                        }

                        // Status = RequestTimeout
                        if ((status == HttpStatusCode.OK && responseKeyValue["Result"] == "0000") ||
                            status == HttpStatusCode.RequestTimeout)
                        {
                            if (status == HttpStatusCode.RequestTimeout)
                            {
                                StringBuilder bodyString = new StringBuilder();
                                bodyString.AppendLine("データチャージのリクエストがタイムアウトです。");
                                bodyString.AppendLine("RequestId");
                                bodyString.AppendLine(requestId);

                                Task<bool> task = NoticeMailWorker.SendAsync(bodyString.ToString());
                            }

                            return true;
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                string response = "";
                if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null)
                {
                    response = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    DebugLog(response);
                }
                DebugLog(string.Format("ContBillRequest Error:{0}", ex.Message));
                _datachargeRepo.ExecuteDatachargeLogWriteApi(mainModel, (int)ActionTypeEnum.Error,
                    requestId, "", "", "", 0, "", serialCode, ex.Message);
            }
            catch (Exception ex)
            {
                DebugLog(string.Format("ContBillRequest Error:{0}", ex.Message));
                _datachargeRepo.ExecuteDatachargeLogWriteApi(mainModel, (int)ActionTypeEnum.Error,
                    requestId, "", "", "", 0, "", serialCode, ex.Message);
            }

            return false;
        }

        /// <summary>
        /// ログイン時のOpenIDとして取得するIDはUri形式でその最後の要素だけをAuSystemIDとして利用する
        /// </summary>
        private  string GetAuSystemID(string openid)
        {
            string[] tmp = openid.Split('/');
            return tmp.Last();
        }

        /// <summary>
        /// テスト用の手抜きログ吐き
        /// </summary>
        [Conditional("DEBUG")]
        public  void DebugLog(string message)
        {
            try
            {
                string log = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), "auDataCharge.txt");
                System.IO.File.AppendAllText(log, string.Format("{0}:{1}{2}", DateTime.Now, message, Environment.NewLine));
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Bodyに入ってくるKeyValue値をDictionaryに展開
        /// </summary>
        private  Dictionary<string, string> GetKeyValue(string bodyText)
        {
            Dictionary<string, string> results = new Dictionary<string, string>();
            string[] pair = bodyText.Split('\n');

            if (pair != null && pair.Length > 0)
            {
                foreach (string keyValueStr in pair)
                {
                    string[] kv = keyValueStr.Split('=');
                    if (kv.Length == 2)
                    {
                        results.Add(kv[0], kv[1]);
                    }
                }
            }

            return results;
        }

        #endregion

        #region Public Method

        public  PointDatachargeViewModel CreateViewModel(QolmsJotoModel mainModel)
        {
            PointDatachargeViewModel result = new PointDatachargeViewModel();
            QhYappliPortalDatachargeReadApiResults apiResult = _datachargeRepo.ExecuteDatachargeReadApi(mainModel);

            result.EventIdN = apiResult.EventIdN.ConvertAll(i => 
                new DatachargeEventIdItem() { 
                    DispName = i.DispName,
                    EventId = i.EventId,
                    Point = i.Point.TryToValueType(int.MinValue),
                    Size = i.Size.TryToValueType(int.MinValue)
                });
            result.DatachargeHistN = apiResult.DatachargeHistN.ConvertAll(i => 
                new DatachargeHistItem() { 
                    ActionDate = i.ActionDate.TryToValueType(DateTime.MinValue),
                    DispName = i.DispName,
                    Point = i.Point.TryToValueType(int.MinValue),
                    Size = i.Size.TryToValueType(int.MinValue)
                });

            // ポイント数の表示
            try
            {
                result.Point = _pointRepo.GetQolmsPoint(
                    mainModel.ApiExecutor,
                    mainModel.ApiExecutorName,
                    mainModel.SessionId,
                    mainModel.ApiAuthorizeKey,
                    mainModel.AuthorAccount.AccountKey);
            }
            catch (Exception ex)
            {
                result.Point = 0;
            }

            return result;
        }

        public  bool Charge(QolmsJotoModel mainModel, int capacity)
        {
            DateTime actionDate = DateTime.Now;
            // チャージマスタ
            QhYappliPortalDatachargeEventIdReadApiResults result = _datachargeRepo.ExecuteDatachargeEventIdReadApi(mainModel, actionDate, capacity);

            // Point減算
            DateTime limit = DateTime.MinValue;

            Dictionary<string, int> pointResult = _pointRepo.AddQolmsPoints(
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName,
                mainModel.SessionId,
                mainModel.ApiAuthorizeKey,
                mainModel.AuthorAccount.AccountKey,
                QjConfiguration.PointServiceno,
                new List<QolmsPointGrantItem>
                {
                new QolmsPointGrantItem()
                {
                    ActionDate = actionDate,
                    SerialCode = Guid.NewGuid().ToApiGuidString(),
                    PointItemNo = (byte)QjPointItemTypeEnum.Datacharge,
                    Point = int.Parse(result.Point) * -1,
                    PointTargetDate = actionDate,
                    PointExpirationDate = DateTime.MaxValue,
                    Reason = "auデータと交換"
                }
                });

            if (pointResult.First().Value == 0)
            {
                // 成功したらチャージ
                if (this.PostDataChargeRequest(mainModel, result.EventId, pointResult.First().Key))
                {
                    return true;
                }
                else
                {
                    DateTime pointLimitDate = new DateTime(
                        actionDate.Year,
                        actionDate.Month,
                        1).AddMonths(7).AddDays(-1); // ポイント有効期限は 6 ヶ月後の月末（起点は操作日時）

                    // チャージ失敗
                    Dictionary<string, int> removePointResult = _pointRepo.AddQolmsPoints(
                        mainModel.ApiExecutor,
                        mainModel.ApiExecutorName,
                        mainModel.SessionId,
                        mainModel.ApiAuthorizeKey,
                        mainModel.AuthorAccount.AccountKey,
                        QjConfiguration.PointServiceno,
                        new List<QolmsPointGrantItem>
                        {
                            new QolmsPointGrantItem()
                            {
                                ActionDate = actionDate,
                                SerialCode = Guid.NewGuid().ToApiGuidString(),
                                PointItemNo = (byte)QjPointItemTypeEnum.RecoveryPoint,
                                Point = int.Parse(result.Point),
                                PointTargetDate = actionDate,
                                PointExpirationDate = pointLimitDate,
                                Reason = "auデータと交換失敗のためポイント復元"
                            }
                        });

                    if (removePointResult.First().Value != 0)
                    {
                        StringBuilder bodyString = new StringBuilder();
                        bodyString.AppendLine("データチャージポイント修正のエラーです。");
                        bodyString.AppendLine(string.Format("error:{0}", removePointResult.First().Key));

                        Task<bool> task = NoticeMailWorker.SendAsync(bodyString.ToString());
                    }

                    // メール
                    if (!string.IsNullOrWhiteSpace(QjConfiguration.IsSendMailPointExcange) && bool.Parse(QjConfiguration.IsSendMailPointExcange))
                    {
                        StringBuilder bodyString = new StringBuilder();
                        bodyString.AppendLine("データチャージのエラーです。");
                        bodyString.AppendLine("SerialCode:");
                        bodyString.AppendLine(pointResult.First().Key);

                        Task<bool> task = NoticeMailWorker.SendAsync(bodyString.ToString());
                    }

                    throw new InvalidOperationException(
                        string.Format("データチャージに失敗しました。SerialCode:{0}", pointResult.First().Key));
                }
            }
            else
            {
                throw new InvalidOperationException("ポイントの減算に失敗しました。");
            }
        }

        #endregion
    }

}