''' <summary>
''' カラダカルテのデバイスを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class DeviceItem

#Region "Public Property"

    ''' <summary>
    ''' デバイス名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DeviceName As String = String.Empty

    ''' <summary>
    ''' デバイス名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DevicePropertyName As String = String.Empty

    ''' <summary>
    ''' チェックボックスの値を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Checked As Boolean = False

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="DeviceItem" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class
