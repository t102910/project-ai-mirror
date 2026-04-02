@Imports MGF.QOLMS.QolmsYappli
@ModelType PortalConnectionSettingViewModel

@Code
    ViewData("Title") = "連携設定"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"
    Dim isTanitaConnected As Boolean = Me.Model.ConnectionSettingItems.ContainsKey(47005)
    Dim tanita As Integer
    Dim isAlkooConnected As Boolean = Me.Model.ConnectionSettingItems.ContainsKey(47006) AndAlso Me.Model.ConnectionSettingItems(47006).Status = 2
    Dim alkoo As Integer
    If isTanitaConnected Then
        tanita = 47005
    ElseIf isAlkooConnected Then
        alkoo = 47006
    End If

    Dim isKayoinobaConnected As Boolean = Me.Model.ConnectionSettingItems.ContainsKey(47010)
    Dim isFitbitConnected As Boolean = Me.Model.ConnectionSettingItems.ContainsKey(47011)
    
End Code

<body id="" class="lower">
    @Html.AntiForgeryToken()
    @*@Html.Action("PortalHeaderPartialView", "Portal")*@
    <main id="main-cont" class="clearfix" role="main">
       @Select Case Me.Model.FromPageNoType
           Case QyPageNoTypeEnum.PortalHome
                @<section class="home-btn-wrap">
                   <a href="../Portal/Home" class="home-btn"><i class="la la-angle-left"></i><i class="la la-home la-15x"></i><span> ホーム</span></a>
                </section>  
       End Select

	    <section class="contents-area mb0">
		    <h2 class="title">連携設定</h2>
		    <hr>
		    <section class="section default">
			    外部データ連携を設定します。
		    </section>
            <ul class="nav nav-tabs mb20">
			    <li class="@IIf(Me.Model.TabNoType < 2 OrElse Me.Model.TabNoType > 4, "active", String.Empty).ToString()"><a href="#input-1" data-toggle="tab"><span>アプリ</span></a></li>
			    <li class="@IIf(Me.Model.TabNoType = 2, "active", String.Empty).ToString()"><a href="#input-2" data-toggle="tab"><span>法 人</span></a></li>
			    <li class="@IIf(Me.Model.TabNoType = 3, "active", String.Empty).ToString()" ><a href="#input-3" data-toggle="tab"><span>病 院</span></a></li>
			    <li class="@IIf(Me.Model.TabNoType = 4, "active", String.Empty).ToString()" ><a href="#input-4" data-toggle="tab"><span>薬 局</span></a></li>
		    </ul>
	    </section>

        <section class="data-area tab-content">
		    <div class="tab-pane @IIf(Me.Model.TabNoType < 2 OrElse Me.Model.TabNoType > 4, "active", String.Empty).ToString()" id="input-1" >
			    @*現状はタブが１か３しかないので３以外の場合は１を表示することに*@
			     <article class="data-card">
			        <h3 class="card-title two-pane">
				        <span>タニタ連携</span>
				        <span class="right-area"><a class="ico" href="native:/tab/custom/782690f6"><i class="la la-pencil"></i>変更</a></span>
			        </h3>
			        <section class="inner">
				        <div class="federation-status low-height center">
					        <p class="@IIf(isTanitaConnected AndAlso Me.Model.ConnectionSettingItems(tanita).Devices.Contains(1), "on", "off")">体 重</p>
					        <p class="@IIf(isTanitaConnected AndAlso Me.Model.ConnectionSettingItems(tanita).Devices.Contains(2), "on", "off")">血 圧</p>
                            <p class="@IIf(isTanitaConnected AndAlso Me.Model.ConnectionSettingItems(tanita).Devices.Contains(3), "on", "off")">歩 数</p>
				        </div>
			        </section>
		        </article>
		        <article class="data-card">
			        <h3 class="card-title two-pane">
				        <span>ALKOO連携</span>
				        <span class="right-area"><a class="ico" href="native:/tab/custom/8287f3fa"><i class="la la-pencil"></i>変更</a></span>
			        </h3>
			        <section class="inner">
				        <div class="federation-status low-height center">
					        <p class="@IIf(isAlkooConnected, "on", "off")">歩 数</p>
				        </div>
			        </section>
		        </article>

