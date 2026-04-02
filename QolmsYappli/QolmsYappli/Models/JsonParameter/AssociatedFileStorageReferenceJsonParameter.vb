Imports System.Runtime.Serialization

''' <summary>
''' ビュー 内に展開する健診結果付随 ファイル 情報への参照 パラメータ を表します。
''' この クラス は継承できません。
''' </summary>
''' <remarks></remarks>
<DataContract()>
Public NotInheritable Class AssociatedFileStorageReferenceJsonParameter
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
    ''' 連携 システム 番号を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property LinkageSystemNo As String = String.Empty

    ''' <summary>
    ''' 連携 システム ID を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property LinkageSystemId As String = String.Empty

    ''' <summary>
    ''' 健診受診を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property RecordDate As String = String.Empty

    ''' <summary>
    ''' 施設 キー を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property FacilityKey As String = String.Empty

    ''' <summary>
    ''' データ キー を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property DataKey As String = String.Empty

    ''' <summary>
    ''' URL 連携の場合に使用する キー を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Obsolete("未使用です。")>
    <DataMember()>
    Public Property AdditionalKey As String = String.Empty

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="AssociatedFileStorageReferenceJsonParameter" /> クラス の新しい インスタンス を初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region

End Class
