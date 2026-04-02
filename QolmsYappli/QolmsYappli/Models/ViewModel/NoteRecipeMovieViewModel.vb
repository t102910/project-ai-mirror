''' <summary>
''' 「レシピ動画」画面ビュー モデルを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class NoteRecipeMovieViewModel
    Inherits QyNotePageViewModelBase

#Region "Public Property"

    ''' <summary>
    ''' 動画の種別を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MovieType As Byte = Byte.MinValue

    ''' <summary>
    ''' 動画アイテムのリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MovieItemN As New List(Of RecipeMovieItem)

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="NoteRecipeMovieViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' メイン モデルを指定して、
    ''' <see cref="NoteRecipeMovieViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <remarks></remarks>
    Public Sub New(mainModel As QolmsYappliModel)

        MyBase.New(mainModel, QyPageNoTypeEnum.NoteRecipeMovie)

    End Sub


#End Region

End Class
