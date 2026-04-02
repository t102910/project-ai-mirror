Imports System.Runtime.Serialization

''' <summary>
''' 添付ファイルを追加した結果を保持する、
''' JSON 形式のコンテンツを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<DataContract()>
<Serializable()>
Public NotInheritable Class AttachedFileAddJsonResult
    Inherits QyJsonResultBase

#Region "Public Property"

    ''' <summary>
    ''' 暗号化されたサムネイル画像ファイル情報への参照パラメータを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property ThumbnailReference As String = String.Empty

    ''' <summary>
    ''' 暗号化されたファイル情報への参照パラメータを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property DataReference As String = String.Empty

    ''' <summary>
    ''' オリジナル ファイル名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property OriginalName As String = String.Empty

    ''' <summary>
    ''' エラー メッセージを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Message As String = String.Empty

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="AttachedFileAddJsonResult" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region

End Class
