<Serializable()>
Public NotInheritable Class HealthAgeReportItem

#Region "Public Property"

    ''' <summary>
    ''' 健康年齢レポート情報の種別を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property HealthAgeReportType As QyHealthAgeReportTypeEnum = QyHealthAgeReportTypeEnum.None

    ''' <summary>
    ''' 健康年齢値情報のリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property HealthAgeValueN As New List(Of HealthAgeValueItem)()

    ''' <summary>
    ''' 健診結果レベル判定を取得または設定します（0～3）。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Deviance As Decimal = Decimal.MinValue

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="HealthAgeReportItem" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class
