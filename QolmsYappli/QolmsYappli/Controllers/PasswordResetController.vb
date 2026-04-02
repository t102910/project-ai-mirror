
Public NotInheritable Class PasswordResetController
    Inherits QyMvcControllerBase

    
#Region "Constructor"

    ''' <summary>
    ''' <see cref="PasswordResetController" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region

#Region "ページ ビュー アクション"
    
#Region "「メールアドレス入力」画面"
    
    <HttpGet()>
    <QyLogging()>
    Public Function Recover() As ActionResult

        Return View()
    End Function
        
    <HttpPost()>
    <QyActionMethodSelector("Send")>
    <QyLogging()>
    Public Function RecoverResult(model As PasswordResetRecoverInputModel) As JsonResult

        If Me.ModelState.IsValid Then
                    
            If PasswordResetRecoverWorker.SendUrl(model.MailAddress) Then
            '成功なら完了画面を表示
                Return New PasswordResetRecoverJsonResult()With{
                    .IsSuccess = Boolean.TrueString
                }.ToJsonResult()
            End If

        End If        

        Dim messages As New Dictionary(Of String,String)()
      
        For Each key As String In Me.ModelState.Keys
            For Each e As ModelError In Me.ModelState(key).Errors
                If messages.ContainsKey(key) Then
                    messages(key) = $"{messages(key)}{Environment.NewLine}{HttpUtility.HtmlEncode(e.ErrorMessage)}"
                Else
                    messages.Add(key, HttpUtility.HtmlEncode(e.ErrorMessage))

                End If
            Next
        Next

        '検証の結果を返却
        Return New PasswordResetRecoverJsonResult()With{
                    .IsSuccess = Boolean.FalseString,
                    .Messages = messages
                }.ToJsonResult()

    End Function
        
    <HttpPost()>
    <QyActionMethodSelector("Success")>
    <QyLogging()>
    Public Function RecoverResult() As ActionResult

        Return PartialView("_PasswordResetRecoverSuccessPartialView")
        
    End Function

#End Region
    
#Region "「本人確認」画面"
    
    <HttpGet()>
    <QyLogging()>
    Public Function RecoveryIdentifier(token As String) As ActionResult

        Dim inputModel As New PasswordResetRecoveryIdentifierInputModel()
        If not String.IsNullOrWhiteSpace(token) Andalso PasswordResetRecoveryIdentifierWorker.IsValidToken(token,InputModel) Then
            
            Return View(inputModel)

        End If
           
        Me.SetErrorMessage("リンクを確認してください。")

        Throw New InvalidOperationException("リンクを確認してください。")

    End Function
        
    <HttpPost()>
    <QyActionMethodSelector("Identifier")>
    <QyLogging()>
    Public Function RecoveryIdentifierResult(model As PasswordResetRecoveryIdentifierInputModel) As ActionResult

        If Me.ModelState.IsValid Then
                    
            If PasswordResetRecoveryIdentifierWorker.Identifier(model) Then
                '//完了
                '検証の結果を返却
                Return New PasswordResetRecoveryIdentifierJsonResult()With{
                        .IsSuccess = Boolean.TrueString
                    }.ToJsonResult()

            Else
                
                '//失敗でも画面には成功と同じように返す

                Return New PasswordResetRecoveryIdentifierJsonResult()With{
                        .IsSuccess = Boolean.TrueString
                    }.ToJsonResult()

            End If
        End If
                
        Dim messages As New Dictionary(Of String,String)()
      
        For Each key As String In Me.ModelState.Keys
            For Each e As ModelError In Me.ModelState(key).Errors
                If messages.ContainsKey(key) Then
                    messages(key) = $"{messages(key)}{Environment.NewLine}{HttpUtility.HtmlEncode(e.ErrorMessage)}"
                Else
                    messages.Add(key, HttpUtility.HtmlEncode(e.ErrorMessage))

                End If
            Next
        Next

        '検証の結果を返却
        Return New PasswordResetRecoveryIdentifierJsonResult()With{
                    .IsSuccess = Boolean.FalseString,
                    .Messages = messages
                }.ToJsonResult()

    End Function

            
    <HttpPost()>
    <QyActionMethodSelector("Completion")>
    <QyLogging()>
    Public Function RecoveryIdentifierResult() As ActionResult

        Return PartialView("_PasswordResetRecoveryIdentifierCompletionPartialView")
        
    End Function

