
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 医療・健康情報ホーム画面用の読み込み結果
    /// </summary>
    internal sealed class PhrForHomeReaderResults : QsDbReaderResultsBase<MGF_NULL_ENTITY>
    {
        #region "Private Property"
        #endregion

        #region "Public Property"
        /// <summary>
        /// 成功フラグ
        /// </summary>
        public bool IsSuccess { get; set; } = false;

        /// <summary>
        /// 取得日時
        /// </summary>
        public string TargetDate { get; set; } = string.Empty;

        /// <summary>
        /// 摂取カロリー
        /// </summary>
        public int DailyCaloricIntake { get; set; } = int.MinValue;

        /// <summary>
        /// 消費カロリー
        /// </summary>
        public int CaloriesBurned { get; set; } = int.MinValue;

        /// <summary>
        /// 歩数
        /// </summary>
        public decimal Steps { get; set; } = decimal.MinValue;

        /// <summary>
        /// 運動
        /// </summary>
        public int Exercise { get; set; } = int.MinValue;

        /// <summary>
        /// 食事カロリー合計
        /// </summary>
        public int CaloryInMeal { get; set; } = int.MinValue;

        /// <summary>
        /// 食事種別
        /// </summary>
        public string MealType { get; set; } = string.Empty;

        /// <summary>
        /// 体重
        /// </summary>
        public decimal BodyWeight { get; set; } = decimal.MinValue;

        /// <summary>
        /// BMI
        /// </summary>
        public decimal BMI { get; set; } = decimal.MinValue;

        /// <summary>
        /// 気分
        /// </summary>
        public string Feelings { get; set; } = string.Empty;

        /// <summary>
        /// 就寝時間
        /// </summary>
        public string BedTime { get; set; } = string.Empty;

        /// <summary>
        /// 起床時間
        /// </summary>
        public string WakeupTime { get; set; } = string.Empty;

        /// <summary>
        /// 睡眠時間
        /// </summary>
        public decimal SleepTime { get; set; } = decimal.MinValue;

        /// <summary>
        /// 収縮期血圧１
        /// </summary>
        public decimal SBP1 { get; set; } = decimal.MinValue;

        /// <summary>
        /// 拡張期血圧１
        /// </summary>
        public decimal DBP1 { get; set; } = decimal.MinValue;

        /// <summary>
        /// 収縮期血圧２
        /// </summary>
        public decimal SBP2 { get; set; } = decimal.MinValue;

        /// <summary>
        /// 拡張期血圧２
        /// </summary>
        public decimal DBP2 { get; set; } = decimal.MinValue;

        /// <summary>
        /// 血糖値今回
        /// </summary>
        public decimal BloodSugarLevel1 { get; set; } = decimal.MinValue;

        /// <summary>
        /// 血糖値測定タイミング今回
        /// </summary>
        public string BloodSugarLevelTiming1 { get; set; } = string.Empty;

        /// <summary>
        /// 血糖値測定時間今回
        /// </summary>
        public string BloodSugarLevelLogTime1 { get; set; } = string.Empty;

        /// <summary>
        /// 血糖値前回
        /// </summary>
        public decimal BloodSugarLevel2 { get; set; } = decimal.MinValue;

        /// <summary>
        /// 血糖値測定タイミング前回
        /// </summary>
        public string BloodSugarLevelTiming2 { get; set; } = string.Empty;

        /// <summary>
        /// 血糖値測定時間前回
        /// </summary>
        public string BloodSugarLevelLogTime2 { get; set; } = string.Empty;
        #endregion

        #region "Constructor"
        #endregion

        #region "Private Method"
        #endregion

        #region "Public Method"
        #endregion
    }
}
