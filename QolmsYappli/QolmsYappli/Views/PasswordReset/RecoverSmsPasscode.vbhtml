@Imports MGF.QOLMS.QolmsYappli
@ModelType PasswordResetRecoverSMSPassCodeViewModel

@Code
    ViewData("Title") = "パスワードリセット"
    Layout = "~/Views/Shared/_PasswordResetLayout.vbhtml"
End Code


<body id="sign-up" class="lower">

    @Html.AntiForgeryToken()

    <main id="main-cont" class="clearfix" role="main">

        <section class="home-btn-wrap" data-pageno="">
            <a class="home-btn type-2" href="../start/login"><i class="la la-angle-left"></i><span> 戻る</span></a>
        </section>

        <section class="contents-area">

            <h2 class="title">@ViewData("Title")</h2>
            <hr>
            <div>
                <section class="section default">
                    <p id="cphone" data-phone="@Model.CryptPhoneNumber">@Model.DispPhoneNumber</p>にパスコードを送信しました。<br />
                    <br />
                    パスコードの有効期限は送信から10分間です。<br />
                    リトライ回数の上限や期限が切れてしまった場合は再度パスコードの送信をお願いいたします。<br />
                </section>

                <h3 class="title mt10">
                    <span>パスコード</span>
                </h3>
                <div class="form wizard-form mb30">


                    <label for="mail" class="t-row">
                        <span class="label-txt">
                            <span class="ico required">必須</span>
                            パスコード
                        </span>
                        <input type="text" id="Pass" name="model.Passcode" class="form-control mb10" placeholder="パスコード" value="" required="required" maxlength="256" style="ime-mode:active;" autocomplete="off">
                        <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                    </label>


                    <p class="submit-area mb30">

                        @*<section id="summary-cation" class="section caution mt0 mb10 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>*@

                        <a id="passcode" href="javascript:void(0);" class="btn btn-submit">送 信</a>
                    </p>
                </div>

            </div>
        </section>

    </main>

    @Html.Action("PasswordResetFooterPartialView", "PasswordReset")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/passwordreset/recoversms")
</body>
