@Imports MGF.QOLMS.QolmsYappli
@Imports MGF.QOLMS.QolmsCryptV1
@ModelType PortalChallengeDetailViewModel

@Code
    ViewData("Title") = "チャレンジ"
    Layout = "~/Views/Shared/_PortalLayout.vbhtml"
    
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

        <p class="section default">
            エントリー完了しました。<br />
            タニタヘルスプラネットおよびALKOO連携をしてください。
    
        </p>
        
        <a href="native:/tab/custom/782690f6" class="btn btn-submit block mb20">
            タニタヘルスプラネット連携
        </a>
        
        <a href="native:/tab/custom/8287f3fa" class="btn btn-submit block mb20">
            ALKOO連携
        </a>

        <a href="../Portal/ChallengeEdit?key=@key" class="btn btn-submit block mb20">
            エントリー情報編集
        </a>

    </section>
  
@*            
                    @If today > Me.Model.ChallengeItem.UserStartDate Then
                 
                         @<div class="wrap">
                            <a class="mb10 block" href="../Portal/challengecolumn?key=@key">
				                <img class="w-max" src="/dist/img/challenge/tmpl/btn-4.jpg">
			                </a>
			            </div>
                    End If*@
