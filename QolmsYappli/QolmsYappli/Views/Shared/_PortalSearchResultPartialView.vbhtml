@Imports MGF.QOLMS.QolmsYappli
@ModelType PortalSearchViewModel

@Code
    Dim hasData As Boolean = Me.Model.MedicalInstitutionN.Any()
End Code
		<div class="reload-area"><!-- AJAX-非同期更新エリア 医療機関情報のload中は.loadingを付与してください-->

@If hasData Then

    			@<h3 class="title mt10">医療機関一覧</h3>
    For Each item As MedicalInstitutionItem In Me.Model.MedicalInstitutionN
        @<a class="article" href="javascript:void(0);" data-codeno="@item.CodeNo">
				<section class="inner">
					<div class="info">
						<p class="kana"><span>@item.KanaName</span></p>
						<h4 class="name">@item.InstitutionName</h4>
						<p class="address">〒@item.PostalCode.ToString().Insert(3, "-") @item.Address</p>
                        <p class="departments">
                            @If item.OptionFlags And QyMedicalSearchTypeEnum.AuLaterPayment Then
                                @<span class="special">#医療費あと払い対応</span>
                            End If
                            @If item.OptionFlags And QyMedicalSearchTypeEnum.NightTimeService Then
                                @<span class="special">#夜間診療可能</span>
                            End If
                            @If item.OptionFlags And QyMedicalSearchTypeEnum.HolidayService Then
                                @<span class="special">#日祝診療可能</span>
                            End If
                            @If item.OptionFlags And QyMedicalSearchTypeEnum.PharumoAutoRegistration Then
                                @<span class="special">#薬局連携</span>
                            End If      
                            @For i As Integer = 0 To item.DepartmentN.Count - 1
                                    @<span>@item.DepartmentN.Item(i).ToString</span>
                                Next
                            </p>
						@*<p class="departments">
							<span class="special">#AU後払い対応</span><span class="special">#夜間診療可能</span><span class="special">#日祝診療可能</span>
							<span>診療科名入ります</span><span>診療科名</span><span>診療科名入ります</span><span>診療科名入ります</span><span>診療科名入ります</span><span>診療科名</span><span>診療科名</span><span>診療科名</span><!-- 改行無しで吐き出してください -->
						</p>*@
					</div>
				</section>
			</a>
    
Next
 @QyHtmlHelper.Pager(Me.Model.PageIndex, Me.Model.PageCount)  
Else
   @<h3 class="title mt10 hide">医療機関一覧</h3>
   @<span> 検索結果はありません</span>
End If

            </div>