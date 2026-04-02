@Imports MGF.QOLMS.QolmsYappli
@ModelType VitalStepsGraphPartialViewModel

@Code
    Dim hasData As Boolean = Me.Model.ItemList.Any()
End Code

@If hasData AndAlso Me.Model.RowCount = 1 Then
    @<div id="gh-area" class="reload-area" data-vital-type="@QyVitalTypeEnum.Steps.ToString()">
        <script type="text/javascript">
            var hrChartData_gh = {
                data: {
                    title: '歩数',
                    unit: '歩',
                    targetValues: @Me.Model.TargetValue,
                    max: @Me.Model.YAxisMax,
                    min: @Me.Model.YAxisMin,
                    data: @Html.Raw(Me.Model.GraphData)
                }
            };
        </script>

        <h3 class="title mt10">最近の歩数</h3>

		<div class="chart-draw cont-margin">
			<div class="chart-area" data-ref="gh" data-chart="normalLine">
				<div class="chart-draw-area" style="background-color:#f7f7f7; height:300px;"></div> 
			</div>
		</div>

        <section class="table-responsive data-table type-1">
            <table class="table elm" data-vital-type="@QyVitalTypeEnum.Steps.ToString()">
		        <tr>
			        <th>日付</th>
			        @For Each kv As KeyValuePair(Of Date, SortedDictionary(Of Date, VitalValueItem)) In Me.Model.ItemList
                        If kv.Value.Count = 1 AndAlso kv.Value.Values.First().Value1 > 0 Then
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
                <tr>
                    <th>歩数</th>
                    @For Each kv As KeyValuePair(Of Date, SortedDictionary(Of Date, VitalValueItem)) In Me.Model.ItemList
                        @If kv.Value.Count = 1 AndAlso kv.Value.Values.First().Value1 > 0 Then
                            @<td data-selected-day="@kv.Key.ToString("yyyy/MM/dd")">
                                @String.Format("{0:#,###.####} 歩", kv.Value.Values.First().Value1)
                            </td>
                        Else
                            @<td></td>
                        End If
                    Next
                    <th>歩数</th>
                </tr>
            </table>
        </section>
    </div>
Else
    @<div id="gh-area" class="reload-area" data-vital-empty-type="@QyVitalTypeEnum.Steps.ToString()">
        <h3 class="title mt10">最近の歩数</h3>

        <section class="section caution mt10 mb0">
            未登録です
        </section>
    </div>
End If
