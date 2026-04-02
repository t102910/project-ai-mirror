@Imports MGF.QOLMS.QolmsYappli
@ModelType StartSetupStepThreePartialViewModel2

@Code
    Dim StdTargetDate As String = DateAdd(DateInterval.Month, 1, Date.Now.Date).ToString("yyyy年MM月dd日")
    Dim StdCaloriesIn As Integer = Me.Model.PageViewModel.StdEstimatedEnergyRequirement
    Dim StdCaloriesOut As Integer = StdCaloriesIn
    Dim hasError As Boolean = Me.TempData("ErrorMessage") IsNot Nothing AndAlso TypeOf Me.TempData("ErrorMessage") Is Dictionary(Of String, String) AndAlso DirectCast(Me.TempData("ErrorMessage"), Dictionary(Of String, String)).Any()
End Code

<section class="contents-area" data-step-mode="3">
	<h2 class="title relative">
		目標の設定！
		<img id="y-1" src="../dist/img/tmpl/s-3.png">
	</h2>
	<hr>
	<div class="section default">
		あなたの1日の目標エネルギー量を設定しましょう！<br>
		下の質問欄に答えると、目標の値が変わります！
		または、直接入力して目標を立てましょう！
        <p class="center mt10">
			<a href="javascript:void(0);" id="skip" class="btn btn-default">スキップして始める！</a>
			<span class="small">（この設定はその他メニューの「目標設定」から設定できます。）</span>
		</p>
	</div>

	<form>
@*      <div class="center mb30 wizard-form radio-btn-style">
			<input type="radio" id="input1" name="val1" value="1">
			<label for="input1">バリバリ指導して欲しい！</label>
			<input type="radio" id="input2" name="val1" value="2">
			<label for="input2">やさしく指導して欲しい♪</label>
			<input type="radio" id="input3" name="val1" value="3">
			<label for="input3">ほっといて欲しい…</label>
		</div>*@

@*        <div class="center mb30 cal-info-wrap">
            （※仮デザインです。下記は選択肢になります。）
        </div>*@

		<h3 class="title mt20 two-pane">目標入力</h3>
		<h4 class="">目標体重</h4>
		<label class="t-row line">
			<div class="input-group datetime-select mb5">
                <input type="number" id="input3" name="val3" class="form-control" value="@Me.Model.PageViewModel.TargetWeight" placeholder="目標体重を入力" maxlength="5" style="ime-mode:disabled;" autocomplete="off">
				<span class="input-group-addon">kg</span>
			</div>
            <section id="caution-targetweight" class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
        </label>
		<h4 class="">いつまでに</h4>
		<label class="t-row">
			<div class="input-group datetime-select mb5">
				<span class="input-group-addon">期限日</span>
                <input type="text" id="input4" name="val4" class="form-control picker" value="@IIf(Me.Model.PageViewModel.TargetDate = String.Empty, StdTargetDate, Me.Model.PageViewModel.TargetDate)" readonly="readonly" style="background-color:white;" autocomplete="off">
			</div>
            <section id="caution-targetdate" class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
        </label>
		<div class="submit-area">
			<a href="javascript:void(0);" class="btn btn-submit" id="calc">カロリー目標を計算</a>
		</div>
		
        <h4 class="">目標とする「摂取カロリー」</h4>
		<label for="input1" class="t-row">
		    <div class="input-group datetime-select mb5">
    @*				<input type="tel" id="input1" name="val1" class="form-control" value="@IIf(Me.Model.PageViewModel.CaloriesIn > 0, Me.Model.PageViewModel.CaloriesIn.ToString(), StdCaloriesIn.ToString())" placeholder="摂取カロリーを入力" maxlength="4" style="ime-mode:disabled;" autocomplete="off">*@
                <input type="tel" id="input1" name="val1" class="form-control" value="@IIf(hasError, Me.Model.PageViewModel.CaloriesIn, StdCaloriesIn.ToString())" placeholder="摂取カロリーを入力" maxlength="4" style="ime-mode:disabled;" autocomplete="off">
			    <span class="input-group-addon">kcal</span>
		    </div>
            <span class="small">@String.Format("あなたの基礎代謝量は{0:#,###}kcalです！", Me.Model.PageViewModel.NowBasalMetabolism)</span>
            @QyHtmlHelper.ToValidationMessageArea({"model.CaloriesIn"}, Me.TempData("ErrorMessage"), "alert alert-danger thin mt10", True)
            <section id="caution-caloriesin" class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
		</label>
@*		<h4 class="">目標とする「消費カロリー」</h4>
		<label for="input2" class="t-row">*@
			<div class="input-group datetime-select mb5 hide">
				@*<input type="tel" id="input2" name="val2" class="form-control"" value="@IIf(Me.Model.PageViewModel.CaloriesOut > 0, Me.Model.PageViewModel.CaloriesOut.ToString(), StdCaloriesOut.ToString())" placeholder="消費カロリーを入力" maxlength="4" style="ime-mode:disabled;" autocomplete="off">*@
                <input type="tel" id="input2" name="val2" class="form-control"" value="@IIf(hasError, Me.Model.PageViewModel.CaloriesOut, StdCaloriesOut.ToString())" placeholder="消費カロリーを入力" maxlength="4" style="ime-mode:disabled;" autocomplete="off">
				@*<span class="input-group-addon">kcal</span>*@
			</div>
@*            <span class="small">@String.Format("標準体重時に必要となるカロリーは{0:#,###}kcalです！", Me.Model.PageViewModel.StdEstimatedEnergyRequirement)</span>
            @QyHtmlHelper.ToValidationMessageArea({"model.CaloriesOut"}, Me.TempData("ErrorMessage"), "alert alert-danger thin mt10", True)
		</label>*@
			
		<div class="submit-area">
			<a href="javascript:void(0);" class="btn btn-close no-ico" id="prev">戻 る</a>
			<a href="javascript:void(0);" class="btn btn-submit" id="regist">目標登録！</a>
		</div>
        <section id="caution" class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
	</form>
</section>

<section class="progress-area">
	<h3>利用開始まであと<strong>1</strong>ステップ！</h3>
	<ul class="progressbar clearfix">
		<li class="active"><span>情報入力</span></li>
@*		<li class="active"><span>現状把握</span></li>*@
		<li class="here"><span>目標設定</span></li>
		<li><span>利用開始！</span></li>
	</ul>
</section>
