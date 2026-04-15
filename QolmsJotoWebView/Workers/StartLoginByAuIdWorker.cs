using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace MGF.QOLMS.QolmsJotoWebView
{
    public class StartLoginByAuIdWorker
    {

        #region Constant


        #endregion

        #region PrivateMethod

        private static QiQolmsYappliStartLoginEditReadApiResults ExecuteStartLoginEditReadApi(QolmsJotoModel mainModel)
        {
            var args = new QiQolmsYappliStartLoginEditReadApiArgs(
                QiApiTypeEnum.QolmsYappliStartLoginEditRead,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName)
            {
                Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString()
            };

            return QsApiManager.ExecuteQolmsIdentityApi<QiQolmsYappliStartLoginEditReadApiResults>(args, mainModel.SessionId, mainModel.ApiAuthorizeKey);
        }

        private static QiQolmsYappliStartLoginEditWriteApiResults ExecuteStartLoginEditWriteApi(QolmsJotoModel mainModel, string openId, string openIdType)
        {
            var args = new QiQolmsYappliStartLoginEditWriteApiArgs(
                QiApiTypeEnum.QolmsYappliStartLoginEditWrite,
                QsApiSystemTypeEnum.Qolms,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName)
            {
                Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                OpenId = openId,
                OpenIdType = openIdType,
                UserId = string.IsNullOrWhiteSpace(openId) ? openId : $"JOTO{openId.Trim()}"
            };

            return QsApiManager.ExecuteQolmsIdentityApi<QiQolmsYappliStartLoginEditWriteApiResults>(args, mainModel.SessionId, mainModel.ApiAuthorizeKey);
        }

        #endregion

        public static LoginModel CreateViewModel(QolmsJotoModel mainModel)
        {
            var result = StartLoginByAuIdWorker.ExecuteStartLoginEditReadApi(mainModel);

            var model = new LoginModel()
            {
                OpenIdType = result.OpenIdType.TryToValueType(Byte.MinValue),
                OpenId = result.OpenId
            };
            return model;
        }

        public static string Edit(QolmsJotoModel mainModel, string openId)
        {
            var result = StartLoginByAuIdWorker.ExecuteStartLoginEditWriteApi(mainModel, openId, $"1");
            var ret = string.Empty;
            foreach (var error in result.ErrorList)
            {
                switch (error)
                {
                    case "OpenId":
                        ret = $"{ret}このauIDは既に登録されています。";
                        break;
                    case "UserId":
                        ret = $"{ret}ユーザーIDが登録できません。";
                        break;
                    default:
                        break;
                }
            }
            return ret;
        }

        public static string GetOpenId(LoginModel loginModel, string code)
        {
            var ret = string.Empty;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var redirecturi = HttpUtility.UrlEncode($"{QjConfiguration.JotoWebViewUri}start/logineditauidresult");
            var apiResult = new AccessTokenRequestResults();
            using (var wb = new WebClient())
            {
                wb.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                var data = new NameValueCollection();
                data["grant_type"] = "authorization_code";
                data["code"] = code;
                data["redirect_uri"] = redirecturi;
                data["client_id"] = QjConfiguration.AuClientId;
                data["client_secret"] = QjConfiguration.AuClientSecret;
                data["code_verifier"] = loginModel.CodeVerifier;

                var response = wb.UploadValues(loginModel.GetAccessTokenRequestUrl(), "POST", data);
                using (var stream = new MemoryStream(response))
                {
                    var serializer = new DataContractJsonSerializer(typeof(AccessTokenRequestResults));
                    apiResult = (AccessTokenRequestResults)serializer.ReadObject(stream);
                }
            }

            SecurityKey seckey = null;
            var handler = new JwtSecurityTokenHandler();
            var token = (JwtSecurityToken)Convert.ChangeType(handler.ReadToken(apiResult.id_token), typeof(JwtSecurityToken));
            foreach (var keyItem in loginModel.OpenIdConfig.JsonWebKeySet.Keys)
            {
                if (keyItem.Kid == token.Header.Kid)
                {
                    var rsa = RSA.Create();
                    var padded = keyItem.N.Length % 4 == 0 ? keyItem.N : $"{keyItem.N}====".Substring(keyItem.N.Length % 4);
                    var base64 = padded.Replace("_", "/").Replace("-", "+");
                    var modulusBytes = Convert.FromBase64String(base64);

                    var tmp = new byte[modulusBytes.Length - 2];
                    if (modulusBytes[0] == 0)
                    {   
                        Array.Copy(modulusBytes, 1, tmp, 0, tmp.Length);
                    }
                    rsa.ImportParameters(new RSAParameters()
                    {
                        Exponent = modulusBytes,
                        Modulus = tmp
                    });
                    seckey = new RsaSecurityKey(rsa);
                }
            }

            var tokenValidationParam = new TokenValidationParameters()
            {
                IssuerSigningKey = seckey,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidIssuer = loginModel.OpenIdConfig.Issuer,
                ValidateAudience = true,
                ValidAudience = QjConfiguration.AuClientId,
                ValidateLifetime = QjConfiguration.JwtValidateLifetimeFlag.TryToValueType(true)
            };

            SecurityToken securityToken = null;
            handler.InboundClaimTypeMap.Clear();
            var cmp = handler.ValidateToken(apiResult.id_token, tokenValidationParam, out securityToken);

            if (token.Payload.First(m => m.Key == "nonce").Value.ToString() != loginModel.nonce)
            {
                return ret;
            }
            return cmp.Claims.First(c => c.Type == "sub").Value;

        }

    }
}












