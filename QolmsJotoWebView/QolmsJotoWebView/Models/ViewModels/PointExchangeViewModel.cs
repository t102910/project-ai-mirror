using System;
using System.Collections.Generic;
using MGF.QOLMS.QolmsJotoWebView;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 「沖縄CLIPマルシェポイント交換」画面ビュー モデルを表します。
    /// このクラスは継承できません。
    /// </summary>
    [Serializable]
    public sealed class PointExchangeViewModel : QjPointPageViewModelBase
    {
        #region Public Property

        ///// <summary>
        ///// 現在のポイントを取得または設定します。
        ///// </summary>
        //public int FromPageNoType { get; set; } = int.MinValue;

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
        /// 新しいインスタンスを初期化します。
        /// </summary>
        public PointExchangeViewModel()
            : base()
        {
        }

        /// <summary>
        /// メインモデルを指定して初期化します。
        /// </summary>
        public PointExchangeViewModel(QolmsJotoModel mainModel)
            : base(mainModel, QjPageNoTypeEnum.None)
        {
        }

        /// <summary>
        /// 値を指定して初期化します。
        /// </summary>
        public PointExchangeViewModel(
            QolmsJotoModel mainModel,
            string searchText,
            int pageIndex,
            int pageCount,
            IEnumerable<MedicalInstitutionItem> MedicalInstitutionN)
            : base(mainModel, QjPageNoTypeEnum.PortalSearchDetail)
        {
            // 必要ならここにプロパティ設定を追加
        }

        #endregion
    }
}
