@Imports MGF.QOLMS.QolmsYappli
@ModelType PortalMedicineConnectionRequestInputModel

@Code
    ViewData("Title") = "薬局連携申請"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"
    
End Code

<body id="gulf-cont" class="lower gulf">

    @Html.AntiForgeryToken()

    <main id="main-cont" class="clearfix" role="main">

       @Select Case Me.Model.FromPageNoType
           Case QyPageNoTypeEnum.PortalHome
                @<section class="home-btn-wrap" data-pageno="@Convert.ToByte(Me.Model.FromPageNoType)">
                   <a class="home-btn type-2" href="../Portal/medicineconnectionsearch?fromPageNo=1" ><i class="la la-angle-left"></i><span> 戻る</span></a>
                </section>  
           Case Else
                @<section class="home-btn-wrap type2" data-pageno="@Convert.ToByte(Me.Model.FromPageNoType)">
		            <a class="home-btn type-2" href="../Portal/medicineconnectionsearch"><i class="la la-angle-left"></i><span> 戻る</span></a>
	            </section> 
       End Select

	    <section class="contents-area mb100">

            <h2 class="title">@ViewData("Title")</h2>
		    <hr>
   
            <div>
		        <section class="section default">
			        対象薬局でのお薬情報連携の申請をします。<br/>
                    連携が完了すると、翌日以降にお薬情報が閲覧頂けるようになります。

		        </section>

                @*入力*@
		  		<h3 class="title mt10">
				    <span>連携情報</span>
			    </h3>
                <div class="form wizard-form mb30">
                    <label for="input1" class="t-row line">
			            <span class="label-txt">
				            <span class="ico required">必須</span>
                                薬 局
			            </span>
                
					    <input type="text" id="facility" name ="model.LinkageSystemNo" class="form-control mb10" value="@Me.Model.FacilityName" data-facilitykey="@Me.Model.FacilityKey" data-linkage="@Model.LinkageSystemNo" data-pharmacyid="@Me.Model.PharmacyId.ToString()" disabled >

                        <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
		            
                    </label>


		            <label for="id" class="t-row line">
			            <span class="label-txt">
				            <span class="ico required">必須</span>
                            患者番号(領収書に記載の患者番号を記入下さい。)

                            <a id="image" href="javascript:void(0);" class="btn btn-submit no-ico narrow low-height mb0 mt10 list-submit">イメージ</a>
			            </span>
					    <input type="text" id="id" name ="model.PatientCardNo" value="@Me.Model.PatientCardNo" class="form-control mb10" placeholder="患者番号"  required="required" maxlength="20" style="ime-mode:active;" autocomplete="off">
                        <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
 		            </label>


                    <div class="t-row line">
                        <label for="name" class="label-txt"><span class="ico required">必須</span> お名前</label>
					    <input type="text" id="family-name" name ="model.FamilyName" class="form-control mb10" placeholder="姓" required="required" maxlength="25" style="ime-mode:active;" autocomplete="off" value="@Me.Model.FamilyName">
					    <input type="text" id="given-name" name="model.GivenName" class="form-control" placeholder="名" required="required"maxlength="25" style="ime-mode:active;" autocomplete="off" value="@Me.Model.GivenName">
                        <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                    </div>  

		            <label for="sex" class="t-row line">
			            <span class="label-txt">
				            <span class="ico required">必須</span>
                            性 別
			            </span>
                        <p id="sex" name="model.SexType" style="font-size:16px; padding:10px 12px;" data-value="@Me.Model.SexType.ToString()">@QyDictionary.SexType(Me.Model.SexType)</p>
                        <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
		            </label>

		            <div class="t-row">
		                <label for="birth-year" class="label-txt">
                            <span class="ico required">必須</span>
                            生年月日
		                </label>
                        <p  id="birthday" name="model.SexType" style="font-size:16px; padding:10px 12px;" data-birthyear="@Me.Model.BirthYear"data-birthmonth="@Me.Model.BirthMonth"data-birthday="@Me.Model.BirthDay" @String.Format("{0}年 {1}月 {2}日", Me.Model.BirthYear, Me.Model.BirthMonth, Me.Model.BirthDay)" > @String.Format("{0}年 {1}月 {2}日", Me.Model.BirthYear, Me.Model.BirthMonth, Me.Model.BirthDay)</p>
                        <section class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                    
                    </div>

                    <section id="summary-cation" class="section caution mt10 mb0 hide"><h4></h4>アラートエリアです。不要時.hideで消してください。</section>
                    <p class="submit-area mb30">
				        <a id="request" href="javascript:void(0);" class="btn btn-submit">申 請</a>
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

        <div id="modal-text" class="hide">

	        <div class="text-body">

    	        <img class="w-max" src="/dist/img/medicine/medicine-patientcode.png">
	        </div>
            <br />
	        @*<div class="text-unit"></div>*@
	        <i class="la la-close" id="modal-close"></i>
        </div>

    </main>

    @Html.Action("PortalFooterPartialView", "Portal")
    @QyHtmlHelper.RenderScriptTag("~/dist/js/portal/medicineconnectionrequest")

</body>


