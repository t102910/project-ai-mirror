Imports System.Runtime.Serialization

''' <summary>
''' HOMEのタスク用POSTの結果を保持する、
''' JSON 形式のコンテンツを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<DataContract()>
<Serializable()>
Public Class PortalHomeTaskJsonResult
    Inherits QyJsonResultBase

#Region "Public Property"

    ''' <summary>
    ''' 歩数の登録があるかどうかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns>on:登録済み string.enpty:未登録</returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Steps As String = String.Empty

    ''' <summary>
    ''' 体重の登録があるかどうかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns>on:登録済み string.enpty:未登録</returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Weight As String = String.Empty

    ''' <summary>
    ''' 朝食の登録があるかどうかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns>on:登録済み string.enpty:未登録</returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Breakfast As String = String.Empty

    ''' <summary>
    ''' 昼食の登録があるかどうかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns>on:登録済み string.enpty:未登録</returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Lunch As String = String.Empty

    ''' <summary>
    ''' 夕食の登録があるかどうかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns>on:登録済み string.enpty:未登録</returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Dinner As String = String.Empty

    ''' <summary>
    ''' コラムの既読の登録があるかどうかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns>on:既読 string.enpty:未読 disabled:コラムがない日 hide:チャレンジ未参加</returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Column As String = String.Empty

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PortalHomeTaskJsonResult" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region
End Class
