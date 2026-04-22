using System;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Sql.Core;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
 
    /// <summary>
    /// アップロード ファイル情報を、
    /// データベース テーブルから取得するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class FileStorageReader<TEntity> : QsDbReaderBase, IQsDbDistributedReader<MGF_NULL_ENTITY, FileStorageReaderArgs, FileStorageReaderResults> where TEntity : QsUploadFileDataEntityBase
    {

        /// <summary>
        /// クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public FileStorageReader() : base()
        {
        }

        /// <summary>
        /// 分散トランザクションを使用してデータベース テーブルから値を取得します。
        /// </summary>
        /// <param name="args">DB 引数クラス。</param>
        /// <returns>
        /// DB 戻り値クラス。
        /// </returns>
        /// <remarks></remarks>
        public FileStorageReaderResults ExecuteByDistributed(FileStorageReaderArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");

            FileStorageReaderResults result = new FileStorageReaderResults() { IsSuccess = false };

            // ゲスト処方せん時は対象者アカウントキーがGuid.Emptyとなるため、AutorKeyを引数に設定する
            DbFileStorageReaderCore<TEntity> fileStorageReader = new DbFileStorageReaderCore<TEntity>(args.AuthorKey, args.AuthorKey);

           
            // ファイル情報
            result.FileStorageItem = fileStorageReader.GetFileStorageItem(args.FileKey);

            // 処理結果
            result.IsSuccess = result.FileStorageItem.AccountKey != Guid.Empty && result.FileStorageItem.FileKey != Guid.Empty;
            

            return result;
        }
    }


}