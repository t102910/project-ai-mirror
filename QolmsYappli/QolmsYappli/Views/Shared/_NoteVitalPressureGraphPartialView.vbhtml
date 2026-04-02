@Imports MGF.QOLMS.QolmsYappli
@ModelType VitalPressureGraphPartialViewModel

@Code
    Dim hasData As Boolean = Me.Model.ItemList.Any()
End Code

@If hasData Then
    @<div id="bp-area" class="reload-area" data-vital-type="@QyVitalTypeEnum.BloodPressure.ToString()">
        <script type="text/javascript">
            var hrChartData_bp = {
                startDate: '@Me.Model.ItemList.First().Key.ToString("yyyy/M/d")',
                data: {
                    title: '血圧',
                    unit: 'mmHg',
                    chartType: 'columnrange',
                    chartType2: 'bloodPressure',
                    dataTitle: ['血圧（午前）', '血圧（午後）'],
                    targetValues: @Me.Model.TargetValue,
                    max: @Me.Model.YAxisMax,
                    min: @Me.Model.YAxisMin,
                    dayLabel: @Html.Raw(Me.Model.GraphLabel),
                    data: @Html.Raw(Me.Model.GraphData)
                }
            };
        </script>
        
        <h3 class="title mt10">最近の血圧</h3>

        <div class="chart-draw cont-margin">
            <div class="chart-area" data-ref="bp">
                <div class="chart-draw-area" style="background-color:#f7f7f7; height:300px;"></div> 
            </div>
        </div>

        <section class="table-responsive data-table type-1">
            <table class="table elm" data-vital-type="@QyVitalTypeEnum.BloodPressure.ToString()">
		        <tr>
			        <th>日付</th>
			        @For Each kv As KeyValuePair(Of Date, SortedDictionary(Of Date, VitalValueItem)) In Me.Model.ItemList
                        If kv.Value.Count > 2 AndAlso (kv.Value.Values(0).Value1 > 0 OrElse kv.Value.Values(1).Value1 > 0) Then
			                @<td data-selected-day="@kv.Key.ToString("yyyy年M月d日")">
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
  
                    Dim title As String = If(a = 0, "午前平均", "午後平均")
                    @<tr>
                        <th>@title</th>
                        @For Each kv As KeyValuePair(Of Date, SortedDictionary(Of Date, VitalValueItem)) In Me.Model.ItemList
                            @If kv.Value.Count > a AndAlso (kv.Value.Values(a).Value1 > 0 OrElse kv.Value.Values(a).Value2 > 0) Then
                                @<td data-selected-day="@kv.Key.ToString("yyyy年M月d日")">
                                    @String.Format("{0:0.####} / {1:0.####} mmHg", If(kv.Value.Values(a).Value1 > 0, kv.Value.Values(a).Value1, "－"), If(kv.Value.Values(a).Value2 > 0, kv.Value.Values(a).Value2, "－"))
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
Else
    @<div id="bp-area" class="reload-area" data-vital-empty-type="@QyVitalTypeEnum.BloodPressure.ToString()">
        <h3 class="title mt10">最近の血圧</h3>

        <section class="section caution mt10 mb0">
            未登録です
        </section>
    </div>
End If
