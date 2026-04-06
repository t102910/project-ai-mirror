// mgf.integration.companyconnectionedit 名前空間オブジェクト
mgf.integration = mgf.integration || {};
mgf.integration.companyconnectionedit = mgf.integration.companyconnectionedit || {};

mgf.integration.companyconnectionedit = (function () {
    // 遷移元画面番号を保持している場合のみ、遷移先URLに引き継ぐ。
    function fromPageSuffix() {
        var fromPageNo = $("section.home-btn-wrap").data("pageno");
        return (fromPageNo !== undefined && fromPageNo !== null && fromPageNo !== "")
            ? "&fromPageNo=" + encodeURIComponent(fromPageNo)
            : "";
    }

    // チェックされた開示許可をフラグ値として合算する。
    function buildRelationContentFlags() {
        var checked = 0;
        $("#connection-data :checked").each(function () {
            checked += parseInt($(this).data("content"), 10) || 0;
        });
        return checked;
    }

    // 画面下部の要約メッセージ領域にエラーを表示する。
    function setSummaryMessage(message) {
        $("#summary-cation").text(message).removeClass("hide");
    }

    $("main").on("click", "#request", function () {
        var linkageSystemNo = $(this).data("no");
        var linkageSystemName = $(this).data("linkagesystemname") || "企業";

        $("#summary-cation").addClass("hide");
        mgf.lockScreen();

        // 編集内容をサーバーへPOSTし、成功時は連携詳細へ戻す。
        $.ajax({
            type: "POST",
            url: "../Integration/CompanyConnectionEditResult",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                LinkageSystemNo: linkageSystemNo,
                RelationContentFlags: buildRelationContentFlags(),
                MailAddress: $("#mail").val()
            },
            async: true,
            beforeSend: function () {
                mgf.portal.checkSessionByAjax();
            }
        }).done(function (data) {
            var json = (typeof data === "string") ? $.parseJSON(data) : data;

            if ($.isTrueString(json.IsSuccess)) {
                location.href = "../Integration/CompanyConnection?linkageSystemNo="
                    + encodeURIComponent(json.LinkageSystemNo)
                    + "&linkageSystemName=" + encodeURIComponent(linkageSystemName)
                    + fromPageSuffix();
                return;
            }

            var messages = json.Messages || {};
            setSummaryMessage(messages.summary || "入力内容を確認してください。");
            mgf.unlockScreen();
        }).fail(function () {
            // 通信失敗時は要約エラーのみ表示する。
            setSummaryMessage("登録処理に失敗しました。時間を置いて再度お試しください。");
            mgf.unlockScreen();
        });

        return false;
    });

    mgf.prohibitHistoryBack();

    return {};
})();
