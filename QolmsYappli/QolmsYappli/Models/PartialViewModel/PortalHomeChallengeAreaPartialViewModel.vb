<Serializable()>
Public NotInheritable Class PortalHomeChallengeAreaPartialViewModel
    Inherits QyPartialViewModelBase(Of PortalHomeViewModel)

#Region "Public Property"

    ''' <summary>
    ''' チャレンジのキーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Challengekey As Guid = Guid.Empty

    ''' <summary>
    ''' チャレンジの日数を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ChallengeAllDays As Integer = Integer.MinValue

    ''' <summary>
    ''' チャレンジの開始日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ChallengeStartDate As Date = Date.MinValue

    ''' <summary>
    ''' チャレンジの経過日数を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ChallengeTargetDay As Integer = Integer.MinValue

    ''' <summary>
    ''' チャレンジの体重減量目標までの値を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ChallengeTargetWeightLoss As Decimal = Decimal.MinValue

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PortalHomeChallengeAreaPartialViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' <see cref="PortalHomeChallengeAreaPartialViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(model As PortalHomeViewModel)

        MyBase.New(model)

    End Sub

#End Region

End Class
