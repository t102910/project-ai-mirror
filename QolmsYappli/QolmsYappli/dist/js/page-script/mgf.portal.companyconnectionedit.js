//  名前空間オブジェクト
mgf.portal.companyconnectionedit = mgf.portal.companyconnectionedit || {};

mgf.portal.companyconnectionedit = (function () {

    $("main").on("click", "#request", function () {
        mgf.lockScreen()

        $(".caution").addClass("hide");
        var linkageSystemNo = $("#request").data('no');
        console.log(linkageSystemNo);

        var mail = $("#mail").val();
        console.log(mail);

        var checked = 0
        $('#connection-data :checked').each(function () {
            //値を取得
            var val = $(this).data("content");
            console.log(val);
            checked = checked + val;
        });

        $.ajax({
            type: "POST", 
            url: "../Portal/CompanyConnectionEditResult",
            data: {
                __editVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                LinkageSystemNo: linkageSystemNo,
                RelationContentFlags: checked,
                MailAddress: mail
            },
            async: true,
            beforeSend: function (jqXHR) {
                mgf.portal.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {
            //console.log(data)
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {
                    //成功した時の表示
                    console.log("success")
                    console.log($.parseJSON(data)["Message"])
                    console.log($.parseJSON(data)["LinkageSystemNo"])

                    location.href = "../Portal/CompanyConnection?linkagesystemno=" + $.parseJSON(data)["LinkageSystemNo"]+"&fromPageNo="+ $(".home-btn-wrap").data("pageno");

                }
                else {

                    var messages = $.parseJSON(data)["Massages"];
                    console.log($.parseJSON(data)["Massages"])

                    var summaryMessage = true
                    $(messages).each(function (key, value) {

                        if (value["Key"] == "summary") {
                            $("#summary-cation").text(value["Value"]).removeClass("hide");
                            summaryMessage = false

                        } else {

                            $("[name='" + value["Key"] + "']").nextAll(".caution").text(value["Value"]).removeClass("hide");
                        }
                    })
                    if (summaryMessage) {
                        $("#summary-cation").text("入力内容を確認してください。").removeClass("hide");

                    }
                    mgf.unlockScreen();

                }
            } catch (ex) {

                mgf.unlockScreen();

            }
        }).fail(function (data, textStatus, jqXHR) {

            $("#caution").text("");
            $("#caution").append($("<p>" + "登録処理に失敗しました。5秒後に自動的に再読み込みします。お手数ですが再読み込み後に再度お試しください。" + "</p>"))
            $("#caution").removeClass("hide");

            //mgf.unlockScreen();
            //var timer = setTimeout(function () {
            //    location.href = "../Portal/AlkooConnection"
            //}, 5000);
        });
        return false;

    });

    $("main").on("click", "#Information", function () {

        return false;
    });

    mgf.prohibitHistoryBack();

})();