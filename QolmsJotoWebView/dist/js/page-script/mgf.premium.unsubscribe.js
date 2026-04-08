// mgf.premium.unsubscribe 名前空間オブジェクト
mgf.premium.unsubscribe = mgf.premium.unsubscribe || {};

mgf.premium.unsubscribe = (function () {
    function submitUnsubscribe() {
        mgf.premium.promiseLockScreen($("#unsubscribe-form")).then(function () {
            this.submit();
        });
    }

    $("main").on("click", ".js-unsubscribe-submit", function (event) {
        event.preventDefault();
        submitUnsubscribe();
    });

    mgf.prohibitHistoryBack();

    return {
        submitUnsubscribe: submitUnsubscribe
    };
})();
