// mgf.start.loginedit 名前空間オブジェクト
mgf.start.loginedit = mgf.start.loginedit || {};

// DOM 構築完了
mgf.start.loginedit = (function () {
    // ヒストリー バック禁止
    mgf.prohibitHistoryBack();

    //はい
    $("main").on("click", "#auid-login", function () {
        console.log("click");

        mgf.lockScreen()
        
        location.href = "../Start/LoginEditResult";

        //$.ajax({
        //    type: "GET",
        //    url: "../Start/LoginEditResult",
        //    data: {
        //        __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val()
        //    },
        //    async: true,
        //    beforeSend: function (jqXHR) {
        //        mgf.start.checkSessionByAjax();
        //    }
        //}).done(function (data, textStatus, jqXHR) {
        //    try {
        //        if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {
        //            //成功した時の表示
        //            console.log("success")
                  
        //        }
        //        else {
        //            console.log("else")
        //        }
        //    } catch (ex) { }

        //}).always(function (jqXHR, textStatus) {
        //    // 画面をアンロック
        //    mgf.unlockScreen();
        //});
        return false;

    })

    //はい
    $("main").on("click", "#submit", function () {
        console.log("click");

        mgf.lockScreen()

        $.ajax({
            type: "POST",
            url: "../Start/LoginEditResult?ActionSource=JOTOID",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                userId: $("#userid").val(),
                Password: $("#password").val(),
                Password2: $("#password2").val()
            },
            async: true,
            beforeSend: function (jqXHR) {
                mgf.start.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {
                    //成功した時の表示

                    $(".alert").addClass("hide");
                    $("#message-modal").modal("show");
                    
                }
                else {
                    var messages = $.parseJSON(data)["Massages"];

                    console.log(messages);
                    var summaryMessage = true
                    $(messages).each(function (key, value) {
                        console.log(value);

                        $("." + value["Key"]).text(value["Value"]).removeClass("hide");
                    });
            
                    mgf.unlockScreen();

                }
            } catch (ex) { }

        }).always(function (jqXHR, textStatus) {
            // 画面をアンロック
            mgf.unlockScreen();
        });
        return false;

    })

    $("main").on("click", "#message-modal .close", function () {
        $("#message-modal").modal("hide");
        location.href = "../start/loginedit";
    })

    $("main").on("click", "#message-modal .btn-close", function () {
        $("#message-modal").modal("hide");
        location.href = "../start/loginedit";
    })

    $("#opneid-message-modal").modal("show");

    
})();