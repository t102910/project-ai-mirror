<Serializable()>
Public NotInheritable Class PortalHomeDataAreaPartialViewModel
    Inherits QyPartialViewModelBase(Of PortalHomeViewModel)

#Region "Public Property"

    ''' <summary>
    ''' 現在のポイント数を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PointNow As Integer = Integer.MinValue

    ' ''' <summary>
    ' ''' ポイントMAX値を取得または設定します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Property PointMax As Integer = Integer.MinValue

    ''' <summary>
    ''' Daily　OR　Monthly　を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Mode As String = String.Empty

    ''' <summary>
    ''' 表示日（月）を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ShowDay As Date = Date.MinValue

    ''' <summary>
    ''' 摂取カロリーの目標値を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TargetCalorieIn As Integer = Integer.MinValue

    ' ''' <summary>
    ' ''' 消費カロリーの目標値を取得または設定します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Property TargetCalorieOut As Integer = Integer.MinValue

    ''' <summary>
    ''' 朝食のカロリーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CalBreakfast As Decimal = Decimal.MinValue

    ''' <summary>
    ''' 昼食のカロリーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CalLunch As Decimal = Decimal.MinusOne

    ''' <summary>
    ''' 夕食のカロリーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CalDinner As Decimal = Decimal.MinValue

    ''' <summary>
    ''' 間食のカロリーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CalSnacking As Decimal = Decimal.MinValue

    ''' <summary>
    ''' 消費カロリーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CalConsumption As Decimal = Decimal.MinValue

    ''' <summary>
    ''' 歩数を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Steps As Integer = Integer.MinValue

    ''' <summary>
    ''' 歩数による消費カロリーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property StepsCal As Integer = Integer.MinValue

    ''' <summary>
    ''' 運動の消費カロリーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ExerciseCal As Integer = Integer.MinValue

    ''' <summary>
    ''' 基礎代謝量を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BasalMetabolism As Integer = Integer.MinValue

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PortalHomeDataAreaPartialViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' <see cref="PortalHomeDataAreaPartialViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(model As PortalHomeViewModel)

        MyBase.New(model)

    End Sub

#End Region

End Class
