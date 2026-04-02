using System;
using System.Collections.Generic;

namespace MGF.QOLMS.QolmsJotoWebView
{
    [Serializable]
    public sealed class HealthAgeReportItem
    {
        #region Public Property

        /// <summary>
        /// 健康年齢レポート情報の種別を取得または設定します。
        /// </summary>
        public QjHealthAgeReportTypeEnum HealthAgeReportType { get; set; }
            = QjHealthAgeReportTypeEnum.None;

        /// <summary>
        /// 健康年齢値情報のリストを取得または設定します。
        /// </summary>
        public List<HealthAgeValueItem> HealthAgeValueN { get; set; }
            = new List<HealthAgeValueItem>();

        /// <summary>
        /// 健診結果レベル判定を取得または設定します（0～3）。
        /// </summary>
        public decimal Deviance { get; set; } = decimal.MinValue;

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="HealthAgeReportItem" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public HealthAgeReportItem()
        {
        }

        #endregion
    }
}
