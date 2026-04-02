@Imports MGF.QOLMS.QolmsYappli
@ModelType PortalHospitalConnectionRequestInputModel

@Code
    ViewData("Title") = "病院連携申請"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"
    
End Code

<body id="gulf-cont" class="lower gulf">

    @Html.AntiForgeryToken()

    <main id="main-cont" class="clearfix" role="main">
       @Select Case Me.Model.FromPageNoType
           Case QyPageNoTypeEnum.PortalHome
                @<section class="home-btn-wrap" data-pageno="@Convert.ToByte(Me.Model.FromPageNoType)">
                   <a class="home-btn type-2" href="../Portal/connectionsetting?fromPageNo=1&tabno=3" ><i class="la la-angle-left"></i><span> 戻る</span></a>
                </section>  
           Case Else
                @<section class="home-btn-wrap type2" data-pageno="@Convert.ToByte(Me.Model.FromPageNoType)">
		            <a class="home-btn type-2" href="../Portal/connectionsetting?TabNo=3"><i class="la la-angle-left"></i><span> 戻る</span></a>
	            </section> 
       End Select
	    <section class="contents-area mb100">

            <h2 class="title">@ViewData("Title")</h2>
		    <hr>
   
            <div>
                @If Me.Model.LinkageSystemNo > 0 AndAlso Me.Model.HospitalList.Count = 0 Then
                    @<section class="section default">
                        サービス終了のため編集できません。
                    </section>
                ElseIf Me.Model.LinkageSystemNo = 47106 OrElse Me.Model.LinkageSystemNo = 47000016 Then
                    @<section class="section default">
                        2025/4より、診察券番号は8桁から12桁に変更になりました。<br />
                        登録済みの8桁の診察券番号は、前に0を追加して12桁にしています。<br />
                        病院にデータ連携の申請をします。<br />
                        連携申請を行うと、病院側へアプリ内の各種データが開示されます。
                    </section>
                Else
                    @<section class="section default">
                        病院にデータ連携の申請をします。<br />
                        連携申請を行うと、病院側へアプリ内の各種データが開示されます。
                    </section>
                End If

                @*入力*@
                <h3 class="title mt10">
                    <span> 連携情報</span>
                </h3>
                @*                <p class="section default">
            現在一部のユーザー様で健診結果が正しく表示されていない事象が確認されています。<br />
            2023年2月末頃の復旧を予定しております。
        </p>*@
                <div class="form wizard-form mb30">
                    <label for="input1" class="t-row line">
                        <span class="label-txt">
                            <span class="ico required">必須</span>
                            病 院
                        </span>
                        <select id="no" name="model.LinkageSystemNo" class="form-control" required="required" @IIf(Me.Model.LinkageSystemNo > 0, "disabled", "").ToString()>
                            <option value="0">選択してください</option>

                            @For Each item As KeyValuePair(Of Integer, String) In Me.Model.HospitalList
                                @<option value="@item.Key.ToString()" @IIf(item.Key = Me.Model.LinkageSystemNo, "selected", "").ToString()>@item.Value</option>

                            Next
                        </select>
                        <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>

                    </label>

                    @code

                        Dim disabled = String.Empty
                        If Me.Model.StatusType = 2 Then

                            disabled = "disabled"
                        End If

                    End Code

                    <label for="id" class="t-row line">
                        <span class="label-txt">
                            <span class="ico required">必須</span>
                            診察券番号/個人番号
                        </span>
                        @If (Me.Model.LinkageSystemNo = 47106 OrElse Me.Model.LinkageSystemNo = 47000016) AndAlso Me.Model.LinkageSystemId.Count = 8 Then
                            '中部地区医師会、北部地区医師会病院で8桁で連携済みの場合先頭を0で補完
                            Me.Model.LinkageSystemId = Me.Model.LinkageSystemId.PadLeft(12, "0"c)
                        End If
                        <input type="text" id="id" name="model.LinkageSystemId" class="form-control mb10" placeholder="診察券番号/個人番号" value="@Me.Model.LinkageSystemId" required="required" maxlength="12" style="ime-mode:active;" autocomplete="off" @disabled>
                        <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                    </label>


                    <div class="t-row line">
                        <label for="name" class="label-txt"><span class="ico required">必須</span> お名前</label>
                        <input type="text" id="family-name" name="model.FamilyName" class="form-control mb10" placeholder="姓" value="@Me.Model.FamilyName" required="required" maxlength="25" style="ime-mode:active;" autocomplete="off" @disabled>
                        <input type="text" id="given-name" name="model.GivenName" class="form-control" placeholder="名" value="@Me.Model.GivenName" required="required" maxlength="25" style="ime-mode:active;" autocomplete="off" @disabled>
                        <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                    </div>

                    <div class="t-row line">
                        <label for="222" class="label-txt"><span class="ico required">必須</span> お名前（カナ）</label>
                        <input type="text" id="family-kana-name" name="model.FamilyKanaName" class="form-control mb10 disabled" placeholder="セイ" value="@Me.Model.FamilyKanaName" required="required" maxlength="25" style="ime-mode:active;" autocomplete="off" @disabled>
                        <input type="text" id="given-kana-name" name="model.GivenKanaName" class="form-control" placeholder="メイ" value="@Me.Model.GivenKanaName" required="required" maxlength="25" style="ime-mode:active;" autocomplete="off" @disabled>
                        <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                    </div>

                    <label for="sex" class="t-row line">
                        <span class="label-txt">
                            <span class="ico required">必須</span>
                            性 別
                        </span>
                        <p id="sex" name="model.SexType" style="font-size:16px; padding:10px 12px;" data-value="@Me.Model.SexType.ToString()">@QyDictionary.SexType(Me.Model.SexType)</p>

                        @*                       @QyHtmlHelper.ToSexDropDownList("sex", "model.SexType", Me.Model.SexType.ToString(), "form-control disabled")

                <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>*@
                    </label>

                    <div class="t-row line">
                        <label for="birth-year" class="label-txt">
                            <span class="ico required">必須</span>
                            生年月日
                        </label>
                        <p id="birthday" name="model.SexType" style="font-size:16px; padding:10px 12px;" data-birthyear="@Me.Model.BirthYear" data-birthmonth="@Me.Model.BirthMonth" data-birthday="@Me.Model.BirthDay">@String.Format("{0}年 {1}月 {2}日", Me.Model.BirthYear, Me.Model.BirthMonth, Me.Model.BirthDay) </p>

                        <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>

                    </div>

                    <label for="mail" class="t-row line">
                        <span class="label-txt"><span class="ico required">必須</span> 連絡先メールアドレス</span>
                        <input type="text" id="mail" name="model.MailAddress" class="form-control w-max" value="@Me.Model.MailAddress" required="required" maxlength="256" style="ime-mode:disabled;" autocomplete="off" @disabled>
                        <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                    </label>


                    <div class="t-row">
                        <label for="input2" class="label-txt">
                            <span class="ico required">必須</span>
                            開示許可
                        </label>
                        <div class="wizard-form inline-block link-ico tanita" id="connection-data">
                            @For Each item As QyRelationContentTypeEnum In [Enum].GetValues(GetType(QyRelationContentTypeEnum))

                                'Noneと未実装項目を除外
                                If Not (item = QyRelationContentTypeEnum.None _
                                    OrElse item = QyRelationContentTypeEnum.Contact _
                                    OrElse item = QyRelationContentTypeEnum.Dental _
                                    OrElse item = QyRelationContentTypeEnum.Assessment) Then


                                    If Me.Model.LinkageSystemNo > 0 Then

                                        @<input id="@item.ToString()" type="checkbox" name="" value="true" @IIf(item = QyRelationContentTypeEnum.Information OrElse (Me.Model.RelationContentFlags And item) = item, "checked", String.Empty) data-content="@Convert.ToInt64(item)">
                                        @<label for="@item.ToString()">@QyDictionary.RelationContentType(item)</label>

                                    Else

                                        ' 新規画面なのでALL ON
                                        @<input id="@item.ToString()" type="checkbox" name="" value="true" checked data-content="@Convert.ToInt64(item)">
                                        @<label for="@item.ToString()">@QyDictionary.RelationContentType(item)</label>

                                    End If
                                End If

                            Next

                        </div>

                    </div>

                    @*<p style="font-size:0.9em; padding:0px 10px 10px 10px;">
                ※健診データがJOTOホームドクターアプリ上に反映されましたら「joto-kenshin@au-mobile.com 」よりメールいたします。<br />
                ※迷惑メール設定をされている方は、メールの受信設定で「joto-kenshin@au-mobile.com」​からのメールが受信できるよう設定の上、ご送信ください。​<br />
                ※メールが届かない場合はメールアドレスが間違っている可能性がございます。
            </p>*@

                    <section id="summary-cation" class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                    <p class="submit-area mb30">
                        <a href="javascript:void(0);" id="reconnection" class="btn btn-submit">申 請</a>
                    </p>
                </div>

            </div>
	    </section>
       <!-- 「個人情報更新の確認」ダイアログ -->
        <div class="modal fade" id="identity-modal" tabindex="-1" data-backdrop="static" data-keyboard="false" data-row-no="-1">
	        <div class="modal-dialog">
		        <div class="modal-content">
			        <div class="modal-header">
				        <button type="button" class="close"><span>×</span></button>
				        <h4 class="modal-title">更新の確認</h4>
			        </div>
			        <div class="modal-body">
				        入力された個人情報が登録と一致しませんでした。個人情報更新しますか？
			        </div>
			        <div class="modal-footer">
				        <button type="button" class="btn btn-close no-ico mb0">閉じる</button>
				        <button type="button" class="btn btn-submit">更 新</button>
			        </div>
		        </div>
	        </div>
        </div>
    </main>

    @Html.Action("PortalFooterPartialView", "Portal")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/portal/hospitalconnectionrequest")
</body>
