''' <summary>
''' ガルフスポーツクラブのフィットネス動画の情報を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class MovieItem

#Region "Public Property"

    ''' <summary>
    ''' URLを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Id As String = String.Empty

    ''' <summary>
    ''' 運動登録用マスタ番号を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ExerciseType As Integer = Integer.MinValue

    ''' <summary>
    ''' 動画の説明を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Discription As String = String.Empty

    ''' <summary>
    ''' 一回当たりの運動時間を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Time As String = String.Empty

    ''' <summary>
    ''' 一回当たりの消費カロリーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Calorie As Integer = Integer.MinValue


#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="DeviceItem" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class
