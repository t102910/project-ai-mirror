<Serializable()>
Public NotInheritable Class PremiumHistoryViewModel
    Inherits QyPageViewModelBase

#Region "Constant"

    Private Shared ReadOnly methodStrings As New Dictionary(Of Byte, String) From {
        {1, "auかんたん決済"},
        {2, "クレジットカード"}
    }

    Private Shared ReadOnly successCodeHash As New HashSet(Of String)(
        {
            "MPL01000",
            "200"
        }
    )

#End Region

#Region "Public Property"

    Public Property PaymentLogN As New List(Of PaymentLogItem)()

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="PremiumHistoryViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

    ''' <summary>
    ''' メイン モデルを指定して、
    ''' <see cref="PremiumHistoryViewModel" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="items">支払い（定期課金）ログ情報のコレクション。</param>
    ''' <remarks></remarks>
    Public Sub New(mainModel As QolmsYappliModel, items As IEnumerable(Of PaymentLogItem))

        MyBase.New(mainModel, QyPageNoTypeEnum.PremiumHistory)

        Me.InitializeBy(items)

    End Sub

#End Region

#Region "Private Method"

    Private Sub InitializeBy(items As IEnumerable(Of PaymentLogItem))

        If items IsNot Nothing AndAlso items.Any() Then Me.PaymentLogN = items.ToList()

    End Sub

#End Region

#Region "Public Method"

    Public Function ToDowngradeMessage(item As PaymentLogItem) As String

        Dim sb As New StringBuilder()

        If item.PaymentDate = Date.MinValue Then
            sb.Append(New Date(item.PaymentYear, item.PaymentMonth, 1).ToString("yyyy年M月d日(ddd)"))
        Else
            sb.Append(item.PaymentDate.ToString("yyyy年M月d日(ddd)"))
        End If

        sb.Append("の課金が出来なかった為、会員ステータスを変更しました。")
        sb.Append("<br/>再度プレミアム会員登録をご希望の場合は<a href='../Premium/MethodChange'>こちら</a>。")

        If item.PaymentType = 1 Then sb.AppendFormat("<br/>(エラーコード：{0})", item.StatusCode)

        Return sb.ToString()

    End Function

    Public Function ToDateString(item As PaymentLogItem) As String

        If item.PaymentDate = Date.MinValue Then
            Return New Date(item.PaymentYear, item.PaymentMonth, 1).ToString("yyyy年<br/>M月d日(ddd)")
        Else
            Return item.PaymentDate.ToString("yyyy年<br/>M月d日(ddd)")
        End If

    End Function

    Public Function ToMethodString(item As PaymentLogItem) As String

        If PremiumHistoryViewModel.methodStrings.ContainsKey(item.PaymentType) Then
            Return PremiumHistoryViewModel.methodStrings(item.PaymentType)
        Else
            Return String.Empty
        End If

    End Function

    Public Function IsPaid(item As PaymentLogItem) As Boolean

        Return PremiumHistoryViewModel.successCodeHash.Contains(item.StatusCode)

    End Function

#End Region

End Class
