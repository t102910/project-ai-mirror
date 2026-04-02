@Imports MGF.QOLMS.QolmsYappli
@ModelType NoteMealSearchResultAreaPartialViewModel

<div class="search-result-area">
    @If Me.Model.PageViewModel.SearchedMealItemN.Count > 0 Then
	    @<div class="item search">
            @For Each item As FoodItem In Me.Model.PageViewModel.SearchedMealItemN
            Dim setSpan As String = item.label + "（" + item.calorie + "kcal）"
		    @<a href="javascript:void(0);" class="meal" data-name="@item.label" data-cal="@item.calorie" 
                data-pal="@item.protein,@item.lipid,@item.carbohydrate,@item.salt_amount,@item.available_carbohydrate,@item.fiber"><span>@setSpan</span></a>
            Next
	    </div>
    End If
    @If Me.Model.PageViewModel.DispedPage < Me.Model.PageViewModel.SearchedMaxPage Then
	    @<a class="btn btn-submit btn-continued white block mt20 mb10" data-page="@Me.Model.PageViewModel.DispedPage">続きを表示</a>
    ElseIf Me.Model.PageViewModel.SearchedMaxPage > 0 Then
        @<p>これ以上の検索結果はありません</p>
    End If
</div>
