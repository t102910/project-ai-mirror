@Imports MGF.QOLMS.QolmsYappli
@ModelType PortalSearchdetailViewModel

	<div class="modal-content">
        @If Me.Model.CodeNo > 0 Then
			@<div class="modal-header">
				<button type="button" class="close" data-dismiss="modal"><span>×</span></button>
				<h4 class="modal-title">@Me.Model.InstitutionName の詳細情報</h4>
			</div>
			@<div class="modal-body data-area" style="background-color:#ffffff;padding:15px;">
				<div class="article">
					<section class="inner">
						<div class="info">
							<p class="kana"><span>@Me.Model.InstitutionKana</span></p>
							<h4 class="name">@Me.Model.InstitutionName</h4>
						    <p class="address">〒@Me.Model.PostalCode.ToString().Insert(3,"-") @Me.Model.Address
                                    @If String.IsNullOrWhiteSpace(Me.Model.RouteName) Then
                                        If String.IsNullOrWhiteSpace(Me.Model.NeareStstation) Then
                                                @<span>（@Me.Model.RouteRemarks） </span>
                                        Else
                                                @<span>（@Me.Model.NeareStstation @Me.Model.Transportation @Me.Model.RequiredTime 分） </span>
                                        End If
                                    Else
                                        If Me.Model.RouteName.EndsWith("道") OrElse Me.Model.RouteName.EndsWith("ﾌｪﾘｰ") Then
                                            @<span>（@Me.Model.RouteName@Me.Model.NeareStstation @Me.Model.Transportation @Me.Model.RequiredTime 分） </span>
                                        Else
                                            @<span>（@Me.Model.RouteName @Me.Model.NeareStstation 駅 @Me.Model.Transportation @Me.Model.RequiredTime 分）</span>
                                        End If
                                    End If
						    </p>

							<p class="departments mb20">
							    @If Me.Model.OptionFlags And QyMedicalSearchTypeEnum.AuLaterPayment Then
                                    @<span class="special">#医療費あと払い対応</span>
                                End If
                                @If Me.Model.OptionFlags And QyMedicalSearchTypeEnum.NightTimeService Then
                                    @<span class="special">#夜間診療可能</span>
                                End If
                                @If Me.Model.OptionFlags And QyMedicalSearchTypeEnum.HolidayService Then
                                    @<span class="special">#日祝診療可能</span>
                                End If
                                @If Me.Model.OptionFlags And QyMedicalSearchTypeEnum.PharumoAutoRegistration Then
                                    @<span class="special">#薬局連携</span>
                                End If   
                                @For i As Integer = 0 To Me.Model.DepartmentN.Count-1
                                    @<span>@Me.Model.DepartmentN.Item(i).ToString</span>
								Next
							</p>

                            <p class="right mb0">
                                @If Not String.IsNullOrEmpty(Me.Model.Url) Then
							        @<a href="native:/action/open_browser?url=@Me.Model.Url" target="_blank" class="btn btn-link no-ico narrow"><i class="la la-external-link"></i> Webサイトを見る</a>
                                End If
                                @If Not String.IsNullOrEmpty(Me.Model.Tel) Then
							        @<a href="tel:@Me.Model.Tel" class="btn btn-submit no-ico narrow"><i class="la la-fax"></i> 電話する</a>
                                End If

                            </p><!-- 押したら.disabledを付与してください -->
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
										<td>祝</td>
									</tr>
								</thead>
								<tbody>
                                  @If Me.Model.MedicalOfficeHouersN.Count = 0 Then
                                    @<tr>
										<th><span> </span></th>

                                        @For counter As Integer = 1 To 8
                                            Dim index As Integer = counter
	    							        @<td>-</td>
                                        Next
									</tr>
                                  End If

                                    @For Each item As List(Of MedicalOfficeHouers) In Me.Model.MedicalOfficeHouersN
                                    
                                        @<tr>
										    <th>
                                                <span>
                                                    @If item(0).AcceptedEnd.StartsWith("23:59") Then
                                                        @String.Format("{0}～{1}", item(0).AcceptedStart, "24:00")
                                                    Else
                                                        @String.Format("{0}～{1}", item(0).AcceptedStart, item(0).AcceptedEnd)
                                                    End If
										        </span>
										    </th>

                                            @For counter As Integer = 1 To 8
                                                Dim index As Integer = counter
                                                If item.Where(Function(i) i.DayOfWeek = index).ToList.Count > 0 Then
		    								        @<td><i class="la la-check-circle"></i></td>
                                                Else
	    									        @<td>-</td>
                                                End If
                                            Next
									    </tr>
                                    Next
			                            
									<tr>
										<td class="spacer" colspan="9"></td>
									</tr>
									<tr>
										<th>受付メモ</th>
										<td class="memo" colspan="8">@Me.Model.AcceptedTimeMemo</td>
									</tr>
								    <tr>
										<th>休診</th>
                                        @If String.IsNullOrWhiteSpace(Me.Model.ClosedMemo) Then
										    @<td class="memo" colspan="8">-</td>
                                        Else
										    @<td class="memo" colspan="8">@Me.Model.ClosedMemo</td>
                                        End If
									</tr>
								</tbody>
							</table>

							<section id="g-map" class="mb70">
								<div id="map" style="width:100%; height: 300px;" data-latitude = "@Me.Model.Latitude"  data-longitude = "@Me.Model.Longitude"></div>
							</section>
							
						</div>
						<div class="submit-area">
                                @If (Me.Model.OptionFlags And QyMedicalSearchTypeEnum.AuLaterPayment) = 0 Then
                    
                                    @If Me.Model.RequestFlag Then
							            @<p class="center mb20"><a  id="postpayRequest" href="javascript:void(0);" class="btn btn-atobarai disabled" data-codeno="@Model.CodeNo"><span><span>「医療費あと払い」はまだご利用できません。</span><br>利用希望を申請する</span></a></p>

                                    Else
							            @<p class="center mb20"><a  id="postpayRequest" href="javascript:void(0);" class="btn btn-atobarai" data-codeno="@Model.CodeNo"><span><span>「医療費あと払い」はまだご利用できません。</span><br>利用希望を申請する</span></a></p>
                                        
                                    End If
                                    @<p class="left red">※申請で必ず利用できるようになるとは限りません。</p>
                                
                                End If
						</div>
					</section>
				</div>
			</div>
			@<div class="modal-footer">
				<button type="button" class="btn btn-close no-ico mb0" data-dismiss="modal">閉じる</button>
			</div>
        End If
         </div>