Imports System.Threading.Tasks
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsCryptV1
Imports MGF.QOLMS.QolmsDbEntityV1

Friend NotInheritable Class PasswordResetRecoverWorker

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

    Const Private MAIL_SUBJECT As String = "【JOTOホームドクター】パスワードリセット用URL"

    const DEFALT_EXPIRES_MINUTES As Integer = 30

    Private Shared ReadOnly SETTING_EXPIRES_MINUTES As String = New Lazy(Of String)(Function() GetConfigSettings("PasswordResetUrlExpiresMinutes")).Value

#End Region

#Region "Constructor"

    ''' <summary>
    ''' デフォルト コンストラクタは使用できません。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub

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

    Private Shared Function ExecutePasswordResetRecoverWriteApi(mailaddress As String,userHostAddress As String,userHostName As String,userAgent As String) As QiQolmsJotoPasswordResetTokenWriteApiResults

        Dim apiArgs As New QiQolmsJotoPasswordResetTokenWriteApiArgs(
            QiApiTypeEnum.QolmsJotoPasswordResetTokenWrite,
            QsApiSystemTypeEnum.Qolms,
            Guid.Empty,
            String.Empty
        ) With {
            .MailAddress = mailaddress,
            .UserHostAddress = userHostAddress,
            .UserHostName = userHostName,
            .UserAgent = userAgent,
            .ExpiresMinutes = If(String.IsNullOrEmpty(SETTING_EXPIRES_MINUTES),DEFALT_EXPIRES_MINUTES.ToString(),SETTING_EXPIRES_MINUTES)
        }
        Dim apiResults As QiQolmsJotoPasswordResetTokenWriteApiResults = QsApiManager.ExecuteQolmsIdentityApi(Of QiQolmsJotoPasswordResetTokenWriteApiResults)(
            apiArgs,
            PasswordResetRecoverWorker.DUMMY_SESSION_ID,
            PasswordResetRecoverWorker.DUMMY_API_AUTHORIZE_KEY
        )
        If apiResults.IsSuccess.TryToValueType(False) Then
            
            Return apiResults
        
        Else

             Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsIdentityApiName(apiArgs)))
        End If

    End Function
#End Region

#Region "Public Method"

    ''' <summary>
    ''' メールアドレスにURLを送信
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function SendUrl(mailaddress As String) As Boolean

        Dim userHostAddress As String = HttpContext.Current.Request.UserHostAddress
        Dim userHostName As String = HttpContext.Current.Request.UserHostName
        Dim userAgent As String = HttpContext.Current.Request.UserAgent

        '入力されたメールアドレスを登録する
        With PasswordResetRecoverWorker.ExecutePasswordResetRecoverWriteApi(mailaddress,userHostAddress,userHostName,userAgent)
            If .IsSuccess.TryToValueType(False) Then

                Dim resetkey As Guid = .PasswordResetKey.TryToValueType(Guid.Empty)
                Dim expires As Date = .Expires.TryToValueType(Date.MinValue)

                If resetkey <> Guid.Empty AndAlso expires > Date.MinValue Then

                    'Token作成
                    '複合化してJSON形式へ
                    Dim json As New PasswordResetRecoveryIdentifierTokenJsonParameter() With {
                        .PasswordResetkey = resetkey.ToString(),
                        .Expires = expires.ToString()
                    }

                    Dim crptJson As String = String.Empty
                    Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsWeb)

                        Dim sr As New QsJsonSerializer()
                        crptJson = crypt.EncryptString(sr.Serialize(json))

                    End Using
           
                    Dim url As String = $"{CreateReturnUrl("PasswordReset/AppLink")}?token={ crptJson }"

                    'メールテンプレートを取得
                    Dim sb As New StringBuilder()
                    Dim bodyFile As String = HttpContext.Current.Server.MapPath("~/App_Data/MailBodyPasswordRestUrl.txt")
                    If Not String.IsNullOrWhiteSpace(bodyFile) Then
                        sb.AppendLine(IO.File.ReadAllText(bodyFile))
                    End If

                    Dim footerFile As String = HttpContext.Current.Server.MapPath("~/App_Data/MailFooter.txt")
                    If Not String.IsNullOrWhiteSpace(footerFile) Then
                        sb.AppendLine(IO.File.ReadAllText(footerFile))
                    End If

                    Dim mailbody As String = String.Format(sb.ToString(), mailaddress, url)
                    '// メール送信
                    NoticeMailWorker.SendUser(mailaddress, PasswordResetRecoverWorker.MAIL_SUBJECT, mailbody)

                    '// 成功
                    Return True

                End If

            End If
        End With

        Return False

    End Function

    Public Shared Function CreateReturnUrl(str As String) As String

        Dim root As String = ConfigurationManager.AppSettings("QolmsYappliSiteUri")

        If Not root.EndsWith("/") Then
            root += "/"
        End If

        Dim url As String = root + str

        Return url

    End Function

#End Region

End Class
