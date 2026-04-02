@Imports MGF.QOLMS.QolmsYappli
@ModelType PortalUnsubscribeInputModel

@Code
    ViewData("Title") = "退会"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"
    
    Dim isYappli As Boolean = Me.Context.Request.UserAgent.ToLower().Contains("yappli")
    
End Code

<body id="" class="lower">
    @Html.AntiForgeryToken()
    @*@Html.Action("PortalHeaderPartialView", "Portal")*@
    <main id="main-cont" class="clearfix" role="main">
	    <section class="contents-area">
                <h2 class="title relative">退会</h2>
		        <hr>

            @If Me.Model.PremiumMemberShipType = QyMemberShipTypeEnum.LimitedTime OrElse Me.Model.PremiumMemberShipType = QyMemberShipTypeEnum.Premium Then
                @<h3 class="title mt10" id ="hist">
				    <span>プレミアム会員の退会</span>
			    </h3>
                @<p class="section default">
                    @Html.Raw(Me.Model.PremiumDescription) 
                </p>
			    @<section id="premiumCaution" class="section caution mt10 mb0 hide"><h4>heading...</h4>アラートエリアです。不要時.hideで消してください。</section>
                @<div class="submit-area">
                    @If Not isYappli Then
                        @<a href="native:/tab/custom/c9511513" class="btn btn-close no-ico">戻 る</a>
                    End If
                    <a href="javascript:void(0);" class="btn btn-submit" id="premium">プレミアム会員を辞める</a>
			    </div>
                
		        @<br/>

                @<h3 class="title mt10" id ="hist">
			        <span>退会</span>
			    </h3>
            End If

            <p class="section default">
                @Html.Raw(Me.Model.Description)
            </p>
			<label class="mb5"><input type="checkbox" id="consent"> 上記に同意し、退会処理を続けます</label>　

            <select id="reasonCode" name="model.ReasonCode"class="form-control mb20" required="required">
                <option value="">▼退会理由を選択してください</option>
                @For Each item As KeyValuePair(Of Integer, String) In Me.Model.ReasonList
                    @<option value="@item.Key">@item.Value</option>
                Next
            </select>
            
            <h4 class="">退会理由コメント</h4>
            <input id="reasonComment" name="model.ReasonComment"class="form-control" type="text">
			
            <section id="caution" class="section caution mt10 mb0 hide"><h4>heading...</h4>アラートエリアです。不要時.hideで消してください。</section>
            <div class="submit-area">
                @If Not isYappli Then
                    @<a href="native:/tab/custom/c9511513" class="btn btn-close no-ico">戻 る</a>
                End If
                    <a href="javascript:void(0);" class="btn btn-submit"id="all">退会する</a>
			</div>
				
	    </section>

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

         <!-- 「確認」ダイアログ -->
        <div class="modal fade" id="premium-cation-modal" tabindex="-1" data-backdrop="static" data-keyboard="false">
	        <div class="modal-dialog">
		        <div class="modal-content">
			        <div class="modal-header">
				        <button type="button" class="close"><span>×</span></button>
				        <h4 class="modal-title">確認</h4>
			        </div>
			        <div class="modal-body">
                        プレミアム会員の退会をすると、プレミアム会員限定のサービスはご利用いただけません。<br />
                        よろしいですか？
			        </div>
			        <div class="modal-footer">
				        <button type="button" class="btn btn-close no-ico mb0">いいえ</button>
				        <button type="button" class="btn btn-submit no-ico mb0">は い</button>
			        </div>
		        </div>
	        </div>
	     </div>

        <!-- 「確認」ダイアログ -->
        <div class="modal fade" id="delete-cation-modal" tabindex="-1" data-backdrop="static" data-keyboard="false">
	        <div class="modal-dialog">
		        <div class="modal-content">
			        <div class="modal-header">
				        <button type="button" class="close"><span>×</span></button>
				        <h4 class="modal-title">確認</h4>
			        </div>
			        <div class="modal-body">
                        アカウントの退会を行うとJOTOホームドクターのすべてのサービスがご利用いただけなくなります。
                        @If Me.Model.PremiumMemberShipType = QyMemberShipTypeEnum.Business OrElse Me.Model.PremiumMemberShipType = QyMemberShipTypeEnum.BusinessFree Then
                            @<br/>
                            @<p class="red">※企業連携は先に企業連携画面から解除してください。</p>
                        End If
			        </div>
			        <div class="modal-footer">
				        <button type="button" class="btn btn-close no-ico mb0">いいえ</button>
                        <button type="button" class="btn btn-submit no-ico mb0">は い</button>

			        </div>
		        </div>
	        </div>
	     </div>

    </main>
    @Html.Action("PortalFooterPartialView", "Portal")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/portal/unsubscribe")

</body>
<script>
</script>