@Imports MGF.QOLMS.QolmsYappli
@ModelType NoteMealListInputModel

@Code
    ViewData("Title") = "食事"
    Layout = "~/Views/Shared/_NoteLayout.vbhtml"
    
    Dim resultCount As Integer = 0
End Code

<body id="meal" class="lower" data-next="@Me.Model.InputNext" data-search="@Me.Model.SearchFlag.ToString()">
    @Html.AntiForgeryToken()
    @*@Html.Action("NoteHeaderPartialView", "Note")*@

<main id="main-cont" class="clearfix" role="main">
	<section class="contents-area mb100">
		<div class="input-area" id="input-1">
@*			<div class="target-date-time">
                <div class="input-next hide"><!-- 続けて登録時に表示する -->
					<p class="input-next"><strong><em>12月23日</em>（<em>午前10時13分</em>）の<em>朝食</em></strong>を<span class="inline-block">続けて登録する</span></p>
				</div><!-- input-next end -->	

				<div class="next-hide"><!-- 続けて登録時に隠す -->
				    <div class="input-group mb10">
					    <span class="input-group-addon">食事日</span>
                        <input type="text" id="record-date" class="form-control picker" name="record-date" value="@String.Format("{0:yyyy年MM月dd日}", Me.Model.RecordDate)" readonly="readonly" style="background-color:white;" autocomplete="off">
				    </div>
                    @QyHtmlHelper.ToMealTypeRadioButton("mealtype", "mealtype", Me.Model.MealType.ToString(), css:="mb10 mealtime")
					<div class="input-group datetime-select">
				        <span class="input-group-addon">時間</span>
				        @QyHtmlHelper.ToMeridiemDropDownList("meridiem", "meridiem", Me.Model.Meridiem, css:="form-control ap")
                        @QyHtmlHelper.ToHourDropDownList("hour", "hour", Me.Model.Hour, css:="form-control hour")
                        @QyHtmlHelper.ToMinuteDropDownList("minute", "minute", Me.Model.Minute, css:="form-control minute")
					</div>
                </div>
			</div>*@
			
            @Html.Action("NoteMealEditDatePartialView", "Note")

		    <div class="input-area-inner">
                <div class="inline-input hide"><!-- 登録直後のみ表示する -->
				    <div class="img-upload">
					    <label for="sample1">
						    <i class="la la-camera"></i>続けて写真から登録
						    <input type="file" id="sample1" accept="image/*" disabled="disabled">
					    </label>
				    </div>
			    </div>

                <section class="section caution mt10 mb10 hide" id="photo-caution"><h4>heading...</h4>アラートエリアです。不要時.hideで消してください。</section>

                <div class="analyzed-area"><!-- 登録直後のみ非表示にする -->
@*			        <div class="thumbnail-area">
				        <p class="meal-photo">
                            <img src="@QyAccountItemBase.CreateThumbnailPhotoUri(Me.Model.ForeignKey(0), 1, "Storage", "MealFile", QyAccountItemBase.EncryptPhotoReference(Me.Model.AuthorKey, Me.Model.ForeignKey(0), QyFileTypeEnum.Thumbnail))" data-foreignkey="@Me.Model.ForeignKey(0)">
				        </p>
			        </div>
			        <h4 class="line center">解析結果</h4>
                    @If Me.Model.FoodN.Any() AndAlso Me.Model.FoodN(0).Any() Then
                        @<div class="item search">
                        @For Each item As FoodItem In Me.Model.FoodN(0)
                            
                            resultCount += 1
                            
                            If resultCount >= 10 Then
                                Exit For
                            End If
                            
                            Dim setSpan As String = item.label + "（" + item.calorie + "kcal）"
                            Dim palString As String = item.label _
                                + "," + item.calorie _
                                + "," + item.protein _
                                + "," + item.lipid _
                                + "," + item.carbohydrate _
                                + "," + item.salt_amount _
                                + "," + item.available_carbohydrate _
                                + "," + item.fiber + ","
                            @<a href="javascript:void(0);" class="meal" data-name="@item.label" data-cal="@item.calorie" data-pal="@palString"><span>@setSpan</span></a>
                        Next
                        </div>
                    Else
                        @<p>解析できませんでした</p>
                    End If
			        
                    <!-- 現状、3件が上限なので10件以上となることはない -->
                    @If resultCount >= 10 Then
				        @<a href="javascript:void(0);" class="btn btn-submit white block mt20 mb10">続きを表示</a>
                    End If*@

                    @Html.Action("NoteMealAnalyzeResultPartialView", "Note")

			        <hr class="line">
			        <div class="navigation-area">
				        <p class="balloon">
					        解析結果が見つからなかったら検索してね！
				        </p>
				        <img class="shika-image" src="/dist/img/tmpl/s-1.png">
			        </div>
