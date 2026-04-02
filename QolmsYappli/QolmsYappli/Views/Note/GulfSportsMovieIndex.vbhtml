
@Imports MGF.QOLMS.QolmsYappli
@*@ModelType PortalUnsubscribeInputModel*@

@Code
    ViewData("Title") = "ガルフスポーツTOP"
    Layout = "~/Views/Shared/_NoteLayout.vbhtml"
    
    
End Code
<script type="text/javascript" src="https://www.youtube.com/player_api"></script>

<body id="gulf-index" class="lower gulf">
    @Html.AntiForgeryToken()
    @*@Html.Action("PortalHeaderPartialView", "Portal")*@
    <main id="main-cont" class="clearfix" role="main">
	    <section class="contents-area mb20">
		    <h2 class="center mt40 mb20"><img class="w200" src="/dist/img/gulf-mov/index-title.png"></h2>
            <div class="right">
                <a href="../Note/Exercise" class="btn btn-close no-ico">戻 る</a>
			</div>
		    <ul>
			    <li class="img-max-cont mb20"><a href="../Note/GulfSportsMovie?movieType=1"><img src="/dist/img/gulf-mov/index-btn-1.jpg"></a></li>
			    <li class="img-max-cont mb20"><a href="../Note/GulfSportsMovie?movieType=2"><img src="/dist/img/gulf-mov/index-btn-2.jpg"></a></li>
			    <li class="img-max-cont mb20"><a href="../Note/GulfSportsMovie?movieType=3"><img src="/dist/img/gulf-mov/index-btn-3.jpg"></a></li>
			    @*<li class="img-max-cont mb5"><img src="/dist/img/gulf-mov/index-btn-4.png"></li>*@
		    </ul>
		    <p class="right"><a href="native:/action/open_browser?url=https%3A%2F%2Fwww.gulfwavezone.jp%2F">ガルフ公式ホームページはこちら</a></p>
	    </section>

@*        <!-- 「登録完了」ダイアログ -->
        <div class="modal fade" id="finish-modal" tabindex="-1" data-backdrop="static" data-keyboard="false">
	        <div class="modal-dialog">
		        <div class="modal-content">
			        <div class="modal-header">
				        <button type="button" class="close"><span>×</span></button>
				        <h4 class="modal-title">確認</h4>
			        </div>
			        <div class="modal-body">
				        登録しました。
			        </div>
			        <div class="modal-footer">
				        <button type="button" class="btn btn-close no-ico mb0">閉じる</button>
			        </div>
		        </div>
	        </div>
        </div>*@


    </main>
    @Html.Action("PortalFooterPartialView", "Portal")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/note/gulfsportsmovieindex")

</body>
<script>
</script>