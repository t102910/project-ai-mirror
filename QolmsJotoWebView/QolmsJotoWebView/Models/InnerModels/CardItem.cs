using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// クレジットカードの情報を表します。
    /// このクラスは継承できません。
    /// </summary>
    [Serializable()]
    public sealed class CardItem
    {
        #region Public Property

        /// <summary>
        /// カードブランドを取得または設定します。
        /// </summary>
        public string Brand { get; set; } = string.Empty;

        /// <summary>
        /// カード番号下４桁を取得または設定します。
        /// </summary>
        public string Last4 { get; set; } = string.Empty;

        /// <summary>
        /// 有効期限（年）を取得または設定します。
        /// </summary>
        public int ExpYear { get; set; } = int.MinValue;

        /// <summary>
        /// 有効期限（月）を取得または設定します。
        /// </summary>
        public int ExpMonth { get; set; } = int.MinValue;

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="CardItem" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public CardItem()
        {
        }

        #endregion
    }
}
