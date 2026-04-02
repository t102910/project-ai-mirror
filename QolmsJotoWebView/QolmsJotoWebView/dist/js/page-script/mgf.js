// Boolean 文字列の判定
$.extend({
    isTrueString: function isTrueString(obj) {
        return (typeof (obj) == "string" && obj.toLowerCase() == "true");
    },
    isFalseString: function isFalseString(obj) {
        return (typeof (obj) == "string" && obj.toLowerCase() == "false");
    }
});

// date 型を "yyyy/MM/dd" 形式の文字列へ変換
$.extend({
    toYmdDateString: function toYmdString(obj) {
        var result = "";

        if ($.type(obj) == $.type(new Date())) {
            var result = [
                obj.getFullYear(),
                ('0' + (obj.getMonth() + 1)).slice(-2),
                ('0' + obj.getDate()).slice(-2)
            ].join('/');
        }

        return result;
    }
});

// FormData 型を JSON オブジェクトへ変換（暫定）
// Preview Yappli + iOS 環境で FormData を Ajax で送信できなくなったため（2020/03/11 時点）
// 配列が ネスト するような複雑な構造は想定外
// IE 11 は未対応（対応するなら https://unpkg.com/formdata-polyfill の使用を検討）
$.extend({
    toJsonObject: function toJsonObject(obj) {
        var result = {};

        if ($.type(obj) == $.type(new FormData())) {
            obj.forEach(function (value, key) {
                if (result[key]) {
                    if (Array.isArray(result[key])) {
                        result[key].push(value);
                    }
                    else {
                        result[key] = new Array(result[key], value);
                    }
                }
                else {
                    result[key] = value;
                }
            });
        }

        return result;
    }
});

// ルート 名前空間 オブジェクト
var mgf = mgf || {};

// ヒストリー バック の禁止
mgf.prohibitHistoryBack = function () {
    try {
        history.pushState(null, null, null);

        window.addEventListener("popstate", function () {
            history.pushState(null, null, null);
        });
    } catch (ex) { }
};

// 画面を ロック
// loader.gif 廃止に伴い、共通ローディング表示は行わない
mgf.lockScreen = function () {
    return true;
};

// 画面を アンロック
mgf.unlockScreen = function () {
    return true;
};

// 画面を ロック（Safari、iOS ブラウザ 対策）
mgf.promiseLockScreen = function (context, controllerName) {
    var dfd = new $.Deferred();

    mgf.lockScreen();

    // 割り込み タイミング を作る
    $.ajax({
        type: "POST",
        url: "../" + controllerName + "/AjaxCheckSession",
        async: true
    }).done(function (data, textStatus, jqXHR) {
        try {
            // 死活 チェック のみ
            if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0) {
                // 成功
                dfd.resolveWith(context);
            } else {
                // 失敗
                dfd.rejectWith(context);
            }
        } catch (ex) {
            // 失敗
            dfd.rejectWith(context);
        }
    }).fail(function (jqXHR, textStatus, errorThrown) {
        // 失敗
        dfd.rejectWith(context);
    });

    return dfd.promise();
};

// セッション を チェック
mgf.checkSession = function (controllerName) {
    var result = false;

    $.ajax({
        type: "POST",
        url: "../" + controllerName + "/AjaxCheckSession",
        async: false
    }).done(function (data, textStatus, jqXHR) {
        try {
            if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) result = true;
        } catch (ex) { }
    }).fail(function (jqXHR, textStatus, errorThrown) {
        throw new Error("Session Check Error.");
    });

    return result;
};

// 非同期で セッション を チェック（Promise 版）
mgf.checkSessionAsync = function (context, controllerName) {
    var dfd = new $.Deferred();

    $.ajax({
        type: "POST",
        url: "../" + controllerName + "/AjaxCheckSession",
        async: true
    }).done(function (data, textStatus, jqXHR) {
        try {
            if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {
                // 成功
                dfd.resolve(context);
            } else {
                // 失敗
                dfd.reject(context);
            }
        } catch (ex) {
            // 失敗
            dfd.reject(context);
        }
    }).fail(function (jqXHR, textStatus, errorThrown) {
        // 失敗
        dfd.reject(context);
    });

    return dfd.promise();
};

// セッション を チェック し、無効なら「ログイン」画面へ遷移（廃止予定）
mgf.checkSessionByWindow = function (controllerName) {
    $.ajax({
        type: "POST",
        url: "../" + controllerName + "/AjaxCheckSessionByWindow",
        async: false
    }).fail(function (jqXHR, textStatus, errorThrown) {
        throw new Error("Session Check Error.");
    });
};

// 「ログイン」画面へ遷移
mgf.redirectToLogin = function () {
    mgf.lockScreen();
    location.href = "../Start/Login";
};

// 「ログイン」画面へ遷移（ReturnUrl付き）
mgf.redirectToLoginWithReturnUrl = function () {
    mgf.lockScreen();
    location.href = "../Start/Login?ReturnUrl=" + location.pathname.toLowerCase().replace("result", "");
};

// ページ トップ へ戻る ボタン の表示切替
$(function () {
    if ($(".topbtn__wrap") != undefined) {
        //console.log($(".topbtn__wrap"))

        var position = '';
        var speed = 400;

        $(window).scroll(function () {
            //console.log($(this).scrollTop())

            if ($(this).scrollTop() > position && $(this).scrollTop() > 900) {
                //console.log('fadeIn')
                $(".topbtn__wrap").removeClass("hide");
                $(".topbtn__wrap").fadeIn(speed, function () {
                    return false;
                });
            } else if ($(this).scrollTop() < position && $(this).scrollTop() < 900) {
                //console.log('fadeOut')
                $(".topbtn__wrap").fadeOut(speed, function () {
                    $(".topbtn__wrap").addClass("hide");
                    return false;
                });
            }
            position = $(this).scrollTop();

            false;
        });
    }
});

// ページ トップ へ スクロール
mgf.scrollTop = function () {

    var speed = 400;
    $('body,html').animate({ scrollTop: 0 }, speed, 'swing');
    false;
}
