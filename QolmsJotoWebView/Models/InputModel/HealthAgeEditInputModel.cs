using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace MGF.QOLMS.QolmsJotoWebView
{
    [Serializable]
    public sealed class HealthAgeEditInputModel
        : QjHealthPageViewModelBase,
          IQjModelUpdater<HealthAgeEditInputModel>,
          IValidatableObject
    {
        #region Constant

        private const string REQUIRED_ERROR_MESSAGE = "{0}を入力してください。";
        private const string RANGE_ERROR_MESSAGE = "{0}は{1}～{2}の範囲で入力してください。";
        private const string OVERLAP_ERROR_MESSAGE = "{0}が重複しています。";
        private const string REVERSE_ERROR_MESSAGE = "{0}が逆転しています。";
        private const string DECIMAL_PART_ERROR_MESSAGE = "{0}は整数で入力して下さい。";
        private const string DECIMAL_PART_DIGIT_ERROR_MESSAGE = "{0}は小数点以下{1}桁以内で入力してください。";

        private const decimal MIN_VALUE = 1m;
        private const decimal MAX_VALUE = 999999.9999m;

        private static readonly Dictionary<QjHealthAgeValueTypeEnum, Tuple<decimal, decimal, int>> valueRanges
         = new Dictionary<QjHealthAgeValueTypeEnum, Tuple<decimal, decimal, int>>
                 {
                { QjHealthAgeValueTypeEnum.BMI, Tuple.Create(10m, 100m, 1) },
                { QjHealthAgeValueTypeEnum.Ch014, Tuple.Create(60m, 300m, 0) },
                { QjHealthAgeValueTypeEnum.Ch016, Tuple.Create(30m, 150m, 0) },
                { QjHealthAgeValueTypeEnum.Ch019, Tuple.Create(10m, 2000m, 0) },
                { QjHealthAgeValueTypeEnum.Ch021, Tuple.Create(10m, 500m, 1) },
                { QjHealthAgeValueTypeEnum.Ch023, Tuple.Create(20m, 1000m, 1) },
                { QjHealthAgeValueTypeEnum.Ch025, Tuple.Create(1m, 1000m, 0) },
                { QjHealthAgeValueTypeEnum.Ch027, Tuple.Create(1m, 1000m, 0) },
                { QjHealthAgeValueTypeEnum.Ch029, Tuple.Create(1m, 1000m, 0) },
                { QjHealthAgeValueTypeEnum.Ch035, Tuple.Create(3m, 20m, 1) },
                { QjHealthAgeValueTypeEnum.Ch035FBG, Tuple.Create(20m, 600m, 0) },
                { QjHealthAgeValueTypeEnum.Ch037, Tuple.Create(1m, 5m, 0) },
                { QjHealthAgeValueTypeEnum.Ch039, Tuple.Create(1m, 5m, 0) }
            };

        #endregion

        #region Variable

        private readonly Dictionary<QjHealthAgeValueTypeEnum, DateTime> _latestDateN
       = new Dictionary<QjHealthAgeValueTypeEnum, DateTime>();

        #endregion

        #region Public Property

        public QjPageNoTypeEnum FromPageNoType { get; set; } = QjPageNoTypeEnum.None;

        public bool IsMaintenance { get; set; }

        public string MaintenanceMessage { get; set; } = string.Empty;

        public DateTime RecordDate { get; set; } = DateTime.MinValue;

        public string BMI { get; set; } = string.Empty;

        public string Ch014 { get; set; } = string.Empty;

        public string Ch016 { get; set; } = string.Empty;

        public string Ch019 { get; set; } = string.Empty;

        public string Ch021 { get; set; } = string.Empty;

        public string Ch023 { get; set; } = string.Empty;

        public string Ch025 { get; set; } = string.Empty;

        public string Ch027 { get; set; } = string.Empty;

        public string Ch029 { get; set; } = string.Empty;

        public string Ch035 { get; set; } = string.Empty;

        public string Ch035FBG { get; set; } = string.Empty;

        public string Ch037 { get; set; } = string.Empty;

        public string Ch039 { get; set; } = string.Empty;

        #endregion

        #region Constructor

        public HealthAgeEditInputModel() : base() { }

        public HealthAgeEditInputModel(
            QolmsJotoModel mainModel,
            Dictionary<QjHealthAgeValueTypeEnum, Tuple<DateTime, decimal>> valueN)
            : base(mainModel, QjPageNoTypeEnum.HealthAgeEdit)
        {
            RecordDate = DateTime.Now;

            if (valueN == null || !valueN.Any()) return;

            foreach (var i in valueN)
            {
                var key = i.Key;
                if (key == QjHealthAgeValueTypeEnum.None) continue;

                PropertyInfo pi = GetType().GetProperty(key.ToString());
                if (pi == null || _latestDateN.ContainsKey(key)) continue;

                _latestDateN.Add(key, i.Value.Item1);
                pi.SetValue(this, i.Value.Item2 > 0 ? i.Value.Item2.ToString("0.####") : string.Empty);
            }
        }

        #endregion

        #region Validation Helpers

        private string CreateErrorMessage(string format, string propertyName, string displayName, object arg1 = null, object arg2 = null)
            => string.Format(format, string.IsNullOrWhiteSpace(displayName) ? propertyName : displayName, arg1, arg2);

        public int GetDecimalPartScale(decimal value)
            => (decimal.GetBits(value)[3] >> 16) & 0xFF;

        private decimal CheckDecimalValue(string value, decimal lowerLimit = MIN_VALUE, decimal upperLimit = MAX_VALUE)
        {
            if (string.IsNullOrWhiteSpace(value)) return decimal.MinValue;

            return decimal.TryParse(value, out var d) && d >= lowerLimit && d <= upperLimit
                ? d
                : decimal.MinValue;
        }

        private bool CheckDecimalPartScale(decimal value, int scaleLimit)
            => GetDecimalPartScale(value) <= scaleLimit;

        private List<Tuple<QjHealthAgeValueTypeEnum, string>> CheckPressureValue(string value1, string value2)
        {
            var result = new List<Tuple<QjHealthAgeValueTypeEnum, string>>();
            decimal decimalValue1 = decimal.MinValue;
            decimal decimalValue2 = decimal.MinValue;

            // 血圧（上）
            if (!string.IsNullOrWhiteSpace(value1))
            {
                decimalValue1 = CheckDecimalValue(
                    value1,
                    valueRanges[QjHealthAgeValueTypeEnum.Ch014].Item1,
                    valueRanges[QjHealthAgeValueTypeEnum.Ch014].Item2
                );

                if (decimalValue1 == decimal.MinValue)
                {
                    result.Add(
                        Tuple.Create(
                            QjHealthAgeValueTypeEnum.Ch014,
                            CreateErrorMessage(
                                RANGE_ERROR_MESSAGE,
                                string.Empty,
                                "血圧（上）",
                                valueRanges[QjHealthAgeValueTypeEnum.Ch014].Item1,
                                valueRanges[QjHealthAgeValueTypeEnum.Ch014].Item2
                            )
                        )
                    );
                }
                else if (!CheckDecimalPartScale(decimalValue1, valueRanges[QjHealthAgeValueTypeEnum.Ch014].Item3))
                {
                    if (valueRanges[QjHealthAgeValueTypeEnum.Ch014].Item3 == 0)
                    {
                        result.Add(
                            Tuple.Create(
                                QjHealthAgeValueTypeEnum.Ch014,
                                CreateErrorMessage(DECIMAL_PART_ERROR_MESSAGE, string.Empty, "血圧（上）")
                            )
                        );
                    }
                    else
                    {
                        result.Add(
                            Tuple.Create(
                                QjHealthAgeValueTypeEnum.Ch014,
                                CreateErrorMessage(
                                    DECIMAL_PART_DIGIT_ERROR_MESSAGE,
                                    string.Empty,
                                    "血圧（上）",
                                    valueRanges[QjHealthAgeValueTypeEnum.Ch014].Item3
                                )
                            )
                        );
                    }
                }
            }
            else
            {
                result.Add(
                    Tuple.Create(
                        QjHealthAgeValueTypeEnum.Ch014,
                        CreateErrorMessage(REQUIRED_ERROR_MESSAGE, string.Empty, "血圧（上）")
                    )
                );
            }

            // 血圧（下）
            if (!string.IsNullOrWhiteSpace(value2))
            {
                decimalValue2 = CheckDecimalValue(
                    value2,
                    valueRanges[QjHealthAgeValueTypeEnum.Ch016].Item1,
                    valueRanges[QjHealthAgeValueTypeEnum.Ch016].Item2
                );

                if (decimalValue2 == decimal.MinValue)
                {
                    result.Add(
                        Tuple.Create(
                            QjHealthAgeValueTypeEnum.Ch016,
                            CreateErrorMessage(
                                RANGE_ERROR_MESSAGE,
                                string.Empty,
                                "血圧（下）",
                                valueRanges[QjHealthAgeValueTypeEnum.Ch016].Item1,
                                valueRanges[QjHealthAgeValueTypeEnum.Ch016].Item2
                            )
                        )
                    );
                }
                else if (!CheckDecimalPartScale(decimalValue2, valueRanges[QjHealthAgeValueTypeEnum.Ch016].Item3))
                {
                    if (valueRanges[QjHealthAgeValueTypeEnum.Ch016].Item3 == 0)
                    {
                        result.Add(
                            Tuple.Create(
                                QjHealthAgeValueTypeEnum.Ch016,
                                CreateErrorMessage(DECIMAL_PART_ERROR_MESSAGE, string.Empty, "血圧（下）")
                            )
                        );
                    }
                    else
                    {
                        result.Add(
                            Tuple.Create(
                                QjHealthAgeValueTypeEnum.Ch016,
                                CreateErrorMessage(
                                    DECIMAL_PART_DIGIT_ERROR_MESSAGE,
                                    string.Empty,
                                    "血圧（下）",
                                    valueRanges[QjHealthAgeValueTypeEnum.Ch016].Item3
                                )
                            )
                        );
                    }
                }
            }
            else
            {
                result.Add(
                    Tuple.Create(
                        QjHealthAgeValueTypeEnum.Ch016,
                        CreateErrorMessage(REQUIRED_ERROR_MESSAGE, string.Empty, "血圧（下）")
                    )
                );
            }

            // 上下の整合性
            if (!string.IsNullOrWhiteSpace(value1)
                && !string.IsNullOrWhiteSpace(value2)
                && decimalValue1 != decimal.MinValue
                && decimalValue2 != decimal.MinValue)
            {
                if (decimalValue1 == decimalValue2)
                {
                    result.Add(
                        Tuple.Create(
                            QjHealthAgeValueTypeEnum.Ch014,
                            CreateErrorMessage(OVERLAP_ERROR_MESSAGE, string.Empty, "血圧の上下")
                        )
                    );
                }
                else if (decimalValue1 < decimalValue2)
                {
                    result.Add(
                        Tuple.Create(
                            QjHealthAgeValueTypeEnum.Ch014,
                            CreateErrorMessage(REVERSE_ERROR_MESSAGE, string.Empty, "血圧の上下")
                        )
                    );
                }
            }

            return result;
        }

        private List<Tuple<QjHealthAgeValueTypeEnum, string>> CheckUrineValue(QjHealthAgeValueTypeEnum valueType, string value, string name)
        {
            switch (valueType)
            {
                case QjHealthAgeValueTypeEnum.Ch037:
                case QjHealthAgeValueTypeEnum.Ch039:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(valueType));
            }

            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            var result = new List<Tuple<QjHealthAgeValueTypeEnum, string>>();
            bool isError = false;

            if (!string.IsNullOrWhiteSpace(value))
            {
                var decimalValue = CheckDecimalValue(
                    value,
                    valueRanges[valueType].Item1,
                    valueRanges[valueType].Item2
                );

                if (decimalValue == decimal.MinValue)
                {
                    isError = true;
                }
                else if (!CheckDecimalPartScale(decimalValue, valueRanges[valueType].Item3))
                {
                    isError = true;
                }
            }
            else
            {
                isError = true;
            }

            if (isError)
            {
                result.Add(
                    Tuple.Create(
                        valueType,
                        CreateErrorMessage("{0}を選択してください。", string.Empty, name)
                    )
                );
            }

            return result;
        }

        private List<Tuple<QjHealthAgeValueTypeEnum, string>> CheckOtherValue(QjHealthAgeValueTypeEnum valueType, string value, string name)
        {
            switch (valueType)
            {
                case QjHealthAgeValueTypeEnum.BMI:
                case QjHealthAgeValueTypeEnum.Ch019:
                case QjHealthAgeValueTypeEnum.Ch021:
                case QjHealthAgeValueTypeEnum.Ch023:
                case QjHealthAgeValueTypeEnum.Ch025:
                case QjHealthAgeValueTypeEnum.Ch027:
                case QjHealthAgeValueTypeEnum.Ch029:
                case QjHealthAgeValueTypeEnum.Ch035:
                case QjHealthAgeValueTypeEnum.Ch035FBG:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(valueType));
            }

            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            var result = new List<Tuple<QjHealthAgeValueTypeEnum, string>>();

            if (!string.IsNullOrWhiteSpace(value))
            {
                var decimalValue = CheckDecimalValue(
                    value,
                    valueRanges[valueType].Item1,
                    valueRanges[valueType].Item2
                );

                if (decimalValue == decimal.MinValue)
                {
                    result.Add(
                        Tuple.Create(
                            valueType,
                            CreateErrorMessage(
                                RANGE_ERROR_MESSAGE,
                                string.Empty,
                                name,
                                valueRanges[valueType].Item1,
                                valueRanges[valueType].Item2
                            )
                        )
                    );
                }
                else if (!CheckDecimalPartScale(decimalValue, valueRanges[valueType].Item3))
                {
                    if (valueRanges[valueType].Item3 == 0)
                    {
                        result.Add(
                            Tuple.Create(
                                valueType,
                                CreateErrorMessage(DECIMAL_PART_ERROR_MESSAGE, string.Empty, name)
                            )
                        );
                    }
                    else
                    {
                        result.Add(
                            Tuple.Create(
                                valueType,
                                CreateErrorMessage(
                                    DECIMAL_PART_DIGIT_ERROR_MESSAGE,
                                    string.Empty,
                                    name,
                                    valueRanges[valueType].Item3
                                )
                            )
                        );
                    }
                }
            }
            else
            {
                result.Add(
                    Tuple.Create(
                        valueType,
                        CreateErrorMessage(REQUIRED_ERROR_MESSAGE, string.Empty, name)
                    )
                );
            }

            return result;
        }

        #endregion

        #region Public Method

        public void UpdateByInput(HealthAgeEditInputModel inputModel)
        {
            if (inputModel == null) return;

            RecordDate = inputModel.RecordDate;
            BMI = string.IsNullOrWhiteSpace(inputModel.BMI) ? string.Empty : inputModel.BMI.Trim();
            Ch014 = string.IsNullOrWhiteSpace(inputModel.Ch014) ? string.Empty : inputModel.Ch014.Trim();
            Ch016 = string.IsNullOrWhiteSpace(inputModel.Ch016) ? string.Empty : inputModel.Ch016.Trim();
            Ch019 = string.IsNullOrWhiteSpace(inputModel.Ch019) ? string.Empty : inputModel.Ch019.Trim();
            Ch021 = string.IsNullOrWhiteSpace(inputModel.Ch021) ? string.Empty : inputModel.Ch021.Trim();
            Ch023 = string.IsNullOrWhiteSpace(inputModel.Ch023) ? string.Empty : inputModel.Ch023.Trim();
            Ch025 = string.IsNullOrWhiteSpace(inputModel.Ch025) ? string.Empty : inputModel.Ch025.Trim();
            Ch027 = string.IsNullOrWhiteSpace(inputModel.Ch027) ? string.Empty : inputModel.Ch027.Trim();
            Ch029 = string.IsNullOrWhiteSpace(inputModel.Ch029) ? string.Empty : inputModel.Ch029.Trim();
            Ch035 = string.IsNullOrWhiteSpace(inputModel.Ch035) ? string.Empty : inputModel.Ch035.Trim();
            Ch035FBG = string.IsNullOrWhiteSpace(inputModel.Ch035FBG) ? string.Empty : inputModel.Ch035FBG.Trim();
            Ch037 = string.IsNullOrWhiteSpace(inputModel.Ch037) ? string.Empty : inputModel.Ch037.Trim();
            Ch039 = string.IsNullOrWhiteSpace(inputModel.Ch039) ? string.Empty : inputModel.Ch039.Trim();
        }

        public string GetLatestDateString(QjHealthAgeValueTypeEnum valueType, string format = "yyyy年M月d日更新")
        {
            if (_latestDateN.ContainsKey(valueType) && _latestDateN[valueType] != DateTime.MinValue && !string.IsNullOrWhiteSpace(format))
            {
                return _latestDateN[valueType].ToString(format);
            }

            return string.Empty;
        }

        [Obsolete("健康年齢（ベイジアン ネットワーク）算出用")]
        public void SetValues(DateTime recordDate, Dictionary<QjHealthAgeValueTypeEnum, Tuple<DateTime, decimal>> valueN)
        {
            RecordDate = recordDate;

            if (valueN == null || !valueN.Any()) return;

            foreach (var i in valueN)
            {
                var key = i.Key;

                if (key == QjHealthAgeValueTypeEnum.None) continue;

                PropertyInfo pi = GetType().GetProperty(key.ToString());

                if (pi != null && !_latestDateN.ContainsKey(key))
                {
                    _latestDateN.Add(key, i.Value.Item1);
                    pi.SetValue(this, i.Value.Item2 > 0 ? i.Value.Item2.ToString("0.####") : string.Empty);
                }
            }
        }

        #endregion

        #region IValidatableObject Support

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var result = new List<ValidationResult>();
            var dicN = new Dictionary<string, List<string>>();
            var messageN = new List<Tuple<QjHealthAgeValueTypeEnum, string>>();

            if (RecordDate == DateTime.MinValue)
            {
                dicN.Add(nameof(RecordDate), new List<string> { "健診受診日が不正です。" });
            }
            else if (RecordDate.Date > DateTime.Now.Date)
            {
                dicN.Add(nameof(RecordDate), new List<string> { "健診受診日が未来です。" });
            }

            messageN.AddRange(CheckOtherValue(QjHealthAgeValueTypeEnum.BMI, BMI, "BMI"));
            messageN.AddRange(CheckPressureValue(Ch014, Ch016));
            messageN.AddRange(CheckOtherValue(QjHealthAgeValueTypeEnum.Ch019, Ch019, "中性脂肪"));
            messageN.AddRange(CheckOtherValue(QjHealthAgeValueTypeEnum.Ch021, Ch021, "HDLコレステロール"));
            messageN.AddRange(CheckOtherValue(QjHealthAgeValueTypeEnum.Ch023, Ch023, "LDLコレステロール"));
            messageN.AddRange(CheckOtherValue(QjHealthAgeValueTypeEnum.Ch025, Ch025, "AST（GOT）"));
            messageN.AddRange(CheckOtherValue(QjHealthAgeValueTypeEnum.Ch027, Ch027, "ALT（GPT）"));
            messageN.AddRange(CheckOtherValue(QjHealthAgeValueTypeEnum.Ch029, Ch029, "γ-GT（γ-GTP）"));
            messageN.AddRange(CheckOtherValue(QjHealthAgeValueTypeEnum.Ch035, Ch035, "HbA1c（NGSP）"));
            messageN.AddRange(CheckOtherValue(QjHealthAgeValueTypeEnum.Ch035FBG, Ch035FBG, "空腹時血糖"));
            messageN.AddRange(CheckUrineValue(QjHealthAgeValueTypeEnum.Ch037, Ch037, "尿糖"));
            messageN.AddRange(CheckUrineValue(QjHealthAgeValueTypeEnum.Ch039, Ch039, "尿蛋白（定性）"));

            foreach (var i in messageN)
            {
                var key = i.Item1.ToString();
                if (!dicN.ContainsKey(key)) dicN.Add(key, new List<string>());
                dicN[key].Add(i.Item2);
            }

            foreach (var i in dicN)
            {
                result.Add(new ValidationResult(string.Join(Environment.NewLine, i.Value), new[] { i.Key }));
            }

            return result;
        }

        #endregion
    }
}
