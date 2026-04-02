Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations

''' <summary>
''' 「退会」画面ビュー モデルを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class PortalUnsubscribeInputModel
    Inherits QyPortalPageViewModelBase
    ''' <summary>
    ''' プレミアムメンバーシップタイプを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PremiumMemberShipType As Byte = Byte.MinValue

    ''' <summary>
    ''' プレミアム退会説明文章を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PremiumDescription As String = String.Empty

    ''' <summary>
    ''' 退会説明文章を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Description As String = String.Empty

    ''' <summary>
    ''' 退会理由のリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ReasonList As New List(Of KeyValuePair(Of Integer, String))()

    ''' <summary>
    ''' 退会理由を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("退会理由")>
    <Required(AllowEmptyStrings:=False, ErrorMessage:="{0}は必須項目です。")>
    Public Property ReasonCode As Integer = Integer.MinValue

    ''' <summary>
    ''' 退会コメントを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DisplayName("退会コメント")>
    Public Property ReasonComment As String = String.Empty

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PortalUnsubscribeInputModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' メイン モデルを指定して、
    ''' <see cref="PortalUnsubscribeInputModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <remarks></remarks>
    Public Sub New(mainModel As QolmsYappliModel)

        MyBase.New(mainModel, QyPageNoTypeEnum.PortalUnsubscribe)

    End Sub

#End Region

End Class
