mgf.start.loginbyid = mgf.start.loginbyid || {};

mgf.start.loginbyid=(function() {  
    //// 方言の配列
	//const dialects = [
    //    "メッセージ１メッセージ１メッセージ１",
    //    "メッセージ２メッセージ２メッセージ２",
    //    "メッセージ３メッセージ３メッセージ３",
    //    "メッセージ４メッセージ４メッセージ４",
    //    "メッセージ５メッセージ５メッセージ５",
    //    "メッセージ６メッセージ６メッセージ６",
    //    "メッセージ７メッセージ７メッセージ７",
    //    "メッセージ８メッセージ８メッセージ８",
    //    "メッセージ９メッセージ９メッセージ９",
    //    "メッセージ１０メッセージ１０メッセージ１０"
	//];

    //// 画面を ロック（この画面専用）
    //lockScreen = function () {
    //    $("#progress-area").addClass("hide");
    //    $("#text-area").addClass("hide");

    //    // 吹き出しの作成
    //    var loader = $("<div class='loader'><div class='arrow-box'><p class='typ'></p></div></div>");

    //    loader.find("p").text(dialects[Math.floor(Math.random() * dialects.length)]);
    //    $("#white-out div.loader").replaceWith(loader);

    //    // 吹き出し文字の アニメーション
    //    var container = $(".typ");
    //    var speed = 120;
    //    var content = $(container).html();
    //    var text = $.trim(content);
    //    var newHtml = "";

    //    text.split("").forEach(function(v) {
    //        newHtml += '<span>' + v + '</span>';
    //    });
        
    //    $(container).html(newHtml);
        
    //    var txtNum = 0;
        
    //    setInterval(function() {
    //        $(container).find('span').eq(txtNum).css({opacity: 1});
    //        txtNum++
    //    }, speed);

    //    $("#white-out").removeClass("hide");

    //    //console.log("lockScreen");
    //    //alert("lockScreen");
    //};

    // 画面をロック（Safari、iOS ブラウザ対策）（この画面専用）
    promiseLockScreen = function (context) {
        var dfd = new $.Deferred();

        mgf.lockScreen();

        // 割り込みタイミングを作る
        $.ajax({
            type: "POST",
            url: "../Start/AjaxCheckSession",
            async: true
        }).done(function (data, textStatus, jqXHR) {
            try {
                // 死活チェックのみ
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0) {
                    dfd.resolveWith(context);
                } else {
                    dfd.rejectWith(context);
                }
            } catch (ex) {
                dfd.rejectWith(context);
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            dfd.rejectWith(context);
        });

        return dfd.promise();
    };

    // ヒストリーバックの禁止
    mgf.prohibitHistoryBack();

    //// ログイン
    //$(document).on("click", "div.submit-area button[type='submit']", function () {
    //    if ($("form")[0].checkValidity()) {
    //        // 画面をロック後、サブミット
    //        mgf.start.promiseLockScreen($(this));
    //    }

    //    return true;
    //});

    //ログイン処理
    function login() {

        //mgf.start.promiseLockScreen($(this));
        promiseLockScreen($(this));
        //iOSかどうかの判定処理
        var isiOS = navigator.userAgent.toLowerCase().indexOf('ios') !== -1;

        if (isiOS) {
            //iOSはローディングアニメーションが動かないので遅延
            var timer = setTimeout(function () {
                location.replace("../Start/LoginByAuId");
            }, 1000);
        } else {
            location.replace("../Start/LoginByAuId");
        }

    };

    function jotoLogin() {
        mgf.start.promiseLockScreen($(this))
            .then(function () {
                //
                try {

                    $("<form>", {
                        action: "../Start/LoginJotoIdResult",
                        method: "POST"
                    }).appendTo(document.body);

                    $("<input>").attr({
                        type: "hidden",
                        name: "__RequestVerificationToken",
                        value: $("input[name='__RequestVerificationToken']").val()
                    }).appendTo($("form:last"));

                    $("<input>").attr({
                        type: "hidden",
                        name: "userid",
                        value: $("#joto-userid").val()
                    }).appendTo($("form:last"));

                    $("<input>").attr({
                        type: "hidden",
                        name: "password",
                        value: $("#joto-password").val()
                    }).appendTo($("form:last"));

                    // サブミット
                    $("form:last").submit();


                } catch (ex) {
                    // 画面をアンロック
                    mgf.unlockScreen();
                }

            })
    }


    //ログイン処理
    function applelogin() {
        console.log("applelogin");
        //mgf.start.promiseLockScreen($(this));
        promiseLockScreen($(this));
        //iOSかどうかの判定処理
        var isiOS = navigator.userAgent.toLowerCase().indexOf('ios') !== -1;

        if (isiOS) {
            //iOSはローディングアニメーションが動かないので遅延
            console.log("ios applelogin");
            var timer = setTimeout(function () {
                location.replace("../Start/LoginByAppleId");
            }, 1000);
        } else {
            console.log("else applelogin");
            location.replace("../Start/LoginByAppleId");
        }

    };

    if ($("#agreement-check-modal") !== undefined && $("#agreement-check-modal").length > 0) {
        $('main').on("click", "#auid-login", function () {

            $("#agreement-check-modal").modal("show");
            $("#agreement-check-modal").data("login-type", "auid");
        });

        // 「ログイン」ボタン
        $('main').on("click", "#joto-id-login", function () {

            $("#agreement-check-modal").modal("show");
            $("#agreement-check-modal").data("login-type", "jotoid");
        });

        // 「ログイン」ボタン
        $('main').on("click", "#appleid-login", function () {

            console.log("#appleid-login");
            $("#agreement-check-modal").modal("show");
            $("#agreement-check-modal").data("login-type", "appleid");
        });


    } else {
        $('main').on("click", "#auid-login", function () {
            login();

        });
        $('main').on("click", "#joto-id-login", function () {
            jotoLogin();

        });
        $('main').on("click", "#appleid-login", function () {
            console.log("#appleid-login");
            applelogin();

        });
    }
            //}).done(function (data, textStatus, jqXHR) {
            //    console.log($(data))
            //    console.log($(textStatus))
            //    console.log($(jqXHR))
            //    try {
            //        if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("text/html;") >= 0) {
            //            console.log("200")
            //        } else {
            //             エラー
            //        }
            //    } catch (ex) {
            //         エラー
            //    }
            //}).fail(function (jqXHR, textStatus, errorThrown) {
            //     エラー
            //});

            //mgf.start.promiseLockScreen($(this));
            //location.replace("../Start/LoginByAuId");
    //    });
    //}

    //完了モーダル
    // 「×」ボタン
    $('main').on("click", "#agreement-check-modal div.modal-header button.close", function () {
        try {
            $("#agreement-check-modal").modal("hide");

        } catch (ex) {
            //console.log(ex);
        }
    });

    // 「閉じる」ボタン
    $('main').on("click", "#agreement-check-modal div.modal-footer button.btn-close", function () {
        try {
            $("#agreement-check-modal").modal("hide");

        } catch (ex) {
            //console.log(ex);
        }
    });

    // 「OK」ボタン
    $('main').on("click", "#agreement-check-modal div.modal-footer button.btn-submit", function () {
        try {

            $("#agreement-check-modal").modal("hide");
            if ($("#agreement-check-modal").data("login-type") == "auid") {
                login();

            } else if ($("#agreement-check-modal").data("login-type") == "jotoid") {
                jotoLogin();
            } else if ($("#agreement-check-modal").data("login-type") == "appleid") {
                console.log("#agreement #appleid-login");
                applelogin();
            }
        } catch (ex) {
            //console.log(ex);
        }
    });

    $("main").on("change", "#password-open", function () {
        if ($("#joto-password").attr("type") == "password") {
            $(this).next().removeClass('eye-close').addClass('eye-open')
            $("#joto-password").attr("type", "text");
        } else {
            $("#joto-password").attr("type", "password");
            $(this).next().removeClass('eye-open').addClass('eye-close')
        }
    })

    //$(document).on("click", "a.auid-login, a.wowid-login", function (event) {
    //    mgf.start.promiseLockScreen($(this));
    //});
    window.onunload = function () {
        mgf.unlockScreen();
    };

    var autoLogin = $("#auid-login").data("autologin");
    //console.log(autoLogin)
    if (autoLogin == 'True') {
        login();
    }

})();
