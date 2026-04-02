Imports MGF.QOLMS.QolmsCryptV1
Imports MGF.QOLMS.QolmsApiCoreV1

''' <summary>
''' 「コルムス ヤプリ サイト」で使用する、
''' クッキーに関する補助機能を提供します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class QyCookieHelper

#Region "Constant"

    ''' <summary>
    ''' 認証チケット名を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const AUTH_TICKET_NAME As String = "QolmsYappliAuthTicket"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Private Const REMEMBER_ID_COOKIE_NAME As String = "Mgf.Qolms.QolmsYappliRememberId"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Private Const REMEMBER_LOGIN_COOKIE_NAME As String = "Mgf.Qolms.QolmsYappliRememberLogin"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Private Const AGREEMENT_VERSION_COOKIE_NAME As String = "Mgf.Qolms.QolmsYappliAgreementVersion"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Private Const AUTO_AUIDLOGIN_COOKIE_NAME As String = "Mgf.Qolms.QolmsYappliAutoAuIdLogin"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Private Const ENTRY_APPLEIDLOGIN_COOKIE_NAME As String = "Mgf.Qolms.QolmsYappliEntryAppleIdLogin"

#End Region

#Region "Constructor"

    ''' <summary>
    ''' デフォルト コンストラクタは使用できません。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub

#End Region

#Region "Public Method"

    ''' <summary>
    ''' 認証クッキーを設定します。
    ''' </summary>
    ''' <param name="response">HTTP 応答。</param>
    ''' <param name="userId">ユーザー ID。</param>
    ''' <param name="timeout">セッション タイムアウトまでの時間（分）。</param>
    ''' <remarks></remarks>
    Public Shared Sub SetFormsAuthCookie(response As HttpResponseBase, userId As String, timeout As Integer)

        ' TODO: 引数のチェック

        ' TODO: 要検討、認証チケットを作成
        Dim ticket As New FormsAuthenticationTicket(
            1,
            QyCookieHelper.AUTH_TICKET_NAME,
            Date.Now(),
            Date.Now.AddMinutes(timeout),
            False,
            New AuthTicketJsonParameter() With {.UserId = userId}.ToJsonString()
        )
        Dim cookie As New HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket))

        With cookie
            ' 有効期限
            .Expires = DateTime.Now.AddMinutes(timeout)

            ' パス
            .Path = FormsAuthentication.FormsCookiePath

            ' クライアントスクリプトからのアクセスを許可しない
            .HttpOnly = True
        End With

        ' クッキーに追加
        response.Cookies.Add(cookie)

    End Sub

    ''' <summary>
    ''' 認証クッキーの有効性をチェックします。
    ''' </summary>
    ''' <param name="request">HTTP 要求。</param>
    ''' <param name="userId">ユーザー ID。</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CheckFormsAuthCookie(request As HttpRequestBase, userId As String) As Boolean

        ' TODO: 引数のチェック

        Dim result As Boolean = False
        Dim cookie As HttpCookie = request.Cookies(FormsAuthentication.FormsCookieName)

        If cookie IsNot Nothing Then
            Try
                ' 認証チケットの確認
                Dim ticket As FormsAuthenticationTicket = FormsAuthentication.Decrypt(cookie.Value)

                result = String.Compare(ticket.Name, QyCookieHelper.AUTH_TICKET_NAME, True) = 0 _
                    AndAlso String.Compare(QyJsonParameterBase.FromJsonString(Of AuthTicketJsonParameter)(ticket.UserData).UserId, userId, True) = 0
            Catch
            End Try
        End If

        Return result

    End Function

    Public Shared Function SetRememberIdCookie(response As HttpResponseBase, remember As Boolean, userId As String, loginAt As Date) As Boolean

        Dim value As String = String.Empty

        If remember AndAlso Not String.IsNullOrWhiteSpace(userId) AndAlso loginAt <> Date.MinValue Then
            Try
                Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsWeb)
                    value = crypt.EncryptString(
                        New RememberIdCookieJsonParameter() With {
                            .UserId = userId,
                            .LoginAt = loginAt.ToApiDateString()
                        }.ToJsonString()
                    )
                End Using
            Catch
            End Try
        End If

        Dim cookie As New HttpCookie(QyCookieHelper.REMEMBER_ID_COOKIE_NAME, value)

        With cookie
            ' 有効期限
            .Expires = loginAt.AddYears(1).AddDays(-1) ' TODO:

            ' パス
            If FormsAuthentication.FormsCookiePath.EndsWith("/") Then
                .Path = String.Format("{0}{1}", FormsAuthentication.FormsCookiePath, "Start/LoginById")
            Else
                .Path = String.Format("{0}{1}", FormsAuthentication.FormsCookiePath, "/Start/LoginById")
            End If

            ' クライアントスクリプトからのアクセスを許可しない
            .HttpOnly = True
        End With

        ' クッキーに追加
        response.Cookies.Add(cookie)

        Return Not String.IsNullOrWhiteSpace(value)

    End Function

    Public Shared Function GetRememberIdCookie(request As HttpRequestBase) As RememberIdCookieJsonParameter

        Dim result As RememberIdCookieJsonParameter = Nothing
        Dim cookie As HttpCookie = request.Cookies(QyCookieHelper.REMEMBER_ID_COOKIE_NAME)

        If cookie IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(cookie.Value) Then
            Try
                Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsWeb)
                    result = QyJsonParameterBase.FromJsonString(Of RememberIdCookieJsonParameter)(crypt.DecryptString(cookie.Value))
                End Using
            Catch
            End Try
        End If

        Return result

    End Function

    Public Shared Function DisableRememberIdCookie(response As HttpResponseBase) As Boolean

        Return Not QyCookieHelper.SetRememberIdCookie(response, False, String.Empty, Date.MinValue)

    End Function

    Public Shared Function SetRememberLoginCookie(response As HttpResponseBase, remember As Boolean, userId As String, passwordHash As String, loginAt As Date,expires As date) As Boolean

        Dim value As String = String.Empty

        If remember AndAlso Not String.IsNullOrWhiteSpace(userId) AndAlso Not String.IsNullOrWhiteSpace(passwordHash) AndAlso loginAt <> Date.MinValue Then
            Try
                Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsWeb)
                    value = crypt.EncryptString(
                        New RememberLoginCookieJsonParameter() With {
                            .UserId = userId,
                            .PasswordHash = passwordHash,
                            .LoginAt = loginAt.ToApiDateString(),
                            .Expires = Expires.ToApiDateString()
                        }.ToJsonString()
                    )
                End Using
            Catch
            End Try
        End If

        Dim cookie As New HttpCookie(QyCookieHelper.REMEMBER_LOGIN_COOKIE_NAME, value)

        With cookie
            ' 有効期限
            .Expires = loginAt.AddYears(1).AddDays(-1) ' TODO:

            ' パス
            If FormsAuthentication.FormsCookiePath.EndsWith("/") Then
                .Path = String.Format("{0}{1}", FormsAuthentication.FormsCookiePath, "Start/LoginById")
            Else
                .Path = String.Format("{0}{1}", FormsAuthentication.FormsCookiePath, "/Start/LoginById")
            End If

            ' クライアントスクリプトからのアクセスを許可しない
            .HttpOnly = True
        End With

        ' クッキーに追加
        response.Cookies.Add(cookie)

        Return Not String.IsNullOrWhiteSpace(value)

    End Function

    Public Shared Function GetRememberLoginCookie(request As HttpRequestBase) As RememberLoginCookieJsonParameter

        Dim result As RememberLoginCookieJsonParameter = Nothing
        Dim cookie As HttpCookie = request.Cookies(QyCookieHelper.REMEMBER_LOGIN_COOKIE_NAME)

        If cookie IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(cookie.Value) Then
            Try
                Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsWeb)
                    result = QyJsonParameterBase.FromJsonString(Of RememberLoginCookieJsonParameter)(crypt.DecryptString(cookie.Value))
                End Using
            Catch
            End Try
        End If

        Return result

    End Function

    Public Shared Function DisableRememberLoginCookie(response As HttpResponseBase) As Boolean

        Return Not QyCookieHelper.SetRememberLoginCookie(response, False, String.Empty, String.Empty, Date.MinValue,Date.MinValue)

    End Function


    Public Shared Function SetAgreementVersionCookie(response As HttpResponseBase, AgreementVersion As String) As Boolean

        Dim value As String = String.Empty
        value = AgreementVersion

        Dim cookie As New HttpCookie(QyCookieHelper.AGREEMENT_VERSION_COOKIE_NAME, value)

        With cookie
            ' 有効期限
            .Expires = Date.MaxValue

            ' パス
            If FormsAuthentication.FormsCookiePath.EndsWith("/") Then
                .Path = String.Format("{0}{1}", FormsAuthentication.FormsCookiePath, "Start/")
            Else
                .Path = String.Format("{0}{1}", FormsAuthentication.FormsCookiePath, "/Start/")
            End If

            ' クライアントスクリプトからのアクセスを許可しない
            .HttpOnly = True
        End With

        ' クッキーに追加
        response.Cookies.Add(cookie)

        Return Not String.IsNullOrWhiteSpace(value)

    End Function

    Public Shared Function GetAgreementVersionCookie(request As HttpRequestBase) As String

        Dim cookie As HttpCookie = request.Cookies(QyCookieHelper.AGREEMENT_VERSION_COOKIE_NAME)

        If cookie IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(cookie.Value) Then

            Return cookie.Value
        End If

        Return String.Empty

    End Function

    Public Shared Function DisableAgreementVersionCookie(response As HttpResponseBase) As Boolean

        Return Not QyCookieHelper.SetAgreementVersionCookie(response, String.Empty)

    End Function

    Public Shared Function SetAutoAuIdLoginCookie(response As HttpResponseBase, LoginType As String, Expires As Date) As Boolean

        Dim value As String = String.Empty
        value = New AuIdLoginCookieJsonParameter() With {
                            .LoginType = LoginType,
                            .Expires = Expires.ToApiDateString()
                        }.ToJsonString()

        Dim cookie As New HttpCookie(QyCookieHelper.AUTO_AUIDLOGIN_COOKIE_NAME, value)

        With cookie
            ' 有効期限
            .Expires = Date.MaxValue

            ' パス
            If FormsAuthentication.FormsCookiePath.EndsWith("/") Then
                .Path = String.Format("{0}{1}", FormsAuthentication.FormsCookiePath, "Start/")
            Else
                .Path = String.Format("{0}{1}", FormsAuthentication.FormsCookiePath, "/Start/")
            End If

            ' クライアントスクリプトからのアクセスを許可しない
            .HttpOnly = True
        End With

        ' クッキーに追加
        response.Cookies.Add(cookie)

        Return Not String.IsNullOrWhiteSpace(value)

    End Function

    Public Shared Function GetAutoAuIdLoginCookie(request As HttpRequestBase) As AuIdLoginCookieJsonParameter

        Dim result As AuIdLoginCookieJsonParameter = Nothing
        Dim cookie As HttpCookie = request.Cookies(QyCookieHelper.AUTO_AUIDLOGIN_COOKIE_NAME)

        If cookie IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(cookie.Value) Then

            Try
                result = QyJsonParameterBase.FromJsonString(Of AuIdLoginCookieJsonParameter)(cookie.Value)

            Catch
            End Try

            Return result
        End If

        Return result

    End Function

    Public Shared Function DisableAutoAuIdLoginCookie(response As HttpResponseBase) As Boolean

        Return Not QyCookieHelper.SetAutoAuIdLoginCookie(response, String.Empty, Date.MinValue)

    End Function

    Public Shared Function SetAppleIdEntryCookie(response As HttpResponseBase, first As String, last As String) As Boolean

        '有効期限　固定半年
        Dim Expires As Date = Date.Now.AddMonths(6)

        Dim value As String = String.Empty
        Try
            Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsWeb)
                value = crypt.EncryptString(
                        New AppleIdEntryCookieJsonParameter() With {
                            .FirstName = first,
                            .LastName = last,
                            .Expires = Expires.ToApiDateString()
                        }.ToJsonString()
                    )
            End Using
        Catch
        End Try

        Dim cookie As New HttpCookie(QyCookieHelper.ENTRY_APPLEIDLOGIN_COOKIE_NAME, value)

        With cookie
            ' 有効期限
            .Expires = Expires

            ' パス
            If FormsAuthentication.FormsCookiePath.EndsWith("/") Then
                .Path = String.Format("{0}{1}", FormsAuthentication.FormsCookiePath, "Start/")
            Else
                .Path = String.Format("{0}{1}", FormsAuthentication.FormsCookiePath, "/Start/")
            End If

            ' クライアントスクリプトからのアクセスを許可しない
            .HttpOnly = True
        End With

        ' クッキーに追加
        response.Cookies.Add(cookie)

        Return Not String.IsNullOrWhiteSpace(value)

    End Function

    Public Shared Function GetAppleIdEntryCookie(request As HttpRequestBase) As AppleIdEntryCookieJsonParameter

        Dim result As AppleIdEntryCookieJsonParameter = Nothing
        Dim cookie As HttpCookie = request.Cookies(QyCookieHelper.ENTRY_APPLEIDLOGIN_COOKIE_NAME)

        If cookie IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(cookie.Value) Then

            Try
                Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsWeb)
                    result = QyJsonParameterBase.FromJsonString(Of AppleIdEntryCookieJsonParameter)(crypt.DecryptString(cookie.Value))
                End Using
            Catch
            End Try

            Return result
        End If

        Return result

    End Function

    Public Shared Function DisableAppleIdEntryCookie(response As HttpResponseBase) As Boolean

        Return Not QyCookieHelper.SetAutoAuIdLoginCookie(response, String.Empty, Date.MinValue)

    End Function

#End Region

End Class
