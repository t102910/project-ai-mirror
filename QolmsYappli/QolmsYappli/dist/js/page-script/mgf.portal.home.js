// mgf.portal 名前空間オブジェクト
mgf.portal.home = mgf.portal.home || {};
// DOM 構築完了
mgf.portal.home = (function () {
    //$(function () {

    //    //$('body').click(function(){
    //    //    gauge2.set(1200);

    //    //});

    //});


    //var opts2 = {
    //    lines: 12,
    //    angle: 0.22,
    //    lineWidth: 0.15,
    //    pointer: {
    //        length: 0.9,
    //        strokeWidth: 0.035,
    //        color: '#000000'
    //    },
    //    limitMax: 'false',
    //    colorStart: '#f58585',
    //    colorStop: '#f58585',
    //    strokeColor: '#EEEEEE',
    //    generateGradient: false
    //};

    //var opts3 = {
    //    lines: 12,
    //    angle: 0.22,
    //    lineWidth: 0.15,
    //    pointer: {
    //        length: 0.9,
    //        strokeWidth: 0.035,
    //        color: '#000000'
    //    },
    //    limitMax: 'false',
    //    colorStart: '#7b8af7',
    //    colorStop: '#7b8af7',
    //    strokeColor: '#EEEEEE',
    //    generateGradient: false
    //};

    //

    var today = new Date();

    donutReload();

    // ajzx処理の変数
    var jqxhrGetNews;
    var jqxhrGetToday;
    var jqxhrGetAlkoo;

    //ajax処理の変数を保持
    var jqxhrArray = [];

    //gauge.setTextField(document.getElementById("preview-textfield"));
    //文字をアニメーションさせる為に必要（暫定で消去）

    //donutReload();

    // 左右ボタンOR デイリーマンスリー OR Today が押された時
    function buttonClicked(lr) {

        // 画面をロック
        mgf.lockScreen();

        var lrstr = "";
        if (lr == "Right") {
            lrstr = "../Portal/HomeResult?ActionSource=Right"
        } else if (lr == "Left") {
            lrstr = "../Portal/HomeResult?ActionSource=Left"
        } else if (lr == "Monthly") {
            lrstr = "../Portal/HomeResult?ActionSource=Monthly"
        } else if (lr == "Weekly") {
            lrstr = "../Portal/HomeResult?ActionSource=Weekly"
        } else {
            lrstr = "../Portal/HomeResult?ActionSource=Today"
        }

        //////console.log($("#showday").data("now-day"));
        // 現在日（OR月）とモードを取得してページ更新
        $.ajax({
            type: "POST",
            url: lrstr,
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                Monthly: $("#monthly").hasClass("current"),
                NowDay: $("#showday").data("now-day")
            },
            async: true,
            beforeSend: function (jqXHR) {
                // セッションのチェック
                mgf.portal.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("text/html;") >= 0) {

                    //画面取得成功
                    //パーシャルビューを書き換え
                    $(document).find("#update-area").replaceWith($(data))
                    GetTasks();
                    donutReload();

                    // 画面をアンロック
                    mgf.unlockScreen();

                } else {

                    // 画面をアンロック
                    mgf.unlockScreen();
                }
            } catch (ex) { }
        }).always(function (jqXHR, textStatus) {
            // 画面をアンロック
            mgf.unlockScreen();
        });

    };

    function ChallengeTargetData(targetDay) {

        // 画面をロック
        mgf.lockScreen();

        var lrstr = "../Portal/HomeResult?ActionSource=ChallengeTargetData"

        $.ajax({
            type: "POST",
            url: lrstr,
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                targetDay: targetDay
            },
            async: true,
            beforeSend: function (jqXHR) {
                // セッションのチェック
                mgf.portal.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("text/html;") >= 0) {

                    //画面取得成功
                    //パーシャルビューを書き換え
                    $(document).find("#update-area").replaceWith($(data))
                    GetTasks();
                    donutReload();

                    // 画面をアンロック
                    mgf.unlockScreen();

                } else {

                    // 画面をアンロック
                    mgf.unlockScreen();
                }
            } catch (ex) { }
        }).always(function (jqXHR, textStatus) {
            // 画面をアンロック
            mgf.unlockScreen();
        });

    };

    function GetTasks() {

        ////console.log("GetTasks")
        ////console.log($("#showday").data("now-day"))
        // 画面をロック
        mgf.lockScreen();

        $.ajax({
            type: "POST",
            url: "../Portal/HomeResult?ActionSource=Task",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                showDay: $("#showday").data("now-day")
            },
            async: true
            //beforeSend: function (jqXHR) {
            //    mgf.portal.checkSessionByAjax();
            //}
        }).done(function (data, textStatus, jqXHR) {
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {

                    $("#entered-steps").addClass($.parseJSON(data)["Steps"])
                    $("#entered-weight").addClass($.parseJSON(data)["Weight"])
                    $("#entered-breakfast").addClass($.parseJSON(data)["Breakfast"])
                    $("#entered-lunch").addClass($.parseJSON(data)["Lunch"])
                    $("#entered-dinner").addClass($.parseJSON(data)["Dinner"])
                    $("#entered-column").addClass($.parseJSON(data)["Column"])
                    // 画面をアンロック
                    mgf.unlockScreen();
                }
                else {
                    //////console.log("else")

                }
            } catch (ex) {

            }
        }).always(function (jqXHR, textStatus) {
            // 画面をアンロック
            mgf.unlockScreen();
        });

    };


    // ドーナツゲージの書き換え処理
    function donutReload() {

        try {

            var color = ''
            if ($("#preview-textfield").data("more") >= 0) {
                if ($("body").hasClass("biz-member")) {
                    // 青(biz)
                    color = '#004ec0'
                }
                else {
                    // オレンジ
                    color = '#ff7800'
                }
                $("body").removeClass("over")
            }
            else {
                // 赤(over)
                color = '#e60012'
                $("body").addClass("over")
            }
            var opts = {
                lines: 12,
                angle: 0.22,
                lineWidth: 0.1,
                pointer: {
                    length: 0.9,
                    strokeWidth: 0.035,
                    color: '#000000'
                },
                limitMax: 'false',
                colorStart: color,
                colorStop: color,
                strokeColor: '#EEEEEE',
                generateGradient: false
            };

            var target = document.getElementById('canvas-preview');
            var gauge = new Donut(target).setOptions(opts);
            var valueP = parseInt($("#preview-textfield").data("gaugeval"));
            var max = parseInt($("#preview-textfield").data("gaugemax"));
            gauge.maxValue = max;
            gauge.animationSpeed = 32;
            gauge.set(valueP);

            //var target2 = document.getElementById('canvas-preview2');
            //var gauge2 = new Donut(target2).setOptions(opts2);
            //var valueIn = parseInt(document.getElementById("preview-textfield2").innerText);
            //var maxValueIn = parseInt($(document).find('#targetIn').data('num'));
            //if (valueIn >= maxValueIn) {

            //    maxValueIn = valueIn
            //}
            //gauge2.maxValue = maxValueIn;
            //gauge2.animationSpeed = 32;
            //gauge2.set(valueIn);
            //gauge2.setTextField(document.getElementById("preview-textfield2"));


            //var target3 = document.getElementById('canvas-preview3');
            //var gauge3 = new Donut(target3).setOptions(opts3);
            //var valueOut = parseInt(document.getElementById("preview-textfield3").innerText);
            //var maxValueOut = parseInt($(document).find('#targetOut').data('num'));
            //if (valueOut >= maxValueOut) {
            //    maxValueOut = valueOut
            //}
            //gauge3.maxValue = maxValueOut;
            //gauge3.animationSpeed = 32;
            //gauge3.set(valueOut);
            //gauge3.setTextField(document.getElementById("preview-textfield3"));

        } catch (e) {

        }

    };

    function getNews() {

        jqxhrGetNews = $.ajax({
            type: "POST",
            url: "../Portal/HomeResult?ActionSource=News",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val()
            },
            async: true
            //beforeSend: function (jqXHR) {
            //    mgf.portal.checkSessionByAjax();
            //}
        }).done(function (data, textStatus, jqXHR) {
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {
                    //成功した時の表示
                    //console.log("success")
                    //console.log($.parseJSON(data)["News"])

                    var news = $.parseJSON(data)["News"]

                    news.forEach(function (value, key) {
                        //////console.log(value)
                        $(".news-area ul").append('<li class="news-item"><span>' + value + '</span><li>');
                    });

                    NewsAnim();

                }
                else {
                    //console.log("else")
                }
            } catch (ex) {

            }
        }).always(function (jqXHR, textStatus) {
            // 画面をアンロック
        });

        jqxhrArray.push(jqxhrGetNews);

    };

    function getToday() {

        lrstr = "../Portal/HomeResult?ActionSource=Today"

        var date = $("#showday").data("now-day");
        //console.log(date);
        if (date == undefined || date == null) {

            date = $.toYmdDateString(new date());
            //console.log(date)
        }

        //.todayに移動

        if ($("#challenge-area").length > 0 && !$("#challenge-area").hasClass("hide")) {

            $selectDate(today);
        }

        // 現在日（OR月）とモードを取得してページ更新
        jqxhrGetToday = $.ajax({
            type: "POST",
            url: lrstr,
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                Monthly: $("#monthly").hasClass("current"),
                NowDay: date
            },
            async: true
            //beforeSend: function (jqXHR) {
            //    // セッションのチェック
            //    mgf.portal.checkSessionByAjax();
            //}
        }).done(function (data, textStatus, jqXHR) {
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("text/html;") >= 0) {

                    //画面取得成功
                    //パーシャルビューを書き換え
                    $(document).find("#update-area").replaceWith($(data))
                    GetTasks();
                    donutReload();

                } else {

                }
            } catch (ex) { }
        }).always(function (jqXHR, textStatus) {
            // 画面をアンロック
        });
        jqxhrArray.push(jqxhrGetToday);

    };

    //選択している日付の年月日をArrayで返します
    function getSelectedDateString() {

        var returnStr = ""
        if ($("#showday").length > 0) {
            var dateStr = $("#showday").data("now-day");
            var arr = dateStr.split("/");
            returnStr = arr[0] + arr[1] + arr[2]
        }
        console.log(returnStr);

        return returnStr
    }

    //$("main").on("click", "#daily", function () {

    //    $("#data-period li.current").removeClass("current")
    //    $("#daily").addClass("current");
    //    buttonClicked("Daily");
    //    return false;

    //});

    //$("main").on("click", "#weekly", function () {

    //    $("#data-period li.current").removeClass("current")
    //    $("#weekly").addClass("current")
    //    buttonClicked("Weekly");
    //    return false;

    //});

    //$("main").on("click", "#monthly", function () {

    //    $("#data-period li.current").removeClass("current")
    //    $("#monthly").addClass("current")
    //    buttonClicked("Monthly");
    //    return false;

    //});
    // デイリー、マンスリーが切り替わった時
    //$("main").on("change", "input[name='monthly']", function () {

    //    buttonClicked("Monthly");
    //    return false;

    //});

    // 左向きボタン
    $("main").on("click", "#leftbutton", function () {

        ////console.log("left");
        if ($("#challenge-area").length > 0 && !$("#challenge-area").hasClass("hide")) {
            //console.log("challenge");
            var selectedDate = $("#challenge-area").data("target-day");
            var alldays = $("#challenge-area").data("all-days");
            var targetDate = selectedDate - 1;

            if (0 < targetDate && targetDate < alldays) {
                //チャレンジ期間内
                $selectDate(targetDate);

                if ($("#challenge-area").hasClass("hide")) {
                    $(".news-area").removeClass("hide");
                    $("#challenge-area").addClass("hide");

                }
            } else {
                if ($(".news-area").hasClass("hide")) {
                    $(".news-area").removeClass("hide");
                    $("#challenge-area").addClass("hide");

                    buttonClicked("Left");

                }
                $("#challenge-area").data("target-day", targetDate)

            }

        } else {
            //console.log("not-challenge");

            if ($("#challenge-area").length > 0) {
                var showday = $("#showday").data("now-day");
                var challenge_start_day = $("#challenge-area").data("start-day")
                var sp1 = showday.split('/');

                var dt = new Date(parseInt(sp1[0], 10), parseInt(sp1[1], 10) - 1, parseInt(sp1[2], 10));
                dt = new Date(dt.setDate(dt.getDate() - 1));

                var nextday = dt.getFullYear() + "/" + ('00' + (dt.getMonth() + 1)).slice(-2) + "/" + ('00' + (dt.getDate())).slice(-2)

                //console.log(challenge_start_day);
                //console.log(nextday);
                //console.log(challenge_start_day == nextday);

                if (challenge_start_day == nextday) {
                    $(".news-area").removeClass("hide");
                    $("#challenge-area").addClass("hide");

                    var alldays = $("#challenge-area").data("all-days");
                    $("#challenge-area").data("target-day", alldays)
                    $selectDate(alldays);

                }
            }
            buttonClicked("Left");

        }
        return false;

    });

    // 右向きボタン
    $("main").on("click", "#rightbutton", function () {


        if ($("#challenge-area").length > 0 && !$("#challenge-area").hasClass("hide")) {
            //console.log("challenge");
            var selectedDate = $("#challenge-area").data("target-day");
            var alldays = $("#challenge-area").data("all-days");
            var targetDate = selectedDate + 1;

            if (0 < targetDate && targetDate < alldays) {
                //チャレンジ期間内
                $selectDate(targetDate);
                if ($("#challenge-area").hasClass("hide")) {
                    $(".news-area").addClass("hide");
                    $("#challenge-area").removeClass("hide");

                }
            } else {
                if ($(".news-area").hasClass("hide")) {
                    $(".news-area").removeClass("hide");
                    $("#challenge-area").addClass("hide");

                    buttonClicked("Right");
                }

                $("#challenge-area").data("target-day", targetDate)

            }
        } else {

            //console.log("not-challenge");

            if ($("#challenge-area").length > 0) {
                var showday = $("#showday").data("now-day");
                var challenge_start_day = $("#challenge-area").data("start-day")
                var sp1 = showday.split('/');

                var dt = new Date(parseInt(sp1[0], 10), parseInt(sp1[1], 10) - 1, parseInt(sp1[2], 10));
                dt = new Date(dt.setDate(dt.getDate() + 1));

                var nextday = dt.getFullYear() + "/" + ('00' + (dt.getMonth() + 1)).slice(-2) + "/" + ('00' + (dt.getDate())).slice(-2)

                //console.log(challenge_start_day);
                //console.log( nextday);
                //console.log(challenge_start_day == nextday);

                if (challenge_start_day == nextday) {
                    $(".news-area").addClass("hide");
                    $("#challenge-area").removeClass("hide");

                    $("#challenge-area").data("target-day", 1)
                    $selectDate(1);
                }
            }
            buttonClicked("Right");

        }

        return false;
    });

    //// Todayボタン
    //$("main").on("click", "#todayB", function () {

    //    buttonClicked("Today");
    //    return false;

    //});

    // 「更新」ボタン
    $(document).on("click", "#reload-btn a.btn", function () {
        // 画面をロック後、遷移
        mgf.portal.promiseLockScreen($(this))
            .then(function () {
                $(location).attr("href", "../Portal/Home");
            });

        return false;
    });

    function GetAlkoo() {
        // 「ALKOO連携」歩数の更新
        if ($("#alkoo").hasClass("true")) {

            jqxhrGetAlkoo = $.ajax({
                type: "POST",
                url: "../Portal/HomeResult?ActionSource=Alkoo",
                data: {
                    __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val()
                },
                async: true
                //beforeSend: function (jqXHR) {
                //    mgf.portal.checkSessionByAjax();
                //}
            }).done(function (data, textStatus, jqXHR) {
                try {
                    if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {
                        //成功した時の表示
                        if ($.parseJSON(data)["Steps"] > 0) {
                            var steps = $.parseJSON(data)["Steps"].replace(/(\d)(?=(\d\d\d)+$)/g, "$1,");
                            //////console.log(steps)
                            $("#steps-data").replaceWith('<td id="steps-data">' + steps + '<i class="unit">歩</i></td>')
                            $("#steps-input").replaceWith('<span id="steps-input" class="steps-input din">' + steps + '</span>')
                        }
                    }
                    else {
                        //////console.log("else")
                        return false;

                    }
                } catch (ex) {
                    return false;

                }
            }).always(function (jqXHR, textStatus) {

            });
            jqxhrArray.push(jqxhrGetAlkoo);
        }
    };
    // 詳細Dataエリア表示切替
    $("main").on("click", "#cal-drower", function () {

        if ($(this).hasClass('on')) {
            $('#table').slideUp();
            $(this).removeClass('on');
        } else {
            $('#table').slideDown();
            $(this).addClass('on');
        }
        return false;

    });

    // 「ポイント」、「歩数」アイコン からの遷移
    $("main").on("click", "#points-area p.point-exchange-btn-wrap a", function () {
        // 画面をロック後、遷移
        console.log(this)
        mgf.portal.promiseLockScreen($(this))
            .then(function () {
                // Ajaxリクエストのキャンセル
                cancelAjaxRequest();

                if (this.hasClass("steps-input")) {

                    var dateStr = getSelectedDateString();

                    $(location).attr("href", $(this).attr("href") + "?selectdate=" + dateStr);
                } else {
                    $(location).attr("href", $(this).attr("href"));

                }

            });

        return false;
    });

    // 「食事」、「運動」アイコン からの遷移
    $("main").on("click", "#cal-data a", function () {
        // 画面をロック後、遷移

        mgf.portal.promiseLockScreen($(this))
            .then(function () {
                // Ajaxリクエストのキャンセル
                cancelAjaxRequest();

                var dateStr = getSelectedDateString();

                $(location).attr("href", $(this).attr("href") + "?selectdate=" + dateStr);
            });

        return false;
    });
    // 「体重」「血圧」「血糖値」アイコン からの遷移
    $("main").on("click", "#menu-nav a", function () {
        // 画面をロック後、遷移

        mgf.portal.promiseLockScreen($(this))
            .then(function () {

                // Ajaxリクエストのキャンセル
                cancelAjaxRequest();
                var dateStr = getSelectedDateString();

                $(location).attr("href", $(this).attr("href") + "&selectdate=" + dateStr);
            });

        return false;
    });


    // 「健康診断」アイコン からの遷移
    $("main").on("click", "#examination", function () {
        // 非同期で連携があるかを確認してから遷移
        mgf.lockScreen();

        $.ajax({
            type: "POST",
            url: "../Portal/HomeResult?ActionSource=Examination",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val()
            },
            async: true,
            beforeSend: function (jqXHR) {
                mgf.portal.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {
                    //成功した時の表示

                    // Ajaxリクエストのキャンセル
                    cancelAjaxRequest();

                    location.href = "../note/Examination"
                    mgf.unlockScreen();
                }
                else {
                    $("#message-modal .modal-title").text("お知らせ")
                    $("#message-modal .modal-body").text("医療機関連携がありません。医療機関連携から連携を開始してください。")
                    $("#message-modal").modal("show");
                    mgf.unlockScreen();
                    return false;
                }
            } catch (ex) {
                mgf.unlockScreen();
                return false;

            }
        }).fail(function (data, textStatus, jqXHR) {

            ////console.log("fail")
            mgf.unlockScreen();

        }).always(function (jqXHR, textStatus) {

        });

        return false;
    });

    //チャレンジ経過日数UI
    var inViewwidth = $('#select-day-ui').innerWidth(); //ステージのwidth
    var inViewItem = Math.ceil(inViewwidth / 60, 10); //見えているitemの個数
    var paddingItem = parseInt(inViewItem / 2, 10); //1日目と最終日を中央にするためのpaddingItemの追加個数
    var itemIndex = $('.today').index('#select-day-ui li'); //.todayのインデックス

    //日付の移動
    //今日に移動させる場合　$selectDate(today);
    //任意の日に移動させる場合　$selectDate(任意の日);
    function $selectDate(targetDate) {
        //console.log("$selectDate:" + targetDate);
        if (targetDate == today) {  //今日に移動する
            var todayPos = itemIndex + paddingItem - inViewItem / 2 + 1;

            $('#select-day-ui ul').scrollLeft(0).animate({
                scrollLeft: 60 * todayPos
            }, 500, 'linear');

            //$('#select-day-ui ul li').removeClass('selected').eq(Math.round(todayPos) + paddingItem-1).addClass('selected')

        } else {

            $('#select-day-ui ul').animate({
                scrollLeft: 60 * (targetDate) -30
            }, 500, 'linear');

        }
    }

    //ロード時にリスト前後にpaddingItem追加
    for (var i = 0; i < paddingItem; i++) {
        $('#select-day-ui ul').append('<li class="padding"></li>').prepend('<li class="padding"></li>');
    }

    //スクロールがストップした時の挙動
    $('#select-day-ui ul').on("scrollstop", function () {
        var scrollLeft = $(this).scrollLeft(); //現在のスクロール量
        var currentPos = parseInt((scrollLeft) / 60, 10); //現在中央にあるitemのインデックス
        var currentItem = parseInt(inViewItem / 2 + currentPos); //paddingItemを含めたインデックス

        //scrollStopしたとき、中央のitemにselectedをつける
        $.when(
            $('#select-day-ui ul li').removeClass('selected').eq(currentItem).addClass('selected')
        ).done(function () {

        });

        if ($('.off').hasClass('selected')) {

            var todayPos = itemIndex + paddingItem - inViewItem / 2 + 1;
            //console.log("off,todayPos:" + todayPos);
            $('#select-day-ui ul').animate({
                scrollLeft: 60 * (todayPos)-30
            }, 500, 'linear');

        }
        else {
            //ここにカロリー情報などを取得して表示する処理を書きます
            var targetDate = $('.selected').data('date');
            if ($("#challenge-area").data("target-day") != targetDate) {
                //console.log("update:"+targetDate);
                $("#challenge-area").data("target-day", targetDate);
                ChallengeTargetData(targetDate);
            }
        }

    });

    //スクロールストップイベントをバインド
    $(function () {
        var scrollStopEvent = new $.Event("scrollstop");
        var delay = 200;
        var timer;
        //console.log("scroll Stop")
        function scrollStopEventTrigger() {
            if (timer) {
                clearTimeout(timer);
            }
            timer = setTimeout(function () { $('#select-day-ui ul').trigger(scrollStopEvent) }, delay);
        }
        $('#select-day-ui ul').on("scroll", scrollStopEventTrigger);
        $('#select-day-ui ul').on("touchmove", scrollStopEventTrigger);
    });

    // 目標のリストに各ページをリンク
    $("main").on("click", "#entered-steps", function () {

        mgf.portal.promiseLockScreen($(this))
            .then(function () {

                // Ajaxリクエストのキャンセル
                cancelAjaxRequest();
                var dateStr = getSelectedDateString();

                $(location).attr("href", "../note/walk?selectdate=" + dateStr);
            });

        return false;
    });

    $("main").on("click", "#entered-weight", function () {

        mgf.portal.promiseLockScreen($(this))
             .then(function () {

                 // Ajaxリクエストのキャンセル
                 cancelAjaxRequest();
                 var dateStr = getSelectedDateString();

                 $(location).attr("href", "../note/vital?tabno=1&selectdate=" + dateStr);
             });

        return false;
    });

    $("main").on("click", "#entered-breakfast", function () {

        mgf.portal.promiseLockScreen($(this))
              .then(function () {

                  // Ajaxリクエストのキャンセル
                  cancelAjaxRequest();
                  var dateStr = getSelectedDateString();

                  $(location).attr("href", "../note/calomeal?meal=1&selectdate=" + dateStr);
              });
        return false;

    });
    $("main").on("click", "#entered-lunch", function () {

        mgf.portal.promiseLockScreen($(this))
          .then(function () {

              // Ajaxリクエストのキャンセル
              cancelAjaxRequest();
              var dateStr = getSelectedDateString();

              $(location).attr("href", "../note/calomeal?meal=2&selectdate=" + dateStr);
          });
        return false;
    });
    $("main").on("click", "#entered-dinner", function () {

        mgf.portal.promiseLockScreen($(this))
          .then(function () {

              // Ajaxリクエストのキャンセル
              cancelAjaxRequest();
              var dateStr = getSelectedDateString();

              $(location).attr("href", "../note/calomeal?meal=3&selectdate=" + dateStr);
          });
        return false;
    });
    $("main").on("click", "#entered-column", function () {

        if (!$("#entered-column").hasClass("disabled")) {

            var targetDate = $('.selected').data('date');
            var key = $("#entered-column").data("key");

            mgf.portal.promiseLockScreen($(this))
             .then(function () {
                 // Ajaxリクエストのキャンセル
                 cancelAjaxRequest();
                 $(location).attr("href", "../portal/ChallengeColumn?key=" + key + "&targetday=" + targetDate + "&frompageno=1");
             });
            return false;
        }

    });

    $("main").on("click", "a.loading-link", function () {

        mgf.portal.promiseLockScreen($(this))
         .then(function () {
             // Ajaxリクエストのキャンセル
             cancelAjaxRequest();
             var href = $(this).attr("href");
             $(location).attr("href", href);
         });
        return false;

    });

    $("main").on("click", ".get-url", function () {

        var url_type = $(this).data("url-type");
        var url = "";
        // 画面をロック
        mgf.lockScreen();

        $.ajax({
            type: "POST",
            url: "../Portal/HomeResult?ActionSource=Url",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                urlType: url_type
            },
            async: true
            //beforeSend: function (jqXHR) {
            //    mgf.portal.checkSessionByAjax();
            //}
        }).done(function (data, textStatus, jqXHR) {
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0) {

                    console.log($.parseJSON(data)["IsSuccess"]);
                    if ($.isTrueString($.parseJSON(data)["IsSuccess"])) {

                        // Ajaxリクエストのキャンセル
                        cancelAjaxRequest();

                        url = $.parseJSON(data)["Url"];
                        prm = $.parseJSON(data)["Param"];

                        //console.log(url);
                        //console.log(prm);

                        $("<form>", {
                            action: url,
                            method: "POST"
                        }).appendTo(document.body);

                        $(prm).each(function (key, value) {

                            //console.log(key);
                            //console.log(value);
                            $("<input>").attr({
                                type: "hidden",
                                name: value["Key"],
                                value: value["Value"]
                            }).appendTo($("form:last"));
                        })

                        // サブミット
                        $("form:last").submit();

                    } else {

                        $("#message-modal .modal-title").text("お知らせ")
                        $("#message-modal .modal-body").text("タニタヘルスプラネットの連携をしてください。")
                        $("#message-modal").modal("show");
                        return false;
                    }

                    // 画面をアンロック
                    mgf.unlockScreen();
                }
                else {
                }
            } catch (ex) {

            }
        }).always(function (jqXHR, textStatus) {
            // 画面をアンロック
            mgf.unlockScreen();
        });

    });

    //問診
    $("main").on("click", "#monshin", function () {
        // 非同期で連携があるかを確認してから遷移
        mgf.lockScreen();

        $.ajax({
            type: "POST",
            url: "../Portal/MonshinUrl",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val()
            },
            async: true,
            beforeSend: function (jqXHR) {
                mgf.portal.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {
                    //成功した時の表示

                    var url = $.parseJSON(data)["Url"];

                    location.href = url;
                    
                    // Ajaxリクエストのキャンセル
                    cancelAjaxRequest();

                    // location.href = "native:/action/open_browser?url=" + url
                    mgf.unlockScreen();
                }
                else {
                    //$("#message-modal .modal-title").text("お知らせ")
                    //$("#message-modal .modal-body").text("医療機関連携がありません。医療機関連携から連携を開始してください。")
                    //$("#message-modal").modal("show");
                    mgf.unlockScreen();
                    return false;
                }
            } catch (ex) {
                mgf.unlockScreen();
                return false;

            }
        }).fail(function (data, textStatus, jqXHR) {

            ////console.log("fail")
            mgf.unlockScreen();

        }).always(function (jqXHR, textStatus) {

        });

        return false;
    });



    // Ajaxリクエストのキャンセル
    cancelAjaxRequest = function () {

        // todo:タイマーがあったらここでとめる
        //if (mgf.timerIdSetNoticeUnreadCount) {
        //    clearTimeout(mgf.timerIdSetNoticeUnreadCount);
        //}

        $.each(jqxhrArray, function (index, value) {
            console.log(index)
            console.log(value)
            value.abort();

        })
    }

    // ヒストリー バック禁止
    mgf.prohibitHistoryBack();

    //セッションのチェック
    mgf.portal.checkSessionByAjax();

    if ($("#update-area").hasClass("today-update")) {
        getToday();
        getNews();
        GetAlkoo();
    }

    //$selectDate(3);
    //console.log("ready");
    if ($('#ai-modal').length > 0) {
        $('#ai-modal').modal('show');
    }
})();

$(window).on('load', function () {
    $('#federation-modal').modal('show');
    //メニューのスライダーをイニシャライズ
    //メニューのスライダー
    setInterval(function () {
        var windowWidth = $(window).width();
        var sliderWidth = $('#menu-slider').width();
        var restCount = $('.scroll-inner').data('count');
        var restWidth = Math.abs(Math.ceil((windowWidth - sliderWidth) / 90)) + 1;
        if (windowWidth < sliderWidth && $('.localbtn__wrap').hasClass('move')) {
            if (restWidth >= restCount) {
                $('.scroll-inner').animate({
                    scrollLeft: 90 * restCount
                });
                $('.scroll-inner').data('count', restCount + 1);
            } else {
                $('.scroll-inner').animate({
                    scrollLeft: 0
                });
                $('.scroll-inner').data('count', 0);
            }
        }
    }, 3000);
    $('.localbtn__wrap').on('touchstart click', function () {
        $(this).removeClass('move');
    });

    //////console.log("load");
});
