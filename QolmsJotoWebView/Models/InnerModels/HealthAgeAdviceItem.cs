using System;
using System.Collections.Generic;

namespace MGF.QOLMS.QolmsJotoWebView
{
    [Serializable]
    public sealed class HealthAgeAdviceItem
    {
        #region Public Property

        /// <summary>
        /// 表題を取得または設定します。
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 基本アドバイスを取得または設定します。
        /// </summary>
        public string Based { get; set; } = string.Empty;

        /// <summary>
        /// 詳細アドバイスのリストを取得または設定します。
        /// </summary>
        public List<string> DetailN { get; set; } = new List<string>();

        /// <summary>
        /// 目標項目のリストを取得または設定します。
        /// </summary>
        public List<HealthAgeAdviceGoalItem> GoalN { get; set; } = new List<HealthAgeAdviceGoalItem>();

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="HealthAgeAdviceItem" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public HealthAgeAdviceItem()
        {
        }

        #endregion
    }
}
