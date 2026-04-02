@Imports System.Collections.ObjectModel
@Imports MGF.QOLMS.QolmsYappli
@Imports MGF.QOLMS.QolmsCryptV1
@ModelType NoteExaminationResultPartialViewModel

        @Code
            Dim matrix As ExaminationMatrix = Nothing
            Dim xAxis As ReadOnlyCollection(Of ExaminationAxis) = Nothing
            Dim yAxis As ReadOnlyCollection(Of ExaminationAxis) = Nothing
            Dim element As ExaminationItem = Nothing
    
            With Me.Model.PageViewModel

                If .MatrixN.Any Then
                    matrix = .MatrixN.First.Copy()
                
                    If .NarrowInCategory <> Byte.MinValue Then
                        matrix = matrix.NarrowInCategory(.NarrowInCategory)
                    End If
        
                    If .NarrowInAbnormal Then
                        matrix = matrix.NarrowInAbnormal()
                    End If
        
                    If matrix.ColCount > 0 AndAlso matrix.RowCount > 0 Then
                        xAxis = matrix.XAxis
                        yAxis = matrix.YAxis
                    End If
                End If
        
            End With
        End Code

<div class="update-area">
    @If matrix IsNot Nothing AndAlso xAxis IsNot Nothing AndAlso yAxis IsNot Nothing Then
        
        
     @* 健診結果（総合判定）*@
    @<div class="kenshin-result" style="opacity: 0;">
            
        @Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsWeb)
            
                

            For x As Integer = 0 To matrix.ColCount - 1
                        
                Dim dateStr As String = String.Empty
                Dim dateStrMD As String = String.Empty
                Dim dateStrN As String() = Split(xAxis(x).Header2, "/")
                dateStr = String.Format("{0}年{1}月{2}日", dateStrN(0), dateStrN(1), dateStrN(2))
                
                Dim judg As String = String.Empty
                Dim judgContent As String = String.Empty
                Dim colorClass As String = String.Empty
                If xAxis(x).ExaminationJudgementN.Any() AndAlso xAxis(x).ExaminationJudgementN.ContainsKey("総合所見") Then
                                                
                    judg = xAxis(x).ExaminationJudgementN("総合所見").Judgment1
                    judgContent = xAxis(x).ExaminationJudgementN("総合所見").Value
                    
                    Dim ab As String = "AB"
                    Dim c As String = "C"
                    If ab.Contains(judg.Substring(0)) Then
                        colorClass = "type-AB"

                    ElseIf c.Contains(judg.Substring(0)) Then
                        colorClass = "type-C"

                    Else
                        colorClass = "type-D"
                        
                    End If

                End If

			@<div class="item">
				<article class="card">
					<table class="table table-bordered mb0">
						<tr>
							<th>受診機関</th>
							<td>@xAxis(x).Header1</td>
						</tr>
						<tr>
							<th>受診日</th>
							<td>@dateStr</td>
						</tr>
						<tr>
							<th>総合判定<a id="result-description" href="#"><i class="la la-question-circle"></i></a></th>
							<td><span class="@colorClass">@judg</span></td>
						</tr>
                        <tr>
                            <th>総合所見</th>
                            @If True Then
                                @<td class="left">@judgContent</td>

                            Else
                                @<td class="left"><a href="#" class="modal-text">@judgContent</a></td>
                            End If

                        </tr>
		                    <tr>
                            <th>添付ファイル</th>
                                @If xAxis(x).AssociatedFileN.Any() Then

                                For Each item As AssociatedFileItem In xAxis(x).AssociatedFileN
                                 
                                If item.DataType = QyExaminationDataTypeEnum.OverallAssessmentPdf _
                                AndAlso item.DataKey <> Guid.Empty AndAlso item.FacilityKey <> Guid.Empty _
                                AndAlso item.LinkageSystemNo > 0 AndAlso Not String.IsNullOrWhiteSpace(item.LinkageSystemId) _
                                AndAlso Not String.IsNullOrWhiteSpace(item.FileStorageReferenceJson) Then
                                 
                                @<td class="left"><a href="javascript:void(0);" class="associated-file" data-reference="@crypt.EncryptString(item.FileStorageReferenceJson)">PDFファイル</a></td>
                                End If
                                 
                                Next
                                End If

                        </tr>
                    </table>
				</article>
			</div>
                Next
                

        End Using
			
	</div>
	@<h3 class="title mt10 mb10 two-pane">
		<span>健診結果一覧</span>
		<span class="last-child"><a href="#anc-1" class="result-view"><i class="la la-stethoscope la-2x"></i>絞り込み</a></span>
	</h3>
	@<section class="contents-block mb30">
		<div class="fixed-table">
            @* テーブルのデータ*@
			<div class="scroll-wrapper table-responsive">

              @If matrix IsNot Nothing AndAlso xAxis IsNot Nothing AndAlso yAxis IsNot Nothing Then
                  
          
				@<table class="table table-bordered table-striped kenshin-result-table">
					<thead>
						<tr>
							<th></th>
							<td class="unit"></td>
		                    @For x As Integer = 0 To matrix.ColCount - 1
                                @<td class="@String.Format("td-{0}", x + 1)"><span>@xAxis(x).Header1</span></td>
		  
		                    Next
						</tr>
						<tr>
							<th></th>
							<td class="unit"></td>
                                @For x As Integer = 0 To matrix.ColCount - 1
                                    Dim dateStr As String = String.Empty
                                    Dim dateStrMD As String = String.Empty
                                    Dim dateStrN As String() = Split(xAxis(x).Header2, "/")
                                    dateStr = dateStrN(0)
                                    dateStrMD = dateStrN(1) & "/" + dateStrN(2)
                                    @<td class="@String.Format("td-{0}", x + 1)" >@dateStr<br>@dateStrMD</td>
                                Next
						</tr>
					</thead>
					<tbody>

                        @code
                            Dim headerCount As Integer = 0
                        End Code
                        @For y As Integer = 0 To matrix.RowCount - 1
                               
                            Dim lastStr As String = IIf(y = matrix.RowCount - 1 OrElse yAxis(y + 1).Header2 = "groupRow", "last-child", String.Empty)
                                   
                            Dim unitStr As String = String.Empty
                            Dim valueStr As String = String.Empty
                                   
                            If Not String.IsNullOrWhiteSpace(yAxis(y).HeaderStandardValue) Then
                                valueStr = yAxis(y).HeaderStandardValue
                            End If
                                   
                            If Not String.IsNullOrWhiteSpace(yAxis(y).HeaderUnit) Then
                                unitStr = yAxis(y).HeaderUnit
                            End If
                                    
							@<tr class="@IIf(yAxis(y).Header2 = "groupRow", "header", String.Format("header-{0} t-body " + lastStr, headerCount))">
                                <!-- ヘッダーの一番最後に.last-childをつけてください -->
                                        
                                @If yAxis(y).Header2 = "groupRow" Then
                                    headerCount += 1
                                    @<th colspan="2" data-name="@String.Format("header-{0}", headerCount)"></th>
                                Else
                                    @<th><span class="reader-dot"></span></th>
                                        
                                    @<td class="unit"><span><br></span></td>
                                End If
                                        
                                @For x As Integer = 0 To matrix.ColCount - 1
                                    If yAxis(y).Header2 = "groupRow" Then
                                    Dim judg As String = String.Empty
                                    Dim colorClass As String = String.Empty

                                    If xAxis(x).ExaminationJudgementN.Any() AndAlso xAxis(x).ExaminationJudgementN.ContainsKey(yAxis(y).Header1) Then
                                                
                                        judg = xAxis(x).ExaminationJudgementN(yAxis(y).Header1).Judgment1
          
                                        Dim ab As String = "AB"
                                        Dim c As String = "C"
                                        If ab.Contains(judg.Substring(0)) Then
                                            colorClass = "type-AB"

                                        ElseIf c.Contains(judg.Substring(0)) Then
                                            colorClass = "type-C"

                                        Else
                                            colorClass = "type-D"
                        
                                        End If
                                    End If

								        @<td class="@String.Format("td-{0}", x + 1)"><i>
                                            @If Not String.IsNullOrWhiteSpace(colorClass) Then
                                                @<span class="@colorClass">@judg</span>
                                            End If
                                            </i></td>
                                    Else
                                    element = matrix.GetItem(x, y)
                                           
                                    If element.ValueType = QyExaminationItemValueTypeEnum.PQ Then
								            @<td class="@String.Format("td-{0}", x + 1)">
                                                <span class="value-modal" data-title="@yAxis(y).Header1" data-unit="@element.Unit" data-standard="@element.ReferenceDisplayName">

                                                    @If element.IsHigher Then
                                                        @<i class="high high-low-ico">H</i>
                                                    ElseIf element.IsLower Then
                                                        @<i class="low high-low-ico">L</i>
                                                    End If

                                                    @element.Value.ToString()

								                </span></td>
                                        Else
								            @<td class="@String.Format("td-{0}", x + 1)">
                                                @If element.Value.Length > 5 Then

                                                        
                                                    @<span class="modal-text" data-title="@yAxis(y).Header1" data-standard="@element.ReferenceDisplayName">@element.Value</span>
                                                Else
                                                    @<span>@element.Value</span>
                                                End If
                                                                
								                </td>
                                        End If
                                        
                                    End If
                                Next
							</tr>
                        Next

					</tbody>
				</table>
              End If
                  
			</div>
            @* テーブルのヘッダ *@
			<div class="th-wrapper">
              @If matrix IsNot Nothing AndAlso xAxis IsNot Nothing AndAlso yAxis IsNot Nothing Then
                  

				@<table class="table table-bordered table-striped">
					<thead>
						<tr>
							<th><span>施設名</span></th>
							<td class="unit"></td>
						</tr>
						<tr>
							<th>検査日<br>　</th>
							<td class="unit"><i class="open-closer open"></i></td>
						</tr>
					</thead>
                    <tbody>

                        @code
                            Dim headerCount As Integer = 0
                        End Code

                        @For y As Integer = 0 To matrix.RowCount - 1
                            Dim unitStr As String = String.Empty
                            Dim valueStr As String = String.Empty
                                   
                            If Not String.IsNullOrWhiteSpace(yAxis(y).HeaderStandardValue) Then
                                valueStr = yAxis(y).HeaderStandardValue
                            End If
                                   
                            If Not String.IsNullOrWhiteSpace(yAxis(y).HeaderUnit) Then
                                unitStr = yAxis(y).HeaderUnit
                            End If
                                    
                            @<tr class="@IIf(yAxis(y).Header2 = "groupRow", "header", String.Format("header-{0} t-body", headerCount))">

                            @If yAxis(y).Header2 = "groupRow" Then
                                headerCount += 1
                                @<th colspan="2" data-name="@String.Format("header-{0}", headerCount)">@yAxis(y).Header1</th>
                            Else
                          
                            element = matrix.GetItem(0, y)
                            
                                @<th class="title"  data-unit="@unitStr" data-standard="@valueStr" data-comment="@yAxis(y).Comment"><span class="reader-dot">@yAxis(y).Header1</span></th>
                                @<td class="unit">
                                    @*  基準値、単位の表示はJS側で必ず上書きされているのでコメントアウト
                                        <span class="@IIf(yAxis(y).HasDifferentUnit, "cation", "").ToString">
                                        @valueStr
                                        
                                        @If not String.IsNullOrWhiteSpace(valueStr) andalso Not String.IsNullOrWhiteSpace(unitStr) Then
                                            @<br>
                                        End If

                                        @IIf(Not String.IsNullOrWhiteSpace(unitStr), String.Format("({0})", unitStr), String.Empty)
                                    </span>*@
                                 </td>
                                        
                            End If
                                </tr>
                        Next

					</tbody>
				</table>
              End If
                  
			</div>
		</div>
	</section>
        
    Else

            @<div class="center mt20 mb100">
                <img src="../dist/img/tmpl/error.png" class="w200 mb10">
                <div>結果がありません</div>
                
            </div>
    End If
</div>