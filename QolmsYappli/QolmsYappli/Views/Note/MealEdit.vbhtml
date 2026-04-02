@Imports MGF.QOLMS.QolmsYappli
@ModelType NoteMealListInputModel

@Code
    ViewData("Title") = "食事"
    Layout = "~/Views/Shared/_NoteLayout.vbhtml"
    
    Dim resultCount As Integer = 0
End Code

<body id="meal" class="lower" data-search="@Me.Model.SearchFlag.ToString()">
    @Html.AntiForgeryToken()
    @*@Html.Action("NoteHeaderPartialView", "Note")*@

<main id="main-cont" class="clearfix" role="main">
	<section class="contents-area mb100">
		<div class="input-area" id="input-1">

            @Html.Action("NoteMealEditDatePartialView", "Note")

		    <div class="input-area-inner">

                @Html.Action("NoteMealAnalyzeResultPartialView", "Note", New RouteValueDictionary From {{"pageType", "Edit"}})

			    <hr class="line">
			    <div class="navigation-area">
				    <p class="balloon">
					    解析結果が見つからなかったら検索してね！
				    </p>
				    <img class="shika-image" src="/dist/img/tmpl/s-1.png">
			    </div>

                @Html.Action("NoteMealEditSearchAreaPartialView", "Note")

			    <hr class="line">
			    <div class="navigation-area">
				    <p class="balloon">
					    品名・カロリーを入力して登録することもできます。
				    </p>
				    <img class="shika-image" src="/dist/img/tmpl/s-6.png">
			    </div>

                @Html.Action("NoteMealEditHinmokuCalPartialView", "Note")

                <div class="thumbnail-area mt10 edit-thumbnail-area hide"><!-- inputfile登録後このエリアを表示します。 -->
					<p class="meal-photo meal-photo-edit">
						<img src="@Me.Model.PhotoData" data-file-name="@Me.Model.PhotoName">
						<span class="remover"><i class="la la-remove"></i></span>
					</p>
				</div>

				<div class="input-group type-2">
					<div class="img-upload">
						<label for="sample1">
							<i class="la la-camera"></i>写真を変更
							<input type="file" id="sample1" accept="image/*" disabled="disabled">
						</label>
					</div>
					<section class="section caution mt10 mb0 hide" id="photo-caution"><h4>heading...</h4>アラートエリアです。不要時.hideで消してください。</section>
				</div>

			    <div class="submit-area mb30">
                    <a href="/Note/Meal4" class="btn btn-close" id="back">戻 る</a>
				    <a href="javascript:void(0);" class="btn btn-submit upload" id="addfirstS">登 録</a>
			    </div>
                <section class="section caution mt10 mb10 hide" id="caution"><h4>heading...</h4>アラートエリアです。不要時.hideで消してください。</section>
		    </div>
        </div>
	</section>

	<canvas id="canvas" width="0" height="0"></canvas>
</main>

    @Html.Action("NoteFooterPartialView", "Note")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/note/mealedit")
</body>
