Imports System.Runtime.Serialization

''' <summary>
''' ビュー 内に展開する タニタ 会員 QR コード 情報への参照 パラメータ を表します。
''' この クラス は継承できません。
''' </summary>
''' <remarks></remarks>
<Obsolete("実装中")>
<DataContract()>
Public NotInheritable Class NoteVitalTanitaQrReferenceJsonParameter
    Inherits QyJsonParameterBase

#Region "Public Property"

    ''' <summary>
    ''' アカウント キー を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Accountkey As String = String.Empty

    ''' <summary>
    ''' ログイン 日時を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property LoginAt As String = String.Empty

    ''' <summary>
    ''' 会員識別番号を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property MemberNo As String = String.Empty

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="NoteVitalTanitaQrReferenceJsonParameter" /> クラス の新しい インスタンス を初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region

End Class
