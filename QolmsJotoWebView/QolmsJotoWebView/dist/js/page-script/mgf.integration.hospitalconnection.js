// mgf.integration.hospitalconnection 名前空間オブジェクト
mgf.integration = mgf.integration || {};
mgf.integration.hospitalconnection = mgf.integration.hospitalconnection || {};

mgf.integration.hospitalconnection = (function () {
    function fromPageSuffix() {
        var fromPageNo = $("section.home-btn-wrap").data("pageno");
        return (fromPageNo !== undefined && fromPageNo !== null && fromPageNo !== "")
            ? "&fromPageNo=" + encodeURIComponent(fromPageNo)
            : "";
    }

    $("main").on("click", "#delete", function () {
        $("#delete-modal").modal("show");
        return false;
    });

    $("main").on("click", "#delete-modal .btn-delete", function () {
        var hospitalName = $("#delete").data("hospitalname") || "城東区医師会病院";
        var url = "../Integration/HospitalConnectionRequest?hospitalName=" + encodeURIComponent(hospitalName) + fromPageSuffix();

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
