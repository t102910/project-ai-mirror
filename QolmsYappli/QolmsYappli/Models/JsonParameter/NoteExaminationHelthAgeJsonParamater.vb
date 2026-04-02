Imports System.Runtime.Serialization

<DataContract()>
<Serializable()>
Public NotInheritable Class NoteExaminationHelthAgeJsonParamater
    Inherits QyJsonParameterBase

#Region "Public Property"

    <DataMember()>
    Public Property Accountkey As Guid = Guid.Empty

    <DataMember()>
    Public Property LoginAt As Date = Date.MinValue

    <DataMember()>
    Public Property healthAgeCalcN As Dictionary(Of Date, Dictionary(Of String, String))


#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="NoteExaminationHelthAgeJsonParamater" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region

End Class
