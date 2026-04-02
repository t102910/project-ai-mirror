@Imports MGF.QOLMS.QolmsYappli
@ModelType PortalCompanyConnectionRequestInputModel

@Code
    ViewData("Title") = "法人連携申請"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"
    
End Code

<body id="gulf-cont" class="lower gulf">

    @Html.AntiForgeryToken()

<main id="main-cont" class="clearfix" role="main">

       @Select Case Me.Model.FromPageNoType
           Case QyPageNoTypeEnum.PortalHome
                @<section class="home-btn-wrap" data-pageno="@Convert.ToByte(Me.Model.FromPageNoType)">
                   <a class="home-btn type-2" href="../Portal/connectionsetting?fromPageNo=1&tabno=2" ><i class="la la-angle-left"></i><span> 戻る</span></a>
                </section>  
           Case Else
                @<section class="home-btn-wrap type2" data-pageno="@Convert.ToByte(Me.Model.FromPageNoType)">
		            <a class="home-btn type-2" href="../Portal/connectionsetting"><i class="la la-angle-left"></i><span> 戻る</span></a>
	            </section> 
       End Select
	<section class="contents-area">

        <h2 class="title">@ViewData("Title")</h2>
		<hr>
        <div>
		    <section class="section default">
			    法人と連携します。
                連携を開始した時点で基本情報が管理者に公開されます。<br />
			    連携が完了すると健診情報がある場合は１時間程度で閲覧いただけるようになります。
		    </section>

            @*入力*@
		  	<h3 class="title mt10">
				<span>連携情報</span>
			</h3>
            <div class="form wizard-form mb30">

		        <label for="code" class="t-row line">
			        <span class="label-txt">
				        <span class="ico required">必須</span>
                        企業コード
			        </span>
					<input type="text" id="code" name ="model.CompanyCode" class="form-control mb10" placeholder="企業コード" value="" required="required" maxlength="25" style="ime-mode:active;" autocomplete="off">
                    <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
 		        </label>

		        <label for="id" class="t-row line">
			        <span class="label-txt">
				        <span class="ico required">必須</span>
                        社員番号
			        </span>
					<input type="text" id="id" name ="model.Companyid" class="form-control mb10" placeholder="社員番号" value="" required="required" maxlength="25" style="ime-mode:active;" autocomplete="off">
                    <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
 		        </label>

                <div class="t-row line">
                        <label for="name" class="label-txt"><span class="ico required">必須</span> お名前</label>
					    <input type="text" id="family-name" name ="model.FamilyName" class="form-control mb10" placeholder="姓" value="@Me.Model.FamilyName" required="required" maxlength="25" style="ime-mode:active;" autocomplete="off">
					    <input type="text" id="given-name" name="model.GivenName" class="form-control" placeholder="名" value="@Me.Model.GivenName"required="required"maxlength="25" style="ime-mode:active;" autocomplete="off">
                        <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                </div>  

                <div class="t-row line">
                        <label for="kana" class="label-txt"><span class="ico required">必須</span> お名前（カナ）</label>
						<input type="text" id="family-kana-name" name="model.FamilyKanaName" class="form-control mb10 disabled" placeholder="セイ" value="@Me.Model.FamilyKanaName" required="required" maxlength="25" style="ime-mode:active;" autocomplete="off">
						<input type="text" id="given-kana-name" name="model.GivenKanaName" class="form-control" placeholder="メイ" value="@Me.Model.GivenKanaName" required="required" maxlength="25" style="ime-mode:active;" autocomplete="off">
                        <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                </div>

		        <label for="sex" class="t-row line">
			        <span class="label-txt">
				        <span class="ico required">必須</span>
                        性 別
			        </span>
                    <p id="sex" name="model.SexType" style="font-size:16px; padding:10px 12px;" data-value="@Me.Model.SexType.ToString()">@QyDictionary.SexType(Me.Model.SexType)</p>

		        </label>

		        <div class="t-row">
		            <label for="birth-year" class="label-txt">
                        <span class="ico required">必須</span>
                        生年月日
		            </label>
                    <p  id="birthday" name="model.SexType" style="font-size:16px; padding:10px 12px;" data-birthyear="@Me.Model.BirthYear"data-birthmonth="@Me.Model.BirthMonth"data-birthday="@Me.Model.BirthDay">@String.Format("{0}年 {1}月 {2}日", Me.Model.BirthYear, Me.Model.BirthMonth, Me.Model.BirthDay) </p>
                    <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                    
                </div>

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


                                @<input id="@item.ToString()" type="checkbox" name="" value="true" checked="" data-content="@Convert.ToInt64(item)">
                                @<label for="@item.ToString()">@QyDictionary.RelationContentType(item)</label>

                            End If

                        Next

                    </div>


                </div>

                <section id="summary-cation" class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                <p class="submit-area mb30">
				    <a id="request" href="javascript:void(0);" class="btn btn-submit">確 認</a>
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

    @If Me.Model.MembershipType = QyMemberShipTypeEnum.LimitedTime OrElse Me.Model.MembershipType = QyMemberShipTypeEnum.Premium Then

        @<!-- 「連携解除の確認」ダイアログ -->
        @<div class="modal fade" id="disconnect-modal" tabindex="-1" data-backdrop="static" data-keyboard="false" data-row-no="-1">
	        <div class="modal-dialog">
		        <div class="modal-content">
			        <div class="modal-header">
				        <button type="button" class="close"><span>×</span></button>
				        <h4 class="modal-title">確認</h4>
			        </div>
			        <div class="modal-body">
				        企業連携を利用するとプレミアム会員が解約されます。<br/>
                        よろしいですか？
			        </div>
			        <div class="modal-footer">
				        <button type="button" class="btn btn-close no-ico mb0">閉じる</button>
				        <button type="button" class="btn btn-delete">解 除</button>
			        </div>
		        </div>
	        </div>
        </div>

    End If

</main>

    @Html.Action("PortalFooterPartialView", "Portal")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/portal/companyconnectionrequest")
</body>
