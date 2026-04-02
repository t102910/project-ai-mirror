//  名前空間オブジェクト
mgf.portal.medicineconnection = mgf.portal.medicineconnection || {};

mgf.portal.medicineconnection = (function () {

    $("main").on("click", "#cancel", function () {

        var btn = $("#cancel").text();
        var message = ""
        if ($("#cancel").hasClass("request")) {
            message = "申請を取り消しますか？"

        } else {
            message = "連携を解除しますか？"

        }

        $("#delete-modal .modal-body").text(message);
        $("#delete-modal .btn-delete").text(btn);
        $("#delete-modal").modal("show");
    });

    //閉じる
    $("body").on("click", "#delete-modal .btn-close", function () {
        $("#delete-modal").modal("hide");

    });
    //×
    $("body").on("click", "#delete-modal .close", function () {
        $("#delete-modal").modal("hide");

    });

    //閉じる
    $("body").on("click", "#info-modal .btn-close", function () {
        $("#info-modal").modal("hide");

    });
    //×
    $("body").on("click", "#info-modal .close", function () {
        $("#info-modal").modal("hide");

    });

    $("body").on("click", "#delete-modal .btn-delete", function () {

        $("#delete-modal").modal("hide");

        mgf.lockScreen();

        var no = $("#cancel").data("no");
        var facilitykey = $("#cancel").data("facilitykey");
        //console.log(no);

        $.ajax({
            type: "POST",
            url: "../Portal/medicineconnectionResult?ActionSource=Cancel",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                LinkageSystemNo: no,
                Facilitykey: facilitykey
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
                    //console.log("success")

                    var str = "../Portal/ConnectionSetting?FromPageNo=" + $(".home-btn-wrap").data("pageno") + "&tabNo=4";
                    location.href = str;
                }
                else {

                    //console.log("error")
                    //エラーメッセージ
                    var messages = $.parseJSON(data)["Massage"];

                    $("#info-modal .modal-body").text("解除に失敗しました。(" + messages +")");
                    $("#info-modal").modal("show");

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
        return false;

    });

    //$("main").on("click", "#edit", function () {
    //    mgf.portal.promiseLockScreen($(this))
    //           .then(function () {
    //               var no = $(this).data("no");
    //               //console.log(no);
    //               //編集画面へ遷移
    //               location.href = "../Portal/medicineconnectionRequest?linkageSystemNo=" + no;
    //           })
    //});

    mgf.prohibitHistoryBack();

})();