@*                <article class="data-card">
			        <h3 class="card-title two-pane">
				        <span>オンライン通いの場連携</span>
				        <span class="right-area"><a class="ico" href="@String.Format("../portal/kayoinobaconnection{0}", IIf(Me.Model.FromPageNoType = QyPageNoTypeEnum.PortalHome, "?fromPageNo=1", String.Empty))"><i class="la la-pencil"></i>変更</a></span>
			        </h3>
			        <section class="inner">
				        <div class="federation-status low-height center">
					        <p class="@IIf(isKayoinobaConnected, "on", "off")">連 携</p>

				        </div>
			        </section>
		        </article>*@

                <article class="data-card">
			        <h3 class="card-title two-pane">
				        <span>Fitbit連携</span>
				        <span class="right-area"><a class="ico" href="@String.Format("../portal/fitbitconnection{0}", IIf(Me.Model.FromPageNoType = QyPageNoTypeEnum.PortalHome, "?fromPageNo=1", String.Empty))"><i class="la la-pencil"></i>変更</a></span>
			        </h3>
			        <section class="inner">
				        <div class="federation-status low-height center">
					        <p class="@IIf(isFitbitConnected, "on", "off")">連 携</p>
				        </div>
			        </section>
		        </article>


		    </div>
		    <div class="tab-pane @IIf(Me.Model.TabNoType = 2, "active", String.Empty).ToString()" id="input-2">
                @If Me.Model.ConnectionSettingCompanyItemN.Count = 0 Then

			        @<p class="center">
				        <a href="@String.Format("../portal/companyconnectionrequest{0}", IIf(Me.Model.FromPageNoType = QyPageNoTypeEnum.PortalHome, "?fromPageNo=1", String.Empty))" class="btn btn-submit">新規連携登録</a>
			        </p>
                End If

                @For Each item As KeyValuePair(Of Integer, ConnectionSettingCompanyItem) In Me.Model.ConnectionSettingCompanyItemN
			        @<a href="@String.Format("../portal/companyconnection?LinkageSystemNo={0}&fromPageNo={1}", item.Key, Convert.ToByte(Me.Model.FromPageNoType))" class="data-card">
				        <h3 class="card-title two-pane">
					        <span>@item.Value.LinkageSysyemName</span>
					        <span class="right-area">
                            
                                @If item.Value.Status = 1 Then
                                    @<span class="ico pending"><i class="la la-clock-o on"></i>連携承認待ち</span>
                                
                                ElseIf item.Value.Status = 2 Then
                                    @<span class="ico on"><i class="la la-check-circle-o"></i>連携済み</span>
                                End If

                            </span>
				        </h3>
					    <section class="inner">
					   	    <div class="federation-status low-height center">
					            <p class="on">健 診</p>
				            </div>
				        </section>
			        </a>                    
                                Next
		    </div>
		    <div class="tab-pane @IIf(Me.Model.TabNoType = 3, "active", String.Empty).ToString()" id="input-3">
			    <p class="center">
				    <a href="@String.Format("../portal/hospitalconnectionRequest?fromPageNo={0}", Convert.ToByte(Me.Model.FromPageNoType))" class="btn btn-submit">新規連携登録</a>

                    
			    </p>
                @For Each item As KeyValuePair(Of Integer, ConnectionSettingHospitalItem) In Me.Model.ConnectionSettingHospitalItemN
                    @<a href="@String.Format("../portal/hospitalconnection?LinkageSystemNo={0}&fromPageNo={1}", item.Key, Convert.ToByte(Me.Model.FromPageNoType))" class="data-card">
                        
				        <h3 class="card-title two-pane">
					        <span>@item.Value.LinkageSysyemName</span>
					        <span class="right-area">

                                @If item.Value.Status = 1 Then
                                    @<span class="ico pending"><i class="la la-clock-o on"></i>連携申請中</span>
                                
                                ElseIf item.Value.Status = 2 Then
                                    @<span class="ico on"><i class="la la-check-circle-o"></i>連携済み</span>
                                
                                ElseIf item.Value.Status = 3 Then
                                    @<span class="ico pending"><i class="la la-times"></i>承認不可</span>
                                
                                End If

					        </span>
				        </h3>
				        <section class="inner">
					   	    <div class="federation-status low-height center">
					            <p class="on">健 診</p>
				            </div>
				        </section>
			        </a>
                Next
	
		    </div>

            <div class="tab-pane @IIf(Me.Model.TabNoType = 4, "active", String.Empty).ToString()" id="input-4">
			    <p class="center">
				    <a href="@String.Format("../portal/MedicineConnectionSearch?fromPageNo={0}", Convert.ToByte(Me.Model.FromPageNoType))" class="btn btn-submit">新規連携登録</a>
                    
			    </p>
                @For Each item As KeyValuePair(Of Guid, ConnectionSettingPharmacyItem) In Me.Model.ConnectionSettingPharmacyItemN
                    @<a href="@String.Format("../portal/medicineconnection?LinkageSystemNo={0}&facilitykey={1}&fromPageNo={2}", item.Value.LinkageSystemNo, item.Key, Convert.ToByte(Me.Model.FromPageNoType))" class="data-card">
				        <h3 class="card-title two-pane">
					        <span>@item.Value.LinkageSysyemName</span>
					        <span class="right-area">

                                @If item.Value.Status = 1 Then
                                    @<span class="ico pending"><i class="la la-clock-o on"></i>連携申請中</span>
                                
                                ElseIf item.Value.Status = 2 Then
                                    @<span class="ico on"><i class="la la-check-circle-o"></i>連携済み</span>
                                
                                ElseIf item.Value.Status = 3 Then
                                    @<span class="ico pending"><i class="la la-times"></i>承認不可</span>
                                
                                End If

					        </span>
				        </h3>
				        <section class="inner">
					   	    <div class="federation-status low-height center">
					            <p class="on">おくすり</p>
				            </div>
				        </section>
			        </a>
                Next
	
		    </div>
	    </section>
    </main>

    @Html.Action("PortalFooterPartialView", "Portal")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/portal/tanitaconnection")
</body>
