@Imports MGF.QOLMS.QolmsYappli
@Imports MGF.QOLMS.QolmsCryptV1
@ModelType PortalLocalIdVerificationDetailViewModel

@Code
    ViewData("Title") = "エントリー状況"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"

    'Dim dockDispKey As Guid = Guid.Parse("ded05070-8718-4313-924a-25233e35e218")

    'Dim first As String = String.Empty
    'If Me.TempData("first") = True Then
    '    first = "first"
    'End If


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
						市民確認申請の状況
					</h3>
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
					<div class="result-area">

							@Select Case Me.Model.Status
                                Case 1
									@<p class="result-text">市民確認未承認</p>

                                Case 2
									@<p class="result-text">市民確認承認済み</p>

                                Case 3
									@<p class="result-text">市民確認非承認</p>

                            End Select
					</div>

					<h4 Class="sub-title">
						メッセージ
					</h4>
					<div Class="section default">
						@code
                            Dim str As String = String.Empty
                            Select Case Me.Model.Status
                                Case 1
                                    str = "市民確認申請が未承認となっております。申請がお済みの方は、申請結果の通知をお待ちください。申請がお済みでない方は、「申請画面に戻る」より申請を行ってください。申請状況は、ぴったりサービスよりご確認いただけます。"
                                Case 2
                                    str = "市民確認申請が承認済みとなりました。"
                                Case 3
                                    str = "宜野湾市民であることの確認ができませんでした。本プロジェクト限定ポイントの付与は宜野湾市民限定となっております。ご不明な点がございましたら、プロジェクト事務局（098-880-2469）までお問い合わせください。​"

                            End Select
						End Code

						@str
						<br/>
						@Me.Model.Reason
					</div>

					@If Me.Model.Status <> 2 Then
						@<p Class="center">
							<a href = "../Portal/LocalIdVerificationRequest" Class="font-l type-3 bold btn btn-close type-2">
								申請画面に戻る
							</a>
						</p>

                    End If

				</div>
			</div>
			<p>
				<a href = "../Portal/LocalIdVerificationRegister" Class="btn btn-default">
					エントリー情報編集
				</a>
			</p>
		</section>	
	</main>

    @Html.Action("PortalFooterPartialView", "Portal")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/portal/localidverificationdetail")

</body>