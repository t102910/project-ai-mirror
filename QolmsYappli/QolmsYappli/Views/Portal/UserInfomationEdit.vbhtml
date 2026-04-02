@Imports MGF.QOLMS.QolmsYappli
@ModelType PortalUserInfomationEditInputModel

@Code
    ViewData("Title") = "ユーザー情報編集"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"
End Code

<body id="info" class="lower">

    @Html.AntiForgeryToken()

    <main id="main-cont" class="clearfix" role="main">
 
                <section class="home-btn-wrap type2" data-pageno="@Convert.ToByte(Me.Model.FromPageNoType)">
                    <a class="home-btn type-2" href="../Portal/userinfomation"><i class="la la-angle-left"></i><span> 戻る</span></a>
                </section>
        <section class="contents-area mb100">

            <h2 class="title">@ViewData("Title")</h2>
            <hr>

            <div>
                <section class="section default">
                    ユーザー情報を編集します。<br />

                </section>

                @*入力*@
                <h3 class="title mt10">
                    <span>ユーザー情報</span>
                </h3>
                
                <div class="form wizard-form mb30">

                    <div class="t-row line">
                        <label for="name" class="label-txt"><span class="ico required">必須</span> お名前</label>
                        <input type="text" id="family-name" name="model.FamilyName" class="form-control mb10" placeholder="姓" value="@Me.Model.FamilyName" required="required" maxlength="25" style="ime-mode:active;" autocomplete="off">
                        <input type="text" id="given-name" name="model.GivenName" class="form-control" placeholder="名" value="@Me.Model.GivenName" required="required" maxlength="25" style="ime-mode:active;" autocomplete="off">
                        <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                    </div>

                    <div class="t-row line">
                        <label for="222" class="label-txt"><span class="ico required">必須</span> お名前（カナ）</label>
                        <input type="text" id="family-kana-name" name="model.FamilyKanaName" class="form-control mb10 disabled" placeholder="セイ" value="@Me.Model.FamilyKanaName" required="required" maxlength="25" style="ime-mode:active;" autocomplete="off">
                        <input type="text" id="given-kana-name" name="model.GivenKanaName" class="form-control" placeholder="メイ" value="@Me.Model.GivenKanaName" required="required" maxlength="25" style="ime-mode:active;" autocomplete="off">
                        <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                    </div>

                    <label for="sex" class="t-row line">
                        <span class="label-txt">
                            性 別
                        </span>
                        <p id="sex" name="model.SexType" style="font-size:16px; padding:10px 12px;" data-value="@Me.Model.SexType.ToString()">@QyDictionary.SexType(Me.Model.SexType)</p>

                    </label>

                    <div class="t-row line">
                        <label for="birth-year" class="label-txt">
                            生年月日
                        </label>
                        <p id="birthday" name="model.SexType" style="font-size:16px; padding:10px 12px;" data-birthyear="@Me.Model.BirthYear" data-birthmonth="@Me.Model.BirthMonth" data-birthday="@Me.Model.BirthDay">@String.Format("{0}年 {1}月 {2}日", Me.Model.BirthYear, Me.Model.BirthMonth, Me.Model.BirthDay) </p>

                        <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>

                    </div>

                    <div class="t-row line">

                        <label for="mail" class="">
                            <span class="label-txt"><span class="ico required">必須</span> 連絡先メールアドレス</span>
                            <input type="text" id="mail" name="model.MailAddress" class="form-control w-max" value="@Me.Model.MailAddress" required="required" maxlength="256" style="ime-mode:disabled;" autocomplete="off">
                            <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                        </label>
                    </div>


                    <div class="t-row line">

                        <label for="prefectures-radio" class="form-wrap">
                            <span class="label-txt">
                                <span class="ico required">必須</span>

                                居住地都道府県
                            </span>
                            <div id="prefectures" name="prefectures" class="mb10 toggle-btn-set two-btn">
                                <p>
                                    <input type="radio" name="model.Prefectures" id="prefectures-okinawa" value="47" @IIf(Me.Model.Prefectures <> 0, "checked", String.Empty)>
                                    <label class="button-style-label" for="prefectures-okinawa">沖縄県</label>
                                </p>
                                <p>
                                    <input type="radio" name="model.Prefectures" id="prefectures-other" value="0" @IIf(Me.Model.Prefectures = 0, "checked", String.Empty)>
                                    <label class="button-style-label" for="prefectures-other">沖縄県外</label>
                                </p>
                            </div>
                        </label>
                    </div>
                    <div class="t-row line">

                        <label for="city" class="form-wrap">
                            <span class="label-txt">
                                <span class="ico na">任意</span>

                                市町村
                            </span>
                            <select id="city" name="model.CityNo" class="form-control" required="required">
                                <option value="0">選択してください</option>

                                @For Each item As CityItem In Me.Model.CityItemN

                                    @<option value="@item.CityNo" @IIf(Me.Model.CityNo = item.CityNo, "selected", String.Empty)> @item.CityName</option>

                                Next
                            </select>
                            <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>

                        </label>
                        <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                    </div>

                    <label for="phone" Class="t-row form-wrap ">
                        <Span Class="label-txt">
                            <Span Class="ico na">任意</Span>
                            電話番号
                        </Span>
                        <input type="text" id="phone" name="model.PhoneNo" Class="form-control w-max" value="@Me.Model.PhoneNo" required="required" maxlength="12" style="ime-mode:disabled;" autocomplete="off">
                        <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>

                    </label>

                    <p Class="submit-area mb30">
                        <a href="javascript:void(0);" id="reconnection" Class="btn btn-submit">更 新</a>
                    </p>
                </div>

                </div>
        </section>
        <!-- 「個人情報更新の確認」ダイアログ -->
        <div Class="modal fade" id="identity-modal" tabindex="-1" data-backdrop="static" data-keyboard="false" data-row-no="-1">
            <div Class="modal-dialog">
                <div Class="modal-content">
                    <div Class="modal-header">
                        <Button type = "button" Class="close"><span>×</span></button>
                        <h4 Class="modal-title">更新の確認</h4>
                    </div>
                    <div Class="modal-body">
                        入力された個人情報が登録と一致しませんでした。個人情報更新しますか？
                    </div>
                    <div Class="modal-footer">
                        <Button type = "button" Class="btn btn-close no-ico mb0">閉じる</button>
                        <Button type = "button" Class="btn btn-submit">更 新</button>
                    </div>
                </div>
            </div>
        </div>
    </main>

    @Html.Action("PortalFooterPartialView", "Portal")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/portal/userinfomationedit")
</body>