@*			        <div class="flex-wrap">
				        <div class="inline-input">
					        <div id="search-input" class="w-max">
						        <div class="input-group w-max">
							        <span class="input-group-addon font-s">品名検索</span>
                                    <input id="searchText" name="searchText" type="text" value="" class="form-control" placeholder="品名を入力" maxlength="100" autocomplete="off">
						        </div>
					        </div>
				        </div>
				        <div class="submit-area">
					        <span href="javascript:void(0);" class="btn btn-submit search search-result-btn disabled" id="search">検索</span>
				        </div>
			        </div>*@

                    @Html.Action("NoteMealEditSearchAreaPartialView", "Note")

			        <hr class="line">
			        <div class="navigation-area">
				        <p class="balloon">
					        品名・カロリーを入力して登録することもできます。
				        </p>
				        <img class="shika-image" src="/dist/img/tmpl/s-6.png">
			        </div>
@*			        <div class="input-group mb10">
				        <span class="input-group-addon">品目</span>
                        <input id="hinmoku-2" name="hinmoku" type="text" value="@Me.Model.ItemName(0)" class="form-control input" placeholder="品目を入力" maxlength="100" autocomplete="off">
			        </div>
			        <div class="input-group mb10" class="line">
				        <span class="input-group-addon">カロリー</span>
                        <input id="cal-2" name="cal" type="tel" value="@Me.Model.Calorie(0)" class="form-control input" placeholder="カロリーを入力" maxlength="4" style="ime-mode:disabled;" autocomplete="off">
				        <span class="input-group-addon">kcal</span>
			        </div>*@

                    @Html.Action("NoteMealEditHinmokuCalPartialView", "Note")

                </div>

			    <div class="submit-area mb30">
                    <!-- 続けて登録時に隠す -->
                    <a href="/Note/Meal4" class="btn btn-close" id="back">戻 る</a>
				    <a href="javascript:void(0);" class="btn btn-submit upload" id="addfirstS">登 録</a>

                    <!-- 続けて登録時に表示する -->
                    <a href="/Note/Meal4" class="btn btn-close upload hide" id="finish">登録完了</a>
                    <a href="javascript:void(0);" class="btn btn-submit upload disabled hide" id="addnext"
                        data-recorddate="" data-mealtype="" data-mealtypetext="" data-meridiem="" data-hour="" data-minute="">登 録</a>
			    </div>
                <section class="section caution mt10 mb10 hide" id="caution"><h4>heading...</h4>アラートエリアです。不要時.hideで消してください。</section>
		    </div>
        </div>
	</section>

    <!-- サムネイル編集エリア -->
@*    <div id="thumbnail-edit">
	    <div class="thumbnail-area">
		    <p class="meal-photo">
			    <img src="" data-file-name="">
		    </p>
	    </div>
	    <div class="submit-area type-2">
		    <a href="javascript:void(0);" id="thumbnail-edit-close" class="btn btn-close mb10">写真を撮りなおす</a>
		    <a href="javascript:void(0);" class="btn btn-submit" id="analysis">この写真を解析</a>
	    </div>
    </div>*@

    @Html.Action("NoteMealEditThumbnailPartialView", "Note")

	<canvas id="canvas" width="0" height="0"></canvas>
</main>

    @Html.Action("NoteFooterPartialView", "Note")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/note/mealregisterfromphoto")
</body>
