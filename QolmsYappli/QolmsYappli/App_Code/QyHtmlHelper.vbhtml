@Imports MGF.QOLMS.QolmsYappli
@Imports System.Web.Optimization

@Helper RenderStyleTag(virtualPath As String)
    
    Dim urlList As New List(Of String)()

    If BundleTable.EnableOptimizations Then
        urlList.Add(Styles.Url(virtualPath).ToHtmlString())
    Else
        urlList.AddRange(New BundleResolver(BundleTable.Bundles).GetBundleContents(virtualPath).ToList().ConvertAll(Function(i) Styles.Url(i).ToHtmlString()))
    End If

    For Each url As String In urlList
        If Not String.IsNullOrWhiteSpace(url) Then
            If url.StartsWith("/QolmsYappli/", StringComparison.CurrentCultureIgnoreCase) Then
                url = url.Remove(0, 12)
            End If
       
            @Styles.RenderFormat("<link rel=""stylesheet"" href=""..{0}"" type=""text/css"">", url)
        End If
    Next

End Helper

@Helper RenderScriptTag(virtualPath As String)

    Dim urlList As New List(Of String)()

    If BundleTable.EnableOptimizations Then
        urlList.Add(Scripts.Url(virtualPath).ToHtmlString())
    Else
        urlList.AddRange(New BundleResolver(BundleTable.Bundles).GetBundleContents(virtualPath).ToList().ConvertAll(Function(i) Scripts.Url(i).ToHtmlString()))
    End If

    For Each url As String In urlList
        If Not String.IsNullOrWhiteSpace(url) Then
            If url.StartsWith("/QolmsYappli/", StringComparison.CurrentCultureIgnoreCase) Then
                url = url.Remove(0, 12)
            End If
        
            @Scripts.RenderFormat("<script type=""text/javascript"" src=""..{0}""></script>", url)
        End If
    Next

End Helper

@Helper CrLfToSpanTag(s As String)

    Dim items As List(Of String) = s.Replace(vbCrLf, vbLf).Replace(vbCr, vbLf).Split(vbLf).ToList()
    
    If items IsNot Nothing Then
        If items.Count > 0 Then
            For Each item As String In items
                @<span>@item</span>
            Next
        Else
            @<span>@s</span>
        End If
    End If

End Helper

@Helper CrLfToBreakTag(s As String)

    Dim items As List(Of String) = s.Replace(vbCrLf, vbLf).Replace(vbCr, vbLf).Split(vbLf).ToList()
    
    If items IsNot Nothing Then
        If items.Count > 0 Then
            For Each item As String In items
                @item@<br />
            Next
        Else
            @s
        End If
    End If

End Helper

@Helper ToMeridiemTimeString(value As Date)

    @String.Format("{0} {1}:{2:d2}", value.ToString("tt"), value.Hour Mod 12, value.Minute)
    
End Helper

@Helper ToMeridiemDropDownList(id As String, name As String, selected As String, Optional css As String = "form-control ap")
    
    @<select id="@id" name="@name" class="@css">
        @For Each kv As KeyValuePair(Of String, String) In QyDictionary.Meridiem
            If kv.Key.CompareTo(selected.ToLower()) = 0 Then
                @<option value="@kv.Key" selected="selected">@kv.Value</option>
            Else
                @<option value="@kv.Key">@kv.Value</option>
            End If
        Next
    </select>
    
End Helper

@Helper ToHourDropDownList(id As String, name As String, selected As String, Optional css As String = "form-control hour")

    Dim value As String = String.Empty
    
    @<select id="@id" name="@name" class="@css">
        @For a As Integer = 0 To 11
            value = a.ToString()
        
            If selected.CompareTo(value) = 0 Then
                @<option value="@value" selected="selected">@String.Format("{0}", value)</option>
            Else
                @<option value="@value">@String.Format("{0}", value)</option>
            End If
        Next
    </select>
    @<span class="input-group-addon hour2">時</span>
    
End Helper

@Helper ToMinuteDropDownList(id As String, name As String, selected As String, Optional css As String = "form-control minute")

Dim hash As New HashSet(Of String)()
    
For a As Integer = 0 To 59
    hash.Add(a.ToString())
Next
    
    @<select id="@id" name="@name" class="@css">
        @For Each s As String In hash
            If s.CompareTo(selected) = 0 Then
                @<option value="@s" selected="selected">@String.Format("{0}", s)</option>
            Else
                @<option value="@s">@String.Format("{0}", s)</option>
            End If
        Next
    </select>
    @<span class="input-group-addon minute2">分</span>
    
End Helper

