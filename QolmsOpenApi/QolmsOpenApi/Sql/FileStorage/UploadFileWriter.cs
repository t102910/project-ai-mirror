using System;
using System.Collections.Generic;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
 
    /// <summary>
    /// アップロード ファイル情報を、
    /// データベース テーブルへ登録するための機能を提供します。
    /// このクラスは継承できません。
    /// </summary>
    /// <remarks></remarks>
    internal sealed class UploadFileWriter : QsDbWriterBase, IQsDbDistributedWriter<MGF_NULL_ENTITY, UploadFileWriterArgs, UploadFileWriterResults> 
    {
        /// <summary>
        /// <see cref="UploadFileWriter &lt; TEntity &gt;" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <remarks></remarks>
        public UploadFileWriter() : base()
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
        public UploadFileWriterResults ExecuteByDistributed(UploadFileWriterArgs args)
        {            
            UploadFileWriterResults result = new UploadFileWriterResults()
            {
                Result = 0,
                IsSuccess = false
            };

            DateTime now = DateTime.Now;
            if (args.Entity.DataState == QsDbEntityStateTypeEnum.Added)
            {
                args.Entity.CREATEDDATE = now;
            }
            if(args.Entity.DataState == QsDbEntityStateTypeEnum.Deleted)
            {
                args.Entity.DELETEFLAG = true;
            }
            args.Entity.UPDATEDDATE = now;
            // DB へ登録
            QhUploadFileEntityWriter writer = new QhUploadFileEntityWriter();
            var writerArgs = new QhUploadFileEntityWriterArgs()
            {
                Data = new List<QH_UPLOADFILE_DAT>{ args.Entity }
            };

            QhUploadFileEntityWriterResults writerResults = QsDbManager.WriteByCurrent(writer, writerArgs);

            if (writerResults.IsSuccess && writerResults.Result == 1)
            {
                // 処理件数
                result.Result = 1;
                // 成功
                result.IsSuccess = true;
            }
            else
            {
                throw new InvalidOperationException("QH_UPLOADFILE_DATテーブルへの登録に失敗しました。");
            }              
            
            return result;
        }
    }


}