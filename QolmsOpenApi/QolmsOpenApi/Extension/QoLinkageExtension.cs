using MGF.QOLMS.QolmsOpenApi.Models;
using MGF.QOLMS.QolmsOpenApi.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Extension
{
    /// <summary>
    /// 連携システム番号の各種変換を行う拡張メソッド
    /// </summary>
    public static class QoLinkageExtension
    {
        /// <summary>
        /// 連携システム番号からNotificationHubの設定に変換する
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <returns></returns>
        public static NotificationHubsSettings ToNotificationHubSettings(this int linkageSystemNo)
        {
            switch (linkageSystemNo)
            {
                case QoLinkage.TIS_LINKAGE_SYSTEM_NO:
                    return new NotificationHubsSettings
                    {
                        HubConnectionString = QoApiConfiguration.TisNotificationHubConnectionString,
                        HubName = QoApiConfiguration.TisNotificationHubName
                    };
                case QoLinkage.QOLMS_NAVI_LINKAGE_SYSTEM_NO:
                    return new NotificationHubsSettings
                    {
                        HubConnectionString = QoApiConfiguration.QolmsNaviNotificationHubConnectionString,
                        HubName = QoApiConfiguration.QolmsNaviNotificationHubName
                    };
                case QoLinkage.HEALTHDIARY_LINKAGE_SYSTEM_NO:
                    return new NotificationHubsSettings
                    {
                        HubConnectionString = QoApiConfiguration.HealthDiaryNotificationHubConnectionString,
                        HubName = QoApiConfiguration.HealthDiaryNotificationHubName
                    };
                case QoLinkage.MEINAVI_LINKAGE_SYSTEM_NO:
                    return new NotificationHubsSettings
                    {
                        HubConnectionString = QoApiConfiguration.MEINaviNotificationHubConnectionString,
                        HubName = QoApiConfiguration.MEINaviNotificationHubName
                    };
                case QoLinkage.JOTO_LINKAGE_SYSTEM_NO:
                    return new NotificationHubsSettings
                    {
                        HubConnectionString = QoApiConfiguration.QolmsJotoNotificationHubConnectionString,
                        HubName = QoApiConfiguration.QolmsJotoNotificationHubName
                    };
                default:
                    return null;
            }
        }

        /// <summary>
        /// 連携システム番号から対応するUrlSchemeに変換する
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <returns></returns>
        public static string ToUrlScheme(this int linkageSystemNo)
        {
            switch (linkageSystemNo)
            {
                case QoLinkage.TIS_LINKAGE_SYSTEM_NO:
                    return QoApiConfiguration.TisAppUrlScheme;
                case QoLinkage.QOLMS_NAVI_LINKAGE_SYSTEM_NO:
                    return QoApiConfiguration.QolmsNaviAppUrlScheme;
                case QoLinkage.MEINAVI_LINKAGE_SYSTEM_NO:
                    return QoApiConfiguration.MEINaviAppUrlScheme;
                case QoLinkage.JOTO_LINKAGE_SYSTEM_NO:
                    return QoApiConfiguration.MEINaviAppUrlScheme;
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// 連携システム番号が医療ナビ系(HOSPA/医療ナビ）であるかどうかを判定します。
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <returns></returns>
        public static bool IsNaviApp(this int linkageSystemNo)
        {
            switch(linkageSystemNo)
            {
                case QoLinkage.TIS_LINKAGE_SYSTEM_NO:
                case QoLinkage.QOLMS_NAVI_LINKAGE_SYSTEM_NO:
                case QoLinkage.MEINAVI_LINKAGE_SYSTEM_NO:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 連携システム番号が連携システム対象外(お薬手帳やKAGAMINO)かどうかを判定します。
        /// </summary>
        /// <param name="linkageSystemNo"></param>
        /// <returns>true: 連携システム対象外 / false: 連携システム対象</returns>
        public static bool IsNoLinkageSystem(this int linkageSystemNo)
        {
            switch (linkageSystemNo)
            {
                case QoLinkage.QOLMS_COMMON_LINKAGE_SYSTEM_NO:
                case QoLinkage.QOLMS_MEDICINE_APP_LINKAGE_SYSTEM_NO:
                    return true;
                default:
                    return false;
            }
        }
    }
}