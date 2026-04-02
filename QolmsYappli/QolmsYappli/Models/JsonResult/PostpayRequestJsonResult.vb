Imports System.Runtime.Serialization
''' <summary>
''' 後払いリクエストの結果を保持する、
''' JSON 形式のコンテンツを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<DataContract()>
<Serializable()>
Public NotInheritable Class PostpayRequestJsonResult
    Inherits QyJsonResultBase
#Region "Public Property"

    ''' <summary>
    ''' 処理結果を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property RequestFlag As String = Boolean.FalseString

#End Region

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
