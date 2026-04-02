using MGF.QOLMS.QolmsApiCoreV1;

/// <summary>
/// 健康年齢値情報の種別を表します。
/// </summary>
/// <remarks>
/// 既存のメンバーを変更しないでください。
/// 必要に応じて新規の値を持つメンバーを追加してください。
/// </remarks>
public enum QjHealthAgeValueTypeEnum
{
    /// <summary>
    /// 未指定です。
    /// </summary>
    None = QsApiHealthAgeValueTypeEnum.None,

    /// <summary>
    /// BMI です。
    /// </summary>
    BMI = QsApiHealthAgeValueTypeEnum.BMI,

    /// <summary>
    /// 収縮期血圧です。
    /// </summary>
    Ch014 = QsApiHealthAgeValueTypeEnum.Ch014,

    /// <summary>
    /// 拡張期血圧です。
    /// </summary>
    Ch016 = QsApiHealthAgeValueTypeEnum.Ch016,

    /// <summary>
    /// 中性脂肪です。
    /// </summary>
    Ch019 = QsApiHealthAgeValueTypeEnum.Ch019,

    /// <summary>
    /// HDL コレステロールです。
    /// </summary>
    Ch021 = QsApiHealthAgeValueTypeEnum.Ch021,

    /// <summary>
    /// LDL コレステロールです。
    /// </summary>
    Ch023 = QsApiHealthAgeValueTypeEnum.Ch023,

    /// <summary>
    /// GOT（AST）です。
    /// </summary>
    Ch025 = QsApiHealthAgeValueTypeEnum.Ch025,

    /// <summary>
    /// GPT（ALT）です。
    /// </summary>
    Ch027 = QsApiHealthAgeValueTypeEnum.Ch027,

    /// <summary>
    /// γ-GT（γ-GTP）です。
    /// </summary>
    Ch029 = QsApiHealthAgeValueTypeEnum.Ch029,

    /// <summary>
    /// HbA1c（NGSP）です。
    /// </summary>
    Ch035 = QsApiHealthAgeValueTypeEnum.Ch035,

    /// <summary>
    /// 空腹時血糖です。
    /// </summary>
    Ch035FBG = QsApiHealthAgeValueTypeEnum.Ch035FBG,

    /// <summary>
    /// 尿糖です。
    /// </summary>
    Ch037 = QsApiHealthAgeValueTypeEnum.Ch037,

    /// <summary>
    /// 尿蛋白（定性）です。
    /// </summary>
    Ch039 = QsApiHealthAgeValueTypeEnum.Ch039,

    /// <summary>
    /// 健康年齢算出です。
    /// </summary>
    Calculation = QsApiHealthAgeValueTypeEnum.Calculation,

    /// <summary>
    /// 同世代健康年齢分布です。
    /// </summary>
    AgeDistribution = QsApiHealthAgeValueTypeEnum.AgeDistribution,

    /// <summary>
    /// 同世代健診値比較です。
    /// </summary>
    InsComparison = QsApiHealthAgeValueTypeEnum.InsComparison,

    /// <summary>
    /// 健診結果レベル判定です。
    /// </summary>
    InsDeviance = QsApiHealthAgeValueTypeEnum.InsDeviance,

    /// <summary>
    /// 健康年齢改善アドバイスです。
    /// </summary>
    Advice = QsApiHealthAgeValueTypeEnum.Advice
}
