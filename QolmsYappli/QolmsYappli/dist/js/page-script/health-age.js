; (function ($) {

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
            months: ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月'],
            shortMonths: ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月'],
            weekdays: ['日', '月', '火', '水', '木', '金', '土'],
            numericSymbols: null
        }
    });



    //分布

    var drawGraph_d = function ($g, dataName, chartName) {
        // UTC time
        var data = eval("hrChartData_" + chartName + "['data']");

        var chartType = data.chartType || 'column';
        var chartType2 = data.chartType2;
        var chartSeries = [];
        var paramsW = {
            credits: { enabled: false },

            chart: {
                type: chartType,
                //zoomType: 'x'
            },
            title: { text: null },
            yAxis: {
                min: data.min,
                max: data.max,
            },
            xAxis: {
                title: '',
                type: 'category',
                categories: data.dayLabel,
                tickInterval: 1,
                labels: {
                    rotation: 90,
                    padding: 1,
                }
            },
            legend: {
                enabled: false
            },
            tooltip: {
                enabled: false
            },
            plotOptions: {
                series: {
                    connectNulls: true,
                }
            }
        };

        switch (chartType2) {
            case 'distribution':

                paramsW.yAxis = [
                    {
                        labels: { enabled: false },
                        title: '',
                        min: data.min,
                        max: data.max
                    }
                ];
                chartSeries.push({
                    color: '#85b3fe',
                    pointWidth: 20,
                    data: data.data
                });

                break;
        }
        paramsW.series = chartSeries;
        $g.highcharts(paramsW);
    };

    //ネガティブがある棒グラフ（同性同年代との比較）
    var drawGraph_negativebar = function ($g, dataName, chartName) {
        // UTC time

        var data = eval("hrChartData_" + chartName + "['data']");

        var chartType = data.chartType || 'bar';
        var chartType2 = data.chartType2;
        var chartSeries = [];
        var paramsW = {
            credits: { enabled: false },
            chart: {
                type: chartType,
                //zoomType: 'x'
            },
            title: { text: null },
            yAxis: {
                min: data.min,
                max: data.max,
                title: '',
            },
            xAxis: {
                title: '',
                type: 'category',
                categories: data.dayLabel,
                //dateTimeLabelFormats: labelFormats
                tickInterval: 1,
                labels: {
                    style: {
                        fontSize: '12px'
                    }
                }
            },
            legend: {
                enabled: false
            },
            tooltip: {
                enabled: false
            },
            plotOptions: {
                series: {
                    connectNulls: true,
                }
            }
        };

        paramsW.yAxis.plotBands = [{
            //color: 'rgba(203,230,210,0.7)', 
            from: data.targetValues[0],
            to: data.targetValues[1]
        }, {
            //color: 'rgba(251,209,192,0.7)', 
            from: data.targetValues[2],
            to: data.targetValues[3]

        }],
		chartSeries.push({
		    name: data.title,
		    color: '#85b3fe',
		    negativeColor: '#85b3fe',
		    data: data.data
		});

        paramsW.series = chartSeries;
        $g.highcharts(paramsW);
    };


    //ノーマルの折れ線グラフはここに入る
    var drawGraph_normal = function ($g, dataName, chartName) {
        // UTC time

        var data = eval("hrChartData_" + chartName + "['data']");

        var chartType = data.chartType || 'line';
        var chartType2 = data.chartType2;
        var chartSeries = [];
        var paramsW = {
            credits: { enabled: false },
            chart: {
                type: chartType,
                //zoomType: 'x'
            },
            title: { text: null },
            yAxis: {
                min: data.min,
                max: data.max,
                title: '',
            },
            xAxis: {
                title: '',
                type: 'category',
                categories: data.dayLabel,
                //dateTimeLabelFormats: labelFormats
                tickInterval: 1
            },
            legend: {
                enabled: false
            },
            /*
                        tooltip: {
                            valueSuffix: data.unit,
                            dateTimeLabelFormats: tpLabelFormats
                        },
            */
            plotOptions: {
                series: {
                    connectNulls: true,
                }
            }
        };

        paramsW.yAxis.plotBands = [{
            color: 'rgba(203,230,210,0.7)',
            from: data.targetValues[0],
            to: data.targetValues[1]
        }, {
            color: 'rgba(250,243,192,0.7)',
            from: data.targetValues[2],
            to: data.targetValues[3]

        }, {
            color: 'rgba(250,243,192,0.7)',
            from: data.targetValues[4],
            to: data.targetValues[5]

        }, {
            color: 'rgba(251,209,192,0.7)',
            from: data.targetValues[6],
            to: data.targetValues[7]

        }, {
            color: 'rgba(251,209,192,0.7)',
            from: data.targetValues[8],
            to: data.targetValues[9]

        }],
		chartSeries.push({
		    name: data.title,
		    color: '#85b3fe',
		    data: data.data
		});

        paramsW.series = chartSeries;
        $g.highcharts(paramsW);
    };

    $.fn.chartInit = function (options) {

        var settings = $.extend({
            'chartName': ""
        }, options);

        var chartName = settings['chartName'];

        //window.alert(chartName);

        setTimeout(function () {
            $('.chart-area').each(function () {
                if ($(this).data("ref") == chartName) {
                    var $box = $(this);
                    var ref = $box.data('ref');
                    var kind = $box.data('kind');
                    var chartType = $box.data('chart');
                    if (ref != null) {
                        var $g = $('.chart-draw-area', $box);
                        $box.data('draw', true);

                        if (ref == 'distribution' && chartName == 'distribution') {
                            drawGraph_d($g, ref, chartName);
                        } else if (ref == ref && chartName == 'all') {
                            $('.chart-draw-area').chartInit({ 'chartName': ref });
                        } else {
                            if (kind == 'negativebar') {
                                drawGraph_negativebar($g, ref, ref);
                            } else if (ref == chartName) {
                                drawGraph_normal($g, ref, chartName);
                            }
                        }
                    }
                }
            });
        }, 500);

        return this;
    }
})(jQuery);

