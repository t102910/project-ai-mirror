Imports System.Threading.Tasks
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsCryptV1
Imports MGF.QOLMS.QolmsKddiMessageCastApiCoreV1F472
Imports MGF.QOLMS.QolmsKddiMessageCastApiCoreV1F472.API.messages

Friend NotInheritable Class PasswordResetRecoverSmsWorker


#Region "Public Prop"

    Public Shared Property LogFilePath As String = String.Empty

#End Region

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

    Private Const MAIL_SUBJECT As String = "【JOTOホームドクター】PassCode"

    Const DEFALT_EXPIRES_MINUTES As Integer = 30

    Private Shared ReadOnly SETTING_EXPIRES_MINUTES As String = New Lazy(Of String)(Function() GetConfigSettings("PasswordResetSMSPassCodeExpiresMinutes")).Value

    ''' <summary>
    ''' リトライカウント
    ''' </summary>
    Private Shared ReadOnly _retryCount As New Lazy(Of String)(Function() ConfigurationManager.AppSettings("SMSAuthenticationCount"))

    ''' <summary>
    ''' パスコードの桁数
    ''' </summary>
    Private Shared ReadOnly _passCodeLength As New Lazy(Of String)(Function() ConfigurationManager.AppSettings("SMSPassCodeLength"))

    ''' <summary>
    ''' パスコードの有効期限
    ''' </summary>
    Private Shared ReadOnly _passCodeExpiresMinutes As New Lazy(Of String)(Function() ConfigurationManager.AppSettings("SMSPassCodeExpiresMinutes"))

    ''' <summary>
    '''  ログ のファイル名です
    ''' </summary>
    Private Const SMS_LOG_FILENAME As String = "SMS.Log"
#End Region

#Region "Constructor"

    ''' <summary>
    ''' デフォルト コンストラクタは使用できません。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub

#End Region


