using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MGF.QOLMS.QolmsJotoWebView
{
    [Serializable]
    public sealed class HealthAgeLipidReportPartialViewModel : QjHealthAgeReportPartialViewModelBase
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

        public HealthAgeReportGraphItem Ch019GraphSetting
        {
            get { return this._graphSettingN[QjHealthAgeValueTypeEnum.Ch019]; }
        }

        public HealthAgeReportGraphItem Ch021GraphSetting
        {
            get { return this._graphSettingN[QjHealthAgeValueTypeEnum.Ch021]; }
        }

        public HealthAgeReportGraphItem Ch023GraphSetting
        {
            get { return this._graphSettingN[QjHealthAgeValueTypeEnum.Ch023]; }
        }

        #endregion

        #region Constructor

        public HealthAgeLipidReportPartialViewModel(HealthAgeViewModel model, HealthAgeReportItem reportItem)
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
                case QjHealthAgeValueTypeEnum.Ch019:
                    // 中性脂肪
                    // 29 以下 ：低中性脂肪血症
                    // 30～149 ：正常
                    // 150～299：軽度高中性脂肪血症
                    // 300～749：中等度高中性脂肪血症
                    // 750 以上：高度高中性脂肪血症
                    result = "[30.0, 149.9, 10.0, 29.9, 150.0, 299.9, null, null, 300.0, 2000.0]";
                    break;

                case QjHealthAgeValueTypeEnum.Ch021:
                    // HDL コレステロール
                    // 19 以下 ：先天性異常の疑い
                    // 20～39  ：低 HDL コレステロール血症
                    // 40～99  ：正常
                    // 100 以上：高 HDL コレステロール血症や先天性の異常の疑い
                    result = "[40.0, 99.9, 20.0, 39.9, null, null, 10.0, 19.9, 100.0, 500.0]";
                    break;

                case QjHealthAgeValueTypeEnum.Ch023:
                    // LDL コレステロール
                    // 60～119 ：正常
                    // 120～139：境界域
                    // 140 以上：高 LDL コレステロール血症や先天性の異常の疑い
                    result = "[60.0, 119.9, null, null, 120.0, 139.9, null, null, 140.0, 1000.0]";
                    break;
            }

            return result;
        }

        private void SetDeviance(HealthAgeReportItem reportItem)
        {
            if (reportItem != null
                && reportItem.HealthAgeReportType == QjHealthAgeReportTypeEnum.Lipid
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
                && reportItem.HealthAgeReportType == QjHealthAgeReportTypeEnum.Lipid
                && reportItem.HealthAgeValueN != null
                && reportItem.HealthAgeValueN.Any())
            {
                List<HealthAgeValueItem> valueN = reportItem.HealthAgeValueN
                    .Where(i => i.HealthAgeValueType == QjHealthAgeValueTypeEnum.InsComparison)
                    .OrderBy(i => i.SortOrder)
                    .ToList();

                if (valueN.Count == 3)
                {
                    this._graphDataN[QjHealthAgeValueTypeEnum.InsComparison] = valueN.ConvertAll(i => i.Comparison);
                    redCodeN = valueN.ConvertAll(i => i.IsRedCode);
                }
                else
                {
                    this._graphDataN[QjHealthAgeValueTypeEnum.InsComparison] = Enumerable.Repeat(decimal.MinValue, 3).ToList();
                    redCodeN = Enumerable.Repeat(false, 3).ToList();
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

            this._graphSettingN[QjHealthAgeValueTypeEnum.InsComparison].Label = "['中性<br/>脂肪', 'HDL', 'LDL']";
            this._graphSettingN[QjHealthAgeValueTypeEnum.InsComparison].TargetValue = "[-5, 0, 0, 5]"; // TODO: 仮
            this._graphSettingN[QjHealthAgeValueTypeEnum.InsComparison].Data = string.Format("[{0}]", string.Join(",", valueStringN));
        }

        // 中性脂肪、HD Lコレステロール、LDL コレステロール
        private void SetLineGraphSetting(HealthAgeReportItem reportItem, QjHealthAgeValueTypeEnum valueType)
        {
            if (valueType == QjHealthAgeValueTypeEnum.Ch019
                || valueType == QjHealthAgeValueTypeEnum.Ch021
                || valueType == QjHealthAgeValueTypeEnum.Ch023)
            {
                if (reportItem != null
                    && reportItem.HealthAgeReportType == QjHealthAgeReportTypeEnum.Lipid
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

                if (valueType == QjHealthAgeValueTypeEnum.Ch019)
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
                else
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
            // 棒グラフ 1 種、折れ線グラフ 3 種
            this._graphDataN = new Dictionary<QjHealthAgeValueTypeEnum, List<decimal>>
            {
                {
                    QjHealthAgeValueTypeEnum.InsComparison,
                    Enumerable.Repeat(decimal.MinValue, 3).ToList()
                },
                {
                    QjHealthAgeValueTypeEnum.Ch019,
                    Enumerable.Repeat(decimal.MinValue, 3).ToList()
                },
                {
                    QjHealthAgeValueTypeEnum.Ch021,
                    Enumerable.Repeat(decimal.MinValue, 3).ToList()
                },
                {
                    QjHealthAgeValueTypeEnum.Ch023,
                    Enumerable.Repeat(decimal.MinValue, 3).ToList()
                }
            };

            // 棒グラフ 1 種、折れ線グラフ 3 種
            this._graphSettingN = new Dictionary<QjHealthAgeValueTypeEnum, HealthAgeReportGraphItem>
            {
                {
                    QjHealthAgeValueTypeEnum.InsComparison,
                    new HealthAgeReportGraphItem
                    {
                        Title = "'脂質'",
                        AxisMax = decimal.Zero,
                        AxisMin = decimal.Zero,
                        Label = "[]",
                        TargetValue = "[]",
                        Data = "[]"
                    }
                },
                {
                    QjHealthAgeValueTypeEnum.Ch019,
                    new HealthAgeReportGraphItem
                    {
                        Title = "'中性脂肪'",
                        AxisMax = decimal.Zero,
                        AxisMin = decimal.Zero,
                        Label = "[]",
                        TargetValue = "[]",
                        Data = "[]"
                    }
                },
                {
                    QjHealthAgeValueTypeEnum.Ch021,
                    new HealthAgeReportGraphItem
                    {
                        Title = "'HDLコレステロール'",
                        AxisMax = decimal.Zero,
                        AxisMin = decimal.Zero,
                        Label = "[]",
                        TargetValue = "[]",
                        Data = "[]"
                    }
                },
                {
                    QjHealthAgeValueTypeEnum.Ch023,
                    new HealthAgeReportGraphItem
                    {
                        Title = "'HDLコレステロール'",
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
            this.SetLineGraphSetting(reportItem, QjHealthAgeValueTypeEnum.Ch019);
            this.SetLineGraphSetting(reportItem, QjHealthAgeValueTypeEnum.Ch021);
            this.SetLineGraphSetting(reportItem, QjHealthAgeValueTypeEnum.Ch023);
        }

        #endregion
    }
}