//; (function ($) {

//    var labelFormats = {
//        hour: ' ',
//        day: '%b%e日',
//        week: '%b%e日',
//        month: '%m月',
//        year: '%Y年'
//    };
//    var tpLabelFormats = {
//        day: '%Y年%b%e日 (%a)',
//        week: '%Y年%b%e日',
//        month: '%Y年%b',
//        year: '%Y年'
//    };

//    Highcharts.setOptions({
//        lang: {
//            resetZoom: '元の表示に戻す',
//            months: ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月'],
//            shortMonths: ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月'],
//            weekdays: ['日', '月', '火', '水', '木', '金', '土'],
//            numericSymbols: null
//        }
//    });



//    //分布

//    var drawGraph_d = function ($g, dataName, chartName) {
//        // UTC time
//        var data = eval("hrChartData_" + chartName + "['data']");

//        var chartType = data.chartType || 'column';
//        var chartType2 = data.chartType2;
//        var chartSeries = [];
//        var paramsW = {
//            credits: { enabled: false },

//            chart: {
//                type: chartType,
//                //zoomType: 'x'
//            },
//            title: { text: null },
//            yAxis: {
//                min: data.min,
//                max: data.max,
//            },
//            xAxis: {
//                title: '',
//                type: 'category',
//                categories: data.dayLabel,
//                tickInterval: 1,
//                labels: {
//                    rotation: 90,
//                    padding: 1,
//                }
//            },
//            legend: {
//                enabled: false
//            },
//            tooltip: {
//                enabled: false
//            },
//            plotOptions: {
//                series: {
//                    connectNulls: true,
//                }
//            }
//        };

//        switch (chartType2) {
//            case 'distribution':

//                paramsW.yAxis = [
//                    {
//                        labels: { enabled: false },
//                        title: '',
//                        min: data.min,
//                        max: data.max
//                    }
//                ];
//                chartSeries.push({
//                    color: '#85b3fe',
//                    pointWidth: 20,
//                    data: data.data
//                });

//                break;
//        }
//        paramsW.series = chartSeries;
//        $g.highcharts(paramsW);
//    };

//    //ネガティブがある棒グラフ（同性同年代との比較）
//    var drawGraph_negativebar = function ($g, dataName, chartName) {
//        // UTC time

//        var data = eval("hrChartData_" + chartName + "['data']");

