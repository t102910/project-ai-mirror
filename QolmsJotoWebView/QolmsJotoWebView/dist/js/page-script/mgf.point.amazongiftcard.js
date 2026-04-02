// mgf.point.amazongiftcard 名前空間オブジェクト
mgf.point.amazongiftcard = mgf.point.amazongiftcard || {};

mgf.point.amazongiftcard = (function () {
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

    function scrolltoHist() {
        var $target = $("#hist");
        if ($target.length === 0) { return false; }
        $("body,html").animate({ scrollTop: $target.offset().top }, 400, "swing");
        return false;
    }

    function copyTextToClipboard(textVal) {
        var copyFrom = document.createElement("textarea");
        copyFrom.textContent = textVal;
        var bodyElm = document.getElementsByTagName("body")[0];
        bodyElm.appendChild(copyFrom);
        copyFrom.select();
        var retVal = document.execCommand("copy");
        bodyElm.removeChild(copyFrom);
        return retVal;
    }

    var confirmationModal = getModal("#confirmation-modal");
    var finishModal = getModal("#finish-modal");
    var copyModal = getModal("#copy-modal");

    mgf.prohibitHistoryBack();

    if (finishModal) {
        finishModal.show();
    }

    $("main").on("click", "a.exchange", function () {
        if ($(this).hasClass("disabled")) { return false; }
        hideCaution();
        $("#confirmation-modal .modal-body").text($(this).text());
        $("#confirmation-modal .btn-submit").data("itemid", $(this).data("itemid"));
        if (confirmationModal) { confirmationModal.show(); }
        return false;
    });

    $("main").on("click", "#confirmation-modal .btn-submit", function () {
        var itemId = $(this).data("itemid");
        if (!itemId) {
            showCaution("交換対象が取得できませんでした。");
            return false;
        }

        hideCaution();

        if (!enableServerSubmit) {
            if (confirmationModal) { confirmationModal.hide(); }
            if (finishModal) { finishModal.show(); } else { alert("交換しました。"); }
            return false;
        }

        mgf.lockScreen();
        mgf.point.checkSessionAsync($(this)).then(function () {
            $.ajax({
                type: "POST",
                url: "../Point/AmazonGiftCardResult",
                data: {
                    __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                    itemid: itemId
                },
                async: true
            }).done(function (data, textStatus, jqXHR) {
                try {
                    var contentType = jqXHR.getResponseHeader("Content-Type") || "";
                    var resultJson = parseJsonResult(data);
                    if (jqXHR.status === 200 && contentType.indexOf("application/json") >= 0 && resultJson && $.isTrueString(resultJson["IsSuccess"])) {
                        location.href = "../Point/AmazonGiftCard" + ($("#back").data("back") || "");
                        return;
                    }
                    showCaution((resultJson && resultJson["Message"]) || "交換に失敗しました。");
                } catch (e) {
                    showCaution("交換に失敗しました。");
                }
            }).fail(function () {
                showCaution("交換に失敗しました。");
            }).always(function () {
                if (confirmationModal) { confirmationModal.hide(); }
                mgf.unlockScreen();
            });
        }, mgf.redirectToLogin);

        return false;
    });

    $("main").on("click", "#finish-modal div.modal-header button.close", function () {
        if (finishModal) { finishModal.hide(); }
        scrolltoHist();
        return false;
    });

    $("main").on("click", "#finish-modal div.modal-footer button.btn-close", function () {
        if (finishModal) { finishModal.hide(); }
        scrolltoHist();
        return false;
    });

    $("main").on("click", "#confirmation-modal div.modal-header button.close", function () {
        if (confirmationModal) { confirmationModal.hide(); }
        scrolltoHist();
        return false;
    });

    $("main").on("click", "#confirmation-modal div.modal-footer button.btn-close", function () {
        if (confirmationModal) { confirmationModal.hide(); }
        scrolltoHist();
        return false;
    });

    $("main").on("click", "i.la-copy", function () {
        copyTextToClipboard($(this).data("couponid"));
        if (copyModal) { copyModal.show(); }
        return false;
    });

    $("main").on("shown.bs.modal", "#copy-modal", function () {
        setTimeout(function () {
            if (copyModal) { copyModal.hide(); }
        }, 300);
    });

    return {};
})();
