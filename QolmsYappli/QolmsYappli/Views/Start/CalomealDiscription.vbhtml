@Imports MGF.QOLMS.QolmsYappli

@Code
    ViewData("Title") = "お知らせ"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"
    
End Code

<body id="" class="lower">
    @Html.AntiForgeryToken()
    <main id="main-cont" class="clearfix" role="main">


        <section class="contents-area">
            <h2 class="title">@ViewData("Title")</h2>
            <hr>
            <p class="section default">
                カロリーに加え、合計30種類の栄養素がAIで画像解析できるようになりました。毎日の食事を撮影して、健康管理にお役立てください。
            </p>

            <div class="" style="display:flex;">

                <i class="w-max" style="padding:5px;"><img src="/dist/img/meal/calomeal_view1.jpg" width="100%"></i>
                <i class="w-max" style="padding:5px;"><img src="/dist/img/meal/calomeal_view2.jpg" width="100%"></i>

            </div>

        </section>

        @if ViewData("First")

            @<section class="contents-area">
                <p class="submit-area mb30 mr10">
                    @*@Code
                        dim url as string = string.format("../note/calomeal?selectdate={0}",model)
                    End Code*@
                    <a href="../note/calomeal" id="submit" class="btn btn-submit">OK</a>
                </p>
            </section>
        End If



    </main>
    
    @Html.Action("PortalFooterPartialView", "Portal")

</body>