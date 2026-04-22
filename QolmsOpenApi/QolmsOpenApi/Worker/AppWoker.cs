using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// スマホアプリに関する処理
    /// </summary>
    public class AppWorker
    {
        // コンフィグ設定名
        
        // アプリ判別パラメータ
        /// <summary>
        /// Qolmsお薬手帳
        /// </summary>
        public const string UrlParam_QolmsMedicineApp = "qm";
        /// <summary>
        /// KagaMino
        /// </summary>
        public const string UrlParam_KagaminoApp = "k";
        /// <summary>
        /// Tis HertPlus
        /// </summary>
        public const string UrlParam_TisApp = "t";
        /// <summary>
        /// 医療ナビ
        /// </summary>
        public const string UrlParam_QolmsNaviApp = "navi";

        /// <summary>
        /// MEIナビ
        /// </summary>
        public const string UrlParam_MeiNaviApp = "mei";

        /// <summary>
        /// 健康DIARY
        /// </summary>
        public const string UrlParam_HealthDiaryApp = "hd";

        /// <summary>
        /// 心拍見守り
        /// </summary>
        public const string UrlParam_HeartMonitorApp = "hm";

        /// <summary>
        /// 心拍見守り
        /// </summary>
        public const string UrlParam_JotoApp = "joto";

        /// <summary>
        /// メルアド変更用
        /// </summary>
        /// <param name="scheme"></param>
        /// <param name="mailAddressReference"></param>
        /// <param name="accountKeyReference"></param>
        /// <returns></returns>
        public static string ChangeMail(string scheme, string mailAddressReference, string accountKeyReference)
        {
            string result = string.Empty;

            if (!string.IsNullOrWhiteSpace(mailAddressReference) && !string.IsNullOrWhiteSpace(accountKeyReference))
            {
                Nullable<DateTime> timestamp = DateTime.MinValue;
                string key = accountKeyReference.ToDecrypedReference(ref timestamp);

                if (timestamp != null && timestamp >= DateTime.Now)
                {
                    Guid accountKey = key.ToValueType<Guid>();
                    // Dim scheme As String = ConfigurationManager.AppSettings("MinawellUrlScheme").Trim()

                    if (accountKey != Guid.Empty && !string.IsNullOrWhiteSpace(scheme))
                        result = string.Format("{0}changemail?mr={1}&ar={2}", scheme, mailAddressReference, accountKey.ToEncrypedReference());
                }
            }

            return result;
        }

        /// <summary>
        /// SystemTypeから対応するパラメータを返す
        /// </summary>
        /// <param name="systemType"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string GetUrlParam(QsApiSystemTypeEnum systemType)
        {
            return systemType.ToUrlParam();
        }

        /// <summary>
        /// パラメータからUrlスキームを返す
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string GetUrlScheme(string p)
        {
            return p.GetUrlScheme();            
        }

        /// <summary>
        /// パラメータからバージョンファイルを返す
        /// </summary>
        /// <param name="os"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string GetVersionFileName(string os,string p)
        {
            string result = string.Empty;
            switch (p)
            {
                case UrlParam_QolmsMedicineApp:
                    // Qolmsお薬手帳
                    result = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), string.Format("{0}_medicineapp_version.json", os.ToLower()));
                    break;
                case UrlParam_KagaminoApp:
                    // Kagamino(DataUploader)
                    result = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), string.Format("{0}_kagamino_version.json", os.ToLower()));
                    break;
                case UrlParam_TisApp:
                    // TIS 
                    result = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), string.Format("{0}_tis_version.json", os.ToLower()));
                    break;
                case UrlParam_QolmsNaviApp:
                    // 医療ナビ
                    result = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), string.Format("{0}_naviapp_version.json", os.ToLower()));
                    break;
                case UrlParam_MeiNaviApp:
                    // MEIナビ
                    result = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), string.Format("{0}_meiapp_version.json", os.ToLower()));
                    break;
                case "":
                    result = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), string.Format("{0}_minawell_version.json", os.ToLower()));
                    break;
            }
            return result;
        }
    }

}