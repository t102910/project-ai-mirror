Imports System.Collections.ObjectModel

<Serializable()>
Public NotInheritable Class HealthAgePressureReportPartialViewModel
    Inherits QyHealthAgeReportPartialViewModelBase

#Region "Variable"

    Private _deviance As Decimal = Decimal.Zero

    Private _graphDataN As New Dictionary(Of QyHealthAgeValueTypeEnum, List(Of Decimal))()

    Private _graphSettingN As New Dictionary(Of QyHealthAgeValueTypeEnum, HealthAgeReportGraphItem)()

#End Region

#Region "Public Property"

    Public ReadOnly Property InsDeviance As Decimal

        Get
            Return Me._deviance
        End Get

    End Property

    Public ReadOnly Property InsComparisonGraphSetting As HealthAgeReportGraphItem

        Get
            Return Me._graphSettingN(QyHealthAgeValueTypeEnum.InsComparison)
        End Get

    End Property

    Public ReadOnly Property Ch014GraphSetting As HealthAgeReportGraphItem

        Get
            Return Me._graphSettingN(QyHealthAgeValueTypeEnum.Ch014)
        End Get

    End Property

    Public ReadOnly Property Ch016GraphSetting As HealthAgeReportGraphItem

        Get
            Return Me._graphSettingN(QyHealthAgeValueTypeEnum.Ch016)
        End Get

    End Property

#End Region

#Region "Constructor"

    Public Sub New(model As HealthAgeViewModel, reportItem As HealthAgeReportItem)

        MyBase.New(model, reportItem)

        Me.InitializeBy(reportItem)

    End Sub

#End Region

