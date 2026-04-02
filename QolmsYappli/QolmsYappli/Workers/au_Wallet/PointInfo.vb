Imports System.Runtime.Serialization

<Serializable()>
<DataContract()>
Public Class PointInfo
      Inherits QyJsonParameterBase

    ''' <summary>
    ''' ポイント受付番号 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property pointReceiptNo As String = String.Empty

    ''' <summary>
    ''' 処理日
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property processDay As String = String.Empty

    ''' <summary>
    ''' 処理時間
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property processTime As String = String.Empty

    ''' <summary>
    ''' 獲得発生年月日
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property obtnHpnDay As String = String.Empty

    ''' <summary>
    ''' 利用WALLETポイント発生年月日
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property useAuIDHpnDay As String = String.Empty

    ''' <summary>
    ''' 利用auポイント発生年月日
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property useCmnHpnDay As String = String.Empty

End Class