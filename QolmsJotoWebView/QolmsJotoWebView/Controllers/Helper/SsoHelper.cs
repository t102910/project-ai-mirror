using MGF.QOLMS.QolmsJwtAuthCore;
using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    internal sealed class SsoHelper
    {
        #region "Constructor"

        /// <summary>
        /// <see cref="SsoHelper" /> クラス の新しい インスタンス を 1 度だけ初期化します。
        /// </summary>
        static SsoHelper() { }

        #endregion

        #region "Public Method"

        public static string GetJwt(string authorizationHeader)
        {
            const string httpBearerSchemeName = "Bearer";

            string result = string.Empty;

            if (!string.IsNullOrWhiteSpace(authorizationHeader))
            {
                string verifiedAuthorizationHeader = authorizationHeader.Trim();

                if (verifiedAuthorizationHeader.IndexOf(httpBearerSchemeName, StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    verifiedAuthorizationHeader = verifiedAuthorizationHeader.Substring(httpBearerSchemeName.Length, verifiedAuthorizationHeader.Length - httpBearerSchemeName.Length).Trim();

                    result = verifiedAuthorizationHeader;
                }
            }

            // TODO: デバッグ
            AccessLogWorker.WriteAccessLog(null, "SSO JWT", AccessLogWorker.AccessTypeEnum.None, result);

            return result;
        }

        public static QjJwtTokenValidationResults ValidateJwt(string tokenString)
        {
            return new QjJwtTokenValidator().Validate(tokenString);
        }

        #endregion
    }
}