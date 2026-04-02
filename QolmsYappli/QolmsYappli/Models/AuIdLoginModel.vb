Imports MGF.QOLMS.QolmsApiCoreV1
Imports Microsoft.IdentityModel.Tokens
Imports System.IdentityModel.Tokens.Jwt
Imports System.Security.Cryptography.X509Certificates

Public NotInheritable Class AuIdLoginModel
    'Implements IQyModelUpdater(Of LoginModel)
    Public Shared ReadOnly CLIENT_ID As String = ConfigurationManager.AppSettings("AuClientId")   '事前に取得するクライアントID。
    Public Shared ReadOnly CLIENT_SECRET As String = ConfigurationManager.AppSettings("AuClientSecret")  'クライアントIDに対応するクライアントシークレット

    Private Const response_type As String = "code"
    Private Const scope As String = "openid"    'scope=openid△一般スコープ



    'Discovery応答
    'Public Class DiscoveryResults
    '    Public Property issuer As String
    '    Public Property token_endpoint_auth_methods_supported() As String
    '    Public Property request_uri_parameter_supported As Boolean
    '    Public Property authorization_endpoint As String
    '    Public Property token_endpoint As String
    '    Public Property jwks_uri As String
    '    Public Property response_types_supported() As String
    '    Public Property subject_types_supported() As String
    '    Public Property id_token_signing_alg_values_supported() As String
    '    Public Property code_challenge_methods_supported() As String
    '    Public Property grant_types_supported() As String
    'End Class

    'アクセストークン取得要求の応答
    Public Class AccessTokenRequestResults
        Public Property access_token As String
        Public Property token_type As String
        Public Property expires_in As Integer
        Public Property refresh_token As String
        Public Property id_token As String
    End Class

    'JWKセットドキュメント応答
    Public Class JwkKeyRequestResults
        Public Property keys As List(Of Key)
    End Class

    Public Class Key
        Public Property kty As String
        Public Property alg As String
        Public Property use As String
        Public Property kid As String
        Public Property n As String
        Public Property e As String
    End Class

    Private _state As String = String.Empty
    Private _nonce As String = String.Empty
    Private _code_verifier As String = String.Empty
    Private _redirect_uri As String = String.Empty
    Private _discovery_uri As String = String.Empty

#Region "Public Property"

    'Public Property AuthInf As DiscoveryResults = Nothing
    Public Property OpenIdConfig As Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectConfiguration = Nothing

    'Public Property AccessTokenInf As AccessTokenRequestResults = Nothing

    'OpenID?AuID?
    Public Property UserId As String = String.Empty

    'WowIDかどうか
    Public Property ByWowId As Boolean = False

    Public ReadOnly Property State As String
        Get
            Return _state
        End Get
    End Property
    Public ReadOnly Property Nonce As String
        Get
            Return _nonce
        End Get
    End Property

    Public ReadOnly Property RedirectUri As String
        Get
            Return _redirect_uri
        End Get
    End Property

    Public ReadOnly Property CodeVerifier As String
        Get
            Return _code_verifier
        End Get
    End Property


#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="AuIdLoginModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(discoveryUri As String, redirectUri As String)
        _discovery_uri = discoveryUri
        _redirect_uri = RedirectUri
        _state = Guid.NewGuid.ToString("N")
        _nonce = Guid.NewGuid.ToString("N")
        _code_verifier = Guid.NewGuid.ToString("N") & Guid.NewGuid.ToString("N") '１つだと４３文字いかないから無理やり。。  '"dBjftJeZ4CVP-mB92K27uhbUJU1p1r_wW1gFWFOEjXk" ' 
        '→E9Melhoa2OwvFrEMTJguCHaoeK1t8URWbuGJSstw-cMになればOKらしい。
    End Sub

#End Region

