''' <summary>
''' バイタル情報を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class VitalValueItem

#Region "Public Property"

    ''' <summary>
    ''' 測定日時を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RecordDate As Date = Date.MinValue

    ''' <summary>
    ''' バイタル情報の種別を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property VitalType As QyVitalTypeEnum = QyVitalTypeEnum.None

    ''' <summary>
    ''' 測定値 1 を取得または設定します。
    ''' 血圧の場合は最高血圧を表します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Value1 As Decimal = Decimal.Zero

    ''' <summary>
    ''' 測定値 2 を取得または設定します。
    ''' 血圧の場合は最低血圧を表します。
    ''' 体重の場合は対となる身長を表します（BMI 算出用）。
    ''' それ以外では使用しません。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Value2 As Decimal = Decimal.Zero

    ''' <summary>
    ''' 測定条件を取得または設定します。
    ''' 血糖値の場合は測定タイミングを表します。
    ''' それ以外では使用しません。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    Public Property ConditionType As QyVitalConditionTypeEnum = QyVitalConditionTypeEnum.None

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="VitalValueItem" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class
