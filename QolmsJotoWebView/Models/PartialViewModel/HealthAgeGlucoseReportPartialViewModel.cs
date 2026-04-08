using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MGF.QOLMS.QolmsJotoWebView
{
    [Serializable]
    public sealed class HealthAgeGlucoseReportPartialViewModel : QjHealthAgeReportPartialViewModelBase
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

        public HealthAgeReportGraphItem Ch035GraphSetting
        {
            get { return this._graphSettingN[QjHealthAgeValueTypeEnum.Ch035]; }
        }

        public HealthAgeReportGraphItem Ch035FbgGraphSetting
        {
            get { return this._graphSettingN[QjHealthAgeValueTypeEnum.Ch035FBG]; }
        }

        #endregion

        #region Constructor

        public HealthAgeGlucoseReportPartialViewModel(HealthAgeViewModel model, HealthAgeReportItem reportItem)
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

            switch (valueType)
            {
                case QjHealthAgeValueTypeEnum.Ch035:
                    // HbA1c
                    // 5.8 以下：正常
                    // 5.9～6.4：正常又は良いコントロールの糖尿病
                    // 6.5以上 ：コントロールの悪い糖尿病又は合併症
                    result = "[0, 5.8, null, null, 5.9, 6.4, null, null, 6.5, 20.0]";
                    break;

                case QjHealthAgeValueTypeEnum.Ch035FBG:
                    // 空腹時血糖
                    // 59 以下 ：低血糖症
                    // 60～109 ：正常
                    // 110～125：境界型糖尿病
                    // 126 以上：糖尿病
                    result = "[60.0, 109.9, 0, 59.9, 110.9, 125.9, null, null, 126.0, 600.0]";
                    break;
            }

            return result;
        }

        private void SetDeviance(HealthAgeReportItem reportItem)
        {
            if (reportItem != null
                && reportItem.HealthAgeReportType == QjHealthAgeReportTypeEnum.Glucose
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
                && reportItem.HealthAgeReportType == QjHealthAgeReportTypeEnum.Glucose
                && reportItem.HealthAgeValueN != null
                && reportItem.HealthAgeValueN.Any())
            {
                List<HealthAgeValueItem> valueN = reportItem.HealthAgeValueN
                    .Where(i => i.HealthAgeValueType == QjHealthAgeValueTypeEnum.InsComparison)
                    .OrderBy(i => i.SortOrder)
                    .ToList();

                if (valueN.Count == 2)
                {
                    this._graphDataN[QjHealthAgeValueTypeEnum.InsComparison] = valueN.ConvertAll(i => i.Comparison);
                    redCodeN = valueN.ConvertAll(i => i.IsRedCode);
                }
                else
                {
                    this._graphDataN[QjHealthAgeValueTypeEnum.InsComparison] = Enumerable.Repeat(decimal.MinValue, 2).ToList();
                    redCodeN = Enumerable.Repeat(false, 2).ToList();
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

            this._graphSettingN[QjHealthAgeValueTypeEnum.InsComparison].Label = "['HbA1c', '空腹時<br/>血糖']";
            this._graphSettingN[QjHealthAgeValueTypeEnum.InsComparison].TargetValue = "[-5, 0, 0, 5]"; // TODO: 仮
            this._graphSettingN[QjHealthAgeValueTypeEnum.InsComparison].Data = string.Format("[{0}]", string.Join(",", valueStringN));
        }

        // HbA1c、空腹時血糖
        private void SetLineGraphSetting(HealthAgeReportItem reportItem, QjHealthAgeValueTypeEnum valueType)
        {
            if (valueType == QjHealthAgeValueTypeEnum.Ch035 || valueType == QjHealthAgeValueTypeEnum.Ch035FBG)
            {
                if (reportItem != null
                    && reportItem.HealthAgeReportType == QjHealthAgeReportTypeEnum.Glucose
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

                if (valueType == QjHealthAgeValueTypeEnum.Ch035)
                {
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
                }
                else
                {
                    this._graphDataN[valueType].ForEach(i =>
                    {
                        if (i >= decimal.Zero)
                        {
                            valueStringN.Add(i.ToString()); // 整数

                            axisMin = Math.Min(axisMin, i);
                            axisMax = Math.Max(axisMax, i);
                        }
                        else
                        {
                            valueStringN.Add("null");
                        }
                    });
                }

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
            // 棒グラフ 1 種、折れ線グラフ 2 種
            this._graphDataN = new Dictionary<QjHealthAgeValueTypeEnum, List<decimal>>
            {
                {
                    QjHealthAgeValueTypeEnum.InsComparison,
                    Enumerable.Repeat(decimal.MinValue, 2).ToList()
                },
                {
                    QjHealthAgeValueTypeEnum.Ch035,
                    Enumerable.Repeat(decimal.MinValue, 3).ToList()
                },
                {
                    QjHealthAgeValueTypeEnum.Ch035FBG,
                    Enumerable.Repeat(decimal.MinValue, 3).ToList()
                }
            };

            // 棒グラフ 1 種、折れ線グラフ 2 種
            this._graphSettingN = new Dictionary<QjHealthAgeValueTypeEnum, HealthAgeReportGraphItem>
            {
                {
                    QjHealthAgeValueTypeEnum.InsComparison,
                    new HealthAgeReportGraphItem
                    {
                        Title = "'血糖'",
                        AxisMax = decimal.Zero,
                        AxisMin = decimal.Zero,
                        Label = "[]",
                        TargetValue = "[]",
                        Data = "[]"
                    }
                },
                {
                    QjHealthAgeValueTypeEnum.Ch035,
                    new HealthAgeReportGraphItem
                    {
                        Title = "'HbA1c'",
                        AxisMax = decimal.Zero,
                        AxisMin = decimal.Zero,
                        Label = "[]",
                        TargetValue = "[]",
                        Data = "[]"
                    }
                },
                {
                    QjHealthAgeValueTypeEnum.Ch035FBG,
                    new HealthAgeReportGraphItem
                    {
                        Title = "'空腹時血糖'",
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
            this.SetLineGraphSetting(reportItem, QjHealthAgeValueTypeEnum.Ch035);
            this.SetLineGraphSetting(reportItem, QjHealthAgeValueTypeEnum.Ch035FBG);
        }

        #endregion
    }
}
