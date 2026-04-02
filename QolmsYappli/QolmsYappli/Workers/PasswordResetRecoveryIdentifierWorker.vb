Imports System.Threading.Tasks
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsCryptV1
Imports MGF.QOLMS.QolmsDbEntityV1

Friend NotInheritable Class PasswordResetRecoveryIdentifierWorker

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

    'Private ReadOnly MAIL_SUBJECT As String = If(String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings("PasswordResetRetryCountForMail")), "【JOTOホームドクター】パスワードリセット", ConfigurationManager.AppSettings("PasswordResetMailSubject"))

    'Private ReadOnly MAIL_RETRYCOUNT As String = ConfigurationManager.AppSettings("PasswordResetRetryCountForMail")

    'Private ReadOnly SMS_RETRYCOUNT As String = ConfigurationManager.AppSettings("PasswordResetRetryCountForSMS")

    Private Shared ReadOnly _mailSubject As New Lazy(Of String)(Function() ConfigurationManager.AppSettings("PasswordResetMailSubject"))

    Private Shared ReadOnly _mailRetryCount As New Lazy(Of String)(Function() ConfigurationManager.AppSettings("PasswordResetRetryCountForMail"))
    Private Shared ReadOnly _smsRetryCount As New Lazy(Of String)(Function() ConfigurationManager.AppSettings("PasswordResetRetryCountForSMS"))


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
    ''' メールのタイトルを取得します。
    ''' </summary>
    Public Shared ReadOnly Property MailSubject As String
        Get
            Dim result As String = String.Empty

            '設定があれば設定値、なければ初期値1
            If Not String.IsNullOrWhiteSpace(_mailSubject.Value) Then

                result = _mailSubject.Value
            Else
                result = "【JOTOホームドクター】パスワードリセット"
            End If

            Return result
        End Get

    End Property

    ''' <summary>
    ''' メールでのパスワードリセットのリトライカウントを取得します。
    ''' </summary>
    Public Shared ReadOnly Property MailRetryCount As Integer
        Get
            Dim result As Integer = Integer.MinValue

            '設定があれば設定値、なければ初期値1
            If Not String.IsNullOrWhiteSpace(_mailRetryCount.Value) AndAlso Integer.TryParse(_mailRetryCount.Value, result) Then

                result = If(result > 0, result, 1)
            Else
                result = 1
            End If

            Return result
        End Get

    End Property

    ''' <summary>
    ''' SMSでのパスワードリセットのリトライカウントを取得します。
    ''' </summary>
    Public Shared ReadOnly Property SMSRetryCount As Integer
        Get
            Dim result As Integer = Integer.MinValue

            '設定があれば設定値、なければ初期値1
            If Not String.IsNullOrWhiteSpace(_smsRetryCount.Value) AndAlso Integer.TryParse(_smsRetryCount.Value, result) Then

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

    Private Shared Function ExecutePasswordResetTokenValidApi(resetkey As Guid) As QiQolmsJotoPasswordResetTokenReadApiResults
                
        Dim apiArgs As New QiQolmsJotoPasswordResetTokenReadApiArgs(
            QiApiTypeEnum.QolmsJotoPasswordResetTokenRead,
            QsApiSystemTypeEnum.Qolms,
            Guid.Empty,
            String.Empty
        ) With {
            .PasswordResetKey = resetkey.ToApiGuidString()
        }
        Dim apiResults As QiQolmsJotoPasswordResetTokenReadApiResults = QsApiManager.ExecuteQolmsIdentityApi(Of QiQolmsJotoPasswordResetTokenReadApiResults)(
            apiArgs,
            PasswordResetRecoveryIdentifierWorker.DUMMY_SESSION_ID,
            PasswordResetRecoveryIdentifierWorker.DUMMY_API_AUTHORIZE_KEY
        )
        If apiResults.IsSuccess.TryToValueType(False) Then
            
            Return apiResults
        
        Else

             Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsIdentityApiName(apiArgs)))
        End If

    End Function

        
    Private Shared Function ExecutePasswordResetIdentifierApi(inputModel As PasswordResetRecoveryIdentifierInputModel,resetkey As Guid,birthday As Date) As QiQolmsJotoPasswordResetWriteApiResults
        
        Dim apiArgs As New QiQolmsJotoPasswordResetWriteApiArgs(
            QiApiTypeEnum.QolmsJotoPasswordResetWrite,
            QsApiSystemTypeEnum.Qolms,
            Guid.Empty,
            String.Empty
        ) With {
            .UserId = $"JOTO{inputModel.JotoId}",
            .FamilyName = inputModel.FamilyName,
            .GivenName = inputModel.GivenName,
            .SexType = Convert.ToByte(inputModel.Sex.TryToValueType(QySexTypeEnum.None)).ToString(),
            .Birthday = birthday.ToApiDateString(),
            .MailAddress = inputModel.MailAddress,
            .PasswordResetKey = resetkey.ToApiGuidString()
        }
        Dim apiResults As QiQolmsJotoPasswordResetWriteApiResults = QsApiManager.ExecuteQolmsIdentityApi(Of QiQolmsJotoPasswordResetWriteApiResults)(
            apiArgs,
            PasswordResetRecoveryIdentifierWorker.DUMMY_SESSION_ID,
            PasswordResetRecoveryIdentifierWorker.DUMMY_API_AUTHORIZE_KEY
        )
        If apiResults.IsSuccess.TryToValueType(False) Then
            
            Return apiResults
        
        Else

             Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsIdentityApiName(apiArgs)))
        End If
    End Function

