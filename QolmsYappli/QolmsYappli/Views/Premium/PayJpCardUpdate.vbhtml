@Imports MGF.QOLMS.QolmsYappli
@ModelType PremiumPayJPViewModel 

@Code
    ViewData("Title") = "クレジットカード決済に変更"
    Layout = "~/Views/Shared/_PremiumLayout.vbhtml"

    Dim isDemo As Boolean = False
    Dim settingStr As String = ConfigurationManager.AppSettings("DemoMode")
    If String.IsNullOrEmpty(settingStr) = False AndAlso settingStr.ToLower() = "true" Then
        isDemo = True
    End If
End Code

<body id="premium-regist" class="lower">
    <main id="main-cont" class="clearfix" role="main">
		@Using (Html.BeginForm())
		    @Html.AntiForgeryToken()

            @<section class="contents-area">
		        <h2 class="title relative">クレジットカード決済に変更</h2>
		        <hr>
		        <h3 class="title">ご利用内容</h3>
                <p class="section default">                
			        現在のご利用内容を変更します。<br>
                    カード情報を入力してクレジットカードを登録してください。
                </p>

                @If Not String.IsNullOrWhiteSpace(Me.Model.card.Last4) Then
                    @<h3 class="title">現在のご利用内容</h3>
		            @<div class="section borderd card-info">
			            <p>
                            @Select Case Me.Model.card.Brand.ToLower
                                Case "visa"
    				            @<i class="la la-cc-visa huge"></i>
                                Case "jcb"
    				            @<i class="la la-cc-jcb huge"></i>
                                Case "american express"
    				            @<i class="la la-cc-amex huge"></i>
                                Case "mastercard"
    				            @<i class="la la-cc-mastercard huge"></i>     
                                Case "diners club"
    				            @<i class="la la-cc-diners-club huge"></i>
                                Case "discover"
    				            @<i class="la la-cc-discover huge"></i>
                            End Select

				            <span class="huge">@Me.Model.card.Brand</span>
			            </p>
			            下4桁 <span class="huge">@Me.Model.card.Last4</span>　
			            有効期限 <span class="huge">@String.Format("{0}/{1}", Me.Model.card.ExpMonth, Me.Model.card.ExpYear)</span>
		            </div>
                End If

		        <p class="balloon">
                     初回課金日　@(Me.Model.StartDate.ToString("yyyy年M月d日"))<br>
                     お支払い金額　@(Me.Model.Amount)円<br>お支払方法　クレジットカード
		            @*お支払い金額　300円(税別)<br>お支払方法　　クレジットカード*@
                </p>

                <section id="caution" class="section caution mt10 mb10 hide"><h4>heading...</h4>アラートエリアです。不要時.hideで消してください。</section>

                <script type="text/javascript">
                    function onCreatedToken(){
                        $("#caution").addClass("hide");
                    }
                </script>
@*                <script type="text/javascript"
                    src = "https://checkout.pay.jp/"
                            Class="payjp-button"
                            data-key="pk_test_ff4614bd38b01b643b213554"
                            data-on-created="onCreated"
                            data-text="支払う"
                            data-submit-text="支払う"
                            data-partial="true">
                </script>*@
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
                @*<script src="https://checkout.pay.jp/" class="payjp-button" data-on-created="onCreatedToken" data-key="@Me.Model.Key" data-partial="true"></script>*@
                <div class="submit-area">
                    <a href="../Premium/MethodChange" class="btn btn-close no-ico mb10">戻 る</a>
                    <a href="javascript:void(0);" class="btn btn-submit mb10" id="Register">カードを登録</a>
                </div>
	        </section>
		End Using

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

    @QyHtmlHelper.RenderScriptTag("~/dist/js/Premium/payjpcardupdate")
</body>

@*<script type="text/javascript">
</script>*@