@Helper ToMealTypeDropDownList(id As String, name As String, selected As String, Optional css As String = "form-control")

    @*バグあり、廃止予定*@
    @<div id="@id" name="@name" class="@css">
        @For Each kv As KeyValuePair(Of QyMealTypeEnum, String) In QyDictionary.MealType
            If kv.Key.ToString.CompareTo(selected) = 0 Then
                @<p>
				    <input type="radio" name="@name" id="@id-@kv.Key" value="@kv.Key" checked="checked">
				    <label class="button-style-label" for="@id-@kv.Key">@kv.Value</label>
			    </p>
            Else
                @<p>
				    <input type="radio" name="@name" id="@id-@kv.Key" value="@kv.Key" checked="">
				    <label class="button-style-label" for="@id-@kv.Key">@kv.Value</label>
			    </p>
            End If
        Next
    </div>                                                                                                                               
                                                                                                                                                 
End Helper

@Helper ToMealTypeRadioButton(id As String, name As String, selected As String, Optional css As String = "form-control")

    @<div id="@id" name="@name" class="@css">
        @For Each kv As KeyValuePair(Of QyMealTypeEnum, String) In QyDictionary.MealType
            If kv.Key.ToString.CompareTo(selected) = 0 Then
                @<p>
				    <input type="radio" name="@name" id="@id-@kv.Key" value="@kv.Key" checked="checked">
				    <label class="button-style-label" for="@id-@kv.Key">@kv.Value</label>
			    </p>
            Else
                @<p>
				    <input type="radio" name="@name" id="@id-@kv.Key" value="@kv.Key">
				    <label class="button-style-label" for="@id-@kv.Key">@kv.Value</label>
			    </p>
            End If
        Next
    </div>                                                                                                                               
                                                                                                                                                 
End Helper

@Helper ToVitalConditionDropDownList(id As String, name As String, selected As String, Optional css As String = "form-control val")

    @<select id="@id" name="@name" class="@css">
        @For Each kv As KeyValuePair(Of QyVitalConditionTypeEnum, String) In QyDictionary.VitalConditionType
            If kv.Key.ToString.CompareTo(selected) = 0 Then
                @<option value="@kv.Key" selected="selected">@kv.Value</option>
            Else
                @<option value="@kv.Key">@kv.Value</option>
            End If
        Next
    </select>                                                                                                                               
                                                                                                                                                 
End Helper

@Helper ToValidationMessageArea(keys As IEnumerable(Of String), errorMessage As Object, Optional css As String = "block alert alert-danger thin mt10", Optional useP As Boolean = False)

Dim messages As New List(Of String)()
    
If keys IsNot Nothing AndAlso keys.Count() > 0 AndAlso errorMessage IsNot Nothing AndAlso TypeOf errorMessage Is Dictionary(Of String, String) Then
With DirectCast(errorMessage, Dictionary(Of String, String))
    For Each key As String In keys
        If .ContainsKey(key) AndAlso Not String.IsNullOrWhiteSpace(.Item(key)) Then
            messages.Add(.Item(key))
        End If
    Next
End With
End If

If messages.Count > 0 Then
If Not useP Then
            @<span class = "@css" >@QyHtmlHelper.CrLfToBreakTag(String.Join(Environment.NewLine, messages))</span>
Else
            @<p class = "@css" >@QyHtmlHelper.CrLfToBreakTag(String.Join(Environment.NewLine, messages))</p>
End If
Else
If Not useP Then
            @<span class="@css hide"></span>
Else
            @<p class="@css hide"></p>
End If
End If
    
End Helper


@Helper ToSexDropDownList(id As String, name As String, selected As String, Optional css As String = "form-control")
    
    @<select id="@id" name="@name"class="@css" required="required">
        <option value="">性別</option>
        @For Each kv As KeyValuePair(Of QySexTypeEnum, String) In QyDictionary.SexType
            If kv.Key.ToString.CompareTo(selected) = 0 Then
                @<option value="@kv.Key" selected="selected">@kv.Value</option>
            Else
                @<option value="@kv.Key">@kv.Value</option>
            End If
        Next
    </select>
    
End Helper

@Helper ToYearDropDownList(id As String, name As String, selected As String, Optional css As String = "form-control")

    Dim value As String = String.Empty
    
    @<select id="@id" name="@name" class="@css" required="required">
        <option value="">西暦（和暦）年</option>
        @For Each kv As KeyValuePair(Of String, String) In QyDictionary.Year

            value = kv.Key.ToString()
            
            If selected.CompareTo(value) = 0 Then
                @<option value="@kv.Key" selected="selected">@kv.Value</option>
            Else
                @<option value="@kv.Key">@kv.Value</option>
            End If
        Next
    </select>
    
End Helper

@Helper ToMonthDropDownList(id As String, name As String, selected As String, Optional css As String = "form-control")

    Dim value As String = String.Empty
    
    @<select id="@id" name="@name" class="@css" required="required">
        <option value="">月</option>
        @For a As Integer = 1 To 12
            value = a.ToString()
        
            If selected.CompareTo(value) = 0 Then
                @<option value="@value" selected="selected">@String.Format("{0}月", value)</option>
            Else
                @<option value="@value">@String.Format("{0}月", value)</option>
            End If
        Next
    </select>
    
