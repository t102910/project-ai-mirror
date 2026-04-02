@Imports MGF.QOLMS.QolmsYappli
@ModelType PortalTermsViewModel


@Code
    ViewData("Title") = "Terms"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"
    
    Dim isYappli As Boolean = Me.Context.Request.UserAgent.ToLower().Contains("yappli")
    
End Code

<body id="" class="lower">
    @Html.AntiForgeryToken()
    @*@Html.Action("PortalHeaderPartialView", "Portal")*@

    <main id="main-cont" class="clearfix" role="main">
	    <section class="contents-area">
		    <h2 class="title relative">利用規約</h2>
		    <hr>
            <p class="section default">
			    アカウント登録時（@Me.Model.AcceptDate.ToString("yyyy年M月d日")）に同意頂いた利用規約は以下となります。
		    </p>
            <div class="mb30">
                @Html.Raw(Me.Model.TermsText)
            </div>
            @If Not isYappli Then
				@<div class="submit-area">
                     <a href="../Portal/Information" class="btn btn-close no-ico">戻 る</a>
				 </div>
            End If
	    </section>
    </main>
    @Html.Action("PortalFooterPartialView", "Portal")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/portal/terms")

</body>
