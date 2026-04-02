// mgf.integration.hospitalconnectionrequest 名前空間オブジェクト
mgf.integration = mgf.integration || {};
mgf.integration.hospitalconnectionrequest = mgf.integration.hospitalconnectionrequest || {};

mgf.integration.hospitalconnectionrequest = (function () {
    function moveToResult(context) {
        var fromPageNo = $("section.home-btn-wrap").data("pageno");
        var hospitalName = $("#request").data("hospitalname") || "城東区医師会病院";
        var patientNo = $("#patient-no").val() || "00012345";
        var url = "../Integration/HospitalConnection?mockConnected=true&hospitalName=" + encodeURIComponent(hospitalName)
            + "&patientNo=" + encodeURIComponent(patientNo);

        if (fromPageNo !== undefined && fromPageNo !== null && fromPageNo !== "") {
            url += "&fromPageNo=" + encodeURIComponent(fromPageNo);
        }

        mgf.portal.promiseLockScreen(context).then(function () {
            location.href = url;
        });
    }

    $("main").on("click", "#request", function () {
        $("#confirm-modal").modal("show");
        return false;
    });

    $("main").on("click", "#confirm-modal .btn-delete", function () {
        $("#confirm-modal").modal("hide");
        moveToResult($(this));
        return false;
    });

    $("main").on("click", "#confirm-modal .btn-close, #confirm-modal .close", function () {
        $("#confirm-modal").modal("hide");
        return false;
    });

    mgf.prohibitHistoryBack();

    return {};
})();
