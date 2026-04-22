using MGF.QOLMS.QolmsOpenApi.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Extension
{
    /// <summary>
    /// URLスキーム関連
    /// </summary>
    public static class UrlSchemeExtension
    {
        /// <summary>
        /// パラメータ文字列に対応したURLスキームを返す
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static string GetUrlScheme(this string parameter)
        {
            switch (parameter)
            {
                case AppWorker.UrlParam_QolmsMedicineApp:
                    // Qolmsお薬手帳
                    return QoApiConfiguration.QolmsMedicineAppUrlScheme;
                case AppWorker.UrlParam_KagaminoApp:
                    // Kagamino(DataUploader)
                    return QoApiConfiguration.KagaminoAppUrlScheme;
                case AppWorker.UrlParam_TisApp:
                    // HOSPA
                    return QoApiConfiguration.TisAppUrlScheme;
                case AppWorker.UrlParam_QolmsNaviApp:
                    // 医療ナビ
                    return QoApiConfiguration.QolmsNaviAppUrlScheme;
                case AppWorker.UrlParam_MeiNaviApp:
                    // MEIナビ
                    return QoApiConfiguration.MEINaviAppUrlScheme;
                case AppWorker.UrlParam_HealthDiaryApp:
                    // 健康DIARY
                    return QoApiConfiguration.HealthDiaryAppUrlScheme;
                    // 心拍見守りアプリ
                case AppWorker.UrlParam_HeartMonitorApp:
                    return QoApiConfiguration.HeartMonitorAppUrlScheme;
                case AppWorker.UrlParam_JotoApp:
                    // JOTOネイティブ
                    return QoApiConfiguration.JotoAppUrlScheme;
            }
            return string.Empty;
        }
    }
}