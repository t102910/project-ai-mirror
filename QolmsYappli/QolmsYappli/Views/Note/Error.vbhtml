@Imports MGF.QOLMS.QolmsYappli

@Code
    ViewData("Title") = "エラー"
    Layout = "~/Views/Shared/_ErrorLayout.vbhtml"
    
    Dim segments() As String = Me.Context.Request.Url.Segments
    Dim returnPath As String = String.Empty
    
    If segments IsNot Nothing AndAlso segments.Length = 3 Then
        returnPath = String.Format("..{0}{1}{2}", segments(0).ToLower(), segments(1).ToLower(), segments(2).ToLower().Replace("result", String.Empty))
    End If
End Code

<body id="error" class="lower">
    <main id="main-cont" class="" role="main">
        <section class="contents-area">
            <h2 class="title">
                エラーが発生しました
            </h2>
            <hr>
            <div class="center">
                <img src="../dist/img/tmpl/error.png" class="w200 mb20">
            </div>

            <section class="section caution">
                @If Me.TempData.ContainsKey("ErrorMessage") AndAlso Not String.IsNullOrWhiteSpace(Me.TempData("ErrorMessage").ToString()) Then
                    @QyHtmlHelper.CrLfToBreakTag(Me.TempData("ErrorMessage").ToString())
                Else
                    @Html.Raw("ページの表示に失敗しました。")
                End If
            </section>

			<div class="submit-area">
                <a href="@returnPath" class="btn btn-close no-ico">戻 る</a>
			</div>
        </section>
    </main>

    @Html.Action("NoteFooterPartialView", "Note")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/note/error")
</body>
