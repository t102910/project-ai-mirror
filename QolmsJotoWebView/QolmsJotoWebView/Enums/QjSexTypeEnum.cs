using MGF.QOLMS.QolmsApiCoreV1;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 性別の種別を表します。
    /// </summary>
    public enum QjSexTypeEnum : byte
    {
        /// <summary>
        /// 未指定です。
        /// </summary>
        None = QsApiSexTypeEnum.None,

        /// <summary>
        /// 男性です。
        /// </summary>
        Male = QsApiSexTypeEnum.Male,

        /// <summary>
        /// 女性です。
        /// </summary>
        Female = QsApiSexTypeEnum.Female,

        /// <summary>
        /// その他です。
        /// </summary>
        Other = QsApiSexTypeEnum.Other
    }
}