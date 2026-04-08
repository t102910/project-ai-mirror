// mgf.integration.hospitalconnection 名前空間オブジェクト
mgf.integration = mgf.integration || {};
mgf.integration.hospitalconnection = mgf.integration.hospitalconnection || {};

mgf.integration.hospitalconnection = (function () {
    // 遷移元画面番号を保持している場合のみ、遷移先URLに引き継ぐ。
    function fromPageSuffix() {
        var fromPageNo = $("section.home-btn-wrap").data("pageno");
        return (fromPageNo !== undefined && fromPageNo !== null && fromPageNo !== "")
            ? "&fromPageNo=" + encodeURIComponent(fromPageNo)
            : "";
    }

    // 解除/申請取り消しボタンクリックで確認ダイアログのメッセージを設定して表示する。
    $("main").on("click", "#cancel", function () {
        var btn = $(this).text() || "解除する";
        var message = $(this).hasClass("request") ? "申請を取り消しますか？" : "連携を解除しますか？";

        $("#delete-modal .modal-body").text(message);
        $("#delete-modal .btn-delete").text(btn);
        $("#delete-modal").modal("show");
        return false;
    });

    // 編集ボタンクリックで病院連携申請画面へ遷移する。
    $("main").on("click", "#edit", function () {
        var linkageSystemNo = $(this).data("no");
        var url = "../Integration/HospitalConnectionRequest?linkageSystemNo=" + encodeURIComponent(linkageSystemNo) + fromPageSuffix();

        mgf.portal.promiseLockScreen($(this)).then(function () {
            location.href = url;
        });

        return false;
    });

    // 確認ダイアログの解除実行ボタンクリックで解除APIを呼び、成功時は連携設定へ戻る。
    $("main").on("click", "#delete-modal .btn-delete", function () {
        var linkageSystemNo = $("#cancel").data("no");

        $("#delete-modal").modal("hide");

        mgf.lockScreen();
        $.ajax({
            type: "POST",
            url: "../Integration/HospitalConnectionResult?ActionSource=Delete",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                linkageSystemNo: linkageSystemNo
            },
            async: true,
            beforeSend: function () {
                mgf.portal.checkSessionByAjax();
            }
        }).done(function (data) {
            var json = (typeof data === "string") ? $.parseJSON(data) : data;

            if ($.isTrueString(json.IsSuccess)) {
                location.href = "../Portal/ConnectionSetting?tabNo=3" + fromPageSuffix();
                return;
            }

            var messages = json.Messages || {};
            $("#caution").removeClass("hide").find("h4").next().remove();
            $("#caution").append(document.createTextNode(messages.summary || "連携解除に失敗しました。"));
            mgf.unlockScreen();
        }).fail(function () {
            $("#caution").removeClass("hide").find("h4").next().remove();
            $("#caution").append(document.createTextNode("連携解除に失敗しました。時間を置いて再度お試しください。"));
            mgf.unlockScreen();
        });

        return false;
    });

    // 確認ダイアログの閉じるボタンクリックでダイアログを閉じる。
    $("main").on("click", "#delete-modal .btn-close, #delete-modal .close", function () {
        $("#delete-modal").modal("hide");
        return false;
    });

    mgf.prohibitHistoryBack();

    return {};
})();
