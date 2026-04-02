Imports System.Runtime.Serialization

<DataContract()>
<Serializable()>
Public NotInheritable Class AppleIdEntryCookieJsonParameter
    Inherits QyJsonParameterBase

#Region "Public Property"

    ''' <summary>
    ''' 暗号化したAppleIdUserNameJsonParameterを入れる
    ''' </summary>
    ''' <returns></returns>
    <DataMember()>
    Public Property FirstName As String = String.Empty

    <DataMember()>
    Public Property LastName As String = String.Empty

    <DataMember()>
    Public Property Expires As String = String.Empty

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="AuIdLoginCookieJsonParameter" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region

End Class
