// mgf.integration 名前空間 オブジェクト
mgf.integration = mgf.integration || {};

// 画面を ロック（Safari、iOS ブラウザ 対策）
mgf.integration.promiseLockScreen = function (context) {
    return mgf.promiseLockScreen(context, "integration");
};

// セッション の チェック
mgf.integration.checkSession = function () {
    return mgf.checkSession("integration");
};

// 非同期で セッション を チェック（Promise 版）
mgf.integration.checkSessionAsync = function (context) {
    return mgf.checkSessionAsync(context, "integration");
};

// メイン ウィンドウ からの セッション の チェック（廃止予定）
mgf.integration.checkSessionByWindow = function () {
    return mgf.checkSessionByWindow("integration");
};

//// Ajax 要求向け セッション の チェック（廃止予定）
//mgf.integration.checkSessionByAjax = function () {
//    if (!mgf.integration.checkSession()) {
//        location.href = "../Start/Login?ReturnUrl=" + location.pathname.toLowerCase().replace("result", "");
//    }
//};

// ページ トップ へ スクロール
$("html").on('click', ".topbtn__wrap", function () {
    mgf.scrollTop();
});
