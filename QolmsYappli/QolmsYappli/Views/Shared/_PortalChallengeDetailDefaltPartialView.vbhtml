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


<section class="contents-area">

    <a href="../Portal/ChallengeEdit?key=@key" class="btn btn-submit block mb20">
        エントリー情報編集
    </a>

</section>
@*    <div class="wrap">
        <a class="mb10 block" href="../Portal/challengecolumn?key=@key">
            <img class="w-max" src="/dist/img/challenge/tmpl/btn-4.jpg">
        </a>
    </div>*@
