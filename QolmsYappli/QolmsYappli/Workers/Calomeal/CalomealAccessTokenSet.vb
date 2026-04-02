Imports System.Runtime.Serialization

''' <summary>
''' カロミルのトークンのセットを格納するクラスです。
''' </summary>
''' <remarks></remarks>
<Serializable()>
<DataContract()>
Public NotInheritable Class CalomealAccessTokenSet

#Region "Public Property"

    ''' <summary>
    ''' token_typeを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember>
    Public Property token_type As String = String.Empty

    ''' <summary>
    ''' expires_in を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember>
    Public Property expires_in As String = String.Empty

    ''' <summary>
    ''' トークン
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember>
    Public Property access_token As String = String.Empty

    ''' <summary>
    ''' トークンの有効期限
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember>
    Public Property TokenExpires As Date = Date.MinValue

    ''' <summary>
    ''' リフレッシュトークン
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember>
    Public Property refresh_token As String = String.Empty

    ''' <summary>
    ''' リフレッシュトークンの有効期限
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember>
    Public Property RefreshTokenExpires As Date = Date.MinValue

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
