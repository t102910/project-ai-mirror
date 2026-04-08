﻿// mgf.premium.paymentmethod 名前空間オブジェクト
mgf.premium.paymentmethod = mgf.premium.paymentmethod || {};

mgf.premium.paymentmethod = (function () {
    function syncPaymentType() {
        var selected = $("input[name='payment']:checked").val() || "1";
        $("#paymentType").val(selected);
    }

    $(syncPaymentType);

    $("main").on("change", "input[name='payment']", function () {
        syncPaymentType();
    });

    $("main").on("submit", "#payment-method-form", function () {
        syncPaymentType();
        mgf.premium.promiseLockScreen($(this));
    });

    mgf.prohibitHistoryBack();

    return {
        syncPaymentType: syncPaymentType
    };
})();
