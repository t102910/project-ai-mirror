@Imports MGF.QOLMS.QolmsYappli
@ModelType PortalTargetSettingInputModel2

@Code
    ViewData("Title") = "目標設定"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"

    Dim height As Decimal = Decimal.MinValue
    Decimal.TryParse(Me.Model.Height, height)

    Dim weight As Decimal = Decimal.MinValue
    Decimal.TryParse(Me.Model.Weight, weight)

    Dim targetWeight As Decimal = Decimal.MinValue
    Decimal.TryParse(Me.Model.TargetWeight, targetWeight)

    Dim isYappli As Boolean = Me.Context.Request.UserAgent.ToLower().Contains("yappli")
    Dim tabId As Integer = If(Me.Model.TabId.CompareTo("input-2") = 0, 1, 0)
End Code

<body id="info" class="lower">
    @Html.AntiForgeryToken()
    @*@Html.Action("PortalHeaderPartialView", "Portal")*@

    <main id="main-cont" class="clearfix" role="main">
        <section class="contents-area">
            <ul class="nav nav-tabs mb20">
                <li class="input-1 @IIf(tabId = 0, "active", String.Empty)"><a href="#input-1" data-toggle="tab"><span>カロリー目標</span></a></li>
                <li class="input-2 @IIf(tabId = 1, "active", String.Empty)"><a href="#input-2" data-toggle="tab"><span>バイタル目標</span></a></li>
            </ul>
            <div class="tab-content">
                <div id="input-1" class="tab-pane fade in @IIf(tabId = 0, "active", String.Empty)">
                    <div>
                        <h3 class="title mt10">身長</h3>
                        <label for="height" class="t-row line">
                            <div class="input-group datetime-select" data-defalt="@IIf(height <= 0, String.Empty, height.ToString("#.0"))">
                                <input type="number" id="height" name="height" class="form-control" value="@IIf(height <= 0, String.Empty, height.ToString("#.0"))" placeholder="身長を入力" maxlength="5" style="ime-mode:disabled;" autocomplete="off">
                                <span class="input-group-addon">cm</span>
                            </div>
                            <section id="caution-height" class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                        </label>

                        <h3 class="title mt10">体重</h3>
                        <label for="weight" class="t-row line">
                            <div class="input-group datetime-select" data-defalt="@IIf(weight <= 0, String.Empty, weight.ToString("#.0"))">@**@
                                <input type="number" id="weight" name="weight" class="form-control" value="@IIf(weight <= 0, String.Empty,weight.ToString("#.0"))" placeholder="体重を入力" maxlength="5" style="ime-mode:disabled;" autocomplete="off">
                                <span class="input-group-addon">kg</span>
                            </div>
                            <section id="caution-weight" class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                        </label>
                    </div>

                    <div id="momentum" class="t-row" data-defalt="@Me.Model.PhysicalActivityLevel">
                        <h3 class="title mt10">あなたの運動量</h3>
                        <label>
                            <span></span>
                            <span>運動量</span>
                            <span>１日の過ごし方</span>
                        </label>
                        <label for="act1">
                            <span><input type="radio" id="act1" name="act" value="1" @Html.Raw(IIf(Me.Model.PhysicalActivityLevel = 1, "checked='checked'", String.Empty))></span>
                            <span>少ない人</span>
                            <span>デスクワーク中心</span>
                        </label>
                        <label for="act2">
                            <span><input type="radio" id="act2" name="act" value="2" @Html.Raw(IIf(Me.Model.PhysicalActivityLevel = 2, "checked='checked'", String.Empty))></span>
                            <span>普通の人</span>
                            <span>立ち仕事中心</span>
                        </label>
                        <label for="act3">
                            <span><input type="radio" id="act3" name="act" value="3" @Html.Raw(IIf(Me.Model.PhysicalActivityLevel = 3, "checked='checked'", String.Empty)) "></span>
                            <span>多めの人</span>
                            <span>力仕事中心<span>
                        </label>
                        <section id="caution-physicalactivitylevel" class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                    </div>

                    <h3 class="title mt10">カロリー目標</h3>
                    <h4 class="">目標体重</h4>
                    <label class="t-row line">
                        <div class="input-group datetime-select mb5">
                            <input type="number" id="target-weight" name="val3" class="form-control" value="@IIf(targetWeight <= 0, String.Empty, targetWeight.ToString("#.0"))" placeholder="目標体重を入力" maxlength="5" style="ime-mode:disabled;" autocomplete="off">
                            <span class="input-group-addon">kg</span>
                        </div>
                        <section id="caution-targetweight" class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                    </label>
                    <h4 class="">いつまでに</h4>
                    <label class="t-row">
                        <div class="input-group datetime-select mb5">
                            <span class="input-group-addon">期限日</span>
                            <input type="text" id="target-date" name="val4" class="form-control picker" value="@String.Format("{0:yyyy年MM月dd日}", IIf(Me.Model.TargetDate = Date.MinValue, Date.Now.AddMonths(1), Me.Model.TargetDate))" readonly="readonly" style="background-color:white;" autocomplete="off">
                        </div>
                        <section id="caution-targetdate" class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                    </label>
                    <div class="submit-area hide">
                        <a href="javascript:void(0);" class="btn btn-submit" id="calc">カロリー目標を計算</a>
                    </div>

                    <h4 class="hide">目標とする「摂取カロリー」</h4>
                    <label class="t-row">
                        <div class="input-group datetime-select mb5">
                            <input id="target-caloriein" type="tel" name="val1" class="form-control hide" value="@Me.Model.TargetValue1" placeholder="カロリーを入力" maxlength="4" style="ime-mode:disabled;" autocomplete="off">
                            <span class="input-group-addon hide">kcal</span>
                        </div>
                        <span class="small hide" id="NowBasalMetabolism-target">@String.Format("あなたの基礎代謝量は{0:#,###}kcalです！", Me.Model.NowBasalMetabolism)</span>
                        <section id="caution-targetvalue1" class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                    </label>

                    <input id="target-calorieout" type="tel" name="val2" class="form-control hide" value="@Me.Model.TargetValue2" placeholder="カロリーを入力" maxlength="4" style="ime-mode:disabled;" autocomplete="off">

                    <div class="submit-area" style="margin-bottom:150px;">
                        @If Not isYappli Then
                            @<a href="../Portal/Information" class="btn btn-close no-ico">戻 る</a>
                        End If
                        <a href="javascript:void(0);" class="btn btn-submit btn-submit-regist">登 録</a>
                    </div>
                    <section id="caution1" class="section caution mt10 mb0 other-caution hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                </div>

                <div id="input-2" class="tab-pane fade in @IIf(tabId = 1, "active", String.Empty)">
                    <h3 class="title mt10">歩数目標を設定</h3>
                    <div class="input-group mb10">
                        <span class="input-group-addon">上限目標</span>
                        <input type="tel" name="val3" class="form-control" value="@Me.Model.TargetValue3" placeholder="歩数を入力" maxlength="6" style="ime-mode:disabled;" autocomplete="off">
                        <span class="input-group-addon">歩</span>
                    </div>
                    <div class="input-group mb5">
                        <span class="input-group-addon">下限目標</span>
                        <input type="tel" name="val4" class="form-control" value="@Me.Model.TargetValue4" placeholder="歩数を入力" maxlength="6" style="ime-mode:disabled;" autocomplete="off">
                        <span class="input-group-addon">歩</span>
                    </div>
                    <section id="caution-targetvalue4" class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                    <p class="mb30 center standard" data-vital-type="@QyVitalTypeEnum.Steps">
                        <a href="javascript:void(0);">「歩数」に平均的な目標を設定する</a>
                    </p>

                    <h3 class="title mt10">体重目標を設定</h3>
                    <div class="input-group mb10">
                        <span class="input-group-addon">上限目標</span>
                        <input type="number" name="val5" class="form-control" value="@Me.Model.TargetValue5" placeholder="体重を入力" maxlength="11" style="ime-mode:disabled;" autocomplete="off">
                        <span class="input-group-addon">kg</span>
                    </div>
                    <div class="input-group mb5">
                        <span class="input-group-addon">下限目標</span>
                        <input type="number" name="val6" class="form-control" value="@Me.Model.TargetValue6" placeholder="体重を入力" maxlength="11" style="ime-mode:disabled;" autocomplete="off">
                        <span class="input-group-addon">kg</span>
                    </div>
                    <section id="caution-targetvalue6" class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                    <p class="mb30 center standard" data-vital-type="@QyVitalTypeEnum.BodyWeight">
                        <a href="javascript:void(0);">「体重」に平均的な目標を設定する</a>
                    </p>

                    <h3 class="title mt10">血圧目標を設定</h3>
                    <div class="border-bottom">
                        <div class="input-group mb10">
                            <span class="input-group-addon">【上】上限目標</span>
                            <input type="tel" name="val7" class="form-control" value="@Me.Model.TargetValue7" placeholder="血圧を入力" maxlength="11" style="ime-mode:disabled;" autocomplete="off">
                            <span class="input-group-addon">mmHg</span>
                        </div>
                        <div class="input-group mb10">
                            <span class="input-group-addon">【上】下限目標</span>
                            <input type="tel" name="val8" class="form-control" value="@Me.Model.TargetValue8" placeholder="血圧を入力" maxlength="11" style="ime-mode:disabled;" autocomplete="off">
                            <span class="input-group-addon">mmHg</span>
                        </div>
                        <section id="caution-targetvalue8" class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                    </div>
                    <div>
                        <div class="input-group mb10 pt10">
                            <span class="input-group-addon">【下】上限目標</span>
                            <input type="tel" name="val9" class="form-control" value="@Me.Model.TargetValue9" placeholder="血圧を入力" maxlength="11" style="ime-mode:disabled;" autocomplete="off">
                            <span class="input-group-addon">mmHg</span>
                        </div>
                        <div class="input-group mb10">
                            <span class="input-group-addon">【下】下限目標</span>
                            <input type="tel" name="val10" class="form-control" value="@Me.Model.TargetValue10" placeholder="血圧を入力" maxlength="11" style="ime-mode:disabled;" autocomplete="off">
                            <span class="input-group-addon">mmHg</span>
                        </div>
                        <section id="caution-targetvalue10" class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                    </div>
                    <p class="mb30 center standard" data-vital-type="@QyVitalTypeEnum.BloodPressure">
                        <a href="javascript:void(0);">「血圧」に平均的な目標を設定する</a>
                    </p>

                    <h3 class="title mt10">血糖値目標を設定</h3>
                    <div class="border-bottom">
                        <div class="input-group mb10">
                            <span class="input-group-addon">【空腹時】上限目標</span>
                            <input type="tel" name="val11" class="form-control" value="@Me.Model.TargetValue11" placeholder="血糖値を入力" maxlength="11" style="ime-mode:disabled;" autocomplete="off">
                            <span class="input-group-addon">mg/dl</span>
                        </div>
                        <div class="input-group mb10">
                            <span class="input-group-addon">【空腹時】下限目標</span>
                            <input type="tel" name="val12" class="form-control" value="@Me.Model.TargetValue12" placeholder="血糖値を入力" maxlength="11" style="ime-mode:disabled;" autocomplete="off">
                            <span class="input-group-addon">mg/dl</span>
                        </div>
                        <section id="caution-targetvalue12" class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                    </div>
                    <div>
                        <div class="input-group mb10 pt10">
                            <span class="input-group-addon">【その他】上限目標</span>
                            <input type="tel" name="val13" class="form-control" value="@Me.Model.TargetValue13" placeholder="血糖値を入力" maxlength="11" style="ime-mode:disabled;" autocomplete="off">
                            <span class="input-group-addon">mg/dl</span>
                        </div>
                        <div class="input-group mb10">
                            <span class="input-group-addon">【その他】下限目標</span>
                            <input type="tel" name="val14" class="form-control" value="@Me.Model.TargetValue14" placeholder="血糖値を入力" maxlength="11" style="ime-mode:disabled;" autocomplete="off">
                            <span class="input-group-addon">mg/dl</span>
                        </div>
                        <section id="caution-targetvalue14" class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                    </div>
                    <p class="mb10 center standard" data-vital-type="@QyVitalTypeEnum.BloodSugar">
                        <a href="#">「血糖値」に平均的な目標を設定する</a>
                    </p>

                    <div class="submit-area">
                        @If Not isYappli Then
                            @<a href="../Portal/Information" class="btn btn-close no-ico">戻 る</a>
                        End If
                        <a href="javascript:void(0);" class="btn btn-submit  btn-submit-regist">登 録</a>
                    </div>
                    <section id="caution2" class="section caution mt10 mb0 other-caution hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                </div>
            </div>
        </section>

        @If Me.TempData("IsFinish") IsNot Nothing AndAlso Me.TempData("IsFinish") Then
            @<!-- 「登録完了」ダイアログ -->
            @<div class="modal fade" id="finish-modal" tabindex="-1" data-backdrop="static" data-keyboard="false">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close"><span>×</span></button>
                            <h4 class="modal-title">確認</h4>
                        </div>
                        <div class="modal-body">
                            登録しました。
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-close no-ico mb0">閉じる</button>
                        </div>
                    </div>
                </div>
            </div>
        End If

        <div class="modal fade" id="error-modal" tabindex="-1" data-backdrop="static" data-keyboard="false">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close"><span>×</span></button>
                        <h4 class="modal-title">確認</h4>
                    </div>
                    <div class="modal-body">
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-close no-ico mb0">閉じる</button>
                    </div>
                </div>
            </div>
        </div>
    </main>

    @Html.Action("PortalFooterPartialView", "Portal")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/portal/targetsetting2")
</body>
