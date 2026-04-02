@Imports MGF.QOLMS.QolmsYappli
@Imports MGF.QOLMS.QolmsCryptV1
@ModelType PortalLocalIdVerificationRequestViewModel

@Code
    ViewData("Title") = "参加エントリー申請"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"
End Code


<body id="confirm" class="lower ginowan">
    @Html.AntiForgeryToken()

	<main id="main-cont" class="clearfix" role="main">
		<section class="home-btn-wrap">
			<a href="../Portal/Home" class="home-btn type-2"><i class="la la-angle-left"></i><span> 戻る</span></a>
		</section>
		<section class="contents-area mb20">
			<div class="box type-2">
				<div class="wrap">
											
					<h3 class="title">
						プロジェクトエントリー完了
					</h3>
					<div class="document-scroll-wrap">

						<div class="mb10">

							プロジェクトへのエントリーが完了しました。​<br/>
							アプリを通じてぎのわんスマート健康増進プロジェクトに関するサービスが受けられます。​<br/>
							プロジェクト限定ポイントの付与を受けるには宜野湾市民であることが必須になります。​<br/>
							ポイントの付与をご希望される方は下記の説明をご覧のうえ、​<br/>
							市民確認のために宜野湾市へ「ぴったりサービス」を使って申請をお願いいたします。​<br/>
							※お手続きにはマイナンバーカードが必要となります。​<br/>​
							<br/>

							宜野湾市民限定ポイント の付与を受けるには宜野湾市民であることが必須になります。<br/>
							市民確認のために宜野湾市へ「ぴったりサービス」を使って申請をお願いいたします。<br/>
							※お手続きにはマイナンバーカードが必要となります。<br/>
							■ぴったりサービスとは<br/>
												
							<a href="@String.Format("native:/action/open_browser?url={0}", "https://app.oss.myna.go.jp/Application/resources/about/index.html")">ぴったりサービスについて</a>

							@*<a href="@String.Format("native:/action/open_browser?url={0}", "https://app.oss.myna.go.jp/Application/resources/about/index.html")">https://app.oss.myna.go.jp/Application/resources/about/index.html</a>*@
							<br/>

							【ぴったりサービス申請の流れ】​<br/>
							①	下にある「JOTO連携ID」の横のコピーボタンを押してコピーする。​<br/>
							②	「ぴったりサービスへ進む」でぴったりサービスのサイトへ移る。​<br/>
							③	必要事項を入力する（マイナンバーカードを使って自動入力が可能です）。​<br/>
							④	「JOTO連携ID」の欄にコピーしていた「JOTO連携ID」を張り付ける。​<br/>
							⑤	マイナンバーカードを使って申請する（スマートフォン等のリーダーでマイナンバーカードを読み取り、暗証番号の入力が必要となります）。​<br/>

							【市民確認完了後のお知らせ】​<br/>
							・宜野湾市にて申請内容から市民確認を行います。​<br/>
							・市民確認後、Push通知にて結果のお知らせが届きますので内容をご確認ください。また、「市民確認申請の状況」画面でもエントリー結果の確認が可能です。​<br/>

							【注意点】<br/>
							・申請者が市民でない場合は「市民確認非承認」となります。<br/>
							・申請に不備があった場合は、ご案内までにお時間をいただく場合がございます。お手数ですが再度申請いただくか下記までお問い合わせください<br/>
							＜お問い合わせ窓口＞<br/>
							プロジェクト事務局： 098-880-2469 <br/>

						</div>
					</div>

					<div class="bottom-fix-wrap mt10">
						<ul class="progressbar">
							<li class="@IIf(Me.Model.Status >= 1, "active", String.Empty).ToString()">
								<span class="step">Step1</span>
								<span>市民確認未承認<span>
							</li>
							<li class="@IIf(Me.Model.Status = 2, "active", String.Empty).ToString()">
								<span class="step">Step2</span>
								<span>市民確認承認済み<span>
							</li>
							<li class="@IIf(Me.Model.Status = 2, "active", String.Empty).ToString()">
								<span>完了！</span>
							</li>
						</ul>
						<div class="copy-able-area">
							<h4 class="sub-title">
								JOTO連携ID
							</h4>
							<p class="copy-tapable" data-copy="@Me.Model.LinkageSystemId">@Me.Model.LinkageSystemId</p><!-- ここ改行しないで下さい -->
						
						</div>
						<p class="center">
							<a href="@String.Format("native:/action/open_browser?url={0}", Me.Model.Url)" class="font-l type-3 bold btn btn-link">
								ぴったりサービスで申請する
							</a>
						</p>
					</div>
				</div>
			</div>
						
			<p>
				<a href = "../Portal/LocalIdVerificationRegister" Class="btn btn-default">
					エントリー情報編集
				</a>
			</p>
		</section>
		        
		<!-- 「コピー完了」ダイアログ -->
        <div class="modal fade" id="copy-modal" tabindex="-1" data-backdrop="static" data-keyboard="false" data-row-no="-1">
            <div class="modal-dialog">
	            <div class="modal-content">
		            <div class="modal-body">
			            コピーしました。
		            </div>
	            </div>
            </div>
        </div>
	</main>

    @Html.Action("PortalFooterPartialView", "Portal")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/portal/localidverificationrequest")

</body>