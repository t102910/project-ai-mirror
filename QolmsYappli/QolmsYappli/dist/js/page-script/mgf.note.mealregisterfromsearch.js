// mgf.note.mealregisterfromsearch 名前空間オブジェクト
mgf.note.mealregisterfromsearch = mgf.note.mealregisterfromsearch || {};

// DOM 構築完了
mgf.note.mealregisterfromsearch = (function () {
    // 画像、ファイル処理用
	const THUMBNAIL_WIDTH = 2000; // 画像リサイズ後の横の長さの最大値
	const THUMBNAIL_HEIGHT = 2000; // 画像リサイズ後の縦の長さの最大値
    const CONTENT_TYPES = ["image/jpeg", "image/png", "image/bmp", "image/x-bmp", "image/x-ms-bmp"];

    var $uploadFile = null;
    var $uploadImg = null;
    var $uploadAlert = null;
    var $thumbnailArea = null;
    var objFile = null;

    // 検索エリアを非表示にする
    $("#search").closest(".flex-wrap").addClass("hide");

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
        //                name: "model.InputNext",
        //                value: "next"
        //            }).appendTo($("form:last"));

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "SearchText",
        //                value: $("input[name='searchText']").val()
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
        //                name: "PageType",
        //                value: "Search"
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
        fd.append("model.InputNext", "next");
        fd.append("SearchText", $("input[name='searchText']").val());
        fd.append("model.RecordDate", $("#record-date").val());
        fd.append("model.MealType", $("#mealtype input[type='radio']:checked").val());
        fd.append("model.Meridiem", time1);
        fd.append("model.Hour", time2);
        fd.append("model.Minute", time3);
        fd.append("model.ItemName", "不明");
        fd.append("model.Calorie", "0");
        fd.append("PageType", "Search");

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

    // 「登録」ボタン
    $("main").on("click", ".submit-area .btn-submit.upload", function () {
        // 画面をロック
        mgf.lockScreen();

        var fd = new FormData();
        var now = new Date();
        var date = "";
        var time1 = "";
        var time2 = "";
        var time3 = "";
        var mealType = "";
        var mealTypeText = "";
        var itemName = "不明";
        var calorie = 0;
        var palStr = "";
        var url = "";
        var count = 0;

        itemName = $("#hinmoku-2").val();
        calorie = $("#cal-2").val();
        palStr = $("#hinmoku-2").data("pal");

        if ($(this).attr("id") != "addnext") {
            // 新規登録

            // 日付
            date = $(".target-date-time .form-control:input[type='text'].picker").val();

            // 時間
            var $select = $(".target-date-time .datetime-select .form-control:input");
            time1 = $select.eq(0).val();
            time2 = $select.eq(1).val();
            time3 = $select.eq(2).val();

            // 食事種別
            var $radio = $(".target-date-time input[type='radio']:checked");
            mealType = $radio.val();
            mealTypeText = $radio.next().text();

            url = "../Note/MealRegisterFromSearchResult?ActionSource=Add";
        } else {
            // 続けて登録
            date = $(this).data("recorddate");
            time1 = $(this).data("meridiem");
            time2 = $(this).data("hour");
            time3 = $(this).data("minute");
            mealType = $(this).data("mealtype");
            mealTypeText = $(this).data("mealtypetext");

            url = "../Note/MealRegisterFromSearchResult?ActionSource=AddNext";
        }

        // POST 内容を構築
        fd.append("__RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());
        fd.append("model.RecordDate", date);
        fd.append("model.Meridiem", time1);
        fd.append("model.Hour", time2);
        fd.append("model.Minute", time3);
        fd.append("model.MealType", mealType);
        fd.append("model.ItemName", itemName);
        fd.append("model.Calorie", calorie);
        fd.append("model.PalString", palStr);
        fd.append("model.IsCheckFutureDate", "True");
        fd.append("PhotoData", $(".meal-photo img").attr("src"));
        fd.append("PhotoName", $(".meal-photo img").data("file-name"));

        // 登録内容を POST
        $.ajax({
            type: "POST",
            url: url,
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

                    // 品目、カロリーをクリア
                    $("#hinmoku-2").val("");
                    $("#cal-2").val("");

                    if ($(this).attr("id") != "addnext") {
                        // 新規登録 -> 続けて登録
                        settingAddNext(date, time1, time2, time3, mealType, mealTypeText);
                    }

                    // 検索を表示
                    $("#searchText").val("");
                    $("#search").addClass("disabled");
                    $(".flex-wrap").removeClass("hide");

                    // 品目、カロリーをクリア
                    $("#hinmoku-2").parent().addClass("hide");
                    $("#cal-2").parent().addClass("hide");
                    $("#hinmoku-2").val("");
                    $("#hinmoku-2").data("pal", "");
                    $("#cal-2").val("");

                    // 「ファイルを選択」を非表示
                    $(".img-upload").parent().addClass("hide");
                    $("input[type=file]").val("");

                    //　サムネイルをクリア
                    $(".thumbnail-area").addClass("hide");
                    $(".thumbnail-area img").attr("src", "").data("file-name", "");

                    // エラー表示をクリア
                    $(".caution").text("").addClass("hide");

                    // 画面をアンロック
                    mgf.unlockScreen();
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
                    $("#caution").text("登録に失敗しました。status:" + jqXHR.status).removeClass("hide");

                    // 画面をアンロック
                    mgf.unlockScreen();
                }
            } catch (ex) {
                // エラー
                $("#caution").text("登録に失敗しました。exception:" + ex.message).removeClass("hide");

                // 画面をアンロック
                mgf.unlockScreen();
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            // エラー
            $("#caution").text("登録に失敗しました。status:" + jqXHR.status + " error:" + errorThrown).removeClass("hide");

            // 画面をアンロック
            mgf.unlockScreen();
        });

        return false;
    });

    // 「写真を追加」ボタン
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
        var $inputArea = $(".input-area-inner");

        $uploadAlert = $("#photo-caution");
        $uploadFile = $(this);
        $uploadImg = $inputArea.find("img");
        $thumbnailArea = $inputArea.find(".thumbnail-area");

        // エラー表示をクリア
        $uploadAlert.text("").addClass("hide");

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
        var $inputArea = $(this).closest(".input-area-inner");

        // サムネイルをクリアし非表示
        $inputArea.find("img").attr("src", "").data("file-name", "");
        $inputArea.find(".thumbnail-area").addClass("hide");
        $(".img-upload").find("input[type=file]").val("");
        objFile = null;
    });

    // 画面を続けて登録の状態に設定する
    function settingAddNext(date, time1, time2, time3, mealType, mealTypeText) {
        // 新規登録 -> 続けて登録

        // 日付入力エリアを隠す
        $(".target-date-time").addClass("hide");

        $("#record-date").datepicker("setDate", date);
        $(".target-date-time .datetime-select [name=meridiem]").val(time1);
        $(".target-date-time .datetime-select [name=hour]").val(time2);
        $(".target-date-time .datetime-select [name=minute]").val(time3);

        // 文字エリア入れ替え用文字列             
        var htmlStr = "";

        htmlStr = "<p class='mb5 input-next'><strong><em>" + $("#record-date").val().slice(-6);
        htmlStr = htmlStr + "</em>（<em>";
        htmlStr = htmlStr + $(".target-date-time .datetime-select [name=meridiem] option:selected").text();
        htmlStr = htmlStr + $(".target-date-time .datetime-select [name=hour] option:selected").text() + "時";
        htmlStr = htmlStr + $(".target-date-time .datetime-select [name=minute] option:selected").text() + "分";
        htmlStr = htmlStr + "</em>）の<em>";
        htmlStr = htmlStr + mealTypeText;
        htmlStr = htmlStr + "</em></strong>を<span class='inline-block'>続けて登録する</span></p></div>";

        // 文字エリアを入れ替えて表示
        $(".input-area-inner div.input-next").find("p.input-next").replaceWith(htmlStr);
        $(".input-next").removeClass("hide");

        // 「戻る」「登録」ボタン非表示
        $("#back").addClass("hide");
        $("#addfirstS").addClass("hide");

        $("#addnext").data("recorddate", date);
        $("#addnext").data("mealtype", mealType);
        $("#addnext").data("mealtypetext", mealTypeText);
        $("#addnext").data("meridiem", time1);
        $("#addnext").data("hour", time2);
        $("#addnext").data("minute", time3);

        //「(続けて)登録」「登録完了」ボタン表示
        $("#addnext").addClass("disabled").removeClass("hide");
        $("#finish").removeClass("hide");
    };

    // 続けて登録
    if ($("#meal").data("next") == "next") {
        // 日付
        var date = $(".target-date-time .picker").val();

        // 時間
        var $select = $(".target-date-time .datetime-select .form-control:input");
        var time1 = $select.eq(0).val();
        var time2 = $select.eq(1).val();
        var time3 = $select.eq(2).val();

        // 食事種別
        var $radio = $(".target-date-time input[type='radio']:checked");
        var mealType = $radio.val();
        var mealTypeText = $radio.next().text();

        // 画面を続けて登録の状態に設定する
        settingAddNext(date, time1, time2, time3, mealType, mealTypeText);
    }

    // 「品目」「カロリー」が入力されたら「登録」ボタンを有効化
    changeUploadBtnDisabled();

    // 写真を追加ボタン有効化
    $("#sample1").removeAttr("disabled");

})();
