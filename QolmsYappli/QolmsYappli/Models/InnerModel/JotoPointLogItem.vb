''' <summary>
''' JOTO ポイント ログ情報を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Public NotInheritable Class JotoPointLogItem

#Region "Public Property"

    ''' <summary>
    ''' 操作日時を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ActionDate As Date = Date.MinValue

    ''' <summary>
    ''' 対象日時を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TargetDate As Date = Date.MinValue

    ''' <summary>
    ''' ポイント項目番号を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ItemNo As Integer = Integer.MinValue

    ''' <summary>
    ''' ポイント項目名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ItemName As String = String.Empty

    ''' <summary>
    ''' ポイントを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Point As Integer = Integer.MinValue

    ''' <summary>
    ''' 付与理由を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Reason As String = String.Empty

    ''' <summary>
    ''' 有効期限を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ExpirationDate As Date = Date.MinValue

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="JotoPointLogItem" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class
