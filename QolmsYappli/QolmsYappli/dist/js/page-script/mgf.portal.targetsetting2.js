// mgf.portal.targetsetting 名前空間オブジェクト
mgf.portal.targetsetting = mgf.portal.targetsetting || {};

// DOM 構築完了
mgf.portal.targetsetting = (function () {
    // 標準値の取得
    $("main").on("click", "#input-2 p.standard a", function () {
        // 画面をロック
        mgf.lockScreen();

        //var message = "標準値の取得に失敗しました。";
        var vitalType = $(this).closest("p").data("vital-type");

        $.ajax({
            type: "POST",
            url: "../Portal/TargetSettingResult2?ActionSource=StandardValue",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                VitalType: vitalType
            },
            async: true,
            beforeSend: function (jqXHR) {
                // セッションのチェック
                mgf.portal.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {
            if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {
                try {
                    var lower1 = $.parseJSON(data)["Lower1"];
                    var upper1 = $.parseJSON(data)["Upper1"];
                    var lower2 = $.parseJSON(data)["Lower2"];
                    var upper2 = $.parseJSON(data)["Upper2"];

                    switch ($.parseJSON(data)["VitalType"]) {
                        case "Steps":
                            // 歩数
                            $("#input-2 input[name='val3']").val(upper1);
                            $("#input-2 input[name='val4']").val(lower1);

                            break;

                        case "BodyWeight":
                            // 体重
                            $("#input-2 input[name='val5']").val(upper1);
                            $("#input-2 input[name='val6']").val(lower1);

                            break;

                        case "BloodPressure":
                            // 血圧
                            $("#input-2 input[name='val7']").val(upper1);
                            $("#input-2 input[name='val8']").val(lower1);
                            $("#input-2 input[name='val9']").val(upper2);
                            $("#input-2 input[name='val10']").val(lower2);

                            break;

                        case "BloodSugar":
                            // 血糖値
                            $("#input-2 input[name='val11']").val(upper1);
                            $("#input-2 input[name='val12']").val(lower1);
                            $("#input-2 input[name='val13']").val(upper2);
                            $("#input-2 input[name='val14']").val(lower2);

                            break;
                    }
                } catch (ex) {
                    // エラー
                }
            } else {
                // エラー
            }
        }).always(function (jqXHR, textStatus, errorThrown) {
            // Ajaxエラー
            //showStandardValueAlert(message);

            // 画面をアンロック
            mgf.unlockScreen();
        });

        return false;
    });

    // 登録
    $("main").on("click", ".btn-submit-regist", function () {
        // 画面をロック
        mgf.lockScreen();

        // エラーをクリア
        $(".caution").text("").addClass("hide");

        var fd = new FormData();

        // POST 内容を構築

        fd.append("__RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());

        // 選択中のタブ ID
        if ($("#input-2").hasClass("active")) {
            fd.append("model.TabId", "input-2");
        } else {
            fd.append("model.TabId", "input-1");
        }

        // 身長
        fd.append("model.Height", $("#height").val());
        // 体重
        fd.append("model.Weight", $("#weight").val());
        // 運動量
        fd.append("model.PhysicalActivityLevel", $('input[name=act]:checked').val());
        // 目標体重
        fd.append("model.TargetWeight", $("#target-weight").val());
        // 期限日
        fd.append("model.TargetDate", $("#target-date").val());
        // 摂取カロリー
        fd.append("model.TargetValue1", $("#input-1 input[name='val1']").val());
        // 消費カロリー
        fd.append("model.TargetValue2", $("#input-1 input[name='val2']").val());
        // 歩数上限目標
        fd.append("model.TargetValue3", $("#input-2 input[name='val3']").val());
        // 歩数下限目標
        fd.append("model.TargetValue4", $("#input-2 input[name='val4']").val());
        // 体重上限目標
        fd.append("model.TargetValue5", $("#input-2 input[name='val5']").val());
        // 体重下限目標
        fd.append("model.TargetValue6", $("#input-2 input[name='val6']").val());
        // 血圧（上）上限目標
        fd.append("model.TargetValue7", $("#input-2 input[name='val7']").val());
        // 血圧（上）下限目標
        fd.append("model.TargetValue8", $("#input-2 input[name='val8']").val());
        // 血圧（下）上限目標
        fd.append("model.TargetValue9", $("#input-2 input[name='val9']").val());
        // 血圧（下）下限目標
        fd.append("model.TargetValue10", $("#input-2 input[name='val10']").val());
        // 血糖値（空腹時）上限目標
        fd.append("model.TargetValue11", $("#input-2 input[name='val11']").val());
        // 血糖値（空腹時）下限目標
        fd.append("model.TargetValue12", $("#input-2 input[name='val12']").val());
        // 血糖値（その他）上限目標
        fd.append("model.TargetValue13", $("#input-2 input[name='val13']").val());
        // 血糖値（その他）下限目標
        fd.append("model.TargetValue14", $("#input-2 input[name='val14']").val());


        // 登録内容を POST
        $.ajax({
            type: "POST",
            url: "../Portal/TargetSettingResult2?ActionSource=Edit",
            //processData: false,
            //contentType: false,
            //dataType: "json",
            //data: fd,
            traditional: true,
            data: $.toJsonObject(fd),
            async: true,
            beforeSend: function (jqXHR) {
                // セッションのチェック
                mgf.portal.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {
                    // 成功
                    var values = $.parseJSON(data)["Values"];

                    // リダイレクト
                    location.href = values[0]["Value"];
                } else if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0) {
                    // エラー
                    var values = $.parseJSON(data)["Values"];
                    $.each(values, function (index, val) {
                        var selector = "#caution-" + val["Key"].toLowerCase().replace("model.", "");
                        var $alert = $("main").find(selector);

                        if ($alert == null) {
                            selector = ".other-caution";
                            if ($(selector).text() != "") {
                                $(selector).append("<br />");
                            }
                        }

                        $(selector).append($("<p>" + val["Value"] + "</p>"));
                        $(selector).removeClass("hide");

                        if (val["Key"].toLowerCase().replace("model.", "") == "input-1") {

                            alert(val["Value"]);

                        } else if (val["Key"].toLowerCase().replace("model.", "") == "input-2"){
                            alert(val["Value"]);
                        }

                    });

                    // 画面をアンロック
                    mgf.unlockScreen();
                } else {
                    // エラー
                    $(".other-caution").text("登録に失敗しました。status:" + jqXHR.status).removeClass("hide");

                    // 画面をアンロック
                    mgf.unlockScreen();
                }
            } catch (ex) {
                // エラー
                //console.log(ex);
                $(".other-caution").text("登録に失敗しました。exception:" + ex.message).removeClass("hide");

                // 画面をアンロック
                mgf.unlockScreen();
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            // エラー
            $(".other-caution").text("登録に失敗しました。status:" + jqXHR.status + " error:" + errorThrown).removeClass("hide");

            // 画面をアンロック
            mgf.unlockScreen();
        });

        return false;
    });

    // 「確認」ダイアログの「×」ボタン（Yappli 用）
    $("main").on("click", "#finish-modal div.modal-header button.close", function () {
        try {
            $("#finish-modal").modal("hide");
        } catch (ex) {
            //console.log(ex);
            location.href = "../Portal/TargetSetting2";
        }
    });

    // 「確認」ダイアログの「閉じる」ボタン（Yappli 用）
    $("main").on("click", "#finish-modal div.modal-footer button.btn-close", function () {
        try {
            $("#finish-modal").modal("hide");
        } catch (ex) {
            //console.log(ex);
            location.href = "../Portal/TargetSetting2";
        }
    });

    // 「エラー」ダイアログの「×」ボタン（Yappli 用）
    $("main").on("click", "#error-modal div.modal-header button.close", function () {
        try {
            $("#error-modal").modal("hide");
        } catch (ex) {
            //console.log(ex);
            location.href = "../Portal/TargetSetting2";
        }
    });

    // 「エラー」ダイアログの「閉じる」ボタン（Yappli 用）
    $("main").on("click", "#error-modal div.modal-footer button.btn-close", function () {
        try {
            $("#error-modal").modal("hide");
        } catch (ex) {
            //console.log(ex);
            location.href = "../Portal/TargetSetting2";
        }
    });

    // ヒストリー バック禁止
    mgf.prohibitHistoryBack();

    // 登録完了後のリロードなら「確認」ダイアログを表示（Yappli 用）
    if ($("#finish-modal").length == 1) {
        $("#finish-modal").modal("show");
    }

    $("main").on("focusout", "#weight", function () {

        if ($('#weight').closest('div').data("defalt") == $("#weight").val()) {
            return false;
        }
        //console.log($('input[name=act]:checked').val().length);

        if ($("#weight").val().length > 0 && $("#height").val().length > 0 && $('input[name=act]:checked').val().length > 0) {
            $.ajax({
                type: "POST",
                url: "../Portal/TargetSettingResult2?ActionSource=Calculation",
                data: {
                    __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                    weight: $("#weight").val(),
                    height: $("#height").val(),
                    physicalActivityLevel: $('input[name=act]:checked').val(),
                    type: 'weight'
                },
                async: true,
                beforeSend: function (jqXHR) {
                    // セッションのチェック
                    mgf.portal.checkSessionByAjax();
                }
            }).done(function (data, textStatus, jqXHR) {

                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {

                    var values = $.parseJSON(data)["Values"]

                    $.each(values, function (index, val) {

                        if (val["Key"] == "NowBasalMetabolism") {

                            $("#NowBasalMetabolism").replaceWith('<p class="cal-info logona" id="NowBasalMetabolism">' + val["Value"] + '<i>kcal</i></p>')
                            $("#NowBasalMetabolism-target").text("あなたの基礎代謝量は" + val["Value"] + "kcalです！")

                        } else if (val["Key"] == "StdBasalMetabolism") {

                            $("#StdBasalMetabolism").text(val["Value"] + "kcal")
                        } else if (val["Key"] == "NowEstimatedEnergyRequirement") {

                            $("#NowEstimatedEnergyRequirement").replaceWith('<p class="cal-info logona"id="NowEstimatedEnergyRequirement">' + val["Value"] + '<i>kcal</i></p>')

                        } else if (val["Key"] == "StdEstimatedEnergyRequirement") {

                            $("#StdEstimatedEnergyRequirement").text(val["Value"] + "kcal")
                            $("#StdEstimatedEnergyRequirement-target").text("標準体重時に必要となるカロリーは" + val["Value"] + "kcalです！")
                            //$("#target-caloriein").val(val["Value"].replace(/,/g, ''))
                            $("#target-calorieout").val(val["Value"].replace(/,/g, ''))

                        }
                    });
                    $('#weight').closest('div').data('defalt', '');
                }
            })
        }
    });
    $("main").on("focusout", "#height", function () {
        if ($('#height').closest('div').data("defalt") == $("#height").val()) {
            return false;
        }
        if ($("#weight").val().length > 0 && $("#height").val().length > 0 && $('input[name=act]:checked').val().length > 0) {
            $.ajax({
                type: "POST",
                url: "../Portal/TargetSettingResult2?ActionSource=Calculation",
                data: {
                    __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                    weight: $("#weight").val(),
                    height: $("#height").val(),
                    physicalActivityLevel: $('input[name=act]:checked').val(),
                    type: 'height'
                },
                async: true,
                beforeSend: function (jqXHR) {
                    // セッションのチェック
                    mgf.portal.checkSessionByAjax();
                }
            }).done(function (data, textStatus, jqXHR) {

                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {

                    var values = $.parseJSON(data)["Values"]

                    $.each(values, function (index, val) {

                        if (val["Key"] == "NowBasalMetabolism") {

                            $("#NowBasalMetabolism").replaceWith('<p class="cal-info logona" id="NowBasalMetabolism">' + val["Value"] + '<i>kcal</i></p>')
                            $("#NowBasalMetabolism-target").text("あなたの基礎代謝量は" + val["Value"] + "kcalです！")

                        } else if (val["Key"] == "StdBasalMetabolism") {

                            $("#StdBasalMetabolism").text(val["Value"] + "kcal")
                        } else if (val["Key"] == "NowEstimatedEnergyRequirement") {

                            $("#NowEstimatedEnergyRequirement").replaceWith('<p class="cal-info logona"id="NowEstimatedEnergyRequirement">' + val["Value"] + '<i>kcal</i></p>')

                        } else if (val["Key"] == "StdEstimatedEnergyRequirement") {

                            $("#StdEstimatedEnergyRequirement").text(val["Value"] + "kcal")
                            $("#StdEstimatedEnergyRequirement-target").text("標準体重時に必要となるカロリーは" + val["Value"] + "kcalです！")
                            //$("#target-caloriein").val(val["Value"].replace(/,/g, ''))
                            $("#target-calorieout").val(val["Value"].replace(/,/g, ''))

                        }
                    });

                    $('#height').closest('div').data('defalt', '');

                }
            })
        }

    });


    $("main").on("change", "input[name=act]", function () {
        if ($('input[name=act]').closest('div').data("defalt") == $('input[name=act]:checked').val()) {
            return false;
        }

        if ($("#weight").val().length > 0 && $("#height").val().length > 0 && $('input[name=act]:checked').val().length > 0) {

            $.ajax({
                type: "POST",
                url: "../Portal/TargetSettingResult2?ActionSource=Calculation",
                data: {
                    __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                    weight: $("#weight").val(),
                    height: $("#height").val(),
                    physicalActivityLevel: $('input[name=act]:checked').val(),
                    type: 'physicalActivityLevel'
                },
                async: true,
                beforeSend: function (jqXHR) {
                    // セッションのチェック
                    mgf.portal.checkSessionByAjax();
                }
            }).done(function (data, textStatus, jqXHR) {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {

                    var values = $.parseJSON(data)["Values"]

                    $.each(values, function (index, val) {

                        if (val["Key"] == "NowBasalMetabolism") {

                            $("#NowBasalMetabolism").replaceWith('<p class="cal-info logona" id="NowBasalMetabolism">' + val["Value"] + '<i>kcal</i></p>')
                            $("#NowBasalMetabolism-target").text("あなたの基礎代謝量は" + val["Value"] + "kcalです！")

                        } else if (val["Key"] == "StdBasalMetabolism") {

                            $("#StdBasalMetabolism").text(val["Value"] + "kcal")
                        } else if (val["Key"] == "NowEstimatedEnergyRequirement") {

                            $("#NowEstimatedEnergyRequirement").replaceWith('<p class="cal-info logona"id="NowEstimatedEnergyRequirement">' + val["Value"] + '<i>kcal</i></p>')

                        } else if (val["Key"] == "StdEstimatedEnergyRequirement") {

                            $("#StdEstimatedEnergyRequirement").text(val["Value"] + "kcal")
                            $("#StdEstimatedEnergyRequirement-target").text("標準体重時に必要となるカロリーは" + val["Value"] + "kcalです！")
                            //$("#target-caloriein").val(val["Value"].replace(/,/g, ''))
                            $("#target-calorieout").val(val["Value"].replace(/,/g, ''))

                        }
                    });
                    $('input[name=act]').closest('div').data('defalt', '');

                }
            })
        }

    });

    // 計算
    $("main").on("click", "#calc", function () {
        // 画面をロック
        mgf.lockScreen();

        // エラーをクリア
        $(".caution").addClass("hide");

        var fd = new FormData();

        // POST 内容を構築

        fd.append("__RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());

        // 体重
        fd.append("model.Weight", $("#weight").val());
        // 運動量
        fd.append("model.PhysicalActivityLevel", $('input[name=act]:checked').val());
        // 目標体重
        fd.append("model.TargetWeight", $("#target-weight").val());
        // 期限日
        fd.append("model.TargetDate", $("#target-date").val());
        // ボタンの種類
        fd.append("model.ButtonType", "calc");

        // 登録内容を POST
        $.ajax({
            type: "POST",
            url: "../Portal/TargetSettingResult2?ActionSource=CalcTargetCalorieIn",
            //processData: false,
            //contentType: false,
            //dataType: "json",
            //data: fd,
            traditional: true,
            data: $.toJsonObject(fd),
            async: true,
            beforeSend: function (jqXHR) {
                // セッションのチェック
                mgf.portal.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {
                    // 成功
                    var values = $.parseJSON(data)["Values"];

                    $("#target-caloriein").val(values[0]["Value"].replace(/,/g, ''));

                    // 画面をアンロック
                    mgf.unlockScreen();
                } else if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0) {
                    // エラー
                    var values = $.parseJSON(data)["Values"];

                    $.each(values, function (index, val) {
                        var key = val["Key"].toLowerCase().replace("model.", "");
                        $("#caution-" + key).text("");
                        $("#caution-" + key).append($("<p>" + val["Value"] + "</p>"));
                        $("#caution-" + key).removeClass("hide");
                    });

                    // 画面をアンロック
                    mgf.unlockScreen();
                } else {
                    // 画面をアンロック
                    mgf.unlockScreen();
                }
            } catch (ex) {
                //console.log(ex);

                // 画面をアンロック
                mgf.unlockScreen();
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            // 画面をアンロック
            mgf.unlockScreen();
        });

        return false;
    });

})();
