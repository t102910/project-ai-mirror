@Imports MGF.QOLMS.Yappli
@modelType loginmodel

@Code
    Me.ViewData("Title") = "ログイン"
    Layout = "~/Views/Shared/_StartLayout.vbhtml"
    Dim bShowNewAccount As Boolean = True
    Dim param As String = Request.Params("ReturnUrl")
    If String.IsNullOrEmpty(param) = False AndAlso param.ToLower().Contains("datauploaderauth") Then
        bShowNewAccount = False
    End If

End Code

<body id="login" class="before-login">
    
    <main id="main-cont" class="clearfix" role="main">
        <h1><img src="/dist/img/tmpl/joto-logo.png"></h1>
        <h2 class="section-title">ログイン（アカウントをお持ちの方）</h2>
        <div class="box mb10">
            <section class="inner">
                <h3 class="title">au IDでログイン</h3>
                <p class="center"><a id="auid-login" @*href="/2.html"*@ class="auid-login" data-autologin="@Me.TempData("IsAuAutoLogin")"><i></i><span>au IDでログイン</span></a></p>
                <p style="padding: 0px 20px 10px;">Pontaポイント、auデータチャージのご利用などau関連サービスをご利用の方はauIDでのログインを推奨します。</p>

                <h3 class="title">Apple IDでサインイン</h3>
                <p class="center"><a id="appleid-login" class="appleid-login"><i class="la la-apple"></i><span>Apple IDでサインイン</span></a></p>
            </section>
        </div>

        <h2 class="center mb10" style="font-size:1.2em;">または</h2>

        <div class="box">
            <section class="inner">

                <h3 class="title">JOTO ID・パスワードでログイン</h3>
                @Using Html.BeginForm("LoginJotoIdResult", "Start", Nothing, FormMethod.Post, Nothing)
                    @Html.AntiForgeryToken()

                    @<input type="text" id="joto-userid" name="userid" class="form-control mb15" value="@Me.Model.UserId" placeholder="ID（携帯電話番号もしくはメールアドレス）">

                    @<div Class="password-view-change">
                        <input type="password" id="joto-password" name="password" Class="form-control " value="" placeholder="パスワード">
                        <p Class="password-view-change-wrap">
                            <input type="checkbox" id="password-open" name="password-open" Class="mb20" />
                            <Label Class="password-open-btn eye-close" for="password-open"></Label>
                        </p>
                    </div>

                    If Not String.IsNullOrWhiteSpace(Me.Model.Message) Then
                        @<div class="alert alert-danger">@Me.Model.Message</div>
                    End If

                    @<div class="center">
                        <button id="joto-id-login" class="btn btn-submit w-80p mb20" type="button">ログイン</button>
                        @*<p><a href="">ID・パスワードを忘れた方</a></p>*@
                    </div>

                End Using

            </section>
        </div>
        <p class="center mb20">
            <a href="~/Start/registerid">
                <strong>アカウントを新規作成</strong><br>（はじめてログインする方はこちら）
            </a>
        </p>
        <p class="center mb20">
            <a href="~/PasswordReset/SelectMethod">
                <strong>JOTO ID のパスワードを忘れた方</strong>
            </a>
        </p>
        <h2 class="section-title">お知らせ</h2>
        <div class="box">
            <section class="inner">

                <section class="section caution">
                    <h4>JOTOメンテナンスのお知らせ</h4>
                    2026/1/28（水） 22:00～23:00 の間、メンテナンスを行います。ご迷惑をおかけしますが、ご理解とご協力をお願い申し上げます。
                    <p class="date">2026/1/21（水）</p>
                </section>

                <section class="section caution">
                    <h4>KDDIポイントシステムメンテナンス</h4>
                    2026/1/27（火）～2026/1/30（金）の日程で 00:00 ～ 06:00 の間、KDDIポイントシステムメンテナンスのためPontaポイントの交換のご利用ができません。ご迷惑をおかけしますが、何卒ご理解の程よろしくお願い致します。
                    <p class="date">2026/1/21（水）</p>
                </section>

                <div class="balloon-2 no-wrap">
                    <h3>お知らせ</h3>
                    <p class="wrap">
                        アプリがリニューアルしました。​<br />
                        食事・運動・バイタルが登録しやすくなり、​その日に摂取できるカロリーが一目で分かるようになりました。
                    </p>
                </div>
                <p class="center pt10 mb20"><img class="shika" src="/dist/img/tmpl/s-5.png"><img class="shika" src="/dist/img/tmpl/s-6.png"></p>
            </section>
        </div>
        @If HttpContext.Current.IsDebuggingEnabled = True Then
            @<div class="box">
                <section class="inner">
                    @Using Html.BeginForm("LoginResult", "Start", Nothing, FormMethod.Post, Nothing)
                        @Html.AntiForgeryToken()

                        @<h3 class="title">QOLMS IDでログイン(＊テスト用です・リリース前に削除します)</h3>
                        @<p class="form-wrap">
                            <input type="text" id="userid" name="userid" class="form-control" value="@Me.Model.UserId" placeholder="QOLMS IDを入力">
                        </p>
                        @<p class="form-wrap">
                            <input type="password" id="password" name="password" class="form-control" value="@Me.Model.Password" placeholder="パスワードを入力">
                        </p>

                        @If Not String.IsNullOrWhiteSpace(Me.Model.Message) Then
                            @<div class="alert alert-danger">@Me.Model.Message</div>
                        End If

                        @<div class="submit-area">
                            <button class="btn btn-submit" type="submit">ログイン</button>

                            @*<p class="left"><a href=""><i class="la la-user-plus"></i> QOLMS新規アカウント登録</a></p>*@
                        </div>
                    End Using
                </section>
            </div>
        End If

        @If bShowNewAccount Then
            @<p class=""><a href="native:/tab/bio/dde3b934"><i class="la la-angle-right"></i> 利用規約</a></p>
            @<p class=""><a href="native:/tab/bio/0a078204"><i class="la la-angle-right"></i> プライバシーポリシー</a></p>
        End If

        @If Me.TempData("IsConsent") IsNot Nothing AndAlso Me.TempData("IsConsent") Then
            @<div class="modal fade" id="agreement-check-modal" tabindex="-1" data-backdrop="static" data-keyboard="false" data-agreement="@Me.TempData("AgreementVersion")">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close"><span>×</span></button>
                            <h4 class="modal-title">利用規約変更のお知らせ</h4>
                        </div>
                        <div class="modal-body">
                            <p class="">
                                平素よりJOTOホームドクターをご利用いただき、ありがとうございます。<br />
                                このたび、「JOTOホームドクター利用規約」の内容を一部改訂致します。<br />

                                JOTOホームドクターをご利用いただくためには、利用規約への同意が必要です。<br />
                                改定後の利用規約のご確認とご同意の上、JOTOホームドクターをご利用ください。<br />
                            </p>
                            <p class=""><a href="native:/tab/bio/dde3b934"><i class="la la-angle-right"></i> 利用規約</a></p>
                            <p class="">令和6年12月05日改訂</p>
                            <p class=""><a href="native:/tab/bio/0a078204"><i class="la la-angle-right"></i> プライバシーポリシー</a></p>
                            <p class="">令和5年11月6日改訂</p>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-submit no-ico mb0">同意する</button>
                            <button type="button" class="btn btn-close no-ico mb0">同意しない</button>
                        </div>
                    </div>
                </div>
            </div>
        End If

    </main>


    @Html.Action("PortalFooterPartialView", "Portal")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/start/loginbyid")

</body>