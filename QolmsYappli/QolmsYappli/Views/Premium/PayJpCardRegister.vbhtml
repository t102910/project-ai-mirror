@Imports MGF.QOLMS.QolmsYappli
@ModelType PremiumPayJPViewModel 

@Code
    ViewData("Title") = "クレジットカード決済"
    Layout = "~/Views/Shared/_PremiumLayout.vbhtml"
End Code

<body id="premium-payjp" class="lower">
    <main id="main-cont" class="clearfix" role="main">
        <section class="contents-area">
            @Using (Html.BeginForm())
                @Html.AntiForgeryToken()
                    
                @<section class="contents-area">
		            <h2 class="title relative">@*クレジットカード精算*@クレジットカード決済</h2>
		            <hr>
		            <h3 class="title">ご利用内容</h3>
                    <p class="section default">
			            カード情報を入力してクレジットカードを登録してください。
                @*        無料期間中にクレジットカードの登録をしていただき、サービスを継続する場合はこちらを選択してください。<br>
                        登録が可能になる時期は追って通知致します。*@
                    </p>
		            <p class="balloon">
		                初回課金日　@(Me.Model.StartDate.ToString("yyyy年M月d日"))<br>
                        お支払い金額　@(Me.Model.Amount)円<br>お支払方法　クレジットカード
		                @*お支払い金額　300円(税別)<br>お支払方法　　クレジットカード*@
                    </p>

                    <section id="caution" class="section caution mt10 mb10 hide"><h4>heading...</h4>アラートエリアです。不要時.hideで消してください。</section>

@*                        <div class="submit-area">
                        <a href="javascript:void(0);" class="btn btn-submit" id="Reserve">クレジットカード精算を予約</a>
                    </div>*@

                    <script type="text/javascript">
                        function onCreatedToken() {
                            $("#caution").addClass("hide");
                        }
                    </script>
@*                        @<script type="text/javascript"
                        src = "https://checkout.pay.jp/"
                                Class="payjp-button"
                                data-key=""
                                data-on-created="onCreated"
                                data-text="支払う"
                                data-submit-text="支払う"
                                data-partial="true">
                    </script>*@
                    @*' pay.JP*@
                    <script src="https://checkout.pay.jp/"
                            class="payjp-button" 
                            data-on-created="onCreatedToken" 
                            data-payjp-key="@Me.Model.Key" 
                            data-payjp-partial="true" 
                            data-payjp-extra-attribute-email
                            data-payjp-extra-attribute-phone
                            data-payjp-extra-attribute-phone-code
                            data-payjp-three-d-secure="true" 
                            data-payjp-three-d-secure-workflow="redirect"
                            ></script>
                    <div class="submit-area">
                        <a href="javascript:void(0);" class="btn btn-submit" id="Register">カードを登録</a>
                    </div>
	            </section>

            End Using
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
    </main>
    @Html.Partial("_PremiumFooterPartialView")
        
    @*最低限しか追加していないのでスクリプト追加の場合は検討*@
    @QyHtmlHelper.RenderScriptTag("~/dist/js/premium/payjpcardregister")

@*    <script>
	</script>*@ 
</body>
