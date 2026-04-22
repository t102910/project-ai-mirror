using System;
using System.Collections.Generic;
using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
    /// <summary>
    /// バイタル情報 格納 テーブル ストレージ へ値を登録するための情報を格納する引数 クラス を表します。
    /// この クラス は継承できません。
    /// </summary>
    /// <typeparam name="TTableEntity">アクセス ログ テーブル ストレージ エンティティ の型。</typeparam>
    internal sealed class HealthRecordTableEntityWriterArgs<TTableEntity>
        : QsAzureTableStorageWriterArgsBase<TTableEntity>
        where TTableEntity : QsHealthRecordTableEntityBase
    {
        #region "Public Property"

        #endregion

        #region "Constructor"

        /// <summary>
        /// <see cref="HealthRecordTableEntityWriterArgs&lt;TTableEntity&gt;" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        public HealthRecordTableEntityWriterArgs() : base() { }

        #endregion
    }
}
