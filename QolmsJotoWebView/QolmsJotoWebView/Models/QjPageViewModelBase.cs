using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// JOTO WebView で使用する、
    /// 画面ビューモデルの基本クラス
    /// </summary>
    [Serializable()]
    public abstract class QjPageViewModelBase
    {
        #region "Variable"

        /// <summary>
        /// 画面番号を保持します
        /// </summary>
        private QjPageNoTypeEnum _pageNo { get; set; } = QjPageNoTypeEnum.None;

        /// <summary>
        /// セッション内での画面の表示回数を保持します。
        /// </summary>
        private int _pageViewCount { get; set; } = 0;

        /// <summary>
        /// JavaScript が有効かを保持します。
        /// </summary>
        private bool _enableJavaScript { get; set; } = false;

        /// <summary>
        /// クッキーが有効かを保持します。
        /// </summary>
        private bool _enableCookies { get; set; } = false;

        /// <summary>
        /// デバッグ ビルドかを保持します。
        /// </summary>
        private bool _isDebug { get; set; } = false;

        /// <summary>
        /// API 認証キーを保持します。
        /// </summary>
        private Guid _apiAuthorizeKey { get; set; } = Guid.Empty;

        /// <summary>
        /// API 認証有効期限を保持します。
        /// </summary>
        private DateTime _apiAuthorizeExpires { get; set; } = DateTime.MinValue;

        #endregion

        #region "Public Property"

        /// <summary>
        /// 画面番号を取得します。
        /// </summary>
        public QjPageNoTypeEnum PageNo { get => this._pageNo;}

        /// <summary>
        /// セッション内での画面の表示回数を取得します。
        /// </summary>
        public int PageViewCount { get => this._pageViewCount; }

        /// <summary>
        /// 自動ログインかを取得または設定します。
        /// </summary>
        public bool IsAutoLogin { get; set; } = false;

        /// <summary>
        /// ユーザー ID を取得または設定します。
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// パスワード ハッシュを取得または設定します。
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// 所有者のアカウント キーを取得または設定します。
        /// </summary>
        public Guid AuthorKey { get; set; } = Guid.Empty;

        /// <summary>
        /// 所有者 アカウント キー ハッシュ を取得または設定します。
        /// </summary>
        public string AuthorKeyHash { get; set; } = string.Empty;

        /// <summary>
        /// 所有者の姓名を取得または設定します。
        /// </summary>
        public string AuthorName { get; set; } = string.Empty;

        /// <summary>
        /// 所有者の性別の種別を取得または設定します。
        /// </summary>
        public QjSexTypeEnum AuthorSex { get; set; } = QjSexTypeEnum.None;

        /// <summary>
        /// 所有者の生年月日を取得または設定します。
        /// </summary>
        public DateTime AuthorBirthday { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 会員の種別を取得または設定します。
        /// </summary>
        public QjMemberShipTypeEnum MembershipType { get; set; } = QjMemberShipTypeEnum.None;

        /// <summary>
        /// ビュー内に展開する暗号化された所有者のアカウント キーを取得または設定します。
        /// </summary>
        public string EncryptedAuthorKey { get; set; } = string.Empty;

        /// <summary>
        /// ログイン カウントを取得または設定します。
        /// </summary>
        public int LoginCount { get; set; } = int.MinValue;

        /// <summary>
        /// ログイン日時を取得または設定します。
        /// </summary>
        public DateTime LoginAt { get; set; } = DateTime.MinValue;

        /// <summary>
        /// JavaScript が有効かを取得します。
        /// </summary>
        private bool EnableJavaScript { get => this._enableJavaScript; }

        /// <summary>
        /// クッキーが有効かを取得します。
        /// </summary>
        private bool EnableCookies { get => this._enableCookies; }

        /// <summary>
        /// デバッグ ビルドかを取得します。
        /// </summary>
        private bool IsDebug { get => this._isDebug; }

        /// <summary>
        /// API 認証キーを取得します。
        /// </summary>
        private Guid ApiAuthorizeKey { get => this._apiAuthorizeKey; }

        /// <summary>
        /// API 認証有効期限を取得します。
        /// </summary>
        private DateTime ApiAuthorizeExpires { get => this._apiAuthorizeExpires; }

        #endregion

        #region "Constructor"

        /// <summary>
        /// 値を指定して、クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="mainModel">メイン モデル</param>
        /// <param name="pageNo">画面番号の種別</param>
        protected QjPageViewModelBase(QolmsJotoModel mainModel, QjPageNoTypeEnum pageNo) : base() 
        {
            if (mainModel == null)
            {
                throw new ArgumentNullException("mainModel", "メイン モデルが Null 参照です。");
            }

            this._pageNo = pageNo;

            this.IsAutoLogin = mainModel.AuthorAccount.IsAutoLogin;
            this.UserId = mainModel.AuthorAccount.UserId;
            this.PasswordHash = mainModel.AuthorAccount.PasswordHash;

            this.AuthorKey = mainModel.AuthorAccount.AccountKey;
            this.AuthorKeyHash = mainModel.AuthorAccount.AccountKeyHash;
            this.AuthorName = mainModel.AuthorAccount.Name;
            this.AuthorSex = mainModel.AuthorAccount.SexType;
            this.AuthorBirthday = mainModel.AuthorAccount.Birthday;
            this.MembershipType = mainModel.AuthorAccount.MembershipType;
            this.EncryptedAuthorKey = mainModel.AuthorAccount.EncryptedAccountKey;

            this.LoginCount = mainModel.AuthorAccount.LoginCount;
            this.LoginAt = mainModel.AuthorAccount.LoginAt;

            this._pageViewCount = mainModel.GetPageViewCount(this._pageNo);

            this._enableJavaScript = mainModel.EnableJavaScript;
            this._enableCookies = mainModel.EnableCookies;
            this._isDebug = mainModel.IsDebug;
            this._apiAuthorizeKey = mainModel.ApiAuthorizeKey;
            this._apiAuthorizeExpires = mainModel.ApiAuthorizeExpires;
        }

        //検証用
        protected QjPageViewModelBase() : base() { }

        #endregion

    }
}