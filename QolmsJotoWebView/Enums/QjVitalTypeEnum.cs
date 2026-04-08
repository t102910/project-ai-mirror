using MGF.QOLMS.QolmsApiCoreV1;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// バイタル情報の種別を表します。
    /// </summary>
    public enum QjVitalTypeEnum : byte
    {
        /// <summary>
        /// 未指定です。
        /// </summary>
        None = QsApiVitalTypeEnum.None,

        /// <summary>
        /// 血圧（mmHg）です。
        /// </summary>
        BloodPressure = QsApiVitalTypeEnum.BloodPressure,

        /// <summary>
        /// 脈拍（回/分）です。
        /// </summary>

        Pulse = QsApiVitalTypeEnum.Pulse,

        /// <summary>
        /// 血糖値（mg/dl）です。
        /// </summary>

        BloodSugar = QsApiVitalTypeEnum.BloodSugar,

        /// <summary>
        /// HbA1c（%）です。
        /// </summary>

        Glycohemoglobin = QsApiVitalTypeEnum.Glycohemoglobin,

        /// <summary>
        /// 体重（kg）です。
        /// </summary>

        BodyWeight = QsApiVitalTypeEnum.BodyWeight,

        /// <summary>
        /// 腹囲（cm）です。
        /// </summary>

        BodyWaist = QsApiVitalTypeEnum.BodyWaist,

        /// <summary>
        /// 体温（℃）です。
        /// </summary>

        BodyTemperature = QsApiVitalTypeEnum.BodyTemperature,

        /// <summary>
        /// 歩数（歩）です。
        /// </summary>

        Steps = QsApiVitalTypeEnum.Steps,

        /// <summary>
        /// 身長（cm）です。
        /// </summary>

        BodyHeight = QsApiVitalTypeEnum.BodyHeight,

        /// <summary>
        /// BMI です。
        /// </summary>

        BodyMassIndex = QsApiVitalTypeEnum.BodyMassIndex,

        /// <summary>
        /// 体脂肪率（%）です。
        /// </summary>

        BodyFatPercentage = QsApiVitalTypeEnum.BodyFatPercentage,

        /// <summary>
        /// 筋肉量（kg）です。
        /// </summary>

        MuscleMass = QsApiVitalTypeEnum.MuscleMass,

        /// <summary>
        /// 推定骨量（kg）です。
        /// </summary>

        BoneMass = QsApiVitalTypeEnum.BoneMass,

        /// <summary>
        /// 内脂肪レベルです。
        /// </summary>

        VisceralFat = QsApiVitalTypeEnum.VisceralFat,

        /// <summary>
        /// 基礎代謝（kcal）です。
        /// </summary>

        BasalMetabolism = QsApiVitalTypeEnum.BasalMetabolism,

        /// <summary>
        /// 体内年齢（歳）です。
        /// </summary>

        BodyAge = QsApiVitalTypeEnum.BodyAge,

        /// <summary>
        /// 水分率（%）です。
        /// </summary>

        TotalBodyWater = QsApiVitalTypeEnum.TotalBodyWater,

        /// <summary>
        /// METs です。
        /// </summary>
        Mets = 20
        ///TODO 定義待ち 
        /// Mets = QsApiVitalTypeEnum.Mets

    }
}