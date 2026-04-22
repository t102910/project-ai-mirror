using System.Collections.Generic;
using MGF.QOLMS.QolmsAzureStorageCoreV1;
using Microsoft.WindowsAzure.Storage.Table;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
    /// <summary>
    /// バイタル情報 格納 テーブル ストレージ から値を取得するための機能を提供します。
    /// この クラス は継承できません。
    /// </summary>
    /// <typeparam name="TTableEntity">バイタル情報 格納 テーブル ストレージ エンティティ の型。</typeparam>
    internal sealed class HealthRecordTableEntityReader<TTableEntity>
        : QsAzureTableStorageReaderBase<TTableEntity>,
        IQsAzureTableStorageReader<TTableEntity, HealthRecordTableEntityReaderArgs<TTableEntity>, HealthRecordTableEntityReaderResults<TTableEntity>>
        where TTableEntity : QsHealthRecordTableEntityBase, new()
    {
        #region "Constructor"

        /// <summary>
        /// <see cref="HealthRecordTableEntityReader&lt;TTableEntity&gt;" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        public HealthRecordTableEntityReader() : base(true) { }

        #endregion

        #region "IQsAzureTableStorageReader Support"

        /// <summary>
        /// Azure テーブル ストレージ から値を取得します。
        /// </summary>
        /// <param name="args">引数 クラス。</param>
        /// <returns>戻り値 クラス。</returns>
        HealthRecordTableEntityReaderResults<TTableEntity> IQsAzureTableStorageReader<TTableEntity, HealthRecordTableEntityReaderArgs<TTableEntity>, HealthRecordTableEntityReaderResults<TTableEntity>>.Execute(HealthRecordTableEntityReaderArgs<TTableEntity> args)
        {
            var result = new HealthRecordTableEntityReaderResults<TTableEntity>() { IsSuccess = false };

            // PartitionKey = AccountKey (32桁の16進数文字列)
            var partitionKey = args.AccountKey.ToString("N");
            
            // RowKey = RecordDate (yyyyMMddHHmmssfffffff形式)
            var fromRowKey = args.FromDate.ToString("yyyyMMddHHmmssfffffff");
            var toRowKey = args.ToDate.ToString("yyyyMMddHHmmssfffffff");

            // 範囲クエリ作成
            var pkFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey);
            var fromFilter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, fromRowKey);
            var toFilter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThanOrEqual, toRowKey);
            
            var combinedFilter = TableQuery.CombineFilters(
                TableQuery.CombineFilters(pkFilter, TableOperators.And, fromFilter),
                TableOperators.And,
                toFilter
            );

            var query = new TableQuery<TTableEntity>().Where(combinedFilter);

            // データ取得
            result.Result = this.SelectByQuery(query);
            result.IsSuccess = true;

            return result;
        }

        #endregion
    }
}
