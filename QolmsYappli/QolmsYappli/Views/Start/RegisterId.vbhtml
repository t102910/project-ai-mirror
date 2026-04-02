@Imports MGF.QOLMS.QolmsYappli
@ModelType StartRegisterUserIdInputModel

@Code
    ViewData("Title") = "アカウント登録"
    Layout = "~/Views/Shared/_StartLayout.vbhtml"

    Dim str As String = "!#$%&()*+,-./<=>?@[\]^_{​​​​​​​​|}​​​​​​​​​~:;"
End Code

<body id="sign-up" class="lower">
    <main id="main-cont" role="main">
        <div id="cont-area">
            <div class="cont-wrap contents-area">
                <h2 class="title">アカウント登録フォーム</h2>
                <hr>
                <div class="inner type-1">
                    <p class="section default">
                        必須項目を入力し、登録ボタンを押してください。<br />
                        性別、生年月日は登録後変更できません。
                    </p>
                    @Using Html.BeginForm("RegisterIdResult", "Start", Nothing, FormMethod.Post, New With {.class = "form-inline"})
                        @Html.AntiForgeryToken()

                        @<label for="111" class="t-row line">
                            <span class="label-txt"><span class="ico required">必須</span> ログイン情報</span>
                            <input type="text" id="" name="model.UserId" class="form-control mb10" placeholder="ID（携帯電話番号もしくはメールアドレス）" value="@Me.Model.UserId" required="required" maxlength="100" autocomplete="off">
                            @QyHtmlHelper.ToValidationMessageArea({"UserId", "model.UserId"}, Me.TempData("ErrorMessage"), "alert alert-danger thin mt10", True)
                            <p class="section default">
                                パスワードは<span class="red">大文字、小文字、数字、記号の4種類中の3種類を含む</span>ように入力してください。下記の記号が使用可能です。<br />
                                <span class="small gray">@str</span>
                            </p>
                            <input type="text" id="" name="model.Password" class="form-control mb10" placeholder="パスワード" value="@Me.Model.Password" required="required" style="" autocomplete="off" maxlength="32">
                            @QyHtmlHelper.ToValidationMessageArea({"Password", "model.Password"}, Me.TempData("ErrorMessage"), "alert alert-danger thin mt10", True)
                            <input type="text" id="" name="model.Password2" class="form-control mb10" placeholder="パスワードを再度入力" value="@Me.Model.Password2" required="required" style="" autocomplete="off" maxlength="32">
                            @QyHtmlHelper.ToValidationMessageArea({"Password", "model.Password2"}, Me.TempData("ErrorMessage"), "alert alert-danger thin mt10", True)
                        </label>

                        @<div class="t-row line">
                            <label for="111" class="label-txt"><span class="ico required">必須</span> お名前</label>
                            <input type="text" id="111" name="model.FamilyName" class="form-control mb10" placeholder="姓" value="@Me.Model.FamilyName" required="required" maxlength="25" style="ime-mode:active;" autocomplete="off">
                            <input type="text" id="" name="model.GivenName" class="form-control" placeholder="名" value="@Me.Model.GivenName" required="required" maxlength="25" style="ime-mode:active;" autocomplete="off">
                            @QyHtmlHelper.ToValidationMessageArea({"model.FamilyName", "model.GivenName"}, Me.TempData("ErrorMessage"), "alert alert-danger thin mt10", True)
                        </div>
                        @<div class="t-row line">
                            <label for="222" class="label-txt"><span class="ico required">必須</span> お名前（カナ）</label>
                            <input type="text" id="222" name="model.FamilyKanaName" class="form-control mb10" placeholder="セイ" value="@Me.Model.FamilyKanaName" required="required" maxlength="25" style="ime-mode:active;" autocomplete="off">
                            <input type="text" id="" name="model.GivenKanaName" class="form-control" placeholder="メイ" value="@Me.Model.GivenKanaName" required="required" maxlength="25" style="ime-mode:active;" autocomplete="off">
                            @QyHtmlHelper.ToValidationMessageArea({"model.FamilyKanaName", "model.GivenKanaName"}, Me.TempData("ErrorMessage"), "alert alert-danger thin mt10", True)
                        </div>
                        @<label for="333" class="t-row line">
                            <span class="label-txt"><span class="ico required">必須</span> 性　別</span>
                            @QyHtmlHelper.ToSexDropDownList("333", "model.Sex", Me.Model.Sex.ToString)
                            @QyHtmlHelper.ToValidationMessageArea({"model.Sex"}, Me.TempData("ErrorMessage"), "alert alert-danger thin mt10", True)
                        </label>
                        @<div class="t-row line">
                            <label for="444" class="label-txt"><span class="ico required">必須</span> 生年月日</label>
                            @QyHtmlHelper.ToYearDropDownList("444", "model.BirthYear", Me.Model.BirthYear, "form-control mb10")

                            @QyHtmlHelper.ToMonthDropDownList("", "model.BirthMonth", Me.Model.BirthMonth, "form-control mb10")

                            @QyHtmlHelper.ToDayDropDownList("", "model.BirthDay", Me.Model.BirthDay, "form-control")

                            @QyHtmlHelper.ToValidationMessageArea({"model.BirthYear"}, Me.TempData("ErrorMessage"), "alert alert-danger thin mt10", True)
                        </div>
                        @<label for="mail" class="t-row">
                            <span class="label-txt"><span class="ico required">必須</span> 連絡先メールアドレス</span>
                            <input type="text" id="mail" name="model.MailAddress" class="form-control w-max" value="@Me.Model.MailAddress" required="required" maxlength="256" style="ime-mode:disabled;" autocomplete="off">
                            @QyHtmlHelper.ToValidationMessageArea({"model.MailAddress"}, Me.TempData("ErrorMessage"), "alert alert-danger thin mt10", True)
                        </label>
                        @<div class="submit-area">
                            @*<a href="#" class="display-M-mb10" data-toggle="modal" data-target="#terms"><i class="fa fa-angle-right"></i>利用規約はこちら</a>*@
                            <button class="btn btn-submit display-L-ml5" type="submit" id="submitbutton">利用規約に同意して登録</button>
                        </div>
                    End Using
                </div>
            </div>
            @Html.Action("PortalFooterPartialView", "Portal")
        </div>
    </main>

    <div id='btn-pagetop'><a href='#top'><i class='fa fa-caret-up'></i></a></div>

    @Html.Action("StartRegisterDialogPartialView", "Start")

    <div id="lock-layer"></div>

    @QyHtmlHelper.RenderScriptTag("~/dist/js/start/register")
</body>