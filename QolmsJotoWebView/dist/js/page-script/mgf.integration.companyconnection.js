// mgf.integration.companyconnection 名前空間オブジェクト
mgf.integration = mgf.integration || {};
mgf.integration.companyconnection = mgf.integration.companyconnection || {};

mgf.integration.companyconnection = (function () {
    // 遷移元画面番号を保持している場合のみ、遷移先URLに引き継ぐ。
    function fromPageSuffix() {
        var fromPageNo = $("section.home-btn-wrap").data("pageno");
        return (fromPageNo !== undefined && fromPageNo !== null && fromPageNo !== "")
            ? "&fromPageNo=" + encodeURIComponent(fromPageNo)
            : "";
    }

    $("main").on("click", "#edit", function () {
        var linkageSystemNo = $(this).data("no");
        var linkageSystemName = $(this).data("linkagesystemname") || "企業";
        var url = "../Integration/CompanyConnectionEdit?linkageSystemNo=" + encodeURIComponent(linkageSystemNo)
            + "&linkageSystemName=" + encodeURIComponent(linkageSystemName)
            + fromPageSuffix();

        mgf.portal.promiseLockScreen($(this)).then(function () {
            location.href = url;
        });
        return false;
    });

    $("main").on("click", "#delete", function () {
        $("#delete-modal").modal("show");
        return false;
    });

    $("main").on("click", "#delete-modal .btn-delete", function () {
        var no = $("#delete").data("no");

        $("#delete-modal").modal("hide");

        mgf.lockScreen();
        // 解除実行はサーバーPOSTで行い、成功時に連携設定へ戻す。
        $.ajax({
            type: "POST",
            url: "../Integration/CompanyConnectionResult?ActionSource=Delete",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                linkageSystemNo: no
            },
            async: true,
            beforeSend: function () {
                mgf.portal.checkSessionByAjax();
            }
        }).done(function (data) {
            var json = (typeof data === "string") ? $.parseJSON(data) : data;

            if ($.isTrueString(json.IsSuccess)) {
                location.href = "../Portal/ConnectionSetting?tabNo=2" + fromPageSuffix();
                return;
            }

            var messages = json.Messages || {};
            // 解除失敗時は画面内の警告領域へ表示する。
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

    $("main").on("click", "#delete-modal .btn-close, #delete-modal .close", function () {
        $("#delete-modal").modal("hide");
        return false;
    });

    mgf.prohibitHistoryBack();

    return {};
})();
