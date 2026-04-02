''' <summary>
''' 「食事（4）」画面ビュー モデルを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class NoteMealViewModel4
    Inherits QyNotePageViewModelBase

#Region "Public Property"

    Public Property RecordDate As Date = Date.MinValue

    Public Property FilterDate As Date = Date.MinValue

    Public Property MealItemN As New List(Of MealItem)()

    ''' <summary>
    ''' 食事種別を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MealType As QyMealTypeEnum = QyMealTypeEnum.None

    Public Property HistoryFlag As Boolean = False

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="NoteMealViewModel4" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' メイン モデルを指定して、
    ''' <see cref="NoteMealViewModel4" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <remarks></remarks>
    Public Sub New(mainModel As QolmsYappliModel)

        MyBase.New(mainModel, QyPageNoTypeEnum.NoteMeal)

    End Sub

    ''' <summary>
    ''' 値を指定して、
    ''' <see cref="NoteMealViewModel4" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="recordDate"></param>
    ''' <param name="mealItemN"></param>
    ''' <remarks></remarks>
    Public Sub New(mainModel As QolmsYappliModel, recordDate As Date, mealItemN As List(Of MealItem))

        MyBase.New(mainModel, QyPageNoTypeEnum.NoteMeal)

        ' TODO:
        Me.RecordDate = recordDate
        Me.MealItemN = mealItemN

    End Sub


    ''' <summary>
    ''' 値を指定して、
    ''' <see cref="NoteMealViewModel4" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="recordDate"></param>
    ''' <param name="mealItemN"></param>
    ''' <remarks></remarks>
    Public Sub New(mainModel As QolmsYappliModel, recordDate As Date, mealItemN As List(Of MealItem), mealType As QyMealTypeEnum)

        MyBase.New(mainModel, QyPageNoTypeEnum.NoteMeal)

        ' TODO:
        Me.RecordDate = recordDate
        Me.MealItemN = mealItemN
        Me.MealType = mealType

    End Sub
#End Region

End Class
