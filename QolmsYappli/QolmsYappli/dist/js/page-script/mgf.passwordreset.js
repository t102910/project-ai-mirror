// mgf.start 名前空間 オブジェクト
mgf.passwordreset = mgf.passwordreset || {};

// 画面を ロック（Safari、iOS ブラウザ 対策）
mgf.passwordreset.promiseLockScreen = function (context) {
    return mgf.promiseLockScreen(context, "passwordreset");
};

// セッション の チェック
mgf.passwordreset.checkSession = function () {
    return mgf.checkSession("passwordreset");
};

// 非同期で セッション を チェック（Promise 版）
mgf.passwordreset.checkSessionAsync = function (context) {
    return mgf.checkSessionAsync(context, "passwordreset");
};

// メイン ウィンドウ からの セッション の チェック（廃止予定）
mgf.passwordreset.checkSessionByWindow = function () {
    return mgf.checkSessionByWindow("passwordreset");
};

// Ajax 要求向け セッション の チェック（廃止予定）
mgf.passwordreset.checkSessionByAjax = function () {
    if (!mgf.passwordreset.checkSession()) {
        location.href = "../Start/Login?ReturnUrl=" + location.pathname.toLowerCase().replace("result", "");
    }
};

// ヘッダー ロゴ 画像からの遷移
$(document).on("click", "#logo-area h1 a", function () {
    // 画面を ロック 後、遷移
    promiseLockScreen($(this))
        .then(function () {
            $(location).attr("href", $(this).attr("href"));
        });
    return false;
});

//$(window).unload(function () {
//    mgf.unlockScreen();
//});
