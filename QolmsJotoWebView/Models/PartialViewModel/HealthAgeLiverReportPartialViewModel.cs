using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MGF.QOLMS.QolmsJotoWebView
{
    [Serializable]
    public sealed class HealthAgeLiverReportPartialViewModel : QjHealthAgeReportPartialViewModelBase
    {
        #region Variable

        private decimal _deviance = decimal.Zero;

        private Dictionary<QjHealthAgeValueTypeEnum, List<decimal>> _graphDataN = new Dictionary<QjHealthAgeValueTypeEnum, List<decimal>>();

        private Dictionary<QjHealthAgeValueTypeEnum, HealthAgeReportGraphItem> _graphSettingN = new Dictionary<QjHealthAgeValueTypeEnum, HealthAgeReportGraphItem>();

        private QjSexTypeEnum _sexType = QjSexTypeEnum.None;

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

        public HealthAgeReportGraphItem Ch025GraphSetting
        {
            get { return this._graphSettingN[QjHealthAgeValueTypeEnum.Ch025]; }
        }

        public HealthAgeReportGraphItem Ch027GraphSetting
        {
            get { return this._graphSettingN[QjHealthAgeValueTypeEnum.Ch027]; }
        }

        public HealthAgeReportGraphItem Ch029GraphSetting
        {
            get { return this._graphSettingN[QjHealthAgeValueTypeEnum.Ch029]; }
        }

        #endregion

        #region Constructor

        public HealthAgeLiverReportPartialViewModel(HealthAgeViewModel model, HealthAgeReportItem reportItem)
            : base(model, reportItem)
        {
            this._sexType = model.AuthorSex; // TODO:

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
                case QjHealthAgeValueTypeEnum.Ch025:
                    // GOT（AST）
                    // 8 未満  ：低値
                    // 8～38   ：正常
                    // 39～89  ：軽度上昇
                    // 90～499 ：中程度上昇
                    // 500 以上：高度上昇
                    result = "[8.0, 38.9, 0, 7.9, 39.0, 89.9, null, null, 90.0, 1000.0]";
                    break;

                case QjHealthAgeValueTypeEnum.Ch027:
                    // GPT（ALT）
                    // 4 未満  ：低値
                    // 4～43   ：正常
                    // 44～89  ：軽度上昇
                    // 90～499 ：中程度上昇
                    // 500 以上：高度上昇
                    result = "[4.0, 43.9, 0, 3.9, 44.0, 89.9, null, null, 90.0, 1000.0]";
                    break;

                case QjHealthAgeValueTypeEnum.Ch029:
                    // γ-GT（γ-GTP）
                    // 男性 86 以下、女性 48 以下：正常
                    // 男性 87～499、女性 49～499：アルコール多量摂取の場合、適正量を心がける。まれに肝炎、肝硬変の発症 が見られる。薬物による肝障害の有無については他の検査結果をみて判定。
                    // 500 以上                  ：入院して精密検査を受け、日常生活での医師の指導が必要となる。
                    if (this._sexType == QjSexTypeEnum.Male)
                    {
                        // 男性
                        result = "[0, 86.9, null, null, 87.0, 499.9, null, null, 500.0, 1000.0]";
                    }
                    else if (this._sexType == QjSexTypeEnum.Female)
                    {
                        // 女性
                        result = "[0, 48.9, null, null, 49.0, 499.9, null, null, 500.0, 1000.0]";
                    }
                    break;
            }

            return result;
        }

        private void SetDeviance(HealthAgeReportItem reportItem)
        {
            if (reportItem != null
                && reportItem.HealthAgeReportType == QjHealthAgeReportTypeEnum.Liver
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
                && reportItem.HealthAgeReportType == QjHealthAgeReportTypeEnum.Liver
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

            this._graphSettingN[QjHealthAgeValueTypeEnum.InsComparison].Label = "['AST<br/>(GOT)', 'ALT<br/>(GPT)', 'γ-GT<br/>(γ-GTP)']";
            this._graphSettingN[QjHealthAgeValueTypeEnum.InsComparison].TargetValue = "[-5, 0, 0, 5]"; // TODO: 仮
            this._graphSettingN[QjHealthAgeValueTypeEnum.InsComparison].Data = string.Format("[{0}]", string.Join(",", valueStringN));
        }

        // AST（GOT）、ALT（GPT）、γ-GTP（γ-GT）
        private void SetLineGraphSetting(HealthAgeReportItem reportItem, QjHealthAgeValueTypeEnum valueType)
        {
            if (valueType == QjHealthAgeValueTypeEnum.Ch025
                || valueType == QjHealthAgeValueTypeEnum.Ch027
                || valueType == QjHealthAgeValueTypeEnum.Ch029)
            {
                if (reportItem != null
                    && reportItem.HealthAgeReportType == QjHealthAgeReportTypeEnum.Liver
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
                        valueStringN.Add(i.ToString()); // 整数

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
            // 棒グラフ 1 種、折れ線グラフ 3 種
            this._graphDataN = new Dictionary<QjHealthAgeValueTypeEnum, List<decimal>>
            {
                {
                    QjHealthAgeValueTypeEnum.InsComparison,
                    Enumerable.Repeat(decimal.MinValue, 3).ToList()
                },
                {
                    QjHealthAgeValueTypeEnum.Ch025,
                    Enumerable.Repeat(decimal.MinValue, 3).ToList()
                },
                {
                    QjHealthAgeValueTypeEnum.Ch027,
                    Enumerable.Repeat(decimal.MinValue, 3).ToList()
                },
                {
                    QjHealthAgeValueTypeEnum.Ch029,
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
                        Title = "'肝臓'",
                        AxisMax = decimal.Zero,
                        AxisMin = decimal.Zero,
                        Label = "[]",
                        TargetValue = "[]",
                        Data = "[]"
                    }
                },
                {
                    QjHealthAgeValueTypeEnum.Ch025,
                    new HealthAgeReportGraphItem
                    {
                        Title = "'AST（GOT）'",
                        AxisMax = decimal.Zero,
                        AxisMin = decimal.Zero,
                        Label = "[]",
                        TargetValue = "[]",
                        Data = "[]"
                    }
                },
                {
                    QjHealthAgeValueTypeEnum.Ch027,
                    new HealthAgeReportGraphItem
                    {
                        Title = "'ALT（GPT）'",
                        AxisMax = decimal.Zero,
                        AxisMin = decimal.Zero,
                        Label = "[]",
                        TargetValue = "[]",
                        Data = "[]"
                    }
                },
                {
                    QjHealthAgeValueTypeEnum.Ch029,
                    new HealthAgeReportGraphItem
                    {
                        Title = "'γ-GT（γ-GTP）'",
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
            this.SetLineGraphSetting(reportItem, QjHealthAgeValueTypeEnum.Ch025);
            this.SetLineGraphSetting(reportItem, QjHealthAgeValueTypeEnum.Ch027);
            this.SetLineGraphSetting(reportItem, QjHealthAgeValueTypeEnum.Ch029);
        }

        #endregion
    }
}
