''' <summary>
''' 所有者アカウント情報を表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public NotInheritable Class AuthorAccountItem
    Inherits QyAccountItemBase

#Region "Public Property"

    ''' <summary>
    ''' 自動ログインかを取得または設定します。
    ''' </summary>
    ''' <remarks></remarks>
    <Obsolete>
    Public Property IsAutoLogin As Boolean = False

    ''' <summary>
    ''' ユーザー ID を取得または設定します。OpenIDログインの場合は、ログインユーザIDがないためランダム生成値を設定します。ログイン以外で使用、変更しないでください。
    ''' </summary>
    ''' <remarks></remarks>
    Public Property UserId As String = String.Empty

    ''' <summary>
    ''' パスワード ハッシュを取得または設定します。
    ''' </summary>
    ''' <remarks></remarks>
    <Obsolete>
    Public Property PasswordHash As String = String.Empty

    ''' <summary>
    ''' ログイン カウントを取得または設定します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Property LoginCount As Integer = Integer.MinValue

    ''' <summary>
    ''' ログイン日時を取得または設定します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Property LoginAt As Date = Date.MinValue

    ''' <summary>
    ''' WowIDログインかどうかを取得または設定します。あくまでどちらのボタンを押したかにすぎないため、正確な判定を求められる処理においてはApesから属性情報を取得するようにしてください。
    ''' </summary>
    ''' <remarks></remarks>
    Public Property LoginByWowId As Boolean = False

    ''' <summary>
    ''' OpenIDを取得または設定します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Property OpenId As String = String.Empty

    ''' <summary>
    ''' OpenID種別を取得または設定します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Property OpenIdType As Byte = Byte.MinValue
    ' ''' <summary>
    ' ''' 会員レベルを取得または設定します。
    ' ''' </summary>
    ' ''' <remarks></remarks>
    '<Obsolete("廃止予定。MembershipType を使用してください。")>
    'Public Property MemberShipLevel As QyMemberShipTypeEnum = QyMemberShipTypeEnum.None

    ''' <summary>
    ''' 会員の種別を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MembershipType As QyMemberShipTypeEnum = QyMemberShipTypeEnum.None

    ' ''' <summary>
    ' ''' 会員有効期限を取得または設定します。
    ' ''' </summary>
    ' ''' <remarks></remarks>
    'Public Property MembershipExpirationDate As Date = Date.MaxValue

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="AuthorAccountItem" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New()

    End Sub

#End Region

End Class
