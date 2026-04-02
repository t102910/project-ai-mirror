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
                    <p class="detail-data hide" data-recorddate="@item.MealDate" data-mealtype="@item.MealType" data-seq="@item.Sequence" data-photokey="@item.PhotoKey"></p>
			    </section>
		    </article>
        Next
    End If
</div>
