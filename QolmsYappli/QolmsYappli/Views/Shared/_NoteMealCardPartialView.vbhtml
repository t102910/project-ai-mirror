@Imports MGF.QOLMS.QolmsYappli
@ModelType NoteMealViewModel

<div id="update-data-area">
    @If Me.Model.MealItemN.Count > 0 Then
        For Each item As MealItem In Me.Model.MealItemN
            @<article>
                <section class="inner">
				    <p class="photo-area">
                        @If item.PhotoKey = Guid.Empty Then
                            @<img src="../dist/img/tmpl/ajax-loader2.gif" class="photo inview" data-reference="../dist/img/tmpl/no-image.png" />
                        Else
                            @<img src="../dist/img/tmpl/ajax-loader2.gif" class="photo inview" data-reference="@QyAccountItemBase.CreateThumbnailPhotoUri(item.PhotoKey, 1, "Storage", "MealFile", QyAccountItemBase.EncryptPhotoReference(Me.Model.AuthorKey, item.PhotoKey, QyFileTypeEnum.Thumbnail))" />
                        End If
                            
                        @*<img class="photo" src="../Linkage/PatientCardFile?Reference=@item.ToEncryptedFileStorageReference(QyFileTypeEnum.Thumbnail)" id="fileImg" data-img="../Linkage/PatientCardFile?Reference=@item.ToEncryptedFileStorageReference(QhFileTypeEnum.Edited)" class="pointer" data-toggle="tooltip" data-placement="top" title="@item.OriginalName" />*@
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
                        Dim r1 As String = String.Empty
                        Dim r2 As String = String.Empty
                        Dim r3 As String = String.Empty
                    
                        Dim rList As New List(Of String)()
                    
                        If item.FoodN.Any AndAlso item.FoodN.Count = 3 Then
                            For i As Integer = 0 To item.FoodN.Count - 1
                                rList.Add(item.FoodN.Item(i).label.ToString + "," + item.FoodN.Item(i).calorie.ToString)
                            Next
                            r1 = rList.Item(0)
                            r2 = rList.Item(1)
                            r3 = rList.Item(2)
                        End If
                    End Code
				    <a href="javascript:void(0);" class="edit" data-recorddate="@item.MealDate" data-mealtype="@item.MealType" data-seq="@item.Sequence" data-r1="@r1" data-r2="@r2" data-r3="@r3"><i class="la la-edit"></i></a>
			    </section>
		    </article>
        Next
    End If
</div>
