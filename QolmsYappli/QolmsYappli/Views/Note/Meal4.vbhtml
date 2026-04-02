@Imports MGF.QOLMS.QolmsYappli
@ModelType NoteMealViewModel4

@Code
    ViewData("Title") = "食事"
    Layout = "~/Views/Shared/_NoteLayout.vbhtml"
End Code

<body id="meal" class="lower">
    @Html.AntiForgeryToken()

    <main id="main-cont" class="clearfix" role="main">
        <section class="home-btn-wrap">
           <a href="../Portal/Home" class="home-btn disabled"><i class="la la-angle-left"></i><i class="la la-home la-15x"></i><span> ホーム</span></a>
        </section>
	    <section class="contents-area mb20">
		    <div class="input-area" id="input-1">
			
                @Html.Action("NoteMealEditDatePartialView", "Note", New RouteValueDictionary From {{"displayTime", Boolean.FalseString}})

			    <div class="input-area-inner">
				    <h4 class="line center">検索で食事を登録</h4>

                    @Html.Action("NoteMealEditSearchAreaPartialView", "Note")

				    <h4 class="line center pt20">他の登録方法で食事を登録</h4>
				    <label for="sample1" class="btn btn-style2 block center ">
					    <i class="la la-camera"></i>写真から登録
					    <input type="file" id="sample1" accept="image/*" class="hide" disabled="disabled">
				    </label>
                    <a href="javascript:void(0);" class="btn btn-style2 block center mb0" id="history"><i class="la la-clock-o"></i>履歴から登録</a>
			    </div>
                <section class="section caution mt10 mb10 hide"><h4>heading...</h4>アラートエリアです。不要時.hideで消してください。</section>
		    </div>
	    </section>	

        <canvas id="canvas" width="0" height="0"></canvas>

        <!-- 一覧 -->
	    <section class="data-area  pb50 mb30">
		    <h3 class="title mt10 two-pane">
			    <span id="filter-title">最近の食事</span>
			    <span class="last-child"><span class="calendar-view"><i class="la la-calendar la-2x"></i>絞り込み</span></span>
		    </h3>
		    <div id="calender-area">
			    <p class="wrap">
                    <input type="text" id="filter-date" class="form-control picker" name="" value="@String.Format("{0:yyyy年MM月dd日}", Me.Model.FilterDate)" readonly="readonly" style="background-color:white;" autocomplete="off" placeholder="日付を変更">
			        <span class="submit-area-2"><span id="filter-submit" class="btn btn-submit narrow">絞り込む</span></span>
			    </p>
		    </div>
            @*@Html.Action("NoteMealCardAreaPartialView4", "Note")*@
            <div id="update-data-area" class="reload-area loading">
            </div>
	    </section>

        <!-- サムネイル編集エリア -->

        @Html.Action("NoteMealEditThumbnailPartialView", "Note")

        <!--削除モーダル -->
        <div class="modal fade" id="delete-modal" tabindex="-1" data-backdrop="static" data-keyboard="false" data-record-date="0001/01/01 00:00:00" data-meal-type="0" data-seq="0">
	        <div class="modal-dialog">
		        <div class="modal-content">
			        <div class="modal-header">			
                        <button type="button" class="close" data-dismiss="modal"><span>×</span></button>
				        <h4 class="modal-title">削除確認</h4>
			        </div>
			        <div class="modal-body">
				        この項目を削除します。よろしいですか
			        </div>
			        <div class="modal-footer">
				        <button type="button" class="btn btn-close no-ico mb0" data-dismiss="modal">閉じる</button>
				        <button id="delete-button" type="button" class="btn btn-delete">削 除</button>
			        </div>
		        </div>
	        </div>
        </div>
    </main>

    @Html.Action("NoteFooterPartialView", "Note")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/note/meal4")
</body>
