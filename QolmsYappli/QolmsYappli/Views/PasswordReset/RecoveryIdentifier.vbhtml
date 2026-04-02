@Imports MGF.QOLMS.QolmsYappli
@ModelType PasswordResetRecoveryIdentifierInputModel

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
                    パスワードリセット
                    メールアドレスの受信設定を確認してください
                </section>

                @*入力*@
                <h3 class="title mt10">
                    <span>登録情報</span>
                </h3>
                <div class="form wizard-form mb30">

                    <input type="text" id="password-reset-key" name="model.PasswordResetKey" class="form-control mb10 hide" value="@Me.Model.PasswordResetKey">

                    <label for="jotoid" class="t-row line">
                        <span class="label-txt">
                            <span class="ico required">必須</span>
                            JOTO ID
                        </span>
                        <input type="text" id="jotoid" name="model.JotoId" class="form-control mb10" placeholder="JOTO ID" value="@Me.Model.JotoId" required="required" maxlength="100" style="ime-mode:active;" autocomplete="off">
                        <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                    </label>

                    <div class="t-row line">
                        <label for="name" class="label-txt"><span class="ico required">必須</span> お名前</label>
                        <input type="text" id="family-name" name="model.FamilyName" class="form-control mb10" placeholder="姓" value="@Me.Model.FamilyName" required="required" maxlength="25" style="ime-mode:active;" autocomplete="off">
                        <input type="text" id="given-name" name="model.GivenName" class="form-control" placeholder="名" value="@Me.Model.GivenName" required="required" maxlength="25" style="ime-mode:active;" autocomplete="off">
                        <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                    </div>

                    <label for="sex" class="t-row line">
                        <span class="label-txt"><span class="ico required">必須</span> 性　別</span>
                        @QyHtmlHelper.ToSexDropDownList("sex", "model.Sex", Me.Model.Sex)
                        <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>

                    </label>
                    <div class="t-row line">
                        <label for="birth-year" class="label-txt"><span class="ico required">必須</span> 生年月日</label>
                        @QyHtmlHelper.ToYearDropDownList("birth-year", "model.BirthYear", Me.Model.BirthYear, "form-control mb10")

                        @QyHtmlHelper.ToMonthDropDownList("birth-month", "model.BirthMonth", Me.Model.BirthMonth, "form-control mb10")

                        @QyHtmlHelper.ToDayDropDownList("birth-day", "model.BirthDay", Me.Model.BirthDay, "form-control")

                        <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                    </div>
                    <label for="mail" class="t-row">
                        <span class="label-txt"><span class="ico required">必須</span> 登録メールアドレス</span>
                        <input type="text" id="mail" name="model.MailAddress" class="form-control w-max " value="@Me.Model.MailAddress" required="required" maxlength="256" style="ime-mode:disabled;" autocomplete="off" disabled>
                        <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                    </label>

                    <section id="summary-cation" class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                    <p class="submit-area mb30">
                        <a id="request" href="javascript:void(0);" class="btn btn-submit">確 認</a>
                    </p>
                </div>

            </div>
        </section>

    </main>
    @Html.Action("PasswordResetFooterPartialView", "PasswordReset")
    @QyHtmlHelper.RenderScriptTag("~/dist/js/passwordreset/recoveryidentifier")
</body>