#End Region

#Region "「ダイナミックリンク」画面"
    
    <HttpGet()>
    <QyLogging()>
    Public Function AppLink(token As String) As ActionResult

        dim useragent As String = Me.HttpContext.Request.UserAgent
        ViewData("UserAgent") = useragent

        Dim url As String = $"{PasswordResetRecoverWorker.CreateReturnUrl("PasswordReset/RecoveryIdentifier")}?token={ token }"

        Dim jotodeeplink As String = ConfigurationManager.AppSettings("DeepLinkWebView")
        Dim deeplink As String = If(String.IsNullOrWhiteSpace(jotodeeplink), "jotohdr:/tab/custom/5d67c170", jotodeeplink)
        Dim query As String = $"url={HttpUtility.UrlEncode(url)}"
        'URLパラメータを作成してメール送信
        'iOS の例: {deeplink}://?goto=login
        'Android の例: intent://?goto=login#Intent;scheme={deeplink};package={deeplink};S.browser_fallback_url=?goto=login;end";

        Dim iosUrl As String = $"{deeplink}?{query}"
        Dim androidUrl As String = $"intent://?{query}#Intent;scheme={deeplink};package={deeplink};S.browser_fallback_url=?{query};end"";"
                
        ViewData("AppLink") = iosUrl

        'Select True
        '    Case useragent.Contains("iPhone")
        '        ViewData("AppLink") = iosUrl
        '    Case useragent.Contains("Android")
        '        ViewData("AppLink") = androidUrl
        '    Case Else
        '        ViewData("AppLink") = ""

        'End Select
        Return View()

    End Function
#End Region

