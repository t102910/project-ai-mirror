@Imports MGF.QOLMS.QolmsYappli
@Imports MGF.QOLMS.QolmsCryptV1

@ModelType PortalChallengeViewModel

@Code
    ViewData("Title") = "チャレンジ"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"
    
    Dim now As Date = Date.Now
End Code

<body id="challenge" class="lower">
    @Html.AntiForgeryToken()
    
<main id="main-cont" class="clearfix" role="main">

    @If Me.TempData("Nodata") Then

        @<div>nodata</div>
    End If

	<section class="contents-area mb20">

        @For Each item As ChallengeItem In Me.Model.ChallengeItemN
                @code
                    Dim challenging As String = String.Empty
                    Dim caution As String = String.Empty

                    'マスタのエントリー終了

                    If item.EntryEndDate.Date < now.Date _
                        OrElse (Date.MinValue < item.UserEndDate AndAlso item.UserEndDate.Date < now.Date) Then
                        challenging = "closed"

                    ElseIf item.UserStartDate > Date.MinValue Then

                        challenging = "challenging"

                    End If



                    'If item.UserStartDate > Date.MinValue Then
                    '    'とりあえずデータがあれば参加中にする

                    '    If item.UserEndDate.Date < now.Date Then
                    '        'データありかつ終了日が本日より前
                    '        challenging = "closed"
                    '    Else
                    '        challenging = "challenging"
                    '    End If
                    'Else
                    '    If item.EntryEndDate.Date < now.Date Then
                    '        challenging = "closed"

                    '    End If

                    'End If

                    'Dim now As Date = Date.Now

                    'If item.UserStartDate < now AndAlso now < item.UserEndDate Then
                    'challenging = "challenging"
                    'End If

                    'todo: 現状通知機能は必要ないので後で実装
                    'If item.StatusType Then
                    'caution = "caution"
                    'End If

                    Dim key As String = String.Empty
                    Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsWeb)

                        key = crypt.EncryptString(item.Challengekey.ToString())

                    End Using

                End Code
            @<a class="box type-2" href="#" data-key="@key" data-date="@item.UserEndDate.Date.ToString()">

			    <main class="@challenging"><!-- チャレンジ中なら.challenging、終了なら.closed、入力なしなら.cautionを追加 -->
				    <img class="main-photo mb20" src="@(item.ImageSrc + "JOTO_3.jpg")" alt="">

			    </main>
		    </a>
        Next

		<div class="box type-2 comming-soon">
			<div class="wrap">
				comming-soon...
			</div>
		</div>
	</section>	
</main>

    @Html.Action("PortalFooterPartialView", "Portal")

    @QyHtmlHelper.RenderScriptTag("~/dist/js/portal/challenge")

</body>