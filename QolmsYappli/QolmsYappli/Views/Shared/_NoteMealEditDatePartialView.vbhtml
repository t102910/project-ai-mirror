@Imports MGF.QOLMS.QolmsYappli
@Imports System.Reflection
@ModelType NoteMealEditDatePartialViewModel

@Code
    Dim recordDate As Date = Date.MinValue
    Dim mealType As QyMealTypeEnum = QyMealTypeEnum.None
    Dim meridiem As String = String.Empty
    Dim hour As String = String.Empty
    Dim minute As String = String.Empty
    
    Dim properties As PropertyInfo() = Me.Model.PageViewModel.GetType().GetProperties()
    
    For Each pi As PropertyInfo In properties

        Select Case pi.Name
            Case "RecordDate"
                recordDate = DirectCast(pi.GetValue(Me.Model.PageViewModel), Date)
            Case "MealType"
                mealType = DirectCast(pi.GetValue(Me.Model.PageViewModel), QyMealTypeEnum)
            Case "Meridiem"
                meridiem = DirectCast(pi.GetValue(Me.Model.PageViewModel), String)
            Case "Hour"
                hour = DirectCast(pi.GetValue(Me.Model.PageViewModel), String)
            Case "Minute"
                minute = DirectCast(pi.GetValue(Me.Model.PageViewModel), String)
        End Select

    Next
    
    If meridiem = String.Empty Then
        meridiem = recordDate.ToString("tt", System.Globalization.CultureInfo.InvariantCulture).ToLower()
    End If
    
    If hour = String.Empty Then
        hour = (recordDate.Hour Mod 12).ToString()
    End If
    
    If minute = String.Empty Then
        minute = recordDate.Minute
    End If
    
End Code

<div class="target-date-time">
    <div class="input-next hide"><!-- 続けて登録時に表示する -->
		<p class="input-next"><strong><em>MM月dd日</em>（<em>午前00時00分</em>）の<em>朝食</em></strong>を<span class="inline-block">続けて登録する</span></p>
	</div><!-- input-next end -->	

	<div class="next-hide"><!-- 続けて登録時に隠す -->
		<div class="input-group mb10">
			<span class="input-group-addon">食事日</span>
            <input type="text" id="record-date" class="form-control picker" name="record-date" value="@String.Format("{0:yyyy年MM月dd日}", recordDate)" readonly="readonly" style="background-color:white;" autocomplete="off">
		</div>
        @QyHtmlHelper.ToMealTypeRadioButton("mealtype", "mealtype", mealType.ToString(), css:="mb10 mealtime")
        @If Me.Model.DisplayTime Then
		    @<div class="input-group datetime-select">
			    <span class="input-group-addon">時間</span>
			    @QyHtmlHelper.ToMeridiemDropDownList("meridiem", "meridiem", meridiem, css:="form-control ap")
                @QyHtmlHelper.ToHourDropDownList("hour", "hour", hour, css:="form-control hour")
                @QyHtmlHelper.ToMinuteDropDownList("minute", "minute", minute, css:="form-control minute")
		    </div>
        End If
    </div>
</div>
