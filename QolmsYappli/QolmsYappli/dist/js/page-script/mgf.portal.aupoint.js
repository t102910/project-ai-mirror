// mgf.portal.aupoint 名前空間オブジェクト
mgf.portal.aupoint = mgf.portal.aupoint || {};

// DOM 構築完了
mgf.portal.aupoint = (function () {
    // ヒストリー バック禁止
    mgf.prohibitHistoryBack();

    //確認モーダル「交換する」
    $("main").on("click", "#confirmation-modal .btn-submit", function () {
        // 画面をロック
        mgf.lockScreen();
        $.ajax({
            type: "POST",
            url: "../Portal/aupointResult",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                itemid: $(this).data("itemid")
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
        //console.log("click");
        $("#confirmation-modal .modal-body").text($(this).text());
        $("#confirmation-modal .btn-submit").data("itemid", $(this).data("itemid"));
        //console.log($("#confirmation-modal .btn-submit").data("capacity"));
        $("#confirmation-modal").modal("show");
        return false;

    })

    //完了モーダル
    // 「×」ボタン
    $("main").on("click", "#finish-modal div.modal-header button.close", function () {
        try {
            $("#finish-modal").modal("hide");
            location.href = "../Portal/aupoint" + $("#back").data("back");
        } catch (ex) {
            //console.log(ex);
        }
    });

    // 「閉じる」ボタン
    $("main").on("click", "#finish-modal div.modal-footer button.btn-close", function () {
        try {
            $("#finish-modal").modal("hide");
            location.href = "../Portal/aupoint" + $("#back").data("back");
        } catch (ex) {
            //console.log(ex);
        }
    });


})();