''' <summary>
''' Amazonギフト券情報を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class AmazonGiftCardItem

#Region "Public Property"

    ''' <summary>
    ''' クーポン種別を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property GiftCardType As Byte = Byte.MinValue

    ''' <summary>
    ''' 消費ポイントを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DemandPoint As Integer = Integer.MinValue

    ''' <summary>
    ''' 表示名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property GiftCardName As String = String.Empty

    ''' <summary>
    ''' 残り枚数のカウントを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RestCount As Integer = Integer.MinValue

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="AmazonGiftCardItem" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class
