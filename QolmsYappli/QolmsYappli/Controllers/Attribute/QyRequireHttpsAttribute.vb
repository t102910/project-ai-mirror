Imports System.Web.Mvc
''' <summary>
''' 「コルムス ヤプリ サイト」に対するセキュリティ保護されていない HTTP 要求を、
''' HTTPS を介して強制的に再送信する属性を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<AttributeUsage(AttributeTargets.Class, AllowMultiple:=False)>
Friend NotInheritable Class QyRequireHttpsAttribute
    Inherits RequireHttpsAttribute

#Region "Constructor"

    ''' <summary>
    ''' <see cref="QyRequireHttpsAttribute" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region

#Region "Public Method"

    ''' <summary>
    ''' 要求がセキュリティ保護されている（HTTPS）かどうかを判断し、
    ''' 保護されていない場合は HandleNonHttpsRequest メソッドを呼び出します。
    ''' </summary>
    ''' <param name="filterContext">RequireHttpsAttribute 属性を使用するために必要な情報をカプセル化するオブジェクト。</param>
    ''' <remarks></remarks>
    Public Overrides Sub OnAuthorization(filterContext As AuthorizationContext)

        ' Release ビルド時のみチェックする
#If Not DEBUG Then
        MyBase.OnAuthorization(filterContext)
#End If

    End Sub

#End Region

End Class
