<Serializable()>
Public NotInheritable Class NoteMealEditDatePartialViewModel
    Inherits QyPartialViewModelBase(Of QyNotePageViewModelBase)

#Region "Public Property"

    Public Property DisplayTime As Boolean = True

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="NoteMealEditDatePartialViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' <see cref="NoteMealEditDatePartialViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(model As QyNotePageViewModelBase)

        MyBase.New(model)

    End Sub

#End Region

End Class
