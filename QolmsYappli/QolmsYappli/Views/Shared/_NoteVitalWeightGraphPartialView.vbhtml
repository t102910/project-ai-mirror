@Imports MGF.QOLMS.QolmsYappli
@ModelType VitalWeightGraphPartialViewModel

@Code
    Dim hasData As Boolean = Me.Model.ItemList.Any()
End Code

@If hasData Then
    @<div id="w-area" class="reload-area" data-vital-type="@QyVitalTypeEnum.BodyWeight.ToString()">
        <script type="text/javascript">
            var hrChartData_w = {
                startDate: '@Me.Model.ItemList.First().Key.ToString("yyyy/M/d")',
                data: {
                    title: '体重',
                    unit: 'kg',
                    chartType2: 'bodyWeight',
                    dataTitle: ['体重', 'BMI'],
                    targetValues: @Me.Model.TargetValue,
                    max: @Me.Model.YAxisMax,
                    min: @Me.Model.YAxisMin,
                    bmiMax: @Me.Model.YAxisBmiMax,
                    bmiMin: @Me.Model.YAxisBmiMin,
                    dayLabel: @Html.Raw(Me.Model.GraphLabel),
                    data: @Html.Raw(Me.Model.GraphData)
                }
            };
        </script>

        <h3 class="title mt10">最近の体重</h3>

        <div class="chart-draw cont-margin">
            <div class="chart-area" data-ref="w">
                <div class="chart-draw-area" style="background-color:#f7f7f7; height:300px;"></div> 
            </div>
        </div>

        <section class="table-responsive data-table type-1">
            <table class="table elm" data-vital-type="@QyVitalTypeEnum.BodyWeight.ToString()">
		        <tr>
			        <th>日付</th>
			        @For Each kv As KeyValuePair(Of Date, SortedDictionary(Of Date, VitalValueItem)) In Me.Model.ItemList
                        If kv.Value.Count > 2 AndAlso (kv.Value.Values(0).Value1 > 0 OrElse kv.Value.Values(1).Value1 > 0) Then
			                @<td data-selected-day="@kv.Key.ToString("yyyy/MM/dd")">
                                @kv.Key.ToString("M/d")
			                </td>
                        Else
			                @<td>
                                @kv.Key.ToString("M/d")
			                </td>
                        End If
			        Next
			        <th>日付</th>
		        </tr>
                @For a As Integer = 0 To Me.Model.RowCount - 1
                    If a > 1 Then Exit For
                    
                        Dim title As String = If(a = 0, "体重平均", "BMI平均")
                    @<tr>
                        <th>@title</th>
                        @For Each kv As KeyValuePair(Of Date, SortedDictionary(Of Date, VitalValueItem)) In Me.Model.ItemList
                            @If kv.Value.Count > a AndAlso kv.Value.Values(a).Value1 > 0 Then
                                @<td data-selected-day="@kv.Key.ToString("yyyy/MM/dd")">
                                    @String.Format(If(a = 0, "{0:0.####} kg", "{0:0.####}"), kv.Value.Values(a).Value1)
                                </td>
                            Else
                                @<td></td>
                            End If
                        Next
                        <th>@title</th>
                    </tr>
                Next
            </table>
        </section>
    </div>

    @If Me.Model.AgeItemList.Any() Then
        @<div id="ha-area" class="mt20">
            <script type="text/javascript">
                // グラフデータ
                var hrChartData_healthage = {
                    data: {
                        title: '健康年齢',
                        unit: '歳',
                        max: @Me.Model.YAxisAgeMax,
                        min: @Me.Model.YAxisAgeMin,
                        dayLabel: @Html.Raw(Me.Model.AgeGraphLabel),
                        data: @Html.Raw(Me.Model.AgeGraphData)
                    }
                };
            </script>

			<h3 class="title mt10">最近の健康年齢</h3>

			<div class="chart-draw cont-margin">
				<div class="chart-area" data-ref="healthage" data-chart="normalLine">
					<div class="chart-draw-area" style="background-color:#f7f7f7; height:300px;"></div> 
				</div>
			</div>

            <section class="table-responsive data-table type-1">
                <table class="table elm">
		            <tr>
			            <th>日付</th>
			            @For Each kv As KeyValuePair(Of Date, SortedDictionary(Of Date, VitalValueItem)) In Me.Model.AgeItemList
                            If kv.Value.Count = 1 AndAlso kv.Value.Values.First().Value1 > 0 Then
                                @<td>
                                    @kv.Key.ToString("M/d")
                                </td>
                            Else
                                @<td>
                                    @kv.Key.ToString("M/d")
                                </td>
                            End If
			            Next
			            <th>日付</th>
		            </tr>
                    <tr>
                        <th>健康年齢</th>
                        @For Each kv As KeyValuePair(Of Date, SortedDictionary(Of Date, VitalValueItem)) In Me.Model.AgeItemList
                            @If kv.Value.Count = 1 AndAlso kv.Value.Values.First().Value1 > 0 Then
                                @<td>
                                    @String.Format("{0:0.#} 歳", kv.Value.Values.First().Value1)
                                </td>
                            Else
                                @<td></td>
                            End If
                        Next
                        <th>健康年齢</th>
                    </tr>
                </table>
            </section>
        </div>
    End If
Else
    @<div id="w" class="reload-area" data-vital-empty-type="@QyVitalTypeEnum.BodyWeight.ToString()">
        <h3 class="title mt10">最近の体重</h3>

        <section class="section caution mt10 mb0">
            未登録です
        </section>
    </div>
End If
