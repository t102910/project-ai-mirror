Imports System.Runtime.Serialization

''' <summary>
''' バイタル 情報を削除した結果を保持する、
''' JSON 形式の コンテンツ を表します。
''' この クラス は継承できません。
''' </summary>
''' <remarks></remarks>
<DataContract()>
<Serializable()>
Public NotInheritable Class NoteVitalDeleteJsonResult
    Inherits QyJsonResultBase

#Region "Public Property"

    ''' <summary>
    ''' バイタル 情報の種別を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property VitalType As String = String.Empty

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
    ''' <see cref="NoteVitalDeleteJsonResult" /> クラス の新しい インスタンス を初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region

End Class
