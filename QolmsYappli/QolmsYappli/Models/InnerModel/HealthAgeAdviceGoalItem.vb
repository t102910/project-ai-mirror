<Serializable()>
Public NotInheritable Class HealthAgeAdviceGoalItem

#Region "Public Property"

    ''' <summary>
    ''' 項目名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Name As String = String.Empty

    ''' <summary>
    ''' 測定値を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Old As Decimal = Decimal.MinValue

    ''' <summary>
    ''' 目標値を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Goal As Decimal = Decimal.MinValue

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="HealthAgeAdviceGoalItem" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class
