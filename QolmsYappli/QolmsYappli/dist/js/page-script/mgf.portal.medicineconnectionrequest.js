// mgf.portal.medicineconnectionrequest 名前空間オブジェクト
mgf.portal.medicineconnectionrequest = mgf.portal.medicineconnectionrequest || {};

// DOM 構築完了
mgf.portal.medicineconnectionrequest = (function () {
    // ヒストリー バック禁止
    mgf.prohibitHistoryBack();

    function Request(no, facility, pharmacyid, id, family, given, sex, year, month, day, identityflag) {
        mgf.lockScreen()

        console.log(no);
        console.log(facility);
        console.log(pharmacyid);
        console.log(id);
        console.log(family);
        console.log(given);
        console.log(sex);
        console.log(year);
        console.log(month);
        console.log(day);

        $.ajax({
            type: "POST",
            url: "../Portal/MedicineConnectionRequestResult?ActionSource=Request",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                LinkageSystemNo: no,
                Facilitykey: facility,
                PharmacyId: pharmacyid,
                PatientCardNo: id,
                FamilyName: family,
                GivenName: given,
                SexType: sex,
                BirthYear: year,
                BirthMonth: month,
                BirthDay: day,
                IdentityUpdateFlag: identityflag
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
                    //console.log("Request:success")

                    $("#caution").addClass("hide");
                    console.log(facility);
                    console.log(facility);

                    location.href = "../Portal/MedicineConnection?LinkageSystemNo=" + no + "&Facilitykey=" + facility + "&fromPageNo=" + $(".home-btn-wrap").data("pageno");
                }
                else {
                    var messages = $.parseJSON(data)["Massages"];

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

                //console.log(ex.message);
                mgf.unlockScreen();

            }
        }).fail(function (data, textStatus, jqXHR) {

            //$("#caution").text("");
            //$("#caution").append($("<p>" + "登録処理に失敗しました。5秒後に自動的に再読み込みします。お手数ですが再読み込み後に再度お試しください。" + "</p>"))
            //$("#caution").removeClass("hide");

            //mgf.unlockScreen();
            //var timer = setTimeout(function () {
            //    location.href = "../Portal/AlkooConnection"
            //}, 5000);
            //console.log("fail");

            mgf.unlockScreen();

        });
        return false;

    };


    $("main").on("click", ".submit-area .btn-submit", function () {
        mgf.lockScreen();

        var no = $("#facility").data("linkage");
        var facility = $("#facility").data("facilitykey");
        var pharmacyid = $("#facility").data("pharmacyid");
        var id = $("#id").val();
        var family = $("#family-name").val();
        var given = $("#given-name").val();
        var sex = $("#sex").data("value");
        var year = $("#birthday").data("birthyear");
        var month = $("#birthday").data("birthmonth");
        var day = $("#birthday").data("birthday");

        console.log(no);
        console.log(facility);
        console.log(pharmacyid);
        console.log(id);
        console.log(family);
        console.log(given);
        console.log(sex);
        console.log(year);
        console.log(month);
        console.log(day);
        
        $.ajax({
            type: "POST",
            url: "../Portal/MedicineConnectionRequestResult?ActionSource=IsIdentityChecked",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                LinkageSystemNo: no,
                Facilitykey: facility,
                PharmacyId: pharmacyid,
                PatientCardNo: id,
                FamilyName: family,
                GivenName: given,
                SexType: sex,
                BirthYear: year,
                BirthMonth: month,
                BirthDay: day
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
                        //そのまま更新
                        Request(no,facility,pharmacyid, id, family, given, sex, year, month, day, false)

                    } else {

                        //メッセージボックス
                        //一致しなかったので更新の確認
                        //「入力された個人情報が登録と一致しませんでした。個人情報更新しますか？」
                        $("#identity-modal").modal("show");
                        mgf.unlockScreen();
                    }
                }
                else {

                    console.log("else");

                    //名前しか変更の許可をしない
                    //性別、生年月日の変更があった場合はエラーとして処理
                    var messages = $.parseJSON(data)["Massages"];
                    //console.log("Request:Error")
                    //console.log(messages);

                    $(messages).each(function (key, value) {
                        console.log(value["Key"]);
                        console.log(value["Value"]);

                        $("#summary-cation").text(value["Value"]);
                    })

                    $("#summary-cation").removeClass("hide");
                    mgf.unlockScreen();


                }
            } catch (ex) {

                mgf.unlockScreen();

            }
        }).fail(function (data, textStatus, jqXHR) {

            //$("#caution").text("");
            //$("#caution").append($("<p>" + "登録処理に失敗しました。5秒後に自動的に再読み込みします。お手数ですが再読み込み後に再度お試しください。" + "</p>"))
            //$("#caution").removeClass("hide");

            //mgf.unlockScreen();
            //var timer = setTimeout(function () {
            //    location.href = "../Portal/AlkooConnection"
            //}, 5000);
        });
        return false;

    });

    $("main").on("click", "#identity-modal .close", function () {
        $("#identity-modal").modal("hide");
    })

    $("main").on("click", "#identity-modal .btn-close", function () {
        $("#identity-modal").modal("hide");
    })

    $("main").on("click", "#identity-modal .btn-submit", function () {

        var no = $("#facility").data("linkage");
        var facility = $("#facility").data("facilitykey");
        var pharmacyid = $("#facility").data("pharmacyid");
        var id = $("#id").val();
        var family = $("#family-name").val();
        var given = $("#given-name").val();
        var sex = $("#sex").data("value");
        var year = $("#birthday").data("birthyear");
        var month = $("#birthday").data("birthmonth");
        var day = $("#birthday").data("birthday");

        Request(no, facility, pharmacyid, id, family, given,sex,year,month,day,true);
        $("#identity-modal").modal("hide");
    })

    $("main").on("click", "#image", function () {
        $("#modal-text").removeClass("hide");
        $('body').addClass('modal-open');
    })
    //モーダル閉じる
    $('body').on("click", '#modal-close', function () {
        $("#modal-text").addClass("hide");
        $('body').removeClass('modal-open');

    });
    

})();