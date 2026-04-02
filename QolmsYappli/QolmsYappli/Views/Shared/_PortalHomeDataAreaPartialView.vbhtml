@Imports MGF.QOLMS.QolmsCryptV1
@Imports MGF.QOLMS.QolmsYappli
@ModelType PortalHomeDataAreaPartialViewModel  

@Code

    Dim showDay As Date = Me.Model.ShowDay.Date
    
    Dim totalCalOut As Decimal = 0
    'totalCalOut = IIf(Me.Model.BasalMetabolism > 0, Me.Model.BasalMetabolism, 0) + IIf(Me.Model.ExerciseCal > 0, Me.Model.ExerciseCal, 0) + IIf(Me.Model.StepsCal > 0, Me.Model.StepsCal, 0)
    totalCalOut = IIf(Me.Model.ExerciseCal > 0, Me.Model.ExerciseCal, 0) + IIf(Me.Model.StepsCal > 0, Me.Model.StepsCal, 0)

    Dim totalCalIn As Decimal = 0
    totalCalIn = IIf(Me.Model.CalBreakfast > 0, Me.Model.CalBreakfast, 0) + IIf(Me.Model.CalLunch > 0, Me.Model.CalLunch, 0) + IIf(Me.Model.CalDinner > 0, Me.Model.CalDinner, 0) + IIf(Me.Model.CalSnacking > 0, Me.Model.CalSnacking, 0)

    
    Dim calGaugeMax As Decimal = Model.TargetCalorieIn
    Dim calGaugeValue As Decimal = totalCalIn - totalCalOut
    Dim calMore As Decimal = Me.Model.TargetCalorieIn - totalCalIn + totalCalOut
    
    Dim challengeFlag As Boolean = Me.Model.PageViewModel.ChallengeAreaPartialViewModel.Challengekey <> Guid.Empty
    Dim challengekey As String = String.Empty
    If challengeFlag Then
        Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsWeb)
        
            challengekey = crypt.EncryptString(Me.Model.PageViewModel.ChallengeAreaPartialViewModel.Challengekey.ToString())
        End Using
    End If
    
