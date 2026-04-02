// mgf.note.walk 名前空間 オブジェクト
mgf.note.walk = mgf.note.walk || {};

// DOM 構築完了
mgf.note.walk = (function () {
    // グラフ の非同期 ロード
    function vitalGraphLoad(vitalType, context) {
        return $.ajax({
            type: "POST",
            url: "../Note/WalkResult?ActionSource=Graph",
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
    };

    // 「ホーム」ボタン の クリック
    $("main").on("click", "a.home-btn", function () {
        // 画面を ロック
        mgf.note.promiseLockScreen($(this))
         .then(function () {
             location.href = "../Portal/Home"
         });

        return false;
    })

    // 「日付入力」領域の表示切り替え
    $("main").on("click", "#input-all", function () {
        if ($("#walk").hasClass("input-all")) {
            $(this).empty().append("<i class='la la-calendar'></i>日付を変更して入力する");

            $("#walk").removeClass("input-all");
        } else {
            var now = new Date();
            var ymd = now.getFullYear()
                + "年"
                + ("0" + (now.getMonth() + 1)).slice(-2)
                + "月"
                + ("0" + now.getDate()).slice(-2)
                + "日";

            $("input[type='text'].picker").datepicker("setDate", ymd);

            $(this).empty().append("<i class='la la-calendar'></i>現在の日付で入力する");

            $("#walk").addClass("input-all");
        }
    });

    //// 「登録」ボタンのクリック
    //$("main").on("click", "section.contents-area a.btn-submit", function () {
    //    // 画面をロック
    //    mgf.lockScreen();

    //    var fd = new FormData();
    //    var $text = $("div.input-area input.form-control");
    //    var vitalType = "Steps";
    //    var name = "model.Steps";

    //    fd.append("__RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());

    //    if ($text.length == 2 && $text.eq(1).val()) {
    //        if ($("#walk").hasClass("input-all")) {
    //            fd.append("model.StepsDate", $text.eq(0).val());
    //        } else {
    //            var now = new Date();
    //            var ymd = now.getFullYear()
    //                + "年"
    //                + ("0" + (now.getMonth() + 1)).slice(-2)
    //                + "月"
    //                + ("0" + now.getDate()).slice(-2)
    //                + "日";

    //            fd.append("model.StepsDate", ymd);
    //        }

    //        fd.append("model.VitalTypeN", vitalType);
    //        fd.append(name + ".VitalType", vitalType);

    //        // AM / PM、時、分
    //        fd.append(name + ".Meridiem", "am");
    //        fd.append(name + ".Hour", "0");
    //        fd.append(name + ".Minute", "0");

    //        // 値 1
    //        fd.append(name + ".Value1", $text.eq(1).val());

    //        // 値 2
    //        fd.append(name + ".Value2", "");

    //        // 測定タイミング
    //        fd.append(name + ".ConditionType", "None");
    //    }

    //    // 登録内容を POST
    //    $.ajax({
    //        type: "POST",
    //        url: "../Note/WalkResult?ActionSource=Edit",
    //        //processData: false,
    //        //contentType: false,
    //        //data: fd,
    //        //dataType: "json",
    //        traditional: true,
    //        data: $.toJsonObject(fd),
    //        async: true,
    //        beforeSend: function (jqXHR) {
    //            // セッションのチェック
    //            mgf.note.checkSessionByAjax();
    //        }
    //    }).done(function (data, textStatus, jqXHR) {
    //        if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0) {
    //            try {
    //                if ($.isTrueString($.parseJSON(data)["IsSuccess"])) {
    //                    // 成功
    //                    var $text = $("div.input-area input.form-control");

    //                    if ($text.length == 2) $text.eq(1).val("");

    //                    $("div.input-area section.caution").text("").addClass("hide");

    //                    if ($("#walk").hasClass("input-all")) {
    //                        $("#input-all").empty().append("<i class='la la-calendar'></i>日付を変更して入力する");

    //                        $("#walk").removeClass("input-all");
    //                    }

    //                    // グラフだけリロード
    //                    mgf.note.$jqXhrGraphs = [];

    //                    $("section.data-area").children("div.reload-area").each(function () {
    //                        var vitalType = $(this).data("vital-type");
    //                        var title = $(this).children("h3.title").text();

    //                        if (!vitalType) vitalType = $(this).data("vital-empty-type");

    //                        if (vitalType && $.parseJSON(data)["VitalTypeN"].indexOf(vitalType) >= 0) {
    //                            $(this).empty().append("<h3 class='title mt10'>" + title + "</h3>").append("<div class='reload-area loading'></div>");

    //                            // グラフの非同期ロード開始
    //                            mgf.note.$jqXhrGraphs.push(vitalGraphLoad(vitalType, $(this)));

    //                            return false;
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
    //        // 画面をアンロック
    //        mgf.unlockScreen();
    //    });
    //});

    // 「登録」ボタン の クリック
    $("main").on("click", "section.contents-area a.btn-submit", function () {
        // 画面を ロック
        mgf.lockScreen();

        var fd = new FormData();
        var $text = $("div.input-area input.form-control");
        var vitalType = "Steps";
        var name = "model.Steps";

        fd.append("__RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());

        if ($text.length == 2 && $text.eq(1).val()) {
            if ($("#walk").hasClass("input-all")) {
                fd.append("model.StepsDate", $text.eq(0).val());
            } else {
                var now = new Date();
                var ymd = now.getFullYear()
                    + "年"
                    + ("0" + (now.getMonth() + 1)).slice(-2)
                    + "月"
                    + ("0" + now.getDate()).slice(-2)
                    + "日";

                fd.append("model.StepsDate", ymd);
            }

            fd.append("model.VitalTypeN", vitalType);
            fd.append(name + ".VitalType", vitalType);

            // AM / PM、時、分
            fd.append(name + ".Meridiem", "am");
            fd.append(name + ".Hour", "0");
            fd.append(name + ".Minute", "0");

            // 値 1
            fd.append(name + ".Value1", $text.eq(1).val());

            // 値 2
            fd.append(name + ".Value2", "");

            // 測定タイミング
            fd.append(name + ".ConditionType", "None");
        }

        // セッション チェック 後、登録内容を POST
        mgf.note.checkSessionAsync($(this))
            .then(
                function () {
                    $.ajax({
                        type: "POST",
                        url: "../Note/WalkResult?ActionSource=Edit",
                        traditional: true,
                        data: $.toJsonObject(fd),
                        async: true
                    }).done(function (data, textStatus, jqXHR) {
                        if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0) {
                            try {
                                if ($.isTrueString($.parseJSON(data)["IsSuccess"])) {
                                    // 成功
                                    var $text = $("div.input-area input.form-control");

                                    if ($text.length == 2) $text.eq(1).val("");

                                    $("div.input-area section.caution").text("").addClass("hide");

                                    if ($("#walk").hasClass("input-all")) {
                                        $("#input-all").empty().append("<i class='la la-calendar'></i>日付を変更して入力する");

                                        $("#walk").removeClass("input-all");
                                    }

                                    // グラフ だけ リロード
                                    mgf.note.$jqXhrGraphs = [];

                                    $("section.data-area").children("div.reload-area").each(function () {
                                        var vitalType = $(this).data("vital-type");
                                        var title = $(this).children("h3.title").text();

                                        if (!vitalType) vitalType = $(this).data("vital-empty-type");

                                        if (vitalType && $.parseJSON(data)["VitalTypeN"].indexOf(vitalType) >= 0) {
                                            $(this).empty().append("<h3 class='title mt10'>" + title + "</h3>").append("<div class='reload-area loading'></div>");

                                            // 非同期 ロード 開始
                                            mgf.note.$jqXhrGraphs.push(vitalGraphLoad(vitalType, $(this)));

                                            return false;
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

    //// 「詳細」ダイアログの表示
    //$("main").on("click", "[data-selected-day]", function () {
    //    // 画面をロック
    //    mgf.lockScreen();

    //    var vitalType = $(this).closest("table").data("vital-type");
    //    var recordDate = $(this).data("selected-day");

    //    $.ajax({
    //        type: "POST",
    //        url: "../Note/WalkResult?ActionSource=Detail",
    //        data: {
    //            __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
    //            VitalType: vitalType,
    //            RecordDate: recordDate
    //        },
    //        async: true,
    //        beforeSend: function (jqXHR) {
    //            // セッションのチェック
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
    //        // 画面をアンロック
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
                        url: "../Note/WalkResult?ActionSource=Detail",
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

    // 「削除確認」ダイアログ の表示
    $("main").on("click", "#val-modal i.la-remove", function () {
        // 確認 ダイアログ
        var rowNo = $(this).closest("tr").data("row-no");

        if (rowNo >= 0) {
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

        $(".modal-backdrop").remove();
        $("#delete-modal").modal("hide");
    });

    // 「削除確認」ダイアログ の非表示
    $("main").on("hidden.bs.modal", "#delete-modal", function () {
        $("body").addClass("modal-open");
    });

    //// 「詳細」ダイアログの非表示
    //$("main").on("click", "#val-modal-close", function () {
    //    // 画面をロック
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
    //            url: "../Note/WalkResult?ActionSource=Delete",
    //            data: {
    //                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
    //                VitalType: vitalType,
    //                Reference: reference
    //            },
    //            traditional: true,
    //            async: true,
    //            beforeSend: function (jqXHR) {
    //                // セッションのチェック
    //                mgf.note.checkSessionByAjax();
    //            }
    //        }).done(function (data, textStatus, jqXHR) {
    //            try {
    //                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {
    //                    // グラフだけリロード
    //                    mgf.note.$jqXhrGraphs = [];

    //                    $("section.data-area").children("div.reload-area").each(function () {
    //                        var vitalType = $(this).data("vital-type");
    //                        var title = $(this).children("h3.title").text();

    //                        if (!vitalType) vitalType = $(this).data("vital-empty-type");

    //                        if (vitalType && vitalType == $.parseJSON(data)["VitalType"]) {
    //                            $(this).empty().append("<h3 class='title mt10'>" + title + "</h3>").append("<div class='reload-area loading'></div>");

    //                            // グラフの非同期ロード開始
    //                            mgf.note.$jqXhrGraphs.push(vitalGraphLoad(vitalType, $(this)));

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
    //            // 画面をアンロック
    //            mgf.unlockScreen();
    //        });
    //    } else {
    //        // 画面をアンロック
    //        mgf.unlockScreen();
    //    }

    //    // 詳細の非表示
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
                            url: "../Note/WalkResult?ActionSource=Delete",
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
                                    // グラフ だけ リロード
                                    mgf.note.$jqXhrGraphs = [];

                                    $("section.data-area").children("div.reload-area").each(function () {
                                        var vitalType = $(this).data("vital-type");
                                        var title = $(this).children("h3.title").text();

                                        if (!vitalType) vitalType = $(this).data("vital-empty-type");

                                        if (vitalType && vitalType == $.parseJSON(data)["VitalType"]) {
                                            $(this).empty().append("<h3 class='title mt10'>" + title + "</h3>").append("<div class='reload-area loading'></div>");

                                            // 非同期 ロード 開始
                                            mgf.note.$jqXhrGraphs.push(vitalGraphLoad(vitalType, $(this)));

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

        // 詳細の非表示
        $("body").removeClass("modal-open");
    });

    // ヒストリー バック禁止
    mgf.prohibitHistoryBack();

    //// セッションのチェック
    //mgf.note.checkSessionByAjax();

    // グラフの非同期ロード
    //$("section.data-area").children("div.reload-area").each(function () {
    //    var vitalType = $(this).data("vital-type");

    //    if (vitalType) {
    //        // グラフの非同期ロード開始
    //        mgf.note.$jqXhrGraphs.push(vitalGraphLoad(vitalType, $(this)));
    //    }
    //});

    // セッション チェック 後、グラフ を非同期 ロード
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
