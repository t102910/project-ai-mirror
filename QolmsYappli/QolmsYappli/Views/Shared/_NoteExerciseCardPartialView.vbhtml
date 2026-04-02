@Imports MGF.QOLMS.QolmsYappli
@ModelType NoteexerciseViewModel

	    <section class="data-area">
            @*<a href="../Note/GulfSportsMovieIndex" class="block mb20 img-max-cont"><img src="../dist/img/sports/gulf-btn.png" alt="" class="w-max"></a>*@
		    <h3 class="title mt10">最近の運動</h3>

            @If Me.Model.ExerciseItemN.Count > 0 Then
                For Each item As ExerciseItem In Me.Model.ExerciseItemN
                    @<article>
			    <section class="inner">
				    <p class="photo-area">
                        @If Not String.IsNullOrWhiteSpace(item.PhotoKey) Then
                        @<img class="photo" src="@item.PhotoKey">
                        Else
                         @<img class="photo" src="../dist/img/tmpl/no-image.png">
                        End If
					    
				    </p>
				    <div class="info">
                        @code
                        Dim ci As New System.Globalization.CultureInfo("en-US")
                        End Code

					    <p><span class="date">@item.ExerciseDate.ToString("M月d日(ddd)", System.Globalization.CultureInfo.CurrentCulture) @QyHtmlHelper.ToMeridiemTimeString(item.ExerciseDate)</span></p>
					    <p class="bold">@item.ExerciseName</p>
					    <span class="cal">@String.Format("{0:###,##0}", item.Calorie)<i>kcal</i></span>
				    </div>
                    <a href="javascript:void(0);" class="remove" data-recorddate="@item.ExerciseDate" data-exercisetype="@item.ExerciseType" data-seq="@item.Sequence" data-calorie="@item.Calorie"><i class="la la-remove"></i></a>

				    @*<a href="" class="remove" onclick="return confirm('消去してよろしいですか？')"><i class="la la-remove"></i></a>*@


			    </section>
		    </article>
                Next
            End If

	    </section>