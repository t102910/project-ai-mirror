''' <summary>
''' KDDIのかんたん決済、クレジットカード決済のAPIから取得できる情報のうち、新規登録に必要な情報を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Public NotInheritable Class PaymentInf

#Region "Public Property"

    ''' <summary>
    ''' 加盟店管理番号（こちらで発行する最大20桁の英数字）を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MemberManageNo As Long = Long.MinValue

    ''' <summary>
    ''' 継続課金ID（正常登録時に払い出される）を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ContinueAccountId As String = String.Empty

    ''' <summary>
    ''' 課金開始日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ContinueAccountStartDate As Date = Date.MinValue
    ''' <summary>
    ''' 開始日時を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property StartDate As Date = Date.MinValue

    ''' <summary>
    ''' 終了(解約)日時を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property EndDate As Date = Date.MaxValue


    ''' <summary>
    ''' AuSystemIdを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AuId As String = String.Empty

    ''' <summary>
    ''' トランザクションIDを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TransctionId As String = String.Empty

    ''' <summary>
    ''' APIの応答にある、処理結果コードを取得または設定します。　
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>MPL01000が成功コード。それ以外はエラーコード</remarks>
    Public Property AuResultCode As String = String.Empty

    ''' <summary>
    ''' Joto会員レベルを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MemberShipType As Byte = QyMemberShipTypeEnum.None

    ''' <summary>
    ''' 課金種別を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PaymentType As Byte = QyPaymentTypeEnum.None

    ''' <summary>
    ''' 顧客IDを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CustomerId As String = String.Empty

    ''' <summary>
    ''' サブスクリプションIDを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SubscriptionId As String = String.Empty

    ''' <summary>
    ''' 追加セットを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AdditionalSet As String = String.Empty

    ''' <summary>
    ''' を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IsOldPaymentRecordExists As Boolean = False


#End Region

    'ログイン時のOpenIDとして取得するIDはUri形式でその最後の要素だけをAuSystemIDとして利用する
    Private Function GetAuSystemID(ByVal openid As String) As String
        Dim tmp As String() = openid.Split("/"c)
        Return tmp.Last()
    End Function

#Region "Constructor"


#End Region

    Public Sub New(openId As String)
        Me.AuId = GetAuSystemID(openId)
    End Sub
End Class
