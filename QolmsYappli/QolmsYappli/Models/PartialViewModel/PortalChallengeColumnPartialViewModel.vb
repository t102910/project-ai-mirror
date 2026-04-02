<Serializable()>
Public NotInheritable Class PortalChallengeColumnPartialViewModel
    Inherits QyPartialViewModelBase(Of PortalChallengeColumnViewModel)

#Region "Public Property"

    ''' <summary>
    ''' コラムを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ChallengeColumnItem As New ChallengeColumnItem

    ''' <summary>
    ''' 次のコラムがあるかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ExistsNext As Boolean = False

    ''' <summary>
    ''' 前のコラムがあるかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ExistsBefore As Boolean = False


#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PortalChallengeColumnModalPartialView" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' <see cref="PortalChallengeColumnModalPartialView" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(model As PortalChallengeColumnViewModel)

        MyBase.New(model)

    End Sub

#End Region

End Class
