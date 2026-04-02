<Serializable()>
Public NotInheritable Class HealthAgeValueItem

#Region "Public Property"

    ''' <summary>
    ''' 測定日時を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RecordDate As Date = Date.MinValue

    ''' <summary>
    ''' 健康年齢情報の種別を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property HealthAgeValueType As QyHealthAgeValueTypeEnum = QyHealthAgeValueTypeEnum.None

    ''' <summary>
    ''' 測定値を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Value As Decimal = Decimal.Zero

    ''' <summary>
    ''' 同世代健診値比較を取得または設定します（-3.0000～3.0000）。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Comparison As Decimal = Decimal.MinValue

    ''' <summary>
    ''' 劣勢項目かを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IsRedCode As Boolean = False

    <Obsolete()>
    Public Property Label As String = String.Empty

    <Obsolete()>
    Public Property SortOrder As Integer = Integer.MinValue

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="HealthAgeValueItem" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class
