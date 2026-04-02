@Imports MGF.QOLMS.QolmsYappli
@ModelType StartSetupInputModel2

@Code
    'ViewData("Title") = "初期設定"
    Layout = "~/Views/Shared/_StartLayout.vbhtml"
    
    Dim bodyId As String = "info-form"
    
    Select Case Me.Model.StepMode
        'Case 2
        '    ViewData("Title") = "現状把握"
        '    bodyId = "setting"
        
        Case 3
            ViewData("Title") = "目標設定"
            bodyId = "inquiry"
        
        Case Else
            ViewData("Title") = "基本情報"
            bodyId = "info-form"
            
    End Select
End Code

<body id="@bodyId" class="lower">
    @Html.AntiForgeryToken()

    <main id="main-cont" class="clearfix" role="main">
        @Select Case Me.Model.StepMode
            Case 1
                @Html.Action("StartSetupStepOnePartialView2", "Start")

            Case 3
                @Html.Action("StartSetupStepThreePartialView2", "Start")

        End Select
    </main>

@*    <footer role="contentinfo">
	    <div class="inner">
		    <address>COPYRIGHT &copy; Okinawa Cellular Telephone Company,All Rights Reserved.</address>
	    </div>
    </footer>*@

    <div id="white-out" class="hide">
        <div class="loader">
@*            <div class="loader-inner ball-triangle-path">
                <div></div>
                <div></div>
                <div></div>
            </div>*@
        </div>
        <div id="progress-area">
            <div class="progress progress-striped">
                <div class="progress-bar progress-bar-warning active" style="width: 100%"></div><!-- styleを動的に書き換えるとアニメーションします。 -->
            </div>
            <span>100%</span>進行中…<!-- ここも同じ値を -->
        </div>
        <p id="text-area"></p>
    </div>

    @QyHtmlHelper.RenderScriptTag("~/dist/js/start/setup2")
</body>
