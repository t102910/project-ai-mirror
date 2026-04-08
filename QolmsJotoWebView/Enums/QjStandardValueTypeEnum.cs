using MGF.QOLMS.QolmsApiCoreV1;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// バイタル標準値の種別を表します。
    /// </summary>
    public enum QjStandardValueTypeEnum : byte
    {
        /// <summary>
        /// 未指定です。
        /// </summary>
        None = QsApiStandardValueTypeEnum.None,

        /// <summary>
        /// 血圧上です。
        /// </summary>
        BloodPressureUpper = QsApiStandardValueTypeEnum.BloodPressureUpper,

        /// <summary>
        /// 血圧下です。
        /// </summary>
        BloodPressureLower = QsApiStandardValueTypeEnum.BloodPressureLower,

        /// <summary>
        /// その他血糖値です。
        /// </summary>
        BloodSugarOther = QsApiStandardValueTypeEnum.BloodSugarOther,

        /// <summary>
        /// 空腹時血糖値です。
        /// </summary>
        BloodSugarFasting = QsApiStandardValueTypeEnum.BloodSugarFasting
    }
}