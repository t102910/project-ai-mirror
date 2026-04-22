using MGF.QOLMS.QolmsDbCoreV1;
using System;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 運動データ登録（新方式）の結果クラス。
    /// </summary>
    public class ExerciseEventImportWriterResults : QsDbWriterResultsBase
    {
        /// <summary>処理日時</summary>
        public DateTime ActionDate { get; set; }

        /// <summary>処理キー</summary>
        public Guid ActionKey { get; set; }
    }
}
