Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Json

''' <summary>
''' ファイルの分割アップロードを開始した結果を保持する、
''' JSON 形式のコンテンツを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<DataContract()>
<Serializable()>
Public NotInheritable Class ChunkUploadStartJsonResult
    Inherits QyJsonResultBase

#Region "Public Property"

    ''' <summary>
    ''' ファイル キーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Key As String = String.Empty

    ''' <summary>
    ''' ファイルの分割位置と分割サイズのリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property SliceN As New List(Of ChunkUploadSliceJsonResult)()

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
    ''' <see cref="ChunkUploadStartJsonResult" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region

End Class