//        var chartType = data.chartType || 'bar';
//        var chartType2 = data.chartType2;
//        var chartSeries = [];
//        var paramsW = {
//            credits: { enabled: false },
//            chart: {
//                type: chartType,
//                //zoomType: 'x'
//            },
//            title: { text: null },
//            yAxis: {
//                min: data.min,
//                max: data.max,
//                title: '',
//            },
//            xAxis: {
//                title: '',
//                type: 'category',
//                categories: data.dayLabel,
//                //dateTimeLabelFormats: labelFormats
//                tickInterval: 1,
//                labels: {
//                    style: {
//                        fontSize: '12px'
//                    }
//                }
//            },
//            legend: {
//                enabled: false
//            },
//            tooltip: {
//                enabled: false
//            },
//            plotOptions: {
//                series: {
//                    connectNulls: true,
//                }
//            }
//        };

//        paramsW.yAxis.plotBands = [{
//            color: 'rgba(203,230,210,0.7)',
//            from: data.targetValues[0],
//            to: data.targetValues[1]
//        }, {
//            color: 'rgba(251,209,192,0.7)',
//            from: data.targetValues[2],
//            to: data.targetValues[3]

//        }],
//		chartSeries.push({
//		    name: data.title,
//		    color: '#eb5505',
//		    negativeColor: '#85b3fe',
//		    data: data.data
//		});

//        paramsW.series = chartSeries;
//        $g.highcharts(paramsW);
//    };


//    //ノーマルの折れ線グラフはここに入る
//    var drawGraph_normal = function ($g, dataName, chartName) {
//        // UTC time

//        var data = eval("hrChartData_" + chartName + "['data']");

//        var chartType = data.chartType || 'line';
//        var chartType2 = data.chartType2;
//        var chartSeries = [];
//        var paramsW = {
//            credits: { enabled: false },
//            chart: {
//                type: chartType,
//                //zoomType: 'x'
//            },
//            title: { text: null },
//            yAxis: {
//                min: data.min,
//                max: data.max,
//                title: '',
//            },
//            xAxis: {
//                title: '',
//                type: 'category',
//                categories: data.dayLabel,
//                //dateTimeLabelFormats: labelFormats
//                tickInterval: 1
//            },
//            legend: {
//                enabled: false
//            },
//            /*
//                        tooltip: {
//                            valueSuffix: data.unit,
//                            dateTimeLabelFormats: tpLabelFormats
//                        },
//            */
//            plotOptions: {
//                series: {
//                    connectNulls: true,
//                }
//            }
//        };

//        paramsW.yAxis.plotBands = [{
//            color: 'rgba(203,230,210,0.7)',
//            from: data.targetValues[0],
//            to: data.targetValues[1]
//        }, {
//            color: 'rgba(250,243,192,0.7)',
//            from: data.targetValues[2],
//            to: data.targetValues[3]

//        }, {
//            color: 'rgba(250,243,192,0.7)',
//            from: data.targetValues[4],
//            to: data.targetValues[5]

//        }, {
//            color: 'rgba(251,209,192,0.7)',
//            from: data.targetValues[6],
//            to: data.targetValues[7]

//        }, {
//            color: 'rgba(251,209,192,0.7)',
//            from: data.targetValues[8],
//            to: data.targetValues[9]

//        }],
//		chartSeries.push({
//		    name: data.title,
//		    color: '#85b3fe',
//		    data: data.data
//		});

//        paramsW.series = chartSeries;
//        $g.highcharts(paramsW);
//    };

//    $.fn.chartInit = function (options) {

//        //console.log("init");

//        var settings = $.extend({
//            'chartName': ""
//        }, options);

//        var chartName = settings['chartName'];

//        //window.alert(chartName);

//        setTimeout(function () {
//            $('.chart-area').each(function () {
//                if ($(this).data("ref") == chartName) {
//                    var $box = $(this);
//                    var ref = $box.data('ref');
//                    var kind = $box.data('kind');
//                    var chartType = $box.data('chart');
//                    if (ref != null) {

//                        //console.log("draw");

//                        var $g = $('.chart-draw-area', $box);
//                        $box.data('draw', true);

//                        if (ref == 'distribution' && chartName == 'distribution') {
//                            drawGraph_d($g, ref, chartName);
//                        } else if (ref == ref && chartName == 'all') {
//                            $('.chart-draw-area').chartInit({ 'chartName': ref });
//                        } else {
//                            if (kind == 'negativebar') {
//                                drawGraph_negativebar($g, ref, ref);
//                            } else if (ref == chartName) {
//                                drawGraph_normal($g, ref, chartName);
//                            }
//                        }
//                    }
//                }
//            });
//        }, 500);

//        return this;
//    }
//})(jQuery);
