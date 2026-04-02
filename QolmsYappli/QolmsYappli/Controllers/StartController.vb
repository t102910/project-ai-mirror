Imports System.Threading.Tasks
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsAppleAuthApiCore

Public NotInheritable Class StartController
    Inherits QyMvcControllerBase

#Region "Constant"

    ''' <summary>
    ''' ログインしたことを表すメッセージです。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const LOGIN_MESSAGE As String = "手動ログインしました。"

    ''' <summary>
    ''' 自動ログインしたことを表すメッセージです。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const AUTO_LOGIN_MESSAGE As String = "自動ログインしました。"

    ''' <summary>
    ''' ログアウトしたことを表すメッセージです。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const LOGOUT_MESSAGE As String = "ログアウトしました。"

    ''' <summary>
    ''' 不正なユーザーであることを表すエラーメッセージです。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const INVALID_USER_ERROR_MESSAGE As String = "不正なユーザーです。"

    ''' <summary>
    ''' ログインの再試行中であることを表すエラーメッセージです。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const RETRY_ERROR_MESSAGE As String = "ユーザーIDもしくはパスワードが不正です。"

    ''' <summary>
    ''' ロックダウン中であることを表すエラーメッセージです。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const LOCKDOWN_ERROR_MESSAGE As String = "アカウントはロック中です。"

    ''' <summary>
    ''' 予期せぬエラーが発生したことを表すエラーメッセージです。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const UNEXPECTED_ERROR_MESSAGE As String = "予期せぬエラーが発生しました。"

    ''' <summary>
    ''' ヤプリユーザエージェント
    ''' </summary>
    ''' <remarks></remarks>
    Private Const USERAGENT_YAPPLI As String = "yappli"

    ''' <summary>
    ''' データアップローダユーザエージェント
    ''' </summary>
    ''' <remarks></remarks>
    Private Const USERAGENT_DATAUPLOADER As String = "qolmsdatauploader"

    ''' <summary>
    ''' 自動ログインのauIDログインを表す文字列です。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const AUTOLOGIN_AUID As String = "auID"

    ''' <summary>
    ''' 自動ログインのJOTO-IDログインを表す文字列です。
    ''' </summary>
    ''' <remarks></remarks>
    Private Const AUTOLOGIN_JOTOID As String = "JOTOID"

    ''' <summary>
    ''' 自動ログインの期限日数を表します。（本日から）
    ''' </summary>
    ''' <remarks></remarks>
    Private Const AUTOLOGIN_EXPIRES As Integer = 20

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="StartController" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region

#Region "ページ ビュー アクション"

#Region "「Index」画面（不可視）"

    '<HttpGet()>
    '<QyLogging()>
    'Public Function Index() As ActionResult
    '    'ヤプリ用の処理
    '    If Request.UserAgent.ToLower().Contains("yappli/16b0d773") Then
    '        If Me.IsLogin Then
    '            Return Redirect("native:/tab/scrollmenu/38634dbf")
    '        Else
    '            Return Redirect("jotohdr:/tab/custom/828caa93")
    '        End If
    '    ElseIf Request.UserAgent.ToLower().Contains("yappli/8e5c8d3a") Then
    '        If Me.IsLogin Then
    '            Return Redirect("native:/tab/scrollmenu/38634dbf")
    '        Else
    '            Return Redirect("yappli:/tab/custom/828caa93")
    '        End If
    '    Else
    '        ' ログイン済みなら「/Portal/Home」画面へ遷移
    '        If Me.IsLogin Then Return RedirectToAction("Home", "Portal")

    '        Return Me.RedirectToAction("Login")

    '    End If

    'End Function

#End Region

#Region "ログイン失敗後のリロード対応"

    <HttpGet()>
    <QyLogging()>
    Public Function LoginJotoIdResult() As ActionResult

        Dim returnUrl As String = Me.TempData("ReturnUrl").ToString()
        Return RedirectToAction("Login", New With {.ReturnUrl = returnUrl})

    End Function

#End Region

