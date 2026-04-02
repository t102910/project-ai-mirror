@Imports MGF.QOLMS.QolmsApiCoreV1
@Imports MGF.QOLMS.QolmsYappli
@ModelType NoteMealCardAreaPartialViewModel4

<div id="update-data-area">
    @If Me.Model.PageViewModel.MealItemN.Count > 0 Then
        For Each item As MealItem In Me.Model.PageViewModel.MealItemN
            @<article>
                <section class="inner">
				    <p class="photo-area">
                        @If item.PhotoKey = Guid.Empty Then
                            @<img src="../dist/img/tmpl/ajax-loader2.gif" class="photo inview" data-reference="../dist/img/tmpl/no-image.png" />
                        Else
                            @<img src="../dist/img/tmpl/ajax-loader2.gif" class="photo inview" data-reference="@QyAccountItemBase.CreateThumbnailPhotoUri(item.PhotoKey, 1, "Storage", "MealFile", QyAccountItemBase.EncryptPhotoReference(Me.Model.PageViewModel.AuthorKey, item.PhotoKey, QyFileTypeEnum.Thumbnail))" />
                        End If
                    </p>
				    <div class="info">    
					    <p>
                            <span class="date">
                                @Select Case item.MealType
                                    Case QyMealTypeEnum.Breakfast
                                        @<i class="ico breakfast">朝食</i>
                                    Case QyMealTypeEnum.Lunch
                                        @<i class="ico lunch">昼食</i>
                                    Case QyMealTypeEnum.Dinner
                                        @<i class="ico dinner">夕食</i>
                                    Case QyMealTypeEnum.Snacking
                                        @<i class="ico">間食</i>
                                End Select

                                @item.MealDate.ToString("M月d日(ddd)", System.Globalization.CultureInfo.CurrentCulture) @QyHtmlHelper.ToMeridiemTimeString(item.MealDate)
                            </span>
					    </p>

                        @If item.MealName.CompareTo("不明") = 0 And item.Calorie = 0 Then
                            @<p class="read-error"><img src="../dist/img/meal/error.png"></p>
                        Else
					        @<p class="bold">@item.MealName</p>
					        @<span class="cal">@String.Format("{0:###,##0}", item.Calorie)<i>kcal</i></span>
                        End If
				    </div>
				    <a href="javascript:void(0);" class="remove" data-recorddate="@item.MealDate" data-mealtype="@item.MealType" data-seq="@item.Sequence"><i class="la la-remove"></i></a>
                    @Code
                        Dim hour = (item.MealDate.Hour Mod 12).ToString()
                        Dim palString As String = String.Empty
                        If item.FoodN.Any AndAlso item.FoodN.Count > 1 Then
                            @For i As Integer = 0 To item.FoodN.Count - 1
                                palString += item.FoodN.Item(i).label _
                                    + "," + item.FoodN.Item(i).calorie _
                                    + "," + item.FoodN.Item(i).protein _
                                    + "," + item.FoodN.Item(i).lipid _
                                    + "," + item.FoodN.Item(i).carbohydrate _
                                    + "," + item.FoodN.Item(i).salt_amount _
                                    + "," + item.FoodN.Item(i).available_carbohydrate _
                                    + "," + item.FoodN.Item(i).fiber + ","
                            Next
                        End If
                    End Code
                    <a href="javascript:void(0);" class="edit" data-recorddate="@item.MealDate" data-mealtype="@item.MealType" data-seq="@item.Sequence"
                        data-meridiem="@item.MealDate.ToString("tt", System.Globalization.CultureInfo.InvariantCulture).ToLower()" data-hour="@hour" data-minute="@item.MealDate.Minute"
                        data-name="@item.MealName" data-cal="@item.Calorie" data-photokey="@item.PhotoKey" data-pal="@palString"><i class="la la-edit"></i></a>
			    </section>
		    </article>
        Next
    End If
</div>
