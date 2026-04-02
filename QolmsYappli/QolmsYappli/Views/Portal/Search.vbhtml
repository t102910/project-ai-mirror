@Imports MGF.QOLMS.QolmsYappli
@ModelType PortalSearchViewModel

@Code
    ViewData("Title") = "医療機関検索"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"
End Code

<body id="search" class="lower">
    @Html.AntiForgeryToken()
    @*@Html.Action("PortalHeaderPartialView", "Portal")*@

    <main id="main-cont" class="clearfix" role="main">
        <section class="home-btn-wrap">
           <a href="../Portal/Home" class="home-btn"><i class="la la-angle-left"></i><i class="la la-home la-15x"></i><span> ホーム</span></a>
        </section>
	    <section class="contents-area mb20">
            <h2 class="title">@ViewData("Title")</h2>
            <hr />
		    <div class="input-area">
			    <div class="flex-wrap">
				    <input id="searchText" name="searchText" type="text" value="" class="form-control" placeholder="地域や医療機関名を入力" autocomplete="off">
				    <div class="hide-elm mt10">
                        <div class="input-group mb10">
						    <span class="input-group-addon">エリア　</span>
						    <select id="searchArea" class="form-control">
						        <option value="">全て</option>
						    @For Each item As SearchMstItem In Me.Model.AreaN
						        @<option value="@item.Key">@item.Value</option>
						    Next
						    </select>
					    </div>
                        <div id ="searchCity" class="input-group mb10">
						    <span class="input-group-addon">市区町村</span>
						    @For Each item As List(Of SearchMstItem) In Me.Model.CityN
						        @<select id="searchCity-@item.First().Key" class="form-control hide">
						            @For Each item2 As SearchMstItem In item
						                @<option value="@item2.SubKey">@item2.Value（@item2.SubValue）</option>
						            Next
						        </select>
						    Next
					    </div>
                   
					    <div  class="input-group mb10">
						    <span class="input-group-addon">診療科　</span>
						    <select id="searchDepartment" class="form-control">
						        <option value="">全て</option>
						    @For Each item As SearchMstItem In Me.Model.DepartmentN
						        @<option value="@item.Key">@item.Value</option>
						    Next
                           
						    </select>
					    </div>
					    <div  class="input-group mb10 checkbox-area">
                            <label class="inline-block mb5 mr10"><input id="open" type="checkbox" > 本日これから受診可能</label>
					        <label class="inline-block mb5 mr10"><input type="checkbox" data-type="@Integer.Parse(QyMedicalSearchTypeEnum.AuLaterPayment)"> 医療費あと払い対応</label>
					        <label class="inline-block mb5 mr10"><input type="checkbox" data-type="@Integer.Parse(QyMedicalSearchTypeEnum.NightTimeService)"> 夜間診療可能</label>
					        <label class="inline-block mb5 mr10"><input type="checkbox" data-type="@Integer.Parse(QyMedicalSearchTypeEnum.HolidayService)"> 日祝診療可能</label>
					        <label class="inline-block mb5 mr10"><input type="checkbox" data-type="@Integer.Parse(QyMedicalSearchTypeEnum.PharumoAutoRegistration)"> 薬局連携</label>
                        </div>
				
                    </div>
				    <div class="submit-area">
					    <a href="javascript:void(0);" class="btn btn-submit no-ico disabled"><i class="la la-search"></i> 検索</a>
				    </div>
			    </div>
			    <section class="section caution mt10 mb0 hide"><h4>heading...</h4>アラートエリアです。不要時.hideで消してください。</section>
		    </div>
		    <p class="right"><a href="#" id="input-all-2"><i class="la la-search"></i>詳細検索</a></p>
        </section>
	
	    <section class="data-area">
                @Html.Action("PortalSearchResultPartialView", "Portal")
	    </section>

@*	<section class="premium-btn">
		<a href="" class="btn btn-default mb20">
			<span><img src="/dist/img/tmpl/premium.png"> 短い文言で説明はいります。</span>
			<em class="logona">健康年齢の測定はこちら</em>
		</a>
	</section>*@
    </main>
    <div class="modal fade detail" id="detail-modal" data-backdrop="static" data-keyboard="false" tabindex="-1">
	    <div class="modal-dialog">
            @*医療機関詳細*@ 
            @Html.Action("PortalSearchDetailResultPartialView", "Portal")
        </div>
    </div>
    @Html.Action("PortalFooterPartialView", "Portal")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/portal/search")

    <script type="text/javascript">

        ////マップ
        function initMap(lat, lng) {
            if (lat === undefined & lng === undefined) {
                return false;
            }
            var map;
            var marker;
            var center = {
                lat: lat,
                lng: lng
            };
            map = new google.maps.Map(document.getElementById('map'), {
                center: center,
                zoom: 17
            });
            marker = new google.maps.Marker({
                position: center,
                map: map
            });
            return false;
        }

        </script>
    <script src="https://maps.google.com/maps/api/js?key=AIzaSyB4_4sZkEFpqeRDHsmm82DVrVAOsxhoO9E&amp;callback=initMap" "="" type="text/javascript"></script>
    <!-- APIキーを取得し直して下さい -->
</body>