using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// JOTO ポイント ログ情報を表します。
    /// このクラスは継承できません。
    /// </summary>
    public sealed class JotoPointLogItem
    {
        #region "Public Property"

        /// <summary>
        /// 操作日時を取得または設定します。
        /// </summary>
        public DateTime ActionDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 対象日時を取得または設定します。
        /// </summary>
        public DateTime TargetDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// ポイント項目番号を取得または設定します。
        /// </summary>
        public int ItemNo { get; set; } = int.MinValue;

        /// <summary>
        /// ポイント項目名を取得または設定します。
        /// </summary>
        public string ItemName { get; set; } = string.Empty;

        /// <summary>
        /// ポイント項目番号を取得または設定します。
        /// </summary>
        public int Point { get; set; } = int.MinValue;

        /// <summary>
        /// 付与理由を取得または設定します。
        /// </summary>
        public string Reason { get; set; } = string.Empty;

        /// <summary>
        /// 有効期限を取得または設定します。
        /// </summary>
        public DateTime ExpirationDate { get; set; } = DateTime.MinValue;

        #endregion

        #region "Constructor"
        public JotoPointLogItem() : base() { }

        #endregion

    }
}