using MGF.QOLMS.QolmsAzureStorageCoreV1;
using System.Collections.Generic;

namespace MGF.QOLMS.QolmsJotoWebView 
{ 
    /// <summary>
    /// アクセス ログ テーブル ストレージ へ値を登録するための機能を提供します。
    /// この クラス は継承できません。
    /// </summary>
    /// <typeparam name="TTableEntity">アクセス ログ テーブル ストレージ エンティティ の型。</typeparam>
    internal sealed class AccessTableEntityWriter<TTableEntity>
        : QsAzureTableStorageWriterBase<TTableEntity>, IQsAzureTableStorageWriter<TTableEntity, AccessTableEntityWriterArgs<TTableEntity>, AccessTableEntityWriterResults> where TTableEntity : QsAccessTableEntityBase, new()
    {

        #region "Constructor"

        /// <summary>
        /// <see cref="AccessTableEntityWriter" /> クラス の新しい インスタンス を初期化します。
        /// </summary>
        public AccessTableEntityWriter() : base(true) { }

        #endregion

        #region "IQsAzureTableStorageWriter Support"

        /// <summary>
        /// Azure テーブル ストレージ へ値を設定します。
        /// </summary>
        /// <param name="args">引数 クラス。</param>
        /// <returns>
        /// 戻り値 クラス。
        /// </returns>
        AccessTableEntityWriterResults IQsAzureTableStorageWriter<TTableEntity, AccessTableEntityWriterArgs<TTableEntity>, AccessTableEntityWriterResults>.Execute(AccessTableEntityWriterArgs<TTableEntity> args) 
        {
            var result = new AccessTableEntityWriterResults() { IsSuccess = false };
            var entity = new TTableEntity();

            entity.SetPartitionKey(args.AccessDate);
            entity.SetRowKey(args.AccessDate, args.AccountKey);
            entity.AccessType = args.AccessType;
            entity.AccessUri = args.AccessUri;
            entity.Comment = args.Comment;
            entity.UserHostAddress = args.UserHostAddress;
            entity.UserHostName = args.UserHostName;
            entity.UserAgent = args.UserAgent;

            result.IsSuccess = this.InsertOrUpdateEntities(new List<TTableEntity>() { entity }).Count == 1;
            result.Result = result.IsSuccess ? 1 : 0;

            return result;
        }

        #endregion

    }
}