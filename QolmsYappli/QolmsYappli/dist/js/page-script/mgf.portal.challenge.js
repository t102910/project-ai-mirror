// mgf.portal.challenge 名前空間オブジェクト
mgf.portal.challenge = mgf.portal.challenge || {};

// DOM 構築完了
mgf.portal.challenge = (function () {
    // ヒストリー バック禁止
    mgf.prohibitHistoryBack();

    $("#main-cont").on("click", "a.box", function () {

        var key = $(this).data("key");
        var url = "../portal/challengeentry?key="

        console.log(key);
        console.log(url + key);

        location.href = url+key

        return false;
    });

})();