//  名前空間オブジェクト
mgf.passwordreset.recoveryidentifier = mgf.passwordreset.recoveryidentifier || {};

mgf.passwordreset.recoveryidentifier = (function () {

    var completionView = function () {

        $.ajax({
            type: "POST",
            url: "../passwordreset/RecoveryIdentifierResult?ActionSource=Completion",
            data: {
                __editVerificationToken: $("input[name='__RequestVerificationToken']").val(),
            },
            async: true,
            beforeSend: function (jqXHR) {
                //mgf.passwordreset.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {
            //console.log(data)
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("text/html;") >= 0) {
                    //成功した時の表示
                    console.log(data)
                    console.log(jqXHR)

                    //成功時のパーシャルビューで置き換える
                    $(".contents-area div").replaceWith($(data));
                    mgf.unlockScreen();

                }
                else {
                    //なんでもいいから適当にログイン画面にとばす
                    location.href = "../start/login"
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

        });
        return false;
    }


    $("main").on("click", "#request", function () {
        mgf.lockScreen()

        $(".caution").addClass("hide");

        var resetkey = $("#password-reset-key").val();
        console.log(resetkey);

        var jotoid = $("#jotoid").val();
        console.log(jotoid);
        var family = $("#family-name").val();
        console.log(family);
        var given = $("#given-name").val();
        console.log(given);
        var sex = $("#sex").val();
        console.log(sex);
        var year = $("#birth-year").val();
        console.log(year);
        var month = $("#birth-month").val();
        console.log(month);
        var day = $("#birth-day").val();
        console.log(day);

        var mail = $("#mail").val();
        console.log(mail);


        $.ajax({
            type: "POST", 
            url: "../passwordreset/RecoveryIdentifierResult?ActionSource=Identifier",
            data: {
                __editVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                PasswordResetKey: resetkey,
                JotoId: jotoid,
                FamilyName: family,
                GivenName: given,
                Sex: sex,
                BirthYear: year,
                BirthMonth: month,
                BirthDay: day,
                MailAddress: mail
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
                    completionView()
                    mgf.unlockScreen();

                    //location.href = "../Portal/CompanyConnection?linkagesystemno=" + $.parseJSON(data)["LinkageSystemNo"]+"&fromPageNo="+ $(".home-btn-wrap").data("pageno");
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

        });
        return false;

    });

    //$("main").on("click", "#Information", function () {

    //    return false;
    //});

    mgf.prohibitHistoryBack();

})();