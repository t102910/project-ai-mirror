@Imports MGF.QOLMS.QolmsYappli
@ModelType PortalCompanyConnectionEditInputModel

@Code
    ViewData("Title") = String.Format("{0}連携編集", Me.Model.LinkageSystemName)
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"

End Code

<body id="gulf-cont" class="lower gulf">

    @Html.AntiForgeryToken()

<main id="main-cont" class="clearfix" role="main">
       @Select Case Me.Model.FromPageNoType
           Case QyPageNoTypeEnum.PortalHome
                @<section class="home-btn-wrap" data-pageno="@Convert.ToByte(Me.Model.FromPageNoType)">
                   <a class="home-btn type-2" href="@String.Format("../Portal/companyconnection?fromPageNo=1&linkagesystemno={0}", Me.Model.LinkageSystemNo)"><i class="la la-angle-left"></i><span> 戻る</span></a>
                </section>  
           Case Else
                @<section class="home-btn-wrap type2" data-pageno="@Convert.ToByte(Me.Model.FromPageNoType)">
		            <a class="home-btn type-2" href="@String.Format("../Portal/companyconnection?linkagesystemno={0}", Me.Model.LinkageSystemNo)"><i class="la la-angle-left"></i><span> 戻る</span></a>
	            </section> 
       End Select

	<section class="contents-area">
            <h2 class="title">@ViewData("Title")</h2>
		    <hr>
            <div>
@*		        <section class="section default">
			        必要なら説明が入ります
		        </section>*@

                @*入力*@
		  		<h3 class="title mt10">
				    <span>連携情報</span>
			    </h3>
                <div class="form wizard-form mb30">

		            

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

                                    'サンエーでバイタルを表示しない

                                    If Not (Me.Model.LinkageSystemNo = 47107 AndAlso item = QyRelationContentTypeEnum.Vital) Then
                                        @<input id="@item.ToString()" type="checkbox" name="" value="true" @IIf((Me.Model.RelationContentFlags And item) = item, "checked", String.Empty) data-content="@Convert.ToInt64(item)">
                                        @<label for="@item.ToString()">@QyDictionary.RelationContentType(item)</label>

                                    End If

                                End If

                            Next

                        </div>
                        <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                    </div>

                    <label for="code" class="t-row">
			            <span class="label-txt">
				            <span class="ico required">必須</span>
                            従業員連絡用メールアドレス
			            </span>
					    <input type="email" id="mail" name ="model.MailAddress" class="form-control mb10" placeholder="メールアドレス" value="@Me.Model.MailAddress" required="required" maxlength="100" style="ime-mode:active;" autocomplete="off">
                        <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>

 		            </label>

                    <section id="summary-cation" class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                    <p class="submit-area mb30">
				        <a id="request" href="javascript:void(0);" class="btn btn-submit" data-no="@Me.Model.LinkageSystemNo">確 認</a>
			        </p>
                </div>

            </div>
	</section>
</main>

    @Html.Action("PortalFooterPartialView", "Portal")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/portal/companyconnectionedit")
</body>
