mgf.premium.regist = mgf.premium.regist || {};

mgf.premium.regist = (function () {

    // submit
    $(document).on("click", "button[type='submit']", function () {
        if ($("form")[0].checkValidity()) {
            // 画面をロック後、サブミット
            mgf.premium.promiseLockScreen($(this));
        }

        return true;
    });

    $('main').on("change", "input[name='payment']", function () {
        //console.log($('input[name="payment"]:checked').val());
        $('#paymentType').val($('input[name="payment"]:checked').val());
    })

    // ヒストリーバックの禁止
    mgf.prohibitHistoryBack();

    //$(window).unload(function () {
    //    mgf.unlockScreen();
    //});

})();

