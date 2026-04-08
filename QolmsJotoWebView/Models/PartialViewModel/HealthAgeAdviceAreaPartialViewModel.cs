using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MGF.QOLMS.QolmsJotoWebView
{
    [Serializable]
    public sealed class HealthAgeAdviceAreaPartialViewModel : QjPartialViewModelBase<HealthAgeViewModel>
    {
        #region Variable

        private bool _hasData = false;

        private List<HealthAgeAdviceItem> _adviceData = new List<HealthAgeAdviceItem>();

        #endregion

        #region Public Property

        public bool HasData
        {
            get { return this._hasData; }
        }

        public ReadOnlyCollection<HealthAgeAdviceItem> AdviceData
        {
            get { return this._adviceData.ToList().AsReadOnly(); }
        }

        #endregion

        #region Private Method

        private void InitializeBy(IEnumerable<HealthAgeAdviceItem> items)
        {
            this._hasData = false;
            this._adviceData = new List<HealthAgeAdviceItem>();

            if (items != null && items.Any())
            {
                this._hasData = true;
                this._adviceData = items.ToList();
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="HealthAgeAdviceAreaPartialViewModel" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        protected HealthAgeAdviceAreaPartialViewModel()
            : base()
        {
        }

        /// <summary>
        /// <see cref="HealthAgeAdviceAreaPartialViewModel" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public HealthAgeAdviceAreaPartialViewModel(HealthAgeViewModel model)
            : base(model)
        {
            this.InitializeBy(model.HealthAgeAdviceN);
        }

        #endregion
    }
}
