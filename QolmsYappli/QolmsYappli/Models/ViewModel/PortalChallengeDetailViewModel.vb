''' <summary>
''' 「チャレンジ詳細」画面ビュー モデルを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class PortalChallengeDetailViewModel
    Inherits QyPortalPageViewModelBase

#Region "Public Property"

    ''' <summary>
    ''' チャレンジを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ChallengeItem As New ChallengeItem

    ''' <summary>
    ''' 目標達成種別を取得または設定します。
    ''' </summary>
    ''' <value>1:達成、2:途中、3:失敗</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TargetAchievedType As Byte = Byte.MinValue

    ' ''' <summary>
    ' ''' 連携システム番号を取得または設定します。
    ' ''' </summary>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Property LinkageSystemNo As Integer = Integer.MinValue

    ' ''' <summary>
    ' ''' 連携システムIDを取得または設定します。
    ' ''' </summary>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Property LinkageSystemId As String = String.Empty

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PortalChallengeDetailsViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' メイン モデルを指定して、
    ''' <see cref="PortalChallengeDetailsViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <remarks></remarks>
    Public Sub New(mainModel As QolmsYappliModel)

        MyBase.New(mainModel, QyPageNoTypeEnum.PortalChallenge)

    End Sub

#End Region

End Class