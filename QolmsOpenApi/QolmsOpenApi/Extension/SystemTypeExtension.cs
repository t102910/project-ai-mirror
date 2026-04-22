using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Models;
using MGF.QOLMS.QolmsOpenApi.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Extension
{
    /// <summary>
    /// SystemTypeから各種変換・取得を行うExtension
    /// </summary>
    public static class SystemTypeExtension
    {
        /// <summary>
        /// QsApiSystemTypeEnumからQR有効期限の設定値を取得する
        /// </summary>
        /// <param name="systemType"></param>
        /// <returns></returns>
        public static QrExpirationConfig GetQrExpirationHour(this QsApiSystemTypeEnum systemType)
        {
            switch (systemType)
            {
                // HOSPA
                case QsApiSystemTypeEnum.TisiOSApp:
                case QsApiSystemTypeEnum.TisAndroidApp:
                case QsApiSystemTypeEnum.QolmsTisApp:
                    return QoApiConfiguration.TisQrExpirationHour;
                // 医療ナビ
                case QsApiSystemTypeEnum.QolmsNaviiOsApp:
                case QsApiSystemTypeEnum.QolmsNaviAndroidApp:
                case QsApiSystemTypeEnum.QolmsNaviApp:
                    return QoApiConfiguration.QolmsNaviQrExpirationHour;
                // MEIナビ
                case QsApiSystemTypeEnum.MeiNaviiOSApp:
                case QsApiSystemTypeEnum.MeiNaviAndroidApp:
                case QsApiSystemTypeEnum.MeiNaviApp:
                    return QoApiConfiguration.MEINaviQrExpirationHour;
                default:
                    return new QrExpirationConfig();
            }
        }

        /// <summary>
        /// QsApiSystemTypeEnumの数値文字列からQR有効期限の設定値を取得する
        /// </summary>
        /// <param name="systemTypeString"></param>
        /// <returns></returns>
        public static QrExpirationConfig GetQrExpirationHour(this string systemTypeString)
        {
            return systemTypeString.TryToValueType(QsApiSystemTypeEnum.None).GetQrExpirationHour();
        }

        /// <summary>
        /// QsApiSystemTypeEnumから対応する連携システム番号を取得する。
        /// 対応がなければ0を返す
        /// </summary>
        /// <param name="systemType"></param>
        /// <returns></returns>
        public static int ToLinkageSystemNo(this QsApiSystemTypeEnum systemType)
        {
            switch(systemType)
            {
                case QsApiSystemTypeEnum.QolmsiOSApp:
                case QsApiSystemTypeEnum.QolmsAndroidApp:
                case QsApiSystemTypeEnum.QolmsMedicineApp:
                    return QoLinkage.QOLMS_MEDICINE_APP_LINKAGE_SYSTEM_NO;
                case QsApiSystemTypeEnum.TisiOSApp:
                case QsApiSystemTypeEnum.TisAndroidApp:
                case QsApiSystemTypeEnum.QolmsTisApp:
                    return QoLinkage.TIS_LINKAGE_SYSTEM_NO;
                case QsApiSystemTypeEnum.QolmsNaviiOsApp:
                case QsApiSystemTypeEnum.QolmsNaviAndroidApp:
                case QsApiSystemTypeEnum.QolmsNaviApp:
                    return QoLinkage.QOLMS_NAVI_LINKAGE_SYSTEM_NO;
                case QsApiSystemTypeEnum.MeiNaviiOSApp:
                case QsApiSystemTypeEnum.MeiNaviAndroidApp:
                case QsApiSystemTypeEnum.MeiNaviApp:
                    return QoLinkage.MEINAVI_LINKAGE_SYSTEM_NO;
                case QsApiSystemTypeEnum.HealthDiaryiOSApp:
                case QsApiSystemTypeEnum.HealthDiaryAndroidApp:
                case QsApiSystemTypeEnum.HealthDiaryApp:
                    return QoLinkage.HEALTHDIARY_LINKAGE_SYSTEM_NO;
                case QsApiSystemTypeEnum.HeartMonitoriOSApp:
                case QsApiSystemTypeEnum.HeartMonitorAndroidApp:
                case QsApiSystemTypeEnum.HeartMonitorApp:
                    return QoLinkage.JOTO_HEARTMONITOR_SYSTEM_NO;
                case QsApiSystemTypeEnum.JotoNativeiOSApp:
                case QsApiSystemTypeEnum.JotoNativeAndroidApp:
                case QsApiSystemTypeEnum.QolmsJoto:
                case QsApiSystemTypeEnum.JotoNative:
                    return QoLinkage.JOTO_LINKAGE_SYSTEM_NO;
                // 対応が必要なら以下に追加
                default:
                    return 0;
            }
        }

        /// <summary>
        /// QsDbApplicationTypeEnumから対応する連携システム番号を取得する。
        /// </summary>
        /// <param name="toSystemType"></param>
        /// <returns></returns>
        public static int ToLinkageSystemNo(this QsDbApplicationTypeEnum toSystemType)
        {
            switch (toSystemType)
            {
                case QsDbApplicationTypeEnum.QolmsiOSApp:
                case QsDbApplicationTypeEnum.QolmsAndroidApp:
                case QsDbApplicationTypeEnum.QolmsMedicineApp:
                    return QoLinkage.QOLMS_MEDICINE_APP_LINKAGE_SYSTEM_NO;
                case QsDbApplicationTypeEnum.JotoiOSApp:
                case QsDbApplicationTypeEnum.JotoAndroidApp:
                    break;
                case QsDbApplicationTypeEnum.CcciOSApp:
                case QsDbApplicationTypeEnum.CccAndroidApp:
                    break;
                case QsDbApplicationTypeEnum.KagaminoiOSApp:
                case QsDbApplicationTypeEnum.KagaminoAndroidApp:
                    break;
                case QsDbApplicationTypeEnum.TisiOSApp:
                case QsDbApplicationTypeEnum.TisAndroidApp:
                case QsDbApplicationTypeEnum.QolmsTisApp:
                    return QoLinkage.TIS_LINKAGE_SYSTEM_NO;
                case QsDbApplicationTypeEnum.QolmsNaviiOsApp:
                case QsDbApplicationTypeEnum.QolmsNaviAndroidApp:
                case QsDbApplicationTypeEnum.QolmsNaviApp:
                    return QoLinkage.QOLMS_NAVI_LINKAGE_SYSTEM_NO;
                case QsDbApplicationTypeEnum.MeiNaviiOSApp:
                case QsDbApplicationTypeEnum.MeiNaviAndroidApp:
                case QsDbApplicationTypeEnum.MeiNaviApp:
                    return QoLinkage.MEINAVI_LINKAGE_SYSTEM_NO;
                case QsDbApplicationTypeEnum.HealthDiaryiOSApp:
                case QsDbApplicationTypeEnum.HealthDiaryAndroidApp:
                case QsDbApplicationTypeEnum.HealthDiaryApp:
                    return QoLinkage.HEALTHDIARY_LINKAGE_SYSTEM_NO;
                case QsDbApplicationTypeEnum.HeartMonitoriOSApp:
                case QsDbApplicationTypeEnum.HeartMonitorAndroidApp:
                case QsDbApplicationTypeEnum.HeartMonitorApp:
                    return QoLinkage.JOTO_HEARTMONITOR_SYSTEM_NO;
                case QsDbApplicationTypeEnum.JotoNativeiOSApp:
                case QsDbApplicationTypeEnum.JotoNativeAndroidApp:
                case QsDbApplicationTypeEnum.JotoNative:
                    return QoLinkage.JOTO_LINKAGE_SYSTEM_NO;
                default:
                    break;
            }
            return 99999;
        }

        /// <summary>
        /// QsApiSystemTypeEnumの数値文字列から連携システム番号を取得する
        /// </summary>
        /// <param name="systemTypeString"></param>
        /// <returns></returns>
        public static int ToLinkageSystemNo(this string systemTypeString)
        {
            return ((QsApiSystemTypeEnum)systemTypeString.TryToValueType((int)QsApiSystemTypeEnum.None)).ToLinkageSystemNo();
        }

        /// <summary>
        /// QsApiSystemTypeEnumからシステム名称を取得する
        /// </summary>
        /// <param name="systemType"></param>
        /// <returns></returns>
        public static string ToSystemName(this QsApiSystemTypeEnum systemType)
        {
            switch (systemType)
            {
                case QsApiSystemTypeEnum.QolmsiOSApp:
                case QsApiSystemTypeEnum.QolmsAndroidApp:
                case QsApiSystemTypeEnum.QolmsMedicineApp:
                    return "お薬手帳QOLMS";
                case QsApiSystemTypeEnum.QolmsNaviApp:
                case QsApiSystemTypeEnum.QolmsNaviiOsApp:
                case QsApiSystemTypeEnum.QolmsNaviAndroidApp:
                    return "医療ナビ";
                case QsApiSystemTypeEnum.MeiNaviApp:                
                case QsApiSystemTypeEnum.MeiNaviiOSApp:
                case QsApiSystemTypeEnum.MeiNaviAndroidApp:
                    return "ツーイン";
                case QsApiSystemTypeEnum.QolmsTisApp:
                case QsApiSystemTypeEnum.TisiOSApp:
                case QsApiSystemTypeEnum.TisAndroidApp:
                    return "HOSPA";
                case QsApiSystemTypeEnum.KagaminoiOSApp:
                case QsApiSystemTypeEnum.KagaminoAndroidApp:
                    return "KAGAMINO";
                case QsApiSystemTypeEnum.HealthDiaryiOSApp:
                case QsApiSystemTypeEnum.HealthDiaryAndroidApp:
                case QsApiSystemTypeEnum.HealthDiaryApp:
                    return "健幸DX手帳";
                case QsApiSystemTypeEnum.HeartMonitoriOSApp:
                case QsApiSystemTypeEnum.HeartMonitorAndroidApp:
                case QsApiSystemTypeEnum.HeartMonitorApp:
                    return "心拍見守りアプリ";
                case QsApiSystemTypeEnum.JotoiOSApp:
                case QsApiSystemTypeEnum.JotoAndroidApp:
                case QsApiSystemTypeEnum.JotoNativeiOSApp:
                case QsApiSystemTypeEnum.JotoNativeAndroidApp:
                    return "JOTOホームドクター";
                default:
                    return "QOLMS";
            }
        }

        /// <summary>
        /// QsApiSystemTypeEnumからOSに対応するSystemTypeリストを返す
        /// 例えばHOSPAのiOSのシステムタイプが渡されるとHOSPA(全体)というコードを含んだリストを返します。
        /// お知らせの取得等で利用します。
        /// </summary>
        /// <param name="systemType"></param>
        /// <returns></returns>
        public static List<byte> ToApplicationTypeByteList(this QsApiSystemTypeEnum systemType)
        {
            // 自身をまず含める
            var appSystemTypeList = new List<byte> { (byte)systemType };
            // アプリごとに全体を表すコードを追加する
            switch (systemType)
            {
                // HOSPA
                case QsApiSystemTypeEnum.TisiOSApp:
                case QsApiSystemTypeEnum.TisAndroidApp:
                    appSystemTypeList.Add((byte)QsApiSystemTypeEnum.QolmsTisApp);
                    break;
                // 医療ナビ
                case QsApiSystemTypeEnum.QolmsNaviiOsApp:
                case QsApiSystemTypeEnum.QolmsNaviAndroidApp:
                    appSystemTypeList.Add((byte)QsApiSystemTypeEnum.QolmsNaviApp);
                    break;
                // MEIナビ
                case QsApiSystemTypeEnum.MeiNaviiOSApp:
                case QsApiSystemTypeEnum.MeiNaviAndroidApp:
                    appSystemTypeList.Add((byte)QsApiSystemTypeEnum.MeiNaviApp);
                    break;
                // 健康DIARY
                case QsApiSystemTypeEnum.HealthDiaryiOSApp:
                case QsApiSystemTypeEnum.HealthDiaryAndroidApp:
                    appSystemTypeList.Add((byte)QsApiSystemTypeEnum.HealthDiaryApp);
                    break;
                // 心拍見守りアプリ
                case QsApiSystemTypeEnum.HeartMonitoriOSApp:
                case QsApiSystemTypeEnum.HeartMonitorAndroidApp:
                    appSystemTypeList.Add((byte)QsApiSystemTypeEnum.HeartMonitorApp);
                    break;
                // お薬手帳
                case QsApiSystemTypeEnum.QolmsiOSApp:
                case QsApiSystemTypeEnum.QolmsAndroidApp:
                    appSystemTypeList.Add((byte)QsApiSystemTypeEnum.QolmsMedicineApp);
                    break;
                // JOTO
                case QsApiSystemTypeEnum.JotoNativeiOSApp:
                case QsApiSystemTypeEnum.JotoNativeAndroidApp:
                    appSystemTypeList.Add((byte)QsApiSystemTypeEnum.JotoNative);
                    break;
                // 必要に応じて追加
                default:
                    break;
            }
            return appSystemTypeList;
        }

        /// <summary>
        /// QsApiSystemTypeEnumの数値文字列から対応するQsDbApplicationTypeEnumを返す
        /// Platformを区別しないアプリを識別するコードを取得するために利用する
        /// </summary>
        /// <param name="systemTypeString"></param>
        /// <returns></returns>
        public static QsDbApplicationTypeEnum ToApplicationType(this string systemTypeString)
        {
            var systemType = systemTypeString.TryToValueType(QsApiSystemTypeEnum.None);

            switch (systemType)
            {
                // HOSPA
                case QsApiSystemTypeEnum.TisiOSApp:
                case QsApiSystemTypeEnum.TisAndroidApp:
                case QsApiSystemTypeEnum.QolmsTisApp:
                    return QsDbApplicationTypeEnum.QolmsTisApp;
                // 医療ナビ
                case QsApiSystemTypeEnum.QolmsNaviiOsApp:
                case QsApiSystemTypeEnum.QolmsNaviAndroidApp:
                case QsApiSystemTypeEnum.QolmsNaviApp:
                    return QsDbApplicationTypeEnum.QolmsNaviApp;
                // MEIナビ
                case QsApiSystemTypeEnum.MeiNaviiOSApp:
                case QsApiSystemTypeEnum.MeiNaviAndroidApp:
                case QsApiSystemTypeEnum.MeiNaviApp:
                    return QsDbApplicationTypeEnum.MeiNaviApp;
                // Kagamino
                case QsApiSystemTypeEnum.KagaminoiOSApp:
                case QsApiSystemTypeEnum.KagaminoAndroidApp:
                    return QsDbApplicationTypeEnum.KagaminoApp;
                // 心拍見守りアプリ
                case QsApiSystemTypeEnum.HeartMonitoriOSApp:
                case QsApiSystemTypeEnum.HeartMonitorAndroidApp:
                case QsApiSystemTypeEnum.HeartMonitorApp:
                    return QsDbApplicationTypeEnum.HeartMonitorApp;
                // 健康DIARY
                case QsApiSystemTypeEnum.HealthDiaryiOSApp:
                case QsApiSystemTypeEnum.HealthDiaryAndroidApp:
                case QsApiSystemTypeEnum.HealthDiaryApp:
                    return QsDbApplicationTypeEnum.HealthDiaryApp;
                // Live2DWeb
                case QsApiSystemTypeEnum.QolmsLive2DWeb:
                    return QsDbApplicationTypeEnum.QolmsLive2DWeb;
                // お薬手帳QOLMS
                case QsApiSystemTypeEnum.QolmsiOSApp:
                case QsApiSystemTypeEnum.QolmsAndroidApp:
                case QsApiSystemTypeEnum.QolmsMedicineApp:
                    return QsDbApplicationTypeEnum.QolmsMedicineApp;
                // お薬手帳QOLMS
                case QsApiSystemTypeEnum.JotoNativeiOSApp:
                case QsApiSystemTypeEnum.JotoNativeAndroidApp:
                case QsApiSystemTypeEnum.JotoNative:
                    return QsDbApplicationTypeEnum.JotoNative;
                default:
                    return QsDbApplicationTypeEnum.None;
            }
        }

        /// <summary>
        /// QsDbApplicationTypeEnumに対応するPush通知クラスのインスタンスを返す
        /// </summary>
        /// <param name="toSystemType"></param>
        /// <returns></returns>
        public static QoPushNotification ToNotificationInstance(this QsDbApplicationTypeEnum toSystemType)
        {
            switch (toSystemType)
            {
                case QsDbApplicationTypeEnum.QolmsiOSApp:
                case QsDbApplicationTypeEnum.QolmsAndroidApp:
                    break;
                case QsDbApplicationTypeEnum.JotoiOSApp:
                case QsDbApplicationTypeEnum.JotoAndroidApp:
                case QsDbApplicationTypeEnum.JotoNativeiOSApp:
                case QsDbApplicationTypeEnum.JotoNativeAndroidApp:
                case QsDbApplicationTypeEnum.JotoNative:
                    return new QoPushNotification(QoApiConfiguration.QolmsJotoNotificationHubConnectionString, QoApiConfiguration.QolmsJotoNotificationHubName);
                case QsDbApplicationTypeEnum.CcciOSApp:
                case QsDbApplicationTypeEnum.CccAndroidApp:
                    break;
                case QsDbApplicationTypeEnum.KagaminoiOSApp:
                case QsDbApplicationTypeEnum.KagaminoAndroidApp:
                    break;
                case QsDbApplicationTypeEnum.TisiOSApp:
                case QsDbApplicationTypeEnum.TisAndroidApp:
                case QsDbApplicationTypeEnum.QolmsTisApp:
                    return new QoPushNotification(QoApiConfiguration.TisNotificationHubConnectionString, QoApiConfiguration.TisNotificationHubName);
                case QsDbApplicationTypeEnum.QolmsNaviiOsApp:
                case QsDbApplicationTypeEnum.QolmsNaviAndroidApp:
                case QsDbApplicationTypeEnum.QolmsNaviApp:
                    return new QoPushNotification(QoApiConfiguration.QolmsNaviNotificationHubConnectionString, QoApiConfiguration.QolmsNaviNotificationHubName);
                case QsDbApplicationTypeEnum.MeiNaviiOSApp:
                case QsDbApplicationTypeEnum.MeiNaviAndroidApp:
                case QsDbApplicationTypeEnum.MeiNaviApp:
                    return new QoPushNotification(QoApiConfiguration.MEINaviNotificationHubConnectionString, QoApiConfiguration.MEINaviNotificationHubName);
                case QsDbApplicationTypeEnum.HealthDiaryiOSApp:
                case QsDbApplicationTypeEnum.HealthDiaryAndroidApp:
                case QsDbApplicationTypeEnum.HealthDiaryApp:
                    return new QoPushNotification(QoApiConfiguration.HealthDiaryNotificationHubConnectionString, QoApiConfiguration.HealthDiaryNotificationHubName);

                default:
                    break;
            }
            return null;
        }

        /// <summary>
        /// QsDbApplicationTypeEnumから対応するURLスキームを返す
        /// </summary>
        /// <param name="toSystemType"></param>
        /// <returns></returns>
        public static string ToAppUrlScheme(this QsDbApplicationTypeEnum toSystemType)
        {
            switch (toSystemType)
            {
                // お薬手帳QOLMS
                case QsDbApplicationTypeEnum.QolmsiOSApp:
                case QsDbApplicationTypeEnum.QolmsAndroidApp:
                case QsDbApplicationTypeEnum.QolmsMedicineApp:
                    return QoApiConfiguration.QolmsMedicineAppUrlScheme;
                case QsDbApplicationTypeEnum.JotoiOSApp:
                case QsDbApplicationTypeEnum.JotoAndroidApp:
                case QsDbApplicationTypeEnum.JotoNativeiOSApp:
                case QsDbApplicationTypeEnum.JotoNativeAndroidApp:
                    return QoApiConfiguration.JotoAppUrlScheme;
                    break;
                case QsDbApplicationTypeEnum.CcciOSApp:
                case QsDbApplicationTypeEnum.CccAndroidApp:
                    break;
                // KAGAMINO
                case QsDbApplicationTypeEnum.KagaminoiOSApp:
                case QsDbApplicationTypeEnum.KagaminoAndroidApp:
                case QsDbApplicationTypeEnum.KagaminoApp:
                    return QoApiConfiguration.KagaminoAppUrlScheme;
                // HOSPA
                case QsDbApplicationTypeEnum.TisiOSApp:
                case QsDbApplicationTypeEnum.TisAndroidApp:
                case QsDbApplicationTypeEnum.QolmsTisApp:
                    return QoApiConfiguration.TisAppUrlScheme;
                // 医療ナビ
                case QsDbApplicationTypeEnum.QolmsNaviiOsApp:
                case QsDbApplicationTypeEnum.QolmsNaviAndroidApp:
                case QsDbApplicationTypeEnum.QolmsNaviApp:
                    return QoApiConfiguration.QolmsNaviAppUrlScheme;
                // MEIナビ
                case QsDbApplicationTypeEnum.MeiNaviiOSApp:
                case QsDbApplicationTypeEnum.MeiNaviAndroidApp:
                case QsDbApplicationTypeEnum.MeiNaviApp:
                    return QoApiConfiguration.MEINaviAppUrlScheme;
                // 健康DIARY
                case QsDbApplicationTypeEnum.HealthDiaryiOSApp:
                case QsDbApplicationTypeEnum.HealthDiaryAndroidApp:
                case QsDbApplicationTypeEnum.HealthDiaryApp:
                    return QoApiConfiguration.HealthDiaryAppUrlScheme;
                // 心拍見守りアプリ
                case QsDbApplicationTypeEnum.HeartMonitoriOSApp:
                case QsDbApplicationTypeEnum.HeartMonitorAndroidApp:
                case QsDbApplicationTypeEnum.HeartMonitorApp:
                    return QoApiConfiguration.HeartMonitorAppUrlScheme;
                default:
                    break;
            }
            return null;
        }

        /// <summary>
        /// QsDbApplicationTypeEnumから対応するURLパラメータ文字を返す
        /// </summary>
        /// <param name="systemType"></param>
        /// <returns></returns>
        public static string ToUrlParam(this QsDbApplicationTypeEnum systemType)
        {
            switch (systemType)
            {
                // お薬手帳QOLMS
                case QsDbApplicationTypeEnum.QolmsiOSApp:
                case QsDbApplicationTypeEnum.QolmsAndroidApp:
                case QsDbApplicationTypeEnum.QolmsMedicineApp:
                    return AppWorker.UrlParam_QolmsMedicineApp;
                case QsDbApplicationTypeEnum.JotoiOSApp:
                case QsDbApplicationTypeEnum.JotoAndroidApp:
                    break;
                case QsDbApplicationTypeEnum.CcciOSApp:
                case QsDbApplicationTypeEnum.CccAndroidApp:
                    break;
                // KAGAMINO
                case QsDbApplicationTypeEnum.KagaminoiOSApp:
                case QsDbApplicationTypeEnum.KagaminoAndroidApp:
                case QsDbApplicationTypeEnum.KagaminoApp:
                    return AppWorker.UrlParam_KagaminoApp;
                // HOSPA
                case QsDbApplicationTypeEnum.TisiOSApp:
                case QsDbApplicationTypeEnum.TisAndroidApp:
                case QsDbApplicationTypeEnum.QolmsTisApp:
                    return AppWorker.UrlParam_TisApp;
                // 医療ナビ
                case QsDbApplicationTypeEnum.QolmsNaviiOsApp:
                case QsDbApplicationTypeEnum.QolmsNaviAndroidApp:
                case QsDbApplicationTypeEnum.QolmsNaviApp:
                    return AppWorker.UrlParam_QolmsNaviApp;
                // MEIナビ
                case QsDbApplicationTypeEnum.MeiNaviiOSApp:
                case QsDbApplicationTypeEnum.MeiNaviAndroidApp:
                case QsDbApplicationTypeEnum.MeiNaviApp:
                    return AppWorker.UrlParam_MeiNaviApp;
                // 健康DIARY
                case QsDbApplicationTypeEnum.HealthDiaryiOSApp:
                case QsDbApplicationTypeEnum.HealthDiaryAndroidApp:
                case QsDbApplicationTypeEnum.HealthDiaryApp:
                    return AppWorker.UrlParam_HealthDiaryApp;
                // 心拍見守りアプリ
                case QsDbApplicationTypeEnum.HeartMonitoriOSApp:
                case QsDbApplicationTypeEnum.HeartMonitorAndroidApp:
                case QsDbApplicationTypeEnum.HeartMonitorApp:
                    return AppWorker.UrlParam_HeartMonitorApp;
                default:
                    break;
            }
            return string.Empty;
        }

        /// <summary>
        /// QsApiSystemTypeEnumから対応するURLパラメータ文字を返す
        /// </summary>
        /// <param name="systemType"></param>
        /// <returns></returns>
        public static string ToUrlParam(this QsApiSystemTypeEnum systemType)
        {
            switch (systemType)
            {
                // お薬手帳QOLMS
                case QsApiSystemTypeEnum.QolmsiOSApp:
                case QsApiSystemTypeEnum.QolmsAndroidApp:
                case QsApiSystemTypeEnum.QolmsMedicineApp:
                    return AppWorker.UrlParam_QolmsMedicineApp;
                case QsApiSystemTypeEnum.JotoiOSApp:
                case QsApiSystemTypeEnum.JotoAndroidApp:
                    break;
                case QsApiSystemTypeEnum.CcciOSApp:
                case QsApiSystemTypeEnum.CccAndroidApp:
                    break;
                // KAGAMINO
                case QsApiSystemTypeEnum.KagaminoiOSApp:
                case QsApiSystemTypeEnum.KagaminoAndroidApp:
                    return AppWorker.UrlParam_KagaminoApp;
                // HOSPA
                case QsApiSystemTypeEnum.TisiOSApp:
                case QsApiSystemTypeEnum.TisAndroidApp:
                case QsApiSystemTypeEnum.QolmsTisApp:
                    return AppWorker.UrlParam_TisApp;
                // 医療ナビ
                case QsApiSystemTypeEnum.QolmsNaviiOsApp:
                case QsApiSystemTypeEnum.QolmsNaviAndroidApp:
                case QsApiSystemTypeEnum.QolmsNaviApp:
                    return AppWorker.UrlParam_QolmsNaviApp;
                // MEIナビ
                case QsApiSystemTypeEnum.MeiNaviiOSApp:
                case QsApiSystemTypeEnum.MeiNaviAndroidApp:
                case QsApiSystemTypeEnum.MeiNaviApp:
                    return AppWorker.UrlParam_MeiNaviApp;
                // 健康DIARY
                case QsApiSystemTypeEnum.HealthDiaryiOSApp:
                case QsApiSystemTypeEnum.HealthDiaryAndroidApp:
                case QsApiSystemTypeEnum.HealthDiaryApp:
                    return AppWorker.UrlParam_HealthDiaryApp;
                // 心拍見守り
                case QsApiSystemTypeEnum.HeartMonitoriOSApp:
                case QsApiSystemTypeEnum.HeartMonitorAndroidApp:
                case QsApiSystemTypeEnum.HeartMonitorApp:
                    return AppWorker.UrlParam_HeartMonitorApp;
                // JOTO
                case QsApiSystemTypeEnum.JotoNativeiOSApp:
                case QsApiSystemTypeEnum.JotoNativeAndroidApp:
                case QsApiSystemTypeEnum.JotoNative:
                    return AppWorker.UrlParam_JotoApp;
                default:
                    break;
            }
            return string.Empty;
        }
    }
}