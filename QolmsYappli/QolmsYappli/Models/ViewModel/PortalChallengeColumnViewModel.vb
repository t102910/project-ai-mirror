''' <summary>
''' 「チャレンジコラム」画面ビュー モデルを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class PortalChallengeColumnViewModel
    Inherits QyPortalPageViewModelBase

#Region "Public Property"

    ''' <summary>
    ''' 遷移元の画面番号の種別を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FromPageNoType As QyPageNoTypeEnum = QyPageNoTypeEnum.None

    ''' <summary>
    ''' 画面読み込み時に初期表示するコラムの番号を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TargetColumnNo As Integer = Integer.MinValue


    ''' <summary>
    ''' チャレンジのリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ChallengeColumnItemN As New List(Of ChallengeColumnItem)()

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PortalChallengeColumnViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' メイン モデルを指定して、
    ''' <see cref="PortalChallengeColumnViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <remarks></remarks>
    Public Sub New(mainModel As QolmsYappliModel)

        MyBase.New(mainModel, QyPageNoTypeEnum.PortalChallenge)

    End Sub

#End Region
End Class
