Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1

''' <summary>
''' ログインに関する機能を提供します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class LoginWorker

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

    Private Shared Function ExecuteQolmsYappliLoginApi(
        sessionId As String,
        userId As String,
        password As String,
        passwordHash As String
    ) As QiQolmsYappliLoginApiResults

        Dim apiArgs As New QiQolmsYappliLoginApiArgs(
            QiApiTypeEnum.QolmsYappliLogin,
            QsApiSystemTypeEnum.Qolms,
            Guid.Empty,
            String.Empty
        ) With {
            .SessionId = sessionId,
            .UserId = userId.Trim(),
            .Password = password.Trim(),
            .PasswordHash = passwordHash.Trim()
        }
        Dim apiResults As QiQolmsYappliLoginApiResults = QsApiManager.ExecuteQolmsIdentityApi(Of QiQolmsYappliLoginApiResults)(
            apiArgs,
            LoginWorker.DUMMY_SESSION_ID,
            LoginWorker.DUMMY_API_AUTHORIZE_KEY
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsIdentityApiName(apiArgs)))
            End If
        End With

    End Function

    Private Shared Function ExecuteQolmsYappliJotoIdLoginApi(
    sessionId As String,
    userId As String,
    password As String,
    passwordHash As String
) As QiQolmsYappliJotoIdLoginApiResults

        Dim apiArgs As New QiQolmsYappliJotoIdLoginApiArgs(
            QiApiTypeEnum.QolmsYappliJotoIdLogin,
            QsApiSystemTypeEnum.Qolms,
            Guid.Empty,
            String.Empty
        ) With {
            .SessionId = sessionId,
            .UserId = String.Format("JOTO{0}", userId.Trim()),
            .Password = password.Trim(),
            .PasswordHash = passwordHash.Trim()
        }
        Dim apiResults As QiQolmsYappliJotoIdLoginApiResults = QsApiManager.ExecuteQolmsIdentityApi(Of QiQolmsYappliJotoIdLoginApiResults)(
            apiArgs,
            LoginWorker.DUMMY_SESSION_ID,
            LoginWorker.DUMMY_API_AUTHORIZE_KEY
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
        sessionId As String,
        userId As String,
        password As String,
        passwordHash As String,
        ByRef refAuthorAccount As AuthorAccountItem,
        ByRef refApiAuthorizeKey As Guid,
        ByRef refApiAuthorizeExpires As Date,
        ByRef refApiAuthorizeKey2 As Guid,
        ByRef refApiAuthorizeExpires2 As Date,
        ByRef refLoginRetryCount As Byte,
        ByRef refLoginLockdownExpires As Date,
        ByRef refIsSettingComplete As Boolean
    ) As QsApiOpenIdLoginResultTypeEnum

        refAuthorAccount = New AuthorAccountItem() With {
            .AccountKey = Guid.Empty,
            .AccountKeyHash = String.Empty,
            .FamilyName = String.Empty,
            .MiddleName = String.Empty,
            .GivenName = String.Empty,
            .FamilyKanaName = String.Empty,
            .MiddleKanaName = String.Empty,
            .GivenKanaName = String.Empty,
            .FamilyRomanName = String.Empty,
            .MiddleRomanName = String.Empty,
            .GivenRomanName = String.Empty,
            .SexType = QySexTypeEnum.None,
            .Birthday = Date.MinValue,
            .AcceptFlag = False,
            .EncryptedAccountKey = String.Empty,
            .IsAutoLogin = False,
            .UserId = String.Empty,
            .PasswordHash = String.Empty,
            .LoginCount = Integer.MinValue,
            .LoginAt = Date.MinValue
        }

        ' QolmsApi 用
        refApiAuthorizeKey = Guid.Empty
        refApiAuthorizeExpires = Date.MinValue

        ' QolmsJotoApi 用
        refApiAuthorizeKey2 = Guid.Empty
        refApiAuthorizeExpires2 = Date.MinValue

        refLoginRetryCount = Byte.MinValue
        refLoginLockdownExpires = Date.MinValue

        Dim result As QsApiOpenIdLoginResultTypeEnum = QsApiOpenIdLoginResultTypeEnum.None

        With LoginWorker.ExecuteQolmsYappliLoginApi(sessionId, userId, password, passwordHash)
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
                    refAuthorAccount.UserId = .UserId
                    refAuthorAccount.PasswordHash = .PasswordHash
                    refAuthorAccount.LoginCount = .LoginCount.TryToValueType(Integer.MinValue)
                    refAuthorAccount.LoginAt = .LoginAt.TryToValueType(Date.MinValue)
                    refIsSettingComplete = .IsSettingComplete.TryToValueType(False)

                    ' QolmsApi 用
                    refApiAuthorizeKey = .AuthorizeKey.TryToValueType(Guid.Empty)
                    refApiAuthorizeExpires = .AuthorizeExpires.TryToValueType(Date.MinValue)

                    ' QolmsJotoApi 用
                    refApiAuthorizeKey2 = .AuthorizeKey2.TryToValueType(Guid.Empty)
                    refApiAuthorizeExpires2 = .AuthorizeExpires2.TryToValueType(Date.MinValue)

            End Select


        End With

        Return result

    End Function

    Public Shared Function IdAuth(
        sessionId As String,
        userId As String,
        password As String,
        passwordHash As String,
        ByRef refAuthorAccount As AuthorAccountItem,
        ByRef refApiAuthorizeKey As Guid,
        ByRef refApiAuthorizeExpires As Date,
        ByRef refApiAuthorizeKey2 As Guid,
        ByRef refApiAuthorizeExpires2 As Date,
        ByRef refLoginRetryCount As Byte,
        ByRef refLoginLockdownExpires As Date,
        ByRef refIsSettingComplete As Boolean
    ) As QsApiOpenIdLoginResultTypeEnum

        refAuthorAccount = New AuthorAccountItem() With {
            .AccountKey = Guid.Empty,
            .AccountKeyHash = String.Empty,
            .FamilyName = String.Empty,
            .MiddleName = String.Empty,
            .GivenName = String.Empty,
            .FamilyKanaName = String.Empty,
            .MiddleKanaName = String.Empty,
            .GivenKanaName = String.Empty,
            .FamilyRomanName = String.Empty,
            .MiddleRomanName = String.Empty,
            .GivenRomanName = String.Empty,
            .SexType = QySexTypeEnum.None,
            .Birthday = Date.MinValue,
            .AcceptFlag = False,
            .EncryptedAccountKey = String.Empty,
            .IsAutoLogin = False,
            .UserId = String.Empty,
            .PasswordHash = String.Empty,
            .LoginCount = Integer.MinValue,
            .LoginAt = Date.MinValue
        }

        ' QolmsApi 用
        refApiAuthorizeKey = Guid.Empty
        refApiAuthorizeExpires = Date.MinValue

        ' QolmsJotoApi 用
        refApiAuthorizeKey2 = Guid.Empty
        refApiAuthorizeExpires2 = Date.MinValue

        refLoginRetryCount = Byte.MinValue
        refLoginLockdownExpires = Date.MinValue

        Dim result As QsApiOpenIdLoginResultTypeEnum = QsApiOpenIdLoginResultTypeEnum.None

        With LoginWorker.ExecuteQolmsYappliJotoIdLoginApi(sessionId, userId, password, passwordHash)
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
                    refAuthorAccount.UserId = .UserId
                    refAuthorAccount.PasswordHash = .PasswordHash
                    refAuthorAccount.LoginCount = .LoginCount.TryToValueType(Integer.MinValue)
                    refAuthorAccount.LoginAt = .LoginAt.TryToValueType(Date.MinValue)
                    refIsSettingComplete = .IsSettingComplete.TryToValueType(False)

                    ' QolmsApi 用
                    refApiAuthorizeKey = .AuthorizeKey.TryToValueType(Guid.Empty)
                    refApiAuthorizeExpires = .AuthorizeExpires.TryToValueType(Date.MinValue)

                    ' QolmsJotoApi 用
                    refApiAuthorizeKey2 = .AuthorizeKey2.TryToValueType(Guid.Empty)
                    refApiAuthorizeExpires2 = .AuthorizeExpires2.TryToValueType(Date.MinValue)

                    refAuthorAccount.OpenId = .OpenId

            End Select


        End With

        Return result

    End Function

#End Region

End Class