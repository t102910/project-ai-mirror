@Imports MGF.QOLMS.QolmsYappli
@Code
    ViewData("Title") = "レシピ動画一覧"

    Layout = "~/Views/Shared/_NoteLayout.vbhtml"

End Code


<body id="gulf-index" class="lower gulf">

    <main id="main-cont" class="clearfix" role="main">
        <section class="home-btn-wrap">
           <a href="../Portal/Home" class="home-btn disabled"><i class="la la-angle-left"></i><i class="la la-home la-15x"></i><span> ホーム</span></a>
        </section>
	    <section class="contents-area mb20">
		    <h2 class="center mt40 mb20"><img class="w-max" src="/dist/img/recipe/index-title.png"></h2>
		    <ul>
			    <li class="img-max-cont mb20"><a href="../note/recipemovie?movietype=1"><img src="/dist/img/recipe/index-btn-1.jpg"></a></li>
			    @*<li class="img-max-cont mb5"><img src="/dist/img/recipe/index-coming-soon.png"></li>*@
		    </ul>
	    </section>
    </main>
</body>