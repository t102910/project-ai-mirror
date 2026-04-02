@Imports MGF.QOLMS.QolmsYappli
@ModelType StartRegisterInputModel

@Code
    ViewData("Title") = "アカウント登録"
    Layout = "~/Views/Shared/_StartLayout.vbhtml"
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
                        性別、生年月日は登録後変更できません。<br />
                        <br />
                        Sign In With Apple （Apple IDでサインイン）を使用している場合、
                        Apple IDで登録された名前を使います。<br />
                        名前のよみがな（カナ）は下記のフォームにご記入ください。
                    </p>
                    @Using Html.BeginForm("RegisterResult", "Start", Nothing, FormMethod.Post, New With {.class = "form-inline"})
                        @Html.AntiForgeryToken()

                            @<div class="t-row line @IIf(Me.Model.OpenIdType <> 3 OrElse String.IsNullOrWhiteSpace(Me.Model.FamilyName) OrElse String.IsNullOrWhiteSpace(Me.Model.GivenName), String.Empty, "hide")">
                                <label for="111" class="label-txt"><span class="ico required">必須</span> お名前</label>
                                <input type="text" id="111" name="model.FamilyName" class="form-control mb10" placeholder="姓" value="@Me.Model.FamilyName" required="required" maxlength="25" style="ime-mode:active;" autocomplete="off">
                                <input type="text" id="" name="model.GivenName" class="form-control" placeholder="名" value="@Me.Model.GivenName" required="required" maxlength="25" style="ime-mode:active;" autocomplete="off">
                                @QyHtmlHelper.ToValidationMessageArea({"model.FamilyName", "model.GivenName"}, Me.TempData("ErrorMessage"), "alert alert-danger thin mt10", True)
                            </div>

                        @<div class="t-row line">
                             <label for="222" class="label-txt"><span class="ico required">必須</span> カナ </label>
                            <input type="text" id="222" name="model.FamilyKanaName" class="form-control mb10" placeholder="セイ" value="@Me.Model.FamilyKanaName" required="required" maxlength="25" style="ime-mode:active;" autocomplete="off">
                            <input type="text" id="" name="model.GivenKanaName" class="form-control" placeholder="メイ" value="@Me.Model.GivenKanaName" required="required" maxlength="25" style="ime-mode:active;" autocomplete="off">
                            @QyHtmlHelper.ToValidationMessageArea({"model.FamilyKanaName", "model.GivenKanaName"}, Me.TempData("ErrorMessage"), "alert alert-danger thin mt10", True)
                        </div>

                        @<label for="333" class="t-row line">
                            <span class="label-txt"><span class="ico required">必須</span> 性　別</span>
                            @QyHtmlHelper.ToSexDropDownList("333", "model.Sex", Me.Model.Sex.ToString)
                            @QyHtmlHelper.ToValidationMessageArea({"model.Sex"}, Me.TempData("ErrorMessage"), "alert alert-danger thin mt10", True)
                        </label>
                        @<div class="t-row @IIf(Me.Model.OpenIdType = 3 AndAlso Not String.IsNullOrWhiteSpace(Me.Model.MailAddress), String.Empty, "line")">
                            <label for="444" class="label-txt"><span class="ico required">必須</span> 生年月日</label>
                            @QyHtmlHelper.ToYearDropDownList("444", "model.BirthYear", Me.Model.BirthYear, "form-control mb10")

                            @QyHtmlHelper.ToMonthDropDownList("", "model.BirthMonth", Me.Model.BirthMonth, "form-control mb10")

                            @QyHtmlHelper.ToDayDropDownList("", "model.BirthDay", Me.Model.BirthDay, "form-control")

                            @QyHtmlHelper.ToValidationMessageArea({"model.BirthYear"}, Me.TempData("ErrorMessage"), "alert alert-danger thin mt10", True)
                        </div>
                        @<label for="mail" class="t-row @IIf(Me.Model.OpenIdType = 3 AndAlso Not String.IsNullOrWhiteSpace(Me.Model.MailAddress), "hide", String.Empty)" >
                            <span class="label-txt"><span class="ico required">必須</span> 連絡先メールアドレス</span>
                            <input type="text" id="mail" name="model.MailAddress" class="form-control w-max" value="@Me.Model.MailAddress" required="required" maxlength="256" style="ime-mode:disabled;" autocomplete="off">
                            @QyHtmlHelper.ToValidationMessageArea({"model.MailAddress"}, Me.TempData("ErrorMessage"), "alert alert-danger thin mt10", True)
                        </label>
                        @*
                @<p class="dt-style t-row l-w200 line">
                    <label for="loginid"><span class="ico required">必須</span> 希望ログインID</label>
                    <input type="text" id="loginid" name="model.UserId" class="form-control w-max" value="@Me.Model.UserId" required="required"maxlength="100" style="ime-mode:disabled;" autocomplete="off">
                    @QyHtmlHelper.ToValidationMessageArea({"model.UserId"}, Me.TempData("ErrorMessage"))
                    @QyHtmlHelper.ToValidationMessageArea({"UserId"}, Me.TempData("ErrorMessage"))
                </p>
                @<p class="section">
                パスワードは<span class="red">半角英数記号</span>にて入力してください。下記の文字が使用可能です。<br />
            <span class="small gray">!"#$%&amp;'()*+,-./0123456789&lt;=&gt;?@@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~:;</span><br />
            </p>
                @<p class="dt-style t-row l-w200 line">
                                                <label for="pass"><span class="ico required">必須</span> パスワード</label><!-- input typeはパスワード？ -->
                    <input type="password" id="pass" name="model.Password" class="form-control w-max" value="@Me.Model.Password" required="required" maxlength="32" style="ime-mode:disabled;" autocomplete="off">
                    @QyHtmlHelper.ToValidationMessageArea({"model.Password"}, Me.TempData("ErrorMessage"))
                </p>
                @<p class="dt-style t-row l-w200 line">
                                                <label for="pass"><span class="ico required">必須</span> パスワード（確認）</label><!-- input typeはパスワード？ -->
                    <input type="password" id="pass" name="model.Password2" class="form-control w-max" value="@Me.Model.Password2" required="required" maxlength="32" style="ime-mode:disabled;" autocomplete="off">
                    @QyHtmlHelper.ToValidationMessageArea({"model.Password2"}, Me.TempData("ErrorMessage"))
                </p>
                        *@
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