#Region "Private Property"
    ''' <summary>
    ''' パスコード入力のリトライカウントを取得します。
    ''' </summary>
    Private Shared ReadOnly Property RetryCount As Integer
        Get

            Dim result As Integer = Integer.MinValue
            '設定があれば設定値、なければ初期値1
            If Not String.IsNullOrWhiteSpace(_retryCount.Value) AndAlso Integer.TryParse(_retryCount.Value, result) Then

                result = If(result > 0, result, 1)
            Else
                result = 1
            End If

            Return result
        End Get

    End Property

    ''' <summary>
    ''' パスコードの桁数を取得します
    ''' </summary>
    Private Shared ReadOnly Property PassCodeLength As Integer
        Get
            Dim result As Integer = Integer.MinValue

            '設定があれば設定値、なければ初期値6
            If Not String.IsNullOrWhiteSpace(_passCodeLength.Value) AndAlso Integer.TryParse(_passCodeLength.Value, result) Then

                result = If(result > 0, result, 6)
            Else
                result = 6
            End If

            Return result
        End Get

    End Property

    ''' <summary>
    ''' パスコードの有効期限（分）を取得します。
    ''' </summary>
    Private Shared ReadOnly Property PassCodeExpiresMinutes As Integer
        Get
            Dim result As Integer = Integer.MinValue

            '設定があれば設定値、なければ初期値1
            If Not String.IsNullOrWhiteSpace(_passCodeExpiresMinutes.Value) AndAlso Integer.TryParse(_passCodeExpiresMinutes.Value, result) Then

                result = If(result > 0, result, 1)
            Else
                result = 1
            End If

            Return result
        End Get

    End Property

#End Region

#Region "Private Method"

    ''' <summary>
    ''' Config設定を取得します。
    ''' </summary>
    ''' <param name="settingsName">ConfigのKey名</param>
    ''' <returns>
    ''' Configのvalue値
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function GetConfigSettings(settingsName As String) As String

        Dim result As String = String.Empty

        If Not String.IsNullOrWhiteSpace(settingsName) Then

            Try
                result = ConfigurationManager.AppSettings(settingsName)
            Catch
            End Try

        End If

        Return result

    End Function

    Private Shared Function ExecutePasswordResetRecoverWriteApi(phoneNumber As String, userHostAddress As String, userHostName As String, userAgent As String) As QiQolmsJotoSmsPassCodeWriteApiResults

        'SmsAuthenticationType 画面番号入れる
        Dim apiArgs As New QiQolmsJotoSmsPassCodeWriteApiArgs(
            QiApiTypeEnum.QolmsJotoSmsPassCodeWrite,
            QsApiSystemTypeEnum.Qolms,
            Guid.Empty,
            String.Empty
        ) With {
            .PhoneNumber = phoneNumber,
            .SmsAuthenticationType = "1",
            .PassCodeLength = PassCodeLength.ToString(),
            .UserHostAddress = userHostAddress,
            .UserHostName = userHostName,
            .UserAgent = userAgent,
            .ExpiresMinutes = PassCodeExpiresMinutes.ToString()
        }
        Dim apiResults As QiQolmsJotoSmsPassCodeWriteApiResults = QsApiManager.ExecuteQolmsIdentityApi(Of QiQolmsJotoSmsPassCodeWriteApiResults)(
            apiArgs,
            PasswordResetRecoverSmsWorker.DUMMY_SESSION_ID,
            PasswordResetRecoverSmsWorker.DUMMY_API_AUTHORIZE_KEY
        )
        If apiResults.IsSuccess.TryToValueType(False) Then

            Return apiResults

        Else

            Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsIdentityApiName(apiArgs)))
        End If

    End Function

    Private Shared Function ExecutePasswordResetRecoverReadApi(phoneNumber As String, passCode As String) As QiQolmsJotoSmsPassCodeReadApiResults

        Dim apiArgs As New QiQolmsJotoSmsPassCodeReadApiArgs(
            QiApiTypeEnum.QolmsJotoSmsPassCodeRead,
            QsApiSystemTypeEnum.Qolms,
            Guid.Empty,
            String.Empty
        ) With {
            .PhoneNumber = phoneNumber,
            .PassCode = passCode
        }
        Dim apiResults As QiQolmsJotoSmsPassCodeReadApiResults = QsApiManager.ExecuteQolmsIdentityApi(Of QiQolmsJotoSmsPassCodeReadApiResults)(
            apiArgs,
            PasswordResetRecoverSmsWorker.DUMMY_SESSION_ID,
            PasswordResetRecoverSmsWorker.DUMMY_API_AUTHORIZE_KEY
        )
        If apiResults.IsSuccess.TryToValueType(False) Then

            Return apiResults

        Else

            Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsIdentityApiName(apiArgs)))
        End If

    End Function
    Private Shared Function ExecutePasswordResetWriteApi(accountkey As Guid, phoneNumber As String, userHostAddress As String, userHostName As String, userAgent As String) As QiQolmsJotoSmsPasswordResetWriteApiResults

        Dim apiArgs As New QiQolmsJotoSmsPasswordResetWriteApiArgs(
            QiApiTypeEnum.QolmsJotoSmsPasswordResetWrite,
            QsApiSystemTypeEnum.Qolms,
            Guid.Empty,
            String.Empty
        ) With {
            .AccountKey = accountkey.ToApiGuidString(),
            .PhoneNumber = phoneNumber,
            .UserHostAddress = userHostAddress,
            .UserHostName = userHostName,
            .UserAgent = userAgent
        }
        Dim apiResults As QiQolmsJotoSmsPasswordResetWriteApiResults = QsApiManager.ExecuteQolmsIdentityApi(Of QiQolmsJotoSmsPasswordResetWriteApiResults)(
            apiArgs,
            PasswordResetRecoverSmsWorker.DUMMY_SESSION_ID,
            PasswordResetRecoverSmsWorker.DUMMY_API_AUTHORIZE_KEY
        )
        If apiResults.IsSuccess.TryToValueType(False) Then

            Return apiResults

        Else

            Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsIdentityApiName(apiArgs)))
        End If

    End Function

    Private Shared Function ExecuteSmsSendWriteApi(accountkey As Guid, phoneNumber As String, smsSendType As QjApiSmsSendTypeEnum, message As String, comment As String) As QiQolmsJotoSmsSendWriteApiResults

        Dim apiArgs As New QiQolmsJotoSmsSendWriteApiArgs(
            QiApiTypeEnum.QolmsJotoSmsSendWrite,
            QsApiSystemTypeEnum.Qolms,
            Guid.Empty,
            String.Empty
        ) With {
            .accountkey = accountkey.ToApiGuidString(),
            .phoneNumber = phoneNumber,
            .smsSendType = Convert.ToByte(smsSendType).ToString(),
            .message = message,
            .comment = comment
        }
        Dim apiResults As QiQolmsJotoSmsSendWriteApiResults = QsApiManager.ExecuteQolmsIdentityApi(Of QiQolmsJotoSmsSendWriteApiResults)(
            apiArgs,
            PasswordResetRecoverSmsWorker.DUMMY_SESSION_ID,
            PasswordResetRecoverSmsWorker.DUMMY_API_AUTHORIZE_KEY
        )
        If apiResults.IsSuccess.TryToValueType(False) Then

            Return apiResults

        Else

            Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsIdentityApiName(apiArgs)))
        End If

    End Function

    ''' <summary>
    ''' ファイルパスをセットします
    ''' </summary>
    ''' <param name="path"></param>
    ''' <param name="Name"></param>
    Private Shared Sub GetLogFilePath(path As String, Name As String)

        'ログパスをセット
        PasswordResetRecoverSmsWorker.LogFilePath = System.IO.Path.Combine(path, $"{Name}_{Date.Today.ToString("yyyyMM")}")

    End Sub

    ''' <summary>
    ''' 本番出力用ログ
    ''' </summary>
    ''' <param name="message"></param>
    Public Shared Sub FileLog(ByVal message As String)
        Try
            Dim log As String = PasswordResetRecoverSmsWorker.LogFilePath
            System.IO.File.AppendAllText(log, String.Format("{0}:{1}{2}", Date.Now, message, vbCrLf))
        Catch ex As Exception
        End Try

    End Sub

