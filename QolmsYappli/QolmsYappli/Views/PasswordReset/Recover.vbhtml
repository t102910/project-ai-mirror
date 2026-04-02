@Imports MGF.QOLMS.QolmsYappli
@ModelType PasswordResetrecoverInputModel

@Code
    ViewData("Title") = "パスワードリセット"
    Layout = "~/Views/Shared/_PasswordResetLayout.vbhtml"

End Code

<body id="sign-up" class="lower">

    @Html.AntiForgeryToken()

    <main id="main-cont" class="clearfix" role="main">

        <section class="home-btn-wrap" data-pageno="">
            <a class="home-btn type-2" href="../passwordreset/selectmethod"><i class="la la-angle-left"></i><span> 戻る</span></a>
        </section>

        <section class="contents-area">

            <h2 class="title">@ViewData("Title")</h2>
            <hr>
            <div>
                <section class="section default">
                    パスワードリセットのURLを送信します。<br />
                    JOTOホームドクターに登録済みのメールアドレスを入力してください。
                </section>

                @*入力*@
                <h3 class="title mt10">
                    <span>メール</span>
                </h3>
                <div class="form wizard-form mb30">

                    <label for="mail" class="t-row">
                        <span class="label-txt">
                            <span class="ico required">必須</span>
                            登録メールアドレス
                        </span>
                        <input type="text" id="mail" name="model.MailAddress" class="form-control mb10" placeholder="メールアドレス" value="" required="required" maxlength="256" style="ime-mode:active;" autocomplete="off">
                        <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                    </label>

                    <section class="section default">
                        「joto-hdr@qolms.com」からメールが送信されます。<br />
                        メールが届かない場合は受信設定のご確認をお願いいたします。<br />
                        <br />
                        リセット用のURLはメール送信から30分間です。<br />
                        期限が切れてしまった場合は再度パスワードリセットメールの送信をお願いいたします。<br />
                    </section>

                    <p class="submit-area mb30">

                        @*<section id="summary-cation" class="section caution mt0 mb10 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>*@

                        <a id="request" href="javascript:void(0);" class="btn btn-submit">送 信</a>
                    </p>
                </div>

            </div>
        </section>

    </main>

    @Html.Action("PasswordResetFooterPartialView", "PasswordReset")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/passwordreset/recover")
</body>
