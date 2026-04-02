//  名前空間オブジェクト
mgf.portal.fitbitconnection = mgf.portal.fitbitconnection || {};

mgf.portal.fitbitconnection = (function () {

    mgf.prohibitHistoryBack();

    $("main").on("click", "#connection", function () {
        mgf.lockScreen()
        //console.log(name)
        $.ajax({
            type: "POST",
            url: "../Portal/fitbitconnectionResult?ActionSource=Update",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val()
            },
            async: true,
            beforeSend: function (jqXHR) {
                mgf.portal.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {

                    //成功した時の表示
                    var url = $.parseJSON(data)["Url"]
                    location.href = url;

                }
                else {

                    $("#caution").text("");
                    $("#caution").append($("<p>" + $.parseJSON(data)["Message"] + "</p>"))
                    $("#caution").removeClass("hide");
                    //alert(caution.text());
                    return false;

                }
            } catch (ex) {
                return false;

            }
        }).always(function (jqXHR, textStatus) {
            // 画面をアンロック
            mgf.unlockScreen();
        });
    });


    $("main").on("click", "#cancel", function () {

        mgf.lockScreen()

        $.ajax({
            type: "POST",
            url: "../Portal/fitbitconnectionResult?ActionSource=Cancel",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                cancel: "cancel",
                dummy: "dummy"
            },
            async: true,
            beforeSend: function (jqXHR) {
                mgf.portal.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {

                    //成功した時の表示
                    //var url = $.parseJSON(data)["Url"]
                    //location.href = url;
                    location.href = "../portal/fitbitconnection" + "?fromPageNo=" + $(".home-btn-wrap").data("pageno");;

                }
                else {

                    $("#caution").text("");
                    $("#caution").append($("<p>" + $.parseJSON(data)["Message"] + "</p>"))
                    $("#caution").removeClass("hide");
                    //alert(caution.text());
                    return false;
                }

            } catch (ex) {
                return false;

            }

        }).always(function (jqXHR, textStatus) {
            // 画面をアンロック
            mgf.unlockScreen();
        });
    });

})();
