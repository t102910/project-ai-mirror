// mgf.point 名前空間 オブジェクト
mgf.point = mgf.point || {};

// 画面を ロック（Safari、iOS ブラウザ 対策）
mgf.point.promiseLockScreen = function (context) {
    return mgf.promiseLockScreen(context, "point");
};

// セッション の チェック
mgf.point.checkSession = function () {
    return mgf.checkSession("point");
};

// 非同期で セッション を チェック（Promise 版）
mgf.point.checkSessionAsync = function (context) {
    return mgf.checkSessionAsync(context, "point");
};

// メイン ウィンドウ からの セッション の チェック（廃止予定）
mgf.point.checkSessionByWindow = function () {
    return mgf.checkSessionByWindow("point");
};

//// Ajax 要求向け セッション の チェック（廃止予定）
//mgf.point.checkSessionByAjax = function () {
//    if (!mgf.point.checkSession()) {
//        location.href = "../Start/Login?ReturnUrl=" + location.pathname.toLowerCase().replace("result", "");
//    }
//};

// ページ トップ へ スクロール
$("html").on('click', ".topbtn__wrap", function () {
    mgf.scrollTop();
});
