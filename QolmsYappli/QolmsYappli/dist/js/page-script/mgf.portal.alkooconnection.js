//  名前空間オブジェクト
mgf.portal.alkooconnection = mgf.portal.alkooconnection || {};

mgf.portal.alkooconnection = (function () {

    mgf.prohibitHistoryBack();

    function connect(tanitaCancelFlag) {
        mgf.lockScreen()

        $.ajax({
            type: "POST",
            url: "../Portal/AlkooConnectionResult?ActionSource=Register",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                tanitaCancelFlag: tanitaCancelFlag
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
                    //console.log("ID:" + $.parseJSON(data)["ID"])
                    //console.log("ID:" + $.parseJSON(data)["URL"])
                    $("#caution").addClass("hide");
                    var url = "native:/action/open_browser?url=" + $.parseJSON(data)["URL"] + $.parseJSON(data)["ID"]
                    //var url = "native:/action/open_browser?url=" + "https%3A%2F%2Ffox233d.dev.navitime.co.jp%2Fiphone_walking%2Fhtml%2Fjoto%2Fredirect%3Fto%3DcooperationPageJump%26jotoId%3D" + $.parseJSON(data)["ID"]
                    location.href = url

                    var timer = setTimeout(function () {
                        location.href ="../Portal/AlkooConnection"
                    }, 3000);
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

                mgf.unlockScreen();

            }
        }).fail(function (data, textStatus, jqXHR) {

            $("#caution").text("");
            $("#caution").append($("<p>" + "登録処理に失敗しました。5秒後に自動的に再読み込みします。お手数ですが再読み込み後に再度お試しください。" + "</p>"))
            $("#caution").removeClass("hide");

            mgf.unlockScreen();
            var timer = setTimeout(function () {
                location.href = "../Portal/AlkooConnection"
            }, 5000);
        });
        return false;

    }

    //連携
    $("main").on("click", "#connection", function () {

        if (document.getElementById("regist-modal") != null) {
            $("#regist-modal").modal("show");
            return false;
        } else {
            connect(false)
        }
    })

    // 「連携の確認」ダイアログ 内の ボタン の クリック
    $("main").on("click", "#regist-modal button", function () {
        $("#regist-modal").modal("hide");

        if ($(this).hasClass("btn-delete")) {

            connect(true)

        }
    })

    

    // 「連携解除」ボタン の クリック
    $("main").on("click", "#cancel", function () {
        // 確認 ダイアログ を表示
        $("#disconnect-modal").modal("show");

        return false;
    });

    // 「連携解除の確認」ダイアログ 内の ボタン の クリック
    $("main").on("click", "#disconnect-modal button", function () {
        // 確認 ダイアログ を非表示
        $("#disconnect-modal").modal("hide");

        if ($(this).hasClass("btn-delete")) {
            // 連携解除
            mgf.lockScreen()

            $.ajax({
                type: "POST",
                url: "../Portal/AlkooConnectionResult?ActionSource=Cancel",
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
                        // 成功

                        // リロード
                        location.href = "../Portal/AlkooConnection"
                    } else {
                        // エラー
                        $("#caution").text("");
                        $("#caution").append($("<p>" + $.parseJSON(data)["Message"] + "</p>"))
                        $("#caution").removeClass("hide");

                        // 画面を アンロック
                        mgf.unlockScreen();
                    }
                } catch (ex) {
                    // エラー
                    $("#caution").text("");
                    $("#caution").append($("<p>" + $.parseJSON(data)["Message"] + "</p>"))
                    $("#caution").removeClass("hide");

                    // 画面を アンロック
                    mgf.unlockScreen();
                }
            }).fail(function (jqXHR, textStatus, errorThrown) {
                // Ajax エラー
                $("#caution").text("");
                $("#caution").append($("<p>連携の解除に失敗しました。</p>"))
                $("#caution").removeClass("hide");

                // 画面を アンロック
                mgf.unlockScreen();
            });
        }

        return false;
    });


    $("main").on("click", "#reconnection", function () {
        mgf.lockScreen();

        $.ajax({
            type: "POST",
            url: "../Portal/AlkooConnectionResult?ActionSource=Reconnection",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val()
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

                    $("#caution").addClass("hide");
                    var url = "native:/action/open_browser?url=" + $.parseJSON(data)["URL"] + $.parseJSON(data)["ID"]
                    location.href = url

                    mgf.unlockScreen();

                    //var timer = setTimeout(function () {
                    //    location.href ="../Portal/AlkooConnection"
                    //}, 3000);
                }
                else {
                    //console.log("else")
                    $("#caution-reconect").text("");
                    $("#caution-reconect").append($("<p>" + $.parseJSON(data)["Message"] + "</p>"))
                    $("#caution-reconect").removeClass("hide");
                    //alert(caution.text());
                    mgf.unlockScreen();

                }
            } catch (ex) {

                mgf.unlockScreen();

            }
        }).fail(function (data, textStatus, jqXHR) {

            $("#caution-reconect").text("");
            $("#caution-reconect").append($("<p>" + "失敗しました。" + "</p>"))
            $("#caution-reconect").removeClass("hide");
            mgf.unlockScreen();

        });
        return false;
    })

})();
