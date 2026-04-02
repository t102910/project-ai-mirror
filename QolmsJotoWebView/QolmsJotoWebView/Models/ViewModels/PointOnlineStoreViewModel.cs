using System;
using System.Collections.Generic;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 「オンラインストアポイント交換」画面ビュー モデルを表します。
    /// このクラスは継承できません。
    /// </summary>
    [Serializable()]
    public sealed class PointOnlineStoreViewModel : QjPointPageViewModelBase
    {
        #region Public Property

        /// <summary>
        /// 現在のポイントを取得または設定します。
        /// </summary>
        public int FromPageNoType { get; set; } = int.MinValue;

        /// <summary>
        /// 交換対象クーポンのリストを取得または設定します。
        /// </summary>
        public List<CouponItem> CouponN { get; set; } = new List<CouponItem>();

        /// <summary>
        /// 保持ポイントを取得または設定します。
        /// </summary>
        public int Point { get; set; } = int.MinValue;

        /// <summary>
        /// ポイント交換履歴のリストを取得または設定します。
        /// </summary>
        public List<PointExchangeHistItem> PointExchangeHistN { get; set; } = new List<PointExchangeHistItem>();

        /// <summary>
        /// ポイント交換の説明文を取得または設定します。
        /// </summary>
        public string Description { get; set; } = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="PointOnlineStoreViewModel" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public PointOnlineStoreViewModel()
            : base()
        {
        }

        /// <summary>
        /// メイン モデルを指定して、
        /// <see cref="PointOnlineStoreViewModel" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="mainModel">メイン モデル。</param>
        public PointOnlineStoreViewModel(QolmsJotoModel mainModel)
            : base(mainModel, QjPageNoTypeEnum.PointOnlineStore)
        {
        }

        #endregion
    }
}