Imports System.IO
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Json
Imports System.Web.Mvc

''' <summary>
''' 「JOTO ホーム ドクター」で使用する、
''' JSON 形式の コンテンツ を応答に送信するための基本 クラス を表します。
''' </summary>
''' <remarks></remarks>
<DataContract()>
<Serializable()>
Public MustInherit Class QyJsonResultBase

#Region "Public Property"

    ''' <summary>
    ''' 処理結果を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property IsSuccess As String = Boolean.FalseString

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="QyJsonResultBase" /> クラス の新しい インスタンス を初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub New()
    End Sub

#End Region

#Region "Public Method"

    ''' <summary>
    ''' JsonResult クラス へ変換します。
    ''' </summary>
    ''' <returns>
    ''' <see cref="JsonResult" /> クラス。
    ''' </returns>
    ''' <remarks></remarks>
    Public Overridable Function ToJsonResult() As JsonResult

        ' JsonResult へ変換
        Using ms As New IO.MemoryStream()
            With New DataContractJsonSerializer(Me.GetType)
                .WriteObject(ms, Me)
            End With

            Return New JsonResult() With {
                .ContentEncoding = Encoding.UTF8,
                .ContentType = "application/json",
                .Data = Encoding.UTF8.GetString(ms.ToArray())
            }
        End Using

    End Function

    ''' <summary>
    ''' 必要に応じて プロパティ を サニタイズ して、 
    ''' <see cref="JsonResult" /> クラス へ変換します。
    ''' </summary>
    ''' <returns>
    ''' <see cref="JsonResult" /> クラス。
    ''' </returns>
    ''' <remarks></remarks>
    Public Function ToJsonResultWithSanitize() As JsonResult

        ' コピー インスタンス で操作
        Dim result As Object = Me.Copy()

        ' サニタイズ
        result.GetType().GetProperties() _
            .ToList() _
            .ForEach(
                Sub(p)
                    If p.IsDefined(GetType(QyForceSanitizing), False) Then
                        If p.PropertyType Is GetType(String) Then
                            ' String 型
                            Dim value As String = DirectCast(p.GetValue(result), String)

                            If Not String.IsNullOrWhiteSpace(value) Then p.SetValue(result, HttpUtility.HtmlEncode(value))
                        ElseIf p.PropertyType Is GetType(List(Of String)) Then
                            ' List(Of String) 型
                            Dim value As List(Of String) = DirectCast(p.GetValue(result), List(Of String)).ConvertAll(Function(i) If(String.IsNullOrWhiteSpace(i), String.Empty, HttpUtility.HtmlEncode(i)))

                            If value IsNot Nothing AndAlso value.Any() Then p.SetValue(result, value)
                        End If
                    End If
                End Sub
            )

        ' JsonResult へ変換
        Using ms As New MemoryStream()
            With New DataContractJsonSerializer(result.GetType)
                .WriteObject(ms, result)
            End With

            Return New JsonResult() With {
                .ContentEncoding = Encoding.UTF8,
                .ContentType = "application/json",
                .Data = Encoding.UTF8.GetString(ms.ToArray())
            }
        End Using

    End Function

#End Region

End Class
