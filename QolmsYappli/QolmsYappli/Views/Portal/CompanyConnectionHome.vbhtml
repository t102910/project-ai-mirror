@Imports MGF.QOLMS.QolmsYappli
@ModelType PortalCompanyConnectionHomeViewModel

@Code
    ViewData("Title") = "企業連携ホーム"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"
    
End Code
<header class="contents-header">
	<h1><img src="/dist/img/biz/biz-logo.png"></h1>

     @Select Case Me.Model.FromPageNoType
         Case QyPageNoTypeEnum.PortalHome
	    @<section class="home-btn-wrap">
		    <a href="../Portal/Home" class="home-btn type-2"><i class="la la-angle-left"></i><span> 戻る</span></a>
	    </section>         
     End Select

</header>
<body id="menu" class="lower business">

    @Html.AntiForgeryToken()

    <main id="main-cont" class="clearfix" role="main">
	    <section class="contents-area mb20">
		    <div class="flex-wrap">
			    <section class="menu-list left-cont examination">
				    <h3><img src="/dist/img/biz/menu-1.png" alt="" /></h3>
				    <div class="inner">
					    健康診断の結果を表示します
				    </div>
			    </section>
                @*カミングスーンは.coming-soonを付与してください*@
			    <section class="menu-list right-cont mov-for-female">
				    <h3><img src="/dist/img/biz/menu-2.png" alt="" /></h3>
                    <div class="inner">
                        セミナー動画など女性の健康に関する各種コンテンツを配信しています。
                    </div>
			    </section>
		    </div>
		    <div class="flex-wrap">
			    <section class="menu-list left-cont coming-soon">
				    <h3><img src="/dist/img/biz/menu-3.png" alt="" /></h3>
				    <div class="inner">
				    </div>
			    </section>
			    <section class="menu-list right-cont coming-soon">
				    <h3><img src="/dist/img/biz/menu-4.png" alt="" /></h3>
				    <div class="inner"></div>
			    </section>
		    </div>
		
		
	    </section>	
    </main>

    @Html.Action("PortalFooterPartialView", "Portal")
    @QyHtmlHelper.RenderScriptTag("~/dist/js/portal/companyconnectionhome")
</body>