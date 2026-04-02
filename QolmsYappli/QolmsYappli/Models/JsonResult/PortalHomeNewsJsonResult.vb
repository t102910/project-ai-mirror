Imports System.Runtime.Serialization

''' <summary>
''' HOMEのNEWS取得用POSTの結果を保持する、
''' JSON 形式のコンテンツを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<DataContract()>
<Serializable()>
Public Class PortalHomeNewsJsonResult
    Inherits QyJsonResultBase

#Region "Public Property"

    ''' <summary>
    ''' newsを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property News As New List(Of String)()

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PortalHomeNewsJsonResult" /> クラス の新しい インスタンス を初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region
End Class
