''' <summary>
''' クレジットカードの情報を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class ChallengeInputItem

#Region "Public Property"

    ''' <summary>
    ''' 必須入力項目を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RequiredN As New List(Of ChallengeInputValueItem)

    ''' <summary>
    ''' 任意入力項目を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property OptionalN As New List(Of ChallengeInputValueItem)

    ''' <summary>
    ''' パスコードを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PassCode As String = String.Empty

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="CardItem" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class
