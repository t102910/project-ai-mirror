@Imports MGF.QOLMS.QolmsYappli
@Imports MGF.QOLMS.QolmsCryptV1
@ModelType PortalChallengeDetailViewModel

@Code
    ViewData("Title") = "チャレンジ"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"
    
    Dim dockDispKey As Guid = Guid.Parse("ded05070-8718-4313-924a-25233e35e218")

    Dim first As String = String.Empty
    If Me.TempData("first") = True Then
        first = "first"
    End If
    
    Dim key As String = String.Empty
    Dim externalid As String = String.Empty
    Using resource As New QsCrypt(QsCryptTypeEnum.QolmsWeb)

        key = resource.EncryptString(Me.Model.ChallengeItem.Challengekey.ToString())
        externalid = resource.EncryptString(Me.Model.ChallengeItem.ExternalId)
    End Using

    Dim today As Date = Date.Now
    
    'Todo:目標達成したかどうかはAPI側で計算するようにする
    
End Code

<body id="challenge" class="lower">
    @Html.AntiForgeryToken()
    

<main id="main-cont" class="clearfix" role="main">
	<section class="home-btn-wrap">
		<a href="../Portal/challenge" class="home-btn type-2"><i class="la la-angle-left"></i><span> 戻る</span></a>
	</section>
	<section class="contents-area mb20 @first" data-date="@Me.Model.ChallengeItem.UserEndDate.Date.ToString()">
        <div class="box type-2 pb10">
            <main>

                @If today < Me.Model.ChallengeItem.UserStartDate Then

                    @<img class="main-photo mb20" src="@(Me.Model.ChallengeItem.ImageSrc + "JOTO_1_1.jpg")" alt="">
                Else
                    @<img class="main-photo mb20" src="@(Me.Model.ChallengeItem.ImageSrc + "JOTO_3.jpg")" alt="">

                End If
            </main>

            @*チャレンジステータスの表示*@
            @*@If Me.Model.ChallengeItem.UserEndDate.Date >= today.Date Then

                @<p class="now-challenging">
                    参加中
                </p>
            Else
                @<p class="now-challenging closed">
                    終了
                </p>
            End If*@


            @If Me.Model.ChallengeItem.EntryEndDate.Date < Now.Date OrElse Me.Model.ChallengeItem.UserEndDate.Date < Now.Date Then
                @<p class="now-challenging closed">
                    終了
                </p>
            ElseIf Me.Model.ChallengeItem.UserStartDate > Date.MinValue Then

                @<p class="now-challenging">
                    参加中
                </p>
            End If



            @*チャレンジ毎にPartialViewへ*@
            @Html.Action("PortalChallengeDetailPartialView", "Portal")

        </div>
	</section>	
</main>
    
    @Html.Action("PortalFooterPartialView", "Portal")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/portal/challengedetail")

</body>