#Region "SMS"

    '<QyLogging()>

    <HttpGet()>
    Public Function RecoverSMS() As ActionResult
        'セッション残らないように先に削除
        QySessionHelper.RemoveItem(Me.Session, "PhoneNumber")

        Return View()
    End Function

    <HttpPost()>
    <QyActionMethodSelector("Send")>
    Public Function RecoverSMSResult(model As PasswordResetRecoverSMSInputModel) As JsonResult

        If Me.ModelState.IsValid Then
            If PasswordResetRecoverSmsWorker.SendPassCode(model.PhoneNumber) Then

            End If

            'セッションに電話番号保存しとく
            QySessionHelper.SetItem(Me.Session, "PhoneNumber", model.PhoneNumber)

            '成功ならパスコード画面へリダイレクト(JS)
            Return New PasswordResetRecoverSMSJsonResult() With {
                .IsSuccess = Boolean.TrueString
            }.ToJsonResult()
            'End If

        End If

        Dim messages As New Dictionary(Of String, String)()

        For Each key As String In Me.ModelState.Keys
            For Each e As ModelError In Me.ModelState(key).Errors
                If messages.ContainsKey(key) Then
                    messages(key) = $"{messages(key)}{Environment.NewLine}{HttpUtility.HtmlEncode(e.ErrorMessage)}"
                Else
                    messages.Add(key, HttpUtility.HtmlEncode(e.ErrorMessage))

                End If
            Next
        Next

        '検証の結果を返却
        Return New PasswordResetRecoverSMSJsonResult() With {
                    .IsSuccess = Boolean.FalseString,
                    .Messages = messages
                }.ToJsonResult()

    End Function

    '<HttpPost()>
    '<QyActionMethodSelector("Success")>
    'Public Function RecoverSMSResult(PhoneNumber As String) As ActionResult

    '    Dim model As New PasswordResetRecoverSMSInputModel()
    '    QySessionHelper.GetItem(Of PasswordResetRecoverSMSInputModel)(Me.Session, "PasswordResetSms", model)

    '    Dim str As String = String.Empty
    '    If Not String.IsNullOrWhiteSpace(PhoneNumber) Then
    '        Using resource As New QolmsCryptV1.QsCrypt(QolmsCryptV1.QsCryptTypeEnum.QolmsWeb)
    '            str = resource.EncryptString(PhoneNumber)
    '        End Using
    '    End If

    '    Dim disp As String = PhoneNumber.Substring((PhoneNumber.Length - 4), 4).PadLeft(PhoneNumber.Length, "*"c)
    '    Dim viewModel As New PasswordResetRecoverSMSPassCodeViewModel() With {
    '        .CryptPhoneNumber = str,
    '        .DispPhoneNumber = disp}
    '    Return PartialView("_PasswordResetRecoverSMSPassCodePartialView", viewModel)

    'End Function



    '<QyLogging()>

    <HttpGet()>
    Public Function RecoverSmsPasscode() As ActionResult

        'セッション電話番号
        Dim phoneNumber As String = String.Empty
        QySessionHelper.GetItem(Of String)(Me.Session, "PhoneNumber", phoneNumber)
        Dim str As String = String.Empty
        If Not String.IsNullOrWhiteSpace(phoneNumber) Then
            Using resource As New QolmsCryptV1.QsCrypt(QolmsCryptV1.QsCryptTypeEnum.QolmsWeb)
                str = resource.EncryptString(phoneNumber)
            End Using

            Dim disp As String = phoneNumber.Substring((phoneNumber.Length - 4), 4).PadLeft(phoneNumber.Length, "*"c)
            Dim viewModel As New PasswordResetRecoverSMSPassCodeViewModel() With {
                .CryptPhoneNumber = str,
                .DispPhoneNumber = disp}

            Return View(viewModel)
        End If

        Return RedirectToAction("RecoverSMS")

    End Function

    <HttpPost()>
    <QyActionMethodSelector("PassCode")>
    Public Function RecoverSmsPasscodeResult(PhoneNumber As String, PassCode As String) As ActionResult

        Dim message As String = String.Empty
        Dim used As Boolean = False
        If PasswordResetRecoverSmsWorker.PassWordReset(PhoneNumber, PassCode, message, used) Then
            '成功なら完了画面を表示
            '成功したらセッションの電話番号も削除
            QySessionHelper.RemoveItem(Me.Session, "PhoneNumber")

            Return PartialView("_PasswordResetRecoverySMSCompletionPartialView")

        End If

        Return New PasswordResetRecoverSMSJsonResult() With {
            .IsSuccess = Boolean.FalseString,
            .Messages = New Dictionary(Of String, String)() From {{"model.Passcode", message}},
            .PassDisabled = If(used, Boolean.TrueString, Boolean.FalseString)
        }.ToJsonResult()
        ''成功時も失敗時も画面は成功
        'Return PartialView("_PasswordResetRecoverySMSCompletionPartialView")

        'Return View()
    End Function
#End Region

#Region "リセット方法の選択画面"

    <HttpGet()>
    Public Function SelectMethod() As ActionResult
        Return View()
    End Function

#End Region

#End Region

#Region "パーシャル ビュー アクション"

#Region "共通パーツ"

    '''' <summary>
    '''' 「ヘッダー」パーシャル ビューの表示要求を処理します。
    '''' </summary>
    '''' <returns>
    '''' アクションの結果。
    '''' </returns>
    '''' <remarks></remarks>
    '<ChildActionOnly()>
    'Public Function PasswordResetHeaderPartialView() As ActionResult

    '    ' パーシャル ビューを返却
    '    Return PartialView("_PasswordResetHeaderPartialView")

    'End Function

    ''' <summary>
    ''' 「フッター」パーシャル ビューの表示要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクションの結果。
    ''' </returns>
    ''' <remarks></remarks>
    <ChildActionOnly()>
    <OutputCache(Duration:=600)>
    Public Function PasswordResetFooterPartialView() As ActionResult

        ' パーシャル ビューを返却
        Return PartialView("_PasswordResetFooterPartialView")

    End Function

#End Region
#End Region

End Class
