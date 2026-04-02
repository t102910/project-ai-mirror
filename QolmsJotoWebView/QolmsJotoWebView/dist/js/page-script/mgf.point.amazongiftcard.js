// mgf.point.amazongiftcard 名前空間オブジェクト
mgf.point.amazongiftcard = mgf.point.amazongiftcard || {};

// DOM 構築完了
mgf.point.amazongiftcard = (function () {
    // ヒストリー バック禁止
    mgf.prohibitHistoryBack();

    let confirmationmodal = new bootstrap.Modal($("#confirmation-modal"));
    let finishmodal = null;
    let copymodal = new bootstrap.Modal($("#copy-modal"));

    if ($("#finish-modal").length > 0) {

        finishmodal = new bootstrap.Modal($("#finish-modal"));
        finishmodal.show();
    }

    // チャージボタン
    $("main").on("click", "a.exchange", function (e) {
        $("#confirmation-modal .modal-body").text($(this).text());
        $("#confirmation-modal .btn-submit").data("itemid", $(this).data("itemid"));

        confirmationmodal.show();

        return false;
    })

    //確認モーダル「交換する」
    $("main").on("click", "#confirmation-modal .btn-submit", function () {
        // 画面をロック
        mgf.lockScreen();
        $.ajax({
            type: "POST",
            url: "../point/AmazonGiftCardResult",
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
                    //$("#finish-modal .modal-body").text($.parseJSON(data)["Message"]);
                    //$("#finish-modal").modal("show");

                    //$("#caution").addClass("hide");
                    location.href = "../point/AmazonGiftCard" ;

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
            confirmationmodal.hide();
        });
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
        copymodal.show();
    });

    $("main").on("shown.bs.modal", "#copy-modal", function (e) {
        setTimeout(function () {
            copymodal.hide();
        }, 300);
        //alert("");
    });

})();
