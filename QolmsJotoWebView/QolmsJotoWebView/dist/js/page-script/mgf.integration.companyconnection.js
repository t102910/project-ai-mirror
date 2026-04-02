// mgf.integration.companyconnection 名前空間オブジェクト
mgf.integration = mgf.integration || {};
mgf.integration.companyconnection = mgf.integration.companyconnection || {};

mgf.integration.companyconnection = (function () {
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
        var linkageSystemName = $("#delete").data("linkagesystemname") || "企業";
        var url = "../Integration/CompanyConnectionRequest?linkageSystemName=" + encodeURIComponent(linkageSystemName) + fromPageSuffix();

        $("#delete-modal").modal("hide");
        mgf.portal.promiseLockScreen($(this)).then(function () {
            location.href = url;
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
