using MGF.QOLMS.QolmsApiCoreV1;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using MGF.QOLMS;


namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// KDDI のOwl（属性取得API)呼び出しに関する機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    internal sealed class AuOwlAccessUsecase
    {
        #region Constant
        #endregion

        #region Constructor

        /// <summary>
        /// デフォルトコンストラクタは使用できません。
        /// </summary>
        private AuOwlAccessUsecase()
        {
        }

        #endregion

        private class MyStringWriter : StringWriter
        {
            public override Encoding Encoding
            {
                get
                {
                    return Encoding.GetEncoding("shift-jis");
                }
            }
        }

        #region Private Method

        private static string MakeRequestString(string sid, string fid, string utype, string uidtf)
        {
            string result = string.Empty;
            var reqObj = new OwlRequest.biscuitif
            {
                fid = fid,
                sid = sid,
                utype = utype,
                uidtf = uidtf
            };

            using (var writer = new MyStringWriter())
            {
                var serializer = new XmlSerializer(typeof(OwlRequest.biscuitif));
                var xsn = new XmlSerializerNamespaces();
                xsn.Add("cocoa", "http://www.kddi.com/cocoa");
                serializer.Serialize(writer, reqObj, xsn);
                result = writer.ToString();
            }

            return result;
        }

        /// <summary>
        /// Owlにユーザ属性を問い合わせます。
        /// </summary>
        /// <param name="openid">ユーザのシステムAuID</param>
        /// <param name="retryCount">流量制限によってエラー時に、リトライを行う場合はリトライ数、リトライせず返す場合は０(省略可)。</param>
        /// <returns>成功した場合(ステータス200)は取得したXMLをデシリアライズしたクラスを返却、失敗時はnull</returns>
        /// <remarks>ステータス200でもデータが取れているとは限らない(メンテナンス中なども含む)のでresultStatus(0：正常。3：異常、6：メンテナンス中)をチェックする必要があります</remarks>
        private static biscuitif OwlRequestByOpenid(string openid, int retryCount = 0)
        {
            DebugLog($"OwlRequest :{QjConfiguration.AuOwlUri}");

            biscuitif results = null;
            var uriObj = new Uri(QjConfiguration.AuOwlUri);

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                using (var wb = new WebClient())
                {
                    wb.Headers.Add($"X-KDDI-API-KEY:{QjConfiguration.AuOwlKey}");
                    wb.Headers[HttpRequestHeader.Host] = uriObj.Host;
                    wb.Headers[HttpRequestHeader.AcceptCharset] = "Windows-31J";
                    wb.Headers[HttpRequestHeader.ContentType] = "text/xml";

                    var enc = Encoding.GetEncoding("shift-jis");
                    string postStr = MakeRequestString(QjConfiguration.AuOwlSid, QjConfiguration.AuOwlFid, "OPENID", openid);
                    DebugLog($"PostData :{postStr}");

                    byte[] postData = enc.GetBytes(postStr);
                    byte[] resData = wb.UploadData(uriObj, postData);
                    wb.Dispose();

                    string resText = enc.GetString(resData);
                    DebugLog($"ResData :{resText}");

                    var serializer = new XmlSerializer(typeof(biscuitif));
                    results = (biscuitif)serializer.Deserialize(new StringReader(resText));
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null)
                {
                    string body = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    DebugLog(body);

                    foreach (string key in ex.Response.Headers)
                    {
                        if (key == "Retry-After" && IsNumeric(ex.Response.Headers.Get(key)) && retryCount > 0)
                        {
                            System.Threading.Thread.Sleep(1000 * int.Parse(ex.Response.Headers.Get(key)));
                            return OwlRequestByOpenid(openid, retryCount - 1);
                        }

                        DebugLog($"Ex Header>{key}:{ex.Response.Headers.Get(key)}");
                    }
                }

                AccessLogWorker.WriteAccessLog(null, string.Empty, AccessLogWorker.AccessTypeEnum.Error, $"OwlRequest Error:{ex.Message}");
                DebugLog($"OwlRequest Error:{ex.Message}");
            }
            catch (Exception ex)
            {
                AccessLogWorker.WriteAccessLog(null, string.Empty, AccessLogWorker.AccessTypeEnum.Error, $"OwlRequest Error:{ex.Message}");
                DebugLog($"OwlRequest Error:{ex.Message}");
            }

            return results;
        }

        /// <summary>
        /// Owlにユーザ属性を問い合わせます。
        /// </summary>
        /// <param name="auid">ユーザのシステムAuID</param>
        /// <param name="retryCount">流量制限によってエラー時に、リトライを行う場合はリトライ数、リトライせず返す場合は０(省略可)。</param>
        /// <returns>成功した場合(ステータス200)は取得したXMLをデシリアライズしたクラスを返却、失敗時はnull</returns>
        /// <remarks>ステータス200でもデータが取れているとは限らない(メンテナンス中なども含む)のでresultStatus(0：正常。3：異常、6：メンテナンス中)をチェックする必要があります</remarks>
        private static biscuitif OwlRequest(string auid, int retryCount = 0)
        {
            DebugLog($"OwlRequest :{QjConfiguration.AuOwlUri}");
            
            biscuitif results = null;
            var uriObj = new Uri(QjConfiguration.AuOwlUri);

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                using (var wb = new WebClient())
                {
                    wb.Headers.Add($"X-KDDI-API-KEY:{QjConfiguration.AuOwlKey}");
                    wb.Headers[HttpRequestHeader.Host] = uriObj.Host;
                    wb.Headers[HttpRequestHeader.AcceptCharset] = "Windows-31J";
                    wb.Headers[HttpRequestHeader.ContentType] = "text/xml";

                    var enc = Encoding.GetEncoding("shift-jis");
                    string postStr = MakeRequestString(QjConfiguration.AuOwlSid, QjConfiguration.AuOwlFid, "AUID", auid);
                    DebugLog($"PostData :{postStr}");

                    byte[] postData = enc.GetBytes(postStr);
                    byte[] resData = wb.UploadData(uriObj, postData);
                    wb.Dispose();

                    string resText = enc.GetString(resData);
                    DebugLog($"ResData :{resText}");

                    var serializer = new XmlSerializer(typeof(biscuitif));
                    results = (biscuitif)serializer.Deserialize(new StringReader(resText));
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null)
                {
                    string body = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    DebugLog(body);

                    foreach (string key in ex.Response.Headers)
                    {
                        if (key == "Retry-After" && IsNumeric(ex.Response.Headers.Get(key)) && retryCount > 0)
                        {
                            System.Threading.Thread.Sleep(1000 * int.Parse(ex.Response.Headers.Get(key)));
                            return OwlRequest(auid, retryCount - 1);
                        }

                        DebugLog($"Ex Header>{key}:{ex.Response.Headers.Get(key)}");
                    }
                }

                AccessLogWorker.WriteAccessLog(null, string.Empty, AccessLogWorker.AccessTypeEnum.Error, $"OwlRequest Error:{ex.Message}");
                DebugLog($"OwlRequest Error:{ex.Message}");
            }
            catch (Exception ex)
            {
                AccessLogWorker.WriteAccessLog(null, string.Empty, AccessLogWorker.AccessTypeEnum.Error, $"OwlRequest Error:{ex.Message}");
                DebugLog($"OwlRequest Error:{ex.Message}");
            }

            return results;
        }

        private static string _logPath = string.Empty;

        [Conditional("DEBUG")]
        private static void DebugLog(string message)
        {
            try
            {
                string log = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/log"), "OwlsAccesslog.Log");
                File.AppendAllText(log, $"{DateTime.Now}:{message}\r\n");
            }
            catch (Exception)
            {
            }
        }

        private static void WriteAccessLog(string apesData)
        {
            if (string.IsNullOrEmpty(apesData))
                return;

            try
            {
                string cryptData = "";
                using (var crypt = new QolmsCryptV1.QsCrypt(QolmsCryptV1.QsCryptTypeEnum.QolmsSystem))
                {
                    cryptData = crypt.EncryptString(apesData);
                }

                AccessLogWorker.WriteAccessLog(null, string.Empty, AccessLogWorker.AccessTypeEnum.None, cryptData);
            }
            catch (Exception)
            {
            }
        }

        private static string GetAuSystemID(string openid)
        {
            string[] tmp = openid.Split('/');
            return tmp.Last();
        }

        private static string[] SeparateFirstAndLastNamesIfPossible(string nameStr)
        {
            string[] result = new string[2];

            if (nameStr.Contains(" "))
            {
                result = nameStr.Split(' ');
            }
            else if (nameStr.Contains("　"))
            {
                result = nameStr.Split('　');
            }
            else
            {
                result[0] = nameStr;
                result[1] = "";
            }

            return result;
        }
        public static bool IsNumeric(object expression)
        {
            if (expression == null)
                return false;

            string str = expression.ToString().Trim();

            // 既に数値型の場合は true
            if (expression is IConvertible convertible &&
                convertible.GetTypeCode() != TypeCode.String &&
                convertible.GetTypeCode() != TypeCode.Object)
            {
                return true;
            }

            // 文字列として数値か判定
            return double.TryParse(str, System.Globalization.NumberStyles.Any,
                                  System.Globalization.CultureInfo.InvariantCulture, out _);
        }


        #endregion

        #region Public Method

        /// <summary>
        /// ユーザ属性を取得します。これは新規登録時のデフォルト値を用意する目的なので、エラー時リトライしません。
        /// </summary>
        /// <param name="openIdFormatAuId">Uri形式のAuIDもしくはWowID</param>
        /// <returns></returns>
        public static AuUserInf GetUserInf(string openIdFormatAuId)
        {
            biscuitif biscuitInf = OwlRequest(GetAuSystemID(openIdFormatAuId));
            var results = new AuUserInf();

            if (biscuitInf != null && biscuitInf.resultStatus.Trim() == "0" && biscuitInf.csAttrib != null)
            {
                string[] kanjiNames = SeparateFirstAndLastNamesIfPossible(biscuitInf.csAttrib.nameKanji);
                results.FamilyName = kanjiNames[0];
                results.GivenName = kanjiNames[1];

                string[] kanaNames = SeparateFirstAndLastNamesIfPossible(biscuitInf.csAttrib.nameKana);
                results.FamilyKanaName = kanaNames[0];
                results.GivenKanaName = kanaNames[1];

                string yyyyMMddStr = biscuitInf.csAttrib.birthday;
                if (yyyyMMddStr.Length == 8)
                {
                    results.BirthYear = yyyyMMddStr.Substring(0, 4);
                    results.BirthMonth = int.Parse(yyyyMMddStr.Substring(4, 2)).ToString();
                    results.BirthDay = int.Parse(yyyyMMddStr.Substring(6, 2)).ToString();
                }

                switch (biscuitInf.csAttrib.sex.Trim())
                {
                    case "1":
                        results.Sex = QjSexTypeEnum.Male;
                        break;
                    case "2":
                        results.Sex = QjSexTypeEnum.Female;
                        break;
                    default:
                        results.Sex = QjSexTypeEnum.None;
                        break;
                }

                if (!string.IsNullOrEmpty(biscuitInf.csAttrib.eMail1) && biscuitInf.csAttrib.eMail1SendFlg.Trim() == "1")
                {
                    results.MailAddress = biscuitInf.csAttrib.eMail1;
                }
                else if (!string.IsNullOrEmpty(biscuitInf.csAttrib.eMail2) && biscuitInf.csAttrib.eMail2SendFlg.Trim() == "1")
                {
                    results.MailAddress = biscuitInf.csAttrib.eMail2;
                }
                else if (!string.IsNullOrEmpty(biscuitInf.csAttrib.ezMail) && biscuitInf.csAttrib.ezMailSendFlg.Trim() == "1")
                {
                    results.MailAddress = biscuitInf.csAttrib.ezMail;
                }
            }

            return results;
        }

        public static bool IsMobileSubscriberOfAu(string openIdFormatAuId)
        {
            biscuitif results = OwlRequest(GetAuSystemID(openIdFormatAuId));

            if (results != null && results.resultStatus.Trim() == "0" && results.auCntrctAttrib != null &&
                IsNumeric(results.auIdAttrib.auIdLink) && int.Parse(results.auIdAttrib.auIdLink) == 1)
            {
                foreach (var item in results.auCntrctAttrib)
                {
                    if (!string.IsNullOrEmpty(item.subscrCd.Trim()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 指定日にAuの携帯契約者かどうかを返します。非契約者なら1,契約者なら2,取得に失敗したら9を返します。
        /// AuIDを取得できた場合はAUIDを引数で返します。
        /// </summary>
        /// <param name="openId">他社のOpenID</param>
        /// <param name="timming"></param>
        /// <param name="auId"></param>
        /// <returns></returns>
        public static int IsMobileSubscriberOfAu(string openId, string timming, ref string auId)
        {
            biscuitif results = OwlRequestByOpenid(openId);

            if (results == null || results.resultStatus.Trim() != "0")
            {
                DebugLog("★契約情報取得失敗");
                return 9;
            }

            auId = results.auIdAttrib.auId;

            if (results.auCntrctAttrib != null && IsNumeric(results.auIdAttrib.auIdLink) &&
                int.Parse(results.auIdAttrib.auIdLink) == 1)
            {
                if (results.auCntrctAttrib != null && results.auCntrctAttrib.Any(m => !string.IsNullOrEmpty(m.subscrCd.Trim())))
                {
                    if (results.auCntrctAttrib.Any(m => string.IsNullOrWhiteSpace(m.auKaiyakuDay) ||
                        m.auKaiyakuDay.TryToValueType<int>(0) > timming.TryToValueType<int>(0)))
                    {
                        DebugLog("★Auの契約者です");
                        return 2;
                    }
                }
            }

            DebugLog("★Auの契約者ではないです");
            return 1;
        }

        #endregion
    }

}