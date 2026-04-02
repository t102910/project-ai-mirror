@Imports MGF.QOLMS.QolmsYappli
@ModelType PortalTanitaConnectionInputModel

@Code
    ViewData("Title") = "タニタヘルスプラネット連携"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"
    
End Code

<body id="" class="lower">
    @Html.AntiForgeryToken()
    @*@Html.Action("PortalHeaderPartialView", "Portal")*@
    <main id="main-cont" class="clearfix" role="main">
	    <section class="contents-area">
            <h2 class="title">タニタヘルスプラネット連携</h2>
		    <hr>

                @If String.IsNullOrWhiteSpace(Me.Model.ConnectionID) Then
                    @<p class="section default">
                
                        タニタヘルスプラネットとデータ連携の設定をします。<br/>
                        連携すると、体組成計、活動量計で計測したデータ、入力した血圧データが自動で連携されます。<br/>
                        からだカルテ または ヘルスプラネット のIDとパスワードを入力して連携してください。
                        連携開始時に過去1か月分のデータを取得します。
                    </p>
                    
                Else
                    @<p class="section default">
                
                        タニタヘルスプラネットとデータ連携中です。<br/>
                        連携するデータを変更する場合はこちらの画面で変更できます。
                    </p>
                End If

            <p class="center mb20"><img src="/dist/img/tanita/tanita.jpg" class="max-image"></p>
            <a href="native:/action/open_browser?url=https%3A%2F%2Fwww.healthplanet.jp%2F" class="btn btn-submit block">ヘルスプラネットについて詳しく</a><!-- ここにヤプリ呼び出しのスキーマ -->
            <!--タニタ連携済みかどうかによって出し分け-->
          @*  @QyHtmlHelper.ToValidationMessageInSectionTag({"ID"}, Me.TempData("ErrorMessage"))
            @QyHtmlHelper.ToValidationMessageInSectionTag({"Password"}, Me.TempData("ErrorMessage"))
            @QyHtmlHelper.ToValidationMessageInSectionTag({"tanita"}, Me.TempData("ErrorMessage"))
            *@

                @If String.IsNullOrWhiteSpace( Me.Model.ConnectionID) Then

			    @<h3 class="title">ログイン情報</h3>		
			    @<label for="ID"  class="t-row line">
				    <span class="label-txt"><span class="ico required">必須</span> からだカルテ または ヘルスプラネット のID</span>
				    <input type="text" id="ID" name ="ID" class="form-control mb10" placeholder="半角英数" value="" required="required" maxlength="64" style="ime-mode:active;" autocomplete="off">
				    <p class="alert alert-danger thin mt10 hide">
					    表示テスト
				    </p>
			    </label>
			    @<label for="Password"  class="t-row line">
				    <span class="label-txt"><span class="ico required">必須</span> パスワード</span>
				    <input type="Password" id="Password" name ="Password" class="form-control mb10" placeholder="半角英数" value="" required="required" maxlength="16" style="ime-mode:active;" autocomplete="off">
				    <p class="alert alert-danger thin mt10 hide">
					    表示テスト
				    </p>
			    </label>
                Else
                    @<input id ="connect" type="hidden" name ="ConnectionID" value ="@Me.Model.ConnectionID">
                End If

                <h3 class="title">連携するデータ</h3>
			    <div class="wizard-form inline-block link-ico tanita" id="connection-data">
                
                    @For Each item As DeviceItem In Me.Model.Devices
                        If item.Checked Then
			                @<input id="@item.DevicePropertyName" type="checkbox" name="@item.DevicePropertyName" value="true" checked="checked">
				            @<label for="@item.DevicePropertyName">@item.DeviceName</label>
                        Else
			                @<input id="@item.DevicePropertyName" type="checkbox" value="true" name="@item.DevicePropertyName" >
				            @<label for="@item.DevicePropertyName">@item.DeviceName</label>
                        End If
                    Next
			    </div>
				<section id="caution" class="section caution mt10 mb10 hide"><h4>heading...</h4>アラートエリアです。不要時.hideで消してください。</section>

                @If String.IsNullOrWhiteSpace(Me.Model.ConnectionID) Then
                    @<div class="submit-area">
                        <button id="connection" class="btn btn-submit" type="submit">連携</button>
			        </div>
                Else
                    'todo:連携解除時のメッセージ、完了メッセージ
                    @<div class="submit-area">
                        <a id="cancel" class="btn btn-close">連携解除</a>
			        </div>                    
                End If

        </section>

        <!-- 「ALKOO連携解除の確認」ダイアログ -->
        @If Me.Model.AlkooConnectedFlag Then
            
            @<div class="modal fade" id="alkoo-disconnect-modal" tabindex="-1" data-backdrop="static" data-keyboard="false" data-row-no="-1">
	            <div class="modal-dialog">
		            <div class="modal-content">
			            <div class="modal-header">
				            <button type="button" class="close"><span>×</span></button>
				            <h4 class="modal-title">登録の確認</h4>
			            </div>
			            <div class="modal-body">
				            ALKOOアプリとの連携を解除してタニタ連携に切り替えます。<br/>
                            よろしいですか？
			            </div>
			            <div class="modal-footer">
				            <button type="button" class="btn btn-close no-ico mb0">閉じる</button>
				            <button type="button" class="btn btn-delete">連 携</button>
			            </div>
		            </div>
	            </div>
            </div>
            
        End If

        <!-- 「連携解除の確認」ダイアログ -->
        <div class="modal fade" id="disconnect-modal" tabindex="-1" data-backdrop="static" data-keyboard="false" data-row-no="-1">
	        <div class="modal-dialog">
		        <div class="modal-content">
			        <div class="modal-header">
				        <button type="button" class="close"><span>×</span></button>
				        <h4 class="modal-title">連携解除の確認</h4>
			        </div>
			        <div class="modal-body">
				        タニタヘルスプラネットとの連携を解除します。<br/>
                        よろしいですか？
			        </div>
			        <div class="modal-footer">
				        <button type="button" class="btn btn-close no-ico mb0">閉じる</button>
				        <button type="button" class="btn btn-delete">解 除</button>
			        </div>
		        </div>
	        </div>
        </div>
    </main>

    @Html.Action("PortalFooterPartialView", "Portal") 

    @QyHtmlHelper.RenderScriptTag("~/dist/js/portal/tanitaconnection")
</body>
