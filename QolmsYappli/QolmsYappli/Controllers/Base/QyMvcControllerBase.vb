Imports System.Web.Configuration
Imports System.Web.Mvc

''' <summary>
''' 「コルムス ヤプリ サイト」で使用する、
''' MVC コントローラーの基本クラスを表します。
''' </summary>
''' <remarks></remarks>
Public MustInherit Class QyMvcControllerBase
    Inherits System.Web.Mvc.Controller

#Region "Constant"

    ''' <summary>
    ''' 一時データのディクショナリ内でのログイン済みフラグのキー名を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Protected Const KEY_IS_LOGIN As String = "IsLogin"

    ''' <summary>
    ''' 一時データのディクショナリ内でのエラー メッセージのキー名を表します。
    ''' </summary>
    ''' <remarks></remarks>
    Protected Const KEY_ERROR_MESSAGE As String = "ErrorMessage"

    ''' <summary>
    ''' 標準のエラー メッセージを表します。
    ''' </summary>
    ''' <remarks></remarks>
    Protected Const DEFAULT_ERROR_MESSAGE As String = "ページの表示に失敗しました。"

#End Region

#Region "Variable"

    ''' <summary>
    ''' Error 画面に表示するエラー メッセージを保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Private errorMessage As String = QyMvcControllerBase.DEFAULT_ERROR_MESSAGE

    ''' <summary>
    ''' JavaScript が有効かを保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Private _enableJavaScript As Boolean = False

    ''' <summary>
    ''' クッキーが有効かを保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Private _enableCookies As Boolean = False

    ''' <summary>
    ''' デバッグ ビルドかを保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Private _isDebug As Boolean = False

    ''' <summary>
    ''' デバッグ ログのフォルダパスを保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Private _debugLogPath As String = String.Empty
#End Region

#Region "Public Property"

    ''' <summary>
    ''' JavaScript が有効かを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property EnableJavaScript As Boolean

        Get
            Return Me._enableJavaScript
        End Get

    End Property

    ''' <summary>
    ''' クッキーが有効かを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property EnableCookies As Boolean

        Get
            Return Me._enableCookies
        End Get

    End Property

    ''' <summary>
    ''' デバッグ ビルドかを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IsDebug As Boolean

        Get
            Return Me._isDebug
        End Get

    End Property

    ''' <summary>
    ''' デバッグ ログのフォルダパスを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property DebugLogPath As String

        Get
            Return Me._debugLogPath
        End Get

    End Property

    ''' <summary>
    ''' ログイン中かを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected ReadOnly Property IsLogin As Boolean

        Get
            Return QyLoginHelper.CheckLogin(Me.Session, Me.Request)
        End Get

    End Property

    ''' <summary>
    ''' アクション ソースを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected ReadOnly Property ActionSource As String

        Get
            Try
                Return DirectCast(Me.ControllerContext.Controller.ValueProvider.GetValue("ActionSource").ConvertTo(GetType(String)), String)
            Catch
                Return String.Empty
            End Try
        End Get

    End Property

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="QyMvcControllerBase" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub New()

        MyBase.New()

#If DEBUG Then
        Me._isDebug = True
#Else
        Me._isDebug = False
#End If

    End Sub

#End Region

#Region "Protected Method"

    ''' <summary>
    ''' アクション メソッド の呼び出し前に呼び出されます。
    ''' </summary>
    ''' <param name="filterContext">現在の要求および アクション に関する情報。</param>
    ''' <remarks></remarks>
    Protected Overrides Sub OnActionExecuting(filterContext As ActionExecutingContext)

        ' JavaScript が有効かを取得
        Me._enableJavaScript = HtmlHelper.UnobtrusiveJavaScriptEnabled

        ' クッキー が有効かを取得
        Me._enableCookies = Not Me.Session.IsCookieless

        ' デバッグ ログのパスを設定
        Me._debugLogPath = Me.HttpContext.Server.MapPath("~/App_Data/log")

        Dim mainModel As QolmsYappliModel = If(QyLoginHelper.CheckLogin(Me.Session, Me.Request), QyLoginHelper.GetQolmsYappliModel(Me.Session), Nothing)

        If mainModel IsNot Nothing Then
            With mainModel
                ' JavaScript が有効かを設定
                .SetEnableJavaScript(Me._enableJavaScript)

                ' クッキー が有効かを設定
                .SetEnableCookies(Me._enableCookies)

                ' デバッグ ビルド かを設定
                .SetIsDebug(Me._isDebug)

                ' デバッグ ログのパスを設定
                .SetDebugLogPath(Me._debugLogPath)

                ' セッション ID を設定
                .SetSessionId(Me.Session.SessionID)

                ' 画面の表示回数の カウント と不要な キャッシュ の破棄
                If Not (
                    filterContext.ActionDescriptor.IsDefined(GetType(ChildActionOnlyAttribute), False) _
                    OrElse filterContext.ActionDescriptor.IsDefined(GetType(QyAjaxOnlyAttribute), False)
                ) Then
                    ' 画面の表示回数を カウント
                    If filterContext.ActionDescriptor.IsDefined(GetType(QyViewCountAttribute), False) Then
                        .IncrementPageViewCount(DirectCast(filterContext.ActionDescriptor.GetCustomAttributes(GetType(QyViewCountAttribute), False).First(), QyViewCountAttribute).PageNo)
                    End If

                    ' TODO: 不要な キャッシュ を破棄
                End If
            End With
        End If

        ' アクセス ログ を出力
        If filterContext.ActionDescriptor.IsDefined(GetType(QyLoggingAttribute), False) Then
            Dim uri As String = String.Empty

            Try
                uri = $"/{filterContext.ActionDescriptor.ControllerDescriptor.ControllerName}/{filterContext.ActionDescriptor.ActionName}".ToLower() ' パス 部分は小文字で統一しておく

                If Not String.IsNullOrWhiteSpace(filterContext.HttpContext.Request.Url.Query) Then uri += filterContext.HttpContext.Request.Url.Query
            Catch
            End Try

            ' アクセス ログ
            AccessLogWorker.WriteAccessLog(filterContext.HttpContext, uri, AccessLogWorker.AccessTypeEnum.Show, AccessLogWorker.CommentTypeEnum.None)
        End If

        ' QolmsApi 用 API 認証 キー を取得
        If filterContext.ActionDescriptor.IsDefined(GetType(QyApiAuthorizeAttribute), False) _
            AndAlso mainModel IsNot Nothing Then

            With mainModel
                ' API 認証有効期限切れなら API 認証 キー を再取得
                If Date.Now > .ApiAuthorizeExpires Then
                    Dim authorizeKey As Guid = Guid.Empty
                    Dim authorizeExpires As Date = Date.MinValue

                    ' API 認証 キー および API 認証有効期限を取得
                    ApiAuthorizeWorker.NewKey(.AuthorAccount.AccountKey, .SessionId, authorizeKey, authorizeExpires)

                    If authorizeKey = Guid.Empty OrElse authorizeExpires = Date.MinValue Then Throw New ArgumentOutOfRangeException("ApiAuthorizeKey、ApiAuthorizeExpires", "QolmsApi 用 API 認証 キー および API 認証有効期限の取得に失敗しました。")

                    ' API 認証 キー および API 認証有効期限を設定
                    .SetApiAuthorizeKey(authorizeKey)
                    .SetApiAuthorizeExpires(authorizeExpires)
                End If
            End With
        End If

        ' QolmsJotoApi 用 API 認証 キー を取得
        If filterContext.ActionDescriptor.IsDefined(GetType(QjApiAuthorizeAttribute), False) _
            AndAlso mainModel IsNot Nothing Then

            With mainModel
                ' API 認証有効期限切れなら API 認証 キー を再取得
                If Date.Now > .ApiAuthorizeExpires2 Then
                    Dim authorizeKey As Guid = Guid.Empty
                    Dim authorizeExpires As Date = Date.MinValue

                    ' API 認証 キー および API 認証有効期限を取得
                    ApiAuthorizeWorker.NewKey2(.AuthorAccount.AccountKey, .SessionId, authorizeKey, authorizeExpires)

                    If authorizeKey = Guid.Empty OrElse authorizeExpires = Date.MinValue Then Throw New ArgumentOutOfRangeException("ApiAuthorizeKey、ApiAuthorizeExpires", "QolmsJotoApi 用 API 認証 キー および API 認証有効期限の取得に失敗しました。")

                    ' API 認証 キー および API 認証有効期限を設定
                    .SetApiAuthorizeKey2(authorizeKey)
                    .SetApiAuthorizeExpires2(authorizeExpires)
                End If
            End With
        End If

        MyBase.OnActionExecuting(filterContext)

    End Sub

    ''' <summary>
    ''' アクション メソッドの呼び出し後に呼び出されます。
    ''' </summary>
    ''' <param name="filterContext">現在の要求およびアクションに関する情報。</param>
    ''' <remarks></remarks>
    Protected Overrides Sub OnActionExecuted(filterContext As ActionExecutedContext)

        '' クリック ジャッキング 対策
        'If Not filterContext.ActionDescriptor.IsDefined(GetType(ChildActionOnlyAttribute), False) Then
        '    filterContext.HttpContext.Response.AddHeader("X-Frame-Options", "DENY")
        '    filterContext.HttpContext.Response.AddHeader("Content-Security-Policy", "frame-ancestors 'none'")
        'End If

        ' JSON レスポンスの XSS に対する予防的措置
        If filterContext.Result.GetType() = GetType(JsonResult) Then
            filterContext.HttpContext.Response.AddHeader("X-Content-Type-Options", "nosniff")
            filterContext.HttpContext.Response.AddHeader("Content-Disposition", String.Format("attachment; filename=""{0:yyyyMMddHHmmssfffffff}.json""", Date.Now))
        End If

        MyBase.OnActionExecuted(filterContext)

    End Sub

    ''' <summary>
    ''' アクションでハンドルされない例外が発生したときに呼び出されます。
    ''' </summary>
    ''' <param name="filterContext">現在の要求およびアクションに関する情報。</param>
    ''' <remarks></remarks>
    Protected Overrides Sub OnException(filterContext As ExceptionContext)

        ' エラーログを出力
        Dim uri As String = String.Empty

        Try
            uri = String.Format("/{0}/{1}", filterContext.RouteData.GetRequiredString("controller"), filterContext.RouteData.GetRequiredString("action"))

            If Not String.IsNullOrWhiteSpace(filterContext.HttpContext.Request.Url.Query) Then uri = uri + filterContext.HttpContext.Request.Url.Query
        Catch
        End Try

        ' エラーログ
        AccessLogWorker.WriteErrorLog(filterContext.HttpContext, uri, filterContext.Exception)

        ' エラー画面へ遷移
        If filterContext.HttpContext.IsCustomErrorEnabled Then
            filterContext.ExceptionHandled = True

            Me.TempData.Clear()
            Me.TempData.Add(QyMvcControllerBase.KEY_IS_LOGIN, QyLoginHelper.CheckLogin(Me.Session, Me.Request).ToString())
            Me.TempData.Add(QyMvcControllerBase.KEY_ERROR_MESSAGE, Me.errorMessage)

            ' 標準のエラーメッセージへ戻しておく
            Me.errorMessage = QyMvcControllerBase.DEFAULT_ERROR_MESSAGE
        Else
            MyBase.OnException(filterContext)
        End If

    End Sub

    ''' <summary>
    ''' Error 画面に表示するエラー メッセージを設定します。
    ''' </summary>
    ''' <param name="message">エラー メッセージ。</param>
    ''' <remarks></remarks>
    Protected Sub SetErrorMessage(message As String)

        If Not String.IsNullOrWhiteSpace(errorMessage) Then Me.errorMessage = message

    End Sub

    ''' <summary>
    ''' メイン モデルを取得します。
    ''' </summary>
    ''' <returns>
    ''' 成功ならメイン モデル、
    ''' 失敗なら Nothing。
    ''' </returns>
    ''' <remarks></remarks>
    Protected Function GetQolmsYappliModel() As QolmsYappliModel

        Return QyLoginHelper.GetQolmsYappliModel(Me.Session)

    End Function

    ''' <summary>
    ''' パーシャル ビュー モデルの親ビュー モデルとなる画面ビュー モデル取得します。
    ''' </summary>
    ''' <typeparam name="TModel">画面ビュー モデルの型。</typeparam>
    ''' <returns>
    ''' 成功なら画面ビュー モデル、
    ''' 失敗なら Nothing。
    ''' </returns>
    ''' <remarks></remarks>
    Protected Function GetPageViewModel(Of TModel As QyPageViewModelBase)() As TModel

        Dim result As TModel = Nothing

        If Me.ControllerContext.IsChildAction Then
            Try
                result = DirectCast(Me.ControllerContext.ParentActionViewContext.ViewData.Model, TModel)
            Catch
            End Try
        End If

        Return result

    End Function

