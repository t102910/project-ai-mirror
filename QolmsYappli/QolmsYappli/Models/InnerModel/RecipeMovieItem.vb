''' <summary>
''' レシピスポーツクラブのフィットネス動画の情報を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class RecipeMovieItem

#Region "Public Property"

    ''' <summary>
    ''' URLを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Id As String = String.Empty

    ''' <summary>
    ''' 動画の説明を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Discription As String = String.Empty

    ''' <summary>
    ''' 一回当たりの運動時間を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Time As String = String.Empty

    ''' <summary>
    ''' 一回当たりの消費カロリーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Calorie As Integer = Integer.MinValue

    ''' <summary>
    ''' 食事名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ItemName As String = String.Empty

    ''' <summary>
    ''' 食事の成分のJsonを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MealValue As String = String.Empty

    ''' <summary>
    ''' 食事のサムネイルを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Imagekey As Guid = Guid.Empty


#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="DeviceItem" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class
