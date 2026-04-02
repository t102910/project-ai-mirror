using System;
using System.Collections.ObjectModel;

namespace MGF.QOLMS.QolmsJotoWebView
{
    [Serializable]
    public abstract class QjHealthAgeReportPartialViewModelBase
        : QjPartialViewModelBase<QjPageViewModelBase>
    {
        #region Variable

        /// <summary>
        /// ビューに展開する展開する健康年齢レポート情報を保持します。
        /// </summary>
        protected HealthAgeReportItem _reportItem = new HealthAgeReportItem(); // TODO:

        // protected List<HealthAgeReportGraphItem> _graphIemN = new List<HealthAgeReportGraphItem>(); // TODO

        #endregion

        #region Public Property

        public HealthAgeReportItem ReportItem
        {
            get { return this._reportItem; }
        }

        // public ReadOnlyCollection<HealthAgeReportGraphItem> GraphIemN
        // {
        //     get { return this._graphIemN.AsReadOnly(); }
        // }

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="QjHealthAgeReportPartialViewModelBase" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        protected QjHealthAgeReportPartialViewModelBase()
            : base()
        {
        }

        protected QjHealthAgeReportPartialViewModelBase(HealthAgeViewModel model, HealthAgeReportItem reportItem)
            : base(model)
        {
            this._reportItem = reportItem;

            // this.InitializeBy(this._reportItem);
        }

        #endregion

        #region Protected Method

        protected abstract void InitializeBy(HealthAgeReportItem reportItem);

        #endregion
    }
}
