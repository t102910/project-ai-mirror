''' <summary>
''' 「食事」画面ビュー モデルを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class NoteMealViewModel
    Inherits QyNotePageViewModelBase

#Region "Public Property"

    ''' <summary>
    ''' 編集日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RecordDate As Date = Date.MinValue

    ''' <summary>
    ''' 食事インプットモデルを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property InputModel As New NoteMealInputModel()

    ''' <summary>
    ''' 食事アイテムのリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MealItemN As New List(Of MealItem)

    ''' <summary>
    ''' 文字検索結果の食事アイテムのリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SearchedMealItemN As New List(Of List(Of FoodItem))

    ''' <summary>
    ''' 文字検索結果の最大ページ数を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SearchedMaxPage As Integer = Integer.MinValue

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="NoteMealViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' メイン モデルを指定して、
    ''' <see cref="NoteMealViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <remarks></remarks>
    Public Sub New(mainModel As QolmsYappliModel)

        MyBase.New(mainModel, QyPageNoTypeEnum.NoteMeal)

    End Sub

    ''' <summary>
    ''' 値を指定して、
    ''' <see cref="NoteMealViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <remarks></remarks>
    Public Sub New(mainModel As QolmsYappliModel, recordDate As Date, mealItemN As List(Of MealItem))

        MyBase.New(mainModel, QyPageNoTypeEnum.NoteMeal)

        Me.RecordDate = recordDate

        Me.MealItemN = mealItemN

    End Sub

#End Region

End Class
