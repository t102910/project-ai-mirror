using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// au WALLET の交換履歴を表します。
    /// このクラスは継承できません。
    /// </summary>
    [Serializable()]
    public sealed class AuWalletPointHistItem
    {
        #region Public Property

        /// <summary>
        /// 操作日時を取得または設定します。
        /// </summary>
        public DateTime ActionDate { get; set; } = DateTime.MinValue;

        // /// <summary>
        // /// データサイズを取得または設定します。
        // /// </summary>
        // public int Size { get; set; } = int.MinValue;

        /// <summary>
        /// ポイントを取得または設定します。
        /// </summary>
        public int Point { get; set; } = int.MinValue;

        /// <summary>
        /// 表示名を取得または設定します。
        /// </summary>
        public string DispName { get; set; } = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="AuWalletPointHistItem" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public AuWalletPointHistItem()
        {
        }

        #endregion
    }
}