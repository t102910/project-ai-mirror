using System;
using System.Collections.Generic;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 「au WALLETポイント交換」画面ビュー モデルを表します。
    /// このクラスは継承できません。
    /// </summary>
    [Serializable()]
    public sealed class PortalAuWalletPointExchangeViewModel : QjPageViewModelBase
    {
        #region Public Property

        /// <summary>
        /// 現在のポイントを取得または設定します。
        /// </summary>
        public int FromPageNoType { get; set; } = int.MinValue;

        /// <summary>
        /// ポイント交換対象のリストを取得または設定します。
        /// </summary>
        public List<AuWalletPointItem> AuWalletPointItemN { get; set; } = new List<AuWalletPointItem>();

        /// <summary>
        /// 保持ポイントを取得または設定します。
        /// </summary>
        public int Point { get; set; } = int.MinValue;

        /// <summary>
        /// ポイント交換履歴のリストを取得または設定します。
        /// </summary>
        public List<AuWalletPointHistItem> AuWalletPointHistN { get; set; } = new List<AuWalletPointHistItem>();

        /// <summary>
        /// ポイント交換説明文を取得または設定します。
        /// </summary>
        public string Description { get; set; } = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="PortalAuWalletPointExchangeViewModel" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public PortalAuWalletPointExchangeViewModel()
            : base()
        {
        }

        /// <summary>
        /// メイン モデルを指定して、
        /// <see cref="PortalAuWalletPointExchangeViewModel" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="mainModel">メイン モデル。</param>
        public PortalAuWalletPointExchangeViewModel(QolmsJotoModel mainModel)
            : base(mainModel, QjPageNoTypeEnum.PointPonta)
        {
        }

        #endregion
    }
}