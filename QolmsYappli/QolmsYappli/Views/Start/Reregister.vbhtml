@Code

    ViewData("Title") = "再登録"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"
    
End Code

<body id="challenge" class="lower">
    @Html.AntiForgeryToken()

    <main id="main-cont" class="clearfix" role="main" style="background: #efefef">
	    <section class="contents-area mb20">
		    <div class="box type-2 success-contents">
			    <p class="success-image">
				    @*<img class="point-get" src="/dist/img/challenge/tmpl/point-get.png" alt="" />*@
				    <img class="shika" src="/dist/img/tmpl/s-3.png" alt="" />
			    </p>
			    <div class="text-area mb30">
				    <p class="small mb30">
					        おかえりなさい！
				    </p>
				@*    <p class="point-result">
					    <span class="point">1000</span><i>P</i>
				    </p>*@
				    以前のアカウントを<br/>復活します
			    </div>
			    <div class="text-area">
				    <a class="btn btn-submit big" href="#">
					    復活する
				    </a>
			    </div>
		    </div>
	    </section>	
    </main>
    
    @Html.Action("PortalFooterPartialView", "Portal")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/start/reregister")

</body>