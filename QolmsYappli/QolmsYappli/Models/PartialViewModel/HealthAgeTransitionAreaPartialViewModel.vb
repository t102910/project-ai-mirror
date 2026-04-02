Imports System.Collections.ObjectModel

<Serializable()>
Public NotInheritable Class HealthAgeTransitionAreaPartialViewModel
    Inherits QyPartialViewModelBase(Of HealthAgeViewModel)

#Region "Variable"

    Private _hasData As Boolean = False

    Private _ageData As New List(Of Tuple(Of String, String))()

#End Region

#Region "Public Property"

    Public ReadOnly Property HasData As Boolean

        Get
            Return Me._hasData
        End Get

    End Property

    Public ReadOnly Property AgeData As ReadOnlyCollection(Of Tuple(Of String, String))

        Get
            Return Me._ageData.ToList().AsReadOnly()
        End Get

    End Property

#End Region

#Region "Private Method"

    Private Sub InitializeBy(items As IEnumerable(Of HealthAgeValueItem))

        Me._hasData = False
        Me._ageData = New List(Of Tuple(Of String, String))()

        If items IsNot Nothing Then
            Dim ageN As List(Of HealthAgeValueItem) = items.Where(Function(i) i.HealthAgeValueType = QyHealthAgeValueTypeEnum.Calculation).OrderByDescending(Function(i) i.RecordDate).Take(3).ToList()

            Select Case ageN.Count
                Case 2
                    ageN.Add(New HealthAgeValueItem() With {.RecordDate = Date.MinValue, .Value = Decimal.MinValue})

                Case 1
                    ageN.AddRange(
                        {
                            New HealthAgeValueItem() With {.RecordDate = Date.MinValue, .Value = Decimal.MinValue},
                            New HealthAgeValueItem() With {.RecordDate = Date.MinValue, .Value = Decimal.MinValue}
                        }
                    )

                Case 0
                    ageN.AddRange(
                        {
                            New HealthAgeValueItem() With {.RecordDate = Date.MinValue, .Value = Decimal.MinValue},
                            New HealthAgeValueItem() With {.RecordDate = Date.MinValue, .Value = Decimal.MinValue},
                            New HealthAgeValueItem() With {.RecordDate = Date.MinValue, .Value = Decimal.MinValue}
                        }
                    )

            End Select

            ageN.Reverse()

            For a As Integer = 0 To 2
                If ageN(a).RecordDate <> Date.MinValue And ageN(a).Value > Decimal.Zero Then
                    Me._ageData.Add(New Tuple(Of String, String)(ageN(a).RecordDate.ToString("yyyy年M月d日"), ageN(a).Value.ToString("0.#")))

                    Me._hasData = True
                Else
                    Me._ageData.Add(New Tuple(Of String, String)("－", "－"))
                End If
            Next
        Else
            Me._ageData.Add(New Tuple(Of String, String)("－", "－"))
            Me._ageData.Add(New Tuple(Of String, String)("－", "－"))
            Me._ageData.Add(New Tuple(Of String, String)("－", "－"))
        End If

    End Sub

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="HealthAgeTransitionAreaPartialViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' <see cref="HealthAgeTransitionAreaPartialViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(model As HealthAgeViewModel)

        MyBase.New(model)

        Me.InitializeBy(model.HealthAgeN)

    End Sub

#End Region

End Class
