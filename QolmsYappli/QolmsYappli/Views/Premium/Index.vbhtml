@Imports MGF.QOLMS.QolmsYappli
@ModelType PaymentInf

@Code
    ViewData("Title") = "プレミアム会員"
    Layout = "~/Views/Shared/_PremiumLayout.vbhtml"
    
    Dim isPremiumMember As Boolean = False
    If Model.MemberShipType = QyMemberShipTypeEnum.LimitedTime OrElse Model.MemberShipType = QyMemberShipTypeEnum.Premium Then
        isPremiumMember = True
    End If

    Dim isDemo As Boolean = False
    Dim settingStr As String = ConfigurationManager.AppSettings("DemoMode")
    If String.IsNullOrEmpty(settingStr) = False AndAlso settingStr.ToLower() = "true" Then
        isDemo = True
    End If
End Code
<body id="premium-info" class="lower">
    @Html.AntiForgeryToken()

    <main id="main-cont" class="clearfix" role="main">
	@If isPremiumMember Then
	    @<section class="contents-area">
		    <h2 class="title relative">プレミアム会員<img id="s-3" src="/dist/img/tmpl/s-3.png"></h2>
		    <hr>
		    <h3 class="title">プレミアム会員メニュー</h3>
		    <p class="balloon">
			    プレミアム会員のご登録ありがとうございます。お支払い履歴の確認や、お支払い方法の変更はこちらから！
		    </p>
@*           @If isDemo  Then
                 @Html.ActionLink("プレミアム会員を退会する(ダミー)", "DummyLeave", Nothing, New With {.class = "btn btn-submit mb10"})
           Else
                @*@
			<a href="@Url.Action("History")" class="btn btn-link back-FFF no-ico block"><i class="la la-file-text"></i> お支払い履歴を確認する</a>
            <a href="@Url.Action("MethodChange")" class="btn btn-link back-FFF no-ico block"><i class="la la-credit-card"></i> お支払い方法を切り替える</a>
            <a href="#" id="doc-premium-open" class="btn btn-link back-FFF no-ico block" ><i class="la la-clipboard"></i> プレミアム会員特典を確認する</a>
            <a href="#" id="doc-open" class="btn btn-link back-FFF no-ico block"><i class="la la-clipboard"></i> 会員利用規約を確認する</a>
                
            @*@Html.ActionLink("プレミアム会員を退会する", "Leave", Nothing, New With {.class = "btn btn-submit mb10"})*@
           @*End If*@
	    </section>
	Else
	 @<section class="contents-area">

        @If Me.Model.IsOldPaymentRecordExists Then
            @<p class="center mb10"><a href="@Url.Action("History")" class="btn btn-link back-FFF no-ico"><i class="la la-file-text"></i> お支払い履歴を確認する</a></p>
	    End If

		<h2 class="title relative">プレミアム会員になろう!<img id="s-3" src="/dist/img/tmpl/s-3.png"></h2>
		<hr>
                
		<h3 class="title">「プレミアム会員」とは？</h3>
		<p class="balloon">

			ライト会員（無料会員）にプラスして以下の特典がありますよ！
		</p>
	</section>
		@<p class="img-area center gray-bg-area">
			<img src="/dist/img/premium-info/1.png">
		</p>
		
		@<p class="img-area center">
			<img src="/dist/img/premium-info/2.png">
		</p>
		
		@<p class="img-area center gray-bg-area">
			<img src="/dist/img/premium-info/3.png">
		</p>

	 	@<p class="img-area center">
			<img src="/dist/img/premium-info/4.png">
		</p>
	    
        @<p class="img-area center gray-bg-area">
			<img src="/dist/img/premium-info/5.png">
		</p>
		
		@<p class="section default font-s">
			@*<strong class="font-l type-3">月額300円無料キャンペーンについて</strong><br>
			<strong>概要：</strong>お申し込み月と、その後の2か月間の月額利用料金を無料といたします。<br>
			<strong>対象：</strong>JOTOホームドクターアプリからプレミアム会員登録を申し込みいただいた方<br>
			<strong>適用方法：</strong>プレミアム会員登録するだけで、適用となります。*@
			    ※課金開始は翌月からとなります。<br>
		</p>
	 
	@<section class="contents-area">	
		<div class="submit-area">
			@*<a href="#" class="btn btn-close no-ico mb10">いまはやめとく</a>*@
            @Html.ActionLink("プレミアム会員登録する", "regist", Nothing, New With {.class = "btn btn-submit mb10"})
		</div>
	</section>
	    @<p class="center mb30"><a href="#" id="doc-open" class="btn btn-link back-FFF no-ico"><i class="la la-clipboard"></i> 会員利用規約を確認する</a></p>
	 
	End If

        @*プレミアム会員特典のドキュメント*@
        <div id="doc-premium-body" class="section default doc-default hide-elm">
 		    <section class="contents-area">
			    <h3 class="title">「プレミアム会員」とは？</h3>

		    </section>
		    <p class="img-area center gray-bg-area">
			    <img src="/dist/img/premium-info/1.png">
		    </p>
		
		    <p class="img-area center">
			    <img src="/dist/img/premium-info/2.png">
		    </p>
		
		    <p class="img-area center gray-bg-area">
			    <img src="/dist/img/premium-info/3.png">
		    </p>

	 	    <p class="img-area center">
			    <img src="/dist/img/premium-info/4.png">
		    </p>
	    
        	    <p class="img-area center gray-bg-area">
			    <img src="/dist/img/premium-info/5.png">
		    </p>
		
		    <p class="section default font-s">
