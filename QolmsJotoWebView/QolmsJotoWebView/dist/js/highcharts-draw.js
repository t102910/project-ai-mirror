;(function($) {

	var labelFormats = {
		hour: ' ',
		day: '%b%e日',
		week: '%b%e日',
		month: '%m月',
		year: '%Y年'
	};
	var tpLabelFormats = {
		day: '%Y年%b%e日 (%a)',
		week: '%Y年%b%e日',
		month: '%Y年%b',
		year: '%Y年'
	};

	Highcharts.setOptions({
		lang: {
			resetZoom: '元の表示に戻す',
			months: ['1月','2月','3月','4月','5月','6月','7月','8月','9月','10月','11月','12月'],
			shortMonths: ['1月','2月','3月','4月','5月','6月','7月','8月','9月','10月','11月','12月'],
			weekdays: ['日','月','火','水','木','金','土'],
			thousandsSep: ',',
			numericSymbols: null
		}
	});

	var drawGraph_bp = function ($g, dataName) {
		// UTC time
		var data = hrChartData_bp['data'];

		var startDate = new Date(hrChartData_bp.startDate);
		var yy = startDate.getFullYear();
		var mm = startDate.getMonth();
		var dd = startDate.getDate();

		var chartType = data.chartType||'line';
		var chartType2 = data.chartType2;
		var chartSeries = [];
		var paramsW = {
			credits: { enabled:false },
			chart: {
				type: chartType,
				//zoomType: 'x'
			},
			title: { text:null },
			yAxis: {
				title: { text:null },
				min: data.min,
				max:data.max
			},
			xAxis: {
				title: { text:null },
				gridLineWidth: 1,
				type: 'category',
				categories:data.dayLabel,
				tickInterval: 3
// 				dateTimeLabelFormats: labelFormats
			},
			tooltip: {
//				valueSuffix: data.unit,
// 				dateTimeLabelFormats: tpLabelFormats
/*
				formatter:function(){
                    return 'x value: ' + this.data.data[0] + ' y value ' + this.data.data[1];
                }
*/
			}
		};

		switch (chartType2) {
		case 'bloodPressure':

			paramsW.yAxis.plotLines = [{
				width:2, color:'rgba(180,215,254,1)',
				dashStyle: 'solid',
				value: data.targetValues[0],
			},
			{
				width:2, color:'rgba(180,215,254,1)',
				dashStyle: 'solid',
				value: data.targetValues[1]
			},
			{
				width:2, color:'rgba(198,187,254,1)',
				dashStyle: 'solid',
				value: data.targetValues[2]
			},
			{
				width:2, color:'rgba(198,187,254,1)',
				dashStyle: 'solid',
				value: data.targetValues[3]
			}
			];
			
			
			paramsW.tooltip = {
				valueSuffix: data.unit,
// 				dateTimeLabelFormats: tpLabelFormats,
				formatter:function(){
		            return '<small style="font-size: 10px">' + this.x + '</small><br>' + '<tspan style="fill:' + this.series.color + '">血圧</tspan><tspan dx="0">: </tspan>' + '<b>' + this.point.high + '</b>' + ' / ' + '<b>' + this.point.low + 'mmHg</b>';
		        }
			};
			

			paramsW.yAxis.plotBands = [{
				color: 'rgba(180,215,254,0.5)', 
				from: data.targetValues[0], 
				to: data.targetValues[1]
			},{
				color: 'rgba(198,187,254,0.5)', 
	            from: data.targetValues[2], 
	            to: data.targetValues[3]
	            
	        }],
			paramsW.subtitle = {
				align:'right', verticalAlign:'bottom', y:-5,
				useHTML: true,
				//text: '<div id="chartBPNote"><span class="bpUU">&mdash;&middot;&mdash;【上】目標上限</span>　　<span class="bpUL">&mdash;&middot;&mdash;【上】目標下限</span></div><br><div id="chartBPNote"><span class="bpLU">&ndash;&ndash;&ndash;【下】目標上限</span>　 　<span class="bpLL">&ndash;&ndash;&ndash;【下】目標下限</span></div>'
				text: '<div id="chartBPNote"><span class="bpUU" style="color:#9b88f8!important;">■【上】目標</span><br><span class="bpUL">■【下】目標</span></div>'
			};

			chartSeries.push({
				name: data.dataTitle[0],
				color: '#85b3fe',
				data: data.data[0]
			});
			chartSeries.push({
				name: data.dataTitle[1],
				color: '#EB5406',
				data: data.data[1]
			});
			break;
		}

		paramsW.series = chartSeries;
		$g.highcharts(paramsW);
	};
	
	
	
	//体重
	
	var drawGraph_w = function ($g, dataName) {
		// UTC time
		var data = hrChartData_w['data'];

		var startDate = new Date(hrChartData_w.startDate);
		var yy = startDate.getFullYear();
		var mm = startDate.getMonth();
		var dd = startDate.getDate();

		var chartType = data.chartType||'line';
		var chartType2 = data.chartType2;
		var chartSeries = [];
		var paramsW = {
			credits: { enabled:false },
			chart: {
				type: chartType,
				//zoomType: 'x'
			},
			title: { text:null },
			yAxis: {
				title: { text:null },
				min: data.min,
				max:data.max
			},
			xAxis: {
				title: { text:null },
				gridLineWidth: 1,
				type: 'category',
				categories:data.dayLabel,
				tickInterval: 3
// 				dateTimeLabelFormats: labelFormats
			},
			tooltip: {
				valueSuffix: data.unit
/*
				dateTimeLabelFormats: tpLabelFormats
*/
			},
			plotOptions: {
				series: {
					connectNulls: true,
				}
			}
		};

		switch (chartType2) {			
		case 'bodyWeight':
			
			paramsW.yAxis = [
				{
					labels: {
						formatter: function() { return this.value +'kg'; }
					},
					title: {
						text: '体重',
						style: { color: "#85b3fe"}
					},
					min: data.min,
					max: data.max
				},
				{
					labels: {
						formatter: function() { return this.value +''; }
					},
					title: {
						text: 'BMI',
						style: { color: '#eb5405'}
					},
					opposite: true,		//trueにすると,グラフの右側に表示
					min: data.bmiMin,
					max: data.bmiMax
				}
			];	
			
			paramsW.yAxis[0].plotLines = [{
				width:2, color:'rgba(180,215,254,1)',
				dashStyle: 'solid',
				value: data.targetValues[0],
			},
			{
				width:2, color:'rgba(180,215,254,1)',
				dashStyle: 'solid',
				value: data.targetValues[1]
			}
			];
			
			paramsW.yAxis[1].plotLines = [{
				width:2, color:'rgba(253,172,130,1)',
				dashStyle: 'solid',
				value: data.targetValues[2]
			},
			{
				width:2, color:'rgba(253,172,130,1)',
				dashStyle: 'solid',
				value: data.targetValues[3]
			}
			];

			paramsW.yAxis[0].plotBands = [{
				color: 'rgba(180,215,254,0.3)', 
				from: data.targetValues[0], 
				to: data.targetValues[1]
			}],
	        paramsW.yAxis[1].plotBands = [{
				color: 'rgba(250,209,188,0.3)', 
	            from: data.targetValues[2], 
	            to: data.targetValues[3]
	        }],
			paramsW.subtitle = {
				align:'right', verticalAlign:'bottom', y:-5,
				useHTML: true,
				text: '<div id="chartBPNote"><span class="bpUL">■【体重】目標</span><br><span class="bpUL" style="color:#eb5405!important;">■【BMI】目標</span></div>'
			};
			chartSeries.push({
				name: data.dataTitle[0],
				color: '#85b3fe',
				data: data.data[0]
			});
			chartSeries.push({
				name: data.dataTitle[1],
				color: '#EB5406',
				yAxis: 1,
				tooltip: {
		            valueSuffix: ''
		        },
				data: data.data[1]
			});
			
			break;
		}
		paramsW.series = chartSeries;
		$g.highcharts(paramsW);
	};

	
	var drawGraph_bgl = function ($g, dataName) {
		// UTC time
		var data = hrChartData_bgl['data'];

		var startDate = new Date(hrChartData_bgl.startDate);
		var yy = startDate.getFullYear();
		var mm = startDate.getMonth();
		var dd = startDate.getDate();

		var chartType = data.chartType||'line';
		var chartType2 = data.chartType2;
		var chartSeries = [];
		var paramsW = {
			credits: { enabled:false },
			chart: {
				type: chartType,
				//zoomType: 'x'
			},
			title: { text:null },
			yAxis: {
				title: { text:null },
				min: data.min,
				max:data.max
			},
			xAxis: {
				title: { text:null },
				gridLineWidth: 1,
				type: 'category',
				categories:data.dayLabel,
				tickInterval: 3
// 				dateTimeLabelFormats: labelFormats
			},
			tooltip: {
				valueSuffix: data.unit,
// 				dateTimeLabelFormats: tpLabelFormats
			},
			plotOptions: {
				series: {
					connectNulls: true,
				}
			}
		};

		switch (chartType2) {
		case 'bloodGlucoseLevel':
			
			paramsW.yAxis.plotLines = [{
				width:2, color:'rgba(253,172,130,1)',
				dashStyle: 'solid',
				value: data.targetValues[0],
			},
			{
				width:2, color:'rgba(253,172,130,1)',
				dashStyle: 'solid',
				value: data.targetValues[1]
			},
			{
				width:2, color:'rgba(180,215,254,1)',
				dashStyle: 'solid',
				value: data.targetValues[2]
			},
			{
				width:2, color:'rgba(180,215,254,1)',
				dashStyle: 'solid',
				value: data.targetValues[3]
			}
			];
			
			paramsW.yAxis.plotBands = [{
				color: 'rgba(250,209,188,0.3)', 
				from: data.targetValues[0], 
				to: data.targetValues[1]
			},{
				
				color: 'rgba(180,215,254,0.3)', 
	            from: data.targetValues[2], 
	            to: data.targetValues[3]
	            
	        }],
			paramsW.subtitle = {
				align:'right', verticalAlign:'bottom', y:-5,
				useHTML: true,
				//text: '<div id="chartBPNote"><span class="bpUU">&mdash;&middot;&mdash;【通常時】目標上限</span>　　<span class="bpUL">&mdash;&middot;&mdash;【通常時】目標下限</span></div><br><div id="chartBPNote"><span class="bpLU">&ndash;&ndash;&ndash;【空腹時】目標上限</span>　 　<span class="bpLL">&ndash;&ndash;&ndash;【空腹時】目標下限</span></div>'
				text: '<div id="chartBPNote"><span class="bpUL">■【その他】目標</span><br><span class="bpUU" style="color:#eb5405!important;">■【空腹時】目標</span></div>'
			};
			chartSeries.push({
				name: data.dataTitle[0],
				color: '#EB5406',
				data: data.data[0]
			});
			chartSeries.push({
				name: data.dataTitle[1],
				color: '#85b3fe',
				data: data.data[1]
			});
			break;
		}
		paramsW.series = chartSeries;
		$g.highcharts(paramsW);
	};
	
	//健康年齢
	var drawGraph_healthage = function ($g, dataName) {
		// UTC time
		
		var data = hrChartData_healthage['data'];
		
		var chartType = data.chartType||'line';
		var chartSeries = [];
		var paramsW = {
			credits: { enabled:false },
			
			chart: {
				type: chartType,
				//zoomType: 'x'
			},
			title: { text:null },
			yAxis: {
				labels: {
					formatter: function() { return this.value +'歳'; }
				},
				title: { text:null },
				min: data.min,
				max:data.max
			},
			xAxis: {
				title: { text:null },
				gridLineWidth: 1,
				type: 'category',
				categories:data.dayLabel,
				//dateTimeLabelFormats: labelFormats
				tickInterval: 3
			},
			tooltip: {
				valueSuffix: data.unit,
				//dateTimeLabelFormats: tpLabelFormats
			},
			plotOptions: {
				series: {
					connectNulls: true,
				}
			}
		};

		chartSeries.push({
			name: data.title,
			color: '#EB5406',
			data: data.data
		});

		paramsW.series = chartSeries;
		$g.highcharts(paramsW);
	};
	
	//心拍数
	var drawGraph_pr = function ($g, dataName) {
		// UTC time
		
		var data = hrChartData_pr['data'];
		
		var chartType = data.chartType||'line';
		var chartSeries = [];
		var paramsW = {
			credits: { enabled:false },
			
			chart: {
				type: chartType,
				//zoomType: 'x'
			},
			title: { text:null },
			yAxis: {
				labels: {
					formatter: function() { return this.value +'bpm'; }
				},
				title: { text:null },
				min: data.min,
				max:data.max
			},
			xAxis: {
				title: { text:null },
				gridLineWidth: 1,
				type: 'category',
				categories:data.dayLabel,
				//dateTimeLabelFormats: labelFormats
			},
			tooltip: {
				valueSuffix: data.unit,
				//dateTimeLabelFormats: tpLabelFormats
			},
			plotOptions: {
				series: {
					connectNulls: false,
				}
			}
		};
		
		paramsW.yAxis.plotLines = [{
			width:2, color:'rgba(180,215,254,1)',
			dashStyle: 'solid',
			value: data.targetValues[0],
		},
		{
			width:2, color:'rgba(180,215,254,1)',
			dashStyle: 'solid',
			value: data.targetValues[1]
		}
		];
		paramsW.yAxis.plotBands = [{
			color: 'rgba(180,215,254,0.3)', 
			from: data.targetValues[0], 
			to: data.targetValues[1] 
		}],
		paramsW.subtitle = {
			align:'right', verticalAlign:'bottom', y:0,
			useHTML: true,
			text: '<div id="chartBPNote"><span class="bpUL">■正常値</span></div>'
		};
	
		chartSeries.push({
			name: data.title,
			color: '#EB5406',
			data: data.data
		});
	
		paramsW.series = chartSeries;
		$g.highcharts(paramsW);
	};
	
	//運動強度
	var drawGraph_mets = function ($g, dataName) {
		// UTC time
		
		var data = hrChartData_mets['data'];
		
		var chartType = data.chartType||'line';
		var chartSeries = [];
		var paramsW = {
			credits: { enabled:false },
			
			chart: {
				type: chartType,
				//zoomType: 'x'
			},
			title: { text:null },
			yAxis: {
				labels: {
					formatter: function() { return this.value +'Mets'; }
				},
				title: { text:null },
				min: data.min,
				max:data.max
			},
			xAxis: {
				title: { text:null },
				gridLineWidth: 1,
				type: 'category',
				categories:data.dayLabel,
				//dateTimeLabelFormats: labelFormats
			},
			tooltip: {
				valueSuffix: data.unit,
				//dateTimeLabelFormats: tpLabelFormats
			},
			plotOptions: {
				series: {
					connectNulls: false,
				}
			}
		};
		
		paramsW.yAxis.plotLines = [{
			width:2, color:'rgba(180,215,254,1)',
			dashStyle: 'solid',
			value: data.targetValues[0],
		},
		{
			width:2, color:'rgba(180,215,254,1)',
			dashStyle: 'solid',
			value: data.targetValues[1]
		}
		];
		paramsW.yAxis.plotBands = [{
			color: 'rgba(180,215,254,0.3)', 
			from: data.targetValues[0], 
			to: data.targetValues[1] 
		}],
		paramsW.subtitle = {
			align:'right', verticalAlign:'bottom', y:0,
			useHTML: true,
			text: '<div id="chartBPNote"><span class="bpUL">■目標値</span></div>'
		};
	
		chartSeries.push({
			name: data.title,
			color: '#EB5406',
			data: data.data
		});
	
		paramsW.series = chartSeries;
		$g.highcharts(paramsW);
	};
	
	
	
	//ノーマルの折れ線グラフはここに入る
	var drawGraph_normal = function ($g, dataName , chartName) {
		// UTC time
		
		var data = eval("hrChartData_" + chartName + "['data']");
		var dateN = eval("hrChartData_" + chartName);
		
		var startDate = new Date(dateN.startDate);
		var yy = startDate.getFullYear();
		var mm = startDate.getMonth();
		var dd = startDate.getDate();

		var chartType = data.chartType||'line';
		var chartType2 = data.chartType2;
		var chartSeries = [];
		var paramsW = {
			credits: { enabled:false },
			chart: {
				type: chartType,
				//zoomType: 'x'
			},
			title: { text:null },
			yAxis: {
				title: { text:null },
				min: data.min,
				max:data.max
			},
			xAxis: {
				title: { text:null },
				gridLineWidth: 1,
				type: 'category',
				//dateTimeLabelFormats: labelFormats
				tickInterval: 3
			},
			tooltip: {
				valueSuffix: data.unit,
				//dateTimeLabelFormats: tpLabelFormats
			},
			plotOptions: {
				series: {
					connectNulls: true,
				}
			}
		};

		paramsW.yAxis.plotLines = [{
			width:2, color:'rgba(180,215,254,1)',
			dashStyle: 'solid',
			value: data.targetValues[0],
		},
		{
			width:2, color:'rgba(180,215,254,1)',
			dashStyle: 'solid',
			value: data.targetValues[1]
		}
		];
		paramsW.yAxis.plotBands = [{
			color: 'rgba(180,215,254,0.3)', 
			from: data.targetValues[0], 
			to: data.targetValues[1] 
		}],
		paramsW.subtitle = {
			align:'right', verticalAlign:'bottom', y:0,
			useHTML: true,
			text: '<div id="chartBPNote"><span class="bpUL">■目標</span></div>'
		};
		chartSeries.push({
			name: data.title,
/*
			pointStart: Date.UTC(yy, mm, dd),
			pointInterval: 24 * 3600 * 1000,
*/
			color: '#EB5406',
			data: data.data
		});

		paramsW.series = chartSeries;
		$g.highcharts(paramsW);
	};
	
	
	$.fn.chartInit = function(options) {
		
		var settings = $.extend( {
			'chartName' : "bp"
		}, options);
		
		var chartName = settings['chartName']; 
		
		setTimeout(function () {
			$('.chart-draw-area').each(function () {
				var $box = $(this).parents('.chart-area');
				var ref = $box.data('ref');
				var chartType = $box.data('chart');
				if (ref != null){
					var $g = $('.chart-draw-area', $box);
					$box.data('draw', true);
					
					if(ref == 'bp' && chartName == 'bp'){
						drawGraph_bp($g, ref);
					}else if(ref == 'w' && chartName == 'w'){
						drawGraph_w($g, ref);
					}else if(ref == 'bgl' && chartName == 'bgl'){
						drawGraph_bgl($g, ref);
					}else if(ref == 'healthage' && chartName == 'healthage'){
						drawGraph_healthage($g, ref);
					}else if(ref == 'pr' && chartName == 'pr'){
						drawGraph_pr($g, ref);
					}else if(ref == 'mets' && chartName == 'mets'){
						drawGraph_mets($g, ref);
					}else if(ref == ref && chartName == 'all'){
						$('.chart-draw-area').chartInit({'chartName' : ref});
					}else{
						if(ref == chartName && chartName == chartName){
							drawGraph_normal($g, ref , chartName);
						}
					}
				}
			});
		}, 500);
		return this;
	}
})(jQuery);	


$(function () {
	$('.drawer-closed , .drawer-handle , .open-status').click(function(){
		$('.chart-draw-area').chartInit({'chartName' : 'all'});
	});
	
	var timer = false;
	$(window).on('resize', function(){
	    if (timer !== false) {
	        clearTimeout(timer);
	    }
	    timer = setTimeout(function() {
			$('.chart-draw-area').chartInit({'chartName' : 'all'});
	    }, 50);
	});
});






