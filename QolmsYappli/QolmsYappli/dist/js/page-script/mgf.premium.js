// mgf.start 名前空間 オブジェクト
mgf.premium = mgf.premium || {};

// 画面を ロック（Safari、iOS ブラウザ 対策）
mgf.premium.promiseLockScreen = function (context) {
    return mgf.promiseLockScreen(context, "premium");
};

// セッション の チェック
mgf.premium.checkSession = function () {
    return mgf.checkSession("premium");
};

// 非同期で セッション を チェック（Promise 版）
mgf.premium.checkSessionAsync = function (context) {
    return mgf.checkSessionAsync(context, "premium");
};

// メイン ウィンドウ からの セッション の チェック（廃止予定）
mgf.premium.checkSessionByWindow = function () {
    return mgf.checkSessionByWindow("premium");
};

// Ajax 要求向け セッション の チェック（廃止予定）
mgf.premium.checkSessionByAjax = function () {
    if (!mgf.premium.checkSession()) {
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

// ページ トップ へ スクロール
$("html").on('click', ".topbtn__wrap", function () {
    mgf.scrollTop();
});
