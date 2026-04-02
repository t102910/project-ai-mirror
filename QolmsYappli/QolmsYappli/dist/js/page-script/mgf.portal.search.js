// mgf.portal 名前空間オブジェクト
mgf.portal.search = mgf.portal.search || {};
// DOM 構築完了
mgf.portal.search = (function () {
 
    var searchText
    var searchDepartment
    var searchArea
    var searchCity
    var typesum
    var openFlag
    function paging(index) {
        // 画面をロック
        mgf.lockScreen();
        $.ajax({
            type: "POST",
            url: "../Portal/SearchResult?ActionSource=Search",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                SearchText: searchText,
                SearchDepartment: searchDepartment,
                SearchCity: searchCity,
                Index: index,
                OptionFlags: typesum,
                OpenFlag: openFlag
            },
            async: true,
            beforeSend: function (jqXHR) {
                // セッションのチェック
                mgf.portal.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("text/html;") >= 0) {

                    //パーシャルビューを書き換え
                    $(document).find("div.reload-area").replaceWith($(data));

                } else if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0) {

                    // エラー
                    $('#alertarea').removeClass('hide').empty().each(function () {
                        var $div = $(this);

                        $.each($.parseJSON(data)["Messages"], function () {
                            $div.append($("<p>" + this + "</p>"));
                        });
                    });
                }
            } catch (ex) { }
        }).always(function (jqXHR, textStatus) {
            // 画面をアンロック
            mgf.unlockScreen();
        });
        return false;
    }

    function PostpayRequest() {
        mgf.lockScreen();
        $.ajax({
            type: "POST",
            url: "../Portal/SearchResult?ActionSource=Request",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                CodeNo:$("#postpayRequest").data("codeno"),
                RequestFlag:true
            },
            async: true,
            beforeSend: function (jqXHR) {
                // セッションのチェック
                mgf.portal.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0 && $.isTrueString($.parseJSON(data)["IsSuccess"])) {
                    if ( $.isTrueString($.parseJSON(data)["RequestFlag"])) {
                        //disabled
                        $("#postpayRequest").addClass("disabled")
                    }
                } 
            } catch (ex) { }
        }).always(function (jqXHR, textStatus) {
            // 画面をアンロック
            mgf.unlockScreen();
        });
        return false;
    }

    //「検索」ボタン
    $('main').on("click", "div.submit-area a.btn-submit", function () {
        searchText = $("input[name='searchText']").val();
        searchDepartment = "";
        searchArea = ""
        searchCity = "";
        if ($("body").hasClass("input-all")) {
            if ($("#searchDepartment").val() != "") {
                searchDepartment = $("#searchDepartment").val();
            }

            if ($("#searchArea").val() != "") {
                searchArea = $("#searchArea").val();
                searchCity = $("#searchCity-" + searchArea).val();
            }
            openFlag = $("#open").prop("checked");
           
            typesum = 0

            $(".checkbox-area input[type='checkbox']").each(function () {
                //チェックされていれば、data-type要素を加算
                if ($(this).prop("checked") && $(this).data("type") !== undefined) {
                    typesum = typesum + parseInt($(this).data("type"));
                }
            });


        }
        else {
            //詳細検索は対象ではないのでクリアする
            $("#searchDepartment").val("");
            $("#searchArea").val("");
            for (var i = 1; i <= 5; i++) {
                $("#searchCity-" + i).prop("disabled", true);
                $("#searchCity-" + i).addClass("hide");
                $("#searchCity-" + i).prop("selectedIndex", 0);
            }
            $("#searchCity-").prop("disabled", true);
            $("#searchCity-1").removeClass("hide");

        }
        paging(0);
        return false;
    });

   
    // 検索文字列が入力されたら検索ボタン有効化
    $('main').on("input keyup blur", "#searchText", function (ev) {

        if ($.trim($(this).val()) == "" && !$("body").hasClass("input-all")) {
            $("div.submit-area a.btn-submit").addClass("disabled");
        } else {
            $("div.submit-area a.btn-submit").removeClass("disabled");
        }
    });

    // 詳細検索欄を表示・非表示
    $("main").on("click", "#input-all-2", function (ev) {
        if ($("body").hasClass("input-all")) {
            //詳細検索を非表示
            $(this).empty().append("<i class='la la-search'></i>詳細検索");
            $("body").removeClass("input-all");
            if ($("input[name='searchText']").val() =="") {
                $("div.submit-area a.btn-submit").addClass("disabled");
            }
        } else {
            //詳細検索を表示
            $(this).empty().append("<i class='la la-search'></i>詳細検索を閉じる");
            $("body").addClass("input-all");
            //詳細検索を表示しているときは検索ボタンを有効化
            $("div.submit-area a.btn-submit").removeClass("disabled");
        }
    });

    //「市区町村」は消す
    $("#searchCity").addClass("hide");

    //「エリア」選択
    $('main').on("change", "#searchArea", function () {
        var selectArea = "";
        if ($("#searchArea option:selected").val() == "") {
            //「市区町村」は消す
            $("#searchCity").addClass("hide");
        }
        else {
            selectArea = $("#searchArea option:selected").val();
            $("#searchCity").removeClass("hide");
            for (var i = 1; i <= 5; i++) {
                $("#searchCity-" + i).prop("disabled", true);
                $("#searchCity-" + i).addClass("hide");
            }
            $("#searchCity-" + selectArea).prop("disabled", false);
            $("#searchCity-" + selectArea).removeClass("hide");
        }
        return false;
    });
    //ページ切り替え
    $('main').on("click", "li.page-item a.page-link", function () {
        if ($(this).hasClass("disabled")) {
            return false;
        }
        var index = $(this).data("page-index");
        if (index !== undefined) {
            paging(index);
            $("input[name='searchText']").val(searchText);
            $("div.submit-area a.btn-submit").removeClass("disabled");
            if (searchDepartment != "") {
                $("#searchDepartment").val(searchDepartment);
            }
            else {
                $("#searchDepartment").val("");
            }

            if (searchArea == "") {
                //「市区町村」は消す
                $("#searchArea").val("");
                $("#searchCity").addClass("hide");
            }
            else {
                $("#searchArea").val(searchArea);
                $("#searchCity").removeClass("hide");
                for (var i = 1; i <= 5; i++) {
                    $("#searchCity-" + i).prop("disabled", true);
                    $("#searchCity-" + i).addClass("hide");
                }
                $("#searchCity-" + searchArea).prop("disabled", false);
                $("#searchCity-" + searchArea).removeClass("hide");

                if (searchCity != "") {
                    $("#searchCity-" + searchArea).val(searchCity)
                }
            }

            if (!$("body").hasClass("input-all") && (searchDepartment != "" || searchArea != "" || searchCity != "")) {
                //詳細検索を表示
                $(this).empty().append("<i class='la la-search'></i>詳細検索を閉じる");
                $("body").addClass("input-all");
            }
        }
        return false;
    });


    // 詳細表示ページへ
    $('main').on("click", "div.reload-area a.article", function () {
        // 画面をロック
        mgf.lockScreen();
        $.ajax({
            type: "POST",
            url: "../Portal/SearchResult?ActionSource=Detail",
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                SearchText: $("input[name='searchText']").val(),
                CodeNo: $(this).data("codeno")
            },
            async: true,
            beforeSend: function (jqXHR) {
                // セッションのチェック
                mgf.portal.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {
            try {
                if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("text/html;") >= 0) {

                    //パーシャルビューを書き換え
                    $('.modal-content').replaceWith($(data));
                    var lat = parseFloat($("#map").data("latitude"));
                    var lng = parseFloat($("#map").data("longitude"));
                    initMap(lat, lng);
                    $("#map").css('width', '99%');
                    $('#detail-modal').modal('show');

                    //console.log($("#postpayRequest"));

                    $('div.modal').off("click", "#postpayRequest");
                    $('div.modal').on("click", "#postpayRequest", function () {
                        PostpayRequest();
                    });
                } else if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0) {

                    // エラー
                    $('#alertarea').removeClass('hide').empty().each(function () {
                        var $div = $(this);

                        $.each($.parseJSON(data)["Messages"], function () {
                            $div.append($("<p>" + this + "</p>"));
                        });
                    });
                }
            } catch (ex) { }
        }).always(function (jqXHR, textStatus) {
            // 画面をアンロック
            mgf.unlockScreen();
        });
        return false;
    });
    //詳細画面GoogleMapが初期表示できないのでリサイズで再描画
    $("#detail-modal").on("shown.bs.modal", function () {
        //console.log("shown start");
        $("#map").css('width', '100%');
        //console.log("Shown End");
    });

    //Geolocation APIに対応している
    if (navigator.geolocation) {

        // 現在地を取得
        navigator.geolocation.getCurrentPosition(
            function (position) {
                // 画面をロック
                mgf.lockScreen();
                // 取得したデータ
                var data = position.coords;
                var lat = data.latitude;
                var lng = data.longitude;
                //console.log("GPS :" + lat + "," + lng);

                $.ajax({
                    type: "POST",
                    url: "../Portal/SearchResult?ActionSource=Search",
                    data: {
                        __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                        SearchText: "",
                        searchDepartment: "",
                        Latitude: lat,
                        Longitude: lng,
                    },
                    async: true,
                    beforeSend: function (jqXHR) {
                        // セッションのチェック
                        mgf.portal.checkSessionByAjax();
                    }
                }).done(function (data, textStatus, jqXHR) {
                    try {
                        if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("text/html;") >= 0) {

                            //パーシャルビューを書き換え
                            $(document).find("div.reload-area").replaceWith($(data));

                        } else if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0) {

                            // エラー
                            $('#alertarea').removeClass('hide').empty().each(function () {
                                var $div = $(this);

                                $.each($.parseJSON(data)["Messages"], function () {
                                    $div.append($("<p>" + this + "</p>"));
                                });
                            });
                            // 画面をアンロック
                            mgf.unlockScreen();
                        }
                    } catch (ex) { }
                }).always(function (jqXHR, textStatus) {
                    // 画面をアンロック
                    mgf.unlockScreen();
                });
            },

            // [第2引数] 取得に失敗した場合の関数
            function (error) {
                // エラーコード(error.code)の番号
                // 0:UNKNOWN_ERROR				原因不明のエラー
                // 1:PERMISSION_DENIED			利用者が位置情報の取得を許可しなかった
                // 2:POSITION_UNAVAILABLE		電波状況などで位置情報が取得できなかった
                // 3:TIMEOUT					位置情報の取得に時間がかかり過ぎた…

                // エラー番号に対応したメッセージ
                var errorInfo = [
                    "原因不明のエラーが発生しました。",
                    "位置情報の取得が許可されませんでした。",
                    "電波状況などで位置情報が取得できませんでした。",
                    "位置情報の取得に時間がかかり過ぎてタイムアウトしました。"
                ];

                // エラー番号
                var errorNo = error.code;
                // エラーメッセージ
                var errorMessage = "[エラー番号: " + errorNo + "]\n" + errorInfo[errorNo];
                // アラート表示
                //console.log("GPS :" + errorMessage);

                // 画面をアンロック
                mgf.unlockScreen();
            },
            // [第3引数] オプション
            {
                "enableHighAccuracy": false,
                "timeout": 8000,
                "maximumAge": 2000,
            }
        )
    };



})();