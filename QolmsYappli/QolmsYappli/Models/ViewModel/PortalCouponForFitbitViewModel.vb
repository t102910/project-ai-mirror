''' <summary>
''' 「Fitbit クーポン」画面ビュー モデルを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class PortalCouponForFitbitViewModel
    Inherits QyPortalPageViewModelBase

#Region "Public Property"

    ''' <summary>
    ''' 現在のポイントを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FromPageNoType As Integer = Integer.MinValue

    ''' <summary>
    ''' 交換対象クーポンのリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CouponN As New List(Of CouponItem)()

    ''' <summary>
    ''' 保持ポイントを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Point As Integer = Integer.MinValue

    ''' <summary>
    ''' ポイント交換履歴のリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PointExchangeHistN As New List(Of PointExchangeHistItem)()

    ''' <summary>
    ''' ポイント交換の説明文を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Description As String = String.Empty

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PortalCouponForFitbitViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' メイン モデルを指定して、
    ''' <see cref="PortalCouponForFitbitViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <remarks></remarks>
    Public Sub New(mainModel As QolmsYappliModel)

        MyBase.New(mainModel, QyPageNoTypeEnum.PortalCouponForFitbit)

    End Sub

#End Region

End Class