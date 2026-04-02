@Imports MGF.QOLMS.QolmsYappli
@ModelType StartSetupStepTwoPartialViewModel

<section class="contents-area" data-step-mode="2">
	<h2 class="title relative">
		現状の把握！
		<img id="s-2" src="../dist/img/tmpl/s-4.png">
	</h2>
	<hr>

	<form>
	    <div class="section default">
            @String.Format("現在の体重{0}kg、身長{1}cmの、あなたの1日の適正エネルギー量をチェックしましょう！", Me.Model.PageViewModel.Weight, Me.Model.PageViewModel.Height)
            <p class="center mt10">
			    <a href="javascript:void(0);" id="skip" class="btn btn-default">スキップして始める！</a>
			    <span class="small">（この設定はその他メニューの「目標設定」から設定できます。）</span>
		    </p>	    
        </div>
		<div class="center mb30 cal-info-wrap">
			<strong>現体重</strong>の基礎代謝量は…<br>
			<p class="cal-info logona">@Me.Model.PageViewModel.NowBasalMetabolism.ToString("#,###")<i>kcal</i></p>
@*			（<strong>標準体重時</strong>@String.Format("の基礎代謝量は{0:#,###}kcalです。", Me.Model.PageViewModel.StdBasalMetabolism)）*@
            あなたが標準体重の場合の<br/>
            基礎代謝量は<strong class="red">@String.Format("{0:#,###}kcal", Me.Model.PageViewModel.StdBasalMetabolism)</strong>です。
		</div>
		
        <div class="center mb30 cal-info-wrap">
			<strong>現体重</strong>の推定エネルギー量は…<br>
			<p class="cal-info logona">@Me.Model.PageViewModel.NowEstimatedEnergyRequirement.ToString("#,###")<i>kcal</i></p>
@*			（<strong>標準体重時</strong>@String.Format("の推定エネルギー量は{0:#,###}kcalです。", Me.Model.PageViewModel.StdEstimatedEnergyRequirement)）*@
            あなたが標準体重の場合の<br/>
            推定エネルギー量は<strong class="red">@String.Format("{0:#,###}kcal", Me.Model.PageViewModel.StdEstimatedEnergyRequirement)</strong>です。
		</div>
			
		<div class="submit-area">
			<a href="javascript:void(0);" class="btn btn-close no-ico">戻 る</a> 
			<a href="javascript:void(0);" class="btn btn-submit">次 へ</a>
		</div>
	</form>
</section>

<section class="progress-area">
	<h3>利用開始まであと<strong>2</strong>ステップ！</h3>
	<ul class="progressbar clearfix">
		<li class="active"><span>情報入力</span></li>
		<li class="here"><span>現状把握</span></li>
		<li><span>目標設定</span></li>
		<li><span>利用開始！</span></li>
	</ul>
</section>
