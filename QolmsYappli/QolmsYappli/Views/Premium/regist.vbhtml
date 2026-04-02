@Imports MGF.QOLMS.QolmsYappli
@ModelType premiumregistViewModel

@Code
    ViewData("Title") = "プレミアム会員登録"
    Layout = "~/Views/Shared/_PremiumLayout.vbhtml"
    
    Dim isDemo As Boolean = False
    Dim settingStr As String = ConfigurationManager.AppSettings("DemoMode")
    If String.IsNullOrEmpty(settingStr) = False AndAlso settingStr.ToLower() = "true" Then
        isDemo =True 
    End If
End Code

<body id="premium-regist" class="lower">
    <main id="main-cont" class="clearfix" role="main">
        <section class="contents-area">
		    <h2 class="title relative">プレミアム会員登録</h2>
		    <hr>
            <div id="momentum" class="t-row">
			<div class="section default">
                @String.Format("{0:yyyy年M月d日}",Model.PaymentStartDate) 以後、 毎月１日に課金が発生します。<br />
                月額・・・・330円(税込)<br />
                * 消費税率変更により請求額が変更になる可能性があります。<br />
				お支払い方法を選択してください。
			</div>
			<label>
				<span></span>
				<span>お支払い方法</span>
			</label>
			<label>
				<span><input type="radio" name="payment" checked="checked" value="1" ></span>
				<span>
					<img class="au-logo" src="/dist/img/premium-info/logo.png">
				</span>
			</label>
			<label>
				<span><input type="radio" name="payment" value="2"></span>
				<span>
					<i class="la la-credit-card"></i>クレジットカード
      @*              クレジットカードにて清算<br><br>
					キャンペーン終了後にクレジットカードを登録頂きます。<br>
                    クレジット登録を希望する方はこちらから登録してください。<br>
                    クレジット登録が可能な時期は追ってご連絡させて頂きます。<br>*@

				</span>
			</label>
@*			<label>
				<span><input type="radio" name="hoge" disabled=""></span>
				<span>
					他支払い方法
				</span>
			</label>
			<p class="alert alert-danger thin mt10 hide"></p>*@
		</div>
		<div class="submit-area no-line">
			@*<a href="" class="btn btn-submit mb10">プレミアム会員登録</a>*@
             @If isDemo Then
                @<form method="post" action="@Url.Action("DummyRegistResult")">
                    @Html.AntiForgeryToken()

                    <p>まだ、かんたん決済(本番)に接続しないようにしていますので、こちらを押して疑似的にプレミアム会員になることができます。</p>
                    <p>なお、かんたん決済(本番)に接続する際には、疑似的に作成したプレミアム会員登録情報はリセットしますのでご了承ください。</p>
                    <button  class="btn btn-submit mb10" type="submit">プレミアム会員登録(ダミー)</button>
                </form>
             Else
                 @<form method="post" action="@Url.Action("RegistResult")">
                     @Html.AntiForgeryToken()

                     <input id="paymentType" name="PaymentType" type="hidden" value="1" />
                     <button  class="btn btn-submit mb10" type="submit">プレミアム会員登録</button>
                 </form>
          
                End If
		</div>
		
		   
        </section> 
    </main> 
    @Html.Partial("_PremiumFooterPartialView")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/Premium/regist")
</body>

