@Imports MGF.QOLMS.QolmsYappli
@ModelType PortalLocalIdVerificationRegisterInputModel

@Code
    ViewData("Title") = "エントリー情報入力"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"

    'Dim first As String = String.Empty
    'If Me.TempData("first") = True Then
    '    first = "first"
    'End If

    'Dim key As String = String.Empty
    'Dim externalid As String = String.Empty

    'Dim today As Date = Date.Now


End Code


<body id="confirm" class="lower ginowan">
    @Html.AntiForgeryToken()

<main id="main-cont" class="clearfix" role="main">
	<section class="home-btn-wrap">
		<a href="../Portal/Home" class="home-btn type-2"><i class="la la-angle-left"></i><span> 戻る</span></a>
	</section>
	<section class="contents-area mb20">
		<div class="box type-2">
			<div class="wrap">
				<h3 class="title">
					入力フォーム
				</h3>
				<p class="exp mb30">
					必要な情報を編集してください。
				</p>
				<form class="center">
					<p class="alert alert-danger thin mt10 hide">
						アラートエリア
					</p>
					<div class="m-auto w250 left">
						@*<label class="small-label"><i class="ico required">必須</i> エントリーコード</label>
						<input type="text" name="pass" class="form-control mb20 w250 inline-block" placeholder="半角英数" required value="">
						
						<label class="small-label"><i class="ico required">必須</i> お名前</label>
						<input type="tel" name="Name" class="form-control mb20 w250 inline-block add" value="" placeholder="沖縄 太郎" required>
						<label class="small-label"><i class="ico required">必須</i> 生年月日</label>
						<input type="date" name="Birthday" class="form-control mb20 w250 inline-block add" value="" placeholder="1981/07/02" required>*@
						<label class="small-label"><i class="ico required">必須</i> メールアドレス</label>
						<input type="" name="model.MailAddress" class="form-control mb10 w250 inline-block add" value="@Me.Model.MailAddress" placeholder="example@example.com" required>
                                                
                        <section class="section caution mt0 mb20 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
						
                        <label class="small-label"><i class="ico required">必須</i> 電話番号</label>
						<input type="tel" name="model.PhoneNumber" class="form-control mb10 w250 inline-block add" value="@Me.Model.PhoneNumber" placeholder="ハイフンなしで電話番号を入力" pattern="^[0-9]+$" required maxlength="11">
                                                
                        <section class="section caution mt0 mb20 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
						@*<label class="small-label"><i class="ico required">必須</i> 住所</label>
						<input type="" id="address" name="Address" class="form-control mb20 w250 inline-block add" value="住所" placeholder="住所を入力" required>*@
						@*<label class="small-label"><i class="ico na">任意</i> 被保険者番号</label>
						<input type="" name="InsuredNumber" class="form-control mb20 w250 inline-block add" placeholder="被保険者番号を入力(任意)" required value="" >*@
					</div>
					@*<div class="form wizard-form mb30">
						<div class="t-row">
							<p class="exp mb10 bold">
								開示許可
							</p>
							<div class="wizard-form inline-block link-ico tanita" id="connection-data">
								<input id="Information" type="checkbox" name="" value="true" checked data-content="1">
								<label for="Information">基本情報</label>
								<input id="Vital" type="checkbox" name="" value="true" checked data-content="2">
								<label for="Vital">バイタル情報</label>
								<input id="Medicine" type="checkbox" name="" value="true" checked data-content="4">
								<label for="Medicine">お薬情報</label>
								<input id="Examination" type="checkbox" name="" value="true" checked data-content="8">
								<label for="Examination">検査・健診情報</label>
								<input id="Meal" type="checkbox" name="" value="true" checked data-content="128">
								<label for="Meal">食事情報</label>
							</div>
						</div>
					</div>*@
                    
                    <div class="form wizard-form mb30 hide">
                        <div class="t-row">
                            <label for="input2" class="label-txt">
                                    <span class="ico required">必須</span>
                                開示許可
                                </label>

                            <div class="wizard-form inline-block link-ico tanita" id="connection-data">
                                @For Each item As QyRelationContentTypeEnum In [Enum].GetValues(GetType(QyRelationContentTypeEnum))

                                    If Not (item = QyRelationContentTypeEnum.None _
                                    OrElse item = QyRelationContentTypeEnum.Contact _
                                    OrElse item = QyRelationContentTypeEnum.Dental _
                                    OrElse item = QyRelationContentTypeEnum.Assessment) Then

                                        If Me.Model.RelationContentFlags = QyRelationContentTypeEnum.None Then
                                            ' 新規画面なのでALL ON
                                            @<input id="@item.ToString()" type="checkbox" name="" value="true" checked data-content="@Convert.ToInt64(item)">
                                            @<label for="@item.ToString()">@QyDictionary.RelationContentType(item)</label>
                                        Else
                                            '編集
                                            @<input id="@item.ToString()" type="checkbox" name="" value="true" @IIf(item = QyRelationContentTypeEnum.Information OrElse (Me.Model.RelationContentFlags And item) = item, "checked", String.Empty) data-content="@Convert.ToInt64(item)">
                                            @<label for="@item.ToString()">@QyDictionary.RelationContentType(item)</label>
                                        End If

                                    End If

                                Next
                                                            
                                <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                            </div>
                        </div>

                    </div>

                    <section id="summary-cation" class="section caution mt10 mb0 hide left"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>

					<div class="submit-area">
                                                        
						<p class="center">
							<a href="javascript:void(0);" class="btn btn-default edit">
								O K
							</a>
						</p>
                        @If Me.Model.LinkageSystemNo > 0 Then
                            @<p class="center">
							    <a href="javascript:void(0);" class="btn btn-default cancel" style="background-color:#6c6c6c;">
								    エントリー解除
							    </a>
						    </p>
                        End If
					</div>
					@*<p class="exp mb30" style="font-size:1em; text-align:left;">
						※電話番号は本事業の緊急連絡先として登録をお願い致します。尚、本事業以外で利用することはなく本事業が終了次第連絡先は削除いたします。
					</p>*@
				</form>
			</div>
		</div>
	</section>	
</main>
    
        <div class="modal fade" id="cancel-modal" tabindex="-1" data-backdrop="static" data-keyboard="false" data-row-no="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close"><span>×</span></button>
                        <h4 class="modal-title">中止</h4>
                    </div>
                    <div class="modal-body">
                        エントリーを解除しますか？<br/>
                        エントリー解除をした場合、本プロジェクトで今後獲得予定のポイントについては解除後に消失します。
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-close no-ico mb0">閉じる</button>
                        <button type="button" class="btn btn-delete">解 除</button>
                    </div>
                </div>
            </div>
        </div>

    @Html.Action("PortalFooterPartialView", "Portal")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/portal/localidverificationregister")

</body>
