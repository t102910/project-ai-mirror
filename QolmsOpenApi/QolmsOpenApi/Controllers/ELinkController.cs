using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using Newtonsoft.Json;
using MGF.QOLMS.QolmsOpenApi.Attribute;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Worker;
using MGF.QOLMS.QolmsApiEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Controllers
{
    /// <summary>
    /// ELinkController
    /// </summary>
    public class ELinkController : QoApiControllerBase
    {
        /// <summary>
        /// e薬Link用のワンタイムコードを生成します。
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>
        //[QoApiAuthorize(QoApiAuthorizeTypeEnum.JwtAccessKey, QoApiFunctionTypeEnum.NoteMedicine)]
        //[ActionName("OnetimeCodeGenerate")]
        //public QoElinkOnetimeCodeGenerateApiResults PostOnetimeCodeGenerate(QoElinkOnetimeCodeGenerateApiArgs args)
        //{
        //    DebugLog("PostOnetimeCodeGenerate()");
        //    return this.ExecuteWorkerMethod(args, ELinkWorker.OnetimeCodeGenerate);
        //}

        /// <summary>
        /// e薬Link用のワンタイムコードを生成します。(検証用)
        /// </summary>
        /// <param name="args">Web API 引数クラス。</param>
        /// <returns>Web API 戻り値クラス。</returns>
        /// <remarks></remarks>        
        [ActionName("TestOnetimeCodeGenerate")]
        public QoElinkOnetimeCodeGenerateApiResults PostTestOnetimeCodeGenerate(QoElinkOnetimeCodeGenerateApiArgs args)
        {
            DebugLog("GetOnetimeCodeGenerate()");
            return this.ExecuteWorkerMethod(args, ELinkWorker.OnetimeCodeGenerate);
        }

        //[ActionName("ELinkRead")]
        //public QoELinkReadApiResults PostELinkRead(QoELinkReadApiArgs args)
        //{
        //    return this.ExecuteWorkerMethod(args, ELinkWorker.Read);
        //}

        //[ActionName("ELinkRead")]
        //public QoELinkReadApiResults GetELinkRead(QoELinkReadApiArgs args)
        //{
        //    return this.ExecuteWorkerMethod(args, ELinkWorker.Read);
        //}

        //public string GetOnetimeCode(Guid accountKey)
        //{
        //    return ELinkWorker.WriteELinkOnetimeCode(accountKey);
        //}

        /// <summary>
        /// e薬Linkワンタイムコードを利用してお薬情報を取得します。(検証用)
        /// </summary>
        /// <param name="onetimeCode">ワンタイムコード</param>
        /// <param name="from_date">開始日 yyyy-MM-dd</param>
        /// <param name="to_date">終了日</param>
        /// <returns></returns>
        [ActionName("RetrieveTest")]
        public string GetRetrieveTest(string onetimeCode, string from_date, string to_date)
        {
            DebugLog("PostRetrieveTest(" + onetimeCode + ")");
            string url = ConfigurationManager.AppSettings["EKusuLinkApiUri"]; // "https://qolms-dev-core-west-api10.azurewebsites.net/api/"
            string requestEndPoint = url + "/One_Time/Retrieve?Method=1&one_time_code=" + onetimeCode + "&from_date=" + from_date + "&to_date=" + to_date;

            //string uri = ConfigurationManager.AppSettings["EKusuLinkApiUri"];
            //string uri2 = "/One_Time/Retrieve?Method=1&one_time_code=" + onetimeCode;
            //string requestEndPoint = Convert.ToString(new Uri(new Uri(uri), uri2));

            DebugLog(requestEndPoint);

            try
            {
                string resString = HttpRequest.GetString(requestEndPoint);
                DebugLog(resString);

                If01Results elinkResults = JsonConvert.DeserializeObject<If01Results>(resString);

                if (elinkResults != null)
                {
                    // これでいいかわからない。とりあえず。
                    foreach (KeyValuePair<string, List<string>> item in elinkResults.systems)
                    {
                        DebugLog(item.Key);
                        foreach (string Val in item.Value)
                            DebugLog(System.Text.Encoding.GetEncoding("shift_jis").GetString(Convert.FromBase64String(Val)));
                    }
                }
                return resString;
            }
            catch (Exception ex)
            {
                return ex.Message + ex.StackTrace;
            }
        }

        //public string GetErrorTest(Guid accountKey)
        //{
        //    string url = ConfigurationManager.AppSettings["EKusuLinkApiUri"]; // "https://qolms-dev-core-west-api10.azurewebsites.net/api/"
        //    string requestEndPoint = url + "/One_Time/Retrieve?Method=1&one_time_code=";
        //    string resString;
        //    StringBuilder result = new StringBuilder();

        //    // {"11", "ワンタイムコードが指定されていません。"},
        //    result.AppendLine("Test 11 ワンタイムコードが指定されていません。");
        //    resString = HttpRequest.GetString(requestEndPoint);
        //    result.AppendLine(resString);

        //    // {"12", "ワンタイムコードが200文字を超えています"},
        //    result.AppendLine("Test 12 ワンタイムコードが200文字を超えています");
        //    resString = HttpRequest.GetString(requestEndPoint + "MG01X12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890");
        //    result.AppendLine(resString);

        //    // {"13", "ワンタイムコードのフォーマットが不正です。"},
        //    result.AppendLine("Test 13 ワンタイムコードのフォーマットが不正です。");
        //    resString = HttpRequest.GetString(requestEndPoint + "hogehoge");
        //    result.AppendLine(resString);

        //    // {"15", "ワンタイムコードが不正です。"},
        //    result.AppendLine("Test 15 ワンタイムコードが不正です。");
        //    resString = HttpRequest.GetString(requestEndPoint + "NG99X123");
        //    result.AppendLine(resString);

        //    // {"21", "調剤日抽出範囲開始日が指定されていません。"},
        //    // {"22", "調剤日抽出範囲開始日のフォーマットがYYYY-MM-DD形式ではありません。"},
        //    // {"23", "調剤日抽出範囲終了日が指定されていません。"},
        //    // {"24", "調剤日抽出範囲終了日のフォーマットがYYYY-MM-DD形式ではありません。"},
        //    // {"25", "調剤日抽出範囲が365日を超えています。"},
        //    result.AppendLine("Test 25 調剤日抽出範囲が365日を超えています。");
        //    resString = HttpRequest.GetString(requestEndPoint + ELinkWorker.WriteELinkOnetimeCode(accountKey) + "&from_date=2021-01-01&to_date=2022-01-02");
        //    result.AppendLine(resString);

        //    // {"31", "予約項目01が200文字を超えています。"},
        //    result.AppendLine("Test 31 予約項目01が200文字を超えています。");
        //    resString = HttpRequest.GetString(requestEndPoint + ELinkWorker.WriteELinkOnetimeCode(accountKey) + "&reserve_01=1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890");
        //    result.AppendLine(resString);

        //    // {"32", "予約項目02が200文字を超えています。"},
        //    result.AppendLine("Test 32 予約項目02が200文字を超えています。");
        //    resString = HttpRequest.GetString(requestEndPoint + ELinkWorker.WriteELinkOnetimeCode(accountKey) + "&reserve_02=12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901");
        //    result.AppendLine(resString);


        //    return result.ToString();
        //}


        // テスト用の手抜きログ吐き
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
    }
}



