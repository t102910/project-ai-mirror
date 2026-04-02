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

            @If Me.Model.ChallengeItem.UserEndDate.Date < today.Date Then
                 @<section class="comment-area">
				        <h3><img src="/dist/img/challenge/tmpl/close-title.png" alt="" /></h3>
				        <div class="comment">

                            @Select Case Me.Model.TargetAchievedType
                            Case 1
                                @string.Format("目標達成おめでとうございます。よく頑張りましたね。毎日継続して健康管理を実行したおかげで成功したのだと思います。この成功をバネに健康生活を維持しましょう。次の目標は一年後の体重維持です。今回の三ヶ月で学んだ健康管理を習慣にして継続すれば必ず達成できるものと信じています。この減量成功ストーリーについて家族や友人、そして職場の同僚にぜひ共有して、みんなの健康維持に役立つように知らせてくれると助かります。")
                            
                            Case 2
                                @String.Format("よく頑張りましたね。目標達成にはあと一歩の惜しいところまで行きましたが、減量には成功できましたね。今回のように減量そのもを成功させ、着実に結果を出すことが大切です。何事も急がば回れです。減量ではリバウンドを防いで、ゆっくりと持続できに体脂肪を落としていくことです。これから一年後にもっと減量ができるように、今回勉強したことを継続して実行していきましょう。成功をお祈りしています。")
                            
                            Case 3
                                @String.Format("三ヶ月間のプログラムご参加、ありがとうございました。今回の期間中には減量できなくでも、勉強したことを実行することにより、少しずつでも減量していくチャレンジ精神が大切です。そのためにも三ヶ月間の生活を思い出し、減量するためにどの点を改善すべきだったか振り返ってみましょう。七転び八起き、失敗は成功のもとです。いや、健康増進には失敗も成功もありません。今回参加したことをバネにすることです。さらなる挑戦を期待しています。")
                            
                            End Select

				        </div>
                        @If (Me.Model.ChallengeItem.StatusType And 4) <> 4 Then
                                @<a class="block" href="https://joto-naha.com/?key=@externalid">
				            @*@<a class="block" href="../portal/ChallengeCompleted?key=@externalid">*@
					            <img class="w-max" src="/dist/img/challenge/tmpl/questionnaire-btn.png">
				            </a>
                        End If
			        </section>
            End If
                
			       

                                <!--10月1日まで表示-->
                    @If Me.Model.ChallengeItem.Challengekey = dockDispKey AndAlso today < New Date(2021, 10, 16) Then
                    @*    @<div class="wrap center">
					        <p  style="font-size:1.5em;color:rgb(51, 51, 51);">未登録の方は<p>

                            <a class="mb10 block" href="native:/action/open_browser?url=https%3A%2F%2Fwww.smartkensa.com%2Fgp%2Fentry%2F459">
					            <img class="w-max" src="/dist/img/challenge/tmpl/dock_entry.png">
                            </a>
			            </div>
                
                        @<div class="wrap">
                            <a class="mb10 block" href="native:/action/open_browser?url=https%3A%2F%2Fwww.smartkensa.com%2Flogin%0D%0A">
					            <img class="w-max" src="/dist/img/challenge/tmpl/dock_mypage.png">
				            </a>
			            </div>
                
                        *@
                        @<div class="wrap">
					        <p  style="font-size:1.5em;color:rgb(51, 51, 51);">未登録の方は<p>

                            <a class="mb10 block" href="native:/action/open_browser?url=https%3A%2F%2Fwww.smartkensa.com%2Fgp%2Fentry%2F459">
					            <img class="w-max" src="/dist/img/challenge/tmpl/dock_entry2.png">
                            </a>
			            </div>
                
                    End If
                        <div class="wrap">
                            <a class="mb10 block" href="native:/action/open_browser?url=https%3A%2F%2Fwww.smartkensa.com%2Flogin">
					            <img class="w-max" src="/dist/img/challenge/tmpl/dock_mypage2.png">
				            </a>
			            </div>
@*                
                        <div class="wrap">
					        <p  style="font-size:1.5em;color:rgb(51, 51, 51);">２回目の検査はこちら<p>

                            <a class="mb10 block" href="native:/action/open_browser?url=https%3A%2F%2Fwww.smartkensa.com%2Fgp%2Fentry%2F492">
					            <img class="w-max" src="/dist/img/challenge/tmpl/dock_entry2.png">
                            </a>
			            </div>*@
            
                    @If today > Me.Model.ChallengeItem.UserStartDate Then
                 
                         @<div class="wrap">
                            <a class="mb10 block" href="../Portal/challengecolumn?key=@key">
				                <img class="w-max" src="/dist/img/challenge/tmpl/btn-4.jpg">
			                </a>
			            </div>
                    End If