#Region "「ログイン」画面"

    <HttpGet()>
    <OutputCache(CacheProfile:="DisableCacheProfile")>
    <QyLogging()>
    Public Function Login(Optional ByVal ReturnUrl As String = "") As ActionResult

        AccessLogWorker.DebugLog("~/App_Data/Log", "loginReturnUrl.log", String.Format("{0}:{1}{2}", Now, $"Login", vbCrLf), Me.HttpContext)
        AccessLogWorker.DebugLog("~/App_Data/Log", "loginReturnUrl.log", String.Format("{0}:{1}{2}", Now, $"1:{ReturnUrl}", vbCrLf), Me.HttpContext)

        ' ログ出力
        AccessLogWorker.WriteAccessLog(
            Me.HttpContext,
            String.Empty,
            AccessLogWorker.AccessTypeEnum.Login,
            String.Format("ReturnUrl={0}", ReturnUrl)
        )

        'ヤプリ用の処理
        If Request.UserAgent.ToLower().Contains("yappli/16b0d773") Then
            If Me.IsLogin Then
                'Return Redirect("native:/tab/scrollmenu/38634dbf")
                'Return Redirect("native:/tab/custom/51885554")
                Return RedirectToAction("Home", "Portal")
            Else
                Me.TempData("ReturnUrl") = ReturnUrl
                'Return Redirect("jotohdr:/tab/custom/828caa93")
            End If
        ElseIf Request.UserAgent.ToLower().Contains("yappli/8e5c8d3a") Then
            If Me.IsLogin Then
                'Return Redirect("native:/tab/scrollmenu/38634dbf")
                'Return Redirect("native:/tab/custom/51885554")
                Return RedirectToAction("Home", "Portal")

            Else
                Me.TempData("ReturnUrl") = ReturnUrl
                ' Return Redirect("native:/tab/custom/828caa93")
            End If
        Else
            ' ログイン済みなら「/Portal/Home」画面へ遷移
            If Me.IsLogin Then
                Return Redirect(Me.MakeRedirectUri(ReturnUrl))
            End If

            ' 次回HTTP要求時に新規セッションを開始
            QySessionHelper.NewSession(Me.Session, Me.Response)

        End If

        AccessLogWorker.DebugLog("~/App_Data/Log", "loginReturnUrl.log", String.Format("{0}:{1}{2}", Now, $"2:{ReturnUrl}", vbCrLf), Me.HttpContext)

        Return Me.RedirectToAction("LoginById", New With {.ReturnUrl = ReturnUrl})

    End Function

    <HttpGet()>
    <OutputCache(CacheProfile:="DisableCacheProfile")>
    <QyLogging()>
    Public Async Function LoginById(Optional ByVal ReturnUrl As String = "") As Threading.Tasks.Task(Of ActionResult)

        'QyCookieHelper.SetAppleIdEntryCookie(Me.HttpContext.Response, "takahashi", "yukari")


        ' Autoログインクッキーの取得
        Dim autoLogin As AuIdLoginCookieJsonParameter = QyCookieHelper.GetAutoAuIdLoginCookie(Me.Request)
        Dim rememberId As RememberIdCookieJsonParameter = QyCookieHelper.GetRememberIdCookie(Me.Request)
        Dim rememberLogin As RememberLoginCookieJsonParameter = QyCookieHelper.GetRememberLoginCookie(Me.Request)
 
        ' 規約等の更新に同意済かどうか
        Dim consentDate As String = ConfigurationManager.AppSettings("AgreementVersion") '利用規約の更新日時
        Dim LastConsentDate As String = QyCookieHelper.GetAgreementVersionCookie(Me.HttpContext.Request)

        Me.TempData("IsAuAutoLogin") = False

        'auLogin
        If Not String.IsNullOrWhiteSpace(consentDate) AndAlso Not String.IsNullOrWhiteSpace(LastConsentDate) AndAlso consentDate = LastConsentDate Then
            Me.TempData("IsConsent") = False
            Me.TempData("AgreementVersion") = consentDate
            'System.IO.File.AppendAllText(log, String.Format("{0}:{1}{2}", Now, "IsConsent OK", vbCrLf))
            If Request.UserAgent.ToLower().Contains(USERAGENT_YAPPLI) Then
                'System.IO.File.AppendAllText(log, String.Format("{0}:{1}{2}", Now, "IsYappli OK", vbCrLf))
                If autoLogin IsNot Nothing AndAlso autoLogin.LoginType = AUTOLOGIN_AUID AndAlso Date.Parse(autoLogin.Expires) > Date.Now Then
                    ' auID Auto Login
                    'デバッグ用ログ
                    AccessLogWorker.DebugLog("~/App_Data/Log","login.log",String.Format("{0}:{1}{2}", Now, "Success", vbCrLf),Me.HttpContext)
                    AccessLogWorker.DebugLog("~/App_Data/Log","login.log",String.Format("{0}:{1}{2}", Now, autoLogin.ToJsonString(), vbCrLf),Me.HttpContext)

                    '規約同意済み
                    Me.TempData("IsAuAutoLogin") = True

                Else If rememberLogin IsNot Nothing AndAlso Date.Parse(rememberLogin.Expires) > Date.Now Then
                           
                    ' JOTOID Auto Login             
                    'デバッグ用ログ
                    AccessLogWorker.DebugLog("~/App_Data/Log","login.log",String.Format("{0}:{1}{2}", Now, "Success", vbCrLf),Me.HttpContext)
                    AccessLogWorker.DebugLog("~/App_Data/Log","login.log",String.Format("{0}:{1}{2}", Now, rememberLogin.ToJsonString(), vbCrLf),Me.HttpContext)
                    Dim isLockDown As Boolean = False

                    If  rememberLogin IsNot Nothing Andalso not String.IsNullOrWhiteSpace(rememberLogin.UserId) AndAlso not String.IsNullOrWhiteSpace(rememberLogin.PasswordHash) Then

                        'JOTO用のIDで保存されるので、入力用のIDに変換する
                        Dim jotoUserId As String = rememberLogin.UserId.Substring(4)

                        ' 手動ログイン開始
                        Dim refAuthorAccount As New AuthorAccountItem()
                        Dim refApiAuthorizeKey As Guid = Guid.Empty
                        Dim refApiAuthorizeExpires As Date = Date.MinValue
                        Dim refApiAuthorizeKey2 As Guid = Guid.Empty
                        Dim refApiAuthorizeExpires2 As Date = Date.MinValue
                        Dim refLoginRetryCount As Byte = Byte.MinValue
                        Dim refLoginLockdownExpires As Date = Date.MinValue
                        Dim refIsSettingComplete As Boolean = False
                        Dim userAgent As String = If(Me.HttpContext IsNot Nothing AndAlso Me.HttpContext.Request IsNot Nothing, Me.HttpContext.Request.UserAgent, String.Empty)

                        Try
                            Select Case LoginWorker.IdAuth(
                                Me.Session.SessionID,
                                jotoUserId,
                                String.Empty,
                                rememberLogin.PasswordHash,
                                refAuthorAccount,
                                refApiAuthorizeKey,
                                refApiAuthorizeExpires,
                                refApiAuthorizeKey2,
                                refApiAuthorizeExpires2,
                                refLoginRetryCount,
                                refLoginLockdownExpires,
                                refIsSettingComplete
                            )

                                Case QsApiOpenIdLoginResultTypeEnum.Success
                                    ' ログイン可能
                                    If QyLoginHelper.ToLogin(
                                        Me.Session,
                                        Me.Response,
                                        refAuthorAccount,
                                        refApiAuthorizeKey,
                                        refApiAuthorizeExpires,
                                        refApiAuthorizeKey2,
                                        refApiAuthorizeExpires2,
                                        true,
                                        true
                                    ) Then

                                        'ログイン成功
                                        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
                                        ' 会員種別を取得
                                        mainModel.AuthorAccount.MembershipType = CType(PremiumWorker.GetMemberShipType(mainModel), QyMemberShipTypeEnum)
                                        ' ポイント付与（対象は操作日）
                                        Dim actionDate As Date = Now
                                        Dim limit As Date = New Date(actionDate.Year, actionDate.Month, 1).AddMonths(7).AddDays(-1) ' ポイント有効期限は 6 ヶ月後の月末
                                        'Try
                                        Await Task.Run(
                                            Sub()
                                                QolmsPointWorker.AddQolmsPoints(
                                                mainModel.ApiExecutor,
                                                mainModel.ApiExecutorName,
                                                mainModel.SessionId,
                                                mainModel.ApiAuthorizeKey,
                                                mainModel.AuthorAccount.AccountKey,
                                                New List(Of QolmsPointGrantItem)() From {
                                                    New QolmsPointGrantItem(mainModel.AuthorAccount.MembershipType, actionDate, Guid.NewGuid.ToApiGuidString(), QyPointItemTypeEnum.Login, limit)
                                                    }
                                                )

                                            End Sub
                                        )

                                        ' ログインモデルを削除
                                        QyLoginHelper.RemoveLoginModel(Me.Session)

                                        ' ログ出力
                                        AccessLogWorker.WriteAccessLog(
                                            Me.HttpContext,
                                            String.Empty,
                                            AccessLogWorker.AccessTypeEnum.Login,
                                            String.Format("AccountKey={0}：Name={1}：{2}", refAuthorAccount.AccountKey, refAuthorAccount.Name, StartController.LOGIN_MESSAGE)
                                        )
                                        '初期設定を済ませてない人は初期設定へ
                                        If refIsSettingComplete = False Then Return RedirectToAction("Setup2")

                                        'セッション切れてログインの場合、ログイン前にいたところにリダイレクト。それ以外は「/Portal/Home」画面へ遷移
                                        Return Redirect(returnUrl)
                                    Else
                                        ' 不明なエラー
                                        Throw New InvalidOperationException(String.Format("AccountKey={0}：{1}", refAuthorAccount.AccountKey, StartController.UNEXPECTED_ERROR_MESSAGE))
                                    End If

                                Case QsApiOpenIdLoginResultTypeEnum.Lockdown
                                    ' ロックダウン

                                    ' エラー画面に表示するメッセージ
                                    Me.SetErrorMessage(String.Format("{0} 回連続してログインに失敗しました。{1}アカウントは {2:yyyy年M月d日 H時m分s秒} までロックされます。", refLoginRetryCount, Environment.NewLine, refLoginLockdownExpires))

                                    Throw New InvalidOperationException(String.Format("UserId={0}：{1}", jotoUserId, StartController.LOCKDOWN_ERROR_MESSAGE))

                                Case Else
                                    ' 不明なエラー
                                    Throw New InvalidOperationException(String.Format("UserId={0}：{1}", jotoUserId, StartController.UNEXPECTED_ERROR_MESSAGE))

                            End Select
                        Catch
                            Throw
                        End Try
            
                    End If

                End If
            End If
        Else
            Me.TempData("IsConsent") = True
            Me.TempData("AgreementVersion") = consentDate
        End If

        ' ログイン済みなら「/Portal/Home」画面へ遷移
        If Me.IsLogin Then
            Return Redirect(Me.MakeRedirectUri(ReturnUrl))
        End If
        TempData.Keep()
                
        ' 手動ログイン
        QyLoginHelper.SetLoginModel(
            Me.Session,
            New LoginModel() With {
                .RememberId = rememberId IsNot Nothing,
                .UserId = String.Empty
            }
        )

        If String.IsNullOrWhiteSpace(ReturnUrl) = False Then Me.TempData("ReturnUrl") = ReturnUrl
        Return View(QyLoginHelper.GetLoginModel(Me.Session))

    End Function

    <HttpGet()>
    <OutputCache(CacheProfile:="DisableCacheProfile")>
    <QyLogging()>
    Public Async Function LoginByAuId(Optional ByVal ReturnUrl As String = "", Optional ByVal byWowId As Boolean = False) As Threading.Tasks.Task(Of ActionResult)

        Dim consentDate As String = ConfigurationManager.AppSettings("AgreementVersion") '利用規約の更新日時
        Dim LastConsentDate As String = QyCookieHelper.GetAgreementVersionCookie(Me.HttpContext.Request)

        If Not String.IsNullOrWhiteSpace(consentDate) Then

            If String.IsNullOrWhiteSpace(LastConsentDate) OrElse consentDate <> LastConsentDate Then

                QyCookieHelper.DisableAgreementVersionCookie(Me.HttpContext.Response)
                QyCookieHelper.SetAgreementVersionCookie(Me.HttpContext.Response, consentDate)

            End If
        End If

        ' ログイン済みなら「/Portal/Home」画面へ遷移
        If Me.IsLogin Then Return Redirect(Me.MakeRedirectUri(ReturnUrl))
        TempData.Keep()
        If String.IsNullOrWhiteSpace(ReturnUrl) = False Then Me.TempData("ReturnUrl") = ReturnUrl

        'どっち使う？Dim loginModel As AuIdLoginModel = AuIdLoginWorker.GetAuthInf(redirectUrl)
        Dim LoginModel As AuIdLoginModel = Nothing
        Try
            LoginModel = Await AuIdLoginWorker.GetAuthInfAync(byWowId)
        Catch ex As Exception
            AccessLogWorker.WriteErrorLog(
                        Me.HttpContext,
                        String.Empty,
                        String.Format("ディスカバリーに失敗 Wow?={0}", byWowId))
            Return RedirectToAction("Login")
        End Try


        If LoginModel IsNot Nothing Then
            QySessionHelper.RemoveItem(Me.Session, GetType(AuIdLoginModel).Name)
            QySessionHelper.SetItem(Me.Session, GetType(AuIdLoginModel).Name, LoginModel)

            '  Return RedirectToAction("Register")
            Dim url As String = LoginModel.GetAuthorizationRequestUrl()
            AuIdLoginWorker.DebugLog(url)
            Try
                'テスト用ログ
                Dim log As String = System.IO.Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Log"), "login.log")
                System.IO.File.AppendAllText(log, String.Format("{0}:{1}{2}", Now, url, vbCrLf))
            Catch ex As Exception

            End Try

            Return Redirect(url)
        End If
        Return RedirectToAction("Login")

    End Function

    <HttpPost()>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <ValidateInput(False)>
    <QyLogging()>
    Public Async Function LoginResult(model As LoginModel) As Threading.Tasks.Task(Of ActionResult)

        'auのautoログイン設定のcookieを削除
        QyCookieHelper.DisableAutoAuIdLoginCookie(Me.HttpContext.Response)

        Dim returnUrl As String = ""
        If Me.TempData("ReturnUrl") IsNot Nothing Then returnUrl = Me.TempData("ReturnUrl").ToString()
        If String.IsNullOrEmpty(returnUrl) Then returnUrl = Url.Action("Home", "Portal")
        ' ログイン済みなら「/Portal/Home」画面へ遷移
        'If Me.IsLogin Then Return RedirectToAction("Home", "Portal")
        If Me.IsLogin Then Return Redirect(returnUrl)

        ' ログインモデルを取得
        Dim inputModel As LoginModel = QyLoginHelper.GetLoginModel(Me.Session)

        ' セッション切れなら「ログイン」画面へ遷移
        If inputModel Is Nothing Then Return RedirectToAction("Login")

        inputModel.UpdateByInput(model)

        ' 手動ログイン開始
        Dim refAuthorAccount As New AuthorAccountItem()
        Dim refApiAuthorizeKey As Guid = Guid.Empty
        Dim refApiAuthorizeExpires As Date = Date.MinValue
        Dim refApiAuthorizeKey2 As Guid = Guid.Empty
        Dim refApiAuthorizeExpires2 As Date = Date.MinValue
        Dim refLoginRetryCount As Byte = Byte.MinValue
        Dim refLoginLockdownExpires As Date = Date.MinValue
        Dim refIsSettingComplete As Boolean = False
        Dim userAgent As String = If(Me.HttpContext IsNot Nothing AndAlso Me.HttpContext.Request IsNot Nothing, Me.HttpContext.Request.UserAgent, String.Empty)

        Try
            Select Case LoginWorker.Auth(
                Me.Session.SessionID,
                inputModel.UserId,
                inputModel.Password,
                String.Empty,
                refAuthorAccount,
                refApiAuthorizeKey,
                refApiAuthorizeExpires,
                refApiAuthorizeKey2,
                refApiAuthorizeExpires2,
                refLoginRetryCount,
                refLoginLockdownExpires,
                refIsSettingComplete
            )

                Case QsApiOpenIdLoginResultTypeEnum.Success
                    ' ログイン可能
                    If QyLoginHelper.ToLogin(
                        Me.Session,
                        Me.Response,
                        refAuthorAccount,
                        refApiAuthorizeKey,
                        refApiAuthorizeExpires,
                        refApiAuthorizeKey2,
                        refApiAuthorizeExpires2,
                        inputModel.RememberId,
                        inputModel.RememberLogin
                    ) Then

                        ' ログイン成功
                        'Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
                        '' ポイント付与（対象は操作日）
                        'Dim limit As Date = Now.Date.AddMonths(7).AddDays(-1) ' ポイント有効期限は 6 ヶ月後の月末
                        'Await Task.Run(
                        '    Sub()
                        '        QolmsPointWorker.AddQolmsPoints(
                        '        mainModel.ApiExecutor,
                        '        mainModel.ApiExecutorName,
                        '        mainModel.SessionId,
                        '        mainModel.ApiAuthorizeKey,
                        '        mainModel.AuthorAccount.AccountKey,
                        '        New List(Of QolmsPointGrantItem)() From {
                        '            New QolmsPointGrantItem(mainModel.AuthorAccount.MemberShipLevel, Now, Guid.NewGuid.ToApiGuidString(), QyPointItemTypeEnum.Lunch, limit, New Date(2018, 1, 2))
                        '            }
                        '        )

                        '    End Sub
                        ')
                        ' ログインモデルを削除
                        QyLoginHelper.RemoveLoginModel(Me.Session)

                        ' ログ出力
                        AccessLogWorker.WriteAccessLog(
                            Me.HttpContext,
                            String.Empty,
                            AccessLogWorker.AccessTypeEnum.Login,
                            String.Format("AccountKey={0}：Name={1}：{2}", refAuthorAccount.AccountKey, refAuthorAccount.Name, StartController.LOGIN_MESSAGE)
                        )
                        '初期設定を済ませてない人は初期設定へ
                        If refIsSettingComplete = False Then Return RedirectToAction("Setup2")

                        'セッション切れてログインの場合、ログイン前にいたところにリダイレクト。それ以外は「/Portal/Home」画面へ遷移
                        Return Redirect(returnUrl)
                    Else
                        ' 不明なエラー
                        Throw New InvalidOperationException(String.Format("AccountKey={0}：{1}", refAuthorAccount.AccountKey, StartController.UNEXPECTED_ERROR_MESSAGE))
                    End If

                Case QsApiOpenIdLoginResultTypeEnum.Retry
                    ' リトライ
                    inputModel.LoginResultType = QsApiLoginResultTypeEnum.Retry
                    inputModel.Message = StartController.RETRY_ERROR_MESSAGE
                    inputModel.Password = String.Empty

                    ' ログ出力
                    AccessLogWorker.WriteErrorLog(
                        Me.HttpContext,
                        String.Empty,
                        String.Format("UserId={0}：{1}", inputModel.UserId, StartController.RETRY_ERROR_MESSAGE)
                    )
                    If String.IsNullOrWhiteSpace(returnUrl) = False Then Me.TempData("ReturnUrl") = returnUrl
                    ' 「ログイン」画面へ遷移
                    Return View("LoginById", inputModel)

                Case QsApiOpenIdLoginResultTypeEnum.Lockdown
                    ' ロックダウン

                    ' エラー画面に表示するメッセージ
                    Me.SetErrorMessage(String.Format("{0} 回連続してログインに失敗しました。{1}アカウントは {2:yyyy年M月d日 H時m分s秒} までロックされます。", refLoginRetryCount, Environment.NewLine, refLoginLockdownExpires))

                    Throw New InvalidOperationException(String.Format("UserId={0}：{1}", inputModel.UserId, StartController.LOCKDOWN_ERROR_MESSAGE))

                Case Else
                    ' 不明なエラー
                    Throw New InvalidOperationException(String.Format("UserId={0}：{1}", inputModel.UserId, StartController.UNEXPECTED_ERROR_MESSAGE))

            End Select
        Catch
            Throw
        End Try

    End Function


    <HttpPost()>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <ValidateInput(False)>
    <QyLogging()>
    Public Async Function LoginJotoIdResult(model As LoginModel) As Threading.Tasks.Task(Of ActionResult)

        'auのautoログイン設定のcookieを削除
        QyCookieHelper.DisableAutoAuIdLoginCookie(Me.HttpContext.Response)

        '利用規約の更新日時
        Dim consentDate As String = ConfigurationManager.AppSettings("AgreementVersion") 
        Dim LastConsentDate As String = QyCookieHelper.GetAgreementVersionCookie(Me.HttpContext.Request)

        If Not String.IsNullOrWhiteSpace(consentDate) Then

            If String.IsNullOrWhiteSpace(LastConsentDate) OrElse consentDate <> LastConsentDate Then

                QyCookieHelper.DisableAgreementVersionCookie(Me.HttpContext.Response)
                QyCookieHelper.SetAgreementVersionCookie(Me.HttpContext.Response, consentDate)
                
                Me.TempData("IsConsent") = false
                Me.TempData("AgreementVersion") = consentDate

            End If
        End If

        Dim returnUrl As String = ""
        If Me.TempData("ReturnUrl") IsNot Nothing Then returnUrl = Me.TempData("ReturnUrl").ToString()
        If String.IsNullOrEmpty(returnUrl) Then returnUrl = Url.Action("Home", "Portal")
        ' ログイン済みなら「/Portal/Home」画面へ遷移
        If Me.IsLogin Then Return Redirect(returnUrl)

        ' ログインモデルを取得
        Dim inputModel As LoginModel = QyLoginHelper.GetLoginModel(Me.Session)

        ' セッション切れなら「ログイン」画面へ遷移
        If inputModel Is Nothing Then Return RedirectToAction("Login")

        inputModel.UpdateByInput(model)

        inputModel.RememberLogin =True

        ' 手動ログイン開始
        Dim refAuthorAccount As New AuthorAccountItem()
        Dim refApiAuthorizeKey As Guid = Guid.Empty
        Dim refApiAuthorizeExpires As Date = Date.MinValue
        Dim refApiAuthorizeKey2 As Guid = Guid.Empty
        Dim refApiAuthorizeExpires2 As Date = Date.MinValue
        Dim refLoginRetryCount As Byte = Byte.MinValue
        Dim refLoginLockdownExpires As Date = Date.MinValue
        Dim refIsSettingComplete As Boolean = False
        Dim userAgent As String = If(Me.HttpContext IsNot Nothing AndAlso Me.HttpContext.Request IsNot Nothing, Me.HttpContext.Request.UserAgent, String.Empty)

        Try
            Select Case LoginWorker.IdAuth(
                Me.Session.SessionID,
                inputModel.UserId,
                inputModel.Password,
                inputModel.PasswordHash,
                refAuthorAccount,
                refApiAuthorizeKey,
                refApiAuthorizeExpires,
                refApiAuthorizeKey2,
                refApiAuthorizeExpires2,
                refLoginRetryCount,
                refLoginLockdownExpires,
                refIsSettingComplete
            )

                Case QsApiOpenIdLoginResultTypeEnum.Success
                    ' ログイン可能
                    If QyLoginHelper.ToLogin(
                        Me.Session,
                        Me.Response,
                        refAuthorAccount,
                        refApiAuthorizeKey,
                        refApiAuthorizeExpires,
                        refApiAuthorizeKey2,
                        refApiAuthorizeExpires2,
                        inputModel.RememberId,
                        inputModel.RememberLogin
                    ) Then

                        'ログイン成功
                        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
                        ' 会員種別を取得
                        mainModel.AuthorAccount.MembershipType = CType(PremiumWorker.GetMemberShipType(mainModel), QyMemberShipTypeEnum)
                        ' ポイント付与（対象は操作日）
                        Dim actionDate As Date = Now
                        Dim limit As Date = New Date(actionDate.Year, actionDate.Month, 1).AddMonths(7).AddDays(-1) ' ポイント有効期限は 6 ヶ月後の月末
                        'Try
                        Await Task.Run(
                            Sub()
                                QolmsPointWorker.AddQolmsPoints(
                                mainModel.ApiExecutor,
                                mainModel.ApiExecutorName,
                                mainModel.SessionId,
                                mainModel.ApiAuthorizeKey,
                                mainModel.AuthorAccount.AccountKey,
                                New List(Of QolmsPointGrantItem)() From {
                                    New QolmsPointGrantItem(mainModel.AuthorAccount.MembershipType, actionDate, Guid.NewGuid.ToApiGuidString(), QyPointItemTypeEnum.Login, limit)
                                    }
                                )

                            End Sub
                        )

                        ' ログインモデルを削除
                        QyLoginHelper.RemoveLoginModel(Me.Session)

                        ' ログ出力
                        AccessLogWorker.WriteAccessLog(
                            Me.HttpContext,
                            String.Empty,
                            AccessLogWorker.AccessTypeEnum.Login,
                            String.Format("AccountKey={0}：Name={1}：{2}", refAuthorAccount.AccountKey, refAuthorAccount.Name, StartController.LOGIN_MESSAGE)
                        )
                        '初期設定を済ませてない人は初期設定へ
                        If refIsSettingComplete = False Then Return RedirectToAction("Setup2")

                        'セッション切れてログインの場合、ログイン前にいたところにリダイレクト。それ以外は「/Portal/Home」画面へ遷移
                        Return Redirect(returnUrl)
                    Else
                        ' 不明なエラー
                        Throw New InvalidOperationException(String.Format("AccountKey={0}：{1}", refAuthorAccount.AccountKey, StartController.UNEXPECTED_ERROR_MESSAGE))
                    End If

                Case QsApiOpenIdLoginResultTypeEnum.Retry
                    ' リトライ
                    inputModel.LoginResultType = QsApiLoginResultTypeEnum.Retry
                    inputModel.Message = StartController.RETRY_ERROR_MESSAGE
                    inputModel.Password = String.Empty

                    ' ログ出力
                    AccessLogWorker.WriteErrorLog(
                        Me.HttpContext,
                        String.Empty,
                        String.Format("UserId={0}：{1}", inputModel.UserId, StartController.RETRY_ERROR_MESSAGE)
                    )
                    If String.IsNullOrWhiteSpace(returnUrl) = False Then Me.TempData("ReturnUrl") = returnUrl
                    ' 「ログイン」画面へ遷移
                    Return View("LoginById", inputModel)

                Case QsApiOpenIdLoginResultTypeEnum.Lockdown
                    ' ロックダウン

                    ' エラー画面に表示するメッセージ
                    Me.SetErrorMessage(String.Format("{0} 回連続してログインに失敗しました。{1}アカウントは {2:yyyy年M月d日 H時m分s秒} までロックされます。", refLoginRetryCount, Environment.NewLine, refLoginLockdownExpires))

                    Throw New InvalidOperationException(String.Format("UserId={0}：{1}", inputModel.UserId, StartController.LOCKDOWN_ERROR_MESSAGE))

                Case Else
                    ' 不明なエラー
                    Throw New InvalidOperationException(String.Format("UserId={0}：{1}", inputModel.UserId, StartController.UNEXPECTED_ERROR_MESSAGE))

            End Select
        Catch
            Throw
        End Try

    End Function


    ''' <summary>
    ''' Auの認証が終わったらこちらにリダイレクトされる
    ''' </summary>
    ''' <param name="state"></param>
    ''' <param name="code"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <OutputCache(CacheProfile:="DisableCacheProfile")>
    <QyLogging()>
    Public Async Function AuIdLoginResult(state As String, Optional code As String = "") As Threading.Tasks.Task(Of ActionResult)

        Dim returnUrl As String = ""
        If Me.TempData("ReturnUrl") IsNot Nothing Then returnUrl = Me.TempData("ReturnUrl").ToString()

        Dim loginModel As AuIdLoginModel = Nothing
        QySessionHelper.GetItem(Me.Session, GetType(AuIdLoginModel).Name, loginModel)
        If loginModel IsNot Nothing Then
            'OIDC_AUTHZ_AU_RES　 OpenIDConnect認可応答チェック(d)
            'stateは、こちらが指定したのが返ってくるはず
            If loginModel.State = state Then
                If String.IsNullOrEmpty(code) = False Then
                    Dim refAuthorAccount As New AuthorAccountItem()
                    Dim refApiAuthorizeKey As Guid = Guid.Empty
                    Dim refApiAuthorizeExpires As Date = Date.MinValue
                    Dim refApiAuthorizeKey2 As Guid = Guid.Empty
                    Dim refApiAuthorizeExpires2 As Date = Date.MinValue
                    Dim refLoginRetryCount As Byte = Byte.MinValue
                    Dim refLoginLockdownExpires As Date = Date.MinValue
                    Dim refIsSettingComplete As Boolean = False

                    Select Case AuIdLoginWorker.Auth(Session.SessionID, loginModel, code, refAuthorAccount,
                                            refApiAuthorizeKey,
                                            refApiAuthorizeExpires,
                                            refApiAuthorizeKey2,
                                            refApiAuthorizeExpires2,
                                            refLoginRetryCount,
                                            refLoginLockdownExpires,
                                            refIsSettingComplete)
                        Case QsApiOpenIdLoginResultTypeEnum.Success
                            'ログイン状態に移行
                            If QyLoginHelper.ToLogin(Me.Session,
                                                        Me.Response,
                                                        refAuthorAccount,
                                                        refApiAuthorizeKey,
                                                        refApiAuthorizeExpires,
                                                        refApiAuthorizeKey2,
                                                        refApiAuthorizeExpires2,
                                                        loginModel.ByWowId) Then

                                ' ログインポイント付与（初回は初回登録ポイントも）＞初回登録の５００ポイントはなしに。プレミアム会員初登録時にポイント付与する
                                Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
                                Dim actionDate As Date = Now
                                Dim limit As Date = New Date(actionDate.Year, actionDate.Month, 1).AddMonths(7).AddDays(-1) ' ポイント有効期限は 6 ヶ月後の月末
                                Dim pointItemList As New List(Of QolmsPointGrantItem)
                                'If mainModel.AuthorAccount.LoginCount = 1 Then pointItemList.Add(New QolmsPointGrantItem(mainModel.AuthorAccount.MembershipType, actionDate, Guid.NewGuid.ToApiGuidString(), QyPointItemTypeEnum.InitialRegistration, limit)) '初回登録
                                '暫定対応
                                mainModel.AuthorAccount.MembershipType = CType(PremiumWorker.GetMemberShipType(mainModel), QyMemberShipTypeEnum)

                                pointItemList.Add(New QolmsPointGrantItem(mainModel.AuthorAccount.MembershipType, actionDate, Guid.NewGuid.ToApiGuidString(), QyPointItemTypeEnum.Login, limit)) 'loginポイント
                                Await Task.Run(
                                    Sub()
                                        QolmsPointWorker.AddQolmsPoints(mainModel.ApiExecutor, mainModel.ApiExecutorName, mainModel.SessionId, mainModel.ApiAuthorizeKey,
                                        mainModel.AuthorAccount.AccountKey, pointItemList)
                                    End Sub
                                )
                                'AuidLoginModelをセッションから削除
                                QySessionHelper.RemoveItem(Me.Session, GetType(AuIdLoginModel).Name)

                                'auIDによる自動ログインcookieの設定
                                QyCookieHelper.DisableAutoAuIdLoginCookie(Me.HttpContext.Response)
                                '３週間
                                QyCookieHelper.SetAutoAuIdLoginCookie(Me.HttpContext.Response, AUTOLOGIN_AUID, Date.Now.AddDays(AUTOLOGIN_EXPIRES))

                                'Try
                                '    Dim cookie As AuIdLoginCookieJsonParameter = QyCookieHelper.GetAutoAuIdLoginCookie(Me.HttpContext.Request)

                                '    'テスト用ログ
                                '    Dim log As String = System.IO.Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Log"), "login.log")
                                '    System.IO.File.AppendAllText(log, String.Format("{0}", vbCrLf))
                                '    System.IO.File.AppendAllText(log, String.Format("{0}:{1}{2}", Now, "Success", vbCrLf))
                                '    System.IO.File.AppendAllText(log, String.Format("{0}:{1}{2}", Now, cookie.ToJsonString(), vbCrLf))
                                'Catch ex As Exception
                                '    Dim log As String = System.IO.Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Log"), "login.log")
                                '    System.IO.File.AppendAllText(log, String.Format("{0}:{1}{2}", ex.Message, vbCrLf))

                                'End Try

                                If refIsSettingComplete = False Then Return RedirectToAction("Setup2")

                                Return Redirect(Me.MakeRedirectUri(returnUrl))

                            End If
                        Case QsApiOpenIdLoginResultTypeEnum.NewUser

                            'auIDによる自動ログインcookieの設定
                            QyCookieHelper.DisableAutoAuIdLoginCookie(Me.HttpContext.Response)
                            '３週間
                            QyCookieHelper.SetAutoAuIdLoginCookie(Me.HttpContext.Response, AUTOLOGIN_AUID, Date.Now.AddDays(AUTOLOGIN_EXPIRES))


                            'Try
                            '    Dim cookie As AuIdLoginCookieJsonParameter = QyCookieHelper.GetAutoAuIdLoginCookie(Me.HttpContext.Request)

                            '    'テスト用ログ
                            '    Dim log As String = System.IO.Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Log"), "login.log")
                            '    System.IO.File.AppendAllText(log, String.Format("{0}", vbCrLf))
                            '    System.IO.File.AppendAllText(log, String.Format("{0}:{1}{2}", Now, "NewUser", vbCrLf))
                            '    System.IO.File.AppendAllText(log, String.Format("{0}:{1}{2}", Now, cookie.ToJsonString(), vbCrLf))
                            'Catch ex As Exception
                            '    Dim log As String = System.IO.Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Log"), "login.log")
                            '    System.IO.File.AppendAllText(log, String.Format("{0}:{1}{2}", ex.Message, vbCrLf))

                            'End Try

                            Return RedirectToAction("Reregister", "Start")
                            'Return RedirectToAction("Register", "Start")
                        Case Else
                            Throw New InvalidOperationException(String.Format("OpenId={0}：{1}", loginModel.UserId, StartController.UNEXPECTED_ERROR_MESSAGE))
                    End Select
                Else
                    'Auの認証でエラーで返ってきた場合。Http status 302 , パラメータ（error,error_description)
                    Dim errorCode As String = Request.Params.Item("error")
                    Dim errorDescription As String = Request.Params.Item("error_description")
                    'ログ出力
                    AccessLogWorker.WriteErrorLog(
                        Me.HttpContext,
                        String.Empty,
                        String.Format("ErrorCode={0},ErrorDescription{1}", errorCode, errorDescription)
                    )
                End If
            End If
        End If
        Return RedirectToAction("LoginByAuId")
    End Function

    ''' <summary>
    ''' 新規登録画面
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <QyLogging()>
    Public Function Register(Optional openId As Byte = 1) As ActionResult

        ' ログイン済みなら「/Portal/Home」画面へ遷移
        If Me.IsLogin Then Return RedirectToAction("Home", "Portal")

        Dim model As New StartRegisterInputModel()

        'OpenIDの種別毎に処理を分ける
        Dim openIdType As QsApiOpenIdTypeEnum = DirectCast([Enum].ToObject(GetType(QsApiOpenIdTypeEnum), openId), QsApiOpenIdTypeEnum)

        If openIdType = QsApiOpenIdTypeEnum.AppleId Then
            'AppleID
            AppleIdLoginWorker.DebugLog($"{openIdType}")

            Dim loginModel As AppleIdLoginModel = Nothing
            QySessionHelper.GetItem(Session, GetType(AppleIdLoginModel).Name, loginModel)
            AppleIdLoginWorker.DebugLog($"{loginModel}")

            '名前が取れてないとき、
            If String.IsNullOrWhiteSpace(loginModel.FirstName) AndAlso String.IsNullOrWhiteSpace(loginModel.LastName) Then

                Dim cookie As AppleIdEntryCookieJsonParameter = QyCookieHelper.GetAppleIdEntryCookie(Me.HttpContext.Request)
                If cookie IsNot Nothing Then
                    loginModel.FirstName = If(String.IsNullOrWhiteSpace(cookie.FirstName), loginModel.FirstName, cookie.FirstName)
                    loginModel.LastName = If(String.IsNullOrWhiteSpace(cookie.LastName), loginModel.FirstName, cookie.LastName)
                End If
            End If

            If loginModel IsNot Nothing Then

                AppleIdLoginWorker.DebugLog($"{loginModel.UserId}")

                model.Accountkey = Guid.NewGuid()
                model.OpenId = loginModel.UserId
                model.OpenIdType = openId ' openIDTypeを指定   '全部AUID扱い
                model.FamilyName = loginModel.LastName
                model.GivenName = loginModel.FirstName
                model.MailAddress = loginModel.MailAddress
                QySessionHelper.SetItem(Session, GetType(StartRegisterInputModel).Name, model)
                Return View("Register", model)
            Else
                Return RedirectToAction("LoginByAppleId")
            End If

        Else
            'auID
            Dim loginModel As AuIdLoginModel = Nothing
            QySessionHelper.GetItem(Session, GetType(AuIdLoginModel).Name, loginModel)

            If loginModel IsNot Nothing Then
                model.Accountkey = Guid.NewGuid()
                model.OpenId = loginModel.UserId
                model.OpenIdType = openId ' openIDTypeを指定   '全部AUID扱い
                With AuOwlAccessWorker.GetUserInf(model.OpenId)
                    model.FamilyName = .FamilyName
                    model.GivenName = .GivenName
                    model.FamilyKanaName = .FamilyKanaName
                    model.GivenKanaName = .GivenKanaName
                    model.BirthYear = .BirthYear
                    model.BirthMonth = .BirthMonth
                    model.BirthDay = .BirthDay
                    model.Sex = .Sex
                    model.MailAddress = .MailAddress
                End With
                QySessionHelper.SetItem(Session, GetType(StartRegisterInputModel).Name, model)
                Return View("Register", model)
            Else
                Return RedirectToAction("LoginByAuId")
            End If

        End If

    End Function

    <HttpPost()>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <ValidateInput(False)>
    <QyLogging()>
    Public Async Function RegisterResult(model As StartRegisterInputModel) As Task(Of ActionResult)
        AppleIdLoginWorker.DebugLog(String.Format("{0}:{1}{2}", Now, "RegisterResult", vbCrLf))

        ' ログイン済みなら「/Portal/Home」画面へ遷移
        If Me.IsLogin Then Return RedirectToAction("Home", "Portal")

        Dim sessionmodel As StartRegisterInputModel = Nothing
        QySessionHelper.GetItem(Session, GetType(StartRegisterInputModel).Name, sessionmodel)

        'インプットモデル検証
        If sessionmodel IsNot Nothing Then
            ' モデルへ入力値を反映
            sessionmodel.UpdateByInput(model)

            ' モデルの検証状態を確認
            If Me.ModelState.IsValid Then ' 検証成功
                Dim errorList As List(Of String) = Nothing
                If SignUpWorker.RegisterWrite(sessionmodel, errorList) Then 'IdentityApiへ値を投げる
                    'API実行成功
                    If errorList Is Nothing OrElse errorList.Count = 0 Then

                        Select Case sessionmodel.OpenIdType
                            Case 1
                                'auID

                                Dim loginModel As AuIdLoginModel = Nothing
                                QySessionHelper.GetItem(Me.Session, GetType(AuIdLoginModel).Name, loginModel) ' 
                                If loginModel IsNot Nothing Then
                                    '自動的にログインしなおしてあげる。AuIDの認証とQolmsログイン認証へ（ユーザにはそのままHome画面へ移行した状態にみえる）
                                    QySessionHelper.RemoveItem(Me.Session, GetType(AuIdLoginModel).Name)
                                    Dim loginByWowId As Boolean = loginModel.ByWowId
                                    Return RedirectToAction("LoginByAuId", New With {.ReturnUrl = "", .byWowId = loginByWowId})
                                End If

                            Case 3
                                AppleIdLoginWorker.DebugLog(String.Format("{0}:{1}{2}", Now, sessionmodel.OpenId, vbCrLf))
                                Dim loginModel As AppleIdLoginModel = Nothing
                                QySessionHelper.GetItem(Me.Session, GetType(AppleIdLoginModel).Name, loginModel)

                                ' 手動ログイン開始
                                Dim refLoginModel As New AppleIdLoginModel()
                                Dim refAuthorAccount As New AuthorAccountItem()
                                Dim refApiAuthorizeKey As Guid = Guid.Empty
                                Dim refApiAuthorizeExpires As Date = Date.MinValue
                                Dim refApiAuthorizeKey2 As Guid = Guid.Empty
                                Dim refApiAuthorizeExpires2 As Date = Date.MinValue
                                Dim refLoginRetryCount As Byte = Byte.MinValue
                                Dim refLoginLockdownExpires As Date = Date.MinValue
                                Dim refIsSettingComplete As Boolean = False
                                Dim userAgent As String = If(Me.HttpContext IsNot Nothing AndAlso Me.HttpContext.Request IsNot Nothing, Me.HttpContext.Request.UserAgent, String.Empty)

                                AppleIdLoginWorker.DebugLog(String.Format("{0}:{1}{2}", Now, loginModel.UserId, vbCrLf))
                                Select Case AppleIdLoginWorker.Auth(
                                    Me.Session.SessionID,
                                    loginModel.UserId,
                                    refLoginModel,
                                    refAuthorAccount,
                                    refApiAuthorizeKey,
                                    refApiAuthorizeExpires,
                                    refApiAuthorizeKey2,
                                    refApiAuthorizeExpires2,
                                    refLoginRetryCount,
                                    refLoginLockdownExpires,
                                    refIsSettingComplete
                                )
                                    Case QsApiOpenIdLoginResultTypeEnum.Success
                                        AppleIdLoginWorker.DebugLog(String.Format("{0}:{1}{2}", Now, "Success", vbCrLf))
                                        'ログイン状態に移行
                                        If QyLoginHelper.ToLogin(Me.Session,
                                            Me.Response,
                                            refAuthorAccount,
                                            refApiAuthorizeKey,
                                            refApiAuthorizeExpires,
                                            refApiAuthorizeKey2,
                                            refApiAuthorizeExpires2,
                                            False) Then
                                            AppleIdLoginWorker.DebugLog(String.Format("{0}:{1}{2}", Now, "Success", vbCrLf))

                                            ' ログインポイント付与（初回は初回登録ポイントも）＞初回登録の５００ポイントはなしに。プレミアム会員初登録時にポイント付与する
                                            Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
                                            Dim actionDate As Date = Now
                                            Dim limit As Date = New Date(actionDate.Year, actionDate.Month, 1).AddMonths(7).AddDays(-1) ' ポイント有効期限は 6 ヶ月後の月末
                                            Dim pointItemList As New List(Of QolmsPointGrantItem)
                                            '暫定対応
                                            mainModel.AuthorAccount.MembershipType = CType(PremiumWorker.GetMemberShipType(mainModel), QyMemberShipTypeEnum)

                                            pointItemList.Add(New QolmsPointGrantItem(mainModel.AuthorAccount.MembershipType, actionDate, Guid.NewGuid.ToApiGuidString(), QyPointItemTypeEnum.Login, limit)) 'loginポイント
                                            Await Task.Run(
                                                Sub()
                                                    QolmsPointWorker.AddQolmsPoints(mainModel.ApiExecutor, mainModel.ApiExecutorName, mainModel.SessionId, mainModel.ApiAuthorizeKey,
                                                    mainModel.AuthorAccount.AccountKey, pointItemList)
                                                End Sub
                                            )
                                            ''AuidLoginModelをセッションから削除
                                            'QySessionHelper.RemoveItem(Me.Session, GetType(AuIdLoginModel).Name)

                                            ''auIDによる自動ログインcookieの設定
                                            'QyCookieHelper.DisableAutoAuIdLoginCookie(Me.HttpContext.Response)
                                            ''３週間
                                            'QyCookieHelper.SetAutoAuIdLoginCookie(Me.HttpContext.Response, AUTOLOGIN_AUID, Date.Now.AddDays(AUTOLOGIN_EXPIRES))
                                            AppleIdLoginWorker.DebugLog(String.Format("{0}:{1}{2}", Now, $"refIsSettingComplete={refIsSettingComplete}", vbCrLf))

                                            If refIsSettingComplete = False Then Return RedirectToAction("Setup2")

                                            Return Redirect(Me.MakeRedirectUri(String.Empty))

                                        End If
                                    Case QsApiOpenIdLoginResultTypeEnum.NewUser
                                        QySessionHelper.SetItem(Me.Session, GetType(AppleIdLoginModel).Name, refLoginModel)
                                        ''auIDによる自動ログインcookieの設定
                                        'QyCookieHelper.DisableAutoAuIdLoginCookie(Me.HttpContext.Response)
                                        ''３週間
                                        'QyCookieHelper.SetAutoAuIdLoginCookie(Me.HttpContext.Response, AUTOLOGIN_AUID, Date.Now.AddDays(AUTOLOGIN_EXPIRES))
                                        AppleIdLoginWorker.DebugLog(String.Format("{0}:{1}{2}", Now, $"NewUser", vbCrLf))
                                        AccessLogWorker.DebugLog("~/App_Data/Log", "login.log", String.Format("{0}:{1}{2}", Now, "", vbCrLf), Me.HttpContext)

                                        Return RedirectToAction("Reregister", "Start", New With {.openId = 3})

                                    Case Else
                                        'Throw New InvalidOperationException(String.Format("OpenId={0}：{1}", , StartController.UNEXPECTED_ERROR_MESSAGE))
                                End Select
                            Case Else
                                '未使用
                        End Select



                    Else 'エラーメッセージあり＞メッセージを返却
                        ' 独自にエラーメッセージを用意
                        Dim errorMessage As New Dictionary(Of String, String)()

                        For Each key As String In errorList
                            Select Case key
                                Case "UserId"
                                    errorMessage.Add("UserId", "このユーザーIDは既に使用されています。")
                                Case "OpenId"
                                    errorMessage.Add("OpenId", "このOpenIDは既に登録されています。")
                                Case Else
                                    '未使用
                            End Select
                        Next
                        Me.TempData("ErrorMessage") = errorMessage

                        ' ビューを返却
                        Return View("Register", sessionmodel)
                    End If
                Else
                    'ログ出力(本登録エラー)
                    AccessLogWorker.WriteAccessLog(Me.HttpContext, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("OpenID={0}：{1}", sessionmodel.OpenId, StartController.UNEXPECTED_ERROR_MESSAGE))
                End If

            Else
                ' 検証失敗 ' 独自にエラーメッセージを用意
                Dim errorMessage As New Dictionary(Of String, String)()

                For Each key As String In Me.ModelState.Keys
                    Select Case key
                        Case "model.BirthYear", "model.BirthMonth", "model.BirthDay"
                            ' 生年月日
                            If Not errorMessage.ContainsKey("model.BirthYear") Then
                                For Each e As ModelError In Me.ModelState(key).Errors
                                    errorMessage.Add("model.BirthYear", e.ErrorMessage)
                                Next
                            End If
                        Case Else
                            If Not errorMessage.ContainsKey(key) AndAlso
                                 Me.ModelState(key).Errors.Count > 0 Then
                                errorMessage.Add(key, Me.ModelState(key).Errors.First.ErrorMessage)
                                'For Each e As ModelError In
                                '    errorMessage.Add(key, e.ErrorMessage)
                                'Next
                            End If
                    End Select
                Next
                Me.TempData("ErrorMessage") = errorMessage
                ' ビューを返却
                Return View("Register", sessionmodel)
            End If
        End If

        Return Me.RedirectToAction("LoginById")

    End Function

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function DataUploaderAuth() As ActionResult
        If Request.UserAgent.ToLower().Contains(USERAGENT_DATAUPLOADER) Then
            Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
            Dim token As String = OpenApiWorker.GetApplicationLoginToken(mainModel.ApiExecutor, mainModel.ApiExecutorName, mainModel.SessionId, mainModel.ApiAuthorizeKey, mainModel.AuthorAccount.AccountKey)
            Response.Headers.Add("X-LoginToken", token)
            ViewData("Token") = token
            Return View("DummyResult")
        End If
        Return New EmptyResult()
    End Function

    <HttpGet()>
    <QyLogging()>
    Public Function WowNewAccount() As ActionResult
        Return Redirect(AuIdLoginModel.GetWowNewAccountUrl())
    End Function


#Region "「appleidログイン」画面"

    ''' <summary>
    ''' AppleID ログイン
    ''' </summary>
    ''' <param name="ReturnUrl"></param>
    ''' <returns></returns>
    <HttpGet()>
    <OutputCache(CacheProfile:="DisableCacheProfile")>
    <QyLogging()>
    Public Async Function LoginByAppleId(Optional ByVal ReturnUrl As String = "") As Threading.Tasks.Task(Of ActionResult)

        Dim consentDate As String = ConfigurationManager.AppSettings("AgreementVersion") '利用規約の更新日時
        Dim LastConsentDate As String = QyCookieHelper.GetAgreementVersionCookie(Me.HttpContext.Request)

        If Not String.IsNullOrWhiteSpace(consentDate) Then

            If String.IsNullOrWhiteSpace(LastConsentDate) OrElse consentDate <> LastConsentDate Then

                QyCookieHelper.DisableAgreementVersionCookie(Me.HttpContext.Response)
                QyCookieHelper.SetAgreementVersionCookie(Me.HttpContext.Response, consentDate)

            End If
        End If

        ' ログイン済みなら「/Portal/Home」画面へ遷移
        If Me.IsLogin Then Return Redirect(Me.MakeRedirectUri(ReturnUrl))
        TempData.Keep()
        If String.IsNullOrWhiteSpace(ReturnUrl) = False Then Me.TempData("ReturnUrl") = ReturnUrl

        'どっち使う？Dim loginModel As AuIdLoginModel = AuIdLoginWorker.GetAuthInf(redirectUrl)
        Dim LoginModel As LoginModel = Nothing
        Dim url As String = AppleIdLoginWorker.GetAuthUrl()
        Try
            'テスト用ログ
            Dim log As String = System.IO.Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Log"), "login.log")
            System.IO.File.AppendAllText(log, String.Format("{0}:{1}{2}", Now, url, vbCrLf))
        Catch ex As Exception

        End Try
        Return Redirect(url)

        'If LoginModel IsNot Nothing Then
        '    QySessionHelper.RemoveItem(Me.Session, GetType(AuIdLoginModel).Name)
        '    QySessionHelper.SetItem(Me.Session, GetType(AuIdLoginModel).Name, LoginModel)

        '    '  Return RedirectToAction("Register")


        'End If
        'Return RedirectToAction("Login")

    End Function

    ''' <summary>
    ''' </summary>
    ''' <param name="state"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpPost()>
    <OutputCache(CacheProfile:="DisableCacheProfile")>
    <QyLogging()>
    Public Async Function AppleIdLoginResult(state As AuthorizeApiResults) As Threading.Tasks.Task(Of ActionResult)

        'auのautoログイン設定のcookieを削除
        'QyCookieHelper.DisableAutoAuIdLoginCookie(Me.HttpContext.Response)

        '利用規約の更新日時
        Dim consentDate As String = ConfigurationManager.AppSettings("AgreementVersion")
        Dim LastConsentDate As String = QyCookieHelper.GetAgreementVersionCookie(Me.HttpContext.Request)

        If Not String.IsNullOrWhiteSpace(consentDate) Then

            If String.IsNullOrWhiteSpace(LastConsentDate) OrElse consentDate <> LastConsentDate Then

                QyCookieHelper.DisableAgreementVersionCookie(Me.HttpContext.Response)
                QyCookieHelper.SetAgreementVersionCookie(Me.HttpContext.Response, consentDate)

                Me.TempData("IsConsent") = False
                Me.TempData("AgreementVersion") = consentDate

            End If
        End If

        Dim returnUrl As String = ""
        If Me.TempData("ReturnUrl") IsNot Nothing Then returnUrl = Me.TempData("ReturnUrl").ToString()
        If String.IsNullOrEmpty(returnUrl) Then returnUrl = Url.Action("Home", "Portal")
        ' ログイン済みなら「/Portal/Home」画面へ遷移
        If Me.IsLogin Then Return Redirect(returnUrl)

        '' ログインモデルを取得
        'Dim inputModel As LoginModel = QyLoginHelper.GetLoginModel(Me.Session)

        '' セッション切れなら「ログイン」画面へ遷移
        'If inputModel Is Nothing Then Return RedirectToAction("Login")

        'inputModel.UpdateByInput(model)

        'inputModel.RememberLogin = True

        ' 手動ログイン開始
        Dim refLoginModel As New AppleIdLoginModel()
        Dim refAuthorAccount As New AuthorAccountItem()
        Dim refApiAuthorizeKey As Guid = Guid.Empty
        Dim refApiAuthorizeExpires As Date = Date.MinValue
        Dim refApiAuthorizeKey2 As Guid = Guid.Empty
        Dim refApiAuthorizeExpires2 As Date = Date.MinValue
        Dim refLoginRetryCount As Byte = Byte.MinValue
        Dim refLoginLockdownExpires As Date = Date.MinValue
        Dim refIsSettingComplete As Boolean = False
        Dim userAgent As String = If(Me.HttpContext IsNot Nothing AndAlso Me.HttpContext.Request IsNot Nothing, Me.HttpContext.Request.UserAgent, String.Empty)

        AppleIdLoginWorker.DebugLog(String.Format("{0}:{1}{2}", Now, "AppleIdLoginWorker.Auth", vbCrLf))

        Select Case AppleIdLoginWorker.Auth(
            Me.Session.SessionID,
            state,
            refLoginModel,
            refAuthorAccount,
            refApiAuthorizeKey,
            refApiAuthorizeExpires,
            refApiAuthorizeKey2,
            refApiAuthorizeExpires2,
            refLoginRetryCount,
            refLoginLockdownExpires,
            refIsSettingComplete
        )
            Case QsApiOpenIdLoginResultTypeEnum.Success
                'ログイン状態に移行
                If QyLoginHelper.ToLogin(Me.Session,
                                            Me.Response,
                                            refAuthorAccount,
                                            refApiAuthorizeKey,
                                            refApiAuthorizeExpires,
                                            refApiAuthorizeKey2,
                                            refApiAuthorizeExpires2,
                                            False) Then
                    AppleIdLoginWorker.DebugLog(String.Format("{0}:{1}{2}", Now, "Success", vbCrLf))

                    ' ログインポイント付与（初回は初回登録ポイントも）＞初回登録の５００ポイントはなしに。プレミアム会員初登録時にポイント付与する
                    Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
                    Dim actionDate As Date = Now
                    Dim limit As Date = New Date(actionDate.Year, actionDate.Month, 1).AddMonths(7).AddDays(-1) ' ポイント有効期限は 6 ヶ月後の月末
                    Dim pointItemList As New List(Of QolmsPointGrantItem)
                    '暫定対応
                    mainModel.AuthorAccount.MembershipType = CType(PremiumWorker.GetMemberShipType(mainModel), QyMemberShipTypeEnum)

                    pointItemList.Add(New QolmsPointGrantItem(mainModel.AuthorAccount.MembershipType, actionDate, Guid.NewGuid.ToApiGuidString(), QyPointItemTypeEnum.Login, limit)) 'loginポイント
                    Await Task.Run(
                        Sub()
                            QolmsPointWorker.AddQolmsPoints(mainModel.ApiExecutor, mainModel.ApiExecutorName, mainModel.SessionId, mainModel.ApiAuthorizeKey,
                            mainModel.AuthorAccount.AccountKey, pointItemList)
                        End Sub
                    )
                    ''AppleidLoginModelをセッションから削除
                    'QySessionHelper.RemoveItem(Me.Session, GetType(AuIdLoginModel).Name)

                    ''auIDによる自動ログインcookieの設定
                    'QyCookieHelper.DisableAutoAuIdLoginCookie(Me.HttpContext.Response)
                    ''３週間
                    'QyCookieHelper.SetAutoAuIdLoginCookie(Me.HttpContext.Response, AUTOLOGIN_AUID, Date.Now.AddDays(AUTOLOGIN_EXPIRES))

                    If refIsSettingComplete = False Then Return RedirectToAction("Setup2")

                    Return Redirect(Me.MakeRedirectUri(returnUrl))

                End If
            Case QsApiOpenIdLoginResultTypeEnum.NewUser
                QySessionHelper.SetItem(Me.Session, GetType(AppleIdLoginModel).Name, refLoginModel)
                'appleIDの初回認証cookieの設定
                '2回目以降の認証で名前が取得できないため設定
                If Not (String.IsNullOrWhiteSpace(refLoginModel.FirstName) AndAlso String.IsNullOrWhiteSpace(refLoginModel.LastName)) Then

                    QyCookieHelper.SetAppleIdEntryCookie(Me.HttpContext.Response, refLoginModel.FirstName, refLoginModel.LastName)
                End If
                'Autoログイン設定（未実装）
                ''３週間
                'QyCookieHelper.SetAutoAuIdLoginCookie(Me.HttpContext.Response, AUTOLOGIN_AUID, Date.Now.AddDays(AUTOLOGIN_EXPIRES))

                AppleIdLoginWorker.DebugLog(String.Format("{0}:{1}{2}", Now, "NewUser", vbCrLf))

                Return RedirectToAction("Reregister", "Start", New With {.openId = 3})

            Case Else
                'Throw New InvalidOperationException(String.Format("OpenId={0}：{1}", , StartController.UNEXPECTED_ERROR_MESSAGE))
        End Select
        ''Auの認証でエラーで返ってきた場合。Http status 302 , パラメータ（error,error_description)
        'Dim errorCode As String = Request.Params.Item("error")
        'Dim errorDescription As String = Request.Params.Item("error_description")
        ''ログ出力
        'AccessLogWorker.WriteErrorLog(
        '    Me.HttpContext,
        '    String.Empty,
        '    String.Format("ErrorCode={0},ErrorDescription{1}", errorCode, errorDescription)
        ')
        Return RedirectToAction("LoginByAuId")

    End Function


#End Region

#End Region

#Region "「アカウント新規登録」画面"

    ''' <summary>
    ''' 新規登録画面
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <QyLogging()>
    Public Function RegisterId() As ActionResult

        ' ログイン済みなら「/Portal/Home」画面へ遷移
        If Me.IsLogin Then Return RedirectToAction("Home", "Portal")
        Dim model As New StartRegisterUserIdInputModel()
        model.Accountkey = Guid.NewGuid()

        QySessionHelper.SetItem(Session, GetType(StartRegisterUserIdInputModel).Name, model)

        Return View("RegisterId", model)

    End Function

    <HttpPost()>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <ValidateInput(False)>
    <QyLogging()>
    Public Async Function RegisterIdResult(model As StartRegisterUserIdInputModel) As Task(Of ActionResult)

        Dim sessionmodel As StartRegisterUserIdInputModel = Nothing
        QySessionHelper.GetItem(Session, GetType(StartRegisterUserIdInputModel).Name, sessionmodel)

        'インプットモデル検証
        If sessionmodel IsNot Nothing Then
            ' モデルへ入力値を反映
            sessionmodel.UpdateByInput(model)

            ' モデルの検証状態を確認
            If Me.ModelState.IsValid Then ' 検証成功
                Dim errorList As List(Of String) = Nothing
                If SignUpWorker.RegisterUserIdWrite(sessionmodel, errorList) Then 'IdentityApiへ値を投げる
                    'API実行成功
                    If errorList Is Nothing OrElse errorList.Count = 0 Then
                        Dim loginModel As LoginModel = Nothing
                        QySessionHelper.GetItem(Me.Session, GetType(LoginModel).Name, loginModel) ' 
                        If loginModel IsNot Nothing Then
                            '自動的にログインしなおしてあげる。AuIDの認証とQolmsログイン認証へ（ユーザにはそのままHome画面へ移行した状態にみえる）
                            QySessionHelper.RemoveItem(Me.Session, GetType(LoginModel).Name)

                            ' 手動ログイン開始
                            Dim refAuthorAccount As New AuthorAccountItem()
                            Dim refApiAuthorizeKey As Guid = Guid.Empty
                            Dim refApiAuthorizeExpires As Date = Date.MinValue
                            Dim refApiAuthorizeKey2 As Guid = Guid.Empty
                            Dim refApiAuthorizeExpires2 As Date = Date.MinValue
                            Dim refLoginRetryCount As Byte = Byte.MinValue
                            Dim refLoginLockdownExpires As Date = Date.MinValue
                            Dim refIsSettingComplete As Boolean = False
                            Dim userAgent As String = If(Me.HttpContext IsNot Nothing AndAlso Me.HttpContext.Request IsNot Nothing, Me.HttpContext.Request.UserAgent, String.Empty)

                            Try
                                Select Case LoginWorker.IdAuth(
                                    Me.Session.SessionID,
                                    sessionmodel.UserId,
                                    sessionmodel.Password,
                                    String.Empty,
                                    refAuthorAccount,
                                    refApiAuthorizeKey,
                                    refApiAuthorizeExpires,
                                    refApiAuthorizeKey2,
                                    refApiAuthorizeExpires2,
                                    refLoginRetryCount,
                                    refLoginLockdownExpires,
                                    refIsSettingComplete
                                )

                                    Case QsApiOpenIdLoginResultTypeEnum.Success
                                        ' ログイン可能
                                        If QyLoginHelper.ToLogin(
                                            Me.Session,
                                            Me.Response,
                                            refAuthorAccount,
                                            refApiAuthorizeKey,
                                            refApiAuthorizeExpires,
                                            refApiAuthorizeKey2,
                                            refApiAuthorizeExpires2,
                                            loginModel.RememberId,
                                            loginModel.RememberLogin
                                        ) Then

                                            'ログイン成功
                                            Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
                                            ' ポイント付与（対象は操作日）
                                            Dim actionDate As Date = Now
                                            Dim limit As Date = New Date(actionDate.Year, actionDate.Month, 1).AddMonths(7).AddDays(-1) ' ポイント有効期限は 6 ヶ月後の月末
                                            Try
                                                Await Task.Run(
                                                    Sub()
                                                        QolmsPointWorker.AddQolmsPoints(
                                                        mainModel.ApiExecutor,
                                                        mainModel.ApiExecutorName,
                                                        mainModel.SessionId,
                                                        mainModel.ApiAuthorizeKey,
                                                        mainModel.AuthorAccount.AccountKey,
                                                        New List(Of QolmsPointGrantItem)() From {
                                                            New QolmsPointGrantItem(mainModel.AuthorAccount.MembershipType, actionDate, Guid.NewGuid.ToApiGuidString(), QyPointItemTypeEnum.Login, limit)
                                                            }
                                                        )

                                                    End Sub
                                                )

                                            Catch ex As Exception
                                            End Try
                                            ' ログインモデルを削除
                                            QyLoginHelper.RemoveLoginModel(Me.Session)

                                            ' ログ出力
                                            AccessLogWorker.WriteAccessLog(
                                                Me.HttpContext,
                                                String.Empty,
                                                AccessLogWorker.AccessTypeEnum.Login,
                                                String.Format("AccountKey={0}：Name={1}：{2}", refAuthorAccount.AccountKey, refAuthorAccount.Name, StartController.LOGIN_MESSAGE)
                                            )
                                            '初期設定を済ませてない人は初期設定へ
                                            If refIsSettingComplete = False Then Return RedirectToAction("Setup2")

                                        Else
                                            ' 不明なエラー
                                            Throw New InvalidOperationException(String.Format("AccountKey={0}：{1}", refAuthorAccount.AccountKey, StartController.UNEXPECTED_ERROR_MESSAGE))
                                        End If

                                    Case QsApiOpenIdLoginResultTypeEnum.Retry
                                        ' リトライ
                                        loginModel.LoginResultType = QsApiLoginResultTypeEnum.Retry
                                        loginModel.Message = StartController.RETRY_ERROR_MESSAGE
                                        sessionmodel.Password = String.Empty

                                        ' ログ出力
                                        AccessLogWorker.WriteErrorLog(
                                            Me.HttpContext,
                                            String.Empty,
                                            String.Format("UserId={0}：{1}", sessionmodel.UserId, StartController.RETRY_ERROR_MESSAGE)
                                        )
                                        'If String.IsNullOrWhiteSpace(returnUrl) = False Then Me.TempData("ReturnUrl") = returnUrl
                                        ' 「ログイン」画面へ遷移
                                        Return View("LoginById", loginModel)

                                    Case QsApiOpenIdLoginResultTypeEnum.Lockdown
                                        ' ロックダウン

                                        ' エラー画面に表示するメッセージ
                                        Me.SetErrorMessage(String.Format("{0} 回連続してログインに失敗しました。{1}アカウントは {2:yyyy年M月d日 H時m分s秒} までロックされます。", refLoginRetryCount, Environment.NewLine, refLoginLockdownExpires))

                                        Throw New InvalidOperationException(String.Format("UserId={0}：{1}", sessionmodel.UserId, StartController.LOCKDOWN_ERROR_MESSAGE))

                                    Case Else
                                        ' 不明なエラー
                                        Throw New InvalidOperationException(String.Format("UserId={0}：{1}", sessionmodel.UserId, StartController.UNEXPECTED_ERROR_MESSAGE))

                                End Select
                            Catch
                                Throw
                            End Try


                        End If
                    Else 'エラーメッセージあり＞メッセージを返却
                        ' 独自にエラーメッセージを用意
                        Dim errorMessage As New Dictionary(Of String, String)()

                        For Each key As String In errorList
                            Select Case key
                                Case "UserId"
                                    errorMessage.Add("UserId", "このユーザーIDは既に使用されています。")
                                Case "OpenId"
                                    errorMessage.Add("OpenId", "このOpenIDは既に登録されています。")
                                Case Else
                                    '未使用
                            End Select
                        Next
                        Me.TempData("ErrorMessage") = errorMessage

                        ' ビューを返却
                        Return View("RegisterId", model)
                    End If
                Else
                    'ログ出力(本登録エラー)
                    AccessLogWorker.WriteAccessLog(Me.HttpContext, String.Empty, AccessLogWorker.AccessTypeEnum.Error, String.Format("OpenID={0}：{1}", sessionmodel.UserId, StartController.UNEXPECTED_ERROR_MESSAGE))
                End If

            Else
                ' 検証失敗 ' 独自にエラーメッセージを用意
                Dim errorMessage As New Dictionary(Of String, String)()

                For Each key As String In Me.ModelState.Keys
                    Select Case key
                        Case "model.BirthYear", "model.BirthMonth", "model.BirthDay"
                            ' 生年月日
                            If Not errorMessage.ContainsKey("model.BirthYear") Then
                                For Each e As ModelError In Me.ModelState(key).Errors
                                    errorMessage.Add("model.BirthYear", e.ErrorMessage)
                                Next
                            End If
                        Case Else
                            If Not errorMessage.ContainsKey(key) AndAlso
                                 Me.ModelState(key).Errors.Count > 0 Then
                                errorMessage.Add(key, Me.ModelState(key).Errors.First.ErrorMessage)
                                'For Each e As ModelError In
                                '    errorMessage.Add(key, e.ErrorMessage)
                                'Next
                            End If
                    End Select
                Next
                Me.TempData("ErrorMessage") = errorMessage
                ' ビューを返却
                Return View("RegisterId", model)
            End If
        End If

        Return Me.RedirectToAction("LoginById")

    End Function

#End Region

#Region "「初期設定」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function Setup() As ActionResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()

        ' 編集対象のモデルを作成
        Dim inputModel As StartSetupInputModel = StartSetupWorker.CreateInputModel(mainModel)

        ' モデルをキャッシュへ格納（入力検証エラー時の再展開用）
        mainModel.SetInputModelCache(inputModel)

        ' ビューを返却
        Return View("Setup", inputModel)

    End Function

    <HttpPost()>
    <QyAuthorize()>
    <QyActionMethodSelector("Prev")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <ValidateInput(False)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function SetupResult(stepMode As Byte) As ActionResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim inputModel As StartSetupInputModel = mainModel.GetInputModelCache(Of StartSetupInputModel)()

        Select Case inputModel.StepMode
            Case 2
                ' ステップ 1 へ遷移
                inputModel.StepMode = 1

                ' モデルをキャッシュへ格納
                mainModel.SetInputModelCache(inputModel)

                ' ビューを返却
                Return View("Setup", inputModel)

            Case 3
                ' ステップ 2 へ遷移
                inputModel.StepMode = 2

                ' モデルをキャッシュへ格納
                mainModel.SetInputModelCache(inputModel)

                ' ビューを返却
                Return View("Setup", inputModel)

            Case Else
                ' ステップが不正
                Throw New InvalidOperationException("初期設定のステップが不正です。")

        End Select

    End Function

    <HttpPost()>
    <QyAuthorize()>
    <QyActionMethodSelector("Next")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <ValidateInput(False)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function SetupResult(model As StartSetupInputModel) As ActionResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim inputModel As StartSetupInputModel = mainModel.GetInputModelCache(Of StartSetupInputModel)()

        ' モデルへ入力値をを反映
        inputModel.UpdateByInput(model)

        ' モデルの検証状態を確認
        If Me.ModelState.IsValid Then
            ' 検証成功
            Select Case inputModel.StepMode
                Case 1
                    ' ステップ 2 へ遷移
                    inputModel.StepMode = 2

                    ' モデルをキャッシュへ格納
                    mainModel.SetInputModelCache(inputModel)

                    ' ビューを返却
                    Return View("Setup", inputModel)

                Case 2
                    ' ステップ 3 へ遷移
                    inputModel.StepMode = 3

                    ' モデルをキャッシュへ格納
                    mainModel.SetInputModelCache(inputModel)

                    ' ビューを返却
                    Return View("Setup", inputModel)

                Case 3
                    ' 登録
                    If StartSetupWorker.Edit(mainModel, inputModel) Then
                        ' モデルをキャッシュからクリア
                        mainModel.RemoveInputModelCache(Of StartSetupInputModel)()
                    End If

                    ' 「ホーム」画面へ遷移
                    Return Redirect(Me.MakeRedirectUri(Url.Action("Home", "Portal")))  ' RedirectToAction("Home", "Portal")

                Case Else
                    ' ステップが不正
                    Throw New InvalidOperationException("初期設定のステップが不正です。")

            End Select
        Else
            ' 検証失敗

            ' 独自にエラーメッセージを用意しビューに渡す
            Dim errorMessage As New Dictionary(Of String, String)()

            For Each key As String In Me.ModelState.Keys
                For Each e As ModelError In Me.ModelState(key).Errors
                    Dim m As String = e.ErrorMessage

                    If m.StartsWith("The") And m.EndsWith("field is required.") Then
                        m = m.Replace("The", String.Empty).Replace("field is required.", String.Empty).Replace(" ", String.Empty) + "を入力してください。"
                    Else
                        m = m.Replace("フィールドが必要です", "を入力してください").Replace(" ", String.Empty)
                    End If

                    errorMessage.Add(key, m)
                    'errorMessage.Add(key, e.ErrorMessage.Replace("フィールドが必要です", "を入力してください").Replace(" ", String.Empty))
                Next
            Next

            Me.TempData("ErrorMessage") = errorMessage

            ' ビューを返却
            Return View("Setup", inputModel)
        End If

    End Function

    <HttpPost()>
    <QyAuthorize()>
    <QyActionMethodSelector("Skip")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <ValidateInput(False)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function SetupResult() As ActionResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim inputModel As StartSetupInputModel = mainModel.GetInputModelCache(Of StartSetupInputModel)()

        '' 登録
        'If StartSetupWorker.Edit(mainModel, inputModel) Then
        '    ' モデルをキャッシュからクリア
        '    mainModel.RemoveInputModelCache(Of StartSetupInputModel)()
        'End If
        ' 「ホーム」画面へ遷移
        Return RedirectToAction("Home", "Portal")

        'Return New StartSetupJsonResult() With {.IsSuccess = Boolean.TrueString}.ToJsonResult()

    End Function

#End Region

#Region "「初期設定」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function Setup2() As ActionResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()

        ' 編集対象のモデルを作成
        Dim inputModel As StartSetupInputModel2 = StartSetupWorker2.CreateInputModel(mainModel)

        ' モデルをキャッシュへ格納（入力検証エラー時の再展開用）
        mainModel.SetInputModelCache(inputModel)

        ' ビューを返却
        Return View("Setup2", inputModel)

    End Function

    <HttpPost()>
    <QyAuthorize()>
    <QyActionMethodSelector("Prev")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <ValidateInput(False)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function SetupResult2(stepMode As Byte) As ActionResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim inputModel As StartSetupInputModel2 = mainModel.GetInputModelCache(Of StartSetupInputModel2)()

        Select Case inputModel.StepMode
            'Case 2
            '    ' ステップ 1 へ遷移
            '    inputModel.StepMode = 1

            '    ' モデルをキャッシュへ格納
            '    mainModel.SetInputModelCache(inputModel)

            '    ' ビューを返却
            '    Return View("Setup2", inputModel)

            Case 3
                ' ステップ 1 へ遷移
                inputModel.StepMode = 1

                ' モデルをキャッシュへ格納
                mainModel.SetInputModelCache(inputModel)

                ' ビューを返却
                Return View("Setup2", inputModel)

            Case Else
                ' ステップが不正
                Throw New InvalidOperationException("初期設定のステップが不正です。")

        End Select

    End Function

    <HttpPost()>
    <QyAuthorize()>
    <QyActionMethodSelector("Next")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <ValidateInput(False)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function SetupResult2(model As StartSetupInputModel2) As ActionResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim inputModel As StartSetupInputModel2 = mainModel.GetInputModelCache(Of StartSetupInputModel2)()

        ' モデルへ入力値をを反映
        inputModel.UpdateByInput(model)

        ' モデルの検証状態を確認
        If Me.ModelState.IsValid Then
            ' 検証成功
            Select Case inputModel.StepMode
                Case 1
                    ' ステップ 3 へ遷移
                    inputModel.StepMode = 3

                    ' モデルをキャッシュへ格納
                    mainModel.SetInputModelCache(inputModel)

                    ' ビューを返却
                    Return View("Setup2", inputModel)

                    'Case 2
                    '    ' ステップ 3 へ遷移
                    '    inputModel.StepMode = 3

                    '    ' モデルをキャッシュへ格納
                    '    mainModel.SetInputModelCache(inputModel)

                    '    ' ビューを返却
                    '    Return View("Setup2", inputModel)

                Case 3

                    Dim returnVal As New Dictionary(Of String, String)

                    ' 登録
                    If StartSetupWorker2.Edit(mainModel, inputModel) Then
                        ' モデルをキャッシュからクリア
                        mainModel.RemoveInputModelCache(Of StartSetupInputModel2)()
                    End If

                    ' 「ホーム」画面へ遷移
                    'Return Redirect(Me.MakeRedirectUri(Url.Action("Home", "Portal")))  ' RedirectToAction("Home", "Portal")
                    returnVal.Add("Url", Me.MakeRedirectUri(Url.Action("Home", "Portal")))

                    Return New StartSetup2JsonResult() With {
                        .IsSuccess = Boolean.TrueString,
                        .Values = returnVal
                    }.ToJsonResult()

                Case Else
                    ' ステップが不正
                    Throw New InvalidOperationException("初期設定のステップが不正です。")

            End Select
        Else
            ' 検証失敗

            Select Case inputModel.StepMode
                Case 3

                    Dim returnVal As New Dictionary(Of String, String)

                    For Each key As String In Me.ModelState.Keys
                        For Each e As ModelError In Me.ModelState(key).Errors
                            returnVal.Add(key, e.ErrorMessage)
                        Next
                    Next

                    Return New StartSetup2JsonResult() With {
                        .IsSuccess = Boolean.FalseString,
                        .Values = returnVal
                    }.ToJsonResult()

                Case Else
                    ' 独自にエラーメッセージを用意しビューに渡す
                    Dim errorMessage As New Dictionary(Of String, String)()

                    For Each key As String In Me.ModelState.Keys
                        For Each e As ModelError In Me.ModelState(key).Errors
                            Dim m As String = e.ErrorMessage

                            If m.StartsWith("The") And m.EndsWith("field is required.") Then
                                m = m.Replace("The", String.Empty).Replace("field is required.", String.Empty).Replace(" ", String.Empty) + "を入力してください。"
                            Else
                                m = m.Replace("フィールドが必要です", "を入力してください").Replace(" ", String.Empty)
                            End If

                            errorMessage.Add(key, m)
                            'errorMessage.Add(key, e.ErrorMessage.Replace("フィールドが必要です", "を入力してください").Replace(" ", String.Empty))
                        Next
                    Next

                    Me.TempData("ErrorMessage") = errorMessage

                    ' ビューを返却
                    Return View("Setup2", inputModel)
            End Select
        End If

    End Function

    <HttpPost()>
    <QyAjaxOnly()>
    <QyAuthorize()>
    <QyActionMethodSelector("Calc")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <ValidateInput(False)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function SetupResult2(model As StartSetupInputModel2, buttonType As String) As JsonResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim inputModel As StartSetupInputModel2 = mainModel.GetInputModelCache(Of StartSetupInputModel2)()

        ' モデルへ入力値をを反映
        inputModel.UpdateByInput(model)

        Dim returnVal As New Dictionary(Of String, String)
        Dim success As String = Boolean.FalseString

        ' モデルの検証状態を確認
        If Me.ModelState.IsValid Then
            ' 検証成功

            returnVal.Add("NowTargetCalorieIn", inputModel.NowTargetCalorieIn.ToString())
            success = Boolean.TrueString

        Else
            ' 検証失敗

            For Each key As String In Me.ModelState.Keys
                For Each e As ModelError In Me.ModelState(key).Errors
                    returnVal.Add(key, e.ErrorMessage)
                Next
            Next

        End If

        Return New StartSetup2JsonResult() With {
            .IsSuccess = success,
            .Values = returnVal
        }.ToJsonResult()

    End Function

    <HttpPost()>
    <QyAuthorize()>
    <QyActionMethodSelector("Skip")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <ValidateInput(False)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function SetupResult2() As ActionResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim inputModel As StartSetupInputModel2 = mainModel.GetInputModelCache(Of StartSetupInputModel2)()

        '' 登録
        'If StartSetupWorker.Edit(mainModel, inputModel) Then
        '    ' モデルをキャッシュからクリア
        '    mainModel.RemoveInputModelCache(Of StartSetupInputModel2)()
        'End If
        ' 「ホーム」画面へ遷移
        Return RedirectToAction("Home", "Portal")

        'Return New StartSetupJsonResult() With {.IsSuccess = Boolean.TrueString}.ToJsonResult()

    End Function

#End Region

#Region "「再登録」画面"

    <HttpGet()>
    <QyLogging()>
    Public Function Reregister(Optional openId As Byte = 1) As ActionResult
        AppleIdLoginWorker.DebugLog($"openId ={openId}")
        'OpenIDの種別毎に処理を分ける
        Dim openIdType As QsApiOpenIdTypeEnum = DirectCast([Enum].ToObject(GetType(QsApiOpenIdTypeEnum), openId), QsApiOpenIdTypeEnum)
        QySessionHelper.SetItem(Session, "OpenIdType", openId)

        If openIdType = QsApiOpenIdTypeEnum.AppleId Then

            Dim loginModel As AppleIdLoginModel = Nothing
            QySessionHelper.GetItem(Session, GetType(AppleIdLoginModel).Name, loginModel)

            Dim accountkey As Guid = Guid.Empty
            If loginModel IsNot Nothing AndAlso StartReregisterWorker.RegisteredOpenId(Me.Session.SessionID, loginModel.UserId, QsApiOpenIdTypeEnum.AppleId, accountkey) Then

                Dim account As KeyValuePair(Of String, Guid) = New KeyValuePair(Of String, Guid)(loginModel.UserId, accountkey)

                '過去に登録があるので復活するアカウントキーを保持
                QySessionHelper.SetItem(Session, "ComeBackAccount", account)

                Return View()

            Else

                '' ここは検証用なので消す
                'If StartReregisterWorker.RegisteredOpenId(Me.Session.SessionID, "https://test.connect.auone.jp/net/id/hny_rt_net/cca/a/kddi_sxzftxqgtsl5a20310205161339", QsApiOpenIdTypeEnum.AuId, accountkey) Then

                '    Dim account As KeyValuePair(Of String, Guid) = New KeyValuePair(Of String, Guid)("https://test.connect.auone.jp/net/id/hny_rt_net/cca/a/kddi_sxzftxqgtsl5a20310205161339", accountkey)

                '    '過去に登録があるので復活するアカウントキーを保持
                '    QySessionHelper.SetItem(Session, "ComeBackAccount", account)

                '    Return View()
                'End If

            End If

        Else
            Dim loginModel As AuIdLoginModel = Nothing
            QySessionHelper.GetItem(Session, GetType(AuIdLoginModel).Name, loginModel)

            Dim accountkey As Guid = Guid.Empty
            If loginModel IsNot Nothing AndAlso StartReregisterWorker.RegisteredOpenId(Me.Session.SessionID, loginModel.UserId, QsApiOpenIdTypeEnum.AuId, accountkey) Then

                Dim account As KeyValuePair(Of String, Guid) = New KeyValuePair(Of String, Guid)(loginModel.UserId, accountkey)

                '過去に登録があるので復活するアカウントキーを保持
                QySessionHelper.SetItem(Session, "ComeBackAccount", account)

                Return View()

            Else

                ' ここは検証用なので消す
                If StartReregisterWorker.RegisteredOpenId(Me.Session.SessionID, "https://test.connect.auone.jp/net/id/hny_rt_net/cca/a/kddi_sxzftxqgtsl5a20310205161339", QsApiOpenIdTypeEnum.AuId, accountkey) Then

                    Dim account As KeyValuePair(Of String, Guid) = New KeyValuePair(Of String, Guid)("https://test.connect.auone.jp/net/id/hny_rt_net/cca/a/kddi_sxzftxqgtsl5a20310205161339", accountkey)

                    '過去に登録があるので復活するアカウントキーを保持
                    QySessionHelper.SetItem(Session, "ComeBackAccount", account)

                    Return View()
                End If

            End If

        End If

        '新規登録フローへ返す
        Return RedirectToAction("Register", "Start", New With {.openId = openId})

    End Function


    <HttpGet()>
    <QyLogging()>
    Public Async Function ReregisterResult() As Task(Of ActionResult)
        Dim account As KeyValuePair(Of String, Guid) = New KeyValuePair(Of String, Guid)()
        QySessionHelper.GetItem(Session, "ComeBackAccount", account)
        Dim openId As Byte = Byte.MinValue
        QySessionHelper.GetItem(Session, "OpenIdType", openId)

        If Not String.IsNullOrWhiteSpace(account.Key) AndAlso StartReregisterWorker.Reregister(Me.Session.SessionID, account.Key, QsApiOpenIdTypeEnum.AuId, account.Value) Then

            If openId = 3 Then
                'AppleID
                Dim loginModel As AppleIdLoginModel = Nothing
                QySessionHelper.GetItem(Me.Session, GetType(AppleIdLoginModel).Name, loginModel)

                ' 手動ログイン開始
                Dim refLoginModel As New AppleIdLoginModel()
                Dim refAuthorAccount As New AuthorAccountItem()
                Dim refApiAuthorizeKey As Guid = Guid.Empty
                Dim refApiAuthorizeExpires As Date = Date.MinValue
                Dim refApiAuthorizeKey2 As Guid = Guid.Empty
                Dim refApiAuthorizeExpires2 As Date = Date.MinValue
                Dim refLoginRetryCount As Byte = Byte.MinValue
                Dim refLoginLockdownExpires As Date = Date.MinValue
                Dim refIsSettingComplete As Boolean = False
                Dim userAgent As String = If(Me.HttpContext IsNot Nothing AndAlso Me.HttpContext.Request IsNot Nothing, Me.HttpContext.Request.UserAgent, String.Empty)

                AppleIdLoginWorker.DebugLog(String.Format("{0}:{1}{2}", Now, loginModel.UserId, vbCrLf))
                Select Case AppleIdLoginWorker.Auth(
                    Me.Session.SessionID,
                    loginModel.UserId,
                    refLoginModel,
                    refAuthorAccount,
                    refApiAuthorizeKey,
                    refApiAuthorizeExpires,
                    refApiAuthorizeKey2,
                    refApiAuthorizeExpires2,
                    refLoginRetryCount,
                    refLoginLockdownExpires,
                    refIsSettingComplete
                )
                    Case QsApiOpenIdLoginResultTypeEnum.Success
                        AppleIdLoginWorker.DebugLog(String.Format("{0}:{1}{2}", Now, "Success", vbCrLf))
                        'ログイン状態に移行
                        If QyLoginHelper.ToLogin(Me.Session,
                            Me.Response,
                            refAuthorAccount,
                            refApiAuthorizeKey,
                            refApiAuthorizeExpires,
                            refApiAuthorizeKey2,
                            refApiAuthorizeExpires2,
                            False) Then
                            AppleIdLoginWorker.DebugLog(String.Format("{0}:{1}{2}", Now, "Success", vbCrLf))

                            ' ログインポイント付与（初回は初回登録ポイントも）＞初回登録の５００ポイントはなしに。プレミアム会員初登録時にポイント付与する
                            Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
                            Dim actionDate As Date = Now
                            Dim limit As Date = New Date(actionDate.Year, actionDate.Month, 1).AddMonths(7).AddDays(-1) ' ポイント有効期限は 6 ヶ月後の月末
                            Dim pointItemList As New List(Of QolmsPointGrantItem)
                            '暫定対応
                            mainModel.AuthorAccount.MembershipType = CType(PremiumWorker.GetMemberShipType(mainModel), QyMemberShipTypeEnum)

                            pointItemList.Add(New QolmsPointGrantItem(mainModel.AuthorAccount.MembershipType, actionDate, Guid.NewGuid.ToApiGuidString(), QyPointItemTypeEnum.Login, limit)) 'loginポイント
                            Await Task.Run(
                                Sub()
                                    QolmsPointWorker.AddQolmsPoints(mainModel.ApiExecutor, mainModel.ApiExecutorName, mainModel.SessionId, mainModel.ApiAuthorizeKey,
                                    mainModel.AuthorAccount.AccountKey, pointItemList)
                                End Sub
                            )
                            ''AuidLoginModelをセッションから削除
                            'QySessionHelper.RemoveItem(Me.Session, GetType(AuIdLoginModel).Name)

                            ''auIDによる自動ログインcookieの設定
                            'QyCookieHelper.DisableAutoAuIdLoginCookie(Me.HttpContext.Response)
                            ''３週間
                            'QyCookieHelper.SetAutoAuIdLoginCookie(Me.HttpContext.Response, AUTOLOGIN_AUID, Date.Now.AddDays(AUTOLOGIN_EXPIRES))
                            AppleIdLoginWorker.DebugLog(String.Format("{0}:{1}{2}", Now, $"refIsSettingComplete={refIsSettingComplete}", vbCrLf))

                            If refIsSettingComplete = False Then Return RedirectToAction("Setup2")

                            Return Redirect(Me.MakeRedirectUri(String.Empty))

                        End If
                    Case QsApiOpenIdLoginResultTypeEnum.NewUser
                        QySessionHelper.SetItem(Me.Session, GetType(AppleIdLoginModel).Name, refLoginModel)
                        ''auIDによる自動ログインcookieの設定
                        'QyCookieHelper.DisableAutoAuIdLoginCookie(Me.HttpContext.Response)
                        ''３週間
                        'QyCookieHelper.SetAutoAuIdLoginCookie(Me.HttpContext.Response, AUTOLOGIN_AUID, Date.Now.AddDays(AUTOLOGIN_EXPIRES))
                        AppleIdLoginWorker.DebugLog(String.Format("{0}:{1}{2}", Now, $"NewUser", vbCrLf))
                        AccessLogWorker.DebugLog("~/App_Data/Log", "login.log", String.Format("{0}:{1}{2}", Now, "", vbCrLf), Me.HttpContext)

                        Return RedirectToAction("Reregister", "Start", New With {.openId = 3})

                    Case Else
                        'Throw New InvalidOperationException(String.Format("OpenId={0}：{1}", , StartController.UNEXPECTED_ERROR_MESSAGE))
                End Select
            Else
                'auID
                LoginByAuId()

            End If

        End If

        Return RedirectToAction("LoginByAuId")

    End Function

#End Region

#Region "「ログイン方法を編集」画面"

    <HttpGet()>
    <QyAuthorize()>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function LoginEdit() As ActionResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim model As StartLoginEditInputModel = StartLoginEditWorker.CreateViewModel(mainModel)

        mainModel.SetInputModelCache(model)

        Return View(model)

    End Function

    <HttpPost()>
    <QyAuthorize()>
    <QyActionMethodSelector("JOTOID")>
    <ValidateAntiForgeryToken(Order:=Integer.MaxValue)>
    <ValidateInput(False)>
    <QyApiAuthorize()>
    <QyLogging()>
    Public Function LoginEditResult(model As StartLoginEditInputModel) As ActionResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim errorString As String = String.Empty

        If Me.ModelState.IsValid Then

            If StartLoginEditWorker.Edit(mainModel, model, errorString) Then

                'JOTOIDの編集があったらJOTOIDの自動ログインを削除            
                QyCookieHelper.DisableRememberLoginCookie(response)
                QyCookieHelper.DisableRememberIdCookie(response)

                Return New StartLoginEditJsonResult() With {
                    .IsSuccess = Boolean.TrueString}.ToJsonResult()

            Else

                Dim dic As New Dictionary(Of String, String)
                dic.Add("UserId", errorString)

                Return New StartLoginEditJsonResult() With {
                    .IsSuccess = Boolean.FalseString,
                    .Massages = dic}.ToJsonResult()
            End If

        End If
        ' 検証に失敗
        Dim errorMessage As New Dictionary(Of String, String)()

        For Each key As String In Me.ModelState.Keys
            For Each e As ModelError In Me.ModelState(key).Errors
                If errorMessage.ContainsKey(key) Then
                    errorMessage(key) = errorMessage(key) + HttpUtility.HtmlEncode("," + e.ErrorMessage)
                Else
                    errorMessage.Add(key, HttpUtility.HtmlEncode(e.ErrorMessage))

                End If
            Next
        Next

        Return New StartLoginEditJsonResult() With {
            .IsSuccess = Boolean.FalseString,
            .Massages = errorMessage}.ToJsonResult()

    End Function

    <HttpGet()>
    <QyLogging()>
    Public Async Function LoginEditResult() As Task(Of ActionResult)

        Dim LoginModel As AuIdLoginModel = Nothing
        Try
            LoginModel = Await AuIdLoginWorker.GetAuthInfAync(String.Empty, False)
        Catch ex As Exception
            AccessLogWorker.WriteErrorLog(
                        Me.HttpContext,
                        String.Empty,
                        String.Format("ディスカバリーに失敗 Wow?={0}", False))
            AuIdLoginWorker.DebugLog(ex.Message)
            Return RedirectToAction("Login")
        End Try

        If LoginModel IsNot Nothing Then

            QySessionHelper.RemoveItem(Me.Session, GetType(AuIdLoginModel).Name)
            QySessionHelper.SetItem(Me.Session, GetType(AuIdLoginModel).Name, LoginModel)

            '  Return RedirectToAction("Register")
            Dim url As String = LoginModel.GetAuthorizationRequestUrl()
            AuIdLoginWorker.DebugLog(url)
            Try
                'テスト用ログ
                Dim log As String = System.IO.Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Log"), "login.log")
                System.IO.File.AppendAllText(log, String.Format("{0}:{1}{2}", Now, url, vbCrLf))
                System.IO.File.AppendAllText(log, String.Format("{0}:{1}{2}", Now, LoginModel.UserId, vbCrLf))
                System.IO.File.AppendAllText(log, String.Format("{0}:{1}{2}", Now, LoginModel.OpenIdConfig, vbCrLf))
            Catch ex As Exception

            End Try

            Return Redirect(url)
        End If

        Return RedirectToAction("LoginEdit")

        'Dim str As String = "abcd"

        'Return RedirectToAction("LoginEditAuIdResult", New With {.state = str, .code = str})

        'Return View()

    End Function


    ''' <summary>
    ''' Auの認証が終わったらこちらにリダイレクトされる
    ''' </summary>
    ''' <param name="state"></param>
    ''' <param name="code"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <QyLogging()>
    Public Async Function LoginEditAuIdResult(state As String, Optional code As String = "") As Threading.Tasks.Task(Of ActionResult)

        'AuIdLoginWorker.DebugLog(String.Format("state = {0} ,code = {1}", state, code))
        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()

        Dim LoginModel As AuIdLoginModel = Nothing
        QySessionHelper.GetItem(Me.Session, GetType(AuIdLoginModel).Name, LoginModel)

        If LoginModel IsNot Nothing Then

            'AuIdLoginWorker.DebugLog("OpenIdの取得")
            Dim openId As String = AuIdLoginWorker.GetOpenId(LoginModel, code)
            'AuIdLoginWorker.DebugLog(String.Format("openId = {0}", openId))

            'OpenIdがとれたら
            If Not String.IsNullOrWhiteSpace(openId) Then
                '登録する

                Dim model As New StartLoginEditInputModel() With {
                    .Accountkey = mainModel.AuthorAccount.AccountKey,
                    .openId = openId,
                    .OpenIdType = 1
                } 'auIDの登録
                Dim str As String = String.Empty

                Me.TempData("Result") = StartLoginEditWorker.Edit(mainModel, model, str)
                Me.TempData("ResultDispFlag") = True
                Me.TempData("Message") = str

            End If

        End If

        Return RedirectToAction("LoginEdit")

    End Function

