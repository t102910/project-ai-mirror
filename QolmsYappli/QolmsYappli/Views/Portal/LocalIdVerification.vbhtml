@Imports MGF.QOLMS.QolmsYappli
@*@ModelType PortalChallengeEntryInputModel*@

@Code
    ViewData("Title") = "市民確認エントリー"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"

    Dim dateNow As Date = Date.Now
End Code

<body id="challenge" class="lower ginowan">
    @*@Html.AntiForgeryToken()*@

<main id="confirm" class="clearfix" role="main">
	<section class="home-btn-wrap">
		<a href="../Portal/Home" class="home-btn type-2"><i class="la la-angle-left"></i><span> 戻る</span></a>
	</section>
	<section class="contents-area mb20">
		@*<div style="text-align: center; padding: 100px 20px; color:#777; background-color: #ddd; border-radius: 20px; font-size: 20px; margin-bottom: 32px;">
		    <img class="main-photo mb20" src="../dist/img/challenge/challenge-5/JOTO_3.jpg" alt="">

		</div>*@

        <dev class="box type-2">
		    <img class="main-photo mb20" src="../dist/img/challenge/challenge-5/JOTO_3.jpg" width="100%" style="padding-top: 10px;">
        </dev>

		<h2 class="main-title"><span>ぎのわんスマート健康増進プロジェクト​</span></h2>
		<p class="exp">
            アプリを使って健康づくりを始めてみませんか？？​
            宜野湾市では、アプリを使って市民の皆さまが健康的な生活を送ることができるようサポートをする取り組みを行っています。例えば、日々の食事や歩数、健診結果などをアプリで管理できたり、健康的な生活により獲得した宜野湾市民限定ポイント※1をお買い物で利用することができます。​
            今後は、オンラインによる健診予約、保健指導や、市民の皆さまそれぞれに合った健康づくりのアドバイスなど更にサービスを充実させてまいります。​
            ぜひこの機会に、アプリを使って健康づくりをスタートしましょう。​
            ※1ポイントの付与を受けるためには市民確認のためぴったりサービス申請が必要となります。​
		</p>
		<p class="submit-area">
			<a href="/portal/LocalIdVerificationAgreement" class="btn btn-default">
				さっそくはじめる
			</a>
		</p>
	</section>	
</main>

    @*<main id="main-cont" class="clearfix" role="main">
        <section class="home-btn-wrap">
            <a href="../Portal/challenge" class="home-btn type-2"><i class="la la-angle-left"></i><span> 戻る</span></a>
        </section>
        <section class="contents-area mb20">
            <div class="box type-2">
                <div class="wrap">
                    <img class="w-max" src="/dist/img/tmpl/no-image.png">
                    <p>

                        <a href="/portal/LocalIdVerificationAgreement" class="btn btn-default entry mt10 ">
                            エントリー

                        </a>
                    </p>
                </div>
            </div>
        </section>
    </main>
    <div class="modal fade" id="error-modal" tabindex="-1" data-backdrop="static" data-keyboard="false" data-row-no="-1">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close"><span>×</span></button>
                    <h4 class="modal-title">エラー</h4>
                </div>
                <div class="modal-body">
                    エラーメッセージが入ります
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-close no-ico mb0">閉じる</button>
                </div>
            </div>
        </div>
    </div>*@

    @Html.Action("PortalFooterPartialView", "Portal")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/portal/localidverification")

</body>