#Region "Private Method"

    ' TODO:
    Private Function CreateTargetValues(valueType As QyHealthAgeValueTypeEnum) As String

        Dim result As String = "[]"

        Select Case valueType
            Case QyHealthAgeValueTypeEnum.Ch014
                ' 収縮期血圧
                ' 120 以下：至適血圧
                ' 121～130：正常血圧
                ' 131～140：正常高血圧
                ' 141～160：軽症高血圧
                ' 161～180：中等症高血圧
                ' 181 以上：重症高血圧
                result = "[0, 140.9, null, null, 141.0, 160.9, null, null, 161.0, 300.0]"

            Case QyHealthAgeValueTypeEnum.Ch016
                ' 拡張期血圧
                ' 80 以下 ：至適血圧
                ' 81～85  ：正常血圧
                ' 86～90  ：正常高血圧
                ' 91～100 ：軽症高血圧
                ' 101～110：中等症高血圧
                ' 111 以上：重症高血圧
                result = "[0, 90.9, null, null, 91.0, 100.9, null, null, 101.0, 150.0]"

        End Select

        Return result

    End Function

    Private Sub SetDeviance(reportItem As HealthAgeReportItem)

        If reportItem IsNot Nothing _
            AndAlso reportItem.HealthAgeReportType = QyHealthAgeReportTypeEnum.Pressure _
            AndAlso reportItem.HealthAgeValueN IsNot Nothing _
            AndAlso reportItem.HealthAgeValueN.Any() _
            AndAlso (reportItem.Deviance = 1D OrElse reportItem.Deviance = 2D OrElse reportItem.Deviance = 3D) Then

            Me._deviance = reportItem.Deviance
        End If

    End Sub

    ' 同世代健診値比較
    Private Sub SetBarGraphSetting(reportItem As HealthAgeReportItem)

        Dim redCodeN As New List(Of Boolean)()

        If reportItem IsNot Nothing _
            AndAlso reportItem.HealthAgeReportType = QyHealthAgeReportTypeEnum.Pressure _
            AndAlso reportItem.HealthAgeValueN IsNot Nothing _
            AndAlso reportItem.HealthAgeValueN.Any() Then

            Dim valueN As List(Of HealthAgeValueItem) = reportItem.HealthAgeValueN.Where(Function(i) i.HealthAgeValueType = QyHealthAgeValueTypeEnum.InsComparison).OrderBy(Function(i) i.SortOrder).ToList()

            If valueN.Count = 2 Then
                Me._graphDataN(QyHealthAgeValueTypeEnum.InsComparison) = valueN.ConvertAll(Function(i) i.Comparison)
                redCodeN = valueN.ConvertAll(Function(i) i.IsRedCode)
            Else
                Me._graphDataN(QyHealthAgeValueTypeEnum.InsComparison) = Enumerable.Repeat(Of Decimal)(Decimal.MinValue, 2).ToList()
                redCodeN = Enumerable.Repeat(Of Boolean)(False, 2).ToList()
            End If
        End If

        Dim valueStringN As New List(Of String)()
        Dim axisMin As Decimal = Decimal.MaxValue
        Dim axisMax As Decimal = Decimal.MinValue
        Dim index As Integer = 0

        Me._graphDataN(QyHealthAgeValueTypeEnum.InsComparison).ForEach(
            Sub(i)
                If i <> Decimal.MinValue Then
                    'valueStringN.Add(i.ToString("0.####")) ' 小数点以下 4 桁

                    ' 劣勢項目なら色を変える
                    If redCodeN(index) Then
                        valueStringN.Add(String.Format("{{color:'#ff3c3b',y:{0:0.####}}}", i)) ' 小数点以下 4 桁
                    Else
                        valueStringN.Add(i.ToString("0.####")) ' 小数点以下 4 桁
                    End If

                    axisMin = Math.Min(axisMin, i)
                    axisMax = Math.Max(axisMax, i)
                Else
                    valueStringN.Add("null")
                End If

                index += 1
            End Sub
        )

        With Me._graphSettingN(QyHealthAgeValueTypeEnum.InsComparison)
            If axisMin <> Decimal.MaxValue AndAlso axisMax <> Decimal.MinValue Then
                .AxisMin = Math.Max(axisMin - 1, Decimal.Zero)
                .AxisMax = Math.Max(axisMax + 1, Decimal.Zero)
            End If

            .Label = "['血圧<br/>(上)', '血圧<br/>(下)']"
            .TargetValue = "[-5, 0, 0, 5]" ' TODO: 仮
            .Data = String.Format("[{0}]", String.Join(","c, valueStringN))
        End With

    End Sub

    ' 収縮期血圧、拡張期血圧
    Private Sub SetLineGraphSetting(reportItem As HealthAgeReportItem, valueType As QyHealthAgeValueTypeEnum)

        If valueType = QyHealthAgeValueTypeEnum.Ch014 OrElse valueType = QyHealthAgeValueTypeEnum.Ch016 Then
            If reportItem IsNot Nothing _
                AndAlso reportItem.HealthAgeReportType = QyHealthAgeReportTypeEnum.Pressure _
                AndAlso reportItem.HealthAgeValueN IsNot Nothing _
                AndAlso reportItem.HealthAgeValueN.Any() Then

                Dim valueN As List(Of HealthAgeValueItem) = reportItem.HealthAgeValueN.Where(Function(i) i.HealthAgeValueType = valueType).OrderByDescending(Function(i) i.RecordDate).Take(3).ToList()

                Select Case valueN.Count
                    Case 2
                        valueN.Add(New HealthAgeValueItem() With {.RecordDate = Date.MinValue, .Value = Decimal.MinValue})

                    Case 1
                        valueN.AddRange(
                            {
                                New HealthAgeValueItem() With {.RecordDate = Date.MinValue, .Value = Decimal.MinValue},
                                New HealthAgeValueItem() With {.RecordDate = Date.MinValue, .Value = Decimal.MinValue}
                            }
                        )

                    Case 0
                        valueN.AddRange(
                            {
                                New HealthAgeValueItem() With {.RecordDate = Date.MinValue, .Value = Decimal.MinValue},
                                New HealthAgeValueItem() With {.RecordDate = Date.MinValue, .Value = Decimal.MinValue},
                                New HealthAgeValueItem() With {.RecordDate = Date.MinValue, .Value = Decimal.MinValue}
                            }
                        )

                End Select

                valueN.Reverse()

                Me._graphDataN(valueType) = valueN.ConvertAll(Function(i) i.Value)
            End If

            Dim valueStringN As New List(Of String)()
            Dim axisMin As Decimal = Decimal.MaxValue
            Dim axisMax As Decimal = Decimal.MinValue

            Me._graphDataN(valueType).ForEach(
                Sub(i)
                    If i >= Decimal.Zero Then
                        valueStringN.Add(i.ToString()) ' 整数

                        axisMin = Math.Min(axisMin, i)
                        axisMax = Math.Max(axisMax, i)
                    Else
                        valueStringN.Add("null")
                    End If
                End Sub
            )

            With Me._graphSettingN(valueType)
                If axisMin <> Decimal.MaxValue AndAlso axisMax <> Decimal.MinValue Then
                    .AxisMin = Math.Max(axisMin - 1D, Decimal.Zero)
                    .AxisMax = Math.Max(axisMax + 1D, Decimal.Zero)
                End If

                .Label = "['前々回', '前回', '今回']"
                .TargetValue = Me.CreateTargetValues(valueType)
                .Data = String.Format("[{0}]", String.Join(","c, valueStringN))
            End With
        End If

    End Sub

