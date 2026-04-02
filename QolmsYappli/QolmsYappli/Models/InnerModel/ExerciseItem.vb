''' <summary>
''' 運動の１項目の情報を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class ExerciseItem

#Region "Public Property"

    ''' <summary>
    ''' 運動日時を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ExerciseDate As Date = Date.MinValue

    ''' <summary>
    ''' 運動種別を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ExerciseType As String = String.Empty

    ''' <summary>
    ''' 区分内連番を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Sequence As Integer = Integer.MinValue

    ''' <summary>
    ''' 運動名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ExerciseName As String = String.Empty

    ''' <summary>
    ''' カロリーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Calorie As Integer = Integer.MinValue

    ''' <summary>
    ''' 写真キーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PhotoKey As String = String.Empty

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="ExerciseItem" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class