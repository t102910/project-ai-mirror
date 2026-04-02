@Imports MGF.QOLMS.QolmsYappli
@ModelType PortalSearchdetailViewModel

@*@Code
    ViewData("Title") = "医療機関検索詳細"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"
End Code*@

@*<body id="search" class="lower detail">

    @Html.Action("PortalHeaderPartialView", "Portal")

<main id="main-cont" class="clearfix" role="main">	
	<section class="data-area">
		<div id="" class="mb20">
			<div class="article" href="/search-detail.html">
				<section class="inner">
					<div class="info">
						<p class="kana"><span>@Me.Model.KanaName</span></p>
						<h4 class="name">@Me.Model.InstitutionName</h4>
						<p class="address">@Me.Model.PostalCode @Me.Model.Address<span>（@Me.Model.RouteName@Me.Model.NeareStstation 駅 @Me.Model.Transportation @Me.Model.RequiredTime 分）</span></p>
						<p class="departments mb20">
							<span class="special">#AU後払い対応</span><span class="special">#夜間診療可能</span><span class="special">#日祝診療可能</span>
							<span>診療科名入ります</span><span>診療科名</span><span>診療科名入ります</span><span>診療科名入ります</span><span>診療科名入ります</span><span>診療科名</span><!-- 改行無しで吐き出してください -->
						</p>
						<h4>診察時間など</h4>						
						<table class="consultation-time table table-bordered">
							<thead>
								<tr>
									<td></td>
									<td>月</td>
									<td>火</td>
									<td>水</td>
									<td>木</td>
									<td>金</td>
									<td>土</td>
									<td>日</td>
								</tr>
							</thead>
							<tbody>
								<tr>
									<th>午前<span>9:00～12:00</span></th>
									<td><i class="la la-check-circle"></i></td>
									<td><i class="la la-check-circle"></i></td>
									<td><i class="la la-check-circle"></i></td>
									<td><i class="la la-check-circle"></i></td>
									<td><i class="la la-check-circle"></i></td>
									<td><i class="la la-check-circle"></i></td>
									<td>-</td>
								</tr>
								<tr>
									<th>午後<span>14:00～17:00</span></th>
									<td><i class="la la-check-circle"></i></td>
									<td><i class="la la-check-circle"></i></td>
									<td><i class="la la-check-circle"></i></td>
									<td>-</td>
									<td><i class="la la-check-circle"></i></td>
									<td>-</td>
									<td>-</td>
								</tr>
								<tr>
									<td class="spacer" colspan="8"></td>
								</tr>
								<tr>
									<th>受付メモ</th>
									<td class="memo" colspan="7">受付時間7:30～11:30 13:30～16:30 土曜15時まで　臨時休診あり</td>
								</tr>
								<tr>
									<th>休診</th>
									<td class="memo" colspan="7">祝日、年末年始</td>
								</tr>
								
								
								
							</tbody>
						</table>
						<iframe src="@Me.Model.Url" width="100%" height="300" frameborder="0" style="border:0" allowfullscreen></iframe>
					</div>
					<div class="submit-area">
						<a href="http://yahoo.co.jp" target="_blank" class="btn btn-link no-ico narrow"><i class="la la-external-link"></i> Webサイトを見る</a>
						<a href="tel:@Me.Model.Tel" class="btn btn-submit no-ico narrow"><i class="la la-fax"></i> 電話する</a>
					</div>
				</section>
			</div>
		</div>
	</section>
	
	<section class="premium-btn">
		<a href="" class="btn btn-default mb20">
			<span><img src="/dist/img/tmpl/premium.png"> 短い文言で説明はいります。</span>
			<em class="logona">健康年齢の測定はこちら</em>
		</a>
	</section>
</main>
    @Html.Action("PortalFooterPartialView", "Portal")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/portal/searchdetail")
</body>*@
		<div class="modal-content">
			<div class="modal-header">
				<button type="button" class="close" data-dismiss="modal"><span>×</span></button>
				<h4 class="modal-title">@Me.Model.InstitutionName の詳細情報</h4>
			</div>
			<div class="modal-body data-area">
				<div class="article">
					<section class="inner">
						<div class="info">
							@*<p class="kana"><span>いりょうきかんカナはいります</span></p>*@
							<h4 class="name">@Me.Model.InstitutionName</h4>
						    <p class="address">@Me.Model.PostalCode @Me.Model.Address<span>（@Me.Model.RouteName@Me.Model.NeareStstation 駅 @Me.Model.Transportation @Me.Model.RequiredTime 分）</span></p>
							<p class="departments mb20">
								<span class="special">#AU後払い対応</span><span class="special">#夜間診療可能</span><span class="special">#日祝診療可能</span>
								<span>診療科名入ります</span><span>診療科名</span><span>診療科名入ります</span><span>診療科名入ります</span><span>診療科名入ります</span><span>診療科名</span><!-- 改行無しで吐き出してください -->
							</p>
							<h4>診察時間など</h4>						
							<table class="consultation-time table table-bordered">
								<thead>
									<tr>
										<td></td>
										<td>月</td>
										<td>火</td>
										<td>水</td>
										<td>木</td>
										<td>金</td>
										<td>土</td>
										<td>日</td>
									</tr>
								</thead>
								<tbody>
									<tr>
										<th>午前<span>9:00～12:00</span></th>
										<td><i class="la la-check-circle"></i></td>
										<td><i class="la la-check-circle"></i></td>
										<td><i class="la la-check-circle"></i></td>
										<td><i class="la la-check-circle"></i></td>
										<td><i class="la la-check-circle"></i></td>
										<td><i class="la la-check-circle"></i></td>
										<td>-</td>
									</tr>
									<tr>
										<th>午後<span>14:00～17:00</span></th>
										<td><i class="la la-check-circle"></i></td>
										<td><i class="la la-check-circle"></i></td>
										<td><i class="la la-check-circle"></i></td>
										<td>-</td>
										<td><i class="la la-check-circle"></i></td>
										<td>-</td>
										<td>-</td>
									</tr>
									<tr>
										<td class="spacer" colspan="8"></td>
									</tr>
									<tr>
										<th>受付メモ</th>
										<td class="memo" colspan="7">@Me.Model.AcceptedTimeMemo</td>
									</tr>
									<tr>
										<th>休診</th>
										<td class="memo" colspan="7">祝日、年末年始</td>
									</tr>
									
								</tbody>
							</table>
							
							<section id="g-map" class="mb70">
								<div id="map" style="width:100%; height: 300px;"></div>
							</section>
							
						</div>
						<div class="submit-area">
							<a href="@Me.Model.Url" target="_blank" class="btn btn-link no-ico narrow"><i class="la la-external-link"></i> Webサイトを見る</a>
							<a href="tel:@Me.Model.Tel" class="btn btn-submit no-ico narrow"><i class="la la-fax"></i> 電話する</a>
						</div>
					</section>
				</div>
			</div>
			<div class="modal-footer">
				<button type="button" class="btn btn-close no-ico mb0" data-dismiss="modal">閉じる</button>
			</div>
         </div>