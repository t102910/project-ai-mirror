Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports System.Net
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Json
Imports MGF.QOLMS.QolmsYappli.AuIdLoginModel
Imports System.IO
Imports System.IdentityModel.Tokens.Jwt
Imports Microsoft.IdentityModel.Tokens
Imports Microsoft.IdentityModel.Protocols
Imports Microsoft.IdentityModel.Protocols.OpenIdConnect
Imports System.Threading
Imports System.Security.Cryptography.X509Certificates
Imports System.Security.Cryptography
Imports QolmsYappli.AuIdLoginModel

''' <summary>
''' AuIdログイン(OpenId connect)に関する機能を提供します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class AuIdLoginWorker

#Region "Constant"

    ''' <summary>
    ''' ダミーのセッションIDを現します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared DUMMY_SESSION_ID As String = New String("Z"c, 100)

    ''' <summary>
    ''' ダミーのAPI認証キーを表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared DUMMY_API_AUTHORIZE_KEY As Guid = New Guid(New String("F"c, 32))

    ''' <summary>
    ''' JWTのライフタイム検証をするかどうかを表します。（検証環境は2038/1/19(int32.MaxVale)を超える可能性があるのでOFFできるように）デフォルトはTrue
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared JWT_VALIDATE_LIFETIME_FLAG As Boolean = ConfigurationManager.AppSettings("JwtValidateLifetimeFlag").TryToValueType(True)

#End Region

#Region "Constructor"

    ''' <summary>
    ''' デフォルトコンストラクタは使用できません。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub

#End Region

