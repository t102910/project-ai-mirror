using System;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Sql.Core;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
 
    /// <summary>
    /// アップロード ファイル情報を、
    /// データベース テーブルへ登録するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class FileStorageWriter<TEntity> : QsDbWriterBase, IQsDbDistributedWriter<MGF_NULL_ENTITY, FileStorageWriterArgs, FileStorageWriterResults> where TEntity : QsUploadFileDataEntityBase
    {
        /// <summary>
        /// <see cref="FileStorageWriter &lt; TEntity &gt;" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public FileStorageWriter() : base()
        {
        }



        /// <summary>
        /// 分散トランザクションを使用してデータベース テーブルへ値を設定します。
        /// </summary>
        /// <param name="args">DB 引数クラス。</param>
        /// <returns>
        /// DB 戻り値クラス。
        /// </returns>
        /// <remarks></remarks>
        public FileStorageWriterResults ExecuteByDistributed(FileStorageWriterArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args", "DB引数クラスがNull参照です。");

            FileStorageWriterResults result = new FileStorageWriterResults()
            {
                Result = 0,
                IsSuccess = false
            };
            DbFileStorageWriterCore<TEntity> fileStorageWriter = new DbFileStorageWriterCore<TEntity>(args.AuthorKey, args.ActorKey);

            if (fileStorageWriter.WriteUploadFileEntity(args.FileKey, args.OriginalName, args.ContentType))
            {
                // 処理件数
                result.Result = 1;

                // 成功
                result.IsSuccess = true;
            }
            

            return result;
        }
    }


}