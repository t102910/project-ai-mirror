using MGF.QOLMS.QolmsApiCoreV1;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 食事の種別を表します。
    /// </summary>
    public enum QyMealTypeEnum : byte
    {
        /// <summary>
        /// 未指定です。
        /// </summary>
        None = QsApiMealTypeEnum.None,

        /// <summary>
        /// 朝食です。
        /// </summary>
        Breakfast = QsApiMealTypeEnum.Breakfast,

        /// <summary>
        /// 昼食です。
        /// </summary>
        Lunch = QsApiMealTypeEnum.Lunch,

        /// <summary>
        /// 夕食です。
        /// </summary>
        Dinner = QsApiMealTypeEnum.Dinner,

        /// <summary>
        /// 間食です。
        /// </summary>
        Snacking = QsApiMealTypeEnum.Snacking,

        /// <summary>
        /// その他です。
        /// </summary>
        Other = QsApiMealTypeEnum.Other
    }
}
