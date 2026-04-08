// mgf.portal.localidverificationregister 名前空間オブジェクト
mgf.portal.localidverificationregister = mgf.portal.localidverificationregister || {};

// DOM 構築完了
mgf.portal.localidverificationregister = (function () {
    
    $("body").on("click", ".edit", function () {

        $(".caution").addClass("hide");
        mgf.lockScreen();

        //var pass = $('input[name="pass"]').val();
        ////console.log(pass);

        //var len = $(".add").length;
        //var value = {};
        //var result = {};
        //$(".add").each(function (key, value) {

        //    //    console.log($(value).prop("name"));
        //    //    console.log(value)
        //    var name = $(value).prop("name");
        //    var val = $(value).val();

        //    value[name] = val;

        //    //console.log(key + ' : ' + name + ":"+ val);

        //    if (result[name]) {
        //        if (Array.isArray(result[name])) {
        //            result[name].push(val);
        //        }
        //        else {
        //            result[name] = new Array(result[name], val);
        //        }
        //    }
        //    else {
        //        result[name] = val;
        //    }
        //});

        var mail = $('input[name="model.MailAddress"]').val();
        var phone = $('input[name="model.PhoneNumber"]').val();
        console.log(mail);
        console.log(phone);

        var checked = 0
        $('#connection-data :checked').each(function () {
            //値を取得
            var val = $(this).data("content");
            console.log(val);
            checked = checked + val;
        });

        console.log(checked);

        $.ajax({
            type: "POST",
            url: "../Portal/LocalIdVerificationRegisterResult",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                MailAddress: mail,
                PhoneNumber: phone,
                RelationContentFlags: checked
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
                    location.href = "../portal/LocalIdVerificationRequest";

                } else {

                    var messages = $.parseJSON(data)["Messages"];
                    console.log($.parseJSON(data)["Messages"])

                    var summaryMessage = true
                    $(messages).each(function (key, value) {

                        if (value["Key"] == "summary") {
                            $("#summary-cation").text(value["Value"]).removeClass("hide");
                            summaryMessage = false

                        } else {

                            $("[name='" + value["Key"] + "']").next(".caution").text(value["Value"]).removeClass("hide");
                        }
                    })
                    //if (summaryMessage) {
                    //    $("#summary-cation").text("入力内容を確認してください。").removeClass("hide");

                    //}
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

    $("body").on("click", "#cancel-modal .close", function () {
        $("#cancel-modal").modal("hide");
    })

    $("body").on("click", "#cancel-modal .btn-close", function () {
        $("#cancel-modal").modal("hide");
    })

    function cancel() {

        mgf.lockScreen();

        $.ajax({
            type: "POST",
            url: "../Portal/LocalIdVerificationRegisterCancelResult",
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
                    location.href = "../portal/LocalIdVerification"

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

    //開示許可の基本情報は必須項目
    $("main").on("click", "#Information", function () {

        return false;
    });
    // ヒストリー バック禁止
    mgf.prohibitHistoryBack();

})();