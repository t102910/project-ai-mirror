@Imports MGF.QOLMS.QolmsYappli
@Imports MGF.QOLMS.QolmsCryptV1
@ModelType PortalChallengeColumnViewModel

@Code
    ViewData("Title") = "チャレンジ"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"
    
    Dim dockDispKey As Guid = Guid.Parse("ded05070-8718-4313-924a-25233e35e218")

    Dim first As String = String.Empty
    If Me.TempData("first") = True Then
        first = "first"
    End If
    Dim challengeKey As String = String.Empty
    @Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsWeb)
        
        challengeKey = crypt.EncryptString(Me.Model.ChallengeColumnItemN.First().Challengekey.ToString())
    End Using
    
End Code

<body id="column" class="lower">
    @Html.AntiForgeryToken()
    
<main id="main-cont" class="clearfix" role="main">

    @Select Case Me.Model.FromPageNoType
        
        Case QyPageNoTypeEnum.PortalHome
            
            @<section class="home-btn-wrap" data-pageno="1">
                   <a href="../Portal/Home" class="home-btn"><i class="la la-angle-left"></i><i class="la la-home la-15x"></i><span> ホーム</span></a>
            </section>  
            
        Case Else
        	@<section class="home-btn-wrap">
		        <a href="../Portal/challenge" class="home-btn type-2"><i class="la la-angle-left"></i><span> 戻る</span></a>
	        </section>
    End Select


	<section class="contents-area mb20">
        <!-- 医師の紹介 -->
		<div class="box type-2">
			<h3 class="title type-2">
				はじめに
			</h3>
			<div class="inner">
				<ul class="column-list dr">
					<li class="">
						<a class="dr" href="#" >
							<span class="photo">
								<img src="/dist/img/column/dr-1.png" />
							</span>
							<p>
								監修医師の紹介
							</p>
						</a>
					</li>
				</ul>
			</div>
		</div>

        <!-- コラム一覧 -->
		<div class="box type-2">
			<h3 id="target-column" class="title type-2" data-key="@challengeKey" data-target-column="@Me.Model.TargetColumnNo">
                健康コラム
			</h3>
            
                @For Each item As ChallengeColumnItem In Me.Model.ChallengeColumnItemN
                    
                    'Dim reference as new ColumnImageStorageReferenceJsonParameter()with{
                    '    .AccountKey = Me.Model.AuthorKey,
                    '    .FileKey = item.}
                    
                @<div class="inner">
				    <ul class="column-list">
					    <li class="@IIf(item.UserReadFlag, "read", String.Empty).ToString()">
						    <a class="column" href="#" data-key="@challengeKey" data-no="@item.ColumnNo">
							    <span class="photo">
								    <img class="inview" src="../dist/img/tmpl/ajax-loader2.gif" data-reference="@QyAccountItemBase.CreateThumbnailPhotoUri(item.ThumbnailKey, 1, "Portal", "GetColumnImage", QyAccountItemBase.EncryptPhotoReference(Me.Model.AuthorKey, item.ThumbnailKey, QyFileTypeEnum.Thumbnail))"/>@*src="/dist/img/column/thumb-1-unread.jpg"*@
							    </span>
							    <p>
								    @item.Title
							    </p>
							    <span class="date">
								    @item.UserDispDate.toString("yyyy年MM月dd日配信")
							    </span>
						    </a>
					    </li>
					    @*<li class="read">
						    <a class="column" href="#">
							    <span class="photo">
								    <img src="/dist/img/column/thumb-1.jpg" />
							    </span>
							    <p>
								    果物と野菜【既読表示】
							    </p>
							    <span class="date">
								    2021年8月12日配信
							    </span>
						    </a>
					    </li>*@
				    </ul>
			    </div>
                Next
	
		</div>
	</section>	
</main>


<div class="modal fade column-modal" id="dr-introduction" tabindex="-1"">
	<div class="modal-dialog">
		<div class="modal-content">
			<div class="header">
				<h4 class="modal-title">
            監修医師の紹介
					<button type="button" class="close" data-dismiss="modal"></button>
				</h4>
			</div>
			<img class="w-max" src="/dist/img/column/dr-1-title.png" />
			<div class="modal-body">
				<p>
					皆さん、こんにちは。<br>
					この健康アプリを利用してくれてありがとうございます。私は沖縄生まれ、
					沖縄育ちです。小学から中学までは佐敷、そして知念高校、琉球大学医学部を卒業して医師になりました。<br>
					その後、米国ハーバード大学大学院に留学して公衆衛生学修士号を取得しました。そのときに勉強したのが、栄養疫学です。
				</p>
				<p>
					今回のアプリでの栄養や生活習慣についてのアドバイスは栄養疫学という分野から得られた最新の知識をもとにしています。<br>
					実際に、減量に成功したり、血糖やコレステロール、中性脂肪などを下げたりする食事内容や生活習慣は何か？を追及する学問です。
				</p>
				<p>
					私は現在、群星沖縄臨床研修センターを軸にして、県内の研修病院を周り、研修医を指導しています。<br>
					研修医にも栄養疫学の大切さを教えています。例えば、果物とフルーツジュースは栄養学では同じですが、栄養疫学では異なります。果物をなまで食べるととても健康的ですが、ジュースにして飲むと太ります。<br>
					ジャガイモは栄養学では野菜ですが、栄養疫学では太る糖質です。<br>
					研修医にはこんなことも教えています。
				</p>
				<p>
					では、これから３ヶ月間、私と一緒に最新の健康情報をゲットして、減量と健康数値の改善を目指して頑張りましょう！
				</p>
			</div>
		</div>
	</div>
</div>

<div class="modal fade column-modal" id="column-detail" tabindex="-1">
	<div class="modal-dialog">
		 <div class="modal-content">
			    <div class="modal-header">
				    <button type="button" class="close"><span>×</span></button>
				    <h4 class="modal-title">title</h4>
			    </div>
			    <div class="modal-body">
                    contents
			    </div>
			    <div class="modal-footer">
				    <button type="button" class="btn btn-close no-ico mb0">閉じる</button>
			    </div>
		    </div>
	</div>
</div>



    @Html.Action("PortalFooterPartialView", "Portal")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/portal/challengecolumn")

</body>