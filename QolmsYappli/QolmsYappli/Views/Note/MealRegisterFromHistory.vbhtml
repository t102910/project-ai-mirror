@Imports MGF.QOLMS.QolmsYappli
@ModelType NoteMealViewModel4

@Code
    ViewData("Title") = "食事"
    Layout = "~/Views/Shared/_NoteLayout.vbhtml"
    End Code

<body id="meal" class="lower meal_history">
    @Html.AntiForgeryToken()
    @*@Html.Action("NoteHeaderPartialView", "Note")*@

<main id="main-cont" class="clearfix" role="main">
	<section class="contents-area mb20">
		<div class="input-area" id="input-1">
@*			<div class="target-date-time">
                <div class="input-group mb10">
					<span class="input-group-addon">食事日</span>
					<input type="text" id="recordDate" class="form-control picker" name="RecordDate" value="@String.Format("{0:yyyy年MM月dd日}", Me.Model.FilterDate)" readonly="readonly" style="background-color:white;" autocomplete="off">
				</div>
                @QyHtmlHelper.ToMealTypeRadioButton("mealtype", "mealtype", Me.Model.MealType.ToString(), css:="mb10 mealtime")
                <div class="input-group datetime-select mb10">
				    <span class="input-group-addon">時間</span>
				    @QyHtmlHelper.ToMeridiemDropDownList("meridiem", "meridiem", Me.Model.FilterDate.ToString("tt", System.Globalization.CultureInfo.InvariantCulture).ToLower(), css:="form-control ap")
                    @QyHtmlHelper.ToHourDropDownList("hour", "hour", (Me.Model.FilterDate.Hour Mod 12).ToString, css:="form-control hour")
                    @QyHtmlHelper.ToMinuteDropDownList("minute", "minute", Me.Model.FilterDate.Minute, css:="form-control minute")
			    </div>
			</div>*@
			
            @Html.Action("NoteMealEditDatePartialView", "Note")

			<div class="input-area-inner">				
				<div class="submit-area type-3">
					<a href="/Note/Meal4" class="btn btn-close">戻 る</a>
					<a href="javascript:void(0);" class="btn btn-submit upload disabled">登 録</a>
				</div>
			</div>

            <section class="section caution mt10 mb0 hide"><h4>heading...</h4>アラートエリアです。不要時.hideで消してください。</section>
		</div>
	</section>	
	
    <!-- 一覧 -->
	<section class="data-area">
		<h3 class="title mt10 two-pane">
			<span id="filter-title">最近の食事</span>
			<span class="last-child"><span class="calendar-view"><i class="la la-calendar la-2x"></i>絞り込み</span></span>
		</h3>
        <div class="navigation-area type-2">
			<p class="balloon">
				複数の食事を選択して一括登録が可能です！
			</p>
			<img class="shika-image" src="/dist/img/tmpl/s-1.png">
		</div>
		<div id="calender-area">
			<p class="wrap">
				<input type="text" id="filter-date" class="form-control picker" name="" value="@String.Format("{0:yyyy年MM月dd日}", Me.Model.FilterDate)" readonly="readonly" style="background-color:white;" autocomplete="off" placeholder="日付を変更">
				<span class="submit-area-2"><span id="filter-submit" class="btn btn-submit narrow">絞り込む</span></span>
			</p>
		</div>
        @Html.Action("NoteMealRegisterFromHistoryCardAreaPartialView", "Note")
    </section>
</main>

    @Html.Action("NoteFooterPartialView", "Note")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/note/mealregisterfromhistory")
</body>
