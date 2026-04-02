// mgf.portal.medicineconnectionsearch 名前空間オブジェクト
mgf.portal.medicineconnectionsearch = mgf.portal.medicineconnectionsearch || {};

// DOM 構築完了
mgf.portal.medicineconnectionsearch = (function () {
    // ヒストリー バック禁止
    mgf.prohibitHistoryBack();

    $("main").on("click", ".facility", function () {

        var facilitykey = $(this).data("facilitykey");
        var linkage = $(this).data("linkage");
        var frompageno = $(".home-btn-wrap").data("pageno");
        var link = "../portal/medicineconnectionagreement?";
        link = link + "linkageSystemNo=" + linkage;
        link = link + "&facilitykey=" + facilitykey;
        link = link + "&fromPageNo=" + frompageno;

        console.log(link);
        location.href = link;

    })


    $("main").on("click", ".page-link", function () {

        var index = $(this).data("page-index");
        var link = "../portal/medicineconnectionsearch?pageindex=" + index + "&fromPageNo=" + $(".home-btn-wrap").data("pageno");

        console.log(link);
        location.href = link;

    })


})();