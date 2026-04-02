using System;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 連携内容種別を表します。
    /// </summary>
    [Flags]
    public enum QjRelationContentTypeEnum : long
    {
        /// <summary>
        /// 未指定です。
        /// </summary>
        None = QsDbRelationTypeEnum.None,

        /// <summary>
        /// 基本情報です。
        /// </summary>
        Information = QsDbRelationTypeEnum.Information,

        /// <summary>
        /// バイタル情報です。
        /// </summary>
        Vital = QsDbRelationTypeEnum.Vital,

        /// <summary>
        /// お薬情報です。
        /// </summary>
        Medicine = QsDbRelationTypeEnum.Medicine,

        /// <summary>
        /// 検査・ 健診情報です。
        /// </summary>
        Examination = QsDbRelationTypeEnum.Examination,

        /// <summary>
        /// 連絡手帳です。
        /// </summary>
        Contact = QsDbRelationTypeEnum.Contact,

        /// <summary>
        /// 歯科情報です。
        /// </summary>
        Dental = QsDbRelationTypeEnum.Dental,

        /// <summary>
        /// 活動情報です。
        /// </summary>
        Assessment = QsDbRelationTypeEnum.Assessment,

        /// <summary>
        /// 食事情報です。
        /// </summary>
        Meal = QsDbRelationTypeEnum.Meal
    }
}