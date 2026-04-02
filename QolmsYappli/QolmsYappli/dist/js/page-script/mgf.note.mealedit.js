// mgf.note.mealedit 名前空間オブジェクト
mgf.note.mealedit = mgf.note.mealedit || {};

// DOM 構築完了
mgf.note.mealedit = (function () {
    // 画像、ファイル処理用
	const THUMBNAIL_WIDTH = 2000; // 画像リサイズ後の横の長さの最大値
	const THUMBNAIL_HEIGHT = 2000; // 画像リサイズ後の縦の長さの最大値
    const CONTENT_TYPES = ["image/jpeg", "image/png", "image/bmp", "image/x-bmp", "image/x-ms-bmp"];

    var $uploadFile = null;
    var $thumbnailArea = $(".edit-thumbnail-area");
    var $uploadImg =  $thumbnailArea.find("img");
    var $uploadAlert = null;
    var objFile = null;

    // 食事種別変更時、時刻を設定
    $("main").on("change", "input[name='mealtype']", function () {
        var mealType = $("#mealtype input[type='radio']:checked").val();
        var time1 = "";
        var time2 = "";
        var time3 = "0";

        switch (mealType) {
            case "Breakfast":
                time1 = "am";
                time2 = 7;
                break;
            case "Lunch":
                time1 = "pm";
                time2 = 0;
                break;
            case "Dinner":
                time1 = "pm";
                time2 = 7;
                break;
            default:
                return false;
        }

        $("select[name='meridiem']").val(time1);
        $("select[name='hour']").val(time2);
        $("select[name='minute']").val(time3);

        return false;
    });

    // 「解析結果」から選択
    $("main").on("click", ".meal", function () {

        var itemName = $(this).data("name");
        var itemCal = $(this).data("cal");

        $("#hinmoku-2").val(itemName);
        $("#cal-2").val(itemCal);

        $(".submit-area .btn-submit.upload").removeClass("disabled")

        // 登録ボタンの位置(画面下部)までスクロールする
        scrolltoBottom();
    });

    // 「続きを表示」ボタン
    $("main").on("click", "a.btn-showmore", function () {

        mgf.lockScreen();

        $.ajax({
            type: "POST",
            url: "../Note/MealEditResult?ActionSource=ShowMore",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                page: $(this).data("page")
            },
            async: true,
            beforeSend: function (jqXHR) {
                // セッションのチェック
                mgf.note.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("text/html;") >= 0) {
                    // 成功

                    // パーシャル ビューを書き換え
                    $(document).find("a.btn-showmore").replaceWith($(data));
                    // 画面をアンロック
                    mgf.unlockScreen();
                } else {

                    // 画面をアンロック
                    mgf.unlockScreen();
                }
            } catch (ex) {
                // エラー

                // 画面をアンロック
                mgf.unlockScreen();
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            // エラー
            // 画面をアンロック
            mgf.unlockScreen();
        });

        return false;
    });

    // 「品名」が入力されたら「検索」ボタンを有効化・無効化
    $("main").on("input keyup blur", "#searchText", function () {
        if ($.trim($("#searchText").val()) == "") {
            $("#search").addClass("disabled");
        } else {
            $("#search").removeClass("disabled");
        }
    });

    // 「検索」ボタン
    $("main").on("click", "#search", function () {
        // 時間
        var $select = $(".target-date-time .datetime-select .form-control:input");
        var time1 = $select.eq(0).val();
        var time2 = $select.eq(1).val();
        var time3 = $select.eq(2).val();

        //// 画面をロック後、サブミット
        //mgf.note.promiseLockScreen($(this))
        //    .then(function () {
        //        //
        //        try {
        //            $("<form>", {
        //                action: "../Note/Meal4Result?ActionSource=Search",
        //                method: "POST"
        //            }).appendTo(document.body);

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "__RequestVerificationToken",
        //                value: $("input[name='__RequestVerificationToken']").val()
        //            }).appendTo($("form:last"));

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "SearchText",
        //                value: $("#searchText").val()
        //            }).appendTo($("form:last"));

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "model.RecordDate",
        //                value: $("#record-date").val()
        //            }).appendTo($("form:last"));

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "model.MealType",
        //                value: $("#mealtype input[type='radio']:checked").val()
        //            }).appendTo($("form:last"));

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "model.Meridiem",
        //                value: time1
        //            }).appendTo($("form:last"));    

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "model.Hour",
        //                value: time2
        //            }).appendTo($("form:last"));

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "model.Minute",
        //                value: time3
        //            }).appendTo($("form:last"));

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "model.PhotoData",
        //                value: $uploadImg.attr("src")
        //            }).appendTo($("form:last"));

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "model.PhotoName",
        //                value: $uploadImg.data("file-name")
        //            }).appendTo($("form:last"));

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "PageType",
        //                value: "Edit"
        //            }).appendTo($("form:last"));

        //            // サブミット
        //            $("form:last").submit();

        //        } catch (ex) {
        //            // 画面をアンロック
        //            mgf.unlockScreen();
        //        }
        //    })

        // 画面をロック
        mgf.lockScreen();

        // エラークリア
        $(".caution").text("").addClass("hide");

        // POST 内容を構築
        var fd = new FormData();

        fd.append("__RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());
        fd.append("SearchText", $("#searchText").val());
        fd.append("model.RecordDate", $("#record-date").val());
        fd.append("model.MealType", $("#mealtype input[type='radio']:checked").val());
        fd.append("model.Meridiem", time1);
        fd.append("model.Hour", time2);
        fd.append("model.Minute", time3);
        fd.append("model.PhotoData", $uploadImg.attr("src"));
        fd.append("model.PhotoName", $uploadImg.data("file-name"));
        fd.append("model.ItemName", "不明");
        fd.append("model.Calorie", "0");
        fd.append("PageType", "Edit");

        // 登録内容を POST
        $.ajax({
            type: "POST",
            url: "../Note/Meal4Result?ActionSource=Search",
            //processData: false,
            //contentType: false,
            //data: fd,
            traditional: true,
            data: $.toJsonObject(fd),
            async: true,
            beforeSend: function (jqXHR) {
                // セッションのチェック
                mgf.note.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {
                    // 成功

                    // リダイレクト
                    location.href = "../Note/MealSearchResult";

                    return false;

                } else if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0) {
                    // エラー
                    var messages = $.parseJSON(data)["Messages"];
                    var length = messages.length;
                    var $alert = $("#caution");

                    if (length == 1) {
                        $alert.text(messages[0]).removeClass("hide");
                    } else {
                        $alert.text("");

                        $.each(messages, function (i, v) {
                            $alert.append(v);
                            if (i != length - 1) $alert.append("<br />");
                        });

                        $alert.removeClass("hide");
                    }

                    // 画面をアンロック
                    mgf.unlockScreen();
                } else {
                    // エラー
                    $("#caution").text("失敗しました。status:" + jqXHR.status).removeClass("hide");

                    // 画面をアンロック
                    mgf.unlockScreen();
                }
            } catch (ex) {
                //console.log(ex);
                // エラー
                $("#caution").text("失敗しました。exception:" + ex.message).removeClass("hide");

                // 画面をアンロック
                mgf.unlockScreen();
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            // エラー
            $("#caution").text("失敗しました。status:" + jqXHR.status + " error:" + errorThrown).removeClass("hide");

            // 画面をアンロック
            mgf.unlockScreen();
        });

        return false;
    });

    // 「品目」「カロリー」が入力されたら「登録」ボタンを有効化
    $("main").on("input keyup blur", ".input-area-inner input.input", function () {
        changeUploadBtnDisabled();
    });

    // 「品目」「カロリー」が入力されたら「登録」ボタンを有効化
    function changeUploadBtnDisabled() {
        if (($.trim($("#hinmoku-2").val()) == "") || ($.trim($("#cal-2").val()) == "")) {
            $(".submit-area .btn-submit.upload").addClass("disabled")
        } else {
            $(".submit-area .btn-submit.upload").removeClass("disabled")
        }
    };

    // 「写真を変更」ボタン
    $("main").on("click", "#sample1", function () {
        // 画面をロック
        mgf.lockScreen();

        // 1秒後に画面をアンロック
        setTimeout(
            function(){
                mgf.unlockScreen();
            }
            ,"1000");
    });

    // ファイルの追加
    $("main").on("change", "input[type=file]", function () {
        // 初期化
        var message = "jpg、jpeg、png、bmpファイルを選択してください。";

        $uploadAlert = $("#photo-caution");
        $uploadFile = $(this);

        // エラークリア
        $(".caution").text("").addClass("hide");

        // ファイルを取得
        objFile = $uploadFile.prop("files")[0];

        if ($.inArray(objFile.type, CONTENT_TYPES) >= 0) {
            // 画面をロック
            mgf.lockScreen();

            // 画像をリサイズする
            var image = new Image();
            var reader = new FileReader();

            // ファイルの読み込みに成功
            reader.onload = function(e) {
                // 画像の読み込みに成功
                image.onload = function() {
                    var width, height;

                    if (image.width > image.height){
                        // 横長の画像は横のサイズを指定値にあわせる
                        var ratio = image.height/image.width;

                        width = THUMBNAIL_WIDTH;
                        height = THUMBNAIL_WIDTH * ratio;
                    } else {
                        // 縦長の画像は縦のサイズを指定値にあわせる
                        var ratio = image.width/image.height;
                        
                        width = THUMBNAIL_HEIGHT * ratio;
                        height = THUMBNAIL_HEIGHT;
                    }

                    // canvas のサイズを上で算出した値に変更
                    var canvas = $("#canvas").attr("width", width).attr("height", height).removeClass("hide");
                    var ctx = canvas[0].getContext("2d");

                    // canvas に既に描画されている画像をクリア
                    ctx.clearRect(0, 0, width, height);

                    // EXIF を考慮して回転し、canvas に描画
                    EXIF.getData(objFile, function() {
                        var orientation = EXIF.getTag(this, "Orientation");

                        var isiOSVer13_4 = navigator.userAgent.toLowerCase().indexOf('ios 13.4') !== -1;
                        var isiOSVer13_5 = navigator.userAgent.toLowerCase().indexOf('ios 13.5') !== -1;
                        var isiOSVer13_6 = navigator.userAgent.toLowerCase().indexOf('ios 13.6') !== -1;
                        var isiOSVer13_7 = navigator.userAgent.toLowerCase().indexOf('ios 13.7') !== -1;
                        var isiOSVer14 = navigator.userAgent.toLowerCase().indexOf('ios 14') !== -1;
                        // "iPhone;" もしくは "iPad;" が出現して、かつ "OS 15_" が出現する
                        var isiOSVer15 = ((navigator.userAgent.toLowerCase().indexOf("iphone;") !== -1 || navigator.userAgent.toLowerCase().indexOf("ipad;") !== -1) &&
                                           navigator.userAgent.toLowerCase().indexOf("os 15_") !== -1);
                        // "iPhone;" もしくは "iPad;" が出現して、かつ "OS 16_" が出現する
                        var isiOSVer16 = ((navigator.userAgent.toLowerCase().indexOf("iphone;") !== -1 || navigator.userAgent.toLowerCase().indexOf("ipad;") !== -1) &&
                                           navigator.userAgent.toLowerCase().indexOf("os 16_") !== -1);

                        if (isiOSVer13_4 || isiOSVer13_5 || isiOSVer13_6 || isiOSVer13_7 || isiOSVer14 || isiOSVer15 || isiOSVer16) {
                            ctx.drawImage(image, 0, 0, image.width, image.height, 0, 0, width,height);
    
                        }else {
                            switch(orientation) {
                                case 8:
                                    $("#canvas").attr("width", height).attr("height", width); // 90 度まわすなら逆に
                                    ctx.translate(0 , width); // 90 度まわすなら逆に
                                    ctx.rotate(-90 * Math.PI / 180);
                                    ctx.drawImage(image, 0, 0, image.width, image.height, 0, 0, width,height);

                                    break;

                                case 3:
                                    ctx.translate(width, height);
                                    ctx.rotate(180 * Math.PI / 180);
                                    ctx.drawImage(image, 0, 0, image.width, image.height, 0, 0, width,height);

                                    break;

                                case 6:
                                    $("#canvas").attr("width", height).attr("height", width); // 90 度まわすなら逆に
                                    ctx.translate(height, 0); // 90 度まわすなら逆に
                                    ctx.rotate(90 * Math.PI / 180);
                                    ctx.drawImage(image, 0, 0, image.width, image.height, 0, 0, width,height);

                                    break;

                                default:
                                    ctx.drawImage(image, 0, 0, image.width, image.height, 0, 0, width,height);

                                    break;
                            }
                        }
                        // canvas から Base64 画像データを取得
                        var base64 = canvas.get(0).toDataURL("image/jpeg");
                        
                        $uploadImg.attr("src", base64);
                        $uploadImg.data("file-name", objFile.name);
                        $uploadFile.val("");
                        objFile = null;

                        // サムネイルを表示
                        $thumbnailArea.removeClass("hide");

                        // 画面をアンロック
                        mgf.unlockScreen();
                    });
                };

                // 画像の読み込みに失敗（画像ではない）
                image.onerror = function() {
                    $uploadFile.val("");
                    objFile = null;
                    $uploadAlert.text(message).removeClass("hide");

                    // 画面をアンロック
                    mgf.unlockScreen();
                };

                // ファイルの内容を image へセット
                image.src = e.target.result;
            };

            // ファイルの読み込みに失敗
            reader.onerror = function() {
                $uploadFile.val("");
                objFile = null;
                $uploadAlert.text(message).removeClass("hide");

                // 画面をアンロック
                mgf.unlockScreen();
            };

            // ファイルの読み込み
            reader.readAsDataURL(objFile);
        } else {
            // 画像ではない
            $uploadFile.val("");
            objFile = null;
            $uploadAlert.text(message).removeClass("hide");

            // 画面をアンロック
            mgf.unlockScreen();
        }
    });

    // サムネイルの「×」ボタン
    $("main").on("click", "span.remover", function () {
        // サムネイルをクリアし非表示
        $thumbnailArea.addClass("hide");
        $uploadImg.attr("src", "").data("file-name", "");
        $("input[type=file]").val("");
        objFile = null;
    });

    // 「登録」ボタン
    $("main").on("click", "#addfirstS", function () {
        // 画面をロック
        mgf.lockScreen();

        // エラークリア
        $(".caution").text("").addClass("hide");

        var $editAlert = $("#caution");
        
        var fd = new FormData();
        var date = "";
        var time1 = "";
        var time2 = "";
        var time3 = "";
        var mealType = "";
        var itemName = "不明";
        var calorie = 0;

        // 日付
        date = $("#record-date").val();

        // 時間
        time1 = $("#meridiem").val();
        time2 = $("#hour").val();
        time3 = $("#minute").val();

        // 食事種別
        mealType = $("#mealtype input[type='radio']:checked").val();

        // 品目
        itemName = $("#hinmoku-2").val();

        // カロリー
        calorie = $("#cal-2").val();

        // POST 内容を構築
        fd.append("__RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());
        fd.append("model.RecordDate", date);
        fd.append("model.Meridiem", time1);
        fd.append("model.Hour", time2);
        fd.append("model.Minute", time3);
        fd.append("model.MealType", mealType);
        fd.append("model.ItemName", itemName);
        fd.append("model.Calorie", calorie);
        fd.append("model.PalString", "");
        fd.append("model.PhotoData", $uploadImg.attr("src"));
        fd.append("model.PhotoName", $uploadImg.data("file-name"));
        fd.append("model.IsCheckFutureDate", "True");

        $.ajax({
            type: "POST",
            url: "../Note/MealEditResult?ActionSource=Edit",
            //processData: false,
            //contentType: false,
            //dataType: "json",
            //data: fd,
            traditional: true,
            data: $.toJsonObject(fd),
            async: true,
            beforeSend: function (jqXHR) {
                // セッションのチェック
                mgf.note.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {
                    // 成功

                    // リダイレクト
                    location.href = "/Note/Meal4"
                    return false;
                } else if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0) {
                    // エラー
                    var messages = $.parseJSON(data)["Messages"];
                    var length = messages.length;

                    if (length == 1) {
                        $editAlert.text(messages[0]).removeClass("hide");
                    } else {
                        $editAlert.text("");

                        $.each(messages, function (i, v) {
                            $editAlert.append(v);
                            if (i != length - 1) $editAlert.append("<br />");
                        });

                        $editAlert.removeClass("hide");
                    }

                    // 画面をアンロック
                    mgf.unlockScreen();
                } else {
                    // エラー
                    $editAlert.text("登録に失敗しました。status:" + jqXHR.status).removeClass("hide");

                    // 画面をアンロック
                    mgf.unlockScreen();
                }
            } catch(ex) {
                // エラー
                $editAlert.text("登録に失敗しました。exception:" + ex.message).removeClass("hide");

                // 画面をアンロック
                mgf.unlockScreen();
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            // エラー
            $editAlert.text("登録に失敗しました。status:" + jqXHR.status + " error:" + errorThrown).removeClass("hide");

            // 画面をアンロック
            mgf.unlockScreen();
        });

        return false;
    });

    // 写真が存在する場合はサムネイルを表示する
    if ($uploadImg.attr("src") != "" && $uploadImg.data("file-name") != "") {
        $thumbnailArea.removeClass("hide");
    }

    // 登録ボタンの位置(画面下部)までスクロールする
    function scrolltoBottom() {
        var speed = 400;
        var target = $(".submit-area .btn-submit.upload");
        var position = target.offset().top;
        $('body,html').animate({ scrollTop: position }, speed, 'swing');
        return false;
    };

    // 「品目」「カロリー」が入力されたら「登録」ボタンを有効化
    changeUploadBtnDisabled();

    // 検索から戻ってきた場合
    if ($("#meal").data("search") == "True") {
        // 登録ボタンの位置(画面下部)までスクロールする
        scrolltoBottom();
        $("#meal").attr("data-search", "");
    }

    // 写真を変更ボタン有効化
    $("#sample1").removeAttr("disabled");

})();
