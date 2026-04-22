using System;
using MGF.QOLMS.QolmsAzureStorageCoreV1;

namespace MGF.QOLMS.QolmsOpenApi.AzureStorage
{
    /// <summary>
    /// <see cref="HealthRecordTableEntityReader&lt;TTableEntity&gt;" /> の引数 クラス。
    /// </summary>
    /// <typeparam name="TTableEntity">バイタル情報 格納 テーブル ストレージ エンティティ の型。</typeparam>
    internal sealed class HealthRecordTableEntityReaderArgs<TTableEntity>
        : QsAzureTableStorageReaderArgsBase<TTableEntity>
        where TTableEntity : QsHealthRecordTableEntityBase
    {
        #region "Public Property"

        /// <summary>
        /// アカウントキー
        /// </summary>
        public Guid AccountKey { get; set; }

        /// <summary>
        /// 取得開始日時
        /// </summary>
        public DateTime FromDate { get; set; }

        /// <summary>
        /// 取得終了日時
        /// </summary>
        public DateTime ToDate { get; set; }

        #endregion

        #region "Constructor"

        /// <summary>
        /// <see cref="HealthRecordTableEntityReaderArgs&lt;TTableEntity&gt;" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        public HealthRecordTableEntityReaderArgs() : base() { }

        #endregion
    }
}
