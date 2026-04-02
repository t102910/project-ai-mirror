// mgf.note.mealsearchresult 名前空間オブジェクト
mgf.note.mealsearchresult = mgf.note.mealsearchresult || {};

// DOM 構築完了
mgf.note.mealsearchresult = (function () {

    // 「品名」が入力されたら「検索」ボタンを有効化
    $("main").on("input keyup blur", "#searchText", function () {
        if ($.trim($("#searchText").val()) == "") {
            $("#search").addClass("disabled");
        } else {
            $("#search").removeClass("disabled");
        }
    });

    // 「検索」ボタン
    $("main").on("click", "#search", function () {
        //// 画面をロック後、サブミット
        //mgf.note.promiseLockScreen($(this))
        //    .then(function () {
        //        //
        //        try {
        //            $("<form>", {
        //                action: "../Note/Meal4Result?ActionSource=SearchAgain",
        //                method: "POST"
        //            }).appendTo(document.body);

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "__RequestVerificationToken",
        //                value: $("input[name='__RequestVerificationToken']").val()
        //            }).appendTo($("form:last"));

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "searchText",
        //                value: $("#searchText").val()
        //            }).appendTo($("form:last"));

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "pageType",
        //                value: $("#meal").data("pagetype")
        //            }).appendTo($("form:last"));

        //            // サブミット
        //            $("form:last").submit();

        //        } catch (ex) {
        //            // 画面をアンロック
        //            mgf.unlockScreen();
        //        }
        //    });

        // 画面をロック
        mgf.lockScreen();

        // POST 内容を構築
        var fd = new FormData();

        fd.append("__RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());
        fd.append("searchText", $("#searchText").val());
        fd.append("pageType", $("#meal").data("pagetype"));

        // 登録内容を POST
        $.ajax({
            type: "POST",
            url: "../Note/Meal4Result?ActionSource=SearchAgain",
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

                    // リダイレクト
                    location.href = "../Note/MealSearchResult";

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
                    $("section.caution").text("失敗しました。status:" + jqXHR.status).removeClass("hide");

                    // 画面をアンロック
                    mgf.unlockScreen();
                }
            } catch (ex) {
                //console.log(ex);
                // エラー
                $("section.caution").text("失敗しました。exception:" + ex.message).removeClass("hide");

                // 画面をアンロック
                mgf.unlockScreen();
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            // エラー
            $("section.caution").text("失敗しました。status:" + jqXHR.status + " error:" + errorThrown).removeClass("hide");

            // 画面をアンロック
            mgf.unlockScreen();
        });

        return false;
    });

    // 「続きを表示」ボタン
    $("main").on("click", "a.btn-continued", function () {

        mgf.lockScreen();

        $.ajax({
            type: "POST",
            url: "../Note/Meal4Result?ActionSource=ShowMore",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                page: $(this).data("page")
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

                    // パーシャル ビューを書き換え
                    $(document).find("a.btn-continued").replaceWith($(data));
                    // 画面をアンロック
                    mgf.unlockScreen();
                } else {

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
    });

    // 検索結果をクリック
    $("main").on("click", ".meal", function () {
        var hinmoku = $(this).data("name");
        var cal = $(this).data("cal");
        var pal = $(this).data("pal");

        ////post
        //// 画面をロック後、サブミット
        //mgf.note.promiseLockScreen($(this))
        //    .then(function () {
        //        //
        //        try {
        //            $("<form>", {
        //                action: "../Note/Meal4Result?ActionSource=SearchResult",
        //                method: "POST"
        //            }).appendTo(document.body);

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "__RequestVerificationToken",
        //                value: $("input[name='__RequestVerificationToken']").val()
        //            }).appendTo($("form:last"));

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "Item",
        //                value: hinmoku
        //            }).appendTo($("form:last"));

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "Calorie",
        //                value: cal
        //            }).appendTo($("form:last"));

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "OtherParameter",
        //                value: pal
        //            }).appendTo($("form:last"));

        //            // サブミット
        //            $("form:last").submit();

        //        } catch (ex) {
        //            // 画面をアンロック
        //            mgf.unlockScreen();
        //        }
        //    })

        // 画面をロック
        mgf.lockScreen();

        // POST 内容を構築
        var fd = new FormData();

        fd.append("__RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());
        fd.append("Item", hinmoku);
        fd.append("Calorie", cal);
        fd.append("OtherParameter", pal);

        // 登録内容を POST
        $.ajax({
            type: "POST",
            url: "../Note/Meal4Result?ActionSource=SearchResult",
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

                    // リダイレクト
                    location.href = "../Note/" + $.parseJSON(data)["Messages"][0];

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
                    $("section.caution").text("失敗しました。status:" + jqXHR.status).removeClass("hide");

                    // 画面をアンロック
                    mgf.unlockScreen();
                }
            } catch (ex) {
                //console.log(ex);
                // エラー
                $("section.caution").text("失敗しました。exception:" + ex.message).removeClass("hide");

                // 画面をアンロック
                mgf.unlockScreen();
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            // エラー
            $("section.caution").text("失敗しました。status:" + jqXHR.status + " error:" + errorThrown).removeClass("hide");

            // 画面をアンロック
            mgf.unlockScreen();
        });

        return false;
    });

})();