#Region "Private Method"

    ''' <summary>
    ''' CodeVerifierからCodeCharengeを生成して返す
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetCodeChallenge() As String
        Dim origByte As Byte() = System.Text.Encoding.ASCII.GetBytes(_code_verifier)

        Dim sha256 As System.Security.Cryptography.SHA256 = New System.Security.Cryptography.SHA256CryptoServiceProvider()
        Dim hashValue As Byte() = sha256.ComputeHash(origByte)


        'Base64 Encode 結果から、改行と = を取り除いた上で、+ を - に、/ を _ にそれぞれ置換
        Return Convert.ToBase64String(hashValue, Base64FormattingOptions.None).Replace("=", "").Replace("+", "-").Replace("/", "_")
        '        Return HttpServerUtility.UrlTokenEncode(hashValue)
    End Function
#End Region
#Region "Public Method"

    ' ''' <summary>
    ' ''' インプット モデルの内容を現在のインスタンスに反映します。
    ' ''' </summary>
    ' ''' <param name="inputModel">インプット モデル。</param>
    ' ''' <remarks></remarks>
    'Public Sub UpdateByInput(inputModel As LoginModel) Implements IQyModelUpdater(Of LoginModel).UpdateByInput

    '    If inputModel IsNot Nothing Then
    '        With inputModel
    '            Me.UserId = If(String.IsNullOrWhiteSpace(.UserId), String.Empty, .UserId.Trim())
    '            Me.Password = If(String.IsNullOrWhiteSpace(.Password), String.Empty, .Password.Trim())
    '            Me.RememberId = .RememberId
    '            Me.RememberLogin = .RememberLogin
    '        End With
    '    End If

    'End Sub

    '認可要求URLを返す
    Public Function GetAuthorizationRequestUrl() As String
        ' Dim endpoint As String = AuthInf.authorization_endpoint
        Dim endpoint As String = OpenIdConfig.AuthorizationEndpoint

        '        Return String.Format("{0}&response_type={1}&client_id={2}&redirect_uri={3}&scope={4}",
        ' Return String.Format("{0}&response_type={1}&client_id={2}&redirect_uri={3}&scope={4}&state={5}&nonce={8}",
        Return String.Format("{0}&response_type={1}&client_id={2}&redirect_uri={3}&scope={4}&state={5}&code_challenge={6}&code_challenge_method={7}&nonce={8}",
                             endpoint,
                             response_type,
                             CLIENT_ID, _
                             System.Web.HttpUtility.UrlEncode(_redirect_uri), _
                             scope, _
                            _state, _
                            GetCodeChallenge(), _
                            "S256",
                            _nonce)
    End Function

    'アクセストークン取得要求URLを返す
    Public Function GetAccessTokenRequestUrl() As String
        'Return AuthInf.token_endpoint
        Return OpenIdConfig.TokenEndpoint
    End Function

    ''' <summary>
    ''' ログアウト用のURLを返す。
    ''' </summary>
    ''' <param name="isWow">WowIDでログインしている場合はTrue</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetLogoutUrl(isWow As Boolean) As String
        If isWow Then
            Return ConfigurationManager.AppSettings("WowLogoutUri")
        Else
            Return ConfigurationManager.AppSettings("AuLogoutUri")
        End If
    End Function

    ''' <summary>
    ''' WowID新規登録画面のURLを返す
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetWowNewAccountUrl() As String
        Return ConfigurationManager.AppSettings("WowNewAccountUri")
    End Function


    Public Function GetAuthorizationRequestUrl(returnUrl As String) As String
        ' Dim endpoint As String = AuthInf.authorization_endpoint
        Dim endpoint As String = OpenIdConfig.AuthorizationEndpoint

        Return String.Format("{0}&response_type={1}&client_id={2}&redirect_uri={3}&scope={4}&state={5}&code_challenge={6}&code_challenge_method={7}&nonce={8}",
                             endpoint,
                             response_type,
                             CLIENT_ID, _
                             System.Web.HttpUtility.UrlEncode(returnUrl), _
                             scope, _
                            _state, _
                            GetCodeChallenge(), _
                            "S256",
                            _nonce)
    End Function
#End Region

End Class