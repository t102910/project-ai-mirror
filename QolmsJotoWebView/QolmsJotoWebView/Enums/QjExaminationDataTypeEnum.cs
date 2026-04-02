namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 記録タイプを表します。
    /// </summary>
    public enum QjExaminationDataTypeEnum
    {
        /// <summary>
        /// 未指定です。
        /// </summary>
        None = 0,

        /// <summary>
        /// 健診総合所見です。
        /// </summary>
        OverallAssessmentPdf = 1,

        /// <summary>
        /// 健診総合所見です。
        /// </summary>
        OverallAssessmentCsv = 2,

        /// <summary>
        /// 健診画像です。
        /// </summary>
        DicomData = 129
    }
}
