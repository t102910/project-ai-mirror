@Imports MGF.QOLMS.QolmsYappli
@ModelType NoteMealAnalyzeResultPartialViewModel

@Code
    Dim foreignKey As Guid = Guid.Empty
    Dim hasAnalysisData As Boolean = False
    If Me.Model.PageViewModel.ForeignKey.Any() Then
        foreignKey = Me.Model.PageViewModel.ForeignKey(0)
    End If
    
    If Me.Model.PageViewModel.FoodN.Any() AndAlso 1 < Me.Model.PageViewModel.FoodN(0).Count Then
        hasAnalysisData = True
    End If

End Code

<div class="analyze-result-area">
    <div class="thumbnail-area">
        <p class="meal-photo">
            @If foreignKey = Guid.Empty Then
                @<img src="../dist/img/tmpl/no-image.png" data-foreignkey="@foreignKey">
            Else
                @<img src="@QyAccountItemBase.CreateThumbnailPhotoUri(foreignKey, 1, "Storage", "MealFile", QyAccountItemBase.EncryptPhotoReference(Me.Model.PageViewModel.AuthorKey, foreignKey, QyFileTypeEnum.Thumbnail))">
            End If
        </p>
    </div>

    @If hasAnalysisData Then
        @<h4 class="line center">解析結果</h4>
    ElseIf Me.Model.PageType <> "Edit" AndAlso hasAnalysisData = False Then
        @<h4 class="center">解析できませんでした</h4>
    End If

    @Html.Partial("_NoteMealAnalyzeResultAreaPartialView")

</div>
