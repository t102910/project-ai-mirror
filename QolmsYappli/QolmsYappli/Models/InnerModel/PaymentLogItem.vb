''' <summary>
''' 支払い（定期課金）ログ情報を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class PaymentLogItem

#Region "Public Property"

    ''' <summary>
    ''' 課金年を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PaymentYear As Integer = Integer.MinValue

    ''' <summary>
    ''' 課金月を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PaymentMonth As Integer = Integer.MinValue

    ''' <summary>
    ''' 課金日時を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PaymentDate As Date = Date.MinValue

    ''' <summary>
    ''' 課金の種別を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PaymentType As Byte = Byte.MinValue

    ''' <summary>
    ''' 課金額を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Amount As Decimal = Decimal.MinValue

    ''' <summary>
    ''' 課金結果コードを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property StatusCode As String = String.Empty

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PaymentLogItem" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

#End Region

End Class
