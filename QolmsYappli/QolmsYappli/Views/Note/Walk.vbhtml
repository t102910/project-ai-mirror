@Imports MGF.QOLMS.QolmsYappli
@ModelType NoteWalkViewModel

@Code
    ViewData("Title") = "歩く"
    Layout = "~/Views/Shared/_NoteLayout.vbhtml"
    
    Dim hasData As Boolean = False
    Dim isYappli As Boolean = Me.Context.Request.UserAgent.ToLower().Contains("yappli")
    Dim isiPhone As Boolean = Me.Context.Request.UserAgent.ToLower().Contains("iphone")
    Dim isAndroid As Boolean = Me.Context.Request.UserAgent.ToLower().Contains("android")
End Code

<body id="walk" class="lower @IIf(Me.Model.RecordDate.Date = Date.Now.Date, String.Empty, "input-all").ToString()" style=" overflow: visible;">
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
					    <span class="input-group-addon">歩いた日</span>
					    <input type="text" class="form-control picker" value="@Me.Model.RecordDate.ToString("yyyy年MM月dd日")" readonly="readonly" style="background-color:white;" autocomplete="off">
				    </div>
			    </div>
                <div class="flex-wrap">
			        <div class="input-group">
				        <input type="tel" class="form-control" value="" placeholder="歩数を入力" maxlength="6" style="ime-mode:disabled;" autocomplete="off">
				        <span class="input-group-addon">歩</span>
			        </div>
			        <div class="submit-area">
				        <a href="javascript:void(0);" class="btn btn-submit">登録</a>
			        </div>
                </div>
                <section class="section caution mt10 mb0 hide"></section>
@*                @If isiPhone Then
                    @<a id="app-btn" href="joto-uploader://" class=""><img src="../dist/img/tmpl/app.png"></a>
                ElseIf isAndroid Then
                    @<a id="app-btn" href="#" class="hide"><img src="../dist/img/tmpl/app.png"></a>
                Else
                    @<a id="app-btn" href="#" class="hide"><img src="../dist/img/tmpl/app.png"></a>
                End If*@
                @If Me.Model.TanitaWalkConnected = False Then
                    @<a id="app-btn" href="native:/tab/custom/b55226cd"><img src="/dist/img/tmpl/app.png"></a>
                End If

		    </div>
		    <p class="right">
                <a href="javascript:void(0);" id="input-all">
                    @If Me.Model.RecordDate.Date = Date.Now.Date Then
                        @<i class="la la-calendar"></i>@String.Format("日時を変更して入力する")
                    Else
                        @<i class="la la-calendar"></i>@String.Format("現在の日時で入力する")
                    End  If
                </a>
		    </p>

	    </section>

@*        @If isiPhone Then
            @<br class="hide"/>@<a id="" href="twitter://" class="hide">（テスト）Twitter</a>
            @<br class="hide"/>@<a id="" href="comgooglemaps://" class= "hide">（テスト）GoogleMap1</a>
            @<br class="hide"/>@<a id="" href="http://maps.google.com" class="hide">（テスト）GoogleMap2</a>
        End If*@

        <section class="data-area">
            @If Me.Model.IsAvailableVitalType(QyVitalTypeEnum.Steps, hasData) AndAlso hasData Then
                @<!-- 「グラフ」、Ajax 非同期更新エリア-->
                @<div id="gh-area" class="reload-area" data-vital-type="@QyVitalTypeEnum.Steps.ToString()">
                    <h3 class="title mt10">最近の歩数</h3>
                    <div class="reload-area loading"></div>
                </div>
            Else
                @<div id="gh-area" class="reload-area" data-vital-empty-type="@QyVitalTypeEnum.Steps.ToString()">
                    <h3 class="title mt10">最近の歩数</h3>
                    <section class="section caution mt10 mb0">
                        未登録です
                    </section>
                </div>
            End If
        </section>

        <!-- 「詳細」ダイアログ、Ajax 非同期更新エリア -->
        <div id="val-modal">
        </div>

        <!-- 「削除確認」ダイアログ -->
        <div class="modal fade" id="delete-modal" tabindex="-1" data-backdrop="static" data-keyboard="false" data-row-no="-1">
	        <div class="modal-dialog">
		        <div class="modal-content">
			        <div class="modal-header">
				        <button type="button" class="close"><span>×</span></button>
				        <h4 class="modal-title">削除確認</h4>
			        </div>
			        <div class="modal-body">
				        この項目を削除します。よろしいですか？
			        </div>
			        <div class="modal-footer">
				        <button type="button" class="btn btn-close no-ico mb0">閉じる</button>
				        <button type="button" class="btn btn-delete">削 除</button>
			        </div>
		        </div>
	        </div>
        </div>
    </main>

    @Html.Action("NoteFooterPartialView", "Note")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/note/walk")
</body>
