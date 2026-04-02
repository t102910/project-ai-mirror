''' <summary>
''' ファルモ連携のトークンのセットを格納するクラスです。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class PharumoTokenSet

#Region "Public Property"

    ''' <summary>
    ''' トークン
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Token As String = String.Empty

    ''' <summary>
    ''' トークンの有効期限
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TokenExpires As Date = Date.MinValue

    ''' <summary>
    ''' リフレッシュトークン
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RefreshToken As String = String.Empty

    ''' <summary>
    ''' リフレッシュトークンの有効期限
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RefreshTokenExpires As Date = Date.MinValue


#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PharumoTokenSet" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class