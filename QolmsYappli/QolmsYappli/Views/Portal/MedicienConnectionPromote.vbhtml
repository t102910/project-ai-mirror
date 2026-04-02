@Imports MGF.QOLMS.QolmsYappli
@*@ModelType PortalHomeViewModel*@

@Code
    ViewData("Title") = "おくすり連携プロモーション"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"
 End Code

<body id="pharmacy-cooperation" class="lower">
    
    @Html.AntiForgeryToken()
    <main id="main-cont" class="clearfix" role="main">
	    <section class="home-btn-wrap">
		    <a href="../Portal/Home" class="home-btn"><i class="la la-angle-left"></i><span> 戻る</span></a>
	    </section>
	    <section class="data-area">
		    <div class="box type-2 first-child">
			    <p class="center btn-wrap"><a href="../note/medicine"><img src="../dist/img/pharmacy-cooperation/btn-1.jpg" alt="" /></a></p>
			    <img src="../dist/img/pharmacy-cooperation/img-1.jpg" alt="" class="w-max" />
		    </div>
		    <div class="box type-2">
			    <img src="../dist/img/pharmacy-cooperation/img-2.jpg" alt="" class="w-max" />
		    </div>
	    </section>
    </main>
    
    @Html.Action("PortalFooterPartialView", "Portal")

</body>
    
