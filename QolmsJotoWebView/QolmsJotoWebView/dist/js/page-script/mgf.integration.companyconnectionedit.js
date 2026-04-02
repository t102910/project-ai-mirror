// mgf.integration.companyconnectionedit 名前空間オブジェクト
mgf.integration = mgf.integration || {};
mgf.integration.companyconnectionedit = mgf.integration.companyconnectionedit || {};

mgf.integration.companyconnectionedit = (function () {
    $("main").on("click", "#request", function () {
        var linkageSystemNo = $(this).data("no");
        var linkageSystemName = $(this).data("linkagesystemname") || "企業";
        var fromPageNo = $("section.home-btn-wrap").data("pageno");
        var url = "../Integration/CompanyConnection?mockConnected=true"
            + "&linkageSystemNo=" + encodeURIComponent(linkageSystemNo)
            + "&linkageSystemName=" + encodeURIComponent(linkageSystemName);

        if (fromPageNo !== undefined && fromPageNo !== null && fromPageNo !== "") {
            url += "&fromPageNo=" + encodeURIComponent(fromPageNo);
        }

        mgf.portal.promiseLockScreen($(this)).then(function () {
            location.href = url;
        });
        return false;
    });

    mgf.prohibitHistoryBack();

    return {};
})();
