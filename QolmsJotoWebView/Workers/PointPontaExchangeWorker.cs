using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsJotoWebView.Models;
using MGF.QOLMS.QolmsJotoWebView.Workers;
using MGF.QOLMS.QolmsJotoWebView.Repositories;

namespace MGF.QOLMS.QolmsJotoWebView
{
    public sealed class PointPontaExchangeWorker
    {

        IPointRepository _pointRepo;
        IPontaExchangeRepository _pontaRepo;

        #region Constant
        //// todo 検証サーバーで試す、リクエストIDの発行
        //// 加盟店ID
        //public  readonly string KAMEITENID = ConfigurationManager.AppSettings["AuWalletPointKameitenId"];

        //// サービスID
        //public  readonly string SERVICEID = ConfigurationManager.AppSettings["AuPaymentServiceId"];

        //// セキュリティキー
        //public  readonly string SECUREKEY = ConfigurationManager.AppSettings["AuPaymentSecureKey"];

        //// 要求URI
        //public  readonly string REQUESTURI = ConfigurationManager.AppSettings["AuWalletPointExchangeUri"];

        //public  readonly string APIKEY = ConfigurationManager.AppSettings["AuWalletPointApiKey"];
        #endregion

        #region Constructor

        /// <summary>
        /// デフォルト コンストラクタは使用できません。
        /// </summary>
        public PointPontaExchangeWorker(IPointRepository pointRepository, IPontaExchangeRepository pontaExchangeRepository)
        {
            _pointRepo = pointRepository;
            _pontaRepo = pontaExchangeRepository;

        }

        #endregion

        #region Private Method

