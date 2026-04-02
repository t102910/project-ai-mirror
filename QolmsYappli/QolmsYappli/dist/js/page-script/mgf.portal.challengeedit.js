// mgf.portal.challengeedit 名前空間オブジェクト
mgf.portal.challengeedit = mgf.portal.challengeedit || {};
// DOM 構築完了
mgf.portal.challengeedit = (function () {

    $("body").on("click", ".edit", function () {

        mgf.lockScreen();

        var pass = $('input[name="pass"]').val();
        //console.log(pass);

        var len = $(".add").length;
        var value = {};
        var result = {};
        $(".add").each(function (key, value) {

            //    console.log($(value).prop("name"));
            //    console.log(value)
            var name = $(value).prop("name");
            var val = $(value).val();

            value[name] = val;

            //console.log(key + ' : ' + name + ":"+ val);

            if (result[name]) {
                if (Array.isArray(result[name])) {
                    result[name].push(val);
                }
                else {
                    result[name] = new Array(result[name], val);
                }
            }
            else {
                result[name] = val;
            }
        });

        var checked = 0
        $('#connection-data :checked').each(function () {
            //値を取得
            var val = $(this).data("content");
            console.log(val);
            checked = checked + val;
        });

        console.log(checked);
        //console.log(result);

        $.ajax({
            type: "POST",
            url: "../Portal/ChallengeEditResult?ActionSource=Edit",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                pass: pass,
                values: result,
                checked: checked
            },
            async: true,
            beforeSend: function (jqXHR) {
                // セッションのチェック
                mgf.portal.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {

            if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0) {
                console.log($.parseJSON(data))

                if ($.isTrueString($.parseJSON(data)["IsSuccess"])) {
                    location.href = "../portal/challengedetail?key=" + $.parseJSON(data)["Key"]

                } else {

                    var messages = $.parseJSON(data)["Messages"];
                    console.log($.parseJSON(data)["Messages"])

                    var messagebody = "";

                    $(messages).each(function (key, value) {

                        messagebody = messagebody + value["Value"] + "<br/>"

                        //$("[name='" + value["Key"] + "']").nextAll(".caution").text().removeClass("hide");
                    })

                    console.log($.parseJSON(data)["Messages"])
                    $("#error-modal .modal-body").html(messagebody);
                    $("#error-modal").modal("show");
                }

                mgf.unlockScreen();

            }
        }).always(function (jqXHR, textStatus) {
            // 画面をアンロック
            mgf.unlockScreen();
        });
        return false;
    });


    $("body").on("click", ".cancel", function () {

        $("#cancel-modal").modal("show");

    });

    $("body").on("click", "#cancel-modal .btn-delete", function () {
        $("#cancel-modal").modal("hide");
        cancel();
    })

    function cancel () {

        mgf.lockScreen();

        $.ajax({
            type: "POST",
            url: "../Portal/ChallengeEditResult?ActionSource=Cancel",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val()
            },
            async: true,
            beforeSend: function (jqXHR) {
                // セッションのチェック
                mgf.portal.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {

            if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0) {
                console.log($.parseJSON(data))

                if ($.isTrueString($.parseJSON(data)["IsSuccess"])) {
                    location.href = "../portal/challengedetail?key=" + $.parseJSON(data)["Key"]

                } else {

                    var messages = $.parseJSON(data)["Messages"];
                    console.log($.parseJSON(data)["Messages"])

                    var messagebody = "";

                    $(messages).each(function (key, value) {

                        messagebody = messagebody + value["Value"] + "<br/>"

                        //$("[name='" + value["Key"] + "']").nextAll(".caution").text().removeClass("hide");
                    })

                    console.log($.parseJSON(data)["Messages"])
                    $("#error-modal .modal-body").html(messagebody);
                    $("#error-modal").modal("show");
                }

                mgf.unlockScreen();

            }
        }).always(function (jqXHR, textStatus) {
            // 画面をアンロック
            mgf.unlockScreen();
        });
        return false;
    };


    // --
    $('body').on("click", "#address-search", function () {
        // 画面をロック
        mgf.lockScreen();
        $.ajax({
            type: "POST",
            url: "../Portal/ChallengeEntryResult?ActionSource=PostCode",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                PostCode: $("#postcode").val()
            },
            async: true,
            beforeSend: function (jqXHR) {
                // セッションのチェック
                mgf.portal.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {

            if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0) {

                //パーシャルビューを書き換え
                //$(document).find("main").replaceWith($(data));
                //$("html,body").animate({ scrollTop: $('body').offset().top });

                var address = $.parseJSON(data)["Address"];
                $("#address").val(address);
                $("#address").focus();
            }

        }).always(function (jqXHR, textStatus) {
            // 画面をアンロック
            mgf.unlockScreen();
        });
        return false;
    });

    $("body").on("click", "#error-modal .close", function () {
        $("#error-modal").modal("hide");
    })

    $("body").on("click", "#error-modal .btn-close", function () {
        $("#error-modal").modal("hide");
    })

    $("body").on("click", "#cancel-modal .close", function () {
        $("#cancel-modal").modal("hide");
    })

    $("body").on("click", "#cancel-modal .btn-close", function () {
        $("#cancel-modal").modal("hide");
    })

    $("body").on("click", "#Information", function () {

        return false;
    });

    // ヒストリー バック禁止
    mgf.prohibitHistoryBack();


})();