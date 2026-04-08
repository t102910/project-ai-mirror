// mgf.integration.tanitaconnection 名前空間オブジェクト
mgf.integration.tanitaconnection = mgf.integration.tanitaconnection || {};

mgf.integration.tanitaconnection = (function () {

    mgf.prohibitHistoryBack();

    // モーダルの存在確認を追加
    let disconnectModal = null;
    let alkooDisconnectModal = null;

    // disconnect-modal が存在するかチェック
    if ($("#disconnect-modal").length > 0) {
        disconnectModal = new bootstrap.Modal($("#disconnect-modal"));
    } else {
        console.warn("disconnect-modal element not found");
    }

    // alkoo-disconnect-modal が存在するかチェック
    if ($("#alkoo-disconnect-modal").length > 0) {
        alkooDisconnectModal = new bootstrap.Modal($("#alkoo-disconnect-modal"));
    } else {
        console.warn("alkoo-disconnect-modal element not found");
    }

    function deviceRegister(name, checked, alkooCancelFlag) {

        mgf.lockScreen();
        //console.log(name)
        $.ajax({
            type: "POST",
            url: "../Integration/TanitaConnectionResult?ActionSource=Update",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                data: name,
                checked: checked,
                alkooCancelFlag: alkooCancelFlag
            },
            async: true,
            beforeSend: function (jqXHR) {
                mgf.integration.checkSession();
            }
        }).done(function (data, textStatus, jqXHR) {
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {
                    //成功した時の表示
                    //console.log("success")
                    $("#connection-data input").prop("checked", false)

                    $.each($.parseJSON(data)["Devises"], function (key, value) {
                        //console.log(key, value)

                        if (value == 1) {
                            $("#BodyCompositionMeter").prop("checked", true)
                        } else if (value == 2) {
                            $("#Sphygmomanometer").prop("checked", true)

                        } else if (value == 3) {
                            $("#Pedometer").prop("checked", true)
                        }
                    })

                    //$("#finish-modal .modal-body").text($.parseJSON(data)["Message"]);
                    //$("#finish-modal").modal("show");
                    $("#caution").addClass("hide");

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
    }

    //連携デバイス
    $("main").on("click", "#connection-data input", function () {
        var linked = $("#connect").length > 0
        //console.log(linked)

        if (linked) {
            //alert("connection-data input");
            //連携済みの場合に呼ばれる
            var name = $(this).prop('name')
            //console.log(name)
            var checked = $(this).prop('checked')
            var alkoo = document.getElementById("alkoo-disconnect-modal") != null
            //console.log(alkoo)
            //歩数の時だけダイアログ確認
            if (name == 'Pedometer' && alkoo && checked) {
                $("#alkoo-disconnect-modal button").addClass('device')

                alkooDisconnectModal.show();
            } else {
                //console.log(name)
                deviceRegister(name, checked, false);
            }
            return false
        }
    })
    // 「連携解除の確認」ダイアログ 内の ボタン の クリック
    $("main").on("click", "#alkoo-disconnect-modal button", function () {
        // 確認 ダイアログ を非表示
        alkooDisconnectModal.hide();

        if ($(this).hasClass("btn-delete")) {
            //歩くの登録
            if ($(this).hasClass("device")) {
                deviceRegister('Pedometer', true, true);
            } else {
                connect(true)
            }
        }
    })


    function connect(alkooCancelFlag) {
        mgf.lockScreen();

        var id = $("#ID").val();
        var password = $("#Password").val();
        var bodyCompositionMeter = $("#BodyCompositionMeter").prop('checked');
        var sphygmomanometer = $("#Sphygmomanometer").prop('checked');
        var pedometer = $("#Pedometer").prop('checked');
        //console.log(id);
        //console.log(password);

        if (id == "" || password == "") {
            $("#caution").text("");
            $("#caution").append($("<p>IDまたはパスワードが空です。</p>"))
            $("#caution").removeClass("hide");
            mgf.unlockScreen()

            return false;
        }

        $.ajax({
            type: "POST",
            url: "../integration/TanitaConnectionResult?ActionSource=Register",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                ID: id,
                Password: password,
                BodyCompositionMeter: bodyCompositionMeter,
                Sphygmomanometer: sphygmomanometer,
                Pedometer: pedometer,
                alkooCancelFlag: alkooCancelFlag
            },
            async: true,
            beforeSend: function (jqXHR) {
                mgf.integration.checkSession();
            }
        }).done(function (data, textStatus, jqXHR) {
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {
                    //成功した時の表示
                    $("#caution").addClass("hide");
                    //console.log("success")
                    //$("#finish-modal .modal-body").text($.parseJSON(data)["Message"]);
                    //$("#finish-modal").modal("show");
                    location.href = "../integration/TanitaConnection"

                }
                else {

                    $("#caution").text("");
                    $("#caution").append($("<p>" + $.parseJSON(data)["Message"] + "</p>"))
                    $("#caution").removeClass("hide");
                    //alert(caution.text());
                    mgf.unlockScreen();

                }
            } catch (ex) {
                mgf.unlockScreen();

            }
        });
        return false;

    }

    //連携
    $("main").on("click", "#connection", function () {
        //console.log($("#Pedometer").prop('checked'))
        if (document.getElementById("alkoo-disconnect-modal") != null && $("#Pedometer").prop('checked')) {
            alkooDisconnectModal.show();
        } else {

            connect(false)
        }
    })

    // 「連携解除」ボタン の クリック
    $("main").on("click", "#cancel", function () {
        // 確認 ダイアログ を表示
        disconnectModal.show();

        return false;
    });

    // 「連携解除の確認」ダイアログ 内の ボタン の クリック
    $("main").on("click", "#disconnect-modal button", function () {
        // 確認 ダイアログ を非表示
        disconnectModal.hide();

        if ($(this).hasClass("btn-delete")) {
            // 連携解除
            mgf.lockScreen();

            $.ajax({
                type: "POST",
                url: "../integration/TanitaConnectionResult?ActionSource=Cancel",
                data: {
                    __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val()
                },
                async: true,
                beforeSend: function (jqXHR) {
                    mgf.integration.checkSession();
                }
            }).done(function (data, textStatus, jqXHR) {
                try {
                    if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {
                        // 成功

                        // リロード
                        location.href = "../integration/TanitaConnection"
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
})();