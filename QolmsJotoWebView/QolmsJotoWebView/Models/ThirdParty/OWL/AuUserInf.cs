namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// KDDIのAPESから取得できるユーザ情報のうち、新規登録に必要な情報を表します。
    /// このクラスは継承できません。
    /// </summary>
    public sealed class AuUserInf
    {
        #region Public Property

        /// <summary>
        /// 漢字姓を取得または設定します。
        /// </summary>
        public string FamilyName { get; set; } = string.Empty;

        /// <summary>
        /// 漢字名を取得または設定します。
        /// </summary>
        public string GivenName { get; set; } = string.Empty;

        /// <summary>
        /// カナ姓を取得または設定します。
        /// </summary>
        public string FamilyKanaName { get; set; } = string.Empty;

        /// <summary>
        /// カナ名を取得または設定します。
        /// </summary>
        public string GivenKanaName { get; set; } = string.Empty;

        /// <summary>
        /// 性別（M|F）を取得または設定します。
        /// </summary>
        public QjSexTypeEnum Sex { get; set; } = QjSexTypeEnum.None;

        /// <summary>
        /// 生年月日の年を取得または設定します。
        /// </summary>
        public string BirthYear { get; set; } = string.Empty;

        /// <summary>
        /// 生年月日の月を取得または設定します。
        /// </summary>
        public string BirthMonth { get; set; } = string.Empty;

        /// <summary>
        /// 生年月日の日を取得または設定します。
        /// </summary>
        public string BirthDay { get; set; } = string.Empty;

        /// <summary>
        /// メールアドレスを取得または設定します。
        /// </summary>
        public string MailAddress { get; set; } = string.Empty;

        #endregion
    }

}