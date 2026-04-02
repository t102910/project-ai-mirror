@Imports System.Collections.ObjectModel
@Imports MGF.QOLMS.QolmsYappli
@ModelType NoteExaminationResultPartialViewModel

   <div id="filter">
		    <h3 id="anc-1" class="title mt10 mb10">
			    <span>結果を絞り込む</span>
		    </h3>
		    <section class="contents-block no-padding">
			    <ul id="abnormal-only-filter" class="form-list">
				    <li>
					    <label><input type="checkbox"> 異常値のみ表示</label>
				    </li>
			    </ul>
                
                @code
                    Dim group As New Dictionary(Of String, String)()
                    For Each item As ExaminationGroupItem In Me.Model.PageViewModel.ExaminationGroupN
                        
                        If Not group.ContainsKey(item.GroupNo) Then
                            group.Add(item.GroupNo, item.Name)
                        End If
                        
                    Next
                End Code
			    <h3 class="line type-3 @IIf(group.Count <= 1, "hide", String.Empty)">検査種別で絞り込み</h3>
			    <ul  id="group-filter" class="form-list @IIf(group.Count <= 1, "hide", String.Empty)">
                    @For Each item As KeyValuePair(Of String, String) In group

				        @<li>
					        <label><input type="checkbox" value="@item.Key"> @item.Value</label>
				        </li>
                    Next
			    </ul>

                @code
                    Dim facilitis As New Dictionary(Of String, String)()
                    For Each item As ExaminationSetItem In Me.Model.PageViewModel.ExaminationSetN
                        
                        If Not facilitis.ContainsKey(item.OrganizationKey) Then
                            facilitis.Add(item.OrganizationKey, item.OrganizationName)
                        End If
                        
                    Next
                End Code
			    <h3 class="line type-3 @IIf(facilitis.Count <= 1, "hide", String.Empty)">施設で絞り込み</h3>
			    <ul id="facility-filter" class="form-list @IIf(facilitis.Count <= 1, "hide", String.Empty)">

                    @For Each item As KeyValuePair(Of String, String) In facilitis
                        
                         @<li>
					        <label><input type="checkbox" value="@item.Key" data-key="@item.Key"> @item.Value</label>
				        </li>
                    Next

			    </ul>

		    </section>

    </div>