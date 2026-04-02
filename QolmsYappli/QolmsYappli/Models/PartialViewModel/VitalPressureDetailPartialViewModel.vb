<Serializable()>
Public NotInheritable Class VitalPressureDetailPartialViewModel
    Inherits QyVitalDetailPartialViewModelBase

#Region "Constructor"

    ''' <summary>
    ''' <see cref="VitalPressureDetailPartialViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(model As NoteVitalViewModel, recordDate As Date)

        MyBase.New(model, QyVitalTypeEnum.BloodPressure, recordDate)

    End Sub

#End Region

End Class
