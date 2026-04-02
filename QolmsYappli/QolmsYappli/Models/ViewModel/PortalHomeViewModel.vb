''' <summary>
''' 「ホーム」画面ビュー モデルを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class PortalHomeViewModel
    Inherits QyPortalPageViewModelBase

#Region "Public Property"

    ' ''' <summary>
    ' ''' ニュースを取得または設定します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Property NewsN As New List(Of String)()

    ' ''' <summary>
    ' ''' 現在のポイント数を取得または設定します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Property PointNow As Integer = Integer.MinValue

    '' ''' <summary>
    '' ''' ポイントMAX値を取得または設定します。
    '' ''' </summary>
    '' ''' <value></value>
    '' ''' <returns></returns>
    '' ''' <remarks></remarks>
    ''Public Property PointMax As Integer = Integer.MinValue

    ' ''' <summary>
    ' ''' Daily　OR　Monthly　を取得または設定します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Property Mode As String = String.Empty

    ' ''' <summary>
    ' ''' 表示日（月）を取得または設定します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Property ShowDay As Date = Date.MinValue

    ' ''' <summary>
    ' ''' 摂取カロリーの目標値を取得または設定します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Property TargetCalorieIn As Integer = Integer.MinValue

    '' ''' <summary>
    '' ''' 消費カロリーの目標値を取得または設定します。
    '' ''' </summary>
    '' ''' <value></value>
    '' ''' <returns></returns>
    '' ''' <remarks></remarks>
    ''Public Property TargetCalorieOut As Integer = Integer.MinValue

    ' ''' <summary>
    ' ''' 朝食のカロリーを取得または設定します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Property CalBreakfast As Decimal = Decimal.MinValue

    ' ''' <summary>
    ' ''' 昼食のカロリーを取得または設定します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Property CalLunch As Decimal = Decimal.MinusOne

    ' ''' <summary>
    ' ''' 夕食のカロリーを取得または設定します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Property CalDinner As Decimal = Decimal.MinValue

    ' ''' <summary>
    ' ''' 間食のカロリーを取得または設定します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Property CalSnacking As Decimal = Decimal.MinValue

    ' ''' <summary>
    ' ''' 消費カロリーを取得または設定します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Property CalConsumption As Decimal = Decimal.MinValue

    ' ''' <summary>
    ' ''' 歩数を取得または設定します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Property Steps As Integer = Integer.MinValue

    ' ''' <summary>
    ' ''' 歩数による消費カロリーを取得または設定します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Property StepsCal As Integer = Integer.MinValue

    ' ''' <summary>
    ' ''' 運動の消費カロリーを取得または設定します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Property ExerciseCal As Integer = Integer.MinValue

    ' ''' <summary>
    ' ''' 基礎代謝量を取得または設定します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Property BasalMetabolism As Integer = Integer.MinValue

    ' ''' <summary>
    ' ''' 健康年齢を取得または設定します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Property HealthAge As Decimal = Decimal.MinValue

    ' ''' <summary>
    ' ''' ポイントフラグを取得または設定します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Property PointFlags As QyHomePointTypeEnum = QyHomePointTypeEnum.None

    ''' <summary>
    ''' タニタ連携状態を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TanitaConnectedFlag As Boolean = False

    ''' <summary>
    ''' セットアップが必要かどうかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SetUpFlag As Boolean = False

    ''' <summary>
    ''' ALKOO接続状態を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AlkooConnectedFlag As Boolean = False

    ''' <summary>
    ''' ALKOO外部IDを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AlkooJotoId As String = String.Empty

    ''' <summary>
    ''' HOMEに設定するURLのリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property UrlList As New Dictionary(Of String, String)

    ''' <summary>
    ''' Yappli SDK を使用した、一意 ID の送信を有効にするかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property EnableYappliSdk As Boolean = False


    ''' <summary>
    ''' PortalHomeDataAreaPartialViewModel するかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PartialViewModel As New PortalHomeDataAreaPartialViewModel

    ''' <summary>
    ''' PortalHomeChallengeAreaPartialViewModel するかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ChallengeAreaPartialViewModel As New PortalHomeChallengeAreaPartialViewModel

    ''' <summary>
    ''' 参加中のチャレンジのリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ChallengeList As New List(Of Guid)

    ''' <summary>
    ''' 健診連携のリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LinkageList As New List(Of Integer)

    ''' <summary>
    ''' カロミルのAIフィードバックの通知表示するかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IsCalomealAiFeedback As Boolean = False

    ''' <summary>
    ''' ぎのわん市民確認済みを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property GinowanStatusType As Byte = Byte.MinValue

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PortalHomeViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' メイン モデルを指定して、
    ''' <see cref="PortalHomeViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <remarks></remarks>
    Public Sub New(mainModel As QolmsYappliModel)

        MyBase.New(mainModel, QyPageNoTypeEnum.PortalHome)

    End Sub

#End Region

End Class
