using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MGF.QOLMS.QolmsJotoWebView
{
    [Serializable]
    public sealed class HealthAgeFatReportPartialViewModel : QjHealthAgeReportPartialViewModelBase
    {
        #region Variable

        private decimal _deviance = decimal.Zero;

        private Dictionary<QjHealthAgeValueTypeEnum, List<decimal>> _graphDataN = new Dictionary<QjHealthAgeValueTypeEnum, List<decimal>>();

        private Dictionary<QjHealthAgeValueTypeEnum, HealthAgeReportGraphItem> _graphSettingN = new Dictionary<QjHealthAgeValueTypeEnum, HealthAgeReportGraphItem>();

        #endregion

        #region Public Property

        public decimal InsDeviance
        {
            get { return this._deviance; }
        }

        public HealthAgeReportGraphItem InsComparisonGraphSetting
        {
            get { return this._graphSettingN[QjHealthAgeValueTypeEnum.InsComparison]; }
        }

        public HealthAgeReportGraphItem BmiGraphSetting
        {
            get { return this._graphSettingN[QjHealthAgeValueTypeEnum.BMI]; }
        }

        #endregion

        #region Constructor

        public HealthAgeFatReportPartialViewModel(HealthAgeViewModel model, HealthAgeReportItem reportItem)
            : base(model, reportItem)
        {
            this.InitializeBy(reportItem);
        }

        #endregion

        #region Private Method

        // TODO:
        private string CreateTargetValues(QjHealthAgeValueTypeEnum valueType)
        {
            string result = "[]";

            if (valueType == QjHealthAgeValueTypeEnum.BMI)
            {
                // BMI
                // 18.4 以下 ：やせ
                // 18.5～24.9：正常
                // 25.0～29.9：過体重
                // 30.0～34.9：肥満
                // 35.0 以上 ：過肥満
                result = "[18.5, 24.9, 0, 18.4, 25.0, 29.9, null, null, 30.0, 100.0]";
            }

            return result;
        }

        private void SetDeviance(HealthAgeReportItem reportItem)
        {
            if (reportItem != null
                && reportItem.HealthAgeReportType == QjHealthAgeReportTypeEnum.Fat
                && reportItem.HealthAgeValueN != null
                && reportItem.HealthAgeValueN.Any()
                && (reportItem.Deviance == 1m || reportItem.Deviance == 2m || reportItem.Deviance == 3m))
            {
                this._deviance = reportItem.Deviance;
            }
        }

        // 同世代健診値比較
        private void SetBarGraphSetting(HealthAgeReportItem reportItem)
        {
            List<bool> redCodeN = new List<bool>();

            if (reportItem != null
                && reportItem.HealthAgeReportType == QjHealthAgeReportTypeEnum.Fat
                && reportItem.HealthAgeValueN != null
                && reportItem.HealthAgeValueN.Any())
            {
                List<HealthAgeValueItem> valueN = reportItem.HealthAgeValueN
                    .Where(i => i.HealthAgeValueType == QjHealthAgeValueTypeEnum.InsComparison)
                    .OrderBy(i => i.SortOrder)
                    .ToList();

                if (valueN.Count == 1)
                {
                    this._graphDataN[QjHealthAgeValueTypeEnum.InsComparison] = valueN.ConvertAll(i => i.Comparison);
                    redCodeN = valueN.ConvertAll(i => i.IsRedCode);
                }
                else
                {
                    this._graphDataN[QjHealthAgeValueTypeEnum.InsComparison] = Enumerable.Repeat(decimal.MinValue, 1).ToList();
                    redCodeN = Enumerable.Repeat(false, 1).ToList();
                }
            }

            List<string> valueStringN = new List<string>();
            decimal axisMin = decimal.MaxValue;
            decimal axisMax = decimal.MinValue;
            int index = 0;

            this._graphDataN[QjHealthAgeValueTypeEnum.InsComparison].ForEach(i =>
            {
                if (i != decimal.MinValue)
                {
                    //valueStringN.Add(i.ToString("0.####")) // 小数点以下 4 桁

                    // 劣勢項目なら色を変える
                    if (redCodeN[index])
                    {
                        valueStringN.Add(string.Format("{{color:'#ff3c3b',y:{0:0.####}}}", i)); // 小数点以下 4 桁
                    }
                    else
                    {
                        valueStringN.Add(i.ToString("0.####")); // 小数点以下 4 桁
                    }

                    axisMin = Math.Min(axisMin, i);
                    axisMax = Math.Max(axisMax, i);
                }
                else
                {
                    valueStringN.Add("null");
                }

                index += 1;
            });

            if (axisMin != decimal.MaxValue && axisMax != decimal.MinValue)
            {
                this._graphSettingN[QjHealthAgeValueTypeEnum.InsComparison].AxisMin = Math.Max(axisMin - 1, decimal.Zero);
                this._graphSettingN[QjHealthAgeValueTypeEnum.InsComparison].AxisMax = Math.Max(axisMax + 1, decimal.Zero);
            }

            this._graphSettingN[QjHealthAgeValueTypeEnum.InsComparison].Label = "['BMI']";
            this._graphSettingN[QjHealthAgeValueTypeEnum.InsComparison].TargetValue = "[-5, 0, 0, 5]"; // TODO: 仮
            this._graphSettingN[QjHealthAgeValueTypeEnum.InsComparison].Data = string.Format("[{0}]", string.Join(",", valueStringN));
        }

        // BMI
        private void SetLineGraphSetting(HealthAgeReportItem reportItem, QjHealthAgeValueTypeEnum valueType)
        {
            if (valueType == QjHealthAgeValueTypeEnum.BMI)
            {
                if (reportItem != null
                    && reportItem.HealthAgeReportType == QjHealthAgeReportTypeEnum.Fat
                    && reportItem.HealthAgeValueN != null
                    && reportItem.HealthAgeValueN.Any())
                {
                    List<HealthAgeValueItem> valueN = reportItem.HealthAgeValueN
                        .Where(i => i.HealthAgeValueType == valueType)
                        .OrderByDescending(i => i.RecordDate)
                        .Take(3)
                        .ToList();

                    switch (valueN.Count)
                    {
                        case 2:
                            valueN.Add(new HealthAgeValueItem { RecordDate = DateTime.MinValue, Value = decimal.MinValue });
                            break;

                        case 1:
                            valueN.AddRange(new[]
                            {
                                new HealthAgeValueItem { RecordDate = DateTime.MinValue, Value = decimal.MinValue },
                                new HealthAgeValueItem { RecordDate = DateTime.MinValue, Value = decimal.MinValue }
                            });
                            break;

                        case 0:
                            valueN.AddRange(new[]
                            {
                                new HealthAgeValueItem { RecordDate = DateTime.MinValue, Value = decimal.MinValue },
                                new HealthAgeValueItem { RecordDate = DateTime.MinValue, Value = decimal.MinValue },
                                new HealthAgeValueItem { RecordDate = DateTime.MinValue, Value = decimal.MinValue }
                            });
                            break;
                    }

                    valueN.Reverse();

                    this._graphDataN[valueType] = valueN.ConvertAll(i => i.Value);
                }

                List<string> valueStringN = new List<string>();
                decimal axisMin = decimal.MaxValue;
                decimal axisMax = decimal.MinValue;

                this._graphDataN[valueType].ForEach(i =>
                {
                    if (i >= decimal.Zero)
                    {
                        valueStringN.Add(i.ToString("0.#")); // 小数点以下 1 桁

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
                    this._graphSettingN[valueType].AxisMin = Math.Max(axisMin - 1m, decimal.Zero);
                    this._graphSettingN[valueType].AxisMax = Math.Max(axisMax + 1m, decimal.Zero);
                }

                this._graphSettingN[valueType].Label = "['前々回', '前回', '今回']";
                this._graphSettingN[valueType].TargetValue = this.CreateTargetValues(valueType);
                this._graphSettingN[valueType].Data = string.Format("[{0}]", string.Join(",", valueStringN));
            }
        }

        #endregion

        #region Protected Method

        protected override void InitializeBy(HealthAgeReportItem reportItem)
        {
            // 棒グラフ 1 種、折れ線グラフ 1 種
            this._graphDataN = new Dictionary<QjHealthAgeValueTypeEnum, List<decimal>>
            {
                {
                    QjHealthAgeValueTypeEnum.InsComparison,
                    Enumerable.Repeat(decimal.MinValue, 1).ToList()
                },
                {
                    QjHealthAgeValueTypeEnum.BMI,
                    Enumerable.Repeat(decimal.MinValue, 3).ToList()
                }
            };

            // 棒グラフ 1 種、折れ線グラフ 1 種
            this._graphSettingN = new Dictionary<QjHealthAgeValueTypeEnum, HealthAgeReportGraphItem>
            {
                {
                    QjHealthAgeValueTypeEnum.InsComparison,
                    new HealthAgeReportGraphItem
                    {
                        Title = "'肥満'",
                        AxisMax = decimal.Zero,
                        AxisMin = decimal.Zero,
                        Label = "[]",
                        TargetValue = "[]",
                        Data = "[]"
                    }
                },
                {
                    QjHealthAgeValueTypeEnum.BMI,
                    new HealthAgeReportGraphItem
                    {
                        Title = "'BMI'",
                        AxisMax = decimal.Zero,
                        AxisMin = decimal.Zero,
                        Label = "[]",
                        TargetValue = "[]",
                        Data = "[]"
                    }
                }
            };

            this.SetDeviance(reportItem);
            this.SetBarGraphSetting(reportItem);
            this.SetLineGraphSetting(reportItem, QjHealthAgeValueTypeEnum.BMI);
        }

        #endregion
    }
}
