@Code
    ViewData("Title") = "パスワードリセット方法の選択"
    Layout = "~/Views/Shared/_PasswordResetLayout.vbhtml"
End Code


<body id="sign-up" class="lower">


    <main id="main-cont" class="clearfix" role="main">

        <section class="home-btn-wrap" data-pageno="">
            <a class="home-btn type-2" href="../start/login"><i class="la la-angle-left"></i><span> 戻る</span></a>
        </section>

        <section class="contents-area">

            <h2 class="title">@ViewData("Title")</h2>
            <hr>
            <div>
                <section class="section default">
                    SMS認証が使用できる携帯番号をJOTOホームドクターへ登録済みの方<br />

                </section>

                <a href="../passwordreset/recoversms" Class="btn btn-link no-ico back-FFF super-btn mb30">
                   SMS

                </a>

                @*<p Class="center ">
                    説明が記載できます
                </p>*@

                <section class="section default">
                    上記以外の方<br />

                </section>

                <a href="../passwordreset/recover" Class="btn btn-link no-ico back-FFF super-btn mb30">
                    メールアドレス

                </a>
                @*<p Class="center mb30">
                    説明が記載できます。
                </p>*@
            </div>
        </section>

    </main>

    @Html.Action("PasswordResetFooterPartialView", "PasswordReset")

</body>