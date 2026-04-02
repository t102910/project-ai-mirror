@ModelType PortalHistoryViewModel

@Code
    ViewData("Title") = "JOTOポイント"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"

    Dim yearList As New List(Of Integer)()
    Dim monthList As New List(Of Integer)() From {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12}

    For a As Integer = 2019 To Date.Now.Year
        yearList.Add(a)
    Next

    Dim today As Date = Date.Now

    Dim isPontaDisabled As String = String.Empty
    If today >= New Date(2025, 11, 7, 1, 0, 0) AndAlso today <= New Date(2025, 11, 7, 6, 0, 0) _
        Or Not Me.Model.IsAuId Then
        isPontaDisabled = "disabled"
    End If

    '表示したいチャレンジのGUIDを宣言してください。
    Dim iheya As Guid = Guid.Parse("a87c8371-e556-4d0f-8cae-c08b8f5a3d2c")
    Dim taketomi As Guid = Guid.Parse("6550b115-7411-4c0e-9ac3-9121ac7093b1")

    '表示したいチャレンジを追加したらこちらに条件を追加してください。
    Dim isChallenge As Boolean = Me.Model.ChallengeEntryList.ContainsKey(iheya) _
        OrElse Me.Model.ChallengeEntryList.ContainsKey(taketomi)

End Code

