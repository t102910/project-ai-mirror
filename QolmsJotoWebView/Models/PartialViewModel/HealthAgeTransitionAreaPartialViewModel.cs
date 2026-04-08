using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MGF.QOLMS.QolmsJotoWebView
{
    [Serializable]
    public sealed class HealthAgeTransitionAreaPartialViewModel : QjPartialViewModelBase<HealthAgeViewModel>
    {
        #region Variable

        private bool _hasData = false;

        private List<Tuple<string, string>> _ageData = new List<Tuple<string, string>>();

        #endregion

        #region Public Property

        public bool HasData
        {
            get { return this._hasData; }
        }

        public ReadOnlyCollection<Tuple<string, string>> AgeData
        {
            get { return this._ageData.ToList().AsReadOnly(); }
        }

        #endregion

        #region Private Method

        private void InitializeBy(IEnumerable<HealthAgeValueItem> items)
        {
            this._hasData = false;
            this._ageData = new List<Tuple<string, string>>();

            if (items != null)
            {
                List<HealthAgeValueItem> ageN = items
                    .Where(i => i.HealthAgeValueType == QjHealthAgeValueTypeEnum.Calculation)
                    .OrderByDescending(i => i.RecordDate)
                    .Take(3)
                    .ToList();

                switch (ageN.Count)
                {
                    case 2:
                        ageN.Add(new HealthAgeValueItem { RecordDate = DateTime.MinValue, Value = decimal.MinValue });
                        break;

                    case 1:
                        ageN.AddRange(new[]
                        {
                            new HealthAgeValueItem { RecordDate = DateTime.MinValue, Value = decimal.MinValue },
                            new HealthAgeValueItem { RecordDate = DateTime.MinValue, Value = decimal.MinValue }
                        });
                        break;

                    case 0:
                        ageN.AddRange(new[]
                        {
                            new HealthAgeValueItem { RecordDate = DateTime.MinValue, Value = decimal.MinValue },
                            new HealthAgeValueItem { RecordDate = DateTime.MinValue, Value = decimal.MinValue },
                            new HealthAgeValueItem { RecordDate = DateTime.MinValue, Value = decimal.MinValue }
                        });
                        break;
                }

                ageN.Reverse();

                for (int a = 0; a <= 2; a++)
                {
                    if (ageN[a].RecordDate != DateTime.MinValue && ageN[a].Value > decimal.Zero)
                    {
                        this._ageData.Add(new Tuple<string, string>(ageN[a].RecordDate.ToString("yyyy年M月d日"), ageN[a].Value.ToString("0.#")));

                        this._hasData = true;
                    }
                    else
                    {
                        this._ageData.Add(new Tuple<string, string>("－", "－"));
                    }
                }
            }
            else
            {
                this._ageData.Add(new Tuple<string, string>("－", "－"));
                this._ageData.Add(new Tuple<string, string>("－", "－"));
                this._ageData.Add(new Tuple<string, string>("－", "－"));
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="HealthAgeTransitionAreaPartialViewModel" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        protected HealthAgeTransitionAreaPartialViewModel()
            : base()
        {
        }

        /// <summary>
        /// <see cref="HealthAgeTransitionAreaPartialViewModel" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public HealthAgeTransitionAreaPartialViewModel(HealthAgeViewModel model)
            : base(model)
        {
            this.InitializeBy(model.HealthAgeN);
        }

        #endregion
    }
}
