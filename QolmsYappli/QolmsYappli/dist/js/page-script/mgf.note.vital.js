// mgf.note.vital 名前空間 オブジェクト
mgf.note.vital = mgf.note.vital || {};

// DOM 構築完了
mgf.note.vital = (function () {
    // グラフ の非同期 ロード
    function vitalGraphLoad(vitalType, context) {
        return $.ajax({
            type: "POST",
            url: "../Note/VitalResult?ActionSource=Graph",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                VitalType: vitalType
            },
            async: true,
            context: context
        }).done(function (data, textStatus, jqXHR) {
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("text/html;") >= 0) {
                    var $div = $(data);
                    var $chartArea = $div.find("div.chart-area");
                    var $chartDrawArea = $div.find("div.chart-draw-area");

                    // 要素の差し替え
                    $(this).replaceWith($div);

                    // グラフ の描画
                    $chartDrawArea.chartInit({ "chartName": $chartArea.data("ref") });

                    // 体重 グラフ の場合は健康年齢 グラフ も考慮する
                    if ($chartArea.data("ref") == "w") {
                        // グラフ の描画
                        var $div2 = $(document).find("#ha-area");

                        if ($div2.length == 1) {
                            var $chartArea2 = $div2.find("div.chart-area");
                            var $chartDrawArea2 = $div2.find("div.chart-draw-area");

                            $chartDrawArea2.chartInit({ "chartName": $chartArea2.data("ref") });
                        }
                    }

                    // 表の スクロール
                    $(document).find(".table-responsive").one('inview', function (event, visible) {
                        if (visible == true) {
                            $(this).scrollTo('100%', 1500);
                        }
                    });
                } else {
                    // エラー
                }
            } catch (ex) {
                // エラー
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            // エラー
        });
    }

    // 「ホーム」ボタン の クリック
    $("main").on("click", "a.home-btn", function () {
        // 画面を ロック 後、遷移
        mgf.note.promiseLockScreen($(this))
            .then(function () {
                location.href = "../Portal/Home"
            });

        return false;
    })

    // 「身長」入力欄の表示切替
    $("main").on("click", "#height-opener a", function () {
        $("#how-tall").addClass("first-time");
    });

    // 「日時入力」領域の表示切り替え
    $("main").on("click", "section.contents-area a.input-expand", function () {
        if ($("#vital").hasClass("input-all")) {
            //$(this).empty().append("<i class='la la-calendar'></i>日時を変更して入力する");

            $("section.contents-area a.input-expand").each(function () {
                $(this).empty().append("<i class='la la-calendar'></i>日時を変更して入力する");
            });

            $("#vital").removeClass("input-all");
        } else {
            var now = new Date();
            var ymd = now.getFullYear()
                + "年"
                + ("0" + (now.getMonth() + 1)).slice(-2)
                + "月"
                + ("0" + now.getDate()).slice(-2)
                + "日";
            var hour = now.getHours();
            var minute = now.getMinutes();
            var meridiem = "";

            if (hour >= 12) {
                hour -= 12;
                meridiem = "pm";
            } else {
                meridiem = "am";
            }

            // 日付
            $(".form-control:input[type='text'].picker").datepicker("setDate", ymd);

            // 時間
            var $select = $("div.datetime-select .form-control:input");

            $select.eq(0).val(meridiem);
            $select.eq(1).val(hour);
            $select.eq(2).val(minute);

            $select.eq(3).val(meridiem);
            $select.eq(4).val(hour);
            $select.eq(5).val(minute);

            $select.eq(6).val(meridiem);
            $select.eq(7).val(hour);
            $select.eq(8).val(minute);

            //$(this).empty().append("<i class='la la-calendar'></i>現在の日時で入力する");

            $("section.contents-area a.input-expand").each(function () {
                $(this).empty().append("<i class='la la-calendar'></i>現在の日時で入力する");
            });

            $("#vital").addClass("input-all");
        }
    });

    //// 「登録」ボタン の クリック
    //$("main").on("click", "section.contents-area a.btn-submit", function () {
    //    // 画面を ロック
    //    mgf.lockScreen();

    //    var fd = new FormData();
    //    var now = new Date();
    //    var hasValue = [false, false, false];

    //    fd.append("__RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());

    //    $("div.input-area").each(function () {
    //        var vitalType = "";
    //        var name = "";
    //        var date = "";
    //        var time1 = "";
    //        var time2 = "";
    //        var time3 = "";
    //        var val1 = "";
    //        var val2 = "";
    //        var val3 = "";
    //        var $select = $(this).find("div.datetime-select .form-control:input");
    //        var $input = $(this).find(".form-control:input.val");

    //        if ($("#vital").hasClass("input-all")) {
    //            // 日付
    //            date = $(this).find(".form-control:input[type='text'].picker").val();

    //            // 時間
    //            time1 = $select.eq(0).val();
    //            time2 = $select.eq(1).val();
    //            time3 = $select.eq(2).val();
    //        } else {
    //            // 日付
    //            date = now.getFullYear()
    //                + "年"
    //                + ("0" + (now.getMonth() + 1)).slice(-2)
    //                + "月"
    //                + ("0" + now.getDate()).slice(-2)
    //                + "日";

    //            // 時間
    //            time1 = "";
    //            time2 = now.getHours();
    //            time3 = now.getMinutes();

    //            if (time2 >= 12) {
    //                time1 = "pm";
    //                time2 -= 12;

    //            } else {
    //                time1 = "am";
    //            }
    //        }

    //        switch ($(this).attr("id")) {
    //            case "input-1":
    //                vitalType = "BodyWeight";
    //                name = "model.Weight";
    //                val1 = $input.eq(0).val();
    //                val2 = $input.eq(1).val();
    //                val3 = "None";

    //                if (val1) hasValue[1] = true;

    //                break;

    //            case "input-2":
    //                vitalType = "BloodPressure";
    //                name = "model.Pressure";
    //                val1 = $input.eq(0).val();
    //                val2 = $input.eq(1).val();
    //                val3 = "None";

    //                if (val1) hasValue[0] = true;

    //                break;

    //            case "input-3":
    //                vitalType = "BloodSugar";
    //                name = "model.Sugar";
    //                val1 = $input.eq(1).val();
    //                val2 = "";
    //                val3 = $input.eq(0).val();

    //                if (val1) hasValue[2] = true;

    //                break;
    //        }

    //        // バイタル 情報の種別
    //        fd.append("model.VitalTypeN", vitalType);
    //        fd.append(name + ".VitalType", vitalType);

    //        // 日、AM/PM、時、分
    //        fd.append(name + "Date", date);
    //        fd.append(name + ".Meridiem", time1);
    //        fd.append(name + ".Hour", time2);
    //        fd.append(name + ".Minute", time3);

    //        // 値 1
    //        fd.append(name + ".Value1", val1);

    //        // 値 2
    //        fd.append(name + ".Value2", val2);

    //        // 測定 タイミング
    //        fd.append(name + ".ConditionType", val3);
    //    });

    //    if (!(hasValue[0] | hasValue[1] | hasValue[2])) {
    //        $("div.input-area section.caution").text("値を入力してください。").removeClass("hide");

    //        // 画面を アンロック
    //        mgf.unlockScreen();

    //        return false;
    //    }

    //    // 登録内容を POST
    //    $.ajax({
    //        type: "POST",
    //        url: "../Note/VitalResult?ActionSource=Edit",
    //        //processData: false,
    //        //contentType: false,
    //        //data: fd,
    //        //dataType: "json",
    //        traditional: true,
    //        data: $.toJsonObject(fd),
    //        async: true,
    //        beforeSend: function (jqXHR) {
    //            // セッション の チェック
    //            mgf.note.checkSessionByAjax();
    //        }
    //    }).done(function (data, textStatus, jqXHR) {
    //        if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0) {
    //            try {
    //                if ($.isTrueString($.parseJSON(data)["IsSuccess"])) {
    //                    // 成功
    //                    $("div.input-area").each(function () {
    //                        var $input = $(this).find(".form-control:input.val");

    //                        if ($input.eq(0).prop("tagName").toLowerCase() == "select") {
    //                            $input.eq(0).val("None");
    //                        } else {
    //                            $input.eq(0).val("");
    //                        }

    //                        if ($(this).attr("id") == "input-1") {
    //                            if ($.parseJSON(data)["VitalTypeN"].indexOf("BodyWeight") >= 0) {
    //                                $input.eq(1).val($.parseJSON(data)["Height"]);

    //                                if ($input.eq(1).val()) {
    //                                    $("#how-tall").removeClass("first-time");
    //                                } else {
    //                                    $("#how-tall").addClass("first-time");
    //                                }
    //                            }
    //                        } else {
    //                            $input.eq(1).val("");
    //                        }
    //                    });

    //                    $("div.input-area section.caution").text("").addClass("hide");

    //                    if ($("#vital").hasClass("input-all")) {
    //                        //$("#input-all").empty().append("<i class='la la-calendar'></i>日時を変更して入力する");

    //                        $("section.contents-area a.input-expand").each(function () {
    //                            $(this).empty().append("<i class='la la-calendar'></i>日時を変更して入力する");
    //                        });

    //                        $("#vital").removeClass("input-all");
    //                    }

    //                    // グラフ だけ リロード
    //                    mgf.note.$jqXhrGraphs = [];

    //                    $("section.data-area").children("div.reload-area").each(function () {
    //                        var vitalType = $(this).data("vital-type");
    //                        var title = $(this).children("h3.title").text();

    //                        if (!vitalType) vitalType = $(this).data("vital-empty-type");

    //                        if (vitalType && $.parseJSON(data)["VitalTypeN"].indexOf(vitalType) >= 0) {
    //                            $(this).empty().append("<h3 class='title mt10'>" + title + "</h3>").append("<div class='reload-area loading'></div>");

    //                            // 体重 グラフ の場合は健康年齢 グラフ も考慮する
    //                            if (vitalType == "BodyWeight") {
    //                                var $div = $(document).find("#ha-area");

    //                                //console.log($div);

    //                                if ($div.length == 1) {
    //                                    $div.remove();
    //                                }
    //                            }

    //                            // グラフ の非同期 ロード 開始
    //                            mgf.note.$jqXhrGraphs.push(vitalGraphLoad(vitalType, $(this)));
    //                        }
    //                    });
    //                } else {
    //                    // 失敗
    //                    var messages = $.parseJSON(data)["Messages"];
    //                    var length = messages.length;
    //                    var $alert = $("div.input-area section.caution");

    //                    if (length == 1) {
    //                        $alert.text(messages[0]).removeClass("hide");
    //                    } else {
    //                        $alert.text("");

    //                        $.each(messages, function (i, v) {
    //                            $alert.append(v);
    //                            if (i != length - 1) $alert.append("<br />");
    //                        });

    //                        $alert.removeClass("hide");
    //                    }
    //                }
    //            } catch (ex) {
    //                // エラー
    //                $("div.input-area section.caution").text("登録に失敗しました。").removeClass("hide");
    //            }
    //        } else {
    //            // エラー
    //            $("div.input-area section.caution").text("登録に失敗しました。").removeClass("hide");
    //        }
    //    }).fail(function (jqXHR, textStatus, errorThrown) {
    //        // エラー
    //        $("div.input-area section.caution").text("登録に失敗しました。").removeClass("hide");
    //    }).always(function (jqXHR, textStatus) {
    //        // 画面を アンロック
    //        mgf.unlockScreen();
    //    });
    //});

    // 「登録」ボタン の クリック
    $("main").on("click", "section.contents-area a.btn-submit", function () {
        // 画面を ロック
        mgf.lockScreen();

        var fd = new FormData();
        var now = new Date();
        var hasValue = [false, false, false];
        var $picker = $("div.input-area.active").find(".form-control:input[type='text'].picker"); // アクティブ タブ の日付を使用する
        var $select = $("div.input-area.active").find("div.datetime-select .form-control:input"); // アクティブ タブ の時間を使用する

        fd.append("__RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());

        $("div.input-area").each(function () {
            var vitalType = "";
            var name = "";
            var date = "";
            var time1 = "";
            var time2 = "";
            var time3 = "";
            var val1 = "";
            var val2 = "";
            var val3 = "";
            var $input = $(this).find(".form-control:input.val");

            if ($("#vital").hasClass("input-all")) {
                // 日付
                date = $picker.val();

                // 時間
                time1 = $select.eq(0).val();
                time2 = $select.eq(1).val();
                time3 = $select.eq(2).val();
            } else {
                // 日付
                date = now.getFullYear()
                    + "年"
                    + ("0" + (now.getMonth() + 1)).slice(-2)
                    + "月"
                    + ("0" + now.getDate()).slice(-2)
                    + "日";

                // 時間
                time1 = "";
                time2 = now.getHours();
                time3 = now.getMinutes();

                if (time2 >= 12) {
                    time1 = "pm";
                    time2 -= 12;

                } else {
                    time1 = "am";
                }
            }

            switch ($(this).attr("id")) {
                case "input-1":
                    vitalType = "BodyWeight";
                    name = "model.Weight";
                    val1 = $input.eq(0).val();
                    val2 = $input.eq(1).val();
                    val3 = "None";

                    if (val1) hasValue[1] = true;

                    break;

                case "input-2":
                    vitalType = "BloodPressure";
                    name = "model.Pressure";
                    val1 = $input.eq(0).val();
                    val2 = $input.eq(1).val();
                    val3 = "None";

                    if (val1) hasValue[0] = true;

                    break;

                case "input-3":
                    vitalType = "BloodSugar";
                    name = "model.Sugar";
                    val1 = $input.eq(1).val();
                    val2 = "";
                    val3 = $input.eq(0).val();

                    if (val1) hasValue[2] = true;

                    break;
            }

            // バイタル 情報の種別
            fd.append("model.VitalTypeN", vitalType);
            fd.append(name + ".VitalType", vitalType);

            // 日、AM/PM、時、分
            fd.append(name + "Date", date);
            fd.append(name + ".Meridiem", time1);
            fd.append(name + ".Hour", time2);
            fd.append(name + ".Minute", time3);

            // 値 1
            fd.append(name + ".Value1", val1);

            // 値 2
            fd.append(name + ".Value2", val2);

            // 測定 タイミング
            fd.append(name + ".ConditionType", val3);
        });

        if (!(hasValue[0] | hasValue[1] | hasValue[2])) {
            $("div.input-area section.caution").text("値を入力してください。").removeClass("hide");

            // 画面を アンロック
            mgf.unlockScreen();

            return false;
        }

        // セッション チェック 後、登録内容を POST
        mgf.note.checkSessionAsync($(this))
            .then(
                function () {
                    $.ajax({
                        type: "POST",
                        url: "../Note/VitalResult?ActionSource=Edit",
                        traditional: true,
                        data: $.toJsonObject(fd),
                        async: true
                    }).done(function (data, textStatus, jqXHR) {
                        if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0) {
                            try {
                                if ($.isTrueString($.parseJSON(data)["IsSuccess"])) {
                                    // 成功
                                    $("div.input-area").each(function () {
                                        var $input = $(this).find(".form-control:input.val");

                                        if ($input.eq(0).prop("tagName").toLowerCase() == "select") {
                                            $input.eq(0).val("None");
                                        } else {
                                            $input.eq(0).val("");
                                        }

                                        if ($(this).attr("id") == "input-1") {
                                            if ($.parseJSON(data)["VitalTypeN"].indexOf("BodyWeight") >= 0) {
                                                $input.eq(1).val($.parseJSON(data)["Height"]);

                                                if ($input.eq(1).val()) {
                                                    $("#how-tall").removeClass("first-time");
                                                } else {
                                                    $("#how-tall").addClass("first-time");
                                                }
                                            }
                                        } else {
                                            $input.eq(1).val("");
                                        }
                                    });

                                    $("div.input-area section.caution").text("").addClass("hide");

                                    if ($("#vital").hasClass("input-all")) {
                                        //$("#input-all").empty().append("<i class='la la-calendar'></i>日時を変更して入力する");

                                        $("section.contents-area a.input-expand").each(function () {
                                            $(this).empty().append("<i class='la la-calendar'></i>日時を変更して入力する");
                                        });

                                        $("#vital").removeClass("input-all");
                                    }

                                    // グラフ だけ リロード
                                    mgf.note.$jqXhrGraphs = [];

                                    $("section.data-area").children("div.reload-area").each(function () {
                                        var vitalType = $(this).data("vital-type");
                                        var title = $(this).children("h3.title").text();

                                        if (!vitalType) vitalType = $(this).data("vital-empty-type");

                                        if (vitalType && $.parseJSON(data)["VitalTypeN"].indexOf(vitalType) >= 0) {
                                            $(this).empty().append("<h3 class='title mt10'>" + title + "</h3>").append("<div class='reload-area loading'></div>");

                                            // 体重 グラフ の場合は健康年齢 グラフ も考慮する
                                            if (vitalType == "BodyWeight") {
                                                var $div = $(document).find("#ha-area");

                                                //console.log($div);

                                                if ($div.length == 1) {
                                                    $div.remove();
                                                }
                                            }

                                            // 非同期 ロード 開始
                                            mgf.note.$jqXhrGraphs.push(vitalGraphLoad(vitalType, $(this)));
                                        }
                                    });
                                } else {
                                    // 失敗
                                    var messages = $.parseJSON(data)["Messages"];
                                    var length = messages.length;
                                    var $alert = $("div.input-area section.caution");

                                    if (length == 1) {
                                        $alert.text(messages[0]).removeClass("hide");
                                    } else {
                                        $alert.text("");

                                        $.each(messages, function (i, v) {
                                            $alert.append(v);
                                            if (i != length - 1) $alert.append("<br />");
                                        });

                                        $alert.removeClass("hide");
                                    }
                                }
                            } catch (ex) {
                                // エラー
                                $("div.input-area section.caution").text("登録に失敗しました。").removeClass("hide");
                            }
                        } else {
                            // エラー
                            $("div.input-area section.caution").text("登録に失敗しました。").removeClass("hide");
                        }
                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        // エラー
                        $("div.input-area section.caution").text("登録に失敗しました。").removeClass("hide");
                    }).always(function (jqXHR, textStatus) {
                        // 画面を アンロック
                        mgf.unlockScreen();
                    });
                },
                function () {
                    // セッション 切れなら「ログイン」画面へ遷移
                    mgf.redirectToLoginWithReturnUrl();
                }
            );
    });

    // 「タブ」の切り替えを トラップ
    $("main").on("click", ".nav-tabs a", function (e) {
        var target = $(this).closest(".nav-tabs").find("li.active a").attr("href");
        var $input = $(target).find(".form-control:input.val");
        var isEditing = false;

        // 入力中か チェック
        switch (target) {
            case "#input-1":
                isEditing = $input.eq(0).val() != "";

                break;

            case "#input-2":
                isEditing = $input.eq(0).val() != "" || $input.eq(1).val() != "";

                break;

            case "#input-3":
                isEditing = $input.eq(1).val() != "";

                break;
        }

        if (isEditing) {
            e.stopImmediatePropagation();

            // 「入力確認」ダイアログ の表示
            $("#editing-modal").modal("show", {
                tabId: $(this).attr("href").replace("#", "")
            });

            return false;
        } else {
            // 「アラート」エリア の非表示
            $("div.input-area section.caution").text("").addClass("hide");
        }

        return true;
    });

    // 「入力確認」ダイアログ の初期化
    $("main").on("show.bs.modal", "#editing-modal", function (e) {
        // 初期化
        if (e.target.id == "editing-modal") {
            try {
                $(this).data("tab-id", e.relatedTarget.tabId);
            } catch (ex) { }
        }

        // ダイアログ の背後に「詳細」ダイアログが連動して表示されるので、子要素を削除・背景を透過して見えないようにする
        $("#val-modal").empty().css("background-color", "rgba(0, 0, 0, 0)");
    });

    // 「入力確認」ダイアログ 内の ボタン の クリック
    $("main").on("click", "#editing-modal button", function () {
        if ($(this).hasClass("btn-default")) {
            var tabId = $("#editing-modal").data("tab-id");
            var $input = null;

            // 入力欄の クリア と身長の初期化
            $input = $("#input-1").find(".form-control:input.val");
            $input.eq(0).val("");
            $input.eq(1).val($input.eq(1).data("default"));

            $input = $("#input-2").find(".form-control:input.val");
            $input.eq(0).val("");
            $input.eq(1).val("");

            $input = $("#input-3").find(".form-control:input.val");
            $input.eq(1).val("");

            // 「アラート」エリア の非表示
            $("div.input-area section.caution").text("").addClass("hide");

            // 「タブ」を切り替え
            $(".nav-tabs a[href='#" + tabId + "']").tab("show");
        }

        // 「入力確認」ダイアログ の非表示
        $(".modal-backdrop").remove();
        $("#editing-modal").modal("hide");
    });

    // 「入力確認」ダイアログ の非表示
    $("main").on("hidden.bs.modal", "#editing-modal", function () {
        $("body").removeClass("modal-open");

        // 「詳細」ダイアログ の透過率を戻す
        $("#val-modal").css("background-color", "rgba(0, 0, 0, .8)");
    });

    //// 「詳細」ダイアログ の表示
    //$("main").on("click", "[data-selected-day]", function () {
    //    // 画面を ロック
    //    mgf.lockScreen();

    //    var vitalType = $(this).closest("table").data("vital-type");
    //    var recordDate = $(this).data("selected-day");

    //    $.ajax({
    //        type: "POST",
    //        url: "../Note/VitalResult?ActionSource=Detail",
    //        data: {
    //            __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
    //            VitalType: vitalType,
    //            RecordDate: recordDate
    //        },
    //        async: true,
    //        beforeSend: function (jqXHR) {
    //            // セッション の チェック
    //            mgf.note.checkSessionByAjax();
    //        }
    //    }).done(function (data, textStatus, jqXHR) {
    //        try {
    //            if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("text/html;") >= 0) {
    //                var $div = $(data);

    //                // 要素の差し替え
    //                $(document).find("#val-modal").replaceWith($div);

    //                // 詳細の表示
    //                $("body").addClass("modal-open");
    //            } else {
    //                // エラー
    //            }
    //        } catch (ex) {
    //            // エラー
    //        }
    //    }).fail(function (jqXHR, textStatus, errorThrown) {
    //        // エラー
    //    }).always(function (jqXHR, textStatus) {
    //        // 画面を アンロック
    //        mgf.unlockScreen();
    //    });
    //});

    // 「詳細」ダイアログ の表示
    $("main").on("click", "[data-selected-day]", function () {
        // 画面を ロック
        mgf.lockScreen();

        var vitalType = $(this).closest("table").data("vital-type");
        var recordDate = $(this).data("selected-day");

        // セッション チェック 後、「詳細」ダイアログ を表示
        mgf.note.checkSessionAsync($(this))
            .then(
                function () {
                    $.ajax({
                        type: "POST",
                        url: "../Note/VitalResult?ActionSource=Detail",
                        data: {
                            __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                            VitalType: vitalType,
                            RecordDate: recordDate
                        },
                        async: true
                    }).done(function (data, textStatus, jqXHR) {
                        try {
                            if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("text/html;") >= 0) {
                                var $div = $(data);

                                // 要素の差し替え
                                $(document).find("#val-modal").replaceWith($div);

                                // 詳細の表示
                                $("body").addClass("modal-open");
                            } else {
                                // エラー
                            }
                        } catch (ex) {
                            // エラー
                        }
                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        // エラー
                    }).always(function (jqXHR, textStatus) {
                        // 画面を アンロック
                        mgf.unlockScreen();
                    });
                },
                function () {
                    // セッション 切れなら「ログイン」画面へ遷移
                    mgf.redirectToLoginWithReturnUrl();
                }
            );
    });

    // 「詳細」ダイアログ 内の各値の「削除」ボタン のクリック
    $("main").on("click", "#val-modal i.la-remove", function () {
        var rowNo = $(this).closest("tr").data("row-no");

        if (rowNo >= 0) {
            // 「削除確認」ダイアログ の表示
            $("#delete-modal").modal("show", {
                rowNo: rowNo
            });
        }
    });

    // 「削除確認」ダイアログ の初期化
    $("main").on("show.bs.modal", "#delete-modal", function (e) {
        // 初期化
        if (e.target.id == "delete-modal") {
            try {
                $(this).data("row-no", e.relatedTarget.rowNo);
            } catch (ex) { }
        }
    });

    // 「削除確認」ダイアログ 内の ボタン の クリック
    $("main").on("click", "#delete-modal button", function () {
        if ($(this).hasClass("btn-delete")) {
            var rowNo = $("#delete-modal").data("row-no");

            $(document).find("#val-modal tr").each(function () {
                if ($(this).data("row-no") == rowNo) {
                    $(this).addClass("hide");

                    return false;
                }
            });
        }

        // 「削除確認」ダイアログ の非表示
        $(".modal-backdrop").remove();
        $("#delete-modal").modal("hide");
    });

    // 「削除確認」ダイアログ の非表示
    $("main").on("hidden.bs.modal", "#delete-modal", function () {
        $("body").addClass("modal-open");
    });

    //// 「詳細」ダイアログ の非表示
    //$("main").on("click", "#val-modal-close", function () {
    //    // 画面を ロック
    //    mgf.lockScreen();

    //    var vitalType;
    //    var reference = [];
    //    var $div = $(this).closest("div");

    //    vitalType = $div.find("table").data("vital-type");

    //    $div.find("tr.hide").each(function () {
    //        reference.push($(this).data("reference"));
    //    });

    //    // 削除対象を POST
    //    if (reference[0]) {
    //        $.ajax({
    //            type: "POST",
    //            url: "../Note/VitalResult?ActionSource=Delete",
    //            data: {
    //                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
    //                VitalType: vitalType,
    //                Reference: reference
    //            },
    //            traditional: true,
    //            async: true,
    //            beforeSend: function (jqXHR) {
    //                // セッション の チェック
    //                mgf.note.checkSessionByAjax();
    //            }
    //        }).done(function (data, textStatus, jqXHR) {
    //            try {
    //                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {
    //                    // 成功
    //                    $("div.input-area").each(function () {
    //                        if ($(this).attr("id") == "input-1") {
    //                            var $input = $(this).find(".form-control:input.val");

    //                            if ($.parseJSON(data)["VitalType"] == "BodyWeight") {
    //                                $input.eq(1).val($.parseJSON(data)["Height"]);

    //                                if ($input.eq(1).val()) {
    //                                    $("#how-tall").removeClass("first-time");
    //                                } else {
    //                                    $("#how-tall").addClass("first-time");
    //                                }
    //                            }

    //                            return false;
    //                        }
    //                    });

    //                    // グラフ だけ リロード
    //                    mgf.note.$jqXhrGraphs = [];

    //                    $("section.data-area").children("div.reload-area").each(function () {
    //                        var vitalType = $(this).data("vital-type");
    //                        var title = $(this).children("h3.title").text();

    //                        if (!vitalType) vitalType = $(this).data("vital-empty-type");

    //                        if (vitalType && vitalType == $.parseJSON(data)["VitalType"]) {
    //                            $(this).empty().append("<h3 class='title mt10'>" + title + "</h3>").append("<div class='reload-area loading'></div>");

    //                            // 体重 グラフ の場合は健康年齢 グラフ も考慮する
    //                            if (vitalType == "BodyWeight") {
    //                                var $div = $(document).find("#ha-area");

    //                                //console.log($div);

    //                                if ($div.length == 1) {
    //                                    $div.remove();
    //                                }
    //                            }

    //                            // グラフ の非同期ロード開始
    //                            mgf.note.$jqXhrGraphs.push(vitalGraphLoad(vitalType, $(this)));

    //                            // 削除対象は 1 種別のはずなので ループ を抜ける 
    //                            return false;
    //                        }
    //                    });
    //                } else {
    //                    // エラー
    //                }
    //            } catch (ex) {
    //                // エラー
    //            }
    //        }).fail(function (jqXHR, textStatus, errorThrown) {
    //            // エラー
    //        }).always(function (jqXHR, textStatus) {
    //            // 画面を アンロック
    //            mgf.unlockScreen();
    //        });
    //    } else {
    //        // 画面を アンロック
    //        mgf.unlockScreen();
    //    }

    //    // 「詳細」ダイアログ の非表示
    //    $("body").removeClass("modal-open");
    //});

    // 「詳細」ダイアログ の非表示
    $("main").on("click", "#val-modal-close", function () {
        // 画面を ロック
        mgf.lockScreen();

        var vitalType;
        var reference = [];
        var $div = $(this).closest("div");

        vitalType = $div.find("table").data("vital-type");

        $div.find("tr.hide").each(function () {
            reference.push($(this).data("reference"));
        });

        if (reference[0]) {
            // セッション チェック 後、削除対象を POST
            mgf.note.checkSessionAsync($(this))
                .then(
                    function () {
                        $.ajax({
                            type: "POST",
                            url: "../Note/VitalResult?ActionSource=Delete",
                            data: {
                                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                                VitalType: vitalType,
                                Reference: reference
                            },
                            traditional: true,
                            async: true
                        }).done(function (data, textStatus, jqXHR) {
                            try {
                                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {
                                    // 成功
                                    $("div.input-area").each(function () {
                                        if ($(this).attr("id") == "input-1") {
                                            var $input = $(this).find(".form-control:input.val");

                                            if ($.parseJSON(data)["VitalType"] == "BodyWeight") {
                                                $input.eq(1).val($.parseJSON(data)["Height"]);

                                                if ($input.eq(1).val()) {
                                                    $("#how-tall").removeClass("first-time");
                                                } else {
                                                    $("#how-tall").addClass("first-time");
                                                }
                                            }

                                            return false;
                                        }
                                    });

                                    // グラフ だけ リロード
                                    mgf.note.$jqXhrGraphs = [];

                                    $("section.data-area").children("div.reload-area").each(function () {
                                        var vitalType = $(this).data("vital-type");
                                        var title = $(this).children("h3.title").text();

                                        if (!vitalType) vitalType = $(this).data("vital-empty-type");

                                        if (vitalType && vitalType == $.parseJSON(data)["VitalType"]) {
                                            $(this).empty().append("<h3 class='title mt10'>" + title + "</h3>").append("<div class='reload-area loading'></div>");

                                            // 体重 グラフ の場合は、健康年齢 グラフ も考慮する
                                            if (vitalType == "BodyWeight") {
                                                var $div = $(document).find("#ha-area");

                                                //console.log($div);

                                                if ($div.length == 1) {
                                                    $div.remove();
                                                }
                                            }

                                            // 非同期 ロード 開始
                                            mgf.note.$jqXhrGraphs.push(vitalGraphLoad(vitalType, $(this)));

                                            // 削除対象は 1 種別のはずなので ループ を抜ける 
                                            return false;
                                        }
                                    });
                                } else {
                                    // エラー
                                }
                            } catch (ex) {
                                // エラー
                            }
                        }).fail(function (jqXHR, textStatus, errorThrown) {
                            // エラー
                        }).always(function (jqXHR, textStatus) {
                            // 画面を アンロック
                            mgf.unlockScreen();
                        });
                    },
                    function () {
                        // セッション 切れなら「ログイン」画面へ遷移
                        mgf.redirectToLoginWithReturnUrl();
                    }
                );
        } else {
            // 画面を アンロック
            mgf.unlockScreen();
        }

        // 「詳細」ダイアログ の非表示
        $("body").removeClass("modal-open");
    });

    // TODO: 実装中
    // 「タニタ QR」ダイアログ の情報取得と表示  
    $("main").on("click", "#tanita-qr", function () {
        // 画面を ロック
        mgf.lockScreen();

        $.ajax({
            type: "POST",
            url: "../Note/VitalTanitaQr",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                Reference: $(this).data("reference")
            },
            async: true
        }).done(function (data, textStatus, jqXHR) {
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {
                    // 成功
                    $("#tanita-qr-modal").modal("show", {
                        qrCode: $.parseJSON(data)["QrCode"],
                        experies: $.parseJSON(data)["Experies"],
                        message: ""
                    });
                } else {
                    //console.log(textStatus);

                    // エラー
                    $("#tanita-qr-modal").modal("show", {
                        qrCode: "",
                        experies: "",
                        message: "QRコードの取得に失敗しました。"
                    });
                }
            } catch (ex) {
                //console.log(ex.message);

                // エラー
                $("#tanita-qr-modal").modal("show", {
                    qrCode: "",
                    experies: "",
                    message: "QRコードの取得に失敗しました。"
                });
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            //console.log(textStatus);

            // エラー
            $("#tanita-qr-modal").modal("show", {
                qrCode: "",
                experies: "",
                message: "QRコードの取得に失敗しました。"
            });
        }).always(function (jqXHR, textStatus) {
            // 画面を アンロック
            mgf.unlockScreen();
        });
    });

    // TODO: 実装中
    // 「タニタ QR」ダイアログ の初期化
    $("main").on("show.bs.modal", "#tanita-qr-modal", function (e) {
        // 初期化
        if (e.target.id == "tanita-qr-modal") {
            //console.log(e.relatedTarget.qrCode);
            //console.log(e.relatedTarget.experies);
            //console.log(e.relatedTarget.message);

            let $canvas = $(this).find("div.modal-body canvas");
            let $span = $(this).find("div.modal-body span");

            try {
                let qrCode = e.relatedTarget.qrCode;
                let experies = e.relatedTarget.experies;
                let message = e.relatedTarget.message;

                if (message == "") {
                    // QR 画像を表示
                    $canvas.qrcode({
                        render: "canvas",
                        minVersion: 9,
                        maxVersion: 9,
                        ecLevel: "L",
                        text: qrCode
                    });
                    $canvas.removeClass("hide");
                    $span.text(experies);
                } else {
                    // エラー
                    $canvas.addClass("hide");
                    $span.text(message);
                }
            } catch (ex) {
                console.log(ex.message);
                $canvas.addClass("hide");
                $span.text("QRコードの取得に失敗しました。");
            }
        }

        // ダイアログ の背後に「詳細」ダイアログが連動して表示されるので、子要素を削除・背景を透過して見えないようにする
        $("#val-modal").empty().css("background-color", "rgba(0, 0, 0, 0)");
    });

    // TODO: 実装中
    // 「タニタ QR」ダイアログ 内の ボタン の クリック
    $("main").on("click", "#tanita-qr-modal button", function () {
        // 「タニタ QR」ダイアログ の非表示
        $(".modal-backdrop").remove();
        $("#tanita-qr-modal").modal("hide");
    });

    // TODO: 実装中
    // 「タニタ QR」ダイアログ の非表示
    $("main").on("hidden.bs.modal", "#tanita-qr-modal", function () {
        $("body").removeClass("modal-open");

        // 「詳細」ダイアログ の透過率を戻す
        $("#val-modal").css("background-color", "rgba(0, 0, 0, .8)");
    });

    // ヒストリー バック 禁止
    mgf.prohibitHistoryBack();

    // セッション の チェック
    //mgf.note.checkSessionByAjax();

    //// グラフ の非同期 ロード
    //$("section.data-area").children("div.reload-area").each(function () {
    //    var vitalType = $(this).data("vital-type");

    //    if (vitalType) {
    //        // グラフ の非同期 ロード 開始
    //        mgf.note.$jqXhrGraphs.push(vitalGraphLoad(vitalType, $(this)));
    //    }
    //});

    // セッション チェック 後 グラフ を非同期 ロード
    mgf.note.checkSessionAsync(null)
        .then(
            function () {
                $("section.data-area").children("div.reload-area").each(function () {
                    var vitalType = $(this).data("vital-type");

                    if (vitalType) {
                        // 非同期 ロード 開始
                        mgf.note.$jqXhrGraphs.push(vitalGraphLoad(vitalType, $(this)));
                    }
                });
            }
        );
})();
