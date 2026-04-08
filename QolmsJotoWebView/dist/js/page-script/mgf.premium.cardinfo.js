﻿// mgf.premium.cardinfo 名前空間オブジェクト
mgf.premium.cardinfo = mgf.premium.cardinfo || {};

mgf.premium.cardinfo = (function () {
    function openFinishModal() {
        $("#finish-modal").modal("show");
    }

    function closeFinishModal() {
        $("#finish-modal").modal("hide");
    }

    function submitCardRegistration() {
        mgf.premium.promiseLockScreen($("#card-register-form"));
        $("#card-register-submit").trigger("click");
    }

    $(function () {
        if ($("#premium-payjp").data("registered") === true || $("#premium-payjp").data("registered") === "true") {
            openFinishModal();
        }
    });

    $("main").on("click", "#Register", function (event) {
        event.preventDefault();
        submitCardRegistration();
    });

    $("main").on("click", "#finish-modal .close, #finish-modal .btn-close", function (event) {
        event.preventDefault();
        closeFinishModal();
    });

    mgf.prohibitHistoryBack();

    return {
        openFinishModal: openFinishModal,
        closeFinishModal: closeFinishModal,
        submitCardRegistration: submitCardRegistration
    };
})();
