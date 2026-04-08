using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 所有者アカウント情報を表します。
    /// このクラスは継承できません。
    /// </summary>
    [Serializable()]
    public sealed class AuthorAccountItem: QjAccountItemBase
    {
        #region "Public Property"

        /// <summary>
        /// 自動ログインかを取得または設定します。
        /// </summary>
        public bool IsAutoLogin { get; set; } = false;

        /// <summary>
        /// ユーザー ID を取得または設定します。OpenIDログインの場合は、ログインユーザIDがないためランダム生成値を設定します。ログイン以外で使用、変更しないでください。
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// パスワード ハッシュを取得または設定します。
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// ログイン カウントを取得または設定します。
        /// </summary>
        public int LoginCount { get; set; } = int.MinValue;

        /// <summary>
        /// ログイン日時を取得または設定します。
        /// </summary>
        public DateTime LoginAt { get; set; } = DateTime.MinValue;

        /// <summary>
        ///  WowIDログインかどうかを取得または設定します。あくまでどちらのボタンを押したかにすぎないため、正確な判定を求められる処理においてはApesから属性情報を取得するようにしてください。
        /// </summary>
        public bool LoginByWowId { get; set; } = false;

        /// <summary>
        /// OpenIDを取得または設定します。
        /// </summary>
        public string OpenId { get; set; } = string.Empty;

        /// <summary>
        /// 会員の種別を取得または設定します。
        /// </summary>
        public QjMemberShipTypeEnum MembershipType { get; set; } = QjMemberShipTypeEnum.None;

        #endregion

        #region "Constructor"

        public AuthorAccountItem() : base() { }

        #endregion

    }
}