Imports System.Runtime.Serialization

''' <summary>
''' タニタ 会員 QR コード 情報を取得した結果を保持する、
''' JSON 形式の コンテンツ を表します。
''' この クラス は継承できません。
''' </summary>
''' <remarks></remarks>
<Obsolete("実装中")>
<DataContract()>
<Serializable()>
Public NotInheritable Class NoteVitalTanitaQrJsonResult
    Inherits QyJsonResultBase

#Region "Public Property"

    ''' <summary>
    ''' QR コード 文字列を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property QrCode As String = String.Empty

    ''' <summary>
    ''' 有効期限を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Experies As String = String.Empty

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="NoteVitalTanitaQrJsonResult" /> クラス の新しい インスタンス を初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region

End Class
