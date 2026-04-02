Imports System.Collections.ObjectModel

<Serializable()>
Public NotInheritable Class HealthAgeUrineReportPartialViewModel
    Inherits QyHealthAgeReportPartialViewModelBase

#Region "Constant"

    Private Shared ReadOnly valueSetN As New Dictionary(Of Decimal, String)() From {
        {1D, "－"},
        {2D, "±"},
        {3D, "＋"},
        {4D, "＋＋"},
        {5D, "＋＋＋"}
    }

#End Region

#Region "Variable"

    Private _tableData As New List(Of Tuple(Of String, String))()

#End Region

#Region "Public Property"

    Public ReadOnly Property TableData As ReadOnlyCollection(Of Tuple(Of String, String))

        Get
            Return Me._tableData.ToList().AsReadOnly()
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

    Private Function ToStringValue(value As Decimal) As String

        Dim result As String = String.Empty

        If HealthAgeUrineReportPartialViewModel.valueSetN.ContainsKey(value) Then result = HealthAgeUrineReportPartialViewModel.valueSetN(value)

        Return result

    End Function

#End Region

#Region "Protected Method"

    Protected Overrides Sub InitializeBy(reportItem As HealthAgeReportItem)

        Me._tableData = New List(Of Tuple(Of String, String))()

        If reportItem IsNot Nothing _
            AndAlso reportItem.HealthAgeReportType = QyHealthAgeReportTypeEnum.Urine _
            AndAlso reportItem.HealthAgeValueN IsNot Nothing _
            AndAlso reportItem.HealthAgeValueN.Any() Then

            ' 尿糖
            Dim ch037N As List(Of HealthAgeValueItem) = reportItem.HealthAgeValueN.Where(Function(i) i.HealthAgeValueType = QyHealthAgeValueTypeEnum.Ch037).OrderByDescending(Function(i) i.RecordDate).Take(3).ToList()

            Select Case ch037N.Count
                Case 2
                    ch037N.Add(New HealthAgeValueItem() With {.RecordDate = Date.MinValue, .Value = Decimal.MinValue})

                Case 1
                    ch037N.AddRange(
                        {
                            New HealthAgeValueItem() With {.RecordDate = Date.MinValue, .Value = Decimal.MinValue},
                            New HealthAgeValueItem() With {.RecordDate = Date.MinValue, .Value = Decimal.MinValue}
                        }
                    )

                Case 0
                    ch037N.AddRange(
                        {
                            New HealthAgeValueItem() With {.RecordDate = Date.MinValue, .Value = Decimal.MinValue},
                            New HealthAgeValueItem() With {.RecordDate = Date.MinValue, .Value = Decimal.MinValue},
                            New HealthAgeValueItem() With {.RecordDate = Date.MinValue, .Value = Decimal.MinValue}
                        }
                    )

            End Select

            ch037N.Reverse()

            ' 尿蛋白
            Dim ch039N As List(Of HealthAgeValueItem) = reportItem.HealthAgeValueN.Where(Function(i) i.HealthAgeValueType = QyHealthAgeValueTypeEnum.Ch039).OrderByDescending(Function(i) i.RecordDate).Take(3).ToList()

            Select Case ch039N.Count
                Case 2
                    ch039N.Add(New HealthAgeValueItem() With {.RecordDate = Date.MinValue, .Value = Decimal.MinValue})

                Case 1
                    ch039N.AddRange(
                        {
                            New HealthAgeValueItem() With {.RecordDate = Date.MinValue, .Value = Decimal.MinValue},
                            New HealthAgeValueItem() With {.RecordDate = Date.MinValue, .Value = Decimal.MinValue}
                        }
                    )

                Case 0
                    ch039N.AddRange(
                        {
                            New HealthAgeValueItem() With {.RecordDate = Date.MinValue, .Value = Decimal.MinValue},
                            New HealthAgeValueItem() With {.RecordDate = Date.MinValue, .Value = Decimal.MinValue},
                            New HealthAgeValueItem() With {.RecordDate = Date.MinValue, .Value = Decimal.MinValue}
                        }
                    )

            End Select

            ch039N.Reverse()

            Me._tableData.Add(New Tuple(Of String, String)(Me.ToStringValue(ch037N(0).Value), Me.ToStringValue(ch039N(0).Value)))
            Me._tableData.Add(New Tuple(Of String, String)(Me.ToStringValue(ch037N(1).Value), Me.ToStringValue(ch039N(1).Value)))
            Me._tableData.Add(New Tuple(Of String, String)(Me.ToStringValue(ch037N(2).Value), Me.ToStringValue(ch039N(2).Value)))
        Else
            Me._tableData.Add(New Tuple(Of String, String)(String.Empty, String.Empty))
            Me._tableData.Add(New Tuple(Of String, String)(String.Empty, String.Empty))
            Me._tableData.Add(New Tuple(Of String, String)(String.Empty, String.Empty))
        End If

    End Sub

#End Region

End Class
