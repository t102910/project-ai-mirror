Imports System.Runtime.Serialization

<DataContract()>
<Serializable()>
Public NotInheritable Class AuIdLoginCookieJsonParameter
    Inherits QyJsonParameterBase

#Region "Public Property"

    <DataMember()>
    Public Property LoginType As String = String.Empty

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