#End Region

#Region "Public Method"

    Public Shared Function IsValidToken(token As String,ByRef inputModel As PasswordResetRecoveryIdentifierInputModel) As Boolean

        inputModel = New PasswordResetRecoveryIdentifierInputModel
        '複合化してJSON形式へ
        Dim json As New PasswordResetRecoveryIdentifierTokenJsonParameter()
        Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsWeb)

            Dim jsonStr As String = crypt.DecryptString(token)

            If not String.IsNullOrWhiteSpace(jsonStr) Then
                Dim sr As New QsJsonSerializer()
                json = sr.Deserialize(Of PasswordResetRecoveryIdentifierTokenJsonParameter)(jsonStr)

            End If
             
        End Using

        'tokenが有効か確認
        Dim resetkey As Guid = Guid.Empty
        Dim expires As Date = Date.MinValue
        If Guid.TryParse(json.PasswordResetkey, resetkey) AndAlso Date.TryParse(json.Expires, expires) Then 
            
            '//Identity処理
            With PasswordResetRecoveryIdentifierWorker.ExecutePasswordResetTokenValidApi(resetkey)
                Dim mailaddress As String = .MailAddress
                Dim accountkey As Guid = .AccountKey.TryToValueType(Guid.Empty)
                expires = .Expires.TryToValueType(Date.MinValue)

                '期限とカウントをチェックする
                Dim count As Integer = .UsedCount.TryToValueType(Integer.MinValue)
                If expires > Date.Now _
                    AndAlso count < PasswordResetRecoveryIdentifierWorker.MailRetryCount _
                    AndAlso accountkey = Guid.Empty Then

                    inputModel.PasswordResetKey = resetkey
                    inputModel.MailAddress = mailaddress
                    '//成功したらTrue
                    Return True

                End If
            End With

        End If

        Return False

    End Function

    ''' <summary>
    ''' 本人確認
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function Identifier(model As PasswordResetRecoveryIdentifierInputModel) As Boolean
        

        Dim resetkey As Guid = model.PasswordResetKey
        Dim birthday As Date = Date.ParseExact($"{model.BirthYear.TryToValueType(Integer.MinValue).ToString("d4")}{model.BirthMonth.TryToValueType(Integer.MinValue).ToString("d2")}{model.BirthDay.TryToValueType(Integer.MinValue).ToString("d2")}", "yyyyMMdd", Nothing, Globalization.DateTimeStyles.None)

        'Identityで本人確認、リセット
        with PasswordResetRecoveryIdentifierWorker.ExecutePasswordResetIdentifierApi(model,resetkey,birthday)
                    
            If .IsSuccess.TryToValueType(False) AndAlso .IsVerification.TryToValueType(False) Then'Issuccess
                        
                'URLパラメータを作成してメール送信
                Dim password As String = .TemporaryPassword
                'メールテンプレートを取得
                Dim sb As New StringBuilder()
                Dim bodyFile As String = HttpContext.Current.Server.MapPath("~/App_Data/MailBodyTemporaryPassword.txt")
                If Not String.IsNullOrWhiteSpace(bodyFile) Then
                    sb.AppendLine(IO.File.ReadAllText(bodyFile))
                End If

                Dim footerFile As String = HttpContext.Current.Server.MapPath("~/App_Data/MailFooter.txt")
                If Not String.IsNullOrWhiteSpace(footerFile) Then
                    sb.AppendLine(IO.File.ReadAllText(footerFile))
                End If

                Dim mailaddress As String = model.MailAddress
                Dim mailbody As String = String.Format(sb.ToString(), mailaddress, password)
                'リセットに成功したらメール送信
                NoticeMailWorker.SendUser(mailaddress, PasswordResetRecoveryIdentifierWorker.MailSubject, mailbody)

                Return True
            End If
        
        End With

        Return false

    End Function

#End Region

End Class
