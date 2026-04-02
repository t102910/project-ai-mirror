
''' <summary>
''' チャレンジエントリー入力項目値を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class ChallengeInputValueItem

#Region "Public Property"

    ''' <summary>
    ''' 入力項目のキーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Key As String = String.Empty

    ''' <summary>
    ''' 入力項目のタイトルを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Title As String = String.Empty

    ''' <summary>
    ''' 入力項目の値を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Value As String = String.Empty


#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="ChallengeInputValueItem" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class