#End Region

#Region "Public Method"
    Friend Shared Function SendPassCode(phoneNumber As String) As Boolean
        FileLog($"パスコード取得")

        GetLogFilePath(HttpContext.Current.Server.MapPath("~/App_Data/log"), "RecoverSms")

        Dim userHostAddress As String = HttpContext.Current.Request.UserHostAddress
        Dim userHostName As String = HttpContext.Current.Request.UserHostName
        Dim userAgent As String = HttpContext.Current.Request.UserAgent
        FileLog($"パスコード取得")

        With PasswordResetRecoverSmsWorker.ExecutePasswordResetRecoverWriteApi(phoneNumber, userHostAddress, userHostName, userAgent)
            FileLog($"{ .IsSuccess.TryToValueType(False)}")

            If .IsSuccess.TryToValueType(False) Then
#If DEBUG Then

                FileLog($"パスコード発行 PassCode: { .PassCode},Expires: { .Expires.TryToValueType(Date.MinValue)},Host: {userHostAddress}")
#Else
                Dim sb As New StringBuilder
                sb.AppendLine($"【JOTO】パスコードは { .PassCode}")
                sb.AppendLine($"有効期限は{ .Expires.TryToValueType(Date.MinValue).ToString("yyyy/MM/dd hh:mm")}です。")

                'phoneNumberローカル開発中は飛ばすようにしておく
                Dim args As New MessagesApiArgs(phoneNumber, sb.ToString())
                Dim results As MessagesApiResults = New MessagesApiResults() With {.IsSuccess = False}
                results = QsSmsApiManager.Execute(Of MessagesApiArgs, MessagesApiResults)(args)

                If results.IsSuccess Then

                    '送信成功
                    FileLog($"パスコード発行 PassCode: { .PassCode},Expires: { .Expires.TryToValueType(Date.MinValue)},Host: {userHostAddress}")
                    Try
                        PasswordResetRecoverSmsWorker.ExecuteSmsSendWriteApi(Guid.Empty, phoneNumber, QjApiSmsSendTypeEnum.PasswordReset, sb.ToString(), String.Empty)
                    Catch ex As Exception
                    End Try

                End If
#End If
            End If

        End With

        Return True

    End Function

    Friend Shared Function PassWordReset(phoneNumber As String, passCode As String, ByRef message As String, ByRef used As Boolean) As Boolean

        GetLogFilePath(HttpContext.Current.Server.MapPath("~/App_Data/log"), "RecoverSms")
        Dim actionDate As Date = Date.Now

        Dim userHostAddress As String = HttpContext.Current.Request.UserHostAddress
        Dim userHostName As String = HttpContext.Current.Request.UserHostName
        Dim userAgent As String = HttpContext.Current.Request.UserAgent

        Dim phone As String = String.Empty
        '電話番号の暗号化を解除
        Using resource As New QsCrypt(QsCryptTypeEnum.QolmsWeb)
            phone = resource.DecryptString(phoneNumber)
        End Using

        'パスコード認証
        With PasswordResetRecoverSmsWorker.ExecutePasswordResetRecoverReadApi(phone, passCode)

            If .IsVerification.TryToValueType(False) Then
                If .AccountKey.TryToValueType(Guid.Empty) <> Guid.Empty AndAlso .Expires.TryToValueType(Date.MinValue) > actionDate Then

                    Dim passwordResult As QiQolmsJotoSmsPasswordResetWriteApiResults = PasswordResetRecoverSmsWorker.ExecutePasswordResetWriteApi(.AccountKey.TryToValueType(Guid.Empty), phone, userHostAddress, userHostName, userAgent)

                    If passwordResult.IsSuccess.TryToValueType(False) Then
