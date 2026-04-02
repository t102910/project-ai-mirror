//  名前空間オブジェクト
mgf.portal.companyconnectionrequest = mgf.portal.companyconnectionrequest || {};

mgf.portal.companyconnectionrequest = (function () {

    function request(identityflag) {

        mgf.lockScreen()

        $(".caution").addClass("hide");

        var facilityid = $("#code").val();
        var employeeno = $("#id").val();
        var family = $("#family-name").val();
        var given = $("#given-name").val();
        var familykana = $("#family-kana-name").val();
        var givenkana = $("#given-kana-name").val();
        var sex = $("#sex").data("value");
        var year = $("#birthday").data("birthyear");
        var month = $("#birthday").data("birthmonth");
        var day = $("#birthday").data("birthday");
        var checked = 0
        $('#connection-data :checked').each(function () {
            //値を取得
            var val = $(this).data("content");
            console.log(val);
            checked = checked + val;
        });

        $.ajax({
            type: "POST",
            url: "../Portal/CompanyConnectionRequestResult?ActionSource=Request",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                FacilityId: facilityid,
                EmployeeNo: employeeno,
                FamilyName: family,
                GivenName: given,
                FamilyKanaName: familykana,
                GivenKanaName: givenkana,
                SexType: sex,
                BirthYear: year,
                BirthMonth: month,
                BirthDay: day,
                IdentityUpdateFlag: identityflag,
                RelationContentFlags: checked
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
                    //console.log("success")
                    //console.log($.parseJSON(data)["Message"])
                    //console.log($.parseJSON(data)["LinkageSystemNo"])

                    var str = "../Portal/CompanyConnection?linkagesystemno=" + $.parseJSON(data)["LinkageSystemNo"] + "&frompageno=" + $(".home-btn-wrap").data("pageno")


                    location.href = str;


                }
                else {

                    var messages = $.parseJSON(data)["Massages"];
                    console.log($.parseJSON(data)["Massages"])

                    var summaryMessage = true
                    $(messages).each(function (key, value) {

                        if (value["Key"] == "summary") {
                            $("#summary-cation").text(value["Value"]).removeClass("hide");
                            summaryMessage = false

                        } else {

                            $("[name='" + value["Key"] + "']").nextAll(".caution").text(value["Value"]).removeClass("hide");
                        }
                    })
                    if (summaryMessage) {
                        $("#summary-cation").text("入力内容を確認してください。").removeClass("hide");

                    }
                    mgf.unlockScreen();

                }
            } catch (ex) {

                mgf.unlockScreen();

            }
        }).fail(function (data, textStatus, jqXHR) {

            $("#caution").text("");
            $("#caution").append($("<p>" + "登録処理に失敗しました。5秒後に自動的に再読み込みします。お手数ですが再読み込み後に再度お試しください。" + "</p>"))
            $("#caution").removeClass("hide");

            //mgf.unlockScreen();
            //var timer = setTimeout(function () {
            //    location.href = "../Portal/AlkooConnection"
            //}, 5000);
        });
        return false;
    }


    function IdentityChangedCheck() {

        mgf.lockScreen()

        $(".caution").addClass("hide");

        var facilityid = $("#code").val();
        var employeeno = $("#id").val();
        var family = $("#family-name").val();
        var given = $("#given-name").val();
        var familykana = $("#family-kana-name").val();
        var givenkana = $("#given-kana-name").val();
        var sex = $("#sex").data("value");
        var year = $("#birthday").data("birthyear");
        var month = $("#birthday").data("birthmonth");
        var day = $("#birthday").data("birthday");
        var checked = 0
        $('#connection-data :checked').each(function () {
            //値を取得
            var val = $(this).data("content");
            console.log(val);
            checked = checked + val;
        });

        $.ajax({
            type: "POST",
            url: "../Portal/CompanyConnectionRequestResult?ActionSource=IsIdentityChecked",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                FacilityId: facilityid,
                EmployeeNo: employeeno,
                FamilyName: family,
                GivenName: given,
                FamilyKanaName: familykana,
                GivenKanaName: givenkana,
                SexType: sex,
                BirthYear: year,
                BirthMonth: month,
                BirthDay: day,
                RelationContentFlags: checked
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
                    //console.log("IsIdentityChecked:success")
                    $(".caution").addClass("hide");

                    var messages = $.parseJSON(data)["Massages"];
                    console.log(messages);
                    console.log(messages.length);

                    if (messages.length == 0) {
                        //Identityモーダルなしで次へ
                        if ($("#disconnect-modal").length > 0) {
                            $("#disconnect-modal").modal("show");

                        } else {
                            request(false);

                        }

                    } else {

                        //メッセージボックス
                        //一致しなかったので更新の確認
                        //「入力された個人情報が登録と一致しませんでした。個人情報更新しますか？」
                        $("#identity-modal").modal("show");
                    }
                }
                else {

                    var messages = $.parseJSON(data)["Massages"];
                    console.log($.parseJSON(data)["Massages"])

                    var summaryMessage = true
                    $(messages).each(function (key, value) {

                        if (value["Key"] == "summary") {
                            $("#summary-cation").text(value["Value"]).removeClass("hide");
                            summaryMessage = false

                        } else {

                            $("[name='" + value["Key"] + "']").nextAll(".caution").text(value["Value"]).removeClass("hide");
                        }
                    })
                    if (summaryMessage) {
                        $("#summary-cation").text("入力内容を確認してください。").removeClass("hide");

                    }
                    mgf.unlockScreen();

                }
            } catch (ex) {

                mgf.unlockScreen();

            }
        }).fail(function (data, textStatus, jqXHR) {

            $("#caution").text("");
            $("#caution").append($("<p>" + "登録処理に失敗しました。5秒後に自動的に再読み込みします。お手数ですが再読み込み後に再度お試しください。" + "</p>"))
            $("#caution").removeClass("hide");

            //mgf.unlockScreen();
            //var timer = setTimeout(function () {
            //    location.href = "../Portal/AlkooConnection"
            //}, 5000);
        }).always(function () {

            //ロックスクリーンの解除
            mgf.unlockScreen();
        });
        return false;
    }

    if ($("#disconnect-modal").length > 0) {
        //プレミアム会員の場合

        $("main").on("click", "#request", function () {

            IdentityChangedCheck();
        });

        $("main").on("click", "#disconnect-modal .btn-delete", function () {

            $("#disconnect-modal").modal("hide");
            request();
        });

        $("main").on("click", "#disconnect-modal .btn-close", function () {

            $("#disconnect-modal").modal("hide");

        });

        $("main").on("click", "#disconnect-modal .close", function () {

            $("#disconnect-modal").modal("hide");

        });
    } else {

        //一般会員の場合
        $("main").on("click", "#request", function () {

            IdentityChangedCheck();
        });

    }

    // premium の場合 
    // submit → identity check → premum check → Register
    //                          → identity modal → premum check → Register

    // 一般会員 の場合 
    // submit → identity check → Register
    //                          → identity modal → Register

    // 個人情報変更ダイアログ
    $("main").on("click", "#identity-modal .btn-submit", function () {

        $("#identity-modal").modal("hide");
        if ($("#disconnect-modal").length > 0) {
            $("#disconnect-modal").modal("show");

        } else {
            request(true);

        }
    });

    $("main").on("click", "#identity-modal .btn-close", function () {

        $("#identity-modal").modal("hide");

    });

    $("main").on("click", "#identity-modal .close", function () {

        $("#identity-modal").modal("hide");

    });


    $("main").on("click", "#Information", function () {

        return false;
    });


    $("#code").blur(function () {
        console.log("blur");
        if ($("#code").val() == "47500107") {
            $("#Vital").addClass("hide");
            $("[for=Vital]").addClass("hide");
        } else {
            $("#Vital").removeClass("hide");
            $("[for=Vital]").removeClass("hide");
        }
    });

    mgf.prohibitHistoryBack();

})();