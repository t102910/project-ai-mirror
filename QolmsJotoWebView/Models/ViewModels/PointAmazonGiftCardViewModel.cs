using System;
using System.Collections.Generic;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 「ポイント交換」画面ビュー モデルを表します。
    /// このクラスは継承できません。
    /// </summary>
    [Serializable]
    public sealed class PointAmazonGiftCardViewModel : QjPointPageViewModelBase
    {
        #region Public Property

        /// <summary>
        /// 現在のポイントを取得または設定します。
        /// </summary>
        public int FromPageNoType { get; set; } = int.MinValue;

        /// <summary>
        /// 交換対象Amazonギフト券のリストを取得または設定します。
        /// </summary>
        public List<AmazonGiftCardItem> GiftCardN { get; set; } = new List<AmazonGiftCardItem>();

        /// <summary>
        /// 保持ポイントを取得または設定します。
        /// </summary>
        public int Point { get; set; } = int.MinValue;

        /// <summary>
        /// Amazonギフト券の交換履歴のリストを取得または設定します。
        /// </summary>
        public List<AmazonGiftCardHistItem> GiftCardHistN { get; set; } = new List<AmazonGiftCardHistItem>();

        /// <summary>
        /// ポイント交換の説明文を取得または設定します。
        /// </summary>
        public string Description { get; set; } = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="PointAmazonGiftCardViewModel" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public PointAmazonGiftCardViewModel()
            : base()
        {
        }

        /// <summary>
        /// メイン モデルを指定して、
        /// <see cref="PointAmazonGiftCardViewModel" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="mainModel">メイン モデル。</param>
        public PointAmazonGiftCardViewModel(QolmsJotoModel mainModel) : base(mainModel, QjPageNoTypeEnum.PointAmazonGiftCard) { }

        /// <summary>
        /// 値を指定して、
        /// <see cref="PointAmazonGiftCardViewModel" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="mainModel">メインモデル。</param>
        /// <param name="searchText">検索文字列。</param>
        /// <param name="pageIndex">ページインデックス。</param>
        /// <param name="pageCount">ページ数。</param>
        /// <param name="MedicalInstitutionN">医療機関情報のコレクション。</param>
        public PointAmazonGiftCardViewModel(
            QolmsJotoModel mainModel,
            string searchText,
            int pageIndex,
            int pageCount,
            IEnumerable<MedicalInstitutionItem> MedicalInstitutionN)
            : base(mainModel, QjPageNoTypeEnum.PointAmazonGiftCard)
        {
            //Me.CodeNo = searchText
            //Me.KanaName = pageIndex
            //Me.PageCount = pageCount
            //Me.MedicalInstitutionN = If(MedicalInstitutionN IsNot Nothing AndAlso MedicalInstitutionN.Any(), MedicalInstitutionN.ToList(), New List(Of MedicalInstitutionItem))
        }

        #endregion
    }
}
