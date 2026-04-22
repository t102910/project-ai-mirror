using System;
using System.Net;
using System.Net.Http;
using System.Configuration;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Enums;
using System.Threading.Tasks;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsOpenApi.Sql;
using System.Linq;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// E薬Linkの機能を提供します。
    /// </summary>
    public sealed class ELinkWorker
    {
        private static readonly string SYSTEM_ID = ConfigurationManager.AppSettings["EKusuLinkApiSystemId"]; //MG01

        private static void WriteAccessLog(string errorMessage)
        {
            AccessLogWorker.WriteAccessLog(
                null, 
                QsApiSystemTypeEnum.QolmsOpenApi, 
                Guid.Empty,
                DateTime.Now, 
                AccessLogWorker.AccessTypeEnum.Error, 
                string.Empty, 
                errorMessage
            );
        }

        // テスト用のログ吐き
        public static void DebugLog(string message)
        {
            try
            {
                string log = System.IO.Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/App_Data"), "ELinkApiAccesslog.txt");
                System.IO.File.AppendAllText(log, string.Format("{0}:{1}{2}", DateTime.Now, message, "\r\n"));
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// e薬Link用のワンタイムコードをDBへ登録します。
        /// </summary>
        /// <param name="accountKey"></param>
        /// <returns></returns>
        public static string WriteELinkOnetimeCode(Guid accountKey)
        {
            DebugLog("WriteELinkOnetimeCode(" + accountKey.ToString() + ")");

            string result = string.Empty;
            DateTime now = DateTime.Now;
            string otc = CreateOnetimeCode();

            // TODO: 運用にあたっては、期限切れコードを削除していく機能も必要となる

            QH_ELINKONETIMECODE_DAT entity = new QH_ELINKONETIMECODE_DAT()
            {
                ACCOUNTKEY = accountKey,
                REQUESTEDDATE = now,
                ONETIMECODE = otc,
                EXPIRES = now.AddMinutes(30),
                DELETEFLAG = false,
                CREATEDDATE = now,
                UPDATEDDATE = now                
            };                       
            QhELinkOnetimeCodeEntityWriter writer = new QhELinkOnetimeCodeEntityWriter();
            QhELinkOnetimeCodeEntityWriterArgs writerArgs = new QhELinkOnetimeCodeEntityWriterArgs() { Data = new List<QH_ELINKONETIMECODE_DAT>() { entity } };
            QhELinkOnetimeCodeEntityWriterResults writerResults = QsDbManager.Write(writer, writerArgs);

            DebugLog("writerResults" + writerResults.IsSuccess.ToString());

            if (writerResults != null)
            {
                {                    
                    if (writerResults.IsSuccess && writerResults.Result == 1)
                        result = otc;
                }
            }
            return result;
        }

        /// <summary>
        /// e薬Link用のワンタイムコードを生成します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoElinkOnetimeCodeGenerateApiResults OnetimeCodeGenerate(QoElinkOnetimeCodeGenerateApiArgs args)
        {
            DebugLog("OnetimeCodeGenerate()");

            QoElinkOnetimeCodeGenerateApiResults result = new QoElinkOnetimeCodeGenerateApiResults() { IsSuccess = bool.FalseString };
            {
                result.OnetimeCode = WriteELinkOnetimeCode(args.ActorKey.TryToValueType<Guid>(Guid.Empty));
                result.Expires = DateTime.Now.AddMinutes(30).ToApiDateString();
                result.IsSuccess = bool.TrueString;
                result.Result = new QoApiResultItem() { Code = "0200", Detail = "正常に終了しました。" };   
            }

            DebugLog("result = " + result.Result.Detail);

            return result;
        }


        /// <summary>
        /// e薬Link用のワンタイムコードを生成します。
        /// </summary>
        /// <returns></returns>
        private static string CreateOnetimeCode()
        {
            DebugLog("CreateOnetimeCode()");

            // Const SYSTEM_ID As String = "QS02" 'TODO：CCC用。web.configへ
            string result = string.Empty;

            // とりあえず最大5回リトライできるようにしておく
            for (int i = 1; i <= 5; i++)
            {
                // 乱数でワンタイムコードを生成
                string otc = string.Format("{0}X{1}", SYSTEM_ID, new Random().Next(1, 999999).ToString("d6")).ToUpper();

                // 未使用かどうかチェック
                if (ReadOnetimeCode(otc) == Guid.Empty)
                {
                    DebugLog("未使用チェック");
                    result = otc;
                    break;
                }
                else
                    DebugLog("リトライ");
                // 使用中のワンタイムコード。リトライ
                continue;
            }
            // 生成できなければExceptionをスロー
            if (string.IsNullOrWhiteSpace(result))
                throw new InvalidOperationException("ワンタイムコードの生成に失敗しました。");

            DebugLog("result = " + result);

            return result;
        }

        private static Guid ReadOnetimeCode(string otc)
        {
            DebugLog("ReadOnetimeCode()");

            Guid result = Guid.Empty;
            OnetimeCodeReader reader = new OnetimeCodeReader();
            OnetimeCodeReaderArgs readerArgs = new OnetimeCodeReaderArgs() { OnetimeCode = otc };
            OnetimeCodeReaderResults readerResults = QsDbManager.Read(reader, readerArgs);

            if (readerResults != null)
            {
                if (readerResults.IsSuccess && readerResults.Result != null && readerResults.Result.Count == 1 && readerResults.Result.First() != null)
                {
                    result = readerResults.Result.First().ACCOUNTKEY;
                }
            }
            return result;
        }


        public static QoELinkReadApiResults Read(QoELinkReadApiArgs args)
        {
            DebugLog("Read()");

            QoELinkReadApiResults myResults = new QoELinkReadApiResults()
            {
                IsSuccess = bool.FalseString
            };
            //string url = ConfigurationManager.AppSettings["EKusuLinkApiUri"]; // "https://qolms-dev-core-west-api10.azurewebsites.net/api/"
            //string requestEndPoint = url + "/One_Time/Retrieve?Method=1&one_time_code=" + args.ActorKey;

            string uri = ConfigurationManager.AppSettings["EKusuLinkApiUri"];
            string uri2 = "/One_Time/Retrieve?Method=1&one_time_code=" + args.ActorKey;
            string requestEndPoint = Convert.ToString(new Uri(new Uri(uri), uri2));

            DebugLog(requestEndPoint);

            string resString = HttpRequest.GetString(requestEndPoint);
            If01Results elinkResults = JsonConvert.DeserializeObject<If01Results>(resString);
            if (elinkResults != null)
            {
                // これでいいかわからない。とりあえず。
                foreach (KeyValuePair<string, List<string>> item in elinkResults.systems)
                    myResults.DataN.AddRange(item.Value);
                myResults.IsSuccess = bool.TrueString;
                myResults.Result = QoApiResultHelper.Build(QoApiResultCodeTypeEnum.Success);
            }
            else
                myResults.Result = QoApiResultHelper.Build(QoApiResultCodeTypeEnum.UnknownError, "EkusuLinkApi呼び出しに失敗しました");

            DebugLog(myResults.Result.Detail);
            return myResults;
        }
    }

    public class HttpRequest
    {
        private const string HttpRequestHeaderKeyName = "LINK-SYSTEM-ID";
        private static readonly HttpClient httpClient = new HttpClient();
        private static HttpRequestMessage CreateRequest(HttpMethod httpMethod, string requestEndPoint)
        {
            HttpRequestMessage request = new HttpRequestMessage(httpMethod, requestEndPoint);

            return AddHeaders(request);
        }

        private static HttpRequestMessage AddHeaders(HttpRequestMessage request)
        {
            request.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Accept-Charset", "utf-8");
            request.Headers.Add(HttpRequestHeaderKeyName, "MG01");

            return request;
        }

        public static string GetString(string endpoint)
        {
            HttpRequestMessage request = CreateRequest(HttpMethod.Get, endpoint);

            ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12; // TLS 1.2

            string resBodyStr;
            HttpStatusCode resStatusCoode = HttpStatusCode.NotFound;
            Task<HttpResponseMessage> response;
            try
            {
                response = httpClient.SendAsync(request);
                resBodyStr = response.Result.Content.ReadAsStringAsync().Result;
                resStatusCoode = response.Result.StatusCode;
            }
            catch (HttpRequestException e)
            {
                // 通信失敗のエラー
                return e.Message;
            }
            if (!resStatusCoode.Equals(HttpStatusCode.OK))

                // レスポンスが200 OK以外の場合
                return string.Format("Request:{0}/Response StatusCode:{1}", endpoint, resStatusCoode);
            if (string.IsNullOrEmpty(resBodyStr))
                return "Response Bodyが空です";

            return resBodyStr;
        }
    }
    [DataContract()]
    [Serializable]
    public class If01Results
    {
        /// <summary>
        ///         ''' 結果コードを取得または設定します。
        ///         ''' </summary>
        [DataMember()]
        public string result_code { get; set; } = string.Empty;

        /// <summary>
        ///         ''' 結果メッセージを取得または設定します。
        ///         ''' </summary>
        [DataMember()]
        public string message { get; set; } = string.Empty;
        /// <summary>
        ///         ''' お薬手帳情報のディクショナリを取得または設定します。
        ///         ''' キーはお薬手帳システムID、要素はBase64エンコードしたお薬手帳データのリスト。
        ///         ''' </summary>
        [DataMember()]
        public Dictionary<string, List<string>> systems { get; set; } = new Dictionary<string, List<string>>();
    }
}


