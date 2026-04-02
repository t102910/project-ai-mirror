''' <summary>
''' データチャージのイベントIDを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class DatachargeEventIdItem

#Region "Public Property"

    ''' <summary>
    ''' イベントIDを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property EventId As String = String.Empty

    ''' <summary>
    ''' データサイズを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Size As Integer = Integer.MinValue

    ''' <summary>
    ''' ポイントを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Point As Integer = Integer.MinValue

    ''' <summary>
    ''' 表示名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DispName As String = String.Empty



#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="DatachargeEventIdItem" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class
