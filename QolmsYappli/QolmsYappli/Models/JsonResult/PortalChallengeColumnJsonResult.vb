Imports System.Runtime.Serialization

''' <summary>
''' チャレンジエントリー画面 POSTの結果を保持する、
''' JSON 形式のコンテンツを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<DataContract()>
<Serializable()>
Public Class PortalChallengeColumnJsonResult
    Inherits QyJsonResultBase

#Region "Public Property"


#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PortalChallengeColumnJsonResult" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region
End Class
