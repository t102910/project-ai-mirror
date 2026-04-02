using System;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>QsDbDosageFormTypeEnum
    /// YappliのHome画面に表示する薬剤の剤型の種別を表します。
    /// </summary>
    /// <remarks>
    /// 既存のIDを変更しないでください。
    /// メンバーを追加した場合は新規のIDを指定してください。
    /// </remarks>
    [Flags]
    public enum QjDosageFormTypeEnum : int
    {
        /// <summary>
        /// 未指定です。
        /// </summary>
        None = QsDbDosageFormTypeEnum.None,

        /// <summary>
        /// 内服です。
        /// </summary>
        Oral = QsDbDosageFormTypeEnum.Oral,

        /// <summary>
        /// 内滴です。
        /// </summary>
        Drip = QsDbDosageFormTypeEnum.Drip,

        /// <summary>
        /// 頓服です。
        /// </summary>
        DoseOfMedicine = QsDbDosageFormTypeEnum.DoseOfMedicine,

        /// <summary>
        /// 注射です。
        /// </summary>
        InjectionDrug = QsDbDosageFormTypeEnum.InjectionDrug,

        /// <summary>
        /// 外用です。
        /// </summary>
        External = QsDbDosageFormTypeEnum.External,

        /// <summary>
        /// 浸煎です。
        /// </summary>
        DipFry = QsDbDosageFormTypeEnum.DipFry,

        /// <summary>
        /// 湯です。
        /// </summary>
        Touzai = QsDbDosageFormTypeEnum.Touzai,

        /// <summary>
        /// 材料です。
        /// </summary>
        Materials = QsDbDosageFormTypeEnum.Materials,

        /// <summary>
        /// その他です。
        /// </summary>
        Other = QsDbDosageFormTypeEnum.Other
    }
}