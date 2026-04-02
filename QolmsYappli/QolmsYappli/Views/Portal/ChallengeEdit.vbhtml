@Imports MGF.QOLMS.QolmsYappli
@ModelType PortalChallengeEditInputModel


@Code
    ViewData("Title") = "チャレンジ"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"
    
    Dim dockDispKey As Guid = Guid.Parse("ded05070-8718-4313-924a-25233e35e218")

    Dim first As String = String.Empty
    If Me.TempData("first") = True Then
        first = "first"
    End If
    
    Dim key As String = String.Empty
    Dim externalid As String = String.Empty
    'Using resource As New QsCrypt(QsCryptTypeEnum.QolmsWeb)

    '    key = resource.EncryptString(Me.Model.ChallengeItem.Challengekey.ToString())
    '    externalid = resource.EncryptString(Me.Model.ChallengeItem.ExternalId)
    'End Using

    Dim today As Date = Date.Now
    
    'Todo:目標達成したかどうかはAPI側で計算するようにする
    
End Code


<body id="challenge" class="lower">
    @Html.AntiForgeryToken()
    

<main id="main-cont" class="clearfix" role="main">
	<section class="home-btn-wrap">
		<a href="../Portal/challenge" class="home-btn type-2"><i class="la la-angle-left"></i><span> 戻る</span></a>
	</section>
	<section class="contents-area mb20">
		<p class="exp mb30">
            @*とりあえず固定表示*@
			必要な情報を編集してください。
		</p>
        <form class="center">
            <p class="alert alert-danger thin mt10 hide">
                アラートエリア
            </p>

            @If Not String.IsNullOrWhiteSpace(Me.Model.ChallengeInputItem.PassCode) Then
                @<input type="text" name="pass" class="form-control mb20 w250 inline-block" placeholder="エントリーコードを入力" required value="@Me.Model.ChallengeInputItem.PassCode" disabled>
            End If

            @For Each item As ChallengeInputValueItem In Me.Model.ChallengeInputItem.RequiredN

                Dim placeholderStr As String = String.Empty
                Dim InitialStr As String = String.Empty

                InitialStr = item.Value

                If item.Key = "PhoneNumber" Then
                    placeholderStr = String.Format("ハイフンなしで{0}を入力", item.Title)
                    @<input type="tel" name="@item.Key" class="form-control mb20 w250 inline-block add" value="@InitialStr" placeholder="@placeholderStr" pattern="^[0-9]+$" required maxlength="11">

                ElseIf item.Key = "Name" Then
                    placeholderStr = String.Format("{0}を入力", item.Title)

                    @<input type="tel" name="@item.Key" class="form-control mb20 w250 inline-block add" value="@InitialStr" placeholder="@placeholderStr" required disabled>

                ElseIf item.Key = "KanaName" Then
                    placeholderStr = String.Format("{0}を入力", item.Title)

                    @<input type="tel" name="@item.Key" class="form-control mb20 w250 inline-block add" value="@InitialStr" placeholder="@placeholderStr" required disabled>


                ElseIf item.Key = "Birthday" Then
                    placeholderStr = String.Format("{0}を入力", item.Title)

                    @<input type="tel" name="@item.Key" class="form-control mb20 w250 inline-block add" value="@InitialStr" placeholder="@placeholderStr" required disabled>

                ElseIf item.Key = "FamilyPhoneNumber" Then
                    placeholderStr = String.Format("{0}を入力", item.Title)

                    @<input type="tel" name="@item.Key" class="form-control mb20 w250 inline-block add" value="@InitialStr" placeholder="@placeholderStr" pattern="^[0-9]+$" required maxlength="11">

                ElseIf item.Key = "FamilyRelationship" Then

                    @<select id="@item.Key" name="@item.Key" class="form-control mb20 w250 inline-block add">
                        <option value="" @IIf(item.Value = String.Empty, "selected='selected'", String.Empty)>選択してください。</option>
                        <option value="子" @IIf(item.Value = "子", "selected='selected'", String.Empty)>子</option>
                        <option value="兄弟姉妹" @IIf(item.Value = "兄弟姉妹", "selected='selected'", String.Empty)>兄弟姉妹</option>
                        <option value="配偶者" @IIf(item.Value = "配偶者", "selected='selected'", String.Empty)>配偶者</option>
                        <option value="親" @IIf(item.Value = "親", "selected='selected'", String.Empty)>親</option>
                        <option value="その他" @IIf(item.Value = "その他", "selected='selected'", String.Empty)>その他</option>

                    </select>
                ElseIf item.Key = "PostalCode" Then
                    'Addressに処理を含むのでスルー
                ElseIf item.Key = "Address" Then

                    If Me.Model.ChallengeInputItem.RequiredN.Where(Function(i) i.Key = "PostalCode").Any() Then

                        Dim postalcode As String = Me.Model.ChallengeInputItem.RequiredN.Where(Function(i) i.Key = "PostalCode").First().Value
                        @<div class="w250 inline-block add">
                            <div style="display:flex;">
                                <input type="tel" id="postcode" name="PostalCode" class="form-control add" value="@postalcode" placeholder="郵便番号" required maxlength="7">
                                <a href="#" id="address-search" Class="btn btn-submit mb20" style="margin-left:10px;">住所入力</a>

                            </div>
                        </div>
                    End If

                    @<input type="" id="address" name="@item.Key" class="form-control mb20 w250 inline-block add" value="@InitialStr" placeholder="住所を入力" required>

                Else
                    placeholderStr = String.Format("{0}を入力", item.Title)
                    @<input type="" name="@item.Key" class="form-control mb20 w250 inline-block add" value="@InitialStr" placeholder="@placeholderStr" required>
                End If

            Next

            @For Each item As ChallengeInputValueItem In Me.Model.ChallengeInputItem.OptionalN

                Dim str As String = String.Empty

                str = item.Title
                If item.Key = "PhoneNumber" Then

                ElseIf item.Key = "InsuredNumber" Then

                    str = String.Format("{0}を入力(任意)", item.Title)
                    If Me.Model.ChallengeItem.Challengekey = Guid.Parse("6550B115-7411-4C0E-9AC3-9121AC7093B1") OrElse Me.Model.ChallengeItem.Challengekey = Guid.Parse("A87C8371-E556-4D0F-8CAE-C08B8F5A3D2C") Then
                        @<input type="" name="@item.Key" class="form-control mb20 w250 inline-block add" placeholder="@str" required value="@Me.Model.linkageSystemId" @IIf(Me.Model.linkageStatus = 2, "disabled", String.Empty)>
                    Else
                        @<input type="" name="@item.Key" class="form-control mb20 w250 inline-block add" placeholder="@str" required value="@item.Value">

                    End If

                Else

                    str = String.Format("{0}を入力(任意)", item.Title)
                    @<input type="" name="@item.Key" class="form-control mb20 w250 inline-block add" placeholder="@str" required value="@item.Value">
                End If

            Next

            @If Me.Model.ChallengeItem.RelationFlag Then
                @<div class="form wizard-form mb30">
                    <div class="t-row">
                        @*<label for="input2" class="label-txt">
                        <span class="ico required">必須</span>
                    開示許可
                    </label>*@

                        <p class="exp mb10">
                            開示許可
                        </p>
                        <div class="wizard-form inline-block link-ico tanita" id="connection-data">
                            @For Each item As QyRelationContentTypeEnum In [Enum].GetValues(GetType(QyRelationContentTypeEnum))

                                If Not (item = QyRelationContentTypeEnum.None _
                                OrElse item = QyRelationContentTypeEnum.Contact _
                                OrElse item = QyRelationContentTypeEnum.Dental _
                                OrElse item = QyRelationContentTypeEnum.Assessment) Then

                                    @<input id="@item.ToString()" type="checkbox" name="" value="true" @IIf((Me.Model.RelationContentFlags And item) = item, "checked", String.Empty) data-content="@Convert.ToInt64(item)">
                                    @<label for="@item.ToString()">@QyDictionary.RelationContentType(item)</label>
                                End If

                            Next

                        </div>
                    </div>

                </div>

            End If

            <p class="center">

                <a href="#" class="btn btn-default edit">
                    O K
                </a>
            </p>

            <p class="center">

                <a href="#" class="btn btn-default cancel" style="background-color:#6c6c6c;">
                    中 止
                </a>
            </p>

            <p class="exp mb30" style="font-size:1em; text-align:left;">
                @*とりあえず固定表示*@
                ※電話番号は本事業の緊急連絡先として登録をお願い致します。尚、本事業以外で利用することはなく本事業が終了次第連絡先は削除いたします。

            </p>
        </form>
            </section>

            <div class="modal fade" id="cancel-modal" tabindex="-1" data-backdrop="static" data-keyboard="false" data-row-no="-1">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type = "button" class="close"><span>×</span></button>
                            <h4 class="modal-title">中止</h4>
                        </div>
                        <div class="modal-body">
                            チャレンジを中止しますか？
                        </div>
                        <div class="modal-footer">
                            <button type = "button" class="btn btn-close no-ico mb0">閉じる</button>
                            <button type = "button" class="btn btn-delete">中 止</button>
                        </div>
                    </div>
                </div>
            </div>
</main>
           <div class="modal fade" id="error-modal" tabindex="-1" data-backdrop="static" data-keyboard="false" data-row-no="-1">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type = "button" class="close"><span>×</span></button>
                            <h4 class="modal-title">エラー</h4>
                        </div>
                        <div class="modal-body">
                            エラーメッセージが入ります
                        </div>
                        <div class="modal-footer">
                            <button type = "button" class="btn btn-close no-ico mb0">閉じる</button>
                        </div>
                    </div>
                </div>
            </div>


        @Html.Action("PortalFooterPartialView", "Portal")

        @QyHtmlHelper.RenderScriptTag("~/dist/js/portal/challengeedit")

</body>
