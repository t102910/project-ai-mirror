//  名前空間オブジェクト
mgf.passwordreset.recoversms = mgf.passwordreset.recoversms || {};

mgf.passwordreset.recoversms = (function () {

    var successView = function (phone) {
        console.log("successView")

        $.ajax({
            type: "POST",
            url: "../passwordreset/RecoverSMSResult?ActionSource=Success",
            data: {
                __editVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                PhoneNumber: phone
            },
            async: true,
            beforeSend: function (jqXHR) {
                //mgf.passwordreset.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {
            console.log(data)
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("text/html;") >= 0 ) {
                    //成功した時の表示
                    console.log(data)
                    console.log(jqXHR)

                    //成功時のパーシャルビューで置き換える
                    $(".contents-area div").replaceWith($(data));
                    mgf.unlockScreen();

                }
                else {
                    //なんでもいいから適当にログイン画面にとばす
                    location.href="../start/login"
                    mgf.unlockScreen();

                }
            } catch (ex) {

                location.href = "../start/login"
                mgf.unlockScreen();

            }
        }).fail(function (data, textStatus, jqXHR) {

            //$("#caution").text("");
            //$("#caution").append($("<p>" + "登録処理に失敗しました。5秒後に自動的に再読み込みします。お手数ですが再読み込み後に再度お試しください。" + "</p>"))
            //$("#caution").removeClass("hide");

        }).always(function (data, textStatus, jqXHR) {
            mgf.unlockScreen();
        });
        return false;
    }


    $("main").on("click", "#request", function () {
        mgf.lockScreen()

        $(".caution").addClass("hide");

        var phone = $("#mail").val();
        console.log(phone);

        var checked = 0
        //$('#connection-data :checked').each(function () {
        //    //値を取得
        //    var val = $(this).data("content");
        //    console.log(val);
        //    checked = checked + val;
        //});

        $.ajax({
            type: "POST", 
            url: "../passwordreset/recoverSMSResult?ActionSource=Send",
            data: {
                __editVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                PhoneNumber: phone
            },
            async: true,
            beforeSend: function (jqXHR) {
                //mgf.passwordreset.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {
            //console.log(data)
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {
                    //成功した時の表示
                    //successView(phone);
                    //パスコードの画面へ
                    console.log("success");
                    mgf.unlockScreen();
                    location.href = "../passwordreset/RecoverSmsPasscode";
                }
                else {
                    var messages = $.parseJSON(data)["Messages"];
                    //console.log($.parseJSON(data)["Messages"])

                    //var summaryMessage = true
                    $(messages).each(function (key, value) {

                        if (value["Key"] == "summary") {
                            $("#summary-cation").text(value["Value"]).removeClass("hide");
                            summaryMessage = false

                        } else {

                            $("[name='" + value["Key"] + "']").nextAll(".caution").text(value["Value"]).removeClass("hide");
                        }
                    })
                    //if (summaryMessage) {
                    //    $("#summary-cation").text("入力内容を確認してください。").removeClass("hide");

                    //}
                    mgf.unlockScreen();

                }
            } catch (ex) {

                mgf.unlockScreen();

            }
        }).fail(function (data, textStatus, jqXHR) {

            //$("#caution").text("");
            //$("#caution").append($("<p>" + "登録処理に失敗しました。5秒後に自動的に再読み込みします。お手数ですが再読み込み後に再度お試しください。" + "</p>"))
            //$("#caution").removeClass("hide");

        }).always(function (data, textStatus, jqXHR) {
            mgf.unlockScreen();
        });
        return false;

    });

    //$("main").on("click", "#Information", function () {

    //    return false;
    //});


    $("main").on("click", "#passcode", function () {

        mgf.lockScreen();
        var phone = $("#cphone").data("phone");

        var pass = $("#Pass").val();

        console.log(pass);

        $.ajax({
            type: "POST",
            url: "../passwordreset/RecoverSmsPasscodeResult?ActionSource=PassCode",
            data: {
                __editVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                PhoneNumber: phone,
                PassCode: pass
            },
            async: true,
            beforeSend: function (jqXHR) {
                //mgf.passwordreset.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {
            //console.log(data)
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("text/html;") >= 0) {

                    //成功時のパーシャルビューで置き換える
                    $(".contents-area div").replaceWith($(data));
                    mgf.unlockScreen();
                }
                else {
                    var messages = $.parseJSON(data)["Messages"];
                    //console.log($.parseJSON(data)["Messages"])
                    //var summaryMessage = true
                    $(messages).each(function (key, value) {

                        if (value["Key"] == "summary") {
                            $("#summary-cation").text(value["Value"]).removeClass("hide");
                            summaryMessage = false

                        } else {

                            $("[name='" + value["Key"] + "']").nextAll(".caution").text(value["Value"]).removeClass("hide");
                        }
                    })
                    if ($.isTrueString($.parseJSON(data)["PassDisabled"])) {
                        $("#Pass").addClass("hide");
                        $("#passcode").addClass("hide");
                    }

                    mgf.unlockScreen();

                }
            } catch (ex) {

                mgf.unlockScreen();

            }
        }).fail(function (data, textStatus, jqXHR) {

            //$("#caution").text("");
            //$("#caution").append($("<p>" + "登録処理に失敗しました。5秒後に自動的に再読み込みします。お手数ですが再読み込み後に再度お試しください。" + "</p>"))
            //$("#caution").removeClass("hide");

        }).always(function (data, textStatus, jqXHR) {
            mgf.unlockScreen();
        });
        return false;
    });
    mgf.prohibitHistoryBack();

})();