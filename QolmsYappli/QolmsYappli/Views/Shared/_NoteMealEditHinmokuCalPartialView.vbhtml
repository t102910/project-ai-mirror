@Imports MGF.QOLMS.QolmsYappli
@ModelType NoteMealEditHinmokuCalPartialViewModel

@Code
    Dim palStr As String = String.Empty
    
    If Me.Model.PageViewModel.PalString.Any() Then
        palStr = Me.Model.PageViewModel.PalString(0)
    End If
End Code

<div class="input-hinmoku-cal">
	<div class="input-group mb10">
		<span class="input-group-addon">品目</span>
        <input id="hinmoku-2" name="hinmoku" type="text" value="@Me.Model.PageViewModel.ItemName(0)" data-pal="@palStr" class="form-control input" placeholder="品目を入力" maxlength="100" autocomplete="off">
	</div>
	<div class="input-group mb10" class="line">
		<span class="input-group-addon">カロリー</span>
        <input id="cal-2" name="cal" type="tel" value="@Me.Model.PageViewModel.Calorie(0)" class="form-control input" placeholder="カロリーを入力" maxlength="4" style="ime-mode:disabled;" autocomplete="off">
        <span class="input-group-addon">kcal</span>
	</div>
</div>
