@Imports System.Collections.ObjectModel
@Imports MGF.QOLMS.QolmsYappli
@Imports MGF.QOLMS.QolmsCryptV1
@ModelType NoteExaminationViewModel

@Code
    ViewData("Title") = "健診結果"
    Layout = "~/Views/Shared/_NoteLayout.vbhtml"
    
End Code

<script src="https://use.typekit.net/tpf8tba.js"></script>
<script>try{Typekit.load({ async: true });}catch(e){}</script>

<body id="kenshin" class="lower result">
    @Html.AntiForgeryToken()
        
    <main id="main-cont" class="clearfix mb100" role="main">
        <section class="home-btn-wrap">
             <a href="../Portal/Home" class="home-btn"><i class="la la-angle-left"></i><i class="la la-home la-15x"></i><span> ホーム</span></a>
        </section> 

	        @Html.Action("NoteExaminationResultPartialView", "Note")

	        @Html.Action("NoteExaminationFilterPartialView", "Note")

	    </section>

    </main>

    <section class="contents-area fixed">
	    <div class="btn-area">
            @Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsWeb)
                @<a id ="calculation" href="javascript:void(0);" class="btn btn-style3 block bold mb0" data-reference="@crypt.EncryptString(Me.Model.HealthAgeCalcJson)">
			        <img class="ico-img" src="/dist/img/health-age/usual.png" alt="" />
                    健康年齢を測定しよう!
		        </a>
            End Using
		    
	    </div>
    </section>
    
    <div id="modal-text" class="hide">
	    <h3><span>文章表示</span></h3>

	    <div class="text-body">文章が表示されます。文章が表示されます。文章が表示されます。文章が表示されます。文章が表示されます。</div>
        <br />
	    <div class="text-standard"></div>
	    <div class="text-unit"></div>
	    <i class="la la-close" id="modal-close"></i>
    </div>

    <div class="modal fade" id="info-modal" tabindex="-1">
	    <div class="modal-dialog">
		    <div class="modal-content">
			    <div class="modal-header">
				    <button type="button" class="close" data-dismiss="modal"><span>×</span></button>
				    <h4 class="modal-title">項目説明文</h4>
			    </div>
			    <div class="modal-body">
				    項目説明文が入ります。項目説明文が入ります。項目説明文が入ります。項目説明文が入ります。項目説明文が入ります。
			    </div>
			    <div class="modal-footer">
				    <button type="button" class="btn btn-close" data-dismiss="modal">閉じる</button>
			    </div>
		    </div>
	    </div>
    </div>
    
    
    @Html.Action("NoteFooterPartialView", "Note")
    @QyHtmlHelper.RenderScriptTag("~/dist/js/note/examination")

    <link rel="stylesheet" href="/dist/css/slick.css" type="text/css"><!-- slider -->
    <link rel="stylesheet" href="/dist/css/slick-theme.css" type="text/css"><!-- slider -->
    <script type="text/javascript" src="/dist/js/slick.min.js"></script><!-- slider -->
</body>


