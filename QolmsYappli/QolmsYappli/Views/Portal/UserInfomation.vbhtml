@Imports MGF.QOLMS.QolmsYappli
@ModelType PortalUserInfomationInputModel

@Code
    ViewData("Title") = "アカウント情報"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"
End Code

<body id="gulf-cont" class="lower gulf">

    @Html.AntiForgeryToken()

    <main id="main-cont" class="clearfix" role="main">

        <section class="contents-area mb100">

            <h2 class="title">@ViewData("Title")</h2>
            <hr>

            <div>
                <section class="section default">
                    アカウント情報を確認できます<br />

                </section>

                @*入力*@
                <h3 class="title mt10">
                    <span>登録情報</span>
                </h3>

                <div class="form wizard-form mb30">


                    <label for="name" class="t-row line">
                        <span class="label-txt">
                            <span class="ico required">必須</span>
                            お名前
                        </span>
                        @Code
                            Dim loginType As String = String.Empty

                            Select Case True
                                Case Not String.IsNullOrWhiteSpace(Me.Model.JotoId) AndAlso Not String.IsNullOrWhiteSpace(Me.Model.OpenId)
                                    '両方ある場合
                                    loginType = "JOTO ID / 外部 ID 連携"
                                Case Not String.IsNullOrWhiteSpace(Me.Model.JotoId)
                                    'JOTOIDのみ
                                    loginType = "JOTO ID"

                                Case Not String.IsNullOrWhiteSpace(Me.Model.OpenId)
                                    'OpenIdのみ
                                    loginType = "外部 ID 連携"

                            End Select


                        End Code

                        <p id="name" name="" style="font-size:16px; padding:10px;" data-value=''>@String.Format("ログイン種別 : {0}", loginType)</p>
                        @If Not String.IsNullOrWhiteSpace(Me.Model.JotoId) Then
                            'JOTOID
                            @<p id="name" name="" style="font-size:16px; padding:10px;" data-value='@Me.Model.JotoId'> @String.Format("ログインID : {0}", Me.Model.JotoId)</p>
                        End If

                    </label>

                    <label for="name" class="t-row line">
                        <span class="label-txt">
                            <span class="ico required">必須</span>
                            お名前
                        </span>
                        <p id="name" name="" style="font-size:16px; padding:10px 12px;" data-value='@String.Format("{0} {1}", Me.Model.FamilyName, Me.Model.GivenName)'>@String.Format("{0} {1}", Me.Model.FamilyName, Me.Model.GivenName)</p>

                    </label>

                    <label for="kananame" class="t-row line">
                        <span class="label-txt">
                            <span class="ico required">必須</span>
                            お名前(カナ)
                        </span>
                        <p id="kananame" name="" style="font-size:16px; padding:10px 12px;" data-value='@String.Format("{0} {1}", Me.Model.FamilyKanaName, Me.Model.GivenKanaName)'>@String.Format("{0} {1}", Me.Model.FamilyKanaName, Me.Model.GivenKanaName)</p>

                    </label>

                    <label for="sex" class="t-row line">
                        <span class="label-txt">
                            <span class="ico required">必須</span>
                            性 別
                        </span>
                        <p id="sex" name="model.SexType" style="font-size:16px; padding:10px 12px;" data-value="@Me.Model.SexType.ToString()">@QyDictionary.SexType(Me.Model.SexType)</p>

                    </label>

                    <div class="t-row line">
                        <label for="birth-year" class="label-txt">
                            <span class="ico required">必須</span>
                            生年月日
                        </label>
                        <p id="birthday" name="model.SexType" style="font-size:16px; padding:10px 12px;" data-birthyear="@Me.Model.BirthYear" data-birthmonth="@Me.Model.BirthMonth" data-birthday="@Me.Model.BirthDay">@String.Format("{0}年 {1}月 {2}日", Me.Model.BirthYear, Me.Model.BirthMonth, Me.Model.BirthDay) </p>

                        <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                    </div>

                    <label for="mailaddress" class="t-row line">
                        <span class="label-txt">
                            <span class="ico required">必須</span>
                            連絡先メールアドレス
                        </span>
                        <p id="mailaddress" name="" style="font-size:16px; padding:10px 12px;" data-value='@Me.Model.MailAddress'>@Me.Model.MailAddress</p>

                    </label>

                    <label for="prefectures-radio" class="t-row line form-wrap">
                        <span class="label-txt">
                            <span class="ico required">必須</span>

                            居住地都道府県
                        </span>
                        <div id="prefectures" name="prefectures" class="mb10 ">
                            <p id="mailaddress" name="" style="font-size:16px; padding:10px 12px;" data-value='@Me.Model.Prefectures'>@IIf(Me.Model.Prefectures = 47, "沖縄県", "沖縄県外").ToString()</p>

                        </div>
                    </label>

                    <label for="city" Class="t-row form-wrap">
                        <Span Class="label-txt">
                            <Span Class="ico na">任意</Span>

                            市町村
                        </Span>
                        @Code
                            Dim list As List(Of String) = Me.Model.CityItemN.Where(Function(i) i.CityNo = Me.Model.CityNo).Select(Function(j) j.CityName).ToList()
                            Dim CityName As String = String.Empty
                            If list.Any() Then
                                CityName = list.First()

                            Else
                                CityName = "未選択"

                            End If
                        End Code
                        <p id="mailaddress" name="" style="font-size:16px; padding:10px 12px;" data-value='@Me.Model.CityNo'>@CityName</p>

                    </label>

                    <Label for="phone" class="t-row form-wrap ">
                        <Span Class="label-txt">
                            <Span Class="ico na">任意</Span>
                            電話番号
                        </Span>
                        <p id="mailaddress" name="" style="font-size:16px; padding:10px 12px;" data-value='@Me.Model.PhoneNo'>@Me.Model.PhoneNo</p>
                    </Label>
                   
                    @*<Label for="phone" class="t-row line form-wrap ">
            <Span Class="label-txt">
                <span class="ico required">必須</span>
                電話番号
            </Span>
            <p id="mailaddress" name="" style="font-size:16px; padding:10px 12px;" data-value='@Me.Model.PhoneNo'>@Me.Model.PhoneNo</p>
            <a href="../portal/SMSAuthentication" id="" Class="btn btn-submit">SMS 認証</a>

        </Label>*@


                    <p Class="submit-area mb30">
                        <a href="../Portal/UserInfomationEdit" id="edit" Class="btn btn-submit">編 集</a>
                    </p>
                </div>

            </div>
        </section>

    </main>

    @Html.Action("PortalFooterPartialView", "Portal")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/portal/userinfomation")
</body>
