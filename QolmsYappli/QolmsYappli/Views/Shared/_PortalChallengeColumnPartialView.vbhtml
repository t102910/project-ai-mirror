@Imports MGF.QOLMS.QolmsYappli
@Imports MGF.QOLMS.QolmsCryptV1
@ModelType PortalChallengeColumnPartialViewModel

    <div class="modal-content">
			<div class="header">
				<h4 class="modal-title">
                    @Me.Model.ChallengeColumnItem.Title
					<button type="button" class="close" data-dismiss="modal"></button>
				</h4>
				<img class="w-max inview" src="../dist/img/tmpl/ajax-loader2.gif" style="object-fit:contain;height:240px;" data-reference="@QyAccountItemBase.CreateThumbnailPhotoUri(Me.Model.ChallengeColumnItem.ImageKey, 1, "Portal", "GetColumnImage", QyAccountItemBase.EncryptPhotoReference(Me.Model.PageViewModel.AuthorKey, Me.Model.ChallengeColumnItem.ImageKey, QyFileTypeEnum.Thumbnail))"/>@*src="/dist/img/column/thumb-1-unread.jpg"*@
			</div>
			
			<div class="modal-body">
				<p>

                @Html.Raw(Me.Model.ChallengeColumnItem.Content)
                </p>
                @If Me.Model.ChallengeColumnItem.UserReadFlag = False Then
                    Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsWeb)
                    
				    @<p class="mb20">
					    <a href="#" class="" id="read" data-key="@crypt.EncryptString(Me.Model.ChallengeColumnItem.Challengekey.ToString())" data-no="@Me.Model.ChallengeColumnItem.ColumnNo">
						    <img class="w-max" src="/dist/img/column/read-btn-unread.png" >
						    <!-- <img class="w-max" src="/dist/img/column/read-btn.png"> --><!-- 既読ボタン（未読に戻すなら文言変更、戻さないならhideでいい気がします） -->
					    </a>
				    </p>
                    End Using
                End If
			</div>
		</div>


