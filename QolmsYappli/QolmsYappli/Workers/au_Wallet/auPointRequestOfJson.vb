Imports System.Runtime.Serialization

<Serializable()>
<DataContract()>
Public NotInheritable Class auPointRequestOfJson
    Inherits QyJsonParameterBase

    ''' <summary>
    ''' 加盟店ID
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property BPIFMerchantPoint_I As New BPIFMerchantPoint_I()

    #Region "Constructor"

    ''' <summary>
    ''' <see cref="auPointRequestOfJson" /> クラス の新しい インスタンス を初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region


End Class
