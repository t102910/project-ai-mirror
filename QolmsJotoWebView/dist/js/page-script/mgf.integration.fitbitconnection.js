// mgf.integration.fitbitconnection 名前空間オブジェクト
mgf.integration = mgf.integration || {};
mgf.integration.fitbitconnection = mgf.integration.fitbitconnection || {};

mgf.integration.fitbitconnection = (function () {
    function moveToMockState(context, isConnected) {
        var fromPageNo = $("section.home-btn-wrap").data("pageno");
        var url = "../Integration/FitbitConnection?mockConnected=" + (isConnected ? "true" : "false");

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
        moveToMockState($(this), false);
        return false;
    });

    mgf.prohibitHistoryBack();

    return {};
})();
