Imports System.Collections.ObjectModel

<Serializable()>
Public NotInheritable Class HealthAgeLipidReportPartialViewModel
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

    Public ReadOnly Property Ch019GraphSetting As HealthAgeReportGraphItem

        Get
            Return Me._graphSettingN(QyHealthAgeValueTypeEnum.Ch019)
        End Get

    End Property

    Public ReadOnly Property Ch021GraphSetting As HealthAgeReportGraphItem

        Get
            Return Me._graphSettingN(QyHealthAgeValueTypeEnum.Ch021)
        End Get

    End Property

    Public ReadOnly Property Ch023GraphSetting As HealthAgeReportGraphItem

        Get
            Return Me._graphSettingN(QyHealthAgeValueTypeEnum.Ch023)
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
            Case QyHealthAgeValueTypeEnum.Ch019
                ' 中性脂肪
                ' 29 以下 ：低中性脂肪血症
                ' 30～149 ：正常
                ' 150～299：軽度高中性脂肪血症
                ' 300～749：中等度高中性脂肪血症
                ' 750 以上：高度高中性脂肪血症
                result = "[30.0, 149.9, 10.0, 29.9, 150.0, 299.9, null, null, 300.0, 2000.0]"

            Case QyHealthAgeValueTypeEnum.Ch021
                ' HDL コレステロール
                ' 19 以下 ：先天性異常の疑い
                ' 20～39  ：低 HDL コレステロール血症
                ' 40～99  ：正常
                ' 100 以上：高 HDL コレステロール血症や先天性の異常の疑い
                result = "[40.0, 99.9, 20.0, 39.9, null, null, 10.0, 19.9, 100.0, 500.0]"

            Case QyHealthAgeValueTypeEnum.Ch023
                ' LDL コレステロール
                ' 60～119 ：正常
                ' 120～139：境界域
                ' 140 以上：高 LDL コレステロール血症や先天性の異常の疑い
                result = "[60.0, 119.9, null, null, 120.0, 139.9, null, null, 140.0, 1000.0]"

        End Select

        Return result

    End Function

    Private Sub SetDeviance(reportItem As HealthAgeReportItem)

        If reportItem IsNot Nothing _
            AndAlso reportItem.HealthAgeReportType = QyHealthAgeReportTypeEnum.Lipid _
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
            AndAlso reportItem.HealthAgeReportType = QyHealthAgeReportTypeEnum.Lipid _
            AndAlso reportItem.HealthAgeValueN IsNot Nothing _
            AndAlso reportItem.HealthAgeValueN.Any() Then

            Dim valueN As List(Of HealthAgeValueItem) = reportItem.HealthAgeValueN.Where(Function(i) i.HealthAgeValueType = QyHealthAgeValueTypeEnum.InsComparison).OrderBy(Function(i) i.SortOrder).ToList()

            If valueN.Count = 3 Then
                Me._graphDataN(QyHealthAgeValueTypeEnum.InsComparison) = valueN.ConvertAll(Function(i) i.Comparison)
                redCodeN = valueN.ConvertAll(Function(i) i.IsRedCode)
            Else
                Me._graphDataN(QyHealthAgeValueTypeEnum.InsComparison) = Enumerable.Repeat(Of Decimal)(Decimal.MinValue, 3).ToList()
                redCodeN = Enumerable.Repeat(Of Boolean)(False, 3).ToList()
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

            .Label = "['中性<br/>脂肪', 'HDL', 'LDL']"
            .TargetValue = "[-5, 0, 0, 5]" ' TODO: 仮
            .Data = String.Format("[{0}]", String.Join(","c, valueStringN))
        End With

    End Sub

    ' 中性脂肪、HD Lコレステロール、LDL コレステロール
    Private Sub SetLineGraphSetting(reportItem As HealthAgeReportItem, valueType As QyHealthAgeValueTypeEnum)

        If valueType = QyHealthAgeValueTypeEnum.Ch019 _
            OrElse valueType = QyHealthAgeValueTypeEnum.Ch021 _
            OrElse valueType = QyHealthAgeValueTypeEnum.Ch023 Then

            If reportItem IsNot Nothing _
                AndAlso reportItem.HealthAgeReportType = QyHealthAgeReportTypeEnum.Lipid _
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

            If valueType = QyHealthAgeValueTypeEnum.Ch019 Then
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
            Else
                Me._graphDataN(valueType).ForEach(
                    Sub(i)
                        If i >= Decimal.Zero Then
                            valueStringN.Add(i.ToString("0.#")) ' 小数点以下 1 桁

                            axisMin = Math.Min(axisMin, i)
                            axisMax = Math.Max(axisMax, i)
                        Else
                            valueStringN.Add("null")
                        End If
                    End Sub
                )
            End If

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

        ' 棒グラフ 1 種、折れ線グラフ 3 種
        Me._graphDataN = New Dictionary(Of QyHealthAgeValueTypeEnum, List(Of Decimal))() From {
            {
                QyHealthAgeValueTypeEnum.InsComparison,
                Enumerable.Repeat(Of Decimal)(Decimal.MinValue, 3).ToList()
            },
            {
                QyHealthAgeValueTypeEnum.Ch019,
                Enumerable.Repeat(Of Decimal)(Decimal.MinValue, 3).ToList()
            },
            {
                QyHealthAgeValueTypeEnum.Ch021,
                Enumerable.Repeat(Of Decimal)(Decimal.MinValue, 3).ToList()
            },
            {
                QyHealthAgeValueTypeEnum.Ch023,
                Enumerable.Repeat(Of Decimal)(Decimal.MinValue, 3).ToList()
            }
        }

        ' 棒グラフ 1 種、折れ線グラフ 3 種
        Me._graphSettingN = New Dictionary(Of QyHealthAgeValueTypeEnum, HealthAgeReportGraphItem)() From {
            {
                QyHealthAgeValueTypeEnum.InsComparison,
                New HealthAgeReportGraphItem() With {
                    .Title = "'脂質'",
                    .AxisMax = Decimal.Zero,
                    .AxisMin = Decimal.Zero,
                    .Label = "[]",
                    .TargetValue = "[]",
                    .Data = "[]"
                }
            },
            {
                QyHealthAgeValueTypeEnum.Ch019,
                New HealthAgeReportGraphItem() With {
                    .Title = "'中性脂肪'",
                    .AxisMax = Decimal.Zero,
                    .AxisMin = Decimal.Zero,
                    .Label = "[]",
                    .TargetValue = "[]",
                    .Data = "[]"
                }
            },
            {
                QyHealthAgeValueTypeEnum.Ch021,
                New HealthAgeReportGraphItem() With {
                    .Title = "'HDLコレステロール'",
                    .AxisMax = Decimal.Zero,
                    .AxisMin = Decimal.Zero,
                    .Label = "[]",
                    .TargetValue = "[]",
                    .Data = "[]"
                }
            },
            {
                QyHealthAgeValueTypeEnum.Ch023,
                New HealthAgeReportGraphItem() With {
                    .Title = "'HDLコレステロール'",
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
        Me.SetLineGraphSetting(reportItem, QyHealthAgeValueTypeEnum.Ch019)
        Me.SetLineGraphSetting(reportItem, QyHealthAgeValueTypeEnum.Ch021)
        Me.SetLineGraphSetting(reportItem, QyHealthAgeValueTypeEnum.Ch023)

    End Sub

#End Region

End Class
