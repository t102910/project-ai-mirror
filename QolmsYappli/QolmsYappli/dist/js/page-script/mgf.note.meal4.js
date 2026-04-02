// mgf.note.meal 名前空間オブジェクト
mgf.note.meal = mgf.note.meal || {};

// DOM 構築完了
mgf.note.meal = (function () {
    // 画像、ファイル処理用
	const THUMBNAIL_WIDTH = 2000; // 画像リサイズ後の横の長さの最大値
	const THUMBNAIL_HEIGHT = 2000; // 画像リサイズ後の縦の長さの最大値
    const CONTENT_TYPES = ["image/jpeg", "image/png", "image/bmp", "image/x-bmp", "image/x-ms-bmp"];

    var $uploadFile = null;
    var $uploadImg = null;
    var $uploadAlert = null;
    var $thumbnailArea = null;
    var objFile = null;

    var defaultMealType = null;

    // 画像の非同期読み込み
    function loadPhoto() {
        $(document).find("#update-data-area img.photo.inview").each(function () {
            $(this).one("inview", { "reference": $(this).data("reference") }, function (event, visible) {
                $.ajax({
                    type: "GET",
                    url: event.data["reference"],
                    async: true,
                    context: event.target
                }).done(function (data, textStatus, jqXHR) {
                    $(this).attr("src", $(this).data("reference"));
                });
            });
        });
    };

    //// 現在の日付をセット
    //function setNowDate() {
    //    // 年月日・時分の取得
    //    var d = new Date();
    //    var year = d.getFullYear();
    //    var month = d.getMonth() + 1;
    //    var day = d.getDate();
    //    var dstr = year + "年" + month + "月" + day + "日"
    //    if (true) {
    
    //    }

    //    $(".target-date-time .picker").datepicker("setDate", dstr);
    //};
    
    // 指定日の日付をセット
    function setNowDate() {
        //// 年月日・時分の取得
        //var d = new Date();
        //var year = d.getFullYear();
        //var month = d.getMonth() + 1;
        //var day = d.getDate();
        //var dstr = year + "年" + month + "月" + day + "日"
        var dstr = $("#record-date").val();
        console.log(dstr);

        $(".target-date-time .picker").datepicker("setDate", dstr);
    };

    // 食事種別の初期化
    function resetMealTypeInputArea() {

        if ($("#mealtype input[type='radio']:checked").length > 0) {

            // 食事種別を指定する
            var $radioButton = $(".target-date-time").find("input[type='radio']")

            var selectedMeal = $("#mealtype input[type='radio']:checked").val()
            console.log(selectedMeal);

            $radioButton.val([selectedMeal])
            
            // デフォルト食事種別を指定する
            if (hour2 >= 5 && hour2 <= 10) {
                defaultMealType ="Breakfast";
            } else if (hour2 >= 11 && hour2 <= 14) {
                defaultMealType ="Lunch";
            } else if (hour2 >= 17 && hour2 <= 23) {
                defaultMealType ="Dinner";
            } else {
                defaultMealType ="Snacking";
            }

            var timeArr = getTime()

        }else{
            // 現在日時をセット
            var d = new Date();
            var hour2 = d.getHours();

            // 食事種別を指定する
            var $radioButton = $(".target-date-time").find("input[type='radio']")
            if (hour2 >= 5 && hour2 <= 10) {
                $radioButton.val(["Breakfast"])
            } else if (hour2 >= 11 && hour2 <= 14) {
                $radioButton.val(["Lunch"])
            } else if (hour2 >= 17 && hour2 <= 23) {
                $radioButton.val(["Dinner"])
            } else {
                $radioButton.val(["Snacking"])
            }
            defaultMealType = $("#mealtype input[type='radio']:checked").val();        
        }

    };

    function lordMealArea(){
       
        $.ajax({
            type: "POST",
            url: "../Note/Meal4Result?ActionSource=Filter",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
            },
            async: true
        }).done(function (data, textStatus, jqXHR) {
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("text/html;") >= 0) {
                    // 成功

                    // ○日の食事 -> 最近の食事
                    $("#filter-title").text("最近の食事")

                    // パーシャル ビューを書き換え
                    $(document).find("#update-data-area").replaceWith($(data));

                    // datepicker 設定
                    $(document).find("#update-data-area .picker").each(function() {
                        $(this).datepicker({
                            format: "yyyy年mm月dd日",
                            weekStart: 1,
                            startDate: '-5y,+1d',
                            startView: 0,
                            language: "ja",
                            todayHighlight: true,
                            orientation: "top auto",
                            autoclose: true
                        }).on('hide', function (e) {
                            $('.picker').blur();
                        });
                    });

                    // 画面をアンロック
                    mgf.unlockScreen();

                    // 画像を非同期でロード
                    loadPhoto();

                    $("a.home-btn").removeClass("disabled")

                } else {
                    // エラー
                    $("a.home-btn").removeClass("disabled")

                }
            } catch (ex) {
                // エラー
                $("a.home-btn").removeClass("disabled")
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            // エラー
            $("a.home-btn").removeClass("disabled")
        });

    };


    // 食事種別毎の時刻を取得(食事種別の変更がない場合は現在時刻)
    function getTime() {

        var mealType = $("#mealtype input[type='radio']:checked").val();
        var time1 = "";
        var time2 = "";
        var time3 = "0";

        // 時間
        if (defaultMealType != mealType && mealType != "Snacking") {
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
            }
        } else {
            // 現在の日時をセット
            var now = new Date();
            time2 = now.getHours();
            time3 = now.getMinutes();

            if (time2 >= 12) {
                time1 = "pm";
                time2 -= 12;
            } else {
                time1 = "am";
            }
        }

        return new Array(time1, time2, time3);
    }

    // 入力欄の初期化
    function resetInputArea() {
        // 現在の日付をセット
        setNowDate();

        // 食事種別の初期化
        resetMealTypeInputArea();

        // 品名テキストボックスをクリア
        $("#searchText").val("");

        // サムネイルをクリア
        $("#thumbnail-edit img").attr("src", "").data("file-name", "");
        $("input[type=file]").val("");
        objFile = null;

        // アラートエリアを隠す 
        $(".caution").text("").addClass("hide");
    };

    // 「写真から登録」ボタン
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
        $uploadAlert = $(".input-area .caution");

        try {
            // 初期化
            var message = "jpg、jpeg、png、bmpファイルを選択してください。";
            var $inputArea = $("#thumbnail-edit");

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
                            //alert(this);
                            //alert(EXIF.getTag(this, "Orientation"));
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

                            image.onload = null;
                            image = null;
                            reader.onload = null;
                            reader = null;

                            // サムネイルを表示
                            $inputArea.fadeIn();

                            // 画面をアンロック
                            mgf.unlockScreen();
                        });
                    };

                    // 画像の読み込みに失敗（画像ではない）
                    image.onerror = function() {
                        $uploadFile.val("");
                        objFile = null;
                        $uploadAlert.text(message).removeClass("hide");

                        image.onerror = null;
                        image = null;

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

                    reader.onerror = null;
                    reader = null;

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
        } catch (ex) {
            // エラー
            objFile = null;
            $uploadAlert.text(ex.message).removeClass("hide");

            // 画面をアンロック
            mgf.unlockScreen();
        }
    });

    // 「品名」が入力されたら「検索」ボタンを有効化
    $("main").on("input keyup blur", "#searchText", function () {
        if ($.trim($("#searchText").val()) == "") {
            $("#search").addClass("disabled");
        } else {
            $("#search").removeClass("disabled");
        }
    });

    // 「検索」ボタン
    $("main").on("click", "#search", function() {
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
        //                value: times[0]
        //            }).appendTo($("form:last"));
                                 
        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "model.Hour",
        //                value: times[1]
        //            }).appendTo($("form:last"));
                                 
        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "model.Minute",
        //                value: times[2]
        //            }).appendTo($("form:last"));

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "PageType",
        //                value: "Search"
        //            }).appendTo($("form:last"));
                    
        //            // サブミット
        //            $("form:last").submit();

        //        } catch (ex) {
        //            console.log(ex);

        //            // 画面をアンロック
        //            mgf.unlockScreen();
        //        }
        //    })

        // 画面をロック
        mgf.lockScreen();

        // POST 内容を構築
        var fd = new FormData();
        var times = getTime();

        fd.append("__RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());
        fd.append("SearchText", $("#searchText").val());
        fd.append("model.RecordDate", $("#record-date").val());
        fd.append("model.MealType", $("#mealtype input[type='radio']:checked").val());
        fd.append("model.Meridiem", times[0]);
        fd.append("model.Hour", times[1]);
        fd.append("model.Minute", times[2]);
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
                    var $alert = $("section.caution");

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
                    $("section.caution").text("失敗しました。status:" + jqXHR.status).removeClass("hide");

                    // 画面をアンロック
                    mgf.unlockScreen();
                }
            } catch (ex) {
                //console.log(ex);
                // エラー
                $("section.caution").text("失敗しました。exception:" + ex.message).removeClass("hide");

                // 画面をアンロック
                mgf.unlockScreen();
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            // エラー
            $("section.caution").text("失敗しました。status:" + jqXHR.status + " error:" + errorThrown).removeClass("hide");

            // 画面をアンロック
            mgf.unlockScreen();
        });

        return false;
    });

    // 「履歴から登録」ボタン
    $("main").on("click", "#history", function() {
        var times = getTime();

        //// 画面をロック後、サブミット
        //mgf.note.promiseLockScreen($(this))
        //    .then(function () {
        //        try {
        //            $("<form>", {
        //                action: "../Note/Meal4Result?ActionSource=History",
        //                method: "POST"
        //            }).appendTo(document.body);

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "__RequestVerificationToken",
        //                value: $("input[name='__RequestVerificationToken']").val()
        //            }).appendTo($("form:last"));

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "recordDate",
        //                value: $("#record-date").val()
        //            }).appendTo($("form:last"));

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "mealType",
        //                value: $("#mealtype input[type='radio']:checked").val()
        //            }).appendTo($("form:last"));

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "meridiem",
        //                value: times[0]
        //            }).appendTo($("form:last"));
                                 
        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "hour",
        //                value: times[1]
        //            }).appendTo($("form:last"));
                                 
        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "minute",
        //                value: times[2]
        //            }).appendTo($("form:last"));

        //            // サブミット
        //            $("form:last").submit();

        //        } catch (ex) {
        //            console.log(ex);

        //            // 画面をアンロック
        //            mgf.unlockScreen();
        //        }
        //    });

        // 画面をロック
        mgf.lockScreen();

        // POST 内容を構築
        var fd = new FormData();
        var times = getTime();

        fd.append("__RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());
        fd.append("recordDate", $("#record-date").val());
        fd.append("mealType", $("#mealtype input[type='radio']:checked").val());
        fd.append("meridiem", times[0]);
        fd.append("hour", times[1]);
        fd.append("minute", times[2]);

        // 登録内容を POST
        $.ajax({
            type: "POST",
            url: "../Note/Meal4Result?ActionSource=History",
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
                    location.href = "../Note/MealRegisterFromHistory";

                    return false;

                } else if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0) {
                    // エラー
                    var messages = $.parseJSON(data)["Messages"];
                    var length = messages.length;
                    var $alert = $("section.caution");

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
                    $("section.caution").text("失敗しました。status:" + jqXHR.status).removeClass("hide");

                    // 画面をアンロック
                    mgf.unlockScreen();
                }
            } catch (ex) {
                //console.log(ex);
                // エラー
                $("section.caution").text("失敗しました。exception:" + ex.message).removeClass("hide");

                // 画面をアンロック
                mgf.unlockScreen();
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            // エラー
            $("section.caution").text("失敗しました。status:" + jqXHR.status + " error:" + errorThrown).removeClass("hide");

            // 画面をアンロック
            mgf.unlockScreen();
        });

        return false;
    });

    // 「絞り込み」領域の表示切り替え
    $("main").on("click", ".calendar-view", function () {
        if ($(this).hasClass("on")) {
            // 非表示
            $(this).removeClass("on").html("<i class='la la-calendar la-2x'></i>絞り込み");
            $("#calender-area").slideUp();

            if ($("#filter-title").text() !== "最近の食事") {
                // 画面をロック
                mgf.lockScreen();

                $.ajax({
                    type: "POST",
                    url: "../Note/Meal4Result?ActionSource=Filter",
                    data: {
                        __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
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

                            // ○日の食事 -> 最近の食事
                            $("#filter-title").text("最近の食事")

                            // パーシャル ビューを書き換え
                            $(document).find("#update-data-area").replaceWith($(data));

                            // datepicker 設定
                            $(document).find("#update-data-area .picker").each(function() {
                                $(this).datepicker({
                                    format: "yyyy年mm月dd日",
                                    weekStart: 1,
                                    startDate: '-5y,+1d',
                                    startView: 0,
                                    language: "ja",
                                    todayHighlight: true,
                                    orientation: "top auto",
                                    autoclose: true
                                }).on('hide', function (e) {
                                    $('.picker').blur();
                                });
                            });

                            // 画面をアンロック
                            mgf.unlockScreen();

                            // 画像を非同期でロード
                            loadPhoto();
                        } else {
                            // エラー

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
            }
        } else {
            // 表示
            // 日付 現在の時刻
            var now = new Date();
            var filterDate = now.getFullYear()
                + "年"
                + (now.getMonth() + 1)
                + "月"
                + now.getDate()
                + "日";

            $("#filter-date").datepicker("setDate", filterDate);
            $(this).addClass("on").html("<i class='la la-calendar la-2x'></i>絞り込み");
            $("#calender-area").slideDown();
        }
    })

    // 「絞り込み」ボタン
    $("main").on("click", "#filter-submit", function () {
        // 画面をロック
        mgf.lockScreen();

        // 日付
        var filter = $("#filter-date").val();

        $.ajax({
            type: "POST",
            url: "../Note/Meal4Result?ActionSource=Filter",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                FilterDate: filter
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

                    // 最近の食事 -> ○日の食事
                    $("#filter-title").text(filter.slice(-6) + "の食事")

                    // 「絞り込み」領域の表示
                    $(".calendar-view").addClass("on").html("<i class='la la-calendar la-2x'></i>絞り込み解除");
                    $("#calender-area").slideDown();

                    // パーシャル ビューを書き換え
                    $(document).find("#update-data-area").replaceWith($(data));

                    // datepicker 設定
                    $(document).find("#update-data-area .picker").each(function() {
                        $(this).datepicker({
                            format: "yyyy年mm月dd日",
                            weekStart: 1,
                            startDate: '-5y,+1d',
                            startView: 0,
                            language: "ja",
                            todayHighlight: true,
                            orientation: "top auto",
                            autoclose: true
                        }).on('hide', function (e) {
                            $('.picker').blur();
                        });
                    });

                    // 画面をアンロック
                    mgf.unlockScreen();

                    // 画像を非同期でロード
                    loadPhoto();
                } else {
                    // エラー

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
    })

    // 「削除確認」ダイアログの表示
    $("main").on("click", "a.remove", function () {
        // 表示
        $("#delete-modal").modal("show", {
            recordDate: $(this).data("recorddate"),
            mealType: $(this).data("mealtype"),
            seq: $(this).data("seq")
        });
    });

    // 「削除確認」ダイアログの初期化
    $("main").on("show.bs.modal", "#delete-modal", function (e) {
        // 初期化
        if (e.target.id == "delete-modal") {
            try {
                $(this).data("record-date", e.relatedTarget.recordDate);
                $(this).data("meal-type", e.relatedTarget.mealType);
                $(this).data("seq", e.relatedTarget.seq);
            } catch (ex) { }
        }
    });

    // 「削除確認」ダイアログの「はい」ボタン
    $("main").on("click", "#delete-button", function () {
        // 画面をロック
        mgf.lockScreen();

        var recordDate = $("#delete-modal").data("record-date");
        var mealType = $("#delete-modal").data("meal-type");
        var sequence = $("#delete-modal").data("seq");

        $.ajax({
            type: "POST",
            url: "../Note/Meal4Result?ActionSource=Delete",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                RecordDate: recordDate,
                MealType: mealType,
                Sequence: sequence
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

                    // 「削除確認」ダイアログの非表示
                    $("body").removeClass("modal-open");
                    $(".modal-backdrop").remove();
                    $("#delete-modal").modal("hide");

                    // 「絞り込み」領域の非表示
                    if ($(".calendar-view").hasClass("on")) {
                        $(".calendar-view").removeClass("on").html("<i class='la la-calendar la-2x'></i>絞り込み");
                        $("#calender-area").slideUp();
                        $("#filter-title").text("最近の食事")
                    }

                    // パーシャル ビューを書き換え
                    $(document).find("#update-data-area").replaceWith($(data));

                    // datepicker 設定
                    $(document).find("#update-data-area .picker").each(function() {
                        $(this).datepicker({
                            format: "yyyy年mm月dd日",
                            weekStart: 1,
                            startDate: '-5y,+1d',
                            startView: 0,
                            language: "ja",
                            todayHighlight: true,
                            orientation: "top auto",
                            autoclose: true
                        }).on('hide', function (e) {
                            $('.picker').blur();
                        });
                    });

                    // 画面をアンロック
                    mgf.unlockScreen();

                    // 画像を非同期でロード
                    loadPhoto();
                } else {
                    // エラー

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

    // 「編集」アイコンクリック
    $("main").on("click", ".edit", function () {
        //// 画面をロック後、サブミット
        //mgf.note.promiseLockScreen($(this))
        //    .then(function () {
        //        try {
        //            $("<form>", {
        //                action: "../Note/Meal4Result?ActionSource=Edit",
        //                method: "POST"
        //            }).appendTo(document.body);

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "__RequestVerificationToken",
        //                value: $("input[name='__RequestVerificationToken']").val()
        //            }).appendTo($("form:last"));

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "model.RecordDate",
        //                value: $(this).data("recorddate")
        //            }).appendTo($("form:last"));

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "model.MealType",
        //                value: $(this).data("mealtype")
        //            }).appendTo($("form:last"));

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "model.Meridiem",
        //                value: $(this).data("meridiem")
        //            }).appendTo($("form:last"));

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "model.Hour",
        //                value: $(this).data("hour")
        //            }).appendTo($("form:last"));

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "model.Minute",
        //                value: $(this).data("minute")
        //            }).appendTo($("form:last"));

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "model.ItemName",
        //                value: $(this).data("name")
        //            }).appendTo($("form:last"));

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "model.Calorie",
        //                value: $(this).data("cal")
        //            }).appendTo($("form:last"));
                    
        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "model.ForeignKey",
        //                value: $(this).data("photokey")
        //            }).appendTo($("form:last"));

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "model.PalString",
        //                value: $(this).data("pal")
        //            }).appendTo($("form:last"));

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "model.BeforeRecordDate",
        //                value: $(this).data("recorddate")
        //            }).appendTo($("form:last"));

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "model.BeforeMealType",
        //                value: $(this).data("mealtype")
        //            }).appendTo($("form:last"));

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "model.Sequence",
        //                value: $(this).data("seq")
        //            }).appendTo($("form:last"));

        //            // サブミット
        //            $("form:last").submit();

        //        } catch (ex) {
        //            console.log(ex);

        //            // 画面をアンロック
        //            mgf.unlockScreen();
        //        }
        //    });

        // 画面をロック
        mgf.lockScreen();

        // POST 内容を構築
        var fd = new FormData();
        var times = getTime();

        fd.append("__RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());
        fd.append("model.RecordDate", $(this).data("recorddate"));
        fd.append("model.MealType", $(this).data("mealtype"));
        fd.append("model.Meridiem", $(this).data("meridiem"));
        fd.append("model.Hour", $(this).data("hour"));
        fd.append("model.Minute", $(this).data("minute"));
        fd.append("model.ItemName", $(this).data("name"));
        fd.append("model.Calorie", $(this).data("cal"));
        fd.append("model.ForeignKey", $(this).data("photokey"));
        fd.append("model.PalString", $(this).data("pal"));
        fd.append("model.BeforeRecordDate", $(this).data("recorddate"));
        fd.append("model.BeforeMealType", $(this).data("mealtype"));
        fd.append("model.Sequence", $(this).data("seq"));

        // 登録内容を POST
        $.ajax({
            type: "POST",
            url: "../Note/Meal4Result?ActionSource=Edit",
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
                    location.href = "../Note/MealEdit";

                    return false;

                } else if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0) {
                    // エラー
                    var messages = $.parseJSON(data)["Messages"];
                    var length = messages.length;
                    var $alert = $("section.caution");

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
                    $("section.caution").text("失敗しました。status:" + jqXHR.status).removeClass("hide");

                    // 画面をアンロック
                    mgf.unlockScreen();
                }
            } catch (ex) {
                //console.log(ex);
                // エラー
                $("section.caution").text("失敗しました。exception:" + ex.message).removeClass("hide");

                // 画面をアンロック
                mgf.unlockScreen();
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            // エラー
            $("section.caution").text("失敗しました。status:" + jqXHR.status + " error:" + errorThrown).removeClass("hide");

            // 画面をアンロック
            mgf.unlockScreen();
        });

        return false;
    });

    // 「写真を撮りなおす」ボタン
    $("main").on("click", "#thumbnail-edit-close", function () {
        // サムネイルをクリア
        $("#thumbnail-edit img").attr("src", "").data("file-name", "");
        $("input[type=file]").val("");
        objFile = null;

        // サムネイルを非表示
        $("#thumbnail-edit").fadeOut();
    });

    // 「この写真を解析」ボタン
    $("main").on("click", "#analysis", function () {
        // 画面をロック
        mgf.lockScreen();
        
        var fd = new FormData();
        var date = "";
        var times = null;
        var mealType = "";
        var mealTypeText = "";
        var itemName = "不明";
        var calorie = 0;

        // 日付
        date = $(".target-date-time .form-control:input[type='text'].picker").val();

        // 時間
        times = getTime();

        var $radio = $(".target-date-time input[type='radio']:checked");
        mealType = $radio.val();
        mealTypeText = $radio.next().text();

        //// POST 内容を構築
        //// 画面をロック後、サブミット
        //mgf.note.promiseLockScreen($(this))
        //    .then(function () {
        //        //
        //        try {
        //            $("<form>", {
        //                action: "../Note/Meal4Result?ActionSource=Analysis",
        //                method: "POST"
        //            }).appendTo(document.body);

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "__RequestVerificationToken",
        //                value: $("input[name='__RequestVerificationToken']").val()
        //            }).appendTo($("form:last"));
             
        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "model.RecordDate",
        //                value: date
        //            }).appendTo($("form:last"));
          
        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "model.Meridiem",
        //                value: times[0]
        //            }).appendTo($("form:last"));
          
        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "model.Hour",
        //                value: times[1]
        //            }).appendTo($("form:last"));

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "model.Minute",
        //                value: times[2]
        //            }).appendTo($("form:last"));

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "model.MealType",
        //                value: mealType
        //            }).appendTo($("form:last"));

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "model.ItemName",
        //                value: itemName
        //            }).appendTo($("form:last"));

        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "model.Calorie",
        //                value: calorie
        //            }).appendTo($("form:last"));
                    
        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "model.PalString",
        //                value: ""
        //            }).appendTo($("form:last"));
                    
        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "PhotoData",
        //                value: $("#thumbnail-edit img").attr("src")
        //            }).appendTo($("form:last"));
                    
        //            $("<input>").attr({
        //                type: "hidden",
        //                name: "PhotoName",
        //                value: $("#thumbnail-edit img").data("file-name")
        //            }).appendTo($("form:last"));
                    
        //            // サブミット
        //            $("form:last").submit();

        //        } catch (ex) {
        //            console.log(ex);
        //            // 画面をアンロック
        //            mgf.unlockScreen();
        //        }
        //    })

        // 画面をロック
        mgf.lockScreen();

        // POST 内容を構築
        fd.append("__RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());
        fd.append("model.RecordDate", date);
        fd.append("model.Meridiem", times[0]);
        fd.append("model.Hour", times[1]);
        fd.append("model.Minute", times[2]);
        fd.append("model.MealType", mealType);
        fd.append("model.ItemName", itemName);
        fd.append("model.Calorie", calorie);
        fd.append("model.PalString", "");
        fd.append("PhotoData", $("#thumbnail-edit img").attr("src"));
        fd.append("PhotoName", $("#thumbnail-edit img").data("file-name"));

        // 登録内容を POST
        $.ajax({
            type: "POST",
            url: "../Note/Meal4Result?ActionSource=Analysis",
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
                    location.href = "../Note/MealRegisterFromPhoto";

                    return false;

                } else if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0) {
                    // エラー
                    var messages = $.parseJSON(data)["Messages"];
                    var length = messages.length;
                    var $alert = $("section.caution");

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
                    $("section.caution").text("失敗しました。status:" + jqXHR.status).removeClass("hide");

                    // 画面をアンロック
                    mgf.unlockScreen();
                }
            } catch (ex) {
                //console.log(ex);
                // エラー
                $("section.caution").text("失敗しました。exception:" + ex.message).removeClass("hide");

                // 画面をアンロック
                mgf.unlockScreen();
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            // エラー
            $("section.caution").text("失敗しました。status:" + jqXHR.status + " error:" + errorThrown).removeClass("hide");

            // 画面をアンロック
            mgf.unlockScreen();
        });

        return false;
    });

    
    $("main").on("click", "a.home-btn", function () {

        if ($(this).hasClass("disabled")) {
            return false;
    
        }
        $("#white-out .loader").css("background-image", "url(../dist/img/tmpl/loader.gif)");
        // 画面をロック
        mgf.note.promiseLockScreen($(this))
         .then(function () {
             location.href = "../Portal/Home"
         });
        return false;

    })
    
    // ヒストリー バックの禁止
    mgf.prohibitHistoryBack();

    // 入力欄の初期化
    resetInputArea();

    // セッションのチェック
    mgf.note.checkSessionByAjax();

    //食事領域の非同期ロード
    lordMealArea();

    // 画像の非同期読み込み
    //loadPhoto();

    // 写真から登録ボタン有効化
    $("#sample1").removeAttr("disabled");

})();
