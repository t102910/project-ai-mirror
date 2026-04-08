// mgf.point.datacharge 名前空間オブジェクト
mgf.point.datacharge = mgf.point.datacharge || {};

mgf.point.datacharge = (function () {
    var enableServerSubmit = false;

    function getModal(selector) {
        var el = document.querySelector(selector);
        return el ? bootstrap.Modal.getOrCreateInstance(el) : null;
    }

    function parseJsonResult(data) {
        if (!data) { return null; }
        if (typeof data === "string") {
            try { return $.parseJSON(data); } catch (e) { return null; }
        }
        return data;
    }

    function showCaution(message) {
        $("#caution").text(message || "").removeClass("hide");
    }

    function hideCaution() {
        $("#caution").text("").addClass("hide");
    }

    var confirmationModal = getModal("#confirmation-modal");
    var finishModal = getModal("#finish-modal");

    mgf.prohibitHistoryBack();

    $("main").on("click", "a.charge", function () {
        if ($(this).hasClass("disabled")) { return false; }
        hideCaution();
        $("#confirmation-modal .modal-body").text($(this).text());
        $("#confirmation-modal .btn-submit").data("capacity", $(this).data("capacity"));
        if (confirmationModal) { confirmationModal.show(); }
        return false;
    });

    $("main").on("click", "#confirmation-modal .btn-submit", function () {
        var capacity = $(this).data("capacity");
        if (!capacity) {
            showCaution("交換容量が取得できませんでした。");
            return false;
        }

        hideCaution();

        if (!enableServerSubmit) {
            $("#finish-modal .modal-body").text(capacity + "MB をチャージしました。");
            if (confirmationModal) { confirmationModal.hide(); }
            if (finishModal) { finishModal.show(); }
            return false;
        }

        mgf.lockScreen();
        mgf.point.checkSessionAsync($(this)).then(function () {
            $.ajax({
                type: "POST",
                url: "../Point/DatachargeResult",
                data: {
                    __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                    capacity: capacity
                },
                async: true
            }).done(function (data, textStatus, jqXHR) {
                try {
                    var contentType = jqXHR.getResponseHeader("Content-Type") || "";
                    var resultJson = parseJsonResult(data);
                    if (jqXHR.status === 200 && contentType.indexOf("application/json") >= 0 && resultJson && $.isTrueString(resultJson["IsSuccess"])) {
                        $("#finish-modal .modal-body").text(resultJson["Message"] || "登録しました。");
                        hideCaution();
                        if (finishModal) { finishModal.show(); }
                        return;
                    }
                    showCaution((resultJson && resultJson["Message"]) || "データチャージに失敗しました。");
                } catch (e) {
                    showCaution("データチャージに失敗しました。");
                }
            }).fail(function () {
                showCaution("データチャージに失敗しました。");
            }).always(function () {
                if (confirmationModal) { confirmationModal.hide(); }
                mgf.unlockScreen();
            });
        }, mgf.redirectToLogin);

        return false;
    });

    $("main").on("click", "#finish-modal div.modal-header button.close", function () {
        if (finishModal) { finishModal.hide(); }
        location.href = "../Point/Datacharge" + ($("#back").data("back") || "");
        return false;
    });

    $("main").on("click", "#finish-modal div.modal-footer button.btn-close", function () {
        if (finishModal) { finishModal.hide(); }
        location.href = "../Point/Datacharge" + ($("#back").data("back") || "");
        return false;
    });

    return {};
})();
