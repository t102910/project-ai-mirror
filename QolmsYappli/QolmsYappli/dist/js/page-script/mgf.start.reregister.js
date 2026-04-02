// mgf.start.reregister 名前空間オブジェクト
mgf.start.reregister = mgf.start.reregister || {};

// DOM 構築完了
mgf.start.reregister = (function () {
    // ヒストリー バック禁止
    mgf.prohibitHistoryBack();

    $("main").on("click", "a.btn-submit", function () {

        mgf.start.promiseLockScreen($(this))
          .then(function () { 
                location.href = "../Start/ReregisterResult"
                return false;
          });
    });

})();