        private bool PostAuWalletPointExchangeRequest(QolmsJotoModel mainModel, int pointId, int point, string serialCode)
        {
            DateTime actionDate = DateTime.Now;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string requestId = string.Empty;
            try
            {
                // Log
                QhYappliPortalAuWalletPointExchangeWriteApiResults apiResult = _pontaRepo.ExecuteAuWalletPointWriteApi(
                    mainModel, actionDate, pointId, point, serialCode, "", "", "", "", "", 0, "", false);
                requestId = apiResult.RequestId;

                DebugLog(mainModel.AuthorAccount.OpenId);
                // DebugLog("テスト用 https://connect.auone.jp/net/id/hny_rt_net/cca/a/kddi_823b7s4nwa65rfv9rvpdgu9njel");

                auPointRequestOfJson data = new auPointRequestOfJson();
                data.BPIFMerchantPoint_I.memberId = QjConfiguration.AuWalletPointKameitenId;
                data.BPIFMerchantPoint_I.serviceId = QjConfiguration.AuPaymentServiceId;
                data.BPIFMerchantPoint_I.secureKey = QjConfiguration.AuPaymentSecureKey;
                data.BPIFMerchantPoint_I.authKbn = "2";
                data.BPIFMerchantPoint_I.auId = this.GetAuSystemId(mainModel.AuthorAccount.OpenId);
                // PointPontaExchangeWorker.GetAuSystemId("https://connect.auone.jp/net/id/hny_rt_net/cca/a/kddi_823b7s4nwa65rfv9rvpdgu9njel");
                // PointPontaExchangeWorker.GetAuSystemId(mainModel.AuthorAccount.OpenId);
                data.BPIFMerchantPoint_I.memberAskNo = requestId;  // こちらで一意の番号　文字列20桁まで あとでチェックするのでDBへ保持する
                data.BPIFMerchantPoint_I.dispKbn = "0";
                data.BPIFMerchantPoint_I.commodity = "ポイント交換（ＪＯＴＯポイント）"; // JOTOではこの文言を使用することで合意している
                data.BPIFMerchantPoint_I.useAuIdPoint = "0"; // 利用Walletポイント（JOTOからは利用しないので0）
                data.BPIFMerchantPoint_I.useCmnPoint = "0"; // 利用Walletポイント（JOTOからは利用しないので0）
                data.BPIFMerchantPoint_I.obtnPoint = point.ToString(); // 獲得Walletポイント（JOTOからはここに交換の値を入れる）
                data.BPIFMerchantPoint_I.tmpObtnKbn = "1"; // 獲得予定区分。予定ではなく即時獲得なので1
                data.BPIFMerchantPoint_I.pointEffTimlmtKbn = "1"; // 有効期限指定区分　0指定1最長

                string delimiter = string.Empty;
                StringBuilder values = new StringBuilder();
                QsJsonSerializer jsr1 = new QsJsonSerializer();
                string json = jsr1.Serialize<auPointRequestOfJson>(data);
                DebugLog(json);
                values.Append(json);

                byte[] byteArray = Encoding.UTF8.GetBytes(values.ToString());

                DebugLog(QjConfiguration.AuWalletPointExchangeUri);
                // リクエスト
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(QjConfiguration.AuWalletPointExchangeUri);
                req.Method = "POST";
                req.ContentType = "application/json;charset=UTF-8";
                req.Headers.Add("Accept-Charset", "UTF-8");
                req.Headers.Add("X-Kddi-Api-Key", QjConfiguration.AuWalletPointApiKey);
                req.Headers.Add("X-Conect-MemberId", QjConfiguration.AuWalletPointKameitenId);
                // req.ContentLength = byteArray.Length;

                DebugLog("-----------開始-------------");
                DebugLog("Request　ヘッダ");
                WebHeaderCollection reqheaderCol = req.Headers;
                for (int i = 0; i < reqheaderCol.Count; i++)
                {
                    DebugLog(string.Format("{0}={1} ", reqheaderCol.Keys[i], reqheaderCol.Get(i)));
                }

                // レスポンス
                auPointResponseOfJson result;

                using (Stream newStream = req.GetRequestStream())
                {
                    newStream.Write(byteArray, 0, byteArray.Length);
                    using (HttpWebResponse res = (HttpWebResponse)req.GetResponse())
                    {
                        HttpStatusCode status = res.StatusCode;
                        // response Header
                        DebugLog("status =" + status);
                        DebugLog("Response　ヘッダ");
                        WebHeaderCollection headerCol = res.Headers;
                        for (int i = 0; i < headerCol.Count; i++)
                        {
                            DebugLog(string.Format("{0}={1} ", headerCol.Keys[i], headerCol.Get(i)));
                        }

                        // response body
                        using (Stream resStream = res.GetResponseStream())
                        {
                            using (StreamReader reader = new StreamReader(resStream))
                            {
                                // Jsonにデシリアライズする
                                QsJsonSerializer jsr = new QsJsonSerializer();
                                string jsonStr = reader.ReadToEnd();

                                result = jsr.Deserialize<auPointResponseOfJson>(jsonStr);

                                DebugLog(jsonStr);
                                DebugLog("Response　Body");
                                DebugLog(string.Format("resultCd :{0}", result.pointIf.control.resultCd));
                                DebugLog(string.Format("pointReceiptNo :{0}", result.pointIf.processResult.pointInfo.pointReceiptNo));
                            }
                        }

                        if ((status == HttpStatusCode.OK && result.pointIf.control.resultCd == "PUD100000"))
                        {
                            // 登録処理
                            PointInfo info = result.pointIf.processResult.pointInfo;
                            _pontaRepo.ExecuteAuWalletPointWriteApi(mainModel, actionDate, pointId, point, serialCode, requestId, "A1", info.pointReceiptNo, info.processDay, info.processTime, (int)status, result.pointIf.control.resultCd, false);

                            DebugLog("通信成功、ポイント付与成功");
                            return true;
                        }
                        else if ((status == HttpStatusCode.OK && !string.IsNullOrWhiteSpace(result.pointIf.control.resultCd)))
                        {
                            // 登録処理
                            PointInfo info = result.pointIf.processResult.pointInfo;
                            _pontaRepo.ExecuteAuWalletPointWriteApi(mainModel, actionDate, pointId, 0, serialCode, requestId, "A1", info.pointReceiptNo, info.processDay, info.processTime, (int)status, result.pointIf.control.resultCd, false);
                        }
                        else if (status == HttpStatusCode.RequestTimeout)
                        {
                            // 登録処理
                            _pontaRepo.ExecuteAuWalletPointWriteApi(mainModel, actionDate, pointId, point, serialCode, requestId, "A1", string.Empty, string.Empty, string.Empty, (int)status, string.Empty, false);

                            StringBuilder bodyString = new StringBuilder();
                            bodyString.AppendLine("Pontaポイント交換のリクエストがタイムアウトです。");
                            bodyString.AppendLine("RequestId");
                            bodyString.AppendLine(requestId);

                            Task<bool> task = NoticeMailWorker.SendAsync(bodyString.ToString());
                            DebugLog("通信タイムアウト");
                        }
                        else
                        {
                            // 登録処理
                            _pontaRepo.ExecuteAuWalletPointWriteApi(mainModel, actionDate, pointId, 0, serialCode, requestId, "A1", string.Empty, string.Empty, string.Empty, (int)status, string.Empty, false);

                            StringBuilder bodyString = new StringBuilder();
                            bodyString.AppendLine(string.Format("Pontaポイント交換のリクエスト({0})", (int)status));
                            bodyString.AppendLine("RequestId");
                            bodyString.AppendLine(requestId);

                            Task<bool> task = NoticeMailWorker.SendAsync(bodyString.ToString());
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
                _pontaRepo.ExecuteAuWalletPointWriteApi(mainModel, actionDate, pointId, 0, serialCode, requestId, "A1", string.Empty, string.Empty, string.Empty, (int)ex.Status, string.Empty, false);
            }
            catch (Exception ex)
            {
                DebugLog(string.Format("ContBillRequest Error:{0}", ex.Message));
                _pontaRepo.ExecuteAuWalletPointWriteApi(mainModel, actionDate, pointId, 0, serialCode, requestId, "A1", string.Empty, string.Empty, string.Empty, 0, string.Empty, false);
                AccessLogWorker.WriteAccessLog(null, string.Empty, AccessLogWorker.AccessTypeEnum.Error, string.Format("PostAuWalletPointExchangeRequest:{0}", ex.Message));
            }
            DebugLog("通信成功、ポイント付与失敗");
            return false;
        }

        // ログイン時のOpenIDとして取得するIDはUri形式でその最後の要素だけをAuSystemIDとして利用する
        private string GetAuSystemId(string openid)
        {
            string[] tmp = openid.Split('/');
            return tmp.Last();
        }

        // テスト用の手抜きログ吐き
        [Conditional("DEBUG")]
        public  void DebugLog(string message)
        {
            try
            {
                string log = System.IO.Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Log"), "auPoint.log");
                System.IO.File.AppendAllText(log, string.Format("{0}:{1}{2}", DateTime.Now, message, Environment.NewLine));
            }
            catch (Exception ex)
            {
            }
        }

        // Bodyに入ってくるKeyValue値をDictionaryに展開
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

        public  PortalAuWalletPointExchangeViewModel CreateViewModel(QolmsJotoModel mainModel)
        {
            DebugLog("CreateViewModel");
            PortalAuWalletPointExchangeViewModel result = new PortalAuWalletPointExchangeViewModel();
            QhYappliPortalAuWalletPointExchangeReadApiResults apiResult = _pontaRepo.ExecuteAuWalletPointReadApi(mainModel);
            result.AuWalletPointItemN = apiResult.AuWalletPointItemN.ConvertAll(i => 
                new AuWalletPointItem() { 
                    AuWalletPointItemId = i.AuWalletPointItemId,
                    DispName = i.DispName,
                    Point = i.Point.TryToValueType(int.MinValue)
            
                });
            result.AuWalletPointHistN = apiResult.AuWalletPointHistN.ConvertAll(i => 
                new AuWalletPointHistItem() { 
                    ActionDate = i.ActionDate.TryToValueType(DateTime.MinValue),
                    DispName = i.DispName,
                    Point = i.Point.TryToValueType(int.MinValue)
                });

            // 履歴に表示する→result:成功コード matchresult:0,1
            // ポイント数の表示
            try
            {
                result.Point = _pointRepo.GetQolmsPoint(
                    mainModel.ApiExecutor,
                    mainModel.ApiExecutorName,
                    mainModel.SessionId,
                    mainModel.ApiAuthorizeKey,
                    mainModel.AuthorAccount.AccountKey
                );
            }
            catch (Exception ex)
            {
                result.Point = 0;
            }

            return result;
        }

        public  bool Exchange(QolmsJotoModel mainModel, int itemid)
        {
            DebugLog("Exchange");
            DateTime actionDate = DateTime.Now;
            // チャージマスタ
            QhYappliPortalAuWalletPointExchangeItemReadApiResults result = _pontaRepo.ExecuteAuPointExchangeItemReadApi(mainModel, actionDate, itemid);

            // point減算
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
                    PointItemNo = (int)QjPointItemTypeEnum.AuPoint,
                    Point = int.Parse(result.Point) * -1,
                    PointTargetDate = actionDate,
                    PointExpirationDate = DateTime.MaxValue,
                    Reason = "Pontaポイントと交換"
                }
                }
            );

            if (pointResult.First().Value == 0)
            {
                // 成功したらチャージ
                if (this.PostAuWalletPointExchangeRequest(mainModel, int.Parse(result.AuWalletPointItemId), int.Parse(result.AuWalletPoint), pointResult.First().Key))
                {
                    return true;
                }
                else
                {
                    DateTime pointLimitDate = new DateTime(
                        actionDate.Year,
                        actionDate.Month,
                        1
                    ).AddMonths(7).AddDays(-1); // ポイント有効期限は 6 ヶ月後の月末（起点は操作日時）

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
                            PointItemNo =(int) QjPointItemTypeEnum.RecoveryPoint,
                            Point = int.Parse(result.Point),
                            PointTargetDate = actionDate,
                            PointExpirationDate = pointLimitDate,
                            Reason = "Pontaポイントと交換失敗のためポイントを復元"
                        }
                        }
                    );

                    if (removePointResult.First().Value != 0)
                    {
                        StringBuilder bodyString1 = new StringBuilder();
                        bodyString1.AppendLine("Pontaポイント修正のエラーです。");
                        bodyString1.AppendLine(string.Format("error:{0}", removePointResult.First().Key));

                        Task<bool> task1 = NoticeMailWorker.SendAsync(bodyString1.ToString());
                    }

                    // メー
                    StringBuilder bodyString = new StringBuilder();
                    bodyString.AppendLine("Pontaポイント交換のエラーです。");
                    bodyString.AppendLine("SerialCode:");
                    bodyString.AppendLine(pointResult.First().Key);

                    Task<bool> task = NoticeMailWorker.SendAsync(bodyString.ToString());

                    throw new InvalidOperationException(string.Format("Pontaポイント交換に失敗しました。SerialCode:{0}", pointResult.First().Key));
                }
            }
            else
            {
                DebugLog("ポイントの減算に失敗しました");
                throw new InvalidOperationException(string.Format("ポイントの減算に失敗しました。{0}", pointResult.First().Value));
            }
        }

        #endregion
    }


}
