using System;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 健康年齢レポート情報の有効性を表します。
    /// このクラスは継承できません。
    /// </summary>
    [Serializable]
    public sealed class AvailableHealthAgeReportItem
    {
        #region Public Property

        /// <summary>
        /// 健康年齢レポート情報の種別を取得または設定します。
        /// </summary>
        public QjHealthAgeReportTypeEnum HealthAgeReportType { get; set; }
            = QjHealthAgeReportTypeEnum.None;

        /// <summary>
        /// 最新の測定日時を取得または設定します。
        /// </summary>
        public DateTime LatestDate { get; set; } = DateTime.MinValue;

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="AvailableHealthAgeReportItem" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public AvailableHealthAgeReportItem()
        {
        }

        #endregion
    }
}