End Helper

@Helper ToDayDropDownList(id As String, name As String, selected As String, Optional css As String = "form-control")
    
    Dim value As String = String.Empty
    
    @<select id="@id" name="@name" class="@css" required="required">
        <option value="">日</option>
        @For a As Integer = 1 To 31
            value = a.ToString()
        
            If selected.CompareTo(value) = 0 Then
                @<option value="@value" selected="selected">@String.Format("{0}日", value)</option>
            Else
                @<option value="@value">@String.Format("{0}日", value)</option>
            End If
        Next
    </select>
    
End Helper

@Helper Pager(index As Integer, count As Integer, Optional pagerSize As Integer = 5)
    
                        If pagerSize < 1 Then Return
    
                        Dim newIndex As Integer = index
                        Dim newCount As Integer = count
    
                        If count < 1 Then newCount = 1
    
                        If newIndex < 0 Then newIndex = 0
                        If newIndex >= newCount Then newIndex = newCount - 1
    
                        Dim minIndex As Integer = (newIndex \ 5) * pagerSize
                        Dim maxIndex As Integer = If(minIndex + pagerSize > newCount, newCount - 1, minIndex + pagerSize - 1)
    
      @<nav class="center" aria-label="Page navigation">
				<ul class="pagination">
                     @If minIndex = 0 Then
               @<li class="page-item disabled">
						<a class="page-link disabled" href="javascript:void(0);" aria-label="Previous">
							<span aria-hidden="false">&laquo;</span>
							<span class="sr-only">Previous</span>
						</a>
					</li>
                     Else
               @<li class="page-item">
						<a class="page-link" href="javascript:void(0);" aria-label="Previous" data-page-index="@(minIndex - 1)">
							<span aria-hidden="true">&laquo;</span>
							<span class="sr-only">Previous</span>
						</a>
					</li>                
                     End If
					@For a As Integer = minIndex To maxIndex
                    If a = newIndex Then
					@<li class="page-item active"><a class="page-link disabled" href="javascript:void(0);" data-page-index="@a">@(a + 1)</a></li>
                    Else
                    @<li class="page-item"><a class="page-link" href="javascript:void(0);" data-page-index="@a">@(a + 1)</a></li>
                  
                    End If
                    Next
					
                    @If maxIndex = newCount - 1 Then
                 @<li class="page-item disabled">
						<a class="page-link disabled" href="javascript:void(0);" aria-label="Next">
							<span aria-hidden="true">&raquo;</span>
							<span class="sr-only">Next</span>
						</a>
					</li>
                    Else
      
                         @<li class="page-item">
						<a class="page-link" href="javascript:void(0);" aria-label="Next" data-page-index="@(maxIndex + 1)">
							<span aria-hidden="false">&raquo;</span>
							<span class="sr-only">Next</span>
						</a>
					</li>
                        
                    End If

					
				</ul>
			</nav>
    
End Helper

@Helper ToValidationMessageInSectionTag(keys As IEnumerable(Of String), errorMessage As Object, Optional css As String = "section caution mt10 mb0")

    Dim messages As New List(Of String)()
    
    If keys IsNot Nothing AndAlso keys.Count() > 0 AndAlso errorMessage IsNot Nothing AndAlso TypeOf errorMessage Is Dictionary(Of String, String) Then
        With DirectCast(errorMessage, Dictionary(Of String, String))
            For Each key As String In keys
                If .ContainsKey(key) AndAlso Not String.IsNullOrWhiteSpace(.Item(key)) Then
                    messages.Add(.Item(key))
                End If
            Next
        End With
    End If

    If messages.Count > 0 Then
        @<section class="@css">@QyHtmlHelper.CrLfToBreakTag(String.Join(Environment.NewLine, messages))</section>
    Else
        @<section class="@css hide"></section>
    End If
    
End Helper

@Helper ToUrineDropDownList(id As String, name As String, selected As String, Optional css As String = "form-control")

    Dim dic As New Dictionary(Of String, String)() From {
        {"0", "選択してください"},
        {"1", "－"},
        {"2", "±"},
        {"3", "＋"},
        {"4", "＋＋"},
        {"5", "＋＋＋"}
    }
    
    @<select id="@id" name="@name" class="@css">
        @For Each kv As KeyValuePair(Of String, String) In dic
            If selected.CompareTo(kv.Key) = 0 Then
                @<option value="@kv.Key" selected="selected">@kv.Value</option>
            Else
                @<option value="@kv.Key">@kv.Value</option>
            End If
        Next
    </select>
    
End Helper
