@ModelType PortalHomeViewModel

@Code
    ViewData("Title") = "ホーム"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"

    Dim premiumClass As String = String.Empty
    If Me.Model.MembershipType = QyMemberShipTypeEnum.Premium OrElse Me.Model.MembershipType = QyMemberShipTypeEnum.LimitedTime Then
        premiumClass = "premium-member"
    ElseIf Me.Model.MembershipType = QyMemberShipTypeEnum.Business OrElse Me.Model.MembershipType = QyMemberShipTypeEnum.BusinessFree Then
        premiumClass = "biz-member"
    End If

    Dim isJoiningChallenge As Boolean = Me.Model.ChallengeAreaPartialViewModel.Challengekey <> Guid.Empty
    Dim today As Date = Date.Now ' New Date(2024, 9, 1, 1, 0, 0)
    Dim ginowanStartDate As Date = New Date(2025, 4, 1, 9, 0, 0)

End Code
<link rel="stylesheet" href="https://maxst.icons8.com/vue-static/landings/line-awesome/line-awesome/1.1.0/css/line-awesome.min.css">

<body id="global-index" class="@premiumClass">

    @Html.AntiForgeryToken()

    <main id="main-cont" class="clearfix" role="main">

        <div class="news-area @IIf(isJoiningChallenge, "hide", String.Empty).ToString()">
            <article id="newsWrap">
                <ul id="js-news" class="js-hiddenXX clearfix">
                </ul>
            </article>
        </div>

        @code
            ''（ちゅらウォーク＞）ぎのわん 優先順に大きい数値を振る
            '左右バナー表示は要検証

            Dim bunnerDic As New Dictionary(Of String, Integer)()

            'bunnerDic.Add("AlkooWalkEvent", 10)
            If Me.Model.GinowanStatusType = 2 Then

                '新ぎのわん
                bunnerDic.Add("NewGinowan", 9)

            Else
                If Me.Model.GinowanStatusType <= 0 _
                    AndAlso Me.Model.ChallengeList.IndexOf(Guid.Parse("3AB347FB-CFDF-4FF9-BA5E-A2A607B7EC29")) >= 0 Then
                    '旧ぎのわんチャレンジ
                    bunnerDic.Add("OldGinowan", 8)
                End If
            End If

            '伊平屋/竹富
            If ginowanStartDate > today Then
                If Me.Model.ChallengeList.IndexOf(Guid.Parse("6550b115-7411-4c0e-9ac3-9121ac7093b1")) >= 0 Then

                    bunnerDic.Add("taketomi", 7)

                End If

                If Me.Model.ChallengeList.IndexOf(Guid.Parse("a87c8371-e556-4d0f-8cae-c08b8f5a3d2c")) >= 0 Then
                    bunnerDic.Add("iheya", 6)

                End If
            End If

            Dim sortedItems As List(Of KeyValuePair(Of String, Integer)) = bunnerDic.OrderByDescending(Function(i) i.Value).ToList()
            ' 上位2つの要素を選ぶ
            Dim topTwoItems As List(Of String) = sortedItems.Take(2).Select(Function(i) i.Key).ToList()

            Dim urlList As New List(Of String)
            Dim fullBunnerList As New List(Of String)
            Dim halfBunnerList As New List(Of String)

            For Each item In topTwoItems

                Select Case item
                    Case "NewGinowan"

                        'ぎのわん市民確認
                        urlList.Add("native:/tab/custom/15a64d9c")
                        fullBunnerList.Add("../dist/img/index/banner-top-ginowan.png")
                        halfBunnerList.Add("../dist/img/index/banner-top-ginowan-2.png")

                    Case "OldGinowan"
                        If ginowanStartDate < today Then
                            '宜野湾開始後
                            urlList.Add("native:/tab/atom/bd782f6b?page_id=01JQNN8GEMYDBB1VE40VDEF4QD&type=basic")
                            fullBunnerList.Add("../dist/img/index/banner-top-oldginowan.png")
                            halfBunnerList.Add("../dist/img/index/banner-top-oldginowan-2.png")

                        Else

                            urlList.Add("native:/tab/custom/15a64d9c")
                            fullBunnerList.Add("../dist/img/index/banner-top-ginowan.png")
                            halfBunnerList.Add("../dist/img/index/banner-top-ginowan-2.png")

                        End If

                    Case "taketomi"
                        '竹富町参加
                        urlList.Add("native:/tab/custom/9986a1df")
                        fullBunnerList.Add("../dist/img/index/banner-top-taketomi.png")
                        halfBunnerList.Add("../dist/img/index/banner-top-taketomi-2.png")

                    Case "iheya"
                        '伊平屋村参加
                        urlList.Add("native:/tab/custom/60109d4d")
                        fullBunnerList.Add("../dist/img/index/banner-top-iheya.png")
                        halfBunnerList.Add("../dist/img/index/banner-top-iheya-2.png")

                    Case "AlkooWalkEvent"
                        ''イベントが開催決定されてから実装予定
                        'urlList.Add("native:/tab/custom/15a64d9c")
                        'fullBunnerList.Add("../dist/img/index/banner-top-nav2024-now.png")
                        'halfBunnerList.Add("../dist/img/index/banner-top-nav2024-2-now.png")

                End Select

            Next
        End Code

        @If urlList.Count = 2 Then
                @<div class="" style="display:flex;">
                    <a class="" href="@urlList.First()" style="width:50%;">
                        <i><img src="@halfBunnerList.First()" class="w-max" alt=""></i>
                    </a>
                    <a class="" href="@urlList.Item(1)" style="width:50%;">
                        <i><img src="@halfBunnerList.Item(1)" class="w-max" alt=""></i>
                    </a>

                </div>

        ElseIf urlList.Count = 1 Then
                @<div>
                    <a class="" href="@urlList.First()">
                        <i class="w-max"><img src="@fullBunnerList.First()" alt="" width="100%"></i>
                    </a>
                </div>
        End If

        @If isJoiningChallenge Then
            'チャレンジ参加中
            @Html.Action("PortalHomeChallengeAreaPartialView", "Portal")

        End If

        @Html.Action("PortalHomeDataAreaPartialView", "Portal")

        <section class="wrapper">
            <section id="menu-nav">
                <ul>
                    <li>
                        <a href="../note/vital?tabno=1" style="padding-right: 0px; padding-left: 0px; max-width:90px;">
                            <i class="body-weight"><img src="/dist/img/index/menu-nav-spacer.png" alt=""></i>
                            体重
                        </a>
                    </li>
                    <li>
                        <a href="../note/vital?tabno=2" style="padding-right: 0px; padding-left: 0px;  max-width:90px;">
                            <i Class="blood-pressure"><img src="/dist/img/index/menu-nav-spacer.png" alt=""></i>
                            血圧
                        </a>
                    </li>
                    <li>
                        <a href="../note/vital?tabno=3" style="padding-right: 0px; padding-left: 0px;  max-width:90px;">
                            <i Class="glucose-level"><img src="/dist/img/index/menu-nav-spacer.png" alt=""></i>
                            血糖値
                        </a>
                    </li>
                    <li>
                        <a href="../note/heartrate?d=d" Class="fitbit-check" style="padding-right: 0px; padding-left: 0px;  max-width:90px;">
                            <i Class="heart-beat"><img src="/dist/img/index/menu-nav-spacer.png" alt=""></i>
                            心拍数
                        </a>
                    </li>
                    <li>
                        <a href="../note/mets?d=d" Class="fitbit-check" style="padding-right: 0px; padding-left: 0px;  max-width:90px; ">
                            <i Class="exercise"><img src="/dist/img/index/menu-nav-spacer.png" alt=""></i>
                            運動強度
                        </a>
                    </li>

                </ul>
            </section>
            <h3 Class="title-2 no-wrap">JOTOインフォメーション</h3>
            <section id="localnav">
                <div class="inner scroll-inner" data-count="0">
                    <ul id="menu-slider" class="localbtn__wrap move">

                        @If ginowanStartDate < today Then

                            @<li class="localbtn">
                                @*<!--ぎのわん市民確認-->*@
                                <a href="../Portal/LocalIdVerification">
                                    <img src="../dist/img/index/banner-29.jpg" alt="">
                                </a>
                            </li>
                        End If

                        @If Me.Model.LinkageList.Contains(47000020) Then 'すながわ内科

                            @<li Class="localbtn">
                                @*<!--問診-->*@
                                @*<a href="../portal/monshin" Class="" id="">*@
                                <a href="" Class="" id="monshin">
                                    @*../portal/monshin *@
                                    <img src="../dist/img/index/banner-28.jpg" alt="">
                                </a>
                            </li>
                        End If

                        <!--<li Class="localbtn">-->
                            @*<!--ちゅらウォーク2023 native:/tab/custom/452d91fd(https://static.cld.navitime.jp/walkingapp-storage/citrus/okinawa-cellular/lp/html/information.html)-->*@
                            <!--<a href="">
                                <img src="../dist/img/index/banner-26.png" alt="">
                            </a>
                        </li>-->

                        <li class="localbtn">
                            @*<!--JOTO初めてガイド-->*@
                            <a href="native:/tab/custom/0cf03827">
                                <img src="../dist/img/index/banner-1.jpg" alt="">
                            </a>
                        </li>

                        @If premiumClass = "biz-member" Then

                            @<li class="localbtn">
                                @*<!--biz-->*@
                                <a href="../portal/CompanyConnectionHome?frompageno=1">
                                    <img src="../dist/img/index/banner-13.jpg" alt="">
                                </a>
                            </li>
                        End If


                        <li class="localbtn">
                            @*<!--オンライン診療  https://app.curon.co/add-clinic/search?pref=okinawa -->*@
                            <a href="native:/action/open_browser?url=https%3A%2F%2Fapp.curon.co%2Fadd-clinic%2Fsearch%3Fpref%3Dokinawa">
                                <img src="../dist/img/index/banner-4.jpg" alt="">
                            </a>
                        </li>
                        <!--<li class="localbtn">-->
                            @*<!--すこやか薬局処方薬ロッカー受け渡しサービス -->*@
                            <!--<a href="native:/tab/custom/d8fd56e6">
                                <img src="../dist/img/index/banner-3.jpg" alt="">
                            </a>
                        </li>-->
                        <li class="localbtn">
                            @*<!--お薬手帳-->*@
                            <a href="../portal/MedicienConnectionPromote">
                                <img src="../dist/img/index/banner-14.jpg" alt="">
                            </a>
                        </li>

                        <li class="localbtn">
                            @*<!--ガルフ-->*@
                            <a href="../note/GulfSportsMovieIndex">
                                <img src="../dist/img/index/banner-7.jpg" alt="">
                            </a>
                        </li>
                        <li class="localbtn">
                            @*<!--レシピ（仮）-->*@
                            <a href="../note/RecipeMovieIndex">
                                <img src="../dist/img/index/banner-9.jpg" alt="">
                            </a>
                        </li>

                    </ul>
                </div>
            </section>
            <h3 Class="title-2 no-wrap">安心サポート</h3>
            <section Class="link-btn-area">
                <div Class="inner">
                    <a id="examination" Class="" href="javascript:void(0);">
                        <img src="../dist/img/index/support-1.jpg" alt="" width=""><span>健康診断</span>
                    </a>
                    <a Class="loading-link" href="../Health/Age?FromPageNo=1">
                        <i Class="healthy-age"><img src="../dist/img/index/support-spacer.png" alt="" width=""></i><span>健康年齢</span>
                    </a>
                    <a Class="" href="../portal/search">
                        <i Class="medical-search"><img src="../dist/img/index/support-spacer.png" alt="" width=""></i><span>医療機関検索</span>
                    </a>
                    <a Class="" href="native:/tab/bio/8dd21169">
                        <img src="../dist/img/index/support-4.jpg" alt="" width=""><span>医療費あと払い</span>
                    </a>
                    <a Class="../dist/img/sample/" href="../note/medicine">
                        <i Class="medicine-note"><img src="../dist/img/index/support-spacer.png" alt="" width=""></i><span>おくすり手帳</span>
                    </a>

                </div>
            </section>
            <h3 Class="title-2 no-wrap">連携</h3>
            <section Class="link-btn-area">
                <div Class="inner">
                    <a Class="" href="../portal/connectionsetting?FromPageNo=1&TabNo=3">
                        <img src="../dist/img/index/federation-1.jpg" alt="" width=""><span>医療機関</span>
                    </a>
                    @code
                        Dim alkooFlag As String = If(Me.Model.AlkooConnectedFlag, "true", String.Empty)
                    End Code
                    <a id="alkoo" Class="@alkooFlag" href="native:/tab/custom/8287f3fa">
                        <img src="../dist/img/index/federation-2.jpg" alt="" width=""><span>ALKOO<br>by Navitime</span>
                    </a>
                    <a Class="" href="native:/tab/custom/782690f6">
                        <img src="../dist/img/index/federation-3.jpg" alt="" width=""><span>タニタヘルス<br>プラネット</span>
                    </a>

                    <a Class="" href="../portal/connectionsetting?FromPageNo=1&TabNo=2">
                        <img src="../dist/img/index/federation-5.jpg" alt="" width=""><span>For Business</span>
                    </a>
                    <a Class="" href="../portal/connectionsetting?FromPageNo=1&TabNo=4">
                        <i Class="medicine-note"><img src="../dist/img/index/support-spacer.png" alt="" width=""></i><span>薬局連携</span>
                    </a>
                    <a Class="" href="../portal/fitbitconnection?FromPageNo=1">
                        <img src="../dist/img/index/federation-6.jpg" alt="" width=""><span>Fitbit連携</span>
                    </a>
                    @*<a class="" href="../premium">
                            <img src="../dist/img/index/federation-5.jpg" alt="" width=""><span>Premium</span>
                        </a>*@
                </div>
            </section>
        </section>
    </main>
    <div Class="modal fade" id="message-modal" tabindex="-1">
        <div Class="modal-dialog">
            <div Class="modal-content">
                <div Class="modal-header">
                    <Button type="button" Class="close" data-dismiss="modal"><span>×</span></Button>
                    <h4 Class="modal-title">タイトル</h4>
                </div>
                <div Class="modal-body">
                    メッセージが入ります
                </div>
                <div Class="modal-footer">
                    <Button type="button" Class="btn btn-close" data-dismiss="modal">閉じる</Button>
                </div>
            </div>
        </div>
    </div>
    @If Me.Model.IsCalomealAiFeedback Then
        @<div Class="modal fade" id="ai-modal" tabindex="-1">
            <div Class="modal-dialog">
                <div Class="modal-content">
                    <div Class="modal-header">
                        <Button type="button" Class="close" data-dismiss="modal"><span>×</span></Button>
                        <h4 Class="modal-title">通知</h4>
                    </div>
                    <div Class="modal-body">
                        新着のAIフィードバックメッセージがあります。
                    </div>
                    <div Class="modal-footer">
                        <Button type="button" Class="btn btn-close" data-dismiss="modal">閉じる</Button>
                    </div>
                </div>
            </div>
        </div>

    End If

    @Html.Action("PortalFooterPartialView", "Portal")
    <link rel="stylesheet" href="../dist/css/slick.css" type="text/css"><!-- slider -->
    <link rel="stylesheet" href="../dist/css/slick-theme.css" type="text/css"><!-- slider -->
    @QyHtmlHelper.RenderScriptTag("~/dist/js/portal/home")

    @*Push通知のテスト中なので一旦しばらくコメントアウト
        @If HttpContext.Current.IsDebuggingEnabled = False Then
        @*@
        <script type = "text/javascript" >
             Console.log("IsDebuggingEnabled =false");
        </script>
        @If Not String.IsNullOrWhiteSpace(Me.Model.AuthorKeyHash) Then
            'Me.Model.EnableYappliSdk AndAlso
            @<script type="text/javascript" src="https://yappli.io/v1/sdk.js"></script>

            @<script>

                     function sendYappliRegist(a) {
                         //reproへのID連携
                         Yappli.setMemberId(a);
                         //YappliへのID連携
                         Yappli.sendRegisteredId(a, function (b, c) {
                             if (!b) {
                                 setTimeout(function () { sendYappliRegist(a) }, 1000.0)
                             }
                         })
                     }
                     sendYappliRegist("@Me.Model.AuthorKeyHash");
            </script>

        End If

    @*End If*@

</body>
