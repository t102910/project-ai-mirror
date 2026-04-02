''' <summary>
''' バイタル情報インプット モデルを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class VitalValueInputModel

#Region "Public Property"

    ''' <summary>
    ''' 値を保持しているかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Obsolete("検討中")>
    Public Property IsEmpty As Boolean = True

    ''' <summary>
    ''' バイタル情報の種別を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property VitalType As QyVitalTypeEnum = QyVitalTypeEnum.None

    ''' <summary>
    ''' AM / PM を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Meridiem As String = String.Empty

    ''' <summary>
    ''' 時を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Hour As String = String.Empty

    ''' <summary>
    ''' 分を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Minute As String = String.Empty

    ''' <summary>
    ''' 測定値 1 を取得または設定します。
    ''' 血圧の場合は最高血圧を表します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Value1 As String = String.Empty

    ''' <summary>
    ''' 測定値 2 を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Value2 As String = String.Empty

    ''' <summary>
    ''' 測定タイミングを取得または設定します。
    ''' 血糖値以外では使用しません。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    Public Property ConditionType As String = String.Empty

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="VitalValueInputModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class
