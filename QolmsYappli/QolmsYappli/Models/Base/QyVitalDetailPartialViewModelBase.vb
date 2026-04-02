Imports System.Collections.ObjectModel

<Serializable()>
Public MustInherit Class QyVitalDetailPartialViewModelBase
    Inherits QyPartialViewModelBase(Of QyPageViewModelBase)

#Region "Variable"

    Protected _vitalType As QyVitalTypeEnum = QyVitalTypeEnum.None

    Protected _recordDate As Date = Date.MinValue

#End Region

#Region "Public Property"

    Public ReadOnly Property VitalType As QyVitalTypeEnum

        Get
            Return Me._vitalType
        End Get

    End Property

    Public ReadOnly Property RecordDate As Date

        Get
            Return Me._recordDate
        End Get

    End Property

    Public ReadOnly Property ItemList As ReadOnlyCollection(Of VitalValueItem)

        Get
            Return Me.GetVitalList().AsReadOnly()
        End Get

    End Property

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="QyVitalDetailPartialViewModelBase" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub New()

        MyBase.New()

    End Sub

    Protected Sub New(model As NoteWalkViewModel, recordDate As Date)

        MyBase.New(model)

        Me._vitalType = QyVitalTypeEnum.Steps
        Me._recordDate = If(recordDate = Date.MinValue, recordDate, recordDate.Date)

    End Sub

    Protected Sub New(model As NoteVitalViewModel, vitalType As QyVitalTypeEnum, recordDate As Date)

        MyBase.New(model)

        Me._vitalType = vitalType
        Me._recordDate = If(recordDate = Date.MinValue, recordDate, recordDate.Date)

    End Sub

#End Region

#Region "Protected Method"

    Protected Overridable Function GetVitalList() As List(Of VitalValueItem)

        Dim result As New List(Of VitalValueItem)()

        Select Case True
            Case Me.PageViewModel.GetType() Is GetType(NoteWalkViewModel) AndAlso Me._vitalType = QyVitalTypeEnum.Steps AndAlso Me._recordDate <> Date.MinValue
                With DirectCast(Me.PageViewModel, NoteWalkViewModel)
                    If .StepsPartialViewModel IsNot Nothing _
                        AndAlso .StepsPartialViewModel.ItemList IsNot Nothing _
                        AndAlso .StepsPartialViewModel.ItemList.ContainsKey(Me._recordDate) Then

                        result = .StepsPartialViewModel.ItemList(Me._recordDate).Select(Function(i) i.Value).ToList()
                    End If
                End With

            Case Me.PageViewModel.GetType() Is GetType(NoteVitalViewModel) AndAlso Me._vitalType = QyVitalTypeEnum.BloodPressure AndAlso Me._recordDate <> Date.MinValue
                With DirectCast(Me.PageViewModel, NoteVitalViewModel)
                    If .PressurePartialViewModel IsNot Nothing _
                        AndAlso .PressurePartialViewModel.ItemList IsNot Nothing _
                        AndAlso .PressurePartialViewModel.ItemList.ContainsKey(Me._recordDate) Then

                        result = .PressurePartialViewModel.ItemList(Me._recordDate).Skip(2).Select(Function(i) i.Value).ToList()
                    End If
                End With

            Case Me.PageViewModel.GetType() Is GetType(NoteVitalViewModel) AndAlso Me._vitalType = QyVitalTypeEnum.BloodSugar AndAlso Me._recordDate <> Date.MinValue
                With DirectCast(Me.PageViewModel, NoteVitalViewModel)
                    If .SugarPartialViewModel IsNot Nothing _
                        AndAlso .SugarPartialViewModel.ItemList IsNot Nothing _
                        AndAlso .SugarPartialViewModel.ItemList.ContainsKey(Me._recordDate) Then

                        result = .SugarPartialViewModel.ItemList(Me._recordDate).Skip(2).Select(Function(i) i.Value).ToList()
                    End If
                End With

            Case Me.PageViewModel.GetType() Is GetType(NoteVitalViewModel) AndAlso Me._vitalType = QyVitalTypeEnum.BodyWeight AndAlso Me._recordDate <> Date.MinValue
                With DirectCast(Me.PageViewModel, NoteVitalViewModel)
                    If .WeightPartialViewModel IsNot Nothing _
                        AndAlso .WeightPartialViewModel.ItemList IsNot Nothing _
                        AndAlso .WeightPartialViewModel.ItemList.ContainsKey(Me._recordDate) Then

                        result = .WeightPartialViewModel.ItemList(Me._recordDate).Skip(2).Select(Function(i) i.Value).ToList()
                    End If
                End With

        End Select

        Return result

    End Function

#End Region

End Class
