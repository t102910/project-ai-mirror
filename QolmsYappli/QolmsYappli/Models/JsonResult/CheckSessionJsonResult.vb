Imports System.Runtime.Serialization

''' <summary>
''' セッションが有効かチェックした結果を保持する、
''' JSON 形式のコンテンツを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<DataContract()>
<Serializable()>
Public NotInheritable Class CheckSessionJsonResult
    Inherits QyJsonResultBase

#Region "Constructor"

    ''' <summary>
    ''' <see cref="CheckSessionJsonResult" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region

End Class
