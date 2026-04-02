@Imports MGF.QOLMS.QolmsYappli
@ModelType StartLoginEditInputModel

@Code
    ViewData("Title") = "ログイン情報の編集"
    Layout = "~/Views/Shared/_StartLayout.vbhtml"

    Dim userIdRegister As String = String.Empty

    If String.IsNullOrWhiteSpace(Me.Model.UserId) Then
        userIdRegister = "unregistered"
    Else
        userIdRegister = "registered"

    End If

    Dim openIdRegister As String = String.Empty

    If String.IsNullOrWhiteSpace(Me.Model.OpenId) Then
        openIdRegister = "unregistered"
    Else
        openIdRegister = "registered"

    End If

    Dim str As String = "!#$%&()*+,-./<=>?@[\]^_{​​​​​​​​|}​​​​​​​​​~:;"

    Dim OpenIdResult As Boolean = Me.TempData("ResultDispFlag")
    Dim OpenIdResultMessage As String = String.Empty
    If Me.TempData("Result") Then

        OpenIdResultMessage = "auIDを登録しました。"
    Else
        OpenIdResultMessage = Me.TempData("Message")
    End If

    'End Select
End Code


<body id="login-setting" class="lower">
    @Html.AntiForgeryToken()

    <main id="main-cont" class="clearfix" role="main">

        <section class="contents-area mb0">
            <h2 class="title pt0">ログイン情報</h2>
            <hr>
        </section>
        <section class="data-area">
            @*	    <section class="home-btn-wrap type-2">
                    <a href="../Portal/Home" class="home-btn"><i class="la la-home"></i><span> ホーム</span></a>
                </section>
                <section class="contents-area mb0">
                    <h2 class="title pt0">ログイン情報</h2>
                    <hr>
                </section>
                <section class="data-area">
                    <div class="page-info">
                        <p class="baloon">
                            もしもの時のために、ログイン方法を複数登録しておきましょう！
                        </p>
                        <p class="image"><img src="/dist/img/tmpl/cheering.png" alt="" /></p>
                    </div>
                    <h2 class="section-title">あなたのログイン情報</h2>
                    <div class="regist-btn-group">
                        <a href="#id-pass" class="id-pass @userIdRegister"><!--登録なしなら.unregistered -->
                            <span>JOTO ID・パスワードでログイン</span>
                            <p class="image"></p>
                        </a>
                        <a href="#au-id" class="au-id @openIdRegister"><!--登録ありなら.registered -->
                            <span>AU IDでログイン</span>
                            <p class="image"></p>
                        </a>
                    </div>*@
            <div id="id-pass" class="box @userIdRegister">
                <!--登録なしなら.unregistered -->
                <section class="inner">
                    <h3 class="title">JOTOID・パスワードでログイン</h3>
                    @If userIdRegister = "registered" Then

                        @<h4 class="title">あなたのID<small>（IDは変更できません）</small></h4>
                        @<span class="font-l type-2">@Me.Model.UserId</span>
                        @<hr class="mb30">
                        @<h4 class="title">パスワードを変更する</h4>
                    Else

                        @<input type="text" id="userid" name="model.UserId" class="form-control mb10" placeholder="ID（携帯電話番号もしくはメールアドレス）" value="" required="required" maxlength="100" autocomplete="off">
                        @QyHtmlHelper.ToValidationMessageArea({"UserId", "model.UserId"}, Me.TempData("ErrorMessage"), "UserId alert alert-danger thin mt10", True)
                    End If

                    <p class="section default">
                        パスワードは<span class="red">大文字、小文字、数字、記号の4種類中の3種類を含む</span>ように入力してください。下記の記号が使用可能です。<br />
                        <span class="small gray">@str</span>
                    </p>

                    <input type="text" id="password" class="form-control mb15" placeholder="新しいパスワードを入力" required="required" maxlength="32" autocomplete="off">
                    <input type="text" id="password2" class="form-control mb20" placeholder="新しいパスワードを入力（再入力）" required="required" maxlength="32" autocomplete="off">
                    @QyHtmlHelper.ToValidationMessageArea({"Password", "model.Password"}, Me.TempData("ErrorMessage"), "Password alert alert-danger thin mt10", True)
                    <div class="center">
                        <button id="submit" class="btn btn-submit w-80p">変更</button>
                    </div>
                </section>
            </div>
            @If openIdRegister = "registered" Then

                @<div id="au-id" class="box registered">
                    <!--登録ありなら.registered -->
                    <section class="inner">
                        <h3 class="title">au IDでログイン</h3>
                        <p>登録済み</p>
                        <p class="center"><a class="auid-login"><i></i><span>登録済み</span></a></p>
                    </section>
                </div>

            Else

                @<div id="au-id" class="box unregistered">
                    <!--登録ありなら.registered -->
                    <section class="inner">
                        <h3 class="title">au IDでログイン</h3>
                        <p>au ID情報を変更するには下のボタンをタップしてください</p>
                        <p class="center"><a id="auid-login" href="" class="auid-login"><i></i><span>au IDでログイン</span></a></p>
                    </section>
                </div>
            End If
        </section>

        <div class="modal fade" id="message-modal" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal"><span>×</span></button>
                        <h4 class="modal-title"></h4>
                    </div>
                    <div class="modal-body">
                        変更しました。
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-close" data-dismiss="modal">閉じる</button>
                    </div>
                </div>
            </div>
        </div>


        @If OpenIdResult Then

            @<div class="modal fade" id="opneid-message-modal" tabindex="-1">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal"><span>×</span></button>
                            <h4 class="modal-title"></h4>
                        </div>
                        <div class="modal-body">
                            @OpenIdResultMessage
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-close" data-dismiss="modal">閉じる</button>
                        </div>
                    </div>
                </div>
            </div>
        End If

    </main>

    @Html.Action("PortalFooterPartialView", "Portal")
    @QyHtmlHelper.RenderScriptTag("~/dist/js/start/loginedit")

</body>


