@Imports MGF.QOLMS.QolmsApiCoreV1
@Imports MGF.QOLMS.QolmsYappli
@ModelType HealthAgeUrineReportPartialViewModel

@Code
    Dim hasData As Boolean = Me.Model.ReportItem.HealthAgeValueN.Any()
End Code

@If hasData Then
    @<article id="urine-area" class="reload-area" data-report-type="@QyHealthAgeReportTypeEnum.Urine.ToString()">
        <h2 class="box-title">
            <b>尿糖・尿蛋白</b>についてのレポート
        </h2>
    
        <section class="inner">
            <table class="table table-bordered sugar-table">
                <thead>
                    <tr>
                        <th></th>
                        <th>前々回</th>
                        <th>前回</th>
                        <th>今回</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <th>尿糖</th>
                        <td>@Me.Model.TableData(0).Item1</td>
                        <td>@Me.Model.TableData(1).Item1</td>
                        <td>@Me.Model.TableData(2).Item1</td>
                    </tr>
                    <tr>
                        <th>尿蛋白</th>
                        <td>@Me.Model.TableData(0).Item2</td>
                        <td>@Me.Model.TableData(1).Item2</td>
                        <td>@Me.Model.TableData(2).Item2</td>
                    </tr>
                </tbody>
            </table>
        </section>
    </article>
Else
    @<article id="urine-area" class="reload-area" data-report-empty-type="@QyHealthAgeReportTypeEnum.Urine.ToString()">
        <h2 class="box-title">
            <b>尿糖・尿蛋白</b>についてのレポート
        </h2>

        <section class="section caution mt10 mb0">
            未測定です
        </section>
    </article>
End If
