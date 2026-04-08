using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// Amazonギフト券の交換の履歴を表します。
    /// このクラスは継承できません。
    /// </summary>
    [Serializable]
    public sealed class AmazonGiftCardHistItem
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
        public byte GiftCardType { get; set; } = byte.MinValue;

        /// <summary>
        /// クーポンコードを取得または設定します。
        /// </summary>
        public string GiftCardId { get; set; } = string.Empty;

        /// <summary>
        /// 表示名を取得または設定します。
        /// </summary>
        public string GiftCardName { get; set; } = string.Empty;

        /// <summary>
        /// 消費ポイントを取得または設定します。
        /// </summary>
        public int DemandPoint { get; set; } = int.MinValue;

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="AmazonGiftCardHistItem" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public AmazonGiftCardHistItem()
        {
        }

        #endregion
    }
}
