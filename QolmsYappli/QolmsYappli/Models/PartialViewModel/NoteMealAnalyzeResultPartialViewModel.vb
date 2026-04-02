<Serializable()>
Public NotInheritable Class NoteMealAnalyzeResultPartialViewModel
    Inherits QyPartialViewModelBase(Of NoteMealListInputModel)

#Region "Public Property"

    ''' <summary>
    ''' 遷移先のページを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PageType As String = String.Empty

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="NoteMealAnalyzeResultPartialViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' <see cref="NoteMealAnalyzeResultPartialViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(model As NoteMealListInputModel)

        MyBase.New(model)

    End Sub

#End Region

End Class