#Region "Private Method"

    'Base64Url文字列をデコード
    Private Shared Function FromBase64Url(ByVal base64Url As String) As Byte()
        Dim padded As String = If(base64Url.Length Mod 4 = 0, base64Url, base64Url & "====".Substring(base64Url.Length Mod 4))
        Dim base64 As String = padded.Replace("_", "/").Replace("-", "+")
        Return Convert.FromBase64String(base64)
    End Function


    ' ''' <summary>
    ' ''' ディスカバリー要求
    ' ''' </summary>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Private Shared Function Discovery(discoveryUrl As String) As DiscoveryResults
    '    Call DebugLog(String.Format("Discovery Url:{0}", discoveryUrl))
    '    Dim result As DiscoveryResults = Nothing
    '    Try
    '        With DirectCast(WebRequest.Create(discoveryUrl), HttpWebRequest)
    '            .Method = "GET"
    '            Using response As HttpWebResponse = DirectCast(.GetResponse(), HttpWebResponse)
    '                If response.StatusCode = HttpStatusCode.OK Then
    '                    Using resStream As IO.Stream = response.GetResponseStream()
    '                        With New DataContractJsonSerializer(GetType(DiscoveryResults))
    '                            result = DirectCast(.ReadObject(resStream), DiscoveryResults)
    '                        End With
    '                    End Using
    '                End If
    '            End Using
    '        End With
    '    Catch ex As Exception
    '        Call DebugLog(String.Format("Discovery Error:{0}", ex.Message))
    '        Throw ex
    '    End Try

    '    Return result
    'End Function

    ''' <summary>
    ''' アクセストークン取得要求
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function AccessTokenRequest(loginModel As AuIdLoginModel, code As String) As AccessTokenRequestResults
        Dim requestResult As AccessTokenRequestResults = Nothing
        Try
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 ' SecurityProtocolType.Tls Or SecurityProtocolType.Tls11 Or SecurityProtocolType.Tls12

            Using wb As WebClient = New WebClient()
                wb.Headers(HttpRequestHeader.ContentType) = "application/x-www-form-urlencoded"
                Dim data As New NameValueCollection()
                data("grant_type") = "authorization_code"
                data("code") = code
                data("redirect_uri") = loginModel.RedirectUri
                data("client_id") = AuIdLoginModel.CLIENT_ID
                data("client_secret") = AuIdLoginModel.CLIENT_SECRET
                data("code_verifier") = loginModel.CodeVerifier

                Call DebugLog(String.Format("AccessTokenRequest code_verifier:{0}", loginModel.CodeVerifier))
                Call DebugLog(String.Format("AccessTokenRequest client_id:{0}", AuIdLoginModel.CLIENT_ID))
                Call DebugLog(String.Format("AccessTokenRequest client_secret:{0}", AuIdLoginModel.CLIENT_SECRET))

                Dim response() As Byte = wb.UploadValues(loginModel.GetAccessTokenRequestUrl, "POST", data)
                Using stream As New MemoryStream(response)
                    With New DataContractJsonSerializer(GetType(AccessTokenRequestResults))
                        requestResult = DirectCast(.ReadObject(stream), AccessTokenRequestResults)
                    End With
                End Using
            End Using


        Catch ex As WebException
            If ex.Status = WebExceptionStatus.ProtocolError AndAlso ex.Response IsNot Nothing Then
                Dim response As String = New StreamReader(ex.Response.GetResponseStream()).ReadToEnd()
                Call DebugLog(response)
            End If
            Call DebugLog(String.Format("AccessTokenRequest Error:{0}", ex.Message))
        Catch ex As Exception
            Call DebugLog(String.Format("AccessTokenRequest Error:{0}", ex.Message))
            Throw ex

        End Try
        Return requestResult
    End Function

    'IDトークンを検証して、成功したらOpenIDを返す
    Private Shared Function GetValidatedUserId(requestResult As AccessTokenRequestResults, loginModel As AuIdLoginModel) As String
        'OIDC_TOKEN_AU_RES  トークン取得応答チェック(f)
        Dim result As String = String.Empty
        Dim handler As New JwtSecurityTokenHandler()
        Dim securityToken As SecurityToken = Nothing
        Dim token As JwtSecurityToken = CType(handler.ReadToken(requestResult.id_token), JwtSecurityToken)
        'DebugLog(String.Format("0.jwksUri>{0}", loginModel.OpenIdConfig.JwksUri))
        'DebugLog(String.Format("1.生IDトークン>{0}", requestResult.id_token))
        'DebugLog(String.Format("2.前部 ： JWTRawヘッダ>{0}", token.RawHeader))
        'DebugLog(String.Format("2.前部 ： JWTDecodeヘッダ>{0}", BitConverter.ToString(FromBase64Url(token.RawHeader))))
        'DebugLog(String.Format("3.中部:    JWTRawペイロード>{0}", token.RawPayload))
        'DebugLog(String.Format("3.中部:    JWTDecodeペイロード>{0}", BitConverter.ToString(FromBase64Url(token.EncodedPayload))))
        'DebugLog(String.Format("4.後部:    Raw署名>{0}", token.RawSignature))
        'DebugLog(String.Format("4.後部:    Decode署名>{0}", BitConverter.ToString(FromBase64Url(token.RawSignature))))

        Try
            Dim seckey As SecurityKey = Nothing
            For Each keyItem As JsonWebKey In loginModel.OpenIdConfig.JsonWebKeySet.Keys
                If keyItem.Kid = token.Header.Kid Then
                    'Call DebugLog("TokenValidation keyid一致")
                    Dim rsa As RSA = RSA.Create()
                    Dim modulusBytes As Byte() = FromBase64Url(keyItem.N)
                    'ライブラリの実装によっては先頭に余計な０値が含まれることがあるらしいので存在してたら抹消して使用する
                    ' https://stackoverflow.com/questions/48998952/c-sharp-rsacryptoserviceprovider-jwt-rs256-validation-fails
                    ' https://tools.ietf.org/html/rfc7518#section-6.3.1.1
                    If modulusBytes(0) = 0 Then
                        Dim tmp As Byte() = New Byte(modulusBytes.Length - 2) {}
                        Array.Copy(modulusBytes, 1, tmp, 0, tmp.Length)
                        modulusBytes = tmp
                    End If
                    rsa.ImportParameters(New RSAParameters() With {.Exponent = FromBase64Url(keyItem.E),
                                                                   .Modulus = modulusBytes})
                    seckey = New RsaSecurityKey(rsa)
                    'Else
                    '    Call DebugLog("TokenValidation keyid一致しないので無視")
                End If
                'Call DebugLog(String.Format("TokenValidation keyid:{0}", keyItem.KeyId))
                'Call DebugLog(String.Format("TokenValidation e:{0}", keyItem.E))
                'Call DebugLog(String.Format("TokenValidation n:{0}", keyItem.N))
                'Call DebugLog(String.Format("TokenValidation decode e:{0}", BitConverter.ToString(FromBase64Url(keyItem.E))))
                'Call DebugLog(String.Format("TokenValidation decode n:{0}", BitConverter.ToString(FromBase64Url(keyItem.N))))
            Next
            'Call DebugLog(String.Format("TokenValidation JWTHeader.Kid:{0}", token.Header.Kid))
            'Call DebugLog(String.Format("TokenValidation alg :{0}", token.Header.Alg))
            'Call DebugLog(String.Format("TokenValidation id_token:{0}", requestResult.id_token))

            'Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = True '検証エラー詳細を吐かせるための設定

            Dim tokenValidationParam As New TokenValidationParameters() With {
                                     .IssuerSigningKey = seckey,
                                     .ValidateIssuerSigningKey = True,
                                     .ValidateIssuer = True,
                                     .ValidIssuer = loginModel.OpenIdConfig.Issuer,
                                     .ValidateAudience = True,
                                     .ValidAudience = AuIdLoginModel.CLIENT_ID,
                                     .ValidateLifetime = JWT_VALIDATE_LIFETIME_FLAG
                                     }

            ' トークンの検証
            Call DebugLog("IDトークンを検証します")
            handler.InboundClaimTypeMap.Clear()
            Dim cmp As System.Security.Claims.ClaimsPrincipal = handler.ValidateToken(requestResult.id_token, tokenValidationParam, securityToken)
            If token.Payload.First(Function(m) m.Key = "nonce").Value.ToString <> loginModel.Nonce Then Throw New Exception("nonce検証失敗") 'noniceの検証 

            'OpenID
            result = cmp.Claims.First(Function(c) c.Type = "sub").Value
            Call DebugLog(String.Format("GetValidatedUserId Success:{0}", result))
        Catch ex As Exception
            Call DebugLog(String.Format("GetValidatedUserId Error:{0}", ex.Message))
            AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("GetValidatedUserId Error:{0}", ex.Message))
        End Try

        Return result
    End Function

    'Private Shared Function GetSecurityKey(JwkRequestUri As String, kid As String) As RsaSecurityKey
    '    '        Dim results As New List(Of RsaSecurityKey)()

    '    Dim jwkRequestResult As JwkKeyRequestResults = Nothing
    '    Try
    '        Using wc As New WebClient
    '            Dim data As Byte() = wc.DownloadData(JwkRequestUri)
    '            Using stream As New MemoryStream(data)
    '                With New DataContractJsonSerializer(GetType(JwkKeyRequestResults))
    '                    jwkRequestResult = DirectCast(.ReadObject(stream), JwkKeyRequestResults)
    '                End With
    '            End Using
    '        End Using
    '    Catch ex As Exception
    '        Call DebugLog("Jwk取得失敗" & ex.Message)
    '        Throw ex
    '    End Try
    '    If jwkRequestResult IsNot Nothing Then
    '        For Each keyItem As Key In jwkRequestResult.keys
    '            'Dim e As Byte() = System.Web.HttpUtility.UrlDecodeToBytes(keyItem.e)
    '            'Dim n As Byte() = System.Web.HttpUtility.UrlDecodeToBytes(keyItem.n)
    '            Call DebugLog(String.Format("Jwk raw e:{0}", keyItem.e))
    '            Call DebugLog(String.Format("Jwk raw n:{0}", keyItem.n))
    '            Dim rsa As RSA = rsa.Create()
    '            'rsa.ImportParameters(New RSAParameters() With {.Exponent = Base64UrlEncoder.DecodeBytes(keyItem.e),
    '            '                                               .Modulus = Base64UrlEncoder.DecodeBytes(keyItem.n)})
    '            rsa.ImportParameters(New RSAParameters() With {.Exponent = Convert.FromBase64String(keyItem.e),
    '                                                           .Modulus = Convert.FromBase64String(keyItem.n)})

    '            'Dim resultKey As New RsaSecurityKey(New RSAParameters() With {.Exponent = e, .Modulus = n})
    '            Dim resultKey As New RsaSecurityKey(rsa)
    '            resultKey.KeyId = keyItem.kid

    '            If keyItem.kid = kid Then
    '                'results.Add(resultKey)

    '                Return resultKey
    '            End If
    '        Next
    '    End If
    '    Return Nothing
    '    '  Return results
    'End Function



    ''' <summary>
    ''' QOLMSのログインApiを実行する
    ''' </summary>
    ''' <param name="sessionId"></param>
    ''' <param name="openId"></param>
    ''' <param name="openIdType"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteQolmsYappliLoginApi(
       sessionId As String,
       openId As String,
       openIdType As Byte
   ) As QiQolmsYappliLoginApiResults

        Dim apiArgs As New QiQolmsYappliLoginApiArgs(
            QiApiTypeEnum.QolmsYappliLogin,
            QsApiSystemTypeEnum.Qolms,
            Guid.Empty,
            String.Empty
        ) With {
            .SessionId = sessionId,
            .OpenId = openId,
            .OpenIdType = openIdType.ToString()
        }
        Dim apiResults As QiQolmsYappliLoginApiResults = QsApiManager.ExecuteQolmsIdentityApi(Of QiQolmsYappliLoginApiResults)(
            apiArgs,
            AuIdLoginWorker.DUMMY_SESSION_ID,
            AuIdLoginWorker.DUMMY_API_AUTHORIZE_KEY
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsIdentityApiName(apiArgs)))
            End If
        End With

    End Function