#End Region
    
#Region "「カロミル説明」画面"

    <HttpGet()>
    <QyLogging()>
    Public Function CalomealDiscription(optional isFirst As Boolean = false) As ActionResult

        ViewData("First") = isFirst
        Return View()

    End Function

#End Region

#Region "「健診画面から戻る」画面"

    <HttpGet()>
    <QyLogging()>
    Public Function ExamintaionReturn(fromPageNo As Nullable(Of Byte)) As ActionResult
                            
        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        If mainModel IsNot Nothing Then
            Dim authorizeKey As Guid = Guid.Empty
            Dim authorizeExpires As Date = Date.MinValue
            ' API 認証 キー および API 認証有効期限を取得
            ApiAuthorizeWorker.NewKey(mainModel.AuthorAccount.AccountKey, mainModel.SessionId,authorizeKey, authorizeExpires)
                            
            mainModel.SetApiAuthorizeKey(authorizeKey)
            mainModel.SetApiAuthorizeExpires(authorizeExpires)
            
            '戻り先の指定
            Dim fromPageNoType As QyPageNoTypeEnum = QyPageNoTypeEnum.None
            If fromPageNo.HasValue Then fromPageNoType = fromPageNo.ToString().TryToValueType(QyPageNoTypeEnum.None)

            If fromPageNoType = QyPageNoTypeEnum.PortalCompanyConnectionHome Then
                '法人連携画面
                Return RedirectToAction("companyconnectionhome","portal")
                
            Else

                Return RedirectToAction("home","portal")
            End If
            
        Else 
                            
            'ログイン画面へリダイレクトする
            Return RedirectToAction("login")

        End If            

    End Function

