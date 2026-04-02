// mgf.portal.localidverificationrequest 名前空間オブジェクト
mgf.portal.localidverificationrequest = mgf.portal.localidverificationrequest || {};

// DOM 構築完了
mgf.portal.localidverificationrequest = (function () {

    var bb = $('.bottom-fix-wrap').outerHeight() + 100;
    var ds = $('.document-scroll-wrap').outerHeight();
    if ($(window).height() < bb + ds) {
        $('.document-scroll-wrap').css({ 'height': 'calc(100vh - ' + bb + 'px)' });

    }

    //クリップボードコピー
    $("main").on("click", ".copy-tapable", function () {
        copyTextToClipboard($(this).data("copy"));
    });

    /**
     * クリップボードコピー関数
     * 入力値をクリップボードへコピーする
     * [引数]   textVal: 入力値
     * [返却値] true: 成功　false: 失敗
     */
    function copyTextToClipboard(textVal) {
        //console.log(textVal);

        // テキストエリアを用意する
        var copyFrom = document.createElement("textarea");
        // テキストエリアへ値をセット
        copyFrom.textContent = textVal;

        // bodyタグの要素を取得
        var bodyElm = document.getElementsByTagName("body")[0];
        // 子要素にテキストエリアを配置
        bodyElm.appendChild(copyFrom);

        // テキストエリアの値を選択
        copyFrom.select();
        // コピーコマンド発行
        var retVal = document.execCommand('copy');
        // 追加テキストエリアを削除
        bodyElm.removeChild(copyFrom);
        // 処理結果を返却
        return retVal;
    }

    //コピーモーダル
    $("main").on("click", ".copy-tapable", function () {
        copyTextToClipboard($(this).data("copy"));

        //alert("copy!");
        $("#copy-modal").modal("show");
    });

    $("main").on("shown.bs.modal", "#copy-modal", function (e) {
        setTimeout(function () {
            $("#copy-modal").modal("hide");
        }, 300);
        //alert("");
    });
    // ヒストリー バック禁止
    mgf.prohibitHistoryBack();

})();