#End Region

#Region "Protected Method"

    Protected Overrides Sub InitializeBy(reportItem As HealthAgeReportItem)

        ' 棒グラフ 1 種、折れ線グラフ 2 種
        Me._graphDataN = New Dictionary(Of QyHealthAgeValueTypeEnum, List(Of Decimal))() From {
            {
                QyHealthAgeValueTypeEnum.InsComparison,
                Enumerable.Repeat(Of Decimal)(Decimal.MinValue, 2).ToList()
            },
            {
                QyHealthAgeValueTypeEnum.Ch014,
                Enumerable.Repeat(Of Decimal)(Decimal.MinValue, 3).ToList()
            },
            {
                QyHealthAgeValueTypeEnum.Ch016,
                Enumerable.Repeat(Of Decimal)(Decimal.MinValue, 3).ToList()
            }
        }

        ' 棒グラフ 1 種、折れ線グラフ 2 種
        Me._graphSettingN = New Dictionary(Of QyHealthAgeValueTypeEnum, HealthAgeReportGraphItem)() From {
            {
                QyHealthAgeValueTypeEnum.InsComparison,
                New HealthAgeReportGraphItem() With {
                    .Title = "'血圧'",
                    .AxisMax = Decimal.Zero,
                    .AxisMin = Decimal.Zero,
                    .Label = "[]",
                    .TargetValue = "[]",
                    .Data = "[]"
                }
            },
            {
                QyHealthAgeValueTypeEnum.Ch014,
                New HealthAgeReportGraphItem() With {
                    .Title = "'血圧（上）'",
                    .AxisMax = Decimal.Zero,
                    .AxisMin = Decimal.Zero,
                    .Label = "[]",
                    .TargetValue = "[]",
                    .Data = "[]"
                }
            },
            {
                QyHealthAgeValueTypeEnum.Ch016,
                New HealthAgeReportGraphItem() With {
                    .Title = "'血圧（下）'",
                    .AxisMax = Decimal.Zero,
                    .AxisMin = Decimal.Zero,
                    .Label = "[]",
                    .TargetValue = "[]",
                    .Data = "[]"
                }
            }
        }

        Me.SetDeviance(reportItem)
        Me.SetBarGraphSetting(reportItem)
        Me.SetLineGraphSetting(reportItem, QyHealthAgeValueTypeEnum.Ch014)
        Me.SetLineGraphSetting(reportItem, QyHealthAgeValueTypeEnum.Ch016)

    End Sub

#End Region

End Class
