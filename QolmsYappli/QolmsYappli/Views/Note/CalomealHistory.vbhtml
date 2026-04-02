
@Imports MGF.QOLMS.QolmsYappli

@Code
    ViewData("Title") = "ガルフスポーツTOP"
    Layout = "~/Views/Shared/_NoteLayout.vbhtml"
    
    
End Code

<body id="gulf-index" class="lower gulf">
    @Html.AntiForgeryToken()
    @*@Html.Action("PortalHeaderPartialView", "Portal")*@
    <main id="main-cont" class="clearfix" role="main">
        <img src='@ViewData("img")' />

    </main>
    @Html.Action("PortalFooterPartialView", "Portal")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/note/gulfsportsmovieindex")

</body>
<script>
</script>