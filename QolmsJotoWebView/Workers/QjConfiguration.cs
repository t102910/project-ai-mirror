using MGF.QOLMS.QolmsCryptV1;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsJotoWebView
{
    internal class QjConfiguration
    {
        #region "Public Constant"
        /// <summary>
        /// 電話番号の正規表現を表します。
        /// </summary>
        public const string REGEX_TEL = @"^0\d{1,3}[-]?\d{1,4}[-]?\d{4}$";

        #endregion

        #region "Config"

        #region 会社情報

        /// <summary>
        /// 沖縄セルラー Joto のサイト名
        /// </summary>
        private const string Config_QolmsJotoSiteName_KeyName = "QolmsJotoSiteName";
        private const string Config_QolmsJotoSiteName_DefaultValue = "QolmsJoto";

        /// <summary>
        /// 沖縄セルラー Joto のサイト名を取得します。
        /// </summary>
        public static string QolmsJotoSiteName => GetConfiguration(Config_QolmsJotoSiteName_KeyName, Config_QolmsJotoSiteName_DefaultValue);

        /// <summary>
        /// 沖縄セルラー Joto のサイト URI
        /// </summary>
        private const string Config_QolmsJotoSiteUri_KeyName = "QolmsJotoSiteUri";
        private const string Config_QolmsJotoSiteUri_DefaultValue = "https://app.QolmsJoto.com/";

        /// <summary>
        /// 沖縄セルラー Joto のサイト URI を取得します。
        /// </summary>
        public static string QolmsJotoSiteUri => GetConfiguration(Config_QolmsJotoSiteUri_KeyName, Config_QolmsJotoSiteUri_DefaultValue);


        /// <summary>
        /// 沖縄セルラー Joto のお問合せページ URI
        /// </summary>
        private const string Config_QolmsJotoSiteContactUri_KeyName = "QolmsJotoSiteContactUri";
        private const string Config_QolmsJotoSiteContactUri_DefaultValue = "https://www.QolmsJoto.com/contact/";

        /// <summary>
        /// 沖縄セルラー Joto のお問合せページ URI を取得します。
        /// </summary>
        public static string QolmsJotoSiteContactUri => GetConfiguration(Config_QolmsJotoSiteContactUri_KeyName, Config_QolmsJotoSiteContactUri_DefaultValue);

        /// <summary>
        /// 沖縄セルラー Joto の運営組織名
        /// </summary>
        private const string Config_QolmsJotoSiteOwnerName_KeyName = "QolmsJotoSiteOwnerName";
        private const string Config_QolmsJotoSiteOwnerName_DefaultValue = "エムジーファクトリー株式会社";

        /// <summary>
        /// 沖縄セルラー Joto の運営組織名を取得します。
        /// </summary>
        public static string QolmsJotoSiteOwnerName => GetConfiguration(Config_QolmsJotoSiteOwnerName_KeyName, Config_QolmsJotoSiteOwnerName_DefaultValue);

        /// <summary>
        /// 沖縄セルラー Joto の運営組織 URI
        /// </summary>
        private const string Config_QolmsJotoSiteOwnerUri_KeyName = "QolmsJotoJotoSiteOwnerUri";
        private const string Config_QolmsJotoSiteOwnerUri_DefaultValue = "http://www.mgfactory.co.jp/";

        /// <summary>
        /// 沖縄セルラー Joto の運営組織 URI を取得します。
        /// </summary>
        public static string QolmsJotoSiteOwnerUri => GetConfiguration(Config_QolmsJotoSiteOwnerUri_KeyName, Config_QolmsJotoSiteOwnerUri_DefaultValue);

        #endregion

        #region カロミルWebView

        /// <summary>
        /// カロミル API URI
        /// </summary>
        private const string Config_CalomealApiUri_KeyName = "CalomealWebViewApiUri";
        private const string Config_CalomealApiUri_DefaultValue = "";

        /// <summary>
        /// カロミル API URI を取得します。
        /// </summary>
        public static string CalomealApiUri => GetConfiguration(Config_CalomealApiUri_KeyName, Config_CalomealApiUri_DefaultValue);

        /// <summary>
        /// CalomealWebViewのApiClientID
        /// </summary>
        private const string Config_CalomealApiClientID_KeyName = "CalomealApiClientID";
        private const string Config_CalomealApiClientID_DefaultValue = "";

        /// <summary>
        /// CalomealWebViewのApiClientID を取得します。
        /// </summary>
        public static string CalomealApiClientID => GetConfiguration(Config_CalomealApiClientID_KeyName, Config_CalomealApiClientID_DefaultValue);

        /// <summary>
        /// CalomealWebViewのApiSecret
        /// </summary>
        private const string Config_CalomealApiClientSecret_KeyName = "CalomealApiClientSecret";
        private const string Config_CalomealApiClientSecret_DefaultValue = "";

        /// <summary>
        /// CalomealWebViewのApiSecret を取得します。
        /// </summary>
        public static string CalomealApiClientSecret => GetConfiguration(Config_CalomealApiClientSecret_KeyName, Config_CalomealApiClientSecret_DefaultValue);

        /// <summary>
        /// CalomealWebViewのリダイレクトURL
        /// </summary>
        private const string Config_CalomealApiRedirectUrl_KeyName = "CalomealApiRedirectUrl";
        private const string Config_CalomealApiRedirectUrl_DefaultValue = "";

        /// <summary>
        /// CalomealWebViewのApiSecret を取得します。
        /// </summary>
        public static string CalomealApiRedirectUrl => GetConfiguration(Config_CalomealApiRedirectUrl_KeyName, Config_CalomealApiRedirectUrl_DefaultValue);

        /// <summary>
        /// 竹富町 JWT の設定
        /// </summary>
        private const string Config_TaketomiJwt_KeyName = "CalomealWebViewTaketomiJwt";
        private const string Config_TaketomiJwt_DefaultValue = "";

        /// <summary>
        /// 竹富町 JWT の設定 を取得します。
        /// </summary>
        public static string CalomealWebViewTaketomiJwt => GetConfiguration(Config_TaketomiJwt_KeyName, Config_TaketomiJwt_DefaultValue);

        /// <summary>
        /// 伊平屋村 JWT の設定
        /// </summary>
        private const string Config_IheyaJwt_KeyName = "CalomealWebViewIheyaJwt";
        private const string Config_IheyaJwt_DefaultValue = "";

        /// <summary>
        /// 伊平屋村 JWT の設定 を取得します。
        /// </summary>
        public static string CalomealWebViewIheyaJwt => GetConfiguration(Config_IheyaJwt_KeyName, Config_IheyaJwt_DefaultValue);

        /// <summary>
        /// 沖縄セルラー JWT の設定
        /// </summary>
        private const string Config_OctJwt_KeyName = "CalomealWebViewOctJwt";
        private const string Config_OctJwt_DefaultValue = "";

        /// <summary>
        /// 沖縄セルラー JWT の設定 を取得します。
        /// </summary>
        public static string CalomealWebViewOctJwt => GetConfiguration(Config_OctJwt_KeyName, Config_OctJwt_DefaultValue);


        #endregion

        #region 問診システム
        /// <summary>
        /// 問診システム BasicUserId
        /// </summary>
        private const string Config_BasicUserId_KeyName = "SunagawaBasicUserId";
        private const string Config_BasicUserId_DefaultValue = "";

        /// <summary>
        /// 問診システム BasicUserId を取得します。
        /// </summary>
        public static string BasicUserId => GetCryptConfiguration(Config_BasicUserId_KeyName, Config_BasicUserId_DefaultValue);

        /// <summary>
        /// 問診システム BasicPass
        /// </summary>
        private const string Config_BasicPass_KeyName = "SunagawaBasicPass";
        private const string Config_BasicPass_DefaultValue = "";

        /// <summary>
        /// 問診システム BasicPass を取得します。
        /// </summary>
        public static string BasicPass => GetCryptConfiguration(Config_BasicPass_KeyName, Config_BasicPass_DefaultValue);

        /// <summary>
        /// 問診システム BasicPass
        /// </summary>
        private const string Config_MonshinUrl_KeyName = "SunagawaMonshinUrl";
        private const string Config_MonshinUrl_DefaultValue = "";

        /// <summary>
        /// 問診システム BasicPass を取得します。
        /// </summary>
        public static string MonshinUrl => GetConfiguration(Config_MonshinUrl_KeyName, Config_MonshinUrl_DefaultValue);

        /// <summary>
        /// 問診システム AESKey
        /// </summary>
        private const string Config_AesKey_KeyName = "SunagawaAesKey";
        private const string Config_AesKey_DefaultValue = "";

        /// <summary>
        /// 問診システム AESKey を取得します。
        /// </summary>
        public static string AesKey => GetCryptConfiguration(Config_AesKey_KeyName, Config_AesKey_DefaultValue);

        /// <summary>
        /// 問診システム AESIv
        /// </summary>
        private const string Config_AesIv_KeyName = "SunagawaAesIv";
        private const string Config_AesIv_DefaultValue = "";

        /// <summary>
        /// 問診システム AESKIv を取得します。
        /// </summary>
        public static string AesIv => GetCryptConfiguration(Config_AesIv_KeyName, Config_AesIv_DefaultValue);

        #endregion

        #region ポイント

        /// <summary>
        /// ポイントサービス番号
        /// </summary>
        private const string Config_PointServiceno_KeyName = "PointServiceno";
        private const int Config_PointServiceno_DefaultValue = 47003;

        /// <summary>
        /// ポイントサービス番号を取得します。
        /// </summary>
        public static int PointServiceno => GetConfiguration(Config_PointServiceno_KeyName, Config_PointServiceno_DefaultValue);

        /// <summary>
        /// 交換エラーでメールを送る
        /// </summary>
        private const string Config_IsSendMailPointExchang_KeyName = "IsSendMailPointExchang";
        private const string Config_IsSendMailPointExchang_DefaultValue = "false";

        /// <summary>
        /// エラーでメールを送るかどうかを取得します。
        /// </summary>
        public static string IsSendMailPointExchang => GetConfiguration(Config_IsSendMailPointExchang_KeyName, Config_IsSendMailPointExchang_DefaultValue);

        #endregion

        #region Pontaポイント

        /// <summary>
        /// 加盟店ID
        /// </summary>
        private const string Config_AuWalletPointKameitenId_KeyName = "AuWalletPointKameitenId";
        private const string Config_AuWalletPointKameitenId_DefaultValue = "";

        /// <summary>
        /// 加盟店IDを取得します。
        /// </summary>
        public static string AuWalletPointKameitenId => GetConfiguration(Config_AuWalletPointKameitenId_KeyName, Config_AuWalletPointKameitenId_DefaultValue);

        /// <summary>
        /// サービスID
        /// </summary>
        private const string Config_AuPaymentServiceId_KeyName = "AuPaymentServiceId";
        private const string Config_AuPaymentServiceId_DefaultValue = "";

        /// <summary>
        /// サービスIDを取得します。
        /// </summary>
        public static string AuPaymentServiceId => GetConfiguration(Config_AuPaymentServiceId_KeyName, Config_AuPaymentServiceId_DefaultValue);

        /// <summary>
        /// セキュリティキー
        /// </summary>
        private const string Config_AuPaymentSecureKey_KeyName = "AuPaymentSecureKey";
        private const string Config_AuPaymentSecureKey_DefaultValue = "";

        /// <summary>
        /// セキュリティキーを取得します。
        /// </summary>
        public static string AuPaymentSecureKey => GetConfiguration(Config_AuPaymentSecureKey_KeyName, Config_AuPaymentSecureKey_DefaultValue);

        /// <summary>
        /// 要求URI
        /// </summary>
        private const string Config_AuWalletPointExchangeUri_KeyName = "AuWalletPointExchangeUri";
        private const string Config_AuWalletPointExchangeUri_DefaultValue = "";

        /// <summary>
        /// 要求URIを取得します。
        /// </summary>
        public static string AuWalletPointExchangeUri => GetConfiguration(Config_AuWalletPointExchangeUri_KeyName, Config_AuWalletPointExchangeUri_DefaultValue);

        /// <summary>
        /// APIキー
        /// </summary>
        private const string Config_AuWalletPointApiKey_KeyName = "AuWalletPointApiKey";
        private const string Config_AuWalletPointApiKey_DefaultValue = "";

        /// <summary>
        /// APIキーを取得します。
        /// </summary>
        public static string AuWalletPointApiKey => GetConfiguration(Config_AuWalletPointApiKey_KeyName, Config_AuWalletPointApiKey_DefaultValue);

        #endregion

        #region データチャージ

        /// <summary>
        /// 加盟店ID
        /// </summary>
        private const string Config_AuPaymentKameitenId_KeyName = "AuPaymentKameitenId";
        private const string Config_AuPaymentKameitenId_DefaultValue = "";

        /// <summary>
        /// 加盟店IDを取得します。
        /// </summary>
        public static string AuPaymentKameitenId => GetConfiguration(Config_AuPaymentKameitenId_KeyName, Config_AuPaymentKameitenId_DefaultValue);

        /// <summary>
        /// 要求URI
        /// </summary>
        private const string Config_AuDatachargeUri_KeyName = "AuDatachargeUri";
        private const string Config_AuDatachargeUri_DefaultValue = "";

        /// <summary>
        /// 要求URIを取得します。
        /// </summary>
        public static string AuDatachargeUri => GetConfiguration(Config_AuDatachargeUri_KeyName, Config_AuDatachargeUri_DefaultValue);

        /// <summary>
        /// テストサーバーの仮想日
        /// </summary>
        private const string Config_AuPaymentTestServerVirtualDate_KeyName = "AuPaymentTestServerVirtualDate";
        private const string Config_AuPaymentTestServerVirtualDate_DefaultValue = "";

        /// <summary>
        /// テストサーバーの仮想日を取得します。
        /// </summary>
        public static string AuPaymentTestServerVirtualDate => GetConfiguration(Config_AuPaymentTestServerVirtualDate_KeyName, Config_AuPaymentTestServerVirtualDate_DefaultValue);

        #endregion

        #region OWL

        /// <summary>
        /// OWL API URI
        /// </summary>
        private const string Config_AuOwlUri_KeyName = "AuOwlUri";
        private const string Config_AuOwlUri_DefaultValue = "";

        /// <summary>
        /// OWL APIのURIを取得します。
        /// </summary>
        public static string AuOwlUri => GetConfiguration(Config_AuOwlUri_KeyName, Config_AuOwlUri_DefaultValue);

        /// <summary>
        /// OWL APIアクセスキー
        /// </summary>
        private const string Config_AuOwlKey_KeyName = "AuOwlKey";
        private const string Config_AuOwlKey_DefaultValue = "";

        /// <summary>
        /// OWL APIのアクセスキーを取得します。
        /// </summary>
        public static string AuOwlKey => GetConfiguration(Config_AuOwlKey_KeyName, Config_AuOwlKey_DefaultValue);

        /// <summary>
        /// OWL APIサービスID
        /// </summary>
        private const string Config_AuOwlSid_KeyName = "AuOwlSid";
        private const string Config_AuOwlSid_DefaultValue = "";

        /// <summary>
        /// OWL APIのサービスIDを取得します。
        /// </summary>
        public static string AuOwlSid => GetConfiguration(Config_AuOwlSid_KeyName, Config_AuOwlSid_DefaultValue);

        /// <summary>
        /// OWL API機能ID
        /// </summary>
        private const string Config_AuOwlFid_KeyName = "AuOwlFid";
        private const string Config_AuOwlFid_DefaultValue = "";

        /// <summary>
        /// OWL APIの機能IDを取得します。
        /// </summary>
        public static string AuOwlFid => GetConfiguration(Config_AuOwlFid_KeyName, Config_AuOwlFid_DefaultValue);

        #endregion

        #region ぎのわん


        /// <summary>
        /// ポイントサービス番号
        /// </summary>
        private const string Config_GinowanApplyUrl_KeyName = "GinowanApplyUrl";
        private const string Config_GinowanApplyUrl_DefaultValue = "";

        /// <summary>
        /// ポイントサービス番号を取得します。
        /// </summary>
        public static string GinowanApplyUrl => GetConfiguration(Config_GinowanApplyUrl_KeyName, Config_GinowanApplyUrl_DefaultValue);

        /// <summary>
        /// 病院連携の有効病院リスト
        /// </summary>
        private const string Config_HospitalEnableLinkageList_KeyName = "HospitalEnableLinkageList";
        private const string Config_HospitalEnableLinkageList_DefaultValue = "";

        /// <summary>
        /// 病院連携の有効病院リストを取得します。
        /// </summary>
        public static string HospitalEnableLinkageList => GetConfiguration(Config_HospitalEnableLinkageList_KeyName, Config_HospitalEnableLinkageList_DefaultValue);

        #endregion

        #region "タニタ"

        /// <summary>
        /// ジョブURL
        /// </summary>
        private const string Config_TanitaConnectionFirstJob_KeyName = "TanitaConnectionFirstJob";
        private const string Config_TanitaConnectionFirstJob_DefaultValue = "";

        /// <summary>
        /// ジョブURLを取得します。
        /// </summary>
        public static string TanitaConnectionFirstJob => GetConfiguration(Config_TanitaConnectionFirstJob_KeyName, Config_TanitaConnectionFirstJob_DefaultValue);

        /// <summary>
        /// ジョブアカウント
        /// </summary>
        private const string Config_TanitaConnectionFirstJobAccount_KeyName = "TanitaConnectionFirstJobAccount";
        private const string Config_TanitaConnectionFirstJobAccount_DefaultValue = "";

        /// <summary>
        /// ジョブアカウントを取得します。
        /// </summary>
        public static string TanitaConnectionFirstJobAccount => GetConfiguration(Config_TanitaConnectionFirstJobAccount_KeyName, Config_TanitaConnectionFirstJobAccount_DefaultValue);

        /// <summary>
        /// ジョブパスワード
        /// </summary>
        private const string Config_TanitaConnectionFirstJobPassword_KeyName = "TanitaConnectionFirstJobPassword";
        private const string Config_TanitaConnectionFirstJobPassword_DefaultValue = "";

        /// <summary>
        /// ジョブパスワードを取得します。
        /// </summary>
        public static string TanitaConnectionFirstJobPassword => GetConfiguration(Config_TanitaConnectionFirstJobPassword_KeyName, Config_TanitaConnectionFirstJobPassword_DefaultValue);

        #endregion

        #region auID


        /// <summary>
        /// auID認証クライアントID
        /// </summary>
        private const string Config_AuClientId_KeyName = "AuClientId";
        private const string Config_AuClientId_DefaultValue = "";

        /// <summary>
        /// auID認証クライアントIDを取得します。
        /// </summary>
        public static string AuClientId => GetConfiguration(Config_AuClientId_KeyName, Config_AuClientId_DefaultValue);

        /// <summary>
        /// auID認証クライアントシークレット
        /// </summary>
        private const string Config_AuClientSecret_KeyName = "AuClientSecret";
        private const string Config_AuClientSecret_DefaultValue = "";

        /// <summary>
        /// auID認証クライアントシークレットを取得します。
        /// </summary>
        public static string AuClientSecret => GetConfiguration(Config_AuClientSecret_KeyName, Config_AuClientSecret_DefaultValue);

        /// <summary>
        /// JwtValidateLifetimeFlag
        /// </summary>
        private const string Config_JwtValidateLifetimeFlag_KeyName = "JwtValidateLifetimeFlag";
        private const string Config_JwtValidateLifetimeFlag_DefaultValue = "";

        /// <summary>
        /// JwtValidateLifetimeFlagを取得します。
        /// </summary>
        public static string JwtValidateLifetimeFlag => GetConfiguration(Config_JwtValidateLifetimeFlag_KeyName, Config_JwtValidateLifetimeFlag_DefaultValue);

        /// <summary>
        /// auID認証サイトURL
        /// </summary>
        private const string Config_AuDiscoveryUri_KeyName = "AuDiscoveryUri";
        private const string Config_AuDiscoveryUri_DefaultValue = "";

        /// <summary>
        /// auID認証サイトURLを取得します。
        /// </summary>
        public static string AuDiscoveryUri => GetConfiguration(Config_AuDiscoveryUri_KeyName, Config_AuDiscoveryUri_DefaultValue);


        /// <summary>
        /// JotoWebViewURL
        /// </summary>
        private const string Config_JotoWebViewUri_KeyName = "JotoWebViewUri";
        private const string Config_JotoWebViewUri_DefaultValue = "";

        /// <summary>
        /// JotoWebViewURLを取得します。
        /// </summary>
        public static string JotoWebViewUri => GetConfiguration(Config_JotoWebViewUri_KeyName, Config_JotoWebViewUri_DefaultValue);

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
                if (string.IsNullOrWhiteSpace(tmp) || !int.TryParse(tmp, out result))
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
        /// 構成から暗号化された値を複合し取得します。ない場合やエラーになっても例外は返さず、defaultValueを返します。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private static string GetCryptConfiguration(string key, string defaultValue = "")
        {
            string result;
            try
            {
                using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
                {
                    result = crypt.DecryptString(ConfigurationManager.AppSettings[key]);
                }
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

        #endregion

        #endregion

    }
}