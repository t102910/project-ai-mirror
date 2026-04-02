Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Json

''' <summary>
''' ファイルの分割位置と分割サイズを保持する、
''' JSON 形式の内部コンテンツを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<DataContract()>
<Serializable()>
Public NotInheritable Class ChunkUploadSliceJsonResult

#Region "Public Property"

    ''' <summary>
    ''' ファイルの分割位置を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Position As String = String.Empty

    ''' <summary>
    ''' ファイルの分割サイズを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Size As String = String.Empty

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="ChunkUploadSliceJsonResult" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class
