//  名前空間オブジェクト
mgf.portal.companyconnectionhome = mgf.portal.companyconnectionhome || {};

mgf.portal.companyconnectionhome = (function () {

    console.log("js");

    $("main").on("click", ".menu-list", function () {

        console.log("click");
        if ($(this).hasClass("examination")){

            location.href = "../note/examination"
        }
        else if ($(this).hasClass("mov-for-female")) {

            location.href = "../portal/movforfemale"
        }
        
    });

    mgf.prohibitHistoryBack();

})();