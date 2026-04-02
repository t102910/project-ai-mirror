Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsAppleAuthApiCore

Friend NotInheritable Class AppleIdLoginWorker

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
    'Private Shared JWT_VALIDATE_LIFETIME_FLAG As Boolean = ConfigurationManager.AppSettings("JwtValidateLifetimeFlag").TryToValueType(True)

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

    Private Shared Function CreateReturnUrl() As String

        Dim root As String = ConfigurationManager.AppSettings("QolmsYappliSiteUri")

        If Not root.EndsWith("/") Then
            root += "/"
        End If

        Dim url As String = root + "start/appleidloginresult"

        Return url

    End Function


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
            AppleIdLoginWorker.DUMMY_SESSION_ID,
            AppleIdLoginWorker.DUMMY_API_AUTHORIZE_KEY
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
    Public Shared Function Auth(
            SessionId As String,
            state As AuthorizeApiResults,
            ByRef refLoginModel As AppleIdLoginModel,
            ByRef refAuthorAccount As AuthorAccountItem,
            ByRef refApiAuthorizeKey As Guid,
            ByRef refApiAuthorizeExpires As Date,
            ByRef refApiAuthorizeKey2 As Guid,
            ByRef refApiAuthorizeExpires2 As Date,
            ByRef refLoginRetryCount As Byte,
            ByRef refLoginLockdownExpires As Date,
            ByRef refIsSettingComplete As Boolean) As QsApiOpenIdLoginResultTypeEnum

        Dim result As QsApiOpenIdLoginResultTypeEnum = QsApiOpenIdLoginResultTypeEnum.None

        DebugLog($"{New QsJsonSerializer().Serialize(state)} ")

        refLoginModel = New AppleIdLoginModel()
        If state.user IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(state.user) Then
            'user情報の取得
            DebugLog($"{state.user} ")

            Dim user As UserOfJson = New QsJsonSerializer().Deserialize(Of UserOfJson)(state.user)
            refLoginModel.FirstName = user.name.firstName
            refLoginModel.LastName = user.name.lastName
            refLoginModel.MailAddress = user.email

        End If

        DebugLog($"given: {refLoginModel.FirstName}, family: {refLoginModel.LastName}, email: {refLoginModel.MailAddress} ")

        'Tokenの検証でID取り出し
        Dim token As String = state.id_token
        DebugLog($"{token}")

        Dim log As String = String.Empty
        Dim idtoken As IdTokenOfJson = QsAppleAuthApiManager.idTokenDecode(token, log)

        DebugLog($"{New QsJsonSerializer().Serialize(idtoken)}")
        DebugLog($"{log}")

        refLoginModel.UserId = idtoken.sub
        If String.IsNullOrWhiteSpace(refLoginModel.MailAddress) Then
            refLoginModel.MailAddress = idtoken.email
        End If

        DebugLog($"{refLoginModel.UserId}")

        'こっからIdentityの認証
        If Not String.IsNullOrEmpty(refLoginModel.UserId) Then

            ' QolmsApi 用
            refApiAuthorizeKey = Guid.Empty
            refApiAuthorizeExpires = Date.MinValue

            ' QolmsJotoApi 用
            refApiAuthorizeKey2 = Guid.Empty
            refApiAuthorizeExpires2 = Date.MinValue

            refLoginRetryCount = Byte.MinValue
            refLoginLockdownExpires = Date.MinValue

            With AppleIdLoginWorker.ExecuteQolmsYappliLoginApi(SessionId, refLoginModel.UserId, QsApiOpenIdTypeEnum.AppleId)
                result = .LoginResultType.TryToValueType(QsApiOpenIdLoginResultTypeEnum.None)
                DebugLog($"{result}")

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
                        refAuthorAccount.OpenId = refLoginModel.UserId
                        refAuthorAccount.OpenIdType = 3 'AppleID

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

    Public Shared Function Auth(
            SessionId As String,
            UserId As String,
            ByRef refLoginModel As AppleIdLoginModel,
            ByRef refAuthorAccount As AuthorAccountItem,
            ByRef refApiAuthorizeKey As Guid,
            ByRef refApiAuthorizeExpires As Date,
            ByRef refApiAuthorizeKey2 As Guid,
            ByRef refApiAuthorizeExpires2 As Date,
            ByRef refLoginRetryCount As Byte,
            ByRef refLoginLockdownExpires As Date,
            ByRef refIsSettingComplete As Boolean) As QsApiOpenIdLoginResultTypeEnum

        Dim result As QsApiOpenIdLoginResultTypeEnum = QsApiOpenIdLoginResultTypeEnum.None

        refLoginModel.UserId = UserId
        DebugLog($"{refLoginModel.UserId}")

        'こっからIdentityの認証
        If Not String.IsNullOrEmpty(refLoginModel.UserId) Then

            ' QolmsApi 用
            refApiAuthorizeKey = Guid.Empty
            refApiAuthorizeExpires = Date.MinValue

            ' QolmsJotoApi 用
            refApiAuthorizeKey2 = Guid.Empty
            refApiAuthorizeExpires2 = Date.MinValue

            refLoginRetryCount = Byte.MinValue
            refLoginLockdownExpires = Date.MinValue

            With AppleIdLoginWorker.ExecuteQolmsYappliLoginApi(SessionId, refLoginModel.UserId, QsApiOpenIdTypeEnum.AppleId)
                result = .LoginResultType.TryToValueType(QsApiOpenIdLoginResultTypeEnum.None)
                DebugLog($"{result}")

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
                        refAuthorAccount.OpenId = refLoginModel.UserId
                        refAuthorAccount.OpenIdType = 3 'AppleID

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

    Public Shared Function GetAuthUrl() As String

        Dim redirect_uri As String = CreateReturnUrl()

        Return QsAppleAuthApiManager.GetAppleIdAuthorizationUrl(redirect_uri)

    End Function

#End Region

    'テスト用の手抜きログ吐き
    '<Conditional("DEBUG")>
    Public Shared Sub DebugLog(ByVal message As String)
        Try
            Dim log As String = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), "ApppleIdLogin.log")
            System.IO.File.AppendAllText(log, String.Format("{0}:{1}{2}", Now, message, vbCrLf))
        Catch ex As Exception
        End Try

    End Sub
End Class
