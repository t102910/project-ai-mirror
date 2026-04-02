''' <summary>
''' 連携設定情報を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class ConnectionSettingItem

#Region "Public Property"

    ''' <summary>
    ''' 連携番号を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LinkageNo As Integer = Integer.MinValue

    ''' <summary>
    ''' 連携デバイスを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Devices As New List(Of String)()

    ''' <summary>
    ''' 連携デバイスを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Tags As New List(Of String)()

    ''' <summary>
    ''' 連携デバイスを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Status As Byte = Byte.MinValue

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="CouponItem" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class