#End Region

#Region "「QRで画面呼び出し」"

    ''' <summary>
    ''' 連携システムID登録 画面を呼び出す外部リンク(QR)
    ''' </summary>
    ''' <param name="no"></param>
    ''' <param name="id"></param>
    ''' <returns></returns>
    <HttpGet()>
    <QyLogging()>
    Public Function AppendLinkageId(no As String, id As String) As ActionResult

        '暗号化自体が間違ってたらここではじく？ログインしてからはじく？
        If String.IsNullOrWhiteSpace(no) OrElse String.IsNullOrWhiteSpace(id) Then
            Throw New Exception("") 'Error Throw かどうかは確認
        End If

        Return RedirectToAction("appendlinkageid", "portal", New With {.linkageSystemNo = no, .linkageSystemId = id})

    End Function

#End Region

#Region "「QRで画面呼び出し(他社)」"

    <HttpGet()>
    <QyLogging()>
    Public Function CalomealDynamicLink(jwt As String) As ActionResult

        AccessLogWorker.DebugLog("~/App_Data/Log", "loginReturnUrl.log", String.Format("{0}:{1}{2}", Now, $"CalomealDynamicLink", vbCrLf), Me.HttpContext)
        AccessLogWorker.DebugLog("~/App_Data/Log", "loginReturnUrl.log", String.Format("{0}:{1}{2}", Now, $"1:{jwt}", vbCrLf), Me.HttpContext)
        Return RedirectToAction("calomealjwtdynamiclink", "note", New With {.jwt = jwt})

    End Function

    <HttpGet()>
    <QyLogging()>
    Public Function AlkooDynamicLink(pageName As String, code As String, ginowanjoin As String) As ActionResult

        AccessLogWorker.DebugLog("~/App_Data/Log", "loginReturnUrl.log", String.Format("{0}:{1}{2}", Now, $"AlkooDynamicLink", vbCrLf), Me.HttpContext)
        AccessLogWorker.DebugLog("~/App_Data/Log", "loginReturnUrl.log", String.Format("{0}:{1}{2}", Now, $"1:{pageName} {code}", vbCrLf), Me.HttpContext)
        Return RedirectToAction("alkoodynamiclink", "note", New With {.urlSettingName = pageName, .code = code, .ginowanjoin = ginowanjoin})

    End Function

