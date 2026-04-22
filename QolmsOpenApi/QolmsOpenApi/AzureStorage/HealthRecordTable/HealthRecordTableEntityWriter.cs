using System.Collections.Generic;
using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
    /// <summary>
    /// バイタル情報 格納 テーブル ストレージ へ値を登録するための機能を提供します。
    /// この クラス は継承できません。
    /// </summary>
    /// <typeparam name="TTableEntity">バイタル情報 格納 テーブル ストレージ エンティティ の型。</typeparam>
    internal sealed class HealthRecordTableEntityWriter<TTableEntity>
        : QsAzureTableStorageWriterBase<TTableEntity>,
        IQsAzureTableStorageWriter<TTableEntity, HealthRecordTableEntityWriterArgs<TTableEntity>, HealthRecordTableEntityWriterResults>
        where TTableEntity : QsHealthRecordTableEntityBase, new()
    {
        #region "Constructor"

        /// <summary>
        /// <see cref="HealthRecordTableEntityWriter&lt;TTableEntity&gt;" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        public HealthRecordTableEntityWriter() : base(true) { }

        #endregion

        #region "IQsAzureTableStorageWriter Support"

        /// <summary>
        /// Azure テーブル ストレージ へ値を設定します。
        /// </summary>
        /// <param name="args">引数 クラス。</param>
        /// <returns>
        /// 戻り値 クラス。
        /// </returns>
        HealthRecordTableEntityWriterResults IQsAzureTableStorageWriter<TTableEntity, HealthRecordTableEntityWriterArgs<TTableEntity>, HealthRecordTableEntityWriterResults>.Execute(HealthRecordTableEntityWriterArgs<TTableEntity> args)
        {
            var result = new HealthRecordTableEntityWriterResults() { IsSuccess = false };

            result.IsSuccess = this.InsertOrUpdateEntities(args.Entities).Count == args.Entities.Count;
            result.Result = result.IsSuccess ? args.Entities.Count : 0;

            return result;
        }

        #endregion
    }
}
