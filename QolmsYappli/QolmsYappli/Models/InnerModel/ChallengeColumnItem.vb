''' <summary>
''' チャレンジコラムの情報を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class ChallengeColumnItem

#Region "Public Property"

    ''' <summary>
    ''' チャレンジキーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Challengekey As Guid = Guid.Empty

    ''' <summary>
    ''' コラムNoを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ColumnNo As Integer = Integer.MinValue

    ''' <summary>
    '''タイトルを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Title As String = String.Empty

    ''' <summary>
    ''' 内容を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Content As String = String.Empty

    ''' <summary>
    ''' 表示順を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DispOrder As Integer = Integer.MinValue

    ''' <summary>
    ''' 表示する経過日数を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Days As Integer = Integer.MinValue

    ''' <summary>
    ''' 画像キーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ImageKey As Guid = Guid.Empty

    ''' <summary>
    ''' サムネイルキーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ThumbnailKey As Guid = Guid.Empty

    ''' <summary>
    ''' ユーザーのコラム表示日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property UserDispDate As Date = Date.MinValue

    ''' <summary>
    ''' ユーザーの既読フラグを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property UserReadFlag As Boolean = False

    ''' <summary>
    ''' ユーザーの既読日時を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property UserReadDate As Date = Date.MinValue


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
