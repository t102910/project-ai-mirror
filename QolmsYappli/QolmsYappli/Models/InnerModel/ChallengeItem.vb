''' <summary>
''' チャレンジの情報を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class ChallengeItem

#Region "Public Property"

    ''' <summary>
    ''' チャレンジキーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Challengekey As Guid = Guid.Empty

    ''' <summary>
    ''' チャレンジ名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Name As String = String.Empty

    ''' <summary>
    ''' 説明を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Description As String = String.Empty

    ''' <summary>
    ''' エントリー開始日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property EntryStartDate As Date = Date.MinValue

    ''' <summary>
    ''' エントリー終了日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property EntryEndDate As Date = Date.MinValue

    ''' <summary>
    ''' チャレンジ開始日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property StartDate As Date = Date.MinValue

    ''' <summary>
    ''' チャレンジ期間を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Period As Integer = Integer.MinValue

    ''' <summary>
    ''' ユーザーのチャレンジ開始日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property UserStartDate As Date = Date.MinValue

    ''' <summary>
    ''' ユーザーのチャレンジ終了日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property UserEndDate As Date = Date.MinValue

    ''' <summary>
    ''' ステータスを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property StatusType As Byte = Byte.MinValue


    ''' <summary>
    ''' チャレンジの外部キーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ExternalId As String = String.Empty

    ''' <summary>
    ''' ステータスを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property StatusTypeMaster As New List(Of ChallengeStatusItem)

    ''' <summary>
    ''' チャレンジのTop画像を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ImageSrc As String = String.Empty

    ''' <summary>
    ''' チャレンジのTop画像を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LinkageSystemNo As Integer = Integer.MinValue

    ''' <summary>
    ''' Viewerアカウントとの連携を作成するかどうかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RelationFlag As Boolean = False

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="ChallengeItem" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class