// mgf.note.heartrate 名前空間オブジェクト
mgf.note.heartrate = mgf.note.heartrate || {};

// DOM 構築完了
mgf.note.heartrate = (function () {

    // 未来日付への遷移コントロール制御
    function toggleNextDate() {

        var type = $('.period-selecter li.on').data("type");

        var stopDate = new Date();
        stopDate = new Date(stopDate.getFullYear(), stopDate.getMonth(), stopDate.getDate());

        var nextDate = new Date($('section.date-selector').data("end_date"));

        if (type == "OneDay") {
            nextDate.setDate(nextDate.getDate() + 1);
            // 未来の日付には遷移できない。
            if (nextDate > stopDate) {
                $('#nextDate').addClass("hide");
            }
        } else if (type == "OneWeek") {
            nextDate.setDate(nextDate.getDate() + 7);
            // 土曜日
            while (stopDate.getDay() != 6) {
                stopDate.setDate(stopDate.getDate() + 1);
            }
            // 対象日を含む週以降には遷移できない。
            if (nextDate > stopDate) {
                $('#nextDate').addClass("hide");
            }
        } else if (type == "OneMonth") {
            // 月単位にページングさせる場合
            // end_date は常に月の末日である前提なので、確実に次月を指すために、27 月の最小日数より小さい値
            nextDate.setDate(nextDate.getDate() + 27);
            var month = nextDate.getMonth();
            // 次月まで送る
            while (nextDate.getMonth() == month) {
                nextDate.setDate(nextDate.getDate() + 1);
            }
            // 月末
            nextDate.setDate(nextDate.getDate() - 1);

            month = stopDate.getMonth();
            // 来月まで送る
            while (stopDate.getMonth() == month) {
                stopDate.setDate(stopDate.getDate() + 1);
            }
            // 今月末
            stopDate.setDate(stopDate.getDate() - 1);
            // 対象日を含む月以降には遷移できない。
            if (nextDate > stopDate) {
                $('#nextDate').addClass("hide");
            }
        } else if (type == "ThreeMonths") {
            nextDate.setDate(nextDate.getDate() + (27 * 3));
            var month = nextDate.getMonth();
            // 次月まで送る
            while (nextDate.getMonth() == month) {
                nextDate.setDate(nextDate.getDate() + 1);
            }
            // 月末
            nextDate.setDate(nextDate.getDate() - 1);

            month = stopDate.getMonth();
            // 来月まで送る
            while (stopDate.getMonth() == month) {
                stopDate.setDate(stopDate.getDate() + 1);
            }
            // 今月末
            stopDate.setDate(stopDate.getDate() - 1);
            // 対象日を含む月以降には遷移できない。
            if (nextDate > stopDate) {
                $('#nextDate').addClass("hide");
            }
        }
    }

    // ホームへ
    $("main").on("click", "a.home-btn", function () {

        // 画面をロック
        mgf.note.promiseLockScreen($(this))
         .then(function () {
             location.href = "../Portal/Home"
         });
        return false;

    })

    // 期間指定のボタンUI
    $("main").on("click", ".period-selecter li", function () {
        if (!$(this).hasClass('on')) {
            $(this).addClass('on').siblings('li').removeClass('on');

            // 画面をロック後、遷移
            mgf.note.promiseLockScreen($(this))
                .then(function () {

                    var dateStr = $('section.date-selector').data("end_date");
                    var arr = dateStr.split("/");
                    var paramStr = arr[0] + arr[1] + arr[2]

                    $(location).attr("href", "../Note/HeartRate" +
                        "?selectdate=" + paramStr +
                        "&periodType=" + $(this).data("type") +
                        "&initDate=True");
                });
        }
        return false;
    });

    // 前へ矢印ボタン
    $("main").on("click", "#prevDate", function () {

        var type = $('.period-selecter li.on').data("type");
        var endDate = new Date($('section.date-selector').data("end_date"));

        if (type == "OneDay") {
            endDate.setDate(endDate.getDate() - 1);
        } else if (type == "OneWeek") {
            endDate.setDate(endDate.getDate() - 7);
        } else if (type == "OneMonth") {
            // 月単位にページングさせる場合 コントローラ側で正規化しています。
            // endDate は常に月の末日である前提なので、確実に前月を指すために、31
            endDate.setDate(endDate.getDate() - 31);
        } else if (type == "ThreeMonths") {
            endDate.setDate(endDate.getDate() - (31 * 3));
        }

        // 画面をロック後、遷移
        mgf.note.promiseLockScreen($(this))
            .then(function () {
                
                var dateStr = $.toYmdDateString(endDate);
                var arr = dateStr.split("/");
                var paramStr = arr[0] + arr[1] + arr[2]

                $(location).attr("href", "../Note/HeartRate" +
                    "?selectdate=" + paramStr +
                    "&periodType=" + type + 
                    "&initDate=False");
            });
    });

    // 次へ矢印ボタン
    $("main").on("click", "#nextDate", function () {

        var type = $('.period-selecter li.on').data("type");
        var endDate = new Date($('section.date-selector').data("end_date"));
        var todayDate = new Date();
        todayDate = new Date(todayDate.getFullYear(), todayDate.getMonth(), todayDate.getDate());

        if (type == "OneDay") {
            endDate.setDate(endDate.getDate() + 1);
        } else if (type == "OneWeek") {
            endDate.setDate(endDate.getDate() + 7);
        } else if (type == "OneMonth") {
            // 月単位にページングさせる場合 コントローラ側で正規化しています。
            // endDate は常に月の末日である前提なので、確実に次月を指すために、27 月の最小日数より小さい値
            endDate.setDate(endDate.getDate() + 27);
        } else if (type == "ThreeMonths") {
            endDate.setDate(endDate.getDate() + (27 * 3));
        }

        // 画面をロック後、遷移
        mgf.note.promiseLockScreen($(this))
            .then(function () {

                var dateStr = $.toYmdDateString(endDate);
                var arr = dateStr.split("/");
                var paramStr = arr[0] + arr[1] + arr[2]

                $(location).attr("href", "../Note/HeartRate" +
                    "?selectdate=" + paramStr +
                    "&periodType=" + type +
                    "&initDate=False");
            });

    });

    // 未来日付への遷移コントロール制御
    toggleNextDate();

    $('.chart-draw-area').chartInit({ 'chartName': 'pr' });

    // ヒストリー バック禁止
    mgf.prohibitHistoryBack();
})();
