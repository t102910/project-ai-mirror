Friend NotInheritable Class PortalInformationWorker

#Region "Constructor"

    ''' <summary>
    ''' デフォルト コンストラクタは使用できません。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub

#End Region

#Region "Public Method"

    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel) As PortalInformationViewModel

        Return New PortalInformationViewModel(mainModel)

    End Function

#End Region

End Class
