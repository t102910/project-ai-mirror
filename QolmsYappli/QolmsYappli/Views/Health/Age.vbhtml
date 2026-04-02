@Imports MGF.QOLMS.QolmsYappli
@ModelType HealthAgeViewModel

@Code
    ViewData("Title") = "健康年齢"
    Layout = "~/Views/Shared/_HealthLayout.vbhtml"
    
    Dim hasData As Boolean = False 
End Code

<body id="health-age" class="lower">
     @Html.AntiForgeryToken()
     @*@Html.Action("HealthHeaderPartialView", "Health")*@

    <main id="main-cont mb100" class="clearfix mt10" role="main" style="margin-top: 15px;">
       @Select Case Me.Model.FromPageNoType
           Case QyPageNoTypeEnum.PortalHome
                @<section class="home-btn-wrap" data-pageno="1">
                   <a href="../Portal/Home" class="home-btn"><i class="la la-angle-left"></i><i class="la la-home la-15x"></i><span> ホーム</span></a>
                </section>  
           Case QyPageNoTypeEnum.NoteExamination
                @<section class="home-btn-wrap" data-pageno="23">
                   <a href="../Note/Examination" class="home-btn"><i class="la la-angle-left"></i><i class="la la-stethoscope la-15x"></i><span> 健診結果</span></a>
                </section>  
       End Select

	    <section class="contents-area">
            <h2 class="title">@ViewData("Title")</h2>
            <hr />
		    <section id="age-area">
                @If Me.Model.HealthAge > Decimal.Zero Then
                    If Me.Model.HealthAgeDistance > Decimal.Zero Then
                        @<div id="result">
                            <p class="inner age-result">
                                <span>あなたの健康年齢は</span>
                                <em class="logona bad">@Me.Model.HealthAge.ToString("0.#")</em>
                                <i>歳です</i>
                            </p>
                            <p class="inner high-low">
                                実際より<span class="high-light">@Math.Abs(Me.Model.HealthAgeDistance).ToString("0.#")</span>歳<span class="high-light high">高い</span>です
                            </p>
                            <p class="date">@Me.Model.LatestDate.ToString("yyyy年M月d日現在")</p>
                        </div>
			            @<p id="result-img">
				            <img src="../dist/img/health-age/bad.png">
				            <a id="view-more" href="javascript:void(0);">
					            <i class="la la-pencil-square-o"></i>
					            測定する
				            </a>
			            </p>
                    ElseIf Me.Model.HealthAgeDistance < Decimal.Zero Then
                        @<div id="result">
                            <p class="inner age-result">
                                <span>あなたの健康年齢は</span>
                                <em class="logona good">@Me.Model.HealthAge.ToString("0.#")</em>
                                <i>歳です</i>
                            </p>
                            <p class="inner high-low">
                                実際より<span class="high-light">@Math.Abs(Me.Model.HealthAgeDistance).ToString("0.#")</span>歳<span class="high-light low">低い</span>です
                            </p>
                            <p class="date">@Me.Model.LatestDate.ToString("yyyy年M月d日現在")</p>
                        </div>
			            @<p id="result-img">
				            <img src="../dist/img/health-age/good.png">
				            <a id="view-more" href="javascript:void(0);">
					            <i class="la la-pencil-square-o"></i>
					            測定する
				            </a>
			            </p>
                    Else
                        @<div id="result">
                            <p class="inner age-result">
                                <span>あなたの健康年齢は</span>
                                <em class="logona good">@Me.Model.HealthAge.ToString("0.#")</em>
                                <i>歳です</i>
                            </p>
                            <p class="inner high-low">
                                実際と同じです
                            </p>
                            <p class="date">@Me.Model.LatestDate.ToString("yyyy年M月d日現在")</p>
                        </div>
			            @<p id="result-img">
				            <img src="../dist/img/health-age/usual.png">
				            <a id="view-more" href="javascript:void(0);">
					            <i class="la la-pencil-square-o"></i>
					            測定する
				            </a>
			            </p>
                    End If
                    
                Else
                    @<div id="result">
                        <p class="inner age-result">
                            <span>あなたの健康年齢は</span>
                            <em class="logona">未測定</em>
                        </p>
                        <p class="inner high-low"></p>
                        <p class="date">@Date.Now.ToString("yyyy年M月d日現在")</p>
                    </div>
			        @<p id="result-img">
				        <img src="../dist/img/health-age/usual.png">
				        <a id="view-more" href="javascript:void(0);">
					        <i class="la la-pencil-square-o"></i>
                            測定する
				        </a>
			        </p>
                End If
		    </section>
	    </section>

        @If Me.Model.MembershipType = QyMemberShipTypeEnum.Premium _
            OrElse Me.Model.MembershipType = QyMemberShipTypeEnum.LimitedTime _
            OrElse Me.Model.MembershipType = QyMemberShipTypeEnum.Business _
            OrElse Me.Model.MembershipType = QyMemberShipTypeEnum.BusinessFree _
            OrElse Me.Model.HasHospitalConnection Then
	        @<div id="data-area" class=""style="padding-bottom: 100px;">
		        <section class="inner">
			        @Html.Action("HealthAgeAdviceAreaPartialView", "Health")
                    
                    @Html.Action("HealthAgeTransitionAreaPartialView", "Health")

			        <h3 class="title mt10">
				        <span>健康年齢の詳細</span>
			        </h3>
			
                    @If Me.Model.IsAvailableHealthAgeReportType(QyHealthAgeReportTypeEnum.Distribution, hasData) AndAlso hasData Then
                        @<article id="distribution-area" class="reload-area" data-report-type="@QyHealthAgeReportTypeEnum.Distribution.ToString()">
                            <div class="reload-area loading"></div>
                        </article>
                    Else
                        @<article id="distribution-area" class="reload-area" data-report-empty-type="@QyHealthAgeReportTypeEnum.Distribution.ToString()">
                            <section class="section caution mt10 mb0">
                                未測定です
                            </section>
                        </article>
                    End If

                    @If Me.Model.IsAvailableHealthAgeReportType(QyHealthAgeReportTypeEnum.Fat, hasData) AndAlso hasData Then
                        @<article id="fat-area" class="reload-area" data-report-type="@QyHealthAgeReportTypeEnum.Fat.ToString()">
                            <h2 class="box-title">
                                <b>肥満</b>についてのレポート
                            </h2>

                            <div class="reload-area loading"></div>
                        </article>
                    Else
                        @<article id="fat-area" class="reload-area" data-report-empty-type="@QyHealthAgeReportTypeEnum.Fat.ToString()">
                            <h2 class="box-title">
                                <b>肥満</b>についてのレポート
                            </h2>

                            <section class="section caution mt10 mb0">
                                未測定です
                            </section>
                        </article>
                    End If

                    @If Me.Model.IsAvailableHealthAgeReportType(QyHealthAgeReportTypeEnum.Glucose, hasData) AndAlso hasData Then
                       @<article id="glucose-area" class="reload-area" data-report-type="@QyHealthAgeReportTypeEnum.Glucose.ToString()">
                            <h2 class="box-title">
                                <b>血糖</b>についてのレポート
                            </h2>

                            <div class="reload-area loading"></div>
                        </article>
                    Else
                        @<article id="glucose-area" class="reload-area" data-report-empty-type="@QyHealthAgeReportTypeEnum.Glucose.ToString()">
                            <h2 class="box-title">
                                <b>血糖</b>についてのレポート
                            </h2>

                            <section class="section caution mt10 mb0">
                                未測定です
                            </section>
                        </article>
                    End If

                    @If Me.Model.IsAvailableHealthAgeReportType(QyHealthAgeReportTypeEnum.Pressure, hasData) AndAlso hasData Then
                       @<article id="pressure-area" class="reload-area" data-report-type="@QyHealthAgeReportTypeEnum.Pressure.ToString()">
                            <h2 class="box-title">
                                <b>血圧</b>についてのレポート
                            </h2>

                            <div class="reload-area loading"></div>
                        </article>
                    Else
                        @<article id="pressure-area" class="reload-area" data-report-empty-type="@QyHealthAgeReportTypeEnum.Pressure.ToString()">
                            <h2 class="box-title">
                                <b>血圧</b>についてのレポート
                            </h2>

                            <section class="section caution mt10 mb0">
                                未測定です
                            </section>
                        </article>
                    End If

                    @If Me.Model.IsAvailableHealthAgeReportType(QyHealthAgeReportTypeEnum.Lipid, hasData) AndAlso hasData Then
                       @<article id="lipid-area" class="reload-area" data-report-type="@QyHealthAgeReportTypeEnum.Lipid.ToString()">
                            <h2 class="box-title">
                                <b>脂質</b>についてのレポート
                            </h2>

                            <div class="reload-area loading"></div>
                        </article>
                    Else
                        @<article id="lipid-area" class="reload-area" data-report-empty-type="@QyHealthAgeReportTypeEnum.Lipid.ToString()">
                            <h2 class="box-title">
                                <b>脂質</b>についてのレポート
                            </h2>

                            <section class="section caution mt10 mb0">
                                未測定です
                            </section>
                        </article>
                    End If

                    @If Me.Model.IsAvailableHealthAgeReportType(QyHealthAgeReportTypeEnum.Liver, hasData) AndAlso hasData Then
                       @<article id="liver-area" class="reload-area" data-report-type="@QyHealthAgeReportTypeEnum.Liver.ToString()">
                            <h2 class="box-title">
                                <b>肝臓</b>についてのレポート
                            </h2>

                            <div class="reload-area loading"></div>
                        </article>
                    Else
                        @<article id="liver-area" class="reload-area" data-report-empty-type="@QyHealthAgeReportTypeEnum.Liver.ToString()">
                            <h2 class="box-title">
                                <b>肝臓</b>についてのレポート
                            </h2>

                            <section class="section caution mt10 mb0">
                                未測定です
                            </section>
                        </article>
                    End If

                    @If Me.Model.IsAvailableHealthAgeReportType(QyHealthAgeReportTypeEnum.Urine, hasData) AndAlso hasData Then
                       @<article id="urine-area" class="reload-area" data-report-type="@QyHealthAgeReportTypeEnum.Urine.ToString()">
                            <h2 class="box-title">
                                <b>尿糖・尿蛋白</b>についてのレポート
                            </h2>

                            <div class="reload-area loading"></div>
                        </article>
                    Else
                        @<article id="urine-area" class="reload-area" data-report-empty-type="@QyHealthAgeReportTypeEnum.Urine.ToString()">
                            <h2 class="box-title">
                                <b>尿糖・尿蛋白</b>についてのレポート
                            </h2>

                            <section class="section caution mt10 mb0">
                                未測定です
                            </section>
                        </article>
                    End If
		        </section>
	        </div>
        End If
    </main>

    @Html.Action("HealthFooterPartialView", "Health")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/health/age")
</body>
