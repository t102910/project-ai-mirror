// mgf.note.meal 名前空間オブジェクト
mgf.note.mealregisterfromhistory = mgf.note.mealregisterfromhistory || {};

// DOM 構築完了
mgf.note.mealregisterfromhistory = (function () {
    // 画像の非同期読み込み
    function loadPhoto() {
        $(document).find("#update-data-area img.photo.inview").each(function () {
            $(this).one("inview", { "reference": $(this).data("reference") }, function (event, visible) {
                $.ajax({
                    type: "GET",
                    url: event.data["reference"],
                    async: true,
                    context: event.target
                }).done(function (data, textStatus, jqXHR) {
                    $(this).attr("src", $(this).data("reference"));
                });
            });
        });
    };

    // 入力欄の初期化
    function resetInputArea() {
        // 登録ボタン使用不可
        $("a.btn-submit.upload").addClass("disabled");

        // アラートエリアを隠す 
        $("section.caution").text("").addClass("hide");
    };

    // 食事種別変更時、時刻を設定
    $("main").on("change", "input[name='mealtype']", function () {
        var mealType = $("#mealtype input[type='radio']:checked").val();
        var time1 = "";
        var time2 = "";
        var time3 = "0";

        switch (mealType) {
            case "Breakfast":
                time1 = "am";
                time2 = 7;
                break;
            case "Lunch":
                time1 = "pm";
                time2 = 0;
                break;
            case "Dinner":
                time1 = "pm";
                time2 = 7;
                break;
            default:
                return false;
        }

        $("select[name='meridiem']").val(time1);
        $("select[name='hour']").val(time2);
        $("select[name='minute']").val(time3);

        return false;
    });

    $("main").on("click", "#update-data-area article", function () {
        if ($(this).hasClass("selected")) {
            $(this).removeClass("selected");
        } else {
            $(this).addClass("selected");
        }

        // 履歴が1つでも選択されていたら「登録」ボタンを有効化
        checkSelectedHistory();
    });

    // 履歴が1つでも選択されていたら「登録」ボタンを有効化
    function checkSelectedHistory() {
        var $selected = $(document).find("#update-data-area article.selected");

        if ($selected.length == 0) {
            $("a.btn-submit.upload").addClass("disabled");
        } else {
            $("a.btn-submit.upload").removeClass("disabled");
        }
    };

    // 「登録」ボタン
    $("main").on("click", "a.btn-submit.upload", function () {
        // 画面をロック
        mgf.lockScreen();

        var fd = new FormData();
        var date = "";
        var time1 = "";
        var time2 = "";
        var time3 = "";
        var mealType = "";

        // 1つも選択されていない場合
        if ($(document).find("#update-data-area article.selected").length == 0) {
            // 画面をアンロック
            mgf.unlockScreen();

            return false;
        }

        // 日付
        date = $("div.target-date-time .form-control:input[type='text'].picker").val();

        // 時間
        var $select = $("div.target-date-time div.datetime-select .form-control:input");
        time1 = $select.eq(0).val();
        time2 = $select.eq(1).val();
        time3 = $select.eq(2).val();

        var $radio = $("div.target-date-time input[type='radio']:checked");
        mealType = $radio.val();

        // POST 内容を構築
        fd.append("__RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());

        $(document).find("#update-data-area article.selected").each(function () {
            fd.append("model.BeforeRecordDate", $(this).find("p.detail-data").data("recorddate"));
            fd.append("model.BeforeMealType", $(this).find("p.detail-data").data("mealtype"));
            fd.append("model.Sequence", $(this).find("p.detail-data").data("seq"));
            fd.append("model.ForeignKey", $(this).find("p.detail-data").data("photokey"));
            fd.append("model.RecordDate", date);
            fd.append("model.Meridiem", time1);
            fd.append("model.Hour", time2);
            fd.append("model.Minute", time3);
            fd.append("model.MealType", mealType);
            fd.append("model.ItemName", "不明");
            fd.append("model.Calorie", "0");
            fd.append("model.IsCheckFutureDate", "True");
        });

        // 登録内容を POST
        $.ajax({
            type: "POST",
            url: "../Note/MealRegisterFromHistoryResult?ActionSource=Add",
            //processData: false,
            //contentType: false,
            //data: fd,
            traditional: true,
            data: $.toJsonObject(fd),
            async: true,
            beforeSend: function (jqXHR) {
                // セッションのチェック
                mgf.note.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {
                    // 成功

                    //成功した時

                    // リダイレクト
                    location.href = "/Note/Meal4"
                    return false;
                } else if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0) {
                    // エラー
                    var messages = $.parseJSON(data)["Messages"];
                    var length = messages.length;
                    var $alert = $("section.caution");

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

                    // 画面をアンロック
                    mgf.unlockScreen();
                } else {
                    // エラー
                    $("section.caution").text("登録に失敗しました。status:" + jqXHR.status).removeClass("hide");

                    // 画面をアンロック
                    mgf.unlockScreen();
                }
            } catch (ex) {
                //console.log(ex);
                // エラー
                $("section.caution").text("登録に失敗しました。exception:" + ex.message).removeClass("hide");

                // 画面をアンロック
                mgf.unlockScreen();
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            // エラー
            $("section.caution").text("登録に失敗しました。status:" + jqXHR.status + " error:" + errorThrown).removeClass("hide");

            // 画面をアンロック
            mgf.unlockScreen();
        });

        return false;
    });

    // 「絞り込み」領域の表示切り替え
    $("main").on("click", ".calendar-view", function () {
        if ($(this).hasClass("on")) {
            // 非表示
            $(this).removeClass("on").html("<i class='la la-calendar la-2x'></i>絞り込み");
            $("#calender-area").slideUp();

            if ($("#filter-title").text() !== "最近の食事") {
                // 画面をロック
                mgf.lockScreen();

                $.ajax({
                    type: "POST",
                    url: "../Note/MealRegisterFromHistoryResult?ActionSource=Filter",
                    data: {
                        __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                    },
                    async: true,
                    beforeSend: function (jqXHR) {
                        // セッションのチェック
                        mgf.note.checkSessionByAjax();
                    }
                }).done(function (data, textStatus, jqXHR) {
                    try {
                        if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("text/html;") >= 0) {
                            // 成功

                            // ○日の食事 -> 最近の食事
                            $("#filter-title").text("最近の食事")

                            // パーシャル ビューを書き換え
                            $(document).find("#update-data-area").replaceWith($(data));

                            // 登録ボタン使用不可
                            $("a.btn-submit.upload").addClass("disabled");

                            // 画面をアンロック
                            mgf.unlockScreen();

                            // 画像を非同期でロード
                            loadPhoto();
                        } else {
                            // エラー

                            // 画面をアンロック
                            mgf.unlockScreen();
                        }
                    } catch (ex) {
                        // エラー

                        // 画面をアンロック
                        mgf.unlockScreen();
                    }
                }).fail(function (jqXHR, textStatus, errorThrown) {
                    // エラー

                    // 画面をアンロック
                    mgf.unlockScreen();
                });

                return false;
            }
        } else {
            // 表示
            // 日付 現在の時刻
            var now = new Date();
            var filterDate = now.getFullYear()
                + "年"
                + (now.getMonth() + 1)
                + "月"
                + now.getDate()
                + "日";

            $("#filter-date").datepicker("setDate", filterDate);
            $(this).addClass("on").html("<i class='la la-calendar la-2x'></i>絞り込み");
            $("#calender-area").slideDown();
        }
    })

    // 「絞り込み」ボタン
    $("main").on("click", "#filter-submit", function () {
        // 画面をロック
        mgf.lockScreen();

        // 日付
        var filter = $("#filter-date").val();

        $.ajax({
            type: "POST",
            url: "../Note/MealRegisterFromHistoryResult?ActionSource=Filter",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                FilterDate: filter
            },
            async: true,
            beforeSend: function (jqXHR) {
                // セッションのチェック
                mgf.note.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("text/html;") >= 0) {
                    // 成功

                    // 最近の食事 -> ○日の食事
                    $("#filter-title").text(filter.slice(-6) + "の食事")

                    // 「絞り込み」領域の表示
                    $(".calendar-view").addClass("on").html("<i class='la la-calendar la-2x'></i>絞り込み解除");
                    $("#calender-area").slideDown();

                    // パーシャル ビューを書き換え
                    $(document).find("#update-data-area").replaceWith($(data));

                    // 登録ボタン使用不可
                    $("a.btn-submit.upload").addClass("disabled");

                    // 画面をアンロック
                    mgf.unlockScreen();

                    // 画像を非同期でロード
                    loadPhoto();
                } else {
                    // エラー

                    // 画面をアンロック
                    mgf.unlockScreen();
                }
            } catch (ex) {
                // エラー

                // 画面をアンロック
                mgf.unlockScreen();
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            // エラー

            // 画面をアンロック
            mgf.unlockScreen();
        });

        return false;
    })

    // ヒストリー バックの禁止
    mgf.prohibitHistoryBack();

    // 入力欄の初期化
    resetInputArea();

    // セッションのチェック
    mgf.note.checkSessionByAjax();

    // 画像の非同期読み込み
    loadPhoto();
})();