#End Region

#Region "「カロミル保健指導」画面"

    <HttpGet()>
    <QyLogging()>
    Public Function CalomealHokenshido(Optional isFirst As Boolean = False) As ActionResult

        Return RedirectToAction("calomealhokenshido", "note")

    End Function

#End Region

    '#Region "「ダイナミックリンク呼び出し」画面"

    '    <HttpGet()>
    '    <QyLogging()>
    '    Public Function AppLink(token As String) As ActionResult

    '        Dim useragent As String = Me.HttpContext.Request.UserAgent
    '        ViewData("UserAgent") = useragent

    '        Dim url As String = $"https://devjoto.qolms.com/note/calomeal"
    '        '// 設定へ外だし

    '        Dim jotodeeplink As String = ConfigurationManager.AppSettings("DeepLinkWebView")
    '        Dim deeplink As String = If(String.IsNullOrWhiteSpace(jotodeeplink), "jotohdr:/tab/custom/5d67c170", jotodeeplink)
    '        Dim query As String = $"url={HttpUtility.UrlEncode(url)}"
    '        'URLパラメータを作成してメール送信
    '        'iOS の例: {deeplink}://?goto=login
    '        'Android の例: intent://?goto=login#Intent;scheme={deeplink};package={deeplink};S.browser_fallback_url=?goto=login;end";

    '        Dim iosUrl As String = $"{deeplink}?{query}"
    '        Dim androidUrl As String = $"intent://?{query}#Intent;scheme={deeplink};package={deeplink};S.browser_fallback_url=?{query};end"";"

    '        ViewData("AppLink") = iosUrl

    '        Return View()

    '    End Function

    '#End Region


