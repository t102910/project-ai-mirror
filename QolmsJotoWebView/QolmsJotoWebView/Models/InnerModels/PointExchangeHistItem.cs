using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// ポイント交換の履歴を表します。
    /// このクラスは継承できません。
    /// </summary>
    [Serializable()]
    public sealed class PointExchangeHistItem
    {
        #region Public Property

        /// <summary>
        /// 交換日時を取得または設定します。
        /// </summary>
        public DateTime IssueDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 有効期限を取得または設定します。
        /// </summary>
        public DateTime ExpirationDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// クーポン種別を取得または設定します。
        /// </summary>
        public byte CouponType { get; set; } = byte.MinValue;

        /// <summary>
        /// クーポンコードを取得または設定します。
        /// </summary>
        public string CouponId { get; set; } = string.Empty;

        /// <summary>
        /// 表示名を取得または設定します。
        /// </summary>
        public string DispName { get; set; } = string.Empty;

        /// <summary>
        /// 消費ポイントを取得または設定します。
        /// </summary>
        public int Point { get; set; } = int.MinValue;

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="PointExchangeHistItem" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public PointExchangeHistItem()
        {
        }

        #endregion
    }
}
