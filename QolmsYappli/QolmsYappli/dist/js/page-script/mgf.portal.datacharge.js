// mgf.portal.datacharge 名前空間オブジェクト
mgf.portal.datacharge = mgf.portal.datacharge || {};

// DOM 構築完了
mgf.portal.datacharge = (function () {
    // ヒストリー バック禁止
    mgf.prohibitHistoryBack();

    //確認モーダル「交換する」
    $("main").on("click", "#confirmation-modal .btn-submit", function () {
        // 画面をロック
        mgf.lockScreen();
        $.ajax({
            type: "POST",
            url: "../Portal/DatachargeResult",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                capacity: $(this).data("capacity")
            },
            async: true,
            beforeSend: function (jqXHR) {
                mgf.portal.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {
            try {
                //console.log(data)

                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {
                    //成功した時の表示
                    //console.log("success")
                    $("#finish-modal .modal-body").text($.parseJSON(data)["Message"]);
                    $("#finish-modal").modal("show");

                    $("#caution").addClass("hide");

                }
                else {
                    //console.log("caution")
                    //console.log($.parseJSON(data)["Message"])
                    //console.log($("#caution"))
                    $("#caution").text("");
                    $("#caution").text($.parseJSON(data)["Message"]);
                    $("#caution").removeClass("hide");
                }
            } catch (ex) {
                //console.log(ex.message)
            }

        }).always(function (jqXHR, textStatus) {
            // 画面をアンロック
            $("#confirmation-modal").modal("hide");
            mgf.unlockScreen();
        });
        return false;
    })

    // チャージボタン
    $("main").on("click", "a.charge", function (e) {
        //console.log("click");
        $("#confirmation-modal .modal-body").text($(this).text());
        $("#confirmation-modal .btn-submit").data("capacity", $(this).data("capacity"));
        //console.log($("#confirmation-modal .btn-submit").data("capacity"));
        $("#confirmation-modal").modal("show");
        return false;

    })

    //完了モーダル
    // 「×」ボタン
    $("main").on("click", "#finish-modal div.modal-header button.close", function () {
        try {
            $("#finish-modal").modal("hide");
            location.href = "../Portal/Datacharge" + $("#back").data("back");
        } catch (ex) {
            //console.log(ex);
        }
    });

    // 「閉じる」ボタン
    $("main").on("click", "#finish-modal div.modal-footer button.btn-close", function () {
        try {
            $("#finish-modal").modal("hide");
            location.href = "../Portal/Datacharge" + $("#back").data("back");
        } catch (ex) {
            //console.log(ex);
        }
    });

    //// 「デジラアプリ」ボタン
    //$("main").on("click", "#dejira", function () {
    //    try {
    //        $("#finish-modal").modal("hide");
    //        var user = getUserType()
    //        if (user == "Android") {
    //            location.href = "https://c00.adobe.com/v3/ef3cb975be7f49b91417bc7916bd4780c937f27660fd54eab839479178dcca3b/start?a_dl=5c73752ab56360dc10eadebd";
    //        }
    //        else {
    //            location.href = "https://c00.adobe.com/v3/ef3cb975be7f49b91417bc7916bd4780c937f27660fd54eab839479178dcca3b/start?a_dl=5c7375bfb56360dc10eadec0";
    //        }
    //    } catch (ex) {
    //        console.log(ex);
    //    }
    //});

    ////ユーザーエージェント
    //function getUserType() {
    //    var ua = [
    //        "iPod",
    //        "iPad",
    //        "iPhone",
    //        "Android"
    //    ]

    //    for (var i = 0; i < ua.length; i++) {
    //        if (navigator.userAgent.indexOf(ua[i]) > 0) {
    //            return ua[i]
    //        }
    //    }
    //    return "Other"
    //}

})();