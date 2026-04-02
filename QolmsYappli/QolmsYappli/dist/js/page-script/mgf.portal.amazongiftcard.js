// mgf.portal.amazongiftcard 名前空間オブジェクト
mgf.portal.amazongiftcard = mgf.portal.amazongiftcard || {};

// DOM 構築完了
mgf.portal.amazongiftcard = (function () {
    // ヒストリー バック禁止
    mgf.prohibitHistoryBack();

    //$("#finish-modal .modal-body").text($.parseJSON(data)["Message"]);
    if ($("#finish-modal") !== undefined) {
        $("#finish-modal").modal("show");
    }

    // チャージボタン
    $("main").on("click", "a.exchange", function (e) {
        $("#confirmation-modal .modal-body").text($(this).text());
        $("#confirmation-modal .btn-submit").data("itemid", $(this).data("itemid"));
        $("#confirmation-modal").modal("show");
        return false;
    })

    //確認モーダル「交換する」
    $("main").on("click", "#confirmation-modal .btn-submit", function () {
        // 画面をロック
        mgf.lockScreen();
        $.ajax({
            type: "POST",
            url: "../Portal/AmazonPointResult",
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
                    //$("#finish-modal .modal-body").text($.parseJSON(data)["Message"]);
                    //$("#finish-modal").modal("show");

                    //$("#caution").addClass("hide");
                    location.href = "../Portal/amazonpoint" + $("#back").data("back");

                }
                else {
                    //console.log("caution")
                    //console.log($.parseJSON(data)["Message"])
                    //console.log($("#caution"))
                    $("#caution").text("");
                    $("#caution").text($.parseJSON(data)["Message"]);
                    $("#caution").removeClass("hide");
                    mgf.unlockScreen();

                }
            } catch (ex) {
                //console.log(ex.message)
                mgf.unlockScreen();
            }

        }).fail(function (jqXHR, textStatus) {
            mgf.unlockScreen();
        }).always(function (jqXHR, textStatus) {
            // 画面をアンロック
            $("#confirmation-modal").modal("hide");
        });
        return false;
    })

    //完了モーダル
    // 「×」ボタン
    $("main").on("click", "#finish-modal div.modal-header button.close", function () {
        try {
            $("#finish-modal").modal("hide");
            //location.href = "../Portal/PointExchange";
            scrolltoHist()
        } catch (ex) {
            //console.log(ex);
        }
    });

    // 「閉じる」ボタン
    $("main").on("click", "#finish-modal div.modal-footer button.btn-close", function () {
        try {
            $("#finish-modal").modal("hide");
            //location.href = "../Portal/PointExchange";
            scrolltoHist()
        } catch (ex) {
            //console.log(ex);
        }
    });


    function scrolltoHist() {
        var speed = 400;
        var target = $("#hist");
        var position = target.offset().top;
        $('body,html').animate({ scrollTop: position }, speed, 'swing');
        return false;
    };

    //クリップボードコピー
    $("main").on("click", "i.la-copy", function () {
        copyTextToClipboard($(this).data("couponid"));
    });

    /**
     * クリップボードコピー関数
     * 入力値をクリップボードへコピーする
     * [引数]   textVal: 入力値
     * [返却値] true: 成功　false: 失敗
     */
    function copyTextToClipboard(textVal) {
        //console.log(textVal);

        // テキストエリアを用意する
        var copyFrom = document.createElement("textarea");
        // テキストエリアへ値をセット
        copyFrom.textContent = textVal;

        // bodyタグの要素を取得
        var bodyElm = document.getElementsByTagName("body")[0];
        // 子要素にテキストエリアを配置
        bodyElm.appendChild(copyFrom);

        // テキストエリアの値を選択
        copyFrom.select();
        // コピーコマンド発行
        var retVal = document.execCommand('copy');
        // 追加テキストエリアを削除
        bodyElm.removeChild(copyFrom);
        // 処理結果を返却
        return retVal;
    }

    //コピーモーダル
    $("main").on("click", "i.la-copy", function () {
        copyTextToClipboard($(this).data("couponid"));

        //alert("copy!");
        $("#copy-modal").modal("show");
    });

    $("main").on("shown.bs.modal", "#copy-modal", function (e) {
        setTimeout(function () {
            $("#copy-modal").modal("hide");
        }, 300);
        //alert("");
    });

})();
