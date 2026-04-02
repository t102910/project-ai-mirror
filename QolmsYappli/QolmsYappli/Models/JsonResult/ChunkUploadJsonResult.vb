Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Json

''' <summary>
''' ファイルを分割アップロードした結果を保持する、
''' JSON 形式のコンテンツを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<DataContract()>
<Serializable()>
Public NotInheritable Class ChunkUploadJsonResult
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
    ''' アップロードの進捗を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Progress As String = String.Empty

    ''' <summary>
    ''' アップロードが完了したかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property IsCompleted As String = String.Empty

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="ChunkUploadJsonResult" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region

End Class
