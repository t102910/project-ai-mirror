Imports System.Runtime.Serialization

''' <summary>
''' ビュー内に展開するファイル情報への参照パラメータを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<DataContract()>
Public NotInheritable Class FileStorageReferenceJsonParameter
    Inherits QyJsonParameterBase

#Region "Public Property"

    ''' <summary>
    ''' アカウント キーを取得または設定します。
    ''' 未指定の場合は処理対象の人物のアカウント キーとして扱います。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property AccountKey As Guid = Guid.Empty

    ''' <summary>
    ''' ファイル キーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property FileKey As Guid = Guid.Empty

    ''' <summary>
    ''' 仮アップロード時のファイル キーを取得または設定します。
    ''' この値は "yyyyMMddHHmmssfffffff" 形式の文字列です。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property TempFileKey As String = String.Empty

    ''' <summary>
    ''' ファイルの種別を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property FileType As QyFileTypeEnum = QyFileTypeEnum.None

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="FileStorageReferenceJsonParameter" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region

End Class
