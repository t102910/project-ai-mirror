Imports System.Runtime.Serialization

''' <summary>
''' カロミルのトークンのセットを格納するクラスです。
''' </summary>
''' <remarks></remarks>
<Serializable()>
<DataContract()>
Public NotInheritable Class CalomealInstructInfo
#Region "Public Property"

        
    ''' <summary>
    ''' ユーザー名
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember>
    Public Property UserName As String = String.Empty

    ''' <summary>
    ''' 店名
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember>
    Public Property StoreName As String = String.Empty

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="FitbitTokenSet" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class
