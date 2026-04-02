@Imports MGF.QOLMS.QolmsYappli

@Code
    ViewData("Title") = "女性の健康サポート画面"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"

End Code
<body id="mov-for-female" class="lower">
    <main id="main-cont" class="clearfix" role="main">
        <section class="data-area no-padding">
            <h2 class="img-max-cont mb10">
                <img class="mb10 w-max" src="/dist/img/mov-for-female/mv.jpg">
            </h2>
            <div class="img-max-cont">
                <img src="/dist/img/mov-for-female/title.jpg" alt="" />


                @*働く女性のためのヘルスケア講座（https://youtu.be/xuhyoKs7A_o）*@
                <img src="/dist/img/mov-for-female/img-1.jpg" class="mov-area" data-yturl="xuhyoKs7A_o" alt="" />
                @*企業・管理職の女性他のためのヘルスケア講座（https://youtu.be/N77ef1DNQn0）*@
                <img src="/dist/img/mov-for-female/img-2.jpg" class="mov-area" data-yturl="N77ef1DNQn0" alt="" />

                <img src="/dist/img/mov-for-female/img-3.jpg" alt="" />
            </div>
                </section>
            </main>

            @Html.Action("PortalFooterPartialView", "Portal")
            @QyHtmlHelper.RenderScriptTag("~/dist/js/portal/movforfemale")

            <!-- 動画再生 -->
            <div id="mov-stage">
                <div class="mov-wrap">
                    <div id="mov">
                        <div id="ytplayer"></div>
                    </div>
                    <div id="info-copy">

                    </div>
                </div>
                <i class="la la-close" id="mov-stage-close"></i>
            </div>
        </body>
