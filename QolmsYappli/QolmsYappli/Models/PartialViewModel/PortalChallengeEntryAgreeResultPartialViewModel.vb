<Serializable()>
Public NotInheritable Class PortalChallengeEntryAgreeResultPartialViewModel
    Inherits QyPartialViewModelBase(Of PortalChallengeEntryInputModel)

#Region "Public Property"

    ''' <summary>
    ''' 規約文章を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Terms As String = String.Empty

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PortalChallengeEntryAgreeResultPartialViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' <see cref="PortalChallengeEntryAgreeResultPartialViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(model As PortalChallengeEntryInputModel)

        MyBase.New(model)

    End Sub

#End Region

End Class
