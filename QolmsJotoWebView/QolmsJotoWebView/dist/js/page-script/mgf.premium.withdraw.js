// mgf.premium.withdraw 名前空間オブジェクト
mgf.premium.withdraw = mgf.premium.withdraw || {};

mgf.premium.withdraw = (function () {
    function noop() {
        return false;
    }

    $("main").on("click", ".js-withdraw-noop", function (event) {
        event.preventDefault();
        noop();
    });

    mgf.prohibitHistoryBack();

    return {
        noop: noop
    };
})();
