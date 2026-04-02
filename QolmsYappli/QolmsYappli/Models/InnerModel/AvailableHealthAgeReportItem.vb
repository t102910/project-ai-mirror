''' <summary>
''' 健康年齢レポート情報の有効性を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class AvailableHealthAgeReportItem

#Region "Public Property"

    ''' <summary>
    ''' 健康年齢レポート情報の種別を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property HealthAgeReportType As QyHealthAgeReportTypeEnum = QyHealthAgeReportTypeEnum.None

    ''' <summary>
    ''' 最新の測定日時を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LatestDate As Date = Date.MinValue

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="AvailableHealthAgeReportItem" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class
