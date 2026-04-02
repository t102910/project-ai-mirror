@Imports MGF.QOLMS.QolmsYappli
@ModelType PortalAuWalletPointExchangeViewModel

@Code
    ViewData("Title") = "Pontaポイント"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"
    
    Dim isYappli As Boolean = Me.Context.Request.UserAgent.ToLower().Contains("yappli")
    Dim auWalletLink As String = "https%3A%2F%2Fwallet.auone.jp%2Fcontents%2Fsp%2Fguide%2Fpoint.html"
End Code
<body id="data-charge" class="lower">
    @Html.AntiForgeryToken()

    <main id="main-cont" class="clearfix" role="main">
	    <section class="contents-area">
            <div id="contents-wrap">
                <h2 class="title relative">
			        Pontaポイント交換
		        </h2>
		        <hr>
                <h4 class="center">JOTOポイントをPontaポイントと交換！</h4>

		        <p class="section default">
                        現在保有いただいているJOTOポイントをPontaポイントと交換することができます。
                    <br />
                        ポイントは<b style="color: #ff0000;">１pt ＝１円</b>として交換できます。交換は５００ポイント、１０００ポイント単位で交換できます。
                    <br />
                    <b style="color: #ff0000;">※JOTOホームドクターで登録したauIDと連携しているPontaポイントへ加算されます。</b>

		        </p>

                <a href="native:/action/open_browser?url=@auWalletLink" class="btn btn-submit block">Pontaポイントについて詳しく</a>
			    <a href="native:/tab/bio/07cd8974" class="btn btn-submit block">JOTOポイントプログラム利用規約</a>

                <div class="remaining-point">
				    <h3>現在の保有ポイント
					    <span>@Me.Model.Point<i>pt</i></span>
				    </h3>
			    </div>

		        @code
                    Dim page As String = String.Empty
                
                    If Me.Model.FromPageNoType = QyPageNoTypeEnum.PortalHome Then

                        page = String.Format("?fromPageNo={0}", Convert.ToByte(Me.Model.FromPageNoType))
                    End If
                End Code
				<div class="right">
                     <a id="back" href="@String.Format("../Portal/history{0}",page )" class="btn btn-close no-ico" data-back="@page">戻 る</a>
				</div>

				<section id="caution" class="section caution mt10 mb10 hide"><h4>heading...</h4>アラートエリアです。不要時.hideで消してください。</section>

		        <form class="form charge-form mb30">
                    @For Each item In Me.Model.AuWalletPointItemN
                        If item.AuWalletPointItemId > 2 Then

                            If item.Point <= Me.Model.Point Then
			                @<a href="javascript:void(0);" class="btn btn-blue exchange" data-toggle="modal" data-itemid ="@item.AuWalletPointItemId"><i>@item.Point</i>P → <i>@item.DispName</i>と交換</a>
                            Else
			                @<a href="javascript:void(0);" class="btn btn-blue exchange disabled" data-toggle="modal" data-itemid ="@item.AuWalletPointItemId"><i>@item.Point</i>P → <i>@item.DispName</i>と交換</a>
                            End If
                        End If

                    Next
		        </form>
		        <p class="section default">
                    <b style="color: #ff0000;">※JOTOホームドクターで登録したauIDと連携しているPontaポイントへ加算されます。</b>
                </p>
            </div>
		    
            <section class="data-area mb20">
		  		<h3 class="title mt10">
				    <span>Pontaポイント交換履歴</span>
			    </h3>

                @If Me.Model.AuWalletPointHistN.Count > 0 Then
                    @For Each item In Me.Model.AuWalletPointHistN
                      @<article>
				            <section class="inner">
					            <div class="info">
						            <p class="date">@item.ActionDate.ToString("MM月dd日 tt hh:mm") </p>
						            <div class="flex-wrap">
							            <div class="before">
								            <strong>JOTOポイント</strong>
								            <p>
									            @item.Point<i>pt</i>
								            </p>
							            </div>
							            <p class="arrow">
								            <i class="la la-arrow-right"></i>
							            </p>
							            <div class="after">
								            <strong>Pontaポイント</strong>
								            <p>
									            @item.Point<i>ポイント</i>
								            </p>
							            </div>
						            </div>
					            </div>
				            </section>
			            </article>
                    Next
                Else
		            @<p>履歴がありません</p>
                                   
                End If
		     </section>
	    </section>


        <!-- 確認モーダル -->
        <div class="modal fade" id="confirmation-modal" tabindex="-1" data-backdrop="static" data-keyboard="false">
	        <div class="modal-dialog">
		        <div class="modal-content">
			        <div class="modal-header">
				        <button type="button" class="close" data-dismiss="modal"><span>×</span></button>
				        <h4 class="modal-title">確認</h4>
			        </div>
			        <div class="modal-body">
					        100MBと交換しますか？
			        </div>
			        <div class="modal-footer">
				        <button type="button" class="btn btn-close mb0" data-dismiss="modal">閉じる</button>
				        <a href="#" class="btn btn-submit" data-itemid ="100">交換する</a>
			        </div>
		        </div>
	        </div>
        </div>
        <!-- 「登録完了」ダイアログ -->
        <div class="modal fade" id="finish-modal" tabindex="-1" data-backdrop="static" data-keyboard="false">
	        <div class="modal-dialog">
		        <div class="modal-content">
			        <div class="modal-header">
				        <button type="button" class="close"><span>×</span></button>
				        <h4 class="modal-title">確認</h4>
			        </div>
			        <div class="modal-body">
				        登録しました。
			        </div>
			        <div class="modal-footer">
				        <button type="button" class="btn btn-close no-ico mb0">閉じる</button>
			        </div>
		        </div>
	        </div>
        </div>
    </main>

    @Html.Action("PortalFooterPartialView", "Portal")
    @QyHtmlHelper.RenderScriptTag("~/dist/js/portal/aupoint")
</body>

