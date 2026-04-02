Imports System.Collections.ObjectModel

<Serializable()>
Public NotInheritable Class HealthAgeDistributionReportPartialViewModel
    Inherits QyHealthAgeReportPartialViewModelBase

#Region "Constant"

    Private Shared ReadOnly ageRanges As New List(Of Tuple(Of Decimal, Decimal, String))() From {
        {New Tuple(Of Decimal, Decimal, String)(Decimal.MinValue, 20D, "'～20歳'")},
        {New Tuple(Of Decimal, Decimal, String)(20D, 25D, "'25歳'")},
        {New Tuple(Of Decimal, Decimal, String)(25D, 30D, "'30歳'")},
        {New Tuple(Of Decimal, Decimal, String)(30D, 35D, "'35歳'")},
        {New Tuple(Of Decimal, Decimal, String)(35D, 40D, "'40歳'")},
        {New Tuple(Of Decimal, Decimal, String)(40D, 45D, "'45歳'")},
        {New Tuple(Of Decimal, Decimal, String)(45D, 50D, "'50歳'")},
        {New Tuple(Of Decimal, Decimal, String)(50D, 55D, "'55歳'")},
        {New Tuple(Of Decimal, Decimal, String)(55D, 60D, "'60歳'")},
        {New Tuple(Of Decimal, Decimal, String)(60D, 65D, "'65歳'")},
        {New Tuple(Of Decimal, Decimal, String)(65D, 70D, "'70歳'")},
        {New Tuple(Of Decimal, Decimal, String)(70D, Decimal.MaxValue, "'70歳～'")}
    }

#End Region

#Region "Variable"

    Private _graphData As New List(Of Decimal)()

    Private _graphSetting As New HealthAgeReportGraphItem()

    Private _healthAge As Decimal = Decimal.MinValue

#End Region

#Region "Public Property"

    Public ReadOnly Property AgeDistributionGraphSetting As HealthAgeReportGraphItem

        Get
            Return Me._graphSetting
        End Get

    End Property

#End Region

#Region "Constructor"

    Public Sub New(model As HealthAgeViewModel, reportItem As HealthAgeReportItem)

        MyBase.New(model, reportItem)

        Me._healthAge = model.HealthAge ' TODO:

        Me.InitializeBy(reportItem)

    End Sub

#End Region

#Region "Private Method"

    ' 同世代健康年齢分布
    Private Sub SetBarGraphSetting(reportItem As HealthAgeReportItem)

        If reportItem IsNot Nothing _
            AndAlso reportItem.HealthAgeReportType = QyHealthAgeReportTypeEnum.Distribution _
            AndAlso reportItem.HealthAgeValueN IsNot Nothing _
            AndAlso reportItem.HealthAgeValueN.Any() Then

            Dim valueN As List(Of HealthAgeValueItem) = reportItem.HealthAgeValueN.Where(Function(i) i.HealthAgeValueType = QyHealthAgeValueTypeEnum.AgeDistribution).OrderBy(Function(i) i.SortOrder).ToList()

            If valueN.Count = 12 Then
                Me._graphData = valueN.ConvertAll(Function(i) i.Value)
            Else
                Me._graphData = Enumerable.Repeat(Of Decimal)(Decimal.MinValue, 12).ToList()
            End If

            Dim valueStringN As New List(Of String)()
            Dim axisMin As Decimal = Decimal.MaxValue
            Dim axisMax As Decimal = Decimal.MinValue

            Dim index As Integer = -1

            Me._graphData.ForEach(
                Sub(i)
                    index += 1

                    If i >= Decimal.Zero Then
                        ' 健康年齢が含まれる値（例：20歳<X≦25歳）なら色を変える
                        If HealthAgeDistributionReportPartialViewModel.ageRanges(index).Item1 < Me._healthAge _
                            And Me._healthAge <= HealthAgeDistributionReportPartialViewModel.ageRanges(index).Item2 Then

                            valueStringN.Add(String.Format("{{color:'#ff7800',y:{0}}}", i))
                        Else
                            valueStringN.Add(i.ToString())
                        End If

                        axisMin = Math.Min(axisMin, i)
                        axisMax = Math.Max(axisMax, i)
                    Else
                        valueStringN.Add("null")
                    End If
                End Sub
            )

            With Me._graphSetting
                If axisMin <> Decimal.MaxValue AndAlso axisMax <> Decimal.MinValue Then
                    .AxisMin = Math.Max(axisMin - 1, Decimal.Zero)
                    .AxisMax = Math.Max(axisMax + 1, Decimal.Zero)
                End If

                .Label = String.Format("[{0}]", String.Join(","c, HealthAgeDistributionReportPartialViewModel.ageRanges.Select(Function(i) i.Item3))) '"['～20歳', '25歳', '30歳', '35歳', '40歳', '45歳', '50歳', '55歳', '60歳', '65歳', '70歳', '70歳～']"
                .TargetValue = "[]"
                .Data = String.Format("[{0}]", String.Join(","c, valueStringN))
            End With
        End If

    End Sub

#End Region

#Region "Protected Method"

    Protected Overrides Sub InitializeBy(reportItem As HealthAgeReportItem)

        ' 棒グラフ 1 種
        Me._graphData = Enumerable.Repeat(Of Decimal)(Decimal.MinValue, 12).ToList()

        ' 棒グラフ 1 種
        Me._graphSetting = New HealthAgeReportGraphItem() With {
            .Title = String.Empty,
            .AxisMax = Decimal.Zero,
            .AxisMin = Decimal.Zero,
            .Label = "[]",
            .TargetValue = "[]",
            .Data = "[]"
        }

        Me.SetBarGraphSetting(reportItem)

    End Sub

#End Region

End Class
