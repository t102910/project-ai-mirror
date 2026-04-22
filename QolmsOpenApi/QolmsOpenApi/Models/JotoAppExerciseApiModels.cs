using MGF.QOLMS.QolmsApiCoreV1;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MGF.QOLMS.QolmsApiEntityV1
{
    // ============================================================
    // 運動マスタ取得 API
    // POST /JotoApp/ExerciseItemList
    // ============================================================

    /// <summary>運動マスタ取得 API 引数</summary>
    [DataContract]
    [Serializable]
    public class QoJotoAppExerciseItemListApiArgs : QoApiArgsBase
    {
        // ActorKey / AuthorKey は QoApiArgsBase に定義済み
        // LinkageSystemNo はサーバー側で 47003 固定のため不要
    }

    /// <summary>運動マスタ取得 API 結果</summary>
    [DataContract]
    [Serializable]
    public class QoJotoAppExerciseItemListApiResults : QoApiResultsBase
    {
        /// <summary>運動マスタ一覧</summary>
        [DataMember(Name = "ExerciseItemN")]
        public List<QoJotoAppExerciseItemValue> ExerciseItemN { get; set; }
            = new List<QoJotoAppExerciseItemValue>();
    }

    /// <summary>運動マスタ 1件分</summary>
    [DataContract]
    [Serializable]
    public class QoJotoAppExerciseItemValue
    {
        /// <summary>運動種別 (1〜254:マスタ, 255:その他)</summary>
        [DataMember(Name = "ExerciseType")]
        public string ExerciseType { get; set; }

        /// <summary>運動名称</summary>
        [DataMember(Name = "ExerciseName")]
        public string ExerciseName { get; set; }

        /// <summary>
        /// 標準消費カロリー。
        /// ExerciseType=255（その他）の場合は 0 を返す。
        /// </summary>
        [DataMember(Name = "Calorie")]
        public string Calorie { get; set; }

        /// <summary>外部キー</summary>
        [DataMember(Name = "ForeignKey")]
        public string ForeignKey { get; set; }
    }

    // ============================================================
    // 運動データ登録 API
    // POST /JotoApp/ExerciseImport
    // ============================================================

    /// <summary>運動データ登録 API 引数</summary>
    [DataContract]
    [Serializable]
    public class QoJotoAppExerciseImportApiArgs : QoApiArgsBase
    {
        /// <summary>リンケージシステム番号（47003 を期待）</summary>
        [DataMember(Name = "LinkageSystemNo")]
        public string LinkageSystemNo { get; set; }

        /// <summary>登録する運動データのリスト</summary>
        [DataMember(Name = "ExerciseValueN")]
        public List<QoJotoAppExerciseImportValue> ExerciseValueN { get; set; }
            = new List<QoJotoAppExerciseImportValue>();
    }

    /// <summary>運動データ登録 API 結果</summary>
    [DataContract]
    [Serializable]
    public class QoJotoAppExerciseImportApiResults : QoApiResultsBase
    {
        // IsSuccess / Result は QoApiResultsBase に定義済み
    }

    /// <summary>運動データ登録 1件分</summary>
    [DataContract]
    [Serializable]
    public class QoJotoAppExerciseImportValue
    {
        /// <summary>記録日時 (yyyy-MM-dd HH:mm:ss)</summary>
        [DataMember(Name = "RecordDate")]
        public string RecordDate { get; set; }

        /// <summary>
        /// 運動種別 (1〜254 or 255)。
        /// 255 = その他（直接カロリー入力）。0 は不正。
        /// </summary>
        [DataMember(Name = "ExerciseType")]
        public string ExerciseType { get; set; }

        /// <summary>開始日時 (yyyy-MM-dd HH:mm:ss)</summary>
        [DataMember(Name = "StartDate")]
        public string StartDate { get; set; }

        /// <summary>終了日時 (yyyy-MM-dd HH:mm:ss)</summary>
        [DataMember(Name = "EndDate")]
        public string EndDate { get; set; }

        /// <summary>運動名称（省略時はマスタから補完）</summary>
        [DataMember(Name = "ItemName")]
        public string ItemName { get; set; }

        /// <summary>
        /// 消費カロリー (1〜9999)。
        /// ExerciseType=255 の場合：ユーザー直接入力値をそのまま使用。
        /// ExerciseType=1〜254 の場合：クライアント指定値を使用（マスタ値は参考）。
        /// </summary>
        [DataMember(Name = "Calorie")]
        public string Calorie { get; set; }

        /// <summary>外部キー（省略時はマスタから補完）</summary>
        [DataMember(Name = "ForeignKey")]
        public string ForeignKey { get; set; }

        /// <summary>補足値（任意）</summary>
        [DataMember(Name = "Value")]
        public string Value { get; set; }
    }
}
