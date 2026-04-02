@Imports MGF.QOLMS.QolmsYappli
@ModelType NoteMealListInputModel

@Code
    ViewData("Title") = "食事"
    Layout = "~/Views/Shared/_NoteLayout.vbhtml"
End Code

<body id="meal" class="lower" data-next="@Me.Model.InputNext">
    @Html.AntiForgeryToken()
    @*@Html.Action("NoteHeaderPartialView", "Note")*@

<main id="main-cont" class="clearfix" role="main">
	<section class="contents-area mb100">
		<div class="input-area" id="input-1">
@*			<div class="target-date-time"><!-- 続けて登録時に隠す -->
				<div class="input-group mb10">
					<span class="input-group-addon">食事日</span>
                    <input type="text" id="record-date" class="form-control picker" name="record-date" value="@String.Format("{0:yyyy年MM月dd日}", Me.Model.RecordDate)" readonly="readonly" style="background-color:white;" autocomplete="off">
				</div>
                    @QyHtmlHelper.ToMealTypeRadioButton("mealtype", "mealtype", Me.Model.MealType.ToString(), css:="mb10 mealtime")
				<div class="input-group datetime-select mb10">
					<span class="input-group-addon">時間</span>
				    @QyHtmlHelper.ToMeridiemDropDownList("meridiem", "meridiem", Me.Model.Meridiem, css:="form-control ap")
                    @QyHtmlHelper.ToHourDropDownList("hour", "hour", Me.Model.Hour, css:="form-control hour")
                    @QyHtmlHelper.ToMinuteDropDownList("minute", "minute", Me.Model.Minute, css:="form-control minute")
				</div>
			</div>*@

            @Html.Action("NoteMealEditDatePartialView", "Note")

			<div class="input-area-inner">
                <div class="input-next mb10 hide"><!-- 続けて登録時に表示する -->
					<p class="mb5 input-next"><strong><em>12月23日</em>（<em>午前10時13分</em>）の<em>朝食</em></strong>を<span class="inline-block">続けて登録する</span></p>
				</div><!-- input-next end -->

@*                <div class="flex-wrap mb10 hide"><!-- 続けて登録時に表示する -->
					<div class="inline-input">
						<div id="search-input" class="w-max">
							<div class="input-group w-max">
								<span class="input-group-addon font-s">品名検索</span>
                                <input id="searchText" name="searchText" type="text" value="" class="form-control" placeholder="品名を入力" maxlength="100" autocomplete="off" accesskey="\">
							</div>
						</div>
					</div>
					<div class="submit-area">
						<a href="javascript:void(0);" class="btn btn-submit search search-result-btn disabled" id="search">検索</a>
					</div>
				</div>*@

                @Html.Action("NoteMealEditSearchAreaPartialView", "Note")

@*				<div class="input-group mb10">
					<span class="input-group-addon">品目</span>
                    <input id="hinmoku-2" name="hinmoku" type="text" value="@Me.Model.ItemName(0)" data-pal="@Me.Model.PalString(0)" class="form-control input" placeholder="品目を入力" maxlength="100" autocomplete="off">
				</div>
				<div class="input-group mb10" class="line">
					<span class="input-group-addon">カロリー</span>
                    <input id="cal-2" name="cal" type="tel" value="@Me.Model.Calorie(0)" class="form-control input" placeholder="カロリーを入力" maxlength="4" style="ime-mode:disabled;" autocomplete="off">
                    <span class="input-group-addon">kcal</span>
				</div>*@
				
                @Html.Action("NoteMealEditHinmokuCalPartialView", "Note")

                <div class="thumbnail-area mt10 hide"><!-- inputfile登録後このエリアを表示します。 -->
					<p class="meal-photo">
						<img src="" data-file-name="">
						<span class="remover"><i class="la la-remove"></i></span>
					</p>
				</div>

				<div class="input-group type-2">
					<div class="img-upload">
						<label for="sample1">
							<i class="la la-camera"></i>写真を追加
							<input type="file" id="sample1" accept="image/*" disabled="disabled">
						</label>
					</div>
					<section class="section caution mt10 mb0 hide" id="photo-caution"><h4>heading...</h4>アラートエリアです。不要時.hideで消してください。</section>
				</div>

				<div class="submit-area mb30">
                    <!-- 続けて登録時に隠す -->
					<a href="/Note/Meal4" class="btn btn-close" id="back">戻 る</a>
					<a href="javascript:void(0);" class="btn btn-submit upload" id="addfirstS">登 録</a>

                    <!-- 続けて登録時に表示する -->
                    <a href="/Note/Meal4" class="btn btn-close upload hide" id="finish">登録完了</a>
                    <a href="javascript:void(0);" class="btn btn-submit upload disabled hide" id="addnext"
                        data-recorddate="" data-mealtype="" data-mealtypetext="" data-meridiem="" data-hour="" data-minute="">登 録</a>
				</div>
                <section class="section caution mt10 mb0 hide" id="caution"><h4>heading...</h4>アラートエリアです。不要時.hideで消してください。</section>
			</div>
		</div>
	</section>

	<canvas id="canvas" width="0" height="0"></canvas>
</main>

        @Html.Action("NoteFooterPartialView", "Note")

        @QyHtmlHelper.RenderScriptTag("~/dist/js/note/mealregisterfromsearch")

</body>
