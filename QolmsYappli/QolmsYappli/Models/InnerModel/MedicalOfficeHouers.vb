''' <summary>
''' 医療機関の診療時間情報を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class MedicalOfficeHouers

#Region "Public Property"
    ''' <summary>
    ''' 病院コードを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CodeNo As Integer = Integer.MinValue
    ''' <summary>
    ''' 曜日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DayOfWeek As Integer = Integer.MinValue
    ''' <summary>
    ''' 日付内連番を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AcceptedTimeNo As Integer = Integer.MinValue
    ''' <summary>
    ''' 受付開始時間を取得または設定します。
    ''' 0000～2359
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AcceptedStart As String = String.Empty
    ''' <summary>
    ''' 受付終了時間を取得または設定します。
    ''' 0000～2359
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AcceptedEnd As String = String.Empty

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="MedicalOfficeHouers" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

    ''' <summary>
    ''' <see cref="MedicalOfficeHouers" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(targer As MedicalOfficeHouers)
        Me.New()
        Me.CodeNo = targer.CodeNo
        Me.DayOfWeek = targer.DayOfWeek
        Me.AcceptedTimeNo = targer.AcceptedTimeNo
        Me.AcceptedStart = targer.AcceptedStart
        Me.AcceptedEnd = targer.AcceptedEnd
    End Sub
#End Region

End Class
