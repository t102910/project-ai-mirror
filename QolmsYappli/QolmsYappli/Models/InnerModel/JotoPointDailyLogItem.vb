''' <summary>
''' 日付毎の JOTO ポイント ログ情報を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Public NotInheritable Class JotoPointDailyLogItem

#Region "Public Property"

    ''' <summary>
    ''' 操作日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ActionDate As Date = Date.MinValue

    ''' <summary>
    ''' 日付内の JOTO ポイント ログ情報のリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PointLogN As New List(Of JotoPointLogItem)()

    ''' <summary>
    ''' 日付内の ポイント合計を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Point As Integer = Integer.MinValue

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="JotoPointDailyLogItem" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class
