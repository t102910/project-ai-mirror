Imports System.Web.Mvc

Public Class CustomActionResult
    Inherits ActionResult

    Public Url As String = String.Empty
    Public PostData As Dictionary(Of String, Object) = Nothing
    Public Encoding As String = "utf-8"

    Private Function BuildPostForm(url As String, postData As Dictionary(Of String, Object), encoding As String) As String
        Dim formId As String = "__PostForm"
        Dim strForm As New StringBuilder()
        With strForm
            .AppendLine("<html>")
            .AppendLine("<head>")
            .AppendFormat("<meta http-equiv='content-type' content='application/x-www-form-urlencoded; charset='{0}'>", encoding)
            .AppendLine("</head>")
            .AppendFormat("<body onload=""document.charset='{0}'; document.forms[0].submit();"">", encoding)
            .AppendFormat("<form id='{0}' name='{0}' action='{1}' method='POST' accept-charset='{2}' enctype='application/x-www-form-urlencoded' >", formId, url, encoding)
            For Each item As KeyValuePair(Of String, Object) In postData
                .AppendFormat("<input type='hidden' name='{0}' value='{1}'/>", item.Key, item.Value)
            Next
            .AppendLine("</form>")
            .AppendLine("</body>")
            .AppendLine("</html>")
        End With
        Return strForm.ToString()
    End Function

    Public Overrides Sub ExecuteResult(context As ControllerContext)
        Dim strHtml As String = Me.BuildPostForm(Me.Url, Me.PostData, Me.Encoding)
        context.HttpContext.Response.Write(strHtml)
    End Sub

    Public Sub New(usl As String, postData As Dictionary(Of String, Object), Optional encoding As String = "utf-8")
        Me.Url = usl
        If postData Is Nothing Then postData = New Dictionary(Of String, Object)
        Me.PostData = postData
        Me.Encoding = encoding
    End Sub
    Public Shared Function RedirectAndPost(url As String, postData As Dictionary(Of String, Object), Optional encoding As String = "utf-8") As ActionResult
        Return New CustomActionResult(url, postData, encoding)
    End Function

End Class
