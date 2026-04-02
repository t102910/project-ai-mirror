@Imports MGF.QOLMS.QolmsYappli
@ModelType PortalCompanyConnectionViewModel

@Code
    ViewData("Title") = "法人連携"
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
	    <section class="contents-area mb0">
		    <h2 class="title">@String.Format("{0}連携", Me.Model.LinkageSystemName)</h2>
		    <hr>
            <section class="section default">
			    連携が完了すると健診情報がある場合は１時間程度で閲覧いただけるようになります。
		    </section>

		    <section class="section center">
                @If Me.Model.StatusType = 2 Then
                    @<span class="ico main" style =""><i class="la la-check-circle-o"></i> 連携済み</span>
                End If
		    </section>
		    <h3 class="title">受け取る情報</h3>
		    <p class="mb40">
                <span class="ico line">検査・健診情報</span>
		    </p>
		    <h3 class="title">開示する情報</h3>
		    <p class="mb40">
                @For Each item As QyRelationContentTypeEnum In [Enum].GetValues(GetType(QyRelationContentTypeEnum))
                    
                    If item <> QyRelationContentTypeEnum.None AndAlso ((Me.Model.ShowType And item) = item) Then
                        @<span class="ico line">@QyDictionary.RelationContentType(item)</span>
                    End If
                Next
@*  
                <span class="ico line">基本情報</span>
                <span class="ico line">連絡手帳</span>
                <span class="ico line">バイタル手帳</span>
                <span class="ico line">お薬手帳</span>
                <span class="ico line">検査手帳</span>*@
		    </p>
            <p class="submit-area">
			   <a href="javascript:void(0);" id="edit" class="btn btn-submit" data-no="@Me.Model.LinkageSystemNo.ToString()">編集する</a>
			</p>
	    </section>
		
	    <section class="contents-area mb0">	
		    <h3 class="title mt10">連携を解除</h3>
		    連携を解除すると連携したユーザーが閲覧できなくなります。ご注意下さい。
		    <div class="submit-area">
			    <a href="javascript:void(0);" id="delete" class="btn btn-close" data-toggle="modal"  data-no="@Me.Model.LinkageSystemNo" >連携を解除する</a>
		    </div>
	    </section>
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
    </main>

    @Html.Action("PortalFooterPartialView", "Portal")
    @QyHtmlHelper.RenderScriptTag("~/dist/js/portal/companyconnection")

</body>
