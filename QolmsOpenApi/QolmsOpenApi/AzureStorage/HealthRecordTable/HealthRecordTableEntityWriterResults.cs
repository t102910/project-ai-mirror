using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
    /// <summary>
    /// バイタル情報 格納 テーブル ストレージ へ値を登録した結果を格納する戻り値 クラス を表します。
    /// この クラス は継承できません。
    /// </summary>
    internal sealed class HealthRecordTableEntityWriterResults
        : QsAzureTableStorageWriterResultsBase
    {
        #region "Constructor"

        /// <summary>
        /// <see cref="HealthRecordTableEntityWriterResults" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        public HealthRecordTableEntityWriterResults() : base() { }

        #endregion
    }
}
