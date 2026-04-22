using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using MGF.QOLMS.QolmsOpenApi.Models;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    internal class QoApiConfiguration
    {
        #region "Public Constant"
        /// <summary>
        /// 電話番号の正規表現を表します。
        /// </summary>
        public const string REGEX_TEL = @"^0\d{1,3}[-]?\d{1,4}[-]?\d{4}$";
        #endregion
        #region "Constant"

        /// <summary>
        /// SignUpのメール送信制限数
        /// </summary>
        private const string Config_SignUpMailAddressMaxCount_KeyName = "MailAddressCount";
        private const int Config_SignUpMailAddressMaxCount_DefaultValue = 5;

        /// <summary>
        /// SignUpのURL
        /// </summary>
        private const string Config_SignUpUrl_KeyName = "SignUpUrl";
        private const string Config_SignUpUrl_Defaultvalue = "https://qolms-dev-core-west-api11.azurewebsites.net/v1/app/signup";

        /// <summary>
        /// TIS アプリURLスキーム
        /// </summary>
        private const string Config_TisAppUrlScheme_KeyName = "TisUrlScheme";
        private const string Config_TisAppUrlScheme_DefaultValue = "heart-plus://";

        /// <summary>
        /// Qolmsお薬手帳アプリURLスキーム
        /// </summary>
        private const string Config_QolmsMedicineAppUrlScheme_KeyName = "QolmsMedicineUrlScheme";
        private const string Config_QolmsMedicineAppUrlScheme_DefaultValue = "qolms-medicine://";

        /// <summary>
        /// KagaminoアプリURLスキーム
        /// </summary>
        private const string Config_KagaminoAppUrlScheme_KeyName = "KagaminoUrlScheme";
        private const string Config_KagaminoAppUrlScheme_DefaultValue = "qolms-uploader://";

        /// <summary>
        /// 健康DIARY URLスキーム
        /// </summary>
        private const string Config_HealthDiaryAppUrlScheme_KeyName = "HealthDiaryUrlScheme";
        private const string Config_HealthDiaryAppUrlScheme_DefaultValue = "healthdiary://";

        /// <summary>
        /// 医療ナビアプリURLスキーム
        /// </summary>
        private const string Config_QolmsNaviAppUrlScheme_KeyName = "QolmsNaviUrlScheme";
        private const string Config_QolmsNaviAppUrlScheme_DefaultValue = "qolms-navi://";

        /// <summary>
        /// MEIナビアプリURLスキーム
        /// </summary>
        private const string Config_MEINaviAppUrlScheme_KeyName = "MEINaviUrlScheme";
        private const string Config_MEINaviAppUrlScheme_DefaultValue = "mei-navi://";

        /// <summary>
        /// 心拍見守りアプリ URLスキーム
        /// </summary>
        private const string Config_HeartMonitorAppUrlScheme_KeyName = "HeartMonitorUrlScheme";
        private const string Config_HeartMonitorAppUrlScheme_DefaultValue = "heartmonitor://";

        /// <summary>
        /// Joto アプリURLスキーム
        /// </summary>
        private const string Config_JotoAppUrlScheme_KeyName = "JotoUrlScheme";
        private const string Config_JotoAppUrlScheme_DefaultValue = "JotoHdr://";

        /// <summary>
        /// パスワードリセットUrl
        /// </summary>
        private const string Config_PasswordResetUrl_KeyName = "PasswordResetUrl";
        private const string Config_PasswordResetUrl_DefaultValue = "https://qolms-dev-core-west-api11.azurewebsites.net/v1/app/resetpw";

        /// <summary>
        /// メールアドレス変更Url
        /// </summary>
        private const string Config_MailAddressChangeUrl_KeyName = "MailAddressChangeUrl";
        private const string Config_MailAddressChangeUrl_DefaultValue = "https://qolms-dev-core-west-api11.azurewebsites.net/v1/app/chmail";

        /// <summary>
        /// Tis の QR コードの有効期限
        /// 受付票の期限、予約票の期限をカンマ区切りで指定
        /// </summary>
        private const string Config_Tis_Qr_Expiration_Hour_KeyName = "TisQrExpirationHour";
        private const string Config_Tis_Qr_Expiration_Hour_DefaultValue = "24,48";


        /// <summary>
        /// 医療ナビの QRコードの有効期限
        /// 受付票の期限、予約票の期限をカンマ区切りで指定
        /// </summary>
        private const string Config_QolmsNavi_Qr_Expiration_Hour_KeyName = "QolmsNaviQrExpirationHour";
        private const string Config_QolmsNavi_Qr_Expiration_Hour_DefaultValue = "24,48";

        /// <summary>
        /// MEIナビの QRコードの有効期限
        /// 受付票の期限、予約票の期限をカンマ区切りで指定
        /// </summary>
        private const string Config_MEINavi_Qr_Expiration_Hour_KeyName = "MEINaviQrExpirationHour";
        private const string Config_MEINavi_Qr_Expiration_Hour_DefaultValue = "24,48";

        /// <summary>
        /// Tis のPush通知用 Azure NotificationHubs設定
        /// </summary>
        private const string Config_Tis_NotificationHubs_ConnectionString_KeyName = "TisNotificationHubConnectionString";
        private const string Config_Tis_NotificationHubs_ConnectionString_DefaultValue = "Endpoint=sb://TisPhrAppProd.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=kv3DlNCKqa4QbR7TclJo0jSkI3V5rvAFn0vRNxCFMM8=";

        /// <summary>
        /// Tis のPush通知用 Azure NotificationHubs設定
        /// </summary>
        private const string Config_Tis_NotificationHubs_Name_KeyName = "TisNotificationHubName";
        private const string Config_Tis_NotificationHubs_Name_DefaultValue = "TisPhrAppNotificationHubDev";


        /// <summary>
        /// 医療ナビのPush通知用 Azure NotificationHubs設定
        /// </summary>
        private const string Config_QolmsNavi_NotificationHubs_ConnectionString_KeyName = "QolmsNaviNotificationHubConnectionString";
        private const string Config_QolmsNavi_NotificationHubs_ConnectionString_DefaultValue = "Endpoint=sb://QolmsNotificationHubNameSpace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=MpJ6PyhEvsVImFzReBspJj5o21EvglqwjGhZZq4faXI=";
        private const string Config_QolmsNavi_NotificationHubs_Name_KeyName = "QolmsNaviNotificationHubName";
        private const string Config_QolmsNavi_NotificationHubs_Name_DefaultValue = "MedicalNaviNotificationHubDev";

        /// <summary>
        /// MEIナビのPush通知用 Azure NotificationHubs設定
        /// </summary>
        private const string Config_MEINavi_NotificationHubs_ConnectionString_KeyName = "MEINaviNotificationHubConnectionString";
        private const string Config_MEINavi_NotificationHubs_ConnectionString_DefaultValue = "Endpoint=sb://QolmsNotificationHubNameSpace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=suziV/B+hFxiMww5IaPnDUOtx4xJlzwm9E9uxhWJJww=";
        private const string Config_MEINavi_NotificationHubs_Name_KeyName = "MEINaviNotificationHubName";
        private const string Config_MEINavi_NotificationHubs_Name_DefaultValue = "MeiNaviNotificationHubDev";

        /// <summary>
        /// 健康DIARYのPush通知用 Azure NotificationHubs設定
        /// </summary>
        private const string Config_HealthDiary_NotificationHubs_ConnectionString_KeyName = "HealthDiaryNotificationHubConnectionString";
        private const string Config_HealthDiary_NotificationHubs_ConnectionString_DefaultValue = "Endpoint=sb://QolmsNotificationHubNameSpace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=HuLQeakhmymVQwqlmA/Iarg/eGCvrsqRrddJaUaj7Ps=";
        private const string Config_HealthDiary_NotificationHubs_Name_KeyName = "HealthDiaryNotificationHubName";
        private const string Config_HealthDiary_NotificationHubs_Name_DefaultValue = "HealthDiaryNotificationHubDev";

        /// <summary>
        /// JotoのPush通知用 Azure NotificationHubs設定
        /// </summary>
        private const string Config_QolmsJoto_NotificationHubs_ConnectionString_KeyName = "QolmsJotoNotificationHubConnectionString";
        private const string Config_QolmsJoto_NotificationHubs_ConnectionString_DefaultValue = "Endpoint=sb://JotoNotificationHubNameSpace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=5kt4vSlJFEx8wbKBMGfF0RY5Xai8TN87OJquQJWP4Xg=";
        private const string Config_QolmsJoto_NotificationHubs_Name_KeyName = "QolmsJotoNotificationHubName";
        private const string Config_QolmsJoto_NotificationHubs_Name_DefaultValue = "JotoNotificationHubDev";

        /// <summary>
        /// アカウント再登録可能間隔
        /// </summary>
        private const string Config_NewUserRegister_Interval_KeyName = "NewUserRegisterInterval";
        private const int Config_NewUserRegister_Interval_DefaultValue = 1440;//24時間

        /// <summary>
        /// LineFollowEventURL
        /// </summary>
        private const string Config_LineFollowEvent_Url_KeyName = "LineFollowEventURL";
        private const string Config_LineFollowEvent_Url_DefaultValue = "https://localhost/";

        /// <summary>
        /// LineFirstEventURL
        /// </summary>
        private const string Config_LineFirstEvent_Url_KeyName = "LineFirstEventURL";
        private const string Config_LineFirstEvent_Url_DefaultValue = "https://localhost/";

        /// <summary>
        /// LineSecondEventURL
        /// </summary>
        private const string Config_LineSecondEvent_Url_KeyName = "LineSecondEventURL";
        private const string Config_LineSecondEvent_Url_DefaultValue = "https://localhost/";

        /// <summary>
        /// LineFifthEventURL
        /// </summary>
        private const string Config_LineFifthEvent_Url_KeyName = "LineFifthEventURL";
        private const string Config_LineFifthEvent_Url_DefaultValue = "https://localhost/";

        /// <summary>
        /// HAIPAPIKEY
        /// </summary>
        private const string Config_HaipApiKey_KeyName = "HaipApiKey";
        private const string Config_HaipApiKey_DefaultValue = "devapiv2.qolms.com-a2bf19dc-ca88-44fb-935f-df7448c30372";

        /// <summary>
        /// Smapa Home取得API URL
        /// </summary>
        private const string Config_SmapaApiHomeUrl_KeyName = "SmapaApiHomeUrl";
        private const string Config_SmapaApiHomeUrl_DefaultValue = "https://web.dev.smapa-checkout.jp/api/v1/checkout/wtr/01";

        /// <summary>
        /// Smapa 退会API URL
        /// </summary>
        private const string Config_SmapaApiRevokeUrl_KeyName = "SmapaApiRevokeUrl";
        private const string Config_SmapaApiRevokeUrl_DefaultValue = "https://web.dev.smapa-checkout.jp/api/v1/checkout/wtr/09";

        /// <summary>
        /// Smapa APIキー
        /// </summary>
        private const string Config_SmapaApiKey_KeyName = "SmapaApiKey";
        private const string Config_SmapaApiKey_DefaultValue = "12345678";

        /// <summary>
        /// ファイル投稿 Zip内のファイル数制限
        /// </summary>
        private const string Config_FilePostingLimit_KeyName = "FilePostingLimit";
        private const string Config_FilePostingLimit_DefaultValue = "100";

        /// <summary>
        /// Joto ぎのわんPJ 基盤 API  ClientId
        /// </summary>
        private const string Config_JotoGinowanClientId_KeyName = "JotoGinowanClientId";
        private const string Config_JotoGinowanClientId_Defaultvalue = "ClientId";

        /// <summary>
        /// Joto ぎのわんPJ 基盤 API  UserName
        /// </summary>
        private const string Config_JotoGinowanUserName_KeyName = "JotoGinowanUserName";
        private const string Config_JotoGinowanUserName_Defaultvalue = "UserName";

        /// <summary>
        /// Joto ぎのわんPJ 基盤 API  Password
        /// </summary>
        private const string Config_JotoGinowanPassword_KeyName = "JotoGinowanPassword";
        private const string Config_JotoGinowanPassword_Defaultvalue = "Password";

        /// <summary>
        /// Joto ぎのわんPJ 基盤 API  Auth API Uri
        /// </summary>
        private const string Config_JotoGinowanAuthApiUri_KeyName = "JotoGinowanAuthApiUri";
        private const string Config_JotoGinowanAuthApiUri_Defaultvalue = "https://localhost/";

        /// <summary>
        /// Joto ぎのわんPJ 基盤 API  Point API Base Uri
        /// </summary>
        private const string Config_JotoGinowanPointApiBaseUri_KeyName = "JotoGinowanPointApiBaseUri";
        private const string Config_JotoGinowanPointApiBaseUri_Defaultvalue = "https://localhost/";

        /// <summary>
        /// Joto ぎのわんPJ 基盤 API  X-Api-Id
        /// </summary>
        private const string Config_JotoGinowanXApiId_KeyName = "JotoGinowanXApiId";
        private const string Config_JotoGinowanXApiId_Defaultvalue = "XApiId";

        /// <summary>
        /// Joto ぎのわんPJ OCC 認証フロー名
        /// </summary>
        private const string Config_JotoGinowanOccAuthFlow_KeyName = "JotoGinowanOccAuthFlow";
        private const string Config_JotoGinowanOccAuthFlow_Defaultvalue = "OccAuthFlow";

        /// <summary>
        /// Joto ぎのわんPJ OCC コンテンツタイプ
        /// </summary>
        private const string Config_JotoGinowanOccContentType_KeyName = "JotoGinowanOccContentType";
        private const string Config_JotoGinowanOccContentType_Defaultvalue = "ContentType";

        /// <summary>
        /// Joto ぎのわんPJ OCC AMZ ターゲット名
        /// </summary>
        private const string Config_JotoGinowanOccAmzTarget_KeyName = "JotoGinowanOccAmzTarget";
        private const string Config_JotoGinowanOccAmzTarget_Defaultvalue = "AmzTarget";

        /// <summary>
        /// グループ Push通知 用 スレッド数を設定
        /// </summary>
        private const string Config_NotificationGroupThreads_KeyName = "NotificationGroupThreads";
        private const int Config_NotificationGroupThreads_DefaultValue = 5;

        /// <summary>
        /// 外部公開用のPushで使用するJOTOネイティブアプリへ飛ばすLinkageSystemNoのリスト
        /// ,区切りでつなぐ
        /// </summary>
        private const string Config_NoticeGroupToSystemJotoList_KeyName = "NoticeGroupToSystemJotoList";
        private const string Config_NoticeGroupToSystemJotoList_DefaultValue = "47900021,47900022";

        /// <summary>
        /// ReproPushEndDate
        /// </summary>
        private const string Config_ReproPushEndDate_KeyName = "ReproPushEndDate";
        private const string Config_ReproPushEndDate_DefaultValue = "2026/04/30";

        /// <summary>
        /// JOTOネイティブアプリPush開始日
        /// </summary>
        private const string Config_JotoNativePushStartDate_KeyName = "JotoNativePushStartDate";
        private const string Config_JotoNativePushStartDate_DefaultValue = "2026/03/27";

        #endregion

        #region "Public Property"
        /// <summary>
        /// SignUpのメール送信制限数を取得します。
        /// </summary>
        public static int SignUpMailAddressMaxCount
        {
            get
            {
                return GetConfiguration(Config_SignUpMailAddressMaxCount_KeyName, Config_SignUpMailAddressMaxCount_DefaultValue);
            }
        }

        /// <summary>
        /// アプリ用SignUpのUrlを取得します。
        /// </summary>
        public static string SignUpUrl
        {
            get
            {
                return GetConfiguration(Config_SignUpUrl_KeyName,Config_SignUpUrl_Defaultvalue);
            }
        }

        /// <summary>
        /// Tis アプリURLスキームを取得します。
        /// </summary>
        public static string TisAppUrlScheme
        {
            get
            {
                return GetConfiguration(Config_TisAppUrlScheme_KeyName, Config_TisAppUrlScheme_DefaultValue);
            }
        }

        /// <summary>
        /// Qolmsお薬手帳 アプリURLスキームを取得します。
        /// </summary>
        public static string QolmsMedicineAppUrlScheme
        {
            get
            {
                return GetConfiguration(Config_QolmsMedicineAppUrlScheme_KeyName, Config_QolmsMedicineAppUrlScheme_DefaultValue);
            }
        }

        /// <summary>
        /// Kagamino アプリURLスキームを取得します。
        /// </summary>
        public static string KagaminoAppUrlScheme
        {
            get
            {
                return GetConfiguration(Config_KagaminoAppUrlScheme_KeyName, Config_KagaminoAppUrlScheme_DefaultValue);
            }
        }

        /// <summary>
        /// 医療ナビ アプリURLスキームを取得します。
        /// </summary>
        public static string QolmsNaviAppUrlScheme
        {
            get
            {
                return GetConfiguration(Config_QolmsNaviAppUrlScheme_KeyName, Config_QolmsNaviAppUrlScheme_DefaultValue);
            }
        }

        /// <summary>
        /// MEIナビ アプリURLスキームを取得します。
        /// </summary>
        public static string MEINaviAppUrlScheme
        {
            get
            {
                return GetConfiguration(Config_MEINaviAppUrlScheme_KeyName, Config_MEINaviAppUrlScheme_DefaultValue);
            }
        }

        /// <summary>
        /// 健康DIARY アプリURLスキームを取得します。
        /// </summary>
        public static string HealthDiaryAppUrlScheme
        {
            get
            {
                return GetConfiguration(Config_HealthDiaryAppUrlScheme_KeyName, Config_HealthDiaryAppUrlScheme_DefaultValue);
            }
        }

        /// <summary>
        /// 心拍見守り アプリURLスキームを取得します。
        /// </summary>
        public static string HeartMonitorAppUrlScheme
        {
            get
            {
                return GetConfiguration(Config_HeartMonitorAppUrlScheme_KeyName, Config_HeartMonitorAppUrlScheme_DefaultValue);
            }
        }

        /// <summary>
        /// Joto アプリURLスキームを取得します。
        /// </summary>
        public static string JotoAppUrlScheme
        {
            get
            {
                return GetConfiguration(Config_JotoAppUrlScheme_KeyName, Config_JotoAppUrlScheme_DefaultValue);
            }
        }

        /// <summary>
        /// パスワードリセットUrlを取得します。
        /// </summary>
        public static string PasswordResetUrl
        {
            get
            {
                return GetConfiguration(Config_PasswordResetUrl_KeyName, Config_PasswordResetUrl_DefaultValue);
            }
        }

        /// <summary>
        /// メールアドレス変更Urlを取得します。
        /// </summary>
        public static string MailAddressChangeUrl
        {
            get
            {
                return GetConfiguration(Config_MailAddressChangeUrl_KeyName, Config_MailAddressChangeUrl_DefaultValue);
            }
        }

        /// <summary>
        /// Tis の QR 有効期限（時間）を取得します。
        /// </summary>
        public static QrExpirationConfig TisQrExpirationHour
        {
            get
            {
                var value = GetConfiguration(Config_Tis_Qr_Expiration_Hour_KeyName, Config_Tis_Qr_Expiration_Hour_DefaultValue);
                return new QrExpirationConfig(value);
            }
        }

        /// <summary>
        /// 医療ナビの QR 有効期限（時間）を取得します。
        /// </summary>
        public static QrExpirationConfig QolmsNaviQrExpirationHour
        {
            get
            {
                var value = GetConfiguration(Config_QolmsNavi_Qr_Expiration_Hour_KeyName, Config_QolmsNavi_Qr_Expiration_Hour_DefaultValue);
                return new QrExpirationConfig(value);
            }
        }

        /// <summary>
        /// MEIナビの QR 有効期限（時間）を取得します。
        /// </summary>
        public static QrExpirationConfig MEINaviQrExpirationHour
        {
            get
            {
                var value = GetConfiguration(Config_MEINavi_Qr_Expiration_Hour_KeyName, Config_MEINavi_Qr_Expiration_Hour_DefaultValue);
                return new QrExpirationConfig(value);
            }
        }

        /// <summary>
        /// Tis Azure Notification Hubs 接続文字列を取得します。
        /// </summary>
        public static string TisNotificationHubConnectionString
        {
            get
            {
                return GetConfiguration(Config_Tis_NotificationHubs_ConnectionString_KeyName, Config_Tis_NotificationHubs_ConnectionString_DefaultValue);
            }
        }

        /// <summary>
        /// Tis Azure Notification Hubs 名称を取得します。
        /// </summary>
        public static string TisNotificationHubName
        {
            get
            {
                return GetConfiguration(Config_Tis_NotificationHubs_Name_KeyName, Config_Tis_NotificationHubs_Name_DefaultValue);
            }
        }

        /// <summary>
        /// 医療ナビ Azure Notification Hubs 接続文字列を取得します。
        /// </summary>
        public static string QolmsNaviNotificationHubConnectionString
        {
            get
            {
                return GetConfiguration(Config_QolmsNavi_NotificationHubs_ConnectionString_KeyName, Config_QolmsNavi_NotificationHubs_ConnectionString_DefaultValue);
            }
        }

        /// <summary>
        /// 医療ナビ Azure Notification Hubs 名称を取得します。
        /// </summary>
        public static string QolmsNaviNotificationHubName
        {
            get
            {
                return GetConfiguration(Config_QolmsNavi_NotificationHubs_Name_KeyName, Config_QolmsNavi_NotificationHubs_Name_DefaultValue);
            }
        }

        /// <summary>
        /// MEIナビ Azure Notification Hubs 接続文字列を取得します。
        /// </summary>
        public static string MEINaviNotificationHubConnectionString
        {
            get
            {
                return GetConfiguration(Config_MEINavi_NotificationHubs_ConnectionString_KeyName, Config_MEINavi_NotificationHubs_ConnectionString_DefaultValue);
            }
        }

        /// <summary>
        /// MEIナビ Azure Notification Hubs 名称を取得します。
        /// </summary>
        public static string MEINaviNotificationHubName
        {
            get
            {
                return GetConfiguration(Config_MEINavi_NotificationHubs_Name_KeyName, Config_MEINavi_NotificationHubs_Name_DefaultValue);
            }
        }

        /// <summary>
        /// 健康DIARY Azure Notification Hubs 接続文字列を取得します。
        /// </summary>
        public static string HealthDiaryNotificationHubConnectionString
        {
            get
            {
                return GetConfiguration(Config_HealthDiary_NotificationHubs_ConnectionString_KeyName, Config_HealthDiary_NotificationHubs_ConnectionString_DefaultValue);
            }
        }

        /// <summary>
        /// 健康DIARY Azure Notification Hubs 名称を取得します。
        /// </summary>
        public static string HealthDiaryNotificationHubName
        {
            get
            {
                return GetConfiguration(Config_HealthDiary_NotificationHubs_Name_KeyName, Config_HealthDiary_NotificationHubs_Name_DefaultValue);
            }
        }

        /// <summary>
        /// QolmsJoto Azure Notification Hubs 接続文字列を取得します。
        /// </summary>
        public static string QolmsJotoNotificationHubConnectionString
        {
            get
            {
                return GetConfiguration(Config_QolmsJoto_NotificationHubs_ConnectionString_KeyName, Config_QolmsJoto_NotificationHubs_ConnectionString_DefaultValue);
            }
        }

        /// <summary>
        /// QolmsJoto Azure Notification Hubs 名称を取得します。
        /// </summary>
        public static string QolmsJotoNotificationHubName
        {
            get
            {
                return GetConfiguration(Config_QolmsJoto_NotificationHubs_Name_KeyName, Config_QolmsJoto_NotificationHubs_Name_DefaultValue);
            }
        }

        /// <summary>
        /// アカウント再登録可能間隔 を取得します。
        /// </summary>
        public static int NewUserRegisterInterval
        {
            get
            {
                return GetConfiguration(Config_NewUserRegister_Interval_KeyName, Config_NewUserRegister_Interval_DefaultValue);
            }
        }

        /// <summary>
        /// LineFollowEventUrl を取得します。
        /// </summary>
        public static string LineFollowEventUrl
        {
            get
            {
                return GetConfiguration(Config_LineFollowEvent_Url_KeyName, Config_LineFollowEvent_Url_DefaultValue);
            }
        }

        /// <summary>
        /// LineFirstEventUrl を取得します。
        /// </summary>
        public static string LineFirstEventUrl
        {
            get
            {
                return GetConfiguration(Config_LineFirstEvent_Url_KeyName, Config_LineFirstEvent_Url_DefaultValue);
            }
        }

        /// <summary>
        /// LineSecondEventUrl を取得します。
        /// </summary>
        public static string LineSecondEventUrl
        {
            get
            {
                return GetConfiguration(Config_LineSecondEvent_Url_KeyName, Config_LineSecondEvent_Url_DefaultValue);
            }
        }

        /// <summary>
        /// LineFifthEventUrl を取得します。
        /// </summary>
        public static string LineFifthEventUrl
        {
            get
            {
                return GetConfiguration(Config_LineFifthEvent_Url_KeyName, Config_LineFifthEvent_Url_DefaultValue);
            }
        }

        /// <summary>
        /// HAIPAPIKEY を取得します。
        /// </summary>
        public static string HaipApiKey
        {
            get
            {
                return GetConfiguration(Config_HaipApiKey_KeyName, Config_HaipApiKey_DefaultValue);
            }
        }

        /// <summary>
        /// Smapa Home取得API URL
        /// </summary>
        public static string SmapaApiHomeUrl => GetConfiguration(Config_SmapaApiHomeUrl_KeyName, Config_SmapaApiHomeUrl_DefaultValue);

        /// <summary>
        /// Smapa 退会API URL
        /// </summary>
        public static string SmapaApiRevokeUrl => GetConfiguration(Config_SmapaApiRevokeUrl_KeyName, Config_SmapaApiRevokeUrl_DefaultValue);

        /// <summary>
        /// Smapa APIキー
        /// </summary>
        public static string SmapaApiKey => GetConfiguration(Config_SmapaApiKey_KeyName, Config_SmapaApiKey_DefaultValue);

        /// <summary>
        /// ファイル投稿 Zip内のファイル数制限
        /// </summary>
        public static string FilePostingLimit => GetConfiguration(Config_FilePostingLimit_KeyName, Config_FilePostingLimit_DefaultValue);

        /// <summary>
        /// Joto ぎのわんPJ 基盤 API  ClientId を取得します。
        /// </summary>
        public static string JotoGinowanClientId => GetConfiguration(Config_JotoGinowanClientId_KeyName, Config_JotoGinowanClientId_Defaultvalue);

        /// <summary>
        /// Joto ぎのわんPJ 基盤 API  UserName を取得します。
        /// </summary>
        public static string JotoGinowanUserName => GetConfiguration(Config_JotoGinowanUserName_KeyName, Config_JotoGinowanUserName_Defaultvalue);

        /// <summary>
        /// Joto ぎのわんPJ 基盤 API  Password を取得します。
        /// </summary>
        public static string JotoGinowanPassword => GetConfiguration(Config_JotoGinowanPassword_KeyName, Config_JotoGinowanPassword_Defaultvalue);

        /// <summary>
        /// Joto ぎのわんPJ 基盤 API  Auth API Uri を取得します。
        /// </summary>
        public static string JotoGinowanAuthApiUri => GetConfiguration(Config_JotoGinowanAuthApiUri_KeyName, Config_JotoGinowanAuthApiUri_Defaultvalue);

        /// <summary>
        /// Joto ぎのわんPJ 基盤 API  Point API Base Uri を取得します。
        /// </summary>
        public static string JotoGinowanPointApiBaseUri => GetConfiguration(Config_JotoGinowanPointApiBaseUri_KeyName, Config_JotoGinowanPointApiBaseUri_Defaultvalue);

        /// <summary>
        /// Joto ぎのわんPJ 基盤 API  X-Api-Id を取得します。
        /// </summary>
        public static string JotoGinowanXApiId => GetConfiguration(Config_JotoGinowanXApiId_KeyName, Config_JotoGinowanXApiId_Defaultvalue);

        /// <summary>
        /// Joto ぎのわんPJ OCC 認証フロー名 を取得します。
        /// </summary>
        public static string JotoGinowanOccAuthFlow => GetConfiguration(Config_JotoGinowanOccAuthFlow_KeyName, Config_JotoGinowanOccAuthFlow_Defaultvalue);

        /// <summary>
        /// Joto ぎのわんPJ OCC コンテンツタイプ を取得します。
        /// </summary>
        public static string JotoGinowanOccContentType => GetConfiguration(Config_JotoGinowanOccContentType_KeyName, Config_JotoGinowanOccContentType_Defaultvalue);

        /// <summary>
        /// Joto ぎのわんPJ OCC AMZ ターゲット名 を取得します。
        /// </summary>
        public static string JotoGinowanOccAmzTarget => GetConfiguration(Config_JotoGinowanOccAmzTarget_KeyName, Config_JotoGinowanOccAmzTarget_Defaultvalue);

        /// <summary>
        /// グループ Push通知 用 スレッド数を設定 を取得します。
        /// </summary>
        public static int NotificationGroupThreads => GetConfiguration(Config_NotificationGroupThreads_KeyName, Config_NotificationGroupThreads_DefaultValue);

        /// <summary>
        /// 外部公開用のPushで使用するJOTOネイティブアプリへ飛ばすLinkageSystemNoのリスト
        /// </summary>
        public static string NoticeGroupToSystemJotoList => GetConfiguration(Config_NoticeGroupToSystemJotoList_KeyName, Config_NoticeGroupToSystemJotoList_DefaultValue);

        /// <summary>
        /// ReproPushEndDate
        /// </summary>
        public static string ReproPushEndDate => GetConfiguration(Config_ReproPushEndDate_KeyName, Config_ReproPushEndDate_DefaultValue);

        /// <summary>
        /// JOTOネイティブアプリPush開始日
        /// </summary>
        public static string JotoNativePushStartDate => GetConfiguration(Config_JotoNativePushStartDate_KeyName, Config_JotoNativePushStartDate_DefaultValue);

        #endregion

        #region "Private Method"
        /// <summary>
        /// 構成から値を取得します。ない場合やエラーになっても例外は返さず、defaultValueを返します。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private static string GetConfiguration(string key, string defaultValue = "")
        {
            string result;
            try
            {
                result = ConfigurationManager.AppSettings[key];
                if (string.IsNullOrWhiteSpace(result))
                    result = defaultValue;
            }
            catch (Exception)
            {
                result = defaultValue;
                //throw;
            }
            return result;
        }

        /// <summary>
        /// 構成から値を取得します。ない場合やエラーになっても例外は返さず、defaultValueを返します。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private static int GetConfiguration(string key, int defaultValue = 0)
        {
            int result;
            try
            {
                string tmp = ConfigurationManager.AppSettings[key];
                if (string.IsNullOrWhiteSpace(tmp) || !int.TryParse(tmp,out result) )
                    result = defaultValue;
            }
            catch (Exception)
            {
                result = defaultValue;
                //throw;
            }
            return result;
        }

        #endregion
    }
}