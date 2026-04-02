@Imports MGF.QOLMS.QolmsYappli
@ModelType NoteRecipeMovieViewModel

@Code
    ViewData("Title") = "RecipeMovie"
    
    Layout = "~/Views/Shared/_NoteLayout.vbhtml"
    
End Code

<body id="gulf-cont" class="lower gulf">
    @Html.AntiForgeryToken()

    <main id="main-cont" class="clearfix" role="main">
	    <section class="contents-area mb20">
		    <h2 class="img-max-cont mb10">
			    <img class="mb10" src="/dist/img/recipe/detail/title-1.png">
			    <img src="/dist/img/recipe/index-btn-1.jpg">
		    </h2>
            <div class="right">
                <a href="../Note/RecipeMovieIndex" class="btn btn-close no-ico">戻 る</a>
			</div>
	    </section>
		    <section class="data-area">

            @For Each item As RecipeMovieItem In Me.Model.MovieItemN
            
		        @<article>
			        <section class="inner">
				        <p class="mov-area disabled" data-yturl="@item.Id">
					        <img class="icon" src="/dist/img/gulf-mov/detail/mov-ico.png">
				        </p>

				        <div class="info">
					        <p class="time"><span>@item.Time</span></p>
					        <p class="bold">@item.Discription</p>
					        <p class="cal-area">
                                @code
                                    Dim url As String = String.Format("https%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv%3D{0}", item.Id)
                                End Code
                                <span class="youtube-icon-area"><a href="native:/action/open_browser?url=@url" class="youtube-icon"></a></span>
						        <span class="cal right" >@item.Calorie<i>kcal</i></span>
						        @*<span><a  href="javascript:void(0);" class="btn btn-submit no-ico narrow low-height mb0 mt10 list-submit" data-exercisetype="@item.ExerciseType" data-calorie="@item.Calorie">カロリー登録</a></span>*@
					        </p>
				        </div>
			        </section>
		        </article>      
                
            Next
	    </section>
                <!-- 動画再生 -->
        <div id="mov-stage">
	        <div class="mov-wrap">
		        <div id="mov">
			        <div id="ytplayer"></div>
		        </div>
		        <div id="info-copy">
			
		        </div>
	        </div>
	        <i class="la la-close" id="mov-stage-close"></i>
        </div>

    </main>

    @Html.Action("NoteFooterPartialView", "Note")
    @QyHtmlHelper.RenderScriptTag("~/dist/js/note/recipemovie")

@*    <script type="text/javascript">
    var ytPlayer2;

    // Player APIの非同期ロード
    (function (document) {
        var api = document.createElement("script"),
            script;
    
        api.src = "//www.youtube.com/player_api";
        script = document.getElementsByTagName("script")[0];
        script.parentNode.insertBefore(api, script);
    }(document));

    // Player APIスタンバイ完了時の処理
    function onYouTubePlayerAPIReady() {
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
    }
    $(function() {
	    $('.mov-area').click(function(){
		    $('#mov-stage').addClass('active');
		    $('body').addClass('.modal-open');
		
		    //.infoブロックをコピペする
		    $('#info-copy').empty();
		    $(this).siblings('.info').clone().appendTo('#info-copy');
		
		    //youtube VideoIDをセットして埋め込む
            var videoId = $(this).data('yturl').replace(/https?:\/\/[0-9a-zA-Z\.]+\//, "");
            ytPlayer2.loadVideoById(videoId);
		
		    return false;
	    });
	    $('#mov-stage-close').click(function(){
		    movStageClose();
		    ytPlayer2.pauseVideo();
	    });

	    function movStageClose(){
		    $('#mov-stage').removeClass('active');
		    $('body').removeClass('.modal-open');
	    }
    });
    </script>*@
</body>