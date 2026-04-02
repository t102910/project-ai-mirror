// mgf.start.setup 名前空間オブジェクト
mgf.start.setup = mgf.start.setup || {};

// DOM 構築完了
mgf.start.setup = (function () {
    var stepMode = $("main").find("section.contents-area").data("step-mode");

    // ステップ 1 へ戻る
    function prevOne($target) {
        // 画面をロック後、サブミット
        mgf.start.promiseLockScreen($target)
            .then(function () {
                try {
                    $("<form>", {
                        action: "../Start/SetupResult2?ActionSource=Prev",
                        method: "POST"
                    }).appendTo(document.body);

                    $("<input>").attr({
                        type: "hidden",
                        name: "__RequestVerificationToken",
                        value: $("input[name='__RequestVerificationToken']").val()
                    }).appendTo($("form:last"));

                    // ステップ
                    $("<input>").attr({
                        type: "hidden",
                        name: "StepMode",
                        value: stepMode
                    }).appendTo($("form:last"));

                    // サブミット
                    $("form:last").submit();
                } catch (ex) {
                    // 画面をアンロック
                    mgf.unlockScreen();
                }
            });
    }

    // ステップ 3 へ進む
    function nextThree($target) {
        // 画面をロック後、サブミット
        mgf.start.promiseLockScreen($target)
            .then(function () {
                try {
                    $("<form>", {
                        action: "../Start/SetupResult2?ActionSource=Next",
                        method: "POST"
                    }).appendTo(document.body);

                    $("<input>").attr({
                        type: "hidden",
                        name: "__RequestVerificationToken",
                        value: $("input[name='__RequestVerificationToken']").val()
                    }).appendTo($("form:last"));

                    // ステップ
                    $("<input>").attr({
                        type: "hidden",
                        name: "model.StepMode",
                        value: stepMode
                    }).appendTo($("form:last"));

                    // 性別の種別
                    $("<input>").attr({
                        type: "hidden",
                        name: "model.SexType",
                        value: $("section.contents-area select[name='val1']").val()
                    }).appendTo($("form:last"));

                    // 生年月日の年
                    $("<input>").attr({
                        type: "hidden",
                        name: "model.BirthYear",
                        value: $("section.contents-area select[name='val2']").val()
                    }).appendTo($("form:last"));

                    // 生年月日の月
                    $("<input>").attr({
                        type: "hidden",
                        name: "model.BirthMonth",
                        value: $("section.contents-area select[name='val3']").val()
                    }).appendTo($("form:last"));

                    // 生年月日の日
                    $("<input>").attr({
                        type: "hidden",
                        name: "model.BirthDay",
                        value: $("section.contents-area select[name='val4']").val()
                    }).appendTo($("form:last"));

                    // 身長
                    $("<input>").attr({
                        type: "hidden",
                        name: "model.Height",
                        value: $("section.contents-area input[name='val5']").val()
                    }).appendTo($("form:last"));

                    // 体重
                    $("<input>").attr({
                        type: "hidden",
                        name: "model.Weight",
                        value: $("section.contents-area input[name='val6']").val()
                    }).appendTo($("form:last"));

                    // 運動量
                    $("<input>").attr({
                        type: "hidden",
                        name: "model.PhysicalActivityLevel",
                        value: $("section.contents-area input[name='val7']:checked").val()
                    }).appendTo($("form:last"));

                    // サブミット
                    $("form:last").submit();
                } catch (ex) {
                    // 画面をアンロック
                    mgf.unlockScreen();
                }
            });
    }

    //// ステップ 2 へ戻る
    //function prevTwo($target) {
    //    // 画面をロック後、サブミット
    //    mgf.start.promiseLockScreen($target)
    //        .then(function () {
    //            try {
    //                $("<form>", {
    //                    action: "../Start/SetupResult?ActionSource=Prev",
    //                    method: "POST"
    //                }).appendTo(document.body);

    //                $("<input>").attr({
    //                    type: "hidden",
    //                    name: "__RequestVerificationToken",
    //                    value: $("input[name='__RequestVerificationToken']").val()
    //                }).appendTo($("form:last"));

    //                // ステップ
    //                $("<input>").attr({
    //                    type: "hidden",
    //                    name: "StepMode",
    //                    value: stepMode
    //                }).appendTo($("form:last"));

    //                // サブミット
    //                $("form:last").submit();
    //            } catch (ex) {
    //                // 画面をアンロック
    //                mgf.unlockScreen();
    //            }
    //        });
    //}

    //// ステップ 3 へ進む
    //function nextThree($target) {
    //    // 画面をロック後、サブミット
    //    mgf.start.promiseLockScreen($target)
    //        .then(function () {
    //            try {
    //                $("<form>", {
    //                    action: "../Start/SetupResult?ActionSource=Next",
    //                    method: "POST"
    //                }).appendTo(document.body);

    //                $("<input>").attr({
    //                    type: "hidden",
    //                    name: "__RequestVerificationToken",
    //                    value: $("input[name='__RequestVerificationToken']").val()
    //                }).appendTo($("form:last"));

    //                // ステップ
    //                $("<input>").attr({
    //                    type: "hidden",
    //                    name: "model.StepMode",
    //                    value: stepMode
    //                }).appendTo($("form:last"));

    //                // サブミット
    //                $("form:last").submit();
    //            } catch (ex) {
    //                // 画面をアンロック
    //                mgf.unlockScreen();
    //            }
    //        });
    //}

    // 目標摂取カロリー計算
    function calculationTargetCalorieIn() {
        // 画面をロック
        mgf.lockScreen();

        // エラーをクリア
        $(".section.caution").addClass("hide");

        var fd = new FormData();

        // POST 内容を構築
        fd.append("__RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());
        // ステップ
        fd.append("model.StepMode", stepMode);
        // 目標体重
        fd.append("model.TargetWeight", $("section.contents-area input[name='val3']").val());
        // 期限日
        fd.append("model.TargetDate", $("section.contents-area input[name='val4']").val());
        // ボタンの種類
        fd.append("model.ButtonType", "calc");

        // 登録内容を POST
        $.ajax({
            type: "POST",
            url: "../Start/SetupResult2?ActionSource=Calc",
            //processData: false,
            //contentType: false,
            //dataType: "json",
            //data: fd,
            traditional: true,
            data: $.toJsonObject(fd),
            async: true,
            beforeSend: function (jqXHR) {
                // セッションのチェック
                mgf.start.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {
                    // 成功

                    // 目標摂取カロリーを設定
                    var values = $.parseJSON(data)["Values"];
                    $("#input1").val(values[0]["Value"].replace(/,/g, ''));

                    // 画面をアンロック
                    mgf.unlockScreen();
                } else if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0) {
                    // エラー

                    // エラーメッセージを設定、表示
                    var values = $.parseJSON(data)["Values"];
                    createErrorMessage(values);

                    // 画面をアンロック
                    mgf.unlockScreen();
                } else {
                    // 画面をアンロック
                    mgf.unlockScreen();
                }
            } catch (ex) {
                // エラー
                //console.log(ex);

                // 画面をアンロック
                mgf.unlockScreen();
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            // 画面をアンロック
            mgf.unlockScreen();
        });
    }

    //  ステップ 4（完了）へ進む
    function nextFinish() {
        // 画面をロック
        mgf.lockScreen();

        // エラーをクリア
        $(".section.caution").addClass("hide");

        var fd = new FormData();

        // POST 内容を構築

        fd.append("__RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());
        // ステップ
        fd.append("model.StepMode", stepMode);
        // 目標体重
        fd.append("model.TargetWeight", $("section.contents-area input[name='val3']").val());
        // 期限日
        fd.append("model.TargetDate", $("section.contents-area input[name='val4']").val());
        // 摂取カロリー
        fd.append("model.CaloriesIn", $("section.contents-area input[name='val1']").val());
        // 消費カロリー
        fd.append("model.CaloriesOut", $("section.contents-area input[name='val2']").val());

        // 登録内容を POST
        $.ajax({
            type: "POST",
            url: "../Start/SetupResult2?ActionSource=Next",
            //processData: false,
            //contentType: false,
            //dataType: "json",
            //data: fd,
            traditional: true,
            data: $.toJsonObject(fd),
            async: true,
            beforeSend: function (jqXHR) {
                // セッションのチェック
                mgf.start.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {
                    // 成功

                    // リダイレクト
                    var values = $.parseJSON(data)["Values"];
                    location.href = values[0]["Value"];
                    return false;
                } else if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0) {
                    // エラー

                    // エラーメッセージを設定、表示
                    var values = $.parseJSON(data)["Values"];
                    createErrorMessage(values);

                    // 画面をアンロック
                    mgf.unlockScreen();
                } else {
                    // エラー
                    $("#caution").text("登録に失敗しました。status:" + jqXHR.status).removeClass("hide");

                    // 画面をアンロック
                    mgf.unlockScreen();
                }
            } catch (ex) {
                // エラー
                //console.log(ex);
                $("#caution").text("登録に失敗しました。exception:" + ex.message).removeClass("hide");

                // 画面をアンロック
                mgf.unlockScreen();
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            // エラー
            $("#caution").text("登録に失敗しました。status:" + jqXHR.status + " error:" + errorThrown).removeClass("hide");

            // 画面をアンロック
            mgf.unlockScreen();
        });
    }

    function createErrorMessage(values) {
        $(".caution").text("");
        $.each(values, function (index, val) {
            var id = "#caution-";
            switch (val["Key"]) {
                case "model.CaloriesIn":
                    id += "caloriesin";
                    break;
                case "model.TargetWeight":
                    id += "targetweight";
                    break;
                case "model.TargetDate":
                    id += "targetdate";
                    break;
                default:
                    id = "#caution";
                    if ($(id).text() != "") {
                        $(id).append("<br />");
                    }
            }

            $(id).append($("<p>" + val["Value"] + "</p>"));
            $(id).removeClass("hide");
        });
    }

    $("main").on("click", "section.contents-area div.submit-area a", function () {
        switch (true) {
            case stepMode == 1 && $(this).hasClass("btn-submit"):
                nextThree($(this));

                break;

            //case stepMode == 2 && $(this).hasClass("btn-close"):
            //    prevOne($(this));

            //    break;

            //case stepMode == 2 && $(this).hasClass("btn-submit"):
            //    nextThree($(this));

            //    break;

            case stepMode == 3:
                switch ($(this).attr("id")) {
                    case "calc":
                        calculationTargetCalorieIn();
                        break;
                    case "prev":
                        prevOne($(this));
                        break;
                    case "regist":
                        nextFinish();
                        break;
                }
                break;
        }
    });

    //skip
    $("main").on("click", "#skip", function () {

        mgf.lockScreen();
        // 画面をロック後、サブミット
        mgf.start.promiseLockScreen($(this))
            .then(function () {
                try {
                    $("<form>", {
                        action: "../Start/SetupResult2?ActionSource=Skip",
                        method: "POST"
                    }).appendTo(document.body);

                    $("<input>").attr({
                        type: "hidden",
                        name: "__RequestVerificationToken",
                        value: $("input[name='__RequestVerificationToken']").val()
                    }).appendTo($("form:last"));

                    // サブミット
                    $("form:last").submit();
                } catch (ex) {
                    // 画面をアンロック
                    mgf.unlockScreen();
                }
            });
    });

    // ヒストリー バック禁止
    mgf.prohibitHistoryBack();


})();
