@Imports MGF.QOLMS.QolmsYappli
@ModelType PremiumHistoryViewModel

@Code
    ViewData("Title") = "お支払い履歴"
    Layout = "~/Views/Shared/_PremiumLayout.vbhtml"
End Code

<body id="premium-info" class="lower">
    @Html.AntiForgeryToken()

    <main id="main-cont" class="clearfix" role="main">
        <section class="contents-area">
            <h2 class="title relative">お支払い履歴</h2>
            <hr>
            <h3 class="title">お支払い履歴</h3>
        </section>

	    <section class="contents-area">
            @If Me.Model.PaymentLogN.Any() Then
                @Code 
                    Dim firstItem As PaymentLogItem = Me.Model.PaymentLogN.First()
                End Code
                
                @If Not Me.Model.IsPaid(firstItem) Then
                    @<section id="caution" class="section caution mt10 mb10">
                        @Html.Raw(Me.Model.ToDowngradeMessage(firstItem))
                    </section>	
                End If
                
                @<table class="table table-bordered table-striped payment">
			        <thead>
				        <tr>
					        <td>課金日</td>
					        <td>お支払い方法</td>
					        <td>金額</td>
					        <td></td>
				        </tr>
			        </thead>
                    <tbody>
                        @For Each item As PaymentLogItem In Me.Model.PaymentLogN
				            @<tr>
					            <td>@Html.Raw(Me.Model.ToDateString(item))</td>
					            <td>@Me.Model.ToMethodString(item)</td>
                                @If Me.Model.IsPaid(item) Then
                                    @<td>@IIf(item.Amount > Decimal.Zero, String.Format("{0:N0}円", item.Amount), String.Empty)</td>
                                    @<td><i class="la la-check-circle"></i></td>
                                Else
                                    @<td></td>
                                    @<td><i class="la la-times-circle"></i></td>
                                End If
				            </tr>
                        Next
                    </tbody> 
                </table>
            Else
                @<section class="section default mt10 mb10">
                    過去1年間に、お支払いはありません。
                </section>
            End If

			<div class="submit-area">
                <a href="../Premium/Index" class="btn btn-close no-ico">戻 る</a>
			</div>
	    </section>
    </main>
 
    @Html.Partial("_PremiumFooterPartialView")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/premium/history")
</body>
