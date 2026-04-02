// mgf.portal.challengeentry 名前空間オブジェクト
mgf.portal.challengeentry = mgf.portal.challengeentry || {};

// DOM 構築完了
mgf.portal.challengeentry = (function () {

    // エントリー説明

    // --規約同意ページへ
	$('body').on("click", ".entry", function () {
		// 画面をロック
		mgf.lockScreen();
		$.ajax({
			type: "POST",
			url: "../Portal/ChallengeEntryResult?ActionSource=Entry",
			data: {
				__RequestVerificationToken: $("input[name='__RequestVerificationToken']").val()
			},
			async: true,
			beforeSend: function (jqXHR) {
				// セッションのチェック
				mgf.portal.checkSessionByAjax();
			}
		}).done(function (data, textStatus, jqXHR) {

		    if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("text/html;") >= 0) {

		        //パーシャルビューを書き換え
		        $(document).find("main").replaceWith($(data));
		        $("body").removeClass("ver-2")
				$("html,body").animate({ scrollTop: $('body').offset().top });

				//スクロールデザイン用JS
				var bb = $('.bottom-fix-wrap').outerHeight() + 100;
				var ds = $('.document-scroll-wrap').outerHeight();
				if ($(window).height() < bb + ds) {
					$('.document-scroll-wrap').css({ 'height': 'calc(100vh - ' + bb + 'px)' });

				}

		    } else if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 ) {
		        //失敗
                console.log("失敗") 
		    }

		}).always(function (jqXHR, textStatus) {
			// 画面をアンロック
			mgf.unlockScreen();
		});
		return false;
	});

    // 規約同意
    // --同意チェックでチャレンジボタンの表示を切替
	$("body").on("change", "#policy", function () {

	    if ($(this).prop('checked')) {
	        console.log("checked");
	        $("a.agree").removeClass("disabled")

	    } else {
	        console.log("checked=false");

	        $("a.agree").addClass("disabled")
	    }
	});

	// --チャレンジ資格確認ページへ
	$('body').on("click", ".agree", function () {
		// 画面をロック
		mgf.lockScreen();
		$.ajax({
			type: "POST",
			url: "../Portal/ChallengeEntryResult?ActionSource=Agree",
			data: {
				__RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
				Agreement: $("#policy").prop("checked")
			},
			async: true,
			beforeSend: function (jqXHR) {
				// セッションのチェック
				mgf.portal.checkSessionByAjax();
			}
		}).done(function (data, textStatus, jqXHR) {

			if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("text/html;") >= 0) {

				//パーシャルビューを書き換え
				$(document).find("main").replaceWith($(data));
				$("html,body").animate({ scrollTop: $('body').offset().top });

			}

		}).always(function (jqXHR, textStatus) {
			// 画面をアンロック
			mgf.unlockScreen();
		});
		return false;
	});


	$("body").on("click", ".pass-check", function () {

	    mgf.lockScreen();

	    var pass = $('input[name="pass"]').val();
	    //console.log(pass);

	    var len = $(".add").length;
	    var value = {};
	    var result = {};
	    $(".add").each(function (key, value) {

	    //    console.log($(value).prop("name"));
	    //    console.log(value)
	        var name = $(value).prop("name");
	        var val = $(value).val();

	        value[name] = val;

	        //console.log(key + ' : ' + name + ":"+ val);

	        if (result[name]) {
	            if (Array.isArray(result[name])) {
	                result[name].push(val);
	            }
	            else {
	                result[name] = new Array(result[name], val);
	            }
	        }
	        else {
	            result[name] = val;
	        }
	    });

	    var checked = 0
	    $('#connection-data :checked').each(function () {
	        //値を取得
	        var val = $(this).data("content");
	        console.log(val);
	        checked = checked + val;
	    });

	    console.log(checked);
	    //console.log(result);

	    $.ajax({
	        type: "POST",
	        url: "../Portal/ChallengeEntryResult?ActionSource=Pass",
	        data: {
	            __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
	            pass: pass,
	            values: result,
	            checked: checked
	        },
	        async: true,
	        beforeSend: function (jqXHR) {
	            // セッションのチェック
	            mgf.portal.checkSessionByAjax();
	        }
	    }).done(function (data, textStatus, jqXHR) {

	       if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 ) {
	           console.log($.parseJSON(data))

	           if ($.isTrueString($.parseJSON(data)["IsSuccess"])) {
	               location.href = "../portal/challengedetail?key="+ $.parseJSON(data)["Key"]

	           } else {

	               var messages = $.parseJSON(data)["Messages"];
	               console.log($.parseJSON(data)["Messages"])

	               var messagebody = "";

	               $(messages).each(function (key, value) {

	                   messagebody = messagebody + value["Value"] +"<br/>"

	                   //$("[name='" + value["Key"] + "']").nextAll(".caution").text().removeClass("hide");
	               })

	               console.log($.parseJSON(data)["Messages"])
	               $("#error-modal .modal-body").html(messagebody);
	               $("#error-modal").modal("show");
	           }
	           
	           mgf.unlockScreen();

	        }
	    }).always(function (jqXHR, textStatus) {
	        // 画面をアンロック
	        mgf.unlockScreen();
	    });
	    return false;
	});



	// --
	$('body').on("click", "#address-search", function () {
		// 画面をロック
		mgf.lockScreen();
		$.ajax({
			type: "POST",
			url: "../Portal/ChallengeEntryResult?ActionSource=PostCode",
			data: {
				__RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
				PostCode: $("#postcode").val()
			},
			async: true,
			beforeSend: function (jqXHR) {
				// セッションのチェック
				mgf.portal.checkSessionByAjax();
			}
		}).done(function (data, textStatus, jqXHR) {

			if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0) {

				//パーシャルビューを書き換え
				//$(document).find("main").replaceWith($(data));
				//$("html,body").animate({ scrollTop: $('body').offset().top });

				var address = $.parseJSON(data)["Address"];
				$("#address").val(address);
				$("#address").focus();
			}

		}).always(function (jqXHR, textStatus) {
			// 画面をアンロック
			mgf.unlockScreen();
		});
		return false;
	});
	


	$("body").on("click", "#error-modal .close", function () {
	    $("#error-modal").modal("hide");
	})

	$("body").on("click", "#error-modal .btn-close", function () {
	    $("#error-modal").modal("hide");
	})

	$("body").on("click", "#Information", function () {

	    return false;
	});

	// ヒストリー バック禁止
	mgf.prohibitHistoryBack();


})();