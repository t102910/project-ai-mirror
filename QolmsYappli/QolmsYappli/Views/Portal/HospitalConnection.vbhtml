@Imports MGF.QOLMS.QolmsYappli
@ModelType PortalHospitalConnectionViewModel

@Code
    ViewData("Title") = "病院連携"
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

        <section class="contents-area mb0">
		    <h2 class="title">@String.Format("{0}連携", Me.Model.LinkageSystemName)</h2>
		    <hr>
@*            <div class="right">
                <a href="../portal/connectionsetting?FromPageNo=1&TabNo=3" class="btn btn-close no-ico">戻 る</a>
		    </div>*@
		    <section class="section center">

            @If Me.Model.StatusType = 1 Then
                @<span class="ico line"><i class="la la-clock-o la-lg"></i> 連携承認待ち</span> 

            ElseIf Me.Model.StatusType = 2 Then
                @<span class="ico main" style =""><i class="la la-check-circle-o"></i> 連携済み</span>

            ElseIf Me.Model.StatusType = 3 Then
                @<span class="ico line mb10" style =""><i class="la la-times"></i> 承認不可</span>

                If Not String.IsNullOrWhiteSpace(Me.Model.DisapprovedReason) Then
                    @<p>@Me.Model.DisapprovedReason</p>

                End If

            End If

		    </section>
	    </section>

        @If Me.Model.LinkageSystemNo <> 47012 Then

	        @<section class="contents-area">
		        <h3 class="title">受け取る情報</h3>
@*                <p class="section default">
                    現在一部のユーザー様で健診結果が正しく表示されていない事象が確認されています。<br />
                    2023年2月末頃の復旧を予定しております。
		        </p>*@
                    <p class="mb40">
                        <span class="ico line">健診情報</span>
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
       

	         </section>
            
            @<section class="contents-area">

                @If Me.Model.StatusType = 2 Then

                    @<p class="submit-area">
			           <a href="javascript:void(0);" id="edit" class="btn btn-submit" data-no="@Me.Model.LinkageSystemNo.ToString()">編集する</a>
			        </p>
                
                             
	                 @<section class="contents-area">
            
                        <div class="mb100">

                            <h3 class="title mt10">
				                <span>病院連携を解除する</span>
			                </h3>
			                <p class="mb20">
			                連携を解除すると連携した病院の健診データが閲覧できなくなります。<br>
			                ご注意下さい。
			                </p>
			                <section class="section caution mt10 mb10 hide"><h4>heading...</h4>アラートエリアです。不要時.hideで消してください。</section>
			                <p class="submit-area">
				                <a href="javascript:void(0);" id="cancel" class="btn btn-delete" data-no="@Me.Model.LinkageSystemNo.ToString()">解除する</a>
			                </p>
		                </div> 	   
	                </section>
        
                else

                    @<div>

                        <h3 class="title mt10">
				            <span>申請を編集する</span>
			            </h3>
			            <p class="mb20">
			            連携申請内容を編集します。<br>
			            </p>
			            <section class="section caution mt10 mb10 hide"><h4>heading...</h4>アラートエリアです。不要時.hideで消してください。</section>
			            <p class="submit-area">
				            <a href="javascript:void(0);" id="edit" class="btn btn-submit" data-no="@Me.Model.LinkageSystemNo.ToString()">編集する</a>
			            </p>
                        <h3 class="title mt10">
			                <span>申請を取り消す</span>
		                </h3>
                        <p class="mb20">
			                病院への連携申請が取り消されます。<br>
			                ご注意下さい。
			                </p>
			                <section  class="section caution mt10 mb10 hide"><h4>heading...</h4>アラートエリアです。不要時.hideで消してください。</section>
			                <p class="submit-area">
				            <a href="javascript:void(0);" id="cancel" class="btn btn-delete request" data-no="@Me.Model.LinkageSystemNo.ToString()">取り消す</a>
			            </p>
                     </div>
                End If
	            </section>
        End If

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
				    病院との連携を解除しますか？
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

    @QyHtmlHelper.RenderScriptTag("~/dist/js/portal/hospitalconnection")
</body>
