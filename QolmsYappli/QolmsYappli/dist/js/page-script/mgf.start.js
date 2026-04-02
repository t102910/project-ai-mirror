// mgf.start 名前空間 オブジェクト
mgf.start = mgf.start || {};

// 画面を ロック（Safari、iOS ブラウザ 対策）
mgf.start.promiseLockScreen = function (context) {
    return mgf.promiseLockScreen(context, "start");
};

// セッション の チェック
mgf.start.checkSession = function () {
    return mgf.checkSession("start");
};

// 非同期で セッション を チェック（Promise 版）
mgf.start.checkSessionAsync = function (context) {
    return mgf.checkSessionAsync(context, "start");
};

// メイン ウィンドウ からの セッション の チェック（廃止予定）
mgf.start.checkSessionByWindow = function () {
    return mgf.checkSessionByWindow("start");
};

// Ajax 要求向け セッション の チェック（廃止予定）
mgf.start.checkSessionByAjax = function () {
    if (!mgf.start.checkSession()) {
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
