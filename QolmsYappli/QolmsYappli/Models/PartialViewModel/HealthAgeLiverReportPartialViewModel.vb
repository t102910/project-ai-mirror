Imports System.Collections.ObjectModel

<Serializable()>
Public NotInheritable Class HealthAgeLiverReportPartialViewModel
    Inherits QyHealthAgeReportPartialViewModelBase

#Region "Variable"

    Private _deviance As Decimal = Decimal.Zero

    Private _graphDataN As New Dictionary(Of QyHealthAgeValueTypeEnum, List(Of Decimal))()

    Private _graphSettingN As New Dictionary(Of QyHealthAgeValueTypeEnum, HealthAgeReportGraphItem)()

    Private _sexType As QySexTypeEnum = QySexTypeEnum.None

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

    Public ReadOnly Property Ch025GraphSetting As HealthAgeReportGraphItem

        Get
            Return Me._graphSettingN(QyHealthAgeValueTypeEnum.Ch025)
        End Get

    End Property

    Public ReadOnly Property Ch027GraphSetting As HealthAgeReportGraphItem

        Get
            Return Me._graphSettingN(QyHealthAgeValueTypeEnum.Ch027)
        End Get

    End Property

    Public ReadOnly Property Ch029GraphSetting As HealthAgeReportGraphItem

        Get
            Return Me._graphSettingN(QyHealthAgeValueTypeEnum.Ch029)
        End Get

    End Property

#End Region

#Region "Constructor"

    Public Sub New(model As HealthAgeViewModel, reportItem As HealthAgeReportItem)

        MyBase.New(model, reportItem)

        Me._sexType = model.AuthorSex ' TODO:

        Me.InitializeBy(reportItem)

    End Sub

#End Region

#Region "Private Method"

    ' TODO:
    Private Function CreateTargetValues(valueType As QyHealthAgeValueTypeEnum) As String

        Dim result As String = "[]"

        Select Case valueType
            Case QyHealthAgeValueTypeEnum.Ch025
                ' GOT（AST）
                ' 8 未満  ：低値
                ' 8～38   ：正常
                ' 39～89  ：軽度上昇
                ' 90～499 ：中程度上昇
                ' 500 以上：高度上昇
                result = "[8.0, 38.9, 0, 7.9, 39.0, 89.9, null, null, 90.0, 1000.0]"

            Case QyHealthAgeValueTypeEnum.Ch027
                ' GPT（ALT）
                ' 4 未満  ：低値
                ' 4～43   ：正常
                ' 44～89  ：軽度上昇
                ' 90～499 ：中程度上昇
                ' 500 以上：高度上昇
                result = "[4.0, 43.9, 0, 3.9, 44.0, 89.9, null, null, 90.0, 1000.0]"

            Case QyHealthAgeValueTypeEnum.Ch029
                ' γ-GT（γ-GTP）
                ' 男性 86 以下、女性 48 以下：正常
                ' 男性 87～499、女性 49～499：アルコール多量摂取の場合、適正量を心がける。まれに肝炎、肝硬変の発症 が見られる。薬物による肝障害の有無については他の検査結果をみて判定。
                ' 500 以上                  ：入院して精密検査を受け、日常生活での医師の指導が必要となる。
                If Me._sexType = QySexTypeEnum.Male Then
                    ' 男性
                    result = "[0, 86.9, null, null, 87.0, 499.9, null, null, 500.0, 1000.0]"
                ElseIf Me._sexType = QySexTypeEnum.Female Then
                    ' 女性
                    result = "[0, 48.9, null, null, 49.0, 499.9, null, null, 500.0, 1000.0]"
                End If

        End Select

        Return result

    End Function

    Private Sub SetDeviance(reportItem As HealthAgeReportItem)

        If reportItem IsNot Nothing _
            AndAlso reportItem.HealthAgeReportType = QyHealthAgeReportTypeEnum.Liver _
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
            AndAlso reportItem.HealthAgeReportType = QyHealthAgeReportTypeEnum.Liver _
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

            .Label = "['AST<br/>(GOT)', 'ALT<br/>(GPT)', 'γ-GT<br/>(γ-GTP)']"
            .TargetValue = "[-5, 0, 0, 5]" ' TODO: 仮
            .Data = String.Format("[{0}]", String.Join(","c, valueStringN))
        End With

    End Sub

    ' AST（GOT）、ALT（GPT）、γ-GTP（γ-GT）
    Private Sub SetLineGraphSetting(reportItem As HealthAgeReportItem, valueType As QyHealthAgeValueTypeEnum)

        If valueType = QyHealthAgeValueTypeEnum.Ch025 _
            OrElse valueType = QyHealthAgeValueTypeEnum.Ch027 _
            OrElse valueType = QyHealthAgeValueTypeEnum.Ch029 Then

            If reportItem IsNot Nothing _
                AndAlso reportItem.HealthAgeReportType = QyHealthAgeReportTypeEnum.Liver _
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

        ' 棒グラフ 1 種、折れ線グラフ 3 種
        Me._graphDataN = New Dictionary(Of QyHealthAgeValueTypeEnum, List(Of Decimal))() From {
            {
                QyHealthAgeValueTypeEnum.InsComparison,
                Enumerable.Repeat(Of Decimal)(Decimal.MinValue, 3).ToList()
            },
            {
                QyHealthAgeValueTypeEnum.Ch025,
                Enumerable.Repeat(Of Decimal)(Decimal.MinValue, 3).ToList()
            },
            {
                QyHealthAgeValueTypeEnum.Ch027,
                Enumerable.Repeat(Of Decimal)(Decimal.MinValue, 3).ToList()
            },
            {
                QyHealthAgeValueTypeEnum.Ch029,
                Enumerable.Repeat(Of Decimal)(Decimal.MinValue, 3).ToList()
            }
        }

        ' 棒グラフ 1 種、折れ線グラフ 3 種
        Me._graphSettingN = New Dictionary(Of QyHealthAgeValueTypeEnum, HealthAgeReportGraphItem)() From {
            {
                QyHealthAgeValueTypeEnum.InsComparison,
                New HealthAgeReportGraphItem() With {
                    .Title = "'肝臓'",
                    .AxisMax = Decimal.Zero,
                    .AxisMin = Decimal.Zero,
                    .Label = "[]",
                    .TargetValue = "[]",
                    .Data = "[]"
                }
            },
            {
                QyHealthAgeValueTypeEnum.Ch025,
                New HealthAgeReportGraphItem() With {
                    .Title = "'AST（GOT）'",
                    .AxisMax = Decimal.Zero,
                    .AxisMin = Decimal.Zero,
                    .Label = "[]",
                    .TargetValue = "[]",
                    .Data = "[]"
                }
            },
            {
                QyHealthAgeValueTypeEnum.Ch027,
                New HealthAgeReportGraphItem() With {
                    .Title = "'ALT（GPT）'",
                    .AxisMax = Decimal.Zero,
                    .AxisMin = Decimal.Zero,
                    .Label = "[]",
                    .TargetValue = "[]",
                    .Data = "[]"
                }
            },
            {
                QyHealthAgeValueTypeEnum.Ch029,
                New HealthAgeReportGraphItem() With {
                    .Title = "'γ-GT（γ-GTP）'",
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
        Me.SetLineGraphSetting(reportItem, QyHealthAgeValueTypeEnum.Ch025)
        Me.SetLineGraphSetting(reportItem, QyHealthAgeValueTypeEnum.Ch027)
        Me.SetLineGraphSetting(reportItem, QyHealthAgeValueTypeEnum.Ch029)

    End Sub

#End Region

End Class
