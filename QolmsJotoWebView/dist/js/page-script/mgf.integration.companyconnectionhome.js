// mgf.integration.companyconnectionhome 名前空間オブジェクト
mgf.integration = mgf.integration || {};
mgf.integration.companyconnectionhome = mgf.integration.companyconnectionhome || {};

mgf.integration.companyconnectionhome = (function () {
    // メニュー項目クリック時、許可がない項目はインフォダイアログを表示し、許可された項目はURLへ遷移する。
    $("main").on("click", ".menu-list", function () {
        var url = $(this).data("url");

        if ($(this).hasClass("examination") || $(this).hasClass("company-request")) {
            location.href = url;
            return false;
        }

        $("#info-modal").modal("show");
        return false;
    });

    // インフォダイアログの閉じるボタンクリックでダイアログを閉じる。
    $("body").on("click", "#info-modal .btn-close, #info-modal .close", function () {
        $("#info-modal").modal("hide");
        return false;
    });

    mgf.prohibitHistoryBack();

    return {};
})();
