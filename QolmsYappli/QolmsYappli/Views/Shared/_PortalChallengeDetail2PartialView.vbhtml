@Imports MGF.QOLMS.QolmsYappli
@Imports MGF.QOLMS.QolmsCryptV1
@ModelType PortalChallengeDetailViewModel

@Code
    ViewData("Title") = "チャレンジ"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"
    
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
    
    
End Code

  
    <section class="contents-area">

        <p class="section default">
            エントリー完了しました。<br />
            Fitbit連携をしてください。
        </p>
        
        <a href="../portal/fitbitconnection?FromPageNo=25" class="btn btn-submit block mb20">
            Fitbit連携
        </a>
        
        <a href="../Portal/ChallengeEdit?key=@key" class="btn btn-submit block mb20">
            エントリー情報編集
        </a>

    </section>
        