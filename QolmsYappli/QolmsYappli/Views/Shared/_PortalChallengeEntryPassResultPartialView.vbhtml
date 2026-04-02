@Imports MGF.QOLMS.QolmsYappli
@ModelType PortalChallengeEntryPassResultPartialViewModel

<main id="main-cont" class="clearfix" role="main">
	<section class="home-btn-wrap">
		<a href="../Portal/challenge" class="home-btn type-2"><i class="la la-angle-left"></i><span> 戻る</span></a>
	</section>
	<section class="contents-area mb20">
		<p class="exp mb30">
            @*とりあえず固定表示*@
			参加に必要な情報を入力してください。
		</p>
        <form class="center">
            <p class="alert alert-danger thin mt10 hide">
                アラートエリア
            </p>
            @If Me.Model.PassCodeVisible Then
                @<input type="text" name="pass" class="form-control mb20 w250 inline-block" placeholder="エントリーコードを入力" required>
            End If

            @For Each item As KeyValuePair(Of String, String) In Me.Model.RequiredN

                Dim placeholderStr As String = String.Empty
                Dim InitialStr As String = String.Empty
                If Me.Model.PageViewModel.RequiredN.ContainsKey(item.Key) Then

                    InitialStr = Me.Model.PageViewModel.RequiredN(item.Key)
                End If

                If item.Key = "PhoneNumber" Then
                    placeholderStr = String.Format("ハイフンなしで{0}を入力", item.Value)
                    @<input type="tel" name="@item.Key" class="form-control mb20 w250 inline-block add" value="@InitialStr" placeholder="@placeholderStr" pattern="^[0-9]+$" required maxlength="11">

                ElseIf item.Key = "FamilyPhoneNumber" Then
                    placeholderStr = String.Format("{0}を入力", item.Value)

                    @<input type="tel" name="@item.Key" class="form-control mb20 w250 inline-block add" value="@InitialStr" placeholder="@placeholderStr" pattern="^[0-9]+$" required maxlength="11">

                ElseIf item.Key = "FamilyRelationship" Then

                    @<select id="@item.Key" name="@item.Key" class="form-control mb20 w250 inline-block add">
                        <option value="" selected="selected">選択してください。</option>
                        <option value="子">子</option>
                        <option value="兄弟姉妹">兄弟姉妹</option>
                        <option value="配偶者">配偶者</option>
                        <option value="親">親</option>
                        <option value="その他">その他</option>

                    </select>
                ElseIf item.Key = "PostalCode" Then
                    'Addressに処理を含むのでスルー
                ElseIf item.Key = "Address" Then

                    If Me.Model.RequiredN.Where(Function(i) i.Key = "PostalCode").Any() Then
                        @<div class="w250 inline-block add">
                            <div style="display:flex;">
                                <input type="tel" id="postcode" name="PostalCode" class="form-control add" value="" placeholder="郵便番号" required maxlength="7">
                                <a href="#" id="address-search" Class="btn btn-submit mb20" style="margin-left:10px;">住所入力</a>

                            </div>
                        </div>
                    End If

                    @<input type="" id="address" name="@item.Key" class="form-control mb20 w250 inline-block add" value="@InitialStr" placeholder="住所を入力" required>

                Else
                    placeholderStr = String.Format("{0}を入力", item.Value)
                    @<input type="" name="@item.Key" class="form-control mb20 w250 inline-block add" value="@InitialStr" placeholder="@placeholderStr" required>
                End If

            Next

            @For Each item As KeyValuePair(Of String, String) In Me.Model.OptionalN

                Dim str As String = String.Empty

                If Me.Model.PageViewModel.OptionalN.ContainsKey(item.Key) Then

                    str = Me.Model.PageViewModel.OptionalN(item.Key)
                End If
                If item.Key = "PhoneNumber" Then

                Else

                    str = String.Format("{0}を入力(任意)", item.Value)
                    @<input type="" name="@item.Key" class="form-control mb20 w250 inline-block add" placeholder="@str" required>
                End If

            Next

            @If Me.Model.RelationFlag Then
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

                                    @<input id="@item.ToString()" type="checkbox" name="" value="true" checked="" data-content="@Convert.ToInt64(item)">
                                    @<label for="@item.ToString()">@QyDictionary.RelationContentType(item)</label>
                                End If

                            Next

                        </div>

                    </div>

                </div>
            End If

            <p class="center">
                <!-- <button class="btn btn-default">
            O　K
        </button> -->
                <a href="#" class="btn btn-default pass-check">
                    O K
                </a>
            </p>
            <p class="exp mb30" style="font-size:1em; text-align:left;">
                @*とりあえず固定表示*@
                ※電話番号は本事業の緊急連絡先として登録をお願い致します。尚、本事業以外で利用することはなく本事業が終了次第連絡先は削除いたします。

            </p>
        </form>
	</section>	
</main>