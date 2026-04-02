@Imports MGF.QOLMS.QolmsYappli
@ModelType PortalMedicineConnectionSearchViewModel

@Code
    ViewData("Title") = "連携先薬局"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"
End Code

<body id="search" class="lower">
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

	    <section class="contents-area mb20">
            <h2 class="title">@ViewData("Title")</h2>
            <hr />

            <p class="section default">
                お薬情報を連携する薬局を以下より選択下さい。
		    </p>
        </section>
	

	    <section class="data-area">
            @For Each item As KeyValuePair(Of Guid, MedicineConnectionFacilityItem) In Me.Model.MedicineConnectionFacilityItemN
                @<a class="article facility" href="javascript:void(0);" data-facilitykey="@item.Key" data-linkage="47009">
				    <section class="inner">
					    <div class="info">
						    <p class="kana"><span>@item.Value.KanaName</span></p>
						    <h4 class="name">@item.Value.Name</h4>
						    <p class="address">@item.Value.Address</p>
                            @*<p class="departments">
                                    <span class="special">#タグ１</span>
                                    <span>情報１</span>
                            </p>*@
					    </div>
				    </section>
			    </a>
    
            Next
            @QyHtmlHelper.Pager(Me.Model.PageIndex, Me.Model.PageCount)

	    </section>
    </main>
    @Html.Action("PortalFooterPartialView", "Portal")
    @QyHtmlHelper.RenderScriptTag("~/dist/js/portal/medicineconnectionsearch")

</body>