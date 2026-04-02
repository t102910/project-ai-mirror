Imports System.Runtime.Serialization

''' <summary>
''' パスワードリセットURL取得のPOSTの結果を保持する、
''' JSON 形式のコンテンツを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<DataContract()>
<Serializable()>
Public Class PasswordResetRecoverJsonResult
    Inherits QyJsonResultBase

#Region "Public Property"

    ''' <summary>
    ''' 計算結果のリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Messages As New Dictionary(Of String, String)()

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PasswordResetRecoverJsonResult" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region
End Class
