using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MGF.QOLMS.QolmsJotoWebView
{
    [Serializable]
    public sealed class HealthAgeUrineReportPartialViewModel : QjHealthAgeReportPartialViewModelBase
    {
        #region Constant

        private static readonly Dictionary<decimal, string> valueSetN = new Dictionary<decimal, string>
        {
            { 1m, "－" },
            { 2m, "±" },
            { 3m, "＋" },
            { 4m, "＋＋" },
            { 5m, "＋＋＋" }
        };

        #endregion

        #region Variable

        private List<Tuple<string, string>> _tableData = new List<Tuple<string, string>>();

        #endregion

        #region Public Property

        public ReadOnlyCollection<Tuple<string, string>> TableData
        {
            get { return this._tableData.ToList().AsReadOnly(); }
        }

        #endregion

        #region Constructor

        public HealthAgeUrineReportPartialViewModel(HealthAgeViewModel model, HealthAgeReportItem reportItem)
            : base(model, reportItem)
        {
            this.InitializeBy(reportItem);
        }

        #endregion

        #region Private Method

        private string ToStringValue(decimal value)
        {
            string result = string.Empty;

            if (HealthAgeUrineReportPartialViewModel.valueSetN.ContainsKey(value)) result = HealthAgeUrineReportPartialViewModel.valueSetN[value];

            return result;
        }

        #endregion

        #region Protected Method

        protected override void InitializeBy(HealthAgeReportItem reportItem)
        {
            this._tableData = new List<Tuple<string, string>>();

            if (reportItem != null
                && reportItem.HealthAgeReportType == QjHealthAgeReportTypeEnum.Urine
                && reportItem.HealthAgeValueN != null
                && reportItem.HealthAgeValueN.Any())
            {
                // 尿糖
                List<HealthAgeValueItem> ch037N = reportItem.HealthAgeValueN
                    .Where(i => i.HealthAgeValueType == QjHealthAgeValueTypeEnum.Ch037)
                    .OrderByDescending(i => i.RecordDate)
                    .Take(3)
                    .ToList();

                switch (ch037N.Count)
                {
                    case 2:
                        ch037N.Add(new HealthAgeValueItem { RecordDate = DateTime.MinValue, Value = decimal.MinValue });
                        break;

                    case 1:
                        ch037N.AddRange(new[]
                        {
                            new HealthAgeValueItem { RecordDate = DateTime.MinValue, Value = decimal.MinValue },
                            new HealthAgeValueItem { RecordDate = DateTime.MinValue, Value = decimal.MinValue }
                        });
                        break;

                    case 0:
                        ch037N.AddRange(new[]
                        {
                            new HealthAgeValueItem { RecordDate = DateTime.MinValue, Value = decimal.MinValue },
                            new HealthAgeValueItem { RecordDate = DateTime.MinValue, Value = decimal.MinValue },
                            new HealthAgeValueItem { RecordDate = DateTime.MinValue, Value = decimal.MinValue }
                        });
                        break;
                }

                ch037N.Reverse();

                // 尿蛋白
                List<HealthAgeValueItem> ch039N = reportItem.HealthAgeValueN
                    .Where(i => i.HealthAgeValueType == QjHealthAgeValueTypeEnum.Ch039)
                    .OrderByDescending(i => i.RecordDate)
                    .Take(3)
                    .ToList();

                switch (ch039N.Count)
                {
                    case 2:
                        ch039N.Add(new HealthAgeValueItem { RecordDate = DateTime.MinValue, Value = decimal.MinValue });
                        break;

                    case 1:
                        ch039N.AddRange(new[]
                        {
                            new HealthAgeValueItem { RecordDate = DateTime.MinValue, Value = decimal.MinValue },
                            new HealthAgeValueItem { RecordDate = DateTime.MinValue, Value = decimal.MinValue }
                        });
                        break;

                    case 0:
                        ch039N.AddRange(new[]
                        {
                            new HealthAgeValueItem { RecordDate = DateTime.MinValue, Value = decimal.MinValue },
                            new HealthAgeValueItem { RecordDate = DateTime.MinValue, Value = decimal.MinValue },
                            new HealthAgeValueItem { RecordDate = DateTime.MinValue, Value = decimal.MinValue }
                        });
                        break;
                }

                ch039N.Reverse();

                this._tableData.Add(new Tuple<string, string>(this.ToStringValue(ch037N[0].Value), this.ToStringValue(ch039N[0].Value)));
                this._tableData.Add(new Tuple<string, string>(this.ToStringValue(ch037N[1].Value), this.ToStringValue(ch039N[1].Value)));
                this._tableData.Add(new Tuple<string, string>(this.ToStringValue(ch037N[2].Value), this.ToStringValue(ch039N[2].Value)));
            }
            else
            {
                this._tableData.Add(new Tuple<string, string>(string.Empty, string.Empty));
                this._tableData.Add(new Tuple<string, string>(string.Empty, string.Empty));
                this._tableData.Add(new Tuple<string, string>(string.Empty, string.Empty));
            }
        }

        #endregion
    }
}
