using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// データチャージのイベントIDを表します。
    /// このクラスは継承できません。
    /// </summary>
    [Serializable()]
    public sealed class DatachargeEventIdItem
    {
        #region Public Property

        /// <summary>
        /// イベントIDを取得または設定します。
        /// </summary>
        public string EventId { get; set; } = string.Empty;

        /// <summary>
        /// データサイズを取得または設定します。
        /// </summary>
        public int Size { get; set; } = int.MinValue;

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
        /// <see cref="DatachargeEventIdItem" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public DatachargeEventIdItem()
        {
        }

        #endregion
    }
}