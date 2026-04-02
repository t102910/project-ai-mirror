Imports System.Runtime.Serialization

<Serializable()>
<DataContract()>
Public Class pointIf
      Inherits QyJsonParameterBase

    ''' <summary>
    ''' 電文制御情報
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property control As New Control()

    ''' <summary>
    ''' 電文データ情報  
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property processResult As New ProcessResult()


End Class
