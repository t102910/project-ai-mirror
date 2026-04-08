// mgf.portal.localidverificationagreement 名前空間オブジェクト
mgf.portal.localidverificationagreement = mgf.portal.localidverificationagreement || {};

// DOM 構築完了
mgf.portal.localidverificationagreement = (function () {
    
    //if ($("section.contents-area").hasClass("first")) {

    //    var timer = setTimeout(function () {
    //        location.href = "jotohdr2://deep.link/action/open_browser?url=https%3A%2F%2Fwww.smartkensa.com%2Fgp%2Fentry%2F459"
    //    }, 1000);

    //}
    //alert("localidverificationagreement");

    //スクロールデザイン用JS
    var bb = $('.bottom-fix-wrap').outerHeight() + 100;
    var ds = $('.document-scroll-wrap').outerHeight();
    if ($(window).height() < bb + ds) {
        $('.document-scroll-wrap').css({ 'height': 'calc(100vh - ' + bb + 'px)' });

    }
    // 規約同意
    // --同意チェックでチャレンジボタンの表示を切替
    $("body").on("change", "#policy", function () {

        if ($(this).prop('checked')) {
            console.log("checked");
            $("a.agree").removeClass("disabled")

        } else {
            console.log("checked=false");

            $("a.agree").addClass("disabled")
        }
    });

    // ヒストリー バック禁止
    mgf.prohibitHistoryBack();

})();