using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// Amazonギフト券情報を表します。
    /// このクラスは継承できません。
    /// </summary>
    [Serializable]
    public sealed class AmazonGiftCardItem
    {
        #region Public Property

        /// <summary>
        /// クーポン種別を取得または設定します。
        /// </summary>
        public byte GiftCardType { get; set; } = byte.MinValue;

        /// <summary>
        /// 消費ポイントを取得または設定します。
        /// </summary>
        public int DemandPoint { get; set; } = int.MinValue;

        /// <summary>
        /// 表示名を取得または設定します。
        /// </summary>
        public string GiftCardName { get; set; } = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="AmazonGiftCardItem" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public AmazonGiftCardItem()
        {
        }

        #endregion
    }
}
