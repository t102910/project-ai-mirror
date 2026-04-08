using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    [Serializable]
    public sealed class HealthAgeAdviceGoalItem
    {
        #region Public Property

        /// <summary>
        /// 項目名を取得または設定します。
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 測定値を取得または設定します。
        /// </summary>
        public decimal Old { get; set; } = decimal.MinValue;

        /// <summary>
        /// 目標値を取得または設定します。
        /// </summary>
        public decimal Goal { get; set; } = decimal.MinValue;

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="HealthAgeAdviceGoalItem" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public HealthAgeAdviceGoalItem()
        {
        }

        #endregion
    }
}
