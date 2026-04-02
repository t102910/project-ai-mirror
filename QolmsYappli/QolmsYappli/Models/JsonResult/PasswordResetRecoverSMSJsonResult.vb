Imports System.Runtime.Serialization

''' <summary>
''' パスワードリセットパスコード取得のPOSTの結果を保持する、
''' JSON 形式のコンテンツを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<DataContract()>
<Serializable()>
Public Class PasswordResetRecoverSMSJsonResult
    Inherits QyJsonResultBase

#Region "Public Property"

    ''' <summary>
    ''' エラーメッセージを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Messages As New Dictionary(Of String, String)()

    ''' <summary>
    ''' パスコードの入力を使用できなくするを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property PassDisabled As String = String.Empty


#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PasswordResetRecoverSMSJsonResult" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region
End Class
