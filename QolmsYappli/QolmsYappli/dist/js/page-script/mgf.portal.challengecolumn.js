// mgf.portal.challengecolumn 名前空間オブジェクト
mgf.portal.challengecolumn = mgf.portal.challengecolumn || {};

// DOM 構築完了
mgf.portal.challengecolumn = (function () {


    // 画像の非同期読み込み
    function loadPhoto() {
        $(document).find("ul.column-list .photo img.inview").each(function () {
            $(this).one("inview", { "reference": $(this).data("reference") }, function (event, visible) {
                $.ajax({
                    type: "GET",
                    url: event.data["reference"],
                    async: true,
                    context: event.target
                }).done(function (data, textStatus, jqXHR) {
                    $(this).attr("src", $(this).data("reference"));
                });
            });
        });
    };

    // 画像の非同期読み込み
    function loadColumnPhoto() {
        $(document).find("#column-detail img.inview").each(function () {
            $(this).one("inview", { "reference": $(this).data("reference") }, function (event, visible) {
                $.ajax({
                    type: "GET",
                    url: event.data["reference"],
                    async: true,
                    context: event.target
                }).done(function (data, textStatus, jqXHR) {

                    $(this).replaceWith('<img class="w-max" style="object-fit:cover;height:240px;" src=' + $(this).data("reference") + '></img>')

                    //$(this).attr("src", $(this).data("reference")).attr("style", "object-fit:cover;height:200px;");
                });
            });
        });
    };


    function DetailModal(key,no) {

        mgf.lockScreen();
        $.ajax({
            type: "POST",
            url: "../Portal/ChallengeColumnResult?ActionSource=Detail",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                key: key,
                no: no,
            },
            async: true,
            beforeSend: function (jqXHR) {
                // セッションのチェック
                mgf.portal.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {

            if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("text/html;") >= 0) {

                //パーシャルビューを書き換え
                $("body").find("#column-detail .modal-content").replaceWith($(data));
                $("#column-detail").modal("show");
                loadColumnPhoto();

            } else {
                //if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0) 失敗の理由を返す場合

                //失敗
                //console.log("失敗")
            }

        }).always(function (jqXHR, textStatus) {
            // 画面をアンロック
            mgf.unlockScreen();
        });
        return false;
    }


    $('body').on("click", ".column-list li a.column", function () {

        var key = $(this).data("key");
        var no = $(this).data("no");
        DetailModal(key,no);
        //console.log(key);
        //console.log(no);
        //// 画面をロック
        //mgf.lockScreen();
        //$.ajax({
        //    type: "POST",
        //    url: "../Portal/ChallengeColumnResult?ActionSource=Detail",
        //    data: {
        //        __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
        //        key:key,
        //        no:no,
        //    },
        //    async: true,
        //    beforeSend: function (jqXHR) {
        //        // セッションのチェック
        //        mgf.portal.checkSessionByAjax();
        //    }
        //}).done(function (data, textStatus, jqXHR) {

        //    if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("text/html;") >= 0) {

        //        //パーシャルビューを書き換え
        //        $("body").find("#column-detail .modal-content").replaceWith($(data));
        //        $("#column-detail").modal("show");
        //        loadColumnPhoto();

        //    } else {
        //        //if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0) 失敗の理由を返す場合

        //        //失敗
        //        //console.log("失敗")
        //    }

        //}).always(function (jqXHR, textStatus) {
        //    // 画面をアンロック
        //    mgf.unlockScreen();
        //});
        //return false;
    });

    $('#column-detail').on("click", "#read", function () {

        var key = $(this).data("key");
        var no = $(this).data("no");

        // 画面をロック
        mgf.lockScreen();
        $.ajax({
            type: "POST",
            url: "../Portal/ChallengeColumnResult?ActionSource=Read",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                key: key,
                no: no,
                read:true
            },
            async: true,
            beforeSend: function (jqXHR) {
                // セッションのチェック
                mgf.portal.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {

            if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {

                console.log("成功");
                $("#read").addClass("hide");


            } else {
                //if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0) 失敗の理由を返す場合

                //失敗
                //console.log("失敗")
            }

        }).always(function (jqXHR, textStatus) {
            // 画面をアンロック
            mgf.unlockScreen();
        });
        return false;
    });

    //$("body").on("click", "#content-modal .close", function () {
    //    $("#content-modal").modal("hide");
    //})

    //$("body").on("click", "#content-modal .btn-close", function () {
    //    $("#content-modal").modal("hide");
    //})

    //$('body').on("click", ".column-list li a.column", function () {

    //    console.log("click")
    //    $("#column-detail").modal("show");
    //})


    $('body').on("click", ".column-list li a.dr", function () {

        console.log("click")
        $("#dr-introduction").modal("show");
    })

    if ($("#target-column").data("target-column") > 0) {

        var key = $("#target-column").data("key");
        var no = $("#target-column").data("target-column");

        console.log(key);
        console.log(no);

        DetailModal(key, no);
    }

    // ヒストリー バック禁止
    mgf.prohibitHistoryBack();

    loadPhoto();

})();