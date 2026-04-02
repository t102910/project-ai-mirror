''' <summary>
''' クレジットカードの情報を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class CardItem

#Region "Public Property"

    ''' <summary>
    ''' カードブランドを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Brand As String = String.Empty

    ''' <summary>
    ''' カード番号下４桁を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Last4 As String = String.Empty

    ''' <summary>
    ''' 有効期限（年）を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ExpYear As Integer = Integer.MinValue

    ''' <summary>
    ''' 有効期限（月）を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ExpMonth As Integer = Integer.MinValue


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