@*			    <strong class="font-l type-3">月額300円無料キャンペーンについて</strong><br>
			    <strong>概要：</strong>お申し込み月と、その後の2か月間の月額利用料金を無料といたします。<br>
			    <strong>対象：</strong>JOTOホームドクターアプリからプレミアム会員登録を申し込みいただいた方<br>
			    <strong>適用方法：</strong>プレミアム会員登録するだけで、適用となります。
*@
			    ※課金開始は翌月からとなります。<br>

		    </p>

		    <p class="center"><a href="#" id="doc-premium-close" class="btn btn-close">閉じる</a></p>
	    </div>
        @*会員規約*@
		<div id="doc-body" class="section default doc-default hide-elm">
			<strong class="mb15 block">ＪＯＴＯホームドクタープレミアム会員利用規約</strong>
			<dl>
				<dt>第1条（定義）</dt>
				<dd>
					<ol>
						<li>
							1.「ＪＯＴＯホームドクタープレミアム会員利用規約」において使用する用語の定義は、次のとおりとします。 なお、本条に定めるほかの用語の定義は、「ＪＯＴＯホームドクターアプリ利用規約」の第2条において定める定義と同義とします。
						</li>
						<li>
							<ol>
								<li>(1)「ＪＯＴＯホームドクタープレミアム会員」(以下「本プレミアム会員」といいます)とは、当社が提供する有料サービスの総称です。</li>
								<li>(2)「プレミアムサービス利用者」とは、会員のうち、当社が別途定めた方法によりプレミアムサービス会員登録を行い利用する資格を持つ個人をいいます。</li>
								<li>(3)「ＪＯＴＯホームドクタープレミアムサービス利用規約」(以下、「本プレミアムサービス利用規約」といいます)とは、プレミアムサービス利用者が本プレミアムサービスを受けるための規約および条件をいいます。</li>
							</ol>
						</li>
					</ol>
				</dd>
				
				<dt>第2条(プレミアムサービス利用規約の位置づけ)</dt>
				<dd>
					<ol>
						<li>1.プレミアムサービス利用者には、本プレミアムサービス利用規約に加え、ＪＯＴＯホームドクター利用規約が適用されます。</li>
						<li>2.本プレミアムサービス利用規約とＪＯＴＯホームドクター利用規約の定めが異なる場合は、本プレミアムサービス利用規約が優先して適用されます。</li>
					</ol>
				</dd>
				
				<dt>第3条(利用料)</dt>
				<dd>
					<ol>
						<li>1.本プレミアムサービスの利用料は、330円（税込）/月となります。</li>
						<li>2.利用料は解約月についても1か月分お支払い頂きます。契約期間の途中で本プレミアムサービス利用者登録を解約された場合も1か月分をお支払い頂きます。
						<li>3.入会月、退会月共に、日割計算は行いません。</li>
						<li>4.無料キャンペーンに基づいて利用している場合、当社が別途定めた方法により利用料免除が認められている場合には、 本条の利用料はプレミアムサービス利用者に対して適用されません。</li>
						<li>5.本プレミアムサービスのご利用には、本プレミアムサービス利用料の他に別途通信料がかかります。</li>
						<li>6.当社は、プレミアムサービスの利用料金の支払いに関する領収書等は発行いたしません。</li>
					</ol>
				</dd>
				
				<dt>第4条(決済)</dt>
				<dd>
					<ol>
						<li>1.プレミアムサービス利用者は、本人の希望する決済手段により当社に対し利用料を支払うものとします。</li>
						<li>2.料金の収納方法等は、当社指定の決済手段で行うものとします。</li>
					</ol>
				</dd>
				
				<dt>第5条(料金の変更)</dt>
				<dd>
					<ol>
						<li>1.本プレミアムサービス利用料を変更する可能性があります。その場合、当サイト上に、料金変更の30日前までに本プレミアムサービス 利用料の変更する旨の告知をいたします。当サイト上にて本プレミアムサービス利用料の変更を告知してから30日が経過した時点で、 当該変更の効力が生じ、プレミアムサービス利用者が当該変更を承諾したものとします。</li>
					</ol>
				</dd>
				
				<dt>第6条(プレミアムサービス内容の変更)</dt>
				
				<dd>
					<ol>
						<li>1.当社は、事前に通知することなくいつでも本プレミアムサービスの内容を変更、停止または中止することができるものとします。</li>
						<li>2.当社は、前項に基づく内容の変更または一時中断、停止および中止によって利用者に不利益または損害が発生した場合においても、 その責任を一切負わないものとします。</li>
					</ol>
				</dd>
				
				<dt>第7条(当社からの解約)</dt>
				<dd>
					<ol>
						<li>1.ＪＯＴＯホームドクター利用規約第16条「利用資格の停止および失効」に定める措置の外、プレミアムサービス利用者が以下のいずれかに 該当するものと当社が判断した場合は、当社は当該プレミアムサービス利用者に事前に何ら通知または催告することなく、本プレミアムサービス利用契約の解約をすることができるものとします。</li>
						<li>
							<ol>
								<li>a.本プレミアムサービス利用規約に違反している場合</li>
								<li>b.利用料金その他の債務の履行を遅滞し、または支払を拒否した場合</li>
							</ol>
						</li>
						<li>2.プレミアムサービス利用者は、当社が本条第１項に定める措置を講じた場合に、当該措置に起因する結果に関し、当社を免責するものとします。</li>
					</ol>
				</dd>
				
				<dt>第8条(プレミアムサービス利用者からの解約)</dt>
				
				<dd>
					<ol>
						<li>1.プレミアムサービス利用者が本プレミアムサービス利用契約を解約する場合は、当社が定める手続きに従い、解約の届出を行うこととします。</li>
						<li>2.当社は、既に受領した利用料その他の払い戻し等は一切行いません。</li>
					</ol>
				</dd>
				
				<dt>第9条(権利の放棄)</dt>
				<dd>
					<ol>
						<li>1.当社が本プレミアムサービス利用規約に示される権利を行使または実施しない場合にも、当該権利を放棄するものではありません。</li>
					</ol>
				</dd>
				
				<dt>第10条 （合意管轄）</dt>
				<dd>
					<ol>
						<li>サービス利用者と当社との間で利用契約に関連して訴訟の必要が生じた場合は、沖縄地方裁判所を第一審の専属的合意管轄裁判所とします。</li>
					</ol>
				</dd>
			</dl>
			
			<p class="right pt20">以上</p>
			
			<p class="mb20 pt30">
				<strong>附則</strong><br>
				(施行期日）本規約は、令和元年5月28日から施行する。
			</p>
			<p class="center"><a href="#" id="doc-close" class="btn btn-close">閉じる</a></p>
		</div>


	@*<section class="premium-btn">
		<a href="" class="btn btn-default mb20">
			<span><img src="/dist/img/tmpl/premium.png"> 短い文言で説明はいります。</span>
			<em class="logona">健康年齢の測定はこちら</em>
		</a>
	</section>*@
</main>
    @Html.Partial("_PremiumFooterPartialView")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/Premium/index")
</body>

<script type="text/javascript">
    $(function () {
        $('#doc-open').click(function () {
            $('#doc-body').slideDown(function () {
                $("html,body").animate({ scrollTop: $('#doc-body').offset().top });
            });
            $(this).hide();
            return false;
        });
        $('#doc-close').click(function () {
            $('#doc-body').slideUp();
            $('#doc-open').show();
            return false;
        });

        $('#doc-premium-open').click(function () {
            $('#doc-premium-body').slideDown(function () {
                $("html,body").animate({ scrollTop: $('#doc-premium-body').offset().top });
            });
            $(this).hide();
            return false;
        });
        $('#doc-premium-close').click(function () {
            $('#doc-premium-body').slideUp();
            $('#doc-premium-open').show();
            return false;
        });
    });
</script>
