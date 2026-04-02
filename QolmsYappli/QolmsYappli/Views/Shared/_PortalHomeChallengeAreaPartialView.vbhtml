@Imports MGF.QOLMS.QolmsYappli
@ModelType PortalHomeChallengeAreaPartialViewModel  
@Code

End Code
            <div id="challenge-area" data-target-day="@Me.Model.ChallengeTargetDay"  data-all-days="@Me.Model.ChallengeAllDays" data-start-day="@Me.Model.ChallengeStartDate.ToString("yyyy/MM/dd")">
                @If Me.Model.ChallengeTargetWeightLoss > 0 Then
		            @<h3 class="title-2 no-wrap mt10">@String.Format("健康チャレンジ実施中(目標まであと {0:###########0.0} kg)", Me.Model.ChallengeTargetWeightLoss)</h3>
                ElseIf Me.Model.ChallengeTargetWeightLoss = 0 Then
		            @<h3 class="title-2 no-wrap mt10">@String.Format("健康チャレンジ実施中(目標達成！)")</h3>
                Else
		            @<h3 class="title-2 no-wrap mt10">@String.Format("健康チャレンジ実施中(体重を入力してください)")</h3>
                End If

		        <section id="select-day-ui" class="">
			        <div class="inner">
				        <ul>

                            @For i As Integer = 1 To Me.Model.ChallengeAllDays
                                Dim strOnOff As String = IIf(i <= Me.Model.ChallengeTargetDay, "on", "off").ToString()
                                Dim strToday As String = IIf(i = Me.Model.ChallengeTargetDay, "today", String.Empty).ToString()
                                
					            @<li class="@strOnOff @strToday" data-date="@i">
						            <span>
							            <strong>@i</strong>
							            <i>日目</i>
						            </span>
					            </li>
                                
                            Next
					        
				        </ul>
			        </div>
		        </section>
            </div>