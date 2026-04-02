@Imports MGF.QOLMS.QolmsYappli
@Imports MGF.QOLMS.QolmsDbEntityV1

@ModelType NoteMedicineTablePartialViewModel

@Code
    Dim rowNo As Integer = 0
End Code


@For Each item As MedicineItem In Me.Model.MedicineSetN

    @<article class="box">
	    <header>
		    <h3 class="box-title">@item.RecordDate.ToString("yyyy年MM月dd日")</h3>
	    </header>
	    <div class="inner">
		    <section class="info-list">
			    <section>
                    
				    <h4 class="">薬局名</h4>
				    <p>@item.PharmacyName</p>
			    </section>
			    <section>
				    <h4 class=""x>調剤日（処方日）</h4>
				    <p>@item.RecordDate.ToString("yyyy年MM月dd日")@IIf(item.PrescriptionDate = Date.MinValue, String.Empty, item.PrescriptionDate.ToString("（yyyy年MM月dd日）"))</p>
			    </section>
			    <section>
				    <h4 class="">薬剤師名</h4>
				    <p>@item.PharmacistName</p>
			    </section>
			    <section>
				    <h4 class="">処方医療機関</h4>
				    <p>@item.FacilityName</p>
			    </section>
		    </section>
            @If item.MedicineUsageN IsNot Nothing Then
                    For Each uItem As QhMedicineSetUsageItemOfJson In item.MedicineUsageN

		                @<div class="medicine-box">
			                <h5 class="title">
                                @If Not String.IsNullOrWhiteSpace(uItem.RepresentedOrganizationName) Then
				                    @<span class="department">@uItem.RepresentedOrganizationName</span>
                                 End If
				                <span class="dr-name">@String.Format("医師名：{0}", uItem.DoctorName)</span>
			                </h5>
			                <h4 class="title">
				                <span class="kind naifuku">@QyDictionary.DosageFormType(uItem.DosageForm)</span>
				                <span class="date">@String.Format("{0}{1}", uItem.Days, uItem.Unit) </span>
                                @If Not String.IsNullOrWhiteSpace(uItem.Usage) Then
				                    @<span class="timing">@uItem.Usage</span>
                                End If

                                @If Not String.IsNullOrWhiteSpace(uItem.Unit) Then
				                    @<span class="divid">@uItem.Unit</span>
                                End If

			                </h4>
			                <div class="inner">
                                @For Each mitem As QhMedicineSetEthicalDrugItemOfJson In uItem.MedicineN
                                    @<div class="medicine-list">
					                    <section class="medicine-name">
						                    <span class="name">
                                                @mitem.MedicineName
						                    </span>
						                    <span class="reader"></span>
						                    <span class="dose">
							                    @String.Format("{0}{1}", mitem.Dose, mitem.Unit)
						                    </span>
					                    </section>
					                    @*<section class="section default">
						                    @mitem.MedicineName
					                    </section>
					                    <section class="section caution">
						                    妊娠中、妊娠している可能性のある方、授乳中の方は、医師、薬剤師に申し出て下さい。<br>
						                    筋肉が痛む、脱力感、発疹、便秘、吐き気、頭痛、めまい等が現れることがあります。
					                    </section>*@
				                    </div>
                                Next
				                
			                </div>
		                </div>
                        
                Next
        End If
		    @*
            'If item.DataType <> Convert.ToByte(QH_MEDICINE_DAT.DataTypeEnum.OtcDrug) Then
            この条件はエラーになるのと現状意味ないので廃止中

            'End If
*@
	    </div>
    </article>
    
Next
