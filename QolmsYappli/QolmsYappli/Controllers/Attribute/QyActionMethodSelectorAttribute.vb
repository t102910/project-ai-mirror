Imports System.Reflection
Imports System.Web.Mvc

''' <summary>
''' 「コルムス ヤプリ サイト」で使用するアクション メソッドを、
''' アクション ソースを元に選択するかを指定する属性を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<AttributeUsage(AttributeTargets.Method, AllowMultiple:=False)>
Friend NotInheritable Class QyActionMethodSelectorAttribute
    Inherits ActionMethodSelectorAttribute

#Region "Public Property"

    ''' <summary>
    ''' アクション ソースを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ActionSources As String = String.Empty

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="QyActionMethodSelectorAttribute" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' アクション ソースを指定して、
    ''' <see cref="QyActionMethodSelectorAttribute" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="actionSources">
    ''' アクション ソース（"," 区切りで複数のソースを指定可能）。
    ''' </param>
    ''' <remarks></remarks>
    Public Sub New(actionSources As String)

        MyBase.New()

        Me.ActionSources = actionSources

    End Sub

#End Region

#Region "Public Method"

    ''' <summary>
    ''' アクション メソッドの選択が、
    ''' 指定されたコントローラー コンテキストで有効かどうかを判断します。
    ''' </summary>
    ''' <param name="controllerContext">コントローラー コンテキスト。</param>
    ''' <param name="methodInfo">アクション メソッドに関する情報。</param>
    ''' <returns>
    ''' アクション メソッドの選択が、
    ''' 指定されたコントローラー コンテキストで有効である場合は True、
    ''' それ以外の場合は False。
    ''' </returns>
    ''' <remarks></remarks>
    Public Overrides Function IsValidForRequest(controllerContext As ControllerContext, methodInfo As MethodInfo) As Boolean

        Const key As String = "ActionSource"
        Dim result As Boolean = False

        If controllerContext.Controller.ValueProvider.GetValue(key) IsNot Nothing Then
            Dim source As String = DirectCast(controllerContext.Controller.ValueProvider.GetValue(key).ConvertTo(GetType(String)), String)

            result = Not String.IsNullOrWhiteSpace(Me.ActionSources) _
                AndAlso Me.ActionSources.Split({","}, StringSplitOptions.RemoveEmptyEntries).Any(Function(i) String.Compare(i.Trim(), source.Trim(), True) = 0)
        End If

        Return result

    End Function

#End Region

End Class
