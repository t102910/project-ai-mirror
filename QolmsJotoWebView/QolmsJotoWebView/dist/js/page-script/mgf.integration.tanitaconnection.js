// mgf.integration.tanitaconnection 名前空間オブジェクト
mgf.integration = mgf.integration || {};
mgf.integration.tanitaconnection = mgf.integration.tanitaconnection || {};

mgf.integration.tanitaconnection = (function () {
    function moveToMockState(context, isConnected) {
        var fromPageNo = $("section.home-btn-wrap").data("pageno");
        var url = "../Integration/TanitaConnection?mockConnected=" + (isConnected ? "true" : "false");

        if (fromPageNo !== undefined && fromPageNo !== null && fromPageNo !== "") {
            url += "&fromPageNo=" + encodeURIComponent(fromPageNo);
        }

        mgf.portal.promiseLockScreen(context).then(function () {
            location.href = url;
        });
    }

    $("main").on("click", "#connection", function () {
        moveToMockState($(this), true);
        return false;
    });

    $("main").on("click", "#cancel", function () {
        $("#disconnect-modal").modal("show");
        return false;
    });

    $("main").on("click", "#disconnect-modal .btn-delete", function () {
        $("#disconnect-modal").modal("hide");
        moveToMockState($(this), false);
        return false;
    });

    $("main").on("click", "#disconnect-modal .btn-close, #disconnect-modal .close", function () {
        $("#disconnect-modal").modal("hide");
        return false;
    });

    mgf.prohibitHistoryBack();

    return {};
})();
