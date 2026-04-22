using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
    /// <summary>
    /// <see cref="HealthRecordTableEntityReader&lt;TTableEntity&gt;" /> の戻り値 クラス。
    /// </summary>
    /// <typeparam name="TTableEntity">バイタル情報 格納 テーブル ストレージ エンティティ の型。</typeparam>
    internal sealed class HealthRecordTableEntityReaderResults<TTableEntity>
        : QsAzureTableStorageReaderResultsBase<TTableEntity>
        where TTableEntity : QsHealthRecordTableEntityBase
    {
        #region "Constructor"

        /// <summary>
        /// <see cref="HealthRecordTableEntityReaderResults&lt;TTableEntity&gt;" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        public HealthRecordTableEntityReaderResults() : base() { }

        #endregion
    }
}
