@Imports MGF.QOLMS.QolmsYappli
@ModelType PortalMedicineConnectionViewModel

@Code
    ViewData("Title") = "薬局連携"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"
    
End Code

<body id="gulf-cont" class="lower gulf">

    @Html.AntiForgeryToken()

    <main id="main-cont" class="clearfix" role="main">

       @Select Case Me.Model.FromPageNoType
           Case QyPageNoTypeEnum.PortalHome
                @<section class="home-btn-wrap" data-pageno="@Convert.ToByte(Me.Model.FromPageNoType)">
                   <a class="home-btn type-2" href="../Portal/connectionsetting?fromPageNo=1&tabno=4" ><i class="la la-angle-left"></i><span> 戻る</span></a>
                </section>  
           Case Else
                @<section class="home-btn-wrap type2" data-pageno="@Convert.ToByte(Me.Model.FromPageNoType)">
		            <a class="home-btn type-2" href="../Portal/connectionsetting"><i class="la la-angle-left"></i><span> 戻る</span></a>
	            </section> 
       End Select

        <section class="contents-area mb0">
		    <h2 class="title">@Me.Model.FacilityName</h2>
		    <hr>

            <section class="section default">
			    お薬情報の連携完了後、最大1日程度で情報が閲覧できるようになります。
		    </section>
		    <section class="section center">

            @If Me.Model.StatusType = 2 Then
                @<span class="ico main" style =""><i class="la la-check-circle-o"></i> 連携済み</span>
            End If
    	    </section>
		    <h3 class="title">受け取る情報</h3>
	    </section>
	    <section class="contents-area">

            <section style="border: 1px solid #ccc;  padding: 10px;">

                <p class="cal-area">
                    <span class="date"><i class="ico breakfast">お薬情報</i></span>
			    </p>
            </section>

	    </section>
 
        <section class="contents-area">
            
            <div class="mb100">

                <h3 class="title mt10">
				    <span>薬局連携を解除する</span>
			    </h3>
			    <p class="mb20">
			    連携を解除すると、連携したお薬情報のデータが閲覧できなくなりますので、ご注意ください。
			    </p>
			    <section class="section caution mt10 mb10 hide"><h4>heading...</h4>アラートエリアです。不要時.hideで消してください。</section>
			    <p class="submit-area">
				    <a href="javascript:void(0);" id="cancel" class="btn btn-delete" data-no="@Me.Model.LinkageSystemNo.ToString()"data-facilitykey="@Me.Model.FacilityKey.ToString()">解除する</a>
			    </p>
		    </div> 	   
	    </section>
    </main>
    <!-- 「個人情報更新の確認」ダイアログ -->
    <div class="modal fade" id="delete-modal" tabindex="-1" data-backdrop="static" data-keyboard="false" data-row-no="-1">
	    <div class="modal-dialog">
		    <div class="modal-content">
			    <div class="modal-header">
				    <button type="button" class="close"><span>×</span></button>
				    <h4 class="modal-title">確認</h4>
			    </div>
			    <div class="modal-body">
				    薬局との連携を解除しますか？
			    </div>
			    <div class="modal-footer">
				    <button type="button" class="btn btn-close no-ico mb0">閉じる</button>
				    <button type="button" class="btn btn-delete">解除する</button>
			    </div>
		    </div>
	    </div>
    </div>

        <!-- 「個人情報更新の確認」ダイアログ -->
    <div class="modal fade" id="info-modal" tabindex="-1" data-backdrop="static" data-keyboard="false" data-row-no="-1">
	    <div class="modal-dialog">
		    <div class="modal-content">
			    <div class="modal-header">
				    <button type="button" class="close"><span>×</span></button>
				    <h4 class="modal-title">お知らせ</h4>
			    </div>
			    <div class="modal-body">
                    エラーメッセージが入ります
			    </div>
			    <div class="modal-footer">
				    <button type="button" class="btn btn-close no-ico mb0">閉じる</button>
			    </div>
		    </div>
	    </div>
    </div>

    @Html.Action("PortalFooterPartialView", "Portal")
    @QyHtmlHelper.RenderScriptTag("~/dist/js/portal/medicineconnection")
</body>
