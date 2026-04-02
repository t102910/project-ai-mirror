@Imports MGF.QOLMS.QolmsYappli
@ModelType NoteMealSearchResultViewModel

@Code
    ViewData("Title") = "食事"
    Layout = "~/Views/Shared/_NoteLayout.vbhtml"
End Code

<body id="meal" class="lower" data-pagetype="@Me.Model.PageType">
    @Html.AntiForgeryToken()
    @*@Html.Action("NoteHeaderPartialView", "Note")*@

    <main id="main-cont" class="clearfix" role="main">
	    <section class="contents-area mb20">
		    <div class="input-area tab-pane fade in active" id="input-1">
			    <div class="input-area-inner">

                    @Html.Action("NoteMealEditSearchAreaPartialView", "Note")

                    @If Me.Model.SearchedMaxPage = 0 Then
                        @<h3 class="mb10 mt20 left">
					        検索結果（0件）
				        </h3>                    
                     ElseIf Me.Model.SearchedMaxPage > 0 Then
                        @<h3 class="mb10 mt20 left">
				            検索結果（全 @Me.Model.SearchedMaxPage ページ）
			            </h3>
                     Else
				        @<h3 class="mb10 mt20 left">
					        検索結果
				        </h3>
                    End If

                    @Html.Action("NoteMealSearchResultAreaPartialView", "Note")

			    </div>
		    </div>
	    </section>
    </main>

    @Html.Action("NoteFooterPartialView", "Note")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/note/mealsearchresult")
</body>
