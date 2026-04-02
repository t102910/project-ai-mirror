<Serializable()>
Public NotInheritable Class VitalStepsDetailPartialViewModel
    Inherits QyVitalDetailPartialViewModelBase

#Region "Constructor"

    ''' <summary>
    ''' <see cref="VitalStepsDetailPartialViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(model As NoteWalkViewModel, recordDate As Date)

        MyBase.New(model, recordDate)

    End Sub

#End Region

End Class
