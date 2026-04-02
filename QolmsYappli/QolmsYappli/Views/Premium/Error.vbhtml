@Imports MGF.QOLMS.QolmsYappli

@Code
    ViewData("Title") = "エラー"
    Layout = "~/Views/Shared/_ErrorLayout.vbhtml"
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
                        @QyHtmlHelper.CrLfToBreakTag(Me.TempData("ErrorMessage").ToString)
                    Else
                        @Html.Raw("ページの表示に失敗しました。")
                    End If
                </section>

			    <div class="submit-area">
                    <a href="~/Premium/index" class="btn btn-close no-ico">戻 る</a>
			    </div>
            </section>
        
    </main>

    @QyHtmlHelper.RenderScriptTag("~/dist/js/premium/error")
</body>
