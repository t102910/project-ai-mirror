@Imports MGF.QOLMS.QolmsApiCoreV1
@Imports MGF.QOLMS.QolmsYappli
@ModelType HealthAgeAdviceAreaPartialViewModel

<h3 class="title mt10">
	<span>健康年齢改善のアドバイス</span>
</h3>

@If Me.Model.HasData Then
    @<article class="age-area mb40">
        @For Each advice As HealthAgeAdviceItem In Me.Model.AdviceData
            @If Not String.IsNullOrWhiteSpace(advice.Title) Then
                @<h3 class="title type-2">@advice.Title</h3>
            End If
            @<section class="inner">
                <p class="mb20">@advice.Based</p>
                @If advice.GoalN.Any() Then
                    @<h4>データから見た目標値</h4>
                    @<table class="table target">
						<thead>
							<tr>
								<td></td>
								<td>現 在</td>
								<td>目 標</td>
							</tr>
						</thead>
                        <tbody>
                            @For Each goal As HealthAgeAdviceGoalItem In advice.GoalN
                                @Code
                                    Dim name As String = goal.Name.Trim()
                                    
                                    Select Case True
                                        Case name.CompareTo("収縮期血圧") = 0
                                            name = "血圧（上）"
                                            
                                        Case name.CompareTo("拡張期血圧") = 0
                                            name = "血圧（下）"
                                            
                                        Case name.CompareTo("AST") = 0
                                            name = "AST（GOT）"
                                            
                                        Case name.CompareTo("ALT") = 0
                                            name = "ALT（GPT）"
                                            
                                        Case name.CompareTo("γ-GT") = 0
                                            name = "γ-GT（γ-GTP）"

                                    End Select
                                End Code
                                    
                                @<tr>
                                    <td>@name</td>
                                    <td>@goal.Old.ToString("0.#")</td>
                                    <td>@goal.Goal.ToString("0.#")</td>
                                </tr>
                            Next
                        </tbody>
                    </table>
                End If
                @If advice.DetailN.Any() Then
                    @<div class="advice-area">
                        <ul class="check-list">
                            @For Each detail As String In advice.DetailN
                                @<li>@detail</li>
                            Next
                        </ul>
                    </div>
                End If
            </section>
        Next
    </article>
Else
    @<article class="age-area mb40">
        <section class="section caution mt10 mb0">
            未測定です
        </section>
    </article>
End If
