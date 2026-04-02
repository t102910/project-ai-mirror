@Imports MGF.QOLMS.QolmsApiCoreV1
@Imports MGF.QOLMS.QolmsYappli
@ModelType HealthAgeTransitionAreaPartialViewModel

<h3 class="title mt10">
	<span>健康年齢の推移</span>
</h3>

@If Me.Model.HasData Then
    @<article class="age-area mb40">
		<section class="inner">
			<div class="flex-box">
				<div class="item">
					<h5>前々回</h5>
					<span class="date">@Me.Model.AgeData(0).Item1</span>
					<section>
					    @Me.Model.AgeData(0).Item2<i>歳</i>	
					</section>
				</div>
				<div class="item">
					<h5>前回</h5>
					<span class="date">@Me.Model.AgeData(1).Item1</span>
					<section>
					    @Me.Model.AgeData(1).Item2<i>歳</i>	
					</section>
				</div>
				<div class="item">
					<h5>今回</h5>
					<span class="date">@Me.Model.AgeData(2).Item1</span>
					<section>
					    @Me.Model.AgeData(2).Item2<i>歳</i>	
					</section>
				</div>
			</div>
			<small>
				※健康年齢は、あなたの健診値に基づいて算出された健康状態を表す指標です。<br>
				健康年齢が高いほど生活習慣病リスクや将来負担する医療費が高くなる可能性があることを意味します。
			</small>
		</section>
	</article>
Else
    @<article class="age-area mb40">
        <section class="section caution mt10 mb0">
            未測定です
        </section>
    </article>
End If
