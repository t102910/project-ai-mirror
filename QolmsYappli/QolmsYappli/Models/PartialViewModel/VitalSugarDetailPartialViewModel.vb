<Serializable()>
Public NotInheritable Class VitalSugarDetailPartialViewModel
    Inherits QyVitalDetailPartialViewModelBase

#Region "Constructor"

    ''' <summary>
    ''' <see cref="VitalSugarDetailPartialViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(model As NoteVitalViewModel, recordDate As Date)

        MyBase.New(model, QyVitalTypeEnum.BloodSugar, recordDate)

    End Sub

#End Region

End Class