#End Region

#Region "Public Method"

    ''' <summary>
    ''' セッションが有効かチェックします。
    ''' </summary>
    ''' <returns>
    ''' アクションの結果。
    ''' </returns>
    ''' <remarks></remarks>
    <HttpPost()>
    <QyAjaxOnly()>
    Public Function AjaxCheckSession() As JsonResult

        Return New CheckSessionJsonResult() With {.IsSuccess = QyLoginHelper.CheckLogin(Me.Session, Me.Request).ToString()}.ToJsonResult()

    End Function

    ''' <summary>
    ''' メイン ウィンドウからの要求に対し、
    ''' セッションが有効かチェックします。
    ''' </summary>
    ''' <returns>
    ''' セッションが有効なら "javascript:void(0);"、
    ''' セッションが無効ならログイン画面へ遷移するスクリプトを返却。
    ''' </returns>
    ''' <remarks></remarks>
    <HttpPost()>
    <QyAjaxOnly()>
    Public Function AjaxCheckSessionByWindow() As JavaScriptResult

        Dim script As String = "javascript:void(0);"

        If Not QyLoginHelper.CheckLogin(Me.Session, Me.Request) Then
            Dim url As String = String.Empty

            With DirectCast(WebConfigurationManager.GetSection("system.web/authentication"), System.Web.Configuration.AuthenticationSection)
                url = .Forms.LoginUrl
            End With

            If url.StartsWith("/") Then
                url += ".."
            ElseIf url.StartsWith("~/") Then
                url = ".." + url.Substring(1)
            End If

            If Not String.IsNullOrWhiteSpace(url) Then script = String.Format("$(location).attr('href', '{0}');", url)
        End If

        Return JavaScript(script)

    End Function

#End Region

End Class
