@Imports MGF.QOLMS.QolmsYappli
@ModelType PortalAlkooConnectionViewModel

@Code
    ViewData("Title") = "ALKOO連携"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"
    
End Code

<body id="" class="lower">
    @Html.AntiForgeryToken()
    @*@Html.Action("PortalHeaderPartialView", "Portal")*@

<main id="main-cont" class="clearfix" role="main">
	<section class="contents-area">
		<h2 class="title">ALKOO(あるこう)連携</h2>
		<hr>
	    @If Me.Model.AlkooConnectedFlag = False Then
		    @<section class="section default">
			    ナビタイムの歩数計測アプリ「ALKOO」と歩数データを連携します。<br/>
				【連携対応アプリ】<br/>
				・Fitbit（ iOS 、Android）<br/>
				・ヘルスケア（iOSのみ）
				※Fitbitから歩数データを自動取得する場合、ALKOOアプリでもFitbit連携の設定が必要です。<br/>

		    </section>


		    @<div class="mb20">
			    <strong class="bold mb20 block">連携すると出来ること</strong>
			
			    <h3 class="title">歩数入力の手間が無くなります</h3>
			    <p class="mb40">
				    ナビタイムの歩数計アプリ「ALKOO(あるこう) by NAVITIME」と連携すれば、歩数を入力しなくても、自動記録されます。
			    </p>
			
			    <h3 class="title">ウォーキングコースで楽しく歩けます</h3>
			    <p class="mb40">
				    沖縄の観光地などを巡るウォーキングコースを見ることができます。イベントも行われるので、より楽しく歩くことができます。
			        <br/>（ご注意）
			        <br/>連携すると入力いただいた過去の歩数は閲覧できません。過去の歩数を確認する場合は、連携解除をお願いします。
                </p>
			
			    <a class="btn alkoo-btn mb40" href="native:/action/open_browser?url=https%3A%2F%2Fstatic.cld.navitime.jp%2Fwalkingapp-storage%2Fcommon%2Fhtml%2Fservice_lp%2Findex_ios.html" > 
				    <span><span></span> ALKOO(あるこう)について </span>
			    </a>
			
			    <div class="gray-area">
				    <ol>
					    <li>
						    <div>
							    連携方法
							    「ALKOO(あるこう) by NAVITIME」をダウンロード
							    <ul class="app-icon">
                                    <li><a href="native:/action/open_browser?url=https%3A%2F%2Fitunes.apple.com%2Fjp%2Fapp%2Fapple-store%2Fid911333356%3Fpt%3D13969%26ct%3Djoto%26mt%3D8"><img src="/dist/img/tmpl/dl-apple.png"></a></li>
								    <li><a href="native:/action/open_browser?url=https%3A%2F%2Fplay.google.com%2Fstore%2Fapps%2Fdetails%3Fid%3Dcom.navitime.local.alkoo%26referrer%3Djoto%0D%0A"><img src="/dist/img/tmpl/dl-google.png"></a></li>

							    </ul>
						    </div>
					    </li>
					    <li>
						    <div>
							    下部「連携する」ボタンをタップ
						    </div>
					    </li>
					    <li>
						    <div>
							    「ALKOO(あるこう) by NAVITIME」が立ち上がるので、「利用規約に同意して連携」をタップ
						    </div>
					    </li>
					    <li>
						    <div>
							    連携が開始されます。
						    </div>
					    </li>
				    </ol>
			    </div>
			
			
		    </div>
		    @<div class="borderd-area flex-area border-none">
			    <p>
				    ALKOOと連携
			    </p>
    @*				<div class="switchArea">
				    <input type="checkbox" id="switch1"><!-- checkedで返却するならchecked="" -->
				    <label for="switch1"><span></span></label>
				    <div class="swImg"></div>
			    </div>*@
		    </div>
			@<section id="caution" class="section caution mt10 mb10 hide"><h4>heading...</h4>アラートエリアです。不要時.hideで消してください。</section>
            @<div class="submit-area mb100">
                <button id="connection" class="btn btn-submit" type="submit">連 携</button>
			</div>

	    Else

		    @<section class="section default">
			    ナビタイムの歩数計測アプリ「ALKOO」と歩数データを連携中です。<br/>
		
				【連携対応アプリ】<br/>
				・Fitbit（ iOS 、Android）<br/>
				・ヘルスケア（iOSのみ）<br/>
				※Fitbitから歩数データを自動取得する場合、ALKOOアプリでもFitbit連携の設定が必要です。<br/>

            </section>
	     
	     	@<div class="section default mb30">
			    <h4 class="mb15 block main-color">歩数データが正常に連携されない場合</h4>
			    <p class="mb20">
			    機種変更など、端末を変更した際に連携が正常に行われない場合がございます。お手数ですが、こちらから再連携をお試し下さい。
			    </p>
			    <section id="caution-reconect" class="section caution mt10 mb10 hide"><h4>heading...</h4>アラートエリアです。不要時.hideで消してください。</section>
			    <p class="submit-area">
				    <a href="" id="reconnection" class="btn btn-submit">再連携する</a>
			    </p>
		    </div>
		    @<div class="mb100">
			    <h4 class="mb15 block">連携を解除する</h4>
			    <p class="mb20">
			    連携を解除するとJOTOホームドクターの「歩く」ページが表示されます。<br>
			    連携を解除すると歩数は自動取得されません。ご注意下さい。
			    </p>
			    <section id="caution" class="section caution mt10 mb10 hide"><h4>heading...</h4>アラートエリアです。不要時.hideで消してください。</section>
			    <p class="submit-area">
				    <a href="" id="cancel" class="btn btn-delete">連携を解除する</a>
			    </p>
		    </div> 	     
	    End If
	</section>

    @If Me.Model.TanitaWalkConnectedFlag Then
       @<!-- 「タニタ歩数連携解除の確認」ダイアログ -->
       @<div class="modal fade" id="regist-modal" tabindex="-1" data-backdrop="static" data-keyboard="false" data-row-no="-1">
	        <div class="modal-dialog">
		        <div class="modal-content">
			        <div class="modal-header">
				        <button type="button" class="close"><span>×</span></button>
				        <h4 class="modal-title">登録の確認</h4>
			        </div>
			        <div class="modal-body">
                        ALKOO by NAVITIMEと連携します。<br/>
				        現在連携中のタニタヘルスプラネットの歩数連携を解除します。<br/>
                        よろしいですか？
			        </div>
			        <div class="modal-footer">
				        <button type="button" class="btn btn-close no-ico mb0">閉じる</button>
				        <button type="button" class="btn btn-delete">解 除</button>
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
				        ALKOOアプリとの連携を解除します。<br/>
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

    @QyHtmlHelper.RenderScriptTag("~/dist/js/portal/alkooconnection")
</body>
