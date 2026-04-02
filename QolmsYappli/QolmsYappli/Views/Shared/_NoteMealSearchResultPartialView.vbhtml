@Imports MGF.QOLMS.QolmsYappli
@ModelType NoteMealViewModel
<div id="sr">

 @If Me.Model.SearchedMaxPage = 0 Then
                    @<h3 class="mb10 left">
					検索結果（0件）
				</h3>
                    
                ElseIf Me.Model.SearchedMaxPage > 0 Then
                    @<h3 class="mb10 left">
					検索結果（全 @Me.Model.SearchedMaxPage ページ）
				</h3>
                Else
                    
                    @<h3 class="mb10 left">
					検索結果
				</h3>
                End If

    @If Me.Model.SearchedMealItemN.Count > 0 Then
        
        @<div id="search-slider">
            
            @For i As Integer = 0 To Me.Model.SearchedMealItemN.Count - 1
            @<div class="item">
                @For j As Integer = 0 To Me.Model.SearchedMealItemN.Item(i).Count - 1
                Dim item As FoodItem = Me.Model.SearchedMealItemN.Item(i).Item(j)

                @<p class="meal" data-cal="@item.calorie" data-pal="@item.protein,@item.lipid,@item.carbohydrate,@item.salt_amount,@item.available_carbohydrate,@item.fiber"><span>@item.label</span></p>
           
                 @*<p class="meal" data-cal="@item.calorie" data-protein="@item.protein" data-lipid="@item.lipid" data-carbohydrate="@item.carbohydrate" data-salt_amount="@item.salt_amount" data-available_carbohydrate="@item.available_carbohydrate" data-fiber="@item.fiber"><span>@item.label</span></p>*@
           
                Next
            </div>
                    
            Next
            
            </div>


            @*@For i As Integer = 0 To Me.Model.SearchedMealItemN.Count - 1
            
            
            If i Mod 10 = 0 Then
                @<div class="item">
                    @For j As Integer = i To i + 10
                    @<p class="meal" data-cal="@Me.Model.SearchedMealItemN.item(i).Calorie"><span>@Me.Model.SearchedMealItemN.Item(i).MealName</span></p>
                    If j Mod 9 = 0 Or j >= Me.Model.SearchedMealItemN.Count - 1 Then
                        Exit For 
                    End If
                    Next
                </div>
                    End If
                
            Next
*@
    ElseIf Me.Model.SearchedMaxPage = 0 Then
       
         @<div class="hide search-slider"></div>
        
    Else
        
                @<div class="hide search-slider"></div>

                End If        
    
    </div>   