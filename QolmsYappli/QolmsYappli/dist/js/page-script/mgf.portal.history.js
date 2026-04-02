// mgf.portal.history 名前空間オブジェクト
mgf.portal.history = mgf.portal.history || {};

// DOM 構築完了
mgf.portal.history = (function () {
    // 「表示」ボタン
    $("main").on("click", "section.contents-area a.btn-submit", function () {
        if (!mgf.portal.checkSession()) {
            // セッション切れ
            mgf.portal.promiseLockScreen($(this))
                .then(function () {
                    location.href = "../Start/Login?ReturnUrl=" + location.pathname.toLowerCase().replace("result", "");
                });
        } else {
            // 画面をロック後、サブミット
            mgf.portal.promiseLockScreen($(this))
                .then(function () {
                    //
                    try {
                        $("<form>", {
                            action: "../Portal/HistoryResult",
                            method: "POST"
                        }).appendTo(document.body);

                        $("<input>").attr({
                            type: "hidden",
                            name: "__RequestVerificationToken",
                            value: $("input[name='__RequestVerificationToken']").val()
                        }).appendTo($("form:last"));

                        // 年
                        $("<input>").attr({
                            type: "hidden",
                            name: "Year",
                            value: $("section.contents-area select[name='Year']").val()
                        }).appendTo($("form:last"));

                        // 月
                        $("<input>").attr({
                            type: "hidden",
                            name: "Month",
                            value: $("section.contents-area select[name='Month']").val()
                        }).appendTo($("form:last"));

                        // 月
                        $("<input>").attr({
                            type: "hidden",
                            name: "fromPageNo",
                            value: $("section.home-btn-wrap").data("pageno")
                        }).appendTo($("form:last"));

                        // サブミット
                        $("form:last").submit();
                    } catch (ex) {
                        // 画面をアンロック
                        mgf.unlockScreen();
                    }
                });
        }

        return false;
    });

    // 詳細展開
    $("main").on("click", "#log-table thead", function () {
        var $tbody = $(this).next("tbody");

        if ($tbody.length == 1) $tbody.toggle();

        return false;
    });


    $("main").on("click", "a.home-btn", function () {

        // 画面をロック
        mgf.portal.promiseLockScreen($(this))
         .then(function () {
             location.href = "../Portal/Home"
         });
        return false;

    })

    // ヒストリー バック禁止
    mgf.prohibitHistoryBack();
})();
