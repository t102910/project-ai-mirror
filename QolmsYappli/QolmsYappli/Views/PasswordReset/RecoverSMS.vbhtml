@Imports MGF.QOLMS.QolmsYappli
@*@ModelType PasswordResetrecoverInputModel*@

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
                    JOTOホームドクターに登録済みの携帯番号を入力してください。<br />
                    パスワードリセット用のパスコードがSMSで届きますので、パスコードを入力して下さい。<br />
                    <br />
                    パスコードが届かない場合は、「SMSが届かない方はこちら」のリンクより、パスワードリセットをして下さい。<br />
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
                        <input type="tel" id="mail" name="model.PhoneNumber" class="form-control mb10" placeholder="電話番号" value="" required="required" maxlength="11" style="ime-mode:active;" autocomplete="off">
                        <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                    </label>

                    @*<section class="section default">

                    </section>*@
                    <p class=" mb20">
                        <a href="~/PasswordReset/Recover">
                                
                            SMS認証ができない方はこちら
                        </a>
                    </p>
                    <p class="submit-area mb30">

                        @*<section id="summary-cation" class="section caution mt0 mb10 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>*@

                        <a id="request" href="javascript:void(0);" class="btn btn-submit">送 信</a>
                    </p>
                </div>

            </div>
        </section>

    </main>

    @Html.Action("PasswordResetFooterPartialView", "PasswordReset")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/passwordreset/recoversms")
</body>
