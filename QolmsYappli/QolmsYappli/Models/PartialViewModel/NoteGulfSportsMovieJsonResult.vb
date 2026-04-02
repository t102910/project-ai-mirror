Imports System.Runtime.Serialization

''' <summary>
''' ガルフスポーツ運動登録時の結果を保持する、
''' JSON 形式のコンテンツを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<DataContract()>
<Serializable()>
Public NotInheritable Class NoteGulfSportsMovieJsonResult
    Inherits QyJsonResultBase

#Region "Public Property"

    ''' <summary>
    ''' エラーメッセージのリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Messages As New List(Of String)()

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="NoteEditJsonResult" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region

End Class