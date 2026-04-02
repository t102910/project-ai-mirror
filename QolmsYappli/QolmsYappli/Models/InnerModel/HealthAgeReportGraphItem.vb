<Serializable()>
Public NotInheritable Class HealthAgeReportGraphItem

#Region "Public Property"

    Public Property Title As String = String.Empty

    Public Property AxisMin As Decimal = Decimal.Zero

    Public Property AxisMax As Decimal = Decimal.Zero

    Public Property Label As String = "[]"

    Public Property TargetValue As String = "[]"

    Public Property Data As String = "[]"

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="HealthAgeReportGraphItem" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class
