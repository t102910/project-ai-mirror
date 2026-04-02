
''' <summary>
''' チャレンジのステータスマスタ情報を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class ChallengeStatusItem

#Region "Public Property"

    ''' <summary>
    ''' チャレンジキーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Challengekey As Guid = Guid.Empty

    ''' <summary>
    ''' ステータスの種別を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>bitFlagです。</remarks>
    Public Property StatusType As Byte = Byte.MinValue

    ''' <summary>
    ''' 名前を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property StatusName As String = String.Empty

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="ChallengeStatusItem" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class