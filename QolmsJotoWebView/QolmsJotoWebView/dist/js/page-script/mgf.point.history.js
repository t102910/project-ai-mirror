// mgf.point.history 名前空間オブジェクト
mgf.point.history = mgf.point.history || {};

mgf.point.history = (function () {
    function moveToHistoryResult(context) {
        var year = $("section.contents-area select[name='Year']").val();
        var month = $("section.contents-area select[name='Month']").val();
        var fromPageNo = $("section.home-btn-wrap").data("pageno");

        var url = "../Point/History?year=" + encodeURIComponent(year) + "&month=" + encodeURIComponent(month);
        if (fromPageNo !== undefined && fromPageNo !== null && fromPageNo !== "") {
            url += "&fromPageNo=" + encodeURIComponent(fromPageNo);
        }

        if (!mgf.point.checkSession()) {
            mgf.point.promiseLockScreen(context).then(function () {
                location.href = "../Start/Login?ReturnUrl=" + location.pathname.toLowerCase().replace("result", "");
            });
            return;
        }

        mgf.point.promiseLockScreen(context).then(function () {
            location.href = url;
        });
    }

    $("main").on("click", "section.contents-area .submit a.btn-submit", function () {
        moveToHistoryResult($(this));
        return false;
    });

    $("main").on("click", "#log-table thead", function () {
        var $tbody = $(this).next("tbody");
        if ($tbody.length === 1) { $tbody.toggle(); }
        return false;
    });

    $("main").on("click", "a.home-btn", function () {
        mgf.point.promiseLockScreen($(this)).then(function () {
            location.href = "../Portal/Home";
        });
        return false;
    });

    mgf.prohibitHistoryBack();

    return {};
})();