#Region "アプリケーション ゲートウェイ の 正常性 プローブ 用応答"

    ''' <summary>
    ''' アプリケーション ゲートウェイ の 正常性 プローブ の要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクション の結果。
    ''' </returns>
    <HttpGet()>
    <OutputCache(CacheProfile:="DisableCacheProfile")>
    Public Function Health() As ActionResult

        ' HTTP ステータス 200 を返すだけで良い
        Return New HttpStatusCodeResult(System.Net.HttpStatusCode.OK)

    End Function

#End Region

#End Region

#Region "「ログアウト」"

    ''' <summary>
    ''' シングルログアウト実装が必要。これが呼ばれるらしいが・・・とりあえずテスト。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <QyLogging()>
    Public Function AuIdLogout(Optional ByVal callback As String = "onLogout") As JavaScriptResult

        '多分今は呼ばれてない
        AuIdLoginWorker.DebugLog("ログアウトらしい！")
        If Request.QueryString IsNot Nothing AndAlso Request.QueryString.Count > 0 Then
            For i As Integer = 0 To Request.QueryString.Count - 1
                AuIdLoginWorker.DebugLog(String.Format("AuIdLogout:{0}={1}", Request.QueryString.Keys.Item(i), Request.QueryString.Item(i)))
            Next
        End If
        If Request.Params IsNot Nothing AndAlso Request.Params.Count > 0 Then
            For i As Integer = 0 To Request.Params.Count - 1
                AuIdLoginWorker.DebugLog(String.Format("AuidLogout:{0}={1}", Request.Params.Keys.Item(i), Request.Params.Item(i)))
            Next
        End If
        If IsLogin Then
            'ログアウト処理
            Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()

            Dim isWow As Boolean = mainModel.AuthorAccount.LoginByWowId
            Dim message As String = If(mainModel Is Nothing, String.Empty, String.Format("AccountKey={0}：Name={1}：{2}", mainModel.AuthorAccount.AccountKey, mainModel.AuthorAccount.Name, StartController.LOGOUT_MESSAGE))

            ' ログ出力
            AccessLogWorker.WriteAccessLog(Me.HttpContext, String.Empty, AccessLogWorker.AccessTypeEnum.Logout, message)

            ' ログアウト
            QyLoginHelper.ToLogout(Me.Session, Me.Response, False)
            Return JavaScript(String.Format("{0}({1});", callback, 0))
        Else
            'エラー通知
            Return JavaScript(String.Format("{0}({1});", callback, 1))
        End If

    End Function

    ''' <summary>
    ''' ログアウト要求を処理します。
    ''' </summary>
    ''' <param name="clearRemember"></param>
    ''' <returns>
    ''' アクションの結果。
    ''' </returns>
    ''' <remarks></remarks>
    <HttpGet()>
    <QyLogging()>
    Public Function Logout(clearRemember As Nullable(Of Boolean)) As ActionResult

        Dim mainModel As QolmsYappliModel = Me.GetQolmsYappliModel()
        Dim isWow As Boolean = mainModel.AuthorAccount.LoginByWowId
        Dim message As String = If(mainModel Is Nothing, String.Empty, String.Format("AccountKey={0}：Name={1}：{2}", mainModel.AuthorAccount.AccountKey, mainModel.AuthorAccount.Name, StartController.LOGOUT_MESSAGE))

        ' ログ出力
        AccessLogWorker.WriteAccessLog(Me.HttpContext, String.Empty, AccessLogWorker.AccessTypeEnum.Logout, message)

        ' ログアウト
        QyLoginHelper.ToLogout(Me.Session, Me.Response, If(clearRemember.HasValue, clearRemember.Value, False))

        ' 「ログイン」画面へ遷移 
        ' Return RedirectToAction("Login")
        If String.IsNullOrWhiteSpace(mainModel.AuthorAccount.OpenId) OrElse mainModel.AuthorAccount.OpenIdType = 3 Then
            'JOTOID もしくは AppleId
            Return RedirectToAction("LogoutView")

        End If

        Return Redirect(AuIdLoginModel.GetLogoutUrl(isWow))
    End Function

    'ヤプリ用に何もできない(ログイン画面に遷移できない）ログアウト結果ビュー
    Public Function LogoutView() As ActionResult
        Return RedirectToAction("Login","Start")
        'Return View("logout")
    End Function

#End Region

#Region "パーシャル ビュー アクション"

#Region "「本登録」画面　規約ダイアログ"

    ''' <summary>
    ''' 「規約ダイアログ」パーシャルビューの表示要求を処理します。
    ''' </summary>
    ''' <returns>
    ''' アクションの結果。
    ''' </returns>
    ''' <remarks></remarks>
    <ChildActionOnly()>
    Public Function StartRegisterDialogPartialView() As ActionResult

        Dim bodyStr As String = String.Empty
        Dim dialogBodyPath As String = Me.HttpContext.Server.MapPath("~/App_Data/TermsOfUse.txt")
        If Not String.IsNullOrWhiteSpace(dialogBodyPath) AndAlso IO.File.Exists(dialogBodyPath) Then
            bodyStr = IO.File.ReadAllText(dialogBodyPath)
        End If

        ' パーシャルビューを返却
        Return PartialView("_StartRegisterDialogPartialView", bodyStr)

    End Function

