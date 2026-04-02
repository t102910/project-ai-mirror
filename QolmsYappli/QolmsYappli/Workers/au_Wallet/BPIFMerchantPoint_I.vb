Imports System.Runtime.Serialization

<Serializable()>
<DataContract()>
Public NotInheritable Class BPIFMerchantPoint_I
    Inherits QyJsonParameterBase

    ''' <summary>
    ''' 加盟店ID
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property memberId As String = String.Empty

    ''' <summary>
    ''' サービスID
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property serviceId As String = String.Empty

    ''' <summary>
    ''' セキュアキー 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property secureKey As String = String.Empty

    ''' <summary>
    ''' 認証区分
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property authKbn As String = String.Empty

    ''' <summary>
    ''' openId
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property openId As String = String.Empty

    ''' <summary>
    ''' auId 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property auId As String = String.Empty

    ''' <summary>
    ''' 加盟店依頼番号
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property memberAskNo As String = String.Empty

    ''' <summary>
    ''' dispKbn 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property dispKbn As String = String.Empty
    ''' <summary>
    ''' 摘要
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property commodity As String = String.Empty

    ''' <summary>
    ''' useAuIdPoint 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property useAuIdPoint As String = String.Empty
    ''' <summary>
    ''' useCmnPoint  
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property useCmnPoint As String = String.Empty
    ''' <summary>
    ''' obtnPoint  
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property obtnPoint As String = String.Empty

    ''' <summary>
    ''' tmpObtnKbn 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property tmpObtnKbn As String = String.Empty

    ''' <summary>
    ''' pointObtnExpctDate 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property pointObtnExpctDate As String = String.Empty

    ''' <summary>
    ''' pointEffTimlmtKbn 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property pointEffTimlmtKbn As String = String.Empty

    ''' <summary>
    ''' obtnPointEffTimlmt  
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property obtnPointEffTimlmt As String = String.Empty


    ''' <summary>
    ''' obtnPointEffTimlmtKbn  
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property obtnPointEffTimlmtKbn As String = String.Empty



    #Region "Constructor"

    ''' <summary>
    ''' <see cref="BPIFMerchantPoint_I" /> クラス の新しい インスタンス を初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region


End Class
