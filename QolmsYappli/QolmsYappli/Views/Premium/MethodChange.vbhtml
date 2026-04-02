@Imports MGF.QOLMS.QolmsYappli
@ModelType PaymentInf 

@Code
    ViewData("Title") = "支払い方法変更"
    Layout = "~/Views/Shared/_PremiumLayout.vbhtml"
End Code

<body id="premium-regist" class="lower">
    <main id="main-cont" class="clearfix" role="main">
	    <section class="contents-area">
		    <h2 class="title relative">
			    支払い方法変更
		    </h2>
		    <hr>
		
		    <div id="momentum" class="t-row">
			    <div class="section default">
				    変更したいお支払い方法を選択してください。<br>
				    現在のお支払い方法は<strong>@IIf(Me.Model.PaymentType = 1,"auかんたん決済","クレジットカード決済")</strong>です。
			    </div>
			    <label>
				    <span></span>
				    <span>お支払い方法</span>
			    </label>
			    <label>
				        <span>
                            @If Me.Model.PaymentType = 1 Then
                                @<input type="radio" name="payment" checked="checked" value="1">
                            Else
                                @<input type="radio" name="payment" value="1">
                            End If
                        </span>
				    <span>
					    <img class="au-logo" src="/dist/img/premium-info/logo.png">
				    </span>
			    </label>
			    <label>
				    <span>
                         @If Me.Model.PaymentType = 2 Then
                                @<input type="radio" name="payment" checked="checked" value="2">
                         Else
                                @<input type="radio" name="payment" value="2">
                         End If
				    </span>
				    <span>
					    <i class="la la-credit-card"></i>
					    クレジットカード<br>
					    <small>（カードを変更する場合もこちら） </small>
				    </span>
			    </label>
			    <p class="alert alert-danger thin mt10 hide"></p>
		    </div>
		    <div class="submit-area no-line">
			    <form method="post" action="@Url.Action("MethodChangeResult")">
                    @Html.AntiForgeryToken()

                    <input id="paymentType" name="paymentType" type="hidden" value="@IIf(Me.Model.PaymentType = 1,"1","2")" />
                     <a href="../Premium/Index" class="btn btn-close no-ico">戻 る</a>
                    <button  class="btn btn-submit" type="submit">次へ</button>
                 </form>
		    </div>
	    </section>
    </main>
 
    @Html.Partial("_PremiumFooterPartialView")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/premium/methodchange")
</body>
    
@*<script type="text/javascript">
</script>*@