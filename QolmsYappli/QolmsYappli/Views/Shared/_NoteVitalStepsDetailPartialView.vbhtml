@Imports MGF.QOLMS.QolmsApiCoreV1
@Imports MGF.QOLMS.QolmsYappli
@ModelType VitalStepsDetailPartialViewModel

@Code
    Dim rowNo As Integer = 0
End Code

<div id="val-modal">
	<h3><span class="day">@Me.Model.RecordDate.ToString("yyyy年M月d日")</span>の歩数</h3>
	<table class="table normal-table" data-vital-type="@QyVitalTypeEnum.Steps.ToString()">
		<thead>
			<tr>
				<td>数値</td>
				<td>削除</td>
			</tr>
		</thead>
		<tbody>
            @For Each item As VitalValueItem In Me.Model.ItemList
                @<tr data-row-no="@rowNo.ToString()" data-reference="@item.RecordDate.ToApiDateString()">
                    <td>@String.Format("{0:#,###.####} 歩", item.Value1)</td>
                    <td><span class="num-remover"><i class="la la-remove"></i></span></td>
                </tr>
                @Code
                    rowNo += 1
                End Code
            Next
		</tbody>
	</table>
	<i class="la la-close" id="val-modal-close"></i>
</div>
