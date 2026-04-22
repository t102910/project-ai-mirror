using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// HealthRecord関連の検証を行う
    /// </summary>
    public interface IHealthRecordValidator
    {
        /// <summary>
        /// HealthRecordの検証を行う
        /// </summary>
        /// <param name="vitalValues"></param>
        /// <returns></returns>
        (bool isValid, string error) Validate(List<QhApiVitalValueItem> vitalValues);

        /// <summary>
        /// HealthRecordAlertの検証を行う
        /// </summary>
        /// <param name="vitalType"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        (bool isValid, string error) ValidateForAlert(QsDbVitalTypeEnum vitalType, decimal value1, decimal value2);

        /// <summary>
        /// 日付範囲のバリデーションを行います。
        /// </summary>
        /// <param name="fromDate">開始日時文字列</param>
        /// <param name="toDate">終了日時文字列</param>
        /// <param name="parsedFrom">パース後の開始日時</param>
        /// <param name="parsedTo">パース後の終了日時</param>
        /// <returns>バリデーション結果（成功/失敗、エラーメッセージ）</returns>
        (bool isValid, string errorMessage) ValidateDateRange(
            string fromDate, 
            string toDate, 
            out DateTime parsedFrom, 
            out DateTime parsedTo);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class HealthRecordValidator: IHealthRecordValidator
    {
        /// <summary>
        /// バイタル値として有効な最小値を表します。
        /// </summary>
        internal const decimal MIN_VALUE = 1M;
        /// <summary>
        /// バイタル値として有効な最大値を表します。
        /// </summary>
        internal const decimal MAX_VALUE = 999999.9999M;
        /// <summary>
        ///  バイタル値として有効な最大値（脈拍、体内年齢）を表します。
        /// </summary>
        internal const decimal THREE_DIGIT_MAX_VALUE = 999M;
        /// <summary>
        ///  運動量値として有効な最大値（消費カロリー）を表します。
        /// </summary>
        internal const decimal FOUR_DIGIT_MAX_VALUE = 9999M;
        /// <summary>
        /// バイタル値として有効な最大値（歩数）を表します。
        /// </summary>
        internal const decimal SIX_DIGIT_MAX_VALUE = 999999M;
        /// <summary>
        /// バイタル値として有効な最大値（HbA1c、体脂肪率、水分率）を表します。
        /// </summary>
        internal const decimal PERCENTAGE_MAX_VALUE = 100M;
        /// <summary>
        /// バイタル値として有効な最小値（Mets）を表します。
        /// </summary>
        internal const decimal POINT_DIGIT_MIN_VALUE = 0.1M;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="vitalValues"></param>
        /// <returns></returns>
        public (bool isValid, string error) Validate(List<QhApiVitalValueItem> vitalValues)
        {
            // データチェック 1つでもNGがあればエラーとする
            foreach (var vital in vitalValues)
            {
                if (!DateTime.TryParse(vital.RecordDate, out var recordDate))
                {
                    return (false, "RecordDateが不正です");
                }
                if (!decimal.TryParse(vital.Value1, out var value1))
                {
                    return (false, "Value1が不正です");
                }

                var value2 = vital.Value2.TryToValueType(decimal.MinValue);

                // 秒以下はWorker側で丸めるのでバリデーションとしては外す
                //if (recordDate.Second != 0 || recordDate.Millisecond != 0)
                //{
                //    return (false, "RecordDateが不正です。RecordDateの秒以下は0にしてください。");
                //}

                var vitalType = (QsDbVitalTypeEnum)vital.VitalType.TryToValueType(byte.MinValue);


                (bool isValid, string error) result;
                switch (vitalType)
                {
                    // 歩数
                    case QsDbVitalTypeEnum.Steps:
                        result = ValidateSteps(recordDate, value1);
                        break;
                    // 睡眠
                    case QsDbVitalTypeEnum.SleepingTime:
                        result = ValidateSleepingTime(value1);
                        break;
                    // 血圧
                    case QsDbVitalTypeEnum.BloodPressure:
                        result = ValidateBloodPressure(value1, value2);
                        break;
                    // 体内年齢、脈拍
                    case QsDbVitalTypeEnum.BodyAge:
                    case QsDbVitalTypeEnum.Pulse:
                        result = ValidateThreeDigitMaxValue(value1);
                        break;
                    // 血糖値
                    case QsDbVitalTypeEnum.BloodSugar:
                        result = ValidateBloodSugar(value1, vital.ConditionType);
                        break;
                    // 消費カロリー
                    case QsDbVitalTypeEnum.CalorieBurn:
                        result = ValidateCalorieBurn(value1);
                        break;
                    // 運動強度
                    case QsDbVitalTypeEnum.Mets:
                        result = ValidateMets(value1);
                        break;
                    // HbA1c、体脂肪率、水分率、血中酸素濃度
                    case QsDbVitalTypeEnum.Glycohemoglobin:
                    case QsDbVitalTypeEnum.BodyFatPercentage:
                    case QsDbVitalTypeEnum.TotalBodyWater:
                    case QsDbVitalTypeEnum.BloodOxygen:
                        result = ValidatePercentageMaxValue(value1);
                        break;
                    default:
                        result = ValidateDefault(value1);
                        break;
                }

                if (!result.isValid)
                {
                    return (false, result.error);
                }
            }

            return (true, null);
        }

        /// <inheritdoc/>
        public (bool isValid, string error) ValidateForAlert(QsDbVitalTypeEnum vitalType, decimal value1, decimal value2)
        {
            (bool isValid, string error) validationRet;
            switch (vitalType)
            {                
                // 心拍
                case QsDbVitalTypeEnum.Pulse:
                    validationRet = ValidateThreeDigitMaxValue(value1);
                    break;               
                // 血中酸素濃度
                case QsDbVitalTypeEnum.BloodOxygen:
                    validationRet = ValidatePercentageMaxValue(value1);
                    break;
                // 今のところAlertは心拍・血中酸素濃度のみ
                default:
                    validationRet = ValidateDefault(value1);
                    break;
            }

            if (!validationRet.isValid)
            {                
                return (false, validationRet.error);
            }

            return (true, null);
        }

        static (bool isValid, string error) ValidateSteps(DateTime recordDate, decimal value1)
        {
            if (recordDate != recordDate.Date)
            {
                return (false, "歩数は時間単位のデータを受け付けません。RecordDateの時間は0:00:00にしてください。");
            }

            if (GetDecimalPartScale(value1) > 0)
            {
                return (false, "歩数に少数は設定できません。");
            }

            if (!(value1 >= MIN_VALUE && value1 <= SIX_DIGIT_MAX_VALUE))
            {
                return (false, "歩数に最小値より小さい値、または最大値より大きい値が設定されています。");
            }

            return (true, null);
        }

        static (bool isValid, string error) ValidateSleepingTime(decimal value1)
        {
            if (value1 < 1 || value1 > (60 * 24) || GetDecimalPartScale(value1) > 0)
            {
                return (false, "Valueが睡眠として不正です");
            }

            return (true, null);
        }

        static (bool isValid, string error) ValidateBloodPressure(decimal value1, decimal value2)
        {
            if (!(value1 >= MIN_VALUE &&
                    value1 <= MAX_VALUE &&
                    value2 >= decimal.MinValue &&
                    value2 <= MAX_VALUE &&
                    value1 > value2))
            {
                return (false, "Valueが血圧値として不正です");
            }

            return (true, null);
        }

        static (bool isValid, string error) ValidateThreeDigitMaxValue(decimal value1)
        {
            if (!(value1 >= MIN_VALUE && value1 <= THREE_DIGIT_MAX_VALUE))
            {
                return (false, "Valueが不正です");
            }
            if (GetDecimalPartScale(value1) > 0)
            {
                return (false, "Valueが不正です");
            }

            return (true, null);
        }

        static (bool isValid, string error) ValidateBloodSugar(decimal value1, string conditionType)
        {
            var hasConditionType = Enum.IsDefined(typeof(QsDbVitalConditionTypeEnum), conditionType.TryToValueType(byte.MaxValue));

            if (!(value1 >= MIN_VALUE &&
                  value1 <= MAX_VALUE &&
                  hasConditionType)
               )
            {
                return (false, "Valueが血糖値として不正です");
            }

            return (true, null);
        }

        static (bool isValid, string error) ValidatePercentageMaxValue(decimal value1)
        {
            if (!(value1 >= MIN_VALUE && value1 <= PERCENTAGE_MAX_VALUE))
            {
                return (false, "Valueが不正です");
            }

            return (true, null);
        }

        static (bool isValid, string error) ValidateDefault(decimal value1)
        {
            if (!(value1 >= MIN_VALUE && value1 <= MAX_VALUE))
            {
                return (false, "Value1が不正です");
            }

            return (true, null);
        }

        static (bool isValid, string error) ValidateCalorieBurn(decimal value1)
        {
            if(!short.TryParse(value1.ToString(), out short shortValue) && !(value1 >= MIN_VALUE && value1 <= FOUR_DIGIT_MAX_VALUE))
            {
                return (false, "Valueが消費カロリーとして不正です");
            }

            return (true, null);
        }

        static (bool isValid, string error) ValidateMets(decimal value1)
        {
            if (!(value1 >= POINT_DIGIT_MIN_VALUE && value1 <= MAX_VALUE))
            {
                return (false, "Value1が不正です");
            }

            return (true, null);
        }

        /// <inheritdoc/>
        public (bool isValid, string errorMessage) ValidateDateRange(
            string fromDate, 
            string toDate, 
            out DateTime parsedFrom, 
            out DateTime parsedTo)
        {
            parsedFrom = DateTime.MinValue;
            parsedTo = DateTime.MinValue;

            // 1. FromDate/ToDateのDateTime変換
            if (!DateTime.TryParse(fromDate, out parsedFrom))
            {
                return (false, "FromDate/ToDateの形式が不正です。");
            }

            if (!DateTime.TryParse(toDate, out parsedTo))
            {
                return (false, "FromDate/ToDateの形式が不正です。");
            }

            // 2. FromDate <= ToDate チェック
            if (parsedFrom > parsedTo)
            {
                return (false, "FromDateはToDate以前の日時を指定してください。");
            }

            // 3. 未来日付チェック (ToDate <= DateTime.Now)
            if (parsedTo > DateTime.Now)
            {
                return (false, "未来の日時は指定できません。");
            }

            // 4. 期間チェック (最大7日間)
            var daysDiff = (parsedTo.Date - parsedFrom.Date).Days;
            if (daysDiff > 7)
            {
                return (false, "取得期間は最大7日間です。");
            }

            return (true, null);
        }

            /// <summary>
            /// 10進数の小数部桁数を取得します。
            /// </summary>
            /// <param name="value">取得対象の10進数</param>
            /// <returns>小数部の桁数</returns>
            static int GetDecimalPartScale(decimal value)
        {
            return decimal.GetBits(value)[3] >> 16 & 0xFF;
        }
    }
}