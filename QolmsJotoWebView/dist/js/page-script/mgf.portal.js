// mgf.portal 名前空間 オブジェクト
mgf.portal = mgf.portal || {};

// 画面を ロック（Safari、iOS ブラウザ 対策）
mgf.portal.promiseLockScreen = function (context) {
    return mgf.promiseLockScreen(context, "portal");
};

// セッション の チェック
mgf.portal.checkSession = function () {
    return mgf.checkSession("portal");
};

// 非同期で セッション を チェック（Promise 版）
mgf.portal.checkSessionAsync = function (context) {
    return mgf.checkSessionAsync(context, "portal");
};

// メイン ウィンドウ からの セッション の チェック（廃止予定）
mgf.portal.checkSessionByWindow = function () {
    return mgf.checkSessionByWindow("portal");
};

// Ajax 要求向け セッション の チェック（廃止予定）
mgf.portal.checkSessionByAjax = function () {
    if (!mgf.portal.checkSession()) {
        location.href = "../Start/Login?ReturnUrl=" + location.pathname.toLowerCase().replace("result", "");
    }
};

// ページ トップ へ スクロール
$("html").on('click', ".topbtn__wrap", function () {
    mgf.scrollTop();
});
