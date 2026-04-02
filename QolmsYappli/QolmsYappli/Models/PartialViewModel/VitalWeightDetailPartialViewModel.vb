<Serializable()>
Public NotInheritable Class VitalWeightDetailPartialViewModel
    Inherits QyVitalDetailPartialViewModelBase

#Region "Constructor"

    ''' <summary>
    ''' <see cref="VitalWeightDetailPartialViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(model As NoteVitalViewModel, recordDate As Date)

        MyBase.New(model, QyVitalTypeEnum.BodyWeight, recordDate)

    End Sub

#End Region

End Class
