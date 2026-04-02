''' <summary>
''' 食事の１品目の情報を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class MealItem

#Region "Public Property"

    ''' <summary>
    ''' 食事日時を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MealDate As Date = Date.MinValue

    ''' <summary>
    ''' 食事区分を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MealType As String = String.Empty

    ''' <summary>
    ''' 区分内連番を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Sequence As Integer = Integer.MinValue

    ''' <summary>
    ''' 品目名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MealName As String = String.Empty

    ''' <summary>
    ''' カロリーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Calorie As Integer = Integer.MinValue

    ''' <summary>
    ''' 写真キーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PhotoKey As Guid = Guid.Empty

    ''' <summary>
    ''' 画像解析結果の候補の食品リストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FoodN As New List(Of FoodItem)()

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="MealItem" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class