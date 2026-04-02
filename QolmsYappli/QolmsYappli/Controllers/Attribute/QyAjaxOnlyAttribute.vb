Imports System.Web.Mvc
''' <summary>
''' 「コルムス ヤプリ サイト」で使用するアクション メソッドが、
''' AJAX 要求によってのみアクセス可能かを指定する属性を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<AttributeUsage(AttributeTargets.Method, AllowMultiple:=False)>
Friend NotInheritable Class QyAjaxOnlyAttribute
    Inherits ActionFilterAttribute

#Region "Constructor"

    ''' <summary>
    ''' <see cref="QyAjaxOnlyAttribute" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region

#Region "Public Method"

    ''' <summary>
    ''' アクション メソッドの実行前に ASP.NET MVC フレームワークによって呼び出されます。
    ''' </summary>
    ''' <param name="filterContext">フィルター コンテキスト。</param>
    ''' <remarks></remarks>
    Public Overrides Sub OnActionExecuting(filterContext As ActionExecutingContext)

        If filterContext.HttpContext.Request.IsAjaxRequest() Then
            MyBase.OnActionExecuting(filterContext)
        Else
            Throw New InvalidOperationException("Ajax リクエストによってのみアクセス可能です。")
        End If

    End Sub

#End Region

End Class
