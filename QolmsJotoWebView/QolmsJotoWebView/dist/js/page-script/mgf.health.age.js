// mgf.health.age 名前空間オブジェクト
mgf.health.age = mgf.health.age || {};

// DOM 構築完了
mgf.health.age = (function () {
    // レポートの非同期ロード
    function reportLoad(reportType, context) {
        return $.ajax({
            type: "POST",
            url: "../Health/AgeResult?ActionSource=Report",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                HealthAgeReportType: reportType
            },
            async: true,
            //beforeSend: function (jqXHR) {
            //    // セッションのチェック
            //    mgf.health.checkSessionByAjax();
            //},
            context: context
        }).done(function (data, textStatus, jqXHR) {
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("text/html;") >= 0) {
                    var $article = $(data);

                    // 要素の差し替え
                    $(this).replaceWith($article);

                    $article.find("div.chart-area").each(function () {
                        // レポート内のグラフの描画
                        $(this).find("div.chart-draw-area").chartInit({ "chartName": $(this).data("ref") });
                    });
                } else {
                    // エラー
                }
            } catch (ex) {
                // エラー
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            // エラー
        });
    };

    $("main").on("click", "#view-more", function () {
        // 画面をロック後、遷移
        mgf.health.promiseLockScreen($(this))
            .then(function () {
                // レポートの非同期ロードをキャンセル
                if (mgf.health.$jqXhrReports) {
                    mgf.health.$jqXhrReports.forEach(function (jqXHR) {
                        jqXHR.abort();
                        console.log("abort");
                    });
                }
                var pageno = $(".home-btn-wrap").data("pageno");

                console.log(pageno);

                if (pageno != undefined) {

                    console.log(true);
                    $(location).attr("href", "../Health/AgeEdit?fromPageNo=" + pageno);

                } else {
                    $(location).attr("href", "../Health/AgeEdit");
                }
            });

        return false;
    });

    // ヒストリー バック禁止
    mgf.prohibitHistoryBack();

    //// セッションのチェック
    //mgf.health.checkSessionByAjax();

    //// レポートの非同期ロード開始
    //$("#data-area section.inner").children("article.reload-area").each(function () {
    //    var reportType = $(this).data("report-type");

    //    if (reportType) {
    //        // レポートの非同期ロード開始
    //        mgf.health.$jqXhrReports.push(reportLoad(reportType, $(this)));
    //    }
    //});

    // セッション チェック 後 レポート を非同期 ロード
    mgf.health.checkSessionAsync(null)
        .then(
            function () {
                $("#data-area section.inner").children("article.reload-area").each(function () {
                    var reportType = $(this).data("report-type");

                    if (reportType) {
                        // 非同期 ロード 開始
                        mgf.health.$jqXhrReports.push(reportLoad(reportType, $(this)));
                    }
                });
            }
        );
})();
