@Imports MGF.QOLMS.QolmsYappli
@ModelType StartSetupStepOnePartialViewModel2

<section class="contents-area" data-step-mode="1">
	<h2 class="title relative">
		基本情報
		<img id="s-1" src="../dist/img/tmpl/s-1.png">
		<img id="y-1" src="../dist/img/tmpl/y-1.png">
	</h2>
	<hr>
	<div class="section default">
		あなたの1日の適正エネルギー量をチェックしましょう！<br>
		下の項目を全て記入してね
        <p class="center mt10">
			<a href="javascript:void(0);" id="skip" class="btn btn-default">スキップして始める！</a>
			<span class="small">（この設定はその他メニューの「目標設定」から設定できます。）</span>
		</p>
	</div>

    <form class="form wizard-form mb30">
		<label for="input1" class="t-row line">
			<span class="label-txt">
				<span class="ico required">必須</span>
				性 別
			</span>
            @QyHtmlHelper.ToSexDropDownList("input1", "val1", Me.Model.PageViewModel.SexType.ToString())
            @QyHtmlHelper.ToValidationMessageArea({"model.SexType"}, Me.TempData("ErrorMessage"), "alert alert-danger thin mt10", True)
		</label>

		<div class="t-row line">
		    <label for="input2" class="label-txt">
                <span class="ico required">必須</span>
                生年月日
		    </label>
            @QyHtmlHelper.ToYearDropDownList("input2", "val2", Me.Model.PageViewModel.BirthYear, "form-control mb10")
            @QyHtmlHelper.ToMonthDropDownList("input3", "val3", Me.Model.PageViewModel.BirthMonth, "form-control mb10")
            @QyHtmlHelper.ToDayDropDownList("input4", "val4", Me.Model.PageViewModel.BirthDay, "form-control")
            @QyHtmlHelper.ToValidationMessageArea({"model.BirthYear"}, Me.TempData("ErrorMessage"), "alert alert-danger thin mt10", True)
		</div>

		<label for="input5" class="t-row line">
			<span class="label-txt">
				<span class="ico required">必須</span>
				身 長
			</span>
			<div class="input-group datetime-select">
				@*<input type="number" id="input5"  name="val5" class="form-control" value="@IIf(Me.Model.PageViewModel.Height > Decimal.Zero, Me.Model.PageViewModel.Height.ToString(), String.Empty)" placeholder="身長を入力" maxlength="5" style="ime-mode:disabled;" autocomplete="off">*@
                <input type="number" id="input5"  name="val5" class="form-control" value="@Me.Model.PageViewModel.Height" placeholder="身長を入力" maxlength="5" style="ime-mode:disabled;" autocomplete="off">                                                        
				<span class="input-group-addon">cm</span>
			</div>
			@QyHtmlHelper.ToValidationMessageArea({"model.Height"}, Me.TempData("ErrorMessage"), "alert alert-danger thin mt10", True)
		</label>
			
		<label for="input6" class="t-row line">
			<span class="label-txt">
				<span class="ico required">必須</span>
				体 重
			</span>
			<div class="input-group datetime-select">
				@*<input type="number" id="input6" name="val6" class="form-control" value="@IIf(Me.Model.PageViewModel.Weight > Decimal.Zero, Me.Model.PageViewModel.Weight.ToString(), String.Empty)" placeholder="体重を入力" maxlength="5" style="ime-mode:disabled;" autocomplete="off">*@
                <input type="number" id="input6" name="val6" class="form-control" value="@Me.Model.PageViewModel.Weight" placeholder="体重を入力" maxlength="5" style="ime-mode:disabled;" autocomplete="off">
				<span class="input-group-addon">kg</span>
			</div>
			@QyHtmlHelper.ToValidationMessageArea({"model.Weight"}, Me.TempData("ErrorMessage"), "alert alert-danger thin mt10", True)
		</label>

		<div id="momentum" class="t-row">
			<span class="label-txt">
				<span class="ico required">必須</span>
				あなたの運動量
			</span>
			<label>
				<span></span>
				<span>運動量</span>
				<span>１日の過ごし方</span>
			</label>
			<label for="input7">
				<span><input type="radio" id="input7" name="val7" value="1" @Html.Raw(IIf(Me.Model.PageViewModel.PhysicalActivityLevel = 1, "checked='checked'", String.Empty))></span>
				<span>少ない人</span>
				<span>デスクワーク中心</span>
			</label>
			<label for="input8">
				<span><input type="radio" id="input8" name="val7" value="2" @Html.Raw(IIf(Me.Model.PageViewModel.PhysicalActivityLevel = 2, "checked='checked'", String.Empty))></span>
				<span>普通の人</span>
				<span>立ち仕事中心</span>
			</label>
			<label for="input9">
				<span><input type="radio" id="input9" name="val7" value="3" @Html.Raw(IIf(Me.Model.PageViewModel.PhysicalActivityLevel = 3, "checked='checked'", String.Empty))></span>
				<span>多めの人</span>
				<span>力仕事中心</span>
			</label>
			@QyHtmlHelper.ToValidationMessageArea({"model.PhysicalActivityLevel"}, Me.TempData("ErrorMessage"), "alert alert-danger thin mt10", True)
		</div>

		<div class="submit-area">
			<a href="javascript:void(0);" class="btn btn-submit">次 へ</a>
		</div>
    </form>
</section>

<section class="progress-area">
	<h3>利用開始まであと<strong>2</strong>ステップ！</h3>
	<ul class="progressbar clearfix">
		<li class="here"><span>情報入力</span></li>
@*		<li><span>現状把握</span></li>*@
		<li><span>目標設定</span></li>
		<li><span>利用開始！</span></li>
	</ul>
</section>
