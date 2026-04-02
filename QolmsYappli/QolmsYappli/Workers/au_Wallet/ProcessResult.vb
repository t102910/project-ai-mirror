Imports System.Runtime.Serialization

<Serializable()>
<DataContract()>
Public Class ProcessResult
      Inherits QyJsonParameterBase

    ''' <summary>
    ''' ポイント返却情報
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property pointInfo As New PointInfo()

End Class