// mgf.premium.index 名前空間オブジェクト
mgf.premium.index = mgf.premium.index || {};

mgf.premium.index =(function () {
    // submit
    $(document).on("click", "button[type='submit']", function () {
        if ($("form")[0].checkValidity()) {
            // 画面をロック後、サブミット
            mgf.premium.promiseLockScreen($(this));
        }

        return true;
    });

    // ヒストリーバックの禁止
    mgf.prohibitHistoryBack();

    $(window).unload(function () {
        mgf.unlockScreen();
    });
})();
