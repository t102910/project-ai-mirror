// mgf.health 名前空間 オブジェクト
mgf.health = mgf.health || {};

// 健康年齢 レポート の非同期 ロード 用
mgf.health.$jqXhrReports = [];

// 画面を ロック（Safari、iOS ブラウザ 対策）
mgf.health.promiseLockScreen = function (context) {
    return mgf.promiseLockScreen(context, "health");
};

// セッション の チェック
mgf.health.checkSession = function () {
    return mgf.checkSession("health");
};

// 非同期で セッション を チェック（Promise 版）
mgf.health.checkSessionAsync = function (context) {
    return mgf.checkSessionAsync(context, "health");
};

// メイン ウィンドウ からの セッション の チェック（廃止予定）
mgf.health.checkSessionByWindow = function () {
    return mgf.checkSessionByWindow("health");
};

// Ajax 要求向け セッション の チェック（廃止予定）
mgf.health.checkSessionByAjax = function () {
    if (!mgf.health.checkSession()) {
        location.href = "../Start/Login?ReturnUrl=" + location.pathname.toLowerCase().replace("result", "");
    }
};

// ページ トップ へ スクロール
$("html").on('click', ".topbtn__wrap", function () {
    mgf.scrollTop();
});
