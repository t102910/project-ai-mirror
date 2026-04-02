
Imports System.Runtime.Serialization

<Serializable()>
<DataContract()>
Public Class Control
      Inherits QyJsonParameterBase

    ''' <summary>
    ''' 処理結果コード 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property resultCd As String = String.Empty

End Class
