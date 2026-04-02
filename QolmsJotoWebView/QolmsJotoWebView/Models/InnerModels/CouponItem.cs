using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// クーポン情報を表します。
    /// このクラスは継承できません。
    /// </summary>
    [Serializable()]
    public sealed class CouponItem
    {
        #region Public Property

        /// <summary>
        /// クーポン種別を取得または設定します。
        /// </summary>
        public byte CouponType { get; set; } = byte.MinValue;

        /// <summary>
        /// 消費ポイントを取得または設定します。
        /// </summary>
        public int Point { get; set; } = int.MinValue;

        /// <summary>
        /// 表示名を取得または設定します。
        /// </summary>
        public string DispName { get; set; } = string.Empty;

        /// <summary>
        /// 残り枚数のカウントを取得または設定します。
        /// </summary>
        public int RestCount { get; set; } = int.MinValue;

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="CouponItem" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public CouponItem()
        {
        }

        #endregion
    }
}
