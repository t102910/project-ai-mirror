// mgf.portal.terms 名前空間オブジェクト
mgf.portal.unsubscribe = mgf.portal.unsubscribe || {};

// DOM 構築完了
mgf.portal.unsubscribe = (function () {
    // ヒストリー バック禁止
    mgf.prohibitHistoryBack();

    $("main").on("click", "#all", function () {
        // 確認ダイアログ
        $("#delete-cation-modal").modal("show");

    })

    $("main").on("click", "#premium", function () {
        // 確認ダイアログ
        $("#premium-cation-modal").modal("show");
    })

    //完了モーダル
    // 「×」ボタン
    $("main").on("click", "#finish-modal div.modal-header button.close", function () {
        try {
            $("#finish-modal").modal("hide");
            if ($("#finish-modal").hasClass("logout")) {
                location.href = "../Start/Logout"
            } else {
                location.href = "../portal/unsubscribe"
            }
        } catch (ex) {
            //console.log(ex);
        }
    });

    // 「閉じる」ボタン
    $("main").on("click", "#finish-modal div.modal-footer button.btn-close", function () {
        try {
            $("#finish-modal").modal("hide");
            if ($("#finish-modal").hasClass("logout")) {
                location.href = "../Start/Logout"
            } else {
                location.href = "../portal/unsubscribe"
            }
        } catch (ex) {
            //console.log(ex);
        }
    });

    //同意
    $('#all').addClass('disabled');
    $('main').on('change', '#consent', function () {
        if ($("#consent").prop("checked")) {
            $('#all').removeClass('disabled');
        }
        else {
            $('#all').addClass('disabled');
        }
    })


    //確認モーダル(退会)
    // 「×」ボタン
    $("main").on("click", "#premium-cation-modal div.modal-header button.close", function () {
        try {
            $("#premium-cation-modal").modal("hide");
        } catch (ex) {
            //console.log(ex);
        }
    });

    // 「閉じる」ボタン
    $("main").on("click", "#premium-cation-modal div.modal-footer button.btn-close", function () {
        try {
            $("#premium-cation-modal").modal("hide");
        } catch (ex) {
            //console.log(ex);
        }
    });

    //はい
    $("main").on("click", "#premium-cation-modal .btn-submit", function () {

        mgf.lockScreen()
        //console.log("premium")
        $("#premium-cation-modal").modal("hide");

        $.ajax({
            type: "POST",
            url: "../Portal/UnsubscribeResult?ActionSource=Premium",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val()
            },
            async: true,
            beforeSend: function (jqXHR) {
                mgf.portal.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {
                    //成功した時の表示
                    //console.log("success")
                    //console.log($.parseJSON(data))
                    $("#finish-modal .modal-body").text($.parseJSON(data)["Message"]);
                    //console.log($.isTrueString($.parseJSON(data)["IsLogout"]));
                    if ($.isTrueString($.parseJSON(data)["IsLogout"])) {
                        $("#finish-modal").addClass("logout");
                    }
                    $("#finish-modal").modal("show");

                }
                else {
                    $("#premiumCaution").text("");
                    $("#premiumCaution").append($("<p>" + $.parseJSON(data)["Message"] + "</p>"))
                    $("#premiumCaution").removeClass("hide");
                }
            } catch (ex) { }

        }).always(function (jqXHR, textStatus) {
            // 画面をアンロック
            mgf.unlockScreen();
        });
        return false;

    })

    //確認モーダル(プレミアム会員退会)
    // 「×」ボタン
    $("main").on("click", "#delete-cation-modal div.modal-header button.close", function () {
        try {
            $("#delete-cation-modal").modal("hide");
        } catch (ex) {
            //console.log(ex);
        }
    });

    // 「閉じる」ボタン
    $("main").on("click", "#delete-cation-modal div.modal-footer button.btn-close", function () {
        try {
            $("#delete-cation-modal").modal("hide");
        } catch (ex) {
            //console.log(ex);
        }
    });

    $("main").on("click", "#delete-cation-modal .btn-submit", function () {

        mgf.lockScreen()
        $("#delete-cation-modal").modal("hide");

        var reasonCode = $("#reasonCode").val();
        var reasonComment = $("#reasonComment").val();
        
        var fd = new FormData();

        fd.append("__RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());
        fd.append("ReasonCode", reasonCode)
        fd.append("ReasonComment", reasonComment)
        //console.log(reasonComment)
        //console.log(!reasonComment)
        //console.log(reasonComment == '')
        //console.log(fd)

        $.ajax({
            type: "POST",
            url: "../Portal/UnsubscribeResult?ActionSource=All",
            traditional: true,
            data: $.toJsonObject(fd),
            async: true,
            beforeSend: function (jqXHR) {
                mgf.portal.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {
                    ////成功した時の表示
                    //console.log("success")
                    //console.log($.parseJSON(data))
                    $("#finish-modal .modal-body").text($.parseJSON(data)["Message"]);
                    //console.log($.isTrueString($.parseJSON(data)["IsLogout"]));
                    if ($.isTrueString($.parseJSON(data)["IsLogout"])) {
                        $("#finish-modal").addClass("logout");
                    }
                    $("#finish-modal").modal("show");
                }
                else {

                    $("#caution").text("");
                    $("#caution").append($("<p>" + $.parseJSON(data)["Message"] + "</p>"))
                    $("#caution").removeClass("hide");
                    //alert(caution.text());
                }
            } catch (ex) { }

        }).always(function (jqXHR, textStatus) {
            // 画面をアンロック
            mgf.unlockScreen();
        });
        return false;

    })

})();