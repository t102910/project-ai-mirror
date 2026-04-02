''' <summary>
''' Fitbitのトークンのセットを格納するクラスです。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class FitbitTokenSet

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


    ''' <summary>
    ''' [任意] トークンのタイプを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property token_type As String = String.Empty

    ''' <summary>
    ''' [任意] Fitbit ユーザー ID を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property user_id As String = String.Empty

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="FitbitTokenSet" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class