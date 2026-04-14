using System;
using System.Collections.Generic;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 「データチャージ」画面ビュー モデルを表します。
    /// このクラスは継承できません。
    /// </summary>
    [Serializable()]
    public sealed class PointDatachargeViewModel : QjPointPageViewModelBase
    {
        #region Public Property

        /// <summary>
        /// 現在のポイントを取得または設定します。
        /// </summary>
        public int FromPageNoType { get; set; } = int.MinValue;

        /// <summary>
        /// データチャージイベントIDのリストを取得または設定します。
        /// </summary>
        public List<DatachargeEventIdItem> EventIdN { get; set; } = new List<DatachargeEventIdItem>();

        /// <summary>
        /// 保持ポイントを取得または設定します。
        /// </summary>
        public int Point { get; set; } = int.MinValue;

        /// <summary>
        /// データチャージ履歴のリストを取得または設定します。
        /// </summary>
        public List<DatachargeHistItem> DatachargeHistN { get; set; } = new List<DatachargeHistItem>();

        /// <summary>
        /// データチャージ説明文を取得または設定します。
        /// </summary>
        public string Description { get; set; } = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="PointDatachargeViewModel" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public PointDatachargeViewModel()
            : base()
        {
        }

        /// <summary>
        /// メイン モデルを指定して、
        /// <see cref="PointDatachargeViewModel" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="mainModel">メイン モデル。</param>
        public PointDatachargeViewModel(QolmsJotoModel mainModel)
            : base(mainModel, QjPageNoTypeEnum.PointDatacharge)
        {
        }

        /// <summary>
        /// 値を指定して、
        /// <see cref="PointDatachargeViewModel" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="mainModel">メインモデル。</param>
        /// <param name="searchText">検索文字列。</param>
        /// <param name="pageIndex">ページインデックス。</param>
        /// <param name="pageCount">ページ数。</param>
        /// <param name="medicalInstitutionN">医療機関情報のコレクション。</param>
        public PointDatachargeViewModel(
            QolmsJotoModel mainModel,
            string searchText,
            int pageIndex,
            int pageCount,
            IEnumerable<MedicalInstitutionItem> medicalInstitutionN)
            : base(mainModel, QjPageNoTypeEnum.PointDatacharge)
        {
            // this.CodeNo = searchText;
            // this.KanaName = pageIndex;
            // this.PageCount = pageCount;
            // this.MedicalInstitutionN = medicalInstitutionN != null && medicalInstitutionN.Any()
            //     ? medicalInstitutionN.ToList()
            //     : new List<MedicalInstitutionItem>();
        }

        #endregion
    }
}