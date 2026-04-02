<Serializable()>
Public NotInheritable Class HealthAgeAdviceItem

#Region "Public Property"

    ''' <summary>
    ''' 表題を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Title As String = String.Empty

    ''' <summary>
    ''' 基本アドバイスを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Based As String = String.Empty

    ''' <summary>
    ''' 詳細アドバイスのリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DetailN As New List(Of String)()

    ''' <summary>
    ''' 目標項目のリストを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property GoalN As New List(Of HealthAgeAdviceGoalItem)()

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="HealthAgeAdviceItem" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class
