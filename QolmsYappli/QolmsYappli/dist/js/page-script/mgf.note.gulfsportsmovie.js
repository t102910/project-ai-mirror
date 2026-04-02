// mgf.note.gulfsportsmovie 名前空間オブジェクト
mgf.note.gulfsportsmovie = mgf.note.gulfsportsmovie || {};

// DOM 構築完了
mgf.note.gulfsportsmovie = (function () {

    $(function () {
        //alert("ready");

        //// Player APIの非同期ロード
        //(function (document) {
        //    var api = document.createElement("script"),
        //        script;

        //    api.src = "//www.youtube.com/player_api";
        //    script = document.getElementsByTagName("script")[0];
        //    script.parentNode.insertBefore(api, script);
        //}(document));

        var tag = document.createElement('script');
        tag.src = "https://www.youtube.com/iframe_api";
        var firstScriptTag = document.getElementsByTagName('script')[0];
        firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);


        // YouTube
        var ytPlayer2;

        // Player APIスタンバイ完了時の処理
        window.onYouTubeIframeAPIReady = function () {

            //alert("onYouTubePlayerAPIReady");

            ytPlayer2 = new YT.Player("ytplayer", {
                width: "100%",
                height: 250,
                videoId: "",
                playerVars: {
                    "rel": 0,
                    "autoplay": 0,
                    "wmode": "opaque"
                }

            });
        };

        $('main').on('click', '.mov-area', function () {

            if ($(this).hasClass("disabled")) {
                return false;
            }

            $('#mov-stage').addClass('active');
            $('body').addClass('.modal-open');

            //.infoブロックをコピペする
            $('#info-copy').empty();
            $(this).siblings('.info').clone().appendTo('#info-copy');

            //youtube VideoIDをセットして埋め込む
            var videoId = $(this).data('yturl').replace(/https?:\/\/[0-9a-zA-Z\.]+\//, "");
            //console.log(ytPlayer2);
            ytPlayer2.loadVideoById(videoId);

            return false;
        });
        $('#mov-stage-close').click(function () {
            movStageClose();
            ytPlayer2.pauseVideo();
        });

        function movStageClose() {
            $('#mov-stage').removeClass('active');
            $('body').removeClass('.modal-open');
        }
    });


    // リスト「運等を登録」ボタンのクリック
    $("main").on("click", ".list-submit", function () {

        // 画面をロック
        mgf.lockScreen();

        var exerciseType = $(this).data("exercisetype");
        var calorie = $(this).data("calorie");

        // 登録内容を POST
        $.ajax({
            type: "POST",
            url: "../Note/GulfSportsMovieResult",
            traditional: true,
            data: {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                exerciseType: exerciseType,
                calorie: calorie
            },
            async: true,
            beforeSend: function (jqXHR) {
                // セッションのチェック
                mgf.note.checkSessionByAjax();
            }
        }).done(function (data, textStatus, jqXHR) {
            if (jqXHR.status == 200 && jqXHR.getResponseHeader("Content-Type").indexOf("application/json;") >= 0) {
                try {
                    if ($.isTrueString($.parseJSON(data)["IsSuccess"])) {
                        // 成功
                        $("#finish-modal .modal-body").text("運動を登録しました。");
                        $("#finish-modal").modal("show")

                    } else {
                        // 失敗
                        $("#finish-modal .modal-body").text($.parseJSON(data)["Messages"][0]);
                        $("#finish-modal").modal("show")

                    }
                } catch (ex) {
                    // エラー
                    $("#finish-modal .modal-body").text("運動の登録に失敗しました。");
                    $("#finish-modal").modal("show")
                    //$("div.input-area section.caution").text("登録に失敗しました。").removeClass("hide");
                }
            } else {
                // エラー
                $("#finish-modal .modal-body").text("登録に失敗しました。");
                $("#finish-modal").modal("show")
                //$("div.input-area section.caution").text("登録に失敗しました。").removeClass("hide");
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            // エラー
            $("#finish-modal .modal-body").text("運動の登録が失敗しました。status = " + textStatus);
            $("#finish-modal").modal("show")
            //$("div.input-area section.caution").text("登録に失敗しました。").removeClass("hide");
        }).always(function (jqXHR, textStatus) {
            // 画面をアンロック
            mgf.unlockScreen();
        });

    });


    //確認モーダル
    // 「×」ボタン
    $("main").on("click", "#finish-modal div.modal-header button.close", function () {
        try {
            $("#finish-modal").modal("hide");
        } catch (ex) {
            //console.log(ex);
        }
    });

    // 「閉じる」ボタン
    $("main").on("click", "#finish-modal div.modal-footer button.btn-close", function () {
        try {
            $("#finish-modal").modal("hide");
        } catch (ex) {
            //console.log(ex);
        }
    });



    //画面のロード終了時に動画再生クリック有効化
    window.onload = function () {

        $('.mov-area').each(function (index) {
            //alert(this);
            $(this).removeClass("disabled");
        });
        //alert("window.onunload");

    };

    ////ヒストリー バックのイベント取得
    //window.addEventListener('popstate', function (e) {

    //    console.log('ボタンがクリックされました');
    //    console.log(e.state);
    //    //return false;
    //});

    // ヒストリー バック禁止
    mgf.prohibitHistoryBack();

    // セッションのチェック
    mgf.note.checkSessionByAjax();

})();
