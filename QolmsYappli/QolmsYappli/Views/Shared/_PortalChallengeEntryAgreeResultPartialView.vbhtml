@Imports MGF.QOLMS.QolmsYappli
@ModelType PortalChallengeEntryAgreeResultPartialViewModel

<main id="main-cont" class="clearfix" role="main">
	<section class="home-btn-wrap">
		<a href="../Portal/challenge" class="home-btn type-2"><i class="la la-angle-left"></i><span> 戻る</span></a>
	</section>
	<section class="contents-area mb20">
        <div class="box type-2">
            <div class="wrap">
                <div class="document-scroll-wrap">
                    <h3 class="title">
                        プロジェクト参加同意書
                    </h3>
                    <p class="mb10">
                        @Html.Raw(Me.Model.Terms)
                    </p>

                    <p class="bold orange mb20 small">
                        併せてアプリのプライバシーポリシーもご確認ください
                    </p>
                </div>
                <div class="bottom-fix-wrap mt10">
                    <p class="center mb10">
                        <a href="native:/tab/bio/0a078204" class="btn btn-default type-3">
                            JOTOホームドクター プライバシーポリシー
                        </a>
                    </p>
                    <p class="center mb30">
                        <label class="accept">
                            <input type="checkbox" id="policy">
                            プライバシーポリシーを確認し、同意しました。<span>必須</span>
                        </label>
                    </p>
                    <p>
                        <a href="" class="btn btn-default agree disabled">
                            参加する
                        </a>
                    </p>
                </div>
            </div>
        </div>
	</section>	
</main>