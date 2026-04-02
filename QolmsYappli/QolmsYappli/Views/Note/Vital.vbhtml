@Imports System.Globalization
@Imports MGF.QOLMS.QolmsYappli
@ModelType NoteVitalViewModel

@Code
    ViewData("Title") = "バイタル"
    Layout = "~/Views/Shared/_NoteLayout.vbhtml"
    
    Dim hasData As Boolean = False 
End Code

<body id="vital" class="lower @IIf(Me.Model.RecordDate.Date = Date.Now.Date, String.Empty, "input-all").ToString()" style=" overflow: visible;">
    @Html.AntiForgeryToken()
    @*@Html.Action("NoteHeaderPartialView", "Note")*@

    <main id="main-cont" class="clearfix" role="main">
        <section class="home-btn-wrap type2">
           <a href="../Portal/Home" class="home-btn"><i class="la la-angle-left"></i><i class="la la-home la-15x"></i><span> ホーム</span></a>
             @If Not String.IsNullOrWhiteSpace(Me.Model.TanitaQrReference) Then
                @<p class="secondary-btn-wrap">
			        <a href="javascript:void(0);" id="tanita-qr" class="btn btn-blue btn-style3" data-reference="@Me.Model.TanitaQrReference">
				        <i class="la la-qrcode la-lg"></i>QR認証
			        </a>
		        </p>
            End If 
        </section>
        <section class="contents-area mb20">
		    <ul class="nav nav-tabs mb20">
			    <li class="@IIf(Me.Model.Tab <= 1, "active", String.Empty).ToString()"><a href="#input-1" data-toggle="tab"><span>体 重</a></li>
			    <li class="@IIf(Me.Model.Tab = 2, "active", String.Empty).ToString()"><a href="#input-2" data-toggle="tab"><span>血 圧</a></li>
			    <li class="@IIf(Me.Model.Tab = 3, "active", String.Empty).ToString()"><a href="#input-3" data-toggle="tab"><span>血糖値</span></a></li>
		    </ul>
            <div class="tab-content">
                <!-- 体重 -->
			    <div class="input-area tab-pane fade in @IIf(Me.Model.Tab <= 1, "active", String.Empty).ToString()" id="input-1">
				    <div class="hide-elm">
					    <div class="input-group mb10">
						    <span class="input-group-addon">計測日</span>
						    <input type="text" class="form-control picker" value="@Me.Model.RecordDate.ToString("yyyy年MM月dd日")" readonly="readonly" style="background-color:white;" autocomplete="off">
					    </div>
				    </div>
				    <div class="hide-elm">
					    <div class="input-group datetime-select mb10">
						    <span class="input-group-addon">時間</span>
                            @QyHtmlHelper.ToMeridiemDropDownList("", "meridiem", Me.Model.RecordDate.ToString("tt", CultureInfo.InvariantCulture).ToLower(), "form-control ap")
                            @QyHtmlHelper.ToHourDropDownList("", "hour", Me.Model.RecordDate.Hour Mod 12, "form-control hour")
                            @QyHtmlHelper.ToMinuteDropDownList("", "minute", Me.Model.RecordDate.Minute, "form-control minute")
					    </div>
				    </div>
				    <div class="flex-wrap">
					    <div class="stretch-width">
						    <div class="input-group mb10">
							    <span class="input-group-addon">体重</span>
							    <input type="number" class="form-control val" value="" placeholder="体重を入力" maxlength="5" style="ime-mode:disabled;" autocomplete="off">
							    <span class="input-group-addon">kg</span>
						    </div>
                            <div id="how-tall" class="mb10 @IIf(Me.Model.Height <= Decimal.Zero, "first-time", String.Empty)"><!-- 身長入力なしの場合.first-timeをつけてください -->
						        <div class="input-group">
							        <span class="input-group-addon">身長</span>
							        <input type="number" class="form-control val" value="@IIf(Me.Model.Height > Decimal.Zero, Me.Model.Height.ToString("0.####"), String.Empty)" placeholder="身長を入力" maxlength="5" style="ime-mode:disabled;" autocomplete="off" data-default="@IIf(Me.Model.Height > Decimal.Zero, Me.Model.Height.ToString("0.####"), String.Empty)">
							        <span class="input-group-addon">cm</span>
						        </div>
                                <p id="height-opener" class="right"><a href="javascript:void(0);"><i class="la la-arrows-v"></i> 身長を変更する</a></p>
                            </div>
					    </div>
					    <div class="submit-area mb10">
						    <a href="javascript:void(0);" class="btn btn-submit">登録</a>
					    </div>
				    </div>
                    <section class="section caution mt0 mb10 hide"></section>
                    
                    <p class="right">
                        <a href="javascript:void(0);" class="input-expand">
                            @If Me.Model.RecordDate.Date = Date.Now.Date Then
                                @<i class="la la-calendar"></i>@String.Format("日時を変更して入力する")
                            Else
                                @<i class="la la-calendar"></i>@String.Format("現在の日時で入力する")
                            End If

                        </a>
                    </p>

                    <!-- グラフ と表 -->
                    <section class="data-area">
                        <!-- Ajax 非同期更新 エリア -->
                        @If Me.Model.IsAvailableVitalType(QyVitalTypeEnum.BodyWeight, hasData) AndAlso hasData Then
                            @<div id="w-area" class="reload-area" data-vital-type="@QyVitalTypeEnum.BodyWeight.ToString()">
                                <h3 class="title mt10">最近の体重</h3>
                                <div class="reload-area loading"></div>
                            </div>
                        Else
                            @<div id="w-area" class="reload-area" data-vital-empty-type="@QyVitalTypeEnum.BodyWeight.ToString()">
                                <h3 class="title mt10">最近の体重</h3>
                                <section class="section caution mt10 mb0">
                                    未登録です
                                </section>
                            </div>
                        End If
                    </section>
			    </div>

                <!-- 血圧 -->
                <div class="input-area tab-pane fade in @IIf(Me.Model.Tab = 2, "active", String.Empty).ToString()" id="input-2">
				    <div class="hide-elm">
					    <div class="input-group mb10">
						    <span class="input-group-addon">計測日</span>
						    <input type="text" class="form-control picker" value="@Me.Model.RecordDate.ToString("yyyy年MM月dd日")" readonly="readonly" style="background-color:white;" autocomplete="off">
					    </div>
				    </div>
                    <div class="hide-elm">
					    <div class="input-group datetime-select mb10">
						    <span class="input-group-addon">時間</span>
                            @QyHtmlHelper.ToMeridiemDropDownList("", "meridiem", Me.Model.RecordDate.ToString("tt", CultureInfo.InvariantCulture).ToLower(), "form-control ap")
                            @QyHtmlHelper.ToHourDropDownList("", "hour", Me.Model.RecordDate.Hour Mod 12, "form-control hour")
                            @QyHtmlHelper.ToMinuteDropDownList("", "minute", Me.Model.RecordDate.Minute, "form-control minute")
					    </div>
                    </div>
				    <div class="flex-wrap">
					    <div class="stretch-width">
						    <div class="input-group mb10">
							    <span class="input-group-addon">上</span>
							    <input type="tel" value="" class="form-control val" placeholder="血圧を入力" maxlength="3" style="ime-mode:disabled;" autocomplete="off">
							    <span class="input-group-addon">mmHg</span>
						    </div>
						    <div class="input-group mb10">
							    <span class="input-group-addon">下</span>
							    <input type="tel" value="" class="form-control val" placeholder="血圧を入力" maxlength="3" style="ime-mode:disabled;" autocomplete="off">
							    <span class="input-group-addon">mmHg</span>
						    </div>
					    </div>
					    <div class="submit-area mb10">
						    <a href="javascript:void(0);" class="btn btn-submit">登録</a>
					    </div>
				    </div>
                    <section class="section caution mt0 mb10 hide"></section>

                    <p class="right">
                        <a href="javascript:void(0);" class="input-expand">
                            @If Me.Model.RecordDate.Date = Date.Now.Date Then
                                @<i class="la la-calendar"></i>@String.Format("日時を変更して入力する")
                            Else
                                @<i class="la la-calendar"></i>@String.Format("現在の日時で入力する")
                            End If

                        </a>
                    </p>
                    <!-- グラフ と表 -->
                    <section class="data-area">
                        @If Me.Model.IsAvailableVitalType(QyVitalTypeEnum.BloodPressure, hasData) AndAlso hasData Then
                            @<!-- Ajax 非同期更新 エリア -->
                            @<div id="bp-area" class="reload-area" data-vital-type="@QyVitalTypeEnum.BloodPressure.ToString()">
                                <h3 class="title mt10">最近の血圧</h3>
                                <div class="reload-area loading"></div>
                            </div>
                        Else
                            @<div id="bp-area" class="reload-area" data-vital-empty-type="@QyVitalTypeEnum.BloodPressure.ToString()">
                                <h3 class="title mt10">最近の血圧</h3>
                                <section class="section caution mt10 mb0">
                                    未登録です
                                </section>
                            </div>
                        End If
                    </section>
                </div>

                <!-- 血糖値 -->
			    <div class="input-area tab-pane fade in @IIf(Me.Model.Tab = 3, "active", String.Empty).ToString()" id="input-3">
				    <div class="hide-elm">
					    <div class="input-group mb10">
						    <span class="input-group-addon">計測日</span>
						    <input type="text" class="form-control picker" value="@Me.Model.RecordDate.ToString("yyyy年MM月dd日")" readonly="readonly" style="background-color:white;" autocomplete="off">
					    </div>
				    </div>
				    <div class="hide-elm">
					    <div class="input-group datetime-select mb10">
						    <span class="input-group-addon">時間</span>
                            @QyHtmlHelper.ToMeridiemDropDownList("", "meridiem", Me.Model.RecordDate.ToString("tt", CultureInfo.InvariantCulture).ToLower(), "form-control ap")
                            @QyHtmlHelper.ToHourDropDownList("", "hour", Me.Model.RecordDate.Hour Mod 12, "form-control hour")
                            @QyHtmlHelper.ToMinuteDropDownList("", "minute", Me.Model.RecordDate.Minute, "form-control minute")
					    </div>
				    </div>
                    @QyHtmlHelper.ToVitalConditionDropDownList("", "condition", "None", "form-control val mb10")
				    <div class="flex-wrap">
					    <div class="stretch-width">
						    <div class="input-group mb10">
							    <input type="tel" class="form-control val" value="" placeholder="血糖値を入力" maxlength="3" style="ime-mode:disabled;" autocomplete="off">
							    <span class="input-group-addon">mg/dl</span>
						    </div>
					    </div>
					    <div class="submit-area mb10">
						    <a href="javascript:void(0);" class="btn btn-submit">登録</a>
					    </div>
				    </div>
                    <section class="section caution mt0 mb10 hide"></section>

                    <p class="right">
                        <a href="javascript:void(0);" class="input-expand">
                            @If Me.Model.RecordDate.Date = Date.Now.Date Then
                                @<i class="la la-calendar"></i>@String.Format("日時を変更して入力する")
                            Else
                                @<i class="la la-calendar"></i>@String.Format("現在の日時で入力する")
                            End If

                        </a>
                    </p>
                    <!-- グラフ と表 -->
                    <section class="data-area">
                        @If Me.Model.IsAvailableVitalType(QyVitalTypeEnum.BloodSugar, hasData) AndAlso hasData Then
                            @<!-- Ajax 非同期更新 エリア -->
                            @<div id="bgl-area" class="reload-area" data-vital-type="@QyVitalTypeEnum.BloodSugar.ToString()">
                                <h3 class="title mt10">最近の血糖値</h3>
                                <div class="reload-area loading"></div>
                            </div>
                        Else
                            @<div id="bgl-area" class="reload-area" data-vital-empty-type="@QyVitalTypeEnum.BloodSugar.ToString()">
                                <h3 class="title mt10">最近の血糖値</h3>
                                <section class="section caution mt10 mb0">
                                    未登録です
                                </section>
                            </div>
                        End If
                    </section>
			    </div>
            </div>
        </section>

        <!-- 「詳細」ダイアログ、Ajax 非同期更新 エリア -->
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

        <!-- 「入力確認」ダイアログ -->
        <div class="modal fade" id="editing-modal" tabindex="-1" data-backdrop="static" data-keyboard="false" data-tab-id="">
	        <div class="modal-dialog">
		        <div class="modal-content">
			        <div class="modal-header">
				        <button type="button" class="close"><span>×</span></button>
				        <h4 class="modal-title">入力確認</h4>
			        </div>
			        <div class="modal-body">
                        入力中の情報は破棄されます。よろしいですか？
			        </div>
			        <div class="modal-footer">
				        <button type="button" class="btn btn-close no-ico mb0">いいえ</button>
				        <button type="button" class="btn btn-default no-ico mb0">は い</button>
			        </div>
		        </div>
	        </div>
        </div>

        <!-- 「タニタ QR」ダイアログ -->
        <div class="modal fade" id="tanita-qr-modal" tabindex="-1" data-backdrop="static" data-keyboard="false">
	        <div class="modal-dialog">
		        <div class="modal-content">
			        <div class="modal-header">
				        <button type="button" class="close"><span>×</span></button>
				        <h4 class="modal-title">タニタ会員QRコード</h4>
			        </div>
			        <div class="modal-body center">
                        <span>QRコードの取得に失敗しました。</span>
                        <br>
                        <canvas width="200" height="200" class="mt10 hide"></canvas>
			        </div>
			        <div class="modal-footer">
				        <button type="button" class="btn btn-close no-ico mb0">閉じる</button>
			        </div>
		        </div>
	        </div>
        </div>
    </main>

    @Html.Action("NoteFooterPartialView", "Note")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/note/vital")
</body>
