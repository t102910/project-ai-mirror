@Imports MGF.QOLMS.QolmsYappli
@ModelType NoteMealAnalyzeResultPartialViewModel

<div class="analyze-result-area">
    @If Me.Model.PageViewModel.FoodN.Any() AndAlso 1 < Me.Model.PageViewModel.FoodN(0).Count Then
        @<div class="item search">
            @For i As Integer = 0 To Me.Model.PageViewModel.FoodN.Item(Me.Model.PageViewModel.DispedPage - 1).Count - 1
                Dim item As FoodItem = Me.Model.PageViewModel.FoodN.Item(Me.Model.PageViewModel.DispedPage - 1).Item(i)
                Dim setSpan As String = item.label + "（" + item.calorie + "kcal）"
                Dim palString As String = item.label _
                    + "," + item.calorie _
                    + "," + item.protein _
                    + "," + item.lipid _
                    + "," + item.carbohydrate _
                    + "," + item.salt_amount _
                    + "," + item.available_carbohydrate _
                    + "," + item.fiber + ","
                @<a href="javascript:void(0);" class="meal" data-name="@item.label" data-cal="@item.calorie" data-pal="@palString"><span>@setSpan</span></a>
            Next
        </div>
    End If

    @If Me.Model.PageViewModel.DispedPage < Me.Model.PageViewModel.SearchedMaxPage Then
	    @<a class="btn btn-submit btn-showmore white block mt20 mb10" data-page="@Me.Model.PageViewModel.DispedPage">続きを表示</a>
    ElseIf Me.Model.PageViewModel.SearchedMaxPage > 1 Then
        @<p>これ以上の解析結果はありません</p>
    End If
</div>