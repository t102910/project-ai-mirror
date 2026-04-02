Imports System.Runtime.Serialization

<DataContract()>
<Serializable()>
Public NotInheritable Class MonshinArgsJsonParamater
    Inherits QyJsonParameterBase

#Region "Public Property"

    <DataMember()>
    Public Property patientID As String = String.Empty

    <DataMember()>
    Public Property patientName As String = String.Empty

    <DataMember()>
    Public Property patientNameKana As String = String.Empty

    <DataMember()>
    Public Property gendar As Byte = Byte.MinValue

    <DataMember()>
    Public Property birthday As String = String.Empty

    <DataMember()>
    Public Property timestamp As String = String.Empty

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="MonshinArgsJsonParamater" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region

End Class