#End Region

#Region "Public Method"

    ' ''' <summary>
    ' ''' ディスカバリーを行って認証エンドポイントにアクセスするのに必要な情報を取得する
    ' ''' </summary>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Shared Function GetAuthInf(redirectUrl As String) As AuIdLoginModel

    '    Dim loginModel As New AuIdLoginModel(redirectUrl)
    '    loginModel.AuthInf = AuIdLoginWorker.Discovery(loginModel.GetDiscoveryUrl)
    '    Return loginModel

    'End Function
    ''' <summary>
    ''' ディスカバリーを行って認証エンドポイントにアクセスするのに必要な情報を取得する
    ''' </summary>
    ''' <param name="isWow"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Async Function GetAuthInfAync(Optional ByVal isWow As Boolean = False) As Threading.Tasks.Task(Of AuIdLoginModel)
        Dim redirectUrl As String = ConfigurationManager.AppSettings("QolmsYappliSiteUri") & "start/auidloginresult"
        Dim discoveryUri As String = ""
        If isWow Then
            discoveryUri = ConfigurationManager.AppSettings("WowDiscoveryUri")
        Else
            discoveryUri = ConfigurationManager.AppSettings("AuDiscoveryUri")
        End If
        Dim loginModel As New AuIdLoginModel(discoveryUri, redirectUrl)
        loginModel.ByWowId = isWow
        'このライブラリ内で、ディスカバリーとJWKセットドキュメント要求をやってくれるぽい
        Dim configurationManage As IConfigurationManager(Of OpenIdConnectConfiguration) = New ConfigurationManager(Of OpenIdConnectConfiguration)(discoveryUri,
                                                                                         New OpenIdConnectConfigurationRetriever())
        loginModel.OpenIdConfig = Await configurationManage.GetConfigurationAsync(CancellationToken.None)
        'GetSecurityKey(loginModel.OpenIdConfig.JwksUri, "")
        Return loginModel
    End Function

    ''' <summary>
    ''' ディスカバリーを行って認証エンドポイントにアクセスするのに必要な情報を取得する
    ''' </summary>
    ''' <param name="isWow"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Async Function GetAuthInfAync(returnUrl As String, Optional ByVal isWow As Boolean = False) As Threading.Tasks.Task(Of AuIdLoginModel)
        Dim redirectUrl As String = ConfigurationManager.AppSettings("QolmsYappliSiteUri") & "start/logineditauidresult"
        Dim discoveryUri As String = ""
        If isWow Then
            discoveryUri = ConfigurationManager.AppSettings("WowDiscoveryUri")
        Else
            discoveryUri = ConfigurationManager.AppSettings("AuDiscoveryUri")
        End If
        Dim loginModel As New AuIdLoginModel(discoveryUri, redirectUrl)
        loginModel.ByWowId = isWow

        'このライブラリ内で、ディスカバリーとJWKセットドキュメント要求をやってくれるぽい
        Dim configurationManage As IConfigurationManager(Of OpenIdConnectConfiguration) = New ConfigurationManager(Of OpenIdConnectConfiguration)(discoveryUri,
                                                                                         New OpenIdConnectConfigurationRetriever())
        loginModel.OpenIdConfig = Await configurationManage.GetConfigurationAsync(CancellationToken.None)
        'GetSecurityKey(loginModel.OpenIdConfig.JwksUri, "")
        Return loginModel
    End Function


    ''' <summary>
    '''  AuのOpenIDを取得し、QOLMSログインを実行した結果を返す
    ''' </summary>
    ''' <param name="sessionId"></param>
    ''' <param name="loginModel"></param>
    ''' <param name="code"></param>
    ''' <param name="refAuthorAccount"></param>
    ''' <param name="refApiAuthorizeKey"></param>
    ''' <param name="refApiAuthorizeExpires"></param>
    ''' <param name="refApiAuthorizeKey2"></param>
    ''' <param name="refApiAuthorizeExpires2"></param>
    ''' <param name="refLoginRetryCount"></param>
    ''' <param name="refLoginLockdownExpires"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Auth(sessionId As String,
                                loginModel As AuIdLoginModel,
                                code As String,
                                ByRef refAuthorAccount As AuthorAccountItem,
                                ByRef refApiAuthorizeKey As Guid,
                                ByRef refApiAuthorizeExpires As Date,
                                ByRef refApiAuthorizeKey2 As Guid,
                                ByRef refApiAuthorizeExpires2 As Date,
                                ByRef refLoginRetryCount As Byte,
                                ByRef refLoginLockdownExpires As Date,
                                ByRef refIsSettingComplete As Boolean) As QsApiOpenIdLoginResultTypeEnum

        Dim result As QsApiOpenIdLoginResultTypeEnum = QsApiOpenIdLoginResultTypeEnum.None

        'OIDC_TOKEN_AU_REQ  トークン取得要求(e)、バリデート
        Dim accessTokenResult As AccessTokenRequestResults = AuIdLoginWorker.AccessTokenRequest(loginModel, code)
        loginModel.UserId = GetValidatedUserId(accessTokenResult, loginModel)

        'QOLMSログインApi呼び出し
        If String.IsNullOrEmpty(loginModel.UserId) = False Then
            ' QolmsApi 用
            refApiAuthorizeKey = Guid.Empty
            refApiAuthorizeExpires = Date.MinValue

            ' QolmsJotoApi 用
            refApiAuthorizeKey2 = Guid.Empty
            refApiAuthorizeExpires2 = Date.MinValue

            refLoginRetryCount = Byte.MinValue
            refLoginLockdownExpires = Date.MinValue

            With AuIdLoginWorker.ExecuteQolmsYappliLoginApi(sessionId, loginModel.UserId, QsApiOpenIdTypeEnum.AuId)
                result = .LoginResultType.TryToValueType(QsApiOpenIdLoginResultTypeEnum.None)

                refLoginRetryCount = .LoginRetryCount.TryToValueType(Byte.MinValue)
                refLoginLockdownExpires = .LoginLockdownExpires.TryToValueType(Date.MinValue)

                Select Case result
                    Case QsApiOpenIdLoginResultTypeEnum.Success
                        ' ログイン可能

                        ' ログイン状態へ移行するために必要な情報を返却
                        refAuthorAccount.AccountKey = .AccountKey.TryToValueType(Guid.Empty)
                        refAuthorAccount.AccountKeyHash = QyAccountItemBase.CreateAccountKeyHashString(refAuthorAccount.AccountKey)
                        refAuthorAccount.FamilyName = .FamilyName
                        refAuthorAccount.MiddleName = .MiddleName
                        refAuthorAccount.GivenName = .GivenName
                        refAuthorAccount.FamilyKanaName = .FamilyKanaName
                        refAuthorAccount.MiddleKanaName = .MiddleKanaName
                        refAuthorAccount.GivenKanaName = .GivenKanaName
                        refAuthorAccount.FamilyRomanName = .FamilyRomanName
                        refAuthorAccount.MiddleRomanName = .MiddleRomanName
                        refAuthorAccount.GivenRomanName = .GivenRomanName
                        refAuthorAccount.SexType = .SexType.TryToValueType(QySexTypeEnum.None)
                        refAuthorAccount.Birthday = .Birthday.TryToValueType(Date.MinValue)
                        refAuthorAccount.AcceptFlag = .IsAccept.TryToValueType(False)
                        refAuthorAccount.EncryptedAccountKey = QyAccountItemBase.EncryptAccountKey(refAuthorAccount.AccountKey)
                        refAuthorAccount.UserId = If(String.IsNullOrEmpty(.UserId), Guid.NewGuid().ToString("N"), .UserId)
                        refAuthorAccount.PasswordHash = .PasswordHash
                        refAuthorAccount.LoginCount = .LoginCount.TryToValueType(Integer.MinValue)
                        refAuthorAccount.LoginAt = .LoginAt.TryToValueType(Date.MinValue)
                        refAuthorAccount.LoginByWowId = loginModel.ByWowId
                        refAuthorAccount.OpenId = loginModel.UserId

                        ' QolmsApi 用
                        refApiAuthorizeKey = .AuthorizeKey.TryToValueType(Guid.Empty)
                        refApiAuthorizeExpires = .AuthorizeExpires.TryToValueType(Date.MinValue)

                        ' QolmsJotoApi 用
                        refApiAuthorizeKey2 = .AuthorizeKey2.TryToValueType(Guid.Empty)
                        refApiAuthorizeExpires2 = .AuthorizeExpires2.TryToValueType(Date.MinValue)

                        refIsSettingComplete = .IsSettingComplete.TryToValueType(False)

                End Select


            End With

        End If

        Return result

    End Function


    ''' <summary>
    '''  AuのOpenIDを取得して返却
    ''' </summary>
    ''' <param name="loginModel"></param>
    ''' <param name="code"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetOpenId(loginModel As AuIdLoginModel,
                                code As String) As String

        'OIDC_TOKEN_AU_REQ  トークン取得要求(e)、バリデート
        AuIdLoginWorker.DebugLog(loginModel.RedirectUri)
        AuIdLoginWorker.DebugLog(code)

        Dim accessTokenResult As AccessTokenRequestResults = AuIdLoginWorker.AccessTokenRequest(loginModel, code)
        loginModel.UserId = GetValidatedUserId(accessTokenResult, loginModel)

        If Not String.IsNullOrWhiteSpace(loginModel.UserId) Then

            Return loginModel.UserId

        End If

        Return String.Empty

    End Function


#End Region

    'テスト用の手抜きログ吐き
    <Conditional("DEBUG")> _
    Public Shared Sub DebugLog(ByVal message As String)
        Try
            Dim log As String = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), "log.txt")
            System.IO.File.AppendAllText(log, String.Format("{0}:{1}{2}", Now, message, vbCrLf))
        Catch ex As Exception
        End Try

    End Sub
End Class