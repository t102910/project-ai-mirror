﻿// mgf.premium.agreement 名前空間オブジェクト
mgf.premium.agreement = mgf.premium.agreement || {};

mgf.premium.agreement = (function () {
    function syncAgreementState() {
        var isChecked = $("#policy").prop("checked");
        var $next = $("#agreement-next");

        $next.toggleClass("disabled", !isChecked);
        $next.attr("aria-disabled", (!isChecked).toString());
    }

    $(syncAgreementState);

    $("main").on("change", "#policy", function () {
        syncAgreementState();
    });

    $("main").on("click", "#agreement-next.disabled", function (event) {
        event.preventDefault();
    });

    mgf.prohibitHistoryBack();

    return {
        syncAgreementState: syncAgreementState
    };
})();
