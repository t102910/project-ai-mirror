﻿// mgf.premium.info 名前空間オブジェクト
mgf.premium.info = mgf.premium.info || {};

mgf.premium.info = (function () {
    function showPanel(selector) {
        var $target = $(selector);
        if ($target.length === 0) {
            return;
        }

        $("[data-panel]").addClass("hide-elm");
        $target.removeClass("hide-elm");

        $("html, body").animate({
            scrollTop: $target.offset().top
        }, 200);
    }

    function hidePanels() {
        $("[data-panel]").addClass("hide-elm");
        mgf.scrollTop();
    }

    $("main").on("click", "[data-panel-target]", function (event) {
        event.preventDefault();
        showPanel($(this).data("panel-target"));
    });

    $("main").on("click", "[data-panel-close]", function (event) {
        event.preventDefault();
        hidePanels();
    });

    mgf.prohibitHistoryBack();

    return {
        showPanel: showPanel,
        hidePanels: hidePanels
    };
})();