#If DEBUG Then
                        FileLog($"仮パスワード発行 Accountkey: { .AccountKey},Host: {userHostAddress}")
                        Return True
#Else
                        Dim sb As New StringBuilder
                        sb.AppendLine($"【JOTO】仮パスワードは {passwordResult.TemporaryPassword }")
                        'SMS送信
                        Dim args As New MessagesApiArgs(phone, sb.ToString())
                        Dim results As MessagesApiResults = New MessagesApiResults() With {.IsSuccess = False}
                        results = QsSmsApiManager.Execute(Of MessagesApiArgs, MessagesApiResults)(args)

                        If results.IsSuccess Then

                            '送信成功
                            FileLog($"仮パスワード発行 Accountkey: { .AccountKey},Host: {userHostAddress}")
                            Try
                                PasswordResetRecoverSmsWorker.ExecuteSmsSendWriteApi(.AccountKey.TryToValueType(Guid.Empty), phone, QjApiSmsSendTypeEnum.TemporaryPassword, sb.ToString(), String.Empty)
                            Catch ex As Exception
                            End Try

                            Return True

                        End If
#End If

                    End If
                End If
            Else
                '認証に失敗(todo:認証回数の上限確認）

                If RetryCount >= .UsedCount.TryToValueType(Byte.MinValue) Then

                    message = .Message
                Else
                    message = "リトライ回数の上限になりました。再度パスコードを発行してください"
                    used = True
                End If
                Return False

            End If

        End With

        '認証失敗以外はすべて成功で返す
        Return True

    End Function

    '''' <summary>
    '''' メールアドレスにURLを送信
    '''' </summary>
    '''' <returns></returns>
    'Public Shared Function SendUrl(mailaddress As String) As Boolean

    '    Dim userHostAddress As String = HttpContext.Current.Request.UserHostAddress
    '    Dim userHostName As String = HttpContext.Current.Request.UserHostName
    '    Dim userAgent As String = HttpContext.Current.Request.UserAgent

    '    '入力されたメールアドレスを登録する
    '    With PasswordResetRecoverSMSWorker.ExecutePasswordResetRecoverWriteApi(mailaddress, userHostAddress, userHostName, userAgent)
    '        If .IsSuccess.TryToValueType(False) Then

    '            'ほんとはIdentityから返ってくるよ
    '            Dim resetkey As Guid = .PasswordResetKey.TryToValueType(Guid.Empty)
    '            Dim expires As Date = .Expires.TryToValueType(Date.MinValue)

    '            If resetkey <> Guid.Empty AndAlso expires > Date.MinValue Then

    '                'Token作成
    '                '複合化してJSON形式へ
    '                Dim json As New PasswordResetRecoveryIdentifierTokenJsonParameter() With {
    '                    .PasswordResetkey = resetkey.ToString(),
    '                    .Expires = expires.ToString()
    '                }

    '                Dim crptJson As String = String.Empty
    '                Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsWeb)

    '                    Dim sr As New QsJsonSerializer()
    '                    crptJson = crypt.EncryptString(sr.Serialize(json))

    '                End Using

    '                Dim url As String = $"{CreateReturnUrl("PasswordReset/AppLink")}?token={ crptJson }"

    '                'メールテンプレートを取得
    '                Dim sb As New StringBuilder()
    '                Dim bodyFile As String = HttpContext.Current.Server.MapPath("~/App_Data/MailBodyPasswordRestUrl.txt")
    '                If Not String.IsNullOrWhiteSpace(bodyFile) Then
    '                    sb.AppendLine(IO.File.ReadAllText(bodyFile))
    '                End If

    '                Dim footerFile As String = HttpContext.Current.Server.MapPath("~/App_Data/MailFooter.txt")
    '                If Not String.IsNullOrWhiteSpace(footerFile) Then
    '                    sb.AppendLine(IO.File.ReadAllText(footerFile))
    '                End If

    '                Dim mailbody As String = String.Format(sb.ToString(), mailaddress, url)
    '                '// メール送信
    '                NoticeMailWorker.SendUser(mailaddress, PasswordResetRecoverSMSWorker.MAIL_SUBJECT, mailbody)

    '                '// 成功
    '                Return True

    '            End If

    '        End If
    '    End With

    '    Return False

    'End Function

    'Public Shared Function CreateReturnUrl(str As String) As String

    '    Dim root As String = ConfigurationManager.AppSettings("QolmsYappliSiteUri")

    '    If Not root.EndsWith("/") Then
    '        root += "/"
    '    End If

    '    Dim url As String = root + str

    '    Return url

    'End Function

#End Region

End Class
