@Imports MGF.QOLMS.QolmsYappli
@ModelType PortalMedicineConnectionAgreementViewModel

@Code
    ViewData("Title") = "薬局連携同意"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"
    
End Code

<body id="" class="lower">
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

	    <section class="contents-area">
            <h2 class="title">@ViewData("Title")</h2>
		    <hr>
            <p class="section default">
					@Html.Raw(Me.Model.TermsString)
			</p>
                    
        </section>
        <section class="contents-area fixed">
	        <p class="submit-area mb30 mr10">
				<a href="@String.Format("../portal/medicineconnectionsearch?fromPageNo={0}", Convert.ToByte(Me.Model.FromPageNoType))"" id="back" class="btn btn-close">同意しない</a>
				<a href="@String.Format("../portal/medicineconnectionRequest?linkageSystemNo={0}&facilitykey={1}&fromPageNo={2}", Me.Model.LinkageSystemNo, Me.Model.Facilitykey,Convert.ToByte(Me.Model.FromPageNoType))" id="submit" class="btn btn-submit">同意する</a>
			</p>
        </section>
            
        <div class="modal fade" id="error-modal" tabindex="-1" data-backdrop="static" data-keyboard="false" data-row-no="-1">
	        <div class="modal-dialog">
		        <div class="modal-content">
			        <div class="modal-header">
				        <button type="button" class="close"><span>×</span></button>
				        <h4 class="modal-title">エラー</h4>
			        </div>
			        <div class="modal-body">
				           
			        </div>
			        <div class="modal-footer">
				        <button type="button" class="btn btn-close no-ico mb0">閉じる</button>
			        </div>
		        </div>
	        </div>
        </div>

        @If Not String.IsNullOrWhiteSpace(ViewData("ErrorMessage")) Then
            @<div class="modal fade" id="error-modal2" tabindex="-1" data-backdrop="static" data-keyboard="false" data-row-no="-1">
	            <div class="modal-dialog">
		            <div class="modal-content">
			            <div class="modal-header">
				            <button type="button" class="close"><span>×</span></button>
				            <h4 class="modal-title">エラー</h4>
			            </div>
			            <div class="modal-body">
				            @ViewData("ErrorMessage")
			            </div>
			            <div class="modal-footer">
				            <button type="button" class="btn btn-close no-ico mb0">閉じる</button>
			            </div>
		            </div>
	            </div>
            </div>
        End If

    </main>
    
    @Html.Action("PortalFooterPartialView", "Portal")

</body>