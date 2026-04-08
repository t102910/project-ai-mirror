using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    [Serializable]
    public sealed class HealthAgeReportGraphItem
    {
        #region Public Property

        public string Title { get; set; } = string.Empty;

        public decimal AxisMin { get; set; } = decimal.Zero;

        public decimal AxisMax { get; set; } = decimal.Zero;

        public string Label { get; set; } = "[]";

        public string TargetValue { get; set; } = "[]";

        public string Data { get; set; } = "[]";

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="HealthAgeReportGraphItem" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public HealthAgeReportGraphItem()
        {
        }

        #endregion
    }
}
