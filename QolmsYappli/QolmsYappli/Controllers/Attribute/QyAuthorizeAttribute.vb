Imports System.Web.Mvc
''' <summary>
''' 「コルムス ヤプリ サイト」で使用するアクション メソッドの実行に、
''' 承認が必要かを指定する属性を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<AttributeUsage(AttributeTargets.Method, AllowMultiple:=False)>
Friend NotInheritable Class QyAuthorizeAttribute
    Inherits AuthorizeAttribute

#Region "Public Property"

    ''' <summary>
    ''' ログイン ページへのリダイレクトを抑制するかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SuppressRedirect As Boolean = False

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="QyAuthorizeAttribute" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

        Me.SuppressRedirect = False

    End Sub

    ''' <summary>
    ''' ログイン ページへのリダイレクトを抑制するかを指定して、
    ''' <see cref="QyAuthorizeAttribute" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="suppressRedirect">
    ''' ログイン ページへのリダイレクトを抑制するなら True、
    ''' 抑制しないなら False を指定。
    ''' </param>
    ''' <remarks></remarks>
    Public Sub New(suppressRedirect As Boolean)

        MyBase.New()

        Me.SuppressRedirect = suppressRedirect

    End Sub

#End Region

#Region "Protected Method"

    ''' <summary>
    ''' カスタムの承認チェックのエントリ ポイントを提供します。
    ''' </summary>
    ''' <param name="httpContext">HTTP コンテキスト。</param>
    ''' <returns>
    ''' ユーザーが承認された場合は True、
    ''' それ以外の場合は False。
    ''' </returns>
    ''' <remarks></remarks>
    <Obsolete("実装中")>
    Protected Overrides Function AuthorizeCore(httpContext As HttpContextBase) As Boolean

        ' 認証、承認処理
        Return QyLoginHelper.CheckLogin(httpContext.Session, httpContext.Request)

    End Function

    ''' <summary>
    ''' 承認されなかった HTTP 要求を処理します。
    ''' </summary>
    ''' <param name="filterContext">フィルター コンテキスト。</param>
    ''' <remarks></remarks>
    Protected Overrides Sub HandleUnauthorizedRequest(filterContext As AuthorizationContext)

        ' ログイン ページへのリダイレクトを抑制するかを指設定
        filterContext.HttpContext.Response.SuppressFormsAuthenticationRedirect = Me.SuppressRedirect

        MyBase.HandleUnauthorizedRequest(filterContext)

    End Sub

#End Region

End Class
