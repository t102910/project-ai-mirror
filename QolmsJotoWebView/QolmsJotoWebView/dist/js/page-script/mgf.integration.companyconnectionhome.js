// mgf.integration.companyconnectionhome 名前空間オブジェクト
mgf.integration = mgf.integration || {};
mgf.integration.companyconnectionhome = mgf.integration.companyconnectionhome || {};

mgf.integration.companyconnectionhome = (function () {
    $("main").on("click", ".menu-list", function () {
        var url = $(this).data("url");

        if ($(this).hasClass("examination")) {
            location.href = url;
            return false;
        }

        $("#info-modal").modal("show");
        return false;
    });

    $("body").on("click", "#info-modal .btn-close, #info-modal .close", function () {
        $("#info-modal").modal("hide");
        return false;
    });

    mgf.prohibitHistoryBack();

    return {};
})();
