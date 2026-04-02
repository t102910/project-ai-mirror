// mgf.premium.unsubscribe 名前空間オブジェクト
mgf.premium.unsubscribe = mgf.premium.unsubscribe || {};

mgf.premium.unsubscribe = (function () {
    function noop() {
        return false;
    }

    $("main").on("click", ".js-unsubscribe-noop", function (event) {
        event.preventDefault();
        noop();
    });

    mgf.prohibitHistoryBack();

    return {
        noop: noop
    };
})();