#End Region

#Region "「初期設定」画面用パーシャル ビュー"

    <ChildActionOnly()>
    Public Function StartSetupStepOnePartialView() As ActionResult

        ' パーシャル ビューを返却
        Return PartialView("_StartSetupStepOnePartialView", New StartSetupStepOnePartialViewModel(Me.GetPageViewModel(Of StartSetupInputModel)()))

    End Function

    <ChildActionOnly()>
    Public Function StartSetupStepTwoPartialView() As ActionResult

        ' パーシャル ビューを返却
        Return PartialView("_StartSetupStepTwoPartialView", New StartSetupStepTwoPartialViewModel(Me.GetPageViewModel(Of StartSetupInputModel)()))

    End Function

    <ChildActionOnly()>
    Public Function StartSetupStepThreePartialView() As ActionResult

        ' パーシャル ビューを返却
        Return PartialView("_StartSetupStepThreePartialView", New StartSetupStepThreePartialViewModel(Me.GetPageViewModel(Of StartSetupInputModel)()))

    End Function

#End Region

#Region "「初期設定」画面用パーシャル ビュー"

    <ChildActionOnly()>
    Public Function StartSetupStepOnePartialView2() As ActionResult

        ' パーシャル ビューを返却
        Return PartialView("_StartSetupStepOnePartialView2", New StartSetupStepOnePartialViewModel2(Me.GetPageViewModel(Of StartSetupInputModel2)()))

    End Function

    <ChildActionOnly()>
    Public Function StartSetupStepThreePartialView2() As ActionResult

        ' パーシャル ビューを返却
        Return PartialView("_StartSetupStepThreePartialView2", New StartSetupStepThreePartialViewModel2(Me.GetPageViewModel(Of StartSetupInputModel2)()))

    End Function

