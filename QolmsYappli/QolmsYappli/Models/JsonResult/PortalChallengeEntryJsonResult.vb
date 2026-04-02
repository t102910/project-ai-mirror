Imports System.Runtime.Serialization

''' <summary>
''' チャレンジエントリー画面 POSTの結果を保持する、
''' JSON 形式のコンテンツを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<DataContract()>
<Serializable()>
Public Class PortalChallengeEntryJsonResult
    Inherits QyJsonResultBase

#Region "Public Property"

    ''' <summary>
    ''' エラーメッセージを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Messages As New Dictionary(Of String, String)

    ''' <summary>
    ''' チャレンジキーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Key As String

    ''' <summary>
    ''' 住所を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Address As String

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PortalChallengeEntryJsonResult" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region
End Class
