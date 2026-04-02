Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Json

''' <summary>
''' データチャージPOSTの結果を保持する、
''' JSON 形式のコンテンツを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<DataContract()>
<Serializable()>
Public Class NoteExaminationPdfJsonResult
    Inherits QyJsonResultBase

#Region "Public Property"

    ''' <summary>
    ''' エラーメッセージを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Message As String = String.Empty

    ''' <summary>
    ''' PDFファイルのバイナリーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property PdfBinary As Byte() = Nothing

    ''' <summary>
    ''' PDFファイルのファイル名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property PdfFileName As String = String.Empty


#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PortalDatachargeJsonResult" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

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
    Public Overloads Function ToJsonResult(allowGet As Boolean) As JsonResult

        ' JsonResult へ変換
        Using ms As New IO.MemoryStream()
            With New DataContractJsonSerializer(Me.GetType)
                .WriteObject(ms, Me)
            End With

            If allowGet Then
                Return New JsonResult() With {
                    .ContentEncoding = Encoding.UTF8,
                    .ContentType = "application/json",
                    .Data = Encoding.UTF8.GetString(ms.ToArray()),
                    .JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    .MaxJsonLength = Integer.MaxValue
                }
            Else
                Return New JsonResult() With {
                  .ContentEncoding = Encoding.UTF8,
                  .ContentType = "application/json",
                  .Data = Encoding.UTF8.GetString(ms.ToArray())
              }
            End If

    
        End Using

    End Function

#End Region

End Class
