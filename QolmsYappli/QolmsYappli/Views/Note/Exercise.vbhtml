@Imports MGF.QOLMS.QolmsYappli
@ModelType NoteExerciseViewModel

@Code
    ViewData("Title") = "運動"
    Layout = "~/Views/Shared/_NoteLayout.vbhtml"
End Code

<body id="sports" class="lower @IIf(Me.Model.RecordDate.Date = Date.Now.Date, String.Empty, "input-all").ToString()">
    @Html.AntiForgeryToken()
    @*@Html.Action("NoteHeaderPartialView", "Note")*@

    <main id="main-cont" class="clearfix" role="main">
        <section class="home-btn-wrap">
           <a href="../Portal/Home" class="home-btn"><i class="la la-angle-left"></i><i class="la la-home la-15x"></i><span> ホーム</span></a>
        </section>
	    <section class="contents-area mb20">
		    <div class="input-area">
			    <div class="hide-elm">
				    <div class="input-group mb10">
					    <span class="input-group-addon">入力日</span>
					    <input type="text" id="recordDate" class="form-control picker" name="RecordDate" value="@String.Format("{0:yyyy年MM月dd日}", Me.Model.RecordDate)" readonly="readonly" style="background-color:white;" autocomplete="off">
				    </div>
			    </div>
			    <div class="hide-elm">
				    <div class="input-group datetime-select mb10">
					    <span class="input-group-addon">時間</span>
                        @QyHtmlHelper.ToMeridiemDropDownList("meridiem", "meridiem", Me.Model.RecordDate.ToString("tt", System.Globalization.CultureInfo.InvariantCulture).ToLower(), css:="form-control ap")
                        @QyHtmlHelper.ToHourDropDownList("hour", "hour", (Me.Model.RecordDate.Hour Mod 12).ToString, css:="form-control hour")
                        @QyHtmlHelper.ToMinuteDropDownList("minute", "minute", Me.Model.RecordDate.Minute, css:="form-control minute")
                        
				    </div>
			    </div>
			    <div id="kind-select-wrap">
				    <a href="#" id="kind-select" class="btn">スタンプから運動登録</a>
				    <p id="selected-area">
					    <span id="changer" class="hide">
    <!-- 						ここにスタンプをclone -->
					    </span>
					    <span id="kind-remover" class="hide"><i class="la la-remove"></i></span>
				    </p>
			    </div>
			    <div class="flex-wrap">
				    <div class="input-group mb10">
					    <span class="input-group-addon">カロリー</span>
					    <input type="tel" id="calorie" class="form-control" name="calorie" value="" maxlength="4" required="required">
					    <span class="input-group-addon">kcal</span>
				    </div>
				    <div class="submit-area">
					    <a href="javascript:void(0);" class="btn btn-submit disabled" >登録</a>
				    </div>
			    </div>
                <section id="alertarea" class="section caution mt10 mb0 hide"><h4></h4></section>
		    </div>
		    <p class="right">
                    @If Me.Model.RecordDate.Date = Date.Now.Date Then
                        @<a href="#" id="input-all" class=""><i class="la la-calendar"></i>@String.Format("日時を変更して入力する")</a>
                        
                    Else
                        @<a href="#" id="input-all" class="all"><i class="la la-calendar"></i>@String.Format("現在の日時で入力する")</a>
                    End If
		    </p>
            <a href="../Note/GulfSportsMovieIndex" class="block mb20 img-max-cont gulf-btn"><img src="/dist/img/sports/gulf-btn.png" alt="" class="w-max"></a>
	    </section>

       @Html.Action("NoteExerciseCardPartialView", "Note")
    </main>

    @Html.Action("NoteFooterPartialView", "Note")

    <div id="kind-stamp">
        @If Me.Model.ExerciseStringN.Count > 0 Then
            @<p class="stamp-wrap">
		        <select class="form-control">
			        <option>スタンプに無い運動を選択</option>           
                    @For Each item As ExerciseItem In Me.Model.ExerciseStringN
                        @<option data-cal="@item.Calorie" data-extype ="@item.ExerciseType">@item.ExerciseName</option>
                    Next
                </select>
	        </p>
         End If

        <ul>
            @If Me.Model.ExerciseStampN.Count > 0 Then
                For Each item As ExerciseItem In Me.Model.ExerciseStampN
                    @<li>
			            <span id="selectedstamp" name ="selectedstamp" data-extype="@item.ExerciseType" data-calorie="@item.Calorie">
				            <img src="@item.PhotoKey">
				            <span id="stampname" name ="stampname">@item.ExerciseName</span>
			            </span>
		            </li> 
                Next
            End If
	    </ul>

        @If Me.Model.ExerciseStringN.Count > 0 Then
            @<p class="stamp-wrap">
		        <select class="form-control">
			        <option>スタンプに無い運動を選択</option>           
                    @For Each item As ExerciseItem In Me.Model.ExerciseStringN
                        @<option data-cal="@item.Calorie" data-extype ="@item.ExerciseType">@item.ExerciseName</option>
                    Next
                </select>
	        </p>
        End If

	    <i class="la la-close" id="kind-stamp-close"></i>
    </div>

    <div class="modal fade" id="delete-modal" tabindex="-1" data-backdrop="static" data-keyboard="false">
	    <div class="modal-dialog">
		    <div class="modal-content">
			    <div class="modal-header">
				    <button type="button" class="close" data-dismiss="modal"><span>×</span></button>
                    <h4 class="modal-title">削除確認</h4>
			    </div>
			    <div class="modal-body">
				    この項目を削除します。よろしいですか？
			    </div>
			    <div class="modal-footer">
				    <button type="button" class="btn btn-close no-ico mb0" data-dismiss="modal">閉じる</button>
				    <button type="button" class="btn btn-delete" data-recorddate="" data-exercisetype="" data-seq="" data-calorie="">削 除</button>
			    </div>
		    </div>
	    </div>
    </div>

    @QyHtmlHelper.RenderScriptTag("~/dist/js/note/exercise")
</body>
