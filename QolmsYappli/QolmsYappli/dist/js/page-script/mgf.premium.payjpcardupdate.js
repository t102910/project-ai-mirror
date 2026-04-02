mgf.premium.payjpcardregister = mgf.premium.payjpcardregister || {};

mgf.premium.payjpcardregister = (function () {

    // ヒストリーバックの禁止
    mgf.prohibitHistoryBack();

	//カード情報登録
    $("main").on("click", "#Register", function () {
    	var token = $(document).find("input[name='payjp-token']").val();

    	// 画面をロック
    	mgf.lockScreen();
    	$.ajax({
    		type: "POST",
            url: "../Premium/PayJpCardUpdateResult?ActionSource=tds",
    		data: {
    			__RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                token: token,
                dummy:''
    		},
    		async: true,
    		beforeSend: function (jqXHR) {
    		    mgf.premium.checkSessionByAjax();
    		}
    	}).done(function (data, textStatus, jqXHR) {
    		try {
       			//console.log(data)
    			if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {
    			    //成功した時の表示
    			    //var msg = $.parseJSON(data)["Message"]
    			    var msghtml = '<div class="modal-body"><p>' + $.parseJSON(data)["Message"] + '</p>'
    			    if ($.isTrueString($.parseJSON(data)["IsExpiration"])) {

    			        var arr = $.parseJSON(data)["ExpDate"].split("/")
    			        var day = new Date(arr[0], arr[1] - 1, arr[2]);
    			        //console.log(day)
    			        // 年月日・時分の取得
    			        var year = day.getFullYear();
    			        var month = day.getMonth()+1;

    			        var dstr = year + '年' + month + '月'
    			        //console.log(dstr)
    			        msghtml += '<p class="red">' + '※クレジットカードの有効期限が近づいています。'
    			        msghtml += dstr + '中にカード情報を更新してください。'+ '</p>'
    			    }
    			    msghtml += '</div>'

    			    //$("#finish-modal").find(".modal-body").replaceWith(msghtml)

    			    //$("#finish-modal .modal-body").text(msg);
    			    //$("#finish-modal").modal("show");
    			    $("#caution").addClass("hide");
                    location.href ='../Premium/PayJpCardUpdateTokenRedirect'
                }
    			else {
    				$("#caution").text("");
    				$("#caution").text($.parseJSON(data)["Message"]);
    				//console.log($("#caution").text())
    				$("#caution").removeClass("hide");
    			}
    		} catch (ex) {
    		    $("#caution").text("");
    		    $("#caution").text("エラーが発生しました。恐れ入りますが、最初からやり直してください。");
    		    //console.log($("#caution").text())
    		    $("#caution").removeClass("hide");
            }

    	}).fail(function (jqXHR, textStatus) {
    	    $("#caution").text("");
    	    $("#caution").text("サーバーエラーが発生しました。サポート窓口へご連絡ください。");
    	    //console.log($("#caution").text())
    	    $("#caution").removeClass("hide");
    	}).always(function (jqXHR, textStatus) {
    		// 画面をアンロック
    		mgf.unlockScreen();
    	});
    	return false;
    });


    ////完了モーダル
    //// 「×」ボタン
    //$("main").on("click", "#finish-modal div.modal-header button.close", function () {
    //    try {
    //        $("#finish-modal").modal("hide");
    //        location.href = '../Premium/Index'
    //    } catch (ex) {
    //        //console.log(ex);
    //    }
    //});

    //// 「閉じる」ボタン
    //$("main").on("click", "#finish-modal div.modal-footer button.btn-close", function () {
    //    try {
    //        $("#finish-modal").modal("hide");
    //        location.href = '../Premium/Index'
    //    } catch (ex) {
    //        //console.log(ex);
    //    }
    //});

})();

