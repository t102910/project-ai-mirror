Imports System.Runtime.Serialization

''' <summary>
''' バイタル 情報を登録した結果を保持する、
''' JSON 形式の コンテンツ を表します。
''' この クラス は継承できません。
''' </summary>
''' <remarks></remarks>
<DataContract()>
<Serializable()>
Public NotInheritable Class NoteVitalEditJsonResult
    Inherits QyJsonResultBase

#Region "Public Property"

    <DataMember()>
    Public Property VitalTypeN As New List(Of String)()

    ''' <summary>
    ''' エラー メッセージ の リスト を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    <QyForceSanitizing()>
    Public Property Messages As New List(Of String)()

    ''' <summary>
    ''' 身長 を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Height As String = String.Empty

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="NoteVitalEditJsonResult" /> クラス の新しい インスタンス を初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region

End Class

