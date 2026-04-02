// mgf.portal.movforfemale 名前空間オブジェクト
mgf.portal.movforfemale = mgf.portal.movforfemale || {};

// DOM 構築完了
mgf.portal.movforfemale = (function () {

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
        tag.src = "https://www.youtube.com/player_api";
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

            //if ($(this).hasClass("disabled")) {
            //    return false;
            //}

            $('#mov-stage').addClass('active');
            $('body').addClass('.modal-open');

            ////.infoブロックをコピペする
            //$('#info-copy').empty();
            //$(this).siblings('.info').clone().appendTo('#info-copy');

            //youtube VideoIDをセットして埋め込む
            var videoId = $(this).data('yturl').replace(/https?:\/\/[0-9a-zA-Z\.]+\//, "");
            console.log(ytPlayer2);
            console.log(videoId);
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


    //画面のロード終了時に動画再生クリック有効化
    window.onload = function () {

        $('.mov-area').each(function (index) {
            //alert(this);
            $(this).removeClass("disabled");
        });
        //alert("window.onunload");

    };


    // ヒストリー バック禁止
    mgf.prohibitHistoryBack();

    // セッションのチェック
    mgf.portal.checkSessionByAjax();

})();
