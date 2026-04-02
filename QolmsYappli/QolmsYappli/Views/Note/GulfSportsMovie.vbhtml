@Imports MGF.QOLMS.QolmsYappli
@ModelType NoteGulfSportsMovieViewModel

@Code
    ViewData("Title") = "ガルフ動画"
    Layout = "~/Views/Shared/_NoteLayout.vbhtml"
    
    Dim model As NoteGulfSportsMovieViewModel = Me.Model'確認用コード
    
End Code
<!--読み込みのタイミング-->

<body id="gulf-cont" class="lower gulf">
    @Html.AntiForgeryToken()

  
    <main id="main-cont" class="clearfix" role="main">
	    <section class="contents-area mb20">
		    <h2 class="img-max-cont mb10">

                @If Me.Model.MovieType = 1 Then

			        @<img class="mb10" src="/dist/img/gulf-mov/detail/title-1.png">
			        @<img src="/dist/img/gulf-mov/index-btn-1.jpg">
                ElseIf Me.Model.MovieType = 2 Then
                    
                    @<img class="mb10" src="/dist/img/gulf-mov/detail/title-2.png">
			        @<img src="/dist/img/gulf-mov/index-btn-2.jpg">
                ElseIf Me.Model.MovieType = 3 Then
                      
                    @<img class="mb10" src="/dist/img/gulf-mov/detail/title-3.png">
			        @<img src="/dist/img/gulf-mov/index-btn-3.jpg">
                End If

		    </h2>
            <div class="right">
                <a href="../Note/GulfSportsMovieIndex" class="btn btn-close no-ico">戻 る</a>
			</div>
	    </section>
	    <section class="data-area">

            @For Each item As MovieItem In Me.Model.MovieItemN
            
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
						        <span><a  href="javascript:void(0);" class="btn btn-submit no-ico narrow low-height mb0 mt10 list-submit" data-exercisetype="@item.ExerciseType" data-calorie="@item.Calorie">カロリー登録</a></span>
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

        <!-- 「登録完了」ダイアログ -->
        <div class="modal fade" id="finish-modal" tabindex="-1" data-backdrop="static" data-keyboard="false" style ="z-index:100000;">
	        <div class="modal-dialog">
		        <div class="modal-content">
			        <div class="modal-header">
				        <button type="button" class="close"><span>×</span></button>
				        <h4 class="modal-title">確認</h4>
			        </div>
			        <div class="modal-body">
				        運動を登録しました。
			        </div>
			        <div class="modal-footer">
				        <button type="button" class="btn btn-close no-ico mb0">閉じる</button>
			        </div>
		        </div>
	        </div>
        </div>

    </main>
    @*<script type="text/javascript" src="https://www.youtube.com/player_api"></script>*@
    @*<script type="text/javascript" src="../dist/js/page-script/youtubeplayer.js"></script>*@
    @QyHtmlHelper.RenderScriptTag("~/dist/js/note/gulfsportsmovie")
    @Html.Action("NoteFooterPartialView", "Note")

    <script>
    
    </script>
</body>


