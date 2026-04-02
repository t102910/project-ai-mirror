@Code
    ViewData("Title") = "SMSAuthentication"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"
End Code


<body id="sign-up" class="lower">

    @Html.AntiForgeryToken()

    <main id="main-cont" class="clearfix" role="main">

        <section class="home-btn-wrap" data-pageno="">
            <a class="home-btn type-2" href="../portal/userinfomation"><i class="la la-angle-left"></i><span> 戻る</span></a>
        </section>

        <section class="contents-area">

            <h2 class="title">@ViewData("Title")</h2>
            <hr>
            <div>
                <section class="section default">
                    パスワードリセットの本人認証用のパスコードを送信します。。<br />
                    JOTOホームドクターに登録済みの携帯電話番号を入力してください。
                </section>

                @*入力*@
                <h3 class="title mt10">
                    <span>電話番号</span>
                </h3>
                <div class="form wizard-form mb30">

                    <label for="mail" class="t-row">
                        <span class="label-txt">
                            <span class="ico required">必須</span>
                            登録された電話番号
                        </span>
                        <input type="text" id="mail" name="model.MailAddress" class="form-control mb10" placeholder="電話番号" value="" required="required" maxlength="256" style="ime-mode:active;" autocomplete="off">
                        <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                    </label>

                    <section class="section default">
                        <br />
                        <br />
                        リセット用のパスコードはメール送信から10分間です。<br />
                        期限が切れてしまった場合は再度パスコードの送信をお願いいたします。<br />
                    </section>

                    <p class="submit-area mb30">

                        @*<section id="summary-cation" class="section caution mt0 mb10 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>*@

                        <a id="request" href="javascript:void(0);" class="btn btn-submit">送 信</a>
                    </p>
                </div>

            </div>
        </section>

    </main>

    @*@Html.Action("PasswordResetFooterPartialView", "PasswordReset")*@

    @QyHtmlHelper.RenderScriptTag("~/dist/js/portal/smsauthentication")
</body>
