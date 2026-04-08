using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// au WALLET の交換マスタを表します。
    /// このクラスは継承できません。
    /// </summary>
    [Serializable()]
    public sealed class AuWalletPointItem
    {
        #region Public Property

        /// <summary>
        /// イベントIDを取得または設定します。
        /// </summary>
        public string AuWalletPointItemId { get; set; } = string.Empty;

        // /// <summary>
        // /// 交換対象のポイント数を取得または設定します。
        // /// </summary>
        // public int Size { get; set; } = int.MinValue;

        /// <summary>
        /// 消費ポイントを取得または設定します。
        /// </summary>
        public int Point { get; set; } = int.MinValue;

        /// <summary>
        /// 表示名を取得または設定します。
        /// </summary>
        public string DispName { get; set; } = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="AuWalletPointItem" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public AuWalletPointItem()
        {
        }

        #endregion
    }
}