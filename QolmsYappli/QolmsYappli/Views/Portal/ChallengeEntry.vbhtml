@Imports MGF.QOLMS.QolmsYappli
@ModelType PortalChallengeEntryInputModel

@Code
    ViewData("Title") = "チャレンジ"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"

    Dim dateNow As Date = Date.Now
End Code

<body id="challenge" class="lower ver-2">
    @Html.AntiForgeryToken()

    <main id="main-cont" class="clearfix" role="main">
	    <section class="home-btn-wrap">
		    <a href="../Portal/challenge" class="home-btn type-2"><i class="la la-angle-left"></i><span> 戻る</span></a>
	    </section>
	    <section class="contents-area mb20">
		    <div class="box type-2">
			    <div class="wrap">
                    <img class="w-max" src="@(Me.Model.ChallengeItem.ImageSrc + "JOTO_4.png")">
                	    <p>

                            <a href="#" class="btn btn-default entry mt10 @IIf(Me.Model.ChallengeItem.EntryStartDate.Date < dateNow.Date AndAlso dateNow.Date <= Me.Model.ChallengeItem.EntryEndDate.Date, String.Empty, "disabled")">
                                @If dateNow <= Me.Model.ChallengeItem.EntryStartDate Then

                                    @("開始までお待ちください")

                                ElseIf Me.Model.ChallengeItem.EntryEndDate.Date < dateNow.Date Then

                                    @("終了しました")

                                Else

                                    @("エントリー")

                                End If

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
    </div>

    @Html.Action("PortalFooterPartialView", "Portal")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/portal/challengeentry")

</body>
