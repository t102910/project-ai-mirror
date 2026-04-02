// mgf.portal.userinfomationedit 名前空間オブジェクト
mgf.portal.userinfomationedit = mgf.portal.userinfomationedit || {};

// DOM 構築完了
mgf.portal.userinfomationedit = (function () {

    // ヒストリー バック禁止
    mgf.prohibitHistoryBack();

    function Request(family, given, familykana, givenkana, sex, year, month, day, mail, prefradio, prefecturesval, phone, identityflag) {
        mgf.lockScreen()

        console.log(family);
        console.log(given);
        console.log(sex);
        console.log(year);
        console.log(month);
        console.log(day);
        console.log(mail);
        console.log(prefradio);
        console.log(prefecturesval);
        console.log(phone);

        $.ajax({
            type: "POST",
            url: "../Portal/UserInfomationEditResult",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                FamilyName: family,
                GivenName: given,
                FamilyKanaName: familykana,
                GivenKanaName: givenkana,
                SexType: sex,
                BirthYear: year,
                BirthMonth: month,
                BirthDay: day,
                MailAddress: mail,
                IdentityUpdateFlag: identityflag,
                Prefectures: prefradio,
                CityNo: prefecturesval,
                PhoneNo: phone
            },
            async: true,
            beforeSend: function (jqXHR) {
                mgf.portal.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {
            //console.log(data)
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {
                    //成功した時の表示
                    //console.log("Request:success");
                    //alert("ssuccess");

                    $("#caution").addClass("hide");
                    //参照画面へ遷移
                    location.href = "../portal/userinfomation";

                }
                else {
                    var messages = $.parseJSON(data)["Massages"];

                    var summaryMessage = true
                    $(messages).each(function (key, value) {

                        if (value["Key"] == "summary") {
                            $("#summary-cation").text(value["Value"]).removeClass("hide");
                            summaryMessage = false

                        } else {
                            console.log(value["Key"]);
                            console.log(value["Value"]);
                            console.log($("[name='" + value["Key"] + "']").nextAll(".caution").text(value["Value"]).removeClass("hide"));
                            $("[name='" + value["Key"] + "']").nextAll(".caution").text(value["Value"]).removeClass("hide");
                        }
                    })
                    if (summaryMessage) {
                        $("#summary-cation").text("入力内容を確認してください。").removeClass("hide");

                    }
                    mgf.unlockScreen();

                }
            } catch (ex) {

                console.log(ex.message);
                mgf.unlockScreen();

            }
        }).fail(function (data, textStatus, jqXHR) {

            console.log("fail");
            console.log(data);
            console.log(textStatus);
            console.log(jqXHR);

            mgf.unlockScreen();

        });
        return false;

    };


    $("main").on("click", ".submit-area .btn-submit", function () {
        mgf.lockScreen();

        var family = $("#family-name").val();
        var given = $("#given-name").val();
        var familykana = $("#family-kana-name").val();
        var givenkana = $("#given-kana-name").val();
        var sex = $("#sex").data("value");
        var year = $("#birthday").data("birthyear");
        var month = $("#birthday").data("birthmonth");
        var day = $("#birthday").data("birthday");
        var mail = $("#mail").val();
        console.log($("#prefectures"));
        var prefradio = $("#prefectures input[type='radio']:checked").val()
        var prefecturesval = $("#city").val();
        var phone = $("#phone").val();

        //var checked = 0

        //$('#connection-data :checked').each(function () {
        //    //値を取得
        //    var val = $(this).data("content");
        //    console.log(val);
        //    checked = checked + val;
        //});

        //$.ajax({
        //    type: "POST",
        //    url: "../Portal/userinfomationResult?ActionSource=IsIdentityChecked",
        //    data: {
        //        __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
        //        FamilyName: family,
        //        GivenName: given,
        //        FamilyKanaName: familykana,
        //        GivenKanaName: givenkana,
        //        SexType: sex,
        //        BirthYear: year,
        //        BirthMonth: month,
        //        BirthDay: day,
        //        MailAddress: mail
        //    },
        //    async: true,
        //    beforeSend: function (jqXHR) {
        //        mgf.portal.checkSessionByAjax();
        //    }
        //}).done(function (data, textStatus, jqXHR) {
        //    //console.log(data)
        //    try {
        //        if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {
        //            //成功した時の表示
        //            //console.log("IsIdentityChecked:success")
        //            $(".caution").addClass("hide");

        //            var messages = $.parseJSON(data)["Massages"];
        //            console.log(messages);
        //            console.log(messages.length);
        //            if (messages.length == 0) {
        //                //そのまま更新

        //            } else {

        //                //メッセージボックス
        //                //一致しなかったので更新の確認
        //                //「入力された個人情報が登録と一致しませんでした。個人情報更新しますか？」
        //                $("#identity-modal").modal("show");
        //                mgf.unlockScreen();
        //            }
        //        }
        //        else {

        //            console.log("else");

        //            //名前しか変更の許可をしない
        //            //性別、生年月日の変更があった場合はエラーとして処理
        //            var messages = $.parseJSON(data)["Massages"];
        //            //console.log("Request:Error")
        //            //console.log(messages);

        //            $(messages).each(function (key, value) {
        //                console.log(value["Key"]);
        //                console.log(value["Value"]);

        //                $("#summary-cation").text(value["Value"]);
        //            })

        //            $("#summary-cation").removeClass("hide");
        //            mgf.unlockScreen();


        //        }
        //    } catch (ex) {

        //        mgf.unlockScreen();

        //    }
        //}).fail(function (data, textStatus, jqXHR) {

        //    //$("#caution").text("");
        //    //$("#caution").append($("<p>" + "登録処理に失敗しました。5秒後に自動的に再読み込みします。お手数ですが再読み込み後に再度お試しください。" + "</p>"))
        //    //$("#caution").removeClass("hide");

        //    //mgf.unlockScreen();
        //    //var timer = setTimeout(function () {
        //    //    location.href = "../Portal/AlkooConnection"
        //    //}, 5000);
        //});
        Request(family, given, familykana, givenkana, sex, year, month, day, mail, prefradio, prefecturesval, phone, false)
        return false;

    });

    $("main").on("click", "#identity-modal .btn-submit", function () {
        $("#identity-modal").modal("hide");

        var family = $("#family-name").val();
        var given = $("#given-name").val();
        var familykana = $("#family-kana-name").val();
        var givenkana = $("#given-kana-name").val();
        var sex = $("#sex").data("value");
        var year = $("#birthday").data("birthyear");
        var month = $("#birthday").data("birthmonth");
        var day = $("#birthday").data("birthday");
        var mail = $("#mail").val();

        var checked = 0

        $('#connection-data :checked').each(function () {
            //値を取得
            var val = $(this).data("content");
            console.log(val);
            checked = checked + val;
        });

        Request(family, given, familykana, givenkana, sex, year, month, day, mail, checked, prefradio, prefecturesval, phone, true)

        return false;

    });

    $("main").on("click", "#identity-modal .close", function () {
        $("#identity-modal").modal("hide");
    })

    $("main").on("click", "#identity-modal .btn-close", function () {
        $("#identity-modal").modal("hide");
    })

    $("main").on("click", "#Information", function () {

        return false;
    });

    mgf.prohibitHistoryBack();

})();