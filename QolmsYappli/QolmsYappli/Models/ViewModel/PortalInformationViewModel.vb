<Serializable()>
Public NotInheritable Class PortalInformationViewModel
    Inherits QyPortalPageViewModelBase

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PortalInformationViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' メイン モデルを指定して、
    ''' <see cref="PortalInformationViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <remarks></remarks>
    Public Sub New(mainModel As QolmsYappliModel)

        MyBase.New(mainModel, QyPageNoTypeEnum.PortalInformation)

    End Sub

#End Region

End Class
