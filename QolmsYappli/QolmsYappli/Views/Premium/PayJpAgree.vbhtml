@Imports MGF.QOLMS.QolmsYappli
@ModelType PremiumPayJPAgreeViewModel

@Code
    ViewData("Title") = "PayJpAgree"
    Layout = "~/Views/Shared/_PremiumLayout.vbhtml"
End Code

@*<h2>PayJpAgree</h2>

    <a href="../premium/payjpcardregister">payjpcardregister</a>
    <a href="../premium/PayJpCardUpdate">PayJpCardUpdate</a>*@


<body id="premium-terms" class="lower">

    <main id="main-cont" class="clearfix" role="main">
        <section class="home-btn-wrap">
            <a href="../premium/" class="home-btn type-2"><i class="la la-angle-left"></i><span> 戻る</span></a>
        </section>
        <section class="contents-area mb20">
            <div class="box type-2">
                <div class="wrap">
                    <div class="document-scroll-wrap">
                        <h3 class="title">プレミアム参加同意書</h3>
                        @*<ul class="list disc-out text-color">
            <li>黒丸付きリスト</li>
            <li>リスト</li>
            <li>リスト</li>
        </ul>
        <h4 class="title mt20">中タイトル</h4>
        <ul class="list kome">
            <li>コメマーク付きリスト</li>
            <li>リスト</li>
            <li>リスト</li>
        </ul>
        <h5 class="title mt20 bold">小タイトル</h5>
        <ol class="num">
            <li>コメマーク付きリスト</li>
            <li>リスト</li>
            <li>リスト</li>
        </ol>*@

                    <p>

                        @Html.Raw(Html.Encode(Me.Model.Terms).Replace(vbCr, "<br />"))
                    </p>
                    </div>

                    <div class="bottom-fix-wrap mt10">
                        <p class="center mb10">
                            <a href="native:/tab/bio/0a078204" class="bold">
                                JOTOホームドクター プライバシーポリシー
                            </a>

                        </p>
                        <p class="center mb10">

                            <a href="native:/action/open_browser?url=https%3A%2F%2Fwww.ppc.go.jp%2F" class="bold">
                                個人情報保護委員会のホームページ
                            </a>
                        </p>
                        <p class="center mb20">
                            <label class="accept">
                                <input type="checkbox" id="policy">
                                プライバシーポリシーを確認し、同意しました。<span>必須</span>
                            </label>
                        </p>
                        <div class="submit-area two-pane">
                            @*<a href="" class="btn btn-close no-ico">
                                同意しない
                            </a>*@
                            <a href="@Model.url" class="btn btn-default agree disabled">
                                次へ
                            </a>

                        </div>
                    </div>
                </div>
            </div>
        </section>
    </main>
</body>
@QyHtmlHelper.RenderScriptTag("~/dist/js/premium/payjpagree")

<script type="text/javascript">

</script>
