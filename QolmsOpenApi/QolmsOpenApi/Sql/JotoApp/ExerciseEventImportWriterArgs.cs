using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsDbLibraryV1;
using System;
using System.Collections.Generic;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// JOTOアプリからの運動データ登録（新方式）のための引数クラス。
    /// 旧 ExerciseEventWriterArgs とは別クラスとして並列実装する。
    /// </summary>
    public class ExerciseEventImportWriterArgs : QsDbWriterArgsBase<MGF_NULL_ENTITY>
    {
        /// <summary>所有者アカウントキー</summary>
        public Guid AuthorKey { get; set; } = Guid.Empty;

        /// <summary>対象者アカウントキー</summary>
        public Guid ActorKey { get; set; } = Guid.Empty;

        /// <summary>登録する運動データのリスト</summary>
        public List<ExerciseEventImportItem> ExerciseEventN { get; set; } = new List<ExerciseEventImportItem>();
    }

    /// <summary>
    /// 運動データ1件分の情報。
    /// </summary>
    public class ExerciseEventImportItem
    {
        /// <summary>記録日時</summary>
        public DateTime RecordDate { get; set; }

        /// <summary>
        /// 運動種別 (1〜254:マスタ定義の運動種別, 255:その他＝直接カロリー入力)
        /// ※0 は不正値。
        /// </summary>
        public byte ExerciseType { get; set; }

        /// <summary>開始日時</summary>
        public DateTime StartDate { get; set; }

        /// <summary>終了日時</summary>
        public DateTime EndDate { get; set; }

        /// <summary>運動名称（マスタ名 or クライアント指定）</summary>
        public string ItemName { get; set; } = string.Empty;

        /// <summary>
        /// 消費カロリー (1〜9999)。
        /// ExerciseType=255 の場合はユーザー直接入力値をそのまま使用。
        /// ExerciseType=1〜254 の場合もクライアント指定値を使用（マスタ値は参考）。
        /// </summary>
        public short Calorie { get; set; }

        /// <summary>外部キー（マスタ参照値 or 空）</summary>
        public string ForeignKey { get; set; } = string.Empty;

        /// <summary>補足値（任意）</summary>
        public string Value { get; set; } = string.Empty;
    }
}
