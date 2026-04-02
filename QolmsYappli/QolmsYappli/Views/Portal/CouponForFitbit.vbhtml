@Imports MGF.QOLMS.QolmsYappli
@ModelType PortalCouponForFitbitViewModel

@Code
    ViewData("Title") = "Fitbit購入値引きクーポンと交換"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"

    Dim isYappli As Boolean = Me.Context.Request.UserAgent.ToLower().Contains("yappli")
    Dim detailedLinks As String = "https%3A%2F%2Fmarche.okinawaclip.com%2F"
End Code
<body id="point-exchange" class="lower">
    @Html.AntiForgeryToken()

    <main id="main-cont" class="clearfix" role="main">
        <section class="contents-area">
            <div id="contents-wrap">
                <h2 class="title relative">
                    JOTOオンラインストアの
                    <br>クーポンと交換
                </h2>
                <hr>
                <p class="exchange-service">
                    <img src="/dist/img/point/joto-online.png">
                </p>
                <p class="section default">
                    @Html.Raw(Me.Model.Description)
                </p>

                <a href="native:/tab/custom/8c665bd5" class="btn btn-submit block">JOTOオンラインストアはこちら</a><!-- ここにヤプリ呼び出しのスキーマ -->
                <a href="native:/tab/bio/07cd8974" class="btn btn-submit block">JOTOポイントプログラム利用規約</a>

                <div class="remaining-point mb20">
                    <h3>
                        現在の保有ポイント
                        <span>@Me.Model.Point<i>pt</i></span>
                    </h3>
                </div>

                @code
                    Dim page As String = String.Empty

                    If Me.Model.FromPageNoType = QyPageNoTypeEnum.PortalHome Then

                        page = String.Format("?fromPageNo={0}", Convert.ToByte(Me.Model.FromPageNoType))
                    End If
                End Code
                <div class="right">
                    <a id="back" href="@String.Format("../Portal/history{0}",page )" class="btn btn-close no-ico" data-back="@page">戻 る</a>
                </div>

                <section id="caution" class="section caution mt10 mb10 hide"><h4>heading...</h4>アラートエリアです。不要時.hideで消してください。</section>

                <h3 class="title mt10">
                    <span>交換クーポンを選択</span>
                </h3>
                <form class="form exchange-form mb30">
                    @For Each item As CouponItem In Me.Model.CouponN
                        If item.Point <= Me.Model.Point AndAlso item.RestCount > 0 Then
                            @<a href="javascript:void(0);" class="btn btn-blue exchange" data-toggle="modal" data-coupon="@item.CouponType"><i>@item.Point</i>P → <i>@item.DispName</i>クーポン</a>

                        Else
                            @<a href="javascript:void(0);" class="btn btn-blue exchange disabled" data-toggle="modal" data-coupon="@item.CouponType"><i>@item.Point</i>P → <i>@item.DispName</i>クーポン</a>

                        End If
                    Next

                </form>
            </div>

            <section class="data-area mb20">
                <h3 class="title mt10" id="hist">
                    <span>クーポン履歴</span>
                </h3>

                @If Me.Model.PointExchangeHistN.Count > 0 Then

                    @For Each item In Me.Model.PointExchangeHistN
                        @<article>
                            <section class="inner">
                                <div class="info">
                                    <p class="date">@item.IssueDate.ToString("MM月dd日 tt hh:mm")</p>
                                    <div class="flex-wrap">
                                        <div class="before">
                                            <strong>JOTOポイント</strong>
                                            <p>
                                                @item.Point<i>pt</i>
                                            </p>
                                        </div>
                                        <p class="arrow">
                                            <i class="la la-arrow-right"></i>
                                        </p>
                                        <div class="after">
                                            <strong>交換クーポン</strong>
                                            <p>
                                                @item.DispName<i>クーポン</i>
                                            </p>
                                        </div>
                                    </div>
                                    @If item.ExpirationDate >= Date.Now Then
                                        @<div class="code">
                                            <strong>発行されたクーポンコード</strong>
                                            <p>@item.CouponId <i class="la la-copy" data-couponid="@item.CouponId" style="font-size:0.75em;margin-left:1em;"></i></p>
                                            <div>@item.ExpirationDate.ToString("yyyy年MM月dd日")</div>
                                        </div>
                                    End If
                                </div>
                            </section>
                        </article>
                    Next
                Else
                    @<p>履歴がありません</p>
                End If
            </section>
        </section>

        <!-- 確認モーダル -->
        <div class="modal fade" id="confirmation-modal" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal"><span>×</span></button>
                        <h4 class="modal-title">確認</h4>
                    </div>
                    <div class="modal-body">
                        <p class="section default">
                            100Pを120円分クーポンと交換しますか？
                        </p>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-close mb0" data-dismiss="modal">閉じる</button>
                        <a href="" class="btn btn-submit" data-toggle="modal" data-target="#coupon-modal-2" data-dismiss="modal">交換する</a>
                    </div>
                </div>
            </div>
        </div>
        <!-- 「登録完了」ダイアログ -->
        @If Me.TempData("IsFinish") IsNot Nothing AndAlso Me.TempData("IsFinish") Then
    @<div class="modal fade" id="finish-modal" tabindex="-1">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal"><span>×</span></button>
                    <h4 class="modal-title">クーポンを発行しました。</h4>
                </div>
                <div class="modal-body">
                    <h3 class="title">クーポンコード</h3>
                    @*<p class="section default">
                    「メールで送信」しておくと便利です。
                    </p>*@
                    <p class="point-conf center mb10">
                        @Me.Model.PointExchangeHistN.First().CouponId
                    </p>
                    @*				            <p class="center">
            <a href="" class="btn btn-submit no-ico"><i class="la la-envelope"></i>この情報をメールで送信</a>
        </p>*@
                </div>
                                    <div class="modal-footer">
                                        <button type = "button" class="btn btn-close mb0" data-dismiss="modal">閉じる</button>
                                    </div>
                                </div>
                            </div>
                        </div>
        End If

        <!-- 「コピー完了」ダイアログ -->
        <div class="modal fade" id="copy-modal" tabindex="-1" data-backdrop="static" data-keyboard="false" data-row-no="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-body">
                        コピーしました。
                    </div>
                </div>
            </div>
        </div>
    </main>
    @Html.Action("PortalFooterPartialView", "Portal")
    @QyHtmlHelper.RenderScriptTag("~/dist/js/portal/couponforfitbit")
</body>

<script>

</script>