@Imports MGF.QOLMS.QolmsYappli
@ModelType NoteMetsViewModel

@Code
    ViewData("Title") = "運動強度"
    Layout = "~/Views/Shared/_NoteLayout.vbhtml"
End Code

<body id="mets" class="lower">
    @Html.AntiForgeryToken()
    @*@Html.Action("NoteHeaderPartialView", "Note")*@

    <main id="main-cont" class="clearfix" role="main">

	    <section class="home-btn-wrap type-2">
		    <a href="../Portal/Home" class="home-btn"><i class="la la-home"></i><span> ホーム</span></a>
	    </section>
	    <section class="contents-area mb0">
		    <h2 class="title pt0">運動強度</h2>
		    <hr>
	    </section>

        <script type="text/javascript">

            //targetValues: [40, 120],
            //    max: 148,
            //min: 20,
            //dayLabel: ['8:10', '', '', '', '', '', '', '', '', '', '8:20', '', '', '', '', '', '', '', '', '', '8:30', '', '', '', '', '', '', '', '', '8:40'],
            //data: [null, null, null, null, null, 80, 90, 100, null, 120, null, null, 100, 90, 70, 75, null, null, null, null, null, null, null, null, null, null, null, null, null, null]

         // グラフデータ
         var hrChartData_mets = {
             data: {
                 title: '運動強度',
                 unit: 'Mets',

                 targetValues: @Me.Model.TargetValue,
                 max: @Me.Model.YAxisMax,
                 min: @Me.Model.YAxisMin,
                 dayLabel: @Html.Raw(Me.Model.GraphLabel),
                 data: @Html.Raw(Me.Model.GraphData)
             }
         };
        </script>
	    <section class="data-area">
		    <ul class="period-selecter pt0">
			    <li class="@IIf(Me.Model.PeriodType = QyPeriodTypeEnum.OneDay, "on", String.Empty).ToString()" data-type="@QyPeriodTypeEnum.OneDay"><span>1日</span></li>
			    <li class="@IIf(Me.Model.PeriodType = QyPeriodTypeEnum.OneWeek, "on", String.Empty).ToString()" data-type="@QyPeriodTypeEnum.OneWeek"><span>1週間</span></li>
			    <li class="@IIf(Me.Model.PeriodType = QyPeriodTypeEnum.OneMonth, "on", String.Empty).ToString()" data-type="@QyPeriodTypeEnum.OneMonth"><span>1ヶ月</span></li>
			    <li class="@IIf(Me.Model.PeriodType = QyPeriodTypeEnum.ThreeMonths, "on", String.Empty).ToString()" data-type="@QyPeriodTypeEnum.ThreeMonths"><span>3ヶ月</span></li>
		    </ul>
		    
		    <div id="w-area" class="reload-area"><!-- AJAX-非同期更新エリア load中は.loadingを付与してください。gifアニメーションと文言出ます。-->
			    <section class="date-selector" data-end_date="@Me.Model.EndDate.ToString("yyyy/MM/dd")">
				    <span id="prevDate" data-target_date="" class="arrow arrow-left"></span>
				    <h3>
                        @If (Me.Model.PeriodType = QyPeriodTypeEnum.OneDay) Then
    					    @Me.Model.StartDate.ToString("yyyy/MM/dd (ddd)")
                        Else
    					    @(Me.Model.StartDate.ToString("yyyy/MM/dd (ddd)") + " - " + Me.Model.EndDate.ToString("yyyy/MM/dd (ddd)"))                            
                        End If
				    </h3>
				    <span id="nextDate" data-target_date="" class="arrow arrow-right"></span>	
			    </section>
			    
			    <div class="chart-draw cont-margin">
				    <div class="chart-area" data-ref="mets" data-chart="normalLine">
					    <div class="chart-draw-area" style="background-color:#f7f7f7; height:300px;"></div> 
				    </div>
			    </div>
		    </div>

	    </section>

    </main>

    @Html.Action("NoteFooterPartialView", "Note")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/note/mets")
</body>
