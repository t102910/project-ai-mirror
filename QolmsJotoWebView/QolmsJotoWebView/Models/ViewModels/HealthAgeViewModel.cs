using System;
using System.Collections.Generic;
using System.Linq;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 「健康年齢」画面ビュー モデルを表します。
    /// このクラスは継承できません。
    /// </summary>
    [Serializable]
    public sealed class HealthAgeViewModel : QjHealthPageViewModelBase
    {
        #region Public Property

        /// <summary>
        /// 遷移元の画面番号の種別を取得または設定します。
        /// </summary>
        public QjPageNoTypeEnum FromPageNoType { get; set; } = QjPageNoTypeEnum.None;

        /// <summary>
        /// 病院連携中かを取得または設定します。
        /// </summary>
        public bool HasHospitalConnection { get; set; } = false;

        /// <summary>
        /// 健康年齢を取得または設定します。
        /// </summary>
        public decimal HealthAge { get; set; } = decimal.MinValue;

        /// <summary>
        /// 実年齢との差を取得または設定します。
        /// </summary>
        public decimal HealthAgeDistance { get; set; } = decimal.MinValue;

        /// <summary>
        /// 測定日を取得または設定します。
        /// </summary>
        public DateTime LatestDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 予測医療費（100% 負担額）を取得または設定します。
        /// </summary>
        public int MedicalExpenses { get; set; } = int.MinValue;

        /// <summary>
        /// 健康年齢の推移のリストを取得または設定します。
        /// </summary>
        public List<HealthAgeValueItem> HealthAgeN { get; set; } = new List<HealthAgeValueItem>();

        /// <summary>
        /// 健康年齢改善アドバイス情報のリストを取得または設定します。
        /// </summary>
        public List<HealthAgeAdviceItem> HealthAgeAdviceN { get; set; } = new List<HealthAgeAdviceItem>();

        /// <summary>
        /// 健康年齢レポート情報の有効性のリストを取得または設定します。
        /// </summary>
        public List<AvailableHealthAgeReportItem> AvailableHealthAgeReportN { get; set; } = new List<AvailableHealthAgeReportItem>();

        // 年齢分布
        public HealthAgeDistributionReportPartialViewModel DistributionPartialViewModel { get; set; } = null;

        // 肥満レポート
        public HealthAgeFatReportPartialViewModel FatPartialViewModel { get; set; } = null;

        // 血糖レポート
        public HealthAgeGlucoseReportPartialViewModel GlucosePartialViewModel { get; set; } = null;

        // 血圧レポート
        public HealthAgePressureReportPartialViewModel PressurePartialViewModel { get; set; } = null;

        // 脂質レポート
        public HealthAgeLipidReportPartialViewModel LipidPartialViewModel { get; set; } = null;

        // 肝臓レポート
        public HealthAgeLiverReportPartialViewModel LiverPartialViewModel { get; set; } = null;

        // 尿糖・尿蛋白レポート
        public HealthAgeUrineReportPartialViewModel UrinePartialViewModel { get; set; } = null;

        #endregion

        #region Constructor

        /// <summary>
        /// <see cref="HealthAgeViewModel" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public HealthAgeViewModel()
            : base()
        {
        }

        /// <summary>
        /// 値を指定して、
        /// <see cref="HealthAgeViewModel" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="mainModel">メイン モデル。</param>
        /// <param name="fromPageNoType">遷移元の画面番号の種別。</param>
        public HealthAgeViewModel(QolmsJotoModel mainModel, QjPageNoTypeEnum fromPageNoType)
            : base(mainModel, QjPageNoTypeEnum.HealthAge)
        {
            this.FromPageNoType = fromPageNoType;
            this.HasHospitalConnection = false;
            this.HealthAge = decimal.MinValue;
            this.HealthAgeDistance = decimal.MinValue;
            this.MedicalExpenses = int.MinValue;
            this.HealthAgeAdviceN = new List<HealthAgeAdviceItem>();
            this.LatestDate = DateTime.MinValue;

            this.AvailableHealthAgeReportN = new[]
            {
                new AvailableHealthAgeReportItem
                {
                    HealthAgeReportType = QjHealthAgeReportTypeEnum.Distribution,
                    LatestDate = DateTime.MinValue
                },
                new AvailableHealthAgeReportItem
                {
                    HealthAgeReportType = QjHealthAgeReportTypeEnum.Fat,
                    LatestDate = DateTime.MinValue
                },
                new AvailableHealthAgeReportItem
                {
                    HealthAgeReportType = QjHealthAgeReportTypeEnum.Glucose,
                    LatestDate = DateTime.MinValue
                },
                new AvailableHealthAgeReportItem
                {
                    HealthAgeReportType = QjHealthAgeReportTypeEnum.Pressure,
                    LatestDate = DateTime.MinValue
                },
                new AvailableHealthAgeReportItem
                {
                    HealthAgeReportType = QjHealthAgeReportTypeEnum.Lipid,
                    LatestDate = DateTime.MinValue
                },
                new AvailableHealthAgeReportItem
                {
                    HealthAgeReportType = QjHealthAgeReportTypeEnum.Liver,
                    LatestDate = DateTime.MinValue
                },
                new AvailableHealthAgeReportItem
                {
                    HealthAgeReportType = QjHealthAgeReportTypeEnum.Urine,
                    LatestDate = DateTime.MinValue
                }
            }.ToList();

            // 各レポートのパーシャル ビュー モデルの初期化（レポートは非同期で表示するため空で初期化）
            this.DistributionPartialViewModel = new HealthAgeDistributionReportPartialViewModel(
                this,
                new HealthAgeReportItem { HealthAgeReportType = QjHealthAgeReportTypeEnum.Distribution }
            );

            this.FatPartialViewModel = new HealthAgeFatReportPartialViewModel(
                this,
                new HealthAgeReportItem { HealthAgeReportType = QjHealthAgeReportTypeEnum.Fat }
            );

            this.GlucosePartialViewModel = new HealthAgeGlucoseReportPartialViewModel(
                this,
                new HealthAgeReportItem { HealthAgeReportType = QjHealthAgeReportTypeEnum.Glucose }
            );

            this.PressurePartialViewModel = new HealthAgePressureReportPartialViewModel(
                this,
                new HealthAgeReportItem { HealthAgeReportType = QjHealthAgeReportTypeEnum.Pressure }
            );

            this.LipidPartialViewModel = new HealthAgeLipidReportPartialViewModel(
                this,
                new HealthAgeReportItem { HealthAgeReportType = QjHealthAgeReportTypeEnum.Lipid }
            );

            this.LiverPartialViewModel = new HealthAgeLiverReportPartialViewModel(
                this,
                new HealthAgeReportItem { HealthAgeReportType = QjHealthAgeReportTypeEnum.Liver }
            );

            this.UrinePartialViewModel = new HealthAgeUrineReportPartialViewModel(
                this,
                new HealthAgeReportItem { HealthAgeReportType = QjHealthAgeReportTypeEnum.Urine }
            );
        }

        #endregion

        #region Public Method

        public bool IsAvailableHealthAgeReportType(QjHealthAgeReportTypeEnum healthAgeReportType, ref bool hasData)
        {
            hasData = false;

            AvailableHealthAgeReportItem item =
                this.AvailableHealthAgeReportN.Find(i => i.HealthAgeReportType == healthAgeReportType);

            if (item == null)
            {
                return false;
            }
            else
            {
                hasData = item.LatestDate != DateTime.MinValue;
                return true;
            }
        }

        #endregion
    }
}
