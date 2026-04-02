// mgf.integration.companyconnectionrequest 名前空間オブジェクト
mgf.integration = mgf.integration || {};
mgf.integration.companyconnectionrequest = mgf.integration.companyconnectionrequest || {};

mgf.integration.companyconnectionrequest = (function () {
    function moveToResult(context) {
        var fromPageNo = $("section.home-btn-wrap").data("pageno");
        var linkageSystemName = $("#request").data("linkagesystemname") || "企業";
        var url = "../Integration/CompanyConnection?mockConnected=true&linkageSystemName=" + encodeURIComponent(linkageSystemName);

        if (fromPageNo !== undefined && fromPageNo !== null && fromPageNo !== "") {
            url += "&fromPageNo=" + encodeURIComponent(fromPageNo);
        }

        mgf.portal.promiseLockScreen(context).then(function () {
            location.href = url;
        });
    }

    $("main").on("click", "#request", function () {
        $("#disconnect-modal").modal("show");
        return false;
    });

    $("main").on("click", "#disconnect-modal .btn-delete", function () {
        $("#disconnect-modal").modal("hide");
        moveToResult($(this));
        return false;
    });

    $("main").on("click", "#disconnect-modal .btn-close, #disconnect-modal .close", function () {
        $("#disconnect-modal").modal("hide");
        return false;
    });

    mgf.prohibitHistoryBack();

    return {};
})();
