''' <summary>
''' au WALLET の交換マスタを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class AuWalletPointItem

#Region "Public Property"

    ''' <summary>
    ''' イベントIDを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AuWalletPointItemId As String = String.Empty

    ' ''' <summary>
    ' ''' 交換対象のポイント数を取得または設定します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Property Size As Integer = Integer.MinValue

    ''' <summary>
    ''' 消費ポイントを取得または設定します。
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
    ''' <see cref="AuWalletPointItem" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class
