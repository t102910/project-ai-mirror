@Imports MGF.QOLMS.QolmsYappli
@ModelType PortalFitbitConnectionViewModel

@Code
    ViewData("Title") = "Fitbit連携"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"
    
End Code

<body id="" class="lower">
    @Html.AntiForgeryToken()
    @*@Html.Action("PortalHeaderPartialView", "Portal")*@
    <main id="main-cont" class="clearfix" role="main">
        @Select Case Me.Model.FromPageNoType
            Case QyPageNoTypeEnum.PortalHome
          
                @<section class="home-btn-wrap" data-pageno="@Convert.ToByte(Me.Model.FromPageNoType)">
                   <a href="../Portal/Home" class="home-btn"><i class="la la-angle-left"></i><i class="la la-home la-15x"></i><span> ホーム</span></a>
                </section>  
                
            Case QyPageNoTypeEnum.PortalChallenge
                
                @<section class="home-btn-wrap" data-pageno="@Convert.ToByte(Me.Model.FromPageNoType)">
                    <a href="../Portal/challenge" class="home-btn type-2"><i class="la la-angle-left"></i><span> 戻る</span></a>
                </section> 
                
            Case Else
               	@<section class="home-btn-wrap" data-pageno="@Convert.ToByte(Me.Model.FromPageNoType)">
		            <a href="../Portal/ConnectionSetting" class="home-btn type-2"><i class="la la-angle-left"></i><span> 戻る</span></a>
	            </section> 

        End Select
	    <section class="contents-area">
            <h2 class="title">Fitbit連携</h2>
		    <hr>

            @If Me.Model.FitbitConnectedFlag = False Then
                ' 未連携
                @<p class="section default">
                
                    Fitbitとデータ連携の設定をします。<br/>
                    連携すると、心拍数と運動強度(METs)を自動で取得します。<br/>
                    Fitbitの連携許可画面ですべての許可が必要です。<br/>

                    ※歩数データを自動取得する場合、ALKOOアプリでもFitbit連携の設定が必要です。<br/>
                </p>
            Else
                ' 連携済み                
                
                @<p class="section default">
                
                    連携済み<br/>
                    ※歩数データを自動取得する場合、ALKOOアプリでもFitbit連携の設定が必要です。<br/>
                </p>
                
                
            End If
    
            @*<p class="center mb20"><img src="/dist/img/tanita/tanita.jpg" class="max-image"></p>*@
            <a href="native:/action/open_browser?url=https%3A%2F%2Fwww.fitbit.com%2F" class="btn btn-submit block">Fitbitについて詳しく</a><!-- ここにヤプリ呼び出しのスキーマ -->
            <!--タニタ連携済みかどうかによって出し分け-->

            @If Me.Model.FitbitConnectedFlag = False Then
                @<div class="submit-area">
                    <button id="connection" class="btn btn-submit" type="submit">連携</button>
			    </div>
            Else
               @<div class="submit-area">
                   <button id="cancel" class="btn btn-close" type="submit">連携解除</button>
			    </div>
                
            End If

        </section>

    </main>

    @Html.Action("PortalFooterPartialView", "Portal") 

    @QyHtmlHelper.RenderScriptTag("~/dist/js/portal/fitbitconnection")
</body>

<script></script>
