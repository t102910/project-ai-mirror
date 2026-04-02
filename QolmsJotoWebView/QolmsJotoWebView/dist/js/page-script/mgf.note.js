// mgf.note 名前空間 オブジェクト
mgf.note = mgf.note || {};

// バイタル グラフ の非同期 ロード 用
mgf.note.$jqXhrGraphs = [];

// 画面を ロック（Safari、iOS ブラウザ 対策）
mgf.note.promiseLockScreen = function (context) {
    return mgf.promiseLockScreen(context, "note");
};

// セッション の チェック
mgf.note.checkSession = function () {
    return mgf.checkSession("note");
};

// 非同期で セッション を チェック（Promise 版）
mgf.note.checkSessionAsync = function (context) {
    return mgf.checkSessionAsync(context, "note");
};

// メイン ウィンドウ からの セッション の チェック（廃止予定）
mgf.note.checkSessionByWindow = function () {
    return mgf.checkSessionByWindow("note");
};

// Ajax 要求向け セッション の チェック（廃止予定）
mgf.note.checkSessionByAjax = function () {
    if (!mgf.note.checkSession()) {
        location.href = "../Start/Login?ReturnUrl=" + location.pathname.toLowerCase().replace("result", "");
    }
};

// ページ トップ へ スクロール
$("html").on('click', ".topbtn__wrap", function () {
    mgf.scrollTop();
});
