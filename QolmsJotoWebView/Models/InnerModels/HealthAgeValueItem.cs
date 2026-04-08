using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    [Serializable]
    public sealed class HealthAgeValueItem
    {
        #region Public Property

        /// <summary>
        /// 測定日時を取得または設定します。
        /// </summary>
        public DateTime RecordDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 健康年齢情報の種別を取得または設定します。
        /// </summary>
        public QjHealthAgeValueTypeEnum HealthAgeValueType { get; set; } = QjHealthAgeValueTypeEnum.None;

        /// <summary>
        /// 測定値を取得または設定します。
        /// </summary>
        public decimal Value { get; set; } = decimal.Zero;

        /// <summary>
        /// 同世代健診値比較を取得または設定します（-3.0000～3.0000）。
        /// </summary>
        public decimal Comparison { get; set; } = decimal.MinValue;

        /// <summary>
        /// 劣勢項目かを取得または設定します。
        /// </summary>
        public bool IsRedCode { get; set; } = false;

        [Obsolete]
        public string Label { get; set; } = string.Empty;

        [Obsolete]
        public int SortOrder { get; set; } = int.MinValue;

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="HealthAgeValueItem" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public HealthAgeValueItem()
        {
        }

        #endregion
    }
}
