using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MGF.QOLMS.QolmsJotoWebView
{
    [Serializable]
    public sealed class HealthAgeDistributionReportPartialViewModel : QjHealthAgeReportPartialViewModelBase
    {
        #region Constant

        private static readonly List<Tuple<decimal, decimal, string>> ageRanges = new List<Tuple<decimal, decimal, string>>
        {
            new Tuple<decimal, decimal, string>(decimal.MinValue, 20m, "'～20歳'"),
            new Tuple<decimal, decimal, string>(20m, 25m, "'25歳'"),
            new Tuple<decimal, decimal, string>(25m, 30m, "'30歳'"),
            new Tuple<decimal, decimal, string>(30m, 35m, "'35歳'"),
            new Tuple<decimal, decimal, string>(35m, 40m, "'40歳'"),
            new Tuple<decimal, decimal, string>(40m, 45m, "'45歳'"),
            new Tuple<decimal, decimal, string>(45m, 50m, "'50歳'"),
            new Tuple<decimal, decimal, string>(50m, 55m, "'55歳'"),
            new Tuple<decimal, decimal, string>(55m, 60m, "'60歳'"),
            new Tuple<decimal, decimal, string>(60m, 65m, "'65歳'"),
            new Tuple<decimal, decimal, string>(65m, 70m, "'70歳'"),
            new Tuple<decimal, decimal, string>(70m, decimal.MaxValue, "'70歳～'")
        };

        #endregion

        #region Variable

        private List<decimal> _graphData = new List<decimal>();

        private HealthAgeReportGraphItem _graphSetting = new HealthAgeReportGraphItem();

        private decimal _healthAge = decimal.MinValue;

        #endregion

        #region Public Property

        public HealthAgeReportGraphItem AgeDistributionGraphSetting
        {
            get { return this._graphSetting; }
        }

        #endregion

        #region Constructor

        public HealthAgeDistributionReportPartialViewModel(HealthAgeViewModel model, HealthAgeReportItem reportItem)
            : base(model, reportItem)
        {
            this._healthAge = model.HealthAge; // TODO:

            this.InitializeBy(reportItem);
        }

        #endregion

        #region Private Method

        // 同世代健康年齢分布
        private void SetBarGraphSetting(HealthAgeReportItem reportItem)
        {
            if (reportItem != null
                && reportItem.HealthAgeReportType == QjHealthAgeReportTypeEnum.Distribution
                && reportItem.HealthAgeValueN != null
                && reportItem.HealthAgeValueN.Any())
            {
                List<HealthAgeValueItem> valueN = reportItem.HealthAgeValueN
                    .Where(i => i.HealthAgeValueType == QjHealthAgeValueTypeEnum.AgeDistribution)
                    .OrderBy(i => i.SortOrder)
                    .ToList();

                if (valueN.Count == 12)
                {
                    this._graphData = valueN.ConvertAll(i => i.Value);
                }
                else
                {
                    this._graphData = Enumerable.Repeat(decimal.MinValue, 12).ToList();
                }

                List<string> valueStringN = new List<string>();
                decimal axisMin = decimal.MaxValue;
                decimal axisMax = decimal.MinValue;

                int index = -1;

                this._graphData.ForEach(i =>
                {
                    index += 1;

                    if (i >= decimal.Zero)
                    {
                        // 健康年齢が含まれる値（例：20歳<X≦25歳）なら色を変える
                        if (HealthAgeDistributionReportPartialViewModel.ageRanges[index].Item1 < this._healthAge
                            && this._healthAge <= HealthAgeDistributionReportPartialViewModel.ageRanges[index].Item2)
                        {
                            valueStringN.Add(string.Format("{{color:'#ff7800',y:{0}}}", i));
                        }
                        else
                        {
                            valueStringN.Add(i.ToString());
                        }

                        axisMin = Math.Min(axisMin, i);
                        axisMax = Math.Max(axisMax, i);
                    }
                    else
                    {
                        valueStringN.Add("null");
                    }
                });

                if (axisMin != decimal.MaxValue && axisMax != decimal.MinValue)
                {
                    this._graphSetting.AxisMin = Math.Max(axisMin - 1, decimal.Zero);
                    this._graphSetting.AxisMax = Math.Max(axisMax + 1, decimal.Zero);
                }

                this._graphSetting.Label = string.Format("[{0}]", string.Join(",", HealthAgeDistributionReportPartialViewModel.ageRanges.Select(i => i.Item3))); // "['～20歳', '25歳', '30歳', '35歳', '40歳', '45歳', '50歳', '55歳', '60歳', '65歳', '70歳', '70歳～']"
                this._graphSetting.TargetValue = "[]";
                this._graphSetting.Data = string.Format("[{0}]", string.Join(",", valueStringN));
            }
        }

        #endregion

        #region Protected Method

        protected override void InitializeBy(HealthAgeReportItem reportItem)
        {
            // 棒グラフ 1 種
            this._graphData = Enumerable.Repeat(decimal.MinValue, 12).ToList();

            // 棒グラフ 1 種
            this._graphSetting = new HealthAgeReportGraphItem
            {
                Title = string.Empty,
                AxisMax = decimal.Zero,
                AxisMin = decimal.Zero,
                Label = "[]",
                TargetValue = "[]",
                Data = "[]"
            };

            this.SetBarGraphSetting(reportItem);
        }

        #endregion
    }
}
