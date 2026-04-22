using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsJwtAuthCore;
using MGF.QOLMS.QolmsOpenApi.Controllers;
using MGF.QOLMS.QolmsOpenApi.Enums;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http.Filters;
using System;
using MGF.QOLMS.QolmsOpenApi.Worker;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsCryptV1;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Attribute
{
    /// <summary>
    /// 認証属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class QoApiAuthorizeAttribute : AuthorizationFilterAttribute
    {


        /// <summary>
        /// Jwt認証の認証スキームを表します。
        /// </summary>
        /// <remarks></remarks>
        private const string HTTP_BEARER_SCHEME_NAME = "Bearer";

        /// <summary>
        /// Basic認証の認証スキームを表します。
        /// </summary>
        /// <remarks></remarks>
        private const string HTTP_BASIC_SCHEME_NAME = "Basic";

        /// <summary>
        /// 認証種別
        /// </summary>
        private QoApiAuthorizeTypeEnum _authType = QoApiAuthorizeTypeEnum.None;
        /// <summary>
        /// 機能権限
        /// </summary>
        private QoApiFunctionTypeEnum _funcType = QoApiFunctionTypeEnum.None;

        /// <summary>
        /// 認証ヘッダなしを許容するか
        /// </summary>
        private bool _isAllowNoToken = false;

        /// <summary>
        ///  デフォルトコンストラクタは使用できません。
        ///  </summary>
        ///  <remarks></remarks>
        private QoApiAuthorizeAttribute()
        {
        }

        /// <summary>
        /// 値を指定して、
        /// <see cref="QoApiAuthorizeAttribute" />クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="authType">認証種別。</param>
        /// <param name="funcType"></param>
        /// <param name="isAllowNoToken">
        /// トークン無しを許容するかどうか。認証あり、なしを併用するメソッドで使用してください。
        /// 許容する場合はこの検証属性ではエラー（HTTP401）になりませんが、独自の検証を用意してください。
        /// </param>
        /// <remarks></remarks>
        public QoApiAuthorizeAttribute(QoApiAuthorizeTypeEnum authType, QoApiFunctionTypeEnum funcType = QoApiFunctionTypeEnum.All, bool isAllowNoToken = false) : base()
        {

            // 認証種別のチェック
            if (authType == QoApiAuthorizeTypeEnum.None)
                throw new ArgumentException("authType", "QoApiAuthorizeTypeEnum.Noneは指定できません。");
            else if (authType.HasFlag(QoApiAuthorizeTypeEnum.JwtToken) && authType.HasFlag(QoApiAuthorizeTypeEnum.JwtAccessKey))
                throw new ArgumentException("authType", "QoApiAuthorizeTypeEnum.JwtToken と QoApiAuthorizeTypeEnum.JwtAccessKey は同時に指定できません。");

            this._authType = authType;
            this._funcType = funcType;
            this._isAllowNoToken = isAllowNoToken;
        }


         private QoApiAuthorizeTypeEnum ExtractAuthorizationType(string authorizationHeader, QoApiAuthorizeTypeEnum authType)
        {
            QoApiAuthorizeTypeEnum result = QoApiAuthorizeTypeEnum.Invalid;

            // 認証ヘッダーのチェック
            if (authType == QoApiAuthorizeTypeEnum.None)
                // 認証なし
                return QoApiAuthorizeTypeEnum.None;
            else if ((authorizationHeader == null) || (string.IsNullOrWhiteSpace(authorizationHeader)))
                return QoApiAuthorizeTypeEnum.Invalid;
            else
            {
                // 認証スキームをチェック
                string verifiedAuthorizationHeader = authorizationHeader.Trim();

                if (verifiedAuthorizationHeader.IndexOf(QoApiAuthorizeAttribute.HTTP_BASIC_SCHEME_NAME) >= 0)
                {
                    if (authType.HasFlag(QoApiAuthorizeTypeEnum.Basic))
                        result = QoApiAuthorizeTypeEnum.Basic;
                    else
                        result = QoApiAuthorizeTypeEnum.Invalid;
                }
                else if (verifiedAuthorizationHeader.IndexOf(QoApiAuthorizeAttribute.HTTP_BEARER_SCHEME_NAME) >= 0)
                {
                    if (authType.HasFlag(QoApiAuthorizeTypeEnum.JwtJotoApiKey))
                        result = QoApiAuthorizeTypeEnum.JwtJotoApiKey;
                    else if (authType.HasFlag(QoApiAuthorizeTypeEnum.JwtToken) || authType.HasFlag(QoApiAuthorizeTypeEnum.JwtAccessKey))
                        result = QoApiAuthorizeTypeEnum.JwtToken | QoApiAuthorizeTypeEnum.JwtAccessKey;
                    else if (authType.HasFlag(QoApiAuthorizeTypeEnum.JwtQolmsApiKey))
                        result = QoApiAuthorizeTypeEnum.JwtQolmsApiKey;
                    else
                        result = QoApiAuthorizeTypeEnum.Invalid;
                }
            }

            return result;
        }

        /// <summary>
        ///   認証ヘッダーからJson Webトークンを検証します。
        ///  </summary>
        ///  <param name="authorizationHeader">認証ヘッダー。</param>
        ///  <param name="authType">認証種別</param>
        ///  <param name="accountKey">アカウントキー</param>
        ///  <param name="funcType">アクセス許可</param>
        ///  <param name="parentKey">親アカウントキー</param>
        ///  <param name="executor">実行者の暗号化したままの文字列</param>
        ///  <returns>成功なら True、失敗なら False。</returns>
        ///  <remarks></remarks>
        private bool ValidateJwtCredentials(string authorizationHeader, QoApiAuthorizeTypeEnum authType, 
            out Guid accountKey, out QoApiFunctionTypeEnum funcType, out Guid parentKey, out string executor)
        {
            bool result = false;
            accountKey = Guid.Empty;
            funcType = QoApiFunctionTypeEnum.None;
            parentKey = Guid.Empty;
            executor = string.Empty;
            // 認証スキーム
            const string HttpBearerSchemeName = "Bearer";

            // 認証ヘッダーのチェック
            if ((authorizationHeader == null) || (string.IsNullOrWhiteSpace(authorizationHeader)))
                return false;

            // 認証スキームをチェック
            string verifiedAuthorizationHeader = authorizationHeader.Trim();

            if (verifiedAuthorizationHeader.IndexOf(HttpBearerSchemeName) != 0)
                return false;

            // 先頭の認証スキームを取り除く
            verifiedAuthorizationHeader = verifiedAuthorizationHeader.Substring(HttpBearerSchemeName.Length, verifiedAuthorizationHeader.Length - HttpBearerSchemeName.Length).Trim();

            try
            {
                // 独自要素のチェック

                if (authType.HasFlag(QoApiAuthorizeTypeEnum.JwtToken))
                {
                    QoJwtAuthenticateValidationResults authResult = new QoJwtTokenValidator().Validate<QoJwtAuthenticateValidationResults>(verifiedAuthorizationHeader);
                    if (authResult.IsSuccess && authResult.AccountKey != Guid.Empty)
                    {
                        accountKey = authResult.AccountKey;
                        funcType = QoApiFunctionTypeEnum.AccessKey | QoApiFunctionTypeEnum.Master;
                        parentKey = Guid.Empty;
                        executor = authResult.Executor;
                        return true;
                    }
                }
                if (authType.HasFlag(QoApiAuthorizeTypeEnum.JwtAccessKey))
                {
                    QoJwtAccessKeyValidationResults accKeyResult = new QoJwtTokenValidator().Validate<QoJwtAccessKeyValidationResults>(verifiedAuthorizationHeader);
                    if (accKeyResult.IsSuccess && accKeyResult.AccountKey != Guid.Empty)
                    {
                        accountKey = accKeyResult.AccountKey;
                        funcType = (QoApiFunctionTypeEnum)accKeyResult.FuncionTypeNo;
                        parentKey = accKeyResult.ParentKey;
                        executor = accKeyResult.Executor;
                        return  true;
                    }
                    else if(accKeyResult.IsSuccess && accKeyResult.AccountKey == Guid.Empty && (QoApiFunctionTypeEnum)accKeyResult.FuncionTypeNo==QoApiFunctionTypeEnum.GuestUser )
                    {
                        //Geuest
                        accountKey = Guid.Empty ;
                        funcType = (QoApiFunctionTypeEnum)accKeyResult.FuncionTypeNo;
                        parentKey = Guid.Empty;
                        executor = accKeyResult.Executor;
                        return true;
                    }
                }
                if (authType.HasFlag(QoApiAuthorizeTypeEnum.JwtQolmsApiKey) || authType.HasFlag(QoApiAuthorizeTypeEnum.JwtJotoApiKey))
                {
                    QoJwtApiKeyValidationResults apiKeyResult = new QoJwtTokenValidator().Validate<QoJwtApiKeyValidationResults>(verifiedAuthorizationHeader);
                    if (apiKeyResult.IsSuccess)
                    {
                        accountKey = Guid.Empty;   //ApiKeyではAccountKeyは入らない。（実行者がシステムアカウントでこのキーでの対象者が特定されないため）
                        funcType = (QoApiFunctionTypeEnum)apiKeyResult.FuncionTypeNo;
                        executor = apiKeyResult.Executor;
                        return true;
                    }
                }
                
            }
            catch (Exception ex)
            {
                result = false;
                Console.WriteLine(string.Format("トークンが無効です。{0}", ex.Message));
            }
            
            if (result == false)
                QoAccessLog.WriteAccessLog( QsApiSystemTypeEnum.QolmsOpenApi, Guid.Empty, DateTime.Now, QoAccessLog.AccessTypeEnum.Api,string.Empty ,
                    string.Format("{0}[header]:{1}/[Result]:{2}", authType.ToString(), verifiedAuthorizationHeader, result),string.Empty,string.Empty,string.Empty  );
            return result;
        }

        /// <summary>
        ///  認証ヘッダーから暗号化されたユーザー名とパスワードを取り出します。
        ///  </summary>
        ///  <param name="authorizationHeader">認証ヘッダー。</param>
        ///  <param name="refUserName">暗号化されたユーザー名が格納される変数。</param>
        ///  <param name="refPassword">暗号化されたパスワードが格納される変数。</param>
        ///  <returns>
        ///  成功ならTrue、
        ///  失敗ならFalse。
        ///  </returns>
        ///  <remarks></remarks>
        private bool ExtractBasicCredentials(string authorizationHeader, ref string refUserName, ref string refPassword)
        {

            // 認証スキーム
            const string HttpBasicSchemeName = "Basic";

            // 認証ヘッダーのチェック
            if ((authorizationHeader == null) || (string.IsNullOrWhiteSpace(authorizationHeader)))
                return false;

            // 認証スキームをチェック
            string verifiedAuthorizationHeader = authorizationHeader.Trim();

            if (verifiedAuthorizationHeader.IndexOf(HttpBasicSchemeName) != 0)
                return false;

            // 先頭の認証スキームを取り除く
            verifiedAuthorizationHeader = verifiedAuthorizationHeader.Substring(HttpBasicSchemeName.Length, verifiedAuthorizationHeader.Length - HttpBasicSchemeName.Length).Trim();

            // Base64文字列からデコード
            string decodedAuthorizationHeader = Encoding.UTF8.GetString(Convert.FromBase64String(verifiedAuthorizationHeader));

            // ユーザー名とパスワードへ分離
            List<string> values = decodedAuthorizationHeader.Split(new[] { ':' }, StringSplitOptions.None).ToList();

            if (values.Count != 2)
                return false;

            refUserName = values[0].Trim();
            refPassword = values[1].Trim();

            //QoAccessLog.WriteInfoLog(string.Format("userName:{0},userPass:{1}", refUserName, refPassword));
            if (string.IsNullOrWhiteSpace(refUserName) || string.IsNullOrWhiteSpace(refPassword))
                return false;

            return true;
        }

        /// <summary>
        /// ユーザー名とパスワードを検証します。
        /// </summary>
        /// <param name="userName">暗号化されたユーザー名。（</param>
        /// <param name="password">暗号化されたパスワード。</param>
        /// <returns> 成功ならTrue、失敗ならFalse。</returns>
        /// <remarks></remarks>
        private bool ValidateBasicCredentials(string userName, string password)
        {
            bool result = false;
            QsApiSystemTypeEnum executeSystemType = QsApiSystemTypeEnum.None;
            Guid executor = Guid.Empty;
            string sessionId = string.Empty;
            Guid authorizeKey = Guid.Empty;

            try
            {
                List<string> values = null;

                using (QsCrypt crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
                {
                    // 暗号化されたユーザー名から実行システムの種別、実行者 ID、セッション ID を取り出す
                    values = crypt.DecryptString(userName).Split(new char[]{':'}, StringSplitOptions.None).ToList();

                    // 暗号化されたパスワードから認証キーを取り出す
                    values.Add(crypt.DecryptString(password));
                }
                //QoAccessLog.WriteInfoLog(string.Format("valuesCount:{0}",values.Count));

                if (values.Count == 4)
                {
                    // 実行システムの種別
                    executeSystemType = values[0].TryToValueType(QsApiSystemTypeEnum.None);

                    // 実行者
                    executor = values[1].TryToValueType(Guid.Empty);

                    // セッション ID
                    sessionId = values[2];

                    // 認証キー
                    authorizeKey = values[3].TryToValueType(Guid.Empty);
                    //QoAccessLog.WriteInfoLog(string.Format("executeSystemType:{0},executor:{1},sessionId:{2},authorizeKey:{3}", executeSystemType, executor, sessionId, authorizeKey));

                    if (executeSystemType != QsApiSystemTypeEnum.None && executor != Guid.Empty && !string.IsNullOrWhiteSpace(sessionId) && authorizeKey != Guid.Empty)

                        // 認証キーの有効性をチェック
                        // // TODO ApiAuthorizeは使用しないので代わりとなるチェック方法を検討
                        result = true;// QsApiManager.CheckAuthorizeKey(QsApiSystemTypeEnum.QolmsApi, executeSystemType, executor, sessionId, authorizeKey)
                }
            }
            catch
            {
            }

            return result;
        }



        /// <summary>
        /// プロセスが承認を要求したときに呼び出します。
        /// </summary>
        /// <param name="actionContext">アクション コンテキスト。</param>
        /// <remarks></remarks>
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            string authorizationHeader = string.Empty; // HttpContext.Current.Request.Headers("Authorization")

            // 認証方法に応じて対象のヘッダフィールド取得
            switch (this._authType)
            {
                case QoApiAuthorizeTypeEnum.JwtJotoApiKey:
                    authorizationHeader = HttpContext.Current.Request.Headers["X-JOTO-API-KEY"];
                    break;
                default:
                    authorizationHeader = HttpContext.Current.Request.Headers["Authorization"];
                    break;
            }

            //QoAccessLog.WriteInfoLog(string.Format("authType:{0}", this._authType));
            // 認証スキームをチェック
            QoApiAuthorizeTypeEnum authType = this.ExtractAuthorizationType(authorizationHeader, this._authType);
            QoApiFunctionTypeEnum funcType = QoApiFunctionTypeEnum.All;
            string executor = string.Empty;

            //QoAccessLog.WriteInfoLog(string.Format("authType:{0},authorizationHeader:{1}", authType, authorizationHeader));
            switch (authType)
            {
                case QoApiAuthorizeTypeEnum.None:
                    // 認証なし
                    return;
                case QoApiAuthorizeTypeEnum.Basic:
                    // Basic認証
                    string userName = string.Empty;
                    string password = string.Empty;
                    // 基本認証を実行
                    if (this.ExtractBasicCredentials(authorizationHeader, ref userName, ref password) && this.ValidateBasicCredentials(userName, password))
                        return;// 認証成功
                    break;
                case QoApiAuthorizeTypeEnum.JwtQolmsApiKey:
                case QoApiAuthorizeTypeEnum.JwtJotoApiKey:
                    // Jwt認証（X-JOTO-API-KEYヘッダ）
                case QoApiAuthorizeTypeEnum.JwtToken:
                case QoApiAuthorizeTypeEnum.JwtAccessKey:
                case QoApiAuthorizeTypeEnum.JwtToken | QoApiAuthorizeTypeEnum.JwtAccessKey:
                     // Jwt認証
                     if (this.ValidateJwtCredentials(authorizationHeader, this._authType, out Guid accountKey, out funcType, out Guid parentKey, out  executor))
                     {
                        bool isOk = false;
                        foreach (QoApiFunctionTypeEnum e in Enum.GetValues(typeof(QoApiFunctionTypeEnum)))
                        {
                            if (e != QoApiFunctionTypeEnum.None && funcType.HasFlag(e) && this._funcType.HasFlag(e))
                            {
                                isOk = true;
                                break;
                            }
                        }
                        if (isOk)
                        {
                            // 認証成功
                            var cnt = (QoApiControllerBase)actionContext.ControllerContext.Controller;
                            cnt.AccountKey = accountKey;
                            cnt.ParentKey = parentKey;
                            cnt.EncExecutor = executor;
                            return;
                        }
                     }
                     break;
                case QoApiAuthorizeTypeEnum.Invalid:
                    // 認証ヘッダなしを許容する
                    if (this._isAllowNoToken)
                        return;
                    break;
            }

            // If Me.ValidateJwtCredentials(authorizationHeader, Me._authType, accountKey) Then
            // ' 認証成功
            // actionContext.Request.Headers.Add("acc", accountKey.ToString("N"))
            // CType(actionContext.ControllerContext.Controller, QoApiControllerBase).AccountKey = accountKey
            // Return
            // End If

            // QoAccessLog.WriteInfoLog("失敗もしくは再チャレンジ");
            // QoAccessLog.WriteInfoLog(authorizationHeader);
            // 認証失敗（再チャレンジ）
            var response = new System.Net.Http.HttpResponseMessage(HttpStatusCode.Unauthorized);

            if (this._authType.HasFlag(QoApiAuthorizeTypeEnum.Basic))
                response.Headers.Add("WWW-Authenticate", "Basic realm =\"QolmsOpenApi\"");
            if (this._authType.HasFlag(QoApiAuthorizeTypeEnum.JwtAccessKey) || this._authType.HasFlag(QoApiAuthorizeTypeEnum.JwtJotoApiKey) || this._authType.HasFlag(QoApiAuthorizeTypeEnum.JwtQolmsApiKey) || this._authType.HasFlag(QoApiAuthorizeTypeEnum.JwtToken))
                response.Headers.Add("WWW-Authenticate", "Bearer realm =\"QolmsOpenApi\"");
            actionContext.Response = response;
        }
    }


}