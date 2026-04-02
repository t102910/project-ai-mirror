<Serializable()>
Public NotInheritable Class NoteMealSearchResultViewModel
    Inherits QyNotePageViewModelBase

#Region "Public Property"

    ''' <summary>
    ''' 検索文字列を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SearchText As String = String.Empty

    ''' <summary>
    ''' 文字検索結果の食事アイテムのリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SearchedMealItemN As New List(Of FoodItem)

    ''' <summary>
    ''' 文字検索結果の最大ページ数を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SearchedMaxPage As Integer = Integer.MinValue

    ''' <summary>
    ''' 文字検索結果の現在ページ数を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DispedPage As Integer = Integer.MinValue

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
    ''' <see cref="NoteMealSearchResultViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region

End Class
