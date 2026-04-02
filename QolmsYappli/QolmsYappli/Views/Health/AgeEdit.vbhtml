@Imports MGF.QOLMS.QolmsYappli
@ModelType HealthAgeEditInputModel

@Code
    ViewData("Title") = "健康年齢測定"
    Layout = "~/Views/Shared/_HealthLayout.vbhtml"
End Code

<body id="health-age-input" class="lower">
    @Html.AntiForgeryToken()
    @*@Html.Action("HealthHeaderPartialView", "Health")*@

    <main id="main-cont" class="clearfix" role="main">
	    <section class="contents-area">
            @If Me.Model.FromPageNoType = QyPageNoTypeEnum.PortalHome Then
                @<div id="page-no"data-pageno="1"></div>

            ElseIf Me.Model.FromPageNoType = QyPageNoTypeEnum.NoteExamination Then
                @<div id="page-no"data-pageno="23"></div>
                
            Else
                @<div id="page-no"data-pageno="0"></div>
                
            End If
            
            @If Not Me.Model.IsMaintenance Then
		        @<section class="section default">
			        前回入力された数値がセットされています。<br>
                    変更があれば修正して送信してください。<br>
			        変更しない項目はそのまま送信してください。
		        </section>
		        @<form class="form">
                    <label class="t-row">
				        <span class="label-txt">
                            <span class="ico required">必須</span>
                            健診受診日
				        </span>
                        @If Me.Model.RecordDate = Date.MinValue Then
                            @<input type="text" name="RecordDate" class="form-control picker" value="@Date.Now.ToString("yyyy年MM月dd日")" readonly="readonly" style="background-color:white;" autocomplete="off">
                        Else
                            @<input type="text" name="RecordDate" class="form-control picker" value="@Me.Model.RecordDate.ToString("yyyy年MM月dd日")" readonly="readonly" style="background-color:white;" autocomplete="off">    
                        End If
                        @QyHtmlHelper.ToValidationMessageInSectionTag({"model.RecordDate"}, Me.TempData("ErrorMessage"))
                    </label>

			        <label class="t-row">
				        <span class="label-txt">
					        <span class="ico required">必須</span>
					        BMI<i>@Me.Model.GetLatestDateString(QyHealthAgeValueTypeEnum.BMI)</i>
				        </span>
				        <input type="number" name="BMI" class="form-control" value="@Me.Model.BMI" placeholder="10.0～100.0" maxlength="5" style="ime-mode:disabled;" autocomplete="off">
                        @QyHtmlHelper.ToValidationMessageInSectionTag({"model.BMI"}, Me.TempData("ErrorMessage"))
			        </label>

			        <label class="t-row">
				        <span class="label-txt">
					        <span class="ico required">必須</span>
					        血圧（上）<i>@Me.Model.GetLatestDateString(QyHealthAgeValueTypeEnum.Ch014)</i>
				        </span>
				        <input type="tel" name="Ch014" class="form-control" value="@Me.Model.Ch014" placeholder="60～300" maxlength="3" style="ime-mode:disabled;" autocomplete="off">
                        @QyHtmlHelper.ToValidationMessageInSectionTag({"model.Ch014"}, Me.TempData("ErrorMessage"))
			        </label>

			        <label class="t-row">
				        <span class="label-txt">
					        <span class="ico required">必須</span>
					        血圧（下）<i>@Me.Model.GetLatestDateString(QyHealthAgeValueTypeEnum.Ch016)</i>
				        </span>
				        <input type="tel" name="Ch016" class="form-control" value="@Me.Model.Ch016" placeholder="30～150" maxlength="3" style="ime-mode:disabled;" autocomplete="off">
                        @QyHtmlHelper.ToValidationMessageInSectionTag({"model.Ch016"}, Me.TempData("ErrorMessage"))
			        </label>

			        <label class="t-row">
				        <span class="label-txt">
					        <span class="ico required">必須</span>
					        中性脂肪<i>@Me.Model.GetLatestDateString(QyHealthAgeValueTypeEnum.Ch019)</i>
				        </span>
				        <input type="tel" name="Ch019" class="form-control" value="@Me.Model.Ch019" placeholder="10～2000" maxlength="4" style="ime-mode:disabled;" autocomplete="off">
                        @QyHtmlHelper.ToValidationMessageInSectionTag({"model.Ch019"}, Me.TempData("ErrorMessage"))
			        </label>

			        <label class="t-row">
				        <span class="label-txt">
					        <span class="ico required">必須</span>
					        HDLコレステロール<i>@Me.Model.GetLatestDateString(QyHealthAgeValueTypeEnum.Ch021)</i>
				        </span>
				        <input type="number" name="Ch021" class="form-control" value="@Me.Model.Ch021" placeholder="10.0～500.0" maxlength="5" style="ime-mode:disabled;" autocomplete="off">
                        @QyHtmlHelper.ToValidationMessageInSectionTag({"model.Ch021"}, Me.TempData("ErrorMessage"))
			        </label>

			        <label class="t-row">
				        <span class="label-txt">
					        <span class="ico required">必須</span>
					        LDLコレステロール<i>@Me.Model.GetLatestDateString(QyHealthAgeValueTypeEnum.Ch023)</i>
				        </span>
				        <input type="number" name="Ch023" class="form-control" value="@Me.Model.Ch023" placeholder="20.0～1000.0" maxlength="6" style="ime-mode:disabled;" autocomplete="off">
                        @QyHtmlHelper.ToValidationMessageInSectionTag({"model.Ch023"}, Me.TempData("ErrorMessage"))
			        </label>

			        <label class="t-row">
				        <span class="label-txt">
					        <span class="ico required">必須</span>
					        AST（GOT）<i>@Me.Model.GetLatestDateString(QyHealthAgeValueTypeEnum.Ch025)</i>
				        </span>
				        <input type="tel" name="Ch025" class="form-control" value="@Me.Model.Ch025" placeholder="1～1000" maxlength="4" style="ime-mode:disabled;" autocomplete="off">
                        @QyHtmlHelper.ToValidationMessageInSectionTag({"model.Ch025"}, Me.TempData("ErrorMessage"))
			        </label>

			        <label class="t-row">
				        <span class="label-txt">
					        <span class="ico required">必須</span>
					        ALT（GPT）<i>@Me.Model.GetLatestDateString(QyHealthAgeValueTypeEnum.Ch027)</i>
				        </span>
				        <input type="tel" name="Ch027" class="form-control" value="@Me.Model.Ch027" placeholder="1～1000" maxlength="4" style="ime-mode:disabled;" autocomplete="off">
                        @QyHtmlHelper.ToValidationMessageInSectionTag({"model.Ch027"}, Me.TempData("ErrorMessage"))
			        </label>

			        <label class="t-row">
				        <span class="label-txt">
					        <span class="ico required">必須</span>
					        γ-GT（γ-GTP）<i>@Me.Model.GetLatestDateString(QyHealthAgeValueTypeEnum.Ch029)</i>
				        </span>
				        <input type="tel" name="Ch029" class="form-control" value="@Me.Model.Ch029" placeholder="1～1000" maxlength="4" style="ime-mode:disabled;" autocomplete="off">
                        @QyHtmlHelper.ToValidationMessageInSectionTag({"model.Ch029"}, Me.TempData("ErrorMessage"))
			        </label>

			        <label class="t-row">
				        <span class="label-txt">
					        <span class="ico required">必須</span>
					        HbA1c（NGSP）<i>@Me.Model.GetLatestDateString(QyHealthAgeValueTypeEnum.Ch035)</i>
				        </span>
				        <input type="number" name="Ch035" class="form-control" value="@Me.Model.Ch035" placeholder="3.0～20.0" maxlength="4" style="ime-mode:disabled;" autocomplete="off">
                        @QyHtmlHelper.ToValidationMessageInSectionTag({"model.Ch035"}, Me.TempData("ErrorMessage"))
			        </label>

			        <label class="t-row">
				        <span class="label-txt">
					        <span class="ico required">必須</span>
					        空腹時血糖<i>@Me.Model.GetLatestDateString(QyHealthAgeValueTypeEnum.Ch035FBG)</i>
				        </span>
				        <input type="tel" name="Ch035FBG" class="form-control" value="@Me.Model.Ch035FBG" placeholder="20～600" maxlength="3" style="ime-mode:disabled;" autocomplete="off">
                        @QyHtmlHelper.ToValidationMessageInSectionTag({"model.Ch035FBG"}, Me.TempData("ErrorMessage"))
			        </label>

			        <label class="t-row">
				        <span class="label-txt">
					        <span class="ico required">必須</span>
					        尿糖<i>@Me.Model.GetLatestDateString(QyHealthAgeValueTypeEnum.Ch037)</i>
				        </span>
                        @QyHtmlHelper.ToUrineDropDownList(String.Empty, "Ch037", Me.Model.Ch037)
                        @QyHtmlHelper.ToValidationMessageInSectionTag({"model.Ch037"}, Me.TempData("ErrorMessage"))
			        </label>

			        <label class="t-row">
				        <span class="label-txt">
					        <span class="ico required">必須</span>
					        尿蛋白（定性）<i>@Me.Model.GetLatestDateString(QyHealthAgeValueTypeEnum.Ch039)</i>
				        </span>
                        @QyHtmlHelper.ToUrineDropDownList(String.Empty, "Ch039", Me.Model.Ch039)
                        @QyHtmlHelper.ToValidationMessageInSectionTag({"model.Ch039"}, Me.TempData("ErrorMessage"))
			        </label>

			        <div class="submit-area  mb100">
                        @QyHtmlHelper.ToValidationMessageInSectionTag({"HealthAgeApi"}, Me.TempData("ErrorMessage"), css:="section caution mt0 mb10")
                        @If Me.Model.FromPageNoType = QyPageNoTypeEnum.PortalHome Then
                            @<a href="../Health/Age?fromPageNo=1" class="btn btn-close no-ico">戻 る</a>

                        ElseIf Me.Model.FromPageNoType = QyPageNoTypeEnum.NoteExamination Then
                            @<a href="../Health/Age?fromPageNo=23" class="btn btn-close no-ico">戻 る</a>
                
                        Else
                            @<a href="../Health/Age" class="btn btn-close no-ico">戻 る</a>
                
                        End If
				        <a href="javascript:void(0);" class="btn btn-submit">送 信</a>
			        </div>
		        </form>
            Else
		        @<section class="section caution">
			        @QyHtmlHelper.CrLfToBreakTag(Me.Model.MaintenanceMessage)
		        </section>
                @<form class="form">
			        <div class="submit-area">
                        @If Me.Model.FromPageNoType = QyPageNoTypeEnum.PortalHome Then
                            @<a href="../Health/Age?fromPageNo=1" class="btn btn-close no-ico">戻 る</a>

                        ElseIf Me.Model.FromPageNoType = QyPageNoTypeEnum.NoteExamination Then
                            @<a href="../Health/Age?fromPageNo=23" class="btn btn-close no-ico">戻 る</a>
                
                        Else
                            @<a href="../Health/Age" class="btn btn-close no-ico">戻 る</a>
                
                        End If
			        </div>
                 </form>
            End If
	    </section>
    </main>

    @Html.Action("HealthFooterPartialView", "Health")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/health/ageedit")
</body>
