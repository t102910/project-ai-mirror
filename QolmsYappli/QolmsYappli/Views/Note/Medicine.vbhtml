@Imports MGF.QOLMS.QolmsYappli
@ModelType NoteMedicineViewModel
@Code
    ViewData("Title") = "おくすり"

    Layout = "~/Views/Shared/_NoteLayout.vbhtml"

End Code

<body id="medicine" class="lower">

    <main id="main-cont" class="clearfix" role="main">
	    <section class="home-btn-wrap type-2">
		    <a href="../Portal/Home" class="home-btn"><i class="la la-home"></i><span> ホーム</span></a>
	    </section>
	    <section class="data-area">
		    <h3 class="title mt10">最近のお薬</h3>
            <!--partial view-->
            @Html.Action("NoteMedicineTablePartialView", "Note")
	    </section>
    </main>
</body>