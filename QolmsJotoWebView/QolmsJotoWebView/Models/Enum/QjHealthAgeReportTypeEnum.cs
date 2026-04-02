using MGF.QOLMS.QolmsApiCoreV1;

/// <summary>
/// 健康年齢レポート情報の種別を表します。
/// </summary>
/// <remarks>
/// 既存のメンバーを変更しないでください。
/// 必要に応じて新規の値を持つメンバーを追加してください。
/// </remarks>
public enum QjHealthAgeReportTypeEnum : byte
{
    /// <summary>
    /// 未指定です。
    /// </summary>
    None = QsApiHealthAgeReportTypeEnum.None,

    /// <summary>
    /// 年齢分布です。
    /// </summary>
    Distribution = QsApiHealthAgeReportTypeEnum.Distribution,

    /// <summary>
    /// 肥満レポートです。
    /// </summary>
    Fat = QsApiHealthAgeReportTypeEnum.Fat,

    /// <summary>
    /// 血糖レポートです。
    /// </summary>
    Glucose = QsApiHealthAgeReportTypeEnum.Glucose,

    /// <summary>
    /// 血圧レポートです。
    /// </summary>
    Pressure = QsApiHealthAgeReportTypeEnum.Pressure,

    /// <summary>
    /// 脂質レポートです。
    /// </summary>
    Lipid = QsApiHealthAgeReportTypeEnum.Lipid,

    /// <summary>
    /// 肝臓レポートです。
    /// </summary>
    Liver = QsApiHealthAgeReportTypeEnum.Liver,

    /// <summary>
    /// 尿糖・尿蛋白レポートです。
    /// </summary>
    Urine = QsApiHealthAgeReportTypeEnum.Urine
}
