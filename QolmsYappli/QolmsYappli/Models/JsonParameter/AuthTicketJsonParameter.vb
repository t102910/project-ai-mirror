Imports System.Runtime.Serialization

<DataContract()>
<Serializable()>
Public NotInheritable Class AuthTicketJsonParameter
    Inherits QyJsonParameterBase

#Region "Public Property"

    ' TODO: 要検討
    <DataMember()>
    Public Property UserId As String = String.Empty

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="AuthTicketJsonParameter" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region

End Class
