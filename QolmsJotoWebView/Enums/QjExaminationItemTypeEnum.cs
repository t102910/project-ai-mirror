namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 検査結果項目の種別を表します。
    /// </summary>
    public enum QjExaminationItemTypeEnum
    {
        /// <summary>
        /// 未指定です。
        /// </summary>
        None = 0,

        /// <summary>
        /// 一連検査グループです。
        /// </summary>
        [System.Obsolete("未使用です。")]
        Group = 1,

        /// <summary>
        /// 検査結果です。
        /// </summary>
        Value = 2,

        /// <summary>
        /// 検査結果に対する判定コメントです。
        /// </summary>
        Judgment = 3,

        /// <summary>
        /// 総合判定コメントです。
        /// </summary>
        TotalJudgment = 4
    }
}
