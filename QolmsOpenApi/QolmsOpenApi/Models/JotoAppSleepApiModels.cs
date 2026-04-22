using MGF.QOLMS.QolmsApiCoreV1;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MGF.QOLMS.QolmsApiEntityV1
{
    /// <summary>
    /// POST /JotoApp/SleepImport リクエスト
    ///
    /// ■ タイムゾーン仕様（重要）
    ///   BedTime / WakeupTime は必ず日本標準時（JST / UTC+9）で送信してください。
    ///   クライアントは端末の設定に関係なく JST に変換した値を渡すこと。
    ///   - 手動入力    : ユーザー入力値をそのまま送信（日本語UIのため既にJST）
    ///   - Apple HealthKit: startDate/endDate（端末ローカル）→ JST に変換して送信
    ///   - Google Health Connect: startTime/endTime（UTC）→ JST(+9h)に変換して送信
    /// </summary>
    [DataContract]
    [Serializable]
    public class QoJotoAppSleepImportApiArgs : QoApiArgsBase
    {
        /// <summary>入眠時刻（JST）。形式: "yyyy/MM/dd HH:mm" ※日付必須</summary>
        [DataMember]
        public string BedTime { get; set; }

        /// <summary>
        /// 起床時刻（JST）。形式: "yyyy/MM/dd HH:mm" ※日付必須。
        /// 日をまたぐ場合は翌日の日付を指定すること。
        /// 例: 23:00就寝→翌07:00起床 → WakeupTime = "2026/03/15 07:00"
        /// </summary>
        [DataMember]
        public string WakeupTime { get; set; }

        /// <summary>
        /// データソース識別子（省略可）。
        /// "Manual" / "AppleHealth" / "GoogleHealth"
        /// </summary>
        [DataMember]
        public string DataSource { get; set; }

        /// <summary>睡眠ステージ一覧（省略可）。将来の詳細分析用。</summary>
        [DataMember]
        public List<QoJotoAppSleepStageValue> SleepStages { get; set; }
    }

    /// <summary>
    /// POST /JotoApp/SleepImport レスポンス
    /// </summary>
    [DataContract]
    [Serializable]
    public class QoJotoAppSleepImportApiResults : QoApiResultsBase
    {
        /// <summary>起床時刻（JST）。形式: "yyyy/MM/dd HH:mm:ss"</summary>
        [DataMember]
        public string WakeupTime { get; set; }

        /// <summary>入眠時刻（JST）。サーバーが計算して返します。形式: "yyyy/MM/dd HH:mm:ss"</summary>
        [DataMember]
        public string BedTime { get; set; }

        /// <summary>睡眠時間（分）。サーバーが計算して返します。</summary>
        [DataMember]
        public string SleepMinutes { get; set; }
    }

    /// <summary>
    /// 睡眠ステージ（SleepStages 要素）
    /// </summary>
    [DataContract]
    [Serializable]
    public class QoJotoAppSleepStageValue
    {
        /// <summary>ステージ種別。"Light" / "Deep" / "REM" / "Awake" / "Core"</summary>
        [DataMember]
        public string StageType { get; set; }

        /// <summary>開始時刻（JST）。形式: "yyyy/MM/dd HH:mm"</summary>
        [DataMember]
        public string StartTime { get; set; }

        /// <summary>終了時刻（JST）。形式: "yyyy/MM/dd HH:mm"</summary>
        [DataMember]
        public string EndTime { get; set; }
    }
}
