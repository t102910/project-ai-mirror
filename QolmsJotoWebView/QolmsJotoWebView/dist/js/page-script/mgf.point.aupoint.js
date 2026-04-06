// mgf.point.aupoint 名前空間オブジェクト
mgf.point.aupoint = mgf.point.aupoint || {};

// DOM 構築完了
mgf.point.aupoint = (function () {
    // ヒストリー バック禁止
    mgf.prohibitHistoryBack();

    let confirmationmodal = new bootstrap.Modal($("#confirmation-modal"));
    let finishmodal = null;

    if ($("#finish-modal").length > 0) {

        finishmodal = new bootstrap.Modal($("#finish-modal"));
        finishmodal.show();
    }


    //確認モーダル「交換する」
    $("main").on("click", "#confirmation-modal .btn-submit", function () {
        // 画面をロック
        mgf.lockScreen();
        $.ajax({
            type: "POST",
            url: "../Point/aupointResult",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                itemid: $(this).data("itemid")
            },
            async: true,
            beforeSend: function (jqXHR) {
                mgf.point.checkSession();
            }
        }).done(function (data, textStatus, jqXHR) {
            try {
                //console.log(data)

                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {
                    //成功した時の表示
                    //console.log("success")
                    $("#finish-modal .modal-body").text("Pontaポイント交換が成功しました。");
                    $("#finish-modal").modal("show");

                    $("#caution").addClass("hide");

                }
                else {
                    //console.log("caution")
                    //console.log($.parseJSON(data)["Message"])
                    //console.log($("#caution"))
                    $("#caution").text("");
                    $("#caution").text("Pontaポイント交換が失敗しました。");
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
    $("main").on("click", "a.exchange", function (e) {
        $("#confirmation-modal .modal-body").text($(this).text());
        $("#confirmation-modal .btn-submit").data("itemid", $(this).data("itemid"));

        confirmationmodal.show();

        return false;
    })


    //完了モーダル
    // 「×」ボタン
    $("main").on("click", "#finish-modal div.modal-header button.close", function () {
        try {

            finishmodal.hide();
            scrolltoHist();
        } catch (ex) {
            //console.log(ex);
        }
    });

    // 「閉じる」ボタン
    $("main").on("click", "#finish-modal div.modal-footer button.btn-close", function () {
        try {

            finishmodal.hide();
            scrolltoHist();
        } catch (ex) {
            //console.log(ex);
        }
    });

    //confirmationモーダル
    // 「×」ボタン
    $("main").on("click", "#confirmation-modal div.modal-header button.close", function () {
        try {

            confirmationmodal.hide();
            scrolltoHist();
        } catch (ex) {
            //console.log(ex);
        }
    });

    // 「閉じる」ボタン
    $("main").on("click", "#confirmation-modal div.modal-footer button.btn-close", function () {
        try {

            confirmationmodal.hide();
            scrolltoHist();
        } catch (ex) {
            //console.log(ex);
        }
    });

})();