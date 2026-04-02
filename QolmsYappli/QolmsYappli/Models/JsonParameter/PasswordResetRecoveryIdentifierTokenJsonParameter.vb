Imports System.Runtime.Serialization

<DataContract()>
<Serializable()>
Public NotInheritable Class PasswordResetRecoveryIdentifierTokenJsonParameter
    Inherits QyJsonParameterBase

#Region "Public Property"

    <DataMember()>
    Public Property PasswordResetkey As String = String.Empty

    <DataMember()>
    Public Property Expires As String = String.Empty

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
