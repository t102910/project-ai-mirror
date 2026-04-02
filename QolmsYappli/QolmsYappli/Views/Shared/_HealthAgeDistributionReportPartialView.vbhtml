@Imports MGF.QOLMS.QolmsApiCoreV1
@Imports MGF.QOLMS.QolmsYappli
@ModelType HealthAgeDistributionReportPartialViewModel

@Code
    Dim hasData As Boolean = Me.Model.ReportItem.HealthAgeValueN.Any()
End Code

@If hasData Then
    @<article id="distribution-area" class="reload-area" data-report-type="@QyHealthAgeReportTypeEnum.Distribution.ToString()">
        <section class="inner reload-area"> <!-- AJAX-非同期更新エリア load中は.loadingを付与してください。gifアニメーションと文言出ます。-->
            <!-- 分布グラフ -->
            <script type="text/javascript">
                // グラフデータ
                var hrChartData_distribution = { // TODO: hrChartData_d と変数名が固定化されているので確認
                    data: {
                        chartType2: 'distribution',
                        max: @Html.Raw(Me.Model.AgeDistributionGraphSetting.AxisMax),
                        min: 0,
                        dayLabel: @Html.Raw(Me.Model.AgeDistributionGraphSetting.Label),
                        data: @Html.Raw(Me.Model.AgeDistributionGraphSetting.Data)
                    }
                };
            </script>

			<h3 class="line mb10">同性・同年齢の健康年齢分布</h3>

            <div class="chart-draw">
                <div class="chart-area" data-ref="distribution">
                    <div class="chart-draw-area" style="background-color:#f7f7f7; height:150px;"></div> 
                    <small>※あなたと同性で実年齢が同じ人の健康年齢の分布を示しています。</small>
                </div>
            </div>
        </section>
    </article>
Else
    @<article id="distribution-area" class="reload-area" data-report-empty-type="@QyHealthAgeReportTypeEnum.Distribution.ToString()">
        <section class="section caution mt10 mb0">
            未測定です
        </section>
    </article>
End If