<body id="point-log" class="lower">
    @Html.AntiForgeryToken()
    @*@Html.Action("PortalHeaderPartialView", "Portal")*@

    <main id="main-cont" class="clearfix" role="main">
        @Select Case Me.Model.FromPageNoType
            Case QyPageNoTypeEnum.PortalHome
                @<section class="home-btn-wrap" data-pageno="1">
                    <a href="../Portal/Home" class="home-btn"><i class="la la-angle-left"></i><i class="la la-home la-15x"></i><span> ホーム</span></a>
                </section>
        End Select
    <section class="contents-area">
        <h2 class="title relative">
            JOTOポイント
        </h2>
        <hr />
        <section class="result-area">
            <img id="point-svg" src="../dist/img/index/points.svg">
            <h2>累計ポイント</h2>
            <h3 id="point-field">@Me.Model.Point.ToString(("###,##0"))<i>pt</i></h3>
            @If Me.Model.ClosestExprirationDate <> Date.MinValue AndAlso Me.Model.ClosestExprirationPoint > 0 Then
                @<p class="end">@String.Format("（{0:yyyy年M月末}に失効 {1:###,##0}pt）", Me.Model.ClosestExprirationDate, Me.Model.ClosestExprirationPoint)</p>
            End If
        </section>

        <section class="section default">
            JOTOホームドクターをご利用いただきありがとうございます。ポイント交換につきまして、停止しておりましたPontaポイントおよびAmazonギフト券への交換を、下記会員に限り再開いたします。<br />
            今後ともJOTOホームドクターをよろしくお願いいたします。<br />
            <br />
            【交換対象会員】<br />
            ・プレミアム会員<br />
            ・For Business契約企業の会員<br />
            ・「ぎのわんスマート健康増進プロジェクト」市民確認承認済みの会員
        </section>

        <h3 class="title mt40">履歴</h3>

        <div class="flex-item">
            <div class="input-group datetime-select">
                <select name="Year" class="form-control">
                    @For Each item As Integer In yearList.OrderByDescending(Function(i) i)
                        @<option value="@item.ToString()" @IIf(item = Me.Model.Year, "selected=""selected""", String.Empty)>@item.ToString()</option>
                    Next
                </select>
                <span class="input-group-addon">年</span>

                <select name="Month" class="form-control minute">
                    @For Each item As Integer In monthList
                        @<option value="@item.ToString()" @IIf(item = Me.Model.Month, "selected=""selected""", String.Empty)>@item.ToString()</option>
                    Next
                </select>
                <span class="input-group-addon minute2">月</span>
            </div>
            <div class="submit">
                <a href="javascript:void(0);" class="btn btn-submit narrow">変更</a>
            </div>
        </div>

        @If Me.Model.PointDailyLogN.Any() Then
            @Code
                Dim isEven As Boolean = True
                Dim dayOfWeek As String = String.Empty
            End Code

            @<table id="log-table">
                @For Each dailyLogItem As JotoPointDailyLogItem In Me.Model.PointDailyLogN
                    @Code
                        isEven = Not isEven

                        Select Case dailyLogItem.ActionDate.DayOfWeek
                            Case 0
                                dayOfWeek = "sunday"
                            Case 6
                                dayOfWeek = "saturday"
                            Case Else
                                dayOfWeek = String.Empty
                        End Select
                    End Code

                    @<thead class="@IIf(isEven, "even", String.Empty)">
                        <tr>
                            <td colspan="2">@String.Format("{0:yyyy年M月d日}", dailyLogItem.ActionDate)<span class="@dayOfWeek">@String.Format("（{0:ddd}）", dailyLogItem.ActionDate)</span></td>
                            <td>
                                @Select Case True
                                    Case dailyLogItem.Point > 0
                                        @<span class="plus">@String.Format("{0:+###,##0;-###,##0;##0}", dailyLogItem.Point)<i>pt</i></span>
                                                                Case dailyLogItem.Point < 0
                                                                    @<span class="minus">@String.Format("{0:+###,##0;-###,##0;##0}", dailyLogItem.Point)<i>pt</i></span>
                                                                Case Else
                                                                    @<span class="">@String.Format("{0:+###,##0;-###,##0;##0}", dailyLogItem.Point)<i>pt</i></span>
                                                            End Select
                            </td>
                        </tr>
                    </thead>
                    @<tbody>
                        <tr>
                            <th rowspan="2">付与時刻</th>
                            <th rowspan="2">内容</th>
                            <th>ポイント</th>
                        </tr>
                        <tr>
                            <th>有効期限</th>
                        </tr>
                        @For Each logItem As JotoPointLogItem In dailyLogItem.PointLogN
                            @<tr>
                                <td rowspan="2">@String.Format("{0:H時m分}", logItem.ActionDate)</td>
                                <td rowspan="2">

                                    @logItem.ItemName
                                    @If logItem.TargetDate <> Date.MaxValue Then

                                        @String.Format("{0:(M月d日分)}", logItem.TargetDate)
                                    End If
                                </td>
                                <td>
                                    @Select Case True
                                        Case logItem.Point > 0
                                            @<span class="plus">@String.Format("{0:+###,##0;-###,##0;##0}", logItem.Point)<i>pt</i></span>
                                                                        Case logItem.Point < 0
                                                                            @<span class="minus">@String.Format("{0:+###,##0;-###,##0;##0}", logItem.Point)<i>pt</i></span>
                                                                        Case Else
                                                                            @<span class="">@String.Format("{0:+###,##0;-###,##0;##0}", logItem.Point)<i>pt</i></span>
                                                                    End Select
                                </td>
                            </tr>
                            @<tr>
                                @If logItem.ExpirationDate >= Date.MinValue.Date AndAlso logItem.ExpirationDate < Date.MaxValue.Date Then
                                    @<td class="expire-date">@String.Format("{0:yyyy年M月末}", logItem.ExpirationDate)</td>
                                Else
                                    @<td class="expire-date">&nbsp;</td>
                                End If
                            </tr>
                        Next
                    </tbody>
                Next
            </table>
        Else
            @<section class="section default mt10 mb20">
                @String.Format("{0}年{1}月に、履歴はありません。", Me.Model.Year, Me.Model.Month)
            </section>
        End If
        <section class="section mb20">
            ※ポイント履歴は過去１年分ご確認いただけます。<br />
            <p class="red">※歩数ポイントの反映のタイミングについて</p>
            反映まで数日要する場合がありますのでご了承ください。3日経っても反映されない場合は、アプリ内のお問い合わせからJOTOホームドクター事務局までお問い合わせください。

        </section>

        <h3 class="title">使 う</h3>
        <section class="section default">
            ためたJOTOポイントを使います。
        </section>

        @code
            Dim page As String = String.Empty

            If Me.Model.FromPageNoType = QyPageNoTypeEnum.PortalHome Then

                page = String.Format("?fromPageNo={0}", Convert.ToByte(Me.Model.FromPageNoType))
            End If

        End Code


        <p Class="arrow-area caution"><span class="big">イチオシ！</span></p>

        <a href="@String.Format("../Portal/CouponForFitbit{0}", page)" Class="btn btn-link no-ico back-FFF logona super-btn mb5">
            <i Class="sans-serif">みんな使える</i>
            JOTOオンラインストアのクーポンと交換

        </a>
        <p Class="center mb30">
            スマートウォッチや各種アクセサリーが購入できます。
        </p>

        <p Class="arrow-area caution"><span class="big">1.2</span>倍換算で<span class="big">オトク！</span></p>

        <a href="@String.Format("../Portal/PointExchange{0}", page)" Class="btn btn-link no-ico back-FFF logona super-btn mb5">
            <i Class="sans-serif">みんな使える</i>
            「沖縄CLIPマルシェ」クーポン交換
        </a>
        <p Class="center mb30">沖縄の特産品を販売する通販サイト「沖縄CLIPマルシェ」</p>


        @If Me.Model.IsPremium OrElse Me.Model.IsforBiz OrElse Me.Model.IsConnectedHospital OrElse isChallenge Then

            @<a href="@String.Format("../Portal/aupoint{0}", page)" Class="btn btn-link no-ico back-FFF logona super-btn mb5 @isPontaDisabled">
                <i Class="sans-serif">auID限定</i>
                Pontaポイントと交換
            </a>

            @<p Class="center mb30">au携帯電話料金の充当など利用方法がたくさん</p>
        End If


        <a href="@String.Format("../Portal/Datacharge{0}", page)" Class="btn btn-link no-ico back-FFF logona super-btn mb5 @IIf(Me.Model.IsMobileSubscriberOfAu, String.Empty, "disabled")">
            <i Class="sans-serif">au限定</i>
            auデータと交換(データチャージ)
        </a>
        <p Class="center mb30">
            データチャージへのポイント交換はUQユーザーの方はご利用できません。povo1.0ユーザーの方はご利用可能でございます。
        </p>

        @If Me.Model.IsPremium OrElse Me.Model.IsforBiz OrElse Me.Model.IsConnectedHospital OrElse isChallenge Then

            @<a href="@String.Format("../Portal/amazonpoint{0}", page)" Class="btn btn-link no-ico back-FFF logona super-btn mb5">
                <i Class="sans-serif">みんな使える</i>
                Amazonギフト券と交換
            </a>
            @<p Class="center mb30">Amazonユーザーに朗報!いつものお買い物で使えます</p>
        End If





        <h3 Class="title mt40">ためる</h3>

        <section Class="section mt10 mb10">
            日々の活動でJOTOポイントがたまります。
        </section>

        <Table Class="table table-bordered point-info">
            <thead>
                <tr>
                    <td></td>
                    <td> 単位</td>
                    <td> 会員<br />種別</td>
                    <td> 獲得<br />Point</td>
                    <td> 一日<br />上限</td>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <th rowspan="2">
                        ログイン
                    </th>
                    <td rowspan="2"> 1回</td>
                    <td Class="center" style="font-size:10px;">無料会員</td>
                    <td Class="right">1</td>
                    <td Class="right">1</td>
                </tr>
                <tr Class="even premium">
                    <td Class="right"></td>
                    <td Class="right">2</td>
                    <td Class="right">2</td>
                </tr>
                <tr Class="even">
                    <th rowspan="2">
                        歩数登録 <br />
                        (5000歩以上)<br />
                        ALKOO/タニタ連携のみ
                    </th>
                    <td rowspan="2">
                        1000 <br>
                        歩毎
                    </td>
                    <td Class="center" style="font-size:10px;">無料会員</td>
                    <td Class="right">1</td>
                    <td Class="right">6</td>
                </tr>
                <tr Class="even premium">
                    <td Class="right"></td>
                    <td Class="right">2</td>
                    <td Class="right">12</td>
                </tr>
                <tr Class="even">
                    <th rowspan="2">
                        運動登録
                    </th>
                    <td rowspan="2"> 1種目</td>
                    <td Class="center" style="font-size:10px;">無料会員</td>
                    <td Class="right">1</td>
                    <td Class="right">1</td>
                </tr>
                <tr Class="even premium">
                    <td Class="right"></td>
                    <td Class="right">2</td>
                    <td Class="right">2</td>
                </tr>
                <tr Class="even">
                    <th rowspan="2">
                        食事登録
                    </th>
                    <td rowspan="2">
                        朝食 <br>
                        昼食 <br>
                        夕食 <br>
                        間食
                    </td>
                    <td Class="center" style="font-size:10px;">無料会員</td>
                    <td Class="right">1</td>
                    <td Class="right">1</td>
                </tr>
                <tr Class="even premium">
                    <td Class="right"></td>
                    <td Class="right">2</td>
                    <td Class="right">2</td>
                </tr>
                <tr>

                    <th rowspan="2">
                        バイタル登録
                    </th>
                    <td rowspan="2">
                        体重 <br />
                        血圧 <br />
                        血糖
                    </td>
                    <td class="center" style="font-size:10px;">無料会員</td>
                    <td class="right">1</td>
                    <td class="right">1</td>
                </tr>
                <tr class="premium">
                    <td class="right"></td>
                    <td class="right">2</td>
                    <td class="right">2</td>
                </tr>
                <tr class="even">
                    <th rowspan="2">
                        医療機関連携し <br />
                        健康診断結果反映
                    </th>
                    <td rowspan="2">
                        1回
                    </td>
                    <td class="center" style="font-size:10px;">無料会員</td>
                    <td class="right">100</td>
                    <td class="right">年1回</td>
                </tr>
                <tr class="even premium">
                    <td class="right"></td>
                    <td class="right">200</td>
                    <td class="right">年1回</td>
                </tr>
            </tbody>
        </Table>

        <section class="section mt10 mb10 red">
            ※獲得したポイントは、6ヶ月後に失効しますのでご注意ください。<br />
            ※歩数登録はＡＬＫＯＯ/タニタ連携せず、手入力で登録した場合はポイント付与の対象外となります。

        </section>
    </section>
    </main>

    @Html.Action("PortalFooterPartialView", "Portal")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/portal/history")
</body>
