@Imports MGF.QOLMS.QolmsApiCoreV1
@Imports MGF.QOLMS.QolmsYappli
@ModelType HealthAgeLiverReportPartialViewModel

@Code
    Dim hasData As Boolean = Me.Model.ReportItem.HealthAgeValueN.Any()
End Code

@If hasData Then
    @<article id="liver-area" class="reload-area" data-report-type="@QyHealthAgeReportTypeEnum.Liver.ToString()">
        <h2 class="box-title">
            <b>肝臓</b>についてのレポート
        </h2>

        <section class="inner">
            <!-- 総合評価 -->
            <div class="section default grade">
                <span class="bold">総合<br>評価</span>
                @Select Case Me.Model.InsDeviance
                   Case 1D
                        @<span>
                            <i class="grade-icon bad"></i>
                        </span>
                        @<span class="bold bad">改善して！</span>
                    Case 2D
                        @<span>
                            <i class="grade-icon ordinary"></i>
                        </span>
                        @<span class="bold bad">注意が必要！</span>
                    Case 3D
                        @<span>
                            <i class="grade-icon good"></i>
                        </span>
                        @<span class="bold bad">よく頑張りました！</span>
                End Select
            </div>
        </section>

        <section class="inner reload-area"> <!-- AJAX-非同期更新エリア load中は.loadingを付与してください。gifアニメーションと文言出ます。-->
            <!-- 比較グラフ -->
            <script type="text/javascript">
                // グラフデータ
                var hrChartData_nb5 = {
                    data: {
                        title: @Html.Raw(Me.Model.InsComparisonGraphSetting.Title),
                        max: 3,
                        min: -3,
                        dayLabel: @Html.Raw(Me.Model.InsComparisonGraphSetting.Label),
                        targetValues: @Html.Raw(Me.Model.InsComparisonGraphSetting.TargetValue),
                        data: @Html.Raw(Me.Model.InsComparisonGraphSetting.Data)
                    }
                };
            </script>

            <h3 class="line mb10">同性・同年代との数値比較</h3>

            <div class="chart-draw mb30">
                <div class="chart-area" data-ref="nb5" data-kind="negativebar">
                    <div class="chart-draw-area" style="background-color:#f7f7f7; height:160px;"></div> 
                </div>
            </div>

            <!-- 測定値グラフ 1 -->
            <script type="text/javascript">
                // グラフデータ
                var hrChartData_got = {
                    data: {
                        title: @Html.Raw(Me.Model.Ch025GraphSetting.Title),
                        max: @Html.Raw(Me.Model.Ch025GraphSetting.AxisMax),
                        min: @Html.Raw(Me.Model.Ch025GraphSetting.AxisMin),
                        dayLabel: @Html.Raw(Me.Model.Ch025GraphSetting.Label),
                        targetValues: @Html.Raw(Me.Model.Ch025GraphSetting.TargetValue), // [0]正常値min [1]正常値max / [2]注意値min [3]注意値max / [4]注意値min [5]注意値max / [6]警告値min [7]警告値max / [8]警告値min [9]警告値max
                        data: @Html.Raw(Me.Model.Ch025GraphSetting.Data)
                    }
                };
            </script>

            <h3 class="line mb10">AST（GOT）</h3>

            <div class="chart-draw mb20">
                <div class="chart-area" data-ref="got">
                    <div class="chart-draw-area" style="background-color:#f7f7f7; height:130px;"></div> 
                </div>
            </div>

            <!-- 測定値グラフ 2 -->
            <script type="text/javascript">
                // グラフデータ
                var hrChartData_gpt = {
                    data: {
                        title: @Html.Raw(Me.Model.Ch027GraphSetting.Title),
                        max: @Html.Raw(Me.Model.Ch027GraphSetting.AxisMax),
                        min: @Html.Raw(Me.Model.Ch027GraphSetting.AxisMin),
                        dayLabel: @Html.Raw(Me.Model.Ch027GraphSetting.Label),
                        targetValues: @Html.Raw(Me.Model.Ch027GraphSetting.TargetValue), // [0]正常値min [1]正常値max / [2]注意値min [3]注意値max / [4]注意値min [5]注意値max / [6]警告値min [7]警告値max / [8]警告値min [9]警告値max
                        data: @Html.Raw(Me.Model.Ch027GraphSetting.Data)
                    }
                };
            </script>

            <h3 class="line mb10">ALT（GPT）</h3>

            <div class="chart-draw mb20">
                <div class="chart-area" data-ref="gpt">
                    <div class="chart-draw-area" style="background-color:#f7f7f7; height:130px;"></div> 
                </div>
            </div>

            <!-- 測定値グラフ 3 -->
            <script type="text/javascript">
                // グラフデータ
                var hrChartData_gt = {
                    data: {
                        title: @Html.Raw(Me.Model.Ch029GraphSetting.Title),
                        max: @Html.Raw(Me.Model.Ch029GraphSetting.AxisMax),
                        min: @Html.Raw(Me.Model.Ch029GraphSetting.AxisMin),
                        dayLabel: @Html.Raw(Me.Model.Ch029GraphSetting.Label),
                        targetValues: @Html.Raw(Me.Model.Ch029GraphSetting.TargetValue), // [0]正常値min [1]正常値max / [2]注意値min [3]注意値max / [4]注意値min [5]注意値max / [6]警告値min [7]警告値max / [8]警告値min [9]警告値max
                        data: @Html.Raw(Me.Model.Ch029GraphSetting.Data)
                    }
                };
            </script>

            <h3 class="line mb10">γ-GT（γ-GTP）</h3>

            <div class="chart-draw mb20">
                <div class="chart-area" data-ref="gt">
                    <div class="chart-draw-area" style="background-color:#f7f7f7; height:130px;"></div> 
                </div>
				<p class="legend-area">
					<span class="legend bad"></span>要改善 / <span class="legend poor"></span>要注意 / <span class="legend good"></span>正常
				</p>
            </div>
        </section>
    </article>
Else
    @<article id="liver-area" class="reload-area" data-report-empty-type="@QyHealthAgeReportTypeEnum.Liver.ToString()">
        <h2 class="box-title">
            <b>肝臓</b>についてのレポート
        </h2>

        <section class="section caution mt10 mb0">
            未測定です
        </section>
    </article>
End If