#End Region

#End Region

#Region "Private"

    Private Function MakeRedirectUri(returnUrl As String) As String
        Dim result As String = returnUrl

        AccessLogWorker.DebugLog("~/App_Data/Log", "loginReturnUrl.log", String.Format("{0}:{1}{2}", Now, $"MakeRedirectUri", vbCrLf), Me.HttpContext)
        AccessLogWorker.DebugLog("~/App_Data/Log", "loginReturnUrl.log", String.Format("{0}:{1}{2}", Now, $"1:{returnUrl}", vbCrLf), Me.HttpContext)

        'ヤプリ用の処理
        If Request.UserAgent.ToLower().Contains(USERAGENT_YAPPLI) Then
            result = returnUrl.ToLower()
            If result.Contains("premium") Then
                'プレミアム会員画面
                result = "native:/tab/custom/314fe19a"
            ElseIf result.Contains("portal") AndAlso result.Contains("search") Then
                '医療機関検索
                result = "native:/tab/custom/93650bb4"
                'ElseIf String.IsNullOrEmpty(result) = False Then
                '    'とにかく何か入ってたら
                '    If returnUrl.ToLower().EndsWith("result") Then result = Left(returnUrl, returnUrl.Length - "result".Length)
            ElseIf result.Contains("start") AndAlso result.Contains("logout") Then

                result = Url.Action("Home", "Portal")

            Else

                If String.IsNullOrEmpty(result) Then
                    'Home
                    'result = "native:/tab/scrollmenu/38634dbf"
                    result = Url.Action("Home", "Portal")

                ElseIf returnUrl.ToLower().EndsWith("result") Then
                    result = Left(returnUrl, returnUrl.Length - "result".Length)

                End If

            End If
        ElseIf Request.UserAgent.ToLower().Contains(USERAGENT_DATAUPLOADER) Then
            'データアップローダからのアクセスは全て同じ所(ダミーページ)へ返す
            result = Url.Action("DataUploaderAuth")
        Else
            '未指定の場合はPortal
            If String.IsNullOrEmpty(returnUrl) Then result = Url.Action("Home", "Portal")
            '～ResultというPostアクションと思われるUrlのときはResultを削る
            If returnUrl.ToLower().EndsWith("result") Then result = Left(returnUrl, returnUrl.Length - "result".Length)
        End If
        AccessLogWorker.DebugLog("~/App_Data/Log", "loginReturnUrl.log", String.Format("{0}:{1}{2}", Now, $"2:{result}", vbCrLf), Me.HttpContext)

        Return result
    End Function


#End Region

#Region "Test"

    'Public Class TestModel

    '    Public Property Name0 As New List(Of String)()

    '    Public Property Name1 As String = String.Empty

    '    Public Property Name2 As String = String.Empty

    '    Public Property Name3 As New List(Of String)()

    'End Class

    '<HttpGet()>
    'Public Function Test() As ActionResult

    '    Return View()

    'End Function

    '<HttpPost()>
    '<QyAjaxOnly()>
    'Public Function TestResult(model As TestModel) As JsonResult

    '    Return New CheckSessionJsonResult() With {.IsSuccess = Boolean.TrueString}.ToJsonResult()

    'End Function


    '''' <summary>
    '''' アプリケーション ゲートウェイ の 正常性 プローブ の要求を処理します。
    '''' </summary>
    '''' <returns>
    '''' アクション の結果。
    '''' </returns>
    '<HttpGet()>
    '<QjApiAuthorize()>
    '<OutputCache(CacheProfile:="DisableCacheProfile")>
    'Public Function Test() As ActionResult

    '    ' HTTP ステータス 200 を返すだけで良い
    '    TestWorker.JotoApiCall(Me.GetQolmsYappliModel())

    '    Return New HttpStatusCodeResult(System.Net.HttpStatusCode.OK)

    'End Function

#End Region

End Class
