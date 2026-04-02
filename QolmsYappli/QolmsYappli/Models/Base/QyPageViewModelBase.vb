Imports System.Collections.ObjectModel

''' <summary>
''' 「コルムス ヤプリ サイト」で使用する、
''' 画面ビュー モデルの基本クラスを表します。
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public MustInherit Class QyPageViewModelBase

#Region "Variable"

    ''' <summary>
    ''' 画面番号を保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Private _pageNo As QyPageNoTypeEnum = QyPageNoTypeEnum.None

    ''' <summary>
    ''' セッション内での画面の表示回数を保持します。
    ''' </summary>
    ''' <remarks></remarks>
    Private _pageViewCount As Integer = 0

    ''' <summary>
    ''' JavaScript が有効かを保持します。
    ''' </summary>
    ''' <remarks></remarks>
    <Obsolete("検証用です。")>
    Private _enableJavaScript As Boolean = False

    ''' <summary>
    ''' クッキーが有効かを保持します。
    ''' </summary>
    ''' <remarks></remarks>
    <Obsolete("検証用です。")>
    Private _enableCookies As Boolean = False

    ''' <summary>
    ''' デバッグ ビルドかを保持します。
    ''' </summary>
    ''' <remarks></remarks>
    <Obsolete("検証用です。")>
    Private _isDebug As Boolean = False

    ''' <summary>
    ''' API 認証キーを保持します。
    ''' </summary>
    ''' <remarks></remarks>
    <Obsolete("検証用です。")>
    Private _apiAuthorizeKey As Guid = Guid.Empty

    ''' <summary>
    ''' API 認証有効期限を保持します。
    ''' </summary>
    ''' <remarks></remarks>
    <Obsolete("検証用です。")>
    Private _apiAuthorizeExpires As Date = Date.MinValue

#End Region

#Region "Public Property"

    ''' <summary>
    ''' 画面番号を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property PageNo As QyPageNoTypeEnum

        Get
            Return Me._pageNo
        End Get

    End Property

    ''' <summary>
    ''' セッション内での画面の表示回数を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property PageViewCount As Integer

        Get
            Return Me._pageViewCount
        End Get

    End Property

    ''' <summary>
    ''' 自動ログインかを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IsAutoLogin As Boolean = False

    ''' <summary>
    ''' ユーザー ID を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property UserId As String = String.Empty

    ''' <summary>
    ''' パスワード ハッシュを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PasswordHash As String = String.Empty

    ''' <summary>
    ''' 所有者のアカウント キーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Obsolete("要検討")>
    Public Property AuthorKey As Guid = Guid.Empty

    ''' <summary>
    ''' 所有者 アカウント キー ハッシュ を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AuthorKeyHash As String = String.Empty

    ''' <summary>
    ''' 所有者の姓名を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Obsolete("要検討")>
    Public Property AuthorName As String = String.Empty

    ''' <summary>
    ''' 所有者の性別の種別を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Obsolete("要検討")>
    Public Property AuthorSex As QySexTypeEnum = QySexTypeEnum.None

    ''' <summary>
    ''' 所有者の生年月日を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AuthorBirthday As Date = Date.MinValue

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
    '<Obsolete("要検討")>
    'Public Property MembershipExpirationDate As Date = Date.MaxValue

    ' ''' <summary>
    ' ''' 所有者の顔写真キーを取得または設定します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    '<Obsolete("要検討")>
    'Public Property AuthorPhotoKey As Guid = Guid.Empty

    ''' <summary>
    ''' ビュー内に展開する暗号化された所有者のアカウント キーを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Obsolete("要検討")>
    Public Property EncryptedAuthorKey As String = String.Empty

    ''' <summary>
    ''' ビュー内に展開する暗号化された所有者の顔写真情報への参照パラメータを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Obsolete("要検討")>
    Public Property EncryptedAuthorPhotoReference As String = String.Empty

    ''' <summary>
    ''' ビュー内に展開する暗号化された所有者の顔写真サムネイル情報への参照パラメータを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Obsolete("要検討")>
    Public Property EncryptedAuthorThumbnailPhotoReference As String = String.Empty

    ''' <summary>
    ''' ログイン カウントを取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LoginCount As Integer = Integer.MinValue

    ''' <summary>
    ''' ログイン日時を取得または設定します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LoginAt As Date = Date.MinValue

    ''' <summary>
    ''' JavaScript が有効かを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Obsolete("検証用です。")>
    Public ReadOnly Property EnableJavaScript As Boolean

        Get
            Return Me._enableJavaScript
        End Get

    End Property

    ''' <summary>
    ''' クッキーが有効かを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Obsolete("検証用です。")>
    Public ReadOnly Property EnableCookies As Boolean

        Get
            Return Me._enableCookies
        End Get

    End Property

    ''' <summary>
    ''' デバッグ ビルドかを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Obsolete("検証用です。")>
    Public ReadOnly Property IsDebug As Boolean

        Get
            Return Me._isDebug
        End Get

    End Property

    ''' <summary>
    ''' API 認証キーを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Obsolete("検証用です。")>
    Public ReadOnly Property ApiAuthorizeKey As Guid

        Get
            Return Me._apiAuthorizeKey
        End Get

    End Property

    ''' <summary>
    ''' API 認証有効期限を取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Obsolete("検証用です。")>
    Public ReadOnly Property ApiAuthorizeExpires As Date

        Get
            Return Me._apiAuthorizeExpires
        End Get

    End Property

#End Region

#Region "Constructor"

    ''' <summary>
    ''' <see cref="QyPageViewModelBase" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub

    ''' <summary>
    ''' 値を指定して、
    ''' <see cref="QyPageViewModelBase" /> クラスの新しいインスタンスを初期化します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="pageNo">画面番号の種別。</param>
    ''' <remarks></remarks>
    Protected Sub New(mainModel As QolmsYappliModel, pageNo As QyPageNoTypeEnum)

        If mainModel Is Nothing Then Throw New ArgumentNullException("mainModel", "メイン モデルが Null 参照です。")

        Me._pageNo = pageNo

        With mainModel
            Me.IsAutoLogin = .AuthorAccount.IsAutoLogin
            Me.UserId = .AuthorAccount.UserId
            Me.PasswordHash = .AuthorAccount.PasswordHash

            Me.AuthorKey = .AuthorAccount.AccountKey
            Me.AuthorKeyHash = .AuthorAccount.AccountKeyHash
            Me.AuthorName = .AuthorAccount.Name
            Me.AuthorSex = .AuthorAccount.SexType
            Me.AuthorBirthday = .AuthorAccount.Birthday
            Me.MembershipType = .AuthorAccount.MembershipType
            'Me.MembershipExpirationDate = .AuthorAccount.MembershipExpirationDate
            Me.EncryptedAuthorKey = .AuthorAccount.EncryptedAccountKey

            Me.LoginCount = .AuthorAccount.LoginCount
            Me.LoginAt = .AuthorAccount.LoginAt

            Me._pageViewCount = .GetPageViewCount(Me._pageNo)

            Me._enableJavaScript = .EnableJavaScript
            Me._enableCookies = .EnableCookies
            Me._isDebug = .IsDebug
            Me._apiAuthorizeKey = .ApiAuthorizeKey
            Me._apiAuthorizeExpires = .ApiAuthorizeExpires
        End With

    End Sub

#End Region

End Class
