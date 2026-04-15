using MGF.QOLMS.QolmsApiCoreV1;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;

namespace MGF.QOLMS.QolmsJotoWebView
{
    public sealed class LoginModel
    {
        #region "public Property"

        /// <summary>
        /// QOLMSの サイト名を取得します。
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string QolmsSiteName { get => QjConfiguration.QolmsJotoSiteName; }

        public string UserId = string.Empty;

        public string Password = string.Empty;

        public string PasswordHash = string.Empty;

        public bool RememberId = false;

        public bool RememberLogin = false;

        public QsApiLoginResultTypeEnum LoginResultType = QsApiLoginResultTypeEnum.None;

        public string Message = string.Empty;

        public string OpenId = string.Empty;

        public byte OpenIdType = byte.MinValue;

        public OpenIdConnectConfiguration OpenIdConfig = new OpenIdConnectConfiguration();

        public string CodeVerifier = string.Empty;

        public string nonce = string.Empty;

        public string state = string.Empty;

        #endregion

        #region "Constructor"

        public LoginModel() : base() 
        {
            this.CodeVerifier = $"{Guid.NewGuid().ToString("N")}{Guid.NewGuid().ToString("N")}";
            this.nonce = Guid.NewGuid().ToString("N");
            this.state = Guid.NewGuid().ToString("N");
            var configurationManage = new ConfigurationManager<OpenIdConnectConfiguration>(QjConfiguration.AuDiscoveryUri, new OpenIdConnectConfigurationRetriever());
            this.OpenIdConfig = configurationManage.GetConfigurationAsync(CancellationToken.None).Result;
        }

        #endregion

        #region PrivateMethod

        /// <summary>
        /// CodeVerifierからCodeCharengeを生成して返す
        /// </summary>
        private string GetCodeChallenge()
        {
            var origByte = Encoding.ASCII.GetBytes($"{this.CodeVerifier}");
            using (var sha256 = new SHA256CryptoServiceProvider())
            {
                var hashValue = sha256.ComputeHash(origByte);
                return Convert.ToBase64String(hashValue, Base64FormattingOptions.None).Replace("=", "").Replace("+", "-").Replace("/", "_");
            }
        }

        #endregion

        #region "Public Method"

        public string GetAuthorizationRequestUrl()
        {
            var endpoint = OpenIdConfig.AuthorizationEndpoint;
            var redirecturi = HttpUtility.UrlEncode($"{QjConfiguration.JotoWebViewUri}start/logineditauidresult");
            var codechallenge = GetCodeChallenge();

            return $"{endpoint}&response_type=code&client_id={QjConfiguration.AuClientId}&redirect_uri={redirecturi}&scope=openid&state={this.state}&code_challenge={codechallenge}&code_challenge_method=S256&nonce={this.nonce}";
                             
        }

        public string GetAccessTokenRequestUrl()
        {
            return OpenIdConfig.TokenEndpoint;
        }


        #endregion
    }

    public class AccessTokenRequestResults
    {
        public string access_token;

        public string token_type;

        public string expires_in;

        public string refresh_token;

        public string id_token;
    }


}