End Code

        <div id="update-area" class="@IIf(Me.Model.PageViewModel.SetUpFlag, String.Empty, "today-update").ToString()">
            <section id="points-area">

                @If Me.Model.PageViewModel.SetUpFlag = False Then
                    @<span id="leftbutton" data-target_date="14" class="arrow arrow-left"></span>

                    @If showDay.Date <> Date.Now.Date Then
			            @<span id="rightbutton" data-target_date="16" class="arrow arrow-right"></span>'<!-- 「今日」なら.hideで消す -->
                   End If
                End If

                <p class="point-exchange-btn-wrap left">
				    <a href="../Portal/History?fromPageNo=1" class="btn point-exchange">
					    <span id="point-input" class="point-input din">@IIf(Me.Model.PointNow > 0, Me.Model.PointNow, 0).ToString()</span><br>
                        <i>ポイント</i>
				    </a>
			    </p>

			    <p class="point-exchange-btn-wrap right">
				    <a href="../Note/Walk" class="btn point-exchange steps-input">
                        <span id="steps-input" class="steps-input din">@Me.Model.Steps</span>
					<i>歩 数</i>
				    </a>
			    </p>
            @If Me.Model.PageViewModel.SetUpFlag = False Then         

			    @<div class="entered-area">
				    <ul>
					    <li id="entered-steps">歩数</li>
					    <li id="entered-weight">体重</li>
					    <li id="entered-breakfast">朝食</li>
					    <li id="entered-lunch">昼食</li>
					    <li id="entered-dinner">夕食</li>
					    <li id="entered-column" class="@IIf(challengeFlag, String.Empty, "hide")" data-key="@challengekey">コラム</li>
				    </ul>
			    </div>
                @<div id="control-area">
			        <p id="date">
                         <span id="showday" data-now-day="@showDay.ToString("yyyy/MM/dd")">@Me.Model.ShowDay.ToString("\'yy年M月d日(ddd)")</span>
			        </p>
		        </div>

                @<div class="gauge">
			        <img id="shika" src="@IIf(calMore >= 0, "../dist/img/index/touroku-shika.png", "../dist/img/index/undou-shika.png").ToString()">
			        <canvas width="250px" height="250px" id="canvas-preview"></canvas>
			        <div id="over-1000" class="point-result">
					    @If calMore >= 0 Then

     					    @<h2>のこり摂取カロリー<br>あと</h2>
					    Else
					        @<h2>カロリーオーバー</h2>
					    End If

                        <h3 id="point-field"><div id="preview-textfield" data-gaugemax="@calGaugeMax"data-gaugeval ="@iif(calGaugeValue>0,calGaugeValue,0).ToString()" data-more="@calMore">@IIf(Me.Model.TargetCalorieIn > 0, Math.Abs(calMore).ToString("#0"), "　")</div></h3>
                    </div>
                     <p class="indication">
					    摂取目安：@Me.Model.TargetCalorieIn.ToString("#,##0")
				    </p>
                </div>
            End If
                
	        </section> 
                
            @If Me.Model.PageViewModel.SetUpFlag = False Then  

            @<section id="data-area" class="mb5">
			    <div id="table">
                    <table class="table data-table logona ">
		                <tr>
			                <td>朝食</td>
			                <td>@String.Format("{0:###,##0}", Me.Model.CalBreakfast)<i class="unit">kcal</i></td>
			                <td rowspan="2">歩数</td>
			                <td id="steps-data">@String.Format("{0:###,##0}", Me.Model.Steps)<i class="unit">歩</i></td>
		                </tr>
		                <tr>
			                <td>昼食</td>
			                <td>@String.Format("{0:###,##0}", Me.Model.CalLunch)<i class="unit">kcal</i></td>
    		                <td>@String.Format("{0:###,##0}", Me.Model.StepsCal)<i class="unit">kcal</i></td>
                        </tr>
		                <tr>
			                <td>夕食</td>
			                <td>@String.Format("{0:###,##0}", Me.Model.CalDinner)<i class="unit">kcal</i></td>
			                <td>運動</td>
			                <td>@String.Format("{0:###,##0}", Me.Model.ExerciseCal)<i class="unit">kcal</i></td>
		                </tr>
		                <tr>
			                <td>間食</td>
			                <td>@String.Format("{0:###,##0}", Me.Model.CalSnacking)<i class="unit">kcal</i></td>
			                <td class="font-s">基礎<br>代謝量 </td>
			                <td><em>@String.Format("{0:###,##0}", Me.Model.BasalMetabolism)</em><i class="unit">kcal</i></td>
                        </tr>
	                </table>
			    </div>
			    <div class="inner">
			        <div id="cal-data">
					    <div class="cal-in cal">
                            <a href="../Note/Calomeal">
						        <h3>摂取カロリー</h3>
						        <div class="data">
							        <p>
								        <span class="result"><em id="preview-textfield2" class="din">@totalCalIn.ToString("#0")</em><i class="unit">kcal</i></span>
							        </p>
						        </div>
						        <h4>
							        <span>食事</span>
							        <i></i>
						        </h4>
                            </a>
					    </div>
					    <div id="cal-drower" class="off"></div>
					    <div class="cal-out cal">
                            <a href="../Note/Exercise">
						        <h3>消費カロリー</h3>
						        <div class="data">
							        <p>
								        <span class="result"><em id="preview-textfield3" class="din">@totalCalOut.ToString("#0")</em><i class="unit">kcal</i></span>
							        </p>
						        </div>
						        <h4>
							        <span>運動</span>
							        <i></i>
						        </h4>
                            
                            </a>
					    </div>
				    </div>
			    </div>

            </section>
            Else
                
               @<section id="data-area" class="mb5" style="height:200px">
                  </section>
         
               @<div id="" class="mb20">
        
                    <div class="inner">
                        <div class="inner center">
			                <a href="native:/tab/custom/aafe1860" class="btn btn-default"><span class="small"></span> 基本情報を入力する！</a>
	                    </div>
                        <p class="section default">
                            すべての機能を利用するには基本情報の登録が必要です。 <br />
                            基本登録が完了すると、基礎代謝・消費カロリー・登録状況を表示できます。 
                        </p>
        
	                </div>
                </div>
            End If

        </div>