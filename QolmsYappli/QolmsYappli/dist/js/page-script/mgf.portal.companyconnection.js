//  名前空間オブジェクト
mgf.portal.companyconnection = mgf.portal.companyconnection || {};

mgf.portal.companyconnection = (function () {

    $("main").on("click", "#delete", function () {

        var no = $("#delete").data("no");

        mgf.lockScreen()

        $.ajax({
            type: "POST",
            url: "../Portal/CompanyConnectionResult?ActionSource=Delete",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                LinkageSystemNo: no
            },
            async: true,
            beforeSend: function (jqXHR) {
                mgf.portal.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {
            //console.log(data)
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {
                    //成功した時の表示

                    var str = "../Portal/ConnectionSetting?tabno=2"
                    var returnPage = $(".home-btn-wrap").data("pageno");
                    if (returnPage > 0) {
                        str = str + "&fromPageNo=" + returnPage;
                    }

                    location.href = str;
                }
                else {
                    //console.log("else")
                    $("#caution").text("");
                    $("#caution").append($("<p>" + $.parseJSON(data)["Message"] + "</p>"))
                    $("#caution").removeClass("hide");
                    //alert(caution.text());
                    mgf.unlockScreen();

                }
            } catch (ex) {

                $("#info-modal .modal-body").text("エラーが発生しました。");
                $("#info-modal").modal("show");
                mgf.unlockScreen();

            }
        }).fail(function (data, textStatus, jqXHR) {

            $("#info-modal .modal-body").text("サーバーでエラーが発生しました。");
            $("#info-modal").modal("show");
            mgf.unlockScreen();

            //mgf.unlockScreen();
            //var timer = setTimeout(function () {
            //    location.href = "../Portal/AlkooConnection"
            //}, 5000);
        });

    });


    $("main").on("click", "#edit", function () {

        var no = $("#edit").data("no");

        var str = ""
        var returnPage = $(".home-btn-wrap").data("pageno");
        if (returnPage > 0) {
            str = "&fromPageNo=" + returnPage;
        }

        mgf.lockScreen()
        location.href = "../Portal/CompanyConnectionEdit?linkagesystemno=" + no + str

        //$.ajax({
        //    type: "POST",
        //    url: "../Portal/CompanyConnectionResult?ActionSource=edit",
        //    data: {
        //        __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
        //        LinkageSystemNo: no
        //    },
        //    async: true,
        //    beforeSend: function (jqXHR) {
        //        mgf.portal.checkSessionByAjax();
        //    }
        //}).done(function (data, textStatus, jqXHR) {
        //    //console.log(data)
        //    try {
        //        if (jqXHR.status == 200 ) {
        //            //成功した時の表示
        //            console.log("success")
        //            console.log($.parseJSON(data)["Message"])
        //            console.log($.parseJSON(data)["LinkageSystemNo"])

        //        }
        //        else {

        //            $("#caution").text("");
        //            $("#caution").append($("<p>" + $.parseJSON(data)["Message"] + "</p>"))
        //            $("#caution").removeClass("hide");
        //            mgf.unlockScreen();

        //        }
        //    } catch (ex) {

        //        mgf.unlockScreen();

        //    }
        //}).fail(function (data, textStatus, jqXHR) {

        //    $("#caution").text("");
        //    $("#caution").append($("<p>" + "登録処理に失敗しました。5秒後に自動的に再読み込みします。お手数ですが再読み込み後に再度お試しください。" + "</p>"))
        //    $("#caution").removeClass("hide");

        //    mgf.unlockScreen();

        //});
        //return false;

    });

    //閉じる
    $("body").on("click", "#info-modal .btn-close", function () {
        $("#info-modal").modal("hide");

    });
    //×
    $("body").on("click", "#info-modal .close", function () {
        $("#info-modal").modal("hide");

    });

    mgf.prohibitHistoryBack();

})();