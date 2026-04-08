namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 検査結果データ型の種別を表します。
    /// </summary>
    public enum QjExaminationItemValueTypeEnum
    {
        /// <summary>
        /// 未指定です。
        /// </summary>
        None = 0,

        /// <summary>
        /// 数値です。
        /// </summary>
        PQ = 1,

        /// <summary>
        /// 順序付コード値です。
        /// </summary>
        CO = 2,

        /// <summary>
        /// 順序なしコード値です。
        /// </summary>
        CD = 3,

        /// <summary>
        /// 文字列です。
        /// </summary>
        ST = 4
    }
}
